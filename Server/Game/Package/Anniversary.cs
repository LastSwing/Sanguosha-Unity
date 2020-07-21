using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;
using CommonClass;

namespace SanguoshaServer.Package
{
    public class Anniversary : GeneralPackage
    {
        public Anniversary() : base("Anniversary")
        {
            skills = new List<Skill>
            {
                new Yanjiao(),
                new YanjiaoMax(),
                new Xingshen(),
                new Andong(),
                new AndongMax(),
                new Yingshi(),
                new YingshiGet(),
                new Sanwen(),
                new Qiai(),
                new Denglou(),
                new Lvli(),
                new Choujue(),
                new Beishui(),
                new Qingjiao(),
                new QingjiaoThrow(),
                new QianxinZG(),
                new QianxinEffect(),
                new QianxinMax(),
                new Zhenxing(),
                new Tuiyan(),
                new Busuan(),
                new BusuanDraw(),
                new Mingjie(),
                new Weilu(),
                new Zengdao(),
                new ZengdaoDamage(),
                new Chijie(),
                new ChijieEffect(),
                new Yinju(),
                new YinjuEffect(),
                new ZhuilieTar(),
                new Zhuilie(),

                new Tunan(),
                new TunanTag(),
                new Bijing(),
                new BijingDiscard(),
                new Tianjiang(),
                new Zhuren(),
                new Manyi(),
                new Mansi(),
                new MansiClear(),
                new Souying(),
                new Zhanyuan(),
                new Xili(),

                new Jiedao(),
                new JiedaoDis(),
                new Kannan(),
                new KannanDamage(),
                new Jixu(),
                new Jijun(),
                new Fangtong(),
                new Lixun(),
                new KuizhuLS(),
                new Fenyue(),
                new Xuhe(),

                new Guolun(),
                new Songsang(),
                new Zhanji(),
                new Guanwei(),
                new Gongqing(),
                new Qinguo(),
                new QinguoRecover(),
                new Youdi(),
                new Duanfa(),
                new Biaozhao(),
                new BiaozhaoEffect(),
                new Yechou(),
                new YechouLose(),
                new Guanchao(),
                new GuanchaoRecord(),
                new Xunxian(),
                new Fuhai(),
                new Lianhua(),
                new Zhafu(),
                new ZhafuEffect(),
                new Songshu(),
                new Sibian(),
            };

            skill_cards = new List<FunctionCard>
            {
                new GuolunCard(),
                new KannanCard(),
                new DuanfaCard(),
                new YanjiaoCard(),
                new TunanCard(),
                new JixuCard(),
                new FuhaiCard(),
                new ZhafuCard(),
                new BusuanCard(),
                new QianxinZGCard(),
                new ZengdaoCard(),
                new TianjiangCard(),
                new ZhurenCard(),
                new YinjuCard(),
                new SongshuCard(),
                new FenyueCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "jiedao", new List<string>{ "#jiedao" } },
                { "kannan", new List<string>{ "#kannan" } },
                { "yanjiao", new List<string>{ "#yanjiao" } },
                { "qinguo", new List<string>{ "#qinguo" } },
                { "bijing", new List<string>{ "#bijing" } },
                { "tunan", new List<string>{ "#tunan" } },
                { "andong", new List<string>{ "#andong" } },
                { "yingshi", new List<string>{ "#yingshi" } },
                { "biaozhao", new List<string>{ "#biaozhao" } },
                { "yechou", new List<string>{ "#yechou" } },
                { "guanchao", new List<string>{ "#guanchao" } },
                { "qingjiao", new List<string>{ "#qingjiao" } },
                { "zhafu", new List<string>{ "#zhafu" } },
                { "busuan", new List<string>{ "#busuan" } },
                { "zengdao", new List<string>{ "#zengdao" } },
                { "qianxin_zg", new List<string>{ "#qianxin_zg" } },
                { "chijie", new List<string>{ "#chijie" } },
                { "yinju", new List<string>{ "#yinju" } },
                { "mansi", new List<string>{ "#mansi" } },
                { "zhuilie", new List<string>{ "#zhuilie" } },
            };
        }
    }

    public class Yanjiao : TriggerSkill
    {
        public Yanjiao() : base("yanjiao")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            view_as_skill = new YanjiaoVS();
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
        public override bool SortFilter(Room room, List<int> to_sorts, List<int> ups, List<int> downs)
        {
            int up = 0, down = 0;
            foreach (int id in ups)
                up += room.GetCard(id).Number;

            foreach (int id in downs)
                down += room.GetCard(id).Number;

            return up == down;
        }
    }

    public class YanjiaoVS : ZeroCardViewAsSkill
    {
        public YanjiaoVS() : base("yanjiao")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(YanjiaoCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(YanjiaoCard.ClassName) { Skill = Name };
        }
    }

    public class YanjiaoCard : SkillCard
    {
        public static string ClassName = "YanjiaoCard";
        public YanjiaoCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            int count = 4 + player.GetMark("xingshen");
            player.SetMark("xingshen", 0);
            room.RemovePlayerStringMark(player, "xingshen");

            List<int> card_ids = room.GetNCards(count);
            foreach (int id in card_ids)
            {
                room.MoveCardTo(room.GetCard(id), player, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name, "yanjiao", null), false);
                Thread.Sleep(400);
            }
            AskForMoveCardsStruct result = room.AskforSortCards(target, "yanjiao", card_ids, true, card_use.Card.SkillPosition);
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            if (result.Success)
            {
                card_ids.RemoveAll(t => result.Top.Contains(t));
                card_ids.RemoveAll(t => result.Bottom.Contains(t));

                if (result.Bottom.Count > 0 && result.Top.Count > 0)
                {
                    moves.Add(new CardsMoveStruct(result.Bottom, target, Place.PlaceHand, new CardMoveReason(MoveReason.S_REASON_GOTBACK, target.Name, "yanjiao", null)));
                    moves.Add(new CardsMoveStruct(result.Top, player, Place.PlaceHand, new CardMoveReason(MoveReason.S_REASON_GOTBACK, player.Name, "yanjiao", null)));

                }
            }

            if (card_ids.Count > 0)
            {
                moves.Add(new CardsMoveStruct(card_ids, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, null, "yanjiao", null)));
                if (card_ids.Count > 1) player.AddMark("yanjiao");
            }
            room.MoveCards(moves, true);
        }
    }

    public class YanjiaoMax : MaxCardsSkill
    {
        public YanjiaoMax() : base("#yanjiao") { }
        public override int GetExtra(Room room, Player target)
        {
            return -target.GetMark("yanjiao");
        }
    }

    public class Xingshen : MasochismSkill
    {
        public Xingshen() : base("xingshen") { }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            int count = 1000, hp = 100;
            int draw = 1;
            int mark = target.GetMark(Name) < 4 ? 1 : 0;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HandcardNum < count) count = p.HandcardNum;
                if (p.Hp < hp) hp = p.Hp;
            }

            if (target.HandcardNum == count) draw = 2;
            room.DrawCards(target, draw, Name);
            if (mark > 0)
            {
                if (target.Hp == hp && target.GetMark(Name) + 2 <= 4) mark = 2;
                target.AddMark(Name, mark);
                room.SetPlayerStringMark(target, Name, target.GetMark(Name).ToString());
            }
        }
    }

    public class Andong : TriggerSkill
    {
        public Andong() : base("andong")
        {
            events.Add(TriggerEvent.DamageDefined);
            skill_type = SkillType.Defense;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.From != null && damage.From != player && damage.From.Alive && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SetTag(Name, data);
                player.SetFlags(Name);
                bool invoke = room.AskForSkillInvoke(player, Name, damage.From, info.SkillPosition);
                player.SetFlags("-andong");
                room.RemoveTag(Name);

                if (invoke)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                List<string> choices = new List<string> { "prevent" };
                if (!damage.From.IsKongcheng()) choices.Add("view");
                player.SetFlags(Name);
                string choice = room.AskForChoice(damage.From, Name, string.Join("+", choices), new List<string> { "@andong:" + player.Name }, data);
                player.SetFlags("-andong");
                if (choice == "prevent")
                {
                    if (damage.From == room.Current) damage.From.SetFlags(Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "#damaged-prevent",
                        From = player.Name,
                        Arg = Name
                    };
                    room.SendLog(log);
                    return true;
                }
                else
                {
                    room.ShowAllCards(damage.From, player, Name);
                    List<int> ids = new List<int>();
                    foreach (int id in damage.From.GetCards("h"))
                    {
                        if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart && RoomLogic.CanGetCard(room, player, damage.From, id))
                            ids.Add(id);
                    }
                    if (ids.Count > 0)
                        room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, damage.From.Name, Name, string.Empty));
                }
            }

            return false;
        }
    }

    public class AndongMax : MaxCardsSkill
    {
        public AndongMax() : base("#andong") { }
        public override bool Ingnore(Room room, Player player, int card_id)
        {
            return player.HasFlag("andong") && room.GetCard(card_id).Suit == WrappedCard.CardSuit.Heart;
        }
    }

    public class Yingshi : TriggerSkill
    {
        public Yingshi() : base("yingshi") { events.Add(TriggerEvent.EventPhaseStart); skill_type = SkillType.Attack; }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                bool check = true;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetPile("reward").Count > 0)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                {
                    foreach (int id in player.GetCards("he"))
                        if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart)
                            return new TriggerStruct(Name, player);
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@yingshi", true, true, info.SkillPosition);
            if (target != null)
            {
                room.SetTag(Name, target);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            List<int> ids = new List<int>();

            foreach (int id in player.GetCards("he"))
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart)
                    ids.Add(id);
            if (ids.Count > 0) room.AddToPile(target, "reward", ids);
            return false;
        }
    }

    public class YingshiGet : TriggerSkill
    {
        public YingshiGet() : base("#yingshi")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.Damage };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Death && player.GetPile("reward").Count > 0)
            {
                Player target = RoomLogic.FindPlayerBySkillName(room, "yingshi");
                if (target != null) return new TriggerStruct(Name, target);
            }
            else if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.To.GetPile("reward").Count > 0
                && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && !damage.Transfer && !damage.Chain)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.Death)
            {
                List<int> ids = player.GetPile("reward");
                room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_GOTCARD, ask_who.Name, "yingshi", string.Empty));
            }
            else if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage)
            {
                List<int> ids = damage.To.GetPile("reward");
                room.FillAG("yingshi", ids, player, null, null, "@yingshi-get");
                int id = room.AskForAG(player, ids, true, "yingshi");
                room.ClearAG(player);
                if (id > -1)
                    room.ObtainCard(player, id);
            }

            return false;
        }
    }

    public class Sanwen : TriggerSkill
    {
        public Sanwen() : base("sanwen")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceHand && base.Triggerable(move.To, room)
                && !move.To.IsKongcheng() && !move.To.HasFlag(Name))
            {
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == move.To)
                        return new TriggerStruct(Name, move.To);
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                List<string> patterns = new List<string>();
                foreach (int id in move.Card_ids)
                {
                    WrappedCard card = room.GetCard(id);
                    string pattern = string.Format("{0}+^{1}|.|.|hand", card.Name.Contains(Slash.ClassName) ? Slash.ClassName : card.Name, id);
                    patterns.Add(pattern);
                }

                List<int> ids = room.AskForExchange(ask_who, Name, ask_who.GetCardCount(false), 0, "@sanwen", string.Empty, string.Join("#", patterns), info.SkillPosition);
                if (ids.Count > 0)
                {
                    room.NotifySkillInvoked(ask_who, Name);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    room.ShowCards(ask_who, ids, Name);
                    room.SetTag(Name, ids);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.SetFlags(Name);
            if (room.GetTag(Name) is List<int> ids && data is CardsMoveOneTimeStruct move)
            {
                room.RemoveTag(Name);
                List<string> patterns = new List<string>();
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    string pattern = card.Name.Contains(Slash.ClassName) ? Slash.ClassName : card.Name;
                    if (!patterns.Contains(pattern))
                        patterns.Add(pattern);
                }
                List<int> discard = new List<int>();
                foreach (int id in move.Card_ids)
                {
                    WrappedCard card = room.GetCard(id);
                    string pattern = card.Name.Contains(Slash.ClassName) ? Slash.ClassName : card.Name;
                    if (patterns.Contains(pattern) && RoomLogic.CanDiscard(room, ask_who, ask_who, id))
                        discard.Add(id);
                }

                int count = discard.Count * 2;
                room.ThrowCard(ref discard, new CardMoveReason(MoveReason.S_REASON_THROW, ask_who.Name, Name, string.Empty), ask_who);
                if (ask_who.Alive)
                    room.DrawCards(ask_who, count, Name);
            }

            return false;
        }
    }

    public class Qiai : TriggerSkill
    {
        public Qiai() : base("qiai")
        {
            frequency = Frequency.Limited;
            events.Add(TriggerEvent.Dying);
            limit_mark = "@qiai";
        }

        public override bool Triggerable(Player target, Room room)
        {
            if (base.Triggerable(target, room) && target.GetMark(limit_mark) > 0)
                foreach (Player p in room.GetOtherPlayers(target))
                    if (!p.IsNude())
                        return true;

            return false;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player pangtong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(pangtong, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, pangtong, info.SkillPosition);
                room.DoSuperLightbox(pangtong, info.SkillPosition, Name);
                room.SetPlayerMark(pangtong, limit_mark, 0);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Alive && !p.IsNude() && player.Alive)
                {
                    List<int> ids = room.AskForExchange(p, Name, 1, 1, "@qiai-give:" + player.Name, string.Empty, "..", string.Empty);
                    if (ids.Count > 0)
                        room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, p.Name, player.Name, Name, string.Empty), false);
                }
            }
            player.SetFlags("-qiai");

            return false;
        }
    }

    public class Denglou : TriggerSkill
    {
        public Denglou() : base("denglou")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            limit_mark = "@denglou";
            frequency = Frequency.Limited;
            view_as_skill = new DenglouVS();
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish && target.IsKongcheng() && target.GetMark(limit_mark) > 0;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player pangtong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(pangtong, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, pangtong, info.SkillPosition);
                room.DoSuperLightbox(pangtong, info.SkillPosition, Name);
                room.SetPlayerMark(pangtong, limit_mark, 0);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.GetNCards(4), get = new List<int>(), basic = new List<int>();
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is BasicCard)
                    basic.Add(id);
                else
                    get.Add(id);
            }
            if (get.Count > 0)
                room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_DRAW, player.Name, Name, string.Empty), true);

            while (player.Alive && basic.Count > 0)
            {
                player.Piles["#denglou"] = new List<int>(basic);
                WrappedCard card = room.AskForUseCard(player, "@@denglou", "@denglou-use", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
                player.Piles["#denglou"].Clear();
                if (card != null)
                    basic.RemoveAll(t => card.SubCards.Contains(t));
                else
                    break;
            }
            if (basic.Count > 0)
            {
                CardsMoveStruct move = new CardsMoveStruct(basic, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, player.Name, Name, string.Empty));
                List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
                room.MoveCardsAtomic(moves, true);
            }

            return false;
        }
    }

    public class DenglouVS : OneCardViewAsSkill
    {
        public DenglouVS() : base("denglou")
        {
            expand_pile = "#denglou";
            response_pattern = "@@denglou";
        }

        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(to_select.Name);
            return player.GetPile(expand_pile).Contains(to_select.Id) && fcard.IsAvailable(room, player, to_select);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player) => card;
    }

    public class Lvli : TriggerSkill
    {
        public Lvli() : base("lvli")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.Damage, TriggerEvent.Damaged };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0) p.SetMark(Name, 0);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if ((triggerEvent == TriggerEvent.Damage || triggerEvent == TriggerEvent.Damaged)
                && base.Triggerable(player, room) && (player.GetMark(Name) == 0 || (player.GetMark(Name) < 2 && player.GetMark("choujue") == 1 && player == room.Current))
                && (player.HandcardNum < player.Hp || (player.Hp < player.HandcardNum && player.IsWounded())))
            {
                if (triggerEvent == TriggerEvent.Damaged && player.GetMark("beishui") == 1 || triggerEvent == TriggerEvent.Damage)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                player.AddMark(Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.HandcardNum < player.Hp)
            {
                int count = player.Hp - player.HandcardNum;
                room.DrawCards(player, count, Name);
            }
            else if (player.Hp < player.HandcardNum && player.IsWounded())
            {
                int max = Math.Min(player.HandcardNum, player.MaxHp);
                int count = max - player.Hp;
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = count,
                    Who = player
                };
                room.Recover(player, recover, true);
            }
            return false;
        }
    }

    public class Choujue : TriggerSkill
    {
        public Choujue() : base("choujue")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Wake;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Phase == PlayerPhase.Finish)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.GetMark(Name) == 0 && Math.Abs(p.Hp - p.HandcardNum) >= 3)
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DoSuperLightbox(ask_who, info.SkillPosition, Name);
            room.SetPlayerMark(ask_who, Name, 1);
            room.SendCompulsoryTriggerLog(ask_who, Name);

            room.LoseMaxHp(ask_who);
            if (ask_who.Alive)
                room.HandleAcquireDetachSkills(ask_who, "beishui", true);

            return false;
        }
    }

    public class Beishui : TriggerSkill
    {
        public Beishui() : base("beishui")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Wake;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.GetMark(Name) == 0 && (target.Hp < 2 || target.HandcardNum < 2) && target.Phase == PlayerPhase.Start;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            room.LoseMaxHp(player);
            if (player.Alive)
                room.HandleAcquireDetachSkills(player, "qingjiao", true);

            return false;
        }
    }

    public class Qingjiao : TriggerSkill
    {
        public Qingjiao() : base("qingjiao")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Attack;
        }

        public override bool Triggerable(Player target, Room room)
        {
            if (base.Triggerable(target, room) && target.Phase == PlayerPhase.Play && target.HandcardNum > 0)
            {
                bool discard = true;
                foreach (int id in target.GetCards("h"))
                {
                    if (!RoomLogic.CanDiscard(room, target, target, id))
                    {
                        discard = false;
                        break;
                    }
                }
                return discard;
            }
            return false;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.ThrowAllHandCards(player);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> names = new List<string>();
            bool slash = false, jink = false, peach = false, ana = false, weapon = false, armor = false, oh = false, dh = false, treasure = false;
            List<int> ids = new List<int>(room.DiscardPile);
            ids.AddRange(room.DrawPile);

            Shuffle.shuffle(ref ids);

            List<int> get = new List<int>();
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (!slash && fcard is Slash)
                {
                    get.Add(id);
                    slash = true;
                }
                else if (!jink && fcard is Jink)
                {
                    get.Add(id);
                    jink = true;
                }
                else if (!peach && fcard is Peach)
                {
                    get.Add(id);
                    peach = true;
                }
                else if (!ana && fcard is Analeptic)
                {
                    get.Add(id);
                    ana = true;
                }
                else if (!dh && fcard is DefensiveHorse)
                {
                    get.Add(id);
                    dh = true;
                }
                else if (!weapon && fcard is Weapon)
                {
                    get.Add(id);
                    weapon = true;
                }
                else if (!armor && fcard is Armor)
                {
                    get.Add(id);
                    armor = true;
                }
                else if (!oh && fcard is OffensiveHorse)
                {
                    get.Add(id);
                    oh = true;
                }
                else if (!treasure && fcard is Treasure)
                {
                    get.Add(id);
                    treasure = true;
                }
                else if (fcard is TrickCard && !names.Contains(card.Name))
                {
                    get.Add(id);
                    names.Add(card.Name);
                }

                if (get.Count >= 8)
                    break;
            }

            if (get.Count > 0)
            {
                player.SetFlags(Name);
                room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty));
            }

            return false;
        }
    }

    public class QingjiaoThrow : TriggerSkill
    {
        public QingjiaoThrow() : base("#qingjiao")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Compulsory;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish && !target.IsNude() && target.HasFlag("qingjiao");
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.ThrowAllHandCardsAndEquips(player);
            return false;
        }
    }

    public class QianxinZG : ViewAsSkill
    {
        public QianxinZG() : base("qianxin_zg")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng() && !player.HasUsed(QianxinZGCard.ClassName) && !player.ContainsTag(Name);
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard qx = new WrappedCard(QianxinZGCard.ClassName) { Skill = Name };
                qx.AddSubCards(cards);
                return qx;
            }
            return null;
        }
    }

    public class QianxinZGCard : SkillCard
    {
        public static string ClassName = "QianxinZGCard";
        public QianxinZGCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = new List<int>(card_use.Card.SubCards);
            player.SetTag("qianxin_zg", ids);
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.ContainsTag("qianxin_zg_from") && p.GetTag("qianxin_zg_from") is List<string> names && names.Contains(player.Name))
                {
                    names.Remove(player.Name);
                    if (names.Count == 0)
                        p.RemoveTag("qianxin_zg_from");
                    else
                        p.SetTag("qianxin_zg_from", names);
                }
            }

            List<string> froms = target.ContainsTag("qianxin_zg_from") ? (List<string>)target.GetTag("qianxin_zg_from") : new List<string>();
            froms.Add(player.Name);
            target.SetTag("qianxin_zg_from", froms);
            
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_PUT, player.Name, Name, string.Empty);
            CardsMoveStruct move = new CardsMoveStruct(ids, null, Place.PlaceUnknown, reason)
            {
                To_pile_name = string.Empty,
                From = player.Name
            };
            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
            room.MoveCardsAtomic(moves, false);

            for (int i = 0; i < ids.Count; i++)
            {
                int id = ids[i];
                room.SetCardMapping(id, null, Place.DrawPile);
                int index = room.AliveCount() * (i + 1) - 1;
                index = Math.Min(index, room.DrawPile.Count);
                room.DrawPile.Insert(index, id);
            }

            object data = room.DrawPile.Count;
            room.RoomThread.Trigger(TriggerEvent.DrawPileChanged, room, null, ref data);
            List<string> arg = new List<string>
            {
                room.DrawPile.Count.ToString()
            };
            room.DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_PILE, arg);
        }
    }

    public class QianxinEffect : TriggerSkill
    {
        public QianxinEffect() : base("#qianxin_zg")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move) 
            {
                if (move.From_places.Contains(Place.DrawPile))
                {
                    if (move.To != null && move.To_place == Place.PlaceHand && move.To == room.Current && move.To.ContainsTag("qianxin_zg_from")
                        && move.To.GetTag("qianxin_zg_from") is List<string> names)
                    {
                        Dictionary<string, List<int>> from_ids = move.To.ContainsTag("qianxin_zg_get")
                            ? (Dictionary<string, List<int>>)move.To.GetTag("qianxin_zg_get") : new Dictionary<string, List<int>>();
                        foreach (string general in names)
                        {
                            Player target = room.FindPlayer(general, true);
                            if (target.ContainsTag("qianxin_zg") && target.GetTag("qianxin_zg") is List<int> ids)
                            {
                                List<int> get = ids.FindAll(t => move.Card_ids.Contains(t));
                                if (get.Count > 0)
                                {
                                    if (from_ids.ContainsKey(target.Name))
                                        from_ids[target.Name].AddRange(get);
                                    else
                                        from_ids[target.Name] = get;
                                }
                            }
                        }
                        if (from_ids.Count > 0)
                            move.To.SetTag("qianxin_zg_get", from_ids);
                    }

                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.ContainsTag("qianxin_zg") && p.GetTag("qianxin_zg") is List<int> ids && ids.Find(t => move.Card_ids.Contains(t)) > 0)
                        {
                            ids.RemoveAll(t => move.Card_ids.Contains(t));
                            if (ids.Count > 0)
                                p.SetTag("qianxin_zg", ids);
                            else
                                p.RemoveTag("qianxin_zg");
                        }
                    }
                }
                else if (move.From_places.Contains(Place.PlaceHand) && move.From.ContainsTag("qianxin_zg_get") && move.From.GetTag("qianxin_zg_get") is Dictionary<string, List<int>> from_ids)
                {
                    List<string> keys = new List<string>(from_ids.Keys);
                    foreach (string key in keys)
                    {
                        from_ids[key].RemoveAll(t => move.Card_ids.Contains(t));
                    }
                    foreach (string key in keys)
                        if (from_ids[key].Count == 0) from_ids.Remove(key);

                    if (from_ids.Count > 0)
                        move.From.SetTag("qianxin_zg_get", from_ids);
                    else
                        move.From.RemoveTag("qianxin_zg_get");
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Discard && player.ContainsTag("qianxin_zg_get"))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.GetTag("qianxin_zg_get") is Dictionary<string, List<int>> from_ids)
            {
                bool has = false;
                player.RemoveTag("qianxin_zg_get");
                List<Player> from = new List<Player>();
                foreach (string general in from_ids.Keys)
                {
                    if (from_ids[general].Find(t => player.GetCards("h").Contains(t)) > 0)
                    {
                        has = true;
                        Player target = room.FindPlayer(general);
                        if (target != null && target.HandcardNum < 4)
                            from.Add(target);
                    }
                }

                if (has)
                {
                    room.SetTag("qianxin_zg", from);
                    bool draw = false;
                    if (from.Count > 0 && room.AskForChoice(player, "qianxin_zg", "draw+max", null, from) == "draw")
                    {
                        draw = true;
                        room.SortByActionOrder(ref from);
                        foreach (Player p in from)
                            if (p.Alive) room.DrawCards(p, new DrawCardStruct(4 - p.HandcardNum, player, "qianxin_zg"));
                    }
                    room.RemoveTag("qianxin_zg");

                    if (!draw)
                    {
                        player.SetFlags("qianxin_zg");
                        LogMessage log = new LogMessage
                        {
                            Type = "#qianxin_zg-less",
                            From = player.Name,
                            Arg = "qianxin_zg"
                        };
                        room.SendLog(log);
                    }
                }
            }

            return false;
        }
    }

    public class QianxinMax : MaxCardsSkill
    {
        public QianxinMax() : base("#qianxin-zg-max") { }
        public override int GetExtra(Room room, Player target)
        {
            return target.HasFlag("qianxin_zg") ? -2: 0;
        }
    }
    public class Zhenxing : TriggerSkill
    {
        public Zhenxing() : base("zhenxing")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Damaged };
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish || triggerEvent == TriggerEvent.Damaged))
                return new TriggerStruct(Name, player);
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
            int count = int.Parse(room.AskForChoice(player, Name, "1+2+3", new List<string> { "@zhenxing-view" }));
            List<int> guanxing = room.GetNCards(count, false);
            if (count == 1)
                room.ObtainCard(player, ref guanxing, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty), false);
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "$ViewDrawPile",
                    From = player.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(guanxing))
                };
                room.SendLog(log, player);
                log.Type = "$ViewDrawPile2";
                log.Arg = guanxing.Count.ToString();
                log.Card_str = null;
                room.SendLog(log, new List<Player> { player });

                bool option = true;
                List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
                foreach (int card in guanxing)
                    suits.Add(room.GetCard(card).Suit);
                foreach (int card in guanxing)
                {
                    if (suits.Count(t => t == room.GetCard(card).Suit) == 1)
                    {
                        option = false;
                        break;
                    }
                }

                room.SetTag(Name, guanxing);
                AskForMoveCardsStruct move = room.AskForMoveCards(player, guanxing, new List<int>(), false, Name, 1, 1, option, false, new List<int>(), info.SkillPosition);
                room.RemoveTag(Name);
                if (move.Success && move.Bottom.Count == 1)
                {
                    List<int> get = new List<int>(move.Bottom);
                    room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty), false);
                }
            }

            return false;
        }
        public override bool MoveFilter(Room room, int id, List<int> downs)
        {
            if (id > -1 && room.ContainsTag(Name) && room.GetTag(Name) is List<int> ids)
            {
                List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
                foreach (int card in ids)
                    suits.Add(room.GetCard(card).Suit);

                int count = suits.Count(t => t == room.GetCard(id).Suit);
                return count == 1;
            }

            return true;
        }
    }

    public class Tuiyan : TriggerSkill
    {
        public Tuiyan() : base("tuiyan")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Wizzard;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Play;
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
            List<int> ids = room.GetNCards(2, false);

            LogMessage log = new LogMessage
            {
                Type = "$ViewDrawPile",
                From = player.Name,
                Card_str = string.Join("+", JsonUntity.IntList2StringList(ids))
            };
            room.SendLog(log, player);
            log.Type = "$ViewDrawPile2";
            log.Arg = ids.Count.ToString();
            log.Card_str = null;
            room.SendLog(log, new List<Player> { player });

            room.FillAG(Name, ids, player);
            room.AskForAG(player, new List<int>(), true, Name);
            room.ClearAG(player);

            return false;
        }
    }

    public class Busuan : ZeroCardViewAsSkill
    {
        public Busuan() : base("busuan") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(BusuanCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(BusuanCard.ClassName) { Skill = Name };
        }
    }

    public class BusuanCard : SkillCard
    {
        public static string ClassName = "BusuanCard";
        public BusuanCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card) => targets.Count == 0 && to_select != Self;

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<string> cards = new List<string>(), choices = new List<string>();
            foreach (FunctionCard fcard in room.AvailableFunctionCards)
            {
                if (fcard is EquipCard || fcard is FireSlash || fcard is ThunderSlash) continue;
                choices.Add(fcard.Name);
            }
            string choice1 = room.AskForChoice(player, "busuan", string.Join("+", choices), new List<string> { "@to-player:" + target.Name }, target);
            choices.Remove(choice1);
            choices.Add("cancel");
            cards.Add(choice1);
            string choice2 = room.AskForChoice(player, "busuan", string.Join("+", choices), new List<string> { "@to-player:" + target.Name }, target);
            if (choice2 != "cancel")
                cards.Add(choice2);
            target.SetTag("busuan", cards);
        }
    }
    public class BusuanDraw : DrawCardsSkill
    {
        public BusuanDraw() : base("#busuan")
        {
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Draw && player.ContainsTag("busuan"))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            int count = n;
            if (player.GetTag("busuan") is List<string> cards)
            {
                player.RemoveTag("busuan");
                if (n > 0)
                {
                    List<int> ids = new List<int>(room.DrawPile);
                    ids.AddRange(room.DiscardPile);
                    Shuffle.shuffle(ref ids);
                    List<int> get = new List<int>();
                    foreach (string card_name in cards)
                    {
                        ids.RemoveAll(t => get.Contains(t));
                        foreach (int id in ids)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name.Contains(card_name))
                            {
                                get.Add(id);
                                count--;
                                break;
                            }
                        }
                        if (count == 0)
                            break;
                    }

                    if (get.Count > 0)
                        room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, "busuan", string.Empty));
                }
            }

            return count;
        }
    }

    public class Mingjie : TriggerSkill
    {
        public Mingjie() : base("mingjie")
        {
            events.Add(TriggerEvent.EventPhaseStart);
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish;
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
            List<int> get = new List<int>(), ids = new List<int>();
            bool red = true;
            do
            {
                ids = room.GetNCards(1, true);
                get.AddRange(ids);
                room.MoveCardTo(room.GetCard(ids[0]), null, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name, Name, string.Empty));
                red = WrappedCard.IsRed(room.GetCard(ids[0]).Suit);
            }
            while (get.Count < 3 && red && room.AskForSkillInvoke(player, Name, null, info.SkillPosition));

            if (get.Count > 0)
                room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty));

            if (!red && player.Alive) room.LoseHp(player);

            return false;
        }
    }

    public class Weilu : TriggerSkill
    {
        public Weilu() : base("weilu")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.ContainsTag("weilu_hp") && p.GetTag("weilu_hp") is int count)
                    {
                        p.RemoveTag("weilu_hp");
                        int n = Math.Min(count, p.GetLostHp());
                        if (n > 0)
                        {
                            RecoverStruct recover = new RecoverStruct
                            {
                                Recover = n,
                                Who = p
                            };
                            room.Recover(p, recover, true);
                        }
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From != player && damage.From.Alive)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && player.ContainsTag(Name))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                room.SetPlayerStringMark(damage.From, Name, string.Empty);

                List<string> generals = player.ContainsTag(Name) ? (List<string>)player.GetTag(Name) : new List<string>();
                generals.Add(damage.From.Name);
                player.SetTag(Name, generals);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.GetTag(Name) is List<string> names)
            {
                player.RemoveTag(Name);
                List<Player> targets = new List<Player>();
                foreach (string general in names)
                {
                    Player target = room.FindPlayer(general);
                    if (target != null)
                        room.RemovePlayerStringMark(target, Name);

                    if (target != null && !targets.Contains(target) && target.Hp > 1)
                        targets.Add(target);
                }

                if (targets.Count > 0)
                {
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.SortByActionOrder(ref targets);
                    foreach (Player p in targets)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                        int count = p.Hp - 1;
                        p.SetTag("weilu_hp", count);
                        room.LoseHp(p, count);
                    }
                }
            }

            return false;
        }
    }

    public class Zengdao : ViewAsSkill
    {
        public Zengdao() : base("zengdao")
        {
            limit_mark = "@zengdao";
            frequency = Frequency.Limited;
            skill_type = SkillType.Replenish;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.HasEquip() && player.GetMark(limit_mark) > 0;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return room.GetCardPlace(to_select.Id) == Place.PlaceEquip;
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            WrappedCard zd = new WrappedCard(ZengdaoCard.ClassName) { Skill = Name, Mute = true };
            zd.AddSubCards(cards);
            return zd;
        }
    }

    public class ZengdaoCard : SkillCard
    {
        public static string ClassName = "ZengdaoCard";
        public ZengdaoCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return base.TargetFilter(room, targets, to_select, Self, card);
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@zengdao", 0);
            room.BroadcastSkillInvoke("zengdao", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "zengdao");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.AddToPile(target, "zengdao", ids);
        }
    }

    public class ZengdaoDamage : TriggerSkill
    {
        public ZengdaoDamage() : base("#zengdao")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Alive && player.GetPile("zengdao").Count > 0)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = player.GetPile("zengdao");
            CardsMoveStruct move = new CardsMoveStruct(ids[0], null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, ask_who.Name, Name, string.Empty));
            room.MoveCardsAtomic(move, true);

            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#AddDamage",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = "zengdao",
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);
            data = damage;

            return false;
        }
    }

    public class Chijie : TriggerSkill
    {
        public Chijie() : base("chijie")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirmed, TriggerEvent.CardFinished };
            skill_type = SkillType.Masochism;
            priority = new Dictionary<TriggerEvent, double>
            {
                { TriggerEvent.TargetConfirmed, 3 },
                { TriggerEvent.CardFinished, 2 },
            };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && room.ContainsTag("chijie_card")
                && room.GetTag("chijie_card") is Dictionary<WrappedCard, List<Player>> _chijie && _chijie.ContainsKey(use.Card))
            {
                _chijie.Remove(use.Card);
                if (_chijie.Count == 0)
                    room.RemoveTag("chijie_card");
                else
                    room.SetTag("chijie_card", _chijie);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.From != player && !player.HasFlag(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is DelayedTrick) && !(fcard is SkillCard) && (use.Card.SubCards.Count > 0 || use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == FireAttack.ClassName
                    || use.Card.Name == Duel.ClassName || use.Card.Name == SavageAssault.ClassName || use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == HiddenDagger.ClassName
                    || use.Card.Name == HoneyTrap.ClassName))
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag(Name, data);
                bool invoke = room.AskForSkillInvoke(player, Name, string.Format("@chijie:{0}::{1}", use.From.Name, use.Card.Name), info.SkillPosition);
                room.RemoveTag(Name);
                if (invoke)
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", -1, gsk.General, gsk.SkinId);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                player.SetFlags(Name);
                Dictionary<WrappedCard, List<Player>> chijie = room.ContainsTag("chijie_card") ? (Dictionary<WrappedCard, List<Player>>)room.GetTag("chijie_card")
                    : new Dictionary<WrappedCard, List<Player>>();
                if (chijie.ContainsKey(use.Card))
                    chijie[use.Card].Add(player);
                else
                    chijie[use.Card] = new List<Player> { player };
                room.SetTag("chijie_card", chijie);
            }

            return false;
        }
    }

    public class ChijieEffect : TriggerSkill
    {
        public ChijieEffect() : base("#chijie")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.Damaged, TriggerEvent.DamageInflicted, TriggerEvent.CardFinished };
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {

            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.Card != null && !damage.Card.HasFlag("chijie_damage")
                && room.ContainsTag("chijie_card") && room.GetTag("chijie_card") is Dictionary<WrappedCard, List<Player>> chijie && chijie.ContainsKey(damage.Card))
            {
                damage.Card.SetFlags("chijie_damage");
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct _damage && player.Alive && _damage.Card != null && !_damage.Card.HasFlag("chijie")
                && room.ContainsTag("chijie_card") && room.GetTag("chijie_card") is Dictionary<WrappedCard, List<Player>> _chijie && _chijie.ContainsKey(_damage.Card)
                && _chijie[_damage.Card].Contains(player))
            {
                _chijie.Remove(_damage.Card);
                if (_chijie.Count == 0)
                    room.RemoveTag("chijie_card");
                else
                    room.SetTag("chijie_card", _chijie);
                _damage.Card.SetFlags("chijie");
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct damage && damage.Card != null && damage.Card.HasFlag("chijie"))
            {
                triggers.Add(new TriggerStruct(Name, player));
            }
            else if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && room.ContainsTag("chijie_card") && !use.Card.HasFlag("chijie_damage")
                && room.GetTag("chijie_card") is Dictionary<WrappedCard, List<Player>> chijie && chijie.ContainsKey(use.Card) && use.Card.SubCards.Count > 0)
            {
                List<int> ids = new List<int>(use.Card.SubCards), subs = room.GetSubCards(use.Card);
                if (ids.SequenceEqual(subs))
                {
                    bool check = true;
                    foreach (int id in ids)
                    {
                        if (room.GetCardPlace(id) != Place.DiscardPile)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        foreach (Player p in chijie[use.Card])
                            if (p.Alive) triggers.Add(new TriggerStruct(Name, p));
                    }
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.DamageInflicted)
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
            else if (data is CardUseStruct use)
            {
                List<int> ids = new List<int>(use.Card.SubCards), subs = room.GetSubCards(use.Card);
                if (ids.SequenceEqual(subs))
                {
                    bool check = true;
                    foreach (int id in ids)
                    {
                        if (room.GetCardPlace(id) != Place.DiscardPile)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        room.RemoveSubCards(use.Card);
                        room.SendCompulsoryTriggerLog(ask_who, "chijie");

                        room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, ask_who.Name, "chijie", string.Empty));
                    }
                }
            }

            return false;
        }
    }

    public class Yinju : ZeroCardViewAsSkill
    {
        public Yinju() : base("yinju")
        {
            skill_type = SkillType.Defense;
            limit_mark = "@yinju";
            frequency = Frequency.Limited;
        }

        public override bool IsEnabledAtPlay(Room room, Player player) => player.GetMark(limit_mark) > 0;
        public override WrappedCard ViewAs(Room room, Player player) => new WrappedCard(YinjuCard.ClassName) { Skill = Name, Mute = true };
    }

    public class YinjuCard : SkillCard
    {
        public static string ClassName = "YinjuCard";
        public YinjuCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card) => targets.Count == 0 && to_select != Self;

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@yinju", 0);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, card_use.From, "yinju", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("yinju", "male", 1, gsk.General, gsk.SkinId);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "yinju");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            card_use.To[0].SetFlags("yinju_" + card_use.From.Name);
        }
    }

    public class YinjuEffect : TriggerSkill
    {
        public YinjuEffect() : base("#yinju")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.TargetChosen };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage && damage.To.HasFlag("yinju_" + player.Name))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill)
            {
                foreach (Player p in use.To)
                    if (p.HasFlag("yinju_" + player.Name)) return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, "yingju");
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "yinju", info.SkillPosition);
            room.BroadcastSkillInvoke("yinju", "male", 2, gsk.General, gsk.SkinId);
            if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage)
            {
                Player to = damage.To;
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, to.Name);
                if (to.IsWounded())
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = Math.Min(to.GetLostHp(), damage.Damage),
                        Who = player
                    };
                    room.Recover(to, recover, true);
                }

                LogMessage log = new LogMessage
                {
                    Type = "#damage-prevent",
                    From = player.Name,
                    To = new List<string> { to.Name },
                    Arg = Name
                };
                room.SendLog(log);
                return true;
            }
            else if (triggerEvent == TriggerEvent.TargetChosen)
                room.DrawCards(player, 1, Name);

            return false;
        }
    }

    public class Zhuilie : TriggerSkill
    {
        public Zhuilie() : base("zhuilie")
        {
            events.Add(TriggerEvent.TargetChosen);
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room) && use.Card.Name.Contains(Slash.ClassName))
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in use.To)
                    if (!RoomLogic.InMyAttackRange(room, player, p))
                        targets.Add(p);

                if (targets.Count > 0)
                    return new TriggerStruct(Name, player, targets);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player player, TriggerStruct info)
        {
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, skill_target.Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.SendCompulsoryTriggerLog(player, Name, true);
            return info;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                if (use.AddHistory)
                {
                    use.AddHistory = false;
                    player.AddHistory(use.Card.Name, -1);
                    data = use;
                }

                JudgeStruct judge = new JudgeStruct
                {
                    Pattern = "Weapon#Horse",
                    Good = true,
                    PlayAnimation = false,
                    Reason = Name,
                    Who = player
                };
                player.SetTag(Name, target.Name);
                room.Judge(ref judge);

                if (judge.IsGood() && target.Alive && target.Hp > 1)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#Liegong_add",
                        From = target.Name,
                        To = new List<string> { target.Name },
                        Arg = (target.Hp - 1).ToString()
                    };
                    room.SendLog(log);

                    int index = 0;
                    for (int i = 0; i < use.EffectCount.Count; i++)
                    {
                        CardBasicEffect effect = use.EffectCount[i];
                        if (effect.To == target)
                        {
                            index++;
                            if (index == info.Times)
                            {
                                effect.Effect1 += target.Hp - 1;
                                data = use;
                                break;
                            }
                        }
                    }
                }
                else if (judge.IsBad() && player.Alive)
                    room.LoseHp(player);
            }

            return false;
        }
    }
    
    public class ZhuilieTar : TargetModSkill
    {
        public ZhuilieTar() : base("#zhuilie")
        {
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from != null && RoomLogic.PlayerHasSkill(room, from, "zhuilie") && to != null;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class Tunan : ViewAsSkill
    {
        public Tunan() : base("tunan")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseStruct.CardUseReason reason, string pattern, string position = null)
        {
            switch (reason)
            {
                case CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY:
                    return RoomLogic.PlayerHasSkill(room, invoker, Name) && !invoker.HasUsed(TunanCard.ClassName);
                case CardUseReason.CARD_USE_REASON_RESPONSE_USE when pattern.StartsWith("@@tunan"):
                    return true;
                default:
                    return false;
            }
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (player == room.Current && cards.Count == 0)
                return new WrappedCard(TunanCard.ClassName) { Skill = Name };
            else if (player != room.Current && cards.Count == 1)
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
            if (player != room.Current && room.GetRoomState().GetCurrentCardUsePattern() == "@@tunan")
            {
                int id = player.GetMark(Name);
                if (!player.HasFlag(Name))
                {
                    result.Add(room.GetCard(id));
                }
                else
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_tunan" };
                    slash.AddSubCard(id);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    result.Add(slash);
                }
            }

            return result;
        }
    }

    public class TunanTag : TargetModSkill
    {
        public TunanTag() : base("#tunan", false) { pattern = ".#@@tunan"; }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && !card.IsVirtualCard() && (room.GetRoomState().GetCurrentResponseSkill() == "tunan" || pattern == "@@tunan"))
                return true;

            return false;
        }
    }

    public class TunanCard : SkillCard
    {
        public static string ClassName = "TunanCard";
        public TunanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card) => targets.Count == 0 && to_select != Self;

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            List<int> ids = room.GetNCards(1, false);
            target.SetMark("tunan", ids[0]);
            WrappedCard card = room.GetCard(ids[0]);
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (!fcard.IsAvailable(room, target, card) || room.AskForUseCard(target, "@@tunan", "@tunan-use", null, -1, HandlingMethod.MethodUse, false) == null)
            {
                target.SetFlags("tunan");
                room.AskForUseCard(target, "@@tunan", "@tunan-slash", null, -1, HandlingMethod.MethodUse, false);
                target.SetFlags("-tunan");
            }
            target.SetMark("tunan", 0);
        }
    }

    public class Bijing : TriggerSkill
    {
        public Bijing() : base("bijing")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            return triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish
                && !target.IsKongcheng() ? new TriggerStruct(Name, target) : new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.AskForExchange(player, Name, 1, 0, "@bijing", string.Empty, ".", info.SkillPosition);
            if (ids.Count == 1)
            {
                player.SetTag(Name, ids[0]);
                room.NotifySkillInvoked(player, Name);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "bijing", info.SkillPosition);
                room.BroadcastSkillInvoke("bijing", "male", 1, gsk.General, gsk.SkinId);

                List<string> args = new List<string> { CommonClass.JsonUntity.Object2Json(ids), Name };
                room.DoNotify(room.GetClient(player), CommandType.S_COMMAND_UPDATE_CARD_FOOTNAME, args);

                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return false;
        }
    }

    public class BijingDiscard : TriggerSkill
    {
        public BijingDiscard() : base("#bijing")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.ContainsTag("bijing") && move.From.Alive
                && move.From.GetTag("bijing") is int id && move.From.Phase == PlayerPhase.NotActive && move.Card_ids.Contains(id) && move.From_places[move.Card_ids.IndexOf(id)] == Place.PlaceHand)
            {
                return new TriggerStruct(Name, move.From);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                if (player.Phase == PlayerPhase.Start && player.ContainsTag("bijing"))
                    return new TriggerStruct(Name, player);
                else if (player.Phase == PlayerPhase.Discard && player.GetMark("bijing") > 0)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime)
            {
                ask_who.RemoveTag("bijing");
                room.SendCompulsoryTriggerLog(ask_who, "bijing");
                if (room.Current != null && room.Current.Alive)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, room.Current.Name);
                    room.Current.AddMark("bijing");
                    room.SetPlayerStringMark(room.Current, "bijing", string.Empty);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                if (player.Phase == PlayerPhase.Start && player.ContainsTag("bijing") && player.GetTag("bijing") is int id)
                {
                    room.RemoveTag("bijing");
                    if (player.GetCards("h").Contains(id) && RoomLogic.CanDiscard(room, player, player, id))
                    {
                        if (RoomLogic.PlayerHasSkill(room, player, Name))
                        {
                            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "bijing", info.SkillPosition);
                            room.BroadcastSkillInvoke("bijing", "male", 2, gsk.General, gsk.SkinId);
                        }
                        room.ThrowCard(id, player);
                    }
                }
                else if (player.Phase == PlayerPhase.Discard && player.GetMark("bijing") > 0)
                {
                    player.SetMark("bijing", 0);
                    Player target = RoomLogic.FindPlayerBySkillName(room, Name);
                    if (target != null)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, player.Name);
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, target, "bijing", info.SkillPosition);
                        room.BroadcastSkillInvoke("bijing", "male", 2, gsk.General, gsk.SkinId);
                    }
                    room.AskForDiscard(player, "bijing", 2, 2, false, true, "@bijing-discard");
                    room.RemovePlayerStringMark(player, "bijing");
                }
            }

            return false;
        }
    }

    public class Tianjiang : TriggerSkill
    {
        public Tianjiang() : base("tianjiang")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart };
            view_as_skill = new TianjiangVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
            {
                bool weapon = false, armor = false, oh = false, dh = false, treasure = false;
                List<int> ids = new List<int>(room.DrawPile);
                Shuffle.shuffle(ref ids);

                List<int> get = new List<int>();
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!dh && fcard is DefensiveHorse)
                    {
                        get.Add(id);
                        dh = true;
                    }
                    else if (!weapon && fcard is Weapon)
                    {
                        get.Add(id);
                        weapon = true;
                    }
                    else if (!armor && fcard is Armor)
                    {
                        get.Add(id);
                        armor = true;
                    }
                    else if (!oh && fcard is OffensiveHorse)
                    {
                        get.Add(id);
                        oh = true;
                    }
                    else if (!treasure && fcard is Treasure)
                    {
                        get.Add(id);
                        treasure = true;
                    }

                    if (get.Count >= 2)
                        break;
                }

                room.SendCompulsoryTriggerLog(player, Name);
                room.BroadcastSkillInvoke(Name, player);

                foreach (int id in get)
                {
                    WrappedCard equip_card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(equip_card.Name);
                    EquipCard equip = (EquipCard)fcard;
                    int equip_index = (int)equip.EquipLocation();

                    int equipped_id = player.GetEquip(equip_index);
                    List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct>();
                    if (equipped_id != -1)
                    {
                        CardsMoveStruct move1 = new CardsMoveStruct(equipped_id, player, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
                        exchangeMove.Add(move1);
                        room.MoveCardsAtomic(exchangeMove, true);
                    }
                    CardsMoveStruct move2 = new CardsMoveStruct(id, player, Place.PlaceEquip, new CardMoveReason(MoveReason.S_REASON_PUT, player.Name, Name, string.Empty));
                    exchangeMove.Add(move2);
                    room.MoveCardsAtomic(exchangeMove, true);

                    LogMessage log = new LogMessage
                    {
                        From = player.Name,
                        Type = "$Install",
                        Card_str = id.ToString()
                    };
                    room.SendLog(log);

                    if (equipped_id != -1)
                    {
                        if (room.GetCardPlace(equipped_id) == Place.PlaceTable)
                        {
                            CardsMoveStruct move3 = new CardsMoveStruct(equipped_id, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
                            room.MoveCardsAtomic(new List<CardsMoveStruct> { move3 }, true);
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

    public class TianjiangVS : OneCardViewAsSkill
    {
        public TianjiangVS() : base("tianjiang")
        {
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player) => room.GetCardPlace(to_select.Id) == Place.PlaceEquip;

        public override bool IsEnabledAtPlay(Room room, Player player) => player.HasEquip();

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard tj = new WrappedCard(TianjiangCard.ClassName) { Skill = Name };
            tj.AddSubCard(card);
            return tj;
        }
    }

    public class TianjiangCard : SkillCard
    {
        public static string ClassName = "TianjiangCard";
        public TianjiangCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0 || to_select == Self)
                return false;
            WrappedCard equip_card = room.GetCard(card.SubCards[0]);
            FunctionCard fcard = Engine.GetFunctionCard(equip_card.Name);
            EquipCard equip = (EquipCard)fcard;
            int equip_index = (int)equip.EquipLocation();
            return RoomLogic.CanPutEquip(to_select, equip_card);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.To[0];
            int id = card_use.Card.GetEffectiveId();
            WrappedCard equip_card = room.GetCard(id);
            FunctionCard fcard = Engine.GetFunctionCard(equip_card.Name);
            EquipCard equip = (EquipCard)fcard;
            int equip_index = (int)equip.EquipLocation();

            int equipped_id = player.GetEquip(equip_index);
            List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct>();
            if (equipped_id != -1)
            {
                CardsMoveStruct move1 = new CardsMoveStruct(equipped_id, player, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
                exchangeMove.Add(move1);
                room.MoveCardsAtomic(exchangeMove, true);
            }
            CardsMoveStruct move2 = new CardsMoveStruct(id, card_use.From, player, Place.PlaceEquip,
                                  Place.PlaceEquip, new CardMoveReason(MoveReason.S_REASON_TRANSFER, player.Name, Name, string.Empty));
            exchangeMove.Add(move2);
            room.MoveCardsAtomic(exchangeMove, true);

            if (equipped_id != -1)
            {
                if (room.GetCardPlace(equipped_id) == Place.PlaceTable)
                {
                    CardsMoveStruct move3 = new CardsMoveStruct(equipped_id, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
                    room.MoveCardsAtomic(new List<CardsMoveStruct> { move3 }, true);
                }
            }
        }
    }

    public class Zhuren : TriggerSkill
    {
        public Zhuren() : base("zhuren")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Wizzard;
            view_as_skill = new ZhurenVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile)
            {
                List<int> remove = new List<int>();
                foreach (int id in move.Card_ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (room.GetCardPlace(id) == Place.DiscardPile && (card.Name == Lance.ClassName || card.Name == PosionedDagger.ClassName || card.Name == QuenchedKnife.ClassName
                        || card.Name == LightningSummoner.ClassName || card.Name == WaterSword.ClassName))
                    {
                        room.SetTag(string.Format("{0}_{1}", Name, card.Name), false);
                        remove.Add(id);
                    }
                }

                if (remove.Count > 0)
                {
                    if (move.Reason.Card != null)
                    {
                        List<int> subs = room.GetSubCards(move.Reason.Card);
                        subs.RemoveAll(t => remove.Contains(t));
                    }

                    Player holder = room.Players[0];
                    room.AddToPile(holder, "#virtual_cards", remove, false);
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class ZhurenVS : OneCardViewAsSkill
    {
        public ZhurenVS() : base("zhuren")
        {
            filter_pattern = ".!";
        }
        
        public override bool IsEnabledAtPlay(Room room, Player player) => !player.HasUsed(ZhurenCard.ClassName) && !player.IsKongcheng();
        //public override bool IsEnabledAtPlay(Room room, Player player) => !player.IsKongcheng();

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard zr = new WrappedCard(ZhurenCard.ClassName) { Skill = Name };
            zr.AddSubCard(card);
            return zr;
        }
    }

    public class ZhurenCard : SkillCard
    {
        public static string ClassName = "ZhurenCard";
        public ZhurenCard() : base(ClassName)
        {
            target_fixed = true;
        }
        private readonly Dictionary<WrappedCard.CardSuit, string> suits = new Dictionary<WrappedCard.CardSuit, string>
        {
            { WrappedCard.CardSuit.Heart, Lance.ClassName },
            { WrappedCard.CardSuit.Diamond, QuenchedKnife.ClassName },
            { WrappedCard.CardSuit.Spade, PosionedDagger.ClassName },
            { WrappedCard.CardSuit.Club, WaterSword.ClassName },
        };
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            int id = card_use.Card.GetEffectiveId();
            WrappedCard card = room.GetCard(id);
            string card_name;
            if (card.Name == Lightning.ClassName)
            {
                card_name = LightningSummoner.ClassName;
            }
            else
            {
                card_name = suits[card.Suit];
            }

            if (!room.ContainsTag(card_name))
            {
                foreach (int card_id in Engine.GetEngineCards())
                {
                    WrappedCard real_card = Engine.GetRealCard(card_id);
                    if (real_card.Name == card_name && room.GetCard(card_id) == null)
                    {
                        room.AddNewCard(card_id);
                        room.SetTag(string.Format("zhuren_{0}", card_name), false);
                        break;
                    }
                }
            }
            
            bool ingame = (bool)room.GetTag(string.Format("zhuren_{0}", card_name));
            List<int> ids = new List<int>();
            if (!ingame && Shuffle.random(card.Number, 18))
            {
                foreach (int card_id in room.RoomCards)
                {
                    if (room.GetCard(card_id).Name == card_name)
                    {
                        ids.Add(card_id);
                        break;
                    }
                }
                room.SetTag(string.Format("zhuren_{0}", card_name), true);
            }
            else
            {
                foreach (int card_id in room.DrawPile)
                {
                    if (room.GetCard(card_id).Name.Contains(Slash.ClassName))
                    {
                        ids.Add(card_id);
                        break;
                    }
                }
            }
            if (ids.Count > 0) room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, "zhuren", string.Empty));
        }
    }

    public class Manyi : TriggerSkill
    {
        public Manyi() : base("manyi")
        {
            events.Add(TriggerEvent.CardEffected);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && base.Triggerable(player, room) && data is CardEffectStruct effect && effect.Card.Name == SavageAssault.ClassName)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, this) || room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            LogMessage log = new LogMessage
            {
                Type = "#Skillnullify",
                From = player.Name,
                Arg = Name,
                Arg2 = SavageAssault.ClassName
            };
            room.SendLog(log);

            return true;
        }
    }

    public class Mansi : TriggerSkill
    {
        public Mansi() : base("mansi")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.CardFinished };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.Card != null && damage.Card.Name == SavageAssault.ClassName)
            {
                string str = RoomLogic.CardToString(room, damage.Card);
                int count = room.ContainsTag(str) ? (int)room.GetTag(str) : 0;
                count++;
                room.SetTag(str, count);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && use.Card.Name == SavageAssault.ClassName)
            {
                string str = RoomLogic.CardToString(room, use.Card);
                int count = room.ContainsTag(str) ? (int)room.GetTag(str) : 0;
                if (count > 0)
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
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
            if (data is CardUseStruct use)
            {
                string str = RoomLogic.CardToString(room, use.Card);
                int count = room.ContainsTag(str) ? (int)room.GetTag(str) : 0;
                ask_who.AddMark(Name, count);
                room.DrawCards(ask_who, count, Name);
            }
            return false;
        }
    }

    public class MansiClear : TriggerSkill
    {
        public MansiClear() : base("#mansi")
        {
            events.Add(TriggerEvent.CardFinished);
        }
        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardUseStruct use && use.Card.Name == SavageAssault.ClassName)
            {
                room.RemoveTag(RoomLogic.CardToString(room, use.Card));
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data) => new List<TriggerStruct>();
    }

    public class Souying : TriggerSkill
    {
        public Souying() : base("souying")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted, TriggerEvent.DamageCaused, TriggerEvent.Damage, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.From != null && damage.To != damage.From)
            {
                damage.From.AddMark(string.Format("{0}_to_{1}", Name, damage.To.Name));
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.Players)
                {
                    foreach (Player p2 in room.GetOtherPlayers(p))
                    {
                        string str = string.Format("{0}_to_{1}", Name, p2.Name);
                        if (p.GetMark(str) > 0)
                            p.SetMark(str, 0);
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room) || player.HasFlag(Name)) return new TriggerStruct();
            if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage && damage.To.IsMale())
            {
                string str = string.Format("{0}_to_{1}", Name, damage.To.Name);
                if (player.GetMark(str) == 1)
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct _damage && _damage.From != null && _damage.From.IsMale())
            {
                string str = string.Format("{0}_to_{1}", Name, player.Name);
                if (_damage.From.GetMark(str) == 1)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                string prompt;
                if (triggerEvent == TriggerEvent.DamageCaused)
                {
                    prompt = string.Format("@souying-add:{0}", damage.To.Name);
                }
                else
                {
                    prompt = string.Format("@souying-reduce:{0}", damage.From.Name);
                }
                if (room.AskForDiscard(player, Name, 1, 1, true, false, prompt, true, info.SkillPosition))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", triggerEvent == TriggerEvent.DamageCaused ? 1 : 2, gsk.General, gsk.SkinId);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            if (data is DamageStruct damage)
            {
                if (triggerEvent == TriggerEvent.DamageInflicted)
                {
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
                }
                else
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#AddDamage",
                        From = player.Name,
                        To = new List<string> { damage.To.Name },
                        Arg = Name,
                        Arg2 = (++damage.Damage).ToString()
                    };
                    room.SendLog(log);

                    data = damage;
                }
            }
            return false;
        }
    }

    public class Zhanyuan : PhaseChangeSkill
    {
        public Zhanyuan() : base("zhanyuan")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0 && player.GetMark("mansi") > 7)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);
            if (player.Alive)
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

                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.IsMale()) targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@zhanyuan", true, false, info.SkillPosition);
                    if (target != null)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                        room.HandleAcquireDetachSkills(player, "xili", true);
                        room.HandleAcquireDetachSkills(target, "xili", true);
                        room.HandleAcquireDetachSkills(player, "-mansi", false);
                    }
                }
            }

            return false;
        }
    }

    public class Xili : TriggerSkill
    {
        public Xili() : base("xili")
        {
            events.Add(TriggerEvent.TargetChosen);
            skill_type = SkillType.Attack;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardUseStruct use && base.Triggerable(player, room) && use.Card.Name.Contains(Slash.ClassName))
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p != player && p.Phase == PlayerPhase.NotActive)
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                if (room.AskForDiscard(ask_who, Name, 1, 1, true, false, string.Format("@xili:{0}::{1}", player.Name, use.Card.Name), true, info.SkillPosition))
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#xili",
                    From = ask_who.Name,
                    To = new List<string> { player.Name },
                    Arg = Name,
                    Arg2 = use.Card.Name
                };
                room.SendLog(log);

                use.ExDamage++;
                data = use;
            }
            return false;
        }
    }

    public class Jiedao : TriggerSkill
    {
        public Jiedao() : base("jiedao")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageCaused)
                player.AddMark(Name);
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0) p.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageCaused && base.Triggerable(player, room) && player.IsWounded() && player.GetMark(Name) == 1)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && room.AskForSkillInvoke(player, Name, damage.To, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                List<string> choices = new List<string>();
                for (int i = 1; i <= player.GetLostHp(); i++)
                    choices.Add(i.ToString());

                string choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { "@jiedao:" + damage.To.Name }, data);
                int count = int.Parse(choice);
                string mark = string.Format("{0}:{1}", Name, choice);
                if (damage.Marks == null)
                    damage.Marks = new List<string> { mark };
                else
                    damage.Marks.Add(mark);

                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, damage.To.Name);
                LogMessage log = new LogMessage
                {
                    Type = "#AddDamage",
                    From = player.Name,
                    To = new List<string> { damage.To.Name },
                    Arg = Name,
                    Arg2 = (damage.Damage += count).ToString()
                };
                room.SendLog(log);

                data = damage;
            }
            return false;
        }
    }

    public class JiedaoDis : TriggerSkill
    {
        public JiedaoDis() : base("#jiedao")
        {
            events.Add(TriggerEvent.DamageComplete);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && player.Alive && !damage.Prevented  && damage.From != null && damage.From.Alive && damage.Marks != null && !damage.From.IsNude())
            {
                foreach (string str in damage.Marks)
                {
                    if (str.StartsWith("jiedao"))
                        return new TriggerStruct(Name, damage.From);
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                int count = 0;
                foreach (string str in damage.Marks)
                {
                    if (str.StartsWith("jiedao"))
                    {
                        string[] strs = str.Split(':');
                        count = int.Parse(strs[1]);
                        break;
                    }
                }

                room.AskForDiscard(ask_who, "jiedao", count, count, false, true, "@jiedao-discard:::" + count.ToString(), false, info.SkillPosition);
            }

            return false;
        }
    }

    public class Kannan : TriggerSkill
    {
        public Kannan() : base("kannan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            view_as_skill = new KannanVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-kannan");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class KannanVS : ZeroCardViewAsSkill
    {
        public KannanVS() : base("kannan")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasFlag(Name) && player.UsedTimes(KannanCard.ClassName) < player.Hp && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(KannanCard.ClassName) { Skill = Name };
        }
    }

    public class KannanCard : SkillCard
    {
        public static string ClassName = "KannanCard";
        public KannanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && Self != to_select && !to_select.HasFlag("kannan") && RoomLogic.CanBePindianBy(room, to_select, Self);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            target.SetFlags("kannan");
            PindianStruct pd = room.PindianSelect(player, target, "kannan");
            room.Pindian(ref pd);
            if (pd.From_number > pd.To_numbers[0])
            {
                player.AddMark("kannan");
                room.SetPlayerStringMark(player, "kannan", player.GetMark("kannan").ToString());
                player.SetFlags("kannan");
            }
            else if (pd.To_numbers[0] > pd.From_number)
            {
                target.AddMark("kannan");
                room.SetPlayerStringMark(target, "kannan", target.GetMark("kannan").ToString());
            }
        }
    }

    public class KannanDamage : TriggerSkill
    {
        public KannanDamage() : base("#kannan")
        {
            events.Add(TriggerEvent.CardUsedAnnounced);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && player.Alive && player.GetMark("kannan") > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#kannan-add",
                    From = player.Name,
                    Arg = player.GetMark("kannan").ToString(),
                    Arg2 = use.Card.Name
                };
                room.SendLog(log);

                use.ExDamage += player.GetMark("kannan");
                data = use;
                player.SetMark("kannan", 0);
                room.RemovePlayerStringMark(player, "kannan");
            }

            return false;
        }
    }

    public class Jixu : TriggerSkill
    {
        public Jixu() : base("jixu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardTargetAnnounced, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
            view_as_skill = new JixuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.HasFlag(Name)) p.SetFlags("-jixu");
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName)
                && player.Phase == PlayerPhase.Play && player.HasUsed(JixuCard.ClassName))
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.HasFlag(Name) && !use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, use.Card, use.To) == null)
                        return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.HasFlag(Name) && !use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, use.Card, use.To) == null)
                    {
                        targets.Add(p);
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                    }
                }

                if (targets.Count > 0)
                {
                    use.To.AddRange(targets);
                    room.SortByActionOrder(ref use);
                    data = use;
                }
            }

            return false;
        }
    }

    public class JixuVS : ZeroCardViewAsSkill
    {
        public JixuVS() : base("jixu")
        { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(JixuCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JixuCard.ClassName) { Skill = Name };
        }
    }

    public class JixuCard : SkillCard
    {
        public static string ClassName = "JixuCard";
        public JixuCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (to_select == Self) return false;
            if (targets.Count == 0)
                return true;
            else
                return to_select.Hp == targets[0].Hp;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            bool has = false;
            foreach (int id in player.GetCards("h"))
            {
                if (room.GetCard(id).Name.Contains(Slash.ClassName))
                {
                    has = true;
                    break;
                }
            }

            List<Player> yes = new List<Player>(), no = new List<Player>();
            foreach (Player p in card_use.To)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#jixu",
                    From = p.Name,
                };
                if (room.AskForChoice(p, "jixu", "has+nohas", new List<string> { "@jixu:" + player.Name }) == "has")
                {
                    yes.Add(p);
                    log.Arg = "has";
                }
                else
                {
                    log.Arg = "nohas";
                    no.Add(p);
                }
                room.SendLog(log);
            }

            LogMessage result = new LogMessage
            {
                Type = has ? "#jixu-has" : "#jixu-no",
                From = player.Name
            };
            room.SendLog(result);

            int count = 0;
            if (has)
            {
                count = no.Count;
                foreach (Player p in no)
                    p.SetFlags("jixu");
            }
            else
            {
                count = yes.Count;
                foreach (Player p in yes)
                {
                    if (player.Alive && p.Alive && !p.IsNude() && RoomLogic.CanDiscard(room, player, p, "he"))
                    {
                        int id = room.AskForCardChosen(player, p, "he", "jixu", false, HandlingMethod.MethodDiscard);
                        room.ThrowCard(id, p, player);
                    }
                }
            }

            if (count > 0)
                room.DrawCards(player, count, "jixu");
            else
                player.SetFlags("Global_PlayPhaseTerminated");
        }
    }

    public class Jijun : TriggerSkill
    {
        public Jijun() : base("jijun")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.CardsMoveOneTime };
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && base.Triggerable(player, room) && use.To.Contains(player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Weapon || !(fcard is EquipCard))
                    return new TriggerStruct(Name, player);
            }
            else if (data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile && move.Reason.Reason == MoveReason.S_REASON_JUDGEDONE && !string.IsNullOrEmpty(move.Reason.EventName)
                && move.Reason.EventName == Name && move.From.Alive && move.Card_ids.Count == 1 && room.GetCardPlace(move.Card_ids[0]) == Place.DiscardPile)
                return new TriggerStruct(Name, move.From);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && room.AskForSkillInvoke(ask_who, Name, "@jijun", info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen)
            {
                JudgeStruct judge = new JudgeStruct
                {
                    Good = true,
                    PlayAnimation = false,
                    Who = player,
                    Reason = Name
                };

                room.Judge(ref judge);
            }
            else if (data is CardsMoveOneTimeStruct move)
            {
                room.AddToPile(ask_who, Name, move.Card_ids);
            }

            return false;
        }
    }

    public class Fangtong : TriggerSkill
    {
        public Fangtong() : base("fangtong")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
                && move.Reason.Reason == MoveReason.S_REASON_THROW && move.Reason.SkillName == Name && move.Card_ids.Count == 1 && move.To_place == Place.PlaceTable && move.From.Alive)
            {
                int count = 0;
                foreach (int id in move.Card_ids)
                    count += room.GetCard(id).Number;

                move.From.SetMark("fangtong_discard", count);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.GetPile("jijun").Count > 0 && player.Phase == PlayerPhase.Finish)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForDiscard(player, Name, 1, 1, true, true, "@fangtong", true, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.AskForExchange(player, Name, player.GetPile("jijun").Count, 1, "@fangtong-disacard", "jijun", string.Empty, info.SkillPosition);
            int count = player.GetMark(Name);
            foreach (int id in ids)
                count += room.GetCard(id).Number;

            CardsMoveStruct move = new CardsMoveStruct(ids, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, ask_who.Name, Name, string.Empty));
            room.MoveCardsAtomic(move, true);

            if (player.Alive)
            {
                player.SetMark(Name, count);
                room.SetPlayerStringMark(player, Name, count.ToString());
                int discard = player.GetMark("fangtong_discard");
                player.SetMark("fangtong_discard", 0);
                if (count + discard == 36)
                {
                    Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@fangtong-target", false, true, info.SkillPosition);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                    room.Damage(new DamageStruct(Name, player, target, 3, DamageStruct.DamageNature.Thunder));
                }
            }

            if (player.Alive && count >= 36)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }

            return false;
        }
    }

    public class Lixun : TriggerSkill
    {
        public Lixun() : base("lixun")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted, TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.Reason.Reason == MoveReason.S_REASON_THROW
                && move.Reason.SkillName == Name && move.To_place == Place.PlaceTable)
                move.From.SetTag(Name, move.Card_ids.Count);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageInflicted && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.GetMark("@zhu") > 1 && player.Phase == PlayerPhase.Play)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct damage)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.AddPlayerMark(player, "@zhu", damage.Damage);
                LogMessage log = new LogMessage
                {
                    Type = "#damaged-prevent",
                    From = player.Name,
                    Arg = Name
                };
                room.SendLog(log);
                return true;
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                JudgeStruct judge = new JudgeStruct
                {
                    Good = true,
                    PlayAnimation = false,
                    Who = player,
                    Reason = Name,
                    Negative = true,
                    Pattern = string.Format(".|.|1~{0}", player.GetMark("@zhu") - 1)
                };

                room.Judge(ref judge);
                if (judge.IsEffected())
                {
                    int count = 0, number = player.GetMark("@zhu");
                    if (player.Alive && room.AskForDiscard(player, Name, number, number, false, false,
                        string.Format("@lixun:::{0}", number), false, info.SkillPosition) && player.ContainsTag(Name) && player.GetTag(Name) is int discard)
                        count = discard;

                    if (player.Alive && count < number) room.LoseHp(player, number - count);
                }
            }

            return false;
        }
    }

    public class KuizhuLS : TriggerSkill
    {
        public KuizhuLS() : base("kuizhu_ls")
        {
            events.Add(TriggerEvent.EventPhaseEnd);
            skill_type = SkillType.Wizzard;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Play && target.HandcardNum < 5;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            int max = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.Hp > max) max = p.Hp;
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Hp == max && p.HandcardNum > player.HandcardNum) targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@kuizhu_ls-target", true, true, info.SkillPosition);
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
            int count = Math.Min(5 - player.HandcardNum, target.HandcardNum - player.HandcardNum);
            room.DrawCards(player, count, Name);
            if (player.Alive && target.Alive && player.HandcardNum > 0)
            {
                object decisionData = string.Format("viewCards:{0}:all", target.Name);
                room.RoomThread.Trigger(TriggerEvent.ChoiceMade, room, player, ref decisionData);


                List<int> disable = new List<int>(), ids = player.GetCards("h");
                foreach (int id in ids)
                    if (!RoomLogic.CanGetCard(room, target, player, id)) disable.Add(id);
                room.FillAG(Name, ids, target, disable);
                List<int> discard = room.AskForExchange(target, Name, ids.Count - disable.Count, 0, "@kuizhu_from:" + player.Name, string.Empty, ".!", string.Empty);
                room.ClearAG(target);

                if (discard.Count > 0)
                {
                    count = discard.Count;
                    room.ThrowCard(ref discard, target);

                    if (player.Alive && target.Alive && player.HandcardNum > 0)
                    {
                        ids = player.GetCards("h");
                        List<int> can = new List<int>(ids);
                        can.RemoveAll(t => !RoomLogic.CanGetCard(room, target, player, t));
                        room.SetTag(Name, can);
                        AskForMoveCardsStruct move = room.AskForMoveCards(target, ids, new List<int>(), false, Name, Math.Min(can.Count, count),
                            Math.Min(can.Count, count), false, true, new List<int>(), string.Empty);
                        room.RemoveTag(Name);
                        List<int> get = new List<int>();
                        if (move.Success && move.Bottom.Count == count)
                        {
                            get = move.Bottom;
                        }
                        else
                        {
                            for (int i = 0; i < Math.Min(can.Count, count); i++)
                                get.Add(can[i]);
                        }

                        if (get.Count > 0)
                        {
                            room.ObtainCard(target, ref get, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, target.Name, player.Name, Name, string.Empty), false);
                            if (player.Alive && target.Alive && get.Count > 1)
                            {
                                bool option = player.GetMark("@zhu") > 0;
                                List<Player> targets = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(target))
                                    if (RoomLogic.InMyAttackRange(room, target, p)) targets.Add(p);

                                bool remove = option;
                                if (targets.Count > 0)
                                {
                                    player.SetFlags(Name);
                                    target.SetFlags("kuizhu_target");
                                    Player victim = room.AskForPlayerChosen(player, targets, Name, "@kuizhu-damage:" + target.Name, option, true, info.SkillPosition);
                                    player.SetFlags("-kuizhu_ls");
                                    target.SetFlags("-kuizhu_target");
                                    if (victim != null)
                                    {
                                        remove = false;
                                        room.Damage(new DamageStruct(Name, target, victim));
                                    }
                                }

                                if (remove) room.AddPlayerMark(player, "@zhu", -1);
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
    
    public class Fenyue : ZeroCardViewAsSkill
    {
        public Fenyue() : base("fenyue") { skill_type = SkillType.Alter; }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.IsKongcheng())
            {
                int count = 0;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    PlayerRole role = p.GetRoleEnum();
                    if (role == PlayerRole.Lord || role == PlayerRole.Loyalist)
                    {
                        if (player.GetRoleEnum() != PlayerRole.Lord && player.GetRoleEnum() != PlayerRole.Loyalist)
                            count++;
                    }
                    else if (player.GetRoleEnum() != p.GetRoleEnum())
                        count++;
                }
                return player.UsedTimes(FenyueCard.ClassName) < count;
            }

            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(FenyueCard.ClassName) { Skill = Name };
        }
    }

    public class FenyueCard : SkillCard
    {
        public static string ClassName = "FenyueCard";
        public FenyueCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && RoomLogic.CanBePindianBy(room, to_select, Self);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            PindianStruct pd = room.PindianSelect(card_use.From, target, "fenyue");

            room.Pindian(ref pd);
            if (pd.Success)
            {
                int number = Engine.GetRealCard(pd.From_card.Id).Number;
                if (number <= 5 && player.Alive && target.Alive && !target.IsNude() && RoomLogic.CanGetCard(room, player, target, "he"))
                {
                    int card_id = room.AskForCardChosen(player, target, "he", "fenyue", false, HandlingMethod.MethodGet);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, "fenyue", string.Empty);
                    room.ObtainCard(player, room.GetCard(card_id), reason, false);
                }

                if (number <= 9 && player.Alive)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in room.DrawPile)
                    {
                        if (room.GetCard(id).Name.Contains(Slash.ClassName))
                        {
                            ids.Add(id);
                            break;
                        }
                    }

                    if (ids.Count > 0)
                    {
                        CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, player.Name, "fenyue", string.Empty);
                        room.ObtainCard(player, ref ids, reason, true);
                    }
                }

                if (player.Alive && target.Alive)
                {
                    WrappedCard slash = new WrappedCard(ThunderSlash.ClassName);
                    slash.DistanceLimited = false;
                    if (RoomLogic.IsProhibited(room, player, target, slash) == null)
                        room.UseCard(new CardUseStruct(slash, player, target, false));
                }
            }
        }
    }

    public class Xuhe : TriggerSkill
    {
        public Xuhe() : base("xuhe")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseEnd };
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                int count = 99;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.MaxHp < count) count = p.MaxHp;
                }
                if (player.MaxHp == count)
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
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && base.Triggerable(player, room) && player.MaxHp > 1)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            string choice = room.AskForChoice(player, Name, "discard+draw+cancel");
            if (choice != "cancel")
            {
                player.SetTag(Name, choice);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.NotifySkillInvoked(player, Name);
                room.LoseMaxHp(player);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.GetTag(Name) is string choice && player.Alive)
            {
                player.RemoveTag(Name);
                List<Player> targets = new List<Player>();
                if (choice == "discard")
                {
                    if (RoomLogic.CanDiscard(room, player, player, "he")) targets.Add(player);
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (RoomLogic.DistanceTo(room, player, p) == 1)
                            targets.Add(p);

                    if (targets.Count > 0)
                    {
                        room.SortByActionOrder(ref targets);
                        foreach (Player p in targets)
                        {
                            if (p.Alive && RoomLogic.CanDiscard(room, player, p, "he") && player.Alive)
                            {
                                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISMANTLE, player.Name, p.Name, Name, null)
                                {
                                    General = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition)
                                };
                                List<int> ints = new List<int>();
                                if (p == player)
                                {
                                    ints.AddRange(room.AskForExchange(player, Name, 1, 1, "@xuhe", null, "..!", info.SkillPosition));
                                }
                                else
                                    ints.Add(room.AskForCardChosen(player, p, "he", Name, false, HandlingMethod.MethodDiscard));

                                room.ThrowCard(ref ints, reason, p, player);
                            }
                        }
                    }
                }
                else
                {
                    targets.Add(player);
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (RoomLogic.DistanceTo(room, player, p) == 1)
                            targets.Add(p);

                    room.SortByActionOrder(ref targets);
                    foreach (Player p in targets)
                        room.DrawCards(p, new DrawCardStruct(1, player, Name));
                }

            }

            return false;
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
                    new CardMoveReason(MoveReason.S_REASON_SWAP, player.Name, target.Name, "guolun", string.Empty));
                CardsMoveStruct move2 = new CardsMoveStruct(new List<int> { id }, player, Place.PlaceHand,
                    new CardMoveReason(MoveReason.S_REASON_SWAP, target.Name, player.Name, "guolun", string.Empty));
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
                && move.Reason.Reason == MoveReason.S_REASON_DRAW && move.Reason.SkillName != Name)
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

    public class Guanwei : TriggerSkill
    {
        public Guanwei() : base("guanwei")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && player.Alive && player == room.Current && !player.HasFlag(Name))
            {
                if (Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill)
                {
                    List<WrappedCard.CardSuit> suits = player.ContainsTag(Name) ? (List<WrappedCard.CardSuit>)player.GetTag(Name) : new List<WrappedCard.CardSuit>();
                    if (use.Card.Suit > WrappedCard.CardSuit.Diamond || (suits.Count > 0 && !suits.Contains(use.Card.Suit)))
                    {
                        player.SetFlags(Name);
                    }
                    else
                    {
                        suits.Add(use.Card.Suit);
                        player.SetTag(Name, suits);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && player.Alive && player == room.Current && !player.HasFlag(Name))
            {
                List<WrappedCard.CardSuit> suits = player.ContainsTag(Name) ? (List<WrappedCard.CardSuit>)player.GetTag(Name) : new List<WrappedCard.CardSuit>();
                if (resp.Card.Suit > WrappedCard.CardSuit.Diamond || (suits.Count > 0 && !suits.Contains(resp.Card.Suit)))
                {
                    player.SetFlags(Name);
                }
                else
                {
                    suits.Add(resp.Card.Suit);
                    player.SetTag(Name, suits);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play && player.Alive && !player.HasFlag(Name) && player.ContainsTag(Name)
                && player.GetTag(Name) is List<WrappedCard.CardSuit> suits && suits.Count > 1)
            {
                bool invoke = true;
                for (int i = 1; i < suits.Count; i++)
                {
                    if (suits[i] != suits[0])
                    {
                        invoke = false;
                        break;
                    }
                }

                if (invoke)
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive && !player.HasFlag(Name) && room.AskForDiscard(ask_who, Name, 1, 0, true, true, "@guanwei:" + player.Name, true, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            if (player.Alive) room.DrawCards(player, new DrawCardStruct(2, ask_who, Name));
            if (player.Alive) player.AddPhase(PlayerPhase.Play);
            return false;
        }
    }

    public class Gongqing : TriggerSkill
    {
        public Gongqing() : base("gongqing")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted, TriggerEvent.DamageDefined };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                int range = RoomLogic.GetAttackRange(room, damage.From);
                if ((triggerEvent == TriggerEvent.DamageDefined && range < 3 && damage.Damage > 1) || (triggerEvent == TriggerEvent.DamageInflicted && range > 3))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            if (triggerEvent == TriggerEvent.DamageDefined)
            {
                damage.Damage = 1;
                LogMessage log = new LogMessage
                {
                    Type = "#ReduceDamage",
                    From = player.Name,
                    Arg = Name,
                    Arg2 = (damage.Damage).ToString()
                };
                room.SendLog(log);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#AddDamaged",
                    From = player.Name,
                    Arg = Name,
                    Arg2 = (++damage.Damage).ToString()
                };
                room.SendLog(log);
            }

            data = damage;

            return false;
        }
    }
    public class Qinguo : TriggerSkill
    {
        public Qinguo() : base("qinguo")
        {
            events.Add(TriggerEvent.CardFinished);
            skill_type = SkillType.Attack;
            view_as_skill = new QinguoVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room) && room.Current == player && Engine.GetFunctionCard(use.Card.Name) is EquipCard)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@qinguo", "@qinguo-slash", null, -1, HandlingMethod.MethodUse, false, info.SkillPosition);
            return new TriggerStruct();
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 1;
        }
    }


    public class QinguoVS : ViewAsSkill
    {
        public QinguoVS() : base("qinguo")
        {
            response_pattern = "@@qinguo";
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
                return new List<WrappedCard> { new WrappedCard(Slash.ClassName) { Skill = Name, ShowSkill = Name } };

            return new List<WrappedCard>();
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
                return cards[0];

            return null;
        }
    }

    public class QinguoRecover : TriggerSkill
    {
        public QinguoRecover() : base("#qinguo")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Recover;
            frequency = Frequency.Compulsory;
        }
        

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceEquip) && move.From.Hp == move.From.GetEquips().Count
                    && move.Card_ids.Count == move.From_places.Count && move.From.IsWounded())
                {
                    int count = 0;
                    foreach (Place place in move.From_places)
                        if (place == Place.PlaceEquip) count++;
                    if (count > 0) return new TriggerStruct(Name, move.From);

                }
                else if (move.To != null && base.Triggerable(move.To, room) && move.To_place == Place.PlaceEquip && move.To.Hp == move.To.GetEquips().Count
                    && move.Card_ids.Count > 0 && move.To.IsWounded())
                {
                    return new TriggerStruct(Name, move.To);
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.Alive && ask_who.IsWounded())
            {
                room.SendCompulsoryTriggerLog(ask_who, Name, true);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke("qinguo", "male", 2, gsk.General, gsk.SkinId);

                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = ask_who
                };
                room.Recover(ask_who, recover, true);
            }

            return false;
        }
    }

    public class Youdi : PhaseChangeSkill
    {
        public Youdi() : base("youdi")
        {
            skill_type = SkillType.Attack;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish && !target.IsKongcheng();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (RoomLogic.CanDiscard(room, p, player, "h")) targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@youdi", true, true, info.SkillPosition);
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);

            int id = room.AskForCardChosen(target, player, "h", Name, false, HandlingMethod.MethodDiscard);
            List<int> ids = new List<int> { id };
            room.ThrowCard(ref ids, player, target);
            if (ids.Count == 1)
            {
                WrappedCard card = room.GetCard(ids[0]);
                bool red = WrappedCard.IsRed(card.Suit);
                if (player.Alive && !card.Name.Contains(Slash.ClassName) && target.Alive && RoomLogic.CanGetCard(room, player, target, "he"))
                {
                    int get = room.AskForCardChosen(player, target, "he", Name, false, HandlingMethod.MethodGet);
                    List<int> gets = new List<int> { get };
                    room.ObtainCard(player, ref gets, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, string.Empty), false);
                }

                if (red && player.Alive)
                    room.DrawCards(player, 1, Name);
            }

            return false;
        }
    }

    public class DuanfaCard : SkillCard
    {
        public static string ClassName = "DuanfaCard";
        public DuanfaCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            if (card_use.From.Alive)
            {
                int count = card_use.Card.SubCards.Count;
                card_use.From.AddMark("duanfa", count);
                room.DrawCards(card_use.From, count, "duanfa");
            }
        }
    }

    public class Duanfa : TriggerSkill
    {
        public Duanfa() : base("duanfa")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new DuanfaVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class DuanfaVS : ViewAsSkill
    {
        public DuanfaVS() : base("duanfa")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude() && player.GetMark(Name) < player.MaxHp;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !RoomLogic.IsJilei(room, player, to_select) && WrappedCard.IsBlack(to_select.Suit) && selected.Count < player.MaxHp - player.GetMark(Name);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
                return null;

            WrappedCard zhiheng_card = new WrappedCard(DuanfaCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            zhiheng_card.AddSubCards(cards);
            return zhiheng_card;
        }
    }

    public class Biaozhao : TriggerSkill
    {
        public Biaozhao() : base("biaozhao") { events.Add(TriggerEvent.EventPhaseStart); }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish && !target.IsNude();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.AskForExchange(player, Name, 1, 0, "@biaozhao", string.Empty, "..", info.SkillPosition);
            if (ids.Count > 0)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.NotifySkillInvoked(player, Name);
                room.AddToPile(player, Name, ids);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return false;
        }
    }

    public class BiaozhaoEffect : TriggerSkill
    {
        public BiaozhaoEffect() : base("#biaozhao")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseStart, TriggerEvent.EventLoseSkill };
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, "biaozhao"))
                {
                    List<int> ids = p.GetPile("biaozhao");
                    if (ids.Count > 0)
                    {
                        bool invoke = false;
                        WrappedCard card1 = room.GetCard(ids[0]);
                        foreach (int id2 in move.Card_ids)
                        {
                            WrappedCard card2 = room.GetCard(id2);
                            if (card1.Number == card2.Number && card1.Suit == card2.Suit)
                            {
                                invoke = true;
                                break;
                            }
                        }
                        if (invoke) triggers.Add(new TriggerStruct(Name, p));
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Start && player.GetPile("biaozhao").Count > 0)
            {
                triggers.Add(new TriggerStruct(Name, player));
            }
            else if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct info && info.Info == "biaozhao" && player.GetPile("biaozhao").Count > 0)
            {
                triggers.Add(new TriggerStruct(Name, player));
            }
            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD && move.From.Alive)
                {
                    List<int> ids = ask_who.GetPile("biaozhao");
                    room.ObtainCard(move.From, ref ids, new CardMoveReason(MoveReason.S_REASON_GOTCARD, move.From.Name, "biaozhao", string.Empty));
                }
                else
                    room.ClearOnePrivatePile(ask_who, "biaozhao");

                if (ask_who.Alive)
                    room.LoseHp(ask_who);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                room.ClearOnePrivatePile(player, "biaozhao");
                Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), "biaozhao", "@biaozhao-draw", false, true, info.SkillPosition);
                if (target.IsWounded())
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = player
                    };
                    room.Recover(target, recover, true);
                }
                if (target.Alive)
                {
                    int count = 0;
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.HandcardNum > count) count = p.HandcardNum;

                    int draw = Math.Min(count - target.HandcardNum, 5);
                    if (draw > 0)
                        room.DrawCards(target, new DrawCardStruct(draw, player, "biaozhao"));
                }
            }
            else if (triggerEvent == TriggerEvent.EventLoseSkill)
            {
                room.ClearOnePrivatePile(player, "biaozhao");
            }
            return false;
        }
    }

    public class Yechou : TriggerSkill
    {
        public Yechou() : base("yechou")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.TurnStart };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.TurnStart && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Death && RoomLogic.PlayerHasSkill(room, player, Name))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.GetLostHp() > 1) targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@yechou", true, true, info.SkillPosition);
                if (target != null)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.SetTag(Name, target);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            target.AddMark(Name);
            room.SetPlayerStringMark(target, Name, string.Empty);
            return false;
        }
    }

    public class YechouLose : TriggerSkill
    {
        public YechouLose() : base("#yechou") { events.Add(TriggerEvent.EventPhaseChanging); frequency = Frequency.Compulsory; }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark("yechou") > 0)
                        triggers.Add(new TriggerStruct(Name, p));

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.LoseHp(ask_who);
            return false;
        }
    }

    public class Guanchao : TriggerSkill
    {
        public Guanchao() : base("guanchao")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
            priority = new Dictionary<TriggerEvent, double>
            {
                { TriggerEvent.CardUsed, 2 },
                { TriggerEvent.CardResponded, 2 },
                { TriggerEvent.EventPhaseStart, 3 },
                { TriggerEvent.EventPhaseChanging, 3 },
            };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == Player.PlayerPhase.Play
                && (player.HasFlag("guanchao++") || player.HasFlag("guanchao--")))
            {
                player.SetFlags("-guanchao++");
                player.SetFlags("-guanchao--");
                player.SetMark("guanchao", 0);
                room.RemovePlayerStringMark(player, "guanchao");
            }
            else if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded) && player.Phase == PlayerPhase.Play
                && (player.HasFlag("guanchao++") || player.HasFlag("guanchao--")))
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
                        bool remove = false;
                        if (card.Number == 0)
                            remove = true;
                        else
                        {
                            if ((player.HasFlag("guanchao++") && card.Number <= player.GetMark(Name))
                                || (player.HasFlag("guanchao--") && card.Number >= player.GetMark(Name) && player.GetMark(Name) > 0))
                                remove = true;
                            else
                            {
                                player.SetMark("guanchao", card.Number);
                                room.SetPlayerStringMark(player, "guanchao", card.Number.ToString());
                            }
                        }

                        if (remove)
                        {
                            player.SetFlags("-guanchao++");
                            player.SetFlags("-guanchao--");
                            player.SetMark("guanchao", 0);
                            room.RemovePlayerStringMark(player, "guanchao");
                        }
                    }
                }
            }
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
            string choice = room.AskForChoice(player, Name, "more+less+cancel");
            if (choice != "cancel")
            {
                room.NotifySkillInvoked(player, Name);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "guanchao", info.SkillPosition);
                room.BroadcastSkillInvoke("guanchao", "male", 1, gsk.General, gsk.SkinId);

                LogMessage log = new LogMessage
                {
                    From = player.Name
                };
                if (choice == "more")
                {
                    player.SetFlags("guanchao++");
                    log.Type = "#guanchao-more";
                }
                else
                {
                    player.SetFlags("guanchao--");
                    log.Type = "#guanchao-less";
                }
                room.SendLog(log);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return false;
        }
    }
    public class GuanchaoRecord : TriggerSkill
    {
        public GuanchaoRecord() : base("#guanchao")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded) && player.Phase == PlayerPhase.Play && player.Alive)
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use)
                    card = resp.Card;

                if (card != null && card.Number > 0)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!(fcard is SkillCard) && !(fcard is ImperialOrder))
                    {
                        int number = player.GetMark("guanchao");
                        if (number > 0 && ((card.Number > number && player.HasFlag("guanchao++")) || (card.Number < number && player.HasFlag("guanchao--"))))
                            return new TriggerStruct(Name, player);
                    }
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, "guanchao");
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "guanchao", info.SkillPosition);
            room.BroadcastSkillInvoke("guanchao", "male", 2, gsk.General, gsk.SkinId);

            if (player.Alive) room.DrawCards(player, 1, "guanchao");
            return false;
        }
    }

    public class Xunxian : TriggerSkill
    {
        public Xunxian() : base("xunxian")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move  && move.Reason.Card != null && ((move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_USE
                || move.Reason.Reason == MoveReason.S_REASON_RESPONSE) && move.Card_ids.Count > 0 && move.To_place == Place.DiscardPile)
            {
                Player from = room.FindPlayer(move.Reason.PlayerId);
                if (base.Triggerable(from, room) && !from.HasFlag(Name) && from.Phase == PlayerPhase.NotActive)
                {
                    List<int> ids = room.GetSubCards(move.Reason.Card);
                    if (ids.SequenceEqual(move.Reason.Card.SubCards) && ids.SequenceEqual(move.Card_ids))
                    {
                        bool check = true;
                        foreach (int id in ids)
                        {
                            if (room.GetCardPlace(id) != Place.DiscardPile)
                            {
                                check = false;
                                break;
                            }
                        }
                        if (check) return new TriggerStruct(Name, from);
                    }
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(ask_who))
                if (p.HandcardNum > ask_who.HandcardNum)
                    targets.Add(p);

            if (targets.Count > 0 && data is CardsMoveOneTimeStruct move)
            {
                Player target = room.AskForPlayerChosen(ask_who, targets, Name, "@xunxian:::" + move.Reason.Card.Name, true, true, info.SkillPosition);
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.SetFlags(Name);
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            if (data is CardsMoveOneTimeStruct move)
            {
                List<int> ids = room.GetSubCards(move.Reason.Card);
                if (ids.Count > 0)
                {
                    room.RemoveSubCards(move.Reason.Card);
                    room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, ask_who.Name, target.Name, Name, string.Empty));
                }
            }
            return false;
        }
    }

    public class Fuhai : TriggerSkill
    {
        public Fuhai() : base("fuhai")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new FuhaiVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play)
            {
                if (player.GetMark(Name) > 0) player.SetMark(Name, 0);
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasFlag(Name))
                        p.SetFlags("-fuhai");
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class FuhaiVS : ZeroCardViewAsSkill
    {
        public FuhaiVS() : base("fuhai")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (player.HasFlag(Name) || player.IsKongcheng()) return false;
            foreach (Player p in room.GetOtherPlayers(player))
                if (!p.IsKongcheng() && !p.HasFlag(Name))
                    return true;

            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(FuhaiCard.ClassName) { Skill = Name };
        }
    }

    public class FuhaiCard : SkillCard
    {
        public static string ClassName = "FuhaiCard";
        public FuhaiCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = null;
            string choic = room.AskForChoice(player, "fuhai", "counterclockwise+clockwise");
            if (choic == "counterclockwise")
            {
                target = room.GetNextAlive(player, 1, false);
                bool succes = true;
                do
                {
                    while (target != player && (target.IsKongcheng() || target.HasFlag("fuhai")))
                        target = room.GetNextAlive(target, 1, false);

                    if (target != player)
                    {
                        player.AddMark("fuhai");
                        succes = DoShow(room, player, target, card_use.Card.SkillPosition);
                        if (!succes)
                        {
                            player.SetFlags("fuhai");
                            int count = player.GetMark("fuhai");
                            room.DrawCards(player, count, "fuhai");
                            if (target.Alive)
                                room.DrawCards(target, count, "fuhai");
                        }
                    }
                    else
                        succes = false;
                }
                while (succes && player.Alive && !player.IsKongcheng());
            }
            else
            {
                target = room.GetLastAlive(player, 1, false);
                bool succes = true;
                do
                {
                    while (target != player && (target.IsKongcheng() || target.HasFlag("fuhai")))
                        target = room.GetLastAlive(target, 1, false);

                    if (target != player)
                    {
                        player.AddMark("fuhai");
                        succes = DoShow(room, player, target, card_use.Card.SkillPosition);
                        Thread.Sleep(400);
                        if (!succes)
                        {
                            player.SetFlags("fuhai");
                            int count = player.GetMark("fuhai");
                            room.DrawCards(player, count, "fuhai");
                            if (target.Alive)
                                room.DrawCards(target, count, "fuhai");

                            Thread.Sleep(400);
                        }
                    }
                    else
                        succes = false;
                }
                while (succes && player.Alive && !player.IsKongcheng());
            }
        }

        private bool DoShow(Room room, Player player, Player target, string position)
        {
            target.SetFlags("fuhai");
            int from = room.AskForExchange(player, "fuhai", 1, 1, "@fuhai-show:" + target.Name, string.Empty, ".", position)[0];
            room.ShowCard(player, from, "fuhai");
            room.SetTag("fuhai", from);
            int to = room.AskForCardShow(target, player, "fuhai", from);
            room.RemoveTag("fuhai");
            room.ShowCard(target, to, "fuhai");
            if (room.GetCard(from).Number >= room.GetCard(to).Number)
            {
                if (RoomLogic.CanDiscard(room, player, player, from)) room.ThrowCard(from, player);

                return true;
            }
            else
            {
                if (RoomLogic.CanDiscard(room, target, target, to)) room.ThrowCard(to, target);

                return false;
            }
        }
    }

    public class Lianhua : TriggerSkill
    {
        public Lianhua() : base("lianhua")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                if (player.HasFlag("lianhua_yingzi"))
                    room.HandleAcquireDetachSkills(player, "-yingzi_zhouyu", true);
                if (player.HasFlag("lianhua_guanxing"))
                    room.HandleAcquireDetachSkills(player, "-guanxing_jx", true);
                if (player.HasFlag("lianhua_gongxin"))
                    room.HandleAcquireDetachSkills(player, "-gongxin", true);
                if (player.HasFlag("lianhua_zhiyan"))
                    room.HandleAcquireDetachSkills(player, "-zhiyan", true);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && (player.GetMark("lianhua_red") > 0 || player.GetMark("lianhua_black") > 0))
            {
                player.SetMark("lianhua_red", 0);
                player.SetMark("lianhua_black", 0);
                room.SetPlayerMark(player, "@danxue", 0);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.Damaged && player.Alive)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p != player && p.Phase == PlayerPhase.NotActive)
                        triggers.Add(new TriggerStruct(Name, p));
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Start)
            {
                triggers.Add(new TriggerStruct(Name, player));
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.Damaged)
            {
                if ((player.GetRoleEnum() == PlayerRole.Lord || player.GetRoleEnum() == PlayerRole.Loyalist)
                    && (ask_who.GetRoleEnum() == PlayerRole.Lord || ask_who.GetRoleEnum() == PlayerRole.Loyalist))
                {
                    ask_who.AddMark("lianhua_red");
                }
                else if (player.GetRoleEnum() == PlayerRole.Rebel && ask_who.GetRoleEnum() == PlayerRole.Rebel)
                {
                    ask_who.AddMark("lianhua_red");
                }
                else
                    ask_who.AddMark("lianhua_black");

                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.SetPlayerMark(ask_who, "@danxue", ask_who.GetMark("lianhua_red") + ask_who.GetMark("lianhua_black"));
            }
            else
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                List<int> ids = new List<int>(room.DrawPile);
                ids.AddRange(room.DiscardPile);
                Shuffle.shuffle(ref ids);
                if (ask_who.GetMark("lianhua_red") + ask_who.GetMark("lianhua_black") <= 3)
                {
                    player.SetFlags("lianhua_yingzi");
                    room.HandleAcquireDetachSkills(player, "yingzi_zhouyu", true);
                    foreach (int id in ids)
                    {
                        if (room.GetCard(id).Name == Peach.ClassName)
                        {
                            room.ObtainCard(player, id, true);
                            break;
                        }
                    }
                }
                else
                {
                    if (ask_who.GetMark("lianhua_red") > ask_who.GetMark("lianhua_black"))
                    {
                        player.SetFlags("lianhua_guanxing");
                        room.HandleAcquireDetachSkills(player, "guanxing_jx", true);
                        foreach (int id in ids)
                        {
                            if (room.GetCard(id).Name == ExNihilo.ClassName)
                            {
                                room.ObtainCard(player, id, true);
                                break;
                            }
                        }
                    }
                    else if (ask_who.GetMark("lianhua_red") < ask_who.GetMark("lianhua_black"))
                    {
                        player.SetFlags("lianhua_zhiyan");
                        room.HandleAcquireDetachSkills(player, "zhiyan", true);
                        foreach (int id in ids)
                        {
                            if (room.GetCard(id).Name == Snatch.ClassName)
                            {
                                room.ObtainCard(player, id, true);
                                break;
                            }
                        }
                    }
                    else
                    {
                        player.SetFlags("lianhua_gongxin");
                        room.HandleAcquireDetachSkills(player, "gongxin", true);
                        bool slash = false, duel = false;
                        foreach (int id in ids)
                        {
                            if (!slash && room.GetCard(id).Name == Slash.ClassName)
                            {
                                room.ObtainCard(player, id, true);
                                slash = true;
                            }
                            else if (!duel && room.GetCard(id).Name == Duel.ClassName)
                            {
                                room.ObtainCard(player, id, true);
                                duel = true;
                            }

                            if (slash && duel)
                                break;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Zhafu : ZeroCardViewAsSkill
    {
        public Zhafu() : base("zhafu")
        {
            frequency = Frequency.Limited;
            limit_mark = "@zhafu";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(limit_mark) > 0;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(ZhafuCard.ClassName) { Skill = Name, Mute = true };
        }
    }

    public class ZhafuCard : SkillCard
    {
        public static string ClassName = "ZhafuCard";
        public ZhafuCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@zhafu", 0);
            room.BroadcastSkillInvoke("zhafu", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "zhafu");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            card_use.To[0].SetTag("zhafu", card_use.From.Name);
            room.SetPlayerStringMark(card_use.To[0], "zhafu", string.Empty);
        }
    }
    public class ZhafuEffect : TriggerSkill
    {
        public ZhafuEffect() : base("#zhafu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Discard && player.ContainsTag("zhafu"))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.GetTag("zhafu") is string general)
            {
                player.RemoveTag("zhafu");
                room.RemovePlayerStringMark(player, "zhafu");
                Player target = room.FindPlayer(general);
                if (target != null && player.HandcardNum > 1)
                {
                    List<int> hands = player.GetCards("h"), ids = room.AskForExchange(player, "zhafu", 1, 1, "@zhafu-give:" + target.Name, string.Empty, ".", string.Empty);
                    hands.RemoveAll(t => ids.Contains(t));
                    room.ObtainCard(target, ref hands, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "zhafu", string.Empty), false);
                }
            }


            return false;
        }
    }

    public class Songshu : TriggerSkill
    {
        public Songshu() : base("songshu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new SongshuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-songshu");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class SongshuVS : ZeroCardViewAsSkill
    {
        public SongshuVS() : base("songshu") { }

        public override bool IsEnabledAtPlay(Room room, Player player) => !player.HasFlag(Name) && !player.IsKongcheng();
        public override WrappedCard ViewAs(Room room, Player player) => new WrappedCard(SongshuCard.ClassName) { Skill = Name };
    }

    public class SongshuCard : SkillCard
    {
        public static string ClassName = "SongshuCard";
        public SongshuCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && RoomLogic.CanBePindianBy(room, to_select, Self) && to_select != Self;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            PindianStruct pd = room.PindianSelect(card_use.From, target, "songshu");

            room.Pindian(ref pd);
            if (!pd.Success)
            {
                if (target.Alive) room.DrawCards(target, new DrawCardStruct(2, card_use.From, "songshu"));
                if (card_use.From.Alive) card_use.From.SetFlags("songshu");
            }
        }
    }
    public class Sibian : PhaseChangeSkill
    {
        public Sibian() : base("sibian")
        {
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who)
        {
            return (base.Triggerable(lidian, room) && lidian.Phase == PlayerPhase.Draw) ? new TriggerStruct(Name, lidian) : new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(lidian, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, lidian, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            List<int> card_ids = room.GetNCards(4), get = new List<int>(), give= new List<int>();
            int max = 0, min = 13;
            foreach (int id in card_ids)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Number > max) max = card.Number;
                if (card.Number < min) min = card.Number;
                room.MoveCardTo(room.GetCard(id), player, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name, Name, null), false);
                Thread.Sleep(400);
            }
            foreach (int id in card_ids)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Number == max || card.Number == min) get.Add(id);
            }

            card_ids.RemoveAll(t => get.Contains(t));
            if (get.Count == 2 && Math.Abs(room.GetCard(get[0]).Number - room.GetCard(get[1]).Number) < room.AliveCount())
                give = card_ids;

            if (get.Count > 0)
                room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty));
            if (player.Alive && give.Count > 0)
            {
                List<Player> targets = new List<Player>();
                int hand_min = 400;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum < hand_min)
                        hand_min = p.HandcardNum;

                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum == hand_min)
                        targets.Add(p);

                Player target = room.AskForPlayerChosen(player, targets, Name, "@sibian", true, true, info.SkillPosition);
                if (target != null)
                    room.ObtainCard(target, ref give, new CardMoveReason(MoveReason.S_REASON_PREVIEWGIVE, player.Name, target.Name, Name, string.Empty));

                card_ids.RemoveAll(t => room.GetCardPlace(t) != Place.PlaceTable);
                if (card_ids.Count > 0)
                {
                    CardsMoveStruct move = new CardsMoveStruct(card_ids, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, player.Name, Name, string.Empty));
                    List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
                    room.MoveCardsAtomic(moves, true);
                }
            }

            return true;
        }
    }
}