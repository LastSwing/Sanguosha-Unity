using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class Traitor : GeneralPackage
    {
        public Traitor() : base("Traitro")
        {
            skills = new List<Skill>
            {
                new Qiuan(),
                new Liangfan(),
            };
            skill_cards = new List<FunctionCard>
            {
            };
            related_skills = new Dictionary<string, List<string>>
            {
            };
        }
    }

    public class Qiuan : TriggerSkill
    {
        public Qiuan() : base("qiuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted, TriggerEvent.EventLoseSkill };
            skill_type = SkillType.Defense;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct info && info.Info == Name && player.GetPile(Name).Count > 0)
                room.ClearOnePrivatePile(player, Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct damage && damage.Card != null && base.Triggerable(player, room) && player.GetPile(Name).Count == 0)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (!(fcard is SkillCard))
                {
                    WrappedCard card = damage.Card;
                    List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card));
                    List<int> ids = new List<int>(card.SubCards);

                    if (table_cardids.Count > 0 && ids.SequenceEqual(table_cardids))
                    {
                        bool check = true;
                        foreach (int id in card.SubCards)
                        {
                            if (room.GetCardPlace(id) != Place.PlaceTable)
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check) return new TriggerStruct(Name, player);
                    }
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
                room.AddToPile(player, Name, damage.Card);

            return true;
        }
    }

    public class Liangfan : TriggerSkill
    {
        public Liangfan() : base("liangfan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Damage, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetPile("qiuan").Count > 0)
            {
                List<int> ids = player.GetPile("qiuan");
                player.SetTag(Name, ids);
                CardsMoveStruct move = new CardsMoveStruct(ids, player, Place.PlaceHand, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name));
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, false);

                room.LoseHp(player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.To.Alive && !damage.To.IsNude() && base.Triggerable(player, room)
                && player.ContainsTag(Name) && player.GetTag(Name) is List<int> ids && RoomLogic.CanGetCard(room, player, damage.To, "he") && damage.Card.SubCards.Count == 1
                && ids.Contains(damage.Card.SubCards[0]))
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (!(fcard is SkillCard))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && room.AskForSkillInvoke(zhurong, Name, damage.To, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, zhurong.Name, damage.To.Name);
                room.BroadcastSkillInvoke(Name, zhurong, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                int card_id = room.AskForCardChosen(zhurong, damage.To, "he", Name, false, HandlingMethod.MethodGet);
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, zhurong.Name);
                room.ObtainCard(zhurong, room.GetCard(card_id), reason, room.GetCardPlace(card_id) != Place.PlaceHand);
            }
            return false;
        }
    }


    public class XingzhaoHegemony : TriggerSkill
    {
        public XingzhaoHegemony() : base("xingzhao_hegemony")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.EventPhaseChanging, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && base.Triggerable(player, room) && damage.From != null &&
                damage.From.Alive && damage.From.HandcardNum != player.HandcardNum)
            {
                int count = 0;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.IsWounded()) count++;

                if (count >= 2)
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.Discard && base.Triggerable(player, room))
            {
                int count = 0;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.IsWounded()) count++;

                if (count >= 3)
                    return new TriggerStruct(Name, player);
            }
            else if (data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceEquip))
            {
                int count = 0;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.IsWounded()) count++;

                if (count >= 4)
                    return new TriggerStruct(Name, move.From);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, ask_who, Name))
            {
                return info;
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage)
            {
                Player target = damage.To.HandcardNum < ask_who.HandcardNum ? damage.To : null;
                if (room.AskForSkillInvoke(ask_who, Name, target, info.SkillPosition))
                {
                    if (target != null)
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, target.Name);
                    return info;
                }
            }
            else if (room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition))
                return info;

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            switch (triggerEvent)
            {
                case TriggerEvent.EventPhaseChanging:
                    room.SkipPhase(player, PlayerPhase.Discard, true);
                    break;
                case TriggerEvent.Damaged when data is DamageStruct damage:
                    Player target = damage.To.HandcardNum < ask_who.HandcardNum ? damage.To : null;
                    if (target != null)
                        room.DrawCards(target, new DrawCardStruct(1, ask_who, Name));
                    else
                        room.DrawCards(ask_who, 1, Name);
                    break;
                case TriggerEvent.CardsMoveOneTime:
                    room.DrawCards(ask_who, 1, Name);
                    break;
            }

            return false;
        }
    }

    public class XingzhaoVHH : ViewHasSkill
    {
        public XingzhaoVHH() : base("#xingzhao_hegemony") { viewhas_skills = new List<string> { "xunxun" }; }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, "xingzhao_hegemony"))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.IsWounded()) return true;
            }

            return false;
        }
    }

    public class BushiHegemony : TriggerSkill
    {
        public BushiHegemony() : base("bushi_hegemony")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.Damaged };
            skill_type = SkillType.Replenish;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && ((triggerEvent == TriggerEvent.Damage && damage.To.Alive) || triggerEvent == TriggerEvent.Damaged))
            { 
                TriggerStruct trigger = new TriggerStruct(Name, player);
                trigger.Times = damage.Damage;
                return trigger;
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                bool must = triggerEvent == TriggerEvent.Damage && RoomLogic.PlayerHasShownSkill(room, player, Name);
                List<Player> targets = new List<Player>();
                Player from = triggerEvent == TriggerEvent.Damage ? damage.To : player;
                foreach (Player p in room.GetAlivePlayers())
                    if (RoomLogic.IsFriendWith(room, from, p)) targets.Add(p);
                if (targets.Count > 1)
                {
                    string prompt = triggerEvent == TriggerEvent.Damage ? "@bushi-self" : "@bushi-let:" + damage.To.Name;
                    Player target = room.AskForPlayerChosen(player, targets, Name, prompt, !must, true, info.SkillPosition);
                    if (target != null)
                    {
                        room.SetTag(Name, target);
                        return info;
                    }
                }
                else
                {
                    bool invoke = true;
                    if (!must)
                        invoke = room.AskForSkillInvoke(player, Name, from, info.Invoker);
                    if (invoke)
                    {
                        room.SetTag(Name, from);
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, from.Name);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.GetTag(Name) is Player target)
            {
                room.RemoveTag(Name);
                room.DrawCards(target, new DrawCardStruct(1, player, Name));
            }
            return false;
        }
    }

    public class MidaoHegemony : TriggerSkill
    {
        public MidaoHegemony() : base("midao_hegemony")
        {
            skill_type = SkillType.Wizzard;
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && player != null && player.Alive && !use.Card.IsVirtualCard())
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || fcard.IsNDTrick())
                {
                    Player zl = RoomLogic.FindPlayerBySkillName(room, Name);
                    if (RoomLogic.IsFriendWith(room, player, zl) && RoomLogic.PlayerHasShownSkill(room, zl, Name))
                        return new TriggerStruct(Name, player, zl);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player zl = room.FindPlayer(info.SkillOwner);
            List<int> ids = room.AskForExchange(player, Name, 1, 0, "@midao-change:" + zl.Name, string.Empty, ".", null);
            if (ids.Count > 0)
            {
                room.ObtainCard(zl, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, info.SkillOwner, string.Empty), false);
                room.BroadcastSkillInvoke(Name, zl);
                room.NotifySkillInvoked(zl, Name);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit> { WrappedCard.CardSuit.Club, WrappedCard.CardSuit.Diamond, WrappedCard.CardSuit.Heart, WrappedCard.CardSuit.Spade };
                suits.Remove(use.Card.Suit);

                List<string> choices = new List<string>();
                foreach (WrappedCard.CardSuit suit in suits)
                    choices.Add(WrappedCard.GetSuitString(suit));
                choices.Add("cancel");
                Player zl = room.FindPlayer(info.SkillOwner);
                string choice = room.AskForChoice(zl, Name, string.Join("+", choices), new List<string> { string.Format("@midao-suit:{0}::{1}", player.Name, use.Card.Name) }, data);
                if (choice != "cancel")
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "$ChangeSuit",
                        From = player.Name,
                        Arg = use.Card.Name,
                        Arg2 = choice
                    };
                    log.Card_str = RoomLogic.CardToString(room, use.Card);
                    room.SendLog(log);

                    WrappedCard.CardSuit suit = WrappedCard.GetSuit(choice);
                    use.Card.SetSuit(suit);
                    use.Card.Modified = true;
                    RoomCard card = room.GetCard(use.Card.GetEffectiveId());
                    card.SetSuit(suit);
                    card.Modified = true;

                }

                if (use.Card.Name.Contains(Slash.ClassName))
                {
                    List<string> slashes = new List<string> { Slash.ClassName, FireSlash.ClassName, ThunderSlash.ClassName };
                    slashes.Remove(use.Card.Name);
                    slashes.Add("cancel");
                    choice = room.AskForChoice(zl, Name, string.Join("+", slashes), new List<string> { string.Format("@midao-slash:{0}::{1}", player.Name, use.Card.Name) }, data);

                    LogMessage log = new LogMessage
                    {
                        Type = "$ChangeSlash",
                        From = player.Name,
                        Arg = use.Card.Name,
                        Arg2 = choice
                    };
                    log.Card_str = RoomLogic.CardToString(room, use.Card);
                    room.SendLog(log);

                    use.Card.ChangeName(choice);
                    room.GetCard(use.Card.GetEffectiveId()).ChangeName(choice);
                }
            }

            return false;
        }
    }

    public class FengshiMB : TriggerSkill
    {
        public FengshiMB() : base("fengshi_mb")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirmed, TriggerEvent.TargetChosen };
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.To.Count == 1 && (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == SavageAssault.ClassName || use.Card.Name == ArcheryAttack.ClassName
                || use.Card.Name == Duel.ClassName || use.Card.Name == Drowning.ClassName || use.Card.Name == FireAttack.ClassName || use.Card.Name == BurningCamps.ClassName)
                && !use.From.IsNude() && !use.To[0].IsNude())
            {
                if (triggerEvent == TriggerEvent.TargetChosen && base.Triggerable(player, room))
                    return new TriggerStruct(Name, player);
                else if (triggerEvent == TriggerEvent.TargetConfirmed && RoomLogic.PlayerHasShownSkill(room, use.To[0], Name))
                    return new TriggerStruct(Name, player, use.To[0]);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                if (triggerEvent == TriggerEvent.TargetChosen && room.AskForDiscard(player, Name, 1, 1, true, true,
                    string.Format("@fengshi-mb:{0}::{1}", use.To[0], use.Card.Name), true, info.SkillPosition))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    return info;
                }
                else if (triggerEvent == TriggerEvent.TargetConfirmed && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, use.To[0], Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                if (triggerEvent == TriggerEvent.TargetChosen && player.Alive && use.To[0].Alive && !use.To[0].IsNude() && RoomLogic.CanDiscard(room, player, use.To[0], "he"))
                {
                    int card_id = room.AskForCardChosen(player, use.To[0], "he", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(card_id, use.To[0], player);
                }
                else if (triggerEvent == TriggerEvent.TargetConfirmed && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                {
                    room.AskForDiscard(use.To[0], Name, 1, 1, false, true,
                    string.Format("@fengshi-mb2:{0}::{1}", player.Name, use.Card.Name), true, info.SkillPosition);
                    if (!player.IsNude() && RoomLogic.CanDiscard(room, use.To[0], player, "he"))
                    {
                        int card_id = room.AskForCardChosen(use.To[0], player, "he", Name, false, HandlingMethod.MethodDiscard);
                        room.ThrowCard(card_id, player, use.To[0]);
                    }
                }
                LogMessage log = new LogMessage
                {
                    Type = "#fengshi-add",
                    From = player.Name,
                    Arg = use.Card.Name
                };
                room.SendLog(log);

                use.ExDamage += 1;
                data = use;
            }
            return false;
        }
    }
}