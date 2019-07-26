using System.Collections.Generic;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Game
{
    public class General
    {
        public string Name => name;
        public Gender GeneralGender => gender;
        public List<string> Skills { set; get; } = new List<string>();
        public string Kingdom => kingdom;
        public int DoubleMaxHp => double_max_hp;
        public List<string> Companions { set; get; } = new List<string>();
        public int Head_max_hp_adjusted_value { set; get; } = 0;
        public int Deputy_max_hp_adjusted_value { set; get; } = 0;
        
        public bool Hidden => hidden;

        private readonly string name;
        private readonly Gender gender;

        private readonly string kingdom;
        private readonly int double_max_hp;
        private bool lord;
        private string package_name;
        private readonly bool hidden;

        public General(string name, string kingdom, bool lord, string pack, int double_max_hp, bool male, bool hidden)
        {
            this.name = name;
            this.kingdom = kingdom;
            this.double_max_hp = double_max_hp;
            gender = male ? Gender.Male : Gender.Female;
            this.hidden = hidden;
            package_name = pack;
            this.lord = lord;
        }

        public bool IsLord() => lord;
        public int GetMaxHpHead() => double_max_hp + Head_max_hp_adjusted_value;
        public int GetMaxHpDeputy() => double_max_hp + Deputy_max_hp_adjusted_value;

        public bool CompanionWith(string name)
        {
            General other = Engine.GetGeneral(name, "Hegemony");
            if (other == null || !Engine.GetMode("Hegemony").GeneralPackage.Contains(package_name)) return false;

            if (kingdom != other.Kingdom)
                return false;
            return lord || other.lord || Companions.Contains(name)
                || other.Companions.Contains(Name);
        }


        public bool IsMale()
        {
            return gender == Gender.Male;
        }

        public bool IsFemale() 
        {
            return gender == Gender.Female;
        }

        public bool HasSkill(string skill, string mode, bool head)
        {
            List<string> skills = Engine.GetGeneralSkills(Name, mode, head);
            return skills.Contains(skill);
        }
    }
}
