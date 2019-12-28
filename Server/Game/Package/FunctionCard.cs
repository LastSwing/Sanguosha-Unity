using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public abstract class FunctionCard
    {
        protected bool target_fixed = false;
        protected bool will_throw = true;
        protected bool has_preact = false;
        protected bool votes = false;
        protected string card_name;
        protected HandlingMethod handling_method;
        protected CardType type_id;
        protected bool auto_use = true;     //默认当卡牌目标为target_fixed且子卡为空时，点击技能按钮就立刻使用。如护驾、酒诗等，以提高操作体验。
                                            //极略（使用制衡或获得完杀）等需要用户手动确认时，须将该项设为false以避免自动使用。

        public enum CardType
        {
            TypeUnknow, TypeSkill, TypeBasic, TypeTrick, TypeEquip
        };

        public enum HandlingMethod
        {
            MethodNone, MethodUse, MethodResponse, MethodDiscard, MethodRecast, MethodPindian, MethodGet
        };

        public string Name => card_name;
        public bool WillThrow => will_throw;
        public bool HasPreact => has_preact;
        public bool Votes => votes;
        public bool AutoUse => auto_use;
        public HandlingMethod Method => handling_method;
        public CardType TypeID => type_id;
        public FunctionCard(string name)
        {
            card_name = name;
        }

        public virtual bool IsKindOf(string cardType)
        {
            Type compare = Type.GetType(string.Format("{0}.{1}", GetType().Namespace, cardType));
            if (compare == null) return false;
            return Name == cardType || GetType().IsSubclassOf(compare);
        }

        public virtual bool IsNDTrick()
        {
            return type_id == CardType.TypeTrick && !(this is DelayedTrick);
        }

        public virtual bool TargetFixed(WrappedCard card) => target_fixed;

        public virtual bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (target_fixed)
                return true;
            else
                return targets.Count() != 0;
        }

        public virtual bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return !TargetFixed(card) && targets.Count() == 0 && to_select != Self && Engine.IsProhibited(room, Self, to_select, card, targets) == null;
        }

        public virtual bool ExtratargetFilter(Room room, List<Player> selected, Player to_select, Player Self, WrappedCard card)
        {
            return TargetFilter(room, selected, to_select, Self, card);
        }

        public virtual void DoPreAction(Room room, Player player, WrappedCard card)
        {
        }

        public virtual void CheckTargetModSkillShow(Room room, CardUseStruct card_use)
        {
        }

        public virtual void OnCardAnnounce(Room room, CardUseStruct use, bool ignore_rule)
        {
            use.Card = RoomLogic.ParseUseCard(room, use.Card).GetUsedCard();
            room.ShowSkill(use.From, use.Card.ShowSkill, use.Card.SkillPosition);

            if (!ignore_rule)
                CheckTargetModSkillShow(room, use);

            WrappedCard use_card = use.Card;

            //将卡牌转化为延时锦囊就相当于改写了该牌的牌名，必须对其重写以保证此延时锦囊将来可以正确生效
            if (Engine.GetFunctionCard(use_card.Name) is DelayedTrick && use_card.IsVirtualCard() && use_card.SubCards.Count == 1)
            {
                RoomCard wrapped = room.GetCard(use_card.GetEffectiveId());
                use_card.Id = wrapped.Id;
                wrapped.TakeOver(use_card);
                use.Card = wrapped.GetUsedCard();
            }

            //record big or small for fighttogether
            if (Engine.GetFunctionCard(use_card.Name) is FightTogether && use.To.Count > 0)
            {
                List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
                if (big_kingdoms.Count > 0)
                {
                    string target_kingdom = (use.To[0].HasShownOneGeneral() ?
                                          (use.To[0].GetRoleEnum() == PlayerRole.Careerist ? use.To[0].Name : use.To[0].Kingdom) : string.Empty);
                    bool big = big_kingdoms.Contains(target_kingdom);
                    if (big)
                        use.Pattern = "big";
                    else
                        use.Pattern = "small";
                }
                else
                    use.Pattern = "unknown";
            }

            OnUse(room, use);
        }

        public virtual void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            room.SortByActionOrder(ref card_use);

            bool hidden = (TypeID == CardType.TypeSkill && !WillThrow);
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            if (!TargetFixed(card_use.Card) || card_use.To.Count > 1 || !card_use.To.Contains(card_use.From))
                log.SetTos(card_use.To);


            List<int> used_cards = new List<int>(card_use.Card.SubCards);
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;

            if (TypeID != CardType.TypeSkill)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, player.Name, null, card_use.Card.Skill, null)
                {
                    Card = card_use.Card,
                    General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
                };
                if (card_use.To.Count == 1)
                    reason.TargetId = card_use.To[0].Name;

                if (used_cards.Count == 0)
                {
                    CardMoveReasonStruct virtual_reason = new CardMoveReasonStruct
                    {
                        Reason = reason.Reason,
                        PlayerId = reason.PlayerId,
                        TargetId = reason.TargetId,
                        SkillName = reason.SkillName,
                        CardString = RoomLogic.CardToString(room, card_use.Card),
                        General = reason.General
                    };
                    ClientCardsMoveStruct move = new ClientCardsMoveStruct(-1, player, Place.PlaceTable, virtual_reason)   //show virtual card on table
                    {
                        From_place = Place.PlaceUnknown,
                        From = player.Name,
                        Is_last_handcard = false,
                    };
                    room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, card_use.Card), move);
                }
                else
                {
                    room.RecordSubCards(card_use.Card);
                    foreach (int id in used_cards)
                    {
                        CardsMoveStruct move = new CardsMoveStruct(id, null, Place.PlaceTable, reason);
                        moves.Add(move);
                    }
                    room.MoveCardsAtomic(moves, true);
                }

                room.SendLog(log);
                if (this is Collateral)
                { // put it here for I don't wanna repeat these codes in Card::onUse
                    Player victim = room.FindPlayer((string)card_use.To[0].GetTag("collateralVictim"), true);
                    if (victim != null)
                    {
                        log = new LogMessage("#CollateralSlash")
                        {
                            From = card_use.From.Name,
                            To = new List<string> { victim.Name },
                        };
                        room.SendLog(log);
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, card_use.To[0].Name, victim.Name);
                    }
                }
            }
            else
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_THROW, card_use.From.Name, null,
                    card_use.Card.Skill, null)
                {
                    Card = card_use.Card,
                    General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
                };

                if (WillThrow && used_cards.Count > 0)
                {
                    room.RecordSubCards(card_use.Card);
                    room.MoveCardTo(card_use.Card, card_use.From, null, Place.PlaceTable, reason, true);
                }

                room.SendLog(log);

                if (WillThrow)
                {
                    List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card_use.Card));
                    if (table_cardids.Count > 0)
                    {
                        CardsMoveStruct move = new CardsMoveStruct(table_cardids, player, null, Place.PlaceTable, Place.DiscardPile, reason);
                        room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
                    }
                    room.RemoveSubCards(card_use.Card);
                }
            }
            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public virtual void OnNullified(Room room, Player player, WrappedCard trick)
        {
        }

        public virtual void Use(Room room, CardUseStruct card_use)
        {
            WrappedCard card = card_use.Card;
            List<Player> targets = card_use.To;
            for (int index = 0; index < targets.Count; index++)
            {
                Player target = targets[index];
                CardEffectStruct effect = new CardEffectStruct
                {
                    Card = card,
                    From = card_use.From,
                    To = target,
                    Multiple = (targets.Count > 1),
                    Drank = card_use.Drank,
                    ExDamage = card_use.ExDamage,
                    BasicEffect = card_use.EffectCount.Count > index ? card_use.EffectCount[index] : new CardBasicEffect(target, 0, 0, 0)
                };

                List<Player> players = new List<Player>();
                for (int i = index; i < targets.Count; i++)
                {
                    if (card_use.EffectCount.Count <= i || !card_use.EffectCount[i].Nullified)
                        players.Add(targets[i]);
                }
                effect.StackPlayers = players;

                room.CardEffect(effect);
            }

            List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card));
            if (table_cardids.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, card_use.From.Name, null, card.Skill, null)
                {
                    Card = card
                };
                if (targets.Count == 1) reason.TargetId = targets[0].Name;
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, card_use.From, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct>{ move }, true);
            }
        }

        public virtual void OnEffect(Room room, CardEffectStruct effect)
        {
        }

        public virtual void DoRecast(Room room, CardUseStruct use)
        {
            WrappedCard card = use.Card;
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_RECAST, use.From.Name)
            {
                SkillName = card.Skill
            };
            room.RecordSubCards(use.Card);
            room.MoveCardTo(card, null, Place.PlaceTable, reason, true);
            if (!string.IsNullOrEmpty(reason.SkillName))
                room.BroadcastSkillInvoke(use.From, use.Card);
            else
                room.BroadcastSkillInvoke("@recast", use.From.IsMale() ? "male" : "female", -1);

            if (!string.IsNullOrEmpty(card.Skill) && !card.Skill.StartsWith("-") && use.IsOwnerUse && RoomLogic.PlayerHasSkill(room, use.From, card.Skill))
                room.NotifySkillInvoked(use.From, card.Skill);

            LogMessage log = new LogMessage("#Card_Recast")
            {
                From = use.From.Name,
                Card_str = RoomLogic.CardToString(room, card)
            };
            room.SendLog(log);

            List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card));
            if (table_cardids.Count > 0)
            {
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, use.From, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct>() { move }, true);
                room.RemoveSubCards(use.Card);
            }

            room.DrawCards(use.From, 1, "recast");
        }

        public virtual string GetCommonEffectName() => null;
        public virtual bool IsAvailable(Room room, Player player, WrappedCard card) => !RoomLogic.IsCardLimited(room, player, card, handling_method)
                || CanRecast(room, player, card);
        public bool CanRecast(Room room, Player player, WrappedCard card)
        {
            bool rec = (card.CanRecast && room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY
                    && !RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodRecast));
            if (!rec) return false;

            List<int> hand_cards = player.GetCards("h");

            if (card.SubCards.Count == 0) return false;

            foreach (int id in card.SubCards)
            {
                if (!hand_cards.Contains(id))
                {
                    rec = false;
                    break;
                }
            }

            return rec;
        }
        public abstract string GetSubtype();
        public virtual WrappedCard Validate(Room room, CardUseStruct use)
        {
            return use.Card;
        }
        public virtual WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            return card;
        }
        public virtual bool IsCancelable(Room room, CardEffectStruct effect) => effect.Card != null && effect.Card.Cancelable;

        public virtual CardBasicEffect FillCardBasicEffct(Room room, Player to)
        {
            return new CardBasicEffect(to, 0, 0, 0);
        }
    }
    public abstract class BasicCard : FunctionCard
    {

        public BasicCard(string name) : base(name)
        {
            handling_method = HandlingMethod.MethodUse;
            type_id = CardType.TypeBasic;
        }
    }

    public abstract class TrickCard : FunctionCard
    {
        public TrickCard(string name) : base(name)
        {
            type_id = CardType.TypeTrick;
            handling_method = HandlingMethod.MethodUse;
        }
        public abstract override string GetSubtype();
    }

    public class GlobalEffect : TrickCard
    {

        public GlobalEffect(string name) : base(name)
        {
            target_fixed = true;
        }
        public override string GetSubtype() => "global_effect";

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            List<Player> targets = new List<Player>();
            if (card_use.To.Count == 0)
            {
                List<Player> all_players = room.GetAllPlayers();
                foreach (Player player in all_players) {
                    Skill skill = RoomLogic.IsProhibited(room, source, player, card_use.Card);
                    if (skill != null)
                    {
                        skill = Engine.GetMainSkill(skill.Name);
                        LogMessage log = new LogMessage
                        {
                            Type = "#SkillAvoid",
                            From = player.Name,
                            Arg = skill.Name,
                            Arg2 = Name
                        };
                        room.SendLog(log);
                        if (RoomLogic.PlayerHasShownSkill(room, player, skill))
                        {
                            room.NotifySkillInvoked(player, skill.Name);
                            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, skill.Name);
                            string genral = gsk.General;
                            int skin_id = gsk.SkinId;
                            string skill_name = skill.Name;
                            int audio = -1;
                            skill.GetEffectIndex(room, player, card_use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                            if (audio >= -1)
                                room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                        }
                        else
                        {
                            int audio = -1;
                            string skill_name = skill.Name;
                            string genral = string.Empty;
                            int skin_id = 0;
                            skill.GetEffectIndex(room, null, card_use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                        }
                    }
                    else
                    {
                        targets.Add(player);
                    }
                }
            }
            else
            {
                targets = card_use.To;
            }

            card_use.To = targets;

            base.OnUse(room, card_use);
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool canUse = false;
            List<Player> players = room.GetAlivePlayers();
            foreach (Player p in players) {
                if (RoomLogic.IsProhibited(room, player, p, card) != null)
                    continue;

                canUse = true;
                break;
            }

            return canUse && base.IsAvailable(room, player, card);
        }
    }

    public class AOE : TrickCard
    {

        public AOE(string name) : base(name)
        {
            target_fixed = true;
        }
        public override string GetSubtype() => "aoe";
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool canUse = false;
            List<Player> players = room.GetOtherPlayers(player);
            foreach (Player p in players)
            {
                if (RoomLogic.IsProhibited(room, player, p, card) != null)
                    continue;

                canUse = true;
                break;
            }

            return canUse && base.IsAvailable(room, player, card);
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            List<Player> targets = new List<Player>();
            if (card_use.To.Count == 0)
            {
                List<Player> all_players = room.GetOtherPlayers(card_use.From);
                foreach (Player player in all_players)
                {
                    Skill skill = RoomLogic.IsProhibited(room, source, player, card_use.Card);
                    if (skill != null)
                    {
                        skill = Engine.GetMainSkill(skill.Name);
                        LogMessage log = new LogMessage
                        {
                            Type = "#SkillAvoid",
                            From = player.Name,
                            Arg = skill.Name,
                            Arg2 = Name
                        };
                        room.SendLog(log);
                        if (RoomLogic.PlayerHasShownSkill(room, player, skill))
                        {
                            room.NotifySkillInvoked(player, skill.Name);
                            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, skill.Name);
                            string genral = gsk.General;
                            int skin_id = gsk.SkinId;
                            string skill_name = skill.Name;
                            int audio = -1;
                            skill.GetEffectIndex(room, player, card_use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                            if (audio >= -1)
                                room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                        }
                        else
                        {
                            int audio = -1;
                            string skill_name = skill.Name;
                            string genral = string.Empty;
                            int skin_id = 0;
                            skill.GetEffectIndex(room, null, card_use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                        }
                    }
                    else
                    {
                        targets.Add(player);
                    }
                }
            }
            else
            {
                targets = card_use.To;
            }

            card_use.To = targets;

            base.OnUse(room, card_use);
        }
    }

    public class SingleTargetTrick : TrickCard
    {

        public SingleTargetTrick(string name) : base(name)
        {
        }
        public override string GetSubtype() => "single_target_trick";
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return !TargetFixed(card) && Engine.IsProhibited(room, Self, to_select, card, targets) == null;
        }
    }

    public abstract class DelayedTrick : TrickCard
    {
        protected JudgeStruct judge = new JudgeStruct();
        private readonly bool movable;
        public DelayedTrick(string name, bool movable = false) : base(name)
        {
            this.movable = movable;
            judge.Negative = true;
        }
        
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return !RoomLogic.PlayerContainsTrick(room, to_select, Name) && base.TargetFilter(room, targets, to_select, Self, card) && to_select.JudgingAreaAvailable;
        }

        public override void OnNullified(Room room, Player target, WrappedCard card)
        {
            RoomThread thread = room.RoomThread;

            string card_str = RoomLogic.CardToString(room, card);
            bool move = false;
            Player p = null;
            if (movable)
            {
                List<Player> players = new List<Player>();
                List<Player> count_players = new List<Player>(room.Players);
                Player starter = target;
                int index = count_players.IndexOf(starter);
                for (int i = index + 1; i < count_players.Count; i++)
                {
                    if (count_players[i].Alive)
                        players.Add(count_players[i]);
                }

                for (int i = 0; i <= index; i++)
                {
                    if (count_players[i].Alive)
                        players.Add(count_players[i]);
                }

                foreach (Player player in players)
                {
                    if (RoomLogic.PlayerContainsTrick(room, player, Name) || !player.JudgingAreaAvailable)
                        continue;

                    Skill skill = RoomLogic.IsProhibited(room, null, player, card);
                    if (skill != null)
                    {
                        skill = Engine.GetMainSkill(skill.Name);
                        LogMessage log = new LogMessage
                        {
                            Type = "#SkillAvoid",
                            From = player.Name,
                            Arg = skill.Name,
                            Arg2 = Name
                        };
                        room.SendLog(log);
                        if (RoomLogic.PlayerHasShownSkill(room, player, skill))
                        {
                            room.NotifySkillInvoked(player, skill.Name);
                            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, skill.Name);
                            string genral = gsk.General;
                            int skin_id = gsk.SkinId;
                            string skill_name = skill.Name;
                            int audio = -1;
                            skill.GetEffectIndex(room, player, card, ref audio, ref skill_name, ref genral, ref skin_id);
                            if (audio >= -1)
                                room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                        }
                        continue;
                    }
                    //if (player.HasFlag(card_str + "_delay_trick_cancel")) continue;
                    /*
                    CardUseStruct use = new CardUseStruct(room.GetCard(card.GetEffectiveId()), null, player)
                    {
                        EffectCount = new List<CardBasicEffect> { FillCardBasicEffct(room, player) }
                    };
                    object data = use;
                    thread.Trigger(TriggerEvent.TargetConfirming, room, player, ref data);
                    CardUseStruct new_use = (CardUseStruct)data;
                    if (new_use.To.Count == 0)
                    {
                        p = player;
                        player.SetFlags(card_str + "_delay_trick_cancel");
                        break;
                    }

                    thread.Trigger(TriggerEvent.TargetChosen, room, null, ref data);
                    thread.Trigger(TriggerEvent.TargetConfirmed, room, player, ref data);
                    */
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_TRANSFER, string.Empty)
                    {
                        Card = card
                    };
                    room.MoveCardTo(card, null, player, Place.PlaceDelayedTrick, reason, true);
                    move = true;

                    break;
                }
            }

            if (p != null)
                OnNullified(room, p, card);
            else if (!move)
            {
                //foreach (Player player in room.GetAllPlayers())
                    //if (player.HasFlag(RoomLogic.CardToString(room, card) + "_delay_trick_cancel"))
                        //player.SetFlags(string.Format("-{0}{1}", card_str, "_delay_trick_cancel"));

                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, string.Empty);
                room.MoveCardTo(card, null, Place.DiscardPile, reason, true);
            }
        }

        public override void OnUse(Room room, CardUseStruct use)
        {
            object data = use;
            RoomThread thread = room.RoomThread;
            thread.Trigger(TriggerEvent.PreCardUsed, room, use.From, ref data);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, use.From.Name, use.To[0].Name, use.Card.Skill, null)
            {
                Card = use.Card,
                General = RoomLogic.GetGeneralSkin(room, use.From, use.Card.Skill, use.Card.SkillPosition)
            };
            room.RecordSubCards(use.Card);
            room.MoveCardTo(use.Card, use.From, Place.PlaceTable, reason, true);

            LogMessage log = new LogMessage
            {
                From = use.From.Name,
                To = new List<string>(),
                Type = "#DelayedTrick",
                Card_str = RoomLogic.CardToString(room, use.Card)
            };
            foreach (Player to in use.To)
                log.To.Add(to.Name);
            room.SendLog(log);

            Thread.Sleep(300);
            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, use.From, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, use.From, ref data);
            thread.Trigger(TriggerEvent.CardUsed, room, use.From, ref data);
            thread.Trigger(TriggerEvent.CardFinished, room, use.From, ref data);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> targets = card_use.To;
            string str = RoomLogic.CardToString(room, card_use.Card);
            if (targets.Count == 0 || RoomLogic.PlayerContainsTrick(room, targets[0], Name))
            {
                /*
                if (movable)
                {
                    OnNullified(room, card_use.From, card_use.Card);
                    if (room.GetCardOwner(card_use.Card.GetEffectiveId()) != card_use.From) return;
                }
                */
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, card_use.From.Name, null, card_use.Card.Skill, null)
                {
                    Card = card_use.Card
                };
                room.MoveCardTo(card_use.Card, null, Place.DiscardPile, reason, true);
                return;
            }

            if (room.GetCardIdsOnTable(room.GetSubCards(card_use.Card)).Count == 0) return;

            CardMoveReason reason2 = new CardMoveReason(MoveReason.S_REASON_USE, card_use.From.Name, targets[0].Name, card_use.Card.Skill, null)
            {
                Card = card_use.Card
            };
            room.MoveCardTo(card_use.Card, null, targets[0], Place.PlaceDelayedTrick, reason2, true);
        }
        public override string GetSubtype() => "delayed_trick";
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            WrappedCard card = effect.Card;
            if (room.GetCardIdsOnTable(card).Count == 0) return;

            LogMessage log = new LogMessage
            {
                From = effect.To.Name,
                Type = "#DelayedTrick",
                Arg = card.Name
            };
            room.SendLog(log);

            JudgeStruct judge_struct = new JudgeStruct
            {
                Pattern = judge.Pattern,
                Good = judge.Good,
                Negative = judge.Negative,
                Reason = judge.Reason,
                PlayAnimation = judge.PlayAnimation,
                Time_consuming = judge.Time_consuming,
                Who = effect.To
            };
            room.Judge(ref judge_struct);
            if (judge_struct.IsEffected())
            {
                TakeEffect(room, effect.To, card);
                List<int> table_cardids = room.GetCardIdsOnTable(card);
                if (table_cardids.Count > 0)
                {
                    //DummyCard dummy(table_cardids);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, null);
                    //room->moveCardTo(&dummy, NULL, Player::DiscardPile, reason, true);
                    CardsMoveStruct move = new CardsMoveStruct(table_cardids, null, Place.DiscardPile, reason);
                    room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
                }
            }
            else if (movable)
            {
                OnNullified(room, effect.To, card);
            }
            else
            {
                List<int> table_cardids = room.GetCardIdsOnTable(card);
                if (table_cardids.Count > 0)
                {
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, null);
                    CardsMoveStruct move = new CardsMoveStruct(table_cardids, null, Place.DiscardPile, reason);
                    room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
                }
            }
        }
        public abstract void TakeEffect(Room room, Player target, WrappedCard card);
    }

    public abstract class EquipCard : FunctionCard
    {
        public enum Location
        {
            WeaponLocation,
            ArmorLocation,
            DefensiveHorseLocation,
            OffensiveHorseLocation,
            TreasureLocation,
            SpecialLocation
        };
        public EquipCard(string name) : base(name)
        {
            type_id = CardType.TypeEquip;
            handling_method = HandlingMethod.MethodUse;
            target_fixed = true;
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            if (room.GetCardPlace(card.Id) == Place.PlaceEquip) return false;
            return RoomLogic.IsProhibited(room, player, player, card) == null
                && RoomLogic.CanPutEquip(player, card) && base.IsAvailable(room, player, card);
        }
        public override void OnUse(Room room, CardUseStruct use)
        {
            Player player = use.From;
            if (use.To.Count == 0)
                use.To.Add(player);

            object data = use;
            RoomThread thread = room.RoomThread;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, player.Name)
            {
                Card = use.Card
            };
            room.RecordSubCards(use.Card);
            room.MoveCardTo(use.Card, player, Place.PlaceTable, reason, true);
            Thread.Sleep(300);
            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, use.From, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, use.From, ref data);
            thread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            thread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            WrappedCard card = card_use.Card;
            if (room.GetCardIdsOnTable(room.GetSubCards(card)).Count == 0) return;

            if (card_use.To.Count == 0 || !card_use.To[0].Alive || card_use.To[0].EquipIsBaned((int)EquipLocation()))
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, card_use.From.Name, null, card.Skill, null)
                {
                    Card = card_use.Card
                };
                room.MoveCardTo(card, room.GetCardOwner(card.GetEffectiveId()), null, Place.DiscardPile, reason, true);
                return;
            }

            int equipped_id = -1;
            Player target = card_use.To[0];
            if (target.HasEquip((int)EquipLocation()))
                equipped_id = target.GetEquip((int)EquipLocation());

            List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct> { };
            CardsMoveStruct move1 = new CardsMoveStruct(card.GetEffectiveId(), target, Place.PlaceEquip,
                new CardMoveReason(MoveReason.S_REASON_USE, target.Name));
            move1.Reason.Card = card_use.Card;
            exchangeMove.Add(move1);
            if (equipped_id != -1)
            {
                CardsMoveStruct move2 = new CardsMoveStruct(equipped_id, target, Place.PlaceTable,
                    new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, target.Name));
                exchangeMove.Add(move2);
            }
            room.MoveCardsAtomic(exchangeMove, true);

            LogMessage log = new LogMessage
            {
                From = target.Name,
                Type = "$Install",
                Card_str = card.GetEffectiveId().ToString()
            };
            room.SendLog(log);

            if (equipped_id != -1)
            {
                if (room.GetCardPlace(equipped_id) == Place.PlaceTable)
                {
                    CardsMoveStruct move3 = new CardsMoveStruct(equipped_id, null, Place.DiscardPile,
                        new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, target.Name));
                    room.MoveCardsAtomic(new List<CardsMoveStruct> { move3 }, true);
                }
            }
        }

        public virtual void OnInstall(Room room, Player player, WrappedCard card)
        {
            Skill skill = Engine.GetSkill(Name);
            if (skill != null)
            {
                if (skill is ViewAsSkill)
                {
                    //room.AttachSkillToPlayer(player, Name);
                    player.AcquireSkill(Name);
                }
                else if (skill is TriggerSkill trigger_skill)
                {
                    if (trigger_skill.ViewAsSkill != null)
                    {
                        //room.AttachSkillToPlayer(player, Name);
                        player.AcquireSkill(Name);
                    }
                }
            }
        }
        public virtual void OnUninstall(Room room, Player player, WrappedCard card)
        {
            Skill skill = Engine.GetSkill(Name);
            if (skill != null)
            {
                if (skill is ViewAsSkill)
                {
                    //room.DetachSkillFromPlayer(player, Name, true);
                    player.DetachSkill(Name, true);
                }
                else if (skill is TriggerSkill trigger_skill)
                {
                    if (trigger_skill.ViewAsSkill != null)
                    {
                        //room.DetachSkillFromPlayer(player, Name, true);
                        player.DetachSkill(Name, true);
                    }
                }
            }
        }

        public abstract Location EquipLocation();
    }

    public abstract class Weapon : EquipCard
    {
        public Weapon(string name, int range) : base(name)
        {
            this.range = range;
        }

        public int Range => range;
        public override string GetSubtype() => "weapon";
        public override Location EquipLocation() => Location.WeaponLocation;
        public override string GetCommonEffectName() => "weapon";

        protected int range;
    }

    public class Armor : EquipCard
    {
        public Armor(string name) : base(name)
        {
        }
        public override string GetSubtype() => "armor";
        public override Location EquipLocation() => Location.ArmorLocation;
        public override string GetCommonEffectName() => "armor";
    }

    public class Horse : EquipCard
    {
        public int Correct { get; protected set; }

        public Horse(string name, int correct) : base(name)
        {
            Correct = correct;
        }
        public override Location EquipLocation()
        {
            if (Correct > 0)
                return Location.DefensiveHorseLocation;
            else
                return Location.OffensiveHorseLocation;
        }
        public override string GetSubtype() => "horse";
        public override string GetCommonEffectName() => "horse";
    }
    public class OffensiveHorse : Horse
    {
        public OffensiveHorse(string name, int correct = -1) : base(name, correct)
        {
        }
        public override string GetSubtype() => "offensive_horse";
    }
    public class DefensiveHorse : Horse
    {

        public DefensiveHorse(string name, int correct = +1) : base(name, correct)
        { }
        public override string GetSubtype() => "defensive_horse";
    }

    public class Treasure : EquipCard
    {
        public Treasure(string name) : base(name)
        {
        }
        public override string GetSubtype() => "treasure";
        public override Location EquipLocation() => Location.TreasureLocation;
        public override string GetCommonEffectName() => "treasure";
    }

    public class SpecialEquip : EquipCard
    {
        public SpecialEquip(string name) : base(name)
        {
        }

        public override string GetSubtype() => "special";
        public override Location EquipLocation() => Location.SpecialLocation;
        public override string GetCommonEffectName() => "horse";
    }


    public class SkillCard : FunctionCard
    {
        public SkillCard(string name) : base(name)
        {
            type_id = CardType.TypeSkill;
            if (will_throw) handling_method = HandlingMethod.MethodDiscard;
        }

        public override string GetSubtype() => "skill_card";
    }

    public class ArraySummonCard:SkillCard
    {
        public ArraySummonCard(string name) : base(name)
        {
            target_fixed = true;
            handling_method = HandlingMethod.MethodNone;
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            BattleArraySkill skill = (BattleArraySkill)Engine.GetTriggerSkill(Name);
            if (skill != null)
            {
                room.ShowSkill(use.From, Name, use.Card.SkillPosition);
                skill.SummonFriends(room, use.From);
            }
            return null;
        }
    }
}
