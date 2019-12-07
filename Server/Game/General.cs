using System.Collections.Generic;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Game
{
    public class General
    {
        public string Name { get; private set; }
        public Gender GeneralGender { get; private set; }
        public List<string> Skills { set; get; } = new List<string>();
        public string Kingdom { get; private set; }
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
            Kingdom = kingdom;
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

            if (Kingdom != other.Kingdom)
                return false;
            return lord || other.lord || Companions.Contains(name)
                || other.Companions.Contains(Name);
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
