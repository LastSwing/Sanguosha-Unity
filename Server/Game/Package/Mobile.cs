using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;
using System.Linq;

namespace SanguoshaServer.Package
{
    public class Mobile : GeneralPackage
    {
        public Mobile() : base("Mobile")
        {
            skills = new List<Skill>
            {
                new Qiancong(),
                new QiancongTar(),
                new QiancongVH(),
                new Shangjian(),
                new Zhanyi(),
                new ZhanyiEffect(),

                new Zhaohuo(),
                new Yixiang(),
                new Yirang(),
                new Kuangcai(),
                new KuangcaiDraw(),
                new KuangcaiTar(),
                new Shejian(),

                new Renshi(),
                new Wuyuan(),
                new Huaizi(),

                new Yingjian(),
                new Shixin(),
                new Fenyin(),
                new FenyinRecord(),
            };

            skill_cards = new List<FunctionCard>
            {
                new ZhanyiCard(),
                new WuyuanCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "kuangcai", new List<string>{ "#kuangcai", "#kuangcai-tar" } },
                { "fenyin", new List<string>{ "#fenyin" } },
                { "qiancong", new List<string>{ "#qiancong", "#qiancong-tar" } },
                { "zhanyi", new List<string>{ "#zhanyi" } },
            };
        }
    }

    public class Qiancong : TriggerSkill
    {
        public Qiancong() : base("qiancong")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Compulsory;
        }

        public override bool Triggerable(Player target, Room room)
        {
            if (base.Triggerable(target, room) && target.Phase == PlayerPhase.Play)
            {
                bool red = false, black = false;
                foreach (int id in target.GetEquips())
                {
                    if (WrappedCard.IsBlack(room.GetCard(id).Suit))
                        black = true;
                    else
                        red = true;
                }

                return (!red && !black) || (red && black);
            }

            return false;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            string choice = room.AskForChoice(player, Name, "basic+trick");
            LogMessage log = new LogMessage
            {
                Type = "#nolimit",
                From = player.Name
            };
            if (choice == "basic")
            {
                player.SetFlags("qiancong_basic");
                log.Arg = "basic";
            }
            else
            {
                player.SetFlags("qiancong_trick");
                log.Arg = "trick";
            }
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.SendLog(log);

            return false;
        }
    }

    public class QiancongTar : TargetModSkill
    {
        public QiancongTar() : base("#qiancong-tar", false) { pattern = "."; }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseStruct.CardUseReason reason, string pattern)
        {
            if (from != null && (Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeBasic && from.HasFlag("qiancong_basic"))
                || (Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeTrick && from.HasFlag("qiancong_trick")))
            {
                return true;
            }

            return false;
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeBasic && from != null && from.HasFlag("qiancong_basic"))
            {
                return 999;
            }

            return 0;
        }
    }

    public class QiancongVH : ViewHasSkill
    {
        public QiancongVH() : base("#qiancong")
        {
            viewhas_skills = new List<string> { "weimu_jx", "mingzhe" };
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (player.Alive && RoomLogic.PlayerHasSkill(room, player, Name) && player.HasEquip())
            {
                List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
                foreach (int id in player.GetEquips())
                {
                    WrappedCard.CardSuit suit = room.GetCard(id).Suit;
                    if (!suits.Contains(suit)) suits.Add(suit);
                }

                if (skill_name == "weimu_jx" && !suits.Contains(WrappedCard.CardSuit.Diamond) && !suits.Contains(WrappedCard.CardSuit.Heart)) return true;
                if (skill_name == "mingzhe" && !suits.Contains(WrappedCard.CardSuit.Club) && !suits.Contains(WrappedCard.CardSuit.Spade)) return true;
            }
            return false;
        }
    }

    public class Shangjian : TriggerSkill
    {
        public Shangjian() : base("shangjian")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0) p.SetMark(Name, 0);
            }
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.NotActive
                && (move.From_places.Contains(Place.PlaceHand) || move.From_places.Contains(Place.PlaceEquip))
                && !(move.To == move.From && (move.To_place == Place.PlaceHand || move.To_place == Place.PlaceEquip)))
                move.From.AddMark(Name, move.Card_ids.Count);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish)
            {
                List<Player> players = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in players)
                {
                    if (p.GetMark(Name) > 0 && p.GetMark(Name) <= p.Hp)
                        skill_list.Add(new TriggerStruct(Name, p));
                }
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = ask_who.GetMark(Name);
            room.DrawCards(ask_who, count, Name);
            return false;
        }
    }

    public class Zhanyi : ViewAsSkill
    {
        public Zhanyi() : base("zhanyi")
        {
            skill_type = SkillType.Attack;
            response_or_use = true;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 0) return false;

            if (!player.HasUsed(ZhanyiCard.ClassName))
                return (room.GetCardPlace(to_select.Id) == Place.PlaceHand || room.GetCardPlace(to_select.Id) == Place.PlaceEquip)
                    && !RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodDiscard);
            else if (player.HasFlag("zhanyi_basic"))
                return Engine.GetFunctionCard(to_select.Name).TypeID == CardType.TypeBasic;

            return false;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ZhanyiCard.ClassName) || player.HasFlag("zhanyi_basic");
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (player.HasFlag("zhanyi_basic") && room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                WrappedCard peach = new WrappedCard(Peach.ClassName);
                WrappedCard ana = new WrappedCard(Analeptic.ClassName);

                return Engine.MatchExpPattern(room, pattern, player, slash) || Engine.MatchExpPattern(room, pattern, player, jink)
                    || Engine.MatchExpPattern(room, pattern, player, peach) || Engine.MatchExpPattern(room, pattern, player, ana);
            }

            return false;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player Self)
        {
            List<WrappedCard> all_cards = new List<WrappedCard>();
            if (Self.HasFlag("zhanyi_basic") && cards.Count == 1)
            {
                CardUseStruct.CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
                if (reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
                {
                    List<string> names = GetGuhuoCards(room, "b");
                    foreach (string name in names)
                    {
                        if (name == Jink.ClassName) continue;
                        WrappedCard card = new WrappedCard(name);
                        card.AddSubCards(cards);
                        all_cards.Add(card);
                    }
                }
                else
                {
                    string pattern = room.GetRoomState().GetCurrentCardUsePattern(Self);
                    WrappedCard slash = new WrappedCard(Slash.ClassName);
                    slash.AddSubCards(cards);
                    WrappedCard fslash = new WrappedCard(FireSlash.ClassName);
                    fslash.AddSubCards(cards);
                    WrappedCard tslash = new WrappedCard(ThunderSlash.ClassName);
                    tslash.AddSubCards(cards);
                    WrappedCard jink = new WrappedCard(Jink.ClassName);
                    jink.AddSubCards(cards);
                    WrappedCard peach = new WrappedCard(Peach.ClassName);
                    peach.AddSubCards(cards);
                    WrappedCard ana = new WrappedCard(Analeptic.ClassName);
                    ana.AddSubCards(cards);

                    if (Engine.MatchExpPattern(room, pattern, Self, slash))
                        all_cards.Add(slash);
                    if (Engine.MatchExpPattern(room, pattern, Self, fslash))
                        all_cards.Add(fslash);
                    if (Engine.MatchExpPattern(room, pattern, Self, tslash))
                        all_cards.Add(tslash);
                    if (Engine.MatchExpPattern(room, pattern, Self, jink))
                        all_cards.Add(jink);
                    if (Engine.MatchExpPattern(room, pattern, Self, peach))
                        all_cards.Add(peach);
                    if (Engine.MatchExpPattern(room, pattern, Self, ana))
                        all_cards.Add(ana);
                }
            }

            return all_cards;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count != 1) return null;

            if (!player.HasUsed(ZhanyiCard.ClassName))
            {
                WrappedCard zy = new WrappedCard(ZhanyiCard.ClassName) { Skill = Name };
                zy.AddSubCards(cards);
                return zy;
            }
            else if (player.HasFlag("zhanyi_basic") && cards[0].IsVirtualCard())
            {
                WrappedCard card = RoomLogic.ParseUseCard(room, cards[0]);
                card.Skill = Name;
                return card;
            }

            return null;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            if (card.Name == ZhanyiCard.ClassName)
                index = 1;
            else
                index = -2;
        }
    }

    public class ZhanyiCard : SkillCard
    {
        public static string ClassName = "ZhanyiCard";
        public ZhanyiCard() : base(ClassName) { target_fixed = true; }
        public override void Use(Room room, CardUseStruct card_use)
        {
            WrappedCard card = room.GetCard(card_use.Card.GetEffectiveId());
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            Player player = card_use.From;
            room.LoseHp(player);
            if (!player.Alive) return;

            switch (fcard.TypeID)
            {
                case CardType.TypeBasic:
                    player.SetFlags("zhanyi_basic");
                    break;
                case CardType.TypeEquip:
                    player.SetFlags("zhanyi_equip");
                    break;
                case CardType.TypeTrick:
                    player.SetFlags("zhanyi_trick");
                    room.DrawCards(player, 3, "zhanyi");
                    break;
            }
        }
    }

    public class ZhanyiEffect : TriggerSkill
    {
        public ZhanyiEffect() : base("#zhanyi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.TargetChosen, TriggerEvent.TrickCardCanceling,
                TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play
                && (player.HasFlag("zhanyi_basic") || player.HasFlag("zhanyi_trick") || player.HasFlag("zhanyi_equip")))
            {
                player.SetFlags("-zhanyi_basic");
                player.SetFlags("-zhanyi_trick");
                player.SetFlags("-zhanyi_equip");
                player.SetMark(Name, 0);
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && player.HasFlag("zhanyi_basic"))
                player.AddMark(Name);
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && player.HasFlag("zhanyi_basic")
                && Engine.GetFunctionCard(use.Card.Name).TypeID == CardType.TypeBasic)
                player.AddMark(Name);
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.PlaceTable && move.From != null && move.Reason.Card == null
                && move.Reason.Reason == MoveReason.S_REASON_THROW && move.Reason.SkillName == "zhanyi" && move.From.Name == move.Reason.PlayerId && move.Card_ids.Count > 0)
            {
                move.From.SetTag(Name, move.Card_ids);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && player.GetMark(Name) == 1 && player.HasFlag("zhanyi_basic")
                && (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == Peach.ClassName
                || (use.Card.Name == Analeptic.ClassName && player.HasFlag("Global_Dying") && use.To.Contains(player))))
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && effect.From != null && effect.From.Alive && effect.From.HasFlag("zhanyi_trick"))
            {
                return new TriggerStruct(Name, effect.From);
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct _use && _use.Card.Name.Contains(Slash.ClassName) && player.HasFlag("zhanyi_equip"))
            {
                return new TriggerStruct(Name, player, _use.To);
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, "zhanyi", info.SkillPosition);
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use)
            {
                room.BroadcastSkillInvoke("zhanyi", "male", 2, gsk.General, gsk.SkinId);

                use.ExDamage++;
                data = use;

                if (use.Card.Name.Contains(Slash.ClassName))
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#card-damage",
                        From = player.Name,
                        Arg = Name,
                        Arg2 = use.Card.Name
                    };
                    room.SendLog(log);
                }
                else
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#card-recover",
                        From = player.Name,
                        Arg = Name,
                        Arg2 = use.Card.Name
                    };

                    room.SendLog(log);
                }
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling)
                return true;
            else if (triggerEvent == TriggerEvent.TargetChosen)
            {
                if (!player.IsNude())
                {
                    room.BroadcastSkillInvoke("zhanyi", "male", 2, gsk.General, gsk.SkinId);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                    room.AskForDiscard(player, "zhanyi", 2, 2, false, true, "@zhanyi:" + ask_who.Name);

                    if (player.ContainsTag(Name) && player.GetTag(Name) is List<int> ids)
                    {
                        player.RemoveTag(Name);

                        List<int> get = new List<int>();
                        foreach (int id in ids)
                            if (room.GetCardPlace(id) == Place.DiscardPile)
                                get.Add(id);

                        if (ask_who.Alive && get.Count > 0)
                        {
                            if (get.Count > 1)
                            {
                                room.FillAG("zhanyi", get, ask_who, null, null, "@zhanyi-get");
                                int card_id = room.AskForAG(ask_who, get, false, "zhanyi");
                                room.ClearAG();
                                get = new List<int> { card_id };
                            }

                            room.ObtainCard(ask_who, ref get, new CardMoveReason(MoveReason.S_REASON_RECYCLE, ask_who.Name, "zhanyi", string.Empty));
                        }
                    }
                 }
            }

            return false;
        }
    }


    public class Zhaohuo : TriggerSkill
    {
        public Zhaohuo() : base("zhaohuo")
        {
            skill_type = SkillType.Masochism;
            events = new List<TriggerEvent> { TriggerEvent.Dying };
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.Dying)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p != player && p.MaxHp > 1) triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }


        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);

            int count = ask_who.MaxHp - 1;
            room.LoseMaxHp(ask_who, count);
            if (ask_who.Alive)
                room.DrawCards(ask_who, count, Name);

            return false;
        }
    }

    public class Yixiang : TriggerSkill
    {
        public Yixiang() : base("yixiang")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Defense;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room) && use.From != null && use.From.Alive && use.From.Hp > player.Hp
                && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill && !player.HasFlag(Name))
            {
                bool slash = false, jink = false, peach = false, ana = false;
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name.Contains(Slash.ClassName))
                        slash = true;
                    else if (card.Name == Jink.ClassName)
                        jink = true;
                    else if (card.Name == Peach.ClassName)
                        peach = true;
                    else if (card.Name == Analeptic.ClassName)
                        ana = true;
                }
                if (!slash || !jink || !peach || !ana)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data , info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);

            bool slash = false, jink = false, peach = false, ana = false;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name.Contains(Slash.ClassName))
                    slash = true;
                else if (card.Name == Jink.ClassName)
                    jink = true;
                else if (card.Name == Peach.ClassName)
                    peach = true;
                else if (card.Name == Analeptic.ClassName)
                    ana = true;
            }

            List<int> get = new List<int>();
            foreach (int id in room.DrawPile)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name.Contains(Slash.ClassName) && !slash)
                {
                    get.Add(id);
                    break;
                }
                else if (card.Name == Jink.ClassName && !jink)
                {
                    get.Add(id);
                    break;
                }
                else if (card.Name == Peach.ClassName && !peach)
                {
                    get.Add(id);
                    break;
                }
                else if (card.Name == Analeptic.ClassName && !ana)
                {
                    get.Add(id);
                    break;
                }
            }

            if (get.Count > 0)
                room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty));

            return false;
        }
    }

    public class Yirang : TriggerSkill
    {
        public Yirang() : base("yirang")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Recover;
        }

        public override bool Triggerable(Player target, Room room)
        {
            if (base.Triggerable(target, room) && target.Phase == PlayerPhase.Play && !target.IsNude())
            {
                bool check = false;
                foreach (int id in target.GetCards("he"))
                {
                    if (Engine.GetFunctionCard(room.GetCard(id).Name).TypeID != CardType.TypeBasic)
                    {
                        check = true;
                        break;
                    }
                }

                if (check)
                {
                    foreach (Player p in room.GetOtherPlayers(target))
                        if (p.MaxHp > target.MaxHp)
                            return true;
                }
            }

            return false;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.MaxHp > player.MaxHp) targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@yirang", true, true, info.SkillPosition);
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

            int count = target.MaxHp - player.MaxHp;
            bool equip = false, trick = false;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                CardType type = Engine.GetFunctionCard(room.GetCard(id).Name).TypeID;
                if (type == CardType.TypeBasic) continue;
                if (type == CardType.TypeTrick)
                    trick = true;
                else if (type == CardType.TypeEquip)
                    equip = true;

                ids.Add(id);
            }
            int heal = 0;
            if (equip) heal++;
            if (trick) heal++;

            ResultStruct result = player.Result;
            result.Assist += ids.Count;
            player.Result = result;

            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);

            if (player.Alive)
            {
                player.MaxHp += count;
                room.BroadcastProperty(player, "MaxHp");

                LogMessage log = new LogMessage
                {
                    Type = "$GainMaxHp",
                    From = player.Name,
                    Arg = "1"
                };
                room.SendLog(log);

                room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, player);
            }
            if (player.Alive && player.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = Math.Min(heal, player.GetLostHp())
                };
                room.Recover(player, recover, true);
            }

            return false;
        }
    }

    public class Kuangcai : TriggerSkill
    {
        public Kuangcai() : base("kuangcai")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
            {
                room.SetPlayerFlag(player, "-kuangcai");
                room.SetPlayerMark(player, Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetPlayerFlag(player, Name);
            room.SetPlayerStringMark(player, Name, "0");
            return false;
        }
    }

    public class KuangcaiDraw : TriggerSkill
    {
        public KuangcaiDraw() : base("#kuangcai")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && player.HasFlag("kuangcai") && data is CardUseStruct use
                && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill && player.GetMark("kuangcai") < 5)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardResponded && player.HasFlag("kuangcai") && data is CardResponseStruct resp && resp.Use && player.GetMark("kuangcai") < 5)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, "kuangcai");
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, "kuangcai", info.SkillPosition);
            room.BroadcastSkillInvoke("kuangcai", "male", 2, gsk.General, gsk.SkinId);
            room.AddPlayerMark(player, "kuangcai");
            room.SetPlayerStringMark(player, "kuangcai", player.GetMark("kuangcai").ToString());

            if (player.GetMark("kuangcai") >= 5)
                player.SetFlags("Global_PlayPhaseTerminated");

            room.DrawCards(player, 1, "kuangcai");

            return false;
        }
    }

    public class KuangcaiTar : TargetModSkill
    {
        public KuangcaiTar() : base("#kuangcai-tar", false) { pattern = "."; }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseStruct.CardUseReason reason, string pattern) => from.HasFlag("kuangcai") ? true : false;
        public override int GetResidueNum(Room room, Player from, WrappedCard card) => from.HasFlag("kuangcai") ? 999 : 0;
    }

    public class Shejian : TriggerSkill
    {
        public Shejian() : base("shejian")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && base.Triggerable(move.From, room)
                && move.From.Phase == PlayerPhase.Discard && move.From == room.Current && move.Reason.PlayerId == move.From.Name
                && (move.From_places.Contains(Place.PlaceEquip) || move.From_places.Contains(Place.PlaceHand))
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
            {
                List<int> ids = move.From.ContainsTag(Name) ? (List<int>)move.From.GetTag(Name) : new List<int>();
                foreach (int id in move.Card_ids)
                    if (!ids.Contains(id)) ids.Add(id);

                move.From.SetTag(Name, ids);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Discard && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && base.Triggerable(player, room) && player.ContainsTag(Name)
                && player.GetTag(Name) is List<int> ids)
            {
                if (ids.Count > 1)
                {
                    bool check = true;
                    for (int x = 0; x < ids.Count; x++)
                    {
                        WrappedCard card = room.GetCard(ids[x]);
                        for (int y = x + 1; y < ids.Count; y++)
                        {
                            WrappedCard card2 = room.GetCard(ids[y]);
                            if (card.Suit == card2.Suit)
                            {
                                check = false;
                                break;
                            }
                        }

                        if (!check) break;
                    }

                    if (check) return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player erzhang, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (!p.IsNude() && RoomLogic.CanDiscard(room, player, p, "he"))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@shejian", true, true, info.SkillPosition);
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
            int id = room.AskForCardChosen(ask_who, target, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
            room.ThrowCard(id, target, ask_who);

            return false;
        }
    }

    public class Renshi : TriggerSkill
    {
        public Renshi() : base("renshi")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
        }
        

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageInflicted && base.Triggerable(player, room) && player.IsWounded() && data is DamageStruct damage
                && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (data is DamageStruct damage)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#damaged-prevent",
                    From = player.Name,
                    Arg = Name
                };
                room.SendLog(log);

                List<int> ids = new List<int>(damage.Card.SubCards), subs = room.GetSubCards(damage.Card);
                if (ids.SequenceEqual(subs))
                {
                    bool check = true;
                    foreach (int id in ids)
                    {
                        if (room.GetCardPlace(id) != Place.PlaceTable)
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        room.RemoveSubCards(damage.Card);
                        room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, player.Name, Name, string.Empty));
                    }

                    if (player.Alive) room.LoseMaxHp(player);
                }
            }

            return true;
        }
    }

    public class Wuyuan : OneCardViewAsSkill
    {
        public Wuyuan() : base("wuyuan")
        {
            filter_pattern = "Slash";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(WuyuanCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard wy = new WrappedCard(WuyuanCard.ClassName) { Skill = Name };
            wy.AddSubCard(card);
            return wy;
        }
    }

    public class WuyuanCard : SkillCard
    {
        public static string ClassName = "WuyuanCard";
        public WuyuanCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0], player = card_use.From;

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            WrappedCard slash = room.GetCard(card_use.Card.GetEffectiveId());
            bool red = WrappedCard.IsRed(slash.Suit);
            string card_name = slash.Name;

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "wuyuan", null);
            room.ObtainCard(target, card_use.Card, reason, false);

            if (player.Alive && player.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(player, recover, true);
            }
            if (target.Alive)
            {
                int count = 1;
                if (card_name != Slash.ClassName) count++;
                room.DrawCards(target, new DrawCardStruct(count, player, "wuyuan"));
            }
            if (red && target.Alive && target.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(target, recover, true);
            }
        }
    }

    public class Huaizi : MaxCardsSkill
    {
        public Huaizi() : base("huaizi")
        {
        }
        public override int GetFixed(Room room, Player target) => RoomLogic.PlayerHasShownSkill(room, target, this) ? target.MaxHp : -1;
    }

    public class YingjianVS : ViewAsSkill
    {
        public YingjianVS() : base("yingjian")
        {
            response_pattern = "@@yingjian";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player Self)
        {
            if (cards.Count == 0)
                return new List<WrappedCard> { new WrappedCard(Slash.ClassName) { Skill = "yingjian", DistanceLimited = false } };

            return new List<WrappedCard>();
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
                return cards[0];

            return null;
        }
    }

    public class Yingjian : TriggerSkill
    {
        public Yingjian() : base("yingjian")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
            view_as_skill = new YingjianVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@yingjian", "@yingjian", null, -1, HandlingMethod.MethodUse, false, info.SkillPosition);
            return new TriggerStruct();
        }
    }

    public class Shixin : TriggerSkill
    {
        public Shixin() : base("shixin")
        {
            events.Add(TriggerEvent.DamageDefined);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.Nature == DamageStruct.DamageNature.Fire && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            LogMessage log = new LogMessage
            {
                Type = "#damaged-prevent",
                From = player.Name,
                Arg = Name
            };
            room.SendLog(log);

            return true;
        }
    }

    public class Fenyin : TriggerSkill
    {
        public Fenyin() : base("fenyin")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase != PlayerPhase.NotActive)
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use)
                    card = resp.Card;

                if (card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!(fcard is SkillCard) && !(fcard is ImperialOrder))
                    {
                        int suit = player.GetMark(Name);
                        if (player.Alive && suit > 0 && ((suit == 1 && card.Suit != WrappedCard.CardSuit.NoSuit) || (suit != 2 && WrappedCard.IsBlack(card.Suit))
                            || (suit != 3 && WrappedCard.IsRed(card.Suit))))
                            return new TriggerStruct(Name, player);
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
            if (player.Alive) room.DrawCards(player, 1, Name);
            return false;
        }
    }
    public class FenyinRecord : TriggerSkill
    {
        public FenyinRecord() : base("#fenyin")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.EventPhaseChanging };
        }
        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark("fenyin") > 0)
            {
                player.SetMark("fenyin", 0);
                room.RemovePlayerStringMark(player, "fenyin");
            }
            else if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded) && player == room.Current && base.Triggerable(player, room))
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use)
                    card = resp.Card;

                if (card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!(fcard is SkillCard) && !(fcard is ImperialOrder))
                    {
                        if (card.Suit == WrappedCard.CardSuit.NoSuit)
                        {
                            player.SetMark("fenyin", 1);
                            room.SetPlayerStringMark(player, "fenyin", "no_suit");
                        }
                        else if (WrappedCard.IsBlack(card.Suit))
                        {
                            player.SetMark("fenyin", 2);
                            room.SetPlayerStringMark(player, "fenyin", "black");
                        }
                        else
                        {
                            player.SetMark("fenyin", 3);
                            room.SetPlayerStringMark(player, "fenyin", "red");
                        }
                    }
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
}