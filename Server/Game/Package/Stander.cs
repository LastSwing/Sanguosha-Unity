using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class Stander : GeneralPackage
    {
        public Stander() : base("Stander")
        {
            skills = new List<Skill>
            {
                new Jianxiong(),
                new Fankui(),
                new Guicai(),
                new Ganglie(),
                new Tuxi(),
                new Luoyi(),
                new LuoyiDamage(),
                new Tiandu(),
                new Yiji(),
                new Luoshen(),
                new LuoshenMove(),
                new Qingguo(),
                new Shensu(),
                new Qiaobian(),
                new Duanliang(),
                new DuanliangTargetMod(),
                new Jushou(),
                new Qiangxi(),
                new Quhu(),
                new Jieming(),
                new Xingshang(),
                new Fangzhu(),
                new Xiaoguo(),
                new Rende(),
                new Wusheng(),
                new Paoxiao(),
                new PaoxiaoTM(),
                new Guanxing(),
                new Kongcheng(),
                new Longdan(),
                new Mashu("machao"),
                new Tieqi(),
                new TieqiInvalid(),
                new Jizhi(),
                new Qicai(),
                new Liegong(),
                new LiegongRange(),
                new Lianhuan(),
                new Niepan(),
                new Huoji(),
                new Kanpo(),
                new Bazhen(),
                new BazhenVH(),
                new Huoshou(),
                new SavageAssaultAvoid("huoshou"),
                new Zaiqi(),
                new Juxiang(),
                new SavageAssaultAvoid("juxiang"),
                new Lieren(),
                new Xiangle(),
                new Fangquan(),
                new Shushen(),
                new Shenzhi(),
                new Kuanggu(),
                new Jijiu(),
                new Chuli(),
                new Wushuang(),
                new Lijian(),
                new Biyue(),
                new Luanji(),
                new Shuangxiong(),
                new ShuangxiongVH(),
                new ShuangxiongGet(),
                new Wansha(),
                new WanshaProhibit(),
                new Luanwu(),
                new Weimu(),
                new Jianchu(),
                new Mashu("pangde"),
                new Leiji(),
                new Guidao(),
                new Beige(),
                new Duanchang(),
                new Xiongyi(),
                new Mashu("mateng"),
                new Mingshi(),
                new Lirang(),
                new LirangClear(),
                new Shuangren(),
                new Sijian(),
                new Suishi(),
                new Kuangfu(),
                new KuangfuJX(),
                new Huoshui(),
                new Qingcheng(),
                new Zhiheng(),
                new Qixi(),
                new Keji(),
                new KejiMax(),
                new Mouduan(),
                new Kurou(),
                new KurouTM(),
                new Yingzi("zhouyu", true),
                new YingziMax("zhouyu"),
                new Fanjian(),
                new Guose(),
                new Liuli(),
                new Qianxun(),
                new Duoshi(),
                new Jieyin(),
                new Xiaoji(),
                new Yinghun("sunjian"),
                new Tianxiang(),
                new TianxiangDraw(),
                new Hongyan(),
                new Tianyi(),
                new TianyiTargetMod(),
                new Buqu(),
                new BuquClear(),
                new Fenji(),
                new Haoshi(),
                new HaoshiGive(),
                new Dimeng(),
                new Zhijian(),
                new Guzheng(),
                new GuzhengRemove(),
                new Duanbing(),
                new Fenxun(),
                new FenxunDistance(),
            };
            skill_cards = new List<FunctionCard>
            {
                new ShowGeneralCard(),
                new ShensuCard(),
                new QiaobianCard(),
                new QiangxiCard(),
                new QuhuCard(),
                new JushouCard(),
                new RendeCard(),
                new FangquanCard(),
                new ChuliCard(),
                new LijianCard(),
                new LuanwuCard(),
                new XiongyiCard(),
                new LirangCard(),
                new HuoshuiCard(),
                new QingchengCard(),
                new ZhihengCard(),
                new KurouCard(),
                new FanjianCard(),
                new LiuliCard(),
                new JieyinCard(),
                new TianxiangCard(),
                new TianyiCard(),
                new HaoshiCard(),
                new DimengCard(),
                new ZhijianCard(),
                new FenxunCard(),
                new YijiCard(),
                new KuangfuCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "luoshen", new List<string> { "#luoshen-move" } },
                { "duanliang", new List<string> { "#duanliang-target" } },
                { "paoxiao", new List<string> { "#paoxiao-tm" } },
                { "liegong", new List<string> { "#liegong-for-lord" } },
                { "bazhen", new List<string> { "#bazhenvh" } },
                { "huoshou", new List<string> { "#sa_avoid_huoshou" } },
                { "juxiang", new List<string> { "#sa_avoid_juxiang" } },
                { "shuangxiong", new List<string> { "#shuangxiong-get" } },
                { "lirang", new List<string> { "#lirang-clear" } },
                {"buqu", new List<string>{ "#buqu-clear" } },
                {"haoshi", new List<string>{ "#haoshi-give" } },
                {"guzheng", new List<string>{ "#guzheng-remove" } },
                { "kurou", new List<string>{ "#kurou-tm" } },
                { "tianyi", new List<string>{ "#tianyi-target" } }
            };

            patterns = new Dictionary<string, CardPattern> {
                { ".", new ExpPattern(".|.|.|hand") },
                { ".S", new ExpPattern(".|spade|.|hand")},
                { ".C", new ExpPattern(".|club|.|hand")},
                { ".H", new ExpPattern(".|heart|.|hand")},
                { ".D", new ExpPattern(".|diamond|.|hand")},
                { ".black", new ExpPattern(".|black|.|hand")},
                { ".red", new ExpPattern(".|red|.|hand")},
                { "..", new ExpPattern(".")},
                { "..S", new ExpPattern(".|spade")},
                { "..C", new ExpPattern(".|club")},
                { "..H", new ExpPattern(".|heart")},
                { "..D", new ExpPattern(".|diamond")},

                { ".Basic", new ExpPattern("BasicCard")},
                { ".Trick", new ExpPattern("TrickCard")},
                { ".Equip", new ExpPattern("EquipCard")},

                { ".Weapon", new ExpPattern("Weapon")},
                { "slash", new ExpPattern(Slash.ClassName)},
                { "jink", new ExpPattern(Jink.ClassName)},
                { "peach", new ExpPattern(Peach.ClassName)},
                { "nullification", new ExpPattern(Nullification.ClassName)},
                { "peach+analeptic", new ExpPattern("Peach,Analeptic")}
            };
        }
    }
}

