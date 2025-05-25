using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceCore.UI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Tools;
using SwordAndSorcerySMAPI.ModSkills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Object = StardewValley.Object;

namespace SwordAndSorcerySMAPI;

public static class ArsenalExtensions
{
    public static string GetExquisiteGemstone(this MeleeWeapon weapon)
    {
        if (weapon.modData.TryGetValue(ModSnS.DataKey_ExquisiteGemstone, out string id))
            return id;
        return null;
    }
    public static void SetExquisiteGemstone(this MeleeWeapon weapon, string exquisiteGemstoneId)
    {
        if (weapon.modData.ContainsKey(ModSnS.DataKey_ExquisiteGemstone))
            weapon.modData.Remove(ModSnS.DataKey_ExquisiteGemstone);
        weapon.modData.Add(ModSnS.DataKey_ExquisiteGemstone, exquisiteGemstoneId);
    }
    public static string GetBladeCoating(this MeleeWeapon weapon)
    {
        if (weapon.modData.TryGetValue(ModSnS.DataKey_BladeCoating, out string id))
            return id;
        return null;
    }
    public static void SetBladeCoating(this MeleeWeapon weapon, string coatingId)
    {
        if (weapon.modData.ContainsKey(ModSnS.DataKey_BladeCoating))
            weapon.modData.Remove(ModSnS.DataKey_BladeCoating);
        weapon.modData.Add(ModSnS.DataKey_BladeCoating, coatingId);
    }
    public static string GetBladeAlloying(this MeleeWeapon weapon)
    {
        if (weapon.modData.TryGetValue(ModSnS.DataKey_BladeAlloying, out string id))
            return id;
        return null;
    }
    public static void SetBladeAlloying(this MeleeWeapon weapon, string alloyId)
    {
        if (weapon.modData.ContainsKey(ModSnS.DataKey_BladeAlloying))
            weapon.modData.Remove(ModSnS.DataKey_BladeAlloying);
        weapon.modData.Add(ModSnS.DataKey_BladeAlloying, alloyId);
    }

    public static string GetExquisiteGemstone(this Slingshot weapon)
    {
        if (weapon.modData.TryGetValue(ModSnS.DataKey_ExquisiteGemstone, out string id))
            return id;
        return null;
    }
    public static void SetExquisiteGemstone(this Slingshot weapon, string exquisiteGemstoneId)
    {
        if (weapon.modData.ContainsKey(ModSnS.DataKey_ExquisiteGemstone))
            weapon.modData.Remove(ModSnS.DataKey_ExquisiteGemstone);
        weapon.modData.Add(ModSnS.DataKey_ExquisiteGemstone, exquisiteGemstoneId);
    }
    public static string GetBladeCoating(this Slingshot weapon)
    {
        if (weapon.modData.TryGetValue(ModSnS.DataKey_BladeCoating, out string id))
            return id;
        return null;
    }
    public static void SetBladeCoating(this Slingshot weapon, string coatingId)
    {
        if (weapon.modData.ContainsKey(ModSnS.DataKey_BladeCoating))
            weapon.modData.Remove(ModSnS.DataKey_BladeCoating);
        weapon.modData.Add(ModSnS.DataKey_BladeCoating, coatingId);
    }
    public static string GetBladeAlloying(this Slingshot weapon)
    {
        if (weapon.modData.TryGetValue(ModSnS.DataKey_BladeAlloying, out string id))
            return id;
        return null;
    }
    public static void SetBladeAlloying(this Slingshot weapon, string alloyId)
    {
        if (weapon.modData.ContainsKey(ModSnS.DataKey_BladeAlloying))
            weapon.modData.Remove(ModSnS.DataKey_BladeAlloying);
        weapon.modData.Add(ModSnS.DataKey_BladeAlloying, alloyId);
    }
}


public class SecondEnchantmentForgeRecipe : CustomForgeRecipe
{
    private class GenericIngredientMatcher(string qualId, int qty) : CustomForgeRecipe.IngredientMatcher
    {
        public string QualifiedId { get; } = qualId;
        public int Quantity { get; } = qty;

        public override bool HasEnoughFor(Item item)
        {
            return item.QualifiedItemId == QualifiedId && item.Stack >= Quantity;
        }

        public override void Consume(ref Item item)
        {
            if (Quantity >= item.Stack)
                item = null;
            else
                item.Stack -= Quantity;
        }
    }
    private class ToolIngredientMatcher : CustomForgeRecipe.IngredientMatcher
    {
        public override bool HasEnoughFor(Item item)
        {
            if (item is Tool t)
            {
                var enchs = BaseEnchantment.GetAvailableEnchantmentsForItem(t);
                return enchs.Count > 0;
            }

            return false;
        }

        public override void Consume(ref Item item)
        {
            item = null;
        }
    }

    public override IngredientMatcher BaseItem { get; } = new ToolIngredientMatcher();
    public override IngredientMatcher IngredientItem { get; } = new GenericIngredientMatcher("(O)DN.SnS_ArcanePrimer", 1);
    public override int CinderShardCost { get; } = 30;
    public override Item CreateResult(Item baseItem, Item ingredItem)
    {
        var t = baseItem.getOne() as Tool;

        var newEnchs = BaseEnchantment.GetAvailableEnchantmentsForItem(t);
        if (newEnchs.Count > 0)
        {
            var oldEnchs = t.enchantments.ToList();
            var enchs = t.enchantments.ToList();
            enchs.RemoveAll(e => e.IsSecondaryEnchantment());

            if (enchs.Count != 1)
            {
                t.enchantments.Remove(enchs[1]);
            }

            t.enchantments.Add(Game1.random.ChooseFrom(newEnchs));

            t.previousEnchantments.Clear();
            t.previousEnchantments.AddRange(oldEnchs.Select(e => e.GetName()));
        }

        return t;
    }
}

public partial class ModSnS
{
    public const string DataKey_ExquisiteGemstone = "swordandsorcery/ExquisiteGemstone";
    public const string DataKey_BladeCoating = "swordandsorcery/BladeCoating";
    public const string DataKey_BladeAlloying = "swordandsorcery/BladeAlloying";

