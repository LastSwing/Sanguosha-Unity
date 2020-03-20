using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class Authority : GeneralPackage
    {
        public Authority() : base("Authority")
        {
            skills = new List<Skill>
            {
                //ImperialOrder
                new SurrenderVS(),
                new SurrenderPorhibit(),
                new StopFightingInvalid(),
                new ImperialOrderControll(),
                new ImperialOrderVS(),

                new Jieyue(),
                new JieyueDraw(),
                new Zhengpi(),
                new ZhengpiTar(),
                new Fengying(),
                new Fudi(),
                new Congjian(),
                new Weidi(),
                new Yongsi(),
                new YongsiVH(),
                new Enyuan(),
                new Xuanhuo(),
                new XuanhuoVS(),
                new XuanhuoClear(),

                new WushengFZ(),
                new PaoxiaoFZ(),
                new PaoxiaoTMFZ(),
                new LongdanFZ(),
                new LiegongFZ(),
                new LiegongFZRange(),
                new TieqiFZ(),
                new KuangguFZ(),

                new Jianglue(),
                new Buyi(),
                new Ganlu(),
                new Zhuwei(),
                new ZhuweiMax(),
                new ZhuweiTar(),
                new Keshou(),

                new Huibian(),
                new Zongyu(),
                new Jianan(),
                new JieyueCC(),
                new TuxiCC(),
                new QiaobianCC(),
                new XiaoguoCC(),
                new DuanliangCC(),
                new DuanliangCCTargetMod(),
            };
            skill_cards = new List<FunctionCard>
            {
                //ImperialOrder
                new BeltsheChao(),
                new Surrender(),
                new SurrenderCard(),
                new PayTribute(),
                new StopFighting(),
                new ChangeGeneral(),

                new ZhengpiCard(),
                new JieyueCard(),
                new FengyingCard(),
                new WeidiCard(),
                new XuanhuoCard(),
                new JianglueCard(),

                new GanluCard(),
                new KeshouCard(),
                new HuibianCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "zhengpi", new List<string> { "#zhengpi-target" } },
                { "jieyue", new List<string> { "#jieyue-draw" } },
                { "jieyue_cc", new List<string> { "#jieyue-draw" } },
                { "paoxiao_fz", new List<string> { "#paoxiao_fz-tm" } },
                { "liegong_fz", new List<string> { "#liegong_fz-for-lord" } },
                { "yongsi", new List<string> { "#yongshi-viewhas" } },
                { "zhuwei", new List<string> { "#zhuwei-max", "#zhuwei-tar" } },
            };
        }
    }

    public abstract class ImperialOrder : FunctionCard
    {
        public override string GetSubtype() => "imperial_order";
        public ImperialOrder(string name) : base(name)
        {
            target_fixed = true;
            type_id = CardType.TypeSkill;
        }

        public abstract bool Effect(Room room, Player player, Player target);

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.RemoveTag("imperialorder_select");
            Player player = card_use.From;
            Player target = card_use.To.Count == 1 ? card_use.To[0] : null;

            LogMessage log = new LogMessage("$annouce_order")
            {
                From = player.Name,
                To = new List<string>(),
                Arg = card_use.Card.Name
            };
            
            if (target == null && !string.IsNullOrEmpty(card_use.Card.UserString))
                target = room.FindPlayer(card_use.Card.UserString);
            if (target != null)
            {
                card_use.To = new List<Player> { target };
                log.Type = "$order_to";
                log.SetTos(card_use.To);
            }

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;

            CardMoveReasonStruct reason = new CardMoveReasonStruct
            {
                Reason = MoveReason.S_REASON_ANNOUNCE,
                PlayerId = player.Name,
                TargetId = null,
                SkillName = card_use.Card.Skill,
                EventName = null,
                CardString = RoomLogic.CardToString(room, card_use.Card),
                General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
            };
            if (card_use.To.Count == 1)
                reason.TargetId = card_use.To[0].Name;

            //show virtual card on table
            ClientCardsMoveStruct move = new ClientCardsMoveStruct(-1, player, Place.PlaceTable, reason)
            {
                From_place = Place.PlaceUnknown,
                From = player.Name,
                Is_last_handcard = false,
            };
            room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, card_use.Card), move);
            room.SendLog(log);
            Thread.Sleep(1000);

            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;

            if (card_use.To.Count == 1)
            {
                bool success = Effect(room, player, card_use.To[0]);
                player.SetTag("ImperialOrder", success);
            }
        }
    }

    //衣带诏
    public class BeltsheChao : ImperialOrder
    {
        public static string ClassName = "BeltsheChao";
        public BeltsheChao() : base(ClassName) { }

        public override bool Effect(Room room, Player player, Player target)
        {
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
            room.SetTag(Name, player);
            string choice = room.AskForChoice(target, Name, "accept+cancel");
            room.RemoveTag(Name);
            if (choice == "accept")
            {
                room.DrawCards(target, new DrawCardStruct(1, player, "ImperialOrder"));
                List<Player> victims = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(target))
                    if (RoomLogic.InMyAttackRange(room, target, p) && RoomLogic.CanSlash(room, target, p))
                        victims.Add(p);

                bool success = false;
                if (victims.Count > 0)
                {
                    room.SetTag("belt_killer", target);
                    Player victim = room.AskForPlayerChosen(player, victims, Name, "@belt_she_chao:" + target.Name);
                    room.RemoveTag("belt_killer");

                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, victim.Name);
                    LogMessage log1 = new LogMessage
                    {
                        From = target.Name,
                        To = new List<string> { victim.Name },
                        Type = "$kill_victim"
                    };
                    room.SendLog(log1);

                    WrappedCard slash = room.AskForUseSlashTo(target, victim, "@kill_victim:" + victim.Name, null);
                    if (slash != null) success = true;
                }

                if (!success) room.LoseHp(target);
                return true;
            }
            else
                return false;
        }
    }

    //俯首称臣
    public class Surrender : ImperialOrder
    {
        public static string ClassName = "Surrender";
        public Surrender() : base(ClassName) { }

        public override bool Effect(Room room, Player player, Player target)
        {
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);

            List<int> throw_ids = new List<int>();
            bool show = false;
            if (target.HandcardNum > 0 || target.GetCards("he").Count > 1)
            {
                int throw_hand = 0;
                bool can_throw = false;
                foreach (int id in target.GetEquips())
                    if (RoomLogic.CanDiscard(room, target, target, id))
                        throw_ids.Add(id);

                if (throw_ids.Count > 0 && throw_ids.Count == target.GetEquips().Count)
                    throw_ids.RemoveAt(0);

                foreach (int id in target.GetCards("h"))
                {
                    if (RoomLogic.CanDiscard(room, target, target, id))
                    {
                        throw_ids.Add(id);
                        throw_hand++;
                    }
                }
                throw_ids.AddRange(target.JudgingArea);

                if (throw_ids.Count > 0) can_throw = true;

                if (can_throw)
                {
                    room.SetTag(Name, player);
                    if (RoomLogic.CanDiscard(room, target, target, "e") && target.GetEquips().Count > 1)
                    {
                        WrappedCard card = room.AskForUseCard(target, "@@surrender", "@surrender", null, -1, HandlingMethod.MethodUse);
                        if (card != null)
                        {
                            throw_ids.Clear();
                            foreach (int id in target.GetCards("he"))
                            {
                                if (card.SubCards.Contains(id)) continue;
                                if (RoomLogic.CanDiscard(room, target, target, id))
                                    throw_ids.Add(id);
                                else if (room.GetCardPlace(id) == Place.PlaceHand)
                                    show = true;
                            }
                        }
                        else
                            throw_ids.Clear();
                    }
                    else
                    {
                        if (room.AskForChoice(target, "surrender", "throw+cancel") == "cancel")
                            throw_ids.Clear();
                        else if (throw_hand < target.HandcardNum)
                            show = true;
                    }
                    room.RemoveTag(Name);
                }
            }

            if (throw_ids.Count > 0)
            {
                LogMessage log = new LogMessage
                {
                    Type = "$do_surrender",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);

                if (show) room.ShowAllCards(target);
                room.ThrowCard(ref throw_ids, new CardMoveReason(MoveReason.S_REASON_THROW, target.Name, target.Name, "surrender", "surrender"), target);

                player.SetFlags("majestic-" + target.Name);
                target.SetFlags("surrender");
                if (throw_ids.Count > 1)
                {
                    target.SetFlags("surrender_draw");
                    room.SetPlayerStringMark(target, "surrender", "drawable");
                }
                else
                    room.SetPlayerStringMark(target, "surrender", "no draw");

                return true;
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "$refuse_surrender",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);

                return false;
            }
        }
    }

    public class SurrenderPorhibit : ProhibitSkill
    {
        public SurrenderPorhibit() : base("surrender-prohibit")
        {}

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && to != null && from.HasFlag("majestic-" + to.Name) && to.HasFlag("surrender"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                return fcard != null && !(fcard is SkillCard);
            }

            return false;
        }
    }

    public class SurrenderVS : OneCardViewAsSkill
    {
        public SurrenderVS() : base("surrender") {}

        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@surrender";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            bool cant_discard_equip = false;
            foreach (int id in player.GetEquips())
            {
                if (!RoomLogic.CanDiscard(room, player, player, id))
                {
                    cant_discard_equip = true;
                    break;
                }
            }

            return selected.Count == 0 && room.GetCardPlace(to_select.Id) == Place.PlaceEquip
                && (!cant_discard_equip || !RoomLogic.CanDiscard(room, player, player, to_select.Id));
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard surrend = new WrappedCard(SurrenderCard.ClassName);
            surrend.AddSubCard(card);
            return surrend;
        }
    }

    public class SurrenderCard : SkillCard
    {
        public static string ClassName = "SurrenderCard";
        public SurrenderCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            //do nothing
        }
    }

    //朝贡
    public class PayTribute : ImperialOrder
    {
        public static string ClassName = "PayTribute";
        public PayTribute() : base(ClassName) { }

        public override bool Effect(Room room, Player player, Player target)
        {
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
            List<int> ids = new List<int>();
            room.SetTag(Name, player);
            if (target.GetCards("he").Count > 1)
                ids = room.AskForExchange(target, Name, player.GetCards("he").Count, 0, "@paytribute:" + player.Name, string.Empty, "..", string.Empty);
            room.RemoveTag(Name);

            if (ids.Count > 0)
            {
                LogMessage log = new LogMessage
                {
                    Type = "$do_paytribute",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);

                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, "paytribute", "paytribute"), false);
                if (!player.IsKongcheng() && ids.Count > 1)
                {
                    ids = room.AskForExchange(player, "paytribute-back", 1, 1, "@paytribute-giveback:" + target.Name, string.Empty, ".", string.Empty);
                    if (ids.Count != 1)
                        ids = new List<int> { player.GetCards("h")[0] };

                    room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "paytribute", "paytribute"), false);
                }

                return true;
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "$refuse_paytribute",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);
                return false;
            }
        }
    }

    //止戈息兵
    public class StopFighting : ImperialOrder
    {
        public static string ClassName = "StopFighting";
        public StopFighting() : base(ClassName) { }

        public override bool Effect(Room room, Player player, Player target)
        {
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
            room.SetTag(Name, player);
            string choice = room.AskForChoice(target, Name, "skillinvalid+cancel");
            room.RemoveTag(Name);
            if (choice == "cancel")
            {
                LogMessage log = new LogMessage
                {
                    Type = "$refuse_stopfighting",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);
                return false;
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "$do_stopfighting",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);

                target.SetMark("stopfighting", 1);
                room.SetPlayerStringMark(target, "non_compulsory", "skill invalid");

                if (target.IsWounded())
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = player,
                        Recover = 1
                    };
                    room.Recover(target, recover, true);
                }

                return true;
            }
        }
    }
    //临阵换将
    public class ChangeGeneral : ImperialOrder
    {
        public static string ClassName = "ChangeGeneral";
        public ChangeGeneral() : base(ClassName)
        {
        }

        public override bool Effect(Room room, Player player, Player target)
        {
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name); room.SetTag(Name, player);
            string choice = room.AskForChoice(target, Name, "accept+cancel");
            room.RemoveTag(Name);
            if (choice == "cancel")
            {
                LogMessage log = new LogMessage
                {
                    Type = "$refuse_change_general",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);
                return false;
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "$do_change_general",
                    From = player.Name,
                    To = new List<string> { target.Name }
                };
                room.SendLog(log);

                if (RoomLogic.CanTransform(target))
                    room.TransformDeputyGeneral(target, 2);

                string pattern = ".|.|.|hand$0";
                target.SetFlags("ChangeGeneralTarget");
                RoomLogic.SetPlayerCardLimitation(target, ClassName, "use,response", pattern);
                room.SetPlayerStringMark(target, "no_handcards", string.Empty);

                return true;
            }
        }
    }

    public class ImperialOrderControll : TriggerSkill
    {
        public ImperialOrderControll() : base("imperial-order-controll")
        {
            global = true;
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HasFlag("surrender"))
                    {
                        p.SetFlags("-surrender");
                        room.RemovePlayerStringMark(p, "surrender");
                        if (p.HasFlag("surrender_draw") && p.HandcardNum < player.MaxHp)
                        {
                            p.SetFlags("-surrender_draw");
                            room.DrawCards(p, p.MaxHp - p.HandcardNum, "surrender");
                        }
                    }

                    if (p.HasFlag("ChangeGeneralTarget"))
                    {
                        RoomLogic.RemovePlayerCardLimitation(p, ChangeGeneral.ClassName);
                        room.RemovePlayerStringMark(p, "no_handcards");
                    }
                }

                if (player.GetMark("stopfighting") > 0)
                {
                    player.SetMark("stopfighting", 0);
                    room.RemovePlayerStringMark(player, "non_compulsory");
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class StopFightingInvalid : InvalidSkill
    {
        public StopFightingInvalid() : base("stopfighting-invalid") { }

        public override bool Invalid(Room room, Player player, string skill)
        {
            Skill s = Engine.GetMainSkill(skill);
            if (s == null || s.Attached_lord_skill || player.HasEquip(skill)) return false;
            if (player.GetMark("stopfighting") > 0)
            {
                return s != null && s.SkillFrequency != Frequency.Compulsory;
            }

            return false;
        }
    }

    public class ImperialOrderVS : ViewAsSkill
    {
        public ImperialOrderVS() : base("imperialorder")
        {
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern.Contains("@@imperialorder");
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }
        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
                return GetImperialOrders(room, player);

            return new List<WrappedCard>();
        }

        public static List<WrappedCard> GetImperialOrders(Room room, Player player)
        {
            List<string> card_names = new List<string>()
            {
                BeltsheChao.ClassName,
                Surrender.ClassName,
                PayTribute.ClassName,
                StopFighting.ClassName,
                ChangeGeneral.ClassName
            };

            CommonClass.Shuffle.shuffle(ref card_names);

            Player target = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HasFlag("imperialorder_target"))
                {
                    target = p;
                    p.SetFlags("-imperialorder_target");
                    break;
                }
            }

            List<WrappedCard> result = new List<WrappedCard>();
            for (int i = 0; i < 2; i++)
            {
                WrappedCard card = new WrappedCard(card_names[i])
                {
                    UserString = target != null ? target.Name : string.Empty
                };
                result.Add(card);
            }

            player.SetTag("imperialorder_select", string.Format("{0}+{1}", result[0].Name, result[1].Name));
            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1)
            {
                FunctionCard fcard = Engine.GetFunctionCard(cards[0].Name);
                if (fcard is ImperialOrder)
                    return cards[0];
            }

            return null;
        }
    }

    //yujin
    public class Jieyue : PhaseChangeSkill
    {
        public Jieyue() : base("jieyue")
        {
            view_as_skill = new JieyueVS();
            frequency = Frequency.Frequent;
            skill_type = SkillType.Wizzard;
        }
        public override bool CanPreShow() => false;
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && !target.IsKongcheng() && target.Phase == PlayerPhase.Start;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard card = room.AskForUseCard(player, "@@jieyue", "@jieyue", null, -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null)
            {
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag(Name));
            if (target != null)
            {
                target.SetFlags("imperialorder_target");        //这个flag表明该玩家为敕令的固定目标，且会自动在ViewAsSkill生成随机敕令牌后清除
                player.SetTag("order_reason", Name);
                WrappedCard card = room.AskForUseCard(player, "@@imperialorder!", "@jieyue-target:" + target.Name, null, - 1, FunctionCard.HandlingMethod.MethodUse);
                if (card == null)
                {
                    string card_name = player.ContainsTag("imperialorder_select") ? ((string)player.GetTag("imperialorder_select")).Split('+')[0] : string.Empty;
                    if (string.IsNullOrEmpty(card_name))
                        card = ImperialOrderVS.GetImperialOrders(room, player)[0];
                    else
                        card = new WrappedCard(card_name);

                    CardUseStruct use = new CardUseStruct(card, player, target);
                    room.UseCard(use);
                }
                player.RemoveTag("imperialorder_select");

                if (!player.ContainsTag("ImperialOrder") || !(bool)player.GetTag("ImperialOrder"))
                    player.SetFlags("jieyue_draw");
                else
                    room.DrawCards(player, 1, Name);
            }

            return false;
        }
    }

    public class JieyueDraw : TriggerSkill
    {
        public JieyueDraw() : base("#jieyue-draw")
        {
            events.Add(TriggerEvent.EventPhaseProceeding);
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == PlayerPhase.Draw && (int)data >= 0 && player.HasFlag("jieyue_draw"))
            {
                LogMessage log = new LogMessage
                {
                    Type = "$jieyue-draw",
                    From = player.Name,
                };
                room.SendLog(log);

                player.SetFlags("-jieyue_draw");
                int count = (int)data;
                count += 3;
                data = count;
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class JieyueVS : OneCardViewAsSkill
    {
        public JieyueVS() : base("jieyue") { filter_pattern = "."; }

        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return pattern == "@@jieyue" && reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard jieyue = new WrappedCard(JieyueCard.ClassName);
            jieyue.AddSubCard(card);
            return jieyue;
        }
    }

    public class JieyueCard : SkillCard
    {
        public static string ClassName = "JieyueCard";
        public JieyueCard() : base(ClassName) {}
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return to_select != Self && targets.Count == 0 && (!to_select.HasShownOneGeneral() || to_select.Kingdom != "wei");
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            player.SetTag("jieyue", target.Name);
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
            LogMessage log = new LogMessage
            {
                Type = "#ChoosePlayerWithSkill",
                From = player.Name,
                To = new List<string> { target.Name },
                Arg = "jieyue"
            };
            room.SendLog(log);

            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "jieyue", string.Empty), false);
        }
    }

    //cuimao
    public class Zhengpi : TriggerSkill
    {
        public Zhengpi() : base("zhengpi")
        {
            view_as_skill = new ZhengpiVS();
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
        }

        public override bool CanPreShow() => true;
        public override bool Triggerable(Player target, Room room)
        {
            return target.Phase == PlayerPhase.Play && base.Triggerable(target, room);
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard card = room.AskForUseCard(player, "@@zhengpi", "@zhengpi-target", null, -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
            if (card != null)
            {
                if (card.SubCards.Count == 1 && room.ContainsTag(Name) && room.GetTag(Name) is Player target)
                {
                    List<int> ids = new List<int>(card.SubCards);
                    room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty));
                }

                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.ContainsTag(Name) ? (Player)room.GetTag(Name) : null;
            room.RemoveTag(Name);
            if (target != null)
            {
                if (target.HasFlag(Name))
                {
                    List<int> ids = new List<int>();
                    foreach (int id in target.GetCards("he"))
                    {
                        if (!(Engine.GetFunctionCard(room.GetCard(id).Name) is BasicCard))
                        {
                            ids.Add(id);
                            break;
                        }
                    }
                    if (ids.Count == 0)
                    {
                        foreach (int id in target.GetCards("h"))
                        {
                            ids.Add(id);
                            if (ids.Count >= 2)
                                break;
                        }
                    }
                    if (ids.Count > 0 && ids.Count < player.GetCards("he").Count)
                    {
                        WrappedCard card = room.AskForUseCard(target, "@@zhengpi!", "@zhengpi-give", null);
                        if (card != null)
                            ids = new List<int>(card.SubCards);
                    }
                    target.SetFlags("-zhengpi");

                    if (ids.Count > 0)
                        room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, string.Empty));
                }
                else if (!target.HasShownOneGeneral())
                {
                    LogMessage log = new LogMessage("$zhengpi")
                    {
                        From = player.Name,
                        To = new List<string> { target.Name }
                    };
                    room.SendLog(log);

                    player.SetFlags("zhengpi_" + target.Name);
                }
            }
            return false;
        }
    }

    public class ZhengpiVS : ViewAsSkill
    {
        public ZhengpiVS() : base("zhengpi") { }

        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern.Contains("@@zhengpi");
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (!player.HasFlag(Name))
                return selected.Count == 0 && Engine.GetFunctionCard(to_select.Name) is BasicCard;
            else
            {
                if (selected.Count > 1) return false;
                bool basic = true;
                foreach (WrappedCard card in selected)
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard))
                        basic = false;

                return basic && (selected.Count == 0 || Engine.GetFunctionCard(to_select.Name) is BasicCard);
            }
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            bool check = false;
            if (cards.Count <= 1 && !player.HasFlag(Name))
                check = true;
            else if (player.HasFlag(Name) && cards.Count > 0)
            {
                if (cards.Count == 1 && !(Engine.GetFunctionCard(cards[0].Name) is BasicCard))
                    check = true;
                else if (cards.Count == 2)
                {
                    check = true;
                    foreach (WrappedCard card in cards)
                        if (!(Engine.GetFunctionCard(card.Name) is BasicCard))
                            check = false;
                }
            }

            if (check)
            {
                WrappedCard card = new WrappedCard(ZhengpiCard.ClassName)
                {
                    Skill = Name
                };
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }

    public class ZhengpiCard : SkillCard
    {
        public static string ClassName = "ZhengpiCard";
        public ZhengpiCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (Self.HasFlag("zhengpi") || to_select == Self || targets.Count > 0)
                return false;
            else
                return to_select.HasShownOneGeneral() ? card.SubCards.Count == 1 : card.SubCards.Count == 0;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return (Self.HasFlag("zhengpi") && targets.Count == 0) || (!Self.HasFlag("zhengpi") && targets.Count == 1);
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            if (card_use.To.Count == 1)
            {
                LogMessage log = new LogMessage("#UseCard")
                {
                    From = card_use.From.Name,
                    To = new List<string>(),
                    Card_str = RoomLogic.CardToString(room, card_use.Card)
                };
                log.SetTos(card_use.To);
                room.SendLog(log);

                room.SetTag("zhengpi", card_use.To[0]);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, card_use.From.Name, card_use.To[0].Name);

                if (card_use.To[0].HasShownOneGeneral())
                    card_use.To[0].SetFlags("zhengpi");
            }
        }
    }

    public class ZhengpiTar : TargetModSkill
    {
        public ZhengpiTar() : base("#zhengpi-target", false)
        {
            pattern = ".";
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
        {
            if (to != null && !to.HasShownOneGeneral() && from.HasFlag("zhengpi_" + to.Name))
                return true;

            return false;
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (to != null && !to.HasShownOneGeneral() && from.HasFlag("zhengpi_" + to.Name))
                return true;

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
                index = -2;
        }
    }

    public class Fengying : ViewAsSkill
    {
        public Fengying() : base("fengying")
        {
            frequency = Frequency.Limited;
            limit_mark = "@lord";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodUse))
                    return false;
            }

            return player.GetMark("@lord") > 0 && !player.IsKongcheng();
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
            {
                WrappedCard card = new WrappedCard(FengyingCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name,
                };

                return card;
            }

            return null;
        }
    }

    public class FengyingCard : SkillCard
    {
        public static string ClassName = "FengyingCard";
        public FengyingCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@lord", 0);
            room.BroadcastSkillInvoke("fengying", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "fengying");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            WrappedCard card = new WrappedCard(ThreatenEmperor.ClassName)
            {
                Skill = "_fengying"
            };
            card.AddSubCards(player.GetCards("h"));

            CardUseStruct use = new CardUseStruct(card, player, new List<Player>());
            room.UseCard(use);
            
            List<Player> drawers = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (RoomLogic.IsFriendWith(room, p, player))
                    drawers.Add(p);

            if (drawers.Count > 0)
            {
                room.SortByActionOrder(ref drawers);
                foreach (Player p in drawers)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                    int i = p.MaxHp - p.HandcardNum;
                    if (i > 0)
                        room.DrawCards(p, new DrawCardStruct(i, player, "fengying"));
                }
            }
        }
    }

    //zhangxiu
    public class Fudi : MasochismSkill
    {
        public Fudi() : base("fudi")
        {
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && !player.IsKongcheng() && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (!player.IsKongcheng() && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                room.SetTag(Name, data);
                List<int> ids = room.AskForExchange(player, Name, 1, 0, "@fudi::" + damage.From.Name, string.Empty, ".", info.SkillPosition);
                room.RemoveTag(Name);
                if (ids.Count == 1)
                {
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, damage.From.Name, Name, null);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.NotifySkillInvoked(player, Name);
                    room.ObtainCard(damage.From, ref ids, reason);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override void OnDamaged(Room room, Player player, DamageStruct damage, TriggerStruct info)
        {
            Player from = damage.From;
            List<Player> targets = new List<Player>();
            int max = player.Hp;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.IsFriendWith(room, p, from) && p.Hp >= max)
                {
                    max = p.Hp;
                    targets.Add(p);
                }
            }

            targets.RemoveAll(t => (t.Hp < max));

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@fudi-target", false, false, info.SkillPosition);
                if (target != null)
                    room.Damage(new DamageStruct(Name, player, target));
            }
        }
    }

    public class Congjian : TriggerSkill
    {
        public Congjian() : base("congjian")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.DamageInflicted };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
            {
                if ((triggerEvent == TriggerEvent.DamageCaused && player.Phase == PlayerPhase.NotActive)
                    || (triggerEvent == TriggerEvent.DamageInflicted && room.Current == player))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", triggerEvent == TriggerEvent.DamageCaused ? 1 : 2, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            DamageStruct damage = (DamageStruct)data;
            if (triggerEvent == TriggerEvent.DamageCaused)
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

                data = damage;
            }

            return false;
        }
    }

    //yuanshu
    public class WeidiVS : ViewAsSkill
    {
        public WeidiVS() : base("weidi") {}

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.HasUsed(WeidiCard.ClassName))
            {
                bool check = false;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.HasFlag("weidi_get"))
                    {
                        check = true;
                        break;
                    }
                }
                return check;
            }

            return false;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return pattern == "@@weidi!";
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return room.GetRoomState().GetCurrentCardUsePattern() == "@@weidi!" && selected.Count == 0;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            if (room.GetRoomState().GetCurrentCardUsePattern() == "@@weidi!" && cards.Count == 0)
                return ImperialOrderVS.GetImperialOrders(room, player);

            return new List<WrappedCard>();
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (room.GetRoomState().GetCurrentCardUsePattern() == "@@weidi!" && cards.Count == 1)
            {
                WrappedCard card = new WrappedCard(WeidiCard.ClassName)
                {
                    UserString = cards[0].Name
                };
                return card;
            }
            else if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY && cards.Count == 0)
            {
                WrappedCard card = new WrappedCard(WeidiCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                return card;
            }

            return null;
        }
    }

    public class Weidi : TriggerSkill
    {
        public Weidi() : base("weidi")
        {
            view_as_skill = new WeidiVS();
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null
                && move.From_places.Contains(Place.DrawPile) && move.To_place == Place.PlaceHand && !move.To.HasFlag("weidi_get") && move.To.Phase == PlayerPhase.NotActive)
                move.To.SetFlags("weidi_get");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class WeidiCard : SkillCard
    {
        public static string ClassName = "WeidiCard";
        public WeidiCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUsePattern() == "@@weidi!" || to_select == Self)
                return false;
            else
                return to_select.HasFlag("weidi_get") && targets.Count == 0;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUsePattern() == "@@weidi!")
                return targets.Count == 0;
            else
                return targets.Count == 1;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            if (!string.IsNullOrEmpty(card_use.Card.UserString) && card_use.To.Count == 0)
            {
                Player player = card_use.From;
                Player target = room.FindPlayer((string)player.GetTag("weidi"));
                FunctionCard fcard = Engine.GetFunctionCard(card_use.Card.UserString);
                if (fcard != null && fcard is ImperialOrder)
                {
                    player.SetTag("order_reason", "weidi");
                    WrappedCard card = new WrappedCard(fcard.Name);
                    room.UseCard(new CardUseStruct(card, player, target));

                    if (!player.ContainsTag("ImperialOrder") || !(bool)player.GetTag("ImperialOrder"))
                    {
                        if (RoomLogic.CanGetCard(room, player, target, "h"))
                        {
                            List<int> ids = target.GetCards("h");
                            room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, "weidi", "weidi"), false);

                            ids = room.AskForExchange(player, "weidi", ids.Count, ids.Count, 
                                string.Format("@weidi-back:{0}::{1}", target.Name, ids.Count), string.Empty, "..", card_use.Card.SkillPosition);
                            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "weidi", "weidi"), false);
                        }
                        else
                            room.ShowAllCards(target);
                    }
                }
            }
            else
                base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            player.SetTag("weidi", target.Name);
            WrappedCard card = room.AskForUseCard(player, "@@weidi!", "@weidi:" + target.Name, null, -1, HandlingMethod.MethodNone, true, card_use.Card.SkillPosition);
            if (card == null)
            {
                string card_name = player.ContainsTag("imperialorder_select") ? ((string)player.GetTag("imperialorder_select")).Split('+')[0] : string.Empty;
                if (string.IsNullOrEmpty(card_name))
                    card_name = ImperialOrderVS.GetImperialOrders(room, player)[0].Name;

                card = new WrappedCard("WeidiCard")
                {
                    UserString = card_name
                };
                room.UseCard(new CardUseStruct(card, player, new List<Player>()));
            }
            player.RemoveTag("imperialorder_select");
        }
    }

    //fazheng
    public class Enyuan : TriggerSkill
    {
        public Enyuan() : base("enyuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.HpRecover, TriggerEvent.Damaged };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
            {
                if ((triggerEvent == TriggerEvent.HpRecover && data is RecoverStruct recover && recover.Who != null && recover.Who != player && recover.Who != player)
                || (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From != null && damage.From != player && damage.From.Alive))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = false;
            if (triggerEvent == TriggerEvent.HpRecover && data is RecoverStruct recover)
            {
                if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, "enyuan_en", recover.Who, info.SkillPosition))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, recover.Who.Name);
                    invoke = true;
                }
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From != null && damage.From != player && damage.From.Alive)
            {
                if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, "enyuan_yuan", damage.From, info.SkillPosition))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                    invoke = true;
                }
            }

            return invoke ? info : new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            if (triggerEvent == TriggerEvent.HpRecover && data is RecoverStruct recover)
            {
                LogMessage log = new LogMessage
                {
                    Type = "$enyuan_en",
                    From = player.Name,
                    To = new List<string> { recover.Who.Name }
                };
                room.SendLog(log);
                room.DrawCards(recover.Who, new DrawCardStruct(1, recover.Who, Name));
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From != null && damage.From != player && damage.From.Alive)
            {
                int result = -1;
                if (damage.From.HandcardNum > 0)
                {
                    List<int> ids = room.AskForExchange(damage.From, "enyuan_yuan", 1, 0, "@enyuan:" + player.Name, string.Empty, ".", string.Empty);
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
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, damage.From.Name, player.Name, Name), false);
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

    public class Yongsi : TriggerSkill
    {
        public Yongsi() : base("yongsi")
        {
            events.Add(TriggerEvent.TargetConfirming);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null && use.To.Contains(player) && use.Card.Name == KnownBoth.ClassName)
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
            room.SendCompulsoryTriggerLog(player, Name);
            room.ShowAllCards(player, null, Name);
            return false;
        }
    }

    public class YongsiVH : ViewHasSkill
    {
        public YongsiVH() : base("#yongshi-viewhas")
        {
            viewhas_treasure.Add(JadeSeal.ClassName);
        }

        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (skill_name == JadeSeal.ClassName && RoomLogic.PlayerHasShownSkill(room, player, "yongsi"))
            {
                bool in_game = false;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HasTreasure(JadeSeal.ClassName))
                    {
                        in_game = true;
                        break;
                    }
                }

                return !in_game;
            }

            return false;
        }
    }


    //fazheng
    public class Xuanhuo : TriggerSkill
    {
        public Xuanhuo() : base("xuanhuo")
        {
            events = new List<TriggerEvent> { TriggerEvent.GeneralShown, TriggerEvent.Death, TriggerEvent.DFDebut, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player == null) return;

            if (triggerEvent == TriggerEvent.GeneralShown)
            {
                if (base.Triggerable(player, room))
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (RoomLogic.IsFriendWith(room, p, player))
                            room.AttachSkillToPlayer(p, "xuanhuovs");
                }
                else if (!RoomLogic.PlayerHasSkill(room, player, "xuanhuovs"))
                {
                    List<Player> fazhengs = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player p in fazhengs)
                    {
                        if (RoomLogic.IsFriendWith(room, player, p))
                        {
                            room.AttachSkillToPlayer(player, "xuanhuovs");
                            break;
                        }
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Who == player)
            {
                if (RoomLogic.PlayerHasSkill(room, player, Name))
                {
                    foreach (Player p in room.GetAlivePlayers())
                        room.DetachSkillFromPlayer(p, "xuanhuovs", false, true);
                }
                else if (!player.General1.Contains("sujiang") && Engine.GetGeneral(player.ActualGeneral1, room.Setting.GameMode).IsLord())
                {
                    foreach (Player p in room.GetAlivePlayers())
                        if (RoomLogic.IsFriendWith(room, player, p))
                            room.DetachSkillFromPlayer(p, "xuanhuovs", false, true);
                }
            }
            else if (triggerEvent == TriggerEvent.DFDebut)
            {
                Player fz = RoomLogic.FindPlayerBySkillName(room, Name);
                if (fz != null && RoomLogic.PlayerHasShownSkill(room, fz, Name) && RoomLogic.IsFriendWith(room, player, fz) && !player.GetAcquiredSkills().Contains("xuanhuovs"))
                    room.AttachSkillToPlayer(player, "xuanhuovs");
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                room.HandleAcquireDetachSkills(player, new List<string> {
                    "-wusheng_fz", "-paoxiao_fz", "-tieqi_fz", "-longdan_fz", "-liegong_fz", "-kuanggu_fz"
                }, true);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class XuanhuoClear : DetachEffectSkill
    {
        public XuanhuoClear() : base("xuanhuo", string.Empty)
        {
        }
        public override void OnSkillDetached(Room room, Player player, object data)
        {
            foreach (Player p in room.GetAlivePlayers())
                room.DetachSkillFromPlayer(p, "xuanhuovs", false, true);
        }
    }

    public class XuanhuoCard : SkillCard
    {
        public static string ClassName = "XuanhuoCard";
        public XuanhuoCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void OnCardAnnounce(Room room, CardUseStruct card_use, bool ignore_rule)
        {
            List<Player> fazhengs = RoomLogic.FindPlayersBySkillName(room, "xuanhuo");
            Player target = null;
            foreach (Player p in fazhengs)
            {
                if (RoomLogic.PlayerHasShownSkill(room, p, "xuanhuo") && RoomLogic.IsFriendWith(room, card_use.From, p))
                {
                    target = p;
                    break;
                }
            }
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.FillAG("xuanhuovs", ids, card_use.From, null, null, "@xuanhuo-give:" + target.Name);
            int id = room.AskForAG(card_use.From, ids, false, "xuanhuo");
            room.ClearAG(card_use.From);
            List<int> card_ids = new List<int> { id };
            room.ObtainCard(target, ref card_ids, new CardMoveReason(MoveReason.S_REASON_GIVE, card_use.From.Name, target.Name, "xuanhuo"), false);

            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, target, "xuanhuo");
            room.BroadcastSkillInvoke("xuanhuo", "male", -1, gsk.General, gsk.SkinId);

            ids.Remove(id);
            room.ThrowCard(ref ids, new CardMoveReason(MoveReason.S_REASON_THROW, card_use.From.Name), card_use.From);

            List<string> skills = new List<string> { "wusheng_fz", "paoxiao_fz", "tieqi_fz", "longdan_fz", "liegong_fz", "kuanggu_fz" };
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.PlayerHasShownSkill(room, p, "wusheng|wusheng_fz"))
                    skills.Remove("wusheng_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "paoxiao|paoxiao_fz"))
                    skills.Remove("paoxiao_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "tieqi|tieqi_fz"))
                    skills.Remove("tieqi_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "longdan|longdan_fz"))
                    skills.Remove("longdan_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "liegong|liegong_fz"))
                    skills.Remove("liegong_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "kuanggu|kuanggu_fz"))
                    skills.Remove("kuanggu_fz");
            }

            if (skills.Count > 0)
            {
                ResultStruct result = target.Result;
                result.Assist++;
                target.Result = result;

                string choice = room.AskForChoice(card_use.From, "xuanhuo", string.Join("+", skills));
                room.HandleAcquireDetachSkills(card_use.From, choice);
                card_use.From.SetFlags("xuanhuo");
            }
        }
    }

    public class XuanhuoVS : ViewAsSkill
    {
        public XuanhuoVS() : base("xuanhuovs")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.HasUsed("XuanhuoCard") && player.HandcardNum >= 2)
            {
                List<Player> fazhengs = RoomLogic.FindPlayersBySkillName(room, "xuanhuo");
                foreach (Player p in fazhengs)
                    if (RoomLogic.PlayerHasShownSkill(room, p, "xuanhuo") && RoomLogic.IsFriendWith(room, player, p))
                        return true;
            }

            return false;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < 2 && room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 2)
            {
                WrappedCard card = new WrappedCard(XuanhuoCard.ClassName);
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }

    public class WushengFZ : OneCardViewAsSkill
    {
        public WushengFZ() : base("wusheng_fz")
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
            return Engine.GetPattern(pattern).GetPatternString() == Slash.ClassName;
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
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            slash.AddSubCard(card);
            slash.Skill = Name;
            slash.ShowSkill = Name;
            return slash;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
            Player fz = RoomLogic.FindPlayerBySkillName(room, "xuanhuo");
            if (fz != null && fz.Alive && RoomLogic.PlayerHasShownSkill(room, fz, "xuanhuo"))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, fz, "xuanhuo");
                room.BroadcastSkillInvoke(skill_name, "male", -1, gsk.General, gsk.SkinId);
            }

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

    public class PaoxiaoTMFZ : TargetModSkill
    {
        public PaoxiaoTMFZ() : base("#paoxiao_fz-tm")
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "paoxiao_fz"))
                return 1000;
            else
                return 0;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
            Player fz = RoomLogic.FindPlayerBySkillName(room, "xuanhuo");
            if (fz != null && fz.Alive && RoomLogic.PlayerHasShownSkill(room, fz, "xuanhuo"))
            {
                index = -1;
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, fz, "xuanhuo");
                skill_name = "paoxiao_fz";
                general_name = gsk.General;
                skin_id = gsk.SkinId;
            }
        }
    }

    public class PaoxiaoFZ : TriggerSkill
    {
        public PaoxiaoFZ() : base("paoxiao_fz")
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
    public class LongdanFZVS : OneCardViewAsSkill
    {
        public LongdanFZVS() : base("longdan_fz")
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
                    if (pattern == Slash.ClassName)
                        return card.Name == Jink.ClassName;
                    else if (pattern == Jink.ClassName)
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
            return pattern == Jink.ClassName || pattern == Slash.ClassName;
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

    public class LongdanFZ : TriggerSkill
    {
        public LongdanFZ() : base("longdan_fz")
        {
            view_as_skill = new LongdanFZVS();
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
                room.DrawCards(player, new DrawCardStruct(1, lord, "shouyue"));
            }
            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
            Player fz = RoomLogic.FindPlayerBySkillName(room, "xuanhuo");
            if (fz != null && fz.Alive && RoomLogic.PlayerHasShownSkill(room, fz, "xuanhuo"))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, fz, "xuanhuo");
                room.BroadcastSkillInvoke(card.Skill, "male", -1, gsk.General, gsk.SkinId);
            }
        }
    }

    public class TieqiFZ : TriggerSkill
    {
        public TieqiFZ() : base("tieqi_fz")
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
                Player fz = RoomLogic.FindPlayerBySkillName(room, "xuanhuo");
                if (fz != null && fz.Alive && RoomLogic.PlayerHasShownSkill(room, fz, "xuanhuo"))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, fz, "xuanhuo");
                    room.BroadcastSkillInvoke(Name, "male", -1, gsk.General, gsk.SkinId);
                }
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
            int index = use.To.IndexOf(target);

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
                string general = room.AskForGeneral(source, choices, null, true, "tieqi", target, false, true);
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

            if (room.AskForCard(target, "tieqi",
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
                
                CardBasicEffect effect = use.EffectCount[index];
                effect.Effect2 = 0;

                Thread.Sleep(500);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#tieqidis",
                    From = target.Name,
                    Arg = "tieqi",
                    Arg2 = suit
                };
                room.SendLog(log);
            }
        }
    }

    public class LiegongFZ : TriggerSkill
    {
        public LiegongFZ() : base("liegong_fz")
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
                    foreach (Player to in use.To)
                    {
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
                Player fz = RoomLogic.FindPlayerBySkillName(room, "xuanhuo");
                if (fz != null && fz.Alive && RoomLogic.PlayerHasShownSkill(room, fz, "xuanhuo"))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, fz, "xuanhuo");
                    room.BroadcastSkillInvoke(Name, "male", -1, gsk.General, gsk.SkinId);
                }
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

            int index = use.To.IndexOf(target);
            CardBasicEffect effect = use.EffectCount[index];
            effect.Effect2 = 0;
            data = use;

            return false;
        }
    }

    public class LiegongFZRange : AttackRangeSkill
    {
        public LiegongFZRange() : base("#liegong_fz-for-lord")
        {
        }
        public override int GetExtra(Room room, Player target, bool include_weapon)
        {
            if (RoomLogic.PlayerHasShownSkill(room, target, "liegong_fz"))
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

    public class KuangguFZ : TriggerSkill
    {
        public KuangguFZ() : base("kuanggu_fz")
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
                int recorded_damage = player.ContainsTag("InvokeKuanggu") ? (int)player.GetTag("InvokeKuanggu") : 0;
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
                Player fz = RoomLogic.FindPlayerBySkillName(room, "xuanhuo");
                if (fz != null && fz.Alive && RoomLogic.PlayerHasShownSkill(room, fz, "xuanhuo"))
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, fz, "xuanhuo");
                    room.BroadcastSkillInvoke(Name, "male", -1, gsk.General, gsk.SkinId);
                }
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
    //wangping
    public class Jianglue : ZeroCardViewAsSkill
    {
        public Jianglue() : base("jianglue")
        {
            frequency = Frequency.Limited;
            limit_mark = "@jiang";
        }

        public override bool IsEnabledAtPlay(Room room, Player player) => player.GetMark("@jiang") > 0;

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(JianglueCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name,
                Mute = true
            };
            return card;
        }
    }

    public class JianglueCard : SkillCard
    {

        public static string ClassName = "JianglueCard";
        public JianglueCard() : base(ClassName) { target_fixed = true; }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@jiang", 0);
            room.BroadcastSkillInvoke("jianglue", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "jianglue");
            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            WrappedCard card = room.AskForUseCard(player, "@@imperialorder!", "@jianglue", null, -1, HandlingMethod.MethodUse);
            if (card == null)
            {
                string card_name = player.ContainsTag("imperialorder_select") ? ((string)player.GetTag("imperialorder_select")).Split('+')[0] : string.Empty;
                if (string.IsNullOrEmpty(card_name))
                    card = ImperialOrderVS.GetImperialOrders(room, player)[0];
                else
                    card = new WrappedCard(card_name);

                room.UseCard(new CardUseStruct(card, player, new List<Player>()));
            }
            player.RemoveTag("imperialorder_select");

            List<Player> do_effect = new List<Player>() { player };

            if (player.Role != "careerist")
                room.KingdomSummons(player.Kingdom);

            player.SetTag("order_reason", "jianglue");
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard != null && fcard is ImperialOrder order)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (RoomLogic.IsFriendWith(room, p, player) && order.Effect(room, player, p))
                        do_effect.Add(p);
            }

            foreach (Player p in do_effect)
            {
                if (p.Alive)
                {
                    if (p != player)
                    {
                        ResultStruct result = card_use.From.Result;
                        result.Assist++;
                        card_use.From.Result = result;
                    }

                    p.MaxHp++;
                    room.BroadcastProperty(p, "MaxHp");

                    LogMessage log = new LogMessage
                    {
                        Type = "$GainMaxHp",
                        From = p.Name,
                        Arg = "1"
                    };
                    room.SendLog(log);

                    room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, p);
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = p,
                        Recover = 1
                    };
                    room.Recover(p, recover, true);
                }
            }

            if (player.Alive && do_effect.Count > 1)
                room.DrawCards(player, new DrawCardStruct(do_effect.Count - 1, player, "jianglue"));
        }
    }

    //wuguotai
    public class Ganlu : ZeroCardViewAsSkill
    {
        public Ganlu() : base("ganlu") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("GanluCard");
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(GanluCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            return card;
        }
    }

    public class GanluCard : SkillCard
    {
        public static string ClassName = "GanluCard";
        public GanluCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count == 0)
                return true;

            if (targets.Count == 1 && (to_select.HasEquip() || targets[0].HasEquip()) && Math.Abs(to_select.GetEquips().Count - targets[0].GetEquips().Count) <= Self.GetLostHp())
            {
                for (int i = 0; i < 6; i++)
                {
                    if (targets[0].GetEquip(i) >= 0 && to_select.EquipIsBaned(i)) return false;
                    if (to_select.GetEquip(i) >= 0 && targets[0].EquipIsBaned(i)) return false;
                }

                return true;
            }

            return false;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 2;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            LogMessage log = new LogMessage
            {
                Type = "#GanluSwap",
                From = source.Name
            };
            log.SetTos(card_use.To);
            room.SendLog(log);

            Player first = card_use.To[0], second = card_use.To[1];
            List<int> equips1 = first.GetEquips(), equips2 = second.GetEquips();

            if (!card_use.To.Contains(source) || (first != source && equips2.Count > equips2.Count) || (first == source && equips2.Count < equips2.Count))
            {
                ResultStruct result = card_use.From.Result;
                result.Assist += Math.Abs(equips1.Count - equips2.Count);
                card_use.From.Result = result;
            }

            List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct>();
            CardsMoveStruct move1 = new CardsMoveStruct(equips1, second, Place.PlaceEquip,
                new CardMoveReason(MoveReason.S_REASON_SWAP, first.Name, second.Name, "ganlu", string.Empty));
            CardsMoveStruct move2 = new CardsMoveStruct(equips2, first, Place.PlaceEquip,
                new CardMoveReason(MoveReason.S_REASON_SWAP, second.Name, first.Name, "ganlu", string.Empty));
            exchangeMove.Add(move2);
            exchangeMove.Add(move1);
            room.MoveCardsAtomic(exchangeMove, false);
        }
    }

    public class Buyi : TriggerSkill
    {
        public Buyi() : base("buyi")
        {
            events = new List<TriggerEvent> { TriggerEvent.QuitDying };
            skill_type = SkillType.Recover;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> resutlt = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.QuitDying && player.Alive && data is DyingStruct dying
                && dying.Damage.From != null && dying.Damage.From.Alive)
            {
                List<Player> wuguotais = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in wuguotais)
                {
                    if (RoomLogic.WillBeFriendWith(room, p, player, Name) && !p.HasFlag(Name))
                    {
                        TriggerStruct trigger = new TriggerStruct(Name, p);
                        resutlt.Add(trigger);
                    }
                }
            }

            return resutlt;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && data is DyingStruct dying
                && dying.Damage.From != null && dying.Damage.From.Alive
                && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                ask_who.SetFlags(Name);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && data is DyingStruct dying  && dying.Damage.From != null && dying.Damage.From.Alive)
            {
                Player target = dying.Damage.From;
                target.SetFlags("imperialorder_target");
                ask_who.SetTag("order_reason", Name);
                WrappedCard card = room.AskForUseCard(ask_who, "@@imperialorder!", string.Format("@buyi:{0}:{1}", player.Name, target.Name), null, -1, FunctionCard.HandlingMethod.MethodUse);
                if (card == null)
                {
                    string card_name = ask_who.ContainsTag("imperialorder_select") ? ((string)ask_who.GetTag("imperialorder_select")).Split('+')[0] : string.Empty;
                    if (string.IsNullOrEmpty(card_name))
                        card = ImperialOrderVS.GetImperialOrders(room, ask_who)[0];
                    else
                        card = new WrappedCard(card_name);

                    CardUseStruct use = new CardUseStruct(card, ask_who, target);
                    room.UseCard(use);
                }
                ask_who.RemoveTag("imperialorder_select");

                if (!ask_who.ContainsTag("ImperialOrder") || !(bool)ask_who.GetTag("ImperialOrder"))
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = ask_who
                    };

                    room.Recover(player, recover, true);
                }
            }

            return false;
        }
    }

    //lukang
    public class Keshou : TriggerSkill
    {
        public Keshou() : base("keshou")
        {
            events.Add(TriggerEvent.DamageInflicted);
            skill_type = SkillType.Defense;
            view_as_skill = new KeshouVS();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetTag("keshou_data", data);
            WrappedCard card = room.AskForUseCard(player, "@@keshou", "@keshuo", null, -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
            room.RemoveTag("keshou_data");
            if (card != null && card.SubCards.Count == 2)
            {
                List<int> ids = new List<int>(card.SubCards);
                room.ThrowCard(ref ids, player, null, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "$keshou",
                From = player.Name,
                Arg = damage.Damage.ToString(),
                Arg2 = (--damage.Damage).ToString()
            };
            room.SendLog(log);

            data = damage;

            bool check = true;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (RoomLogic.IsFriendWith(room, p, player))
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                JudgeStruct judge = new JudgeStruct
                {
                    Who = player,
                    Pattern = ".|red",
                    Good = true
                };
                room.Judge(ref judge);

                if (judge.IsGood())
                    room.DrawCards(player, 1, Name);
            }

            if (damage.Damage < 1)
                return true;

            return false;
        }
    }

    public class KeshouVS : ViewAsSkill
    {
        public KeshouVS() : base("keshou") { response_pattern = "@@keshou"; }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 1) return false;
            if (selected.Count == 1)
                return WrappedCard.IsBlack(selected[0].Suit) == WrappedCard.IsBlack(to_select.Suit);

            return true;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 2)
            {
                WrappedCard card = new WrappedCard(KeshouCard.ClassName);
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }

    public class KeshouCard : SkillCard
    {
        public static string ClassName = "KeshouCard";
        public KeshouCard() : base(ClassName) { target_fixed = true; }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            //do nothing
        }
    }

    public class Zhuwei : TriggerSkill
    {
        public Zhuwei() : base("zhuwei")
        {
            frequency = Frequency.Frequent;
            events= new List<TriggerEvent> { TriggerEvent.FinishJudge, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                player.SetMark("zhuwei_slash", 0);
                player.SetMark("zhuwei_max", 0);
                player.RemoveStringMark(Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.FinishJudge && data is JudgeStruct judge
                && room.GetCardPlace(judge.Card.GetEffectiveId()) == Place.PlaceJudge && base.Triggerable(player, room)
                && (judge.Card.Name.Contains(Slash.ClassName) || judge.Card.Name == Duel.ClassName))
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
            if (data is JudgeStruct judge && room.GetCardPlace(judge.Card.GetEffectiveId()) == Place.PlaceJudge)
            {
                room.ObtainCard(player, judge.Card);

                Player target = null;
                if (room.Current != null && room.Current != player && room.AskForSkillInvoke(player, "zhuwei_max", "@zhuwei:" + room.Current.Name, info.SkillPosition))
                {
                    target = room.Current;
                }
                else if (room.Current == player)
                    target = player;

                if (target != null)
                {
                    LogMessage log = new LogMessage("$zhuwei")
                    {
                        From = target.Name
                    };
                    room.SendLog(log);

                    target.AddMark("zhuwei_slash");
                    room.SetPlayerStringMark(target, Name, target.GetMark("zhuwei_slash").ToString());
                    //if (target == player)
                        target.AddMark("zhuwei_max");
                }
            }
            return false;
        }
    }

    public class ZhuweiTar : TargetModSkill
    {
        public ZhuweiTar() : base("#zhuwei-tar", false)
        {
            pattern = Slash.ClassName;
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (Engine.MatchExpPattern(room, pattern, from, card))
            {
                return from.GetMark("zhuwei_slash");
            }

            return 0;
        }
    }

    public class ZhuweiMax : MaxCardsSkill
    {
        public ZhuweiMax() : base("#zhuwei-max") { }

        public override int GetExtra(Room room, Player target)
        {
            return target.GetMark("zhuwei_max");
        }
    }

    //lord caocao
    public class Huibian : ZeroCardViewAsSkill
    {
        public Huibian() : base("huibian") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(HuibianCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(HuibianCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            return card;
        }
    }

    public class HuibianCard : SkillCard
    {
        public static string ClassName = "HuibianCard";
        public HuibianCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (!RoomLogic.IsFriendWith(room, Self, to_select) || targets.Count > 1) return false;
            if (targets.Count == 1)
                return to_select.IsWounded();

            return true;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 2;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = card_use.To;
            
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            if (!TargetFixed(card_use.Card) || card_use.To.Count > 1 || !card_use.To.Contains(card_use.From))
                log.SetTos(card_use.To);

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            room.SendLog(log);
            room.BroadcastSkillInvoke("huibian", player, card_use.Card.SkillPosition);

            room.Damage(new DamageStruct("huibian", player, targets[0]));
            if (targets[0].Alive)
                room.DrawCards(targets[0], new DrawCardStruct(2, player, "huibian"));
            if (targets[1].Alive && targets[1].IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };

                room.Recover(targets[1], recover, true);
            }
        }
    }

    public class Zongyu : TriggerSkill
    {
        public Zongyu() : base("zongyu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.CardUsed };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceEquip)
            {
                bool check = false;
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCard(id).Name == DragonCarriage.ClassName)
                    {
                        check = true;
                        break;
                    }
                }

                if (check)
                {
                    Player lord = RoomLogic.FindPlayerBySkillName(room, Name);
                    if (lord != null && (lord.GetOffensiveHorse() || lord.GetDefensiveHorse())) return new TriggerStruct(Name, lord);
                }
            }
            else if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Horse)
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (p.GetSpecialEquip())
                            return new TriggerStruct(Name, player);

                    foreach (int card_id in room.DiscardPile)
                        if (room.GetCard(card_id).Name == DragonCarriage.ClassName)
                            return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
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
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceEquip)
            {
                List<int> first_ids = new List<int>(), second_ids = new List<int>();
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCard(id).Name == DragonCarriage.ClassName)
                    {
                        first_ids.Add(id);
                        break;
                    }
                }

                if (ask_who.GetOffensiveHorse()) second_ids.Add(ask_who.OffensiveHorse.Key);
                if (ask_who.GetDefensiveHorse()) second_ids.Add(ask_who.DefensiveHorse.Key);

                if (first_ids.Count > 0 && second_ids.Count > 0)
                {
                    List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct>();
                    CardsMoveStruct move1 = new CardsMoveStruct(first_ids, ask_who, Place.PlaceEquip,
                        new CardMoveReason(MoveReason.S_REASON_SWAP, move.To.Name, ask_who.Name, Name, string.Empty));
                    CardsMoveStruct move2 = new CardsMoveStruct(second_ids, move.To, Place.PlaceEquip,
                        new CardMoveReason(MoveReason.S_REASON_SWAP, ask_who.Name, move.To.Name, Name, string.Empty));
                    exchangeMove.Add(move2);
                    exchangeMove.Add(move1);
                    room.MoveCardsAtomic(exchangeMove, true);
                }
            }
            else if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && data is CardUseStruct use)
            {
                int id = -1;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.GetSpecialEquip())
                    {
                        id = p.Special.Key;
                        break;
                    }
                }

                if (id == -1)
                {
                    foreach (int card_id in room.DiscardPile)
                    {
                        if (room.GetCard(card_id).Name == DragonCarriage.ClassName)
                        {
                            id = card_id;
                            break;
                        }
                    }
                }

                if (id >= 0)
                {
                    room.CancelTarget(ref use, player);
                    data = use;

                    room.MoveCardTo(room.GetCard(id), player, Place.PlaceEquip, true);
                }
            }

            return false;
        }
    }

    public class Jianan : TriggerSkill
    {
        public Jianan() : base("jianan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == PlayerPhase.RoundStart)
            {
                List<string> ac = player.GetAcquiredSkills();
                List<string> skills = new List<string> {
                    "-tuxi_cc", "-qiaobian_cc", "-xiaoguo_cc", "-jieyue_cc", "-duanliang_cc"
                };

                bool check = false;
                foreach (string skill in skills)
                {
                    string _skill = skill.Substring(1, skill.Length - 1);
                    if (ac.Contains(_skill))
                    {
                        check = true;
                        break;
                    }
                }

                if (check)
                {
                    room.HandleAcquireDetachSkills(player, skills, true);
                    room.RemovePlayerDisableShow(player, Name);
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.HasShownOneGeneral() && !player.IsNude() && RoomLogic.CanDiscard(room, player, player, "he"))
            {
                Player lord = RoomLogic.FindPlayerBySkillName(room, Name);
                if (lord != null && RoomLogic.PlayerHasShownSkill(room, lord, Name) && RoomLogic.IsFriendWith(room, player, lord))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.IsNude() || !RoomLogic.CanDiscard(room, player, player, "he")) return new TriggerStruct();

            bool invoke = false;
            if (!player.HasShownAllGenerals() && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.SetPlayerDisableShow(player, player.General1Showed ? "d" : "h", Name);
                invoke = true;
            }
            else if (player.HasShownAllGenerals())
            {
                if (!Engine.GetGeneral(player.General1, room.Setting.GameMode).IsLord())
                {
                    List<TriggerStruct> skills = new List<TriggerStruct>
                    {
                        new TriggerStruct("GameRule_AskForGeneralShowHead", player),
                        new TriggerStruct("GameRule_AskForGeneralShowDeputy", player)
                    };
                    TriggerStruct trigger = room.AskForSkillTrigger(player, "jianan-disable", skills, true);
                    if (trigger.SkillName == "GameRule_AskForGeneralShowHead")
                    {
                        room.HideGeneral(player, true);
                        room.SetPlayerDisableShow(player, "h", Name);
                        invoke = true;
                    }
                    else if (trigger.SkillName == "GameRule_AskForGeneralShowDeputy")
                    {
                        room.HideGeneral(player, false);
                        room.SetPlayerDisableShow(player, "d", Name);
                        invoke = true;
                    }
                }
                else if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                {
                    room.HideGeneral(player, false);
                    room.SetPlayerDisableShow(player, "d", Name);
                    invoke = true;
                }
            }

            if (invoke)
            {
                room.AskForDiscard(player, Name, 1, 1, false, true, "@jianan-discard", false);
                Player lord = RoomLogic.FindPlayerBySkillName(room, Name);
                if (lord != null && lord.Alive)
                {
                    room.NotifySkillInvoked(lord, Name);
                    room.BroadcastSkillInvoke(Name, lord, "head");
                }
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> skills = new List<string>
                    {
                        "tuxi_cc", "qiaobian_cc", "xiaoguo_cc", "jieyue_cc", "duanliang_cc"
                    };
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.PlayerHasShownSkill(room, p, "tuxi_cc|tuxi"))
                    skills.Remove("tuxi_cc");
                if (RoomLogic.PlayerHasShownSkill(room, p, "qiaobian_cc|qiaobian"))
                    skills.Remove("qiaobian_cc");
                if (RoomLogic.PlayerHasShownSkill(room, p, "xiaoguo_cc|xiaoguo"))
                    skills.Remove("xiaoguo_cc");
                if (RoomLogic.PlayerHasShownSkill(room, p, "jieyue_cc|jieyue"))
                    skills.Remove("jieyue_cc");
                if (RoomLogic.PlayerHasShownSkill(room, p, "duanliang_cc|duanliang"))
                    skills.Remove("duanliang_cc");
            }

            if (skills.Count > 0)
            {
                string skill = room.AskForChoice(player, Name, string.Join("+", skills));
                if (skills.Contains(skill))
                    room.AcquireSkill(player, skill, true, true);
            }

            return false;
        }
    }

    public class JieyueCC : PhaseChangeSkill
    {
        public JieyueCC() : base("jieyue_cc")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Wizzard;
        }
        public override bool CanPreShow() => false;
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && !target.IsKongcheng() && target.Phase == PlayerPhase.Start;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard card = room.AskForUseCard(player, "@@jieyue", "@jieyue", null, -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null)
            {
                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "jianan") && lord.General1Showed)
                    room.BroadcastSkillInvoke(Name, lord, "head");

                room.NotifySkillInvoked(player, Name);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)player.GetTag("jieyue"));
            if (target != null)
            {
                target.SetFlags("imperialorder_target");        //这个flag表明该玩家为敕令的固定目标，且会自动在ViewAsSkill生成随机敕令牌后清除
                player.SetTag("order_reason", "jieyue");
                if (room.AskForUseCard(player, "@@imperialorder!", "@jieyue-target:" + target.Name, null, -1, FunctionCard.HandlingMethod.MethodUse) == null)
                {
                    WrappedCard card = ((List<WrappedCard>)room.GetTag("imperialorder_select"))[0];
                    CardUseStruct use = new CardUseStruct(card, player, target);
                    room.UseCard(use);
                }
                if (!player.ContainsTag("ImperialOrder") || !(bool)player.GetTag("ImperialOrder"))
                    player.SetFlags("jieyue_draw");
                else
                    room.DrawCards(player, 1, Name);
            }

            return false;
        }
    }

    class TuxiCC : TriggerSkill
    {
        public TuxiCC() : base("tuxi_cc")
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

            List<Player> choosees = room.AskForPlayersChosen(player, to_choose, Name, 0, 2, "@tuxi-card", true, info.SkillPosition);
            if (choosees.Count > 0)
            {
                room.SortByActionOrder(ref choosees);
                room.SetTag("tuxi_invoke" + player.Name, choosees);

                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "jianan") && lord.General1Showed)
                    room.BroadcastSkillInvoke(Name, lord, "head");
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player source, ref object data, Player ask_who, TriggerStruct info)
        {
            string str = "tuxi_invoke" + source.Name;
            List<Player> targets = room.ContainsTag(str) ? (List<Player>)room.GetTag(str) : new List<Player>();
            room.RemoveTag(str);
            int id = room.AskForCardChosen(source, targets[0], "h", Name, false, FunctionCard.HandlingMethod.MethodGet);
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
                id = room.AskForCardChosen(source, targets[1], "h", Name, false, FunctionCard.HandlingMethod.MethodGet);
                CardsMoveStruct move2 = new CardsMoveStruct
                {
                    Card_ids = new List<int> { id },
                    To = source.Name,
                    To_place = Place.PlaceHand,
                    Reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, source.Name, targets[1].Name, Name, null)
                };
                moves.Add(move2);
            }
            room.MoveCardsAtomic(moves, false);
            return true;
        }
    }

    public class XiaoguoCC : TriggerSkill
    {
        public XiaoguoCC() : base("xiaoguo_cc")
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
            List<int> ints = new List<int>(room.AskForExchange(ask_who, Name, 1, 0, "@xiaoguo", null, "BasicCard!", info.SkillPosition));
            if (ints.Count == 1)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISMANTLE, ask_who.Name, ask_who.Name, Name, null)
                {
                    General = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition)
                };
                room.ThrowCard(ref ints, reason, ask_who, null, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);

                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "jianan") && lord.General1Showed)
                    room.BroadcastSkillInvoke(Name, lord, "head");

                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetTag(Name, ask_who);
            WrappedCard card = room.AskForCard(player, Name, ".Equip", "@xiaoguo-discard", null);
            room.RemoveTag(Name);
            if (card == null)
                room.Damage(new DamageStruct(Name, ask_who, player));

            return false;
        }
    }
    public class QiaobianCC : TriggerSkill
    {
        public QiaobianCC() : base("qiaobian_cc")
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
                Player lord = RoomLogic.GetLord(room, zhanghe.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "jianan") && lord.General1Showed)
                    room.BroadcastSkillInvoke(Name, lord, "head");

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
                room.AskForUseCard(player, "@@qiaobian", use_prompt, null, index, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
            }
            return false;
        }
    }

    public class DuanliangCCVS : OneCardViewAsSkill
    {
        public DuanliangCCVS() : base("duanliang_cc")
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

    public class DuanliangCC : TriggerSkill
    {
        public DuanliangCC() : base("duanliang_cc")
        {
            events.Add(TriggerEvent.CardUsedAnnounced);
            view_as_skill = new DuanliangCCVS();
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

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
            Player lord = RoomLogic.GetLord(room, player.Kingdom);
            if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "jianan") && lord.General1Showed)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, lord, "jianan");
                room.BroadcastSkillInvoke(skill_name, "male", -1, gsk.General, gsk.SkinId);
            }
        }
    }

    public class DuanliangCCTargetMod : TargetModSkill
    {
        public DuanliangCCTargetMod() : base("#duanliang_cc-target")
        {
            pattern = SupplyShortage.ClassName;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "duanliang_cc") && !from.HasFlag("duanliang_cc"))
                return true;
            else
                return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -1;
            if (type == ModType.DistanceLimit && card.Skill == "duanliang_cc")
                index = -2;
            else
            {
                Player lord = RoomLogic.GetLord(room, player.Kingdom);
                if (lord != null && RoomLogic.PlayerHasSkill(room, lord, "jianan") && lord.General1Showed)
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, lord, "jianan");
                    index = -1;
                    general_name = gsk.General;
                    skin_id = gsk.SkinId;
                }
            }
        }
    }
}
