namespace FinalMix.Skills;

internal interface IHaveSharedInfo
{
    public bool HasSharedInfoForLevel(int level);

    public string GetSharedInfo(int level);
}