    public const string SpriteSheetPath = "Textures/DN.SnS/SnSArsenal";

    public readonly static Dictionary<string, string> ExquisiteGemMappings = new()
    {
        { Object.emeraldQID, "(O)DN.SnS_ExquisiteEmerald" },
        { Object.rubyQID, "(O)DN.SnS_ExquisiteRuby" },
        { Object.topazQID, "(O)DN.SnS_ExquisiteTopaz" },
        { Object.aquamarineQID, "(O)DN.SnS_ExquisiteAquamarine" },
        { Object.amethystClusterQID, "(O)DN.SnS_ExquisiteAmethyst" },
        { Object.sapphireQID /* WHAT? */, "(O)DN.SnS_ExquisiteJade" },
        { Object.diamondQID, "(O)DN.SnS_ExquisiteDiamond" },
    };

    public readonly static Dictionary<string, string> PureOreMappings = new()
    {
        { Object.copperQID, "(O)DN.SnS_PureCopperOre" },
        { Object.ironQID, "(O)DN.SnS_PureIronOre" },
        { Object.goldQID, "(O)DN.SnS_PureGoldOre" },
        { Object.iridiumQID, "(O)DN.SnS_PureIridiumOre" },
        { "(O)909", "(O)DN.SnS_PureRadioactiveOre" },
    };

    public readonly static Dictionary<string, int> CoatingIconMapping = new()
    {
        { "(O)766", 13 },
        { "(O)769", 14 },
        { "(O)768", 15 },
        { "(O)684", 16 },
        { "(O)767", 17 },
    };
    public readonly static Dictionary<string, int> AlloyIconMapping = new()
    {
        { "(O)DN.SnS_PureCopperOre",  8 },
        { "(O)DN.SnS_PureIronOre",  9 },
        { "(O)DN.SnS_PureGoldOre", 10 },
        { "(O)DN.SnS_PureIridiumOre", 11 },
        { "(O)DN.SnS_PureRadioactiveOre", 12 },
    };

    public readonly static Dictionary<string, int> CoatingQuantities = new()
    {
        { "(O)766", 999 },
        { "(O)769", 300 },
        { "(O)768", 300 },
        { "(O)684", 500 },
        { "(O)767", 500 },
    };

    public void InitArsenal()
    {
        Helper.Events.Player.Warped += PlayerOnWarped;

        Helper.ConsoleCommands.Add("sns_encrustweapon", "...", (cmd, args) => (Game1.player.CurrentTool as MeleeWeapon)?.SetExquisiteGemstone(args[0]));
        Helper.ConsoleCommands.Add("sns_coatweapon", "...", (cmd, args) => (Game1.player.CurrentTool as MeleeWeapon)?.SetBladeCoating(args[0]));
        Helper.ConsoleCommands.Add("sns_alloyweapon", "...", (cmd, args) => (Game1.player.CurrentTool as MeleeWeapon)?.SetBladeAlloying(args[0]));
        Helper.ConsoleCommands.Add("sns_arsenal", "...", (cmd, args) => Game1.activeClickableMenu = new ArsenalMenu());

        CustomForgeRecipe.Recipes.Add(new SecondEnchantmentForgeRecipe());

        GameLocation.RegisterTileAction("OpenArsenalUI", (loc, args, who, Tile) =>
        {
            Game1.activeClickableMenu = new ArsenalMenu();
            return true;
        });
    }

    private void PlayerOnWarped(object sender, WarpedEventArgs e)
    {
        // Arcane primer recipe is moved to Ch4
        // tile action is moved to Underforge
        /*
        if (e.NewLocation.Name == "Caldera" && !e.Player.craftingRecipes.ContainsKey("DN.SnS_ArcanePrimer"))
        {
            e.Player.craftingRecipes.Add("DN.SnS_ArcanePrimer", 0);
        }
        else if (e.NewLocation.Name == "Blacksmith")
        {
            e.NewLocation.setTileProperty(12, 13, "Buildings", "Action", "OpenArsenalUI");
            e.NewLocation.setTileProperty(13, 13, "Buildings", "Action", "OpenArsenalUI");
        }
        */
    }
}

public class ArsenalMenu : IClickableMenu
{
    private readonly RootElement ui;
    private readonly ItemSlot weaponSlot;
    private readonly ItemWithBorder weaponPreview;
    private readonly ItemSlot gemSlot, coatingSlot, alloyingSlot;
    private readonly Image forgeButton;
    private readonly InventoryMenu invMenu;
    private readonly List<Pixel> pixels = [];
    private float? animStart;
    private bool playedSynthesizeSound = true;