namespace SanguoshaServer.Package
{
    public class ShowGeneralCard : SkillCard
    {
        public static string ClassName = "ShowGeneralCard";
        public ShowGeneralCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.ShowSkill(card_use.From, card_use.Card.UserString, card_use.Card.SkillPosition);
        }
    }

    public class YijiCard : SkillCard
    {
        public static string ClassName = "YijiCard";
        public YijiCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && card.UserString.Split('+').Contains(to_select.Name);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
        }
    }

    #region 魏
    public class Jianxiong : MasochismSkill
    {
        public Jianxiong() : base("jianxiong")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.Card != null && Engine.GetFunctionCard(damage.Card.Name).TypeID != CardType.TypeSkill)
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
        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            room.ObtainCard(target, damage.Card);
        }
    }

    class Fankui : MasochismSkill
    {
        public Fankui() : base("fankui")
        {
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage)
            {
                Player from = damage.From;
                return (from != null && RoomLogic.CanGetCard(room, player, from, "he")) ? new TriggerStruct(Name, player) : new TriggerStruct();
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player simayi, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            if (!damage.From.IsNude() && room.AskForSkillInvoke(simayi, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, simayi, info.SkillPosition);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, simayi.Name, damage.From.Name);
                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player simayi, DamageStruct damage, TriggerStruct info)
        {
            int card_id = room.AskForCardChosen(simayi, damage.From, "he", Name, false, HandlingMethod.MethodGet);
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, simayi.Name);
            room.ObtainCard(simayi, room.GetCard(card_id), reason, false);
        }

    }

    public class Guicai : TriggerSkill
    {
        public Guicai() : base("guicai")
        {
            events.Add(TriggerEvent.AskForRetrial);
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            if (player.IsKongcheng() && player.GetHandPile().Count == 0)
                return new TriggerStruct();
            return new TriggerStruct(Name, player);
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;

            List<string> prompt_list = new List<string> { "@guicai-card", judge.Who.Name, string.Empty, Name, judge.Reason };
            string prompt = string.Join(":", prompt_list);

            room.SetTag(Name, data);
            WrappedCard card = room.AskForCard(player, Name, ".", prompt, data, HandlingMethod.MethodResponse, judge.Who, true);
            room.RemoveTag(Name);
            if (card != null)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.Retrial(card, player, ref judge, Name, false, info.SkillPosition);
                data = judge;
                return info;
            }


            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            room.UpdateJudgeResult(ref judge);
            data = judge;
            return false;
        }
    }

    class Ganglie : MasochismSkill
    {
        public Ganglie() : base("ganglie")
        {
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition) && data is DamageStruct damage)
            {
                if (damage.From != null)
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player xiahou, DamageStruct damage, TriggerStruct info)
        {
            Player from = damage.From;

            JudgeStruct judge = new JudgeStruct
            {
                Pattern = ".|heart",
                Negative = false,
                Good = false,
                Reason = Name,
                PlayAnimation = true,
                Who = xiahou
            };

            room.Judge(ref judge);
            if (judge.IsGood() && from != null && from.Alive)
            {
                bool discard = false;
                if (from.HandcardNum >= 2)
                {
                    from.SetFlags("ganglie_invoker");
                    discard = room.AskForDiscard(from, null, Name, 2, 2, true);
                    from.SetFlags("-ganglie_invoker");
                }
                if (!discard)
                    room.Damage(new DamageStruct(Name, xiahou, from));
            }
        }
    }

    class Tuxi : TriggerSkill
    {
        public Tuxi() : base("tuxi")
        {
            events.Add(TriggerEvent.EventPhaseStart);// << EventPhaseProceeding;
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Draw)
            {
                bool can_invoke = false;
                List<Player> other_players = room.GetOtherPlayers(player);
                foreach (Player p in other_players)
                {
                    if (!p.IsKongcheng())
                    {
                        can_invoke = true;
                        break;
                    }
                }

                if (can_invoke)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> to_choose = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (RoomLogic.CanGetCard(room, player, p, "h"))
                    to_choose.Add(p);
            }

            if (to_choose.Count > 0)
            {
                List<Player> choosees = room.AskForPlayersChosen(player, to_choose, Name, 0, 2, "@tuxi-card", true, info.SkillPosition);
                if (choosees.Count > 0)
                {
                    room.SortByActionOrder(ref choosees);
                    room.SetTag("tuxi_invoke" + player.Name, choosees);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player source, ref object data, Player ask_who, TriggerStruct info)
        {
            string str = "tuxi_invoke" + source.Name;
            List<Player> targets = room.ContainsTag(str) ? (List<Player>)room.GetTag(str) : new List<Player>();
            room.RemoveTag(str);
            int id = room.AskForCardChosen(source, targets[0], "h", Name, false, HandlingMethod.MethodGet);
            CardsMoveStruct move1 = new CardsMoveStruct
            {
                Card_ids = new List<int> { id },
                To = source.Name,
                To_place = Place.PlaceHand,
                Reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, source.Name, targets[0].Name, Name, null)
            };
            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move1 };
            if (targets.Count == 2)
            {
                id = room.AskForCardChosen(source, targets[1], "h", Name, false, HandlingMethod.MethodGet);
                CardsMoveStruct move2 = new CardsMoveStruct
                {
                    Card_ids = new List<int> { id },
                    To = source.Name,
                    To_place = Place.PlaceHand,
                    Reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, source.Name, targets[1].Name, Name, string.Empty)
                };
                moves.Add(move2);
            }
            room.MoveCardsAtomic(moves, false);
            return true;
        }
    }

    public class Luoyi : TriggerSkill
    {
        public Luoyi() : base("luoyi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseProceeding, TriggerEvent.PreCardUsed };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.PreCardUsed && player != null && player.Alive && player.HasFlag(Name) && data is CardUseStruct use)
            {
                if (use.Card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                    if (fcard is Slash || fcard is Duel)
                        room.SetCardFlag(use.Card, Name);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw && (int)data > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                data = (int)data - 1;
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetTag(Name, info.SkillPosition);
            player.SetFlags(Name);

            return false;
        }
    }

    class LuoyiDamage : TriggerSkill
    {
        public LuoyiDamage() : base("luoyi-damage")
        {
            events.Add(TriggerEvent.DamageCaused);
            frequency = Frequency.Compulsory;
            global = true;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && player.HasFlag("luoyi") && data is DamageStruct damage)
            {
                if (damage.Card != null && damage.Card.HasFlag("luoyi") && !damage.Chain && !damage.Transfer && damage.ByUser)
                {
                    TriggerStruct trigger = new TriggerStruct(Name, player)
                    {
                        SkillPosition = (string)player.GetTag("luoyi")
                    };
                    return trigger;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke("luoyi", "male", 1, gsk.General, gsk.SkinId);
            DamageStruct damage = (DamageStruct)data;
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

            return false;
        }
    }
    public class Tiandu : TriggerSkill
    {
        public Tiandu() : base("tiandu")
        {
            frequency = Frequency.Frequent;
            events.Add(TriggerEvent.FinishJudge);
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            JudgeStruct judge = (JudgeStruct)data;
            if (room.GetCardPlace(judge.Card.GetEffectiveId()) == Place.PlaceJudge && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
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
            JudgeStruct judge = (JudgeStruct)data;
            room.ObtainCard(player, judge.Card);
            return false;
        }
    }

    public class Yiji : MasochismSkill
    {
        public Yiji() : base("yiji")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player);
                //trigger.times = damage.damage;
                return trigger;
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
        public override void OnDamaged(Room room, Player guojia, DamageStruct damage, TriggerStruct info)
        {
            List<int> yiji_cards = room.GetNCards(2);
            List<int> origin_yiji = new List<int>(yiji_cards);

            while (guojia.Alive && yiji_cards.Count > 0)
            {
                guojia.PileChange("#yiji", yiji_cards);
                if (!room.AskForYiji(guojia, yiji_cards, Name, true, false, true, -1, room.GetOtherPlayers(guojia), null, null, "#yiji", false, info.SkillPosition))
                    break;

                guojia.Piles["#yiji"].Clear();
                foreach (int id in origin_yiji)
                    if (room.GetCardPlace(id) != Place.DrawPile)
                        yiji_cards.Remove(id);
            }
            if (guojia.GetPile("#yiji").Count > 0) guojia.Piles["#yiji"].Clear();
            if (yiji_cards.Count > 0 && guojia.Alive)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, guojia.Name);
                room.ObtainCard(guojia, ref yiji_cards, reason, false);
                yiji_cards.Clear();
            }

            if (yiji_cards.Count > 0)
                room.ReturnToDrawPile(yiji_cards, false);
        }
    }

    public class Luoshen : TriggerSkill
    {
        public Luoshen() : base("luoshen")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override bool CanPreShow() => false;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start)
            {
                if (base.Triggerable(player, room))
                    return new TriggerStruct(Name, player);
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
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhenji, ref object data, Player ask_who, TriggerStruct info)
        {
            zhenji.SetTag("luoshen_position", info.SkillPosition);
            JudgeStruct judge;
            do
            {
                judge = new JudgeStruct
                 {
                     Pattern = ".|black",
                     Good = true,
                     Reason = Name,
                     PlayAnimation = false,
                     Who = zhenji,
                     Time_consuming = true
                 };
                room.Judge(ref judge);

                if (judge.IsBad())
                    Thread.Sleep(1000);
            }
            while (judge.IsGood() && room.AskForSkillInvoke(zhenji, Name, null, info.SkillPosition));

            List<int> card_list = zhenji.ContainsTag(Name) ? (List<int>)zhenji.GetTag(Name) : new List<int>();
            zhenji.RemoveTag(Name);
            List<int> subcards = new List<int>();
            foreach (int id in card_list)
                if (room.GetCardPlace(id) == Place.PlaceTable && !subcards.Contains(id))
                    subcards.Add(id);
            if (subcards.Count > 0)
                room.ObtainCard(zhenji, ref subcards, new CardMoveReason(MoveReason.S_REASON_GOTBACK, zhenji.Name));

            return false;
        }
    }

    public class LuoshenMove : TriggerSkill
    {
        public LuoshenMove() : base("#luoshen-move")
        {
            events.Add(TriggerEvent.FinishJudge);
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player != null && data is JudgeStruct judge && judge.Reason == "luoshen" && judge.IsGood())
            {
                List<int> luoshen_list = player.ContainsTag("luoshen") ? (List<int>)player.GetTag("luoshen") : new List<int>();
                luoshen_list.Add(judge.Card.GetEffectiveId());
                player.SetTag("luoshen", luoshen_list);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && data is JudgeStruct judge && judge.Reason == "luoshen" && judge.IsGood())
            {
                if (room.GetCardPlace(judge.Card.GetEffectiveId()) == Place.PlaceJudge)
                {
                    TriggerStruct trigger = new TriggerStruct(Name, player)
                    {
                        SkillPosition = (string)player.GetTag("luoshen_position")
                    };
                    return trigger;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhenji, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_JUDGEDONE, zhenji.Name, null, judge.Reason);
            room.MoveCardTo(judge.Card, null, Place.PlaceTable, reason, true);

            return false;
        }

    }

    public class Qingguo : OneCardViewAsSkill
    {
        public Qingguo() : base("qingguo")
        {
            filter_pattern = ".|black|.|hand";
            response_or_use = true;
            skill_type = SkillType.Defense;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return pattern.StartsWith(Jink.ClassName);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard jink = new WrappedCard(Jink.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            jink.AddSubCard(card);
            jink = RoomLogic.ParseUseCard(room, jink);
            return jink;
        }
    }

    public class ShensuCard : SkillCard
    {
        public static string ClassName = "ShensuCard";
        public ShensuCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                Skill = "shensu",
                DistanceLimited = false
            };
            return Slash.Instance.TargetFilter(room, targets, to_select, Self, slash);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> targets = new List<Player>(card_use.To);
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                Skill = "_shensu",
                DistanceLimited = false
            };
            foreach (Player target in card_use.To)
        if (!RoomLogic.CanSlash(room, card_use.From, target, slash))
                targets.Remove(target);

            if (targets.Count > 0)
            {
                string index = "2";
                if (room.GetRoomState().GetCurrentCardUsePattern().EndsWith("1"))
                    index = "1";
                
                room.SetTag("shensu_invoke" + card_use.From.Name, targets);
                
                card_use.From.SetFlags("shensu" + index);
            }
        }
    }

    public class ShensuViewAsSkill : ViewAsSkill
    {
        public ShensuViewAsSkill() : base("shensu")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern) => pattern.StartsWith("@@shensu");
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (room.GetRoomState().GetCurrentCardUsePattern().EndsWith("1"))
                return false;
            else
                return selected.Count == 0 && Engine.GetFunctionCard(to_select.Name) is EquipCard && !RoomLogic.IsJilei(room, player, to_select);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (room.GetRoomState().GetCurrentCardUsePattern().EndsWith("1"))
            {
                if (cards.Count == 0)
                {
                    WrappedCard card = new WrappedCard(ShensuCard.ClassName)
                    {
                        Skill = Name
                    };
                    return card;
                }
            }
            else if (cards.Count == 1)
            {
                WrappedCard card = new WrappedCard(ShensuCard.ClassName)
                {
                    Skill = Name
                };
                card.AddSubCards(cards);
                return card;
            }
            return null;
        }
    }

    public class Shensu : TriggerSkill
    {
        public Shensu() : base("shensu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new ShensuViewAsSkill();
            skill_type = SkillType.Attack;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player xiahouyuan, ref object data, Player ask_who)
        {
            if (!base.Triggerable(xiahouyuan, room) || !Slash.IsAvailable(room, xiahouyuan))
                return new TriggerStruct();

            PhaseChangeStruct change = (PhaseChangeStruct)data;
            if (change.To == PlayerPhase.Judge && !xiahouyuan.IsSkipped(PlayerPhase.Judge) && !xiahouyuan.IsSkipped(PlayerPhase.Draw))
            {
                room.RemoveTag("shensu_invoke1" + xiahouyuan.Name);
                return new TriggerStruct(Name, xiahouyuan);
            }
            else if (change.To == PlayerPhase.Play && RoomLogic.CanDiscard(room, xiahouyuan, xiahouyuan, "he") && !xiahouyuan.IsSkipped(PlayerPhase.Play))
            {
                room.RemoveTag("shensu_invoke2" + xiahouyuan.Name);
                return new TriggerStruct(Name, xiahouyuan);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player xiahouyuan, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            if (change.To == PlayerPhase.Judge && room.AskForUseCard(xiahouyuan, "@@shensu1", "@shensu1", null, 1, HandlingMethod.MethodUse, true, info.SkillPosition) != null)
            {
                if (xiahouyuan.HasFlag("shensu1") && room.ContainsTag("shensu_invoke" + xiahouyuan.Name))
                {
                    room.SkipPhase(xiahouyuan, PlayerPhase.Judge);
                    room.SkipPhase(xiahouyuan, PlayerPhase.Draw);
                    return info;
                }
            }
            else if (change.To == PlayerPhase.Play && room.AskForUseCard(xiahouyuan, "@@shensu2", "@shensu2", null, 2, HandlingMethod.MethodDiscard, true, info.SkillPosition) != null)
            {
                if (xiahouyuan.HasFlag("shensu2") && room.ContainsTag("shensu_invoke" + xiahouyuan.Name))
                {
                    room.SkipPhase(xiahouyuan, PlayerPhase.Play);
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            List<Player> targets;
            if (change.To == PlayerPhase.Judge)
            {
                targets = (List<Player>)room.GetTag("shensu_invoke" + player.Name);
                room.RemoveTag("shensu_invoke" + player.Name);
            }
            else
            {
                targets = (List<Player>)room.GetTag("shensu_invoke" + player.Name);
                room.RemoveTag("shensu_invoke" + player.Name);
            }

            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                Skill = "_shensu",
                DistanceLimited = false
            };

            room.UseCard(new CardUseStruct(slash, player, targets));
            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = card.SubCards.Count + 1;
        }
    }

    public class QiaobianCard : SkillCard
    {
        public static string ClassName = "QiaobianCard";
        public QiaobianCard() : base(ClassName)
        {
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            PlayerPhase phase = (PlayerPhase)Self.GetMark("qiaobianPhase");
            if (phase == PlayerPhase.Draw)
                return targets.Count <= 2 && targets.Count > 0;
            else if (phase == PlayerPhase.Play)
                return targets.Count == 1;
            return false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard qiaobian)
        {
            PlayerPhase phase = (PlayerPhase)Self.GetMark("qiaobianPhase");
            if (phase == PlayerPhase.Draw)
                return targets.Count < 2 && to_select != Self && RoomLogic.CanGetCard(room, Self, to_select, "h");
            else if (phase == PlayerPhase.Play)
            {
                if (targets.Count > 0)
                    return false;
                foreach (WrappedCard card in RoomLogic.GetPlayerEquips(room, to_select))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    EquipCard equip = (EquipCard)fcard;

                    int equip_index = (int)equip.EquipLocation();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p == to_select) continue;
                        if (p.GetEquip(equip_index) < 0)
                            return true;
                    }
                }
                foreach (WrappedCard card in RoomLogic.GetPlayerJudgingArea(room, to_select))
                {
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p == to_select) continue;
                        if (!RoomLogic.PlayerContainsTrick(room, p, card.Name))
                            return true;
                    }
                }
            }
            return false;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> targets = new List<Player>(card_use.To);
            PlayerPhase phase = (PlayerPhase)card_use.From.GetMark("qiaobianPhase");
            if (phase == PlayerPhase.Draw)
            {
                if (targets.Count == 0)
                    return;
                int id = room.AskForCardChosen(card_use.From, targets[0], "h", "qiaobian", false, HandlingMethod.MethodGet);
                List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                CardsMoveStruct move1 = new CardsMoveStruct
                {
                    Card_ids = new List<int> { id },
                    To = card_use.From.Name,
                    To_place = Place.PlaceHand,
                    Reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, card_use.From.Name, targets[0].Name, "qiaobian", null)
                };
                moves.Add(move1);
                if (targets.Count == 2)
                {
                    id = room.AskForCardChosen(card_use.From, targets[1], "h", "qiaobian", false, HandlingMethod.MethodGet);
                    CardsMoveStruct move2 = new CardsMoveStruct
                    {
                        Card_ids = new List<int> { id },
                        To = card_use.From.Name,
                        To_place = Place.PlaceHand,
                        Reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, card_use.From.Name, targets[1].Name, "qiaobian", null)
                    };
                    moves.Add(move2);
                }
                room.MoveCardsAtomic(moves, false);
            }
            else if (phase == PlayerPhase.Play)
            {
                if (targets.Count == 0)
                    return;

                Player from = targets[0];
                if (from.GetCards("ej").Count == 0)
                    return;

                room.SetTag("QiaobianTarget", from);
                int card_id = room.AskForCardChosen(card_use.From, from, "ej", "qiaobian");
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
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (equip_index != -1)
                    {
                        if (p.GetEquip(equip_index) < 0 && RoomLogic.CanPutEquip(p, card))
                            tos.Add(p);
                    }
                    else if (RoomLogic.IsProhibited(room, null, p, card) == null && !RoomLogic.PlayerContainsTrick(room, p, card.Name) && p.JudgingAreaAvailable)
                            tos.Add(p);
                }

                string position = card_use.Card.SkillPosition;
                Player to = room.AskForPlayerChosen(card_use.From, tos, "qiaobian", "@qiaobian-to:::" + card.Name, false, false, position);
                if (to != null)
                {
                    if ((place == Place.PlaceDelayedTrick && from != card_use.From) || (place == Place.PlaceEquip && to != card_use.From))
                    {
                        ResultStruct result = card_use.From.Result;
                        result.Assist++;
                        card_use.From.Result = result;
                    }

                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, from.Name, to.Name);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_TRANSFER, card_use.From.Name, "qiaobian", null)
                    {
                        Card = card
                    };
                    room.MoveCardTo(card, from, to, place, reason);
                }
                room.RemoveTag("QiaobianTarget");
            }
        }
    }

    public class QiaobianViewAsSkill : ZeroCardViewAsSkill
    {
        public QiaobianViewAsSkill() : base("qiaobian")
        {
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@qiaobian";
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(QiaobianCard.ClassName)
            {
                Skill = Name,
                Mute = true
            };
            return card;
        }
    }
    public class Qiaobian : TriggerSkill
    {
        public Qiaobian() : base("qiaobian")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new QiaobianViewAsSkill();
            skill_type = SkillType.Wizzard;
        }
        private readonly List<string> phase_strings = new List<string> { "round_start" , "start" , "judge" , "draw"
                , "play" , "discard", "finish" , "not_active" };
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            player.SetMark("qiaobianPhase", (int)change.To);
            int index = 0;
            switch (change.To)
            {
                case PlayerPhase.RoundStart:
                case PlayerPhase.Start:
                case PlayerPhase.Finish:
                case PlayerPhase.PhaseNone:
                case PlayerPhase.NotActive: return new TriggerStruct();

                case PlayerPhase.Judge: index = 1; break;
                case PlayerPhase.Draw: index = 2; break;
                case PlayerPhase.Play: index = 3; break;
                case PlayerPhase.Discard: index = 4; break;
            }
            if (base.Triggerable(player, room) && index > 0 && RoomLogic.CanDiscard(room, player, player, "h"))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player zhanghe, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            int index = (int)(change.To);
            string discard_prompt = string.Format("#qiaobian:::{0}", phase_strings[index]);

            if (room.AskForDiscard(zhanghe, Name, 1, 1, true, false, discard_prompt, true, info.SkillPosition) && zhanghe.Alive)
            {
                room.BroadcastSkillInvoke("qiaobian", zhanghe, info.SkillPosition);
                room.SkipPhase(zhanghe, change.To);

                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            int index = 0;
            switch (change.To)
            {
                case PlayerPhase.RoundStart:
                case PlayerPhase.Start:
                case PlayerPhase.Finish:
                case PlayerPhase.PhaseNone:
                case PlayerPhase.NotActive: return false;

                case PlayerPhase.Judge: index = 1; break;
                case PlayerPhase.Draw: index = 2; break;
                case PlayerPhase.Play: index = 3; break;
                case PlayerPhase.Discard: index = 4; break;
            }
            if (index == 2 || index == 3)
            {
                string use_prompt = string.Format("@qiaobian-{0}", index);
                room.AskForUseCard(player, "@@qiaobian", use_prompt, null, index, HandlingMethod.MethodUse, true, info.SkillPosition);
            }
            return false;
        }
    }

    public class DuanliangVS : OneCardViewAsSkill
    {
        public DuanliangVS() : base("duanliang")
        {
            filter_pattern = "BasicCard,EquipCard|black";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasFlag(Name);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard card = new WrappedCard(SupplyShortage.ClassName);
                if (Engine.MatchExpPattern(room, pattern, player, card)) return true;
            }
            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard shortage = new WrappedCard(SupplyShortage.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            shortage.AddSubCard(card);
            shortage = RoomLogic.ParseUseCard(room, shortage);

            return shortage;
        }
    }

    public class Duanliang : TriggerSkill
    {
        public Duanliang() : base("duanliang")
        {
            events.Add(TriggerEvent.CardUsedAnnounced);
            view_as_skill = new DuanliangVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardUseStruct use && use.Card.Name == SupplyShortage.ClassName && base.Triggerable(player, room)
                && use.To.Count > 0 && RoomLogic.DistanceTo(room, player, use.To[0]) > 2)
            {
                player.SetFlags(Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class DuanliangTargetMod : TargetModSkill
    {
        public DuanliangTargetMod() : base("#duanliang-target")
        {
            pattern = SupplyShortage.ClassName;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "duanliang") && !from.HasFlag("duanliang"))
                return true;
            else
                return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -1;
            if (type == ModType.DistanceLimit && card.Skill == "duanliang")
                index = -2;
        }
    }

    public class Jushou : PhaseChangeSkill
    {
        public Jushou() : base("jushou")
        {
            view_as_skill = new JushouVS();
            frequency = Frequency.Frequent;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish ?
                new TriggerStruct(Name, player) : new TriggerStruct(); ;
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
        public override bool OnPhaseChange(Room room, Player caoren, TriggerStruct info)
        {
            List<string> kingdoms = new List<string>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HasShownOneGeneral() && !kingdoms.Contains(p.Kingdom))
                    kingdoms.Add(p.Kingdom);

            room.DrawCards(caoren, kingdoms.Count, Name);

            List<int> ids = caoren.GetCardCount(false) == 1 ? caoren.GetCards("h") : new List<int>();
            if (ids.Count == 0)
            {
                WrappedCard use = room.AskForUseCard(caoren, "@@jushou", "@jushou", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
                if (use != null)
                    ids = new List<int>(use.SubCards);
                else
                    ids = room.ForceToDiscard(caoren, caoren.GetCards("h"), 1);
            }

            if (ids.Count == 1)
            {
                    int id = ids[0];
                    WrappedCard card = room.GetCard(id);

                    bool discard = true;
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, caoren, card))
                    {
                        if (RoomLogic.CanDiscard(room, caoren, caoren, id))
                            discard = room.AskForChoice(caoren, Name, "use+discard") == "discard";
                        else
                            discard = false;
                    }

                    if (discard)
                    {
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, caoren, info.SkillPosition);
                        CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISCARD, caoren.Name, null, Name, null)
                        {
                            General = gsk
                        };
                        room.ThrowCard(ref ids, reason, caoren, caoren);
                    }
                    else
                        room.UseCard(new CardUseStruct(card, caoren, new List<Player>(), true));
            }

            if (kingdoms.Count > 2)
                room.TurnOver(caoren);
            return false;
        }
    }

    public class JushouVS : OneCardViewAsSkill
    {
        public JushouVS() : base("jushou")
        {
            response_pattern = "@@jushou";
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(to_select.Name);
            return room.GetCardPlace(to_select.Id) == Place.PlaceHand && (fcard is EquipCard && fcard.IsAvailable(room, player, to_select)
                || RoomLogic.CanDiscard(room, player, player, to_select.Id));
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard jushou = new WrappedCard(JushouCard.ClassName)
            {
                Mute = true,
                Skill = Name
            };
            jushou.AddSubCard(card);
            return jushou;
        }
    }

    public class JushouCard : SkillCard
    {
        public static string ClassName = "JushouCard";
        public JushouCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
        }
    }

    public class QiangxiCard : SkillCard
    {
        public static string ClassName = "QiangxiCard";
        public QiangxiCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0 || to_select == Self)
                return false;

            return RoomLogic.InMyAttackRange(room, Self, to_select, card);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            if (card_use.Card.SubCards.Count == 0)
                room.LoseHp(card_use.From);

            base.Use(room, card_use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.Damage(new DamageStruct("qiangxi", effect.From, effect.To));
        }
    }

    public class Qiangxi : ViewAsSkill
    {
        public Qiangxi() : base("qiangxi")
        {
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(QiangxiCard.ClassName);
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(to_select.Name);
            return selected.Count == 0 && fcard is Weapon && !RoomLogic.IsJilei(room, player, to_select);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
            {
                WrappedCard card = new WrappedCard(QiangxiCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                return card;
            }
            else if (cards.Count == 1)
            {
                WrappedCard card = new WrappedCard(QiangxiCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                card.AddSubCards(cards);
                return card;
            }
            else
                return null;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 2 - card.SubCards.Count;
        }
    }

    public class QuhuCard : SkillCard
    {
        public QuhuCard() : base("QuhuCard")
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.Hp > Self.Hp && RoomLogic.CanBePindianBy(room, to_select, Self);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            PindianStruct pd = room.PindianSelect(card_use.From, target, "quhu");
            room.Pindian(ref pd);

            if (pd.Success)
            {
                room.SetTag("quhu", target);
                List<Player> players = room.GetOtherPlayers(target), wolves = new List<Player>();
                room.RemoveTag("quhu");
                foreach (Player player in players) {
                    if (RoomLogic.InMyAttackRange(room, target, player))
                        wolves.Add(player);
                }

                if (wolves.Count == 0)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#QuhuNoWolf",
                        From = card_use.From.Name,
                        To = new List<string> { target.Name }
                    };
                    room.SendLog(log);

                    return;
                }
                Player wolf = room.AskForPlayerChosen(card_use.From, wolves, "quhu", string.Format("@quhu-damage:{0}", target.Name), false, false, card_use.Card.Skill);

                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, wolf.Name);
                room.Damage(new DamageStruct("quhu", target, wolf));
            }
            else
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, card_use.From.Name);
                room.Damage(new DamageStruct("quhu", target, card_use.From));
            }
        }
    }

    public class Quhu : ZeroCardViewAsSkill
    {
        public Quhu() : base("quhu")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("QuhuCard") && !player.IsKongcheng();
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("QuhuCard")
            {
                Skill = name,
                ShowSkill = Name
            };
            return card;
        }
    }

    public class Jieming : MasochismSkill
    {
        public Jieming() : base("jieming")
        {
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && RoomLogic.PlayerHasSkill(room, player, Name) && data is DamageStruct damage)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player);
                //trigger.times = damage.damage;
                return trigger;
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (!player.Alive)
                return new TriggerStruct();

            Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "jieming-invoke", true, true, info.SkillPosition);
            if (target != null)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", (target == player ? 2 : 1), gsk.General, gsk.SkinId);

                List<string> target_list = player.ContainsTag("jieming_target") ? (List<string>)player.GetTag("jieming_target") : new List<string>();
                target_list.Add(target.Name);
                player.SetTag("jieming_target", target_list);

                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player player, DamageStruct damage, TriggerStruct info)
        {
            List<string> target_list =(List<string>)player.GetTag("jieming_target");
            string target_name = target_list[target_list.Count - 1];
            target_list.RemoveAt(target_list.Count - 1);
            player.SetTag("jieming_target", target_list);

            Player to = room.FindPlayer(target_name);

            if (to != null)
            {
                int upper = Math.Min(5, to.MaxHp);
                int x = upper - to.HandcardNum;
                if (x > 0)
                    room.DrawCards(to, new DrawCardStruct(x, player, Name));
            }
        }
    }

    public class Xingshang : TriggerSkill
    {
        public Xingshang() : base("xingshang")
        {
            events.Add(TriggerEvent.Death);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.IsNude()) return triggers;
            List<Player> caopis = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player caopi in caopis)
                if (caopi != player)
                    triggers.Add(new TriggerStruct(Name, caopi));

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            if (!player.IsNude() && room.AskForSkillInvoke(caopi, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, caopi, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            if (player.IsNude() || caopi == player)
                return false;

            List<int> ids= player.GetCards("h");
            ids.AddRange(player.GetEquips());

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_RECYCLE, caopi.Name);
            room.ObtainCard(caopi, ref ids, reason, false);

            return false;
        }
    }

    public class Fangzhu : MasochismSkill
    {
        public Fangzhu() : base("fangzhu")
        {

            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player to = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "fangzhu-invoke", true, true, info.SkillPosition);
            if (to != null)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name);
                room.BroadcastSkillInvoke(Name, "male", (to.FaceUp ? 1 : 2), gsk.General, gsk.SkinId);
                List<string> target_list = player.ContainsTag("fangzhu_target") ? (List<string>)player.GetTag("fangzhu_target") : new List<string>();
                target_list.Add(to.Name);
                player.SetTag("fangzhu_target", target_list);
                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player caopi, DamageStruct damage, TriggerStruct info)
        {
            List<string> target_list = (List<string>)caopi.GetTag("fangzhu_target");
            string target_name = target_list[target_list.Count - 1];
            target_list.RemoveAt(target_list.Count - 1);
            caopi.SetTag("fangzhu_target", target_list);
            Player to = room.FindPlayer(target_name); ;

            if (to != null)
            {
                if (caopi.IsWounded())
                    room.DrawCards(to, new DrawCardStruct(caopi.GetLostHp(), caopi, Name));
                room.TurnOver(to);
            }
        }

    }

    public class Xiaoguo : TriggerSkill
    {
        public Xiaoguo() : base("xiaoguo")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Attack;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player != null && player.Phase == PlayerPhase.Finish)
            {
                List<Player> yuejins = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player yuejin in yuejins)
                {
                    if (player != yuejin && RoomLogic.CanDiscard(room, yuejin, yuejin, "h"))
                        skill_list.Add(new TriggerStruct(Name, yuejin));
                }
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ints = new List<int>(room.AskForExchange(ask_who, Name, 1, 0, "@xiaoguo:" + player.Name, null, "BasicCard!", info.SkillPosition));
            if (ints.Count == 1)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISMANTLE, ask_who.Name, ask_who.Name, Name, null)
                {
                    General = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition)
                };
                room.ThrowCard(ref ints, reason, ask_who, null, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetTag(Name, ask_who);
            WrappedCard card = room.AskForCard(player, Name, ".Equip", "@xiaoguo-discard:" + ask_who.Name, null);
            room.RemoveTag(Name);
            if (card == null)
                room.Damage(new DamageStruct(Name, ask_who, player));

            return false;
        }
    }
    #endregion

    #region 蜀

    public class RendeCard : SkillCard
    {
        public RendeCard() : base("RendeCard")
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && !to_select.HasFlag("rende_" + Self.Name);
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1 && card.SubCards.Count > 0;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            target.SetFlags("rende_" + card_use.From.Name);
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, card_use.From.Name, target.Name, "rende", null);
            room.ObtainCard(target, card_use.Card, reason, false);

            int old_value = card_use.From.GetMark("rende");
            int new_value = old_value + card_use.Card.SubCards.Count;
            room.SetPlayerMark(card_use.From, "rende", new_value);

            if (old_value < 2 && new_value >= 2)
                room.AskForUseCard(card_use.From, "@@rende", "@rende", null, -1, HandlingMethod.MethodUse, true, card_use.Card.Skill);
        }
    }

    public class RendeVS : ViewAsSkill
    {
        public RendeVS() : base("rende")
        {
            response_pattern = "@@rende";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !player.HasEquip(to_select.Name);
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng();
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            string pattern = room.GetRoomState().GetCurrentCardUsePattern(player);
            if (pattern == response_pattern)
            {
                if (cards.Count > 0)
                    return Engine.CloneCard(cards[0]);
                else
                    return null;
            }
            else
            {
                WrappedCard rende_card = new WrappedCard("RendeCard");
                rende_card.AddSubCards(cards);
                rende_card.Skill = Name;
                rende_card.ShowSkill = Name;
                return rende_card;
            }
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player Self)
        {
            string pattern = room.GetRoomState().GetCurrentCardUsePattern(Self);

            List<WrappedCard> all_cards = new List<WrappedCard>();
            if (pattern == response_pattern)
            {
                List<string> names = GetGuhuoCards(room, "b");
                foreach (string name in names)
                {
                    if (name == Jink.ClassName) continue;
                    WrappedCard card = new WrappedCard(name)
                    {
                        Skill = "_rende"
                    };
                    all_cards.Add(card);
                }
            }

            return all_cards;
        }
    }

    public class Rende : TriggerSkill
    {
        public Rende() : base("rende")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.PreCardUsed };
            view_as_skill = new RendeVS();
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.PreCardUsed && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash && use.Card.IsVirtualCard() && use.Card.SubCards.Count == 0 && use.Card.Skill == Name)
                    player.AddHistory(Slash.ClassName);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && player.GetMark(Name) > 0 && data is PhaseChangeStruct change && change.From == PlayerPhase.Play)
            {
                player.SetMark(Name, 0);
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasFlag("rende_" + player.Name))
                        p.SetFlags("-rende_" + player.Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class Wusheng : OneCardViewAsSkill
    {
        public Wusheng() : base("wusheng")
        {
            response_or_use = true;
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return Slash.IsAvailable(room, player);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return Engine.GetPattern(pattern).GetPatternString().StartsWith(Slash.ClassName);
        }
        public override bool ViewFilter(Room room, WrappedCard card, Player player)
        {
            Player lord = RoomLogic.GetLord(room, player.Kingdom);
            if (lord == null || !RoomLogic.PlayerHasSkill(room, lord, "shouyue") || !lord.General1Showed)
                if (!WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card)))
                    return false;

            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                slash.AddSubCard(card);
                return Slash.IsAvailable(room, player, slash);
            }
            return true;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            slash.AddSubCard(card);
            slash = RoomLogic.ParseUseCard(room, slash);
            return slash;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -1;
            if (!WrappedCard.IsRed(card.Suit))
            {
                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "shouyue") && lord.General1Showed)
                {
                    room.NotifySkillInvoked(lord, "shouyue");
                    room.BroadcastSkillInvoke("shouyue", lord, "head");
                }
            }
        }
    }

    public class PaoxiaoTM : TargetModSkill
    {
        public PaoxiaoTM() : base("#paoxiao-tm")
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "paoxiao") || from.GetSkills(false, false).Contains("paoxiao"))
                return 1000;
            else
                return 0;
        }
    }

    public class Paoxiao : TriggerSkill
    {
        public Paoxiao() : base("paoxiao")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.TargetChosen, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    player.AddMark(Name);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark(Name) > 0)
                        p.SetMark(Name, 0);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room) || !(data is CardUseStruct use))
                return new TriggerStruct();
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            if (!(fcard is Slash)) return new TriggerStruct();

            if (triggerEvent == TriggerEvent.TargetChosen)
            {
                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "shouyue") && lord.General1Showed)
                {
                    if (use.To.Count > 0)
                        return new TriggerStruct(Name, player, use.To);
                }
            }
            else if (triggerEvent == TriggerEvent.CardUsed && player.GetMark(Name) == 2)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who != null)
            {
                bool invoke = RoomLogic.PlayerHasShownSkill(room, ask_who, Name)
                    || room.AskForSkillInvoke(ask_who, Name, triggerEvent == TriggerEvent.TargetChosen ? "#armor_nullify:" + target.Name : null, info.SkillPosition);
                if (invoke)
                    return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (triggerEvent == TriggerEvent.TargetChosen)
            {
                Player lord = RoomLogic.GetLord(room, ask_who.Kingdom);
                room.NotifySkillInvoked(lord, "shouyue");
                room.BroadcastSkillInvoke("shouyue", lord, "head");
                target.AddQinggangTag(RoomLogic.CardToString(room, use.Card));
            }
            else
                room.DrawCards(ask_who, 1, Name);

            return false;
        }
    }

    public class Guanxing : PhaseChangeSkill
    {
        public Guanxing() : base("guanxing")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Wizzard;
        }
        public override bool CanPreShow() => false;
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Start;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasSkill(room, player, "yizhi") && RoomLogic.InPlayerDeputykills(player, "yizhi") && !RoomLogic.InPlayerHeadSkills(player, Name))
            {
                TriggerStruct trigger = info;
                trigger.SkillPosition = "deputy";
                if (room.AskForSkillInvoke(player, Name, null, trigger.SkillPosition))
                {
                    room.BroadcastSkillInvoke("yizhi", player, "deputy");
                    return trigger;
                }
                else
                {
                    return new TriggerStruct();
                }
            }
            else if (!RoomLogic.PlayerHasSkill(room, player, "yizhi"))
            {
                if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
                else
                {
                    return new TriggerStruct();
                }
            }
            // if it runs here, it means player own both two skill;
            bool show1 = RoomLogic.PlayerHasShownSkill(room, player, Name);
            bool show2 = RoomLogic.PlayerHasShownSkill(room, player, "yizhi");
            List<string> choices = new List<string>();
            if (!show2 && player.CanShowGeneral("h"))
                choices.Add("head_invoke_only");
            if (!show1 && player.CanShowGeneral("d"))
                choices.Add("deputy_invoke_only");
            if ((!show1 && player.CanShowGeneral("h")) || (!show2 && player.CanShowGeneral("d")))
                choices.Add("show_all_general");

            if (choices.Count > 1)
                choices.Add("cancel");
            else if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                if (show1 || player.CanShowGeneral("h"))
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                else
                    room.BroadcastSkillInvoke("yizhi", player);
                return info;
            }
            else
                return new TriggerStruct();

            string choice = room.AskForChoice(player, "GuanxingShowGeneral", string.Join("+", choices));
            if (choice == "show_all_general")
            {
                room.ShowGeneral(player, true);
                room.ShowGeneral(player, false);
                room.BroadcastSkillInvoke(Name, player);
                return info;
            }
            else if (choice == "deputy_invoke_only")
            {
                room.BroadcastSkillInvoke("yizhi", player, "deputy");
                TriggerStruct trigger = info;
                trigger.SkillPosition = "deputy";
                return trigger;
            }
            else if (choice == "head_invoke_only")
            {
                room.BroadcastSkillInvoke(Name, player);
                return info;
            }
            else
                return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player zhuge, TriggerStruct info)
        {
            List<int> guanxing = room.GetNCards(GetGuanxingNum(room, zhuge), false);

            LogMessage log = new LogMessage
            {
                Type = "$ViewDrawPile",
                From = zhuge.Name,
                Card_str = string.Join("+", JsonUntity.IntList2StringList(guanxing))
            };
            room.SendLog(log, zhuge);
            log.Type = "$ViewDrawPile2";
            log.Arg = guanxing.Count.ToString();
            log.Card_str = null;
            room.SendLog(log, new List<Player> { zhuge });

            string skill_name = Name;
            if (RoomLogic.PlayerHasSkill(room, zhuge, "yizhi") && !zhuge.General1Showed || !RoomLogic.InPlayerHeadSkills(zhuge, Name))
                skill_name = "yizhi";
            room.AskForGuanxing(zhuge, guanxing, skill_name, true, info.SkillPosition);


            return false;
        }
        private int GetGuanxingNum(Room room, Player zhuge)
        {
            if (RoomLogic.InPlayerHeadSkills(zhuge, Name) && zhuge.General1Showed && RoomLogic.PlayerHasShownSkill(room, zhuge, "yizhi")) return 5;
            return Math.Min(5, room.AliveCount());
        }
    }

    public class Kongcheng : TriggerSkill
    {
        public Kongcheng() : base("kongcheng")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirming, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseProceeding };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && player.Phase == PlayerPhase.Draw && player.GetPile(Name).Count > 0)
            {
                CardsMoveStruct move = new CardsMoveStruct(player.GetPile(Name), player, Place.PlaceHand,
                    new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name));
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, false);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use && base.Triggerable(player, room) && player.IsKongcheng())
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard != null && (fcard is Slash || fcard is Duel) && use.To.Contains(player))
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                if (move.To != null && base.Triggerable(move.To, room) && move.To.Phase == PlayerPhase.NotActive && move.To_place == Place.PlaceHand && move.Card_ids.Count > 0
                    && (move.Reason.Reason == MoveReason.S_REASON_GIVE || move.Reason.Reason == MoveReason.S_REASON_PREVIEWGIVE))
                    return new TriggerStruct(Name, move.To);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, ask_who, Name) || room.AskForSkillInvoke(ask_who, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use)
            {
                room.CancelTarget(ref use, player); // Room::cancelTarget(use, player);
                data = use;
                return true;
            }
            else if(triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                room.AddToPile(ask_who, Name, new List<int>(move.Card_ids), false);
            }

            return false;
        }
    }

    public class LongdanVS : OneCardViewAsSkill
    {
        public LongdanVS() : base("longdan")
        {
            response_or_use = true;
        }
        public override bool ViewFilter(Room room, WrappedCard card, Player player)
        {
            switch (room.GetRoomState().GetCurrentCardUseReason())
            {
                case CardUseReason.CARD_USE_REASON_PLAY:
                    return card.Name == Jink.ClassName;
                case CardUseReason.CARD_USE_REASON_RESPONSE:
                case CardUseReason.CARD_USE_REASON_RESPONSE_USE:
                    string pattern = room.GetRoomState().GetCurrentCardUsePattern();
                    pattern = Engine.GetPattern(pattern).GetPatternString();
                    if (pattern.StartsWith(Slash.ClassName))
                        return card.Name == Jink.ClassName;
                    else if (pattern.StartsWith(Jink.ClassName))
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        return fcard is Slash;
                    }
                    break;
                default:
                    return false;
            }

            return false;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return Slash.IsAvailable(room, player);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            pattern = Engine.GetPattern(pattern).GetPatternString();
            return pattern.StartsWith(Jink.ClassName) || pattern.StartsWith(Slash.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard originalCard, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(originalCard.Name);
            if (fcard is Slash)
            {
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                jink.AddSubCard(originalCard);
                jink.Skill = Name;
                jink.ShowSkill = Name;
                jink = RoomLogic.ParseUseCard(room, jink);
                return jink;
            }
            else if (fcard is Jink)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                slash.AddSubCard(originalCard);
                slash.Skill = Name;
                slash.ShowSkill = Name;
                slash = RoomLogic.ParseUseCard(room, slash);
                return slash;
            }
            else
                return null;
        }
    }

    public class Longdan : TriggerSkill
    {
        public Longdan() : base("longdan")
        {
            view_as_skill = new LongdanVS();
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.SlashMissed };
            skill_type = SkillType.Alter;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.SlashMissed && data is SlashEffectStruct effect)
            {
                if (effect.Slash.Skill == Name && base.Triggerable(player, room))
                    return new TriggerStruct(Name, player);
                else if (effect.Jink.Skill == Name && base.Triggerable(effect.To, room))
                    return new TriggerStruct(Name, effect.To);
            }
            else if (base.Triggerable(player, room))
            {
                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "shouyue") && lord.General1Showed)
                {
                    WrappedCard card = null;
                    if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                        card = use.Card;
                    else if (data is CardResponseStruct respond)
                        card = respond.Card;

                    if (card != null && card.Skill == Name)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.SlashMissed && data is SlashEffectStruct effect)
            {
                ask_who.SetTag(Name, player.Name);
                List<Player> targets = new List<Player>();
                string prompt;
                if (player == ask_who)
                {
                    targets = room.GetOtherPlayers(ask_who);
                    targets.Remove(effect.To);
                    prompt = "@longdan-slash";
                }
                else
                {
                    foreach (Player p in room.GetOtherPlayers(ask_who))
                    {
                        if (p != player && p.IsWounded())
                            targets.Add(p);
                    }
                    prompt = "@longdan-jink";
                }

                Player target = room.AskForPlayerChosen(ask_who, targets, Name, prompt, true, true, info.SkillPosition);
                if (target != null)
                {
                    ask_who.SetTag(Name, target.Name);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    room.NotifySkillInvoked(ask_who, Name);
                    return info;
                }
            }
            else
                return info;

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.SlashMissed && data is SlashEffectStruct effect)
            {
                Player target = ask_who.ContainsTag(Name) ? room.FindPlayer((string)ask_who.GetTag(Name)) : null;
                if (target != null)
                {
                    if (player == ask_who)
                        room.Damage(new DamageStruct(Name, ask_who, target));
                    else
                    {
                        RecoverStruct recover = new RecoverStruct
                        {
                            Who = ask_who,
                            Recover = 1
                        };
                        room.Recover(target, recover, true);
                    }
                }
            }
            else
            {
                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                room.NotifySkillInvoked(lord, "shouyue");
                room.BroadcastSkillInvoke("shouyue", lord, "head");
                room.DrawCards(player, new DrawCardStruct(1, lord, "shouyue"));
            }
            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            if (card.Name == Slash.ClassName)
            {
                index = 1;
            }
            else
            {
                index = 2;
            }
        }
    }
    public class Mashu : DistanceSkill
    {

        public Mashu(string owner) : base("mashu_" + owner)
        {
        }
        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null) => RoomLogic.PlayerHasShownSkill(room, from, Name) ? -1 : 0;

    }

    public class Tieqi : TriggerSkill
    {
        public Tieqi() : base("tieqi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.TargetChosen };
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.Players)
                {
                    if (p.GetMark("@tieqi1") > 0) room.SetPlayerMark(p, "@tieqi1", 0);
                    if (p.GetMark("@tieqi2") > 0) room.SetPlayerMark(p, "@tieqi2", 0);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    return new TriggerStruct(Name, player, use.To);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player player, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, skill_target, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player machao, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            DoTieqi(room, skill_target, machao, ref use, info);
            data = use;
            return false;
        }

        private void DoTieqi(Room room, Player target, Player source, ref CardUseStruct use, TriggerStruct info)
        {
            JudgeStruct judge = new JudgeStruct
            {
                Reason = "tieqi",
                Who = source
            };
            target.SetFlags("TieqiTarget"); //for AI
            room.Judge(ref judge);
            target.SetFlags("-TieqiTarget");

            Thread.Sleep(400);

            List<string> choices = new List<string>();
            if (target.General1Showed && target.GetMark("@tieqi1") == 0)
                choices.Add(target.General1);
            if (!string.IsNullOrEmpty(target.General2) && target.General2Showed && target.GetMark("@tieqi2") == 0)
                choices.Add(target.General2);

            bool haslord = false;
            if (choices.Count == 2)
            {
                Player lord = RoomLogic.GetLord(room, source.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "shouyue") && lord.General1Showed)
                {
                    room.SetPlayerMark(target, "@tieqi1", 1);
                    room.SetPlayerMark(target, "@tieqi2", 1);
                    haslord = true;
                    room.NotifySkillInvoked(lord, "shouyue");
                    room.BroadcastSkillInvoke("shouyue", lord, "head");
                }
            }

            if (!haslord && choices.Count > 0)
            {
                string general = room.AskForGeneral(source, choices, null, true, Name, target, false, true);
                if (general == target.General1)
                    room.SetPlayerMark(target, "@tieqi1", 1);
                else
                    room.SetPlayerMark(target, "@tieqi2", 1);
            }

            string suit = WrappedCard.GetSuitString(judge.Card.Suit);
            LogMessage l = new LogMessage
            {
                Type = "#tieqijudge",
                From = source.Name,
                Arg = Name,
                Arg2 = WrappedCard.GetSuitString(judge.Card.Suit)
            };
            room.SendLog(l);
            target.SetTag("tieqi_judge", judge.Card.Id);

            if (room.AskForCard(target, Name,
                string.Format("..{0}", suit.Substring(0, 1).ToUpper()),
                string.Format("@tieqi-discard:::{0}", string.Format("<color={0}>{1}</color>", WrappedCard.IsBlack(judge.Card.Suit) ? "black" : "red", WrappedCard.GetSuitIcon(judge.Card.Suit))),
                judge.Card) == null)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#NoJink",
                    From = target.Name
                };
                room.SendLog(log);

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, source, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                int index = 0;
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (effect.To == target)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            effect.Effect2 = 0;
                            break;
                        }
                    }
                }

                Thread.Sleep(500);
            }
            else {
                LogMessage log = new LogMessage
                {
                    Type = "#tieqidis",
                    From = target.Name,
                    Arg = Name,
                    Arg2 = suit
                };
                room.SendLog(log);
           }
        }
    }

    public class TieqiInvalid : InvalidSkill
    {
        public TieqiInvalid() : base("#tieqi-invalid")
        {
        }

        public override bool Invalid(Room room, Player player, string skill)
        {
            Skill s = Engine.GetSkill(skill);
            if (s == null || s.SkillFrequency == Frequency.Compulsory || s.Attached_lord_skill) return false;
            if (player.HasEquip(skill)) return false;
            if (player.GetMark("@tieqi1") > 0)
            {
                if (player.HeadSkills.ContainsKey(skill) && (!player.DeputySkills.ContainsKey(skill) || player.GetMark("@tieqi2") > 0))
                    return true;
            }
            if (player.GetMark("@tieqi2") > 0 && player.DeputySkills.ContainsKey(skill) && !player.HeadSkills.ContainsKey(skill))
                return true;

            return false;
        }
    }

    public class Jizhi : TriggerSkill
    {
        public Jizhi() : base("jizhi")
        {
            frequency = Frequency.Frequent;
            events.Add(TriggerEvent.CardUsed);
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.IsNDTrick())
                {
                    if (!use.Card.IsVirtualCard() || use.Card.SubCards.Count == 0)
                        return new TriggerStruct(Name, player);
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
            room.DrawCards(player, 1, Name);
            return false;
        }
    }

    public class Qicai : TargetModSkill
    {
        public Qicai() : base("qicai")
        {
            pattern = "TrickCard";
            skill_type = SkillType.Wizzard;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return RoomLogic.PlayerHasSkill(room, from, Name) ? true : false;
        }
    }

    public class Liegong : TriggerSkill
    {
        public Liegong() : base("liegong")
        {
            events.Add(TriggerEvent.TargetChosen);
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Play && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player to in use.To) {
                        int handcard_num = to.HandcardNum;
                        if (handcard_num >= player.Hp || handcard_num <= RoomLogic.GetAttackRange(room, player))
                            targets.Add(to);
                    }
                    if (targets.Count > 0)
                        return new TriggerStruct(Name, player, targets);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player player, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, skill_target, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;

            LogMessage log = new LogMessage
            {
                Type = "#NoJink",
                From = target.Name
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
                        effect.Effect2 = 0;
                        data = use;
                        break;
                    }
                }
            }

            return false;
        }
    }

    public class LiegongRange : AttackRangeSkill
    {
        public LiegongRange() : base("#liegong-for-lord")
        {
        }
        public override int GetExtra(Room room, Player target, bool include_weapon)
        {
            if (RoomLogic.PlayerHasShownSkill(room, target, "liegong"))
            {
                Player lord = RoomLogic.GetLord(room, target.Kingdom);

                if (lord != null && lord.HasShownSkill("shouyue", true))
                {
                    return 1;
                }
            }
            return 0;
        }
    };

    public class Kuanggu : TriggerSkill
    {
        public Kuanggu() : base("kuanggu")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.PreDamageDone };
            skill_type = SkillType.Recover;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player != null && triggerEvent == TriggerEvent.PreDamageDone && data is DamageStruct damage)
            {
                Player weiyan = damage.From;
                if (weiyan != null)
                {
                    if (RoomLogic.DistanceTo(room, weiyan, damage.To) != -1 && RoomLogic.DistanceTo(room, weiyan, damage.To) <= 1)
                        weiyan.SetTag("InvokeKuanggu", damage.Damage);
                    else
                        weiyan.RemoveTag("InvokeKuanggu");
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.Damage && data is DamageStruct damage)
            {
                int recorded_damage = player.ContainsTag("InvokeKuanggu") ?(int)player.GetTag("InvokeKuanggu") : 0;
                if (recorded_damage > 0)
                {
                    TriggerStruct skill_list = new TriggerStruct(Name, player)
                    {
                        Times = damage.Damage
                    };
                    return skill_list;
                }
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
            List<string> choices = new List<string> { "draw" };
            if (player.IsWounded())
                choices.Add("recover");
            string result = room.AskForChoice(player, Name, string.Join("+", choices), null);
            if (result == "draw")
                room.DrawCards(player, 1, Name);
            else
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(player, recover, true);
            }

            return false;
        }
    }

    public class Lianhuan : OneCardViewAsSkill
    {
        public Lianhuan() : base("lianhuan")
        {
            filter_pattern = ".|club|.|hand";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard originalCard, Player player)
        {
            WrappedCard chain = new WrappedCard(IronChain.ClassName);
            chain.AddSubCard(originalCard);
            chain.Skill = Name;
            chain.ShowSkill = Name;
            chain.CanRecast = true;
            chain = RoomLogic.ParseUseCard(room, chain);
            return chain;
        }
    }

    public class Niepan : TriggerSkill
    {
        public Niepan() : base("niepan")
        {
            events.Add(TriggerEvent.AskForPeaches);
            frequency = Frequency.Limited;
            limit_mark = "@nirvana";
            skill_type = SkillType.Recover;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            if (data is DyingStruct dying && dying.Who == target && target.Hp <= 0
                && base.Triggerable(target, room) && target.GetMark("@nirvana") > 0)
                return new TriggerStruct(Name, target);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player pangtong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(pangtong, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, pangtong, info.SkillPosition);
                room.DoSuperLightbox(pangtong, info.SkillPosition, Name);
                room.SetPlayerMark(pangtong, "@nirvana", 0);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player pangtong, ref object data, Player ask_who, TriggerStruct info)
        {
            room.ThrowAllCards(pangtong);
            RecoverStruct recover = new RecoverStruct
            {
                Recover = Math.Min(3, pangtong.MaxHp) - pangtong.Hp
            };
            room.Recover(pangtong, recover);

            room.DrawCards(pangtong, 3, Name);

            if (pangtong.Chained)
                room.SetPlayerChained(pangtong, false);

            if (!pangtong.FaceUp)
                room.TurnOver(pangtong);

            return false; //return pangtong.Hp > 0 || pangtong.isDead();
        }
    }

    public class Huoji : OneCardViewAsSkill
    {
        public Huoji() : base("huoji")
        {
            filter_pattern = ".|red|.|hand";
            response_or_use = true;
            skill_type = SkillType.Attack;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard fire_attack = new WrappedCard(FireAttack.ClassName);
            fire_attack.AddSubCard(card);
            fire_attack.Skill = Name;
            fire_attack.ShowSkill = Name;
            fire_attack = RoomLogic.ParseUseCard(room, fire_attack);
            return fire_attack;
        }

    }

    public class Bazhen : TriggerSkill
    {
        public Bazhen() : base("bazhen")
        {
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetEmotion(player, "eightdiagram");
            JudgeStruct judge = new JudgeStruct
            {
                Pattern = ".|red",
                Good = true,
                Reason = EightDiagram.ClassName,
                Who = player
            };

            room.Judge(ref judge);
            Thread.Sleep(400);
            if (judge.IsGood())
            {
                WrappedCard jink = new WrappedCard(Jink.ClassName)
                {
                    Skill = Name,
                    SkillPosition = info.SkillPosition
                };
                room.Provide(jink);

                return true;
            }

            return false;
        }
    }

    public class BazhenVH : ViewHasSkill
    {
        public BazhenVH() : base("#bazhenvh")
        {
            viewhas_armors.Add(EightDiagram.ClassName);
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (player.Alive && RoomLogic.PlayerHasSkill(room, player, "bazhen") && !player.GetArmor())
                return true;
            return false;
        }
    }

    public class Kanpo : OneCardViewAsSkill
    {
        public Kanpo() : base("kanpo")
        {
            filter_pattern = ".|black|.|hand";
            response_or_use = true;
            response_pattern = Nullification.ClassName;
            skill_type = SkillType.Alter;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ncard = new WrappedCard(Nullification.ClassName);
            ncard.AddSubCard(card);
            ncard.Skill = Name;
            ncard.ShowSkill = Name;
            ncard = RoomLogic.ParseUseCard(room, ncard);
            return ncard;
        }
        public override bool IsEnabledAtNullification(Room room, Player player)
        {
            List<WrappedCard> handlist = RoomLogic.GetPlayerHandcards(room, player);
            foreach (int id in player.GetHandPile())
            {
                WrappedCard ca = room.GetCard(id);
                handlist.Add(ca);
            }
            foreach (WrappedCard ca in handlist)
            {
                if (WrappedCard.IsBlack(ca.Suit))
                    return true;
            }
            return false;
        }
    };

    public class SavageAssaultAvoid : TriggerSkill
    {
        private string avoid_skill;
        public SavageAssaultAvoid(string avoid_skill) : base("#sa_avoid_" + avoid_skill)
        {
            this.avoid_skill = avoid_skill;
            events.Add(TriggerEvent.CardEffected);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && RoomLogic.PlayerHasSkill(room, player, avoid_skill) && data is CardEffectStruct effect && effect.Card.Name == SavageAssault.ClassName)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, avoid_skill)
              || room.AskForSkillInvoke(player, avoid_skill, data, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, avoid_skill, info.SkillPosition);
                room.BroadcastSkillInvoke(avoid_skill, "male", 1, gsk.General, gsk.SkinId);
                room.ShowSkill(player, avoid_skill, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.NotifySkillInvoked(player, avoid_skill);

            LogMessage log = new LogMessage
            {
                Type = "#Skillnullify",
                From = player.Name,
                Arg = avoid_skill,
                Arg2 = SavageAssault.ClassName
            };
            room.SendLog(log);

            return true;
        }
    }

    public class Huoshou : TriggerSkill
    {
        public Huoshou() : base("huoshou")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.ConfirmDamage, TriggerEvent.CardFinished };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.ConfirmDamage && room.ContainsTag("HuoshouSource") && data is DamageStruct damage) {
                if (damage.Card == null || damage.Card.Name != SavageAssault.ClassName)
                    return;

                Player menghuo = (Player)room.GetTag("HuoshouSource");
                damage.From = menghuo.Alive ? menghuo : null;
                data = damage;
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player == null) return skill_list;
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && !room.ContainsTag("HuoshouSource"))
            {
                if (use.Card.Name == SavageAssault.ClassName)
                {
                    List<Player> menghuos = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player menghuo in menghuos)
                    {
                        if (use.From != menghuo)
                        {
                            skill_list.Add(new TriggerStruct(Name, menghuo));
                            return skill_list;
                        }
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use2)
            {
                if (use2.Card.Name == SavageAssault.ClassName)
                    room.RemoveTag("HuoshouSource");
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = !room.ContainsTag("HuoshouSource") && (RoomLogic.PlayerHasShownSkill(room, ask_who, Name)
                || room.AskForSkillInvoke(ask_who, Name,null, info.SkillPosition));
            if (invoke)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.SetTag("HuoshouSource", ask_who);

            return false;
        }
    }

    public class Zaiqi : PhaseChangeSkill
    {
        public Zaiqi() : base("zaiqi")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Recover;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player menghuo, ref object data, Player ask_who)
        {
            if (base.Triggerable(menghuo, room) && menghuo.Phase == PlayerPhase.Draw && menghuo.IsWounded())
                return new TriggerStruct(Name, menghuo);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player menghuo, TriggerStruct info)
        {
            bool has_heart = false;
            int x = menghuo.GetLostHp();
            List<int> ids = room.GetNCards(x);
            CardsMoveStruct move = new CardsMoveStruct(ids, menghuo, Place.PlaceTable,
                new CardMoveReason(MoveReason.S_REASON_TURNOVER, menghuo.Name, "zaiqi", null));
            room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);

            Thread.Sleep(1000);

            List<int> card_to_throw = new List<int>();
            List<int> card_to_gotback = new List<int>();
            for (int i = 0; i < x; i++)
            {
                if (room.GetCard(ids[i]).Suit == WrappedCard.CardSuit.Heart)
                    card_to_throw.Add(ids[i]);
                else
                    card_to_gotback.Add(ids[i]);
            }
            if (card_to_throw.Count > 0)
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = menghuo,
                    Recover = card_to_throw.Count
                };
                room.Recover(menghuo, recover, true);

                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, menghuo.Name, "zaiqi", null);
                room.ThrowCard(ref card_to_throw, reason, null);
                has_heart = true;
            }
            if (card_to_gotback.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, menghuo.Name);
                room.ObtainCard(menghuo, ref card_to_gotback, reason);
            }

            if (has_heart)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, menghuo, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
            }

            return true;
        }
       
    }

    public class Juxiang : TriggerSkill
    {
        public Juxiang() : base("juxiang")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<Player> zhurongs = RoomLogic.FindPlayersBySkillName(room, Name);
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (zhurongs.Count > 0 && data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceTable) && move.To_place == Place.DiscardPile
                && move.Reason.Reason == MoveReason.S_REASON_USE && move.Reason.Card != null && move.Reason.Card.Name == SavageAssault.ClassName && move.Card_ids.Count > 0)
            {
                List<int> ids = room.GetSubCards(move.Reason.Card);
                if (ids.Count > 0 && ids.SequenceEqual(move.Card_ids) && ids.SequenceEqual(move.Reason.Card.SubCards))
                {
                    bool check = true;
                    foreach (int id in move.Card_ids)
                       if (room.GetCardPlace(id) != Place.DiscardPile)
                        check = false;

                    if (check)
                    {
                        foreach (Player zhurong in zhurongs)
                            if (move.From != zhurong)
                                triggers.Add(new TriggerStruct(Name, zhurong));
                    }
                }
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                List<int> ids = room.GetSubCards(move.Reason.Card);
                if (ids.Count > 0 && ids.SequenceEqual(move.Card_ids) && ids.SequenceEqual(move.Reason.Card.SubCards))
                {
                    bool check = true;
                    foreach (int id in move.Card_ids)
                        if (room.GetCardPlace(id) != Place.DiscardPile)
                            check = false;

                    bool invoke = check && (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, null, info.SkillPosition));
                    if (invoke)
                    {
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                        room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            room.SendCompulsoryTriggerLog(player, Name);
            List<int> ids = new List<int>(move.Card_ids);
            room.RemoveSubCards(move.Reason.Card);
            room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, player.Name, Name, string.Empty));

            return false;
        }
    }

    public class Lieren : TriggerSkill
    {
        public Lieren() : base("lieren")
        {
            events.Add(TriggerEvent.Damage);
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who)
        {
            if (base.Triggerable(zhurong, room) && data is DamageStruct damage && damage.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && !zhurong.IsKongcheng() && !damage.To.IsKongcheng() && damage.To != zhurong && !damage.Chain && !damage.Transfer
                    && !damage.To.HasFlag("Global_DFDebut") && RoomLogic.CanBePindianBy(room, damage.To, zhurong))
                    return new TriggerStruct(Name, zhurong);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && room.AskForSkillInvoke(zhurong, Name, damage.To, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, zhurong.Name, damage.To.Name);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, zhurong, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                PindianStruct pd = room.PindianSelect(zhurong, damage.To, Name);
                room.SetTag("lieren_pd", pd);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who, TriggerStruct info)
        {
            PindianStruct pd = (PindianStruct)room.GetTag("lieren_pd");
            room.RemoveTag("lieren_pd");
            Player target = pd.Tos[0];
            room.Pindian(ref pd);
            if (!pd.Success) return false;

            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, zhurong, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
            if (RoomLogic.CanGetCard(room, zhurong, target, "he"))
            {
                int card_id = room.AskForCardChosen(zhurong, target, "he", Name, false, HandlingMethod.MethodGet);
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, zhurong.Name);
                room.ObtainCard(zhurong, room.GetCard(card_id), reason, room.GetCardPlace(card_id) != Place.PlaceHand);
            }

            return false;
        }
    }

    public class Xiangle : TriggerSkill
    {
        public Xiangle() : base("xiangle")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.To.Contains(player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.SetEmotion(player, "xiangle");
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                Thread.Sleep(400);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player liushan, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(liushan, Name);
            CardUseStruct use = (CardUseStruct)data;

            int index = 0, i;
            for (i = 0; i < use.EffectCount.Count; i++)
            {
                CardBasicEffect effect = use.EffectCount[i];
                if (effect.To == liushan)
                {
                    index++;
                    if (index == info.Times)
                    {
                        use.From.SetTag(Name, i);
                        break;
                    }
                }
            }

            if (room.AskForCard(use.From, Name, ".Basic", "@xiangle-discard:" + liushan.Name, data) == null)
            {
                CardBasicEffect effect = use.EffectCount[i];
                effect.Nullified = true;

                data = use;
            }

            use.From.RemoveTag(Name);

            return false;
        }
    }

    public class FangquanCard : SkillCard
    {
        public static string ClassName = "FangquanCard";
        public FangquanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.To;

            ResultStruct result = effect.From.Result;           //放权辅助+3
            result.Assist += 3;
            effect.From.Result = result;

            LogMessage log = new LogMessage
            {
                Type = "#Fangquan",
                To = new List<string> { player.Name }
            };
            room.SendLog(log);

            room.GainAnExtraTurn(player);
        }
    }

    public class FangquanViewAsSkill : OneCardViewAsSkill
    {
        public FangquanViewAsSkill() : base("fangquan")
        {
            filter_pattern = ".|.|.|hand!";
            response_pattern = "@@fangquan";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard fangquan = new WrappedCard("FangquanCard");
            fangquan.AddSubCard(card);
            fangquan.Skill = Name;
            return fangquan;
        }
    }

    public class Fangquan : TriggerSkill
    {
        public Fangquan() : base("fangquan")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new FangquanViewAsSkill();
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is PhaseChangeStruct change)
            {
                if (base.Triggerable(player, room) && change.To == PlayerPhase.Play && !player.IsSkipped(PlayerPhase.Play))
                {
                    return new TriggerStruct(Name, player);
                }
                else if (change.To == PlayerPhase.NotActive && player.Alive && player.HasFlag(Name) && RoomLogic.CanDiscard(room, player, player, "h"))
                {
                    TriggerStruct trigger = new TriggerStruct(Name, player)
                    {
                        SkillPosition = (string)player.GetTag(Name)
                    };
                    return trigger;
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            if (change.To == PlayerPhase.Play)
            {
                if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                {
                    room.SkipPhase(player, PlayerPhase.Play);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    return info;
                }
            }
            else if (change.To == PlayerPhase.NotActive)
                room.AskForUseCard(player, "@@fangquan", "@fangquan-discard", null, -1, HandlingMethod.MethodDiscard, true, info.SkillPosition);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player liushan, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            if (change.To == PlayerPhase.Play)
            {
                liushan.SetTag(Name, info.SkillPosition);
                liushan.SetFlags(Name);
            }
            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 2;
        }
    }

    public class Shushen : TriggerSkill
    {
        public Shushen() : base("shushen")
        {
            events.Add(TriggerEvent.HpRecover);
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is RecoverStruct recover)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    Times = recover.Recover
                };
                return trigger;
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "shushen-invoke", true, true, info.SkillPosition);
            if (target != null)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

                List<string> target_list = player.ContainsTag("shushen_target") ? (List<string>)player.GetTag("shushen_target") : new List<string>();
                target_list.Add(target.Name);
                player.SetTag("shushen_target", target_list);

                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> target_list = (List<string>)player.GetTag("shushen_target");
            string target_name = target_list[target_list.Count - 1];
            target_list.RemoveAt(target_list.Count - 1);
            player.SetTag("shushen_target", target_list);

            Player to = room.FindPlayer(target_name);
            if (to != null)
                room.DrawCards(to, new DrawCardStruct(1, player, Name));
            return false;
        }
    }

    public class Shenzhi : PhaseChangeSkill
    {
        public Shenzhi() : base("shenzhi")
        {
            skill_type = SkillType.Recover;
            frequency = Frequency.Frequent;
            //This skill can't be frequent in game actually.
            //because the frequency = Frequent has no effect in UI currently, we use this to reduce the AI delay
        }
        public override bool CanPreShow() => false;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Start && !player.IsKongcheng()
                ? new TriggerStruct(Name, player)
                : new TriggerStruct();
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
        public override bool OnPhaseChange(Room room, Player ganfuren, TriggerStruct info)
        {
            int handcard_num = 0;
            foreach (WrappedCard card in RoomLogic.GetPlayerHandcards(room, ganfuren)) {
                if (!RoomLogic.IsJilei(room, ganfuren, card))
                    handcard_num++;
            }
            room.ThrowAllHandCards(ganfuren);
            if (handcard_num >= ganfuren.Hp)
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = ganfuren
                };
                room.Recover(ganfuren, recover, true);
            }
            return false;
        }
    }
    #endregion

    #region 群

    public class Jijiu : OneCardViewAsSkill
    {
        public Jijiu() : base("jijiu")
        {
            filter_pattern = ".|red";
            response_or_use = true;
            skill_type = SkillType.Recover;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            WrappedCard peach = new WrappedCard(Peach.ClassName);
            FunctionCard fcard = Peach.Instance;
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE && player.Phase == PlayerPhase.NotActive
                    && fcard.IsAvailable(room, player, peach))
            {
                return Engine.MatchExpPattern(room, pattern, player, peach);
            }

            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard peach = new WrappedCard(Peach.ClassName);
            peach.AddSubCard(card);
            peach.Skill = Name;
            peach.ShowSkill = Name;
            peach = RoomLogic.ParseUseCard(room, peach);
            return peach;
        }
    }
    public class ChuliCard : SkillCard
    {
        public ChuliCard() : base("ChuliCard")
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 2 || to_select == Self || to_select.IsNude() || !RoomLogic.CanDiscard(room, Self, to_select, "he"))
                return false;

            List<string> kingdoms = new List<string>();
            foreach (Player player in targets)
                if (player.HasShownOneGeneral() && player.GetRoleEnum() != PlayerRole.Careerist)
                    kingdoms.Add(player.Kingdom);

            return !to_select.HasShownOneGeneral() || to_select.GetRoleEnum() == PlayerRole.Careerist || !kingdoms.Contains(to_select.Kingdom);
        }
        public override void Use(Room room, CardUseStruct use)
        {
            List<Player> targets = new List<Player>(use.To), draws = new List<Player>();
            targets.Insert(0, use.From);
            foreach (Player p in targets)
            {
                if (!p.IsNude() && RoomLogic.CanDiscard(room, use.From, p, "he"))
                {
                    int id = -1;
                    if (p == use.From)
                    {
                        List<int> ex = room.AskForExchange(use.From, "chuli", 1, 1, "@chuli", null, "..!", use.Card.SkillPosition);
                        if (ex.Count == 1)
                            id = ex[0];
                        else
                        {
                            foreach (int card_id in use.From.GetCards("he"))
                            {
                                if (RoomLogic.CanDiscard(room, use.From, use.From, card_id))
                                {
                                    id = card_id;
                                    break;
                                }
                            }
                        }
                    }
                    else
                        id = room.AskForCardChosen(use.From, p, "he", "chuli", false, HandlingMethod.MethodDiscard);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISMANTLE, use.From.Name, p.Name, "chuli", string.Empty)
                    {
                        General = RoomLogic.GetGeneralSkin(room, use.From, Name, use.Card.SkillPosition)
                    };
                    List<int> ids = new List<int> { id };
                    if (p == use.From)
                    {
                        CardMoveReason _reason = new CardMoveReason(MoveReason.S_REASON_THROW, use.From.Name, "chuli", string.Empty)
                        {
                            General = RoomLogic.GetGeneralSkin(room, use.From, Name, use.Card.SkillPosition)
                        };
                        room.ThrowCard(ref ids, _reason, p, null);
                    }
                    else
                        room.ThrowCard(ref ids, reason, p, use.From);

                    //判断进入弃牌堆卡牌的花色在下方触发技
                    string key = string.Format("chuli_{0}", p.Name);
                    if (ids.Count > 0 && use.From.HasFlag(key))
                    {
                        use.From.SetFlags("-" + key);
                        draws.Add(p);
                    }
                }
            }
            foreach (Player p in draws)
                room.DrawCards(p, new DrawCardStruct(1, use.From, "chuli"));
        }
    }

    public class Chuli : TriggerSkill
    {
        public Chuli() : base("chuli")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            view_as_skill = new ChuliVS();
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && move.To_place == Place.DiscardPile
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD
                && move.Reason.SkillName == Name && move.Card_ids.Count > 0)
            {
                Player huatuo = room.FindPlayer(move.Reason.PlayerId);
                WrappedCard.CardSuit suit = room.GetCard(move.Card_ids[0]).Suit;
                if (suit == WrappedCard.CardSuit.Spade) huatuo.SetFlags(string.Format("{0}_{1}", Name, move.From.Name));
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class ChuliVS : ZeroCardViewAsSkill
    {
        public ChuliVS() : base("chuli")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("ChuliCard");
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("ChuliCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            return card;
        }
    }
    public class Wushuang : TriggerSkill
    {
        public Wushuang() : base("wushuang")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.TargetConfirmed };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (triggerEvent == TriggerEvent.TargetChosen && use.Card != null && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || fcard is Duel)
                {
                    if (use.To.Count > 0)
                        return new TriggerStruct(Name, player, use.To);
                }
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && use.Card != null && use.Card.Name == Duel.ClassName && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player, new List<Player> { use.From });
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetTag("WushuangData", data); // for AI
            room.SetTag("WushuangTarget", target); // for AI
            bool invoke = RoomLogic.PlayerHasShownSkill(room, ask_who, Name) || room.AskForSkillInvoke(ask_who, Name, target, info.SkillPosition);
            room.RemoveTag("WushuangData");
            room.RemoveTag("WushuangTarget");
            if (invoke)
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            CardUseStruct use = (CardUseStruct)data;
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            if (fcard is Slash || !use.To.Contains(ask_who))
            {
                if (triggerEvent != TriggerEvent.TargetChosen)
                    return false;

                int index = 0;
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (effect.To == target)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            effect.Effect2 = 2;
                            break;
                        }
                    }
                }
            }
            else if (fcard is Duel && use.To.Contains(ask_who))
            {
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (effect.To == ask_who && !effect.Triggered)
                    {
                        effect.Effect3 = 2;
                        break;
                    }
                }
            }

            data = use;

            return false;
        }
    }
    public class LijianCard : SkillCard
    {
        public LijianCard() : base("LijianCard")
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (!to_select.IsMale())
                return false;

            WrappedCard duel = new WrappedCard(Duel.ClassName);
            if (targets.Count == 1 && (RoomLogic.IsCardLimited(room, to_select, duel, HandlingMethod.MethodUse)
                || RoomLogic.IsProhibited(room, to_select, targets[0], duel) != null))
                return false;

            return targets.Count < 2 && to_select != Self;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 2;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player diaochan = card_use.From;

            object data = card_use;
            RoomThread thread = room.RoomThread;

            thread.Trigger(TriggerEvent.PreCardUsed, room, diaochan, ref data);
            room.BroadcastSkillInvoke("lijian", diaochan, card_use.Card.SkillPosition);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_THROW, diaochan.Name, null, "lijian", null)
            {
                General = RoomLogic.GetGeneralSkin(room, diaochan, Name, card_use.Card.SkillPosition)
            };
            room.RecordSubCards(card_use.Card);
            room.MoveCardTo(card_use.Card, diaochan, null, Place.PlaceTable, reason, true);

            LogMessage log = new LogMessage
            {
                From = diaochan.Name,
                To = new List<string>(),
                Type = "#UseCard",
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            foreach (Player p in card_use.To)
                log.To.Add(p.Name);
            room.SendLog(log);

            List<int> table_ids = room.GetCardIdsOnTable(room.GetSubCards(card_use.Card));
            if (table_ids.Count > 0)
            {
                CardsMoveStruct move = new CardsMoveStruct(table_ids, diaochan, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
                room.RemoveSubCards(card_use.Card);
            }
            thread.Trigger(TriggerEvent.CardUsedAnnounced, room, diaochan, ref data);
            thread.Trigger(TriggerEvent.CardTargetAnnounced, room, diaochan, ref data);
            thread.Trigger(TriggerEvent.CardUsed, room, diaochan, ref data);
            thread.Trigger(TriggerEvent.CardFinished, room, diaochan, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player to = card_use.To[0];
            Player from = card_use.To[1];

            WrappedCard duel = new WrappedCard(Duel.ClassName)
            {
                Skill = "_lijian"
            };
            if (room.Setting.GameMode != "Hegemony") duel.Cancelable = false;
            if (!RoomLogic.IsCardLimited(room, from, duel, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, from, to, duel) == null)
                room.UseCard(new CardUseStruct(duel, from, to));
        }
    }
    public class Lijian : OneCardViewAsSkill
    {
        public Lijian() : base("lijian")
        {
            filter_pattern = "..!";
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return room.GetAlivePlayers().Count > 2
                && RoomLogic.CanDiscard(room, player, player, "he") && !player.HasUsed("LijianCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard lijian_card = new WrappedCard("LijianCard")
            {
                Mute = true
            };
            lijian_card.AddSubCard(card);
            lijian_card.Skill = Name;
            lijian_card.ShowSkill = Name;
            return lijian_card;
        }
    }
    public class Biyue : PhaseChangeSkill
    {
        public Biyue() : base("biyue")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            if (player.Phase == PlayerPhase.Finish) return new TriggerStruct(Name, player);
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
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.DrawCards(player, 1 , Name);

            return false;
        }
    }
    public class LuanjiVS : ViewAsSkill
    {
        public LuanjiVS() : base("luanji")
        {
            response_or_use = true;
            skill_type = SkillType.Attack;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (player.HasEquip(to_select.Name))
                return false;
            else
                return !player.ContainsTag(Name) || !((List<int>)player.GetTag(Name)).Contains((int)to_select.Suit);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 2)
            {
                WrappedCard aa = new WrappedCard(ArcheryAttack.ClassName);
                aa.AddSubCards(cards);
                aa.Skill = Name;
                aa.ShowSkill = Name;
                aa = RoomLogic.ParseUseCard(room, aa);
                return aa;
            }
            else
                return null;
        }
    }
    public class Luanji : TriggerSkill
    {
        public Luanji() : base("luanji")
        {
            view_as_skill = new LuanjiVS();
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseChanging, TriggerEvent.CardResponded };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card != null
                && use.Card.Name == ArcheryAttack.ClassName && use.Card.Skill == Name && use.Card.SubCards.Count > 0)
            {
                List<int> suits = player.ContainsTag(Name) ? (List<int>)player.GetTag(Name) : new List<int>();
                foreach (int id in use.Card.SubCards)
                    suits.Add((int)room.GetCard(id).Suit);
                player.SetTag(Name, suits);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Reason == ArcheryAttack.ClassName && resp.Data is CardEffectStruct effect
                && effect.Card.Skill == Name && player.Alive && RoomLogic.IsFriendWith(room, player, effect.From))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && room.AskForSkillInvoke(player, "luanji-draw", "#luanji-draw"))
            {
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, 1, Name);
            return false;
        }
    }
    public class ShuangxiongViewAsSkill : OneCardViewAsSkill
    {
        public ShuangxiongViewAsSkill() : base("shuangxiong")
        {
            response_or_use = true;
        }
        public override bool IsAvailable(Room room, Player player, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_PLAY && player.HasFlag("shuangxiong_" + position) && player.GetMark("shuangxiong") != 0;
        }
        public override bool ViewFilter(Room room, WrappedCard card, Player player)
        {
            if (player.HasEquip(card.Name))
                return false;

            int value = player.GetMark("shuangxiong");
            if (value == 1)
                return WrappedCard.IsBlack(card.Suit);
            else if (value == 2)
                return WrappedCard.IsRed(card.Suit);

            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard duel = new WrappedCard(Duel.ClassName);
            duel.AddSubCard(card);
            duel.Skill = Name;
            duel.ShowSkill = Name;
            duel = RoomLogic.ParseUseCard(room, duel);
            return duel;
        }
    }
    public class Shuangxiong : TriggerSkill
    {
        public Shuangxiong() : base("shuangxiong")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            view_as_skill = new ShuangxiongViewAsSkill();
            skill_type = SkillType.Attack;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null)
            {
                if (triggerEvent == TriggerEvent.EventPhaseStart)
                {
                    if (player.Phase == PlayerPhase.Start)
                    {
                        player.SetMark("shuangxiong", 0);
                        return new TriggerStruct();
                    }
                    else if (player.Phase == PlayerPhase.Draw && base.Triggerable(player, room))
                        return new TriggerStruct(Name, player);
                }
                else if (triggerEvent == TriggerEvent.EventPhaseChanging)
                {
                    PhaseChangeStruct change = (PhaseChangeStruct)data;
                    if (change.To == PlayerPhase.NotActive && player.HasFlag("shuangxiong_head"))
                        room.SetPlayerFlag(player, "-shuangxiong_head");
                    if (change.To == PlayerPhase.NotActive && player.HasFlag("shuangxiong_deputy"))
                        room.SetPlayerFlag(player, "-shuangxiong_deputy");
                    return new TriggerStruct();
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player shuangxiong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(shuangxiong, Name, null, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, shuangxiong, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player shuangxiong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (shuangxiong.Phase == PlayerPhase.Draw && base.Triggerable(shuangxiong, room))
            {
                room.SetPlayerFlag(shuangxiong, "shuangxiong_" + info.SkillPosition);
                shuangxiong.SetTag("shuangxiong", info.SkillPosition);
                JudgeStruct judge = new JudgeStruct
                {
                    Good = true,
                    PlayAnimation = false,
                    Reason = Name,
                    Who = shuangxiong
                };

                room.Judge(ref judge);
                shuangxiong.SetMark("shuangxiong", WrappedCard.IsRed(judge.JudgeSuit) ? 1 : 2);

                return true;
            }

            return false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 2;
        }
    }

    public class ShuangxiongVH : ViewHasSkill
    {
        public ShuangxiongVH() : base("#shuangxiong-viewhas") { viewhas_skills.Add("shuangxiong"); }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (skill_name == "shuangxiong" && (player.HasFlag("shuangxiong_head") || player.HasFlag("shuangxiong_deputy")) && player.GetMark("shuangxiong") != 0)
                return true;

            return false;
        }
    }

    public class ShuangxiongGet : TriggerSkill
    {
        public ShuangxiongGet() : base("#shuangxiong-get")
        {
            events.Add(TriggerEvent.FinishJudge);
            frequency = Frequency.Compulsory;
        }
        public override bool CanPreShow() => false;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && data is JudgeStruct judge)
            {
                if (judge.Reason == "shuangxiong")
                {
                    if (room.GetCardPlace(judge.Card.GetEffectiveId()) == Place.PlaceJudge)
                    {
                        string head = (string)player.GetTag("shuangxiong");
                        TriggerStruct trigger = new TriggerStruct(Name, player)
                        {
                            SkillPosition = head
                        };
                        return trigger;
                    }
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            room.ObtainCard(judge.Who, judge.Card);

            return false;
        }
    }
    public class Wansha : TriggerSkill
    {
        public Wansha() : base("wansha")
        {
            events = new List<TriggerEvent> { TriggerEvent.Dying };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            Player jiayu = room.Current;
            if (triggerEvent == TriggerEvent.Dying && jiayu != null && base.Triggerable(jiayu, room) && jiayu.Phase != PlayerPhase.NotActive)
                return new TriggerStruct(Name, jiayu);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player jiaxu, TriggerStruct info)
        {

            DyingStruct dying = (DyingStruct)data;
            room.NotifySkillInvoked(jiaxu, Name);

            LogMessage log = new LogMessage
            {
                From = jiaxu.Name,
                Arg = Name
            };
            if (jiaxu != dying.Who)
            {
                log.Type = "#WanshaTwo";
                log.To = new List<string> { dying.Who.Name };
            }
            else
            {
                log.Type = "#WanshaOne";
            }
            room.SendLog(log);
            return false;
        }
    }
    public class WanshaProhibit : ProhibitSkill
    {
        public WanshaProhibit() : base("#wansha-prohibit")
        {
        }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (card.Name == Peach.ClassName && room.Current != null && room.Current.Alive && room.Current.Phase != PlayerPhase.NotActive
                && RoomLogic.PlayerHasShownSkill(room, room.Current, "wansha")
                && room.Current != from && !from.HasFlag("Global_Dying"))
                return true;

            return false;
        }
    }
    public class LuanwuCard : SkillCard
    {
        public LuanwuCard() : base("LuanwuCard")
        {
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@chaos", 0);
            room.BroadcastSkillInvoke("luanwu", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "luanwu");
            card_use.To = room.GetOtherPlayers(card_use.From);
            room.SortByActionOrder(ref card_use);
            base.OnUse(room, card_use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            List<Player> players = room.GetOtherPlayers(effect.To);
            List<int> distance_list = new List<int>();
            int nearest = 1000;
            foreach (Player player in players)
            {
                int distance = RoomLogic.DistanceTo(room, effect.To, player);
                distance_list.Add(distance);
                if (distance != -1)
                    nearest = Math.Min(nearest, distance);
            }

            List<Player> luanwu_targets = new List<Player>();
            for (int i = 0; i < distance_list.Count; i++)
            {
                if (distance_list[i] == nearest && RoomLogic.CanSlash(room, effect.To, players[i]))
                    luanwu_targets.Add(players[i]);
            }

            if (luanwu_targets.Count == 0 || room.AskForUseSlashTo(effect.To, luanwu_targets, "@luanwu-slash", null) == null)
            {
                room.LoseHp(effect.To);
                Thread.Sleep(500);
            }
        }
    }
    public class Luanwu : ZeroCardViewAsSkill
    {
        public Luanwu() : base("luanwu")
        {
            frequency = Frequency.Limited;
            limit_mark = "@chaos";
            skill_type = SkillType.Wizzard;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("LuanwuCard")
            {
                ShowSkill = Name,
                Skill = Name,
                Mute = true
            };
            return card;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => player.GetMark("@chaos") >= 1;
    }
    public class Weimu : TriggerSkill
    {
        public Weimu() : base("weimu")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirming, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetConfirming && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null && use.To.Contains(player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is TrickCard && WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, use.Card)))
                {
                    return new TriggerStruct(Name, player);
                }
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceDelayedTrick
                && base.Triggerable(move.To, room) && move.Reason.Reason == MoveReason.S_REASON_TRANSFER)
            {
                WrappedCard card = move.Reason.Card;
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is TrickCard && WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, card)) && room.GetCardPlace(move.Card_ids[0]) == Place.PlaceDelayedTrick)
                {
                    return new TriggerStruct(Name, move.To);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, ask_who, Name) || room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use)
            {
                room.CancelTarget(ref use, player); // Room::cancelTarget(use, player);
                data = use;
                return true;
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && room.GetCardPlace(move.Card_ids[0]) == Place.PlaceDelayedTrick)
            {
                room.SetEmotion(ask_who, "cancel");
                System.Threading.Thread.Sleep(400);

                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, string.Empty)
                {
                    Card = move.Reason.Card
                };
                CardsMoveStruct move2 = new CardsMoveStruct(move.Card_ids, null, Place.DiscardPile, reason);
                room.MoveCardsAtomic(move2, true);
            }

            return false;
        }
    }
    public class Jianchu : TriggerSkill
    {
        public Jianchu() : base("jianchu")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.PlaceTable
                && move.From != null && move.Reason.Reason == MoveReason.S_REASON_DISMANTLE && move.Reason.SkillName == Name 
                && move.From.Name == move.Reason.TargetId && move.Card_ids.Count == 1)
            {
                bool equip = Engine.GetFunctionCard(room.GetCard(move.Card_ids[0]).Name) is EquipCard;
                string tag_name = string.Format("{0}_{1}", Name, move.Reason.TargetId);
                Player pangde = room.FindPlayer(move.Reason.PlayerId, true);
                pangde.SetTag(tag_name, equip);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player p in use.To)
                    {
                        if (!p.IsNude() && p.Alive && RoomLogic.CanDiscard(room, player, p, "he"))
                            targets.Add(p);
                    }
                    if (targets.Count > 0)
                        return new TriggerStruct(Name, player, targets);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player pangde, TriggerStruct info)
        {
            if (!player.IsNude() && RoomLogic.CanDiscard(room, pangde, player, "he") && room.AskForSkillInvoke(pangde, Name, player, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, pangde.Name, player.Name);
                room.BroadcastSkillInvoke(Name, pangde, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player pangde, TriggerStruct info)
        {
            int to_throw = room.AskForCardChosen(pangde, target, "he", Name, false, HandlingMethod.MethodDiscard);
            List<int> ids = new List<int> { to_throw };
            string tag_name = string.Format("{0}_{1}", Name, target.Name);
            pangde.RemoveTag(tag_name);
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISMANTLE, pangde.Name, target.Name, Name, string.Empty)
            {
                General = RoomLogic.GetGeneralSkin(room, pangde, Name, info.SkillPosition)
            };
            room.ThrowCard(ref ids, reason, target, pangde);
            CardUseStruct use = (CardUseStruct)data;
            if (ids.Count > 0)
            {
                Debug.Assert(pangde.ContainsTag(tag_name), "jianchu tag error!");

                if ((bool)pangde.GetTag(tag_name))
                {
                    int index = 0;
                    for (int i = 0; i < use.EffectCount.Count; i++)
                    {
                        CardBasicEffect effect = use.EffectCount[i];
                        if (effect.To == target)
                        {
                            index++;
                            if (index == info.Times)
                            {
                                effect.Effect2 = 0;
                                break;
                            }
                        }
                    }

                    data = use;
                }
                else
                {
                    List<int> card_ids = room.GetCardIdsOnTable(room.GetSubCards(use.Card));
                    if (card_ids.Count > 0 && card_ids.SequenceEqual(use.Card.SubCards))
                    {
                        room.RemoveSubCards(use.Card);
                        room.ObtainCard(target, ref card_ids, new CardMoveReason(MoveReason.S_REASON_GOTBACK, target.Name));
                    }
                }
            }

            return false;
        }
    }
    public class Leiji : TriggerSkill
    {
        public Leiji() : base("leiji")
        {
            events.Add(TriggerEvent.CardResponded);
            skill_type = SkillType.Attack;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardResponded && base.Triggerable(player, room) && data is CardResponseStruct resp)
            {
                WrappedCard card_star = resp.Card;
                if (card_star.Name == Jink.ClassName)
                    skill_list.Add(new TriggerStruct(Name, player));
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "leiji-invoke", true, true, info.SkillPosition);
            if (target != null)
            {
                player.SetTag("leiji-target", target.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            else
            {
                player.RemoveTag("leiji-target");
                return new TriggerStruct();
            }
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhangjiao, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)zhangjiao.GetTag("leiji-target"));
            zhangjiao.RemoveTag("leiji-target");
            if (target != null)
            {
                JudgeStruct judge = new JudgeStruct
                {
                    Pattern = ".|spade",
                    Good = true,
                    Negative = true,
                    Reason = Name,
                    Who = target,
                    PlayAnimation = true
                };

                room.Judge(ref judge);

                if (!judge.IsGood() && target.Alive)
                    room.Damage(new DamageStruct(Name, zhangjiao, target, 2, DamageStruct.DamageNature.Thunder));
            }

            return false;
        }
    }
    public class Guidao : TriggerSkill
    {
        public Guidao() : base("guidao")
        {
            events.Add(TriggerEvent.AskForRetrial);
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            if (!base.Triggerable(target, room))
                return new TriggerStruct();

            if (target.IsKongcheng() && target.GetHandPile().Count == 0)
            {
                bool has_black = false;
                for (int i = 0; i < 4; i++)
                {
                    int equip = target.GetEquip(i);
                    if (equip > -1 && WrappedCard.IsBlack(room.GetCard(equip).Suit))
                    {
                        has_black = true;
                        break;
                    }
                }
                return (has_black) ? new TriggerStruct(Name, target) : new TriggerStruct();
            }
            return new TriggerStruct(Name, target);
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            List<string> prompt_list = new List<string> {"@guidao-card" , judge.Who.Name, string.Empty, Name,
                         judge.Reason };
            string prompt = string.Join(":", prompt_list);
            room.SetTag(Name, data);
            WrappedCard card = room.AskForCard(player, Name, ".|black", prompt, data, HandlingMethod.MethodResponse, judge.Who, true);
            room.RemoveTag(Name);
            if (card != null)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.Retrial(card, player, ref judge, Name, true, info.SkillPosition);
                data = judge;
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            room.UpdateJudgeResult(ref judge);
            data = judge;
            return false;
        }
    }
    public class Beige : TriggerSkill
    {
        public Beige() : base("beige")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged };
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player == null) return skill_list;
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.Card != null && damage.To.Alive)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> caiwenjis = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player caiwenji in caiwenjis)
                        if (RoomLogic.CanDiscard(room, caiwenji, caiwenji, "he"))
                            skill_list.Add(new TriggerStruct(Name, caiwenji));
                }

            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caiwenji, TriggerStruct info)
        {
            if (caiwenji != null)
            {
                room.SetTag("beige_data", data);
                bool invoke = room.AskForDiscard(caiwenji, Name, 1, 1, true, true, "@beige", true, info.SkillPosition);
                room.RemoveTag("beige_data");

                if (invoke)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, caiwenji.Name, ((DamageStruct)data).To.Name);
                    room.BroadcastSkillInvoke(Name, caiwenji, info.SkillPosition);
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caiwenji, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;

            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                PlayAnimation = false,
                Who = player,
                Reason = Name
            };

            room.Judge(ref judge);

            WrappedCard.CardSuit suit = judge.JudgeSuit;
            switch (suit)
            {
                case WrappedCard.CardSuit.Heart:
                    {
                        RecoverStruct recover = new RecoverStruct
                        {
                            Who = caiwenji,
                            Recover = 1
                        };
                        room.Recover(player, recover, true);

                        break;
                    }
                case WrappedCard.CardSuit.Diamond:
                    {
                        room.DrawCards(player, 2, Name);
                        break;
                    }
                case WrappedCard.CardSuit.Club:
                    {
                        if (damage.From != null && damage.From.Alive)
                            room.AskForDiscard(damage.From, Name, 2, 2, false, true, "@beige-discard");

                        break;
                    }
                case  WrappedCard.CardSuit.Spade:
                    {
                        if (damage.From != null && damage.From.Alive)
                            room.TurnOver(damage.From);

                        break;
                    }
                default:
                    break;
            }
            return false;
        }
    }
    public class Duanchang : TriggerSkill
    {
        public Duanchang() : base("duanchang")
        {
            events.Add(TriggerEvent.Death);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override bool CanPreShow() => false;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && RoomLogic.PlayerHasSkill(room, player, Name) && data is DeathStruct death && death.Damage.From != null)
            {


                Player target = death.Damage.From;
                if (!(target.General1.Contains("sujiang") && target.General2.Contains("sujiang")))
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            return info;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DeathStruct death = (DeathStruct)data;
            Player target = death.Damage.From;
            string choice = "head_general";

            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);

            List<string> choices = new List<string>();
            if (!target.General1.Contains("sujiang"))
                choices.Add("head_general");

            if (!string.IsNullOrEmpty(target.General2) && !target.General2.Contains("sujiang"))
                choices.Add("deputy_general");
            choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { "@duanchang-target:" + target.Name }, target);

            LogMessage log = new LogMessage
            {
                Type = choice == "head_general" ? "#DuanchangLoseHeadSkills" : "#DuanchangLoseDeputySkills",
                From = player.Name,
                To = new List<string> { target.Name },
                Arg = Name
            };
            room.SendLog(log);

            List<string> duanchangList = new List<string>(target.DuanChang.Split(','));
            if (choice == "head_general" && !duanchangList.Contains("head"))
                duanchangList.Add("head");
            else if (choice == "deputy_general" && !duanchangList.Contains("deputy"))
                duanchangList.Add("deputy");
            target.DuanChang = string.Join(",", duanchangList);
            room.BroadcastProperty(target, "DuanChang");

            List<string> skills = choice == "head_general" ? Engine.GetGeneralSkills(target.ActualGeneral1, room.Setting.GameMode, true)
                : Engine.GetGeneralSkills(target.ActualGeneral2, room.Setting.GameMode, false);
            foreach (string skill in skills)
                    room.DetachSkillFromPlayer(target, skill, !RoomLogic.PlayerHasShownSkill(room, target, skill), false, choice == "head_general" ? true : false);

            if (death.Damage.From.Alive)
                room.SetPlayerMark(death.Damage.From, "@duanchang", 1);

            return false;
        }
    }
    public class XiongyiCard : SkillCard
    {
        public static string ClassName = "XiongyiCard";
        public XiongyiCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@arise", 0);
            room.BroadcastSkillInvoke("xiongyi", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "xiongyi");
            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAllPlayers()) {
                if (RoomLogic.IsFriendWith(room, p, card_use.From))
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, card_use.From.Name, p.Name);
                    targets.Add(p);
                }
            }
            room.SortByActionOrder(ref targets);
            card_use.To = targets;
            base.Use(room, card_use);

            List<string> kingdom_list = new List<string> { "shu", "wu", "qun", "wei", "careerist" };
            bool invoke = true;
            int n = RoomLogic.GetPlayerNumWithSameKingdom(room, card_use.From);
            foreach (string kingdom in kingdom_list) {
                if (card_use.From.GetRoleEnum() == PlayerRole.Careerist)
                {
                    if (kingdom == "careerist")
                        continue;
                }
                else if (card_use.From.Kingdom == kingdom)
                    continue;
                int other_num = RoomLogic.GetPlayerNumWithSameKingdom(room, card_use.From, kingdom);
                if (other_num > 0 && other_num < n)
                {
                    invoke = false;
                    break;
                }
            }

            if (invoke && card_use.From.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = card_use.From,
                    Recover = 1
                };
                room.Recover(card_use.From, recover, true);
            }
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.DrawCards(effect.To, new DrawCardStruct(3, effect.From, "xiongyi"));
        }
    }
    public class Xiongyi : ZeroCardViewAsSkill
    {
        public Xiongyi() : base("xiongyi")
        {
            frequency = Frequency.Limited;
            limit_mark = "@arise";
            skill_type = SkillType.Replenish;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark("@arise") >= 1;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(XiongyiCard.ClassName)
            {
                ShowSkill = Name,
                Skill = Name,
                Mute = true
            };
            return card;
        }
    }
    public class Mingshi : TriggerSkill
    {
        public Mingshi() : base("mingshi")
        {
            events.Add(TriggerEvent.DamageInflicted);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && !damage.From.HasShownAllGenerals())
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
            DamageStruct damage = (DamageStruct)data;
            room.NotifySkillInvoked(player, Name);

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
    public class LirangCard : SkillCard
    {
        public LirangCard() : base("LirangCard")
        {
            will_throw = false;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            card_use.From.SetTag("lirang_target", card_use.To[0].Name);

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;
        }
    }
    public class LirangViewAsSkill : ViewAsSkill
    {
        public LirangViewAsSkill() : base("lirang")
        {
            expand_pile = "#lirang";
            response_pattern = "@@lirang";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return player.GetPile("#lirang").Contains(to_select.Id);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0) return null;
            WrappedCard Lirang_card = new WrappedCard("LirangCard");
            Lirang_card.AddSubCards(cards);
            return Lirang_card;
        }
    }
    public class Lirang : TriggerSkill
    {
        public Lirang() : base("lirang")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            view_as_skill = new LirangViewAsSkill();
            skill_type = SkillType.Replenish;
        }
        public override bool CanPreShow() => true;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From != null && base.Triggerable(move.From, room) &&
                (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD && move.To_place == Place.PlaceTable)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int card_id = move.Card_ids[i];
                    if (room.GetCardPlace(card_id) == Place.PlaceTable && (move.From_places[i] == Place.PlaceHand || move.From_places[i] == Place.PlaceEquip))
                        room.GetCard(card_id).SetFlags(Name);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;

            if (move.From != null && base.Triggerable(move.From, room)
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD && move.To_place == Place.DiscardPile)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int card_id = move.Card_ids[i];
                    if (room.GetCardPlace(card_id) == Place.DiscardPile && move.From_places[i] == Place.PlaceTable && room.GetCard(card_id).HasFlag(Name))
                        return new TriggerStruct(Name, move.From);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player tar, ref object data, Player player, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            List<int> cards = new List<int>();
            if (move.From != null && base.Triggerable(move.From, room)
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD && move.To_place == Place.DiscardPile)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int card_id = move.Card_ids[i];
                    if (room.GetCardPlace(card_id) == Place.DiscardPile && move.From_places[i] == Place.PlaceTable && room.GetCard(card_id).HasFlag(Name))
                        cards.Add(card_id);
                }
            }

            List<CardsMoveStruct> lirangs = new List<CardsMoveStruct>();
            while (cards.Count > 0)
            {
                player.PileChange("#" + Name, cards);
                WrappedCard card = room.AskForUseCard(player, "@@lirang", "@lirang-distribute:::" + cards.Count.ToString(), null, -1, HandlingMethod.MethodNone, true, info.SkillPosition);
                player.PileChange("#" + Name, cards, false);

                if (card != null && card.SubCards.Count > 0)
                {
                    Player target = room.FindPlayer((string)player.GetTag("lirang_target"), true);
                    player.RemoveTag("lirang_target");
                    Debug.Assert(target.Alive);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_PREVIEWGIVE, player.Name, target.Name, Name, null);
                    CardsMoveStruct moves = new CardsMoveStruct(card.SubCards, target, Place.PlaceHand, reason);
                    lirangs.Add(moves);
                    foreach (int id in card.SubCards)
                        cards.Remove(id);
                }
                else
                    cards.Clear();
            }

            if (lirangs.Count > 0)
            {
                player.AddMark(Name);
                int index = player.GetMark(Name);
                room.SetTag(string.Format("lirang_this_time_{0}_{1}", player.Name, index), lirangs);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            else
                return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            int index = player.GetMark(Name);
            string str = string.Format("lirang_this_time_{0}_{1}", player.Name, index);
            List<CardsMoveStruct> lirangs = (List<CardsMoveStruct>)room.GetTag(str);
            player.RemoveTag(str);
            player.RemoveMark(Name);
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            List<Player> targets = new List<Player>();
            foreach (CardsMoveStruct move_struct in lirangs)
            {
                List<int> ids = move_struct.Card_ids;
                for (int i = 0; i < ids.Count; i++)
                {
                    int card_id = ids[i];
                    if (room.GetCardPlace(card_id) != Place.DiscardPile)
                        move_struct.Card_ids.Remove(card_id);
                }
                if (move_struct.Card_ids.Count > 0)
                {
                    moves.Add(move_struct);
                    Player target = room.FindPlayer(move_struct.To);
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
            }

            if (moves.Count > 0)
            {
                LogMessage l = new LogMessage
                {
                    Type = "#ChoosePlayerWithSkill",
                    From = player.Name,
                    To = new List<string>()
                };
                ;
                l.Arg = Name;
                foreach (Player tar in targets)
                    l.To.Add(tar.Name);
                room.SendLog(l);

                room.MoveCardsAtomic(moves, true);
            }

            return false;
        }
    }
    public class LirangClear : TriggerSkill
    {
        public LirangClear() : base("#lirang-clear")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
        }
        public override int GetPriority() => -1;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From != null)
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    WrappedCard card = room.GetCard(move.Card_ids[i]);
                    if (move.From_places[i] == Place.PlaceTable && card.HasFlag("lirang"))
                        card.SetFlags("-lirang");
                }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    public class Shuangren : PhaseChangeSkill
    {
        public Shuangren() : base("shuangren")
        {
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player jiling, ref object data, Player ask_who)
        {
            if (base.Triggerable(jiling, room) && jiling.Phase == PlayerPhase.Play && !jiling.IsKongcheng())
            {
                bool can_invoke = false;
                List<Player> other_players = room.GetOtherPlayers(jiling);
                foreach (Player player in other_players) {
                    if (RoomLogic.CanBePindianBy(room, player, jiling))
                    {
                        can_invoke = true;
                        break;
                    }
                }

                return can_invoke ? new TriggerStruct(Name, jiling) : new TriggerStruct();
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player jiling, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(jiling))
            {
                if (RoomLogic.CanBePindianBy(room, p, jiling))
                    targets.Add(p);
            }
            Player victim = null;
            if ((victim = room.AskForPlayerChosen(jiling, targets, Name, "@shuangren", true, true, info.SkillPosition)) != null)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, jiling, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, jiling, info.SkillPosition);
                PindianStruct pd = room.PindianSelect(jiling, victim, "shuangren");
                room.SetTag("shuangren_pd", pd);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player jiling, TriggerStruct info)
        {
            PindianStruct pd =  (PindianStruct)room.GetTag("shuangren_pd");
            room.RemoveTag("shuangren_pd");
            if (pd.From != null)
            {
                Player target = pd.Tos[0];
                room.Pindian(ref pd);
                if (pd.Success)
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        DistanceLimited = false,
                        Skill = "_shuangren"
                    };
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers()) {
                        if (RoomLogic.CanSlash(room, jiling, p, slash) && (RoomLogic.IsFriendWith(room, p, target) || target == p))
                            targets.Add(p);
                    }
                    if (targets.Count > 0)
                    {
                        Player slasher = room.AskForPlayerChosen(jiling, targets,Name, "@dummy-slash", false, false, info.SkillPosition);
                        room.UseCard(new CardUseStruct(slash, jiling, slasher), false);
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
    public class Sijian : TriggerSkill
    {
        public Sijian() : base("sijian")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From != null && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceHand) && move.Is_last_handcard)
            {
                List<Player> other_players = room.GetOtherPlayers(move.From);
                foreach (Player p in other_players) {
                    if (RoomLogic.CanDiscard(room, move.From, p, "he"))
                        return new TriggerStruct(Name, move.From);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player tianfeng, TriggerStruct info)
        {
            List<Player> other_players = room.GetOtherPlayers(tianfeng);
            List<Player> targets = new List<Player>();
            foreach (Player p in other_players) {
                if (RoomLogic.CanDiscard(room, tianfeng, p, "he"))
                    targets.Add(p);
            }
            Player to = room.AskForPlayerChosen(tianfeng, targets, Name, "sijian-invoke", true, true, info.SkillPosition);
            if (to != null)
            {
                tianfeng.SetTag("sijian_target", to.Name);
                room.BroadcastSkillInvoke(Name, tianfeng);
                return info;
            }
            else tianfeng.RemoveTag("sijian_target");
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player tianfeng, TriggerStruct info)
        {
            Player to = room.FindPlayer((string)tianfeng.GetTag("sijian_target"));
            tianfeng.RemoveTag("sijian_target");
            if (to != null && RoomLogic.CanDiscard(room, tianfeng, to, "he"))
            {
                int card_id = room.AskForCardChosen(tianfeng, to, "he", Name, false, HandlingMethod.MethodDiscard);
                room.ThrowCard(card_id, to, tianfeng);
            }
            return false;
        }
    }
    public class Suishi : TriggerSkill
    {
        public Suishi() : base("suishi")
        {
            events = new List<TriggerEvent> { TriggerEvent.Dying, TriggerEvent.Death };
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            List<Player> tianfengs = RoomLogic.FindPlayersBySkillName(room, Name);
            Player target = null;
            if (triggerEvent == TriggerEvent.Dying && data is DyingStruct dying)
            {
                if (dying.Damage.From != null)
                    target = dying.Damage.From;
                foreach (Player tianfeng in tianfengs) {
                    if (player != tianfeng && target != null && (RoomLogic.IsFriendWith(room, tianfeng, target) || RoomLogic.WillBeFriendWith(room, tianfeng, target)))
                        triggers.Add(new TriggerStruct(Name, tianfeng));
                }
            }
            else if (triggerEvent == TriggerEvent.Death)
            {
                foreach (Player tianfeng in tianfengs)
                if (player != null && player != tianfeng && (RoomLogic.IsFriendWith(room, tianfeng, player) || RoomLogic.WillBeFriendWith(room, tianfeng, player)))
                    triggers.Add(new TriggerStruct(Name, tianfeng));
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            bool invoke = RoomLogic.PlayerHasShownSkill(room, player, Name) ? true : room.AskForSkillInvoke(player, Name, (int)triggerEvent, info.SkillPosition);
            if (invoke)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                if (triggerEvent == TriggerEvent.Dying)
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                else if (triggerEvent == TriggerEvent.Death)
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            if (triggerEvent == TriggerEvent.Dying)
                room.DrawCards(player, 1, Name);
            else if (triggerEvent == TriggerEvent.Death)
                room.LoseHp(player);
            return false;
        }
    }
    public class Kuangfu : TriggerSkill
    {
        public Kuangfu() : base("kuangfu")
        {
            events.Add(TriggerEvent.Damage);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player panfeng, ref object data, Player ask_who)
        {
            if (base.Triggerable(panfeng, room) && data is DamageStruct damage && damage.Card != null && !damage.Chain && !damage.Transfer)
            {
                Player target = damage.To;
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && target.HasEquip() && !target.HasFlag("Global_DFDebut"))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (target.GetEquip(i) < 0) continue;
                        if (RoomLogic.CanDiscard(room, panfeng, target, target.GetEquip(i)) || panfeng.GetEquip(i) < 0)
                            return new TriggerStruct(Name, panfeng);
                    }
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player panfeng, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && room.AskForSkillInvoke(panfeng, Name, damage.To, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, panfeng.Name, damage.To.Name);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player panfeng, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            Player target = damage.To;

            List<int> disable_equiplist = new List<int>(), equiplist = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                int id = target.GetEquip(i);
                if (id >= 0)
                {
                    bool discard = true;
                    if (!RoomLogic.CanDiscard(room, panfeng, target, id))
                        discard = false;

                    bool put = false;
                    if (RoomLogic.CanPutEquip(panfeng, room.GetCard(id)))
                    {
                        put = true;
                        equiplist.Add(id);
                    }

                    if (!discard && !put) disable_equiplist.Add(id);
                }
            }
            int card_id = room.AskForCardChosen(panfeng, target, "e", Name, false, HandlingMethod.MethodNone, disable_equiplist);
            WrappedCard card = room.GetCard(card_id);

            List<string> choicelist = new List<string>();
            if (RoomLogic.CanDiscard(room, panfeng, target, card_id))
                choicelist.Add("throw");
            if (equiplist.Contains(card_id))
                choicelist.Add("move");

            string choice = room.AskForChoice(panfeng, "kuangfu", string.Join("+", choicelist), new List<string> { "@kuangfu:::" + card.Name });
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, panfeng, Name, info.SkillPosition);
            if (choice.Contains("move"))
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                room.MoveCardTo(card, panfeng, Place.PlaceEquip);
            }
            else
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.ThrowCard(card_id, target, panfeng);
            }

            return false;
        }
    }

    public class KuangfuJX : TriggerSkill
    {
        public KuangfuJX() : base("kuangfu_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage };
            skill_type = SkillType.Attack;
            view_as_skill = new KuangfuJXVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is DamageStruct damage && player != null && player.HasFlag(Name) && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName)
                && damage.Card.GetSkillName() == Name)
                player.SetFlags("-kuangfu_jx");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class KuangfuJXVS : ZeroCardViewAsSkill
    {
        public KuangfuJXVS() : base("kuangfu_jx")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.HasUsed(KuangfuCard.ClassName))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasEquip() && RoomLogic.CanDiscard(room, player, p, "e"))
                        return true;
            }

            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(KuangfuCard.ClassName) { Skill = Name, ShowSkill = Name };
        }
    }

    public class KuangfuCard : SkillCard
    {
        public static string ClassName = "KuangfuCard";
        public KuangfuCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HasEquip() && RoomLogic.CanDiscard(room, player, p, "e"))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, "kuangfu_jx", "@kuangfu_jx-equip", false, false, card_use.Card.SkillPosition);
                if (target != null)
                {
                    int id = room.AskForCardChosen(player, target, "e", "kuangfu_jx", false, HandlingMethod.MethodDiscard);
                    bool self = room.GetCardOwner(id) == player;
                    room.ThrowCard(id, target, player);

                    if (player.Alive)
                    {
                        WrappedCard slash = new WrappedCard(Slash.ClassName)
                        {
                            DistanceLimited = false,
                            Skill = "_kuangfu_jx"
                        };
                        targets.Clear();
                        foreach (Player p in room.GetOtherPlayers(player))
                        {
                            if (RoomLogic.CanSlash(room, player, p, slash))
                                targets.Add(p);
                        }
                        if (targets.Count > 0)
                        {
                            player.SetFlags("kuangfu_jx");
                            Player slasher = room.AskForPlayerChosen(player, targets, "kuangfu_jx", "@dummy-slash", false, false, card_use.Card.SkillPosition);
                            room.UseCard(new CardUseStruct(slash, player, slasher), false);

                            if (player.Alive)
                            {
                                if (self && !player.HasFlag("kuangfu_jx"))
                                {
                                    room.DrawCards(player, 2, "kuangfu_jx");
                                }
                                else if (!self && player.HasFlag("kuangfu_jx"))
                                {
                                    room.AskForDiscard(player, "kuangfu_jx", 2, 2, false, false, "@kuangfu_jx-disacard", false, card_use.Card.SkillPosition);
                                }

                                player.SetFlags("-kuangfu_jx");
                            }
                        }
                    }
                }
            }
        }
    }

    public class HuoshuiCard : SkillCard
    {
        public HuoshuiCard() : base("HuoshuiCard")
        {
            target_fixed = true;
        }
    }
    public class HuoshuiVS : ZeroCardViewAsSkill
    {
        public HuoshuiVS() : base("huoshui")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_PLAY && !invoker.HasShownSkill(Name, position == "head");
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("HuoshuiCard")
            {
                ShowSkill = Name,
                Skill = Name,
                Mute = true
            };
            return card;
        }
    }
    public class Huoshui : TriggerSkill
    {
        public Huoshui() : base("huoshui")
        {
            events = new List<TriggerEvent> { TriggerEvent.GeneralShown, TriggerEvent.EventPhaseStart, TriggerEvent.Death, TriggerEvent.EventAcquireSkill, TriggerEvent.EventLoseSkill };
            view_as_skill = new HuoshuiVS();
            skill_type = SkillType.Wizzard;
        }
        private void DoHuoshui(Room room, Player zoushi, bool set, TriggerEvent triggerEvent)
        {
            if (set && (!zoushi.ContainsTag(Name) || !(bool)zoushi.GetTag("huoshui")))
            {
                room.BroadcastSkillInvoke(Name, zoushi);
                room.NotifySkillInvoked(zoushi, Name);
                foreach (Player p in room.GetOtherPlayers(zoushi))
                {
                    if (RoomLogic.GetHeadActivedSkills(room, zoushi, true, true).Contains(Engine.GetSkill(Name)))
                        room.SetPlayerDisableShow(p, "hd", "huoshui_head");
                    if (RoomLogic.GetDeputyActivedSkills(room, zoushi, true, true).Contains(Engine.GetSkill(Name)))
                        room.SetPlayerDisableShow(p, "hd", "huoshui_deputy");
                }
                zoushi.SetTag(Name, true);
            }
            else if (!set && zoushi.ContainsTag(Name) && (bool)zoushi.GetTag(Name))
            {
                if (triggerEvent == TriggerEvent.Death || triggerEvent == TriggerEvent.EventPhaseStart)
                {
                    foreach (Player p in room.GetOtherPlayers(zoushi))
                    {
                        room.RemovePlayerDisableShow(p, "huoshui_head");
                        room.RemovePlayerDisableShow(p, "huoshui_deputy");
                    }
                    zoushi.SetTag(Name, false);
                }
                else
                {
                    foreach (Player p in room.GetOtherPlayers(zoushi))
                    {
                        if (!RoomLogic.GetHeadActivedSkills(room, zoushi, true, true).Contains(Engine.GetSkill(Name)))
                            room.RemovePlayerDisableShow(p, "huoshui_head");
                        if (!RoomLogic.GetDeputyActivedSkills(room, zoushi, true, true).Contains(Engine.GetSkill(Name)))
                            room.RemovePlayerDisableShow(p, "huoshui_deputy");
                    }
                    if (!RoomLogic.PlayerHasShownSkill(room, zoushi, Name))
                        zoushi.SetTag(Name, false);
                }
            }
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player == null)
                return;
            if (triggerEvent != TriggerEvent.Death && !player.Alive)
                return;
            Player c = room.Current;
            if (c == null || (triggerEvent != TriggerEvent.EventPhaseStart && c.Phase == PlayerPhase.NotActive) || c != player)
                return;

            if ((triggerEvent == TriggerEvent.GeneralShown || triggerEvent == TriggerEvent.EventPhaseStart || triggerEvent == TriggerEvent.EventAcquireSkill)
                && !RoomLogic.PlayerHasShownSkill(room, player, Name))
                return;
            if (triggerEvent == TriggerEvent.EventPhaseStart && !(player.Phase == PlayerPhase.RoundStart || player.Phase == PlayerPhase.NotActive))
                return;
            if (triggerEvent == TriggerEvent.Death && !RoomLogic.PlayerHasShownSkill(room, player, Name))
                return;
            if ((triggerEvent == TriggerEvent.EventAcquireSkill || triggerEvent == TriggerEvent.EventLoseSkill) && data is InfoStruct info && info.Info != Name)
                return;

            bool set = false;
            if (triggerEvent == TriggerEvent.GeneralShown || triggerEvent == TriggerEvent.EventAcquireSkill || (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.RoundStart))
                set = true;

            DoHuoshui(room, player, set, triggerEvent);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    public class QingchengCard : SkillCard
    {
        public QingchengCard() : base("QingchengCard")
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (!to_select.HasShownAllGenerals() || to_select.General1.Contains("sujiang") || string.IsNullOrEmpty(to_select.General2)
                || to_select.General2.Contains("sujiang") || Engine.GetGeneral(to_select.General1, room.Setting.GameMode).IsLord()) return false;
            return targets.Count == 0 && to_select != Self;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, to = effect.To;
            List<string> choices = new List<string>();
            if (!Engine.GetGeneral(to.General1, room.Setting.GameMode).IsLord() && !to.General1.Contains("sujiang") && !to.IsDuanchang(true))
                choices.Add(to.General1);
            if (!string.IsNullOrEmpty(to.General2) && !to.General2.Contains("sujiang") && !to.IsDuanchang(false))
                choices.Add(to.General2);

            if (choices.Count > 0)
            {
                string choice = choices[0];
                if (choices.Count == 2)
                    choice = room.AskForGeneral(player, choices, null, true, "qingcheng");

                room.HideGeneral(to, choice == to.General1);
            }
            
            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(effect.Card.GetEffectiveId()).Name);
            if (fcard is EquipCard)
            {
                List<Player> players = room.GetOtherPlayers(to), targets = new List<Player>();
                foreach (Player target in players)
                {
                    if (TargetFilter(room, new List<Player>(), target, player, effect.Card))
                        targets.Add(target);
                }

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, "qingcheng", "@qingcheng", true, false, effect.Card.SkillPosition);
                    if (target != null)
                    {
                        choices.Clear();
                        if (!Engine.GetGeneral(target.General1, room.Setting.GameMode).IsLord() && !target.General1.Contains("sujiang") && !target.IsDuanchang(true))
                            choices.Add(target.General1);
                        if (!string.IsNullOrEmpty(target.General2) && !target.General2.Contains("sujiang") && !target.IsDuanchang(false))
                            choices.Add(target.General2);

                        if (choices.Count > 0)
                        {
                            string choice = choices[0];
                            if (choices.Count == 2)
                                choice = room.AskForGeneral(player, choices, null, true, "qingcheng");

                            room.HideGeneral(target, choice == target.General1);
                        }
                    }
                }
            }
        }
    }
    public class Qingcheng : OneCardViewAsSkill
    {
        public Qingcheng() : base("qingcheng")
        {
            filter_pattern = ".|black!";
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "he");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard("QingchengCard");
            first.AddSubCard(card);
            first.ShowSkill = Name;
            first.Skill = Name;
            return first;
        }
    }

    #endregion

    #region 吴
    public class ZhihengCard: SkillCard
{
        public ZhihengCard() : base("ZhihengCard")
        {
            target_fixed = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            if (card_use.From.Alive)
                room.DrawCards(card_use.From, card_use.Card.SubCards.Count, "zhiheng");
        }
    }
    public class Zhiheng : ViewAsSkill
    {
        public Zhiheng() : base("zhiheng")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "he") && !player.HasUsed("ZhihengCard");
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count >= player.MaxHp)
                return !RoomLogic.IsJilei(room, player, to_select) && player.GetTreasure() && player.Treasure.Value == LuminouSpearl.ClassName
                        && to_select.Id != player.Treasure.Key && !selected.Contains(room.GetCard(player.Treasure.Key));

            return !RoomLogic.IsJilei(room, player, to_select) && selected.Count < player.MaxHp;
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
                return null;

            WrappedCard zhiheng_card = new WrappedCard("ZhihengCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            zhiheng_card.AddSubCards(cards);
            return zhiheng_card;
        }
    }
    public class Qixi : OneCardViewAsSkill
    {
        public Qixi() : base("qixi")
        {
            response_or_use = true;
            skill_type = SkillType.Attack;
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return WrappedCard.IsBlack(to_select.Suit);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard card = new WrappedCard(Dismantlement.ClassName);
                if (Engine.MatchExpPattern(room, pattern, player, card)) return true;
            }
            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard dismantlement = new WrappedCard(Dismantlement.ClassName);
            dismantlement.AddSubCard(card);
            dismantlement.Skill = Name;
            dismantlement.ShowSkill = Name;
            dismantlement = RoomLogic.ParseUseCard(room, dismantlement);
            return dismantlement;
        }
    }
    public class Keji : TriggerSkill
    {
        public Keji() : base("keji")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.PreCardUsed, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player != null && player.Alive && (triggerEvent == TriggerEvent.PreCardUsed || triggerEvent == TriggerEvent.CardResponded) && player.Phase == PlayerPhase.Play)
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.PreCardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp)
                    card = resp.Card;

                if (card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!(fcard is SkillCard))
                    {
                        WrappedCard.CardSuit suit = RoomLogic.GetCardSuit(room, card);
                        if (suit == WrappedCard.CardSuit.NoSuit) return;
                        bool red = WrappedCard.IsRed(suit);
                        List<bool> suits = player.ContainsTag(Name) ? (List<bool>)player.GetTag(Name) : new List<bool>();
                        if (!suits.Contains(red))
                        {
                            suits.Add(red);
                            player.SetTag(Name, suits);
                            if (suits.Count == 2)
                                player.SetFlags("KejiInPlayPhase");
                        }
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && player != null && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                player.RemoveTag(Name);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change)
            {
                if (!player.HasFlag("KejiInPlayPhase") && change.To == PlayerPhase.Discard)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                if (RoomLogic.PlayerHasShownSkill(room, player, Name))
                    room.NotifySkillInvoked(player, Name);

                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return false;
        }
    }
    public class KejiMax : MaxCardsSkill
    {
        public KejiMax() : base("#keji-max")
        {
        }
        public override int GetExtra(Room room, Player target)
        {
            if (RoomLogic.PlayerHasShownSkill(room, target, "keji") && !target.HasFlag("KejiInPlayPhase"))
                return 4;

            return 0;
        }
    }
    public class Mouduan : TriggerSkill
    {
        public Mouduan() : base("mouduan")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player != null && room.Current == player && triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card != null && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                int suit = (int)RoomLogic.GetCardSuit(room, use.Card);
                int type = (int)fcard.TypeID;
                List<int> suits = player.ContainsTag(Name + "Suit") ? (List<int>)player.GetTag(Name + "Suit") : new List<int>();
                List<int> types = player.ContainsTag(Name + "Type") ? (List<int>)player.GetTag(Name + "Type") : new List<int>();
                if (suit < 4 && !suits.Contains(suit))
                    suits.Add(suit);
                if (type > 0 && !types.Contains(type))
                    types.Add(type);
                
                player.SetTag(Name + "Suit", suits);
                player.SetTag(Name + "Type", types);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                player.RemoveTag(Name + "Suit");
                player.RemoveTag(Name + "Type");
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish)
            {
                List<int> suits = player.ContainsTag(Name + "Suit") ? (List<int>)player.GetTag(Name + "Suit") : new List<int>();
                List<int> types = player.ContainsTag(Name + "Type") ? (List<int>)player.GetTag(Name + "Type") : new List<int>();
                if (suits.Count == 4 || types.Count == 3)
                    return new TriggerStruct(Name, player);
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
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetCards("ej").Count > 0)
                    targets.Add(p);
            }
            if (targets.Count > 0) {
                Player target1 = room.AskForPlayerChosen(player, targets, Name, "@mouduan1", true, false, info.SkillPosition);
                if (target1 != null) {
                    int card_id = room.AskForCardChosen(player, target1, "ej", Name);
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
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (equip_index != -1)
                        {
                            if (p.GetEquip(equip_index) < 0 && RoomLogic.CanPutEquip(p, card))
                                tos.Add(p);
                        }
                        else if (RoomLogic.IsProhibited(room, null, p, card) == null && !RoomLogic.PlayerContainsTrick(room, p, card.Name) && p.JudgingAreaAvailable)
                                tos.Add(p);
                    }

                    room.SetTag("MouduanTarget", target1);
                    string position = info.SkillPosition;
                    Player to = room.AskForPlayerChosen(player, tos, Name, "@mouduan-to:::" + card.Name, false, false, position);
                    room.RemoveTag("MouduanTarget");
                    if (to != null)
                    {
                        if ((place == Place.PlaceDelayedTrick && target1 != player) || (place == Place.PlaceEquip && to != player))
                        {
                            ResultStruct result = player.Result;
                            result.Assist++;
                            player.Result = result;
                        }

                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target1.Name, to.Name);
                        CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_TRANSFER, player.Name, Name, null)
                        {
                            Card = card
                        };
                        room.MoveCardTo(card, target1, to, place, reason);
                        /*
                        if (place == Place.PlaceDelayedTrick)
                        {
                            CardUseStruct use = new CardUseStruct(card, null, to);
                            object _data = use;
                            room.RoomThread.Trigger(TriggerEvent.TargetConfirming, room, to, ref _data);
                            CardUseStruct new_use = (CardUseStruct)_data;
                            if (new_use.To.Count == 0)
                                fcard.OnNullified(room, to, card);

                            foreach (Player p in room.GetAllPlayers())
                                room.RoomThread.Trigger(TriggerEvent.TargetConfirmed, room, p, ref _data);
                        }
                        */
                    }
                }
            }
            return false;
        }
    }
    public class KurouCard : SkillCard
    {
        public KurouCard() : base("KurouCard")
        {
            target_fixed = true;
            will_throw = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.LoseHp(card_use.From);
            room.DrawCards(card_use.From, 3, "kurou");
            card_use.From.SetFlags("kurou");
        }
    }

    public class Kurou : TriggerSkill
    {
        public Kurou() : base("kurou")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Masochism;
            view_as_skill = new KurouVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-kurou");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class KurouVS : OneCardViewAsSkill
    {
        public KurouVS() : base("kurou")
        {

            filter_pattern = "..!";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.Hp > 0 && !player.HasUsed("KurouCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard kr = new WrappedCard("KurouCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            kr.AddSubCard(card);
            return kr;
        }
    }
    public class KurouTM : TargetModSkill
    {
        public KurouTM() : base("#kurou-tm", false)
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (from.HasFlag("kurou"))
                return 1;
            else
                return 0;
        }
    }
    public class Yingzi : DrawCardsSkill
    {
        protected bool m_canPreshow;
        public Yingzi(string owner, bool can_preshow) : base("yingzi_" + owner)
        {
            m_canPreshow = can_preshow;
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }
        public override bool CanPreShow()=> m_canPreshow;
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            return n + 1;
        }
    }
    public class YingziMax : MaxCardsSkill
    {
        protected string owner;
        public YingziMax(string owner) : base("#yingzi-max_" + owner)
        {
            this.owner = owner;
        }
        public override int GetFixed(Room room, Player target)
        {
            if (RoomLogic.PlayerHasShownSkill(room, target, "yingzi_" + owner))
            {
                return target.MaxHp;
            }
            return -1;
        }
    }
    public class FanjianCard : SkillCard
    {
        public FanjianCard() : base("FanjianCard")
        {
            will_throw = false;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player source = effect.From;
            Player target = effect.To;
            int card_id = effect.Card.GetEffectiveId();
            WrappedCard card = room.GetCard(card_id);
            room.ShowCard(source, card_id, "fanjian");
            room.FocusAll(2000);

            WrappedCard.CardSuit suit = card.Suit;
            target.SetTag("fanjian", suit);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, source.Name, target.Name, "fanjian", null);
            room.MoveCardTo(card, target, Place.PlaceHand, reason, false);
            List<string> choices = new List<string>(), promopts = new List<string> { "fanjian" };
            if (!target.IsKongcheng())
            {
                choices.Add("show");
                promopts.Add(string.Format("@fanjian-show:::{0}", WrappedCard.GetSuitIcon(card.Suit)));
            }

            if (target.Hp > 0)
                choices.Add("losehp");

            if (choices.Count > 0)
            {
                string choice = room.AskForChoice(target, "fanjian", string.Join("+", choices), promopts);
                target.RemoveTag("fanjian");
                if (choice.Contains("show"))
                {
                    room.ShowAllCards(target);

                    List<int> slash = new List<int>();
                    foreach (int id in target.GetCards("he"))
                        if (room.GetCard(id).Suit == suit)
                            slash.Add(id);

                    if (slash.Count > 0)
                        room.ThrowCard(ref slash, target);
                    else
                    {
                        LogMessage ll = new LogMessage
                        {
                            Type = "#fanjiannodis",
                            From = target.Name,
                            Arg = WrappedCard.GetSuitString(card.Suit)
                        };
                        room.SendLog(ll);
                        room.LoseHp(target);
                    }
                }
                else
                {
                    room.LoseHp(target);
                }
            }
        }
    }
    public class Fanjian : OneCardViewAsSkill
    {
        public Fanjian() : base("fanjian")
        {
            skill_type = SkillType.Attack;
            filter_pattern = ".";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng() && !player.HasUsed("FanjianCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard fj = new WrappedCard("FanjianCard")
            {
                Skill = Name,
                ShowSkill = Name,
            };
            fj.AddSubCard(card);
            return fj;
        }
    }
    public class Guose : OneCardViewAsSkill
    {
        public Guose() : base("guose")
        {
            filter_pattern = ".|diamond";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard card = new WrappedCard(Indulgence.ClassName);
                if (Engine.MatchExpPattern(room, pattern, player, card)) return true;
            }
            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard indulgence = new WrappedCard(Indulgence.ClassName);
            indulgence.AddSubCard(card);
            indulgence.Skill = Name;
            indulgence.ShowSkill = Name;
            indulgence = RoomLogic.ParseUseCard(room, indulgence);
            return indulgence;
        }
    }
    public class LiuliCard: SkillCard
    {
        public LiuliCard() : base("LiuliCard")
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0)
                return false;

            if (to_select.HasFlag("LiuliSlashSource") || to_select.HasFlag("LiuliSlashTarget") || to_select == Self)
                return false;
            
            Player from = null;
            foreach (Player p in room.GetAllPlayers(true)) {
                if (p.HasFlag("LiuliSlashSource"))
                {
                    from = p;
                    break;
                }
            }

            CardUseStruct use = (CardUseStruct)room.GetTag("liuli");
            WrappedCard slash = use.Card;
            if (from != null && RoomLogic.IsProhibited(room, from, to_select, slash) != null)
                return false;

            return RoomLogic.InMyAttackRange(room, Self, to_select, card);
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            effect.To.SetFlags("LiuliTarget");
        }
    }
    public class LiuliViewAsSkill : OneCardViewAsSkill
    {
        public LiuliViewAsSkill() : base("liuli")
        {
            filter_pattern = "..!";
            response_pattern = "@@liuli";
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard liuli_card = new WrappedCard("LiuliCard")
            {
                Skill = Name,
                Mute = true
            };
            liuli_card.AddSubCard(card);
            return liuli_card;
        }
    }
    public class Liuli : TriggerSkill
    {
        public Liuli() : base("liuli")
        {
            events.Add(TriggerEvent.TargetConfirming);
            view_as_skill = new LiuliViewAsSkill();
            skill_type = SkillType.Defense;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player daqiao, ref object data, Player ask_who)
        {
            if (base.Triggerable(daqiao, room) && data is CardUseStruct use && use.To.Contains(daqiao) && RoomLogic.CanDiscard(room, daqiao, daqiao, "he"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> players = room.GetOtherPlayers(daqiao);
                    players.Remove(use.From);

                    bool can_invoke = false;
                    foreach (Player p in players)
                    {
                        if (RoomLogic.IsProhibited(room, use.From, p, use.Card) == null && RoomLogic.InMyAttackRange(room, daqiao, p))
                        {
                            can_invoke = true;
                            break;
                        }
                    }

                    return can_invoke ? new TriggerStruct(Name, daqiao) : new TriggerStruct();
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player daqiao, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            use.From.SetFlags("LiuliSlashSource");      // a temp nasty trick

            foreach (Player p in use.To)
                p.SetFlags("LiuliSlashTarget");

            string prompt = "@liuli:" + use.From.Name;
            room.SetTag(Name, data);             // for the client (UI)

            int index = 0;
            for (int i = 0; i < use.EffectCount.Count; i++)
            {
                CardBasicEffect effect = use.EffectCount[i];
                if (effect.To == daqiao)
                {
                    index++;
                    if (index == info.Times)
                    {
                        daqiao.SetTag(Name, i);
                        break;
                    }
                }
            }

            WrappedCard c = room.AskForUseCard(daqiao, "@@liuli", prompt, null, -1, HandlingMethod.MethodDiscard, true, info.SkillPosition);

            room.RemoveTag(Name);
            daqiao.RemoveTag(Name);

            use.From.SetFlags("-LiuliSlashSource");
            foreach (Player p in use.To)
                p.SetFlags("-LiuliSlashTarget");

            if (c != null)
            {
                room.BroadcastSkillInvoke(Name, daqiao, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player daqiao, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            List<Player> players = room.GetOtherPlayers(daqiao);
            Player target = null;
            foreach (Player p in players)
            {
                if (p.HasFlag("LiuliTarget"))
                {
                    p.SetFlags("-LiuliTarget");
                    target = p;
                    break;
                }
            }

            if (use.To.Contains(target) && use.To.IndexOf(target) > use.To.IndexOf(daqiao))
            {
                use.To.Insert(use.To.IndexOf(target), target);
                use.EffectCount.Insert(use.To.IndexOf(target), new CardBasicEffect(target, 0, 1, 0));

                int index = 0, count = use.EffectCount.Count;
                for (index = 0; index < count; index++)
                {
                    if (use.EffectCount[index].To == daqiao && !use.EffectCount[index].Triggered)
                        break;
                }
                use.To.RemoveAt(index);
                use.EffectCount.RemoveAt(index);
            }
            else
            {
                int index = 0, count = use.EffectCount.Count;
                for (index = 0; index < count; index++)
                {
                    if (use.EffectCount[index].To == daqiao && !use.EffectCount[index].Triggered)
                        break;
                }
                use.To.RemoveAt(index);
                use.EffectCount.RemoveAt(index);

                use.To.Add(target);
                use.EffectCount.Add(new CardBasicEffect(target, 0, 1, 0));
                room.SortByActionOrder(ref use);
            }
            room.SlashSettlementFinished(daqiao, use.Card);
            data = use;

            return true;
        }
    }
    public class Qianxun : TriggerSkill
    {
        public Qianxun() : base("qianxun")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirming, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetConfirming && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null
                && (use.Card.Name == Snatch.ClassName || use.Card.Name == Indulgence.ClassName) && use.To.Contains(player))
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceDelayedTrick
                && base.Triggerable(move.To, room) && move.Reason.Reason == MoveReason.S_REASON_TRANSFER)
            {
                WrappedCard card = move.Reason.Card;
                if (card.Name == Indulgence.ClassName && room.GetCardPlace(move.Card_ids[0]) == Place.PlaceDelayedTrick)
                {
                    return new TriggerStruct(Name, move.To);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = RoomLogic.PlayerHasShownSkill(room, ask_who, Name) || room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition);
            if (invoke)
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use)
            {
                room.CancelTarget(ref use, player); // Room::cancelTarget(use, player);
                data = use;
                return true;
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && room.GetCardPlace(move.Card_ids[0]) == Place.PlaceDelayedTrick)
            {
                room.SetEmotion(ask_who, "cancel");
                Thread.Sleep(400);

                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, string.Empty)
                {
                    Card = move.Reason.Card
                };
                /*
                room.MoveCardTo(room.GetCard(move.Card_ids[0]), move.To, null, Place.PlaceTable, reason);
                if (room.GetCardPlace(move.Card_ids[0]) == Place.PlaceTable)
                    room.MoveCardTo(room.GetCard(move.Card_ids[0]), null, null, Place.DiscardPile, reason);
                    */
                CardsMoveStruct move2 = new CardsMoveStruct(move.Card_ids, null, Place.DiscardPile, reason);
                room.MoveCardsAtomic(move2, true);
            }

            return false;
        }
    }
    public class Duoshi : OneCardViewAsSkill
    {
        public Duoshi() : base("duoshi")
        {
            filter_pattern = ".|red|.|hand";
            response_or_use = true;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => player.UsedTimes("ViewAsSkill_duoshiCard") < 4;
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard await = new WrappedCard(AwaitExhausted.ClassName);
            await.AddSubCard(card);
            await.Skill = Name;
            await.ShowSkill = Name;
            await = RoomLogic.ParseUseCard(room, await);
            return await;
        }
    }
    public class JieyinCard : SkillCard
    {
        public JieyinCard() : base("JieyinCard") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0)
                return false;

            return to_select.IsMale() && to_select.IsWounded() && to_select != Self;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 2 && targets.Count == 1;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            RecoverStruct recover = new RecoverStruct
            {
                Card = effect.Card,
                Who = effect.From,
                Recover = 1
            };
            List<Player> targets = new List<Player> { effect.From, effect.To };

            room.SortByActionOrder(ref targets);
            foreach (Player target in targets)
                room.Recover(target, recover, true);
        }
    }
    public class Jieyin : ViewAsSkill
    {
        public Jieyin() : base("jieyin")
        {
            skill_type = SkillType.Recover;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.HandcardNum >= 2 && !player.HasUsed("JieyinCard");
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 1 || RoomLogic.IsJilei(room, player, to_select))
                return false;

            return !player.HasEquip(to_select.Name);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            WrappedCard jieyin_card = new WrappedCard("JieyinCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            jieyin_card.AddSubCards(cards);
            return jieyin_card;
        }
    }
    public class Xiaoji : TriggerSkill
    {
        public Xiaoji() : base("xiaoji")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From != null && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceEquip))
                return new TriggerStruct(Name, move.From);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player sunshangxiang, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(sunshangxiang, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, sunshangxiang, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, 2, Name);
            return false;
        }
    }
    public class Yinghun: PhaseChangeSkill
    {
        public Yinghun(string owner) : base("yinghun_" + owner)
        {
        }
        public override bool CanPreShow() => false;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            return (base.Triggerable(target, room)  && target.Phase == PlayerPhase.Start
                && target.IsWounded()) ? new TriggerStruct(Name, target) : new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player to = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "yinghun-invoke", true, true, info.SkillPosition);
            if (to != null)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, to.Name);
                player.SetTag("yinghun_target", to.Name);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player sunjian, TriggerStruct info)
        {
            Player to = room.FindPlayer((string)sunjian.GetTag("yinghun_target"));
            if (to != null)
            {
                int x = sunjian.GetLostHp();

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, sunjian, Name, info.SkillPosition);
                if (x == 1)
                {
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    room.DrawCards(to, new DrawCardStruct(1, sunjian, Name));
                    room.AskForDiscard(to, Name, 1, 1, false, true);
                }
                else
                {
                    to.SetFlags("YinghunTarget");
                    List<string> descriptions = new List<string> { "@to-player:" + to.Name, "@d1tx:::" + x.ToString(), "@dxt1:::" + x.ToString() };
                    string choice = room.AskForChoice(sunjian, Name,  "d1tx+dxt1", descriptions);
                    to.SetFlags("-YinghunTarget");
                    if (choice.Contains("d1tx"))
                    {
                        room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                        room.DrawCards(to, new DrawCardStruct(1, sunjian, Name));
                        room.AskForDiscard(to, Name, x, x, false, true);
                    }
                    else
                    {
                        room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);

                        room.DrawCards(to, new DrawCardStruct(x, sunjian, Name));
                        room.AskForDiscard(to, Name, 1, 1, false, true);
                    }
                }
            }
            return false;
        }
    }
    public class TianxiangCard : SkillCard
    {
        public static string ClassName = "TianxiangCard";
        public TianxiangCard() : base(ClassName)
        {
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            effect.From.SetFlags("tianxiang_invoke");
            effect.From.SetTag("tianxiang_target", effect.To.Name);
            effect.From.SetTag("tianxiang_card", effect.Card.GetEffectiveId());
        }
    }
    public class TianxiangViewAsSkill : OneCardViewAsSkill
    {
        public TianxiangViewAsSkill() : base("tianxiang")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@tianxiang";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard tianxiangCard = new WrappedCard("TianxiangCard");
            tianxiangCard.AddSubCard(card);
            tianxiangCard.Skill = Name;
            return tianxiangCard;
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            if (RoomLogic.IsJilei(room, player, to_select)) return false;
            string pat = ".|heart|.|hand";
            CardPattern pattern = Engine.GetPattern(pat);
            return pattern.Match(player, room, to_select);
        }
    }
    public class Tianxiang : TriggerSkill
    {
        public Tianxiang() : base("tianxiang")
        {
            events.Add(TriggerEvent.DamageInflicted);
            view_as_skill = new TianxiangViewAsSkill();
            skill_type = SkillType.Defense;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player xiaoqiao, ref object data, Player ask_who)
        {
            if (base.Triggerable(xiaoqiao, room) && RoomLogic.CanDiscard(room, xiaoqiao, xiaoqiao, "h"))
                return new TriggerStruct(Name, xiaoqiao);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player xiaoqiao, ref object data, Player ask_who, TriggerStruct info)
        {
            xiaoqiao.SetFlags("-tianxiang_invoke");
            room.SetTag("TianxiangDamage", data);
            room.AskForUseCard(xiaoqiao, "@@tianxiang", "@tianxiang-card", null, -1, HandlingMethod.MethodDiscard, true, info.SkillPosition);
            room.RemoveTag("TianxiangDamage");
            if (xiaoqiao.HasFlag("tianxiang_invoke"))
                return info;

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player xiaoqiao, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)xiaoqiao.GetTag("tianxiang_target"));
            xiaoqiao.RemoveTag("tianxiang_target");
            xiaoqiao.SetFlags("-tianxiang_invoke");
            xiaoqiao.RemoveTag("tianxiang_card");

            DamageStruct damage = (DamageStruct)data;
            damage.Transfer = true;
            damage.To = target;
            damage.TransferReason = Name;

            room.SetTag(xiaoqiao.Name + "_TransferDamage", damage);

            return true;
        }
    }
    public class TianxiangDraw : TriggerSkill
    {
        public TianxiangDraw() : base("tianxiang-draw")
        {
            events.Add(TriggerEvent.DamageComplete);
            frequency = Frequency.Compulsory;
            global = true;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player == null) return new TriggerStruct();
            DamageStruct damage = (DamageStruct)data;
            if (player.Alive && damage.Transfer && damage.TransferReason == "tianxiang")
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, new DrawCardStruct(player.GetLostHp(), player, "tianxiang"));
            return false;
        }
    }
    public class HongyanFilter : FilterSkill
    {
        public HongyanFilter() : base("hongyan")
        {
        }
        
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            int id = to_select.GetEffectiveId();
            return to_select.Suit == WrappedCard.CardSuit.Spade
                    && (room.GetCardPlace(id) == Place.PlaceEquip
                        || room.GetCardPlace(id) == Place.PlaceHand || room.GetCardPlace(id) == Place.PlaceJudge);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            return base.ViewAs(room, cards, player);
        }
        public override void ViewAs(Room room, ref RoomCard card, Player player)
        {
            card.Skill = Name;
            card.SetSuit(WrappedCard.CardSuit.Heart);
        }
    }
    public class Hongyan : TriggerSkill
    {
        public Hongyan() : base("hongyan")
        {
            events.Add(TriggerEvent.FinishRetrial);
            frequency = Frequency.Compulsory;
            view_as_skill = new HongyanFilter();
            skill_type = SkillType.Alter;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && !RoomLogic.PlayerHasShownSkill(room, player, Name) && data is JudgeStruct judge
                && judge.Who == player && judge.Card.Suit == WrappedCard.CardSuit.Spade)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return room.AskForSkillInvoke(player, Name, data, info.SkillPosition) ? info : new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            List<int> cards = new List<int> { judge.Card.GetEffectiveId() };
            room.FilterCards(player, cards, true);
            room.UpdateJudgeResult(ref judge);
            data = judge;
            return false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }
    public class TianyiCard : SkillCard
    {
        public TianyiCard() : base("TianyiCard") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && RoomLogic.CanBePindianBy(room, to_select, Self) && to_select != Self;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            PindianStruct pd = room.PindianSelect(card_use.From, target, "tianyi");

            room.Pindian(ref pd);
            if (pd.Success)
                card_use.From.SetFlags("TianyiSuccess");
            else
                RoomLogic.SetPlayerCardLimitation(card_use.From, "tianyi", "use", Slash.ClassName, true);
        }
    }
    public class TianyiVS : ZeroCardViewAsSkill
    {
        public TianyiVS() : base("tianyi")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("TianyiCard") && !player.IsKongcheng();
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("TianyiCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            return card;
        }
    }

    public class Tianyi : TriggerSkill
    {
        public Tianyi() : base("tianyi")
        {
            events.Add(TriggerEvent.CardTargetAnnounced);
            skill_type = SkillType.Attack;
            view_as_skill = new TianyiVS();
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && player.HasFlag("TianyiSuccess"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> selected = new List<Player>(use.To);
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (!use.To.Contains(p) && fcard.ExtratargetFilter(room, selected, p, player, use.Card))
                            return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            List<Player> targets = new List<Player>();
            List<Player> selected = new List<Player>(use.To);
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            foreach (Player p in room.GetOtherPlayers(player))
                if (!use.To.Contains(p) && fcard.ExtratargetFilter(room, selected, p, player, use.Card))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                room.SetTag("extra_target_skill", data);                   //for AI
                Player target = room.AskForPlayerChosen(player, targets, Name, string.Format("@extra_targets1:::{0}:{1}", use.Card.Name, 1), true, false, info.SkillPosition);
                room.RemoveTag("extra_target_skill");
                if (target != null)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                    GeneralSkin gs = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gs.General, gs.SkinId);
                    room.NotifySkillInvoked(player, Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        To = new List<string> { target.Name },
                        Card_str = RoomLogic.CardToString(room, use.Card),
                        Arg = Name
                    };
                    room.SendLog(log);

                    player.SetTag("extra_targets", target.Name);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag("extra_targets"));
            player.RemoveTag("extra_targets");

            if (target != null && data is CardUseStruct use)
            {
                use.To.Add(target);
                room.SortByActionOrder(ref use);
                data = use;
            }

            return false;
        }
    }

    public class TianyiTargetMod : TargetModSkill
    {
        public TianyiTargetMod() : base("#tianyi-target", false)
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (from.HasFlag("TianyiSuccess"))
                return 1;
            else
                return 0;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from.HasFlag("TianyiSuccess") ? true : false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 1;
        }
    }
    public class Buqu : TriggerSkill
    {
        public Buqu() : base("buqu")
        {
            events.Add(TriggerEvent.AskForPeaches);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DyingStruct dying && dying.Who == player && base.Triggerable(player, room) && player.Hp <= 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            int id = room.GetNCards(1)[0];
            int num = room.GetCard(id).Number;
            bool duplicate = false;
            List<int> buqu = player.GetPile(Name);
            room.AddToPile(player, Name, id);
            Thread.Sleep(500);

            foreach (int card_id in buqu)
            {
                if (room.GetCard(card_id).Number == num)
                {
                    duplicate = true;
                    LogMessage log = new LogMessage
                    {
                        Type = "#BuquDuplicate",
                        From = player.Name
                    };
                    List<string> number_string = new List<string> { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
                    log.Arg = number_string[num - 1];
                    room.SendLog(log);

                    log = new LogMessage
                    {
                        Type = "$BuquDuplicateItem",
                        From = player.Name,
                        Card_str = card_id.ToString()
                    };
                    room.SendLog(log);
                    break;
                }
            }
            if (duplicate)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, null, Name, null);
                List<int> ints = new List<int> { id };
                room.ThrowCard(ref ints, reason, null);
                Thread.Sleep(1000);
            }
            else
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1 - player.Hp,
                    Who = player,
                };
                room.Recover(player, recover, true);
            }

            return false;
        }
    }
    public class BuquClear : DetachEffectSkill
    {
        public BuquClear() : base("buqu", "buqu")
        {
            frequency = Frequency.Compulsory;
        }
    }
    public class Fenji : TriggerSkill
    {
        public Fenji() : base("fenji")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player != null && player.Phase == PlayerPhase.Finish && player.IsKongcheng())
            {
                List<Player> zhoutais = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in zhoutais)
                {
                    triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.IsKongcheng() && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, new DrawCardStruct(2, ask_who, Name));
            room.LoseHp(ask_who);
            return false;
        }
    }
    public class HaoshiCard : SkillCard
    {
        public static string ClassName = "HaoshiCard";
        public HaoshiCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0 || to_select == Self)
                return false;

            return to_select.HandcardNum == Self.GetMark("haoshi");
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, card_use.From.Name,
                card_use.To[0].Name, "haoshi", null);

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            room.MoveCardTo(card_use.Card, card_use.To[0], Place.PlaceHand, reason);
        }
    }
    public class HaoshiViewAsSkill : ViewAsSkill
    {
        public HaoshiViewAsSkill() : base("haoshi")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@haoshi!";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (player.HasEquip(to_select.Name))
                return false;

            int length = player.HandcardNum / 2;
            return selected.Count < length;
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count != player.HandcardNum / 2)
                return null;

            WrappedCard card = new WrappedCard(HaoshiCard.ClassName)
            {
                Skill = "_haoshi",
                Mute = true
            };
            card.AddSubCards(cards);
            return card;
        }
    }
    public class Haoshi : DrawCardsSkill
    {
        public Haoshi() : base("haoshi")
        {
            view_as_skill = new HaoshiViewAsSkill();
            skill_type = SkillType.Replenish;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
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
    public class HaoshiGive : TriggerSkill
    {
        public HaoshiGive() : base("#haoshi-give")
        {
            events.Add(TriggerEvent.AfterDrawNCards);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player lusu, ref object data, Player ask_who)
        {
            if (lusu == null || !lusu.Alive) return new TriggerStruct();
            if (lusu.HasFlag("haoshi"))
            {
                if (lusu.HandcardNum <= 5)
                {
                    lusu.SetFlags("-haoshi");
                    return new TriggerStruct();
                }
                TriggerStruct trigger = new TriggerStruct(Name, lusu)
                {
                    SkillPosition = (string)lusu.GetTag("haoshi")
                };
                return trigger;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player lusu, ref object data, Player ask_who, TriggerStruct info)
        {
            lusu.SetFlags("-haoshi");
            lusu.RemoveTag("haoshi");
            List<Player> other_players = room.GetOtherPlayers(lusu);
            int least = 1000;
            foreach (Player player in other_players)
                least = Math.Min(player.HandcardNum, least);
            lusu.SetMark("haoshi", least);

            if (room.AskForUseCard(lusu, "@@haoshi!", "@haoshi", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition) == null)
            {
                // force lusu to give his half cards
                Player beggar = null;
                foreach (Player player in other_players)
                {
                    if (player.HandcardNum == least)
                    {
                        beggar = player;
                        break;
                    }
                }

                int n = lusu.HandcardNum / 2;
                List<int> to_give = new List<int>(), hands = lusu.GetCards("h");
                for (int i = 0; i < n; i++)
                    to_give.Add(hands[i]);

                ResultStruct result = lusu.Result;
                result.Assist += to_give.Count;
                lusu.Result = result;

                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, lusu.Name, lusu.Name, "haoshi", null);
                CardsMoveStruct move = new CardsMoveStruct(to_give, beggar, Place.PlaceHand, reason);
                List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
                room.MoveCardsAtomic(moves, false);
            }
            return false;
        }
    }
    public class DimengCard : SkillCard
    {
        public DimengCard() : base("DimengCard") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (to_select == Self)
                return false;

            if (targets.Count == 0)
                return true;

            if (targets.Count == 1)
            {
                return Math.Abs(to_select.HandcardNum - targets[0].HandcardNum) == card.SubCards.Count;
            }

            return false;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card) => targets.Count == 2;
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player a = card_use.To[0];
            Player b = card_use.To[1];
            a.SetFlags("DimengTarget");
            b.SetFlags("DimengTarget");
            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player a = card_use.To[0];
            Player b = card_use.To[1];

            int n1 = a.HandcardNum;
            int n2 = b.HandcardNum;

            ResultStruct result = card_use.From.Result;
            result.Assist += Math.Abs(n1 - n2);
            card_use.From.Result = result;

            CardsMoveStruct move1 = new CardsMoveStruct(a.GetCards("h"), b, Place.PlaceHand,
                new CardMoveReason(MoveReason.S_REASON_SWAP, a.Name, b.Name, "dimeng", null));
            CardsMoveStruct move2 = new CardsMoveStruct(b.GetCards("h"), a, Place.PlaceHand,
                new CardMoveReason(MoveReason.S_REASON_SWAP, b.Name, a.Name, "dimeng", null));
            List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct> { move1, move2 };
            room.MoveCards(exchangeMove, false);

            LogMessage log = new LogMessage
            {
                Type = "#Dimeng",
                From = a.Name,
                To = new List<string> { b.Name },
                Arg = n1.ToString(),
                Arg2 = n2.ToString()
            };
            room.SendLog(log);
            a.SetFlags("-DimengTarget");
            b.SetFlags("-DimengTarget");

            Thread.Sleep(500);
        }
    }
    public class Dimeng : ViewAsSkill
    {
        public Dimeng() : base("dimeng")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !RoomLogic.IsJilei(room, player, to_select);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            WrappedCard card = new WrappedCard("DimengCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            card.AddSubCards(cards);
            return card;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("DimengCard");
        }
    }
    public class ZhijianCard : SkillCard
    {
        public ZhijianCard() : base("ZhijianCard")
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
            return to_select.GetEquip(equip_index) < 0 && RoomLogic.CanPutEquip(to_select, equip_card);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            ResultStruct result = card_use.From.Result;
            result.Assist++;
            card_use.From.Result = result;

            room.MoveCardTo(card_use.Card, card_use.From, card_use.To[0], Place.PlaceEquip,
                new CardMoveReason(MoveReason.S_REASON_PUT, card_use.From.Name, "zhijian", null));

            LogMessage log = new LogMessage
            {
                Type = "$ZhijianEquip",
                From = card_use.To[0].Name,
                Card_str = card_use.Card.GetEffectiveId().ToString()
            };
            room.SendLog(log);
            
            base.Use(room, card_use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.DrawCards(effect.From, 1, "zhijian");
        }
    }
    public class Zhijian : OneCardViewAsSkill
    {
        public Zhijian() : base("zhijian")
        {
            filter_pattern = "EquipCard|.|.|hand";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard zhijian_card = new WrappedCard("ZhijianCard");
            zhijian_card.AddSubCard(card);
            zhijian_card.Skill = Name;
            zhijian_card.ShowSkill = Name;
            return zhijian_card;
        }
    }
    public class Guzheng : TriggerSkill
    {
        public Guzheng() : base("guzheng")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.Discard
                && move.From == room.Current && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
            {
                List<Player> erzhangs = RoomLogic.FindPlayersBySkillName(room, Name);
                bool check = false;
                foreach (Player p in erzhangs)
                {
                    if (move.From != p)
                    {
                        check = true;
                        break;
                    }
                }
                if (!check) return;

                List<int> guzhengToGet = move.From.ContainsTag("GuzhengToGet") ? (List<int>)move.From.GetTag("GuzhengToGet") : new List<int>();
                foreach (int card_id in move.Card_ids)
                {
                    if (!guzhengToGet.Contains(card_id))
                        guzhengToGet.Add(card_id);
                }

                move.From.SetTag("GuzhengToGet", guzhengToGet);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player != null && triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && player.Alive)
            {
                List<int> cardsToGet = player.ContainsTag("GuzhengToGet") ? (List<int>)player.GetTag("GuzhengToGet") : new List<int>();
                if (cardsToGet.Count > 0)
                {
                    List<Player> erzhangs = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player erzhang in erzhangs)
                    {
                        if (player != erzhang)
                            skill_list.Add(new TriggerStruct(Name, erzhang));
                    }
                }
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player erzhang, TriggerStruct info)
        {
            List<int> cardsToGet = player.ContainsTag("GuzhengToGet") ? (List<int>)player.GetTag("GuzhengToGet") : new List<int>();
            List<int> cards = new List<int>();
            foreach (int id in cardsToGet)
            {
                if (room.GetCardPlace(id) == Place.DiscardPile)
                    cards.Add(id);
            }
            if (cards.Count > 0)
            {
                List<int> result = room.NotifyChooseCards(erzhang, cards, Name, 1, 0, "@guzheng:" + player.Name, null, info.SkillPosition);
                if (result.Count > 0)
                {
                    ResultStruct assit = erzhang.Result;
                    assit.Assist++;
                    erzhang.Result = assit;

                    room.BroadcastSkillInvoke(Name, erzhang, info.SkillPosition);
                    room.NotifySkillInvoked(erzhang, Name);
                    int to_back = result[0];
                    room.ObtainCard(player, room.GetCard(to_back));
                    cards.Remove(to_back);
                    erzhang.SetTag("GuzhengCards", cards);
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            List<int> cards = (List<int>)player.GetTag("GuzhengCards");
            player.RemoveTag("GuzhengCards");
            if (cards.Count > 0 && room.AskForSkillInvoke(player, Name, "#GuzhengObtain", info.SkillPosition))
                room.ObtainCard(player, ref cards, new CardMoveReason(MoveReason.S_REASON_GOTBACK, player.Name));

            return false;
        }
    }
    public class GuzhengRemove : TriggerSkill
    {
        public GuzhengRemove() : base("#guzheng-remove")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd };
        }
        public override int GetPriority() => -1;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player != null && player.Phase == PlayerPhase.Discard)
                player.RemoveTag("GuzhengToGet");
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    public class Duanbing : TriggerSkill
    {
        public Duanbing() : base("duanbing")
        {
            events.Add(TriggerEvent.CardTargetAnnounced);
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> selected = new List<Player>(use.To);
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (!use.To.Contains(p) && RoomLogic.DistanceTo(room, player, p, use.Card) == 1 && fcard.ExtratargetFilter(room, selected, p, player, use.Card))
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            List<Player> targets = new List<Player>();
            List<Player> selected = new List<Player>(use.To);
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            foreach (Player p in room.GetOtherPlayers(player))
                if (!use.To.Contains(p) && RoomLogic.DistanceTo(room, player, p, use.Card) == 1 && fcard.ExtratargetFilter(room, selected, p, player, use.Card))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                room.SetTag("extra_target_skill", data);                   //for AI
                Player target = room.AskForPlayerChosen(player, targets, Name, string.Format("@duanbing-target:::{0}", use.Card.Name), true, false, info.SkillPosition);
                room.RemoveTag("extra_target_skill");
                if (target != null)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.NotifySkillInvoked(player, Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        To = new List<string> { target.Name },
                        Card_str = RoomLogic.CardToString(room, use.Card),
                        Arg = Name
                    };
                    room.SendLog(log);

                    player.SetTag("extra_targets", target.Name);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag("extra_targets"));
            player.RemoveTag("extra_targets");

            if (target != null && data is CardUseStruct use)
            {
                use.To.Add(target);
                room.SortByActionOrder(ref use);
                data = use;
            }

            return false;
        }
    }
    public class FenxunCard : SkillCard
    {
        public FenxunCard() : base("FenxunCard")
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            effect.From.SetFlags("FenxunInvoker");
            effect.To.SetFlags("FenxunTarget");
        }
    }

    public class Fenxun : OneCardViewAsSkill
    {
        public Fenxun() : base("fenxun")
        {
            filter_pattern = "..!";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "he") && !player.HasUsed("FenxunCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard("FenxunCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            first.AddSubCard(card);
            return first;
        }
    }
    public class FenxunDistance : DistanceSkill
    {
        public FenxunDistance() : base("fenxun-distance")
        {
        }
        public override int GetFixed(Room room, Player from, Player to)
        {
            if (from.HasFlag("FenxunInvoker") && to.HasFlag("FenxunTarget"))
                return 1;
            else
                return 0;
        }
    }

    #endregion
}