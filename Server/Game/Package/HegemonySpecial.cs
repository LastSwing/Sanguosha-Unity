using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class HegemonySpecial : GeneralPackage
    {
        public HegemonySpecial() : base("HegemonySpecial")
        {
            skills = new List<Skill>
            {
                new Tunchu(),
                new TunchuAdd(),
                new TunchuProhibit(),
                new Shuliang(),
                new Dujin(),
                new Weicheng(),
                new Daoshu(),
                new Yuanyu(),
                new Guishu(),
            };
            skill_cards = new List<FunctionCard>
            {
                new GuishuCard(),
                new DaoshuCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "tunchu", new List<string> { "#tunchu-add", "#tunchu-prohibit" } },
            };
        }
    }

    //lifeng
    public class Tunchu : DrawCardsSkill
{
    public Tunchu() : base("tunchu")
    {
        skill_type = SkillType.Replenish;
    }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw && player.GetPile("commissariat").Count == 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                player.SetTag(Name, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            player.SetFlags(Name);
            return n + 2;
        }
    }

    public class TunchuAdd : TriggerSkill
    {
        public TunchuAdd() : base("#tunchu-add")
        {
            events.Add(TriggerEvent.AfterDrawNCards);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && player.HasFlag("tunchu"))
            {
                if (player.IsKongcheng())
                {
                    player.SetFlags("-tunchu");
                    return new TriggerStruct();
                }
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    SkillPosition = (string)player.GetTag("tunchu")
                };
                return trigger;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags("-tunchu");
            player.RemoveTag("tunchu");

            List<int> ids = room.AskForExchange(player, "tunchu", player.HandcardNum, 0, "@tunchu", string.Empty, ".|.|.|hand", info.SkillPosition);
            if (ids.Count > 0) room.AddToPile(player, "commissariat", ids);

            return false;
        }
    }
    

    public class TunchuProhibit : ProhibitSkill
    {
        public TunchuProhibit() : base("#tunchu-prohibit")
        {
        }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && from.GetPile("commissariat").Count > 0 && card != null && card.Name.Contains("Slash"))
            {
                CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
                return reason == CardUseReason.CARD_USE_REASON_PLAY || reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE;
            }

            return false;
        }
    }

    public class Shuliang : TriggerSkill
    {
        public Shuliang() : base("shuliang")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player.Alive && player.Phase == PlayerPhase.Finish && player.HandcardNum < player.Hp)
            {
                List<Player> lifengs = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in lifengs)
                {
                    if (p.GetPile("commissariat").Count > 0)
                    {
                        TriggerStruct trigger = new TriggerStruct(Name, p);
                        skill_list.Add(trigger);
                    }
                }
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lifeng, TriggerStruct info)
        {
            if (player.Alive && player.Phase == PlayerPhase.Finish && player.HandcardNum < player.Hp && lifeng.GetPile("commissariat").Count > 0)
            {
                List<int> ids = room.AskForExchange(lifeng, Name, 1, 0, "@shuliang:" + player.Name, "commissariat", string.Empty, info.SkillPosition);
                if (ids.Count > 0)
                {
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, string.Empty, Name, Name);
                    room.ThrowCard(ref ids, reason, null);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, lifeng.Name, player.Name);
                    room.BroadcastSkillInvoke(Name, lifeng, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive)
                room.DrawCards(player, new DrawCardStruct(2, ask_who, Name));
            return false;
        }
    }

    //lincao
    public class Dujin : DrawCardsSkill
    {
        public Dujin() : base("dujin")
        {
            skill_type = SkillType.Replenish;
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
        public override int GetDrawNum(Room room, Player player, int n)
        {
            int m = player.GetEquips().Count / 2 + 1;
            return n + m;
        }
    }

    //jianggan
    public class Weicheng : TriggerSkill
    {
        public Weicheng() : base("weicheng")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.To != null && move.From != null && base.Triggerable(move.From, room)
                && move.From_places.Contains(Place.PlaceHand) && (move.Reason.Reason == MoveReason.S_REASON_GIVE
                || move.Reason.Reason == MoveReason.S_REASON_EXTRACTION) && move.From.HandcardNum < move.From.Hp)
                return new TriggerStruct(Name, move.From);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            room.DrawCards(player, 1, Name);
            return false;
        }
    }
    public class Daoshu : TriggerSkill
    {
        public Daoshu() : base("daoshu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Wizzard;
            view_as_skill = new DaoshuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-daoshu");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class DaoshuVS : ZeroCardViewAsSkill
    {
        public DaoshuVS() : base("daoshu")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasFlag(Name);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(DaoshuCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            return card;
        }
    }

    public class DaoshuCard : SkillCard
    {
        public static string ClassName = "DaoshuCard";
        private readonly List<string> suits = new List<string> { "spade", "heart", "club", "diamond" };
        public DaoshuCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return Self != to_select && targets.Count == 0 && !to_select.IsKongcheng() && RoomLogic.CanGetCard(room, Self, to_select, "h");
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            if (target.Alive && !target.IsKongcheng() && RoomLogic.CanGetCard(room, player, target, "h"))
            {
                target.SetFlags("daoshu_target");
                string suit = room.AskForChoice(player, "daoshu", string.Join("+", suits));
                target.SetFlags("-daoshu_target");

                int id = room.AskForCardChosen(player, target, "h", "daoshu", false, HandlingMethod.MethodGet);
                room.ObtainCard(player, id);

                if (WrappedCard.GetSuitString(room.GetCard(id).Suit) == suit)
                {
                    room.Damage(new DamageStruct("daoshu", player, target));
                }
                else
                {
                    suit = WrappedCard.GetSuitString(room.GetCard(id).Suit);
                    player.SetFlags("daoshu");
                    List<int> ids = new List<int>();
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "daoshu", string.Empty);
                    foreach (int card_id in player.GetCards("h"))
                    {
                        if (WrappedCard.GetSuitString(room.GetCard(card_id).Suit) != suit)
                            ids.Add(card_id);
                    }

                    if (ids.Count == 0) room.ShowAllCards(player, null, "daoshu");
                    else if (ids.Count == 1)
                    {
                        room.ObtainCard(target, ref ids, reason, true);
                    }
                    else
                    {
                        List<int> to_give = room.AskForExchange(player, "daoshu", 1, 1, "@daoshu-give:" + target.Name, string.Empty, string.Format(".|^{0}|.|hand", suit), card_use.Card.SkillPosition);
                        if (to_give.Count == 1)
                            room.ObtainCard(target, ref to_give, reason, true);
                        else
                        {
                            List<int> give = new List<int> { ids[0] };
                            room.ObtainCard(target, ref give, reason, true);
                        }
                    }
                }
            }
        }
    }

    //himiko
    public class Guishu : TriggerSkill
    {
        public Guishu() : base("guishu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Alter;
            view_as_skill = new GuishuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class GuishuVS : ViewAsSkill
    {
        public GuishuVS() : base("guishu")
        {
            response_or_use = true;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return true;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return to_select.Suit == WrappedCard.CardSuit.Spade && selected.Count == 0 && room.GetCardPlace(to_select.Id) != Place.PlaceEquip;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (cards.Count == 1)
            {
                foreach (FunctionCard fcard in room.AvailableFunctionCards)
                {
                    if (fcard.Name == BefriendAttacking.ClassName && (player.GetMark(Name) == 0 || player.GetMark(Name) == 1))
                    {

                        WrappedCard ba = new WrappedCard(BefriendAttacking.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name,
                        };
                        ba.AddSubCards(cards);
                        result.Add(ba);
                    }

                    if (fcard.Name == KnownBoth.ClassName && (player.GetMark(Name) == 0 || player.GetMark(Name) == 2))
                    {
                        WrappedCard kb = new WrappedCard(KnownBoth.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name,
                        };
                        kb.AddSubCards(cards);
                        result.Add(kb);
                    }
                }
            }

            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
            {
                WrappedCard gs = new WrappedCard(GuishuCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name,
                    UserString = cards[0].Name
                };
                gs.AddSubCard(cards[0]);
                return gs;
            }

            return null;
        }
    }

    public class GuishuCard : SkillCard
    {
        public static string ClassName = "GuishuCard";
        public GuishuCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard ba = new WrappedCard(card.UserString);
            ba.AddSubCard(card);
            ba = RoomLogic.ParseUseCard(room, ba);
            FunctionCard bcard = Engine.GetFunctionCard(card.UserString);

            return bcard.TargetFilter(room, targets, to_select, Self, ba);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player player = use.From;
            WrappedCard ba = new WrappedCard(use.Card.UserString)
            {
                Skill = "guishu",
                ShowSkill = "guishu",
                SkillPosition = use.Card.SkillPosition
            };
            ba.AddSubCard(use.Card);
            ba = RoomLogic.ParseUseCard(room, ba);

            if (ba.Name == BefriendAttacking.ClassName)
                player.SetMark("guishu", 2);
            else
                player.SetMark("guishu", 1);

            return ba;
        }
    }

    public class Yuanyu : TriggerSkill
    {
        public Yuanyu() : base("yuanyu")
        {
            events.Add(TriggerEvent.DamageInflicted);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive && damage.From != damage.To
                && room.GetNextAlive(damage.From) != player && room.GetNextAlive(player) != damage.From)
                    return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = RoomLogic.PlayerHasShownSkill(room, player, Name) ? true : room.AskForSkillInvoke(player, Name, data, info.SkillPosition);
            if (invoke)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#ReduceDamage",
                From = player.Name,
                Arg = Name,
                Arg2 = (--damage.Damage).ToString()
            };
            room.SendLog(log);

            if (damage.Damage < 1)
                return true;
            data = damage;

            return false;
        }
    }
}