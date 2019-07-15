using CommonClass.Game;
using SanguoshaServer.Game;
using System.Collections.Generic;

namespace SanguoshaServer.Package
{
    public class ClassicStanderCards : CardPackage
    {
        public ClassicStanderCards() : base("ClassicStanderCards")
        {
            skills = new List<Skill>
            {
                new ClassicBladeSkill(),
                new BladeTag(),
                new ClassicHalberdSkill()
            };
            cards = new List<FunctionCard> {
                new ClassicBlade(),
                new ClassicHalberd()
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "ClassicBlade", new List<string> { "#blade-target-mod" } },
            };
        }
    }

    public class ClassicBlade : Weapon
    {
        public ClassicBlade() : base("ClassicBlade", 3) { }
    }

    public class ClassicBladeSkill : WeaponSkill
    {
        public ClassicBladeSkill() : base("ClassicBlade")
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashMissed };
            frequency = Frequency.Compulsory;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            SlashEffectStruct effect = (SlashEffectStruct)data;
            if (!effect.To.Alive || effect.To.GetMark("Equips_of_Others_nullified_to_You") > 0)
                return false;

            player.SetFlags("slashTargetFix");
            player.SetFlags("slashTargetFixToOne");
            room.SetCardFlag(player.Weapon.Key, "using");
            effect.To.SetFlags("SlashAssignee");

            WrappedCard slash = room.AskForUseCard(player, "Slash:ClassicBlade", "@blade:" + effect.To.Name, -1, FunctionCard.HandlingMethod.MethodUse, false);
            if (slash == null)
            {
                player.SetFlags("-slashTargetFix");
                player.SetFlags("-slashTargetFixToOne");
                effect.To.SetFlags("-SlashAssignee");
                if (base.Triggerable(player, room))
                    room.SetCardFlag(player.Weapon.Key, "-using");
            }

            return false;
        }
    }

    public class BladeTag : TargetModSkill
    {
        public BladeTag() : base("#blade-target-mod") {}
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (Engine.MatchExpPattern(room, pattern, from, card) && from.HasWeapon("ClassicBlade") && room.GetCard(from.Weapon.Key).HasFlag("using"))
                return true;

            return false;
        }
    }

    public class ClassicHalberd : Weapon
    {
        public ClassicHalberd() : base("ClassicHalberd", 4) { }
    }

    public class ClassicHalberdSkill : TargetModSkill
    {
        public ClassicHalberdSkill() : base("ClassicHalberd") { }

        public override int GetExtraTargetNum(Room room, Player from, WrappedCard card)
        {
            if (from.HasWeapon(Name) && Engine.MatchExpPattern(room, pattern, from, card)
                && card.SubCards.Count > 0 && from.IsLastHandCard(card, true) && !card.SubCards.Contains(from.Weapon.Key))
                return 2;

            return 0;
        }
    }
}