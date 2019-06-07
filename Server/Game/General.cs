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

        private string name;
        private Gender gender;

        private string kingdom;
        private int double_max_hp;
        private bool classic_lord;
        private bool hegemony_lord;
        private bool hidden;

        public General(string name, string kingdom, bool classic_lord = false, bool hegemony_lord = false, int double_max_hp = 4, bool male = true, bool hidden = false)
        {
            this.name = name;
            this.kingdom = kingdom;
            this.double_max_hp = double_max_hp;
            this.gender = male ? Gender.Male : Gender.Female;
            this.hidden = hidden;
            this.hegemony_lord = hegemony_lord;
            this.classic_lord = classic_lord;
        }

        public bool IsLord(bool hegemony_mod = true) => hegemony_mod ? hegemony_lord : classic_lord;
        public int GetMaxHpHead() => double_max_hp + Head_max_hp_adjusted_value;
        public int GetMaxHpDeputy() => double_max_hp + Deputy_max_hp_adjusted_value;

        public bool CompanionWith(string name)
        {
            General other = Engine.GetGeneral(name);
            if (other == null) return false;

            if (kingdom != other.Kingdom)
                return false;
            return hegemony_lord || other.hegemony_lord || Companions.Contains(name)
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
