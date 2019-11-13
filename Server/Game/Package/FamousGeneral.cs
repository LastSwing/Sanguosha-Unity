using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class FamousGeneral : GeneralPackage
    {
        public FamousGeneral() : base("FamousGeneral")
        {
            skills = new List<Skill>
            {
                new Yanyu(),
                new Qiaoshi(),
                new ZhimanJX(),
                new ZhimanJXSecond(),
                new EnyuanJX(),
                new XuanhuoJX(),
                new Fumian(),
                new Daiyan(),
                new Qiangzhi(),
                new Xiantu(),
                new Dangxian(),
                new DangxianJX(),
                new Fuli(),
                new Duliang(),
                new DuliangDraw(),
                new Fulin(),
                new FulinMax(),
                new Fuhun(),
                new Longyin(),
                new Wuyan(),
                new Jujian(),
                new Benxi(),
                new BenxiDistance(),
                new Xiansi(),
                new XiansiVS(),
                new XiansiTar(),
                new Wurong(),
                new Shizhi(),
                new Zhanjue(),
                new Qinwang(),

                new Jianying(),
                new JianyingRecord(),
                new Shibei(),
                new Jigong(),
                new JigongMax(),
                new Shifei(),
                new Qieting(),
                new Xianzhou(),
                new Mingce(),
                new Zhichi(),
                new Zishou(),
                new ZishouProhibit(),
                new Zongshi(),
                new ZongshiMax(),
                new Juece(),
                new Mieji(),
                new Fencheng(),
                new Shouxi(),
                new Huimin(),
                new Xianzhen(),
                new XianzhenTar(),
                new Jinjiu(),
                new Zhige(),
                new Zongzuo(),
                new Huaiyi(),
                new Yaowu(),

                new Zhenjun(),
                new Shenduan(),
                new ShenduanClear(),
                new Yonglue(),
                new Jueqing(),
                new Shangshi(),
                new Luoying(),
                new Jiushi(),
                new Huomo(),
                new Zuoding(),
                new Huituo(),
                new Mingjian(),
                new MingjianTar(),
                new MingjianMax(),
                new Xingshuai(),
                new Zhenlie(),
                new Miji(),
                new Quanji(),
                new QuanjiMax(),
                new Zili(),
                new Paiyi(),
                new Chengxiang(),
                new Renxin(),
                new Jingce(),
                new Qingxi(),
                new Qianju(),
                new Jiangchi(),
                new JiangchiMod(),
                new QiceJX(),
                new Sidi(),
                new SidiSlash(),
                new Faen(),
                new Pindi(),

                new BuyiJX(),
                new Anxu(),
                new Zhuiyi(),
                new Shenxing(),
                new Bingyi(),
                new Anguo(),
                new Wengua(),
                new WenguaAttach(),
                new Fuzhu(),
                new Zenhui(),
                new Jiaojin(),
                new Xuanfeng(),
                new XuanfengClear(),
                new Yaoming(),
                new Kuangbi(),
                new KuangbiClear(),
                new Fenli(),
                new Pingkou(),
                new Lihuo(),
                new Chunlao(),
                new Pojun(),
                new Gongqi(),
                new Jiefan(),
                new Danshou(),
                new Duodao(),
                new Anjian(),
            };

            skill_cards = new List<FunctionCard>
            {
                new YanyuCard(),
                new ShifeiCard(),
                new JiushiCard(),
                new AnxuCard(),
                new XianzhouCard(),
                new HuomoCard(),
                new MingjianCard(),
                new MijiCard(),
                new MingceCard(),
                new PaiyiCard(),
                new ShenxingCard(),
                new AnguoCard(),
                new MiejiCard(),
                new FenchengCard(),
                new RenxinCard(),
                new WenguaCard(),
                new XianzhenCard(),
                new DuliangCard(),
                new KuangbiCard(),
                new HuaiyiCard(),
                new ZhigeCard(),
                new QingxiCard(),
                new JujianCard(),
                new GongqiCard(),
                new JiefanCard(),
                new ChunlaoCard(),
                new PindiCard(),
                new XiansiCard(),
                new WurongCard(),
                new QinwangCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "zhiman_jx", new List<string> { "#zhiman_jx-second" } },
                { "jianying", new List<string> { "#jianying-record" } },
                { "shenduan", new List<string>{ "#shenduan-clear" } },
                { "mingjian", new List<string>{ "#mingjian-tar", "#mingjian-max" } },
                { "quanji", new List<string>{ "#quanji-max" } },
                { "zishou", new List<string>{ "#zishou-prohibit" } },
                { "zongshi", new List<string>{ "#zongshi-max" } },
                { "xianzhen", new List<string>{ "#xianzhen-tar" } },
                { "xuanfeng", new List<string>{ "#xuanfeng-clear" } },
                { "duliang", new List<string>{ "#duliang-draw" } },
                { "fulin", new List<string>{ "#fulin-max" } },
                { "kuangbi", new List<string> { "#kuangbi-clear" } },
                { "jiangchi", new List<string> { "#jiangchi-target" } },
                { "benxi", new List<string> { "#benxi" } },
                { "sidi", new List<string> { "#sidi-slash" } },
            };
        }
    }

    public class YanyuCard : SkillCard
    {
        public static string ClassName = "YanyuCard";
        public YanyuCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodRecast;
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            card_use.From.AddMark("yanyu");
            DoRecast(room, card_use);
        }
    }

    public class YanyuVS : OneCardViewAsSkill
    {
        public YanyuVS() : base("yanyu")
        {
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return room.GetCardPlace(to_select.Id) == Place.PlaceHand && !RoomLogic.IsCardLimited(room, player, to_select, FunctionCard.HandlingMethod.MethodRecast, true)
                && to_select.Name.Contains(Slash.ClassName);
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard yy = new WrappedCard(YanyuCard.ClassName)
            {
                Skill = Name
            };
            yy.AddSubCard(card);
            return yy;
        }
    }

    public class Yanyu : TriggerSkill
    {
        public Yanyu() : base("yanyu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
            view_as_skill = new YanyuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play && player.GetMark(Name) >= 2)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.IsMale()) targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@yanyu", true, true, info.SkillPosition);
                if (target != null)
                {
                    player.SetTag(Name, target.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag(Name));
            player.RemoveTag(Name);
            room.DrawCards(target, new DrawCardStruct(2, player, Name));
            return false;
        }
    }

    public class Qiaoshi : TriggerSkill
    {
        public Qiaoshi() : base("qiaoshi")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (player.Phase == PlayerPhase.Finish)
            {
                List<Player> xiahoushi = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in xiahoushi)
                    if (p.HandcardNum == player.HandcardNum && p != player)
                        result.Add(new TriggerStruct(Name, p));
            }
            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive && ask_who.HandcardNum == player.HandcardNum
                && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, new DrawCardStruct(1, ask_who, Name));
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    public class ZhimanJX : TriggerSkill
    {
        public ZhimanJX() : base("zhiman_jx")
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
                Type = "#damage-prevent",
                From = player.Name,
                To = new List<string> { to.Name },
                Arg = Name
            };
            room.SendLog(log);

            to.SetMark(Name, 1);
            to.SetTag("zhiman_from", player.Name);
            return true;
        }
    }
    public class ZhimanJXSecond : TriggerSkill
    {
        public ZhimanJXSecond() : base("#zhiman_jx-second")
        {
            events.Add(TriggerEvent.DamageComplete);
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            DamageStruct damage = (DamageStruct)data;
            if (damage.To == player && player.GetMark("zhiman_jx") > 0)
            {
                Player masu = room.FindPlayer((string)player.GetTag("zhiman_from"));
                if (damage.From == masu && RoomLogic.PlayerHasShownSkill(room, masu, "zhiman_jx"))
                {
                    List<TriggerStruct> skill_list = new List<TriggerStruct>();
                    TriggerStruct trigger = new TriggerStruct(Name, masu)
                    {
                        SkillPosition = (string)masu.GetTag("zhiman_jx")
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
            to.SetMark("zhiman_jx", 0);
            if (RoomLogic.CanGetCard(room, player, to, "ej"))
            {
                int card_id = room.AskForCardChosen(player, to, "ej", "zhiman_jx", false, HandlingMethod.MethodGet);
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, to.Name, "zhiman_jx", string.Empty);
                room.ObtainCard(player, room.GetCard(card_id), reason);
            }

            return false;
        }
    }

    public class XuanhuoJX : PhaseChangeSkill
    {
        public XuanhuoJX() : base("xuanhuo_jx")
        {
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Draw && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@xuanhuo_jx", true, true, info.SkillPosition);
            if (target != null)
            {
                player.SetTag(Name, target.Name);
                room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag(Name));
            room.DrawCards(target, new DrawCardStruct(2, player, Name));
            if (player.Alive && target.Alive)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(target))
                    if (RoomLogic.InMyAttackRange(room, target, p)) targets.Add(p);

                bool get = true;
                if (targets.Count > 0)
                {
                    target.SetFlags("xuanhuo_source");
                    Player victim = room.AskForPlayerChosen(player, targets, Name, "@xuanhuo-victim:" + target.Name, false, false, info.SkillPosition);
                    target.SetFlags("-xuanhuo_source");
                    room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, target.Name, victim.Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "#CollateralSlash",
                        From = player.Name,
                        To = new List<string> { victim.Name }
                    };
                    room.SendLog(log);
                    if (room.AskForUseSlashTo(target, victim, string.Format("@xuanhuo-slash:{0}:{1}", player.Name, victim.Name)) != null)
                        get = false;
                }

                if (get && target.Alive && !target.IsNude())
                {
                    List<int> ids = room.AskForCardsChosen(player, target, new List<string>{ "he^false^get", "he^false^get" }, Name);
                    if (ids.Count > 0)
                        room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, Name, string.Empty), false);
                }
            }

            return true;
        }
    }
    public class EnyuanJX : TriggerSkill
    {
        public EnyuanJX() : base("enyuan_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.Damaged };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From != null && damage.From != player && damage.From.Alive && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.To != null && move.From.Alive
                && base.Triggerable(move.To, room) && move.To_place == Place.PlaceHand && move.Card_ids.Count >= 2)
                return new TriggerStruct(Name, move.To);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = false;
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From.Alive)
            {
                move.From.SetFlags("en_target");
                invoke = room.AskForSkillInvoke(ask_who, "enyuan_jx_en", move.From, info.SkillPosition);
                move.From.SetFlags("-en_target");
                if (invoke)
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, ask_who.Name, move.From.Name);
                }
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From.Alive)
            {
                invoke = room.AskForSkillInvoke(player, "enyuan_jx_yuan", damage.From, info.SkillPosition);
                if (invoke)
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                }
            }

            return invoke ? info : new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From.Alive)
            {
                LogMessage log = new LogMessage
                {
                    Type = "$enyuan_en",
                    From = ask_who.Name,
                    To = new List<string> { move.From.Name }
                };
                room.SendLog(log);
                room.DrawCards(move.From, new DrawCardStruct(1, ask_who, Name));
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From != null && damage.From != player && damage.From.Alive)
            {
                int result = -1;
                if (damage.From.HandcardNum > 0)
                {
                    List<int> ids = room.AskForExchange(damage.From, "enyuan_jx_yuan", 1, 0, "@enyuan:" + player.Name, string.Empty, ".", string.Empty);
                    if (ids.Count == 1)
                        result = ids[0];
                }
                if (result != -1)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "$enyuan_yuan1",
                        From = player.Name,
                        To = new List<string> { damage.From.Name }
                    };
                    room.SendLog(log);
                    List<int> ids = new List<int> { result };
                    room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, damage.From.Name, player.Name, Name), false);
                }
                else
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "$enyuan_yuan2",
                        From = player.Name,
                        To = new List<string> { damage.From.Name }
                    };
                    room.SendLog(log);
                    room.LoseHp(damage.From);
                }
            }

            return false;
        }
    }

    public class Fumian : TriggerSkill
    {
        public Fumian() : base("fumian")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardTargetAnnounced, TriggerEvent.EventPhaseProceeding, TriggerEvent.EventPhaseChanging };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player.GetMark(Name) > 2
                && WrappedCard.IsRed(use.Card.Suit) && player.GetMark("fumian_target") == 0)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if ((fcard is BasicCard || fcard is TrickCard) && !(fcard is DelayedTrick) && !(fcard is Collateral))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && player.Phase == PlayerPhase.Draw && data is int count && count > 0         //摸排阶段增加摸排量
                && (player.GetMark(Name) == 1 || player.GetMark(Name) == 2))
            {
                count += player.GetMark(Name);
                data = count;
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change                                        //准备阶段因未拥有技能，清除标记
                && change.From == PlayerPhase.Start && player.GetMark(Name) > 0 && !player.HasFlag(Name))
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                player.SetFlags(Name);
                List<string> prompts = new List<string> { string.Empty };
                int mark = player.GetMark(Name);
                if (mark == 0 && player.GetMark("TurnPlay") > 0 || mark == 3)
                {
                    prompts.Add(string.Format("@fumian-draw:::{0}", 2));
                }
                else
                {
                    prompts.Add(string.Format("@fumian-draw:::{0}", 1));
                }
                if (mark == 0 && player.GetMark("TurnPlay") > 0 || mark == 1)
                {
                    prompts.Add(string.Format("@fumian-target:::{0}", 2));
                }
                else
                {
                    prompts.Add(string.Format("@fumian-target:::{0}", 1));
                }

                string choice = room.AskForChoice(player, Name, "draw+target+cancel", prompts);
                bool invoke = false;
                if (choice == "draw")
                {
                    int count = (mark == 0 && player.GetMark("TurnPlay") > 0 || mark == 3) ? 2 : 1;
                    player.SetMark(Name, count);
                    invoke = true;
                }
                else if (choice == "target")
                {
                    player.SetMark(Name, (mark == 0 && player.GetMark("TurnPlay") > 0 || mark == 1) ? 4 : 3);
                    player.SetMark("fumian_target", 0);
                    invoke = true;
                }
                else
                    player.SetMark(Name, 0);

                if (invoke)
                {
                    room.NotifySkillInvoked(player, Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced)
                return info;

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (!use.To.Contains(p) && RoomLogic.IsProhibited(room, use.From, p, use.Card, use.To) == null)
                    {
                        if ((fcard is Slash && player == p) || (fcard is Peach && !p.IsWounded()) || (fcard is Snatch && !RoomLogic.CanGetCard(room, player, p, "hej"))
                            || (fcard is Dismantlement && !RoomLogic.CanDiscard(room, player, p, "hej")) || (fcard is FireAttack && p.IsKongcheng()))
                            continue;

                        targets.Add(p);
                    }
                }

                if (targets.Count > 0)
                {
                    int count = player.GetMark(Name) == 4 ? 2 : 1;
                    room.SetTag(Name, data);
                    List<Player> players = room.AskForPlayersChosen(player, targets, Name, 0, count, string.Format("@fumian-target:::{0}:{1}", use.Card.Name, count), true, info.SkillPosition);
                    room.RemoveTag(Name);
                    if (players.Count > 0)
                    {
                        player.SetMark("fumian_target", 1);
                        use.To.AddRange(players);

                        LogMessage log = new LogMessage
                        {
                            Type = "$extra_target",
                            From = player.Name,
                            Card_str = RoomLogic.CardToString(room, use.Card),
                            Arg = Name
                        };
                        log.SetTos(players);
                        room.SendLog(log);

                        data = use;
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                int count = 1;
                if (player.GetMark(Name) == 2 || player.GetMark(Name) == 4)
                    count = 2;
                LogMessage log = new LogMessage
                {
                    Type = "#fumian-target",
                    From = player.Name,
                    Arg = count.ToString()
                };
                if (player.GetMark(Name) > 2)
                    log.Type = "#fumian-draw";

                room.SendLog(log);
            }

            return false;
        }
    }

    public class Daiyan : TriggerSkill
    {
        public Daiyan() : base("daiyan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change                                        //结束阶段因未拥有技能，清除标记
                && change.From == PlayerPhase.Finish && player.ContainsTag(Name) && !player.HasFlag(Name))
                player.RemoveTag(Name);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && base.Triggerable(player, room) ?
                new TriggerStruct(Name, player) : new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@daiyan", true, true, info.SkillPosition);
            if (target != null)
            {
                player.SetTag("daiyan_target", target.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            player.RemoveTag(Name);
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag("daiyan_target"));
            foreach (int id in room.DrawPile)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Suit == WrappedCard.CardSuit.Heart && Engine.GetFunctionCard(card.Name) is BasicCard)
                {
                    room.ObtainCard(target, id, true);
                    break;
                }
            }

            if (target.Alive && player.ContainsTag(Name) && (string)player.GetTag(Name) == target.Name)
                room.LoseHp(target);

            player.SetTag(Name, target.Name);

            return false;
        }
    }

    public class Qiangzhi : TriggerSkill
    {
        public Qiangzhi() : base("qiangzhi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging, TriggerEvent.CardResponded, TriggerEvent.CardUsed };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }

        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play)
                return new TriggerStruct(Name, player);
            else if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded) && player.GetMark("qiangzhi") > 0 && player.Alive)
            {
                FunctionCard fcard = null;
                if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use)
                    fcard = Engine.GetFunctionCard(resp.Card.Name);
                else if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    fcard = Engine.GetFunctionCard(use.Card.Name);

                int mark = 0;
                string _mark = string.Empty;
                if (fcard is BasicCard)
                {
                    mark = 1;
                }
                else if (fcard is TrickCard)
                {
                    mark = 2;
                }
                else
                {
                    mark = 3;
                }

                if (player.GetMark("qiangzhi") == mark)
                {
                    TriggerStruct trigger = new TriggerStruct(Name, player)
                    {
                        SkillPosition = (string)player.GetTag("qiangzhi_position")
                    };
                    return trigger;
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                    if (!p.IsKongcheng()) targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@qiangzhi", true, true, info.SkillPosition);
                    if (target != null)
                    {
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                        room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                        player.SetTag(Name, target.Name);
                        return info;
                    }
                }
            }
            else if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                Player target = room.FindPlayer((string)player.GetTag(Name));
                player.RemoveTag(Name);
                int id = room.AskForCardChosen(player, target, "h", Name);
                room.ShowCard(target, id, Name);
                FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                int mark = 0;
                string _mark = string.Empty;
                if (fcard is BasicCard)
                {
                    mark = 1;
                    _mark = "BasicCard";
                }
                else if (fcard is TrickCard)
                {
                    mark = 2;
                    _mark = "TrickCard";
                }
                else
                {
                    mark = 3;
                    _mark = "EquipcCard";
                }

                player.SetMark(Name, mark);
                player.SetTag("qiangzhi_position", info.SkillPosition);
                room.SetPlayerStringMark(player, Name, _mark);
            }
            else
            {
                room.DrawCards(player, 1, "qiangzhi");
            }

            return false;
        }
    }

    public class Xiantu : TriggerSkill
    {
        public Xiantu() : base("xiantu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Death, TriggerEvent.EventPhaseEnd };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Damage.From != null && death.Damage.From.Phase == PlayerPhase.Play)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.ContainsTag(Name) && p.GetTag(Name) is string target_name && target_name == death.Damage.From.Name)
                        p.RemoveTag(Name);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play)
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.ContainsTag(Name))
                    {
                        p.RemoveTag(Name);
                        room.LoseHp(p);
                    }
                }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggerStructs = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play)
            {
                List<Player> zhangsong = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in zhangsong)
                    if (p != player)
                        triggerStructs.Add(new TriggerStruct(Name, p));
            }
            return triggerStructs;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, 2, Name);
            if (!ask_who.IsNude() && player.Alive)
            {
                List<int> ids = room.AskForExchange(ask_who, Name, 2, 2, "@xiantu-give:" + player.Name, string.Empty, "..", info.SkillPosition);
                room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, ask_who.Name, player.Name, Name, string.Empty), false);
                ask_who.SetTag(Name, player.Name);
            }

            return false;
        }
    }

    public class Dangxian : PhaseChangeSkill
    {
        public Dangxian() : base("dangxian") { frequency = Frequency.Compulsory; }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Start)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            player.Phase = PlayerPhase.Play;
            room.BroadcastProperty(player, "Phase");
            RoomThread thread = room.RoomThread;
            if (!thread.Trigger(TriggerEvent.EventPhaseStart, room, player))
                thread.Trigger(TriggerEvent.EventPhaseProceeding, room, player);
            thread.Trigger(TriggerEvent.EventPhaseEnd, room, player);

            player.Phase = PlayerPhase.RoundStart;
            room.BroadcastProperty(player, "Phase");

            return false;
        }
    }

    public class DangxianJX : PhaseChangeSkill
    {
        public DangxianJX() : base("dangxian_jx")
        {
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && (player.Phase == PlayerPhase.Start || (player.Phase == PlayerPhase.Play && !player.HasFlag(Name))))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Phase == PlayerPhase.Start) return info;
            if (player.Phase == PlayerPhase.Play)
            {
                player.SetFlags(Name);
                if (player.GetMark(Name) == 0 || room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
                    return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            if (player.Phase == PlayerPhase.Start)
            {
                room.SendCompulsoryTriggerLog(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

                player.Phase = PlayerPhase.Play;
                room.BroadcastProperty(player, "Phase");
                RoomThread thread = room.RoomThread;
                if (!thread.Trigger(TriggerEvent.EventPhaseStart, room, player))
                    thread.Trigger(TriggerEvent.EventPhaseProceeding, room, player);
                thread.Trigger(TriggerEvent.EventPhaseEnd, room, player);

                player.Phase = PlayerPhase.RoundStart;
                room.BroadcastProperty(player, "Phase");
            }
            else
            {
                List<int> ids = new List<int>();
                foreach (int id in room.DiscardPile)
                {
                    if (room.GetCard(id).Name.Contains(Slash.ClassName))
                        ids.Add(id);
                }
                if (ids.Count > 0)
                {
                    room.SendCompulsoryTriggerLog(player, Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

                    Shuffle.shuffle(ref ids);
                    List<int> got = new List<int> { ids[0] };
                    room.LoseHp(player);
                    if (player.Alive)
                        room.ObtainCard(player, ref got, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTBACK, player.Name, Name, string.Empty));
                }
            }

            return false;
        }
    }

    public class Fuli : TriggerSkill
    {
        public Fuli() : base("fuli")
        {
            events.Add(TriggerEvent.AskForPeaches);
            frequency = Frequency.Limited;
            skill_type = SkillType.Recover;
            limit_mark = "@fuli";
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DyingStruct dying && dying.Who == player && base.Triggerable(player, room) && player.GetMark(limit_mark) > 0 && player.Hp <= 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public static int GetKingdoms(Room room)
        {
            List<string> kingdom_set = new List<string>();
            foreach (Player p in room.GetAlivePlayers())
                if (!kingdom_set.Contains(p.Kingdom))
                    kingdom_set.Add(p.Kingdom);

            return kingdom_set.Count;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.SetPlayerMark(player, limit_mark, 0);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.DoSuperLightbox(player, info.SkillPosition, Name);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = GetKingdoms(room);
            player.AddMark("dangxian_jx");
            RecoverStruct recover = new RecoverStruct
            {
                Who = player,
                Recover = count - player.Hp
            };
            room.Recover(player, recover, true);
            int draw = count - player.HandcardNum;
            if (draw > 0)
                room.DrawCards(player, draw, Name);

            if (player.Alive && count >= 3)
                room.TurnOver(player);

            return false;
        }
    }

    public class Duliang : ZeroCardViewAsSkill
    {
        public Duliang() : base("duliang")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(DuliangCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(DuliangCard.ClassName) { Skill = Name };
        }
    }

    public class DuliangCard : SkillCard
    {
        public static string ClassName = "DuliangCard";
        public DuliangCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && !to_select.IsKongcheng() && RoomLogic.CanGetCard(room, Self, to_select, "h");
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            int id = room.AskForCardChosen(player, target, "h", "duliang", false, HandlingMethod.MethodGet);
            List<int> ids = new List<int> { id };
            room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, "duliang", string.Empty), false);

            if (room.AskForChoice(player, "duliang", "view+more", new List<string> { "@duliang:" + target.Name }, target) == "view")
            {
                List<int> top = room.GetNCards(2, false);

                LogMessage log = new LogMessage
                {
                    Type = "$ViewDrawPile",
                    From = target.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(top))
                };
                room.SendLog(log, target);
                log.Type = "$ViewDrawPile2";
                log.Arg = top.Count.ToString();
                log.Card_str = null;
                room.SendLog(log, new List<Player> { target });

                room.ViewCards(target, top, "duliang");
                List<int> move = new List<int>();
                foreach (int card_id in top)
                {
                    WrappedCard card = room.GetCard(card_id);
                    if (Engine.GetFunctionCard(card.Name) is BasicCard)
                        move.Add(card_id);
                }
                if (move.Count > 0)
                    room.ObtainCard(target, ref move, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTCARD, target.Name, "duliang", string.Empty), false);
            }
            else
                target.AddMark("duliang");
        }
    }

    public class DuliangDraw : DrawCardsSkill
    {
        public DuliangDraw() : base("#duliang-draw") { frequency = Frequency.Compulsory; }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == Player.PlayerPhase.Draw && (int)data >= 0 && player.GetMark("duliang") > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override int GetDrawNum(Room room, Player player, int n)
        {
            int count = player.GetMark("duliang");
            player.SetMark("duliang", 0);
            return n + count;
        }
    }

    public class Fulin : TriggerSkill
    {
        public Fulin() : base("fulin")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging, TriggerEvent.EventPhaseStart };
            frequency = Frequency.Compulsory;
        }
        public override bool CanPreShow() => false;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (int id in player.GetCards("h"))
                    if (room.GetCard(id).HasFlag(Name))
                        room.SetCardFlag(id, "-fulin");
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && move.From_places.Contains(Place.PlaceHand) && move.From == room.Current)
                {
                    foreach (int id in move.Card_ids)
                        if (room.GetCard(id).HasFlag(Name))
                            room.SetCardFlag(id, "-fulin");
                }
                else if (move.To != null && move.To_place == Place.PlaceHand && base.Triggerable(move.To, room))
                {
                    foreach (int id in move.Card_ids)
                        room.SetCardFlag(id, Name);
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Discard && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.NotifySkillInvoked(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            return new TriggerStruct();
        }
    }

    public class FulinMax : MaxCardsSkill
    {
        public FulinMax() : base("#fulin-max") { }

        public override bool Ingnore(Room room, Player player, int card_id)
        {
            return RoomLogic.PlayerHasShownSkill(room, player, "fulin") && room.GetCard(card_id).HasFlag("fulin");
        }
    }

    public class Fuhun : TriggerSkill
    {
        public Fuhun() : base("fuhun")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageDone, TriggerEvent.EventPhaseChanging };
            view_as_skill = new FuhunVS();
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageDone && data is DamageStruct damage && damage.Card != null
                && damage.Card.Skill == Name && base.Triggerable(damage.From, room) && damage.From.Phase == PlayerPhase.Play)
            {
                damage.From.SetFlags(Name);
                room.HandleAcquireDetachSkills(damage.From, "wusheng_jx|paoxiao_jx", true);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && player.HasFlag(Name))
            {
                room.HandleAcquireDetachSkills(player, "-wusheng_jx|-paoxiao_jx", true);
            }
        }
    }

    public class FuhunVS : ViewAsSkill
    {
        public FuhunVS() : base("fuhun")
        {
            response_or_use = true;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return Slash.IsAvailable(room, player);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return pattern == Slash.ClassName;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < 2 && !player.HasEquip(to_select.Name);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count != 2)
                return null;

            WrappedCard slash = new WrappedCard(Slash.ClassName);
            slash.AddSubCards(cards);
            slash.Skill = Name;
            slash = RoomLogic.ParseUseCard(room, slash);
            return slash;
        }
    }

    public class Longyin : TriggerSkill
    {
        public Longyin() : base("longyin")
        {
            events.Add(TriggerEvent.CardUsed);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardUseStruct use && use.Card != null && use.From.Alive && use.From.Phase == PlayerPhase.Play && use.Card.Name.Contains(Slash.ClassName))
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag("longyin_data", data);
                bool invoke = room.AskForDiscard(ask_who, Name, 1, 1, true, true, "@longyin:" + use.From.Name, true, info.SkillPosition);
                room.RemoveTag("longyin_data");

                if (invoke)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, use.From.Name);
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
                if (use.AddHistory)
                {
                    use.AddHistory = false;
                    player.AddHistory(Slash.ClassName, -1);
                    data = use;
                }

                if (WrappedCard.IsRed(use.Card.Suit) && ask_who.Alive)
                    room.DrawCards(ask_who, 1, Name);
            }

            return false;
        }
    }

    public class Wuyan : TriggerSkill
    {
        public Wuyan() : base("wuyan")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.DamageInflicted };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && base.Triggerable(player, room) && damage.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is TrickCard)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SendCompulsoryTriggerLog(player, Name);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);


                if (triggerEvent == TriggerEvent.DamageInflicted)
                {
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    LogMessage log = new LogMessage
                    {
                        Type = "#damaged-prevent",
                        From = player.Name,
                        Arg = Name
                    };
                    room.SendLog(log);
                }
                else
                {
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    LogMessage log = new LogMessage
                    {
                        Type = "#damage-prevent",
                        From = player.Name,
                        To = new List<string> { damage.To.Name },
                        Arg = Name
                    };
                    room.SendLog(log);
                }

                return true;
            }

            return false;
        }
    }

    public class JujianCard:SkillCard
{
        public static string ClassName = "JujianCard";
        public JujianCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];

            List<string> choicelist = new List<string> { "draw" };
            if (target.IsWounded())
                choicelist.Add("recover");
            if (!target.FaceUp || target.Chained)
                choicelist.Add("reset");
            string choice = room.AskForChoice(target, "jujian", string.Join("+", choicelist));

            if (choice == "draw")
                room.DrawCards(target, new DrawCardStruct(2, player, Name));
            else if (choice == "recover")
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(target, recover, true);
            }
            else if (choice == "reset")
            {
                if (target.Chained)
                    room.SetPlayerChained(target, false, true);
                if (!target.FaceUp)
                    room.TurnOver(target);
            }
        }
    }

    public class JujianVS : OneCardViewAsSkill
    {
        public JujianVS() : base("jujian")
        {
            filter_pattern = "^BasicCard!";
            response_pattern = "@@jujian";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard jujianCard = new WrappedCard(JujianCard.ClassName) { Skill = Name };
            jujianCard.AddSubCard(card);
            return jujianCard;
        }
    }

    public class Jujian : PhaseChangeSkill
    {
        public Jujian() : base("jujian")
        {
            view_as_skill = new JujianVS();
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Finish && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@jujian", "@jujian", -1, HandlingMethod.MethodUse, true, info.SkillPosition);
            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            return false;
        }
    }

    public class Benxi : TriggerSkill
    {
        public Benxi() : base("benxi")
        {
            frequency = Frequency.Compulsory;
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardUsedAnnounced,
                TriggerEvent.CardTargetAnnounced, TriggerEvent.CardResponded, TriggerEvent.EventPhaseChanging,
             TriggerEvent.CardFinished, TriggerEvent.Damage, TriggerEvent.TargetChosen, TriggerEvent.TrickCardCanceling };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use)
            {
                string card_str = RoomLogic.CardToString(room, use.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                if (player.ContainsTag(str))
                {
                    player.RemoveTag(str);
                    foreach (Player p in use.To)
                        if (p.ArmorNullifiedList.ContainsKey(player.Name) && p.ArmorNullifiedList[player.Name] == card_str)
                            p.RemoveArmorNullified(player);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && player == room.Current && data is CardUseStruct use
                && !(Engine.GetFunctionCard(use.Card.Name) is SkillCard))
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && base.Triggerable(player, room) && player == room.Current)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct _use && _use.To.Count == 1 && base.Triggerable(player, room) && player == room.Current)
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                if (fcard is Slash || (fcard is TrickCard && !(fcard is DelayedTrick) && !(fcard is Nullification) && !(fcard is Collateral)))
                {
                    bool check = true;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        int distance = RoomLogic.DistanceTo(room, player, p, null, true);
                        if (distance != 1)
                        {
                            check = false;
                            break;
                        }
                    }

                    if (check)
                        return new TriggerStruct(Name, player);
                }
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct target_use && room.Current == player && player.Alive)
            {
                string card_str = RoomLogic.CardToString(room, target_use.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                if (player.ContainsTag(str) && player.GetTag(str) is List<int> cards && cards.Contains(0))
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct chose_use && room.Current == player && player.Alive)
            {
                string card_str = RoomLogic.CardToString(room, chose_use.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                if (player.ContainsTag(str) && player.GetTag(str) is List<int> cards && (cards.Contains(1) || cards.Contains(2)))
                    return new TriggerStruct(Name, player, chose_use.To);
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && room.Current == effect.From && effect.From.Alive)
            {
                string card_str = RoomLogic.CardToString(room, effect.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                if (effect.From.ContainsTag(str) && effect.From.GetTag(str) is List<int> cards && cards.Contains(2))
                    return new TriggerStruct(Name, effect.From);
            }
            else if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.Card != null && room.Current == player && player.Alive)
            {
                string card_str = RoomLogic.CardToString(room, damage.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                if (player.ContainsTag(str) && player.GetTag(str) is List<int> cards && cards.Contains(3))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);

            if (triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded)
            {
                room.SendCompulsoryTriggerLog(player, Name);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                player.AddMark(Name);
                room.SetPlayerStringMark(player, Name, player.GetMark(Name).ToString());
            }
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use)
            {
                string card_str = RoomLogic.CardToString(room, use.Card);
                List<string> choices = new List<string> { "more", "armor", "nullified", "draw", "cancel" };
                int count = 0;
                List<int> cards = new List<int>();
                while (count < 2)
                {
                    string choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { "@benxi:::" + use.Card.Name }, data);
                    LogMessage log = new LogMessage
                    {
                        From = player.Name,
                        Arg = use.Card.Name
                    };
                    switch (choice)
                    {
                        case "more":
                            cards.Add(0);
                            log.Type = "#benxi-more";
                            break;
                        case "armor":
                            cards.Add(1);
                            log.Type = "#benxi-armor";
                            break;
                        case "nullified":
                            cards.Add(2);
                            log.Type = "#benxi-nullified";
                            break;
                        case "draw":
                            cards.Add(3);
                            log.Type = "#benxi-draw";
                            break;
                        case "cancel":
                            return false;
                    }
                    choices.Remove(choice);
                    room.NotifySkillInvoked(player, Name);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    player.SetTag(string.Format("{0}_{1}", Name, card_str), cards);
                    room.SendLog(log);
                    count++;
                }
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct _use)
            {
                List<Player> targets = new List<Player>();
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if ((fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, player, p))
                        || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && Snatch.Instance.TargetFilter(room, new List<Player>(), p, player, _use.Card))
                        || (fcard is Dismantlement && !RoomLogic.CanDiscard(room, player, p, "hej"))) continue;

                    if (!_use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, _use.Card) == null)
                        targets.Add(p);
                }

                room.SetTag("extra_target_skill", data);                   //for AI
                Player target = room.AskForPlayerChosen(player, targets, Name, "@extra_targets1:::" + _use.Card.Name, true, true, info.SkillPosition);
                room.RemoveTag("extra_target_skill");
                if (target != null)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        To = new List<string> { target.Name },
                        Card_str = RoomLogic.CardToString(room, _use.Card),
                        Arg = Name
                    };
                    room.SendLog(log);

                    _use.To.Add(target);
                    room.SortByActionOrder(ref _use);
                    data = _use;
                }
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling)
            {
                return true;
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct chose_use)
            {
                string card_str = RoomLogic.CardToString(room, chose_use.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                if (ask_who.GetTag(str) is List<int> cards)
                {
                    if (cards.Contains(2))
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

                    if (cards.Contains(1))
                        player.SetAmorNullified2(ask_who, card_str);
                }
            }
            else if (triggerEvent == TriggerEvent.Damage)
                room.DrawCards(player, 1, Name);

            return false;
        }
    }

    public class BenxiDistance : DistanceSkill
    {
        public BenxiDistance() : base("#benxi")
        {
        }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            return -from.GetMark("benxi");
        }
    }

    public class Jianying : TriggerSkill
    {
        public Jianying() : base("jianying")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Play)
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
                        int suit = player.GetMark("JianyingSuit"), number = player.GetMark("JianyingNumber");
                        if (player.Alive && ((suit > 0 && (int)card.Suit + 1 == suit) || (number > 0 && card.Number == number)))
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
            if (player.Alive)
                room.DrawCards(player, 1, Name);

            return false;
        }
    }

    public class Xiansi : TriggerSkill
    {
        public Xiansi() : base("xiansi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventAcquireSkill, TriggerEvent.GameStart };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
            {
                if (!room.Skills.Contains("xiansivs"))
                    room.Skills.Add("xiansivs");
                foreach (Player p in room.GetOtherPlayers(player))
                    room.HandleAcquireDetachSkills(p, "xiansivs", true);
            }
            else if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && info.Info == Name)
            {
                if (!room.Skills.Contains("xiansivs"))
                    room.Skills.Add("xiansivs");
                foreach (Player p in room.GetOtherPlayers(player))
                    room.HandleAcquireDetachSkills(p, "xiansivs", true);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start)
            {
                bool can_invoke = false;
                List<Player> other_players = room.GetOtherPlayers(player);
                foreach (Player p in other_players)
                {
                    if (!p.IsNude())
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
                if (!p.IsNude())
                    to_choose.Add(p);
            }

            List<Player> choosees = room.AskForPlayersChosen(player, to_choose, Name, 0, 2, "@xiansi", true, info.SkillPosition);
            if (choosees.Count > 0)
            {
                room.SortByActionOrder(ref choosees);
                room.SetTag("tuxi_invoke" + player.Name, choosees);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player source, ref object data, Player ask_who, TriggerStruct info)
        {
            string str = "tuxi_invoke" + source.Name;
            List<Player> targets = room.ContainsTag(str) ? (List<Player>)room.GetTag(str) : new List<Player>();
            room.RemoveTag(str);
            int id = room.AskForCardChosen(source, targets[0], "he", Name, false, HandlingMethod.MethodNone);

            List<int> ids = new List<int> { id };
            CardsMoveStruct move = new CardsMoveStruct(id, source, Place.PlaceTable,
                new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_GAME, source.Name, targets[0].Name, Name, string.Empty));
            room.MoveCardsAtomic(move, true);

            if (source.Alive && targets.Count == 2)
            {
                int id2 = room.AskForCardChosen(source, targets[1], "he", Name, false, HandlingMethod.MethodNone);
                ids.Add(id2);

                move = new CardsMoveStruct(id2, source, Place.PlaceTable,
                    new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_GAME, source.Name, targets[1].Name, Name, string.Empty));
                room.MoveCardsAtomic(move, true);
            }

            List<int> got = new List<int>();
            foreach (int card in ids)
                if (room.GetCardPlace(card) == Place.PlaceTable)
                    got.Add(card);

            if (got.Count > 0 && source.Alive)
                room.AddToPile(source, "revolt", got);

            return false;
        }
    }


    public class XiansiVS : ViewAsSkill
    {
        public XiansiVS() : base("xiansivs")
        {
            attached_lord_skill = true;
            expand_pile = "%revolt";
            skill_type = SkillType.Attack;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            Player zhangjiao = RoomLogic.FindPlayerBySkillName(room, "xiansi");
            return selected.Count < 2 && zhangjiao.GetPile("revolt").Contains(to_select.Id);
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            Player zhangjiao = RoomLogic.FindPlayerBySkillName(room, "xiansi");
            if (zhangjiao == null || zhangjiao.GetPile("revolt").Count < 2)
                return false;

            return Slash.IsAvailable(room, player);
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            Player zhangjiao = RoomLogic.FindPlayerBySkillName(room, "xiansi");
            if (zhangjiao == null || zhangjiao.GetPile("revolt").Count < 2 )
                return false;

            return pattern == Slash.ClassName && room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE;
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 2)
            {
                WrappedCard slash = new WrappedCard(XiansiCard.ClassName);
                slash.AddSubCards(cards);
                return slash;
            }

            return null;
        }
    }

    public class XiansiTar : TargetModSkill
    {
        public XiansiTar() : base("#xiansi-tar", false) { }
        public override bool CheckSpecificTarget(Room room, Player from, Player to, WrappedCard card)
        {
            return card.GetSkillName() == "xiansi" && RoomLogic.PlayerHasSkill(room, to, "xiansi");
        }
    }

    public class XiansiCard : SkillCard
    {
        public static string ClassName = "XiansiCard";
        public XiansiCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_xiansi" };
            return Slash.Instance.TargetFilter(room, targets, to_select, Self, slash);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_xiansi" };
            return Slash.Instance.TargetsFeasible(room, targets, Self, slash);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player target = null;
            foreach (Player p in use.To)
            {
                if (RoomLogic.PlayerHasSkill(room, p, "xiansi"))
                {
                    target = p;
                    break;
                }
            }

            List<int> ids = new List<int>(use.Card.SubCards);
            room.MoveCardTo(use.Card, null, Place.DiscardPile, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, use.From.Name, target.Name, string.Empty));
            room.NotifySkillInvoked(target, "xiansi");

            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, target, "xiansi");
            room.BroadcastSkillInvoke("xiansi", "male", 2, gsk.General, gsk.SkinId);

            WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_xiansi" };
            return slash;
        }
    }

    public class Wurong : OneCardViewAsSkill
    {
        public Wurong() : base("wurong")
        {
            filter_pattern = ".!";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(WurongCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard wr = new WrappedCard(WurongCard.ClassName) { Skill = Name };
            wr.AddSubCard(card);
            return wr;
        }
    }

    public class WurongCard : SkillCard
    {
        public static string ClassName = "WurongCard";
        public WurongCard() : base(ClassName) { will_throw = false; }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && !to_select.IsKongcheng() && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            PindianStruct pd = room.PindianSelect(player, target, "wurong", room.GetCard(card_use.Card.GetEffectiveId()), PindianInfo.PindianType.Show);
            room.Pindian(ref pd, 0, PindianInfo.PindianType.Show);

            WrappedCard from = pd.From_card, to = pd.To_cards[0];
            if (from.Name.Contains(Slash.ClassName) && to.Name != Jink.ClassName)
            {
                room.ThrowCard(from.GetEffectiveId(), player);
                room.Damage(new DamageStruct("wurong", player, target));
            }
            else if (!from.Name.Contains(Slash.ClassName) && to.Name == Jink.ClassName)
            {
                room.ThrowCard(from.GetEffectiveId(), player);
                if (player.Alive && target.Alive && RoomLogic.CanGetCard(room, player, target, "he"))
                {
                    int id = room.AskForCardChosen(player, target, "he", "wurong", false, HandlingMethod.MethodGet);
                    room.ObtainCard(player, room.GetCard(id), new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, "wurong", string.Empty), false);
                }
            }
        }
    }

    public class Shizhi : TriggerSkill
    {
        public Shizhi() : base("shizhi")
        {
            events = new List<TriggerEvent> { TriggerEvent.FinishRetrial, TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
            view_as_skill = new ShizhiFilter();
            skill_type = SkillType.Alter;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.FinishRetrial && base.Triggerable(player, room) && !RoomLogic.PlayerHasShownSkill(room, player, Name) && data is JudgeStruct judge
                && judge.Who == player && judge.Card.Name == Jink.ClassName && player.Hp == 1)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card.Name == Slash.ClassName && use.Card.Skill == Name)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Card.Name == Slash.ClassName && resp.Card.Skill == Name)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.FinishRetrial && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                return info;
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced || triggerEvent == TriggerEvent.CardResponded)
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            return new TriggerStruct();
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

    public class ShizhiFilter : FilterSkill
    {
        public ShizhiFilter() : base("shizhi")
        {
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            int id = to_select.GetEffectiveId();
            return player.Hp == 1 && to_select.Name == Jink.ClassName && (room.GetCardPlace(id) == Place.PlaceHand || room.GetCardPlace(id) == Place.PlaceJudge);
        }

        public override void ViewAs(Room room, ref WrappedCard card, Player player)
        {
            WrappedCard new_card = new WrappedCard(Slash.ClassName, card.Id, card.Suit, card.Number, card.CanRecast, card.Transferable)
            {
                CanRecast = card.CanRecast,
                Skill = Name,
                ShowSkill = card.ShowSkill,
                UserString = card.UserString,
                Flags = card.Flags,
                Mute = card.Mute,
            };

            card.TakeOver(new_card);
            card.Modified = true;
        }
    }

    public class ZhanjueVS : ZeroCardViewAsSkill
    {
        public ZhanjueVS() : base("zhanjue")
        {
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player) => false;
        public override WrappedCard ViewAs(Room room, Player player)
        {
                WrappedCard card = new WrappedCard(Duel.ClassName) { Skill = Name };
                card.AddSubCards(player.GetCards("h"));
                return card;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed(Name))
            {
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse))
                        return false;
                }

                return true;
            }
            return false;
        }
    }
    public class Zhanjue : TriggerSkill
    {
        public Zhanjue() : base("Zhanjue")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.Damaged, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging };
            view_as_skill = new ZhanjueVS();
            skill_type = SkillType.Alter;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageDone && data is DamageStruct damage && damage.Card != null && damage.Card.Name == Duel.ClassName
                && damage.Card.Skill == Name && player.Alive)
            {
                string card_str = RoomLogic.CardToString(room, damage.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                player.SetFlags(str);
            }
            else if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && use.Card != null && use.Card.Name == Duel.ClassName && use.Card.Skill == Name)
            {
                if (use.From.Alive)
                    room.DrawCards(use.From, 1, Name);
                string card_str = RoomLogic.CardToString(room, use.Card);
                string str = string.Format("{0}_{1}", Name, card_str);
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HasFlag(str))
                    {
                        p.SetFlags("-" + str);
                        room.DrawCards(p, 1, Name);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_DRAW
                && move.Reason.SkillName == Name && room.Current == move.To && !move.To.HasFlag(Name))
            {
                move.To.AddMark(Name);
                if (move.To.GetMark(Name) >= 2)
                    move.To.SetFlags(Name);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class Qinwang : OneCardViewAsSkill
    {
        public Qinwang() : base("qingwang") { lord_skill = true; filter_pattern = "..!"; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!Slash.IsAvailable(room, player) || player.HasFlag(string.Format("jijiang_activate_{0}", room.GetRoomState().GlobalActivateID)))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "shu") return true;

            return false;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (pattern != Slash.ClassName || player.HasFlag(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID())))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "shu") return true;

            return false;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            return new WrappedCard(QinwangCard.ClassName) { Skill = Name, Mute = true };
        }
    }

    public class QinwangCard : SkillCard
    {
        public static string ClassName = "QinwangCard";
        public QinwangCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY || room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                FunctionCard fcard = Slash.Instance;
                return fcard.TargetFilter(room, targets, to_select, Self, slash);
            }

            return false;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY || room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                return targets.Count > 0;

            return true;
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player player = use.From;
            if (use.Reason == CardUseReason.CARD_USE_REASON_PLAY)
                player.SetFlags(string.Format("jijiang_activate_{0}", room.GetRoomState().GlobalActivateID));
            else
                player.SetFlags(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID()));

            LogMessage log = new LogMessage("$qinwang-slash")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, use.Card)
            };
            log.SetTos(use.To);
            room.SendLog(log);

            List<string> targets = new List<string>();
            foreach (Player p in use.To)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                targets.Add(p.Name);
            }

            room.BroadcastSkillInvoke("qinwang", player, use.Card.SkillPosition);
            room.NotifySkillInvoked(player, "qinwang");
            room.ThrowCard(use.Card.GetEffectiveId(), player);

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "shu")
                {
                    CardEffectStruct effect = new CardEffectStruct
                    {
                        Card = use.Card,
                        From = player,
                        To = p
                    };
                    WrappedCard card = room.AskForCard(p, "qinwang", Slash.ClassName, string.Format("@qinwang-target:{0}:{1}", player.Name, string.Join("+", targets)),
                        effect, HandlingMethod.MethodResponse);
                    if (card != null)
                    {
                        room.DrawCards(p, 1, "qinwang");
                        Thread.Sleep(500);
                        return card;
                    }
                }
            }

            return null;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            room.ThrowCard(card.GetEffectiveId(), player, null, "qinwang");
            room.BroadcastSkillInvoke("qinwang", player, card.SkillPosition);

            HandlingMethod method = room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE ? HandlingMethod.MethodUse : HandlingMethod.MethodResponse;
            player.SetFlags(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID()));
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "shu")
                {
                    CardEffectStruct effect = new CardEffectStruct
                    {
                        Card = card,
                        From = player,
                        To = p
                    };
                    WrappedCard slash = room.AskForCard(p, "qinwang", Slash.ClassName, "@qinwang:" + player.Name, effect, method);
                    if (slash != null)
                    {
                        room.DrawCards(p, 1, "qinwang");
                        return slash;
                    }
                }
            }

            return null;
        }
    }

    public class JianyingRecord : TriggerSkill
    {
        public JianyingRecord() : base("#jianying-record")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.EventPhaseChanging };
        }
        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (!base.Triggerable(player, room)) return;

            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == Player.PlayerPhase.Play)
            {
                player.SetMark("JianyingSuit", 0);
                player.SetMark("JianyingNumber", 0);
                room.RemovePlayerStringMark(player, "jianying_number");
                room.RemovePlayerStringMark(player, "jianying_suit");
            }
            else if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded) && player.Phase == Player.PlayerPhase.Play)
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
                        player.SetMark("JianyingSuit", (int)card.Suit > 3 ? 0 : (int)card.Suit + 1);
                        player.SetMark("JianyingNumber", card.Number);
                        room.SetPlayerStringMark(player, "jianying_suit", WrappedCard.GetSuitString(card.Suit));
                        room.SetPlayerStringMark(player, "jianying_number", card.Number.ToString());
                    }
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Shibei : TriggerSkill
    {
        public Shibei() : base("shibei")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.PreDamageDone, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    p.SetMark(Name, 0);
            }
            else if (triggerEvent == TriggerEvent.PreDamageDone && room.Current != null)
            {
                player.AddMark(Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && room.Current != null)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            if (player.GetMark(Name) > 0)
            {
                if (player.GetMark(Name) == 1)
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = player,
                        Recover = 1
                    };
                    room.Recover(player, recover, true);
                }
                else
                    room.LoseHp(player);
            }

            return false;
        }
    }

    //郭图逄纪
    public class Jigong : TriggerSkill
    {
        public Jigong() : base("jigong")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageDone, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageDone && data is DamageStruct damage && damage.From != null && damage.From.HasFlag(Name) && damage.From.Phase == Player.PlayerPhase.Play)
            {
                damage.From.AddMark("damage_point_play_phase", damage.Damage);
                room.SetPlayerStringMark(damage.From, Name, damage.From.GetMark("damage_point_play_phase").ToString());
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && player.HasFlag(Name)
                && change.To == PlayerPhase.NotActive)
            {
                player.SetMark("damage_point_play_phase", 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play
                ? new TriggerStruct(Name, player)
                : new TriggerStruct();
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
            room.SetPlayerStringMark(player, Name, "0");
            player.SetFlags(Name);
            room.DrawCards(player, 2, Name);

            return false;
        }
    }

    public class JigongMax : MaxCardsSkill
    {
        public JigongMax() : base("#jigong")
        {
        }
        public override int GetFixed(Room room, Player target)
        {
            if (target.HasFlag("jigong"))
                return target.GetMark("damage_point_play_phase");

            return -1;
        }
    }

    public class ShifeiCard : SkillCard
    {
        public static string ClassName = "ShifeiCard";
        public ShifeiCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            string response_id = room.GetRoomState().GetCurrentResponseID().ToString();
            player.SetFlags("shifei_" + response_id);

            room.NotifySkillInvoked(player, "shifei");

            Player current = room.Current;
            if (current == null || !current.Alive || current.Phase == Player.PlayerPhase.NotActive)
                return null;

            room.DrawCards(current, new DrawCardStruct(1, player, "shifei"));
            List<Player> mosts = new List<Player>();
            int most = -1;
            foreach (Player p in room.GetAlivePlayers())
            {
                int h = p.HandcardNum;
                if (h > most)
                {
                    mosts.Clear();
                    most = h;
                    mosts.Add(p);
                }
                else if (most == h)
                    mosts.Add(p);
            }

            if (most < 0 || mosts.Contains(current))
                return null;

            List<Player> mosts_copy = new List<Player>(mosts);
            foreach (Player p in mosts_copy)
                if (!RoomLogic.CanDiscard(room, player, p, "he"))
                    mosts.Remove(p);

            if (mosts.Count == 0)
                return null;

            Player vic = room.AskForPlayerChosen(player, mosts, "shifei", "@shifei-dis", false, false, card.SkillPosition);
            // it is impossible that vic == NULL
            if (vic == player)
                room.AskForDiscard(player, "shifei", 1, 1, false, true, "@shifei-disself", false, card.SkillPosition);
            else
            {
                int id = room.AskForCardChosen(player, vic, "he", "shifei", false, HandlingMethod.MethodDiscard);
                room.ThrowCard(id, vic, player);
            }

            room.BroadcastSkillInvoke("shifei", player, card.SkillPosition);
            WrappedCard jink = new WrappedCard(Jink.ClassName) { Skill = "_shifei" };
            return jink;
        }
    }


    public class Shifei : ZeroCardViewAsSkill
    {
        public Shifei() : base("shifei")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            string response_id = room.GetRoomState().GetCurrentResponseID().ToString();
            Player current = room.Current;
            if (pattern == "Jink" && !player.HasFlag("shifei_" + response_id) && current != null)
            {
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                if (Jink.Instance.IsAvailable(room, player, jink) && current.Alive && current.Phase != PlayerPhase.NotActive)
                    return true;
            }

            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard c = new WrappedCard(ShifeiCard.ClassName) { Skill = Name };
            return c;
        }
    }

    public class Qieting : TriggerSkill
    {
        public Qieting() : base("qieting")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseChanging };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && room.Current == player && data is CardUseStruct use && use.To.Count > 0 && !player.HasFlag(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                {
                    foreach (Player p in use.To)
                    {
                        if (p != player)
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
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && !player.HasFlag(Name))
            {
                List<Player> caifuren = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in caifuren)
                    if (p != player)
                        result.Add(new TriggerStruct(Name, p));
            }
            return result;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> disable_equiplist = new List<int>(), equiplist = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                if (player.GetEquip(i) >= 0)
                    if (ask_who.GetEquip(i) < 0 && RoomLogic.CanPutEquip(ask_who, room.GetCard(player.GetEquip(i))))

                        equiplist.Add(player.GetEquip(i));
                    else
                        disable_equiplist.Add(player.GetEquip(i));
            }

            List<string> choices = new List<string> { "draw" };
            if (equiplist.Count > 0)
                choices.Add("getequipc");

            if (room.AskForChoice(ask_who, Name, string.Join("+", choices)) == "draw")
                room.DrawCards(ask_who, 1, Name);
            else
            {
                int id = room.AskForCardChosen(ask_who, player, "e", Name, false, HandlingMethod.MethodNone, disable_equiplist);
                room.MoveCardTo(room.GetCard(id), ask_who, Place.PlaceEquip);
            }

            return false;
        }
    }
    
    public class XianzhouCard : SkillCard
    {
        public static string ClassName = "XianzhouCard";
        public XianzhouCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@handover", 0);
            room.BroadcastSkillInvoke("xianzhou", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "xianzhou");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            List<int> ids = new List<int>(card_use.Card.SubCards);
            Player target = card_use.To[0];
            Player player = card_use.From;
            room.ObtainCard(target, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "xianzhou", string.Empty));

            List<string> choices = new List<string>();
            List<string> prompts = new List<string> { string.Empty };
            if (player.GetLostHp() > 0)
            {
                choices.Add("recover");
                prompts.Add(string.Format("@xianzhou-recover:{0}::{1}", player.Name, Math.Min(player.GetLostHp(), ids.Count)));
            }
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(target))
                if (RoomLogic.InMyAttackRange(room, target, p))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                choices.Add("damage");
                prompts.Add("@xianzhou-damage:::" + ids.Count.ToString());
            }
            player.SetFlags("xianzhou_source");
            target.SetMark("xianzhou", ids.Count);
            string choice = room.AskForChoice(target, "xianzou", string.Join("+", choices), prompts);
            player.SetFlags("-xianzhou_source");
            target.SetMark("xianzhou", 0);
            if (choice == "damage")
            {

                List<Player> victims = room.AskForPlayersChosen(target, targets, "xianzhou", 0, Math.Min(targets.Count, ids.Count), "@xianzhou-target");
                if (victims.Count > 0)
                {
                    room.SortByActionOrder(ref victims);
                    foreach (Player p in victims)
                    {
                        room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, target.Name, p.Name);
                        room.Damage(new DamageStruct("xianzhou", target, p));
                    }
                }
            }
            else
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = Math.Min(player.GetLostHp(), ids.Count)
                };
                room.Recover(player, recover, true);
            }
        }
    }

    public class Xianzhou : ZeroCardViewAsSkill
    {
        public Xianzhou() : base("xianzhou")
        {
            frequency = Frequency.Limited;
            limit_mark = "@handover";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetEquips().Count > 0 && player.GetMark(limit_mark) > 0;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard xz = new WrappedCard(XianzhouCard.ClassName)
            {
                Skill = Name,
                Mute = true
            };
            xz.SubCards.AddRange(player.GetEquips());
            return xz;
        }
    }

    public class MingceCard : SkillCard
    {
        public static string ClassName = "MingceCard";
        public MingceCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player target = effect.To, player = effect.From;
            List<Player> targets = new List<Player>();
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                Skill = "_mingce"
            };
            if (Slash.IsAvailable(room, target, slash))
            {
                foreach (Player p in room.GetOtherPlayers(target))
                {
                    if (RoomLogic.CanSlash(room, target, p))
                        targets.Add(p);
                }
            }

            Player victim = null;
            List<string> choicelist = new List<string> { "draw" }, prompts = new List<string> { string.Empty, string.Empty };
            if (targets.Count > 0 && player.Alive)
            {
                victim = room.AskForPlayerChosen(player, targets, "mingce", "@dummy-slash2:" + target.Name);
                victim.SetFlags("MingceTarget"); // For AI

                LogMessage log = new LogMessage
                {
                    Type = "#CollateralSlash",
                    From = player.Name,
                    To = new List<string> { victim.Name }
                };
                room.SendLog(log);

                choicelist.Add("use");
                prompts.Add("@mince-target:" + victim.Name);
            }

            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "mingce", string.Empty);
            room.ObtainCard(target, effect.Card, reason);
            string choice = room.AskForChoice(target, "mingce", string.Join("+", choicelist), prompts);
            if (victim != null && victim.HasFlag("MingceTarget")) victim.SetFlags("-MingceTarget");

            if (choice == "use")
            {
                if (RoomLogic.CanSlash(room, target, victim, slash))
                {
                    room.UseCard(new CardUseStruct(slash, target, victim));
                }
            }
            else
            {
                room.DrawCards(target, new DrawCardStruct(1, player, "mingce"));
            }
        }
    }

    public class Mingce : OneCardViewAsSkill
    {
        public Mingce() : base("mingce")
        {
            filter_pattern = "EquipCard,Slash";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(MingceCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard mingceCard = new WrappedCard(MingceCard.ClassName)
            {
                Skill = Name
            };
            mingceCard.AddSubCard(card);
            return mingceCard;
        }
    }

    public class Zhichi : TriggerSkill
    {
        public Zhichi() : base("zhichi")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.CardEffected, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark("@late") > 0) room.SetPlayerMark(p, "@late", 0);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && player.Phase == PlayerPhase.NotActive && player.GetMark("@late") == 0
                && room.Current != null && room.Current.Alive && room.Current.Phase != PlayerPhase.NotActive)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardEffected && player.GetMark("@late") > 0 && data is CardEffectStruct effect)
            {
                FunctionCard fcard = Engine.GetFunctionCard(effect.Card.Name);
                if (fcard is Slash || (fcard is TrickCard && !(fcard is DelayedTrick)))
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.Damaged)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.AddPlayerMark(player, "@late");

                LogMessage log = new LogMessage
                {
                    Type = "#ZhichiDamaged",
                    From = player.Name
                };
                room.SendLog(log);

                return false;
            }
            else if (data is CardEffectStruct effect)
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                LogMessage log = new LogMessage
                {
                    Type = "#ZhichiAvoid",
                    From = ask_who.Name,
                    Arg = effect.Card.Name
                };
                room.SendLog(log);

                return true;
            }

            return false;
        }
    }

    public class Zishou : DrawCardsSkill
    {
        public Zishou() : base("zishou")
        {
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                player.SetFlags(Name);
                return info;
            }

            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            return n + GetAliveKingdoms(room);
        }

        public static int GetAliveKingdoms(Room room)
        {
            List<string> kingdoms = new List<string>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (!kingdoms.Contains(p.Kingdom))
                    kingdoms.Add(p.Kingdom);
            }

            return kingdoms.Count;
        }
    }

    public class ZishouProhibit : ProhibitSkill
    {
        public ZishouProhibit() : base("#zishou-prohibit")
        {
        }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && from.HasFlag("zishou") && !(Engine.GetFunctionCard(card.Name) is SkillCard))
                return from != to;

            return false;
        }
    }

    public class Zongshi : TriggerSkill
    {
        public Zongshi() : base("zongshi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change
                && change.To == PlayerPhase.Discard && player.HandcardNum > player.Hp && RoomLogic.GetMaxCards(room, player) > player.Hp)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.NotifySkillInvoked(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            return new TriggerStruct();
        }
    }

    public class ZongshiMax : MaxCardsSkill
    {
        public ZongshiMax() : base("#zongshi-max")
        {
        }

        public override int GetExtra(Room room, Player target)
        {
            if (RoomLogic.PlayerHasShownSkill(room, target, "zongshi"))
                return Zishou.GetAliveKingdoms(room);

            return 0;
        }
    }

    public class Juece : PhaseChangeSkill
    {
        public Juece() : base("juece")
        {
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.IsKongcheng()) return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.IsKongcheng()) targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@juece", true, true, info.SkillPosition);
                if (target != null)
                {
                    player.SetTag(Name, target.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag(Name), true);
            if (target.Alive)
            {
                room.Damage(new DamageStruct(Name, player, target));
            }

            return false;
        }
    }

    public class Mieji : OneCardViewAsSkill
    {
        public Mieji() : base("mieji")
        {
            filter_pattern = "TrickCard|black";
            skill_type = SkillType.Wizzard;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(MiejiCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard mieji = new WrappedCard(MiejiCard.ClassName) { Skill = Name };
            mieji.AddSubCard(card);
            return mieji;
        }
    }

    public class MiejiCard : SkillCard
    {
        public static string ClassName = "MiejiCard";
        public MiejiCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && !to_select.IsKongcheng();
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PUT, player.Name, "mieji", string.Empty);
            CardsMoveStruct move = new CardsMoveStruct(new List<int>(card_use.Card.SubCards), null, Place.DrawPile, reason)
            {
                To_pile_name = string.Empty,
                From = player.Name
            };
            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
            room.MoveCardsAtomic(moves, true);

            Player target = card_use.To[0];
            if (target.Alive && !target.IsNude())
            {
                List<int> ids = room.AskForExchange(target, "mieji", 1, 1, "@mieji-discard", string.Empty, "..!", string.Empty);
                if (ids.Count > 0)
                {
                    bool trick = Engine.GetFunctionCard(room.GetCard(ids[0]).Name) is TrickCard;
                    room.ThrowCard(ref ids, target);

                    if (!trick && !target.IsNude())
                    {
                        ids = room.AskForExchange(target, "mieji", 1, 1, "@mieji-notrick", string.Empty, "^TrickCard!", string.Empty);
                        if (ids.Count > 0)
                            room.ThrowCard(ref ids, target);
                    }
                }
            }
        }
    }

    public class Fencheng : ViewAsSkill
    {
        public Fencheng() : base("fencheng")
        {
            skill_type = SkillType.Attack;
            frequency = Frequency.Limited;
            limit_mark = "@burn";
            response_pattern = "@@fencheng";
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (!room.ContainsTag(Name))
                return false;
            else
            {
                int count = (int)room.GetTag("fencheng");
                return RoomLogic.CanDiscard(room, player, player, to_select.Id);
            }
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseStruct.CardUseReason reason, string pattern, string position = null)
        {
            if (reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
                return invoker.GetMark(limit_mark) > 0;
            else if (reason == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                return pattern == response_pattern;

            return false;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (!room.ContainsTag(Name) && cards.Count == 0)
                return new WrappedCard(FenchengCard.ClassName) { Skill = Name, Mute = true };
            else if (room.GetTag(Name) is int count && cards.Count >= count)
            {
                WrappedCard card = new WrappedCard(FenchengCard.ClassName);
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }

    public class FenchengCard : SkillCard
    {
        public static string ClassName = "FenchengCard";
        public FenchengCard() : base(ClassName)
        {
            will_throw = false;
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            if (!room.ContainsTag("fencheng"))
            {
                room.SetPlayerMark(card_use.From, "@burn", 0);
                room.BroadcastSkillInvoke("fencheng", card_use.From, card_use.Card.SkillPosition);
                room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "fencheng");
                card_use.To = room.GetOtherPlayers(card_use.From);
                room.SortByActionOrder(ref card_use);
                room.SetTag("fencheng", 1);
                base.OnUse(room, card_use);
                room.RemoveTag("fencheng");
            }
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From;
            Player target = effect.To;
            if (target.Alive)
            {
                int count = (int)room.GetTag("fencheng");
                WrappedCard card = room.AskForUseCard(target, "@@fencheng", string.Format("@fencheng:{0}::{1}", effect.From.Name, count), -1, HandlingMethod.MethodDiscard);
                if (card != null)
                {
                    List<int> ids = new List<int>(card.SubCards);
                    room.ThrowCard(ref ids, target);

                    room.SetTag("fencheng", ids.Count + 1);
                }
                else
                {
                    room.Damage(new DamageStruct("fencheng", player, target, 2, DamageStruct.DamageNature.Fire));
                    room.SetTag("fencheng", 1);
                }
            }

            Thread.Sleep(500);
        }
    }
    //于禁
    public class Zhenjun : TriggerSkill
    {
        public Zhenjun() : base("zhenjun")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Start)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum > p.Hp && RoomLogic.CanDiscard(room, player, p, "he"))
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum > p.Hp && RoomLogic.CanDiscard(room, player, p, "he"))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                Player to = room.AskForPlayerChosen(player, targets, Name, "@zhenjun-invoke", true, true, info.SkillPosition);
                if (to != null)
                {
                    player.SetTag("zhenjun_target", to.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player to = room.FindPlayer(player.GetTag("zhenjun_target").ToString());
            player.RemoveTag("zhenjun_target");
            if (to != null)
            {
                int count = to.HandcardNum - to.Hp;
                List<int> ids = new List<int>();
                if (to == player)
                {
                    ids = room.AskForExchange(player, Name, count, count, "@zhenjun", string.Empty, "..!", info.SkillPosition);
                }
                else
                {
                    List<string> handle = new List<string>();
                    for (int i = 0; i < count; i++)
                        handle.Add("he^false^discard");

                    ids = room.AskForCardsChosen(player, to, handle, Name);
                }
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISMANTLE, player.Name, to.Name, Name, string.Empty)
                {
                    General = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition)
                };
                room.ThrowCard(ref ids, reason, to, to == player ? null : player);

                if (ids.Count > 0 && player.Alive)
                {
                    int discard_count = 0;
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (!(Engine.GetFunctionCard(card.Name) is EquipCard))
                            discard_count++;
                    }

                    List<string> choices = new List<string>(), prompts = new List<string> { "@zhenjun-choose:" };
                    if (to.Alive)
                    {
                        choices.Add("draw");
                        prompts.Add(string.Format("@zhenjun-draw:{0}::{1}", to.Name, ids.Count));
                    }

                    bool can_discard = false;
                    if (discard_count > 0)
                    {
                        int discard_self = 0;
                        foreach (int id in player.GetCards("he"))
                            if (!RoomLogic.IsJilei(room, player, room.GetCard(id))) discard_self++;

                        if (discard_self >= discard_count) can_discard = true;
                    }

                    if (discard_count > 0)
                    {
                        if (can_discard)
                        {
                            choices.Add("discard");
                            prompts.Add(string.Format("@zhenjun-discard:::{0}", discard_count));
                        }
                    }
                    else
                        choices.Add("cancel");

                    room.SetTag("zhenjun_target", to);
                    string result = room.AskForChoice(player, Name, string.Join("+", choices), prompts);
                    room.RemoveTag("zhenjun_target");
                    if (result == "draw" && to.Alive)
                        room.DrawCards(to, new DrawCardStruct(ids.Count, player, Name));
                    else if (result == "discard" && player.Alive)
                        room.AskForDiscard(player, Name, discard_count, discard_count, false, true, string.Format("@zhenjun-discard:::{0}", discard_count), false, info.SkillPosition);
                }
            }

            return false;
        }
    }

    //韩屎
    public class ShenduanCard : SkillCard
    {
        public static string ClassName = "ShenduanCard";
        public ShenduanCard() : base(ClassName)
        {
            handling_method = HandlingMethod.MethodUse;
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard ss = new WrappedCard(SupplyShortage.ClassName)
            {
                DistanceLimited = false,
                Skill = "_shenduan"
            };
            ss.AddSubCard(card);
            ss = RoomLogic.ParseUseCard(room, ss);

            return SupplyShortage.Instance.TargetFilter(room, targets, to_select, Self, ss);
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player player = use.From;
            room.NotifySkillInvoked(player, "shenduan");
            room.BroadcastSkillInvoke("shenduan", player, use.Card.SkillPosition);

            WrappedCard ss = new WrappedCard(SupplyShortage.ClassName)
            {
                DistanceLimited = false,
                Skill = "_shenduan"
            };
            ss.AddSubCard(use.Card);
            ss = RoomLogic.ParseUseCard(room, ss);

            return ss;
        }
    }

    public class ShenduanVS : OneCardViewAsSkill
    {
        public ShenduanVS() : base("shenduan")
        {
            expand_pile = "#shenduan";
            response_pattern = "@@shenduan";
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return player.GetPile("#shenduan").Contains(to_select.Id);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ss = new WrappedCard(ShenduanCard.ClassName) { Skill = Name };
            ss.AddSubCard(card);
            return ss;
        }
    }

    public class Shenduan : TriggerSkill
    {
        public Shenduan() : base("shenduan")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            view_as_skill = new ShenduanVS();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
               && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD && move.To_place == Player.Place.PlaceTable)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int card_id = move.Card_ids[i];
                    if (room.GetCardPlace(card_id) == Player.Place.PlaceTable && (move.From_places[i] == Player.Place.PlaceHand || move.From_places[i] == Player.Place.PlaceEquip))
                        room.SetCardFlag(card_id, Name);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
                   && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD && move.To_place == Player.Place.DiscardPile)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int card_id = move.Card_ids[i];
                    WrappedCard card = room.GetCard(card_id);
                    if (card.HasFlag(Name) && WrappedCard.IsBlack(card.Suit) && Engine.GetFunctionCard(card.Name) is BasicCard)
                    {
                        return new TriggerStruct(Name, move.From);
                    }
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            List<int> ids = new List<int>(), _ids;

            foreach (int id in move.Card_ids)
            {
                int index = move.Card_ids.IndexOf(id);
                WrappedCard card = room.GetCard(id);
                if (card.HasFlag(Name) && WrappedCard.IsBlack(card.Suit) && Engine.GetFunctionCard(card.Name) is BasicCard && room.GetCardPlace(id) == Player.Place.DiscardPile)
                    ids.Add(id);
            }

            while (ids.Count > 0)
            {
                ask_who.Piles["#shenduan"] = ids;
                WrappedCard card = room.AskForUseCard(ask_who, "@@shenduan", "@shenduan-use", -1, FunctionCard.HandlingMethod.MethodUse, false, info.SkillPosition);
                ask_who.Piles["#shenduan"] = new List<int>();

                if (card != null)
                    ids.RemoveAll(t => card.SubCards.Contains(t));
                else
                    break;

                _ids = new List<int>(ids);
                foreach (int id in _ids)
                    if (room.GetCardPlace(id) != Place.DiscardPile)
                        ids.Remove(id);
            }

            return new TriggerStruct();
        }
    }

    public class ShenduanClear : TriggerSkill
    {
        public ShenduanClear() : base("#shenduan-clear")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
        }
        public override int GetPriority() => -1;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From != null)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    WrappedCard card = room.GetCard(move.Card_ids[i]);
                    if (move.From_places[i] == Player.Place.PlaceTable && card.HasFlag("shenduan"))
                        card.SetFlags("-shenduan");
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Yonglue : TriggerSkill
    {
        public Yonglue() : base("yonglue")
        {
            events = new List<TriggerEvent> { TriggerEvent.PreDamageDone, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.PreDamageDone && data is DamageStruct damage && damage.Card != null && damage.Card.GetSkillName() == Name
                && damage.From != null && damage.From.Alive)
            {
                damage.From.SetFlags(Name);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.Judge && player.JudgingArea.Count > 0)
            {
                List<Player> hss = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player hs in hss)
                {
                    if (hs != player && RoomLogic.InMyAttackRange(room, hs, player))
                        triggers.Add(new TriggerStruct(Name, hs));
                }
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive && player.JudgingArea.Count > 0 && RoomLogic.InMyAttackRange(room, ask_who, player)
                && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                int id = room.AskForCardChosen(ask_who, player, "j", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISMANTLE, ask_who.Name, player.Name, Name, string.Empty)
                {
                    General = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition)
                };
                List<int> ids = new List<int> { id };
                room.ThrowCard(ref ids, reason, player, ask_who);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_yonglue", SkillPosition = info.SkillPosition };
            slash.UserString = ask_who.Name;

            if (Engine.IsProhibited(room, ask_who, player, slash) == null)
                room.UseCard(new CardUseStruct(slash, ask_who, player));

            if (!ask_who.HasFlag(Name) && ask_who.Alive)
                room.DrawCards(ask_who, 1, Name);

            ask_who.SetFlags("-yonglue");

            return false;
        }
    }

    public class Jueqing : TriggerSkill
    {
        public Jueqing() : base("jueqing")
        {
            frequency = Frequency.Compulsory;
            events.Add(TriggerEvent.Predamage);
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SendCompulsoryTriggerLog(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.LoseHp(damage.To, damage.Damage);
                return true;
            }

            return false;
        }
    }

    public class Shangshi : TriggerSkill
    {
        public Shangshi() : base("shangshi")
        {
            events = new List<TriggerEvent> { TriggerEvent.HpChanged, TriggerEvent.MaxHpChanged, TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Replenish;
        }

        private int GetDrawCards(Player player)
        {
            return Math.Max(player.GetLostHp() - player.HandcardNum, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if ((triggerEvent == TriggerEvent.HpChanged || triggerEvent == TriggerEvent.MaxHpChanged) && base.Triggerable(player, room) && GetDrawCards(player) > 0)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && move.From_places.Contains(Place.PlaceHand) && base.Triggerable(move.From, room) && GetDrawCards(move.From) > 0)
                    return new TriggerStruct(Name, move.From);
                else if (move.To != null && move.To_place == Place.PlaceHand && base.Triggerable(move.To, room) && GetDrawCards(move.To) > 0)
                    return new TriggerStruct(Name, move.To);
            }
            return new TriggerStruct();
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
            if (GetDrawCards(ask_who) > 0)
                room.DrawCards(ask_who, GetDrawCards(ask_who), Name);
            return false;
        }
    }

    public class Luoying : TriggerSkill
    {
        public Luoying() : base("luoying")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile &&
                (move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_JUDGEDONE ||
                (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD))
            {
                List<int> ids = new List<int>();
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCard(id).Suit == WrappedCard.CardSuit.Club && room.GetCardPlace(id) == Place.DiscardPile) ids.Add(id);
                }
                if (ids.Count > 0)
                {
                    List<Player> caozhi = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player p in caozhi)
                        if (p != move.From) result.Add(new TriggerStruct(Name, p));
                }
            }

            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile &&
                (move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_JUDGEDONE ||
                (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD))
            {
                List<int> ids = new List<int>();
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCard(id).Suit == WrappedCard.CardSuit.Club && room.GetCardPlace(id) == Place.DiscardPile) ids.Add(id);
                }
                if (ids.Count > 0)
                {
                    AskForMoveCardsStruct ly = room.AskForMoveCards(ask_who, new List<int>(), ids, false, Name, ids.Count, ids.Count, true, true, new List<int>(), info.SkillPosition);
                    if (ly.Success && ly.Bottom.Count > 0)
                    {
                        ask_who.SetTag(Name, ly.Bottom);
                        room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                        room.NotifySkillInvoked(ask_who, Name);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = (List<int>)ask_who.GetTag(Name);
            ask_who.RemoveTag(Name);
            room.ObtainCard(ask_who, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, ask_who.Name, Name, string.Empty));
            return false;
        }
    }

    public class Jiushi : TriggerSkill
    {
        public Jiushi() : base("jiushi")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageComplete, TriggerEvent.PreDamageDone };
            skill_type = SkillType.Masochism;
            view_as_skill = new JiushiVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.PreDamageDone && base.Triggerable(player, room) && !player.FaceUp)
                player.SetTag(Name, true);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageDone && player.Alive && player.ContainsTag(Name) && (bool)player.GetTag(Name))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.RemoveTag(Name);
            if (!player.FaceUp)
            {
                if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.TurnOver(player);
            return false;
        }
    }

    public class JiushiVS : ZeroCardViewAsSkill
    {
        public JiushiVS() : base("jiushi") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.FaceUp && Analeptic.Instance.IsAvailable(room, player, new WrappedCard(Analeptic.ClassName));
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            WrappedCard ana = new WrappedCard(Analeptic.ClassName);
            return player.FaceUp && Engine.MatchExpPattern(room, pattern, player, ana)
                && room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
                && Analeptic.Instance.IsAvailable(room, player, new WrappedCard(Analeptic.ClassName));
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JiushiCard.ClassName);
        }
    }

    public class JiushiCard : SkillCard
    {
        public static string ClassName = "JiushiCard";
        public JiushiCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            if (use.From.FaceUp)
                room.TurnOver(use.From);

            room.NotifySkillInvoked(use.From, "jiushi");
            room.BroadcastSkillInvoke("jiushi", use.From, use.Card.SkillPosition);
            WrappedCard ana = new WrappedCard(Analeptic.ClassName)
            {
                Skill = "_jiushi",
                ShowSkill = "jiushi"
            };
            return ana;
        }
    }

    public class Huomo : TriggerSkill
    {
        public Huomo() : base("huomo")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseChanging, TriggerEvent.CardResponded };
            skill_type = SkillType.Alter;
            view_as_skill = new HuomoVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && Engine.GetFunctionCard(use.Card.Name) is BasicCard)
            {
                string card = use.Card.Name;
                if (card.Contains(Slash.ClassName)) card = Slash.ClassName;
                player.SetFlags(string.Format("huomo_{0}", card));
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Card.Name == Jink.ClassName && resp.Use)
            {
                player.SetFlags(string.Format("huomo_{0}", Jink.ClassName));
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    p.SetFlags(string.Format("-huomo_{0}", Slash.ClassName));
                    p.SetFlags(string.Format("-huomo_{0}", Jink.ClassName));
                    p.SetFlags(string.Format("-huomo_{0}", Analeptic.ClassName));
                    p.SetFlags(string.Format("-huomo_{0}", Peach.ClassName));
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class HuomoVS : ViewAsSkill
    {
        public HuomoVS() : base("huomo")
        {
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count == 0 && WrappedCard.IsBlack(to_select.Suit) && !(Engine.GetFunctionCard(to_select.Name) is BasicCard);
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return GetGuhuo(room, player).Count > 0 && !player.IsNude();
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (player.IsNude() || room.GetRoomState().GetCurrentCardUseReason() != CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE) return false;
            bool invoke = false;
            foreach (WrappedCard card in GetGuhuo(room, player))
            {
                if (Engine.MatchExpPattern(room, pattern, player, card))
                {
                    invoke = true;
                    break;
                }
            }

            return invoke;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (cards.Count == 1)
            {
                foreach (WrappedCard card in GetGuhuo(room, player))
                {
                    card.AddSubCard(cards[0]);
                    result.Add(card);
                }
            }

            return result;
        }

        private List<WrappedCard> GetGuhuo(Room room, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            List<string> guhuo = GetGuhuoCards(room, "b");

            foreach (string card_name in guhuo)
            {
                string card = card_name;
                if (card_name.Contains(Slash.ClassName)) card = Slash.ClassName;
                if (player.HasFlag(string.Format("huomo_{0}", card))) continue;
                WrappedCard wrapped = new WrappedCard(card_name);
                result.Add(wrapped);
            }

            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && RoomLogic.IsVirtualCard(room, cards[0]))
            {
                WrappedCard hm = new WrappedCard(HuomoCard.ClassName)
                {
                    UserString = cards[0].Name
                };
                hm.AddSubCard(cards[0]);
                return hm;
            }

            return null;
        }
    }

    public class HuomoCard : SkillCard
    {
        public static string ClassName = "HuomoCard";
        public HuomoCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard wrapped = new WrappedCard(card.UserString);
            FunctionCard fcard = Engine.GetFunctionCard(card.UserString);
            return fcard.TargetFilter(room, targets, to_select, Self, wrapped);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            WrappedCard wrapped = new WrappedCard(card.UserString);
            FunctionCard fcard = Engine.GetFunctionCard(card.UserString);
            return fcard.TargetsFeasible(room, targets, Self, wrapped);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player player = use.From;
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PUT, player.Name, "huomo", string.Empty);
            CardsMoveStruct move = new CardsMoveStruct(new List<int>(use.Card.SubCards), null, Place.DrawPile, reason)
            {
                To_pile_name = string.Empty,
                From = player.Name
            };
            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
            room.MoveCardsAtomic(moves, true);
            
            room.BroadcastSkillInvoke("huomo", player, use.Card.SkillPosition);
            room.NotifySkillInvoked(player, "huomo");
            WrappedCard wrapped = new WrappedCard(use.Card.UserString);
            return wrapped;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PUT, player.Name, "huomo", string.Empty);
            CardsMoveStruct move = new CardsMoveStruct(new List<int>(card.SubCards), null, Place.DrawPile, reason)
            {
                To_pile_name = string.Empty,
                From = player.Name
            };
            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
            room.MoveCardsAtomic(moves, true);
            
            room.BroadcastSkillInvoke("huomo", player, card.SkillPosition);
            room.NotifySkillInvoked(player, "huomo");
            WrappedCard wrapped = new WrappedCard(card.UserString);
            return wrapped;
        }
    }

    public class Zuoding : TriggerSkill
    {
        public Zuoding() : base("zuoding")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.Damage, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && room.ContainsTag(Name))
                room.RemoveTag(Name);
            else if (triggerEvent == TriggerEvent.Damage && room.Current != null && room.Current.Phase == PlayerPhase.Play && !room.ContainsTag(Name))
                room.SetTag(Name, true);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.TargetChosen && !room.ContainsTag(Name) && data is CardUseStruct use && use.From != null && use.From.Phase == PlayerPhase.Play
                && RoomLogic.GetCardSuit(room, use.Card) == WrappedCard.CardSuit.Spade)
            {
                List<Player> zhongyao = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in zhongyao)
                    if (p != use.From) result.Add(new TriggerStruct(Name, p));
            }

            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in use.To)
                    if (p.Alive) targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(ask_who, targets, Name, "@zuoding", true, true, info.SkillPosition);
                    if (target != null)
                    {
                        ask_who.SetTag(Name, target.Name);
                        room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)ask_who.GetTag(Name));
            ask_who.RemoveTag(Name);
            room.DrawCards(target, new DrawCardStruct(1, ask_who, Name));
            return false;
        }
    }

    public class Huituo : TriggerSkill
    {
        public Huituo() : base("huituo")
        {
            events.Add(TriggerEvent.Damaged);
            skill_type = SkillType.Masochism;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@huituo", true, true, info.SkillPosition);
            if (target != null)
            {
                player.SetTag(Name, target.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)ask_who.GetTag(Name));
            ask_who.RemoveTag(Name);
            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                PlayAnimation = false,
                Who = target,
                Reason = Name
            };
            room.Judge(ref judge);

            if (target.Alive)
            {
                if (WrappedCard.IsBlack(judge.Card.Suit))
                {
                    room.DrawCards(target, new DrawCardStruct(((DamageStruct)data).Damage, player, Name));
                }
                else
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = player,
                        Recover = 1
                    };
                    room.Recover(target, recover, true);
                }
            }

            return false;
        }
    }

    public class Mingjian : ZeroCardViewAsSkill
    {
        public Mingjian() : base("mingjian")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng() && !player.HasUsed(MingjianCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(MingjianCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            card.AddSubCards(player.GetCards("h"));
            return card;
        }
    }

    public class MingjianTar : TargetModSkill
    {
        public MingjianTar() : base("#mingjian-tar", true) { }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            return from != null && Engine.MatchExpPattern(room, pattern, from, card) && from.HasFlag("mingjian") ? 1 : 0;
        }
    }

    public class MingjianMax : MaxCardsSkill
    {
        public MingjianMax() : base("#mingjian-max") { }

        public override int GetExtra(Room room, Player target)
        {
            return target.HasFlag("mingjian") ? 1 : 0;
        }
    }

    public class MingjianCard : SkillCard
    {
        public static string ClassName = "MingjianCard";
        public MingjianCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];

            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "mingjian", string.Empty), false);
            target.SetFlags("mingjian");
        }
    }

    public class Xingshuai : TriggerSkill
    {
        public Xingshuai() : base("xingshuai")
        {
            events = new List<TriggerEvent> { TriggerEvent.AskForPeaches, TriggerEvent.AskForPeachesDone };
            frequency = Frequency.Limited;
            limit_mark = "@xingshuai";
            skill_type = SkillType.Recover;
            lord_skill = true;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.AskForPeachesDone && room.ContainsTag(Name) && room.GetTag(Name) == data)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HasFlag(Name))
                    {
                        p.SetFlags("-xingshuai");
                        room.Damage(new DamageStruct(Name, null, p));
                    }
                }

                room.RemoveTag(Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.AskForPeaches && data is DyingStruct dying_data && dying_data.Who == target && target.Hp <= 0
                && base.Triggerable(target, room) && target.GetMark("@xingshuai") > 0)
            {
                foreach (Player p in room.GetOtherPlayers(target))
                    if (p.Kingdom == "wei")
                        return new TriggerStruct(Name, target);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player pangtong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(pangtong, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, pangtong, info.SkillPosition);
                room.DoSuperLightbox(pangtong, info.SkillPosition, Name);
                room.SetPlayerMark(pangtong, "@xingshuai", 0);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "wei" && room.AskForSkillInvoke(p, Name, "@xingshuai-src:" + player.Name))
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#xingshuai",
                        From = p.Name
                    };
                    room.SendLog(log);

                    p.SetFlags(Name);
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = p,
                        Recover = 1
                    };
                    room.Recover(player, recover, true);
                }
            }
            room.SetTag(Name, data);

            return false;
        }
    }

    public class Zhenlie : TriggerSkill
    {
        public Zhenlie() : base("zhenlie")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Masochism;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.From != null && use.From != player && player.Hp > 0)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || (fcard is TrickCard && !(fcard is DelayedTrick)))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && player.Hp > 0)
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

                bool invoke = room.AskForSkillInvoke(player, Name, string.Format("@zhenlie-from:{0}::{1}", use.From.Name, use.Card.Name), info.SkillPosition);

                player.RemoveTag(Name);
                room.RemoveTag(Name);
                if (invoke)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.LoseHp(player);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && data is CardUseStruct use)
            {
                if (use.From != null && use.From.Alive && RoomLogic.CanDiscard(room, player, use.From, "he"))
                {
                    int id = room.AskForCardChosen(player, use.From, "he", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(id, use.From, player);
                }

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

            return false;
        }
    }

    public class Miji : TriggerSkill
    {
        public Miji() : base("miji")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Replenish;
            view_as_skill = new MijiVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Finish && base.Triggerable(player, room) && player.IsWounded())
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
            int count = player.GetLostHp();
            room.DrawCards(player, count, Name);

            count = Math.Min(count, player.HandcardNum);
            int give = count;
            while (give > 0)
            {
                player.SetMark(Name, give);
                string pattern = count == give ? "@@miji" : "@@miji!";
                string prompt = count == give ? "@miji:::" + count.ToString() : "@miji-norefuse:::" + count.ToString();
                WrappedCard card = room.AskForUseCard(player, pattern, prompt, -1, HandlingMethod.MethodNone, true, info.SkillPosition);
                if (card != null)
                {
                    give -= card.SubCards.Count;
                }
                else
                {
                    if (pattern.EndsWith("!"))
                    {
                        Player target = room.GetOtherPlayers(player)[0];
                        List<int> ids = new List<int>();
                        List<int> cards = player.GetCards("h");
                        for (int i = 0; i < give; i++)
                            ids.Add(cards[i]);

                        List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty);
                        CardsMoveStruct move = new CardsMoveStruct(ids, target, Place.PlaceHand, reason);
                        moves.Add(move);

                        LogMessage l = new LogMessage
                        {
                            Type = "#ChoosePlayerWithSkill",
                            From = player.Name,
                            To = new List<string> { target.Name },
                            Arg = Name
                        };
                        room.SendLog(l);

                        room.MoveCardsAtomic(moves, false);
                    }
                    break;
                }
            }
            player.SetMark(Name, 0);

            return false;
        }
    }

    public class MijiVS : ViewAsSkill
    {
        public MijiVS() : base("miji")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return pattern.Contains("@miji");
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < player.GetMark(Name) && room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            WrappedCard ss = new WrappedCard(MijiCard.ClassName)
            {
                Mute = true
            };
            ss.AddSubCards(cards);
            return ss;
        }
    }

    public class MijiCard : SkillCard
    {
        public static string ClassName = "MijiCard";
        public MijiCard() : base(ClassName)
        {
            handling_method = HandlingMethod.MethodNone;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "miji", string.Empty);
            CardsMoveStruct move = new CardsMoveStruct(new List<int>(card_use.Card.SubCards), target, Place.PlaceHand, reason);
            moves.Add(move);

            LogMessage l = new LogMessage
            {
                Type = "#ChoosePlayerWithSkill",
                From = player.Name,
                Arg = "miji"
            };
            l.SetTos(card_use.To);
            room.SendLog(l);

            room.MoveCardsAtomic(moves, false);
        }
    }

    public class Quanji : MasochismSkill
    {
        public Quanji() : base("quanji")
        {
            skill_type = SkillType.Masochism;
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
            room.DrawCards(target, 1, Name);
            if (target.Alive && !target.IsKongcheng())
            {
                List<int> ids = room.AskForExchange(target, Name, 1, 1, "@quanji", string.Empty, ".", info.SkillPosition);
                room.AddToPile(target, Name, ids, true);
            }
        }
    }

    public class QuanjiMax : MaxCardsSkill
    {
        public QuanjiMax() : base("#quanji-max") { }

        public override int GetExtra(Room room, Player target)
        {
            return target.GetPile("quanji").Count;
        }
    }

    public class Zili : PhaseChangeSkill
    {
        public Zili() : base("zili")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.GetPile("quanji").Count > 2)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            List<string> choices = new List<string> { "draw" };
            if (player.GetLostHp() > 0)
            {
                choices.Add("recover");
            }
            if (room.AskForChoice(player, Name, string.Join("+", choices)) == "recover")
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(player, recover, true);
            }
            else
                room.DrawCards(player, 2, Name);

            room.LoseMaxHp(player);
            if (player.Alive)
                room.HandleAcquireDetachSkills(player, "paiyi", true);

            return false;
        }
    }

    public class Paiyi : OneCardViewAsSkill
    {
        public Paiyi() : base("paiyi")
        {
            filter_pattern = ".|.|.|quanji";
            expand_pile = "quanji";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetPile("quanji").Count > 0 && !player.HasUsed(PaiyiCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard shun = new WrappedCard(PaiyiCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            shun.AddSubCard(card);
            shun = RoomLogic.ParseUseCard(room, shun);
            return shun;
        }
    }

    public class PaiyiCard : SkillCard
    {
        public static string ClassName = "PaiyiCard";
        public PaiyiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            room.DrawCards(target, new DrawCardStruct(2, player, "paiyi"));
            if (target.Alive && player.Alive && target.HandcardNum > player.HandcardNum)
                room.Damage(new DamageStruct("paiyi", player, target));
        }
    }

    public class Chengxiang : MasochismSkill
    {
        public Chengxiang() : base("chengxiang")
        {
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

        public override void OnDamaged(Room room, Player player, DamageStruct damage, TriggerStruct info)
        {
            List<int> card_ids = room.GetNCards(4);
            foreach (int id in card_ids)
            {
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
        }
        public override bool MoveFilter(Room room, int id, List<int> downs)
        {
            int number = 0;
            foreach (int card_id in downs)
                number += room.GetCard(card_id).Number;
 
            if (id != -1)
                number += room.GetCard(id).Number;

            return number <= 13 && number > 0;
        }
    }

    public class Renxin : TriggerSkill
    {
        public Renxin() : base("renxin")
        {
            events.Add(TriggerEvent.DamageInflicted);
            skill_type = SkillType.Defense;
            view_as_skill = new RenxinVS();
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && player.Hp == 1)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    if (p != player)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForUseCard(ask_who, "@@renxin", "@renxin:" + player.Name, -1, HandlingMethod.MethodDiscard, true, info.SkillPosition) != null)
            {
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
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

    public class RenxinVS : OneCardViewAsSkill
    {
        public RenxinVS() : base("renxin")
        {
            filter_pattern = "EquipCard!";
            response_pattern = "@@renxin";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard rx = new WrappedCard(RenxinCard.ClassName) { Skill = Name };
            rx.AddSubCard(card);
            return rx;
        }
    }

    public class RenxinCard : SkillCard
    {
        public static string ClassName = "RenxinCard";
        public RenxinCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = false;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            room.TurnOver(player);
            room.ThrowCard(card_use.Card.GetEffectiveId(), player, player, "renxin");
        }
    }

    public class Jingce : TriggerSkill
    {
        public Jingce() : base("jingce")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.From == room.Current)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                    player.AddMark(Name);
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Who == room.Current && resp.Use)
            {
                player.AddMark(Name);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && base.Triggerable(player, room) && player.GetMark(Name) >= player.Hp)
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
            room.DrawCards(player, 2, Name);
            return false;
        }
    }

    public class Qingxi : TriggerSkill
    {
        public Qingxi() : base("qingxi")
        {
            events.Add(TriggerEvent.DamageCaused);
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who)
        {
            if (base.Triggerable(zhurong, room) && data is DamageStruct damage && damage.Card != null && zhurong.GetWeapon())
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && !zhurong.IsKongcheng() && !damage.To.IsKongcheng() && damage.To != zhurong && !damage.Chain && !damage.Transfer)
                    return new TriggerStruct(Name, zhurong);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && zhurong.GetWeapon())
            {
                room.SetTag(Name, data);
                bool invoke = room.AskForSkillInvoke(zhurong, Name, damage.To, info.SkillPosition);
                room.RemoveTag(Name);
                if (invoke)
                {
                    room.BroadcastSkillInvoke(Name, zhurong, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhurong, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && zhurong.GetWeapon())
            {
                Weapon fcard = (Weapon)Engine.GetFunctionCard(zhurong.Weapon.Value);
                int count = fcard.Range;
                bool discard = false;
                if (damage.To.HandcardNum >= count)
                {
                    damage.To.SetMark(Name, count);
                    room.SetTag(Name, data);
                    WrappedCard card = room.AskForUseCard(damage.To, "@@qingxi", string.Format("@qingxi:{0}::{1}:{2}", zhurong.Name, count, damage.Damage));
                    room.RemoveTag(Name);
                    damage.To.SetMark(Name, 0);
                    if (card != null)
                    {
                        List<int> ids = new List<int>(card.SubCards);
                        room.ThrowCard(ref ids, damage.To);
                        discard = true;

                        if (damage.To.Alive && zhurong.GetWeapon() && RoomLogic.CanDiscard(room, damage.To, zhurong, zhurong.Weapon.Key))
                            room.ThrowCard(zhurong.Weapon.Key, zhurong, damage.To);
                    }
                }
                
                if (!discard)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#AddDamage",
                        From = zhurong.Name,
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

    public class QingxiVS : ViewAsSkill
    {
        public QingxiVS() : base("qingxi")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseStruct.CardUseReason reason, string pattern, string position = null)
        {
            return pattern == "@@qingxi";
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < player.GetMark(Name) && room.GetCardPlace(to_select.Id) == Place.PlaceHand && RoomLogic.CanDiscard(room, player, player, to_select.Id);
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == player.GetMark(Name))
            {
                WrappedCard qx = new WrappedCard(QingxiCard.ClassName) { Mute = true };
                qx.AddSubCards(cards);
                return qx;
            }

            return null;
        }
    }

    public class QingxiCard : SkillCard
    {
        public static string ClassName = "QingxiCard";
        public QingxiCard() : base(ClassName)
        {
            will_throw = true;
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
        }
    }

    public class Qianju : DistanceSkill
    {
        public Qianju() : base("qianju")
        {
        }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            if (RoomLogic.PlayerHasShownSkill(room, from, this) && from.GetLostHp() > 0)
            {
                return -from.GetLostHp();
            }

            return 0;
        }
    }

    public class Jiangchi : TriggerSkill
    {
        public Jiangchi() : base("jiangchi")
        {
            events.Add(TriggerEvent.EventPhaseProceeding);
            skill_type = SkillType.Replenish;
        }
        
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Draw && (int)data >= 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            string choice = room.AskForChoice(player, Name, "more+less+cancel");

            if (choice != "cancel")
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", choice == "more" ? 2 : 1, gsk.General, gsk.SkinId);
                room.NotifySkillInvoked(player, Name);
                player.SetTag(Name, choice);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is int n)
            {
                string choice = player.GetTag(Name).ToString();
                if (choice == "more")
                {
                    n++;
                    RoomLogic.SetPlayerCardLimitation(player, "use,response", Slash.ClassName, true);
                }
                else
                {
                    n--;
                    player.SetFlags(Name);
                }

                data = n;
            }

            return false;
        }
    }

    public class JiangchiMod : TargetModSkill
    {
        public JiangchiMod() : base("#jiangchi-target", false)
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (from.HasFlag("jiangchi"))
                return 1;
            else
                return 0;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from.HasFlag("jiangchi") ? true : false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    //荀攸
    public class QiceJX : ViewAsSkill
    {
        public QiceJX() : base("qice_jx")
        {
            skill_type = SkillType.Alter;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player) => false;
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].Id < 0)
            {
                WrappedCard card = new WrappedCard(cards[0].Name) { Skill = Name };
                card.AddSubCards(player.GetCards("h"));
                return card;
            }

            return null;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed("ViewAsSkill_qice_jxCard"))
            {
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodUse))
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
                WrappedCard card = new WrappedCard(name);
                card.AddSubCards(player.GetCards("h"));
                card = RoomLogic.ParseUseCard(room, card);
                all_cards.Add(card);
            }
            return all_cards;
        }
    }


    public class Sidi : TriggerSkill
    {
        public Sidi() : base("sidi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
            priority = new Dictionary<TriggerEvent, double>
            {
                { TriggerEvent.CardUsedAnnounced, 3 }, { TriggerEvent.EventPhaseStart, 3 }, { TriggerEvent.EventPhaseEnd, 2 }, { TriggerEvent.EventPhaseChanging, 3 }
            };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && player.ContainsTag(Name) && player.Phase == PlayerPhase.Play)
                player.RemoveTag(Name);
            else if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play && player.ContainsTag(Name))
                player.RemoveTag(Name);
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
            {
                player.SetFlags("-sidi");
                if (player.GetMark("@qianxi_red") > 0)
                {
                    RoomLogic.RemovePlayerCardLimitation(player, "use,response", ".|red$0");
                    room.SetPlayerMark(player, "@qianxi_red", 0);
                }
                if (player.GetMark("@qianxi_black") > 0)
                {
                    RoomLogic.RemovePlayerCardLimitation(player, "use,response", ".|black$0");
                    room.SetPlayerMark(player, "@qianxi_black", 0);
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p != player && !p.IsNude() && p.GetEquips().Count > 0)
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive && ask_who.GetEquips().Count > 0)
            {
                bool red = false, black = false;
                foreach (int id in ask_who.GetEquips())
                {
                    WrappedCard card = room.GetCard(id);
                    if (WrappedCard.IsRed(card.Suit))
                        red = true;
                    else
                        black = true;
                }

                string pattern;
                if (red && black)
                    pattern = "^BasicCard!";
                else if (red)
                    pattern = "^BasicCard|red!";
                else
                    pattern = "^BasicCard|black!";

                List<int> ids = room.AskForExchange(ask_who, Name, 1, 0, "@sidi:" + player.Name, string.Empty, pattern, info.SkillPosition);
                if (ids.Count == 1)
                {
                    room.ThrowCard(ref ids, ask_who, null, Name);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);

                    string color = WrappedCard.IsRed(room.GetCard(ids[0]).Suit) ? "red" : "black";
                    player.SetTag(Name, color);

                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            string color = target.GetTag(Name).ToString();
            target.RemoveTag(Name);
            string pattern = string.Format(".|{0}$0", color);
            room.AddPlayerMark(target, "@qianxi_" + color);
            RoomLogic.SetPlayerCardLimitation(target, "use,response", pattern, true);
            target.SetFlags(Name);
            LogMessage log = new LogMessage
            {
                Type = "#NoColor",
                From = target.Name,
                Arg = "no_suit_" + color,
                Arg2 = Name
            };
            room.SendLog(log);

            List<string> names = target.ContainsTag(Name) ? (List<string>)target.GetTag(Name) : new List<string>();
            names.Add(ask_who.Name);
            target.SetTag(Name, names);

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            if (card.Name.Contains(Slash.ClassName))
                index = 2;
        }
    }
    public class SidiSlash : TriggerSkill
    {
        public SidiSlash() : base("#sidi-slash")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd };
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && player.Phase == PlayerPhase.Play && player.ContainsTag("sidi") && player.GetTag("sidi") is List<string> names)
            {
                List<Player> targets = new List<Player>();
                foreach (string name in names)
                {
                    Player target = room.FindPlayer(name);
                    if (target != null)
                        targets.Add(target);
                }

                if (targets.Count > 0)
                {
                    room.SortByActionOrder(ref targets);
                    foreach (Player p in targets)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.Alive && player.Alive)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "sidi", DistanceLimited = false };
                if (RoomLogic.IsProhibited(room, ask_who, player, slash) == null)
                    room.UseCard(new CardUseStruct(slash, ask_who, player));
            }

            return false;
        }
    }

    public class Pindi : TriggerSkill
    {
        public Pindi() : base("pindi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            view_as_skill = new PindiVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasFlag(Name))
                        p.SetFlags("-pindi");
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class PindiVS : OneCardViewAsSkill
    {
        public PindiVS() : base("pindi")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return true;
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(to_select.Name);
            return !player.HasFlag(string.Format("{0}_{1}", Name, fcard.TypeID));
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard pd = new WrappedCard(PindiCard.ClassName) { Skill = Name };
            pd.AddSubCard(card);
            return pd;
        }
    }

    public class PindiCard : SkillCard
    {
        public static string ClassName = "PindiCard";
        public PindiCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && !to_select.HasFlag("pindi");
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card_use.Card.GetEffectiveId()).Name);
            card_use.From.SetFlags(string.Format("pindi_{0}", fcard.TypeID));
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            target.SetFlags("pindi");
            player.AddMark("pindi");

            target.SetFlags("pindi_target");
            string choice = room.AskForChoice(player, "pindi", "draw+discard", new List<string> { "@pindi:" + target.Name }, target);
            target.SetFlags("-pindi_target");
            if (choice == "draw")
                room.DrawCards(target, new DrawCardStruct(player.GetMark("pindi"), player, "pindi"));
            else
                room.AskForDiscard(target, "pindi", player.GetMark("pindi"), player.GetMark("pindi"), false, true,
                    string.Format("@pindi-discard:{0}::{1}", player.Name, player.GetMark("pindi")), false, null);

            if (player.Alive && target.Alive && target.IsWounded() && !player.Chained)
                room.SetPlayerChained(player, true);
        }
    }

    public class Faen : TriggerSkill
    {
        public Faen() : base("faen")
        {
            events = new List<TriggerEvent> { TriggerEvent.TurnedOver, TriggerEvent.ChainStateChanged };
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.TurnedOver && player.Alive && player.FaceUp)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    triggers.Add(new TriggerStruct(Name, p));
            }
            else if (triggerEvent == TriggerEvent.ChainStateChanged && player.Chained)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive)
            {
                player.SetFlags(Name);
                bool invoke = room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition);
                player.SetFlags("-faen");
                if (invoke)
                {
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, new DrawCardStruct(1, ask_who, Name));

            return false;
        }
    }

    public class Shouxi : TriggerSkill
    {
        public Shouxi() : base("shouxi")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Defense;
        }

        private List<string> GetCardNames(Room room, Player player)
        {
            List<string> cards = new List<string>();
            List<string> used = player.ContainsTag(Name) ? (List<string>)player.GetTag(Name) : new List<string>();
            foreach (FunctionCard fcard in room.AvailableFunctionCards)
            {
                string card_name = fcard.Name;
                if (card_name.Contains(Slash.ClassName)) card_name = Slash.ClassName;
                if (!used.Contains(card_name) && !cards.Contains(card_name))
                    cards.Add(card_name);
            }

            return cards;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName)
                && use.From != null && use.From.Alive && GetCardNames(room, player).Count > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && use.From.Alive)
            {
                List<string> choices = GetCardNames(room, player);
                choices.Add("cancel");
                string choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { string.Format("@shouxi:{0}", use.From.Name) }, data);
                if (choice != "cancel")
                {
                    player.SetTag("shouxi_annouced", choice);
                    List<string> used = player.ContainsTag(Name) ? (List<string>)player.GetTag(Name) : new List<string>();
                    used.Add(choice);
                    player.SetTag(Name, used);

                    room.NotifySkillInvoked(player, Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && data is CardUseStruct use && use.From.Alive)
            {
                string card = player.GetTag("shouxi_annouced").ToString();
                player.RemoveTag("shouxi_annouced");

                player.SetFlags("shouxi_from");
                room.SetTag(Name, data);
                List<int> ids = room.AskForExchange(use.From, Name, 1, 0, string.Format("@shouxi-discard:{0}::{1}", player.Name, card), string.Empty,
                    string.Format("{0}!", card), string.Empty);
                player.SetFlags("-shouxi_from");
                room.RemoveTag(Name);

                if (ids.Count > 0)
                {
                    room.ThrowCard(ref ids, use.From);
                    if (use.From.Alive && player.Alive && RoomLogic.CanGetCard(room, use.From, player, "he"))
                    {
                        int id = room.AskForCardChosen(use.From, player, "he", Name, false, HandlingMethod.MethodGet);
                        room.ObtainCard(use.From, room.GetCard(id),
                            new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, use.From.Name, player.Name, Name, string.Empty), false);
                    }
                }
                else
                {
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
                                break;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Huimin : PhaseChangeSkill
    {
        public Huimin() : base("huimin")
        {
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum < p.Hp)
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>(), players = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < p.Hp)
                    targets.Add(p);

            if (targets.Count > 0 && room.AskForSkillInvoke(player, Name, "@huimin:::" + targets.Count.ToString(), info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            List<Player> targets = new List<Player>(), players = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < p.Hp)
                    targets.Add(p);

            room.DrawCards(player, targets.Count, Name);
            List<int> card_ids = room.AskForExchange(player, Name, targets.Count, targets.Count, "@huimin-show:::" + targets.Count.ToString(), string.Empty, ".", info.SkillPosition);

            CardsMoveStruct move = new CardsMoveStruct(card_ids, player, Place.PlaceTable, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_SHOW, player.Name, Name, null))
            {
                From = player.Name
            };
            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
            room.MoveCardsAtomic(moves, true);

            Player starter = room.AskForPlayerChosen(player, targets, Name, "@huimin-starter", false, false, info.SkillPosition);

            room.SortByActionOrder(ref targets);
            for (int i = targets.IndexOf(starter); i < targets.Count; i++)
                players.Add(targets[i]);

            for (int i = 0; i < targets.IndexOf(starter); i++)
                players.Add(targets[i]);

            foreach (Player p in players)
                room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

            Thread.Sleep(500);

            room.FillAG(Name, card_ids);
            List<CardsMoveStruct> list = new List<CardsMoveStruct>();
            foreach (Player p in players)
            {
                int card_id = room.AskForAG(p, card_ids, false, Name);
                card_ids.Remove(card_id);

                List<string> arg = new List<string> { p.Name, card_id.ToString(), "true" };
                room.DoBroadcastNotify(CommonClassLibrary.CommandType.S_COMMAND_TAKE_AMAZING_GRACE, arg);

                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, p.Name, Name, string.Empty);
                CardsMoveStruct _move = new CardsMoveStruct(card_id, p, Place.PlaceHand, reason);
                list.Add(_move);
            }
            if (list.Count > 0) room.MoveCardsAtomic(list, true);

            room.ClearAG();
            room.RemoveTag("HuiminOrigin");

            List<int> ag_list = new List<int>();
            foreach (int id in card_ids)
                if (room.GetCardPlace(id) == Place.PlaceTable)
                    ag_list.Add(id);

            if (ag_list.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, null, Name, null);
                room.ThrowCard(ref ag_list, reason, null);
            }

            return false;
        }
    }

    public class XianzhenCard : SkillCard
    {
        public static string ClassName = "XianzhenCard";
        public XianzhenCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && RoomLogic.CanBePindianBy(room, to_select, Self);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            PindianStruct pd = room.PindianSelect(player, target, "xianzhen");
            room.Pindian(ref pd);
            if (pd.Success)
            {
                player.SetTag("XianzhenTarget", target.Name);
                target.SetFlags("xianzhen");
                target.SetAmorNullified2(player, "xianzhen");
            }
            else
            {
                RoomLogic.SetPlayerCardLimitation(player, "use", Slash.ClassName, true);
            }
        }
    }

    public class XianzhenViewAsSkill : ZeroCardViewAsSkill
    {
        public XianzhenViewAsSkill() : base("xianzhen")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {

            return !player.HasUsed(XianzhenCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(XianzhenCard.ClassName) { Skill = Name };
        }
    }

    public class Xianzhen : TriggerSkill
    {
        public Xianzhen() : base("xianzhen")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            view_as_skill = new XianzhenViewAsSkill();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive
                && player.ContainsTag("XianzhenTarget") && player.GetTag("XianzhenTarget") is string target_name)
            {
                Player target = room.FindPlayer(target_name, true);
                if (target != null)
                {
                    target.SetFlags("-xianzhen");
                    target.RemoveArmorNullified(player);
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class XianzhenTar : TargetModSkill
    {
        public XianzhenTar() : base("#xianzhen-tar", false)
        {
            pattern = "^SkillCard";
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseStruct.CardUseReason reason, string pattern)
        {
            if (from != null && to != null && from == room.Current && to.HasFlag("xianzhen"))
                return true;

            return false;
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card)
        {
            if (from != null && to != null && from == room.Current && to.HasFlag("xianzhen"))
                return true;

            return false;
        }
    }

    public class JinjiuFilter : FilterSkill
    {
        public JinjiuFilter() : base("jinjiu")
        {
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            int id = to_select.GetEffectiveId();
            return to_select.Name == Analeptic.ClassName && (room.GetCardPlace(id) == Place.PlaceHand || room.GetCardPlace(id) == Place.PlaceJudge);
        }

        public override void ViewAs(Room room, ref WrappedCard card, Player player)
        {
            WrappedCard new_card = new WrappedCard(Slash.ClassName, card.Id, card.Suit, card.Number, card.CanRecast, card.Transferable)
            {
                CanRecast = card.CanRecast,
                Skill = Name,
                ShowSkill = card.ShowSkill,
                UserString = card.UserString,
                Flags = card.Flags,
                Mute = card.Mute,
            };

            card.TakeOver(new_card);
            card.Modified = true;
        }
    }

    public class Jinjiu : TriggerSkill
    {
        public Jinjiu() : base("jinjiu")
        {
            events = new List<TriggerEvent> { TriggerEvent.FinishRetrial, TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
            view_as_skill = new JinjiuFilter();
            skill_type = SkillType.Alter;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.FinishRetrial && base.Triggerable(player, room) && !RoomLogic.PlayerHasShownSkill(room, player, Name) && data is JudgeStruct judge
                && judge.Who == player && judge.Card.Name == Analeptic.ClassName)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card.Name == Slash.ClassName && use.Card.Skill == Name)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Card.Name == Slash.ClassName && resp.Card.Skill == Name)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.FinishRetrial && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                return info;
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced || triggerEvent == TriggerEvent.CardResponded)
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            return new TriggerStruct();
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

    public class Zhige : ZeroCardViewAsSkill
    {
        public Zhige() : base("zhige")
        {

        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ZhigeCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(ZhigeCard.ClassName) { Skill = Name };
        }
    }

    public class ZhigeCard : SkillCard
    {
        public static string ClassName = "ZhigeCard";
        public ZhigeCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && RoomLogic.InMyAttackRange(room, to_select, Self) && to_select != Self;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, target = effect.To;

            List<int> ids = new List<int>();
            if (!target.IsNude())
                ids = room.AskForExchange(target, "zhige", 1, 0, "@zhige-give:" + player.Name, string.Empty, "Slash#Weapon", string.Empty);

            if (ids.Count > 0)
                room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, target.Name, player.Name, "zhige", string.Empty), true);
            else
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_zhige" };
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(target))
                    if (RoomLogic.InMyAttackRange(room, target, p) && RoomLogic.IsProhibited(room, target, p, slash) == null)
                        targets.Add(p);

                if (targets.Count > 0)
                {
                    Player victim = room.AskForPlayerChosen(player, targets, "zhige", "@zhige-slash:" + target.Name, false, false, effect.Card.SkillPosition);
                    room.UseCard(new CardUseStruct(slash, target, victim, false));
                }
            }
        }
    }

    public class Zongzuo : TriggerSkill
    {
        public Zongzuo() : base("zongzuo")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.Death };
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.Death)
            {
                int count = 0;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.Kingdom == player.Kingdom) count++;

                if (count == 0)
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                        if (p != player) triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);

            if (triggerEvent == TriggerEvent.GameStart)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                int count = Fuli.GetKingdoms(room);
                ask_who.MaxHp += count;
                room.BroadcastProperty(ask_who, "MaxHp");

                LogMessage log = new LogMessage
                {
                    Type = "$GainMaxHp",
                    From = ask_who.Name,
                    Arg = count.ToString()
                };
                room.SendLog(log);

                room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, ask_who);

                ask_who.Hp += count;
                room.BroadcastProperty(ask_who, "Hp");

                List<string> arg = new List<string> { ask_who.Name, count.ToString(), "0" };
                room.DoBroadcastNotify(CommandType.S_COMMAND_CHANGE_HP, arg);

                room.RoomThread.Trigger(TriggerEvent.HpChanged, room, ask_who);
            }
            else
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                room.LoseMaxHp(ask_who);
                if (ask_who.Alive)
                    room.DrawCards(ask_who, 2, Name);
            }

            return false;
        }
    }

    public class Huaiyi : ZeroCardViewAsSkill
    {
        public Huaiyi() : base("huaiyi")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng() && !player.HasUsed(HuaiyiCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(HuaiyiCard.ClassName) { Skill = Name };
        }
    }

    public class HuaiyiCard : SkillCard
    {
        public static string ClassName = "HuaiyiCard";
        public HuaiyiCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            if (!player.IsKongcheng())
            {
                List<int> ids = player.GetCards("h");
                room.ShowAllCards(player, null, "huaiyi", card_use.Card.SkillPosition);

                List<int> black = new List<int>(), red = new List<int>();
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (WrappedCard.IsBlack(card.Suit))
                        black.Add(id);
                    else
                        red.Add(id);
                }

                if (black.Count > 0 && red.Count > 0)
                {
                    List<int> discard = new List<int>();
                    if (room.AskForChoice(player, "huaiyi", "red+black") == "red")
                    {
                        discard = red;
                    }
                    else
                        discard = black;

                    room.ThrowCard(ref discard, player);

                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (RoomLogic.CanGetCard(room, player, p, "he"))
                            targets.Add(p);
                    }

                    if (targets.Count > 0)
                    {
                        List<Player> players = room.AskForPlayersChosen(player, targets, "huaiyi", 1, discard.Count, "@huaiyi:::" + discard.Count.ToString(), true, card_use.Card.SkillPosition);
                        List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                        foreach (Player p in players)
                        {
                            int id = room.AskForCardChosen(player, p, "he", "huaiyi", false, HandlingMethod.MethodGet);
                            CardsMoveStruct move2 = new CardsMoveStruct
                            {
                                Card_ids = new List<int> { id },
                                To = player.Name,
                                To_place = Place.PlaceHand,
                                Reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, p.Name, "huaiyi", string.Empty)
                            };
                            moves.Add(move2);
                        }

                        room.MoveCardsAtomic(moves, false);

                        if (moves.Count > 1 && player.Alive)
                            room.LoseHp(player);
                    }
                }
            }
        }
    }

    public class Yaowu : TriggerSkill
    {
        public Yaowu() : base("yaowu")
        {
            events.Add(TriggerEvent.Damaged);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                if (WrappedCard.IsRed(damage.Card.Suit) && damage.From != null && damage.From.Alive)
                {
                    List<string> choices = new List<string> { "draw" };
                    if (damage.From.Alive) choices.Add("recover");
                    if (room.AskForChoice(damage.From, Name, string.Join("+", choices)) == "draw")
                    {
                        room.DrawCards(damage.From, 1, Name);
                    }
                    else
                    {
                        RecoverStruct recover = new RecoverStruct
                        {
                            Who = damage.From,
                            Recover = 1
                        };
                        room.Recover(damage.From, recover, true);
                    }
                }
                else
                {
                    room.DrawCards(player, 1, Name);
                }
            }

            return false;
        }

    }
    
    public class BuyiJX : TriggerSkill
    {
        public BuyiJX() : base("buyi_jx")
        {
            events.Add(TriggerEvent.Dying);
            skill_type = SkillType.Recover;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (data is DyingStruct dying)
            {
                List<Player> wuguotai = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in wuguotai)
                {
                    result.Add(new TriggerStruct(Name, p));
                }
            }
            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.HasFlag("Global_Dying") && player.Alive && !player.IsKongcheng() && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int id = -1;
            if (player == ask_who)
                id = room.AskForCardShow(player, player, Name);
            else
                id = room.AskForCardChosen(ask_who, player, "h", Name);
            room.ShowCard(player, id, Name);

            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
            if (!(fcard is BasicCard) && RoomLogic.CanDiscard(room, player, player, id))
            {
                room.ThrowCard(id, player);
                if (player.Alive)
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = ask_who,
                        Recover = 1
                    };
                    room.Recover(player, recover);
                }
            }
            return false;
        }
    }

    public class AnxuCard : SkillCard
    {
        public static string ClassName = "AnxuCard";
        public AnxuCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (to_select == Self)
                return false;
            if (targets.Count == 0)
                return true;
            else if (targets.Count == 1)
                return to_select.HandcardNum != targets[0].HandcardNum;
            else
                return false;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 2;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player from = null, to = null;
            if (card_use.To[0].HandcardNum > card_use.To[1].HandcardNum)
            {
                from = card_use.To[0];
                to = card_use.To[1];
            }
            else
            {
                from = card_use.To[1];
                to = card_use.To[0];
            }
            int id = room.AskForCardChosen(to, from, "h", "anxu");
            List<int> ids = new List<int> { id };
            room.ObtainCard(to, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, to.Name, from.Name, "anxu", string.Empty));
            if (ids.Count == 1 && room.GetCardOwner(ids[0]) == to)
            {
                room.ShowCard(to, ids[0], "anxu");
                if (room.GetCard(ids[0]).Suit != WrappedCard.CardSuit.Spade && card_use.From.Alive)
                    room.DrawCards(card_use.From, 1, "anxu");
            }
        }
    }

    public class Anxu : ZeroCardViewAsSkill
    {
        public Anxu() : base("anxu")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(AnxuCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard ax = new WrappedCard(AnxuCard.ClassName)
            {
                Skill = Name
            };
            return ax;
        }
    }

    public class Zhuiyi : TriggerSkill
    {
    public Zhuiyi() : base("zhuiyi")
    {
        events.Add(TriggerEvent.Death);
    }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return RoomLogic.PlayerHasSkill(room, player, Name) ? new TriggerStruct(Name, player) : new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DeathStruct death)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p != death.Damage.From)
                        targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@zuiyi", true, true, info.SkillPosition);
                    if (target != null)
                    {
                        player.SetTag(Name, target.Name);
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag(Name));
            room.DrawCards(target, new DrawCardStruct(3, player, Name));
            if (target.Alive && target.GetLostHp() > 0)
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(target, recover);
            }

            return false;
        }
    }

    public class ShenxingCard : SkillCard
    {
        public static string ClassName = "ShenxingCard";
        public ShenxingCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            room.DrawCards(card_use.From, 1, "shenxing");
        }
    }

    public class Shenxing : ViewAsSkill
    {
        public Shenxing() : base("shenxing")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude();
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (!RoomLogic.CanDiscard(room, player, player, to_select.Id)) return false;
            return selected.Count < 2;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 2)
            {
                WrappedCard sx = new WrappedCard(ShenxingCard.ClassName)
                {
                    Skill = Name
                };
                sx.AddSubCards(cards);
                return sx;
            }

            return null;
        }
    }

    public class Bingyi : PhaseChangeSkill
    {
        public Bingyi() : base("bingyi")
        {
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Finish && base.Triggerable(player, room) && !player.IsKongcheng())
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

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.ShowAllCards(player, null, Name, info.SkillPosition);
            List<int> ids = player.GetCards("h");
            bool black = WrappedCard.IsBlack(room.GetCard(ids[0]).Suit);
            bool same = true;
            foreach (int id in ids)
            {
                if (black != WrappedCard.IsBlack(room.GetCard(ids[0]).Suit))
                {
                    same = false;
                    break;
                }
            }
            if (same)
            {
                List<Player> players = room.AskForPlayersChosen(player, room.GetAlivePlayers(), Name, 1, player.HandcardNum,
                    string.Format("@bingyi:::{0}", player.HandcardNum), false, info.SkillPosition);
                room.SortByActionOrder(ref players);
                foreach (Player p in players)
                {
                    room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                    room.DrawCards(p, new DrawCardStruct(1, player, Name));
                }
            }

            return false;
        }
    }

    public class Anguo : ZeroCardViewAsSkill
    {
        public Anguo() : base("anguo")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(AnguoCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard ag = new WrappedCard(AnguoCard.ClassName)
            {
                Skill = Name
            };
            return ag;
        }
    }

    public class AnguoCard : SkillCard
    {
        public static string ClassName = "AnguoCard";
        public AnguoCard() : base(ClassName)
        { }

        public override void Use(Room room, CardUseStruct card_use)
        {
            bool draw = false;
            bool recover = false;
            bool use = false;
            Player player = card_use.From, target = card_use.To[0];
            int count = 100, hp = 100, eq = 100 ;
            foreach (Player p in room.GetAllPlayers())
            {
                if (p.HandcardNum < count)
                    count = p.HandcardNum;
                if (p.Hp < hp)
                    hp = p.Hp;
                if (p.GetEquips().Count < eq)
                    eq = p.GetEquips().Count;
            }
            if (target.Alive && target.HandcardNum == count)
            {
                room.DrawCards(target, new DrawCardStruct(1, player, "anguo"));
                draw = true;
            }
            if (target.Alive && target.Hp == hp)
            {
                RecoverStruct re = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(target, re, true);
                recover = true;
            }
            if (target.Alive && target.GetEquips().Count == eq)
            {
                foreach (int id in room.DrawPile)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, target, card))
                    {
                        room.UseCard(new CardUseStruct(card, target, new List<Player>(), false));
                        break;
                    }
                }
                use = true;
            }

            if (!draw && player.Alive && player.HandcardNum == count)
                room.DrawCards(player, new DrawCardStruct(1, player, "anguo"));
            if (!recover && player.Alive && player.Hp == hp)
            {
                RecoverStruct re = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(player, re, true);
            }
            if (!use && player.Alive && player.GetEquips().Count == eq)
            {
                foreach (int id in room.DrawPile)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, player, card))
                    {
                        room.UseCard(new CardUseStruct(card, player, new List<Player>(), false));
                        break;
                    }
                }
            }
        }
    }

    public class Wengua : TriggerSkill
    {
        public Wengua() : base("wengua")
        {
            events.Add(TriggerEvent.GameStart);
            view_as_skill = new WenguaVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (base.Triggerable(player, room))
            {
                if (!room.Skills.Contains("wenguavs"))
                    room.Skills.Add("wenguavs");
                foreach (Player p in room.GetOtherPlayers(player))
                    room.HandleAcquireDetachSkills(p, "wenguavs", true);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class WenguaVS : OneCardViewAsSkill
    {
        public WenguaVS() : base("wengua")
        {
            filter_pattern = "..";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(WenguaCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard wg = new WrappedCard(WenguaCard.ClassName) { Skill = Name };
            wg.AddSubCard(card);
            return wg;
        }
    }

    public class WenguaAttach : OneCardViewAsSkill
    {
        public WenguaAttach() : base("wenguavs")
        {
            filter_pattern = "..";
            attached_lord_skill = true;
            frequency = Frequency.Compulsory;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(WenguaCard.ClassName) && RoomLogic.FindPlayerBySkillName(room, "wengua") != null;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard wg = new WrappedCard(WenguaCard.ClassName) { Skill = "wengua", Mute = true };
            wg.AddSubCard(card);
            return wg;
        }
    }

    public class WenguaCard : SkillCard
    {
        public static string ClassName = "WenguaCard";
        public WenguaCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            List<Player> xushi = RoomLogic.FindPlayersBySkillName(room, "wengua");
            if (xushi.Count <= 1) return false;
            return targets.Count == 0 && xushi.Contains(to_select);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            List<Player> xushi = RoomLogic.FindPlayersBySkillName(room, "wengua");
            if (xushi.Count == 1) return targets.Count == 0;
            if (xushi.Count > 1) return targets.Count == 1;
            return false;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = RoomLogic.FindPlayerBySkillName(room, "wengua");
            if (card_use.To.Count == 1) target = card_use.To[0];
            Player player = card_use.From;

            if (card_use.Card.Mute)
            {
                room.NotifySkillInvoked(target, "wengua");
                room.BroadcastSkillInvoke("wengua", target);
            }

            if (player != target)
                room.ObtainCard(target, card_use.Card,
                    new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "wengua", string.Empty), false);

            int id = card_use.Card.GetEffectiveId();
            if (target.Alive && (room.GetCardPlace(id) == Place.PlaceHand || (player == target && room.GetCardPlace(id) == Place.PlaceEquip)) && room.GetCardOwner(id) == target)
            {
                string choice = room.AskForChoice(target, "wengua", "top+bottom+cancel", null, id);
                if (choice == "top")
                {
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PUT, target.Name, "wengua", string.Empty);
                    CardsMoveStruct move = new CardsMoveStruct(new List<int> { id }, null, Place.DrawPile, reason)
                    {
                        From = target.Name
                    };
                    List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
                    room.MoveCardsAtomic(moves, true);

                    if (target.Alive)
                    {
                        int card_id = room.DrawPile[room.DrawPile.Count - 1];
                        room.ObtainCard(target, room.GetCard(card_id), new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DRAW, target.Name, "wengua", string.Empty), false);
                    }
                    if (player != target && player.Alive)
                    {
                        int card_id = room.DrawPile[room.DrawPile.Count - 1];
                        room.ObtainCard(player, room.GetCard(card_id), new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DRAW, player.Name, "wengua", string.Empty), false);
                    }
                }
                else if (choice == "bottom")
                {
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PUT, target.Name, "wengua", string.Empty);
                    CardsMoveStruct move = new CardsMoveStruct(new List<int> { id }, null, Place.DrawPileBottom, reason)
                    {
                        From = target.Name
                    };
                    List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
                    room.MoveCardsAtomic(moves, true);

                    if (target.Alive)
                        room.DrawCards(target, 1, Name);
                    if (player != target && player.Alive)
                        room.DrawCards(player, new DrawCardStruct(1, target, "wengua"));
                }
            }
        }
    }

    public class Fuzhu : TriggerSkill
    {
        public Fuzhu() : base("fuzhu")
        {
            skill_type = SkillType.Attack;
            events.Add(TriggerEvent.EventPhaseStart);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.IsMale() && player.Alive && player.Phase == PlayerPhase.Finish)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    if (p.Hp * 10 >= room.DrawPile.Count)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.Alive && player.Alive && ask_who.Hp * 10 >= room.DrawPile.Count
                && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = room.Players.Count;
            int used = 0;
            int swap = (int)room.GetTag("SwapPile");
            while (used < count && player.Alive && ask_who.Alive && room.GetTag("SwapPile") is int times && times == swap)
            {
                bool slash = false;
                List<int> ids = new List<int>(room.DrawPile);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (Engine.GetFunctionCard(card.Name) is Slash && RoomLogic.IsProhibited(room, ask_who, player, card) == null)
                    {
                        slash = true;
                        used++;
                        room.UseCard(new CardUseStruct(card, ask_who, player, false));
                        Thread.Sleep(500);
                        break;
                    }
                }
                if (!slash) break;
            }
            room.SwapPile();

            return false;
        }
    }
    
    public class Zenhui : TriggerSkill
    {
        public Zenhui() : base("zenhui")
        {
            events.Add(TriggerEvent.TargetChoosing);
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.To.Count == 1 && use.Card.ExtraTarget && player.Phase == PlayerPhase.Play && base.Triggerable(player, room) && !player.HasFlag(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if ((fcard is Slash || (fcard is TrickCard && !(fcard is DelayedTrick) && !(fcard is Nullification))) && WrappedCard.IsBlack(use.Card.Suit))
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (!use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, use.Card, use.To) == null)
                            return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);

                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if ((fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, player, p))
                        || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && !RoomLogic.CanGetCard(room, player, p, "hej"))
                        || (fcard is Dismantlement && !RoomLogic.CanDiscard(room, player, p, "hej")) || (fcard is Collateral && !p.GetWeapon())) continue;

                    if (fcard is Collateral)
                    {
                        bool check = false;
                        foreach (Player _p in room.GetOtherPlayers(p))
                        {
                            if (RoomLogic.CanSlash(room, p, _p))
                            {
                                check = true;
                                break;
                            }
                        }
                        if (!check) continue;
                    }

                    if (!use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, use.Card, use.To) == null)
                        targets.Add(p);
                }

                if (targets.Count > 0)
                {
                    room.SetTag("zenhui_data", data);
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@zenhui:::" + use.Card.Name, true, true, info.SkillPosition);
                    room.RemoveTag("zenhui_data");
                    if (target != null)
                    {
                        player.SetTag(Name, target.Name);
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            if (data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                Player target = room.FindPlayer(player.GetTag(Name).ToString());
                player.RemoveTag(Name);

                bool give = false;
                if (!target.IsNude())
                {
                    room.SetTag("zenhui_data", data);
                    List<int> ids = room.AskForExchange(target, Name, 1, 0, string.Format("@zenhui-give:{0}:{1}:{2}", player.Name, use.To[0].Name, use.Card.Name), string.Empty, "..", string.Empty);
                    room.RemoveTag("zenhui_data");
                    if (ids.Count > 0)
                    {
                        room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, string.Empty), false);
                        give = true;
                    }
                }

                if (!give)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#zenhui-add",
                        To = new List<string> { target.Name },
                        Arg = use.Card.Name
                    };
                    room.SendLog(log);

                    use.To.Add(target);
                    use.EffectCount.Add(fcard.FillCardBasicEffct(room, target));
                    room.SortByActionOrder(ref use);

                    if (fcard is Collateral)
                    {
                        List<Player> targets = new List<Player>();
                        foreach (Player p in room.GetOtherPlayers(target))
                            if (RoomLogic.CanSlash(room, target, p))
                                targets.Add(p);

                        Player victim = room.AskForPlayerChosen(player, targets, Name, "@Collateral:" + target.Name, false, false, info.SkillPosition);
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, victim.Name);
                        target.SetTag("collateralVictim", victim.Name);
                    }
                }
                else
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#zenhui-change",
                        To = new List<string> { target.Name },
                        Arg = use.Card.Name
                    };
                    room.SendLog(log);

                    use.From = target;
                }

                data = use;
            }

            return false;
        }
    }
    public class Jiaojin : TriggerSkill
    {
        public Jiaojin() : base("jiaojin")
        {
            events.Add(TriggerEvent.DamageInflicted);
            skill_type = SkillType.Defense;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.From != null && damage.From.IsMale() && base.Triggerable(player, room) && !player.IsNude())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SetTag("jiaojin_data", data);
                List<int> ids = room.AskForExchange(player, Name, 1, 0, string.Format("@jiaojin:{0}::{1}", damage.From.Name, damage.Damage), string.Empty, "EquipCard!", info.SkillPosition);
                room.RemoveTag("jiaojin_data");
                if (ids.Count > 0)
                {
                    room.ThrowCard(ref ids, player, null, Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#ReduceDamage",
                From = player.Name,
                Arg = Name,
                Arg2 = (--damage.Damage).ToString()
            };
            room.SendLog(log);

            data = damage;
            if (damage.Damage < 1)
                return true;

            return false;
        }
    }

    public class Xuanfeng : TriggerSkill
    {
        public Xuanfeng() : base("xuanfeng")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseEnd };
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.Discard
                && base.Triggerable(move.From, room) && (move.From_places.Contains(Place.PlaceHand) || move.From_places.Contains(Place.PlaceEquip))
                && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD)
                move.From.AddMark(Name, move.Card_ids.Count);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null
                && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceEquip))
            {
                List<Player> other_players = room.GetOtherPlayers(move.From);
                foreach (Player p in other_players)
                {
                    if (RoomLogic.CanDiscard(room, move.From, p, "he"))
                        return new TriggerStruct(Name, move.From);
                }
            }
            if (triggerEvent == TriggerEvent.EventPhaseEnd && base.Triggerable(player, room) && player.Phase == PlayerPhase.Discard && player.GetMark(Name) > 1)
            {
                List<Player> other_players = room.GetOtherPlayers(player);
                foreach (Player p in other_players)
                {
                    if (RoomLogic.CanDiscard(room, player, p, "he"))
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lengtong, TriggerStruct info)
        {
            List<Player> other_players = room.GetOtherPlayers(lengtong);
            List<Player> targets = new List<Player>();
            foreach (Player p in other_players)
            {
                if (RoomLogic.CanDiscard(room, lengtong, p, "he"))
                    targets.Add(p);
            }
            List<Player> to = room.AskForPlayersChosen(lengtong, targets, Name, 0, 2, "@xuanfeng", true, info.SkillPosition);
            if (to.Count > 0)
            {
                room.SetTag("liefeng_target", to);
                room.BroadcastSkillInvoke(Name, lengtong, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lengtong, TriggerStruct info)
        {
            List<Player> tos = (List<Player>)room.GetTag("liefeng_target");
            room.RemoveTag("liefeng_target");
            if (tos.Count == 1)
            {
                if (RoomLogic.CanDiscard(room, lengtong, tos[0], "he"))
                {
                    int card_id = room.AskForCardChosen(lengtong, tos[0], "he", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(card_id, tos[0], lengtong);

                    if (lengtong.Alive && tos[0].Alive && RoomLogic.CanDiscard(room, lengtong, tos[0], "he"))
                    {
                        card_id = room.AskForCardChosen(lengtong, tos[0], "he", Name, false, HandlingMethod.MethodDiscard);
                        room.ThrowCard(card_id, tos[0], lengtong);
                    }
                }
            }
            else
            {
                room.SortByActionOrder(ref tos);
                foreach (Player p in tos)
                {
                    if (RoomLogic.CanDiscard(room, lengtong, p, "he"))
                    {
                        List<int> ids = new List<int>
                        {
                            room.AskForCardChosen(lengtong, p, "he", Name, false, HandlingMethod.MethodDiscard)
                        };

                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISMANTLE, lengtong.Name, p.Name, Name, null)
                        {
                            General = RoomLogic.GetGeneralSkin(room, lengtong, Name, info.SkillPosition)
                        };
                        room.ThrowCard(ref ids, reason, p, lengtong);
                    }
                }
            }
            return false;
        }
    }

    public class XuanfengClear : TriggerSkill
    {
        public XuanfengClear() : base("#xuanfeng-clear")
        {
            events.Add(TriggerEvent.EventPhaseEnd);
        }

        public override int GetPriority()
        {
            return -2;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && player.GetMark("xuanfeng") > 0)
                player.SetMark("xuanfeng", 0);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Yaoming : TriggerSkill
    {
        public Yaoming() : base("yaoming")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.Damage, TriggerEvent.Damaged };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.ContainsTag(Name))
                        p.RemoveTag(Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if ((triggerEvent == TriggerEvent.Damage || triggerEvent == TriggerEvent.Damaged) && base.Triggerable(player, room))
            {
                List<int> counts = player.ContainsTag(Name) ? (List<int>)player.GetTag(Name) : new List<int>();
                if (counts.Count < 3)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> counts = player.ContainsTag(Name) ? (List<int>)player.GetTag(Name) : new List<int>();
            List<Player> targets = new List<Player>();
            if (!counts.Contains(1))
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.HandcardNum < player.HandcardNum) targets.Add(p);
            }

            if (!counts.Contains(2))
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.HandcardNum > player.HandcardNum && RoomLogic.CanDiscard(room, player, p, "he")) targets.Add(p);
            }

            if (!counts.Contains(3))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum == player.HandcardNum && !p.IsNude() && RoomLogic.CanDiscard(room, p, p, "he")) targets.Add(p);
            }

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@yaoming", true, true, info.SkillPosition);
                if (target != null)
                {
                    player.SetTag("yaoming_target", target.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> counts = player.ContainsTag(Name) ? (List<int>)player.GetTag(Name) : new List<int>();
            Player target = room.FindPlayer(player.GetTag("yaoming_target").ToString());
            player.RemoveTag("yaoming_target");

            if (player.HandcardNum < target.HandcardNum)
            {
                counts.Add(2);
                player.SetTag(Name, counts);

                int id = room.AskForCardChosen(player, target, "he", Name, false, HandlingMethod.MethodDiscard);
                room.ThrowCard(id, target, player);
            }
            else if (player.HandcardNum > target.HandcardNum)
            {
                counts.Add(1);
                player.SetTag(Name, counts);
                room.DrawCards(target, new DrawCardStruct(1, player, Name));
            }
            else
            {
                counts.Add(3);
                player.SetTag(Name, counts);

                List<int> ids = room.AskForExchange(target, Name, 2, 1, "@yaoming-dis", string.Empty, "..!", info.SkillPosition);
                room.ThrowCard(ref ids, target);
                room.DrawCards(target, ids.Count, Name);
            }

            return false;
        }
    }

    public class Kuangbi : PhaseChangeSkill
    {
        public Kuangbi() : base("kuangbi")
        {
            view_as_skill = new KuangbiVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == PlayerPhase.RoundStart && player.GetPile(Name).Count > 0)
            {
                List<int> ids = player.GetPile(Name);
                int count = ids.Count;
                if (count > 0)
                    room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty), true);

                Player target = room.FindPlayer(player.GetTag(Name).ToString());
                player.RemoveTag(Name);
                if (target != null)
                    room.DrawCards(target, count, Name);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            return false;
        }
    }

    public class KuangbiClear : DetachEffectSkill
    {
        public KuangbiClear() : base("kuangbi", "kuangbi") { }
    }

    public class KuangbiVS : ZeroCardViewAsSkill
    {
        public KuangbiVS() : base("kuangbi") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(KuangbiCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(KuangbiCard.ClassName) { Skill = Name };
        }
    }

    public class KuangbiCard : SkillCard
    {
        public static string ClassName = "KuangbiCard";
        public KuangbiCard() : base(ClassName)
        { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && Self != to_select && !to_select.IsNude();
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = room.AskForExchange(target, "kuangbi", 3, 1, "@kuangbi:" + player.Name, string.Empty, "..", string.Empty);
            if (ids.Count > 0)
            {
                room.AddToPile(player, "kuangbi", ids, true);
                player.SetTag("kuangbi", target.Name);
            }
        }
    }

    public class Fenli : TriggerSkill
    {
        public Fenli() : base("fenli")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Wizzard;
        }
        private readonly List<string> phase_strings = new List<string> { "round_start" , "start" , "judge" , "draw"
                , "play" , "discard", "finish" , "not_active" };
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is PhaseChangeStruct change && base.Triggerable(player, room))
            {
                bool invoke = false;
                switch (change.To)
                {
                    case PlayerPhase.Draw:
                        {
                            int hp = 0;
                            foreach (Player p in room.GetAlivePlayers())
                                if (p.HandcardNum > hp) hp = p.HandcardNum;

                            if (player.HandcardNum == hp) invoke = true;
                        }
                        break;
                    case PlayerPhase.Play:
                        {
                            int hp = 0;
                            foreach (Player p in room.GetAlivePlayers())
                                if (p.Hp > hp) hp = p.Hp;

                            if (player.Hp == hp) invoke = true;
                        }
                        break;
                    case PlayerPhase.Discard:
                        if (player.HasEquip())
                        {
                            int hp = 0;
                            foreach (Player p in room.GetAlivePlayers())
                                if (p.GetEquips().Count > hp) hp = p.GetEquips().Count;

                            if (player.GetEquips().Count == hp) invoke = true;
                        }
                        break;
                    default:
                        return new TriggerStruct();
                }

                if (invoke)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player zhanghe, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            int index = (int)(change.To);
            string discard_prompt = string.Format("@fenli:::{0}", phase_strings[index]);

            if (room.AskForSkillInvoke(zhanghe, Name, discard_prompt, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, zhanghe, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            room.SkipPhase(player, change.To);
            return false;
        }
    }

    public class Pingkou : TriggerSkill
    {
        public Pingkou() : base("pingkou")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseSkipping, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseSkipping && data is PhaseChangeStruct change && base.Triggerable(player, room) && room.Current == player)
            {
                player.AddMark(Name);
                room.SetPlayerStringMark(player, Name, player.GetMark(Name).ToString());
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct _change && _change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && player.GetMark(Name) > 0)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = room.AskForPlayersChosen(player, room.GetOtherPlayers(player), Name, 0, player.GetMark(Name),
                "@pingkou:::" + player.GetMark(Name).ToString(), true, info.SkillPosition);
            if (targets.Count > 0)
            {
                room.SetTag(Name, targets);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = (List<Player>)room.GetTag(Name);
            room.RemoveTag(Name);
            foreach (Player p in targets)
                if (p.Alive) room.Damage(new DamageStruct(Name, player, p));

            return false;
        }
    }

    public class Lihuo : TriggerSkill
    {
        public Lihuo() : base("lihuo")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.CardTargetAnnounced, TriggerEvent.DamageDone, TriggerEvent.CardFinished };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && use.Card != null && use.Card.Name == FireSlash.ClassName
                && player.Alive && player.GetMark(RoomLogic.CardToString(room, use.Card)) > 0)
            {
                if (player.GetMark(RoomLogic.CardToString(room, use.Card)) > 1)
                    room.LoseHp(player);

                player.SetMark(RoomLogic.CardToString(room, use.Card), 0);
            }
            else if (triggerEvent == TriggerEvent.DamageDone && data is DamageStruct damage && damage.Card != null && damage.Card.Name == FireSlash.ClassName
                && damage.From.Alive && damage.From.GetMark(RoomLogic.CardToString(room, damage.Card)) == 1)
            {
                damage.From.AddMark(RoomLogic.CardToString(room, damage.Card));
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null && use.Card.Name == Slash.ClassName)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct _use && _use.Card != null
                && _use.Card.Name == FireSlash.ClassName && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                List<Player> selected = new List<Player>(_use.To);
                foreach (Player p in room.GetOtherPlayers(player))
                    if (!_use.To.Contains(p) && fcard.ExtratargetFilter(room, selected, p, player, _use.Card))
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && room.AskForSkillInvoke(player, Name, data))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced)
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
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@extra_targets1:::FireSlash", true, false, info.SkillPosition);
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
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;

            if (triggerEvent == TriggerEvent.CardUsedAnnounced)
            {
                LogMessage log = new LogMessage
                {
                    Type = "$lihuo",
                    From = use.From.Name,
                    Card_str = RoomLogic.CardToString(room, use.Card)
                };

                WrappedCard fire = new WrappedCard(FireSlash.ClassName);
                fire.AddSubCard(use.Card);
                fire = RoomLogic.ParseUseCard(room, fire);
                fire.Skill = use.Card.Skill;
                fire.UserString = use.Card.UserString;
                fire.Mute = true;
                use.Card = fire;
                Thread.Sleep(400);

                player.SetMark(RoomLogic.CardToString(room, fire), 1);
                log.Card_str = RoomLogic.CardToString(room, fire);
                room.SendLog(log);
            }
            else
            {
                Player target = room.FindPlayer((string)player.GetTag("extra_targets"));
                player.RemoveTag("extra_targets");

                if (target != null)
                {
                    use.To.Add(target);
                    room.SortByActionOrder(ref use);
                }
            }

            data = use;

            return false;
        }
    }

    public class Chunlao : TriggerSkill
    {
        public Chunlao() : base("chunlao")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            view_as_skill = new ChunlaoVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish
                && !player.IsKongcheng() && player.GetPile("chun").Count == 0)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.AskForPeaches && data is DyingStruct dying && dying.Who.Alive && base.Triggerable(player, room) && player.GetPile("chun").Count > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                List<int> ids = room.AskForExchange(player, Name, 200, 0, "@chunlao", string.Empty, "Slash", info.SkillPosition);
                if (ids.Count > 0)
                {
                    room.NotifySkillInvoked(player, Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    room.AddToPile(player, "chun", ids);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return false;
        }
    }

    public class ChunlaoVS : OneCardViewAsSkill
    {
        public ChunlaoVS() : base("chunlao")
        {
            filter_pattern = ".|.|.|chun";
            expand_pile = "chun";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            WrappedCard peach = new WrappedCard(Analeptic.ClassName);
            FunctionCard fcard = Analeptic.Instance;
            Player target = room.GetRoomState().GetCurrentAskforPeachPlayer();
            if (target != null && fcard.IsAvailable(room, target, peach)) return true;

            return false;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard c = new WrappedCard(ChunlaoCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name,
                Mute = true
            };
            c.AddSubCard(card);
            return c;
        }
    }

    public class ChunlaoCard : SkillCard
    {
        public static string ClassName = "ChunlaoCard";
        public ChunlaoCard() : base(ClassName)
        {
            handling_method = HandlingMethod.MethodNone;
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, card_use.From, "chunlao", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("chunlao", "male", 2, gsk.General, gsk.SkinId);

            WrappedCard peach = new WrappedCard(Analeptic.ClassName) { Skill = "_chunlao" };
            room.UseCard(new CardUseStruct(peach, card_use.To[0], new List<Player>(), false));
        }
    }

    public class Pojun : TriggerSkill
    {
        public Pojun() : base("pojun")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.TargetChosen };
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetPile(Name).Count > 0)
                    {
                        List<int> ids = p.GetPile(Name);
                        room.ObtainCard(p, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXCHANGE_FROM_PILE, p.Name, Name, string.Empty), false);
                    }
                }
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
                        if (p.Hp > 0 && !p.IsNude())
                            targets.Add(p);
                    }
                    if (targets.Count > 0)
                    return new TriggerStruct(Name, player, targets);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player player, TriggerStruct info)
        {
            if (!skill_target.IsNude() && player.Alive && skill_target.Hp > 0 && room.AskForSkillInvoke(player, Name, skill_target, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player machao, TriggerStruct info)
        {
            List<int> cards = new List<int>();
            skill_target.SetFlags("continuous_card_chosen");
            room.SetTag("askforCardsChosen", skill_target.GetCards("he"));
            room.SetTag(Name, data);
            string prompt;
            do
            {
                cards.Add(room.AskForCardChosen(machao, skill_target, "he", Name, false, HandlingMethod.MethodNone, cards));
                prompt = string.Format("@pojun:{0}::{1}:{2}", skill_target.Name, skill_target.Hp, cards.Count);
                machao.SetTag(Name, cards);
            }
            while (cards.Count < skill_target.Hp && cards.Count < skill_target.GetCardCount(true) && room.AskForSkillInvoke(machao, Name, prompt, info.SkillPosition));
            skill_target.SetFlags("-continuous_card_chosen");
            room.RemoveTag("askforCardsChosen");

            room.RemoveTag(Name);
            machao.RemoveTag(Name);

            room.AddToPile(skill_target, Name, cards);

            return false;
        }
    }

    public class GongqiCard:SkillCard
{
        public static string ClassName = "GongqiCard";
        public GongqiCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            source.SetFlags("InfinityAttackRange");
            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card_use.Card.GetEffectiveId()).Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, source, "gongqi", card_use.Card.SkillPosition);
            if (fcard is EquipCard)
            {
                room.BroadcastSkillInvoke("gongqi", "male", 2, gsk.General, gsk.SkinId);
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(source))
                    if (RoomLogic.CanDiscard(room, source, p, "he")) targets.Add(p);
                if (targets.Count > 0)
                {
                    Player to_discard = room.AskForPlayerChosen(source, targets, "gongqi", "@gongqi-discard", true, false, card_use.Card.SkillPosition);
                    if (to_discard != null)
                        room.ThrowCard(room.AskForCardChosen(source, to_discard, "he", "gongqi", false, HandlingMethod.MethodDiscard), to_discard, source);
                }
            }
            else
            {
                room.BroadcastSkillInvoke("gongqi", "male", 1, gsk.General, gsk.SkinId);
            }
        }
    }

    public class Gongqi : OneCardViewAsSkill
    {
        public Gongqi() : base("gongqi")
        {
            filter_pattern = ".!";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(GongqiCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard gq = new WrappedCard(GongqiCard.ClassName) { Skill = Name, Mute = true };
            gq.AddSubCard(card);
            return gq;
        }
    }

public class JiefanCard: SkillCard
{
        public static string ClassName = "JiefanCard";
        public JiefanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@rescue", 0);
            room.BroadcastSkillInvoke("jiefan", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "jiefan");

            Player target = card_use.To[0];
            card_use.From.SetTag("jiefan", target.Name);
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, card_use.From.Name, target.Name);
            card_use.To.Clear();
            foreach (Player p in room.GetOtherPlayers(target))
                if (RoomLogic.InMyAttackRange(room, p, target))
                    card_use.To.Add(p);

            base.OnUse(room, card_use);

            card_use.From.RemoveTag("jiefan");
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player target = room.FindPlayer(effect.From.GetTag("jiefan").ToString(), true);
            string pattern = ".Weapon";
            if (!target.Alive) pattern = ".Weapon!";
            if (room.AskForCard(effect.To, "jiefan", pattern, "@jiefan-discard:" + target.Name, effect, HandlingMethod.MethodDiscard, null) == null && target.Alive)
                room.DrawCards(target, 1, "jiefan");
        }
    }

    public class Jiefan : ZeroCardViewAsSkill
    {
        public Jiefan() : base("jiefan")
        {
            frequency = Frequency.Limited;
            limit_mark = "@rescue";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(limit_mark) >= 1;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JiefanCard.ClassName) { Skill = Name, Mute = true };
        }
    }

    public class Danshou : TriggerSkill
    {
        public Danshou() : base("danshou")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging, TriggerEvent.TargetConfirmed };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    p.SetMark(Name, 0);
                    if (p.HasFlag(Name))
                        p.SetFlags("-danshou");
                }
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use && base.Triggerable(player, room) && !player.HasFlag(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is BasicCard || (fcard is TrickCard && !(fcard is Nullification)))
                    player.AddMark(Name);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use && base.Triggerable(player, room) && player.GetMark(Name) > 0 && !player.HasFlag(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is BasicCard || (fcard is TrickCard && !(fcard is Nullification)))
                    triggers.Add(new TriggerStruct(Name, player));
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.Finish && !player.IsKongcheng())
            {
                List<Player> zhurans = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in zhurans)
                    if (p != player && !p.HasFlag(Name))
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);

            if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use)
            {
                string prompt;
                if (use.From != null)
                    prompt = string.Format("@danshou-from:{0}::{1}:{2}", use.From.Name, use.Card.Name, player.GetMark(Name));
                else
                    prompt = string.Format("@danshou:::{0}:{1}", use.Card.Name, player.GetMark(Name));

                if (room.AskForSkillInvoke(player, Name, prompt, info.SkillPosition))
                {
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    return info;
                }
            }
            else if (!player.IsKongcheng() && ask_who.GetCardCount(true) >= player.HandcardNum && room.AskForDiscard(ask_who, Name, player.HandcardNum, player.HandcardNum, true, true,
                string.Format("@danshou-discard:{0}::{1}", player.Name, player.HandcardNum), true, info.SkillPosition) && ask_who.Alive)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetConfirmed)
            {
                player.SetFlags(Name);
                room.DrawCards(player, player.GetMark(Name), Name);
            }
            else if (player.Alive && ask_who.Alive)
            {
                room.Damage(new DamageStruct(Name, ask_who, player));
            }

            return false;
        }
    }

    public class Duodao : TriggerSkill
    {
        public Duodao() : base("duodao")
        {
            events.Add(TriggerEvent.Damaged);
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && base.Triggerable(player, room) && !player.IsNude())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SetTag(Name, data);
                bool invoke = room.AskForDiscard(player, Name, 1, 1, true, true, "@duodao:" + damage.From.Name, true, info.SkillPosition);
                room.RemoveTag(Name);

                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && damage.From != null && damage.From.Alive && player.Alive && damage.From.GetWeapon()
                && RoomLogic.CanGetCard(room, player, damage.From, damage.From.Weapon.Key))
            {
                List<int> ids = new List<int> { damage.From.Weapon.Key };
                room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, damage.From.Name, Name, string.Empty));
            }

            return false;
        }
    }

    public class Anjian : TriggerSkill
    {
        public Anjian() : base("anjian")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused };
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From != damage.To && !player.IsKongcheng()
                && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && !damage.Chain && !damage.Transfer && !RoomLogic.InMyAttackRange(room, damage.To, player))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
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
}
