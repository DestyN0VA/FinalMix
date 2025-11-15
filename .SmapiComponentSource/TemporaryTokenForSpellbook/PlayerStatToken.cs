using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using xTile;

namespace FinalMix.TemporaryTokenForSpellbook;

internal class PlayerStatToken : AdvancedToken
{
    public override bool AllowsInput()
    {
        return true;
    }

    public override bool RequiresInput()
    {
        return true;
    }

    public override IEnumerable<string> GetValues(string input)
    {
        string[] inputs = input.Split('|');

        string Key = inputs[0].Trim();
        uint Stat = Game1.stats.Get(Key);

        for (int i = 1; i < inputs.Length; i++)
        {
            if (inputs[i].Trim().StartsWithIgnoreCase("Min"))
            {
                if (IsUintArg(inputs[i], out uint Min) && Stat < Min)
                    return ["false"];
            }
            else if (inputs[i].Trim().StartsWithIgnoreCase("Max"))
            {
                if (IsUintArg(inputs[i], out uint Max) && Stat > Max)
                    return ["false"];
            }
        }

        return ["true"];
    }

    public override bool TryValidateInput(string? input, out string error)
    {
        string[] inputs = input?.Split('|') ?? [];
        error = "";

        if (inputs.Length == 0 || string.IsNullOrEmpty(inputs[0].Trim()))
        {
            error = "Required input token (Stat Key) not provided.";
            return false;
        }

        if (inputs.Length == 1)
        {
            error = "Required Min or Max input arguments not provided.";
            return false;
        }
        else
        {
            inputs = inputs[1..];

            bool foundMinMax = false;
            uint? min = null, max = null;

            foreach (string arg in inputs)
            {
                if (arg.Trim().StartsWithIgnoreCase("Min"))
                {
                    if (!IsUintArg(arg, out uint Min))
                    {
                        error = $"Input '{arg}' could not be parsed as an unsigned integer.";
                        return false;
                    }
                    else
                    {
                        min = Min;
                        foundMinMax = true;
                    }
                }
                else if (arg.Trim().StartsWithIgnoreCase("Max"))
                {
                    if (!IsUintArg(arg, out uint Max))
                    {
                        error = $"Input '{arg}' could not be parsed as an unsigned integer.";
                        return false;
                    }
                    else
                    {
                        max = Max;
                        foundMinMax = true;
                    }
                }
            }

            if (max.HasValue && min.HasValue && max < min)
            {
                error = "Max value is less than Min value, which is not allowed.";
                return false;
            }

            error = "Required Min or Max input arguments not provided.";
            return foundMinMax;
        }
    }

    private static bool IsUintArg(string arg, out uint value)
    {
        value = 0;
        string[] split = arg.Split('=');
        if (split.Length != 2)
            return false;
        if (!uint.TryParse(split[1], out value))
            return false;

        return true;
    }

    protected override bool DidDataChange()
    {
        return true;
    }
}