    public ArsenalMenu()
    : base(Game1.uiViewport.Width / 2 - 350 - IClickableMenu.borderWidth, Game1.uiViewport.Height / 2 - 150 - 100 - IClickableMenu.borderWidth, 700 + IClickableMenu.borderWidth * 2, 300 + IClickableMenu.borderWidth * 2)
    {
        invMenu = new(Game1.uiViewport.Width / 2 - 72 * 5 - 36 + 8, yPositionOnScreen + height, true, highlightMethod:
            (item) =>
            {
                return (item == null || item is MeleeWeapon mw || item.HasContextTag("exquisite_gem") ||
                        ModSnS.CoatingIconMapping.ContainsKey(item.QualifiedItemId) ||
                        ModSnS.AlloyIconMapping.ContainsKey(item.QualifiedItemId));
            });
        ui = new RootElement();
        StaticContainer container = new()
        {
            LocalPosition = new(this.xPositionOnScreen, this.yPositionOnScreen),
            Size = new(this.width, height),
        };
        ui.AddChild(container);

        weaponSlot = new ItemSlot()
        {
            LocalPosition = new(100, height / 4 - 96 / 2),
            ItemDisplay = new MeleeWeapon("0"),
            TransparentItemDisplay = true,
        };

        this.weaponSlot.Callback = this.weaponSlot.SecondaryCallback = (elem) =>
        {
            if (Game1.player.CursorSlotItem != null && Game1.player.CursorSlotItem is not MeleeWeapon)
                return;

            if (Game1.player.CursorSlotItem == null && this.weaponSlot.Item == null)
                return;

            (this.weaponSlot.Item, Game1.player.CursorSlotItem) = (Game1.player.CursorSlotItem, this.weaponSlot.Item);

            UpdatePreview();
        };

        weaponPreview = new ItemSlot()
        {
            LocalPosition = new(116, height / 4 * 2 - 64 / 2),
            ItemDisplay = new MeleeWeapon("0"),
            TransparentItemDisplay = true,
            BoxIsThin = true,
        };

        forgeButton = new()
        {
            LocalPosition = new(116, height / 4 * 3 - 64 / 2),
            Texture = Game1.content.Load<Texture2D>("Textures/DN.SnS/ForgeButton"),
            TexturePixelArea = new(0, 0, 16, 16),
            Callback = (elem) =>
            {
                ApplySlots();
            },
            Scale = 4
        };

        container.AddChild(this.weaponSlot);
        container.AddChild(this.weaponPreview);
        container.AddChild(this.forgeButton);

        static void ModifierSlotShenanigans(ItemSlot modifierSlot, Item cursorItem, int reqQty, out Item Held, out Item Slot)
        {
            Item slotItem = modifierSlot.Item;

            Held = cursorItem;
            Slot = slotItem;

            if (cursorItem == null && slotItem == null)
                return;

            if (cursorItem == null)
            {
                Held = slotItem;
                Slot = null;
                Game1.playSound("dwop");
                return;
            }
            else if (slotItem == null)
            {
                int LeftOver = cursorItem.Stack - reqQty;
                Slot = cursorItem;
                Slot.Stack = Math.Min(reqQty, cursorItem.Stack);
                if (LeftOver > 0)
                {
                    Held = cursorItem.getOne();
                    Held.Stack = LeftOver;
                }
                else
                    Held = null;
                Game1.playSound("button1");
                return;
            }
            else if (cursorItem.canStackWith(slotItem) && slotItem.Stack < reqQty)
            {
                int ToTake = reqQty - slotItem.Stack;
                int LeftOver = cursorItem.Stack - ToTake;
                int NewStack = slotItem.Stack + ToTake;

                Slot = slotItem;
                Slot.Stack = NewStack;

                if (LeftOver > 0)
                {
                    Held = cursorItem;
                    Held.Stack = LeftOver;
                }
                else
                    Held = null;
                Game1.playSound("button1");
                return;
            }
            else if (cursorItem.Stack <= reqQty)
            {
                Held = slotItem;
                Slot = cursorItem;
                Game1.playSound("button1");
                return;
            }
        }

        string spacing = "  ";
        string newline = "\n";

        this.gemSlot = new ItemSlot()
        {
            LocalPosition = new(250, height / 4 - 64 / 2),
            BoxIsThin = true,
            ItemDisplay = ItemRegistry.Create("(O)DN.SnS_ExquisiteDiamond"),
            TransparentItemDisplay = true,
            UserData = $"{I18n.Anvil_ValidOptions()}:\n{string.Join(newline, ModSnS.ExquisiteGemMappings.Values.Select(s => (spacing + ItemRegistry.GetData(s).DisplayName)))}"
        };
        this.gemSlot.Callback = this.gemSlot.SecondaryCallback = (elem) =>
        {
            if (Game1.player.GetCustomSkillLevel(ModSnS.RogueSkill) < 4)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_Locked(4)));
                return;
            }
            if (Game1.player.CursorSlotItem != null &&
                !Game1.player.CursorSlotItem.HasContextTag("exquisite_gem"))
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_NotHere()));
                return;
            }
            ModifierSlotShenanigans(gemSlot, Game1.player.CursorSlotItem, 1, out var Held, out var Slot);
            Game1.player.CursorSlotItem = Held;
            gemSlot.Item = Slot;

            UpdatePreview();
        };
        container.AddChild(gemSlot);
        var gemLabel = new SpaceCore.UI.Label()
        {
            LocalPosition = this.gemSlot.LocalPosition + new Vector2(this.gemSlot.Bounds.Width + 16, 0),
            String = Game1.player.GetCustomSkillLevel(ModSnS.RogueSkill) >= 4 ? I18n.Anvil_GemSocket() : I18n.Anvil_Locked(4),
        };
        container.AddChild(gemLabel);

        this.coatingSlot = new ItemSlot()
        {
            LocalPosition = new(250, height / 4 * 2 - 64 / 2),
            BoxIsThin = true,
            ItemDisplay = ItemRegistry.Create("(O)766"),
            TransparentItemDisplay = true,
            UserData = $"{I18n.Anvil_ValidOptions()}:\n{string.Join(newline, ModSnS.CoatingIconMapping.Keys.Select(s => (spacing + ItemRegistry.GetData(s).DisplayName)))}"
        };
        this.coatingSlot.Callback = this.coatingSlot.SecondaryCallback = (elem) =>
        {
            if (Game1.player.GetCustomSkillLevel(ModSnS.RogueSkill) < 6)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_Locked(6)));
                return;
            }

            if (Game1.player.CursorSlotItem != null &&
                !ModSnS.CoatingIconMapping.ContainsKey(Game1.player.CursorSlotItem.QualifiedItemId))
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_NotHere()));
                return;
            }

            ModifierSlotShenanigans(coatingSlot, Game1.player.CursorSlotItem,
                Game1.player.CursorSlotItem != null
                    ? ModSnS.CoatingQuantities[Game1.player.CursorSlotItem.QualifiedItemId]
                    : 1,
                out var Held, out var Slot);
            Game1.player.CursorSlotItem = Held;
            coatingSlot.Item = Slot;

            UpdatePreview();
        };
        container.AddChild(coatingSlot);
        var coatingLabel = new SpaceCore.UI.Label()
        {
            LocalPosition = this.coatingSlot.LocalPosition + new Vector2(this.coatingSlot.Bounds.Width + 16, 0),
            String = Game1.player.GetCustomSkillLevel(ModSnS.RogueSkill) >= 6 ? I18n.Anvil_Coating() : I18n.Anvil_Locked(6),
        };
        container.AddChild(coatingLabel);

        this.alloyingSlot = new ItemSlot()
        {
            LocalPosition = new(250, height / 4 * 3 - 64 / 2),
            BoxIsThin = true,
            ItemDisplay = ItemRegistry.Create("(O)DN.SnS_PureCopperOre"),
            TransparentItemDisplay = true,
            UserData = $"{I18n.Anvil_ValidOptions()}:\n{string.Join(newline, ModSnS.AlloyIconMapping.Keys.Select(s => (spacing + ItemRegistry.GetData(s).DisplayName)))}"
        };
        this.alloyingSlot.Callback = this.alloyingSlot.SecondaryCallback = (elem) =>
        {
            if (Game1.player.GetCustomSkillLevel(ModSnS.RogueSkill) < 2)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_Locked(2)));
                return;
            }

            if (Game1.player.CursorSlotItem != null &&
                !ModSnS.AlloyIconMapping.ContainsKey(Game1.player.CursorSlotItem.QualifiedItemId))
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_NotHere()));
                return;
            }

            ModifierSlotShenanigans(alloyingSlot, Game1.player.CursorSlotItem, 25, out var Held, out var Slot);
            Game1.player.CursorSlotItem = Held;
            alloyingSlot.Item = Slot;

            UpdatePreview();
        };
        container.AddChild(alloyingSlot);
        var alloyingLabel = new SpaceCore.UI.Label()
        {
            LocalPosition = this.alloyingSlot.LocalPosition + new Vector2(this.alloyingSlot.Bounds.Width + 16, 0),
            String = Game1.player.GetCustomSkillLevel( ModSnS.RogueSkill ) >= 2 ? I18n.Anvil_Alloying() : I18n.Anvil_Locked(2),
        };
        container.AddChild(alloyingLabel);
    }

    public override bool overrideSnappyMenuCursorMovementBan()
    {
        return true;
    }

    public override void update(GameTime time)
    {
        base.update(time);
        this.ui.Update();
        this.invMenu.update(time);

        if (animStart != null && pixels.Count == 0)
        {
            animStart = null;
        }
    }

    private void Pixelize(ItemSlot slot)
    {
        if (slot.Item is not Object)
            return;

        var tex = ItemRegistry.GetData(slot.Item.QualifiedItemId).GetTexture();
        var rect = ItemRegistry.GetData(slot.Item.QualifiedItemId).GetSourceRect();

        var cols = new Color[16 * 16];
        tex.GetData(0, rect, cols, 0, cols.Length);

        for (int i = 0; i < cols.Length; ++i)
        {
            int ix = i % 16;
            int iy = i / 16;

            float velDir = (float)Game1.random.NextDouble() * 3.14f * 2;
            Vector2 vel = new Vector2(MathF.Cos(velDir), MathF.Sin(velDir)) * (60 + Game1.random.Next(70));

            pixels.Add(new Pixel()
            {
                x = slot.Bounds.Location.X + 16 + ix * Game1.pixelZoom,
                y = slot.Bounds.Location.Y + 16 + iy * Game1.pixelZoom,
                color = cols[i],
                scale = 3 + (float)Game1.random.NextDouble() * 3,
                velocity = vel,
            });
        }
    }

    public override void draw(SpriteBatch b)
    {
        base.draw(b);

        IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White );
        IClickableMenu.drawTextureBox(b, this.invMenu.xPositionOnScreen - IClickableMenu.borderWidth, this.invMenu.yPositionOnScreen - IClickableMenu.borderWidth, this.invMenu.width + IClickableMenu.borderWidth * 2, this.invMenu.height + IClickableMenu.borderWidth * 2, Color.White);

        this.ui.Draw(b);
        this.invMenu.draw(b);

        // We use these in this menu, so draw them again here
        if (Game1.hudMessages.Count > 0)
        {
            int heightUsed = 0;
            for (int i = Game1.hudMessages.Count - 1; i >= 0; i--)
            {
                Game1.hudMessages[i].draw(Game1.spriteBatch, i, ref heightUsed);
            }
        }

        drawMouse(b);

        Game1.player.CursorSlotItem?.drawInMenu(b, Game1.getMousePosition().ToVector2(), 1);

        float delta = (float)Game1.currentGameTime.ElapsedGameTime.TotalSeconds;
        float ts = (float)(Game1.currentGameTime.TotalGameTime.TotalSeconds - animStart ?? 0);
        if (ts >= 1.4 && !playedSynthesizeSound)
        {
            Game1.playSound("spacechase0.MageDelve_alchemy_synthesize");
            playedSynthesizeSound = true;
        }
        if (ts < 0) ts = 0;
        Vector2 center = weaponSlot.Position + new Vector2(weaponSlot.Width /2, weaponSlot.Height / 2);
        float velMult = ts * ts * ts * ts * 5;
        List<Pixel> toRemove = [];
        for (int i = 0; i < pixels.Count; ++i)
        {
            Pixel pixel = pixels[i];
            float actualScale = (pixel.scale + MathF.Sin(ts * 3) - 3) % 3 + 3;

            Vector2 ppos = new Vector2(pixel.x, pixel.y) + pixel.velocity * delta;
            pixel.x = ppos.X;
            pixel.y = ppos.Y;
            Vector2 toCenter = center - ppos;
            float dist = Vector2.Distance(center, ppos);
            pixel.velocity = pixel.velocity * 0.99f + toCenter / dist * velMult;

            b.Draw(Game1.staminaRect, new Vector2(pixel.x, pixel.y), null, pixel.color, 0, Vector2.Zero, actualScale, SpriteEffects.None, 1);

            if (float.IsNaN(dist))
            {
                //Console.WriteLine("wat");
            }

            if (dist < 24 || float.IsNaN(dist))
            {
                toRemove.Add(pixel);
            }
        }
        pixels.RemoveAll(toRemove.Contains);


        static string GetDescription(Item item)
        {
            string desc = item.getDescription();
            if (item.HasContextTag("exquisite_gem"))
            {
                // So the gunther donate text doesn't show
                desc = ItemRegistry.GetData(item.QualifiedItemId).Description;
            }
            else if (ModSnS.CoatingIconMapping.ContainsKey(item.QualifiedItemId))
            {
                desc = I18n.GetByKey($"description.coating.{item.QualifiedItemId}");
            }
            else if (ModSnS.AlloyIconMapping.ContainsKey(item.QualifiedItemId))
            {
                desc = I18n.GetByKey($"description.alloying.{item.QualifiedItemId}");
            }

            return desc;
        }

        if (ItemWithBorder.HoveredElement != null)
        {
            if (ItemWithBorder.HoveredElement is ItemSlot slot && slot.Item != null)
            {
                drawToolTip(b, GetDescription(slot.Item), slot.Item.DisplayName, slot.Item);
            }
            else if (ItemWithBorder.HoveredElement.Equals(weaponPreview) && ItemWithBorder.HoveredElement is ItemWithBorder slot1 && slot1.ItemDisplay != null)
            {
                if (!slot1.TransparentItemDisplay)
                    drawToolTip(b, GetDescription(slot1.ItemDisplay), slot1.ItemDisplay.DisplayName, slot1.ItemDisplay);
                else
                    drawToolTip(b, I18n.String_ManabarPeview().Replace(':', ' '), null, null);
            }
            else if (ItemWithBorder.HoveredElement.UserData is string s)
            {
                drawToolTip(b, s, null, null);
            }
        }
        else
        {
            var hover = invMenu.hover(Game1.getMouseX(), Game1.getMouseY(), null);
            if (hover != null)
            {
                drawToolTip(b, GetDescription(hover), invMenu.hoverTitle, hover);
            }
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        Game1.player.CursorSlotItem = this.invMenu.leftClick(x, y, Game1.player.CursorSlotItem, playSound);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        base.receiveRightClick(x, y, playSound);
        Game1.player.CursorSlotItem = this.invMenu.rightClick(x, y, Game1.player.CursorSlotItem, playSound);
    }

    protected override void cleanupBeforeExit()
    {
        base.cleanupBeforeExit();
        List<Item> items = [];
        
        if (this.weaponSlot.Item != null)
            items.Add(this.weaponSlot.Item);
        if (this.alloyingSlot.Item != null)
            items.Add(this.alloyingSlot.Item);
        if (this.coatingSlot.Item != null)
            items.Add(this.coatingSlot.Item);
        if (this.gemSlot.Item != null)
            items.Add(this.gemSlot.Item);

        if (items.Count > 0)
            Game1.player.addItemsByMenuIfNecessary(items);
    }

    public override void emergencyShutDown()
    {
        base.emergencyShutDown();
        this.cleanupBeforeExit();
    }

    private void ApplySlots()
    {
        bool forged = false;
        if (gemSlot.Item != null)
        {
            (this.weaponSlot.Item as MeleeWeapon).SetExquisiteGemstone(this.gemSlot.Item?.QualifiedItemId);
            Pixelize(gemSlot);
            Game1.playSound("spacechase0.MageDelve_alchemy_particlize");
            gemSlot.Item = null;
            forged = true;
        }
        if (coatingSlot.Item != null)
        {
            if (coatingSlot.Item.Stack == ModSnS.CoatingQuantities[coatingSlot.Item.QualifiedItemId])
            {
                (this.weaponSlot.Item as MeleeWeapon).SetBladeCoating(this.coatingSlot.Item?.QualifiedItemId);
                Pixelize(coatingSlot);
                Game1.playSound("spacechase0.MageDelve_alchemy_particlize");
                coatingSlot.Item = null;
                forged = true;
            }
            else
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_NotEnough(ModSnS.CoatingQuantities[coatingSlot.Item.QualifiedItemId], Lexicon.makePlural(coatingSlot.Item.DisplayName))));
        }
        if (alloyingSlot.Item != null)
        {
            if (alloyingSlot.Item.Stack == 25)
            {
                (this.weaponSlot.Item as MeleeWeapon).SetBladeAlloying(this.alloyingSlot.Item.QualifiedItemId);
                Pixelize(alloyingSlot);
                Game1.playSound("spacechase0.MageDelve_alchemy_particlize");
                alloyingSlot.Item = null;
                forged = true;
            }
            else
                Game1.addHUDMessage(new HUDMessage(I18n.Anvil_NotEnough(25, Lexicon.makePlural(alloyingSlot.Item.DisplayName))));
        }

        if (forged)
        {
            animStart = (float)Game1.currentGameTime.TotalGameTime.TotalSeconds;
            playedSynthesizeSound = false;
            Game1.playSound("bigSelect");
            Game1.playSound("boulderCrack");
        }
    }

    private void UpdatePreview()
    {
        if (weaponSlot.Item != null)
        {
            MeleeWeapon preview = (weaponSlot.Item.getOne() as MeleeWeapon);

            if (gemSlot.Item != null)
                preview.SetExquisiteGemstone(gemSlot.Item.QualifiedItemId);

            if (coatingSlot.Item != null && coatingSlot.Item.Stack == ModSnS.CoatingQuantities[coatingSlot.Item.QualifiedItemId])
                preview.SetBladeCoating(coatingSlot.Item.QualifiedItemId);

            if (alloyingSlot.Item != null && alloyingSlot.Item.Stack == 25)
                preview.SetBladeAlloying(alloyingSlot.Item.QualifiedItemId);

            weaponPreview.TransparentItemDisplay = false;
            weaponPreview.ItemDisplay = preview;
            return;
        }
        weaponPreview.ItemDisplay = new MeleeWeapon("0");
        weaponPreview.TransparentItemDisplay = true;
    }
    private class Pixel
    {
        public float x;
        public float y;
        public Color color;
        public float scale;
        public Vector2 velocity;
    }
}

[HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.getExtraSpaceNeededForTooltipSpecialIcons))]
public static class MeleeWeaponTooltipPatch1
{
    public static void Postfix(MeleeWeapon __instance, SpriteFont font, ref Point __result)
    {
        if (__instance.GetExquisiteGemstone() != null)
            __result.Y += Math.Max((int)font.MeasureString("TT").Y, 48); ;
        if (__instance.GetBladeCoating() != null)
            __result.Y += Math.Max((int)font.MeasureString("TT").Y, 48); ;
        if (__instance.GetBladeAlloying() != null)
            __result.Y += Math.Max((int)font.MeasureString("TT").Y, 48);
    }
}

[HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.drawTooltip))]
public static class MeleeWeaponTooltipPatch2
{
    public static void Postfix(MeleeWeapon __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font)
    {
        var tex = Game1.content.Load<Texture2D>(ModSnS.SpriteSheetPath);

        string gem = __instance.GetExquisiteGemstone();
        if (gem != null)
        {
            var data = ItemRegistry.GetData(gem);
            Utility.drawWithShadow(spriteBatch, data.GetTexture(), new Vector2(x + 16, y + 16), data.GetSourceRect(), Color.White, 0, Vector2.Zero, 3f, layerDepth: 1);
            Utility.drawTextWithShadow(spriteBatch, I18n.GetByKey($"tooltip.gem.{gem}"), font, new Vector2(x + 16 + 44, y + 16 + 12), Game1.textColor);
            y += Math.Max((int)font.MeasureString("TT").Y, 48);
        }

        string coating = __instance.GetBladeCoating();
        if (coating != null)
        {
            Utility.drawWithShadow(spriteBatch, tex, new Vector2(x + 4, y + 8), Game1.getSourceRectForStandardTileSheet(tex, ModSnS.CoatingIconMapping[coating], 16, 16), Color.White, 0, Vector2.Zero, 4f, layerDepth: 1);
            Utility.drawTextWithShadow(spriteBatch, I18n.GetByKey($"tooltip.coating.{coating}"), font, new Vector2(x + 16 + 44, y + 16 + 12), Game1.textColor);
            y += Math.Max((int)font.MeasureString("TT").Y, 48);
        }

        string alloy = __instance.GetBladeAlloying();
        if (alloy != null)
        {
            Utility.drawWithShadow(spriteBatch, tex, new Vector2(x + 4, y + 8), Game1.getSourceRectForStandardTileSheet(tex, ModSnS.AlloyIconMapping[alloy], 16, 16), Color.White, 0, Vector2.Zero, 4f, layerDepth: 1);
            Utility.drawTextWithShadow(spriteBatch, I18n.GetByKey($"tooltip.alloying.{alloy}"), font, new Vector2(x + 16 + 44, y + 16 + 12), Game1.textColor);
            y += Math.Max((int)font.MeasureString("TT").Y, 48);
        }
    }
}

