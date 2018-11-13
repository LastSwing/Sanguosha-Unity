using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanguoshaServer.Package
{
    public abstract class GeneralPackage
    {
        protected List<Skill> skills = new List<Skill>();
        protected string name;
        protected List<FunctionCard> skill_cards = new List<FunctionCard>();
        protected Dictionary<string, CardPattern> patterns = new Dictionary<string, CardPattern>();
        protected Dictionary<string, List<string>> related_skills = new Dictionary<string, List<string>>();
        protected Dictionary<string, List<string>> convert_pairs = new Dictionary<string, List<string>>();

        public List<Skill> Skills => skills;
        public string Name => name;
        public List<FunctionCard> SkillCards => skill_cards;
        public Dictionary<string, CardPattern> Patterns => patterns;
        public Dictionary<string, List<string>> RelatedSkills => related_skills;
        public Dictionary<string, List<string>> ConvertPairs => convert_pairs;
        public GeneralPackage(string name)
        {
            this.name = name;
        }

        protected void InsertConverts(string from, string to)
        {
            if (!convert_pairs.Keys.Contains(from))
                convert_pairs.Add(from, new List<string> { to });
            else
                convert_pairs[from].Add(to);
        }
    }

    public abstract class CardPackage
    {
        protected List<Skill> skills = new List<Skill>();
        protected string name;
        protected List<FunctionCard> cards = new List<FunctionCard>();
        Dictionary<string, CardPattern> patterns = new Dictionary<string, CardPattern>();

        public List<Skill> Skills => skills;
        public string Name => name;
        public List<FunctionCard> Cards => cards;
        public CardPackage(string name)
        {
            this.name = name;
        }
    }
}
