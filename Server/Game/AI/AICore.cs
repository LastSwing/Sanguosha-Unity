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
        public List<SkillEvent> Events { get; private set; }
        public List<UseCard> UseCards { get; private set; }
        public AIPackage(string name)
        {
            Name = name;
        }
    }

    public class SkillEvent
    {
        public string Name { private set; get; }
        public List<string> Key { private set; get; }

        public SkillEvent(string name)
        {
            Name = name;
        }

        public virtual void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
        }

        public virtual string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return string.Empty;
        }

        public virtual List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max, object data)
        {
            return new List<Player>();
        }

        public virtual List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids, object data)
        {
            return new List<int>();
        }

        public virtual CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data, HandlingMethod method)
        {
            return new CardUseStruct();
        }

        public virtual bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return false;
        }

        public virtual WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            return null;
        }

        public virtual WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            return null;
        }

        public virtual double GetSkillAdjustValue(TrustedAI ai, Player player)
        {
            return 0;
        }

        public virtual Dictionary<string, List<int>> OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            return new Dictionary<string, List<int>>();
        }

        public virtual List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip)
        {
            return new List<int>();
        }

        public int OnPickAG(TrustedAI ai, Player player, List<int> card_ids, bool refusable)
        {
            return -1;
        }

        public virtual WrappedCard OnCardShow(TrustedAI ai, Player player, Player requestor, object data)
        {
            return null;
        }

        public virtual WrappedCard OnPindian(TrustedAI ai, Player player, Player requestor)
        {
            return null;
        }

        public virtual Player OnYiji(TrustedAI ai, Player player, List<int> ids, ref int id)
        {
            return null;
        }

        public virtual List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return new List<int>();
        }

        public virtual double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            return 0;
        }

        public virtual double UseCardAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 0;
        }

        public virtual double GetDamegeEffect(TrustedAI ai, Player player, DamageStruct damage)
        {
            return 0;
        }

        public virtual bool IsProhibit(TrustedAI ai, Player player, Player to, WrappedCard card)
        {
            return false;
        }

        public virtual bool IsCancelTarget(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            return false;
        }

        public virtual bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            return true;
        }

        public virtual void DamageEffect(TrustedAI ai, ref DamageStruct damage)
        {
        }

        public virtual ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            return new ScoreStruct();
        }

        public virtual bool CanResist(TrustedAI ai, int damage)
        {
            return false;
        }

        public virtual bool CanRetrial(TrustedAI ai, string pattern, Player player, Player judge_who)
        {
            return false;
        }

        public virtual bool RetrialCardMatch(TrustedAI ai, Player player, Player judge_who, string pattern, int id)
        {
            return false;
        }
    }
    public class UseCard
    {
        public string Name { get; private set; }
        public List<string> Key { private set; get; }
        public UseCard(string class_name)
        {
            Name = class_name;
        }

        public virtual void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
        }

        public virtual void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
        }

        public virtual bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
        }

        public virtual double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            return 0;
        }

        public virtual bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            return true;
        }

        public virtual void DamageEffect(TrustedAI ai, ref DamageStruct damage)
        {
        }

        public virtual CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data, HandlingMethod method)
        {
            return new CardUseStruct();
        }

        public virtual WrappedCard OnCardShow(TrustedAI ai, Player player, Player request, object data)
        {
            return null;
        }

        public virtual String OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            return string.Empty;
        }
    }
}






