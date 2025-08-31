namespace FinalMix.Skills.Professions;

internal class GenericProfession(Skill skill, string id, Func<string> name, Func<string> description) : Profession(skill, id)
{
    private Func<string> Name { get; } = name;
    private Func<string> Description { get; } = description;

    public override string GetName()
    {
        return Name();
    }

    public override string GetDescription()
    {
        return Description();
    }
}