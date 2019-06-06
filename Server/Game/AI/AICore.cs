using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static SanguoshaServer.Game.FunctionCard;

namespace SanguoshaServer.AI
{
    public class AIPackage
    {
        public string Name { private set; get; }
        public List<SkillEvent> Events => events;
        public List<UseCard> UseCards => use_cards;

        protected List<UseCard> use_cards = new List<UseCard>();
        protected List<SkillEvent> events = new List<SkillEvent>();
        public AIPackage(string name)
        {
            Name = name;
        }
    }

    public class SkillEvent
    {
        public string Name { private set; get; }
        public List<string> Key => key;
        protected List<string> key = new List<string>();

        public SkillEvent(string name)
        {
            Name = name;
        }

        public virtual void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
        }

        public virtual List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player) => new List<WrappedCard>();

        public virtual string OnChoice(TrustedAI ai, Player player, string choice, object data) => string.Empty;

        public virtual List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max, object data) => new List<Player>();

        public virtual List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids, object data) => new List<int>();

        public virtual CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data) => new CardUseStruct();

        public virtual bool OnSkillInvoke(TrustedAI ai, Player player, object data) => false;

        public virtual WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place) => null;

        public virtual WrappedCard GetTurnUse(TrustedAI ai, Player player) => null;

        public virtual double GetSkillAdjustValue(TrustedAI ai, Player player) => 0;

        public virtual Dictionary<string, List<int>> OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max) => new Dictionary<string, List<int>>();

        public virtual List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip) => null;

        public virtual int OnPickAG(TrustedAI ai, Player player, List<int> card_ids, bool refusable) => -1;

        public virtual WrappedCard OnCardShow(TrustedAI ai, Player player, Player requestor, object data) => null;

        public virtual WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> player) => null;

        public virtual Player OnYiji(TrustedAI ai, Player player, List<int> ids, ref int id) => null;

        public virtual List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile) => new List<int>();

        public virtual double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place) => 0;

        public virtual double UseCardAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card) => 0;

        public virtual bool IsProhibit(TrustedAI ai, Player player, Player to, WrappedCard card) => false;

        public virtual bool IsCancelTarget(TrustedAI ai, WrappedCard card, Player from, Player to) => false;

        public virtual bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to) => true;

        public virtual void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
        }

        public virtual ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage) => new ScoreStruct();

        public virtual bool CanResist(TrustedAI ai, int damage) => false;

        public virtual bool CanRetrial(TrustedAI ai, string pattern, Player player, Player judge_who) => false;

        public virtual bool RetrialCardMatch(TrustedAI ai, Player player, Player judge_who, string pattern, int id) => false;
    }
    public class UseCard
    {
        public struct NulliResult
        {
            public bool Null { set; get; }
            public bool Heg { set; get; }
        }
  
        public string Name { get; private set; }
        public List<string> Key => key;
        protected List<string> key = new List<string>();
        public UseCard(string class_name)
        {
            Name = class_name;
        }

        public virtual void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
        }
        public virtual bool OnSkillInvoke(TrustedAI ai, Player player, object data) => false;
        public virtual void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
        }
        public virtual List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids, object data) => new List<int>();
        public virtual NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult
            {
                Null = false,
                Heg = false
            };
            return result;
        }

        public virtual double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place) => 0;

        public virtual bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to) => true;

        public virtual void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
        }

        public virtual CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data) => new CardUseStruct();

        public virtual WrappedCard OnCardShow(TrustedAI ai, Player player, Player request, object data) => null;

        public virtual string OnChoice(TrustedAI ai, Player player, string choices, object data) => string.Empty;
    }
}






