using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;
using System.Linq;
using static CommonClass.Game.CardUseStruct;
using CommonClass;

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
                new Daigong(),
                new Zhaoxin(),
                new ZhaoxinClear(),
                new Zhongzuo(),
                new Wanlan(),

                new Zhaohuo(),
                new Yixiang(),
                new Yirang(),
                new Kuangcai(),
                new KuangcaiDraw(),
                new KuangcaiTar(),
                new Shejian(),
                new ChijieNSM(),
                new Waishi(),
                new Renshe(),
                new Zhaohan(),
                new Rangjie(),
                new Yizheng(),
                new YizhengEffect(),
                new Zhouxuan(),
                new ZhouxuanEffect(),
                new Fengji(),
                new FengjiMax(),
                new Chengzhao(),

                new Renshi(),
                new Wuyuan(),
                new Huaizi(),
                new Yizan(),
                new Longyuan(),
                new Zhiyi(),
                new ZhiyiTar(),
                new Duoduan(),
                new Gongshun(),

                new Yingjian(),
                new Shixin(),
                new Fenyin(),
                new FenyinRecord(),
            };

            skill_cards = new List<FunctionCard>
            {
                new ZhanyiCard(),
                new WuyuanCard(),
                new ZhaoxinCard(),
                new WaishiCard(),
                new YizhengCard(),
                new ZhouxuanCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "kuangcai", new List<string>{ "#kuangcai", "#kuangcai-tar" } },
                { "fenyin", new List<string>{ "#fenyin" } },
                { "qiancong", new List<string>{ "#qiancong", "#qiancong-tar" } },
                { "zhanyi", new List<string>{ "#zhanyi" } },
                { "zhaoxin", new List<string> { "#zhaoxin-clear" } },
                { "zhiyi", new List<string> { "#zhiyi" } },
                { "yizheng", new List<string> { "#yizheng" } },
                { "zhouxuan", new List<string> { "#zhouxuan" } },
                { "fengji", new List<string> { "#fengji" } },
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
                room.SendCompulsoryTriggerLog(ask_who, "zhanyi");
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
                    room.SendCompulsoryTriggerLog(ask_who, "zhanyi");
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

    public class Daigong : TriggerSkill
    {
        public Daigong() : base("daigong")
        {
            events.Add(TriggerEvent.DamageInflicted);
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.From != null && damage.From.Alive && damage.From != player && !player.IsKongcheng()
                && base.Triggerable(player, room) && !player.HasFlag(Name))
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SetTag(Name, data);
                bool invoke = room.AskForSkillInvoke(player, Name, damage.From, info.SkillPosition);
                room.RemoveTag(Name);

                if (invoke)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                    room.ShowAllCards(player);
                    player.SetFlags(Name);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                List<string> patterns = new List<string> { "diamond", "heart", "club", "spade" };
                List<string> suits = new List<string> { "♠", "♥", "♣", "♦" };
                foreach (int id in player.GetCards("h"))
                {
                    patterns.Remove(WrappedCard.GetSuitString(room.GetCard(id).Suit));
                    suits.Remove(WrappedCard.GetSuitIcon(room.GetCard(id).Suit));
                }
                bool prevent = false;
                if (patterns.Count == 0 || damage.From.IsNude())
                    prevent = true;
                else
                {
                    List<string> builder = new List<string>();
                    foreach (string p in patterns)
                        builder.Add(string.Format(".|{0}", p));

                    room.SetTag(Name, data);
                    List<int> ids = room.AskForExchange(damage.From, Name, 1, 0, string.Format("@daigong:{0}::{1}", player.Name, string.Join(",", suits)),
                        string.Empty, string.Join("#", builder), string.Empty);
                    room.RemoveTag(Name);
                    if (ids.Count > 0)
                    {
                        room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, damage.From.Name, player.Name, Name, string.Empty));
                    }
                    else
                        prevent = true;
                }

                if (prevent)
                {
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

            return false;
        }
    }

    public class Zhaoxin : TriggerSkill
    {
        public Zhaoxin() : base("zhaoxin")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventLoseSkill, TriggerEvent.EventPhaseEnd };
            view_as_skill = new ZhaoxinVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct _info && _info.Info == Name)
            {
                room.ClearOnePrivatePile(player, "ambition");
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Draw)
            {
                List<Player> smz = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in smz)
                {
                    if (p.GetPile("ambition").Count > 0 && (p == player || RoomLogic.InMyAttackRange(room, p, player)))
                    {
                        triggers.Add(new TriggerStruct(Name, player, p));
                    }
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.Name == info.SkillOwner)
            {
                List<int> ids = room.AskForExchange(ask_who, Name, 1, 0, "@zhaoxin", "ambition", string.Empty, info.SkillPosition);
                if (ids.Count > 0)
                {
                    room.NotifySkillInvoked(player, Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "zhaoxin", info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty));
                    return info;
                }
            }
            else
            {
                Player owner = room.FindPlayer(info.SkillOwner);
                bool invoke = room.AskForSkillInvoke(ask_who, Name, string.Format("@zhaoxin-get-from:{0}", info.SkillOwner));
                if (invoke)
                {
                    room.NotifySkillInvoked(owner, Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, owner, "zhaoxin", info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player owner = room.FindPlayer(info.SkillOwner);
            if (ask_who != owner)
            {
                List<int> ids = room.AskForExchange(owner, Name, 1, 1, string.Format("@zhaoxin-give:{0}", ask_who.Name), "ambition", string.Empty, info.SkillPosition);
                room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_GOTCARD, ask_who.Name, Name, string.Empty));

                if (owner.Alive && ask_who.Alive && room.AskForSkillInvoke(owner, Name, ask_who, info.SkillPosition))
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, owner.Name, ask_who.Name);
                    room.Damage(new DamageStruct(Name, owner, ask_who));
                }
            }

            return false;
        }
    }

    public class ZhaoxinVS : ViewAsSkill
    {
        public ZhaoxinVS() : base("zhaoxin") { }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            List<int> ids = player.GetPile("ambition");
            return selected.Count + ids.Count < 3;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            List<int> ids = player.GetPile("ambition");
            return ids.Count < 3 && !player.HasUsed(ZhaoxinCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard zx = new WrappedCard(ZhaoxinCard.ClassName) { Skill = Name, Mute = true };
                zx.AddSubCards(cards);
                return zx;
            }
            return null;
        }
    }

    public class ZhaoxinCard : SkillCard
    {
        public static string ClassName = "ZhaoxinCard";
        public ZhaoxinCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = false;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "zhaoxin", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("zhaoxin", "male", 1, gsk.General, gsk.SkinId);

            room.AddToPile(player, "ambition", card_use.Card);
            if (player.Alive)
                room.DrawCards(player, card_use.Card.SubCards.Count, "zhaoxin");
        }
    }

    public class ZhaoxinClear : DetachEffectSkill
    {
        public ZhaoxinClear() : base("zhaoxin", "ambition") { }
    }

    public class Zhongzuo : TriggerSkill
    {
        public Zhongzuo() : base("zhongzuo")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.Damaged, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if ((triggerEvent == TriggerEvent.Damage || triggerEvent == TriggerEvent.Damaged))
                player.SetFlags(Name);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish)
            {
                List<Player> jys = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in jys)
                    if (p.HasFlag(Name))
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(ask_who, room.GetAlivePlayers(), Name, "@zhongzuo", true, true, info.SkillPosition);
            if (target != null)
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                ask_who.SetTag(Name, target.Name);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)ask_who.GetTag(Name));
            ask_who.RemoveTag(Name);
            room.DrawCards(target, new DrawCardStruct(2, ask_who, Name));
            if (ask_who != target && ask_who.Alive && target.IsWounded())
                room.DrawCards(ask_who, 1, Name);

            return false;
        }
    }

    public class Wanlan : TriggerSkill
    {
        public Wanlan() : base("wanlan")
        {
            events = new List<TriggerEvent> { TriggerEvent.Dying, TriggerEvent.QuitDying };
            frequency = Frequency.Limited;
            skill_type = SkillType.Recover;
            limit_mark = "@wanlan";
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.QuitDying && player.Alive && player.HasFlag(Name) && player.ContainsTag(Name) && player.GetTag(Name) is string player_name)
            {
                player.SetFlags("-wanlan");
                player.RemoveTag(Name);

                if (room.Current != null && room.Current.Alive)
                {
                    Player from = room.FindPlayer(player_name);
                    if (from != null)
                        room.Damage(new DamageStruct(Name, from, room.Current));
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.Dying && player.Alive && player.Hp < 1)
            {
                List<Player> jys = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in jys)
                {
                    if (p.GetMark(limit_mark) > 0 && !p.IsNude())
                    {
                        bool check = true;
                        foreach (int id in p.GetCards("h"))
                        {
                            if (!RoomLogic.CanDiscard(room, p, p, id))
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check)
                            triggers.Add(new TriggerStruct(Name, p));
                    }
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && player.Hp < 1 && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                room.RemovePlayerMark(ask_who, limit_mark);
                room.DoSuperLightbox(ask_who, info.SkillPosition, Name);

                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = ask_who.GetCards("h");
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_THROW, ask_who.Name, Name, string.Empty);
            room.ThrowCard(ref ids, reason, null);

            if (player.Alive && player.Hp < 1)
            {
                int count = 1 - player.Hp;
                RecoverStruct recover = new RecoverStruct
                {
                    Who = ask_who,
                    Recover = count
                };
                room.Recover(player, recover, true);

                player.SetFlags(Name);
                player.SetTag(Name, ask_who.Name);
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

    public class ChijieNSM : TriggerSkill
    {
        public ChijieNSM() : base("chijie_nsm")
        {
            events.Add(TriggerEvent.GameStart);
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> kingdoms = new List<string>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.Kingdom != player.Kingdom && !kingdoms.Contains(p.Kingdom))
                    kingdoms.Add(p.Kingdom);
            }
            if (kingdoms.Count > 0)
            {
                kingdoms.Add("cancel");
                string choice = room.AskForChoice(player, Name, string.Join("+", kingdoms));
                if (choice != "cancel")
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.NotifySkillInvoked(player, Name);
                    player.SetTag(Name, choice);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.ContainsTag(Name) && player.GetTag(Name) is string kingdom)
            {
                player.Kingdom = kingdom;
                room.BroadcastProperty(player, "Kingdom");

                LogMessage log = new LogMessage
                {
                    Type = "#change_kingdom",
                    From = player.Name,
                    Arg = kingdom
                };
                room.SendLog(log);
            }

            return false;
        }
    }

    public class Waishi : ViewAsSkill
    {
        public Waishi() : base("waishi")
        {
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            List<string> kingdoms = new List<string>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (!kingdoms.Contains(p.Kingdom))
                    kingdoms.Add(p.Kingdom);
            }
            return selected.Count < kingdoms.Count;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude() && player.UsedTimes(WaishiCard.ClassName) < 1 + player.GetMark("renshe");
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard ws = new WrappedCard(WaishiCard.ClassName) { Skill = Name };
                ws.AddSubCards(cards);
                return ws;
            }

            return null;
        }
    }

    public class WaishiCard : SkillCard
    {
        public static string ClassName = "WaishiCard";
        public WaishiCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && card.SubCards.Count <= to_select.HandcardNum && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player a = card_use.From;
            Player b = card_use.To[0];

            List<int> from = new List<int>(card_use.Card.SubCards);
            List<int> tos = new List<int>(b.GetCards("h")), to = new List<int>();
            Shuffle.shuffle(ref tos);
            for (int i = 0; i < from.Count; i++)
                to.Add(tos[i]);

            CardsMoveStruct move1 = new CardsMoveStruct(from, b, Place.PlaceHand,
                new CardMoveReason(MoveReason.S_REASON_SWAP, a.Name, b.Name, "waishi", null));
            CardsMoveStruct move2 = new CardsMoveStruct(to, a, Place.PlaceHand,
                new CardMoveReason(MoveReason.S_REASON_SWAP, b.Name, a.Name, "waishi", null));
            List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct> { move1, move2 };
            room.MoveCards(exchangeMove, false);

            if (b.Kingdom == a.Kingdom || b.HandcardNum > a.HandcardNum)
                room.DrawCards(a, 1, "waishi");
        }
    }

    public class Renshe : TriggerSkill
    {
        public Renshe() : base("renshe")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Masochism;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && player.GetMark(Name) > 0)
            {
                if (change.To == PlayerPhase.Play)
                    player.SetFlags(Name);
                if (change.From == PlayerPhase.Play && player.HasFlag(Name))
                {
                    player.SetFlags("-renshe");
                    player.SetMark(Name, 0);
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> choices = new List<string> { "more", "draw" };
            List<string> kingdoms = new List<string>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.Kingdom != player.Kingdom && !kingdoms.Contains(p.Kingdom))
                    kingdoms.Add(p.Kingdom);
            }
            if (kingdoms.Count > 0)
                choices.Add("change");

            choices.Add("cancel");
            string choice = room.AskForChoice(player, Name, string.Join("+", choices));
            if (choice != "cancel")
            {
                player.SetTag(Name, choice);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.NotifySkillInvoked(player, Name);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.GetTag(Name) is string choice)
            {
                player.RemoveTag(Name);
                switch (choice)
                {
                    case "more":
                        player.AddMark(Name);
                        break;
                    case "draw":
                        Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@renshe", false, true, info.SkillPosition);
                        List<Player> targets = new List<Player> { player, target };
                        room.SortByActionOrder(ref targets);
                        foreach (Player p in targets)
                            room.DrawCards(p, new DrawCardStruct(1, player, Name));
                        break;
                    case "change":
                        List<string> kingdoms = new List<string>();
                        foreach (Player p in room.GetAlivePlayers())
                        {
                            if (p.Kingdom != player.Kingdom && !kingdoms.Contains(p.Kingdom))
                                kingdoms.Add(p.Kingdom);
                        }
                        string kingdom = room.AskForChoice(player, Name, string.Join("+", kingdoms));
                        player.Kingdom = kingdom;
                        room.BroadcastProperty(player, "Kingdom");

                        LogMessage log = new LogMessage
                        {
                            Type = "#change_kingdom",
                            From = player.Name,
                            Arg = kingdom
                        };
                        room.SendLog(log);
                        break;
                }
            }

            return false;
        }
    }

    public class Zhaohan : TriggerSkill
    {
        public Zhaohan() : base("zhaohan")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Start && player.GetMark(Name) < 7)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return info;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (player.GetMark(Name) < 4)
            {
                player.MaxHp++;
                room.BroadcastProperty(player, "MaxHp");

                LogMessage log = new LogMessage
                {
                    Type = "$GainMaxHp",
                    From = player.Name,
                    Arg = "1"
                };
                room.SendLog(log);

                room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, player);
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(player, recover, true);
            }
            else
            {
                room.LoseMaxHp(player);
            }
            if (player.Alive)
                player.AddMark(Name);

            return false;
        }
    }

    public class Rangjie : MasochismSkill
    {
        public Rangjie() : base("rangjie")
        {
            view_as_skill = new YijiVS();
            frequency = Frequency.Frequent;
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    Times = damage.Damage
                };
                return trigger;
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> choices = new List<string> { "get", "cancel" };
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetCards("ej").Count > 0)
                {
                    choices.Insert(0, "move");
                    break;
                }
            }

            string choice = room.AskForChoice(player, Name, string.Join("+", choices));
            if (choice != "cancel")
            {
                player.SetTag(Name, choice);
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player player, DamageStruct damage, TriggerStruct info)
        {
            if (player.GetTag(Name) is string choice)
            {
                player.RemoveTag(Name);
                if (choice == "move")
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.GetCards("ej").Count > 0) targets.Add(p);

                    if (targets.Count > 0)
                    {
                        Player from = room.AskForPlayerChosen(player, targets, Name, "@rangjie-move", false, false, info.SkillPosition);

                        room.SetTag("QiaobianTarget", from);
                        int card_id = room.AskForCardChosen(player, from, "ej", Name);
                        WrappedCard card = room.GetCard(card_id);
                        Place place = room.GetCardPlace(card_id);

                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        int equip_index = -1;
                        if (place == Place.PlaceEquip)
                        {
                            EquipCard equip = (EquipCard)fcard;
                            equip_index = (int)equip.EquipLocation();
                        }

                        List<Player> tos = new List<Player>();
                        foreach (Player p in room.GetOtherPlayers(from))
                        {
                            if (equip_index != -1)
                            {
                                if (p.GetEquip(equip_index) < 0 && RoomLogic.CanPutEquip(p, card))
                                    tos.Add(p);
                            }
                            else if (RoomLogic.IsProhibited(room, null, p, card) == null && !RoomLogic.PlayerContainsTrick(room, p, card.Name) && p.JudgingAreaAvailable)
                                tos.Add(p);
                        }

                        Player to = room.AskForPlayerChosen(player, tos, Name, "@rangjie-to:::" + card.Name, false, false, info.SkillPosition);
                        if (to != null)
                        {
                            if ((place == Place.PlaceDelayedTrick && from != player) || (place == Place.PlaceEquip && to != player))
                            {
                                ResultStruct result = player.Result;
                                result.Assist++;
                                player.Result = result;
                            }

                            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, from.Name, to.Name);
                            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_TRANSFER, player.Name, Name, null)
                            {
                                Card = card
                            };
                            room.MoveCardTo(card, from, to, place, reason);
                        }
                        room.RemoveTag("QiaobianTarget");
                    }
                }
                else
                {
                    choice = room.AskForChoice(player, Name, "basic+equip+trick", new List<string> { "@rangjie" });
                    List<int> ids = new List<int>();
                    foreach (int id in room.DrawPile)
                    {
                        WrappedCard card = room.GetCard(id);
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        if (fcard.TypeID == CardType.TypeBasic && choice == "basic")
                        {
                            ids.Add(id);
                            break;
                        }
                        else if (fcard.TypeID == CardType.TypeEquip && choice == "equip")
                        {
                            ids.Add(id);
                            break;
                        }
                        else if (fcard.TypeID == CardType.TypeTrick && choice == "trick")
                        {
                            ids.Add(id);
                            break;
                        }
                    }
                    if (ids.Count > 0)
                    {
                        CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, player.Name, Name, string.Empty);
                        room.ObtainCard(player, ref ids, reason, true);
                    }
                }

                if (player.Alive)
                    room.DrawCards(player, 1, Name);
            }
        }
    }

    public class Yizheng : ZeroCardViewAsSkill
    {
        public Yizheng() : base("yizheng")
        {
            skill_type = SkillType.Attack;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(YizhengCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(YizhengCard.ClassName) { Skill = Name };
        }
    }

    public class YizhengCard : SkillCard
    {
        public static string ClassName = "YizhengCard";
        public YizhengCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && RoomLogic.CanBePindianBy(room, to_select, Self) && to_select.Hp <= Self.Hp;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0], player = card_use.From;
            PindianStruct pd = room.PindianSelect(player, target, "yizheng");

            room.Pindian(ref pd);
            if (pd.Success)
            {
                if (target.Alive)
                {
                    target.AddMark("yizheng", 1);
                    room.SetPlayerStringMark(target, "yizheng", target.GetMark("yizheng").ToString());
                }
            }
            else
            {
                room.LoseMaxHp(player);
            }
        }
    }

    public class YizhengEffect : TriggerSkill
    {
        public YizhengEffect() : base("#yizheng")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.GetMark("yizheng") > 0 && data is PhaseChangeStruct change && change.To == PlayerPhase.Draw)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            LogMessage log = new LogMessage
            {
                Type = "$SkipPhase",
                From = player.Name,
                Arg = "draw"
            };
            room.SendLog(log);

            player.AddMark("yizheng", -1);
            if (player.GetMark("yizheng") == 0)
                room.RemovePlayerStringMark(player, "yizheng");
            else
                room.SetPlayerStringMark(player, "yizheng", player.GetMark("yizheng").ToString());

            return true;
        }
    }

    public class Zhouxuan : TriggerSkill
    {
        public Zhouxuan() : base("zhouxuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded };
            skill_type = SkillType.Wizzard;
            view_as_skill = new ZhouxuanVS();
            priority = new Dictionary<TriggerEvent, double>
            {
                { TriggerEvent.EventPhaseStart, 3 },
                { TriggerEvent.CardUsedAnnounced, 2 },
                {TriggerEvent.CardResponded, 2 }
            };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if ((triggerEvent == TriggerEvent.CardUsedAnnounced || triggerEvent == TriggerEvent.CardResponded) && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && !player.IsNude())
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@zhouxuan", "@zhouxuan", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
            return new TriggerStruct();
        }
    }

    public class ZhouxuanVS : OneCardViewAsSkill
    {
        public ZhouxuanVS() : base("zhouxuan")
        {
            response_pattern = "@@zhouxuan";
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, to_select.Id);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard zx = new WrappedCard(ZhouxuanCard.ClassName) { Skill = Name };
            zx.AddSubCard(card);
            return zx;
        }
    }

    public class ZhouxuanCard : SkillCard
    {
        public static string ClassName = "ZhouxuanCard";
        public ZhouxuanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<string> choices = new List<string> { "trick", "equip" };
            List<string> basic = ViewAsSkill.GetGuhuoCards(room, "b");
            foreach (string card in basic)
            {
                if (card != Slash.ClassName && card.Contains(Slash.ClassName)) continue;
                choices.Add(card);
            }

            string choice = room.AskForChoice(player, "zhouxuan", string.Join("+", choices), new List<string> { "@zhouxuan-ann:" + target.Name }, target);
            Dictionary<string, string> zhouxuan_dic = target.ContainsTag("zhouxuan") ? (Dictionary<string, string>)target.GetTag("zhouxuan") : new Dictionary<string, string>();
            zhouxuan_dic.Remove(player.Name);
            zhouxuan_dic.Add(player.Name, choice);
            target.SetTag("zhouxuan", zhouxuan_dic);
        }
    }

    public class ZhouxuanEffect : TriggerSkill
    {
        public ZhouxuanEffect() : base("#zhouxuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.ContainsTag("zhouxuan") && player.GetTag("zhouxuan") is Dictionary<string, string> zhouxuan)
            {
                string card_name = string.Empty;
                if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use)
                    card_name = use.Card.Name;
                else if (data is CardResponseStruct resp)
                    card_name = resp.Card.Name;

                FunctionCard fcard = Engine.GetFunctionCard(card_name);
                foreach (string player_name in zhouxuan.Keys)
                {
                    Player p = room.FindPlayer(player_name);
                    if (p != null)
                    {
                        string choice = zhouxuan[player_name];
                        if (choice == "trick" && fcard is TrickCard)
                            triggers.Add(new TriggerStruct(Name, p));
                        else if (choice == "equip" && fcard is EquipCard)
                            triggers.Add(new TriggerStruct(Name, p));
                        else if (card_name.Contains(choice))
                            triggers.Add(new TriggerStruct(Name, p));
                    }
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player guojia, TriggerStruct info)
        {
            List<int> yiji_cards = room.GetNCards(3);
            List<int> origin_yiji = new List<int>(yiji_cards);

            while (guojia.Alive && yiji_cards.Count > 0)
            {
                guojia.PileChange("#zhouxuan", yiji_cards);
                if (!room.AskForYiji(guojia, yiji_cards, "zhouxuan", true, false, true, -1, room.GetAlivePlayers(), null, "@zhouxuan-atri", "#zhouxuan", false, info.SkillPosition))
                    break;

                guojia.Piles["#zhouxuan"].Clear();
                foreach (int id in origin_yiji)
                    if (room.GetCardPlace(id) != Place.DrawPile)
                        yiji_cards.Remove(id);
            }
            if (guojia.GetPile("#zhouxuan").Count > 0) guojia.Piles["#zhouxuan"].Clear();
            if (yiji_cards.Count > 0)
                room.ReturnToDrawPile(yiji_cards, false);

            return false;
        }
    }

    public class Fengji : TriggerSkill
    {
        public Fengji() : base("fengji")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Alive && triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                player.SetTag(Name, player.HandcardNum);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.RoundStart && player.ContainsTag(Name) && player.GetTag(Name) is int count
                && player.HandcardNum >= count)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            player.SetFlags(Name);
            room.DrawCards(player, 2, Name);
            return false;
        }
    }

    public class FengjiMax : MaxCardsSkill
    {
        public FengjiMax() : base("#fengji")
        {
        }
        public override int GetFixed(Room room, Player target)
        {
            if (target.HasFlag("fengji"))
            {
                return target.MaxHp;
            }
            return -1;
        }
    }

    public class Chengzhao : TriggerSkill
    {
        public Chengzhao() : base("chengzhao")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime, TriggerEvent.TargetChosen };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceHand)
            {
                move.To.AddMark(Name, move.Card_ids.Count);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0)
                        p.SetMark(Name, 0);
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use)
            {
                if (use.Card != null && use.Card.GetSkillName() == Name)
                {
                    foreach (Player p in use.To)
                        if (p.HasFlag(Name))
                            p.AddQinggangTag(RoomLogic.CardToString(room, use.Card));
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.GetMark(Name) >= 2 && !p.IsKongcheng()) triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(ask_who))
            {
                if (!p.IsKongcheng() && RoomLogic.CanBePindianBy(room, p, ask_who))
                    targets.Add(p);
            }

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(ask_who, targets, Name, "@chengzhao", true, true, info.SkillPosition);
                if (target != null)
                {
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    ask_who.SetTag(Name, target.Name);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.GetTag(Name) is string player_name)
            {
                Player target = room.FindPlayer(player_name);
                ask_who.RemoveTag(Name);

                PindianStruct pd = room.PindianSelect(ask_who, target, Name);
                room.Pindian(ref pd);

                if (pd.Success)
                {
                    target.SetFlags(Name);
                    WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_chengzhao" };
                    room.UseCard(new CardUseStruct(slash, ask_who, target));
                }
            }

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

    public class Yizan : TriggerSkill
    {
        public Yizan() : base("yizan")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            skill_type = SkillType.Alter;
            view_as_skill = new YizanVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && use.Card.Skill == Name)
                player.AddMark(Name);
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Card.Skill == Name)
                player.AddMark(Name);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data) => new List<TriggerStruct>();
    }

    public class YizanVS : ViewAsSkill
    {
        public YizanVS() : base("yizan")
        {
            response_or_use = true;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
            if ((reason == CardUseReason.CARD_USE_REASON_PLAY || reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE) && RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodUse))
                return false;
            else if (reason == CardUseReason.CARD_USE_REASON_RESPONSE && RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodResponse))
                return false;
            else if (selected.Count >= 2)
                return false;

            if (player.GetMark("longyuan") == 0)
            {
                if (selected.Count == 0)
                    return true;
                else
                    return Engine.GetFunctionCard(selected[0].Name).TypeID == CardType.TypeBasic || Engine.GetFunctionCard(to_select.Name).TypeID == CardType.TypeBasic;
            }
            else
                return selected.Count == 0 && Engine.GetFunctionCard(to_select.Name).TypeID == CardType.TypeBasic;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            WrappedCard jink = new WrappedCard(Jink.ClassName);
            WrappedCard peach = new WrappedCard(Peach.ClassName);
            WrappedCard ana = new WrappedCard(Analeptic.ClassName);

            return Engine.MatchExpPattern(room, pattern, player, slash) || Engine.MatchExpPattern(room, pattern, player, jink)
                || Engine.MatchExpPattern(room, pattern, player, peach) || Engine.MatchExpPattern(room, pattern, player, ana);
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player Self)
        {
            List<WrappedCard> all_cards = new List<WrappedCard>();
            if (cards.Count == (Self.GetMark("longyuan") > 0 ? 1 : 2))
            {
                CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
                if (reason == CardUseReason.CARD_USE_REASON_PLAY)
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
            if (cards[0].IsVirtualCard())
            {
                WrappedCard card = RoomLogic.ParseUseCard(room, cards[0]);
                card.Skill = Name;
                return card;
            }

            return null;
        }

        public override bool IsEnabledAtPlay(Room room, Player player) => true;
    }

    public class Longyuan : PhaseChangeSkill
    {
        public Longyuan() : base("longyuan")
        {
            frequency = Frequency.Wake;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.GetMark("yizan") >= 3)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            return false;
        }
    }

    public class Zhiyi : TriggerSkill
    {
        public Zhiyi() : base("zhiyi")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.EventPhaseChanging, TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
            view_as_skill = new ZhiyiVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                player.SetFlags("-zhiyi");
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct _use)
            {
                if (player.ContainsTag(Name) && player.Alive && player.GetTag(Name) is WrappedCard card && _use.Card == card)
                    triggers.Add(new TriggerStruct(Name, player));

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.ContainsTag(Name) && p.ContainsTag("zhiyi_resp") && p.GetTag("zhiyi_resp") is WrappedCard resp_card && _use.Card == resp_card)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }
            else if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && !player.HasFlag(Name) && data is CardUseStruct use)
            {
                if (Engine.GetFunctionCard(use.Card.Name) is BasicCard)
                    triggers.Add(new TriggerStruct(Name, player));
            }
            else if (triggerEvent == TriggerEvent.CardResponded && base.Triggerable(player, room) && !player.HasFlag(Name) && data is CardResponseStruct resp)
            {
                if (Engine.GetFunctionCard(resp.Card.Name) is BasicCard)
                    triggers.Add(new TriggerStruct(Name, player));
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded))
            {
                player.SetFlags(Name);
                WrappedCard card = null;
                bool can_use = true;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (data is CardResponseStruct resp)
                {
                    card = resp.Card;
                    if (card.Name == Jink.ClassName || resp.Data == null || !(resp.Data is CardEffectStruct effect) || effect.Card == null)
                        can_use = false;
                }

                List<string> choices = new List<string> { "draw" };
                if (can_use) choices.Add("use");

                string choice = room.AskForChoice(player, Name, string.Join("+", choices));
                room.SendCompulsoryTriggerLog(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                if (choice == "draw")
                {
                    room.DrawCards(player, 1, Name);
                }
                else
                {
                    player.SetTag(Name, card);
                    if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Data is CardEffectStruct effect)
                        player.SetTag("zhiyi_resp", effect.Card);

                    LogMessage log = new LogMessage("$zhiyi")
                    {
                        From = player.Name,
                        Card_str = RoomLogic.CardToString(room, card)
                    };
                    room.SendLog(log);
                }
            }
            else if (triggerEvent == TriggerEvent.CardFinished && ask_who.GetTag(Name) is WrappedCard card)
            {
                ask_who.RemoveTag("zhiyi_resp");
                ask_who.RemoveTag(Name);
                ask_who.SetTag(Name, card.Name);
                room.AskForUseCard(ask_who, "@@zhiyi", "@zhiyi:::" + card.Name, null, -1, HandlingMethod.MethodUse, false, info.SkillPosition);

                ask_who.RemoveTag(Name);
            }

            return false;
        }
    }

    public class ZhiyiVS : ViewAsSkill
    {
        public ZhiyiVS() : base("zhiyi")
        {
            response_pattern = "@@zhiyi";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
                return cards[0];

            return null;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            WrappedCard card = new WrappedCard((string)player.GetTag(Name));
            card.Skill = "_zhiyi";
            result.Add(card);
            return result;
        }
    }

    public class ZhiyiTar : TargetModSkill
    {
        public ZhiyiTar() : base("#zhiyi", false)
        {
            pattern = Analeptic.ClassName;
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
        {
            return pattern == "@@zhiyi" ? true : false;
        }
    }

    public class Duoduan : TriggerSkill
    {
        public Duoduan() : base("duoduan")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.From != null && use.From != player && !player.HasFlag(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag(Name, data);
                int index = 0;
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (effect.To == player)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            player.SetTag(Name, i);
                            break;
                        }
                    }
                }

                List<int> ids = room.AskForExchange(player, Name, 1, 0, "@duoduan:" + use.From.Name, string.Empty, "..", info.SkillPosition);

                player.RemoveTag(Name);
                room.RemoveTag(Name);
                if (ids.Count == 1)
                {
                    player.SetFlags(Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.NotifySkillInvoked(player, Name);

                    WrappedCard card = room.GetCard(ids[0]);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_RECAST, use.From.Name)
                    {
                        SkillName = Name
                    };
                    room.MoveCardTo(card, null, Place.PlaceTable, reason, true);
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
                    }
                    room.DrawCards(use.From, 1, "recast");

                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, use.From.Name);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && use.From.Alive)
            {
                List<string> choices = new List<string> { "draw" };
                if (!use.From.IsNude() && RoomLogic.CanDiscard(room, use.From, use.From, "he")) choices.Add("discard");
                string choice = room.AskForChoice(use.From, Name, string.Join("+", choices), new List<string> { "@duoduan-from:" + player.Name }, data);
                if (choice == "draw")
                {
                    room.DrawCards(use.From, new DrawCardStruct(2, player, Name));

                    int index = 0;
                    for (int i = 0; i < use.EffectCount.Count; i++)
                    {
                        CardBasicEffect effect = use.EffectCount[i];
                        if (effect.To == player)
                        {
                            index++;
                            if (index == info.Times)
                            {
                                effect.Nullified = true;
                                use.EffectCount[i] = effect;
                                data = use;
                                break;
                            }
                        }
                    }
                }
                else if (room.AskForDiscard(use.From, Name, 1, 1, false, true, string.Format("@duoduan-discard:{0}::{1}", player.Name, use.Card.Name)))
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#NoJink",
                        From = player.Name
                    };
                    room.SendLog(log);

                    int index = 0;
                    for (int i = 0; i < use.EffectCount.Count; i++)
                    {
                        CardBasicEffect effect = use.EffectCount[i];
                        if (effect.To == player)
                        {
                            index++;
                            if (index == info.Times)
                            {
                                effect.Effect2 = 0;
                                data = use;
                                break;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Gongshun : TriggerSkill
    {
        public Gongshun() : base("gongshun")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.TurnStart };
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct _move)
            {
                if (_move.From != null && _move.From_places.Contains(Place.PlaceHand) && _move.From.ContainsTag(Name) && _move.From.GetTag(Name) is List<int> ids)
                {
                    List<int> remove = ids.FindAll(t => _move.Card_ids.Contains(t));
                    foreach (int id in remove)
                        RoomLogic.RemovePlayerCardLimitation(_move.From, Name, "use,response,discard", id.ToString());

                    ids.RemoveAll(t => _move.Card_ids.Contains(t));
                    if (ids.Count == 0)
                        _move.From.RemoveTag(Name);
                    else
                        _move.From.SetTag(Name, ids);
                }
            }
            else if (triggerEvent == TriggerEvent.TurnStart)
            {
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return false;
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