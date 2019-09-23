using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
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

                new BuyiJX(),
                new Anxu(),
                new Zhuiyi(),
                new Shenxing(),
                new Bingyi(),
                new Anguo(),
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
                    if (p.HandcardNum == player.HandcardNum)
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
            if (damage.To == player && player.GetMark("zhiman") > 0)
            {
                Player masu = room.FindPlayer((string)player.GetTag("zhiman_from"));
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
                    Player victim = room.AskForPlayerChosen(player, targets, Name, "@xuanhuo-victim:" + target.Name, false, false, info.SkillPosition);
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
                if (room.AskForSkillInvoke(player, "enyuan_jx_en", move.From, info.SkillPosition))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, player.Name, move.From.Name);
                    invoke = true;
                }
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From.Alive)
            {
                if (room.AskForSkillInvoke(player, "enyuan_jx_yuan", damage.From, info.SkillPosition))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.DoAnimate(CommonClassLibrary.AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                    invoke = true;
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
                    From = player.Name,
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
                    prompts.Add(string.Format("@fumian-draw:::", 2));
                }
                else
                {
                    prompts.Add(string.Format("@fumian-draw:::", 1));
                }
                if (mark == 0 && player.GetMark("TurnPlay") > 0 || mark == 1)
                {
                    prompts.Add(string.Format("@fumian-target:::", 2));
                }
                else
                {
                    prompts.Add(string.Format("@fumian-target:::", 1));
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
                    if (!use.To.Contains(p) && RoomLogic.IsProhibited(room, use.From, p, use.Card, use.To) == null)
                        targets.Add(p);

                if (fcard is Slash)
                    targets.Remove(player);

                if (targets.Count > 0)
                {
                    int count = player.GetMark(Name) == 4 ? 2 : 1;
                    List<Player> players = room.AskForPlayersChosen(player, targets, Name, 0, count, string.Format("@fumian-target:::{0}:{1}", use.Card.Name, count), true, info.SkillPosition);
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
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Start)
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
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
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
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start)
            {
                List<Player> zhangsong = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in zhangsong)
                    triggerStructs.Add(new TriggerStruct(Name, p));
            }
            return triggerStructs;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
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
            else if (triggerEvent == TriggerEvent.PreDamageDone && base.Triggerable(player, room))
            {
                Player current = room.Current;
                if (current == null || !current.Alive || current.Phase == PlayerPhase.NotActive)
                    return;

                player.AddMark(Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room))
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
                int id = room.AskForCardChosen(ask_who, player, "e", Name, false, FunctionCard.HandlingMethod.MethodNone, disable_equiplist);
                room.MoveCardTo(room.GetCard(id), ask_who, Place.PlaceEquip);
            }

            return false;
        }
    }
    
    public class XianzhouCard : SkillCard
    {
        public static string ClassName = "XianzhouCard";
        public XianzhouCard() : base(XianzhouCard.ClassName)
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

            if (room.AskForChoice(target, "xianzou", string.Join("+", choices), prompts) == "damage")
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
            lord_skill = true;
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
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
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
                    AskForMoveCardsStruct ly = room.AskForMoveCards(ask_who, ids, new List<int>(), false, Name, ids.Count, ids.Count, true, true, new List<int>(), info.SkillPosition);
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
                WrappedCard wrapped = new WrappedCard(card);
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
            room.MoveCardTo(use.Card, null, Place.DrawPile, true);
            room.BroadcastSkillInvoke("huomo", player, use.Card.SkillPosition);
            room.NotifySkillInvoked(player, "huomo");
            WrappedCard wrapped = new WrappedCard(use.Card.UserString);
            return wrapped;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            room.MoveCardTo(card, null, Place.DrawPile, true);
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
            room.ObtainCard(target, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "mingjian", string.Empty));
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
            if (triggerEvent == TriggerEvent.AskForPeachesDone)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasFlag(Name))
                        room.Damage(new DamageStruct(Name, null, p));
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.AskForPeaches && base.Triggerable(target, room) && target.GetMark("@xingshuai") > 0 && data is DyingStruct dying_data)
            {
                if (target.Hp > 0)
                    return new TriggerStruct();

                if (dying_data.Who != target)
                    return new TriggerStruct();

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
                if (p.Kingdom == "wei" && room.AskForSkillInvoke(player, Name, "@xingshuai-src:" + player.Name))
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
            if (base.Triggerable(player, room) && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || (fcard is TrickCard && !(fcard is DelayedTrick)))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && room.AskForSkillInvoke(player, Name, string.Format("@zhenlie-from:{0}::{1}", use.From, use.Card.Name), info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.LoseHp(player);
                return info;
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
                if (use.NullifiedList != null)
                    use.NullifiedList.Add(player.Name);
                else
                    use.NullifiedList = new List<string> { player.Name };
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
            if (data is DyingStruct dying && !player.IsKongcheng())
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
            return base.Triggerable(player, room) ? new TriggerStruct(Name, player) : new TriggerStruct();
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
            int count = 0, hp = 0, eq = 0 ;
            foreach (Player p in room.GetAllPlayers())
            {
                if (p.HandcardNum > count)
                    count = p.HandcardNum;
                if (p.Hp > hp)
                    hp = p.Hp;
                if (p.GetEquips().Count > eq)
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
                    if (fcard.IsAvailable(room, target, card))
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
                    if (fcard.IsAvailable(room, player, card))
                    {
                        room.UseCard(new CardUseStruct(card, player, new List<Player>(), false));
                        break;
                    }
                }
            }
        }
    }
}
