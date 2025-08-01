using StardewModdingAPI.Events;
using StardewValley;

namespace FinalMix.Util
{
    internal class AssetManager<T>
    {
        private Dictionary<string, T>? _Asset = null;
        /// <summary>
        /// The Asset for this feature.
        /// </summary>
        public Dictionary<string, T> Asset { get => _Asset ??= Game1.content.Load<Dictionary<string, T>>(AssetName); }

        private string AssetName { get; }
        private string Name { get; }

        /// <summary>
        /// If this feature should be enabled
        /// (i.e. there's any data to work with).
        /// </summary>
        public bool IsActive { get => Asset.Count > 0; }

        /// <summary>
        /// Create an asset manager for a feature with the data model T.
        /// </summary>
        /// <param name="assetName">The Asset Name for the feature, prefixed in the content pipeline with the Mod ID.</param>
        public AssetManager(string name)
        {
            Name = name;
            AssetName = FinalMix.Instance.ModManifest.UniqueID + "/" + name;
            FinalMix.Helper.Events.Content.AssetRequested += AssetRequested;
            FinalMix.Helper.Events.Content.AssetsInvalidated += AssetsInvalidated;
        }

        /// <summary>
        /// Try get the data associated with the key.
        /// </summary>
        /// <param name="Key">The Key of the <c>Data</c> to get.</param>
        /// <param name="Data">The Data associated with the <c>Key</c>, if any.</param>
        /// <returns>Returns <c>False</c> if <see cref="IsActive"/> is false or the key isn't found. Otherwise returns <c>True</c></returns>
        public bool TryGetData(string Key, out T Data)
        {
            Data = default!;
            if (!IsActive)
            {
                FinalMix.Log.Error($"GetData called but asset {Name} is not active.");
                return false;
            }
            else if (!Contains(Key))
                return false;
            else
            {
                Data = Asset[Key];
                return true;
            }
        }
        /// <summary>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.ContainsKey(TKey)"/>
        /// </summary>
        /// <param name="key"><inheritdoc cref="Dictionary{TKey, TValue}.ContainsKey(TKey)"/></param>
        /// <returns><inheritdoc cref="Dictionary{TKey, TValue}.ContainsKey(TKey)"/></returns>
        public bool Contains(string key) => Asset.ContainsKey(key);

        private void AssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(AssetName))
                e.LoadFrom(() => new Dictionary<string, T>(), AssetLoadPriority.Exclusive);
        }

        private void AssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        {
            foreach(var name in e.NamesWithoutLocale)
                if (name.IsEquivalentTo(AssetName)) _Asset = null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