[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.damageMonster), [typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool)])]
public static class GameLocationDamageMonsterFlagsPatch
{
    internal static int IsSwinging = 0;
    internal static bool hasHealedYet = true;

    public static void Prefix()
    {
        if (IsSwinging == 0)
            hasHealedYet = false;
        ++IsSwinging;
    }

    public static void Postfix()
    {
        --IsSwinging;
        if (IsSwinging == 0)
            hasHealedYet = true;
    }
}

[HarmonyPatch(typeof(GameLocation), "isMonsterDamageApplicable")]
public static class GameLocationBatWingDamagePatch
{
    public static void Postfix(Farmer who, ref bool __result)
    {
        if ((who.CurrentTool is MeleeWeapon mw && mw.GetBladeCoating() == "(O)767" ) || (who.GetOffhand() is MeleeWeapon off && off.GetBladeCoating() == "(O)767"))
            __result = true;
    }
}

[HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.DoDamage))]
public static class MeleeWeaponVoidEssenceCoatingPatch
{
    public static void Postfix(MeleeWeapon __instance, GameLocation location, int x, int y, int facingDirection,
        Farmer who)
    {
        if (__instance.GetBladeCoating() != "(O)769")
            return;

        List<int> facings = [0, 1, 2, 3];
        facings.Remove(facingDirection);
        foreach (int newFacing in facings)
        {
            Vector2 z1 = Vector2.Zero, z2 = Vector2.Zero;
            Rectangle area = __instance.getAreaOfEffect(x, y, newFacing, ref z1, ref z2, who.GetBoundingBox(),
                who.FarmerSprite.currentAnimationIndex);
            location.damageMonster(area, (int)((double)(int)__instance.minDamage.Value * (1.0 + (double)who.buffs.AttackMultiplier)), (int)((double)(int)__instance.maxDamage.Value * (1.0 + (double)who.buffs.AttackMultiplier)), false, __instance.knockback.Value * (1f + who.buffs.KnockbackMultiplier), (int)((double)(int)__instance.addedPrecision.Value * (1.0 + (double)who.buffs.WeaponPrecisionMultiplier)), __instance.critChance.Value * (1f + who.buffs.CriticalChanceMultiplier), __instance.critMultiplier.Value * (1f + who.buffs.CriticalPowerMultiplier), (int)__instance.type.Value != 1 || !__instance.isOnSpecial, who);
            /*location.projectiles.RemoveWhere((Func<Projectile, bool>) (projectile =>
               {
               if (areaOfEffect.Intersects(projectile.getBoundingBox()) && !projectile.ignoreMeleeAttacks.Value)
               projectile.behaviorOnCollisionWithOther(location);
               return projectile.destroyMe;
               }));
             */
        }
    }
}

