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
    public class Naturefour : GeneralPackage
    {
        public Naturefour() : base("Naturefour")
        {
            skills = new List<Skill>
            {
                new LuanjiJX(),
                new Xueyi(),
                new XueyiMax(),
                new WeimuJX(),
                new LeijiJX(),
                new Huangtian(),
                new HuangtianVS(),

                new LiegongJX(),
                new LiegongRecord(),
                new LiegongTar(),
                new KuangguJX(),
                new Qimou(),
                new QimouTar(),
                new QimouDistance(),
                new Ruoyu(),
                new Zhiji(),

                new Songwei(),
                new QiceJX(),
                new DuanliangJX(),
                new DuanliangJXTargetMod(),
                new Jiezhi(),
                new JushouJX(),
                new Jiewei(),
                new JiemingJX(),
                new Zaoxian(),

                new BuquJX(),
                new BuquJXClear(),
                new BuquMax(),
                new FenjiJX(),
                new TianxiangJX(),
                new TianxiangSecond(),
                new Hunzi(),
                new Zhiba(),
                new ZhibaVS(),
            };
            skill_cards = new List<FunctionCard>
            {
                new QimouCard(),
                new HuangtianCard(),
                new ZhibaCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "xueyi", new List<string>{ "#xueyi-max" } },
                { "liegong_jx", new List<string>{ "#liegong-damage", "#liegong-tar" } },
                { "qimou", new List<string>{ "#qimou-tar", "#qimou-distance" } },
                { "duanliang_jx", new List<string>{ "#jxduanliang-target" } },
                { "buqu_jx", new List<string>{ "#buqu_jx-clear", "#buqu-max" } },
                { "tianxiang_jx", new List<string>{ "#tianxian-second" } },
            };
        }
    }
    //袁绍
    public class LuanjiJX : ViewAsSkill
    {
        public LuanjiJX() : base("luanji_jx")
        {
            response_or_use = true;
            skill_type = SkillType.Alter;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return true;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 1 || room.GetCardPlace(to_select.Id) == Player.Place.PlaceEquip) return false;
            if (selected.Count == 1)
                return selected[0].Suit == to_select.Suit;

            return true;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count < 2) return null;
            WrappedCard aa = new WrappedCard(ArcheryAttack.ClassName) { Skill = Name };
            aa.AddSubCards(cards);
            return aa;
        }
    }

    public class Xueyi : TriggerSkill
    {
        public Xueyi() : base("xueyi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            lord_skill = true;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change
                && change.To == PlayerPhase.Discard && RoomLogic.GetMaxCards(room, player) > player.Hp && player.HandcardNum > player.Hp)
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

    public class XueyiMax : MaxCardsSkill
    {
        public XueyiMax() : base("#xueyi-max")
        {
        }

        public override int GetExtra(Room room, Player target)
        {
            int count = 0;
            if (RoomLogic.PlayerHasShownSkill(room, target, "xueyi"))
            {
                foreach (Player p in room.GetOtherPlayers(target))
                    if (p.Kingdom == "qun")
                        count += 2;
            }

            return count;
        }
    }

    public class WeimuJX : ProhibitSkill
    {
        public WeimuJX() : base("weimu_jx")
        {
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (card != null && to != null && RoomLogic.PlayerHasShownSkill(room, to, Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                return fcard is TrickCard && WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, card));
            }

            return false;
        }
    }

    public class LeijiJX : TriggerSkill
    {
        public LeijiJX() : base("leiji_jx")
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
                    Pattern = ".|black",
                    Good = true,
                    Negative = true,
                    Reason = Name,
                    Who = target
                };

                room.Judge(ref judge);

                if (!judge.IsGood())
                {
                    if (judge.Card.Suit == WrappedCard.CardSuit.Spade)
                    {
                        if (target.Alive)
                            room.Damage(new DamageStruct(Name, zhangjiao, target, 2, DamageStruct.DamageNature.Thunder));
                    }
                    else
                    {
                        if (zhangjiao.Alive && zhangjiao.GetLostHp() > 0)
                        {
                            RecoverStruct recover = new RecoverStruct
                            {
                                Recover = 1,
                                Who = zhangjiao
                            };
                            room.Recover(zhangjiao, recover, true);
                        }
                        if (target.Alive)
                            room.Damage(new DamageStruct(Name, zhangjiao, target, 1, DamageStruct.DamageNature.Thunder));
                    }
                }
            }

            return false;
        }
    }

    public class Huangtian : TriggerSkill
    {
        public Huangtian() : base("huangtian")
        {
            lord_skill = true;
            frequency = Frequency.Compulsory;
            events.Add(TriggerEvent.GameStart);
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (base.Triggerable(player, room))
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetRoleEnum() != PlayerRole.Lord && p.Kingdom == "qun")
                    {
                        room.HandleAcquireDetachSkills(p, "huangtianvs", true);
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class HuangtianVS : OneCardViewAsSkill
    {
        public HuangtianVS() : base("huangtianvs")
        {
            attached_lord_skill = true;
            frequency = Frequency.Compulsory;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "huantian");
            return jiaozhu.Count > 0 && player.Kingdom == "qun" && !player.HasUsed(HuangtianCard.ClassName);
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return to_select.Name == Jink.ClassName || to_select.Name == Lightning.ClassName;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ht = new WrappedCard(HuangtianCard.ClassName);
            ht.AddSubCard(card);
            return ht;
        }
    }

    public class HuangtianCard : SkillCard
    {
        public static string ClassName = "HuangtianCard";
        public HuangtianCard() : base(ClassName)
        {
            handling_method = HandlingMethod.MethodNone;
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "huantian");
            if (jiaozhu.Count < 2) return false;

            return targets.Count == 0 && jiaozhu.Contains(to_select);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "huantian");
            return (jiaozhu.Count == 1 && targets.Count == 0) || (targets.Count == 1 && jiaozhu.Contains(targets[0]));
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "huantian");
            Player target = null, player = card_use.From;
            if (jiaozhu.Count == 1 && card_use.To.Count == 0)
                target = jiaozhu[0];
            else if (card_use.To.Count == 1 && jiaozhu.Contains(card_use.To[0]))
                target = card_use.To[0];

            if (RoomLogic.PlayerHasSkill(room, target, "weidi_jx"))
            {
                room.BroadcastSkillInvoke("weidi_jx", target);
                room.NotifySkillInvoked(target, "weidi_jx");
            }
            else
            {
                room.BroadcastSkillInvoke("huangtian", target);
                room.NotifySkillInvoked(target, "huangtian");
            }
            room.ObtainCard(target, card_use.Card, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "huangtian", string.Empty));
        }
    }

    public class DuanchangJX : TriggerSkill
    {
        public DuanchangJX() : base("duanchang_jx")
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
                if (!target.General1.Contains("sujiang"))
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            return info;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.NotifySkillInvoked(player, Name);

            DeathStruct death = (DeathStruct)data;
            Player target = death.Damage.From;

            List<string> skills = target.GetSkills(true, false);
            foreach (string skill in skills)
            {
                Skill _s = Engine.GetSkill(skill);
                if (_s != null && !_s.Attached_lord_skill)
                    room.DetachSkillFromPlayer(target, skill, false, player.GetAcquiredSkills().Contains(skill), true);
            }

            if (death.Damage.From.Alive)
                room.SetPlayerMark(death.Damage.From, "@duanchang", 1);

            return false;
        }
    }

    //caopi
    public class Songwei : TriggerSkill
    {
        public Songwei() : base("songwei")
        {
            events.Add(TriggerEvent.FinishJudge);
            skill_type = SkillType.Replenish;
            lord_skill = true;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (player.Kingdom == "wei" && data is JudgeStruct judge && player.Alive && WrappedCard.IsBlack(judge.Card.Suit))
            {
                List<Player> caopis = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in caopis)
                {
                    if (player != p)
                        result.Add(new TriggerStruct(Name, player, p));
                }
            }

            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player caopi = room.FindPlayer(info.SkillOwner);
            if (ask_who.Alive && caopi.Alive && room.AskForSkillInvoke(ask_who, Name, caopi))
            {
                room.NotifySkillInvoked(caopi, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, info.SkillOwner);
                room.BroadcastSkillInvoke(Name, caopi, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player caopi = room.FindPlayer(info.SkillOwner);
            room.DrawCards(player, new DrawCardStruct(1, ask_who, Name));
            return false;
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

    //徐晃
    public class DuanliangJX : OneCardViewAsSkill
    {
        public DuanliangJX() : base("duanliang_jx")
        {
            filter_pattern = "BasicCard,EquipCard|black";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => true;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
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

    public class DuanliangJXTargetMod : TargetModSkill
    {
        public DuanliangJXTargetMod() : base("#jxduanliang-target")
        {
            pattern = "SupplyShortage";
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (to != null && RoomLogic.PlayerHasSkill(room, from, "duanliang_jx") && to.HandcardNum >= from.HandcardNum)
                return true;
            else
                return false;
        }
    }

    public class Jiezhi : TriggerSkill
    {
        public Jiezhi() : base("jiezhi")
        {
            events.Add(TriggerEvent.EventPhaseSkipping);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is PhaseChangeStruct change && change.To == Player.PlayerPhase.Draw && player.Alive)
            {
                List<Player> xuhuangs = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in xuhuangs)
                    if (p != player)
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    //曹仁
    public class JieweiVS : OneCardViewAsSkill
    {
        public JieweiVS() : base("jiewei")
        {
            filter_pattern = ".|.|.|equipped";
            response_pattern = "Nullification";
            skill_type = SkillType.Alter;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ncard = new WrappedCard(Nullification.ClassName) { Skill = Name };
            ncard.AddSubCard(card);
            ncard = RoomLogic.ParseUseCard(room, ncard);
            return ncard;
        }
        public override bool IsEnabledAtNullification(Room room, Player player)
        {
            return player.HasEquip();
        }
    }
    public class JushouJXVS : OneCardViewAsSkill
    {
        public JushouJXVS() : base("jushou_jx")
        {
            response_pattern = "@@jushou_jx";
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
    public class JushouJX : PhaseChangeSkill
    {
        public JushouJX() : base("jushou_jx")
        {
            view_as_skill = new JushouJXVS();
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
            room.TurnOver(caoren);
            room.DrawCards(caoren, 4, Name);

            List<int> ids = caoren.GetCardCount(false) == 1 ? caoren.GetCards("h") : new List<int>();
            if (ids.Count == 0)
            {
                WrappedCard use = room.AskForUseCard(caoren, "@@jushou_jx", "@jushou", -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
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
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISCARD, caoren.Name, null, Name, null)
                    {
                        General = gsk
                    };
                    room.ThrowCard(ref ids, reason, caoren, caoren);
                }
                else
                    room.UseCard(new CardUseStruct(card, caoren, new List<Player>(), true));
            }

            return false;
        }
    }
    public class Jiewei : TriggerSkill
    {
        public Jiewei() : base("jiewei")
        {
            events.Add(TriggerEvent.TurnedOver);
            view_as_skill = new JieweiVS();
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return (base.Triggerable(player, room) && player.FaceUp && player.Alive) ? new TriggerStruct(Name, player) : new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForDiscard(player, Name, 1, 1, true, true, "@jiewei", true, info.SkillPosition))
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
            if (targets.Count > 0)
            {
                Player target1 = room.AskForPlayerChosen(player, targets, Name, "@jiewei1", true, false, info.SkillPosition);
                if (target1 != null)
                {
                    int card_id = room.AskForCardChosen(player, target1, "ej", Name);
                    WrappedCard card = room.GetCard(card_id);
                    Player.Place place = room.GetCardPlace(card_id);

                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    int equip_index = -1;
                    if (place == Player.Place.PlaceEquip)
                    {
                        EquipCard equip = (EquipCard)fcard;
                        equip_index = (int)equip.EquipLocation();
                    }

                    List<Player> tos = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (equip_index != -1)
                        {
                            if (p.GetEquip(equip_index) < 0)
                                tos.Add(p);
                        }
                        else
                        {
                            if (RoomLogic.IsProhibited(room, player, p, card) == null && !RoomLogic.PlayerContainsTrick(room, p, card.Name))
                                tos.Add(p);
                        }
                    }

                    room.SetTag("MouduanTarget", target1);
                    string position = info.SkillPosition;
                    Player to = room.AskForPlayerChosen(player, tos, Name, "@jiewei-to:::" + card.Name, false, false, position);
                    room.RemoveTag("MouduanTarget");
                    if (to != null)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target1.Name, to.Name);
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TRANSFER, player.Name, Name, null);
                        room.MoveCardTo(card, target1, to, place, reason);

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
                    }
                }
            }
            return false;
        }
    }

    public class LiegongJX : TriggerSkill
    {
        public LiegongJX() : base("liegong_jx")
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
                        if (to.HandcardNum <= player.HandcardNum || to.Hp >= player.Hp)
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
            List<string> choices = new List<string>();
            if (target.HandcardNum >= player.Hp || target.HandcardNum <= RoomLogic.GetAttackRange(room, player))
                choices.Add("nojink");
            if (target.Hp >= player.Hp)
                choices.Add("damage");

            if (choices.Count == 0)
                return false;

            CardUseStruct use = (CardUseStruct)data;
            while (choices.Count > 0)
            {
                string choice = room.AskForChoice(player, Name, string.Join("+", choices));
                if (choice == "nojink")
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#NoJink",
                        From = target.Name
                    };
                    room.SendLog(log); bool done = false;
                    int index = use.To.IndexOf(target);
                    for (int i = 0; i < use.EffectCount.Count; i++)
                    {
                        EffctCount effect = use.EffectCount[i];
                        if (effect.Index == index)
                        {
                            effect.Count = 0;
                            done = true;
                            use.EffectCount[i] = effect;
                        }
                    }
                    if (!done)
                    {
                        EffctCount effect = new EffctCount(player, target, 0)
                        {
                            Index = index
                        };
                        use.EffectCount.Add(effect);
                    }
                }
                else if (choice == "damage")
                {
                    string tag = string.Format("liegong_{0}", RoomLogic.CardToString(room, use.Card));
                    List<string> targets = player.ContainsTag(tag) ? (List<string>)player.GetTag(tag) : new List<string>();
                    targets.Add(target.Name);
                    player.SetTag(tag, targets);
                }
                else
                    break;

                choices.Remove(choice);
                if (choices.Count > 0 && !choices.Contains("cancel"))
                    choices.Add("cancel");
                else
                    break;
            }

            return false;
        }
    }

    public class LiegongRecord : TriggerSkill
    {
        public LiegongRecord() : base("#liegong-damage")
        {
            events = new List<TriggerEvent> { TriggerEvent.ConfirmDamage, TriggerEvent.CardFinished };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName))
            {
                string tag = string.Format("liegong_{0}", RoomLogic.CardToString(room, use.Card));
                use.From.RemoveTag(tag);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.ConfirmDamage && data is DamageStruct damage && damage.From != null && damage.Card != null
                && damage.Card.Name.Contains(Slash.ClassName) && !damage.Transfer && !damage.Chain)
            {
                string tag = string.Format("liegong_{0}", RoomLogic.CardToString(room, damage.Card));
                List<string> targets = player.ContainsTag(tag) ? (List<string>)player.GetTag(tag) : new List<string>();
                if (targets.Contains(damage.To.Name))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                string tag = string.Format("liegong_{0}", RoomLogic.CardToString(room, damage.Card));
                List<string> targets = (List<string>)damage.From.GetTag(tag);
                targets.Remove(damage.To.Name);
                player.SetTag(tag, targets);
                damage.Damage++;
                data = damage;
            }

            return false;
        }
    }

    public class LiegongTar : TargetModSkill
    {
        public LiegongTar() : base("#liegong-tar")
        {
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (from != null && RoomLogic.PlayerHasSkill(room, from, "liegong_jx") && to != null)
            {
                int distance = RoomLogic.DistanceTo(room, from, to, card);
                return distance > 0 && RoomLogic.GetCardNumber(room, card) >= distance;
            }

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class KuangguJX : TriggerSkill
    {
        public KuangguJX() : base("kuanggu_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.PreDamageDone };
            skill_type = SkillType.Recover;
            frequency = Frequency.Compulsory;
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
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            return info;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
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

    public class QimouVS : ZeroCardViewAsSkill
    {
        public QimouVS() : base("qimou") {}

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.Hp > 0 && player.GetMark("@mou") > 0;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard qimou = new WrappedCard(QimouCard.ClassName)
            {
                Skill = Name
            };
            return qimou;
        }
    }

    public class Qimou : TriggerSkill
    {
        public Qimou() : base("qimou")
        {
            limit_mark = "@mou";
            skill_type = SkillType.Attack;
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new QimouVS();
            frequency = Frequency.Limited;
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
    }

    public class QimouTar : TargetModSkill
    {
        public QimouTar() : base("#qimou-tar", false) {}

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            return Engine.MatchExpPattern(room, pattern, from, card) ? from.GetMark("qimou") : 0;
        }
    }

    public class QimouDistance : DistanceSkill
    {
        public QimouDistance() : base("#qimou-distance") { }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            return -from.GetMark("qimou");
        }
    }

    public class QimouCard : SkillCard
    {
        public static string ClassName = "QimouCard";
        public QimouCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@mou", 0);
            room.BroadcastSkillInvoke("qimou", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "qimou");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            if (player.Alive && player.Hp > 0)
            {
                List<string> choices = new List<string>();
                for (int i = player.Hp; i > 0; i--)
                    choices.Add(i.ToString());

                int lose = int.Parse(room.AskForChoice(player, "qimou", string.Join("+", choices), new List<string> { "@qimou-lose" }));
                room.LoseHp(player, lose);

                if (player.Alive)
                    player.SetMark("qimou", lose);
            }
        }
    }

    public class Ruoyu : PhaseChangeSkill
    {
        public Ruoyu() : base("ruoyu")
        {
            frequency = Frequency.Wake;
            lord_skill = true;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0)
            {
                int min = 100;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.Hp < min)
                        min = p.Hp;
                }
                if (player.Hp == min)
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
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(player, recover, true);

                room.HandleAcquireDetachSkills(player, "jijiang", true);
            }

            return false;
        }
    }

    public class Zhiji : PhaseChangeSkill
    {
        public Zhiji() : base("zhiji")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.IsKongcheng())
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
                room.HandleAcquireDetachSkills(player, "guanxing_jx", true);

            return false;
        }
    }

    public class JiemingJX : MasochismSkill
    {
        public JiemingJX() : base("jieming_jx")
        {
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && RoomLogic.PlayerHasSkill(room, player, Name) && data is DamageStruct damage)
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
            List<string> target_list = (List<string>)player.GetTag("jieming_target");
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

    public class Zaoxian : PhaseChangeSkill
    {
        public Zaoxian() : base("zaoxian")
        {
            frequency = Frequency.Wake;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.GetPile("field").Count >= 3)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            room.LoseMaxHp(player);
            if (player.Alive)
                room.HandleAcquireDetachSkills(player, "jixi", true);

            return false;
        }
    }

    public class BuquJX : TriggerSkill
    {
        public BuquJX() : base("buqu_jx")
        {
            events.Add(TriggerEvent.AskForPeaches);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Hp <= 0)
            {
                return new TriggerStruct(Name, player);
            }
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
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_PILE, null, Name, null);
                List<int> ints = new List<int> { id };
                room.ThrowCard(ref ints, reason, null);
                Thread.Sleep(1000);
            }
            else
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1 - player.Hp
                };
                room.Recover(player, recover, true);
            }

            return false;
        }
    }

    public class BuquMax : MaxCardsSkill
    {
        public BuquMax() : base("#buqu-max") { }

        public override int GetFixed(Room room, Player target)
        {
            if (target.GetPile("buqu_jx").Count > 0)
                return target.GetPile("buqu_jx").Count;

            return -1;
        }
    }

    public class BuquJXClear : DetachEffectSkill
    {
        public BuquJXClear() : base("buqu_jx", "buqu_jx")
        {
            frequency = Frequency.Compulsory;
        }
    }
    public class FenjiJX : TriggerSkill
    {
        public FenjiJX() : base("fenji_jx")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardsMoveOneTimeStruct move && move.From != null && move.From.Alive && move.From_places.Contains(Place.PlaceHand))
            {
                if ((move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_EXTRACTION
                    || (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD)
                    && !string.IsNullOrEmpty(move.Reason.PlayerId) && move.Reason.PlayerId != move.From.Name)
                {
                    List<Player> zhoutais = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player p in zhoutais)
                    {
                        triggers.Add(new TriggerStruct(Name, p));
                    }
                }
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move && move.From.Alive && ask_who.Alive)
            {
                move.From.SetFlags("fenji_target");
                bool invoke = room.AskForSkillInvoke(ask_who, Name, move.From, info.SkillPosition);
                move.From.SetFlags("-fenji_target");

                if (invoke)
                {
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, move.From.Name);
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move && move.From.Alive)
            {
                room.DrawCards(move.From, new DrawCardStruct(2, ask_who, Name));
                if (ask_who.Alive)
                    room.LoseHp(ask_who);
            }

            return false;
        }
    }

    public class TianxiangJXViewAsSkill : OneCardViewAsSkill
    {
        public TianxiangJXViewAsSkill() : base("tianxiang_jx")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@tianxiang_jx";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard tianxiangCard = new WrappedCard(TianxiangCard.ClassName);
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

    public class TianxiangJX : TriggerSkill
    {
        public TianxiangJX() : base("tianxiang_jx")
        {
            events.Add(TriggerEvent.DamageInflicted);
            skill_type = SkillType.Defense;
            view_as_skill = new TianxiangJXViewAsSkill();
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
            room.AskForUseCard(xiaoqiao, "@@tianxiang_jx", "@tianxiang_jx-card", -1, HandlingMethod.MethodDiscard, true, info.SkillPosition);
            room.RemoveTag("TianxiangDamage");
            if (xiaoqiao.HasFlag("tianxiang_invoke"))
                return info;

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player xiaoqiao, ref object data, Player ask_who, TriggerStruct info)
        {
            LogMessage log = new LogMessage
            {
                Type = "#Tianxiang",
                From = xiaoqiao.Name,
                Arg = Name,
            };
            room.SendLog(log);

            return true;
        }
    }
    
    public class TianxiangSecond : TriggerSkill
    {
        public TianxiangSecond() : base("#tianxian-second")
        {
            events.Add(TriggerEvent.DamageComplete);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.To == player && player.ContainsTag("tianxiang_target") && player.HasFlag("tianxiang_invoke"))
            {
                Player target = room.FindPlayer((string)player.GetTag("tianxiang_target"));
                if (target != null && target.Alive)
                {
                    return new TriggerStruct(Name, player);
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player xiaoqiao, ref object data, Player player, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)xiaoqiao.GetTag("tianxiang_target"));
            xiaoqiao.SetFlags("-tianxiang_invoke");
            DamageStruct damage = (DamageStruct)data;
            room.SetTag("TianxiangDamage", data);
            string choice = room.AskForChoice(xiaoqiao, "tianxiang_jx", "damage+losehp");
            room.RemoveTag("TianxiangDamage");
            xiaoqiao.RemoveTag("tianxiang_target");

            if (choice == "damage")
            {
                room.Damage(new DamageStruct("tianxiang_jx", damage.From, target));
                if (target.Alive)
                    room.DrawCards(target, Math.Min(5, target.GetLostHp()), "tianxiang_jx");
            }
            else if (xiaoqiao.GetTag("tianxiang_card") is int id)
            {
                room.LoseHp(target);
                if (target.Alive && room.GetCardPlace(id) == Place.DiscardPile)
                    room.ObtainCard(target, room.GetCard(id), new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, target.Name, "tianxiang_jx", string.Empty));
            }
            xiaoqiao.RemoveTag("tianxiang_card");

            return false;
        }
    }

    public class Hunzi : PhaseChangeSkill
    {
        public Hunzi() : base("hunzi")
        {
            frequency = Frequency.Wake;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.Hp == 1)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            room.LoseMaxHp(player);
            if (player.Alive)
            {
                List<string> skills = new List<string> { "yinghun_sunce", "yingzi_sunce" };
                room.HandleAcquireDetachSkills(player, skills);
            }

            return false;
        }
    }

    public class Zhiba : TriggerSkill
    {
        public Zhiba() : base("zhiba")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.Pindian };
            lord_skill = true;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetRoleEnum() != PlayerRole.Lord && p.Kingdom == "wu")
                    {
                        room.HandleAcquireDetachSkills(p, "zhibavs", true);
                    }
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class ZhibaVS : ZeroCardViewAsSkill
    {
        public ZhibaVS() : base("zhibavs")
        {
            attached_lord_skill = true;
            frequency = Frequency.Compulsory;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "zhiba");
            return jiaozhu.Count > 0 && player.Kingdom == "wu" && !player.HasUsed(ZhibaCard.ClassName) && !player.IsKongcheng();
        }
        

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(ZhibaCard.ClassName);
        }
    }

    public class ZhibaCard : SkillCard
    {
        public static string ClassName = "ZhibaCard";
        public ZhibaCard() : base(ClassName)
        {
            handling_method = HandlingMethod.MethodNone;
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "zhiba");
            if (jiaozhu.Count < 2) return false;

            return targets.Count == 0 && jiaozhu.Contains(to_select);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "zhiba");
            return (jiaozhu.Count == 1 && targets.Count == 0) || (targets.Count == 1 && jiaozhu.Contains(targets[0]));
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            List<Player> jiaozhu = RoomLogic.FindPlayersBySkillName(room, "zhiba");
            Player target = null, player = card_use.From;
            if (jiaozhu.Count == 1 && card_use.To.Count == 0)
                target = jiaozhu[0];
            else if (card_use.To.Count == 1 && jiaozhu.Contains(card_use.To[0]))
                target = card_use.To[0];

            bool do_pd = true;
            if (target.GetMark("hunzi") > 0)
                do_pd = room.AskForSkillInvoke(target, "zhiba", "@zhiba-refuse:" + player.Name);

            if (do_pd)
            {
                if (RoomLogic.PlayerHasSkill(room, target, "weidi_jx"))
                {
                    room.BroadcastSkillInvoke("weidi_jx", target);
                    room.NotifySkillInvoked(target, "weidi_jx");
                }
                else
                {
                    room.BroadcastSkillInvoke("zhiba", target);
                    room.NotifySkillInvoked(target, "zhiba");
                }

                PindianStruct pd = room.PindianSelect(player, target, "zhiba");
                if (!room.Pindian(ref pd) && room.AskForSkillInvoke(target, "zhiba", "@zhiba"))
                {
                    List<int> ids = new List<int>();
                    if (room.GetCardPlace(pd.From_card.Id) == Place.DiscardPile)
                        ids.Add(pd.From_card.Id);
                    if (room.GetCardPlace(pd.To_card.Id) == Place.DiscardPile)
                        ids.Add(pd.To_card.Id);

                    if (ids.Count > 0 && target.Alive)
                        room.ObtainCard(target, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTBACK, target.Name));
                }
            }
        }
    }
}