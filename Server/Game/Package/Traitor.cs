using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
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

                new XingzhaoHegemony(),
                new XingzhaoVHH(),

                new BushiHegemony(),
                new MidaoHegemony(),

                new FengshiMF(),

                new WenjiHegemony(),
                new WenjiEffectHegemony(),
                new WenjiTar(),
                new TunjiangHegemony(),

                new BiluanHegemony(),
                new LixiaHegemony(),
            };
            skill_cards = new List<FunctionCard>
            {
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "xingzhao_hegemony", new List<string>{ "#xingzhao_hegemony" } },
                { "wenji_hegemony", new List<string>{ "#wenji_hegemony", "#wenji-tar" } },
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
            if (data is CardUseStruct use && player != null && player.Alive && !use.Card.IsVirtualCard() && player.Phase == PlayerPhase.Play && !player.HasFlag(Name))
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
            if (data is CardUseStruct use)
            {
                Player zl = room.FindPlayer(info.SkillOwner);
                List<int> ids = room.AskForExchange(player, Name, 1, 0, string.Format("@midao-change:{0}::{1}", zl.Name, use.Card.Name), string.Empty, ".", null);
                if (ids.Count > 0)
                {
                    player.SetFlags(Name);
                    room.ObtainCard(zl, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, info.SkillOwner, string.Empty), false);
                    room.BroadcastSkillInvoke(Name, zl);
                    room.NotifySkillInvoked(zl, Name);
                    return info;
                }
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

    public class FengshiMF : TriggerSkill
    {
        public FengshiMF() : base("fengshi_mf")
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
                    string.Format("@fengshi-mf:{0}::{1}", use.To[0], use.Card.Name), true, info.SkillPosition))
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
                    string.Format("@fengshi-mf2:{0}", player.Name), true, info.SkillPosition);
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

    public class WenjiHegemony : TriggerSkill
    {
        public WenjiHegemony() : base("wenji_hegemony")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (!p.IsNude()) targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@wenji-ask", true, true, info.SkillPosition);
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);

            List<int> ids = target.GetCards("he");
            if (ids.Count > 1)
                ids = room.AskForExchange(target, Name, 1, 1, "@wenji:" + player.Name, string.Empty, "..", string.Empty);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, null);
            room.ObtainCard(player, ref ids, reason, true);

            if (!target.HasShownOneGeneral() || RoomLogic.IsFriendWith(room, player, target))
            {
                List<int> names = player.ContainsTag(Name) ? (List<int>)player.GetTag(Name) : new List<int>();
                names.AddRange(ids);
                player.SetTag(Name, names);
            }
            else if (player.Alive && target.Alive)
            {
                List<int> give = player.GetCards("he");
                give.RemoveAll(t => ids.Contains(t));
                if (give.Count > 0)
                {
                    ids = room.AskForExchange(player, Name, 1, 1, "@wenji-give:" + target.Name, string.Empty, string.Format("^{0}|.|.|.", ids[0]), info.SkillPosition);
                    reason = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, null);
                    room.ObtainCard(target, ref ids, reason, true);
                }
            }
            return false;
        }
    }
    public class WenjiEffectHegemony : TriggerSkill
    {
        public WenjiEffectHegemony() : base("#wenji_hegemony")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.TrickCardCanceling };
            frequency = Frequency.Compulsory;
        }


        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && use.To.Count > 0 && player.ContainsTag("wenji_hegemony")
                && player.GetTag("wenji_hegemony") is List<int> names && use.Card.SubCards.Count == 1 && !use.Card.IsVirtualCard()
                && names.Contains(use.Card.GetEffectiveId()))
            {
                WrappedCard origin = Engine.GetRealCard(use.Card.Id);
                if (origin.Equals(use.Card))
                    return new TriggerStruct(Name, player, use.To);
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && player != effect.From && effect.From != null && effect.From.Alive
                && effect.From.ContainsTag("wenji_hegemony") && effect.From.GetTag("wenji_hegemony") is List<int> _names 
                && effect.Card.SubCards.Count == 1 && !effect.Card.IsVirtualCard() && _names.Contains(effect.Card.GetEffectiveId()))
            {
                WrappedCard origin = Engine.GetRealCard(effect.Card.Id);
                if (origin.Equals(effect.Card))
                    return new TriggerStruct(Name, effect.From);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct chose_use)
            {
                int index = 0;
                for (int i = 0; i < chose_use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = chose_use.EffectCount[i];
                    if (effect.To == player)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            effect.Effect2 = 0;
                            data = chose_use;
                            break;
                        }
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling)
                return true;

            return false;
        }
    }

    public class WenjiTar : TargetModSkill
    {
        public WenjiTar() : base("#wenji-tar", false)
        {
            pattern = ".";
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (from.ContainsTag("wenji_hegemony") && from.GetTag("wenji_hegemony") is List<int> names && card.SubCards.Count == 1 && !card.IsVirtualCard()
                  && names.Contains(card.GetEffectiveId()))
                return true;

            return false;
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
        {
            if (from.ContainsTag("wenji_hegemony") && from.GetTag("wenji_hegemony") is List<int> names && card.SubCards.Count == 1 && !card.IsVirtualCard()
                && names.Contains(card.GetEffectiveId()))
                return true;

            return false;
        }
    }

    public class TunjiangHegemony : TriggerSkill
    {
        public TunjiangHegemony() : base("tunjiang_hegemony")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardTargetAnnounced };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && player.Phase == PlayerPhase.Play && data is CardUseStruct use && use.To.Count > 0 && !player.HasFlag("tunjian_fail"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                {
                    foreach (Player p in use.To)
                    {
                        if (p != player)
                        {
                            player.SetFlags("tunjian_fail");
                            break;
                        }
                    }

                    if (!player.HasFlag("tunjian_fail")) player.SetFlags(Name);
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && player.HasFlag(Name))
            {
                bool check = true;
                foreach (PhaseStruct phase in player.PhasesState)
                {
                    if (phase.Phase == PlayerPhase.Play && phase.Skipped)
                    {
                        check = false;
                        break;
                    }
                }
                if (check) return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, Fuli.GetKingdoms(room), Name);
            return false;
        }
    }

    public class BiluanHegemony : DistanceSkill
    {
        public BiluanHegemony() : base("biluan_hegemony") { }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            if (RoomLogic.PlayerHasShownSkill(room, to, this))
                return Math.Max(1, to.GetEquips().Count);

            return 0;
        }
    }

    public class LixiaHegemony : TriggerSkill
    {
        public LixiaHegemony() : base("lixia_hegemony")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            view_as_skill = new LixiaHegemonyVS();
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && player.Phase == PlayerPhase.Start)
            {
                List<Player> targets = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in targets)
                {
                    if (p != player && p.HasEquip() && !RoomLogic.IsFriendWith(room, player, p) && RoomLogic.PlayerHasShownSkill(room, p, this) && RoomLogic.CanDiscard(room, player, p, "e"))
                        triggers.Add(new TriggerStruct(Name, player, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player ss = room.FindPlayer(info.SkillOwner);
            if (room.AskForSkillInvoke(player, Name, ss))
            {
                room.BroadcastSkillInvoke(Name, ss, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player ss = room.FindPlayer(info.SkillOwner);
            int card_id = room.AskForCardChosen(player, ss, "e", Name, false, HandlingMethod.MethodDiscard);
            room.ThrowCard(card_id, ss, player);

            List<string> choices = new List<string>();
            if (player.Alive) choices.Add("losehp");
            if (ss.Alive) choices.Add("letdraw");

            if (player.Alive)
            {
                WrappedCard dm = room.AskForUseCard(player, "@@lixia_hegemony", "@lixia-discard:" + ss.Name, null);
                if (dm != null)
                {
                    List<int> ids = new List<int>(dm.SubCards);
                    room.ThrowCard(ref ids, new CardMoveReason(MoveReason.S_REASON_DISCARD, player.Name, Name, string.Empty), player);
                }
                else if (!ss.Alive || room.AskForChoice(player, Name, string.Join("+", choices)) == "losehp")
                {
                    room.LoseHp(player);
                }
                else
                {
                    room.DrawCards(ss, new DrawCardStruct(2, player, Name));
                }
            }
            else if (ss.Alive)
            {
                room.DrawCards(ss, new DrawCardStruct(2, player, Name));
            }


            return false;
        }
    }

    public class LixiaHegemonyVS : ViewAsSkill
    {
        public LixiaHegemonyVS() : base("lixia_hegemony") { }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < 2 && RoomLogic.CanDiscard(room, player, player, to_select.Id) && room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@lixia_hegemony";
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 2)
            {
                WrappedCard dm = new WrappedCard(DummyCard.ClassName);
                dm.AddSubCards(cards);
                return dm;
            }
            return null;
        }
    }
}