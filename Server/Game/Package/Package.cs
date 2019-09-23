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

        public List<Skill> Skills => skills;
        public string Name => name;
        public List<FunctionCard> SkillCards => skill_cards;
        public Dictionary<string, CardPattern> Patterns => patterns;
        public Dictionary<string, List<string>> RelatedSkills => related_skills;
        public GeneralPackage(string name)
        {
            this.name = name;
        }
    }

    public abstract class CardPackage
    {
        protected List<Skill> skills = new List<Skill>();
        protected string name;
        protected List<FunctionCard> cards = new List<FunctionCard>();
        readonly Dictionary<string, CardPattern> patterns = new Dictionary<string, CardPattern>();
        protected Dictionary<string, List<string>> related_skills = new Dictionary<string, List<string>>();
        public List<Skill> Skills => skills;
        public string Name => name;
        public List<FunctionCard> Cards => cards;
        public Dictionary<string, List<string>> RelatedSkills => related_skills;
        public CardPackage(string name)
        {
            this.name = name;
        }
    }
}
