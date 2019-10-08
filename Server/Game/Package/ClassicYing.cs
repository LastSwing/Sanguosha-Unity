using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class ClassicYing : GeneralPackage
    {
        public ClassicYing() : base("ClassicYing")
        {
            skills = new List<Skill>
            {
                new Chenglue(),
                new ChenglueTar(),
                new ShicaiJX(),
                new Cunmu(),
                new Tushe(),
                new Limu(),
                new LimuTar(),
            };

            skill_cards = new List<FunctionCard>
            {
                new ChenglueCard(),
                new LimuCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "chenglue", new List<string>{ "#chenglue-tar" } },
                { "limu", new List<string>{ "#limu-tar" } },
            };
        }
    }

    public class Chenglue : TriggerSkill
    {
        public Chenglue() : base("chenglue")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            frequency = Frequency.Turn;
            view_as_skill = new ChenglueVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
            {
                player.RemoveTag(Name);
                room.RemovePlayerStringMark(player, Name);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class ChenglueVS : ZeroCardViewAsSkill
    {
        public ChenglueVS() : base("chenglue")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ChenglueCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard cl = new WrappedCard(ChenglueCard.ClassName)
            {
                Skill = Name
            };
            return cl;
        }
    }

    public class ChenglueTar : TargetModSkill
    {
        public ChenglueTar() : base("#chenglue-tar", false)
        {
            pattern = "BasicCard,TrickCard";
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (from != null && from.ContainsTag("chenglue") && from.GetTag("chenglue") is List<WrappedCard.CardSuit> suits && suits.Contains(RoomLogic.GetCardSuit(room, card)))
            {
                return true;
            }

            return false;
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (from != null && from.ContainsTag("chenglue") && from.GetTag("chenglue") is List<WrappedCard.CardSuit> suits && suits.Contains(RoomLogic.GetCardSuit(room, card)))
            {
                return 999;
            }

            return 0;
        }
    }

    public class ChenglueCard : SkillCard
    {
        public static string ClassName = "ChenglueCard";
        public ChenglueCard() : base(ClassName)
        {
            will_throw = true;
            target_fixed = true;
        }

        private readonly Dictionary<WrappedCard.CardSuit, string> suits = new Dictionary<WrappedCard.CardSuit, string> {
            { WrappedCard.CardSuit.Club, "♠" },
            { WrappedCard.CardSuit.Spade, "♣" },
            { WrappedCard.CardSuit.Heart, "♥" },
            { WrappedCard.CardSuit.Diamond, "♦" },
        };
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            int count = player.GetMark("chenglue") == 0 ? 1 : 2;
            player.SetMark("chenglue", count == 1 ? 1 : 0);

            List<string> arg = new List<string> {
                GameEventType.S_GAME_EVENT_SKILL_TURN.ToString(),
                player.Name,
                "chenglue",
                count == 1 ? true.ToString() : false.ToString(),
                card_use.Card.SkillPosition
            };
            room.DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);

            room.DrawCards(player, count, "chenglue");
            if (player.Alive)
            {
                List<int> ids = room.AskForExchange(player, "chenglue", count == 1 ? 2 : 1, count == 1 ? 2 : 1,
                    string.Format("@chenglue-discard:::{0}", count == 1 ? 2 : 1), string.Empty, ".!", card_use.Card.SkillPosition);
                room.ThrowCard(ref ids, player);

                string mark = string.Empty;
                List<WrappedCard.CardSuit> discards = new List<WrappedCard.CardSuit>();
                foreach (int id in ids)
                {
                    WrappedCard.CardSuit suit = room.GetCard(id).Suit;
                    string suit_string = suits[suit];
                    if (!mark.Contains(suit_string)) mark += suit_string;
                    if (!discards.Contains(suit)) discards.Add(suit);
                }

                if (player.Alive && !string.IsNullOrEmpty(mark))
                {
                    player.SetTag("chenglue", discards);
                    room.SetPlayerStringMark(player, "chenglue", mark);
                }
            }
        }
    }

    public class ShicaiJX : TriggerSkill
    {
        public ShicaiJX() : base("shicai_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.CardFinished, TriggerEvent.TargetConfirmed };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.ContainsTag(Name))
                        p.RemoveTag(Name);
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                {
                    bool dis = false;
                    foreach (int id in use.Card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.DiscardPile)
                        {
                            dis = true;
                            break;
                        }
                    }

                    FunctionCard.CardType type = fcard.TypeID;
                    List<FunctionCard.CardType> types = player.ContainsTag(Name) ? (List<FunctionCard.CardType>)player.GetTag(Name) : new List<FunctionCard.CardType>();
                    if (dis && !types.Contains(type))
                        return new TriggerStruct(Name, player);
                }
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct _use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                List<FunctionCard.CardType> types = player.ContainsTag(Name) ? (List<FunctionCard.CardType>)player.GetTag(Name) : new List<FunctionCard.CardType>();
                if (fcard is EquipCard && !types.Contains(FunctionCard.CardType.TypeEquip) && room.GetCardPlace(_use.Card.GetEffectiveId()) == Place.PlaceTable)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                FunctionCard.CardType type = fcard.TypeID;
                List<FunctionCard.CardType> types = player.ContainsTag(Name) ? (List<FunctionCard.CardType>)player.GetTag(Name) : new List<FunctionCard.CardType>();
                if (!types.Contains(type))
                {
                    types.Add(type);
                    player.SetTag(Name, types);
                }

                List<int> dis = new List<int>();
                foreach (int id in use.Card.SubCards)
                    if ((room.GetCardPlace(id) == Place.DiscardPile && triggerEvent == TriggerEvent.CardFinished) ||
                        (triggerEvent == TriggerEvent.TargetConfirmed && room.GetCardPlace(id) == Place.PlaceTable))
                        dis.Add(id);

                if (dis.Count > 0 && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PUT, player.Name, Name, string.Empty);
                    CardsMoveStruct move = new CardsMoveStruct(dis, null, Place.DrawPile, reason)
                    {
                        To_pile_name = string.Empty,
                        From = null
                    };

                    List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
                    room.MoveCardsAtomic(moves, true);

                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, 1, Name);
            return false;
        }
    }

    public class Cunmu : TriggerSkill
    {
        public Cunmu() : base("cunmu")
        {
            events.Add(TriggerEvent.CardDrawing);
            skill_type = SkillType.Replenish;
            frequency = Frequency.Compulsory;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (data is List<int> ids)
            {
                int count = ids.Count;
                ids.Clear();
                for (int i = 0; i < count; i++)
                    ids.Add(room.DrawPile[room.DrawPile.Count - 1 - i]);

                data = ids;
            }

            return false;
        }
    }

    public class Tushe : TriggerSkill
    {
        public Tushe() : base("tushe")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen };
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is EquipCard) && !(fcard is SkillCard))
                {
                    bool check = true;
                    foreach (int id in player.GetCards("h"))
                    {
                        WrappedCard card = room.GetCard(id);
                        if (Engine.GetFunctionCard(card.Name) is BasicCard)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                        return new TriggerStruct(Name, player);
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool check = true;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (Engine.GetFunctionCard(card.Name) is BasicCard)
                {
                    check = false;
                    break;
                }
            }
            if (check && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
                room.DrawCards(player, use.To.Count, Name);

            return false;
        }
    }

    public class Limu : OneCardViewAsSkill
    {
        public Limu() : base("limu")
        {
            filter_pattern = ".|diamond";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(Indulgence.ClassName);
            return !RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName) && RoomLogic.IsProhibited(room, player, player, card) == null;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard indulgence = new WrappedCard(LimuCard.ClassName);
            indulgence.AddSubCard(card);
            return indulgence;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 1;
        }
    }

    public class LimuCard : SkillCard
    {
        public static string ClassName = "LimuCard";
        public LimuCard() : base(ClassName)
        {
            will_throw = false;
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            WrappedCard indulgence = new WrappedCard(Indulgence.ClassName)
            {
                Skill = "limu",
                ShowSkill = "limu"
            };
            indulgence.AddSubCard(card_use.Card.GetEffectiveId());
            indulgence = RoomLogic.ParseUseCard(room, indulgence);
            room.UseCard(new CardUseStruct(indulgence, player, player));

            if (player.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(player, recover, true);
            }
        }
    }

    public class LimuTar : TargetModSkill
    {
        public LimuTar() : base("#limu-tar")
        {
            pattern = "BasicCard,TrickCard";
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (from != null && to != null && RoomLogic.PlayerHasShownSkill(room, from, "limu") && RoomLogic.InMyAttackRange(room, from, to, card)
                && from.JudgingArea.Count > 0)
                return true;

            return false;
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card)
        {
            if (from != null && to != null && RoomLogic.PlayerHasShownSkill(room, from, "limu") && RoomLogic.InMyAttackRange(room, from, to, card)
                && from.JudgingArea.Count > 0)
                return true;

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 2;
        }
    }
}