[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.damageMonster), [typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool)])]
public static class GameLocationDamageMonsterWorkaroundPatch
{
    public static int TakeDamageInlineWorkaround(Monster monster, int damage, int xTrajectory, int yTrajectory,
        bool isBomb, double addedPrecision, Farmer who)
    {
        MonsterTakeDamagePatch.Prefix(monster, ref damage, who);
        int ret = monster.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
        MonsterTakeDamagePatch.Postfix(monster, damage, who, ret);
        return ret;
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> insns)
    {
        // Writing a transpiler without a CIL viewer! Fun!
        // Pretty sure there's a utility method for this but I can't find it

        List<CodeInstruction> ret = [];

        foreach (var insn in insns)
        {
            if (insn.Calls(typeof(Monster).GetMethod(nameof(Monster.takeDamage), [typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer)])))
            {
                insn.opcode = OpCodes.Call;
                insn.operand = AccessTools.Method(typeof(GameLocationDamageMonsterWorkaroundPatch), nameof(TakeDamageInlineWorkaround));
            }

            ret.Add(insn);
        }

        return ret;
    }
}

//[HarmonyPatch(typeof(Monster), nameof(Monster.takeDamage),new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) })]
public static class MonsterTakeDamagePatch
{
    public static void Prefix(Monster __instance, ref int damage, Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon mw)
            return;

