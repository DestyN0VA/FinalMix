using StardewValley.Mods;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;

namespace FinalMix.Enhancements.Data;

internal record MateriaData(MateriaType materiaType, int Rank)
{
    public const string EffectKey = "DN.SnS_MateriaEffect";
    public MateriaType Type { get; set; } = materiaType;
    public int Rank { get; set; } = Rank;


    public List<MateriaData> GetFromModData(ModDataDictionary modData)
    {
        if (!modData.TryGetValue(EffectKey, out string Effects))
            return [new(MateriaType.Unforged, -1)];

        List<MateriaData> ret = [];
        foreach (var Effect in Effects.Split())
            if (TryParseEffect(Effect, out var data))
                ret.Add(data);
        return ret;

    }

    private bool TryParseEffect(string Effect, [NotNullWhen(true)] out MateriaData? Materia)
    {
        Materia = null;

        if (!string.IsNullOrEmpty(Effect))
            return false;
        else if (Enum.TryParse<MateriaType>(Effect[..Effect.Length-1], out MateriaType Type))

            return false;


        //return true;
    }
}


public enum MateriaType
{
    Unforged,
    Barrier,
    Restore,
    Haste,
    Lightning,
    Fire,
    Ice
}
