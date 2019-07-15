using CommonClass.Game;
using SanguoshaServer.Game;
using System.Collections.Generic;

namespace SanguoshaServer.Package
{
    public class ManeuveringCards : CardPackage
    {
        public ManeuveringCards() : base("ManeuveringCards")
        {
            skills = new List<Skill>
            {
                new GudingBladeSkill()
            };
            cards = new List<FunctionCard>
            {
                new GudingBlade(),
                new DefensiveHorse("Hualiu")
            };
        }
    }

    public class GudingBlade : Weapon
    {
        public GudingBlade() : base("GudingBlade", 2) { }
    }

    public class GudingBladeSkill : WeaponSkill
    {
        public GudingBladeSkill() : base("GudingBlade")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.To.IsKongcheng()
                && damage.Card != null && damage.Card.Name.Contains("Slash") && !damage.Chain && !damage.Transfer && damage.ByUser)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "$GudingBlade",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = damage.Damage.ToString(),
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);
            data = damage;
            room.SetEmotion(player, "gudingblade");

            return false;
        }
    }
}