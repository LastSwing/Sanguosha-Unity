using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class Anniversary : GeneralPackage
    {
        public Anniversary() : base("Anniversary")
        {
            skills = new List<Skill>
            {
                new Guolun(),
                new Songsang(),
                new Zhanji(),
            };

            skill_cards = new List<FunctionCard>
            {
                new GuolunCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
            };
        }
    }

    public class Guolun : ZeroCardViewAsSkill
    {
        public Guolun() : base("guolun")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(GuolunCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(GuolunCard.ClassName) { Skill = Name };
        }
    }

    public class GuolunCard : SkillCard
    {
        public static string ClassName = "GuolunCard";
        public GuolunCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && !to_select.IsKongcheng() && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            int id = room.AskForCardChosen(player, target, "h", "guolun", false, HandlingMethod.MethodNone);
            room.ShowCard(target, id, "guolun");

            List<int> ids = new List<int>();
            player.SetMark("guolun", id);
            target.SetFlags("guolun");
            if (!player.IsKongcheng())
                ids = room.AskForExchange(player, "guolun", 1, 0, "@guolun:" + target.Name, string.Empty, ".", card_use.Card.SkillPosition);
            player.SetMark("guolun", 0);
            target.SetFlags("-guolun");

            if (ids.Count > 0)
            {
                Player drawer = null;
                if (room.GetCard(ids[0]).Number < room.GetCard(id).Number)
                    drawer = player;
                else if (room.GetCard(ids[0]).Number > room.GetCard(id).Number)
                    drawer = target;

                CardsMoveStruct move1 = new CardsMoveStruct(ids, target, Place.PlaceHand,
                    new CardMoveReason(CardMoveReason.MoveReason.S_REASON_SWAP, player.Name, target.Name, "guolun", string.Empty));
                CardsMoveStruct move2 = new CardsMoveStruct(new List<int> { id }, player, Place.PlaceHand,
                    new CardMoveReason(CardMoveReason.MoveReason.S_REASON_SWAP, target.Name, player.Name, "guolun", string.Empty));
                List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct> { move1, move2 };
                room.MoveCards(exchangeMove, true);

                if (drawer != null && drawer.Alive)
                    room.DrawCards(drawer, new DrawCardStruct(1, player, "guolun"));
            }
        }
    }
    public class Songsang : TriggerSkill
    {
        public Songsang() : base("songsang")
        {
            events.Add(TriggerEvent.Death);
            frequency = Frequency.Limited;
            skill_type = SkillType.Recover;
            limit_mark = "@sang";
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.IsNude()) return triggers;
            List<Player> caopis = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player caopi in caopis)
                if (caopi != player && caopi.GetMark(limit_mark) > 0)
                    triggers.Add(new TriggerStruct(Name, caopi));

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(caopi, Name, data, info.SkillPosition))
            {
                room.SetPlayerMark(caopi, limit_mark, 0);
                room.BroadcastSkillInvoke(Name, caopi, info.SkillPosition);
                room.DoSuperLightbox(caopi, info.SkillPosition, Name);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            if (caopi.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = caopi,
                    Recover = 1
                };
                room.Recover(caopi, recover, true);
            }
            else
            {
                caopi.MaxHp++;
                room.BroadcastProperty(caopi, "MaxHp");

                LogMessage log = new LogMessage
                {
                    Type = "$GainMaxHp",
                    From = caopi.Name,
                    Arg = "1"
                };
                room.SendLog(log);
                room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, caopi);
            }
            room.HandleAcquireDetachSkills(caopi, "zhanji", true);

            return false;
        }
    }

    public class Zhanji : TriggerSkill
    {
        public Zhanji() : base("zhanji")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.To != null && base.Triggerable(move.To, room) && move.To.Phase == PlayerPhase.Play
                && move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_DRAW && move.Reason.SkillName != Name)
                return new TriggerStruct(Name, move.To);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DrawCards(ask_who, 1, Name);

            return false;
        }
    }

}