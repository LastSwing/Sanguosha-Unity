using CommonClass.Game;
using SanguoshaServer.Package;
using SanguoshaServer.Scenario;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static CommonClass.Game.CardUseStruct;
using static SanguoshaServer.Game.BattleArraySkill;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Game
{
    public enum TriggerEvent
    {
        NonTrigger,

        GameStart,
        RoundStart,
        TurnStart,
        EventPhaseStart,
        EventPhaseProceeding,
        EventPhaseEnd,
        EventPhaseChanging,
        EventPhaseSkipping,

        AfterDrawNCards,
        DrawPileChanged,
        CardDrawing,
        SwapPile,

        PreHpRecover,
        HpRecover,
        PreHpLost,
        HpChanging,
        HpChanged,
        MaxHpChanged,
        PostHpReduced,
        HpLost,

        EventLoseSkill,
        EventAcquireSkill,

        StartJudge,
        AskForRetrial,
        FinishRetrial,
        JudgeResult,
        FinishJudge,

        PindianCard,
        PindianVerifying,
        Pindian,

        TurnedOver,
        ChainStateCanceling,
        ChainStateChanged,
        RemoveStateChanged,

        ConfirmDamage,    // confirm the damage's count and damage's nature
        Predamage,        // trigger the certain skill -- jueqing
        DamageForseen,    // the first event in a damage -- kuangfeng dawu
        DamageCaused,     // the moment for -- qianxi..
        DamageInflicted,  // the moment for most of add damamge skills -- tianxiang..
        DamageDefined,   // the moment for reduce or prevent damage  -- kuanshi..
        PreDamageDone,    // before reducing Hp
        DamageDone,       // it's time to do the damage
        Damage,           // the moment for -- lieren..
        Damaged,          // the moment for -- yiji..
        DamageComplete,   // the moment for trigger iron chain

        Dying,
        QuitDying,
        AskForPeaches,
        AskForPeachesDone,
        Death,
        BuryVictim,
        BeforeGameOverJudge,
        GameOverJudge,
        GameFinished,

        SlashEffected,
        SlashProceed,
        SlashHit,
        SlashMissed,

        JinkEffect,

        NullificationEffect,

        CardAsked,
        CardResponded,
        BeforeCardsMove, // sometimes we need to record cards before the move
        CardsMoveOneTime,

        PreCardUsed,
        CardUsedAnnounced,   //Fan lihuo change slash
        CardTargetAnnounced,     //Halberd duanbing extra target
        CardUsed,
        TargetChoosing, //distinguish "choose target" and "confirm target"
        TargetConfirming,
        TargetChosen,
        TargetConfirmed,
        CardEffect,
        CardEffected,
        CardEffectConfirmed, //after Nullification
        CardFinished,
        TrickCardCanceling,

        ChoiceMade,

        StageChange, // For hulao pass only
        FetchDrawPileCard, // For miniscenarios only

        GeneralShown, // For Official Hegemony mode
        GeneralHidden, // For Official Hegemony mode
        GeneralStartRemove, // For Official Hegemony mode
        GeneralRemoved, // For Official Hegemony mode

        DFDebut, // for Dragon Phoenix Debut

        NumOfEvents,
    };

    public abstract class Skill
    {
        public enum Frequency
        {
            Frequent,
            NotFrequent,
            Compulsory,
            Limited,
            Wake,
        };

        public enum SkillType
        {
            Normal,
            Attack,
            Defense,
            Recover,
            Wizzard,
            Alter,
            Replenish,
            Masochism
        };

        protected string name;
        protected string limit_mark;
        protected Frequency frequency;
        protected string relate_to_place;
        protected bool attached_lord_skill;
        protected bool lord_skill;
        protected SkillType skill_type;
        protected bool turn = false;

        public string Name => name;
        public Frequency SkillFrequency => frequency;
        public string LimitMark => limit_mark;
        public string Relate_to_place => relate_to_place;
        public bool Attached_lord_skill => attached_lord_skill;
        public SkillType Skill_type => skill_type;
        public bool LordSkill => lord_skill;
        public bool Visible => !name.StartsWith("#");
        public bool Turn => turn;
        public virtual void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -1;
        }

        public virtual bool CanPreShow()
        {
            if (this is TriggerSkill triskill)
            {
                return triskill.ViewAsSkill == null;
            }

            return false;
        }

        public Skill(string name)
        {
            this.name = name;
        }
        public virtual bool MoveFilter(Room room, int id, List<int> downs) => true;
        public virtual bool SortFilter(Room room, List<int> to_sorts, List<int> ups, List<int> downs) => false;
    }

    public class TriggerSkill : Skill
    {
        //~TriggerSkill() {
        //    view_as_skill = null;
        //}

        protected ViewAsSkill view_as_skill = null;
        protected List<TriggerEvent> events = new List<TriggerEvent>();
        protected bool global;
        protected Dictionary<TriggerEvent, double> priority = new Dictionary<TriggerEvent, double>();

        public ViewAsSkill ViewAsSkill => view_as_skill;
        public List<TriggerEvent> TriggerEvents => events;

        public bool Global => global;

        public TriggerSkill(string name) : base(name)
        {
        }
        virtual public int GetPriority()
        {
            return 3;
        }
        virtual public double GetDynamicPriority(TriggerEvent e)
        {
            if (priority.Keys.Contains(e))
                return priority[e];
            else
                return GetPriority();
        }
        //     double getCurrentPriority() const
        //     {
        //         return current_priority;
        //     }
        //     void setCurrentPriority(double p) const
        //     {
        //         current_priority = p;
        //     }

        public void InsertPriority(TriggerEvent e, double value)
        {
            priority[e] = value;
        }

        public virtual bool Triggerable(Player target, Room room)
        {
            return target != null && target.Alive && RoomLogic.PlayerHasSkill(room, target, Name);
        }
        public virtual void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
        }

        public virtual List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_lists = new List<TriggerStruct>();
            if (Name == "gamerule") return skill_lists;

            TriggerStruct skill_list = Triggerable(triggerEvent, room, player, ref data, null);
            if (!string.IsNullOrEmpty(skill_list.SkillName))
            {
                for (int i = 0; i < skill_list.Times; i++)
                {
                    TriggerStruct skill = skill_list;
                    skill.Times = 1;
                    skill_lists.Add(skill);
                }
            }

            return skill_lists;
        }
        virtual public TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (Triggerable(player, room))
                return new TriggerStruct(Name, player);
            return new TriggerStruct(string.Empty);
        }
        virtual public TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return info;
        }
        virtual public bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return false;
        }
    }

    public abstract class PhaseChangeSkill : TriggerSkill
    {
        public PhaseChangeSkill(string name) : base(name)
        {
            events.Add(TriggerEvent.EventPhaseStart);
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return OnPhaseChange(room, player, info);
        }
        public abstract bool OnPhaseChange(Room room, Player player, TriggerStruct info);
    }
    
    public abstract class DrawCardsSkill : TriggerSkill
    {
        public DrawCardsSkill(string name) : base(name)
        {
            events.Add(TriggerEvent.EventPhaseProceeding);
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Draw && (int)data >= 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int n = (int)data;
            data = GetDrawNum(room, player, n);
            return false;
        }
        public abstract int GetDrawNum(Room room, Player player, int n);

    }

    public class DetachEffectSkill : TriggerSkill
    {
        protected string skill_name;
        protected string pile_name;
        public DetachEffectSkill(string skillname, string pilename) : base(string.Format("#{0}-clear", skillname))
        {
            skill_name = skillname;
            pile_name = pilename;
            events.Add(TriggerEvent.EventLoseSkill);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && data is InfoStruct info && info.Info == skill_name)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    SkillPosition = info.Head ? "head" : "deputy"
                };
                return trigger;
            }
            return new TriggerStruct();
        }
        /*
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct inf)
        {
            if (RoomLogic.PlayerHasSkill(room, player, skill_name) && player.Alive && data is InfoStruct info)
            {
                if (!player.HasShownSkill(skill_name, info.Head) && room.AskForSkillInvoke(player, name, null, inf.SkillPosition))
                {
                    room.ShowSkill(player, skill_name, inf.SkillPosition);
                    return new TriggerStruct();
                }
                else if (player.HasShownSkill(skill_name, info.Head))
                    return new TriggerStruct();
            }
            return inf;
        }
        */
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (!string.IsNullOrEmpty(pile_name))
                room.ClearOnePrivatePile(player, pile_name);

            OnSkillDetached(room, player, data);
            return false;
        }
        public virtual void OnSkillDetached(Room room, Player player, object data)
        {
        }
    }

    public class WeaponSkill : TriggerSkill
    {
        public WeaponSkill(string name) : base(name) { }
        public override int GetPriority() => 2;
        public override bool Triggerable(Player target, Room room)
        {
            if (target == null) return false;
            return target.Alive && target.HasWeapon(Name);
        }
    }

    public abstract class MasochismSkill : TriggerSkill
    {

        public MasochismSkill(string name) : base(name)
        {
            events.Add(TriggerEvent.Damaged);
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return base.Cost(triggerEvent, room, player, ref data, ask_who, info);
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            OnDamaged(room, player, damage, info);

            return false;
        }
        public abstract void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info);
    }

    public class ArmorSkill : TriggerSkill
    {

        public ArmorSkill(string name) : base(name) { }

        public override int GetPriority() => 2;
        public override bool Triggerable(Player target, Room room)
        {
            if (target == null) return false;
            return RoomLogic.HasArmorEffect(room, target, Name);
        }
        public virtual TriggerStruct Cost(Room room, ref object data, TriggerStruct info)
        {
            Player target = room.FindPlayer(info.Invoker, true);
            if (RoomLogic.HasArmorEffect(room, target, Name, false) &&
                (frequency == Frequency.Compulsory || room.AskForSkillInvoke(target, Name, data)))
                return info;

            List<string> all = new List<string>();
            bool show = false;
            bool protect = true;
            foreach (ViewHasSkill vhskill in Engine.ViewHas(room, target, Name, "armor")) {
                Skill mskill = Engine.GetMainSkill(vhskill.Name);
                if (RoomLogic.PlayerHasShownSkill(room, target, mskill))
                {
                    protect = false;
                    if (frequency == Frequency.Compulsory)
                        show = true;
                }
                all.Add(mskill.Name);
            }
            if (protect) target.SetFlags("Global_askForSkillCost");

            if (all.Count > 0)
            {
                List<TriggerStruct> skills = new List<TriggerStruct>();
                foreach (string sk in all)
                {
                    TriggerStruct skill = new TriggerStruct(sk, target)
                    {
                        ResultTarget = info.ResultTarget
                    };
                    skills.Add(skill);
                }
                TriggerStruct result = room.AskForSkillTrigger(target, "armorskill", skills, !show, data);
                
                if (string.IsNullOrEmpty(result.SkillName))
                    return result;
                else
                {
                    TriggerSkill result_skill = Engine.GetTriggerSkill(result.SkillName);
                    if (result_skill != null)
                    {
                        if (show || all.Count > 1 || room.AskForSkillInvoke(target, result_skill.Name, data, result.SkillPosition))
                        {
                            return result;
                        }
                    }
                    else if (show || all.Count > 1 || room.AskForSkillInvoke(target, result_skill.Name, data, result.SkillPosition))
                        return info;
                }
            }
            return new TriggerStruct();
        }
    }

    public class TreasureSkill : TriggerSkill
    {

        public TreasureSkill(string name) : base(name) { }

        public override int GetPriority() => 2;
        public override bool Triggerable(Player target, Room room)
        {
            if (target == null) return false;
            return target.HasTreasure(Name);
        }
    }

    public abstract class ViewAsSkill : Skill
    {
        public enum GuhuoType
        {
            VirtualCard,
            PopUpBox
        }

        protected string response_pattern;
        protected bool response_or_use;
        protected string expand_pile;
        public ViewAsSkill(string name) : base(name)
        {
            response_pattern = null;
            response_or_use = false;
            expand_pile = null;
        }
        public virtual GuhuoType GetGuhuoType()
        {
            return GuhuoType.VirtualCard;
        }

        public virtual bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }
        public abstract WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player);
        public virtual bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            bool huashen = false;
            if (invoker.ContainsTag("Huashens"))
            {
                //List<string> huashens = (List<string>)(invoker.GetTag("Huashens"));                 //for huashen
                //foreach (string general in huashens) {
                //    if (Engine.GetHuashenSkills(general).contains(Name))
                //    {
                //        huashen = true;
                //        break;
                //    }
                //}
            }

            if (!RoomLogic.PlayerHasSkill(room, invoker, Name) && !huashen) return false;

            if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == Nullification.ClassName)
                return IsEnabledAtNullification(room, invoker);

            switch (reason)
            {
                case CardUseReason.CARD_USE_REASON_PLAY:
                    return IsEnabledAtPlay(room, invoker);
                case CardUseReason.CARD_USE_REASON_RESPONSE:
                case CardUseReason.CARD_USE_REASON_RESPONSE_USE:
                    return IsEnabledAtResponse(room, invoker, pattern);
                default:
                    return false;
            }
        }
        public virtual bool IsEnabledAtPlay(Room room, Player player)
        {
            return string.IsNullOrEmpty(response_pattern);
        }
        public virtual bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (!string.IsNullOrEmpty(response_pattern))
                return Engine.GetPattern(pattern).GetPatternString() == Engine.GetPattern(response_pattern).GetPatternString();
            return false;
        }
        public virtual bool IsEnabledAtNullification(Room room, Player player)
        {
            return false;
        }
        public static ViewAsSkill ParseViewAsSkill(Skill skill)
        {
            if (skill == null) return null;
            if (skill is ViewAsSkill vsskill)
            {
                return vsskill;
            }
            if (skill is TriggerSkill trigger_skill)
            {
                return trigger_skill.ViewAsSkill;
            }
            if (skill is DistanceSkill dskill)
            {
                return dskill.ViewAsSkill;
            }

            return null;
        }
        public static ViewAsSkill ParseViewAsSkill(string skill) => ParseViewAsSkill(Engine.GetSkill(skill));
        public virtual List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player) => new List<WrappedCard>();
        public static List<string> GetGuhuoCards(Room room, string type)
        {
            List<string> all = new List<string>();
            if (type.Contains("b"))
            {
                foreach (FunctionCard fcard in room.AvailableFunctionCards)
                {
                    if (fcard is BasicCard && !all.Contains(fcard.Name))
                    {
                        all.Add(fcard.Name);
                    }
                }
            }

            if (type.Contains("t"))
            {
                foreach (FunctionCard fcard in room.AvailableFunctionCards)
                {
                    if (fcard.IsNDTrick() && !all.Contains(fcard.Name))
                    {
                        all.Add(fcard.Name);
                    }
                }
            }

            if (type.Contains("d"))
            {
                foreach (FunctionCard card in room.AvailableFunctionCards)
                {
                    if (!card.IsNDTrick() && card is TrickCard && !all.Contains(card.Name))
                    {
                        all.Add(card.Name);
                    }
                }
            }

            return all;
        }

        public bool IsResponseOrUse() => response_or_use;
        public virtual string GetExpandPile() => expand_pile;

    }

    public abstract class ViewHasSkill : Skill
    {
        public bool Global => global;
        public List<string> Skills => viewhas_skills;
        public List<string> Armors => viewhas_armors;
        public List<string> Treasures => viewhas_treasure;

        protected bool global;
        protected List<string> viewhas_skills = new List<string>();
        protected List<string> viewhas_armors = new List<string>();
        protected List<string> viewhas_treasure = new List<string>();
        public ViewHasSkill(string name) : base(name)
        {
            frequency = Frequency.Compulsory;
        }

        public abstract bool ViewHas(Room room, Player player, string skill_name);
    }

    public class OneCardViewAsSkill : ViewAsSkill
    {

        public OneCardViewAsSkill(string name) : base(name)
        {}
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count != 1)
                return null;
            else
                return ViewAs(room, cards[0], player);
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count == 0 && !to_select.HasFlag("using") && ViewFilter(room, to_select, player);
        }
        public virtual bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            if (!string.IsNullOrEmpty(filter_pattern))
            {
                string pat = filter_pattern;
                if (pat.EndsWith("!"))
                {
                    if (RoomLogic.IsJilei(room, player, to_select)) return false;
                    pat = pat.Substring(0, pat.Length - 1);
                }
                if (response_or_use && pat.Contains("hand"))
                {
                    List<string> handlist = new List<string> { "hand" };
                    foreach (string pile in player.GetPileNames()) {
                        if (pile.StartsWith("&") || pile == "wooden_ox")
                            handlist.Add(pile);
                    }
                    pat = pat.Replace("hand", string.Join(",", handlist));
                }
                return Engine.MatchExpPattern(room, pat, player, to_select);
            }
            return false;
        }

        public virtual WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            return null;
        }

        protected string filter_pattern;
    }

    public abstract class FilterSkill : ZeroCardViewAsSkill
    {     
        public FilterSkill(string name) : base(name)
        {
            frequency = Frequency.Compulsory;
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position)
        {
            if (reason == CardUseReason.CARD_USE_REASON_PLAY)
                return !invoker.HasShownSkill(Name, position == "head");

            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(ShowGeneralCard.ClassName)
            {
                UserString = Name
            };
            return card;
        }
        public abstract bool ViewFilter(Room room, WrappedCard to_select, Player player);
        public abstract void ViewAs(Room room, ref RoomCard card, Player player);
    }
    public abstract class ZeroCardViewAsSkill : ViewAsSkill
    {
        public ZeroCardViewAsSkill(string name) : base(name)
        { }
        
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            return ViewAs(room, player);
        }

        public abstract WrappedCard ViewAs(Room room, Player player);
    }

    public class DiscardSkill : ViewAsSkill
    {

        private WrappedCard card;
        public DiscardSkill() : base("discard")
        {
            card = new WrappedCard(DummyCard.ClassName);
        }

        public int Num { get; set; }
        public int MinNum { get; set; }
        public bool Optional { get; set; }
        public List<int> AvailableCards { get; set; }
        public List<int> Reserved { get; set; } = new List<int>();

        public bool IsFull()
        {
            return card.SubCards.Count >= MinNum;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count + Reserved.Count >= Num || !AvailableCards.Contains(to_select.Id))
                return false;

            if (RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodDiscard) || Reserved.Contains(to_select.Id))
                return false;

            return true;
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if ((!Optional && cards.Count > 0) || cards.Count + Reserved.Count >= MinNum)
            {
                card.ClearSubCards();
                card.AddSubCards(cards);
                card.AddSubCards(Reserved);
                return card;
            }
            else
                return null;
        }
    }

    public class ExchangeSkill : ViewAsSkill
    {
        private WrappedCard card;
        private int num;
        private int minnum;
        private string pattern;
        public ExchangeSkill() : base("exchange")
        {
            card = new WrappedCard(DummyCard.ClassName);
        }

        public void Initialize(int num, int minnum, string expand_pile, string pattern)
        {
            this.num = num;
            this.minnum = minnum;
            this.expand_pile = expand_pile;
            this.pattern = pattern;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count >= num)
                return false;

            string pat = pattern;
            if (pat.EndsWith("!"))
            {
                if (RoomLogic.IsJilei(room, player, to_select)) return false;
                pat = pat.Substring(0, pat.Length -1);
            }

            return Engine.MatchExpPattern(room, pat, player, to_select);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0) return null;
            if (cards.Count >= minnum)
            {
                card.ClearSubCards();
                card.AddSubCards(cards);
                return card;
            }
            else
                return null;
        }
    }

    public class YijiViewAsSkill : ViewAsSkill
    {
        private WrappedCard card;
        private List<int> ids;
        private int max_num;

        public YijiViewAsSkill() : base("yiji")
        {
            card = new WrappedCard(YijiCard.ClassName);
        }

        public void Initialize(List<int> ids, int max_num, List<Player> targets, string expand_pile)
        {
            this.ids = ids;
            this.max_num = max_num;

            List<string> player_names = new List<string>();
            foreach (Player p in targets)
                player_names.Add(p.Name);
            card.UserString = string.Join("+", player_names);
            this.expand_pile = expand_pile;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return ids.Contains(to_select.Id) && selected.Count < max_num;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0 || cards.Count > max_num)
                return null;

            card.ClearSubCards();
            card.AddSubCards(cards);
            return card;
        }
    }

    public class ProhibitSkill : Skill
    {
        public enum ProhibitType
        {
            Chain,
            Pindian,
        }
        public ProhibitSkill(string name) : base(name)
        {
        }

        public virtual bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null) => false;
        public virtual bool IsProhibited(Room room, Player from, Player to, ProhibitType type) => false;
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }
    public abstract class FixCardSkill : Skill
    {
        public FixCardSkill(string name) : base(name)
        {
        }
        public abstract bool IsCardFixed(Room room, Player from, Player to, string flags, HandlingMethod method);
    }
    public class DistanceSkill : Skill
    {
        protected ViewAsSkill view_as_skill;

        public ViewAsSkill ViewAsSkill => view_as_skill;
        public DistanceSkill(string name) : base(name)
        {
            frequency = Frequency.Compulsory;
            view_as_skill = new ShowDistanceSkill(name);
        }
        public virtual int GetCorrect(Room room, Player from, Player to, WrappedCard card = null) => 0;
        public virtual int GetFixed(Room room, Player from, Player to) => 0;
    }

    public class ShowDistanceSkill : ZeroCardViewAsSkill
    {

        public ShowDistanceSkill(string name) : base(name)
        {
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(ShowGeneralCard.ClassName)
            {
                UserString = Name
            };
            return card;
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position)
        {
            DistanceSkill skill = (DistanceSkill)Engine.GetSkill(Name);
            bool head = (position == "head" ? true : false);
            if (skill != null && reason == CardUseReason.CARD_USE_REASON_PLAY && !invoker.HasShownSkill(Name, head))
                return true;

            return false;
        }
    }
    public class MaxCardsSkill : Skill
    {
        public MaxCardsSkill(string name) : base(name)
        { }
        public virtual int GetExtra(Room room, Player target) => 0;
        public virtual int GetFixed(Room room, Player target) => -1;
        public virtual bool Ingnore(Room room, Player player, int card_id) => false;
    }
    public class TargetModSkill : Skill
    {
        protected string pattern = Slash.ClassName;
        public TargetModSkill(string name, bool skill_related = true) : base(name)
        {
            SkillRelated = skill_related;
        }

        public enum ModType
        {
            Residue,
            DistanceLimit,
            ExtraMaxTarget,
            ExtraTarget,
            SpecificAssignee,
            History,
            SpecificTarget,
            AttackRange,
        };

        public bool SkillRelated { get; protected set; }
        public virtual string Pattern => pattern;
        public virtual int GetResidueNum(Room room, Player from, WrappedCard card) => 0;
        public virtual int GetExtraTargetNum(Room room, Player from, WrappedCard card) => 0;
        public virtual bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern) => false;
        public virtual bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern) => false;
        public virtual bool IgnoreCount(Room room, Player from, WrappedCard card) => false;
        public virtual bool CheckExtraTargets(Room room, Player from, Player to, WrappedCard card,
                                  List<Player> previous_targets, List<Player> targets = null) => false;
        public virtual bool CheckSpecificTarget(Room room, Player from, Player to, WrappedCard card) => false;
        public virtual bool InAttackRange(Room room, Player from, Player to, WrappedCard card) => false;
        public virtual void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -1;
        }
    }

    public abstract class InvalidSkill : Skill
    {
        public InvalidSkill(string name) : base(name)
        {
            frequency = Frequency.Compulsory;
        }

        public abstract bool Invalid(Room room, Player player, string skill);
    }
    public class AttackRangeSkill : Skill
    {
        public AttackRangeSkill(string name) : base(name)
        {
        }
        public virtual int GetExtra(Room room, Player target, bool include_weapon) => 0;
        public virtual int GetFixed(Room room, Player target, bool include_weapon) => -1;
    }

    public class BattleArraySkill : TriggerSkill
    {
        public enum ArrayType
        {
            Siege,
            Formation
        };

        public ArrayType Type { get; set; }
        public BattleArraySkill(string name, ArrayType type) : base(name)
        {
            Type = type;
            view_as_skill = new ArraySummonSkill(name);
        }

        public override bool Triggerable(Player player, Room room)
        {
            return base.Triggerable(player, room) && room.GetAlivePlayers().Count >= 4;
        }

       public virtual void SummonFriends(Room room, Player player)
        {
            room.SummonFriends(player, Type);
        }
    }

    public class ArraySummonSkill : ZeroCardViewAsSkill
    {
        public ArraySummonSkill(string name) : base(name)
        { }
        
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(Name)
            {
                Mute = true
            };
            return card;
        }
       public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (room.AliveCount() < 4) return false;
            if (player.HasFlag("Global_SummonFailed")) return false;
            if (!player.CanShowGeneral(RoomLogic.InPlayerHeadSkills(player, Name) ? "h" : "d")) return false;
            if (!player.HasShownOneGeneral() && Hegemony.WillbeRole(room, player) == "careerist") return false;
            BattleArraySkill skill = (BattleArraySkill)Engine.GetTriggerSkill(Name);
            if (skill != null)
            {
                ArrayType type = skill.Type;
                switch (type)
                {
                    case ArrayType.Siege:
                        {
                            if (RoomLogic.WillBeFriendWith(room, player, room.GetNextAlive(player))
                                && RoomLogic.WillBeFriendWith(room, player, room.GetLastAlive(player)))
                                return false;
                            if (!RoomLogic.WillBeFriendWith(room, player, room.GetNextAlive(player)))
                            {
                                if (!room.GetNextAlive(player, 2).HasShownOneGeneral() && room.GetNextAlive(player).HasShownOneGeneral())
                                    return true;
                            }
                            if (!RoomLogic.WillBeFriendWith(room, player, room.GetLastAlive(player)))
                                return !room.GetLastAlive(player, 2).HasShownOneGeneral() && room.GetLastAlive(player).HasShownOneGeneral();
                            break;
                        }
                    case ArrayType.Formation:
                        {
                            int n = room.AliveCount(false);
                            int asked = n;
                            for (int i = 1; i < n; ++i)
                            {
                                Player target = room.GetNextAlive(player, i);
                                if (RoomLogic.IsFriendWith(room, player, target))
                                    continue;
                                else if (!target.HasShownOneGeneral())
                                    return true;
                                else
                                {
                                    asked = i;
                                    break;
                                }
                            }
                            n -= asked;
                            for (int i = 1; i < n; ++i)
                            {
                                Player target = room.GetLastAlive(player, i);
                                if (RoomLogic.IsFriendWith(room, player, target))
                                    continue;
                                else return !target.HasShownOneGeneral();
                            }
                            break;
                        }
                }
            }
            return false;
        }
    }
}