        float mult = 1;
        switch (mw.GetBladeAlloying())
        {
            case "(O)334": mult += 0.05f; break;
            case "(O)335": mult += 0.10f; break;
            case "(O)336": mult += 0.15f; break;
            case "(O)337": mult += 0.20f; break;
            case "(O)910": mult += 0.25f; break;
        }

        switch (mw.GetExquisiteGemstone())
        {
            case "(O)ExquisiteAquamarine":
                if (Game1.random.NextDouble() < 0.15)
                    mult *= mw.critMultiplier.Value * (1 + who.buffs.CriticalPowerMultiplier);
                break;
        }

        if (who.GetOffhand() is MeleeWeapon mw1)
        {
            switch (mw1.GetBladeAlloying())
            {
                case "(O)334": mult += 0.05f; break;
                case "(O)335": mult += 0.10f; break;
                case "(O)336": mult += 0.15f; break;
                case "(O)337": mult += 0.20f; break;
                case "(O)910": mult += 0.25f; break;
            }

            switch (mw.GetExquisiteGemstone())
            {
                case "(O)ExquisiteAquamarine":
                    if (Game1.random.NextDouble() < 0.15)
                        mult *= mw.critMultiplier.Value * (1 + who.buffs.CriticalPowerMultiplier);
                    break;
            }
        }


        if (who == Game1.player && who.HasCustomProfession( RogueSkill.ProfessionHuntersMark ))
        {
            if (ModSnS.State.LastAttacked == __instance)
            {
                mult += 0.025f * ++ModSnS.State.LastAttackedCounter;
            }
            else
            {
                ModSnS.State.LastAttacked = __instance;
                ModSnS.State.LastAttackedCounter = 0;
            }
        }

        damage = (int)(damage * mult);
    }
    public static void Postfix(Monster __instance, int damage, Farmer who, int __result)
    {
        if (__result > 0)
        {
            if (who.CurrentTool is MeleeWeapon mw)
            {
                switch (mw.GetExquisiteGemstone())
                {
                    case "(O)DN.SnS_ExquisiteEmerald":
                        who.buffs.Apply(new Buff("exquisiteemerald", duration: 5000,
                            effects: new BuffEffects() { Speed = { 1.5f } }));
                        break;

                    case "(O)DN.SnS_ExquisiteRuby":
                        DelayedAction.functionAfterDelay(() => { if (__instance.Health > 0) __instance.takeDamage((int)(damage * 0.1f), 0, 0, false, 0, "hitEnemy"); }, 1000);
                        DelayedAction.functionAfterDelay(() => { if (__instance.Health > 0) __instance.takeDamage((int)(damage * 0.1f), 0, 0, false, 0, "hitEnemy"); }, 2000);
                        DelayedAction.functionAfterDelay(() => { if (__instance.Health > 0) __instance.takeDamage((int)(damage * 0.1f), 0, 0, false, 0, "hitEnemy"); }, 3000);
                        break;

                    case "(O)DN.SnS_ExquisiteJade":
                        if (!GameLocationDamageMonsterFlagsPatch.hasHealedYet)
                        {
                            GameLocationDamageMonsterFlagsPatch.hasHealedYet = true;
                            who.Stamina += 2;
                        }
                        break;

                    case "(O)DN.SnS_ExquisiteAmethyst":
                        if (Game1.random.NextDouble() < 0.15)
                            __instance.stunTime.Value = 3000;
                        break;

                    case "(O)DN.SnS_ExquisiteDiamond":
                        if (!GameLocationDamageMonsterFlagsPatch.hasHealedYet)
                        {
                            GameLocationDamageMonsterFlagsPatch.hasHealedYet = true;
                            who.health = Math.Min(who.health + 1, who.maxHealth);
                        }
                        break;
                }

                switch (mw.GetBladeCoating())
                {
                    case "(O)766": // Slime
                        if (!__instance.modData.ContainsKey("DN.SnS_Slimed"))
                        {
                            __instance.modData.Add("DN.SnS_Slimed", "hoot");
                            __instance.addedSpeed = -1.5f;
                        }
                        break;
                    case "(O)768": // solar essence
                        if (__instance.Health <= 0)
                        {
                            __instance.currentLocation.explode(__instance.Tile, 2, who, false, 25);
                        }
                        break;
                }
            }
            if (who.GetOffhand() is MeleeWeapon mw1)
            {
                switch (mw1.GetExquisiteGemstone())
                {
                    case "(O)DN.SnS_ExquisiteEmerald":
                        who.buffs.Apply(new Buff("exquisiteemerald", duration: 5000,
                            effects: new BuffEffects() { Speed = { 1.5f } }));
                        break;

                    case "(O)DN.SnS_ExquisiteRuby":
                        DelayedAction.functionAfterDelay(() => { if (__instance.Health > 0) __instance.takeDamage((int)(damage * 0.1f), 0, 0, false, 0, "hitEnemy"); }, 1000);
                        DelayedAction.functionAfterDelay(() => { if (__instance.Health > 0) __instance.takeDamage((int)(damage * 0.1f), 0, 0, false, 0, "hitEnemy"); }, 2000);
                        DelayedAction.functionAfterDelay(() => { if (__instance.Health > 0) __instance.takeDamage((int)(damage * 0.1f), 0, 0, false, 0, "hitEnemy"); }, 3000);
                        break;

                    case "(O)DN.SnS_ExquisiteJade":
                        if (!GameLocationDamageMonsterFlagsPatch.hasHealedYet)
                        {
                            GameLocationDamageMonsterFlagsPatch.hasHealedYet = true;
                            who.Stamina += 2;
                        }
                        break;

                    case "(O)DN.SnS_ExquisiteAmethyst":
                        if (Game1.random.NextDouble() < 0.15)
                            __instance.stunTime.Value = 3000;
                        break;

                    case "(O)DN.SnS_ExquisiteDiamond":
                        if (!GameLocationDamageMonsterFlagsPatch.hasHealedYet)
                        {
                            GameLocationDamageMonsterFlagsPatch.hasHealedYet = true;
                            who.health = Math.Min(who.health + 1, who.maxHealth);
                        }
                        break;
                }

                switch (mw1.GetBladeCoating())
                {
                    case "(O)766": // Slime
                        if (!__instance.modData.ContainsKey("DN.SnS_Slimed"))
                        {
                            __instance.modData.Add("DN.SnS_Slimed", "hoot");
                            __instance.addedSpeed = -1.5f;
                        }
                        break;
                    case "(O)768": // solar essence
                        if (__instance.Health <= 0)
                        {
                            __instance.currentLocation.explode(__instance.Tile, 2, who, false, 25);
                        }
                        break;
                }
            }
        }
    }

    private class Pixel
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Color Color { get; set; }
        public float Scale { get; set; }
        public Vector2 Velocity { get; set; }
    }
}

