using System;
using System.Collections.Generic;
using System.Threading;
using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class Transformation : GeneralPackage
    {
        public Transformation() : base("Transformation")
        {
            skills = new List<Skill>
            {
                new Zhiyu(),
                new Qice(),
                new Wanwei(),
                new Yuejian(),
                new YuejianMaxCards(),
                new Xiongsuan(),
                new Jili(),
                new Sanyao(),
                new Zhiman(),
                new ZhimanSecond(),
                new Xuanlue(),
                new Yongjin(),
                new Diaodu(),
                new Diancai(),
                new Lianzi(),
                new Jubao(),
                new Jiahe(),
                new JiaheClear(),
                new FlameMap(),
                new FlameMapSkill(),
                new YingziExtra(),
                new HaoshiExtra(),
                new Shelie(),
                new Yigui(),
                new YiguiClear(),
                new YiguiProhibt(),
                new Jihun(),
            };
            skill_cards = new List<FunctionCard>
            {
                new QiceCard(),
                new WanweiCard(),
                new XiongsuanCard(),
                new SanyaoCard(),
                new YongjinCard(),
                //new DiaoduequipCard(),
                //new DiaoduCard(),
                new LianziCard(),
                new FlameMapCard(),
                new YiguiCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "zhiman", new List<string>{ "#zhiman-second" } },
                { "jiahe", new List<string>{ "#jiahe-clear" } },
                { "yigui", new List<string>{ "#yigui-clear" } }
            };
        }
    }

    //xunyou
    public class Zhiyu : MasochismSkill
    {
        public Zhiyu() : base("zhiyu")
        {
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player xunyu, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(xunyu, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, xunyu, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player xunyu, DamageStruct damage, TriggerStruct info)
        {
            room.DrawCards(xunyu, 1, Name);
            room.ShowAllCards(xunyu);
            bool same = true;
            bool isRed = WrappedCard.IsRed(room.GetCard(xunyu.HandCards[0]).Suit);
            foreach (int id in xunyu.HandCards) {
                if (WrappedCard.IsRed(room.GetCard(id).Suit) != isRed)
                {
                    same = false;
                    break;
                }
            }
            if (same && damage.From != null && !damage.From.IsKongcheng() && RoomLogic.CanDiscard(room, damage.From, damage.From, "h"))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, xunyu.Name, damage.From.Name);
                room.AskForDiscard(damage.From, Name, 1, 1);
            }
        }
    }
    public class QiceCard: SkillCard
    {
        public QiceCard() : base("QiceCard")
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard mutable_card = new WrappedCard(card.UserString)
            {
                CanRecast = false
            };
            mutable_card.AddSubCards(card.SubCards);
            mutable_card = RoomLogic.ParseUseCard(room, mutable_card);
            FunctionCard fcard = Engine.GetFunctionCard(card.UserString);

            if (fcard == null || (targets.Count >= card.SubCards.Count && !(fcard is Collateral))) return false;
            List <Player> players = new List<Player>();
            if (fcard is AllianceFeast)
            {
                if (to_select.Role == "careerist")
                {
                    if (card.SubCards.Count < 2)
                        return false;
                }
                else
                {
                    foreach (Player p in room.GetOtherPlayers(Self))
                if (RoomLogic.IsFriendWith(room, p, to_select) && RoomLogic.IsProhibited(room, Self, p, mutable_card) == null)
                        players.Add(p);
                    if (players.Count > card.SubCards.Count - 1) return false;
                }
            }
            else if (fcard is FightTogether)
            {
                List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
                if (big_kingdoms.Count == 0) return false;
                foreach (Player p in room.GetAlivePlayers())
                    players.Add(p);
                List <Player> bigs = new List<Player>(), smalls = new List<Player>();
                foreach (Player p in players) {
                    string kingdom = (p.HasShownOneGeneral() ? (p.Role == "careerist" ? p.Name : p.Kingdom) : null);
                    if (big_kingdoms.Contains(kingdom))
                    {
                        if (RoomLogic.IsProhibited(room, Self, p, mutable_card) == null)
                            bigs.Add(p);
                    }
                    else if (RoomLogic.IsProhibited(room, Self, p, mutable_card) == null)
                        smalls.Add(p);
                }

                string target_kingdom = (to_select.HasShownOneGeneral() ?
                                          (to_select.Role == "careerist" ? to_select.Name : to_select.Kingdom) : null);
                bool big = big_kingdoms.Contains(target_kingdom);
                if (big && bigs.Count > card.SubCards.Count) return false;
                if (!big && smalls.Count > card.SubCards.Count) return false;
            }

            return fcard.TargetFilter(room, targets, to_select, Self, mutable_card) && RoomLogic.IsProhibited(room, Self, to_select, mutable_card, targets) == null;
        }

        public override bool TargetFixed(WrappedCard card)
        {
            WrappedCard mutable_card = new WrappedCard(card.UserString)
            {
                CanRecast = false
            };
            mutable_card.AddSubCards(card.SubCards);
            FunctionCard fcard = Engine.GetFunctionCard(card.UserString);
            return fcard != null && fcard.TargetFixed(mutable_card);
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            WrappedCard mutable_card = new WrappedCard(card.UserString)
            {
                CanRecast = false
            };
            mutable_card.AddSubCards(card.SubCards);
            mutable_card = RoomLogic.ParseUseCard(room, mutable_card);
            FunctionCard fcard = Engine.GetFunctionCard(card.UserString);
            if (fcard is Collateral && targets.Count / 2 > card.SubCards.Count)
                return false;
            else if (targets.Count > card.SubCards.Count)
                return false;

            return fcard != null && fcard.TargetsFeasible(room, targets, Self, mutable_card);
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player source = use.From;

            string c = use.Card.UserString;
            WrappedCard use_card = new WrappedCard(c)
            {
                Skill = "qice"
            };
            use_card.AddSubCards(use.Card.SubCards);
            use_card.CanRecast = false;
            use_card.ShowSkill = "qice";
            use_card = RoomLogic.ParseUseCard(room, use_card);

            bool available = true;
            FunctionCard fcard = Engine.GetFunctionCard(c);
            List<Player> targets = new List<Player>();
            if (fcard is AwaitExhausted)
            {
                foreach (Player p in room.GetAlivePlayers())
            if (RoomLogic.IsProhibited(room, source, p, use_card) == null && RoomLogic.IsFriendWith(room, source, p))
                    targets.Add(p);
            }
            else if (fcard.GetSubtype() == "global_effect")
            {
                foreach (Player p in room.GetAlivePlayers())
            if (RoomLogic.IsProhibited(room, source, p, use_card) == null)
                    targets.Add(p);
            }
            else if (fcard.GetSubtype() == "aoe" && !(fcard is BurningCamps))
            {
                foreach (Player p in room.GetOtherPlayers(source))
            if (RoomLogic.IsProhibited(room, source, p, use_card) == null)
                    targets.Add(p);
            }
            else if (fcard is BurningCamps)
            {
                List<Player> players = RoomLogic.GetFormation(room, room.GetNextAlive(source));
                foreach (Player p in players) {
                    if (RoomLogic.IsProhibited(room, source, p, use_card) == null)
                        targets.Add(p);
                }
            }
            if (targets.Count > use_card.SubCards.Count) return null;

            foreach (Player to in use.To)
            {
                if (RoomLogic.IsProhibited(room, source, to, use_card) != null)
                {
                    available = false;
                    break;
                }
            }
            available = available && fcard.IsAvailable(room, source, use_card);
            if (!available) return null;
            return use_card;
        }
    }
    public class QiceVS : ViewAsSkill
    {
        public QiceVS() : base("qice")
        {
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player) => false;
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].Id < 0)
            {
                WrappedCard card = new WrappedCard("QiceCard");
                card.AddSubCards(player.HandCards);
                card.UserString = cards[0].Name;
                return card;
            }
            return null;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed("QiceCard"))
            {
                foreach (int id in player.HandCards)
                {
                    WrappedCard card = room.GetCard(id);
                    if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse))
                        return false;
                }

                return true;
            }
            return false;
        }
        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            return GetGuhuoCards(room, player);
        }

        public static List<WrappedCard> GetGuhuoCards(Room room, Player player)
        {
            List<string> names = GetGuhuoCards(room, "t");
            List<WrappedCard> all_cards = new List<WrappedCard>();
            foreach (string name in names)
            {
                if (name == "ThreatenEmperor") continue;
                WrappedCard card = new WrappedCard(name)
                {
                    CanRecast = false
                };
                card.AddSubCards(player.HandCards);
                card = RoomLogic.ParseUseCard(room, card);
                if (CheckGuhuo(room, card, player)) all_cards.Add(card);
            }
            return all_cards;
        }

        static bool CheckGuhuo(Room room, WrappedCard card, Player Self)
        {
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (!fcard.IsAvailable(room, Self, card)) return false;
            List<Player> targets = new List<Player>();
            if (fcard is AwaitExhausted)
            {
                if (RoomLogic.IsProhibited(room, Self, Self, card) == null)
                    targets.Add(Self);
                foreach (Player p in room.GetOtherPlayers(Self))
                if (RoomLogic.IsProhibited(room, Self, p, card) == null && RoomLogic.IsFriendWith(room, Self, p))
                    targets.Add(p);
            }
            else if (fcard.GetSubtype() == "global_effect" && !(fcard is FightTogether))
            {
                if (RoomLogic.IsProhibited(room, Self, Self, card) == null)
                    targets.Add(Self);
                foreach (Player p in room.GetOtherPlayers(Self))
                if (RoomLogic.IsProhibited(room, Self, p, card) == null)
                    targets.Add(p);
            }
            else if (fcard is FightTogether)
            {
                List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
                if (big_kingdoms.Count > 0)
                {
                    List<Player> bigs = new List<Player>(), smalls = new List<Player>();
                    List<Player> all = room.GetAlivePlayers();

                    foreach (Player p in all) {
                        if (RoomLogic.IsProhibited(room, Self, p, card) != null) continue;
                        string kingdom = (p.HasShownOneGeneral() ? (p.Role == "careerist" ? p.Name : p.Kingdom) : null);

                        if (big_kingdoms.Contains(kingdom))
                            bigs.Add(p);
                        else
                            smalls.Add(p);
                    }
                    if ((smalls.Count > 0 && smalls.Count < bigs.Count && bigs.Count > 0) || (smalls.Count > 0 && bigs.Count == 0))
                        targets = smalls;
                    else if ((smalls.Count > 0 && smalls.Count > bigs.Count && bigs.Count > 0) || (smalls.Count == 0 && bigs.Count > 0))
                        targets = bigs;
                    else if (smalls.Count == bigs.Count)
                        targets = smalls;
                }
            }
            else if (fcard.GetSubtype() == "aoe" && !(fcard is BurningCamps))
            {
                foreach (Player p in room.GetOtherPlayers(Self))
                if (RoomLogic.IsProhibited(room, Self, p, card) == null)
                    targets.Add(p);
            }
            else if (fcard is BurningCamps)
            {
                List<Player> players = RoomLogic.GetFormation(room, room.GetNextAlive(Self));
                foreach (Player p in players) {
                    if (RoomLogic.IsProhibited(room, Self, p, card) == null)
                        targets.Add(p);
                }
            }
            return (targets.Count <= Self.HandcardNum);
        }
    }
    public class Qice : TriggerSkill
    {
        public Qice() : base("qice")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.PreCardUsed };
            view_as_skill = new QiceVS();
            skill_type = SkillType.Alter;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.PreCardUsed && data is CardUseStruct use && player.GetMark(Name + "_transform") == 0)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID == CardType.TypeTrick && use.Card.Skill == Name && use.From != null && use.From == player)
                    player.SetFlags(Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID == CardType.TypeTrick && use.Card.Skill == Name && player.HasFlag(Name) && player.Alive)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.CanTransform(player) && room.AskForSkillInvoke(player, "transform"))
            {
                //room.BroadcastSkillInvoke("transform", player.IsMale);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.AddMark(Name + "_transform");
            room.TransformDeputyGeneral(player);
            return false;
        }
    }

    //bianhuanhou
    //stupid design
    public class WanweiCard : SkillCard
    {
        public WanweiCard() : base("WanweiCard")
        {
            target_fixed = true;
            will_throw = false;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<string> cards = card_use.From.ContainsTag("wanwei") ? (List<string>)card_use.From.GetTag("wanwei") : new List<string>();
            cards.Add(JsonUntity.Object2Json(card_use.Card.SubCards));
            card_use.From.SetTag("wanwei", cards);
        }
    }
    public class WanweiViewAsSkill : ViewAsSkill
    {
        public WanweiViewAsSkill() : base("wanwei")
        {
            response_pattern = "@@wanwei";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < player.GetMark(Name) && to_select.HasFlag("can_wanwei");
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0 || cards.Count < player.GetMark(Name)) return null;
            WrappedCard card = new WrappedCard("WanweiCard");
            card.AddSubCards(cards);
            card.ShowSkill = Name;
            card.Skill = Name;
            return card;
        }
    }
    public class Wanwei : TriggerSkill
    {
        public Wanwei() : base("wanwei")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.BeforeCardsMove };
            view_as_skill = new WanweiViewAsSkill();
            skill_type = SkillType.Defense;
        }
        public override bool CanPreShow() => true;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && base.Triggerable(player, room) && data is CardsMoveOneTimeStruct move)
            {
                if (move.From == player && (move.From_places.Contains(Place.PlaceHand) || move.From_places.Contains(Place.PlaceEquip))
                        && !(move.To == player && (move.To_place == Place.PlaceHand || move.To_place == Place.PlaceEquip)))
                    foreach (int id in move.Card_ids)
                    room.SetCardFlag(id, "-" + Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.BeforeCardsMove  && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && base.Triggerable(move.From, room) && (move.From_places.Contains(Place.PlaceHand) || move.From_places.Contains(Place.PlaceEquip))
                        && ((move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_DISMANTLE && move.Reason.PlayerId != move.Reason.TargetId)
                        || (move.To != null && move.To != move.From && move.To_place == Place.PlaceHand
                        && move.Reason.Reason != CardMoveReason.MoveReason.S_REASON_GIVE && move.Reason.Reason != CardMoveReason.MoveReason.S_REASON_SWAP)))
                {
                    foreach (int id in move.Card_ids) {
                        if (room.GetCard(id).HasFlag(Name)) return new TriggerStruct();
                    }
                    if (move.Card_ids.Count >= move.From.HandcardNum + move.From.GetEquips().Count) return new TriggerStruct();
                    return new TriggerStruct(Name, move.From);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player ask_who, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            Player player = move.From;
            int num = move.Card_ids.Count;
            bool get = false;
            CardMoveReason reason = move.Reason;
            string target_name = reason.PlayerId;
            if (move.To != null)
                get = true;
            Player target = move.To;
            foreach (int id in player.GetCards("he")) {
                if (get)
                {
                    if (RoomLogic.CanGetCard(room, target, player, id))
                        room.SetCardFlag(id, "can_wanwei");
                }
                else
                    if (RoomLogic.CanDiscard(room, target, player, id))
                    room.SetCardFlag(id, "can_wanwei");
            }
            player.SetMark(Name, num);
            List<string> card_names = new List<string>();
            foreach (int id in move.Card_ids)
                card_names.Add(room.GetCard(id).Name);
            string card_name = string.Join("\\, \\", card_names);
            string prompt = string.Format("@wanwei:{0}::{1}:{2}", get ? target.Name : string.Empty, card_name, num);

            room.SetTag("wanwei_data", new List<int>(move.Card_ids));
            WrappedCard card = room.AskForUseCard(player, "@@wanwei", prompt, -1,  HandlingMethod.MethodNone, true, info.SkillPosition);
            room.RemoveTag("wanwei_data");

            foreach (int id in player.GetCards("he"))
            room.SetCardFlag(id, "-can_wanwei");
            if (card != null)
                return info;
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player ask_who, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            Player player = move.From;
            List<string> strs = (List<string>)player.GetTag(Name);
            List<int> result = JsonUntity.Json2List<int>(strs[strs.Count - 1]);
            strs.RemoveAt(strs.Count - 1);
            player.SetTag(Name, strs);
            if (result.Count == 0) return false;
            
            int num = move.Card_ids.Count;
            move.From_places.Clear();
            foreach (int id in result) {
                room.SetCardFlag(id, Name);
                move.From_places.Add(room.GetCardPlace(id));
            }
            move.Card_ids = result;
            data = move;

            bool get = false;
            CardMoveReason reason = move.Reason;
            string target_name = reason.PlayerId;
            if (move.To != null)
                get = true;

            LogMessage log = new LogMessage
            {
                Type = "#wanwei",
                From = player.Name,
                To = new List<string> { move.To?.Name },
                Arg = num.ToString(),
                Arg2 = get ? "get" : "dismantled"
            };
            room.SendLog(log);
            return false;
        }
        
    }
    public class Yuejian : TriggerSkill
    {
        public Yuejian() : base("yuejian")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Defense;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsed && !player.HasFlag(Name) && room.Current == player && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID != CardType.TypeSkill && use.From != null)
                {
                    foreach (Player p in use.To) {
                        if (p != player && !RoomLogic.IsFriendWith(room, p, player))
                        {
                            player.SetFlags(Name);
                            break;
                        }
                    }
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && !player.HasFlag(Name) && player.Phase ==  PlayerPhase.Discard)
            {
                List<Player> huanghous = RoomLogic.FindPlayersBySkillName(room, Name);
                List<TriggerStruct> skill_list = new List<TriggerStruct>();
                foreach (Player huanghou in huanghous)
                if (RoomLogic.WillBeFriendWith(room, huanghou, player))
                    skill_list.Add(new TriggerStruct(Name, huanghou));
                return skill_list;
            }
            return new List<TriggerStruct>();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player huanghou, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(huanghou, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, huanghou, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags("jianyue_keep");
            return false;
        }
    }
    public class YuejianMaxCards : MaxCardsSkill
    {
        public YuejianMaxCards() : base("#yuejian-maxcard")
        {
        }
        public override int GetFixed(Room room, Player target)
        {
            if (target.HasFlag("jianyue_keep"))
                return target.MaxHp;
            return -1;
        }
    }
    //liguo
    public class XiongsuanCard: SkillCard
    {
        public XiongsuanCard() : base("XiongsuanCard")
        {
            will_throw = true;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0) return false;
            return RoomLogic.WillBeFriendWith(room, Self, to_select);
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@xiong", 0);
            room.BroadcastSkillInvoke("xiongsuan", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "xiongsuan");
            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            string reason = "xiongsuan";
            DamageStruct damage = new DamageStruct(reason, card_use.From, target);
            room.Damage(damage);
            if (card_use.From.Alive)
            {
                room.DrawCards(card_use.From, 3, "xiongsuan");

                if (target.Alive)
                {
                    List<string> skills = new List<string>();
                    foreach (string skill_name in target.GetSkills(false)) {
                        Skill skill = Engine.GetSkill(skill_name);
                        if (skill != null && RoomLogic.PlayerHasShownSkill(room, target, skill)
                                && skill.SkillFrequency == Skill.Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                        {
                            string mark = skill.LimitMark;
                            if (target.GetMark(mark) == 0)
                                skills.Add(skill_name);
                        }
                    }

                    string answer = "cancel";
                    if (skills.Count > 0)
                    {
                        skills.Add("cancel");
                        answer = room.AskForChoice(card_use.From, "xiongsuan", string.Join("+", skills), new List<string> { "#xiongsuan::" + target.Name });
                    }
                    if (answer != "cancel")
                    {
                        Skill skill = Engine.GetSkill(answer);
                        if (skill.SkillFrequency ==  Skill.Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                        {
                            target.SetTag("xiongsuan", skill.LimitMark);
                            room.SetPlayerMark(target, "xiongsuan", 1);

                            LogMessage log = new LogMessage
                            {
                                Type = "$xiongsuan",
                                From = card_use.From.Name,
                                To = new List<string> { card_use.To[0].Name },
                                Arg = answer
                            };
                            room.SendLog(log);
                        }
                    }
                }
            }
        }
    }
    public class XiongsuanVS : OneCardViewAsSkill
    {
        public XiongsuanVS() : base("xiongsuan")
        {
            filter_pattern = "..!";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "he") && player.GetMark("@xiong") > 0;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard("XiongsuanCard")
            {
                Skill = Name,
                ShowSkill = Name,
                Mute = true
            };
            first.AddSubCard(card);
            return first;
        }
    }
    public class Xiongsuan : TriggerSkill
    {
        public Xiongsuan() : base("xiongsuan")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            view_as_skill = new XiongsuanVS();
            frequency = Frequency.Limited;
            limit_mark = "@xiong";
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAllPlayers()) {
                    if (p.GetMark(Name) > 0)
                    {
                        room.SetPlayerMark(p, (string)p.GetTag(Name), 1);
                        p.SetMark(Name, 0);
                        p.RemoveTag(Name);
                    }
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    //zuoci
    public class Yigui : TriggerSkill
    {
        public Yigui() : base("yigui")
        {
            view_as_skill = new YiguiVS();
            events = new List<TriggerEvent> { TriggerEvent.GeneralShown, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Wizzard;
        }

        public static void Acquiregenerals(Room room, Player zuoci, int n)
        {
            List<string> huashens = zuoci.ContainsTag("spirit") ? (List<string>)zuoci.GetTag("spirit") : new List<string>();
            List<string> acquired = GetavailableGenerals(room, zuoci, n);

            if (acquired.Count == 0) return;

            foreach (string general in acquired)
                room.HandleUsedGeneral(general);

            huashens.AddRange(acquired);
            zuoci.SetTag("spirit", huashens);

            List<Player> others = new List<Player>();
            List<Client> clients = new List<Client>();
            foreach (Player p in room.GetOtherPlayers(zuoci))
            {
                Client c = room.GetClient(p);
                if (c != room.GetClient(zuoci) && !clients.Contains(c))
                {
                    others.Add(p);
                    clients.Add(c);
                }
            }
            LogMessage log = new LogMessage
            {
                Type = "#gethuashendetail",
                From = zuoci.Name,
                Arg = "spirit",
                Arg2 = string.Join("\\, \\", acquired),
            };

            LogMessage log1 = new LogMessage
            {
                Type = "#gethuashen",
                From = zuoci.Name,
                Arg = "spirit",
                Arg2 = acquired.Count.ToString()
            };

            room.SendLog(log, zuoci);
            room.SendLog(log1, new List<Player> { zuoci });

            room.DoAnimate(AnimateType.S_ANIMATE_HUASHEN, string.Join("+", acquired), string.Format("null+{0}", zuoci.Name), new List<Player> { zuoci });
            room.DoAnimate(AnimateType.S_ANIMATE_HUASHEN, "-1+-1", string.Format("null+{0}", zuoci.Name), others);
            Thread.Sleep(1500);
        }

        public static void RemoveHuashen(Room room, Player zuoci, List<string> generals)
        {
            List<string> huashens = zuoci.ContainsTag("spirit") ? (List<string>)zuoci.GetTag("spirit") : new List<string>();
            List<string> remove = new List<string>();
            foreach (string name in generals)
            {
                if (huashens.Contains(name))
                {
                    remove.Add(name);
                    room.HandleUsedGeneral("-" + name);
                }
            }
            if (remove.Count == 0) return;

            huashens.RemoveAll(t => remove.Contains(t));
            zuoci.SetTag("spirit", huashens);

            LogMessage log = new LogMessage
            {
                Type = "#drophuashendetail",
                From = zuoci.Name,
                Arg = "spirit",
                Arg2 = string.Join("\\, \\", remove)
            };
            room.SendLog(log);

            room.DoAnimate(AnimateType.S_ANIMATE_HUASHEN, string.Join("+", remove), string.Format("{0}+null", zuoci.Name));
            Thread.Sleep(1500);
        }

        public static List<string> GetavailableGenerals(Room room, Player zuoci, int n)
        {
            List<string> available = new List<string>();
            foreach (string name in room.Generals)
                if (!room.UsedGeneral.Contains(name))
                    available.Add(name);
            List<string> result = new List<string>();
            Shuffle.shuffle(ref available);
            for (int i = 0; i < Math.Min(available.Count, n); i++)
                result.Add(available[i]);

            return result;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.GeneralShown && base.Triggerable(player, room) && player.GetMark(Name) == 0 && data is bool head)
            {
                player.SetMark(Name, 1);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, head ? "head" : "deputy");
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);

                Acquiregenerals(room, player, 2);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    foreach (FunctionCard card in room.AvailableFunctionCards)
                        p.SetFlags("-yigui_" + card.Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }

        public override int GetEffectIndex(Room room, Player player, WrappedCard card)
        {
            return 2;
        }
    }

    public class YiguiClear : DetachEffectSkill
    {
        public YiguiClear() : base("yigui", string.Empty)
        {
        }
        public override void OnSkillDetached(Room room, Player zuoci, object data)
        {
            List<string> huashens = zuoci.ContainsTag("spirit") ? (List<string>)zuoci.GetTag("spirit") : new List<string>();
            Yigui.RemoveHuashen(room, zuoci, huashens);
        }
    };

    public class YiguiCard : SkillCard
    {
        public YiguiCard() : base("YiguiCard")
        {
        }

        public override bool TargetFixed(WrappedCard card)
        {
            string[] strs = card.UserString.Split('_');
            FunctionCard fcard = Engine.GetFunctionCard(strs[0]);
            return fcard != null && fcard.TargetFixed(null);
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            string[] strs = card.UserString.Split('_');
            FunctionCard fcard = Engine.GetFunctionCard(strs[0]);
            WrappedCard _card = new WrappedCard(strs[0])
            {
                Skill = "yigui",
                ShowSkill = "yigui",
                UserString = strs[1]
            };
            return fcard != null && fcard.TargetFilter(room, targets, to_select, Self, _card);
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            string[] strs = card.UserString.Split('_');
            FunctionCard fcard = Engine.GetFunctionCard(strs[0]);
            WrappedCard _card = new WrappedCard(strs[0])
            {
                Skill = "yigui",
                ShowSkill = "yigui",
                UserString = strs[1]
            };
            return fcard.TargetsFeasible(room, targets, Self, _card);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            string[] strs = use.Card.UserString.Split('_');
            //move general card
            Yigui.RemoveHuashen(room, use.From, new List<string> { strs[1] });

            WrappedCard _card = new WrappedCard(strs[0])
            {
                Skill = "yigui",
                ShowSkill = "yigui",
                UserString = strs[1]
            };

            string flag = strs[1];
            if (flag.Contains("Slash"))
                flag = "Slash";
            use.From.SetFlags("yigui_" + flag);

            return _card;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            string[] strs = card.UserString.Split('_');

            //move general card
            Yigui.RemoveHuashen(room, player, new List<string> { strs[1] });

            WrappedCard _card = new WrappedCard(strs[0])
            {
                Skill = "yigui",
                ShowSkill = "yigui",
                UserString = strs[1]
            };

            string flag = strs[1];
            if (flag.Contains("Slash"))
                flag = "Slash";
            player.SetFlags("yigui_" + flag);

            return _card;
        }
    }

    public class YiguiVS : ViewAsSkill
    {
        public YiguiVS() : base("yigui")
        {
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseStruct.CardUseReason reason, string pattern, string position = null)
        {
            if (!RoomLogic.PlayerHasSkill(room, invoker, Name) || !invoker.ContainsTag("spirit")
                || ((List<string>)(invoker.GetTag("spirit"))).Count == 0 || reason == CardUseReason.CARD_USE_REASON_RESPONSE) return false;

            if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "Jink") return false;

            return GetAvailableGuhuo(room, invoker, reason, pattern).Count > 0;
        }

        private List<WrappedCard> GetAvailableGuhuo(Room room, Player player, CardUseReason reason, string pattern, string general_name = null)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            foreach (FunctionCard fcard in room.AvailableFunctionCards)
            {
                string flag = fcard.Name;
                if (flag.Contains("Slash")) flag = "Slash";

                if (fcard.Name.Contains("Nullification") || fcard.Name == "Jink" || player.HasFlag("yigui_" + flag)
                    || (!(fcard is BasicCard) && !(fcard is TrickCard)) || fcard is DelayedTrick) continue;
                WrappedCard card = new WrappedCard(fcard.Name);
                if (!string.IsNullOrEmpty(general_name) && Engine.GetGeneral(general_name) != null)
                {
                    card.Skill = Name;
                    card.UserString = general_name;
                }
                bool available = fcard.IsAvailable(room, player, card);
                if (available && (reason == CardUseReason.CARD_USE_REASON_PLAY || Engine.MatchExpPattern(room, pattern, player, card)))
                    result.Add(card);
            }

            return result;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && Engine.GetFunctionCard(cards[0].Name) == null && Engine.GetGeneral(cards[0].Name) != null)
            {
                List<WrappedCard> vcards = GetAvailableGuhuo(room, player, room.GetRoomState().GetCurrentCardUseReason(),
                    room.GetRoomState().GetCurrentCardUsePattern(), cards[0].Name);

                return vcards;
            }

            return new List<WrappedCard>();
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && Engine.GetFunctionCard(cards[0].Name) != null)
            {
                WrappedCard card = new WrappedCard("YiguiCard")
                {
                    UserString = string.Format("{0}_{1}", cards[0].Name, cards[0].UserString)
                };

                return card;
            }

            return null;
        }
    }

    public class YiguiProhibt : ProhibitSkill
    {
        public YiguiProhibt() : base("#yigui-prohibit")
        {
        }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (RoomLogic.IsVirtualCard(room, card) && card.Skill == "yigui" && to != null && to.HasShownOneGeneral())
            {
                General general = Engine.GetGeneral(card.UserString);
                if (general == null)
                    room.OutPut("化身卡出错 " + card.UserString);

                return general != null && general.Kingdom != Engine.GetGeneral(to.ActualGeneral1).Kingdom;
            }

            return false;
        }
    }

    public class Jihun : TriggerSkill
    {
        public Jihun() : base("jihun")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.QuitDying };
            skill_type = SkillType.Masochism;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room))
                return new List<TriggerStruct> { new TriggerStruct(Name, player) };
            else if (triggerEvent == TriggerEvent.QuitDying && player.Alive)
            {
                List<Player> zuocis = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in zuocis)
                {
                    if (!RoomLogic.IsFriendWith(room, p, player))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Yigui.Acquiregenerals(room, ask_who, 1);
            return false;
        }
    }
        
    //shamoke
    public class Jili : TriggerSkill
    {
        public Jili() : base("jili")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.EventPhaseChanging, TriggerEvent.CardResponded };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (base.Triggerable(player, room))
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp)
                    card = resp.Card;
                if (card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card?.Name);
                    if (fcard.TypeID != CardType.TypeSkill)
                        room.SetPlayerMark(player, "jili", player.GetMark("jili") + 1);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded) && base.Triggerable(player, room))
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp)
                    card = resp.Card;
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard != null && fcard.TypeID != CardType.TypeSkill)
                {
                    int range = RoomLogic.GetAttackRange(room, player, true);
                    if (range > 0 && player.GetMark("jili") == range)
                    {
                        return new TriggerStruct(Name, player);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAllPlayers(true))
                    p.SetMark("jili", 0);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                List<string> list = player.ContainsTag(Name) ?  (List<string>)player.GetTag(Name) : new List<string>();
                list.Add(player.GetMark("jili").ToString());
                player.SetTag(Name, list);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> list = (List<string>)player.GetTag(Name);
            int range = int.Parse(list[list.Count -1]);
            list.RemoveAt(list.Count - 1);
            player.SetTag(Name, list);

            if (range > 0)
                room.DrawCards(player, range, Name);

            return false;
        }
    }
    //masu
    public class SanyaoCard : SkillCard
    {
        public SanyaoCard() : base("SanyaoCard") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0) return false;
            int max = -1000;
            foreach (Player p in room.GetAlivePlayers()) {
                if (max < p.Hp)
                    max = p.Hp;
            }
            return to_select.Hp == max;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.Damage(new DamageStruct("sanyao", effect.From, effect.To));
        }
    }  
    public class Sanyao : OneCardViewAsSkill
    {
        public Sanyao() : base("sanyao")
        {
            filter_pattern = "..!";
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "he") && !player.HasUsed("SanyaoCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard("SanyaoCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            first.AddSubCard(card);
            return first;
        }
    }
    public class Zhiman : TriggerSkill
    {
        public Zhiman() : base("zhiman")
        {
            events.Add(TriggerEvent.DamageCaused);
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.To != null && player != damage.To)
            {
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            room.SetTag("zhiman_data", data);  // for AI
            bool invoke = room.AskForSkillInvoke(player, Name, damage.To, info.SkillPosition);
            room.RemoveTag("zhiman_data");
            if (invoke)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetTag(Name, info.SkillPosition);
            DamageStruct damage = (DamageStruct)data;
            Player to = damage.To;
            LogMessage log = new LogMessage
            {
                Type = "#Zhiman",
                From = player.Name,
                Arg = Name,
                To = new List<string> { to.Name }
            };
            room.SendLog(log);
            to.SetMark(Name, 1);
            to.SetTag("zhiman_from", player.Name);
            return true;
        }
    }
    public class ZhimanSecond : TriggerSkill
    {
        public ZhimanSecond() : base("#zhiman-second")
        {
            events.Add(TriggerEvent.DamageComplete);
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            DamageStruct damage = (DamageStruct)data;
            if (damage.To == player && player.GetMark("zhiman") > 0)
            {
                Player masu = room.FindPlayer((string)player.GetTag( "zhiman_from"));
                if (damage.From == masu && RoomLogic.PlayerHasShownSkill(room, masu, "zhiman"))
                {
                    List<TriggerStruct> skill_list = new List<TriggerStruct>();
                    TriggerStruct trigger = new TriggerStruct(Name, masu)
                    {
                        SkillPosition = (string)masu.GetTag("zhiman")
                    };
                    skill_list.Add(trigger);
                    return skill_list;
                }
            }
            return new List<TriggerStruct>();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player to, ref object data, Player player, TriggerStruct info)
        {
            to.RemoveTag("zhiman_from");
            to.SetMark("zhiman", 0);
            if (RoomLogic.CanGetCard(room, player, to, "ej"))
            {
                int card_id = room.AskForCardChosen(player, to, "ej", "zhiman", false, HandlingMethod.MethodGet);
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name);
                room.ObtainCard(player, room.GetCard(card_id), reason);
            }
            if (player.GetMark(Name + "_transform") == 0 && RoomLogic.IsFriendWith(room, to, player) && RoomLogic.CanTransform(to)
                    && room.AskForSkillInvoke(to, "transform"))
            {
                //room.BroadcastSkillInvoke("transform", to.isMale());
                player.AddMark(Name + "_transform");
                room.TransformDeputyGeneral(to);
            }
            return false;
        }
    }
    //LengTong
    public class Xuanlue : TriggerSkill
    {
        public Xuanlue() : base("xuanlue")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From != null && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceEquip))
            {
                List<Player> other_players = room.GetOtherPlayers(move.From);
                foreach (Player p in other_players) {
                    if (RoomLogic.CanDiscard(room, move.From, p, "he"))
                        return new TriggerStruct(Name, move.From);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lengtong, TriggerStruct info)
        {
            List<Player> other_players = room.GetOtherPlayers(lengtong);
            List<Player> targets = new List<Player>();
            foreach (Player p in other_players) {
                if (RoomLogic.CanDiscard(room, lengtong, p, "he"))
                    targets.Add(p);
            }
            Player to = room.AskForPlayerChosen(lengtong, targets, Name, "liefeng-invoke", true, true, info.SkillPosition);
            if (to != null)
            {
                lengtong.SetTag("liefeng_target", to.Name);
                room.BroadcastSkillInvoke(Name, lengtong, info.SkillPosition);
                return info;
            }
            else lengtong.RemoveTag("liefeng_target");
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lengtong, TriggerStruct info)
        {
            Player to = room.FindPlayer((string)lengtong.GetTag("liefeng_target"));
            lengtong.RemoveTag("liefeng_target");
            if (to != null && RoomLogic.CanDiscard(room, lengtong, to, "he"))
            {
                List<int> ids = new List<int> { room.AskForCardChosen(lengtong, to, "he", Name, false, HandlingMethod.MethodDiscard) };
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISMANTLE, lengtong.Name, to.Name, Name, null);
                room.ThrowCard(ref ids, reason, to, lengtong);
            }
            return false;
        }
    }
    public class YongjinCard : SkillCard
    {
        public YongjinCard() : base("YongjinCard")
        {
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@yong", 0);
            room.BroadcastSkillInvoke("yongjin", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "yongjin");
            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            int n = 3;
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
        if (CanTransfer(room, p).Count > 0)
                targets.Add(p);

            string position = card_use.Card.SkillPosition;
            while (n > 0 && targets.Count > 0)
            {
                card_use.From.SetFlags("yongjin");     //for ai
                Player target = room.AskForPlayerChosen(card_use.From, targets, "yongjin", "@yongjin", true, false, position);
                card_use.From.SetFlags("-yongjin");     //for ai
                int card_id = -1;
                if (target != null)
                {
                    List<int> available_cards = CanTransfer(room, target);
                    List<int> disable_ids = new List<int>();
                    foreach (int id in target.GetEquips()) {
                        if (!available_cards.Contains(id))
                            disable_ids.Add(id);
                    }

                    card_id = room.AskForCardChosen(card_use.From, target, "e", "yongjin", false, HandlingMethod.MethodNone, disable_ids);
                }

                if (card_id >= 0)
                {
                    WrappedCard card = room.GetCard(card_id);
                    int equip_index = -1;
                    EquipCard equip = (EquipCard)(Engine.GetFunctionCard(card.Name));
                    equip_index = (int)equip.EquipLocation();

                    List<Player> tos = new List<Player>();
                    foreach (Player p in room.GetOtherPlayers(target)) {
                        if (p.GetEquip(equip_index) == -1)
                            tos.Add(p);
                    }

                    room.SetTag("YongjinTarget", target);     //for ai
                    Player to = room.AskForPlayerChosen(card_use.From, tos, "yongjin", "@yongjin-to:::" + card.Name, false, true, position);
                    room.RemoveTag("YongjinTarget");
                    if (to != null)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, to.Name);

                        room.MoveCardTo(card, target, to, Place.PlaceEquip,
                            new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TRANSFER, card_use.From.Name, "yongjin", null));
                    }
                }
                else
                    break;

                --n;
                targets.Clear();
                foreach (Player p in room.GetAlivePlayers())
            if (CanTransfer(room, p).Count > 0)
                    targets.Add(p);
            }
            card_use.From.SetFlags("-yongjin");
        }
        List<int> CanTransfer(Room room, Player player)
        {
            List<int> cards = new List<int>();
            if (player.GetEquips().Count > 0)
            {
                foreach (Player p in room.GetOtherPlayers(player)) {
                    foreach (int id in player.GetEquips()) {
                        WrappedCard card = room.GetCard(id);
                        EquipCard equip = (EquipCard)(Engine.GetFunctionCard(card.Name));
                        int equip_index = (int)equip.EquipLocation();
                        if (p.GetEquip(equip_index) == -1 && !cards.Contains(id))
                            cards.Add(id);
                    }
                }
            }
            return cards;
        }
    }
    public class Yongjin : ZeroCardViewAsSkill
    {
        public Yongjin() : base("yongjin")
        {
            frequency = Frequency.Limited;
            limit_mark = "@yong";
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("YongjinCard")
            {
                Skill = Name,
                ShowSkill = Name,
                Mute = true
            };
            return card;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            bool check = false;
            foreach (Player p in room.GetAlivePlayers()) {
                if (p.HasEquip())
                {
                    check = true;
                    break;
                }
            }
            return check && player.GetMark("@yong") >= 1;
        }
    }
    //lvfan
     public class Diaodu : TriggerSkill
    {
        public Diaodu() : base("diaodu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardUsed };
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && player.Alive)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is EquipCard)
                {
                    List<Player> lfs = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player p in lfs)
                    {
                        if (RoomLogic.IsFriendWith(room, player, p))
                            result.Add(new TriggerStruct(Name, player, p));
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (RoomLogic.IsFriendWith(room, player, p) && p.HasEquip() && RoomLogic.CanGetCard(room, player, p, "e"))
                        return new List<TriggerStruct> { new TriggerStruct(Name, player) };
            }

            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsed && player.Alive && room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, info.SkillOwner, player.Name);
                room.BroadcastSkillInvoke(Name, room.FindPlayer(info.SkillOwner), info.SkillPosition);
                return info;
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if (RoomLogic.IsFriendWith(room, player, p) && p.HasEquip() && RoomLogic.CanGetCard(room, player, p, "e"))
                        targets.Add(p);

                Player target = room.AskForPlayerChosen(player, targets, Name, "@diaodu", true, true, info.SkillPosition);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);

                player.SetTag(Name, target.Name);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsed && ask_who.Alive)
                room.DrawCards(ask_who, 1, Name);
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.ContainsTag(Name) && player.GetTag(Name) is string target_name)
            {
                Player target = room.FindPlayer(target_name);
                if (target != null && target.HasEquip() && RoomLogic.CanGetCard(room, player, target, "e"))
                {
                    int id = -1;
                    if (target == player)
                    {
                        List<int> ids = room.AskForExchange(player, Name, 1, 1, "@diaodu-get", string.Empty, ".|.|.|equipped", info.SkillPosition);
                        if (ids.Count == 1)
                            id = ids[0];
                    }
                    else
                    {
                        id = room.AskForCardChosen(player, target, "e", Name, false, HandlingMethod.MethodGet);
                    }

                    if (id == -1)
                    {
                        List<int> ids = target.GetEquips();
                        Shuffle.shuffle(ref ids);
                        foreach (int card_id in ids)
                        {
                            if (RoomLogic.CanGetCard(room, player, target, card_id))
                            {
                                id = card_id;
                                break;
                            }
                        }
                    }

                    if (id >= 0)
                    {
                        List<int> ids = new List<int> { id };
                        room.ObtainCard(player, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, Name, string.Empty));

                        if (ids.Count == 1 && room.GetCardPlace(ids[0]) == Place.PlaceHand && room.GetCardOwner(ids[0]) == player)
                        {
                            List<Player> targets = room.GetOtherPlayers(target);
                            targets.Remove(player);
                            Player second = room.AskForPlayerChosen(player, targets, Name, "@diaodu-give:::" + room.GetCard(ids[0]).Name, true, false, info.SkillPosition);
                            if (second != null)
                                room.ObtainCard(second, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, second.Name, Name, string.Empty));
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Diancai : TriggerSkill
    {
        public Diancai() : base("diancai")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Defense;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.Play)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    p.SetMark(Name, 0);
            }
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move
                && move.From != null && base.Triggerable(move.From, room) && move.From != room.Current
                && (move.From_places.Contains(Place.PlaceHand) || move.From_places.Contains(Place.PlaceEquip))
                && !(move.To == move.From && (move.To_place == Place.PlaceHand || move.To_place == Place.PlaceEquip)))
                move.From.SetMark(Name, move.From.GetMark(Name) + move.Card_ids.Count);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (!(triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play)) return new List<TriggerStruct>();
            List<Player> players = RoomLogic.FindPlayersBySkillName(room, Name);
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            foreach (Player p in players) {
                if (base.Triggerable(p, room) && p.GetMark(Name) > 0 && p.GetMark(Name) >= p.Hp)
                    skill_list.Add(new TriggerStruct(Name, p));
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.SetMark(Name, 0);
            if (room.AskForSkillInvoke(ask_who, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.HandcardNum < ask_who.MaxHp)
                room.DrawCards(ask_who, ask_who.MaxHp - ask_who.HandcardNum, Name);

            if (ask_who.GetMark(Name + "_transform") == 0 && RoomLogic.CanTransform(ask_who) && room.AskForSkillInvoke(ask_who, "transform"))
            {
                //room.BroadcastSkillInvoke("transform", ask_who.isMale());
                ask_who.AddMark(Name + "_transform");
                room.TransformDeputyGeneral(ask_who);
            }
            return false;
        }
    }

    //lord_sunquan
    public class LianziCard : SkillCard
    {
        public LianziCard() : base("LianziCard")
        {
            target_fixed = true;
            will_throw = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            int num = card_use.From.GetPile("flame_map").Count;
            foreach (Player p in room.GetAllPlayers()) {
                if (p.HasShownOneGeneral() && p.Kingdom == "wu" && p.Role != "careerist")
                    num = num + p.GetEquips().Count;
            }

            List<int> card_ids = room.GetNCards(num);
            if (num == 0) return;

            CardType type = Engine.GetFunctionCard(room.GetCard(card_use.Card.GetEffectiveId()).Name).TypeID;
            List<int> gets = new List<int>(), drops = new List<int>();

            foreach (int id in card_ids) {
                if (Engine.GetFunctionCard(room.GetCard(id).Name).TypeID == type)
                    gets.Add(id);
                else
                    drops.Add(id);
                room.MoveCardTo(room.GetCard(id), card_use.From, Place.PlaceTable, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TURNOVER, card_use.From.Name, "lianzi", null), false);
                Thread.Sleep(400);
            }
            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(gets, card_use.From, Place.PlaceHand, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTBACK, card_use.From.Name, "lianzi", null)) },
                true);
            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(drops, null, Place.DiscardPile, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, null, "lianzi", null)) },
                true);

            if (gets.Count > 3)
                room.HandleAcquireDetachSkills(card_use.From, new List<string> { "-lianzi", "zhiheng" }, false);
        }
    }
    public class Lianzi : OneCardViewAsSkill
    {
        public Lianzi() : base("lianzi")
        {
            filter_pattern = ".|.|.|hand!";
            skill_type = SkillType.Replenish;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "h") && !player.HasUsed("LianziCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard("LianziCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            first.AddSubCard(card);
            return first;
        }
    }

    public class Jubao : TriggerSkill
    {
        public Jubao() : base("jubao")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Finish && base.Triggerable(player, room))
            {
                foreach (int id in room.DiscardPile)
                {
                    if (room.GetCard(id).Name == "LuminouSpearl")
                        return new TriggerStruct(Name, player);
                }

                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetTreasure() && p.Treasure.Value == "LuminouSpearl")
                        return new TriggerStruct(Name, player);

                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.DrawCards(player, 1, Name);
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            foreach (Player p in room.GetAlivePlayers()) {
                if (p.GetTreasure() && p.Treasure.Value == "Luminouspearl" && RoomLogic.CanGetCard(room, player, p, "he"))
                {
                    int card_id = room.AskForCardChosen(player, p, "he", Name, false, HandlingMethod.MethodGet);
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name);
                    CardsMoveStruct move = new CardsMoveStruct(card_id, player, Place.PlaceHand, reason);
                    moves.Add(move);
                }
            }
            if (moves.Count > 0) room.MoveCardsAtomic(moves, true);
            return false;
        }
    }

    public class JubaoCardFixed : FixCardSkill
    {
        public JubaoCardFixed() : base("#jubao-treasure")
        {
        }
        public override bool IsCardFixed(Room room, Player from, Player to, string flags, HandlingMethod method)
        {
            if (from != to && method == HandlingMethod.MethodGet && RoomLogic.PlayerHasShownSkill(room, to, "jubao") && flags.Contains("t"))
                return true;

            return false;
        }
    }
    public class FlameMapCard:SkillCard
    {
        public FlameMapCard() : base("FlameMapCard")
        {
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            room.ShowSkill(source, card_use.Card.ShowSkill, null);
            Player sunquan = RoomLogic.GetLord(room, source.Kingdom);
            room.SetCardFlag(card_use.Card.SubCards[0], "flame_map");
            room.AddToPile(sunquan, "flame_map", card_use.Card.SubCards, true, room.GetAllPlayers(), new CardMoveReason(CardMoveReason.MoveReason.S_REASON_UNKNOWN, source.Name));
        }
    }
    public class FlameMapVS : OneCardViewAsSkill
    {
        public FlameMapVS() : base("flamemap")
        {
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(to_select.Name);
            return fcard is EquipCard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            Player sunquan = RoomLogic.GetLord(room, player.Kingdom);
            if (sunquan != null && RoomLogic.PlayerHasShownSkill(room, sunquan, "jiahe") && RoomLogic.WillBeFriendWith(room, player, sunquan)
                && !player.HasUsed("FlameMapCard") && player.CanShowGeneral(null))
                return true;

            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard slash = new WrappedCard("FlameMapCard");
            slash.AddSubCard(card);
            slash.Mute = true;
            slash.ShowSkill = "showforviewhas";
            return slash;
        }
    }
    public class FlameMap : TriggerSkill
    {
        public FlameMap() : base("flamemap")
        {
            events.Add(TriggerEvent.Damaged);
            view_as_skill = new FlameMapVS();
            attached_lord_skill = true;
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && player.GetPile("flame_map").Count > 0 && damage.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash || fcard is TrickCard)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> maps = player.GetPile("flame_map");
            if (maps.Count > 0)
            {
                int card_id;
                if (maps.Count == 1)
                    card_id = maps[0];
                else
                {
                    room.FillAG(Name, maps, player);
                    card_id = room.AskForAG(player, maps, false, Name);
                    room.ClearAG(player);
                }

                LogMessage log = new LogMessage
                {
                    Type = "$FlameMapRemove",
                    From = player.Name,
                    Card_str = room.GetCard(card_id).ToString()
                };
                room.SendLog(log);

                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_PILE, null, Name, null);
                List<int> ids = new List<int> { card_id };
                room.ThrowCard(ref ids, reason, null);
                room.ClearAG(player);
            }
            return false;
        }
    }
    public class FlameMapSkill : TriggerSkill
    {
        public FlameMapSkill() : base("flamemapskill")
        {
            global = true;
            events.Add(TriggerEvent.EventPhaseStart);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                List<string> skills = new List<string>();
                foreach (string skill in (List<string>)player.GetTag(Name))
                skills.Add("-" + skill);

                player.SetMark(Name, 0);
                player.RemoveTag(Name);
                room.HandleAcquireDetachSkills(player, skills, true);
            }
            else if (player.Phase == PlayerPhase.Start)
            {
                Player sunquan = RoomLogic.GetLord(room, player.Kingdom);
                if (sunquan != null && sunquan.Alive && sunquan.GetPile("flame_map").Count > 0 && player.HasShownOneGeneral() && player.Kingdom == "wu")
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player sunquan = RoomLogic.GetLord(room, player.Kingdom);
            int num = sunquan.GetPile("flame_map").Count;
            List<string> skills = new List<string>(), answers = new List<string>();
            if (num >= 1)
                skills.Add("yingziextra");
            if (num >= 2)
                skills.Add("haoshiextra");
            if (num >= 3)
                skills.Add("shelie");
            if (num >= 4)
                skills.Add("duoshi");
            skills.Add("cancel");

            string answer = room.AskForChoice(player, Name, string.Join("+", skills));
            if (answer != "cancel")
            {
                answers.Add(answer);
                player.SetMark(Name, 1);
                player.SetTag(Name, answers);
                room.HandleAcquireDetachSkills(player, answers);
            }

            if (num >= 5 && answers.Count > 0)
            {
                skills.Remove(answer);
                answer = room.AskForChoice(player, Name, string.Join("+", skills));
                if (answer != "cancel")
                {
                    answers.Add(answer);
                    player.SetTag(Name, answers);
                    room.HandleAcquireDetachSkills(player, answer);
                }
            }

            return false;
        }
    }
    public class YingziExtra : DrawCardsSkill
    {
        public YingziExtra() : base("yingziextra")
        {
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }
        /*
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw)
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        */
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            return info;
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            return n + 1;
        }
    }

    public class HaoshiExtra : DrawCardsSkill
    {
        public HaoshiExtra() : base("haoshiextra")
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
            player.SetFlags("haoshi");
            return n + 2;
        }
    }

    public class Shelie : TriggerSkill
    {
        public Shelie() : base("shelie")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw)
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> card_ids = room.GetNCards(5);
            foreach (int id in card_ids) {
                room.MoveCardTo(room.GetCard(id), player, Place.PlaceTable, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TURNOVER, player.Name, Name, null), false);
                Thread.Sleep(400);
            }
            AskForMoveCardsStruct result = room.AskForMoveCards(player, card_ids, new List<int>(), true, Name, 1, 4, false, true, card_ids, info.SkillPosition);

            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(result.Bottom, player, Place.PlaceHand, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTBACK, player.Name, Name, null)) },
                true);
            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(result.Top, null, Place.DiscardPile, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, null, Name, null)) },
                true);

            return true;
        }
        public override bool MoveFilter(Room room, int id, List<int> downs)
        {
            if (id != -1)
            {
                WrappedCard card = room.GetCard(id);
                foreach (int card_id in downs)
                    if (card.Suit == room.GetCard(card_id).Suit)
                        return false;
            }

            return true;
        }
    }

    public class Jiahe : TriggerSkill
    {
        public Jiahe() : base("jiahe")
        {
            lord_skill = true;
            events = new List<TriggerEvent> { TriggerEvent.GeneralShown, TriggerEvent.Death, TriggerEvent.DFDebut };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player == null)
                return new TriggerStruct();
            
            if (triggerEvent == TriggerEvent.GeneralShown && player.General1Showed)
            {
                if (base.Triggerable(player, room))
                {
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (RoomLogic.WillBeFriendWith(room, p, player))
                        {
                            room.AttachSkillToPlayer(p, "flamemap");
                        }
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Who == player && RoomLogic.PlayerHasSkill(room, player, Name))
            {
                foreach (Player p in room.GetAlivePlayers())
                    room.DetachSkillFromPlayer(p, "flamemap");
            }
            else if (triggerEvent == TriggerEvent.DFDebut)
            {
                Player sunquan = RoomLogic.GetLord(room, player.Kingdom);
                if (sunquan != null && RoomLogic.PlayerHasShownSkill(room, sunquan, Name) && !player.GetAcquiredSkills().Contains("flamemap"))
                    room.AttachSkillToPlayer(player, "flamemap");
            }

            return new TriggerStruct();
        }
    }

    public class JiaheClear : DetachEffectSkill
    {
        public JiaheClear() : base("jiahe", "flame_map")
        {
        }
        public override void OnSkillDetached(Room room, Player player, object data)
        {
            foreach (Player p in room.GetAlivePlayers())
                room.DetachSkillFromPlayer(p, "flamemap");
        }
    }
}