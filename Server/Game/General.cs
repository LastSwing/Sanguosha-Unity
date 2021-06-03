using System.Collections.Generic;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Game
{
    public class General
    {
        public enum KingdomENUM
        {
            Wei,
            Qun,
            Shu,
            Wu,
            Jin,
            TobeDicede,
        }

        public string Name { get; private set; }
        public Gender GeneralGender { get; private set; }
        public List<string> Skills { set; get; } = new List<string>();
        public List<KingdomENUM> Kingdom { get; private set; }
        public int DoubleMaxHp { get; private set; }
        public List<string> Companions { set; get; } = new List<string>();
        public int Head_max_hp_adjusted_value { set; get; } = 0;
        public int Deputy_max_hp_adjusted_value { set; get; } = 0;
        public bool Selectable { get; private set; }
        public string Package { get; private set; }
        public bool Hidden { get; private set; }

        private readonly bool lord;

        public General(string name, string kingdom, bool lord, string pack, int double_max_hp, bool male, bool selectable, bool hidden)
        {
            Name = name;
            Kingdom = new List<KingdomENUM>();
            if (kingdom.Contains("wei"))
                Kingdom.Add(KingdomENUM.Wei);
            if (kingdom.Contains("shu"))
                Kingdom.Add(KingdomENUM.Shu);
            if (kingdom.Contains("wu"))
                Kingdom.Add(KingdomENUM.Wu);
            if (kingdom.Contains("qun"))
                Kingdom.Add(KingdomENUM.Qun);
            if (kingdom.Contains("jin"))
                Kingdom.Add(KingdomENUM.Jin);
            if (kingdom.Contains("god"))
                Kingdom.Add(KingdomENUM.TobeDicede);

            DoubleMaxHp = double_max_hp;
            GeneralGender = male ? Gender.Male : Gender.Female;
            Selectable = selectable;
            Package = pack;
            this.lord = lord;
            Hidden = hidden;
        }

        public bool IsLord() => lord;
        public int GetMaxHpHead() => DoubleMaxHp + Head_max_hp_adjusted_value;
        public int GetMaxHpDeputy() => DoubleMaxHp + Deputy_max_hp_adjusted_value;

        public bool CompanionWith(string name)
        {
            General other = Engine.GetGeneral(name, "Hegemony");
            if (other == null || !Engine.GetMode("Hegemony").GeneralPackage.Contains(Package)) return false;
            
            return lord || other.lord || (Kingdom.Count == 1 && other.Kingdom.Count == 1
                && Kingdom[0] == other.Kingdom[0] && (Companions.Contains(name) || other.Companions.Contains(Name)));
        }

        public static KingdomENUM GetKingdom(CommonClass.Game.Player player)
        {
            switch (player.Kingdom)
            {
                case "wei":
                    return KingdomENUM.Wei;
                case "qun":
                    return KingdomENUM.Qun;
                case "shu":
                    return KingdomENUM.Shu;
                case "wu":
                    return KingdomENUM.Wu;
                case "jin":
                    return KingdomENUM.Jin;
            }

            return KingdomENUM.TobeDicede;
        }

        public static string GetKingdom(KingdomENUM kingdom)
        {
            switch (kingdom)
            {
                case KingdomENUM.Wei:
                    return "wei";
                case KingdomENUM.Shu:
                    return "shu";
                case KingdomENUM.Qun:
                    return "qun";
                case KingdomENUM.Wu:
                    return "wu";
                case KingdomENUM.Jin:
                    return "jin";
                default:
                    return "god";
            }
        }

        public bool IsMale()
        {
            return GeneralGender == Gender.Male;
        }

        public bool IsFemale() 
        {
            return GeneralGender == Gender.Female;
        }

        public bool HasSkill(string skill, string mode, bool head)
        {
            List<string> skills = Engine.GetGeneralSkills(Name, mode, head);
            return skills.Contains(skill);
        }
    }
}