[HarmonyPatch(typeof(GameLocation), "onMonsterKilled")]
public static class GameLocationBugMeatCoatingMoreLootPatch
{
    public static void Prefix(GameLocation __instance, Farmer who, Monster monster)
    {
        if (who.CurrentTool is MeleeWeapon mw &&
            mw.GetBladeCoating() == "(O)684")
        {
            __instance.monsterDrop(monster, monster.GetBoundingBox().Center.X, monster.GetBoundingBox().Center.Y, who);
        }
        if (who.GetOffhand() is MeleeWeapon mw2 &&
           mw2.GetBladeCoating() == "(O)684")
        {
            __instance.monsterDrop(monster, monster.GetBoundingBox().Center.X, monster.GetBoundingBox().Center.Y, who);
        }
    }
}

[HarmonyPatch(typeof(Farmer), nameof(Farmer.takeDamage))]
public static class FarmerLessDamageForExquisiteTopazPatch
{
    public static void Prefix(Farmer __instance, ref int damage)
    {
        if (__instance.CurrentTool is MeleeWeapon mw &&
            mw.GetExquisiteGemstone() == "(O)DN.SnS_ExquisiteTopaz")
        {
            damage = Math.Max(1, (int)(damage * 0.85));
        }

        if (__instance.GetOffhand() is MeleeWeapon mw1 &&
            mw1.GetExquisiteGemstone() == "(O)DN.SnS_ExquisiteTopaz")
        {
            damage = Math.Max(1, (int)(damage * 0.85));
        }
    }
}

[HarmonyPatch(typeof(GameLocation), "breakStone")]
public static class GameLocationBreakingStoneFlagPatch
{
    internal static int IsBreakingStone = 0;

    public static void Prefix()
    {
        IsBreakingStone++;
    }

    public static void Postfix()
    {
        IsBreakingStone--;
    }
}

[HarmonyPatch(typeof(Game1), nameof(Game1.createObjectDebris), [typeof(string), typeof(int), typeof(int), typeof(long), typeof(GameLocation)])]
[HarmonyBefore("DaLion.Professions")]
public static class Game1ChangeGemToExquisitePatch
{
    public static void Prefix(ref string id)
    {
        double chanceMult = 1;
        if (Game1.player.hasOrWillReceiveMail("StygiumPendantPower"))
            chanceMult = 2;

        if (Game1.player.GetCustomSkillLevel(ModSnS.RogueSkill) >= 4 &&
            GameLocationBreakingStoneFlagPatch.IsBreakingStone > 0 &&
            ModSnS.ExquisiteGemMappings.TryGetValue(id, out string newId) &&
            Game1.random.NextDouble() < 0.25 * chanceMult)
            id = newId;
        if (Game1.player.GetCustomSkillLevel(ModSnS.RogueSkill) >= 2 &&
            GameLocationBreakingStoneFlagPatch.IsBreakingStone > 0 &&
            ModSnS.PureOreMappings.TryGetValue(id, out newId) &&
            Game1.random.NextDouble() < 0.15 * chanceMult)
            id = newId;
    }
}

[HarmonyPatch(typeof(MeleeWeapon), "GetOneCopyFrom")]
public static class MeleeWeaponCopyDataInGetOneFromPatch
{

    public static void Postfix(MeleeWeapon __instance, Item source)
    {
        if (source is not MeleeWeapon mw) return;

        if (mw.GetExquisiteGemstone() != null) __instance.SetExquisiteGemstone(mw.GetExquisiteGemstone());
        if (mw.GetBladeCoating() != null) __instance.SetBladeCoating(mw.GetBladeCoating());
        if (mw.GetBladeAlloying() != null) __instance.SetBladeAlloying(mw.GetBladeAlloying());
    }
}

// Allow innate enchantments on everything
[HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.CanForge))]
public static class MeleeWeaponAllowForgingDragontoothOnAllWeaponsPatch
{
    public static void Postfix(Item item, ref bool __result)
    {
        if (item?.QualifiedItemId == "(O)852")
            __result = true;
    }
}
