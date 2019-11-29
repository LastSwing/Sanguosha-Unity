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
    public class ClassicSpecial : GeneralPackage
    {
        public ClassicSpecial() : base("ClassicSpecial")
        {
            skills = new List<Skill>
            {
                new Shefu(),
                new ShefuClear(),
                new Benyu(),
                new Mashu("guanyu_sp"),
                new Nuzhan(),
                new NuzhanTM(),
                new Danji(),
                new Yuanhu(),
                new Bifa(),
                new Songci(),
                new Xianfu(),
                new Chouce(),
                new Chenqing(),
                new Moshi(),
                new Shanjia(),
                new ShanjiaDetach(),
                new Qizhi(),
                new Jinqu(),
                new Gushe(),
                new Jici(),
                new Kangkai(),
                new XiaoguoJX(),
                new Zhenwei(),
                new Kunfen(),
                new Fengliang(),
                new Weikui(),
                new WeikuiDistance(),
                new Lizhan(),
                new Gongao(),
                new Juyi(),
                new Weizhong(),
                new Junbing(),
                new Quji(),
                new Tuifeng(),
                new TuifengGet(),
                new TuifengTar(),
                new Zhenlue(),
                new ZhenluePro(),
                new Jianshu(),
                new Yongdi(),
                new Lingren(),
                new Fujian(),

                new Mizhao(),
                new Tianming(),
                new YongsiJX(),
                new YongsiMax(),
                new WeidiJX(),
                new WeidiRemove(),
                new Lihun(),
                new Chongzhen(),
                new Jieyuan(),
                new Fenxin(),
                new Zongkui(),
                new Guqu(),
                new Baijia(),
                new Canshi(),
                new Qiaomeng(),
                new Yicong(),
                new YicongDis(),
                new Jiqiao(),
                new Linglong(),
                new LinglongVH(),
                new LinglongTar(),
                new LinglongMax(),
                new Zhuiji(),
                new Shichou(),
                new Fuqi(),
                new Jiaozi(),
                new Moukui(),
                new MoukuiDis(),
                new Yishe(),
                new Bushi(),
                new Midao(),
                new Rangshang(),
                new Hanyong(),
                new Xionghuo(),
                new XionghuoTri(),
                new XionghuoMax(),
                new XionghuoPro(),
                new Shajue(),
                new Yisuan(),
                new Langxi(),
                new Luanzhan(),
                new Falu(),
                new FaluClear(),
                new Zhenyi(),
                new Dianhua(),

                new Shenxian(),
                new Qiangwu(),
                new QiangwuTar(),
                new Xueji(),
                new Huxiao(),
                new HuxiaoTar(),
                new Wuji(),
                new Liangzhu(),
                new Fanxiang(),
                new Fengpo(),
                new Mashu("mayunlu"),
                new Zhengnan(),
                new Xiefang(),
                new Wuniang(),
                new Xushen(),
                new Zhennan(),
                new Yuhua(),
                new YuhuaMax(),
                new Qirang(),
                new Fanghun(),
                new FanghunDetach(),
                new Fuhan(),
                new Zishu(),
                new Yingyuan(),
                new Ziyuan(),
                new Jujia(),
                new JujiaMax(),

                new Hongyuan(),
                new Huanshi(),
                new Mingzhe(),
                new Aocai(),
                new Duwu(),
                new Hongde(),
                new Dingpan(),
                new Xingwu(),
                new XingwuClear(),
                new Luoyan(),
                new Meibu(),
                new MeibuDistance(),
                new Mumu(),
                new Zhixi(),
            };

            skill_cards = new List<FunctionCard>
            {
                new ShefuCard(),
                //new BenyuCard(),
                new BifaCard(),
                new SongciCard(),
                new QiangwuCard(),
                new XuejiCard(),
                new AocaiCard(),
                new DuwuCard(),
                new MizhaoCard(),
                new WeidiJXCard(),
                new LihunCard(),
                new FanghunCard(),
                new DingpanCard(),
                new GusheCard(),
                new WeikuiCard(),
                new ZiyuanCard(),
                new XionghuoCard(),
                new QujiCard(),
                new JianshuCard(),
                new ZhenyiCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "shefu", new List<string>{ "#shefu-clear"} },
                { "nuzhan", new List<string>{ "#nuzhan" } },
                { "qiangwu", new List<string>{ "#qiangwu-tar" } },
                { "huxiao", new List<string>{ "#huxiao-tar" } },
                { "yongsi_jx", new List<string>{ "#yongsi-max" } },
                { "weidi_jx", new List<string>{ "#weidi-remove" } },
                { "shanjia", new List<string>{ "#shanjia-clear" } },
                { "fanghun", new List<string>{ "#fanghun-clear" } },
                { "yuhua", new List<string>{ "#yuhua-max" } },
                { "linglong", new List<string>{ "#linglong-max", "#linglong-tar", "#linglongvh" } },
                { "xingwu", new List<string>{ "#xingwu-clear" } },
                { "yicong", new List<string>{ "#yicong" } },
                { "weikui", new List<string>{ "#weikui-dis" } },
                { "meibu", new List<string>{ "#meibu-dis" } },
                { "moukui", new List<string>{ "#moukui" } },
                { "jujia", new List<string>{ "#jujia-max" } },
                { "xionghuo", new List<string>{ "#xionghuo", "#xionghuo-max", "#xionghuo-prohibit" } },
                { "tuifeng", new List<string>{ "#tuifeng", "#tuifeng-tar" } },
                { "falu", new List<string>{ "#falu" } },
            };
        }
    }

    public class Shefu : TriggerSkill
    {
        public Shefu() : base("shefu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.EventPhaseStart, TriggerEvent.JinkEffect };
            view_as_skill = new ShefuVS();
            skill_type = SkillType.Wizzard;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == Player.PlayerPhase.Finish && base.Triggerable(player, room)
                && !player.IsKongcheng() && ShefuVS.GuhuoCards(room, player).Count > 0)
            {
                triggers.Add(new TriggerStruct(Name, player));
            }
            else if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && use.Card != null && use.To.Count > 0 && use.IsHandcard)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is SkillCard) return triggers;

                List<Player> chengyus = RoomLogic.FindPlayersBySkillName(room, Name);
                string card_name = fcard.Name;
                if (fcard is Slash) card_name = Slash.ClassName;
                foreach (Player p in chengyus)
                {
                    if (p != player && p.Phase == Player.PlayerPhase.NotActive && p.ContainsTag(string.Format("shefu_{0}", card_name))
                        && p.GetTag(string.Format("shefu_{0}", card_name)) is int id && p.GetPile("ambush").Contains(id))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }
            else if (triggerEvent == TriggerEvent.JinkEffect && data is CardResponseStruct resp && resp.Handcard)
            {
                List<Player> chengyus = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in chengyus)
                {
                    if (p != player && p.Phase == Player.PlayerPhase.NotActive && p.ContainsTag("shefu_Jink")
                        && p.GetTag("shefu_Jink") is int id && p.GetPile("ambush").Contains(id))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player p, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
                room.AskForUseCard(player, "@@shefu", "@shefu", -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
            else
            {
                string card_name;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                {
                    if (use.To.Count == 0) return new TriggerStruct();
                    FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                    card_name = fcard.Name;
                    if (fcard is Slash) card_name = Slash.ClassName;
                }
                else
                    card_name = Jink.ClassName;

                string key = string.Format("shefu_{0}", card_name);
                if (p.ContainsTag(key) && p.GetTag(key) is int id && p.GetPile("ambush").Contains(id))
                {
                    room.SetTag("shefu_data", data);
                    List<int> ids = room.AskForExchange(p, Name, 1, 0, string.Format("@shefu-cancel:::{0}", card_name),
                        "ambush", string.Format("{0}|.|.|ambush", id.ToString()), info.SkillPosition);
                    room.RemoveTag("shefu_data");
                    if (ids.Count == 1)
                    {
                        p.RemoveTag(key);
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, p, Name, info.SkillPosition);
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_PILE, p.Name, string.Empty, Name, string.Empty)
                        {
                            General = gsk
                        };
                        room.MoveCardTo(room.GetCard(id), p, null, Player.Place.DiscardPile, string.Empty, reason, true);

                        room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#ShefuEffect",
                    From = ask_who.Name,
                    To = new List<string> { player.Name },
                    Arg = Name,
                    Arg2 = use.Card.Name
                };
                room.SendLog(log);

                List<Player> targets = new List<Player>(use.To);
                foreach (Player p in targets)
                    room.CancelTarget(ref use, p);

                data = use;
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#ShefuEffect",
                    From = ask_who.Name,
                    To = new List<string> { player.Name },
                    Arg = Name,
                    Arg2 = Jink.ClassName
                };
                room.SendLog(log);
                return true;
            }

            return false;
        }
    }

    public class ShefuClear : DetachEffectSkill
    {
        public ShefuClear() : base("shefu", "ambush") { }
    }

    public class ShefuVS : ViewAsSkill
    {
        public ShefuVS() : base("shefu")
        {
            response_pattern = "@@shefu";
        }

        public override GuhuoType GetGuhuoType() => GuhuoType.PopUpBox;

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count == 0 && room.GetCardPlace(to_select.Id) == Player.Place.PlaceHand;
        }
        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            foreach (string name in GuhuoCards(room, player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(name);
                if (fcard is Slash && name != Slash.ClassName) continue;
                if (player.ContainsTag(string.Format("shefu_{0}", name))) continue;

                WrappedCard card = new WrappedCard(name);
                card.AddSubCards(cards);
                result.Add(card);
            }

            return result;
        }

        public static List<string> GuhuoCards(Room room, Player player)
        {
            List<string> guhuos = GetGuhuoCards(room, "btd");
            List<string> result = new List<string>();
            foreach (string name in guhuos)
            {
                FunctionCard fcard = Engine.GetFunctionCard(name);
                if (fcard is Slash && name != Slash.ClassName) continue;
                if (fcard is Nullification && name != Nullification.ClassName) continue;
                if (player.ContainsTag(string.Format("shefu_{0}", name))) continue;

                result.Add(name);
            }

            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].SubCards.Count == 1)
            {
                WrappedCard shefu = new WrappedCard(ShefuCard.ClassName) { Skill = Name, UserString = cards[0].Name };
                shefu.AddSubCards(cards);
                return shefu;
            }

            return null;
        }
    }

    public class ShefuCard : SkillCard
    {
        public static string ClassName = "ShefuCard";
        public ShefuCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = false;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, card_use.From, "shefu", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("shefu", "male", 1, gsk.General, gsk.SkinId);

            int id = card_use.Card.GetEffectiveId();
            string card_name = card_use.Card.UserString;
            room.AddToPile(card_use.From, "ambush", id, false);
            card_use.From.SetTag(string.Format("shefu_{0}", card_name), id);

            LogMessage log = new LogMessage
            {
                Type = "$ShefuRecord",
                From = card_use.From.Name,
                Card_str = id.ToString(),
                Arg = card_name
            };
            room.SendLog(log, card_use.From);
        }
    }
    public class Benyu : MasochismSkill
    {
        public Benyu() : base("benyu")
        {
            //view_as_skill = new BenyuVS();
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                if (damage.From.HandcardNum > player.HandcardNum && player.HandcardNum < 5)
                    return new TriggerStruct(Name, player);
                else if (player.HandcardNum > damage.From.HandcardNum)
                {
                    int count = 0;
                    foreach (int id in player.GetCards("h"))
                        if (RoomLogic.CanDiscard(room, player, player, id)) count++;
                    if (count > damage.From.HandcardNum)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                bool invoke = false;
                if (damage.From.HandcardNum > player.HandcardNum && player.HandcardNum < 5)
                {
                    if (room.AskForSkillInvoke(player, Name, "@benyu-draw", info.SkillPosition))
                    {
                        room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                        player.SetTag("benyu_type", true);
                        invoke = true;
                    }
                }
                else if (player.HandcardNum > damage.From.HandcardNum)
                {
                    int count = 0;
                    foreach (int id in player.GetCards("h"))
                        if (RoomLogic.CanDiscard(room, player, player, id)) count++;
                    if (count > damage.From.HandcardNum)
                    {
                        room.SetTag(Name, data);
                        invoke = room.AskForDiscard(player, Name, damage.From.HandcardNum + 1, damage.From.HandcardNum + 1, true, false, string.Format("@benyu::{0}:{1}", damage.From.Name, damage.From.HandcardNum + 1), true, info.SkillPosition);
                        room.RemoveTag(Name);

                        if (invoke)
                        {
                            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                            player.SetTag("benyu_type", false);
                        }
                    }
                }
                if (invoke) return info;
            }

            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            bool type = (bool)target.GetTag("benyu_type");
            target.RemoveTag("benyu_type");
            if (type)
            {
                int count = Math.Min(5 - target.HandcardNum, damage.From.HandcardNum - target.HandcardNum);
                if (count > 0)
                    room.DrawCards(target, count, Name);
            }
            else if (damage.From.Alive)
                room.Damage(new DamageStruct(Name, target, damage.From));
        }
    }

    //sp关羽
    public class Danji : PhaseChangeSkill
    {
        public Danji() : base("danji")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Attack;
        }
        public override bool Triggerable(Player player, Room room)
        {
            foreach (Player p in room.GetAlivePlayers())
                if (p.GetRoleEnum() == PlayerRole.Lord && p.Name.Contains("liubei")) return false;

            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Start
                    && player.HandcardNum > player.Hp && player.GetMark(Name) == 0;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetPlayerMark(player, Name, 1);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            return info;
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.LoseMaxHp(player);
            List<string> skills = new List<string>();
            if (info.SkillPosition == "head")
            {
                skills.Add("mashu_guanyu_sp");
                skills.Add("nuzhan");
            }
            else
            {
                skills.Add("mashu_guanyu_sp!");
                skills.Add("nuzhan!");
            }
            room.HandleAcquireDetachSkills(player, skills);

            return false;
        }
    }

    public class Nuzhan : TriggerSkill
    {
        public Nuzhan() : base("nuzhan")
        {
            events.Add(TriggerEvent.ConfirmDamage);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.Card != null && !damage.Chain && !damage.Transfer && damage.ByUser)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && damage.Card.IsVirtualCard() && damage.Card.SubCards.Count == 1)
                {
                    if (Engine.GetFunctionCard(room.GetCard(damage.Card.GetEffectiveId()).Name) is EquipCard)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
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

    public class NuzhanTM : TargetModSkill
    {
        public NuzhanTM() : base("#nuzhan")
        {
        }
        public override bool IgnoreCount(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "nuzhan") && Engine.MatchExpPattern(room, pattern, from, card)
                    && card.IsVirtualCard() && card.SubCards.Count == 1)
            {
                if (Engine.GetFunctionCard(room.GetCard(card.GetEffectiveId()).Name) is TrickCard)
                    return true;
            }

            return false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            if (type == ModType.History)
                index = 1;
            else
                index = -1;
        }
    }


    public class YuanhuViewAsSkill : OneCardViewAsSkill
    {
        public YuanhuViewAsSkill() : base("yuanhu")
        {
            filter_pattern = "EquipCard";
            response_pattern = "@@yuanhu";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard(HuyuanCard.ClassName) { Skill = Name, Mute = true };
            first.AddSubCard(card);
            return first;
        }
    }

    public class Yuanhu : PhaseChangeSkill
    {
        public Yuanhu() : base("yuanhu")
        {
            view_as_skill = new YuanhuViewAsSkill();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && !player.IsNude()
                ? new TriggerStruct(Name, player)
                : new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.RemoveTag("huyuan_equip");
            player.RemoveTag("huyuan_target");
            WrappedCard card = room.AskForUseCard(player, "@@yuanhu", "@huyuan-equip", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null && player.ContainsTag("huyuan_target"))
            {
                player.SetMark(Name, card.GetEffectiveId());
                return info;
            }

            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player caohong, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)caohong.GetTag("huyuan_target"));
            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(caohong.GetMark(Name)).Name);

            if (fcard != null)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, caohong, Name, info.SkillPosition);
                if (fcard is Weapon)
                {
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (RoomLogic.DistanceTo(room, target, p) == 1 && RoomLogic.CanDiscard(room, caohong, p, "he"))
                            targets.Add(p);
                    }
                    if (targets.Count > 0)
                    {
                        Player to_dismantle = room.AskForPlayerChosen(caohong, targets, Name, "@huyuan-discard:" + target.Name, true, false, info.SkillPosition);
                        if (to_dismantle != null)
                        {
                            int card_id = room.AskForCardChosen(caohong, to_dismantle, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                            room.ThrowCard(card_id, to_dismantle, caohong);
                        }
                    }
                }
                else if (fcard is Armor && target != null && target.Alive)
                {
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.DrawCards(target, new DrawCardStruct(1, caohong, Name));
                }
                else if (fcard is Horse && target != null && target.Alive && target.IsWounded())
                {
                    room.BroadcastSkillInvoke(Name, "male", 3, gsk.General, gsk.SkinId);
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = caohong
                    };
                    room.Recover(target, recover, true);
                }
            }

            return false;
        }
    }

    //陈琳
    public class BifaCard : SkillCard
    {
        public static string ClassName = "BifaCard";
        public BifaCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.GetPile("bifa").Count == 0 && to_select != Self;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1 && card.SubCards.Count == 1;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            card_use.From.SetTag("bifa_target", target.Name);
        }
    }
    public class BifaViewAsSkill : OneCardViewAsSkill
    {
        public BifaViewAsSkill() : base("bifa")
        {
            filter_pattern = ".";
            response_pattern = "@@bifa";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard bf = new WrappedCard(BifaCard.ClassName) { Skill = Name };
            bf.AddSubCard(card);
            return bf;
        }
    }

    public class Bifa : TriggerSkill
    {
        public Bifa() : base("bifa")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            view_as_skill = new BifaViewAsSkill();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == Player.PlayerPhase.RoundStart && player.GetPile("bifa").Count > 0)
            {
                int card_id = player.GetPile("bifa")[0];
                Player chenlin = room.FindPlayer(player.GetTag(string.Format("BifaSource{0}", card_id)).ToString());
                player.RemoveTag(string.Format("BifaSource{0}", card_id));
                List<int> ids = new List<int>
                {
                    card_id
                };

                LogMessage log = new LogMessage
                {
                    Type = "$BifaView",
                    From = player.Name,
                    Card_str = card_id.ToString(),
                    Arg = Name
                };
                room.SendLog(log, player);

                room.FillAG(Name, ids, player);
                FunctionCard cd = Engine.GetFunctionCard(room.GetCard(card_id).Name);
                string pattern = string.Empty;
                if (cd is BasicCard)
                    pattern = "BasicCard";
                else if (cd is TrickCard)
                    pattern = "TrickCard";
                else if (cd is EquipCard)
                    pattern = "EquipCard";
                pattern += "|.|.|hand";
                List<int> to_give = new List<int>();
                if (!player.IsKongcheng() && chenlin != null && chenlin.Alive)
                {
                    to_give = room.AskForExchange(player, Name, 1, 0, "@bifa-give:" + chenlin.Name, string.Empty, pattern, string.Empty);
                    if (to_give.Count == 1)
                    {
                        CardMoveReason reasonG = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, chenlin.Name, Name, string.Empty);
                        room.ObtainCard(chenlin, ref to_give, reasonG, false);
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty);
                        List<int> card_ids = new List<int> { card_id };
                        room.ObtainCard(player, ref card_ids, reason, false);
                    }
                }

                room.ClearAG(player);
                if (to_give.Count == 0)
                {
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_PILE, string.Empty, Name, string.Empty);
                    List<int> dis = player.GetPile("bifa");
                    room.ThrowCard(ref dis, reason, null);
                    room.LoseHp(player);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Finish && !player.IsKongcheng())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard card = room.AskForUseCard(player, "@@bifa", "@bifa-remove", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null)
            {
                player.SetMark(Name, card.GetEffectiveId());
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer(player.GetTag("bifa_target").ToString());
            player.RemoveTag("bifa_target");
            if (target != null && target.Alive)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                int id = player.GetMark(Name);
                target.SetTag(string.Format("BifaSource{0}", id), player.Name);
                room.AddToPile(target, Name, id, false);
            }

            return false;
        }
    }

    public class SongciCard : SkillCard
    {
        public static string ClassName = "SongciCard";
        public SongciCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.GetMark(string.Format("songci_{0}", Self.Name)) == 0 && to_select.HandcardNum != to_select.Hp;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            int handcard_num = effect.To.HandcardNum;
            int hp = effect.To.Hp;
            room.SetPlayerMark(effect.To, "@songci", 1);
            effect.To.SetMark(string.Format("songci_{0}", effect.From.Name), 1);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, effect.From, "songci", effect.Card.SkillPosition);
            if (handcard_num > hp)
            {
                room.BroadcastSkillInvoke("songci", "male", 2, gsk.General, gsk.SkinId);
                room.AskForDiscard(effect.To, "songci", 2, 2, false, true);
            }
            else if (handcard_num < hp)
            {
                room.BroadcastSkillInvoke("songci", "male", 1, gsk.General, gsk.SkinId);
                room.DrawCards(effect.To, new DrawCardStruct(2, effect.From, "songci"));
            }
        }
    }

    public class Songci : ZeroCardViewAsSkill
    {
        public Songci() : base("songci")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            foreach (Player sib in room.GetAlivePlayers())
                if (sib.GetMark("songci_" + player.Name) == 0 && sib.HandcardNum != sib.Hp)
                    return true;
            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard c = new WrappedCard(SongciCard.ClassName) { Skill = Name, Mute = true };
            return c;
        }
    }


    public class Xianfu : TriggerSkill
    {
        public Xianfu() : base("xianfu")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.Damaged, TriggerEvent.HpRecover };
            skill_type = SkillType.Wizzard;
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.Damaged && player.Alive && player.ContainsTag(Name))
            {
                List<string> xizhicai = (List<string>)player.GetTag(Name);
                foreach (string name in xizhicai)
                {
                    Player p = room.FindPlayer(name);
                    if (p != null) triggers.Add(new TriggerStruct(Name, p));
                }
            }
            else if (triggerEvent == TriggerEvent.HpRecover && player.ContainsTag(Name))
            {
                List<string> xizhicai = (List<string>)player.GetTag(Name);
                foreach (string name in xizhicai)
                {
                    Player p = room.FindPlayer(name);
                    if (p != null && p.IsWounded()) triggers.Add(new TriggerStruct(Name, p));
                }
            }
            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Random ra = new Random();
            if (triggerEvent == TriggerEvent.GameStart)
            {
                Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@xianfu-target", false, false, info.SkillPosition);
                List<string> froms = target.ContainsTag(Name) ? (List<string>)target.GetTag(Name) : new List<string>();
                if (!froms.Contains(player.Name))
                    froms.Add(player.Name);
                target.SetTag(Name, froms);

                List<string> arg = new List<string>
                {
                    target.Name,
                    "@fu",
                    "1"
                };
                room.DoNotify(room.GetClient(player), CommandType.S_COMMAND_SET_MARK, arg);

                room.NotifySkillInvoked(player, Name);
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                int result = ra.Next(1, 3);
                room.BroadcastSkillInvoke(Name, "male", result, general.General, general.SkinId);
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && ask_who.Alive)
            {
                if (player.GetMark("@fu") == 0)
                    room.SetPlayerMark(player, "@fu", 1);

                room.NotifySkillInvoked(ask_who, Name);
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                int result = ra.Next(3, 5);
                room.BroadcastSkillInvoke(Name, "male", result, general.General, general.SkinId);

                room.Damage(new DamageStruct(Name, null, ask_who, damage.Damage));
            }
            else if (triggerEvent == TriggerEvent.HpRecover && data is RecoverStruct recover && ask_who.IsWounded())
            {
                room.NotifySkillInvoked(ask_who, Name);
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                int result = ra.Next(5, 7);
                room.BroadcastSkillInvoke(Name, "male", result, general.General, general.SkinId);

                RecoverStruct _recover = new RecoverStruct
                {
                    Recover = recover.Recover,
                    Who = player
                };
                room.Recover(ask_who, _recover, true);
            }
            return false;
        }
    }

    public class Chouce : TriggerSkill
    {
        public Chouce() : base("chouce")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged };
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
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                PlayAnimation = false,
                Who = player,
                Reason = Name
            };
            room.Judge(ref judge);

            if (WrappedCard.IsBlack(judge.Card.Suit))
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if (RoomLogic.CanDiscard(room, player, p, "hej"))
                        targets.Add(p);

                if (targets.Count > 0)
                {
                    player.SetFlags(Name);
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@chouce-discard", false, true, info.SkillPosition);
                    player.SetFlags("-chouce");

                    int id = room.AskForCardChosen(player, target, "hej", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(id, room.GetCardPlace(id) != Place.PlaceDelayedTrick ? target : null, player != target ? player : null);
                }
            }
            else
            {
                Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@chouce-draw", false, true, info.SkillPosition);
                DrawCardStruct draw = new DrawCardStruct(1, player, Name);
                if (target.ContainsTag("xianfu") && target.GetTag("xianfu") is List<string> names && names.Contains(player.Name))
                {
                    if (target.GetMark("@fu") == 0)
                        room.SetPlayerMark(target, "@fu", 1);
                    draw.Draw = 2;
                }

                room.DrawCards(target, draw);
            }

            return false;
        }
    }

    public class Chenqing : TriggerSkill
    {
        public Chenqing() : base("chenqing")
        {
            skill_type = SkillType.Recover;
            events = new List<TriggerEvent> { TriggerEvent.AskForPeaches };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DyingStruct dying && room.Round > player.GetMark(Name))
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (p != dying.Who)
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DyingStruct dying && room.Round > player.GetMark(Name))
            {
                List<Player> targets = room.GetOtherPlayers(player);
                targets.Remove(dying.Who);
                room.SetTag(Name, data);
                Player target = room.AskForPlayerChosen(player, targets, Name, "@chenqing:" + dying.Who.Name, true, true, info.SkillPosition);
                room.RemoveTag(Name);
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
            player.SetMark(Name, room.Round);
            if (data is DyingStruct dying)
            {
                Player target = room.FindPlayer((string)player.GetTag(Name));
                player.RemoveTag(Name);

                room.DrawCards(target, new DrawCardStruct(4, player, Name));
                List<int> ids = room.AskForExchange(target, Name, 4, 4, "@chenqing-discard:" + dying.Who.Name, string.Empty, "..!", string.Empty);
                bool heal = false;
                if (ids.Count == 4)
                {
                    heal = true;
                    for (int i = 0; i < ids.Count; i++)
                    {
                        WrappedCard.CardSuit suit = room.GetCard(ids[i]).Suit;
                        for (int j = i + 1; j < ids.Count; j++)
                        {
                            if (suit == room.GetCard(ids[j]).Suit)
                            {
                                heal = false;
                                break;
                            }
                        }

                        if (!heal) break;
                    }
                }
                if (ids.Count > 0)
                    room.ThrowCard(ref ids, target);

                if (heal)
                {
                    WrappedCard peach = new WrappedCard(Peach.ClassName) { Skill = "_chenqing" };
                    if (RoomLogic.IsProhibited(room, target, dying.Who, peach) == null)
                    {
                        Thread.Sleep(700);
                        room.UseCard(new CardUseStruct(peach, target, dying.Who));
                    }
                }
            }

            return false;
        }
    }

    public class Moshi : TriggerSkill
    {
        public Moshi() : base("moshi")
        {
            skill_type = SkillType.Alter;
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseChanging, TriggerEvent.CardResponded };
            view_as_skill = new MoshiVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && player.Phase == PlayerPhase.Play && data is CardUseStruct use
                && (!player.ContainsTag(Name) || ((List<string>)player.GetTag(Name)).Count < 2))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if ((fcard is BasicCard || fcard is TrickCard) && !(fcard is DelayedTrick))
                {
                    List<string> cards = player.ContainsTag(Name) ? (List<string>)player.GetTag(Name) : new List<string>();
                    cards.Add(use.Card.Name);
                    player.SetTag(Name, cards);
                }
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && player.Phase == PlayerPhase.Play
                && (!player.ContainsTag(Name) || ((List<string>)player.GetTag(Name)).Count < 2))
            {
                FunctionCard fcard = Engine.GetFunctionCard(resp.Card.Name);
                if (fcard is BasicCard)
                {
                    List<string> cards = player.ContainsTag(Name) ? (List<string>)player.GetTag(Name) : new List<string>();
                    cards.Add(resp.Card.Name);
                    player.SetTag(Name, cards);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish
                && player.ContainsTag(Name) && !player.IsKongcheng())
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.GetTag(Name) is List<string> cards)
            {
                while (cards.Count > 0 && !player.IsKongcheng())
                {
                    string card_name = cards[0];
                    room.AskForUseCard(player, "@@moshi", string.Format("@moshi:::{0}", card_name), -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
                    cards.RemoveAt(0);
                }
            }

            player.RemoveTag(Name);
            
            return false;
        }
    }

    public class MoshiVS : ViewAsSkill
    {
        public MoshiVS() : base("moshi")
        {
            response_pattern = "@@moshi";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count == 0 && room.GetCardPlace(to_select.Id) == Place.PlaceHand
                && !RoomLogic.IsCardLimited(room, player, to_select, FunctionCard.HandlingMethod.MethodUse, true);
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (cards.Count == 1 && player.GetTag(Name) is List<string> strs)
            {
                string card_name = strs[0];
                WrappedCard card = new WrappedCard(card_name) { Skill = Name };
                card.AddSubCards(cards);
                card = RoomLogic.ParseUseCard(room, card);
                result.Add(card);
            }

            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
                return cards[0];

            return null;
        }
    }

    public class ShanjiaVS : ViewAsSkill
    {
        public ShanjiaVS() : base("shanjia")
        {
            response_pattern = "@@shanjia";
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
                return new List<WrappedCard> { new WrappedCard(Slash.ClassName) { Skill = "_shanjia" } };

            return new List<WrappedCard>();
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
                return cards[0];

            return null;
        }
    }

    public class Shanjia : TriggerSkill
    {
        public Shanjia() : base("shanjia")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Replenish;
            view_as_skill = new ShanjiaVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && move.From_places.Contains(Place.PlaceEquip) && move.From.GetMark(Name) < 3
                    && (move.To != move.From || move.To_place == Place.PlaceSpecial || move.To_place == Place.PlaceTable))
                {
                    int count = move.From.GetMark(Name);
                    foreach (Place place in move.From_places)
                        if (place == Place.PlaceEquip)
                            count++;

                    count = Math.Min(3, count);
                    move.From.SetMark(Name, count);
                    if (base.Triggerable(move.From, room))
                        room.SetPlayerStringMark(move.From, "shanjia_losed", count.ToString());
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
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, 3, Name);
            int count = Math.Max(0, 3 - player.GetMark(Name));
            int hand = 0;

            List<int> ids = new List<int>(), all = new List<int>();
            if (count > 0)
            {
                foreach (int id in player.GetCards("he"))
                {
                    if (RoomLogic.CanDiscard(room, player, player, id))
                    {
                        all.Add(id);
                        if (room.GetCardPlace(id) == Place.PlaceHand) hand++;
                    }
                }

                if (all.Count <= count)
                {
                    ids = all;
                    if (hand < player.HandcardNum)
                        room.ShowAllCards(player, null);
                }
                else
                    ids = room.AskForExchange(player, Name, count, count, "@shanjia-discard:::" + count.ToString(), string.Empty, "..!", info.SkillPosition);

                if (ids.Count > 0)
                    room.ThrowCard(ref ids, player);
            }

            if (player.Alive)
            {
                bool check = true;
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is BasicCard || fcard is TrickCard)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                    room.AskForUseCard(player, "@@shanjia", "@shanjia-slash", -1, HandlingMethod.MethodUse, false, info.SkillPosition);
            }

            return false;
        }
    }

    public class ShanjiaDetach : DetachEffectSkill
    {
        public ShanjiaDetach() : base("shanjia", string.Empty) { }

        public override void OnSkillDetached(Room room, Player player, object data)
        {
            room.RemovePlayerStringMark(player, "shanjia_losed");
        }
    }

    public class Qizhi : TriggerSkill
    {
        public Qizhi() : base("qizhi")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                room.RemovePlayerStringMark(player, Name);
                player.SetMark(Name, 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && base.Triggerable(player, room) && player.Phase != PlayerPhase.NotActive)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is BasicCard || fcard is TrickCard)
                {
                    foreach (Player p in room.GetAlivePlayers())
                        if (!use.To.Contains(p) && !p.IsNude() && RoomLogic.CanDiscard(room, player, p, "he"))
                            return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if (!use.To.Contains(p) && !p.IsNude())
                        targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@qizhi", true, true, info.SkillPosition);
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
            Player target = room.FindPlayer(player.GetTag(Name).ToString());
            player.RemoveTag(Name);
            int id = -1;
            if (player == target)
                id = room.AskForExchange(player, Name, 1, 1, "@qizhi-discard", string.Empty, "..!", info.SkillPosition)[0];
            else
                id = room.AskForCardChosen(player, target, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
            room.ThrowCard(id, target, player != target ? player : null);

            if (target.Alive)
                room.DrawCards(target, new DrawCardStruct(1, player, Name));

            if (player.Alive)
            {
                player.AddMark(Name);
                room.SetPlayerStringMark(player, Name, player.GetMark(Name).ToString());
            }

            return false;
        }
    }

    public class Jinqu : PhaseChangeSkill
    {
        public Jinqu() : base("jinqu") { }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return player.Phase == PlayerPhase.Finish && base.Triggerable(player, room) ? new TriggerStruct(Name, player) : new TriggerStruct();
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
            room.DrawCards(player, 2, Name);
            int count = player.HandcardNum - player.GetMark("qizhi");
            if (count > 0)
                room.AskForDiscard(player, Name, count, count, false, false, string.Format("@jinqu-discard:::{0}", player.GetMark("qizhi")), false, info.SkillPosition);

            return false;
        }
    }



    public class Gushe : ZeroCardViewAsSkill
    {
        public Gushe() : base("gushe")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng() && player.UsedTimes(GusheCard.ClassName) < 1 + player.GetMark("jici");
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(GusheCard.ClassName) { Skill = Name };
        }
    }

    public class GusheCard : SkillCard
    {
        public static string ClassName = "GusheCard";
        public GusheCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count < 3 && to_select != Self && RoomLogic.CanBePindianBy(room, to_select, Self);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count > 0 && targets.Count <= 3;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            PindianStruct pd = room.PindianSelect(player, card_use.To, "gushe", null);
            
            for (int i = 0; i < card_use.To.Count; i++)
            {
                room.Pindian(ref pd, i);
                Player target = pd.Tos[i];
                List<Player> loser = new List<Player>();

                if (pd.From_number > pd.To_numbers[i])
                    loser.Add(target);
                else if (pd.From_number == pd.To_numbers[i])
                {
                    loser.Add(player);
                    loser.Add(target);
                }
                else
                    loser.Add(player);

                foreach (Player p in loser)
                {
                    if (p.Alive)
                    {
                        if (p == player)
                        {
                            room.AddPlayerMark(player, "@she");
                            if (player.GetMark("@she") >= 7)
                            {
                                room.DoAnimate(AnimateType.S_ANIMATE_ABUSE);
                                Thread.Sleep(4500);

                                for (int x = 0; x < 5; x++)
                                {
                                    room.SetEmotion(player, "lightning2");
                                    Thread.Sleep(250);
                                }
                                Thread.Sleep(1000);

                                player.Hp = 0;
                                room.BroadcastProperty(player, "Hp");
                                room.KillPlayer(player, new DamageStruct());
                            }
                        }

                        if (p.Alive)
                        {
                            string prompt = "@gushe:" + player.Name;
                            if (player == p) prompt = "@gushe-self";
                            if (!player.Alive)
                                prompt = "@gushe-force";
                            bool discard = false;
                            if (!p.IsNude())
                                discard = room.AskForDiscard(p, "gushe", 1, 1, player.Alive, true, prompt, false, card_use.Card.SkillPosition);

                            if (!discard && player.Alive)
                                room.DrawCards(player, new DrawCardStruct(1, p, "gushe"));
                        }
                    }
                }
            }
        }
    }

    public class Jici : TriggerSkill
    {
        public Jici() : base("jici")
        {
            events = new List<TriggerEvent> { TriggerEvent.PindianVerifying, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.PindianVerifying && data is PindianStruct pindian
                && pindian.Reason == "gushe" && base.Triggerable(player, room) && pindian.From_number <= player.GetMark("@she"))
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = true;
            if (data is PindianStruct pindian)
            {
                if (pindian.From_number < player.GetMark("@she"))
                    invoke = room.AskForSkillInvoke(player, Name, "@jici:::" + player.GetMark("@she").ToString(), info.SkillPosition);
                else
                    room.NotifySkillInvoked(player, Name);

                if (invoke)
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", pindian.From_number < player.GetMark("@she") ? 1 : 2, gsk.General, gsk.SkinId);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player sunce, TriggerStruct info)
        {
            PindianStruct pindian = (PindianStruct)data;
            int count = player.GetMark("@she");
            if (pindian.From_number < count)
            {
                pindian.From_number = Math.Min(13, pindian.From_number + count);
                data = pindian;
                LogMessage log = new LogMessage
                {
                    From = player.Name,
                    Type = "#gushe-verify",
                    Arg = pindian.From_number.ToString()
                };
                room.SendLog(log);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    From = player.Name,
                    Type = "#gushe-add",
                    Arg = "gushe"
                };
                room.SendLog(log);

                player.AddMark(Name);
            }

            return false;
        }
    }

    public class Kangkai : TriggerSkill
    {
        public Kangkai() : base("kangkai")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Defense;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName))
            {
                List<Player> caoans = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in caoans)
                    if (p == player || RoomLogic.DistanceTo(room, p, player) == 1)
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && player.Alive && ask_who.Alive && (ask_who == player || RoomLogic.DistanceTo(room, ask_who, player) == 1))
            {
                if (room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
                {
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, 1, Name);
            if (player.Alive && player != ask_who)
            {
                room.SetTag(Name, data);
                player.SetFlags(Name);
                List<int> ids = room.AskForExchange(ask_who, Name, 1, 1, "@kangkai-give:" + player.Name, string.Empty, "..", info.SkillPosition);
                player.SetFlags("-kangkai");
                room.RemoveTag(Name);
                room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, ask_who.Name, player.Name, Name, string.Empty));
                if (ids.Count == 1 && player.Alive)
                {
                    WrappedCard card = room.GetCard(ids[0]);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, player, card))
                        room.AskForUseCard(player, ids[0].ToString(), "@kangkai-use");
                }

                ResultStruct result = ask_who.Result;
                result.Assist++;
                ask_who.Result = result;
            }
            
            return false;
        }
    }

    public class XiaoguoJX : TriggerSkill
    {
        public XiaoguoJX() : base("xiaoguo_jx")
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
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISMANTLE, ask_who.Name, ask_who.Name, Name, null)
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
            WrappedCard card = room.AskForCard(player, Name, ".Equip", "@xiaoguo_jx-discard:" + ask_who.Name, null);
            room.RemoveTag(Name);
            if (card == null)
                room.Damage(new DamageStruct(Name, ask_who, player));
            else if (ask_who.Alive)
                room.DrawCards(ask_who, 1, Name);

            return false;
        }
    }

    public class Zhenwei : TriggerSkill
    {
        public Zhenwei() : base("zhenwei")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirming, TriggerEvent.EventPhaseStart };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAllPlayers(true))
                {
                    if (p.ContainsTag(Name) && p.Alive && p.GetTag(Name) is List<int> ids)
                    {
                        p.RemoveTag(Name);
                        List<int> get = new List<int>();
                        foreach (int id in ids)
                            if (room.GetCardPlace(id) == Place.DiscardPile)
                                get.Add(id);

                        room.ObtainCard(p, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, p.Name, Name, string.Empty));
                    }
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use && use.To.Contains(player) && use.To.Count == 1)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || (fcard is TrickCard && WrappedCard.IsBlack(use.Card.Suit)))
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                        if (p != player && p != use.From && p.Hp > player.Hp && !p.IsNude())
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && use.To.Contains(player) && use.To.Count == 1 && player.Alive && ask_who.Hp > player.Hp && !ask_who.IsNude())
            {
                List<CardUseStruct> datas = room.ContainsTag(Name) ? (List<CardUseStruct>)room.GetTag(Name) : new List<CardUseStruct>();
                datas.Add(use);
                room.SetTag(Name, datas);
                bool invoke = room.AskForDiscard(ask_who, Name, 1, 1, true, true, string.Format("@zhenwei:{0}:{1}:{2}", use.From.Name, player.Name, use.Card.Name), true, info.SkillPosition);
                datas.RemoveAt(datas.Count - 1);
                room.SetTag(Name, datas);

                if (invoke)
                {
                    ResultStruct result = ask_who.Result;
                    result.Assist++;
                    ask_who.Result = result;

                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && use.To.Contains(player) && player.Alive && ask_who.Alive)
            {
                List<string> choices = new List<string> { "nullfy" };
                bool transfer = true;
                if (RoomLogic.IsProhibited(room, use.From, ask_who, use.Card) != null
                    || (use.Card.Name == FireAttack.ClassName && ask_who.IsKongcheng())
                    || (use.Card.Name == IronChain.ClassName && !RoomLogic.CanBeChainedBy(room, ask_who, use.From))
                    || (use.Card.Name == Dismantlement.ClassName && !RoomLogic.CanDiscard(room, use.From, ask_who, "hej"))
                    || (use.Card.Name == Snatch.ClassName && !RoomLogic.CanGetCard(room, use.From, ask_who, "hej"))
                    || (Engine.GetFunctionCard(use.Card.Name) is DelayedTrick && RoomLogic.PlayerContainsTrick(room, ask_who, use.Card.Name))
                    || (use.Card.Name == Collateral.ClassName && !ask_who.GetWeapon()))
                    transfer = false;
                if (transfer) choices.Add("transfer");
                string choice = room.AskForChoice(ask_who, Name, string.Join("+", choices), new List<string> { "@zhenwei-card:::" + use.Card.Name }, data);

                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (choice == "transfer")
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#zhenwei",
                        From = ask_who.Name,
                        To = new List<string> { player.Name },
                        Arg = use.Card.Name
                    };
                    room.SendLog(log);

                    room.DrawCards(ask_who, 1, Name);
                    if (ask_who.Alive)
                    {
                        if (use.To.Contains(ask_who) && use.To.IndexOf(ask_who) > use.To.IndexOf(player))
                        {
                            use.To.Insert(use.To.IndexOf(ask_who), ask_who);
                            use.EffectCount.Insert(use.To.IndexOf(ask_who), fcard.FillCardBasicEffct(room, ask_who));

                            int index = 0, count = use.EffectCount.Count;
                            for (index = 0; index < count; index++)
                            {
                                if (use.EffectCount[index].To == player && !use.EffectCount[index].Triggered)
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
                                if (use.EffectCount[index].To == player && !use.EffectCount[index].Triggered)
                                    break;
                            }
                            use.To.RemoveAt(index);
                            use.EffectCount.RemoveAt(index);

                            use.To.Add(ask_who);
                            use.EffectCount.Add(fcard.FillCardBasicEffct(room, ask_who));
                            room.SortByActionOrder(ref use);
                        }
                        if (fcard is Slash)
                            room.SlashSettlementFinished(player, use.Card);

                        data = use;
                        return true;
                    }
                }
                else
                {
                    if (use.From.Alive)
                    {
                        List<int> ids = use.From.ContainsTag(Name) ? (List<int>)use.From.GetTag(Name) : new List<int>();
                        ids.AddRange(use.Card.SubCards);
                        use.From.SetTag(Name, ids);
                    }

                    room.CancelTarget(ref use, player); // Room::cancelTarget(use, player);
                    data = use;
                    return true;
                }
            }
            return false;
        }
    }

    public class Kunfen : TriggerSkill
    {
        public Kunfen() : base("kunfen")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Masochism;
            frequency = Frequency.Compulsory;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return target.Phase == PlayerPhase.Finish && base.Triggerable(target, room);
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = player.GetMark("fengliang") == 0 || room.AskForSkillInvoke(player, Name, null, info.SkillPosition);
            if (invoke)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct(); 
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.LoseHp(player);
            if (player.Alive) room.DrawCards(player, 2, Name);
            return false;
        }
    }

    public class Fengliang : TriggerSkill
    {
        public Fengliang() : base("fengliang")
        {
            events.Add(TriggerEvent.AskForPeaches);
            frequency = Frequency.Wake;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DyingStruct dying && dying.Who == player && base.Triggerable(player, room) && player.GetMark(Name) == 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            room.LoseMaxHp(player);
            RecoverStruct recover = new RecoverStruct
            {
                Who = player,
                Recover = Math.Min(2, player.MaxHp) - player.Hp
            };
            room.Recover(player, recover, true);

            room.HandleAcquireDetachSkills(player, "tiaoxin_jx", true);

            return false;
        }
    }
    
    public class Weikui : ZeroCardViewAsSkill
    {
        public Weikui() : base("weikui")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(WeikuiCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(WeikuiCard.ClassName) { Skill = Name };
        }
    }

    public class WeikuiCard : SkillCard
    {
        public static string ClassName = "WeikuiCard";
        public WeikuiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && !to_select.IsKongcheng();
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            room.LoseHp(player);

            if (player.Alive && target.Alive && !target.IsKongcheng())
            {
                List<int> ids = target.GetCards("h");
                bool jink = false;
                foreach (int id in ids)
                {
                    if (room.GetCard(id).Name == Jink.ClassName)
                    {
                        jink = true;
                        break;
                    }
                }

                if (!jink)
                {
                    List<int> discard = new List<int>();
                    foreach (int id in ids)
                        if (RoomLogic.CanDiscard(room, player, target, id)) discard.Add(id);

                    target.SetFlags("gongxin_target");
                    int result = room.DoGongxin(player, target, target.GetCards("h"), discard, "weikui", "@weikui:" + target.Name, card_use.Card.SkillPosition);
                    target.SetFlags("-gongxin_target");

                    if (result != -1)
                        room.ThrowCard(result, target, player);
                }
                else
                {
                    room.ShowAllCards(target, player, "weikui");
                    player.SetFlags("WeikuiInvoker");
                    target.SetFlags("WeikuiTarget");

                    WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_weikui" };
                    if (RoomLogic.IsProhibited(room, player, target, slash) == null)
                        room.UseCard(new CardUseStruct(slash, player, target), false);
                }
            }
        }
    }

    public class WeikuiDistance : DistanceSkill
    {
        public WeikuiDistance() : base("#weikui-dis")
        {
        }
        public override int GetFixed(Room room, Player from, Player to)
        {
            if (from.HasFlag("WeikuiInvoker") && to.HasFlag("WeikuiTarget"))
                return 1;
            else
                return 0;
        }
    }
    public class Lizhan : PhaseChangeSkill
    {
        public Lizhan() : base("lizhan")
        {
            skill_type = SkillType.Replenish;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> to_choose = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.IsWounded()) to_choose.Add(p);

            if (to_choose.Count > 0)
            {
                List<Player> choosees = room.AskForPlayersChosen(player, to_choose, Name, 0, to_choose.Count, "@lizhan", true, info.SkillPosition);
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

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            string str = "tuxi_invoke" + player.Name;
            List<Player> targets = room.ContainsTag(str) ? (List<Player>)room.GetTag(str) : new List<Player>();
            room.RemoveTag(str);

            foreach (Player p in targets)
                if (p.Alive) room.DrawCards(p, new DrawCardStruct(1, player, Name));

            return false;
        }
    }



    public class Gongao : TriggerSkill
    {
        public Gongao() : base("gongao")
        {
            events.Add(TriggerEvent.Death);
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            List<Player> caopis = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player caopi in caopis)
                if (caopi != player)
                    triggers.Add(new TriggerStruct(Name, caopi));

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player dying, ref object data, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
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
            }

            return false;
        }
    }
    public class Juyi : PhaseChangeSkill
    {
        public Juyi() : base("juyi")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0 && player.MaxHp > room.Players.Count && player.IsWounded())
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

            int count = player.MaxHp - player.HandcardNum;
            if (count > 0)
                room.DrawCards(player, count, Name);

            if (player.Alive)
                room.HandleAcquireDetachSkills(player, "benghuai|weizhong", true);

            return false;
        }
    }

    public class Weizhong : TriggerSkill
    {
        public Weizhong() : base("weizhong")
        {
            events.Add(TriggerEvent.MaxHpChanged);
            frequency = Frequency.Compulsory;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DrawCards(player, 1, Name);
            return false;
        }
    }

    public class Junbing : TriggerSkill
    {
        public Junbing() : base("junbing")
        {
            events.Add(TriggerEvent.EventPhaseStart);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Phase == PlayerPhase.Finish && player.Alive && player.HandcardNum < 2)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive && player.HandcardNum < 2)
            {
                string prompt = string.Empty;
                if (player != ask_who) prompt = "@junbing:" + ask_who.Name;
                if (room.AskForSkillInvoke(player, Name, string.IsNullOrEmpty(prompt) ? null : prompt, info.SkillPosition))
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
            room.DrawCards(player, new DrawCardStruct(1, ask_who, Name));
            List<int> ids = player.GetCards("h");
            if (ids.Count > 0 && ask_who.Alive && player != ask_who)
            {
                room.ObtainCard(ask_who, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, ask_who.Name, Name, string.Empty), false);
                int count = ids.Count;

                if (count > 0 && ask_who.Alive && player.Alive)
                {
                    List<int> give = room.AskForExchange(ask_who, Name, count, count, string.Format("@junbing-give:{0}::{1}", player.Name, count), string.Empty, "..", info.SkillPosition);
                    room.ObtainCard(player, ref give, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, ask_who.Name, player.Name, Name, string.Empty), false);
                }
            }

            return false;
        }
    }

    public class Quji : ViewAsSkill
    {
        public Quji() : base("quji")
        {
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < player.GetLostHp() && RoomLogic.CanDiscard(room, player, player, to_select.Id);
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.IsWounded() && !player.IsNude() && !player.HasUsed(QujiCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == player.GetLostHp())
            {
                WrappedCard qj = new WrappedCard(QujiCard.ClassName) { Skill = Name };
                qj.AddSubCards(cards);
                return qj;
            }

            return null;
        }
    }

    public class QujiCard : SkillCard
    {
        public static string ClassName = "QujiCard";
        public QujiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count < Self.GetLostHp() && to_select.IsWounded();
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;

            bool black = false;
            foreach (int id in card_use.Card.SubCards)
            {
                if (WrappedCard.IsBlack(room.GetCard(id).Suit))
                {
                    black = true;
                    break;
                }
            }

            foreach (Player p in card_use.To)
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(p, recover, true);
            }

            if (player.Alive && black)
                room.LoseHp(player);
        }
    }

    public class Tuifeng : TriggerSkill
    {
        public Tuifeng() : base("tuifeng")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Masochism;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && !player.IsNude() && data is DamageStruct damage)
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
            List<int> ids = room.AskForExchange(player, Name, 1, 0, "@tuifeng", string.Empty, "..", info.SkillPosition);
            if (ids.Count > 0)
            {
                room.NotifySkillInvoked(player, Name);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
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

    public class TuifengGet : PhaseChangeSkill
    {
        public TuifengGet() : base("#tuifeng") { frequency = Frequency.Compulsory; }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Start && target.GetPile("tuifeng").Count > 0;
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "tuifeng", info.SkillPosition);
            room.BroadcastSkillInvoke("tuifeng", "male", 2, gsk.General, gsk.SkinId);

            List<int> ids = player.GetPile("tuifeng");
            int count = ids.Count;
            player.SetMark("tuifeng", count);

            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            CardsMoveStruct move1 = new CardsMoveStruct(ids, null, Place.DiscardPile, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, player.Name, "tuifeng", string.Empty));
            moves.Add(move1);
            room.MoveCardsAtomic(moves, true);

            if (player.Alive)
                room.DrawCards(player, 2 * count, "tuifeng");

            return false;
        }
    }

    public class TuifengTar : TargetModSkill
    {
        public TuifengTar() : base("#tuifeng-tar", false) { }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            return from.GetMark("tuifeng");
        }
    }

    public class Zhenlue : TriggerSkill
    {
        public Zhenlue() : base("zhenlue")
        {
            frequency = Frequency.Compulsory;
            events = new List<TriggerEvent> { TriggerEvent.TrickCardCanceling };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && base.Triggerable(effect.From, room))
                return new TriggerStruct(Name, effect.From);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return true;
        }
    }

    public class ZhenluePro : ProhibitSkill
    {
        public ZhenluePro() : base("#zhenlue") { }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (card != null && to != null && RoomLogic.PlayerHasShownSkill(room, to, this))
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is DelayedTrick) return true;
            }

            return false;
        }
    }

    public class Jianshu : OneCardViewAsSkill
    {
        public Jianshu() : base("jianshu")
        {
            limit_mark = "@jian";
            frequency = Frequency.Limited;
            filter_pattern = ".|black|.|hand";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(limit_mark) > 0;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            return new WrappedCard(JianshuCard.ClassName);
        }
    }

    public class JianshuCard : SkillCard
    {
        public static string ClassName = "JianshuCard";
        public JianshuCard() : base(ClassName) { will_throw = false; }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (to_select == Self || targets.Count >= 2) return false;
            if (targets.Count == 1) return RoomLogic.CanBePindianBy(room, to_select, targets[0]) && RoomLogic.InMyAttackRange(room, to_select, targets[0]);
            return true;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 2;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;

            room.SetPlayerMark(card_use.From, "@jian", 0);
            room.BroadcastSkillInvoke("jianshu", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "jianshu");

            List<Player> targets = new List<Player>(card_use.To);
            room.SortByActionOrder(ref targets);

            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            log.SetTos(targets);

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;
            room.SendLog(log);

            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player from = card_use.To[0], to = card_use.To[1];
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(from, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, from.Name, "mizhao", string.Empty), false);

            if (!from.IsKongcheng() && RoomLogic.CanBePindianBy(room, to, from))
            {
                PindianStruct pd = room.PindianSelect(from, to, "jianshu");
                room.Pindian(ref pd);
                if (pd.From_number > pd.To_numbers[0])
                {
                    if (from.Alive) room.AskForDiscard(from, "jianshu", 2, 2, false, true, "@jianshu-discard");
                    if (to.Alive) room.LoseHp(to);
                }
                else if (pd.To_numbers[0] > pd.From_number)
                {
                    if (to.Alive) room.AskForDiscard(to, "jianshu", 2, 2, false, true, "@jianshu-discard");
                    if (from.Alive) room.LoseHp(from);
                }
                else
                {
                    if (from.Alive) room.LoseHp(from);
                    if (to.Alive) room.LoseHp(to);
                }
            }
        }
    }

    public class Yongdi : TriggerSkill
    {
        public Yongdi() : base("yongdi")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged };
            skill_type = SkillType.Masochism;
            frequency = Frequency.Limited;
            limit_mark = "@yongdi";
        }
        
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && player.GetMark(limit_mark) > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.IsMale()) targets.Add(p);

            Player target = room.AskForPlayerChosen(player, targets, Name, "@yongdi-give", true, true, info.SkillPosition);
            if (target != null)
            {
                room.SetPlayerMark(player, limit_mark, 0);
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.DoSuperLightbox(player, info.SkillPosition, Name);

                room.SetTag(Name, target);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            if (target.Alive)
            {
                target.MaxHp++;
                room.BroadcastProperty(target, "MaxHp");

                LogMessage log = new LogMessage
                {
                    Type = "$GainMaxHp",
                    From = target.Name,
                    Arg = "1"
                };
                room.SendLog(log);

                room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, target);

                if (target.Alive && target.GetRoleEnum() != PlayerRole.Lord)
                {
                    foreach (string skill_name in Engine.GetGeneralSkills(target.General1, room.Setting.GameMode, true))
                    {
                        Skill s = Engine.GetSkill(skill_name);
                        if (s.LordSkill)
                        {
                            room.AddPlayerSkill(player, skill_name);
                            if (s != null && s.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(s.LimitMark))
                                room.SetPlayerMark(player, s.LimitMark, 1);

                            object _data = new InfoStruct() { Info = skill_name, Head = true };
                            room.RoomThread.Trigger(TriggerEvent.EventAcquireSkill, room, player, ref _data);
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Lingren : TriggerSkill
    {
        public Lingren() : base("lingren")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.RoundStart && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.HandleAcquireDetachSkills(player, "-jianxiong_jx|-xingshang", true);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && player.Phase == PlayerPhase.Play && base.Triggerable(player, room) && !player.HasFlag(Name))
            {
                if (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == FireAttack.ClassName || use.Card.Name == Duel.ClassName
                    || use.Card.Name == SavageAssault.ClassName || use.Card.Name == ArcheryAttack.ClassName)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag(Name, data);
                Player target = room.AskForPlayerChosen(player, use.To, Name, "@lingren", true, true, info.SkillPosition);
                room.RemoveTag(Name);
                if (target != null)
                {
                    player.SetFlags(Name);
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && room.GetTag(Name) is Player target)
            {
                int correct = 0;
                if (target.IsKongcheng())
                {
                    correct = 3;
                }
                else
                {
                    bool basic = false, equip = false, trick = false;
                    foreach (int id in target.GetCards("h"))
                    {
                        FunctionCard card = Engine.GetFunctionCard(room.GetCard(id).Name);
                        switch (card.TypeID)
                        {
                            case CardType.TypeBasic:
                                basic = true;
                                break;
                            case CardType.TypeEquip:
                                equip = true;
                                break;
                            case CardType.TypeTrick:
                                trick = true;
                                break;
                        }
                    }

                    player.SetFlags("lingren_basic");
                    string basic_choice = room.AskForChoice(player, Name, "yes+no", new List<string> { "@lingren-basic:" + target.Name }, target);
                    player.SetFlags("-lingren_basic");

                    if ((basic_choice == "yes" && basic) || (basic_choice != "yes" && !basic))
                        correct++;

                    player.SetFlags("lingren_equip");
                    string equip_choice = room.AskForChoice(player, Name, "yes+no", new List<string> { "@lingren-equip:" + target.Name }, target);
                    player.SetFlags("-lingren_equip");

                    if ((equip_choice == "yes" && equip) || (equip_choice != "yes" && !equip))
                        correct++;

                    player.SetFlags("lingren_trick");
                    string trick_choice = room.AskForChoice(player, Name, "yes+no", new List<string> { "@lingren-trick:" + target.Name }, target);
                    player.SetFlags("-lingren_trick");

                    if ((trick_choice == "yes" && trick) || (trick_choice != "yes" && !trick))
                        correct++;

                    LogMessage log = new LogMessage
                    {
                        Type = "#lingren-result",
                        From = player.Name,
                        To = new List<string> { target.Name },
                        Arg = Name,
                        Arg2 = correct.ToString()
                    };

                    room.SendLog(log);
                }

                if (correct > 0)
                {
                    foreach (CardBasicEffect effect in use.EffectCount)
                        if (effect.To == target) effect.Effect1++;
                    data = use;
                }
                if (correct > 1)
                    room.DrawCards(player, 2, Name);
                if (correct > 2 && player.Alive)
                {
                    player.SetMark(Name, 1);
                    room.HandleAcquireDetachSkills(player, "jianxiong_jx|xingshang", true);
                }
            }

            return false;
        }
    }

    public class Fujian : TriggerSkill
    {
        public Fujian() : base("fujian")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }

        public override bool Triggerable(Player target, Room room)
        {
            if (base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish)
                foreach (Player p in room.GetOtherPlayers(target))
                    if (!p.IsKongcheng()) return true;

            return false;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            int min = 1000, count = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < min) min = p.HandcardNum;

            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum == min) count++;

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.IsKongcheng()) targets.Add(p);
            }

            if (targets.Count > 0)
            {
                Shuffle.shuffle(ref targets);
                Player target = targets[0];

                List<int> ids = target.GetCards("h"), views = new List<int>(); ;
                Shuffle.shuffle(ref ids);
                for (int i = 0; i < Math.Min(count, ids.Count); i++)
                    views.Add(ids[i]);

                room.DoGongxin(player, target, views, new List<int>(), Name, "@fujan:" + target.Name, info.SkillPosition);
            }

            return false;
        }
    }

    public class Tianming : TriggerSkill
    {
        public Tianming() : base("tianming")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    return new TriggerStruct(Name, player);
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
            room.AskForDiscard(player, Name, 2, 2, false, true, "@tianming", false, info.SkillPosition);
            room.DrawCards(player, 2, Name);

            int hp = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.Hp > hp)
                    hp = p.Hp;

            List<Player> players = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.Hp == hp) players.Add(p);

            if (players.Count == 1 && players[0] != player && room.AskForSkillInvoke(players[0], Name, "@tianming-disacard"))
            {
                room.AskForDiscard(players[0], Name, 2, 2, false, true, "@tianming", false, info.SkillPosition);
                room.DrawCards(players[0], 2, Name);
            }

            return false;
        }
    }

    public class Mizhao : ZeroCardViewAsSkill
    {
        public Mizhao() : base("mizhao")
        {
            skill_type = SkillType.Wizzard;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng() && !player.HasUsed(MizhaoCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(MizhaoCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            card.AddSubCards(player.GetCards("h"));
            return card;
        }
    }

    public class MizhaoCard : SkillCard
    {
        public static string ClassName = "MizhaoCard";
        public MizhaoCard() : base(ClassName) { handling_method = HandlingMethod.MethodNone; }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (to_select == Self || targets.Count >= 2) return false; ;
            if (targets.Count == 1) return RoomLogic.CanBePindianBy(room, to_select, targets[0]);
            return true;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 2;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            List<Player> targets = new List<Player>(card_use.To);
            room.SortByActionOrder(ref targets);
            
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            log.SetTos(targets);

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;
            room.SendLog(log);

            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player from = card_use.To[0], to = card_use.To[1];
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(from, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, from.Name, "mizhao", string.Empty), false);

            if (!from.IsKongcheng() && RoomLogic.CanBePindianBy(room, to, from))
            {
                PindianStruct pd = room.PindianSelect(from, to, "mizhao");
                room.Pindian(ref pd);
                WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_mizhao" };
                if (pd.From_number > pd.To_numbers[0])
                {
                    if (RoomLogic.IsProhibited(room, from, to, slash) == null)
                        room.UseCard(new CardUseStruct(slash, from, to));
                }
                else if (pd.To_numbers[0] > pd.From_number)
                {
                    if (RoomLogic.IsProhibited(room, to, from, slash) == null)
                        room.UseCard(new CardUseStruct(slash, to, from));
                }
            }
        }
    }


    public class YongsiJX : TriggerSkill
    {
        public YongsiJX() : base("yongsi_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.EventPhaseChanging, TriggerEvent.EventPhaseStart };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.From != null && damage.From.Alive && damage.From.Phase != PlayerPhase.NotActive)
                damage.From.AddMark(Name, damage.Damage);
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Draw && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Discard && base.Triggerable(player, room))
            {
                if ((player.GetMark(Name) == 0 && player.HandcardNum < player.Hp) || player.GetMark(Name) > 1)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Draw)
            {
                room.DrawCards(player, Fuli.GetKingdoms(room), Name);
                return true;
            }
            else
            {
                if (player.GetMark(Name) == 0 && player.HandcardNum < player.Hp)
                    room.DrawCards(player, player.Hp - player.HandcardNum, Name);
            }

            return false;
        }
    }

    public class YongsiMax : MaxCardsSkill
    {
        public YongsiMax() : base("#yongsi-max") { }

        public override int GetFixed(Room room, Player target)
        {
            if (RoomLogic.PlayerHasShownSkill(room, target, "yongsi_jx") && target.GetMark("yongsi_jx") > 1)
                return target.GetLostHp();

            return -1;
        }
    }

    public class WeidiJXCard : SkillCard
    {
        public static string ClassName = "WeidiJXCard";
        public WeidiJXCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && !to_select.HasFlag("weidi_jx") && to_select != Self && to_select.Kingdom == "qun";
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            card_use.From.SetTag("lirang_target", card_use.To[0].Name);
        }
    }
    public class WeidiViewAsSkill : OneCardViewAsSkill
    {
        public WeidiViewAsSkill() : base("weidi_jx")
        {
            expand_pile = "#weidi_jx";
            response_pattern = "@@weidi_jx";
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return player.GetPile(expand_pile).Contains(to_select.Id);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard Lirang_card = new WrappedCard(WeidiJXCard.ClassName);
            Lirang_card.AddSubCard(card);
            return Lirang_card;
        }
    }

    public class WeidiJX : TriggerSkill
    {
        public WeidiJX() : base("weidi_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Replenish;
            view_as_skill = new WeidiViewAsSkill();
            lord_skill = true;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.Discard
                && move.From == room.Current && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD
                && move.To_place == Place.DiscardPile && base.Triggerable(move.From, room))
            {
                List<int> guzhengToGet = move.From.ContainsTag("WeidiToGet") ? (List<int>)move.From.GetTag("WeidiToGet") : new List<int>();
                foreach (int card_id in move.Card_ids)
                {
                    if (!guzhengToGet.Contains(card_id))
                        guzhengToGet.Add(card_id);
                }

                move.From.SetTag("WeidiToGet", guzhengToGet);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && base.Triggerable(player, room))
            {
                List<int> cardsToGet = player.ContainsTag("WeidiToGet") ? (List<int>)player.GetTag("WeidiToGet") : new List<int>();
                foreach (int id in cardsToGet)
                    if (room.GetCardPlace(id) == Place.DiscardPile)
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> cardsToGet = player.ContainsTag("WeidiToGet") ? (List<int>)player.GetTag("WeidiToGet") : new List<int>();
            List<int> cards = new List<int>();
            foreach (int id in cardsToGet)
                if (room.GetCardPlace(id) == Place.DiscardPile)
                    cards.Add(id);

            List<CardsMoveStruct> lirangs = new List<CardsMoveStruct>();
            while (cards.Count > 0)
            {
                player.PileChange("#" + Name, cards);
                WrappedCard card = room.AskForUseCard(player, "@@weidi_jx", "@weidi-distribute", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
                player.PileChange("#" + Name, cards, false);

                if (card != null && card.SubCards.Count > 0)
                {
                    Player target = room.FindPlayer((string)player.GetTag("lirang_target"), true);
                    target.SetFlags(Name);
                    player.RemoveTag("lirang_target");
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PREVIEWGIVE, player.Name, target.Name, Name, null);
                    CardsMoveStruct moves = new CardsMoveStruct(card.SubCards, target, Place.PlaceHand, reason);
                    lirangs.Add(moves);
                    foreach (int id in card.SubCards)
                        cards.Remove(id);
                }
                else
                    cards.Clear();
            }

            foreach (Player p in room.GetAlivePlayers())
                p.SetFlags("-weidi_jx");

            if (lirangs.Count > 0)
            {
                room.SetTag(Name, lirangs);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            else
                return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            List<CardsMoveStruct> lirangs = (List<CardsMoveStruct>)room.GetTag(Name);
            player.RemoveTag(Name);
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
                    Arg = Name
                };
                l.SetTos(targets);
                room.SendLog(l);

                room.MoveCardsAtomic(moves, true);
            }

            return false;
        }
    }
    public class WeidiRemove : TriggerSkill
    {
        public WeidiRemove() : base("#weidi-remove")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd };
        }
        public override int GetPriority() => -1;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player != null && player.Phase == PlayerPhase.Discard && player.ContainsTag("WeidiToGet"))
                player.RemoveTag("WeidiToGet");
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class LihunVS : OneCardViewAsSkill
    {
        public LihunVS() : base("lihun")
        {
            filter_pattern = "..!";
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return room.GetAlivePlayers().Count > 2
                && RoomLogic.CanDiscard(room, player, player, "he") && !player.HasUsed(LihunCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard lijian_card = new WrappedCard(LihunCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name,
                Mute = true
            };
            lijian_card.AddSubCard(card);
            return lijian_card;
        }
    }

    public class LihunCard : SkillCard
    {
        public static string ClassName = "LihunCard";
        public LihunCard() : base(ClassName)
        {
            will_throw = true;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && to_select.IsMale() && !to_select.IsKongcheng();
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];

            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "lihun", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("lihun", "male", 1, gsk.General, gsk.SkinId);

            room.TurnOver(player);
            List<int> ids = target.GetCards("h");
            room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, "lihun", string.Empty), false);

            player.SetTag("lihun", target.Name);
            player.SetTag("lihun_position", card_use.Card.SkillPosition);
        }
    }

    public class Lihun : TriggerSkill
    {
        public Lihun() : base("lihun")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd };
            skill_type = SkillType.Wizzard;
            view_as_skill = new LihunVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == PlayerPhase.Play && player.ContainsTag(Name))
            {
                Player target = room.FindPlayer(player.GetTag(Name).ToString(), true);
                player.RemoveTag(Name);
                if (player.Alive && !player.IsNude() && target.Alive && target.Hp > 0)
                {
                    List<int> ids = new List<int>();
                    if (player.GetCardCount(true) < target.Hp)
                        ids = player.GetCards("he");
                    else
                        ids = room.AskForExchange(player, Name, target.Hp, target.Hp, string.Format("@lihun:{0}::{1}", target.Name, target.Hp),
                        string.Empty, "..", player.GetTag("lihun_position").ToString());

                    room.NotifySkillInvoked(player, Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, player.GetTag("lihun_position").ToString());
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                    room.ObtainCard(target, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Chongzhen : TriggerSkill
    {
        public Chongzhen() : base("chongzhen")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.CardResponded };
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName)
                && use.Card.Skill == "longdan_jx" && base.Triggerable(player, room))
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in use.To)
                    if (!p.IsKongcheng() && RoomLogic.CanGetCard(room, player, p, "h")) targets.Add(p);

                if (targets.Count > 0)
                    return new TriggerStruct(Name, player, targets);
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Who != null && resp.Card != null
                && (resp.Card.Name.Contains(Slash.ClassName) || resp.Card.Name == Jink.ClassName) && resp.Card.Skill == "longdan_jx"
                && base.Triggerable(player, room) && resp.Who.Alive && !resp.Who.IsKongcheng() && RoomLogic.CanGetCard(room, player, resp.Who, "h"))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            Player player = null;
            if (triggerEvent == TriggerEvent.TargetChosen)
                player = target;
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp)
                player = resp.Who;

            if (player.Alive && ask_who.Alive && !player.IsKongcheng() && RoomLogic.CanGetCard(room, ask_who, player, "h"))
            {
                player.SetFlags(Name);
                bool invoke = room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition);
                player.SetFlags("-chongzhen");

                if (invoke)
                {
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            Player player = null;
            if (triggerEvent == TriggerEvent.TargetChosen)
                player = target;
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp)
                player = resp.Who;

            int id = room.AskForCardChosen(ask_who, player, "h", Name, false, FunctionCard.HandlingMethod.MethodGet);
            room.ObtainCard(ask_who, room.GetCard(id), new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, ask_who.Name, player.Name, Name, string.Empty), false);

            return false;
        }
    }

    public class Jieyuan : TriggerSkill
    {
        public Jieyuan() : base("jieyuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.DamageInflicted };
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From != damage.To && !player.IsKongcheng())
            {
                if ((triggerEvent == TriggerEvent.DamageCaused && (damage.To.Hp >= player.Hp || player.GetMark(PlayerRole.Rebel.ToString()) > 0))
                    || (triggerEvent == TriggerEvent.DamageInflicted && (damage.From.Hp >= player.Hp || player.GetMark(PlayerRole.Loyalist.ToString()) > 0)))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (!player.IsKongcheng() && data is DamageStruct damage)
            {
                List<int> ids = new List<int>();
                if (triggerEvent == TriggerEvent.DamageCaused)
                {
                    string prompt = string.Format("@jieyuan-add:{0}", damage.To.Name);
                    string pattern = ".black!";
                    if (player.GetMark(PlayerRole.Renegade.ToString()) > 0)
                    {
                        pattern = "..!";
                        prompt = string.Format("@jieyuan-add1:{0}", damage.To.Name);
                    }
                    ids = room.AskForExchange(player, Name, 1, 0, prompt, string.Empty, pattern, info.SkillPosition);
                }
                else
                {
                    string prompt = string.Format("@jieyuan-reduce:{0}", damage.From.Name);
                    string pattern = ".red!";
                    if (player.GetMark(PlayerRole.Renegade.ToString()) > 0)
                    {
                        prompt = string.Format("@jieyuan-reduce1:{0}", damage.From.Name);
                        pattern = "..!";
                    }
                    ids = room.AskForExchange(player, Name, 1, 0, prompt, string.Empty, pattern, info.SkillPosition);
                }

                if (ids.Count == 1)
                {
                    room.ThrowCard(ids[0], player, null, Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", triggerEvent == TriggerEvent.DamageCaused ? 2 : 1, gsk.General, gsk.SkinId);
                    room.NotifySkillInvoked(player, Name);

                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
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

            return false;
        }
    }

    public class Fenxin : TriggerSkill
    {
        public Fenxin() : base("fenxin")
        {
            events.Add(TriggerEvent.Death);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            List<Player> caopis = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player caopi in caopis)
                if (caopi != player)
                    triggers.Add(new TriggerStruct(Name, caopi));

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            if (caopi.GetMark(player.GetRoleEnum().ToString()) == 0)
            {
                room.BroadcastSkillInvoke(Name, caopi, Name);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(caopi, Name);

            LogMessage log = new LogMessage
            {
                From = caopi.Name
            };
            switch (player.GetRoleEnum())
            {
                case PlayerRole.Rebel:
                    log.Type = "#fenxin-add";
                    break;
                case PlayerRole.Loyalist:
                    log.Type = "#fenxin-reduce";
                    break;
                case PlayerRole.Renegade:
                    log.Type = "#fenxin-pattern";
                    break;
            }
            room.SendLog(log);

            caopi.AddMark(player.GetRoleEnum().ToString());

            return false;
        }
    }

    public class Zongkui : TriggerSkill
    {
        public Zongkui() : base("zongkui")
        {
            events = new List<TriggerEvent> { TriggerEvent.RoundStart, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Wizzard;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.RoundStart && base.Triggerable(player, room))
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.GetMark("@kui") == 0)
                    {
                        triggers.Add(new TriggerStruct(Name, player));
                        break;
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.RoundStart)
            {
                List<Player> himiko = RoomLogic.FindPlayersBySkillName(room, Name);
                if (himiko.Count > 0)
                {
                    int hp = 100;
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.Hp < hp)
                            hp = p.Hp;

                    foreach (Player p in room.GetAlivePlayers())
                        if (p.Hp == hp && !RoomLogic.PlayerHasSkill(room, p, Name) && p.GetMark("@kui") == 0)
                            targets.Add(p);

                    if (targets.Count > 0)
                        foreach (Player p in himiko)
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                foreach (Player p in room.GetOtherPlayers(ask_who))
                    if (p.GetMark("@kui") == 0)
                        targets.Add(p);
            }
            else
            {
                int hp = 100;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.Hp < hp)
                        hp = p.Hp;

                foreach (Player p in room.GetAlivePlayers())
                    if (p.Hp == hp && !RoomLogic.PlayerHasSkill(room, p, Name) && p.GetMark("@kui") == 0)
                        targets.Add(p);
            }

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(ask_who, targets, Name, "@zongkui", triggerEvent == TriggerEvent.EventPhaseStart, true, info.SkillPosition);
                if (target != null)
                {
                    ask_who.SetTag(Name, target.Name);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer(ask_who.GetTag(Name).ToString());
            ask_who.RemoveTag(Name);
            room.SetPlayerMark(target, "@kui", 1);

            return false;
        }
    }

    public class Guqu : TriggerSkill
    {
        public Guqu() : base("guqu")
        {
            events.Add(TriggerEvent.Damaged);
            skill_type = SkillType.Wizzard;
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && player.GetMark("@kui") > 0)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    public class Baijia : TriggerSkill
    {
        public Baijia() : base("baijia")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Wake;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_DRAW
                && move.Reason.SkillName == "guqu")
                move.To.AddMark("baijia_got");
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start && base.Triggerable(player, room)
                && player.GetMark(Name) == 0 && player.GetMark("baijia_got") >= 7)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetPlayerMark(player, Name, 1);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
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

                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.GetMark("gui") == 0)
                        room.SetPlayerMark(p, "@kui", 1);

                room.HandleAcquireDetachSkills(player, "-guqu", false);
                room.HandleAcquireDetachSkills(player, "canshi", true);
            }

            return false;
        }
    }

    public class Canshi : TriggerSkill
    {
        public Canshi() : base("canshi")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirming, TriggerEvent.CardTargetAnnounced };
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use && use.From != null && base.Triggerable(player, room)
                && use.To.Count == 1 && use.From.GetMark("@kui") > 0)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is BasicCard || (fcard is TrickCard && !(fcard is DelayedTrick) && !(fcard is Nullification)))
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct _use && base.Triggerable(player, room) && _use.Card.ExtraTarget)
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                if (fcard is BasicCard || (fcard is TrickCard && !(fcard is DelayedTrick) && !(fcard is Collateral)))
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (p.GetMark("@kui") > 0 && !_use.To.Contains(p))
                            return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (triggerEvent == TriggerEvent.TargetConfirming)
            {
                room.SetTag(Name, data);
                bool invoke = room.AskForSkillInvoke(ask_who, Name, string.Format("@canshi-cancel:{0}::{1}", use.From.Name, use.Card.Name), info.SkillPosition);
                room.RemoveTag(Name);
                if (invoke)
                {
                    room.SetPlayerMark(use.From, "@kui", 0);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    return info;
                }
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced)
            {
                List<Player> targets = new List<Player>();
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if ((fcard is Peach && !p.IsWounded()) || (fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, player, p))
                        || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && !RoomLogic.CanGetCard(room, player, p, "hej"))
                        || (fcard is Dismantlement && !RoomLogic.CanDiscard(room, player, p, "hej"))) continue;

                    if (p.GetMark("@kui") > 0 && !use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, use.Card) == null)
                        targets.Add(p);
                }

                room.SetTag("extra_target_skill", data);                   //for AI
                List<Player> players = room.AskForPlayersChosen(player, targets, Name, 0, targets.Count, "@canshi-extra:::" + use.Card.Name, true, info.SkillPosition);
                room.RemoveTag("extra_target_skill");
                if (players.Count > 0)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    List<string> names = new List<string>();
                    foreach (Player p in players)
                    {
                        room.SetPlayerMark(p, "@kui", 0);
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                        names.Add(p.Name);
                    }
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        Card_str = RoomLogic.CardToString(room, use.Card),
                        Arg = Name
                    };
                    log.SetTos(players);
                    room.SendLog(log);

                    player.SetTag("extra_targets", names);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (triggerEvent == TriggerEvent.TargetConfirming)
            {
                room.CancelTarget(ref use, player); // Room::cancelTarget(use, player);
                data = use;
                return true;
            }
            else
            {
                List<string> names = (List<string>)player.GetTag("extra_targets");
                player.RemoveTag("extra_targets");
                List<Player> targets = new List<Player>();
                foreach (string name in names)
                    targets.Add(room.FindPlayer(name));

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

    public class Qiaomeng : TriggerSkill
    {
        public Qiaomeng() : base("qiaomeng")
        {
            events.Add(TriggerEvent.Damage);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player panfeng, ref object data, Player ask_who)
        {
            if (base.Triggerable(panfeng, room) && data is DamageStruct damage && damage.Card != null)
            {
                Player target = damage.To;
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && target.HasEquip() && !target.HasFlag("Global_DFDebut"))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (target.GetEquip(i) < 0) continue;
                        if (RoomLogic.CanDiscard(room, panfeng, target, target.GetEquip(i)))
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
                room.BroadcastSkillInvoke(Name, panfeng, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player panfeng, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            Player target = damage.To;

            int card_id = room.AskForCardChosen(panfeng, target, "e", Name, false, HandlingMethod.MethodDiscard);
            List<int> ids = new List<int> { card_id };
            room.ThrowCard(ref ids, target, panfeng);
            if (ids.Count == 1)
            {
                card_id = ids[0];
                if (panfeng.Alive && room.GetCardPlace(card_id) == Place.DiscardPile && Engine.GetFunctionCard(room.GetCard(card_id).Name) is Horse)
                    room.ObtainCard(panfeng, card_id);
            }

            return false;
        }
    }

    public class Yicong : TriggerSkill
    {
        public Yicong() : base("yicong")
        {
            events.Add(TriggerEvent.HpChanged);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && ((player.GetMark(Name) == 0 && player.Hp <= 2) || (player.Hp > 2 && player.GetMark(Name) > 0)))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            if (player.Hp > 2)
            {
                player.SetMark(Name, 0);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
            }
            else
            {
                player.SetMark(Name, 1);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
            }

            return false;
        }
    }

    public class YicongDis : DistanceSkill
    {
        public YicongDis() : base("#yicong")
        {
        }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            int count = 0;
            if (RoomLogic.PlayerHasShownSkill(room, from, this) && from.Hp > 2)
                count--;

            if (RoomLogic.PlayerHasShownSkill(room, to, this) && to.Hp <= 2)
                count++;

            return count;
        }
    }

    public class Jiqiao : TriggerSkill
    {
        public Jiqiao() : base("jiqiao")
        {
            events.Add(TriggerEvent.EventPhaseStart);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Play && !player.IsNude())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.AskForExchange(player, Name, 200, 0, "@jiqiao", string.Empty, ".Equip!", info.SkillPosition);
            if (ids.Count > 0)
            {
                room.ThrowCard(ref ids, player, null, Name);
                if (ids.Count > 0)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    player.SetMark(Name, ids.Count);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = player.GetMark(Name) * 2;
            player.SetMark(Name, 0);

            List<int> card_ids = room.GetNCards(count);
            List<int> gets = new List<int>(), drops = new List<int>();

            foreach (int id in card_ids)
            {
                if (Engine.GetFunctionCard(room.GetCard(id).Name).TypeID == CardType.TypeTrick)
                    gets.Add(id);
                else
                    drops.Add(id);
                room.MoveCardTo(room.GetCard(id), player, Place.PlaceTable, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TURNOVER, player.Name, Name, null), false);
                Thread.Sleep(400);
            }
            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(gets, player, Place.PlaceHand, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTBACK, player.Name, Name, null)) },
                true);
            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(drops, null, Place.DiscardPile, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, null, Name, null)) },
                true);

            return false;
        }
    }

    public class Linglong : TriggerSkill
    {
        public Linglong() : base("linglong")
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

    public class LinglongVH : ViewHasSkill
    {
        public LinglongVH() : base("#linglongvh")
        {
            viewhas_armors.Add(EightDiagram.ClassName);
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (player.Alive && RoomLogic.PlayerHasSkill(room, player, "linglong") && !player.GetArmor())
                return true;
            return false;
        }
    }

    public class LinglongTar : TargetModSkill
    {
        public LinglongTar() : base("#linglong-tar")
        {
            pattern = "TrickCard";
            skill_type = SkillType.Wizzard;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return RoomLogic.PlayerHasSkill(room, from, "linglong") && !from.GetTreasure() ? true : false;
        }
    }

    public class LinglongMax : MaxCardsSkill
    {
        public LinglongMax() : base("#linglong-max") { }

        public override int GetExtra(Room room, Player target)
        {
            return RoomLogic.PlayerHasSkill(room, target, "linglong") && !target.GetDefensiveHorse() && !target.GetOffensiveHorse() ? 1 : 0;
        }
    }

    public class Zhuiji : DistanceSkill
    {
        public Zhuiji() : base("zhuiji") { }

        public override int GetFixed(Room room, Player from, Player to)
        {
            if (from != to && from != null && to != null && RoomLogic.PlayerHasShownSkill(room, from, this) && to.Hp <= from.Hp)
                return 1;

            return 0;
        }
    }

    public class Shichou : TriggerSkill
    {
        public Shichou() : base("shichou")
        {
            events.Add(TriggerEvent.CardTargetAnnounced);
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && player.GetLostHp() > 0)
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
                List<Player> players = room.AskForPlayersChosen(player, targets, Name, 0, player.GetLostHp(),
                    string.Format("@extra_targets1:::{0}:{1}", use.Card.Name, player.GetLostHp()), true, info.SkillPosition);
                room.RemoveTag("extra_target_skill");
                if (players.Count > 0)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    List<string> names = new List<string>();
                    foreach (Player p in players)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                        names.Add(p.Name);
                    }
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        Card_str = RoomLogic.CardToString(room, use.Card),
                        Arg = Name
                    };
                    log.SetTos(players);
                    room.SendLog(log);

                    player.SetTag("extra_targets", names);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> names = (List<string>)player.GetTag("extra_targets");
            player.RemoveTag("extra_targets");
            List<Player> targets = new List<Player>();
            foreach (string name in names)
                targets.Add(room.FindPlayer(name));

            if (targets.Count > 0 && data is CardUseStruct use)
            {
                use.To.AddRange(targets);
                room.SortByActionOrder(ref use);
                data = use;
            }

            return false;
        }
    }

    public class Fuqi : TriggerSkill
    {
        public Fuqi() : base("fuqi")
        {
            frequency = Frequency.Compulsory;
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.TrickCardCanceling };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && base.Triggerable(player, room))
            {
                if (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == Duel.ClassName || use.Card.Name == Collateral.ClassName || use.Card.Name == ArcheryAttack.ClassName
                    || use.Card.Name == SavageAssault.ClassName)
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player p in use.To)
                        if (p != player && RoomLogic.DistanceTo(room, player, p, null, true) == 1)
                            targets.Add(p);

                    if (targets.Count > 0)
                        return new TriggerStruct(Name, player, targets);
                }
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && base.Triggerable(effect.From, room) && player != effect.From
                && RoomLogic.DistanceTo(room, effect.From, player, null, true) == 1)
            {
                return new TriggerStruct(Name, effect.From);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct chose_use)
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
            else if (triggerEvent == TriggerEvent.TrickCardCanceling)
                return true;

            return false;
        }
    }

    public class Jiaozi : TriggerSkill
    {
        public Jiaozi() : base("jiaozi")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.DamageInflicted };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
            {
                bool check = true;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.HandcardNum >= player.HandcardNum)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            DamageStruct damage = (DamageStruct)data;
            if (triggerEvent == TriggerEvent.DamageCaused)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
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
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
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

    public class Moukui : TriggerSkill
    {
        public Moukui() : base("moukui")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.CardFinished };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && use.From.ContainsTag(Name)
                && use.From.GetTag(Name) is Dictionary<string, List<string>> discard_list)
            {
                string str = RoomLogic.CardToString(room, use.Card);
                discard_list.Remove(str);
                if (discard_list.Keys.Count == 0)
                    use.From.RemoveTag(Name);
                else
                    use.From.SetTag(Name, discard_list);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && base.Triggerable(player, room))
                return new TriggerStruct(Name, player, use.To);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<string> choices = new List<string> { "draw", "cancel" };
            if (RoomLogic.CanDiscard(room, ask_who, player, "he")) choices.Insert(1, "discard");
            player.SetFlags("moukui_target");
            string choice = room.AskForChoice(ask_who, Name, string.Join("+", choices), new List<string> { "@moukui:" + player.Name }, data);
            player.SetFlags("-moukui_target");
            if (choice != "cancel")
            {
                ask_who.SetTag(Name, choice);
                room.NotifySkillInvoked(ask_who, Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                string choice = ask_who.GetTag(Name).ToString();
                ask_who.RemoveTag(Name);
                if (choice == "draw")
                    room.DrawCards(ask_who, 1, Name);
                else
                {
                    int card_id = room.AskForCardChosen(ask_who, player, "he", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(card_id, player, ask_who);
                }

                Dictionary<string, List<string>> discard_list = ask_who.ContainsTag(Name) ? (Dictionary<string, List<string>>)ask_who.GetTag(Name) : new Dictionary<string, List<string>>();
                string str = RoomLogic.CardToString(room, use.Card);
                if (!discard_list.ContainsKey(str))
                    discard_list[str] = new List<string> { player.Name };
                else
                    discard_list[str].Add(player.Name);

                ask_who.SetTag(Name, discard_list);
            }

            return false;
        }
    }

    public class MoukuiDis : TriggerSkill
    {
        public MoukuiDis() : base("#moukui")
        {
            events.Add(TriggerEvent.SlashMissed);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is SlashEffectStruct effect && player.Alive && effect.To.Alive && RoomLogic.CanDiscard(room, effect.To, player, "he") && player.ContainsTag("moukui")
                && player.GetTag("moukui") is Dictionary<string, List<string>> discard_list)
            {
                string str = RoomLogic.CardToString(room, effect.Slash);
                if (discard_list.ContainsKey(str) && discard_list[str].Contains(effect.To.Name))
                    return new TriggerStruct(Name, effect.To);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.ContainsTag("moukui") && player.GetTag("moukui") is Dictionary<string, List<string>> discard_list && data is SlashEffectStruct effect)
            {
                string str = RoomLogic.CardToString(room, effect.Slash);
                discard_list[str].Remove(ask_who.Name);
                if (discard_list[str].Count == 0) discard_list.Remove(str);
                player.SetTag("moukui", discard_list);

                if (RoomLogic.CanDiscard(room, ask_who, player, "he"))
                {
                    int id = room.AskForCardChosen(ask_who, player, "he", "moukui", false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(id, player, ask_who);
                }
            }

            return false;
        }
    }

    public class Yishe : TriggerSkill
    {
        public Yishe() : base("yishe")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null
                && move.From.Alive && move.From_pile_names.Contains("rice") && move.From.GetPile("rice").Count == 0 && move.From.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = move.From
                };
                room.Recover(move.From, recover, true);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && player.GetPile("rice").Count == 0)
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
            room.DrawCards(player, 2, Name);
            if (player.Alive)
            {
                List<int> ids = room.AskForExchange(player, Name, 2, 2, "@yishe", string.Empty, "..", info.SkillPosition);
                room.AddToPile(player, "rice", ids);
            }

            return false;
        }
    }

    public class Bushi : TriggerSkill
    {
        public Bushi() : base("bushi")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.Damaged };
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct _damage && base.Triggerable(player, room) && player.GetPile("rice").Count > 0)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    Times = _damage.Damage
                };
                return trigger;
            }
            else if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.To.Alive && base.Triggerable(player, room) && player.GetPile("rice").Count > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = new List<int>();
            if (triggerEvent == TriggerEvent.Damaged && player.GetPile("rice").Count > 0)
                ids = room.AskForExchange(player, Name, 1, 0, "@bushi", "rice", string.Empty, info.SkillPosition);
            else if (data is DamageStruct damage && player.GetPile("rice").Count > 0 && damage.To.Alive)
            {
                room.SetTag(Name, player);
                AskForMoveCardsStruct move = room.AskForMoveCards(damage.To, player.GetPile("rice"), new List<int>(), false, Name, 1, 1, true, false, null, null);
                room.RemoveTag(Name);
                if (move.Success && move.Bottom.Count == 1)
                    ids = move.Bottom;
            }

            if (ids.Count == 1)
            {
                room.SetTag(Name, ids);
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = (List<int>)room.GetTag(Name);
            room.RemoveTag(Name);
            if (triggerEvent == TriggerEvent.Damaged)
                room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty));
            else if (data is DamageStruct damage)
                room.ObtainCard(damage.To, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXCHANGE_FROM_PILE, damage.To.Name, Name, string.Empty));

            return false;
        }
    }

    public class Midao : TriggerSkill
    {
        public Midao() : base("midao")
        {
            events.Add(TriggerEvent.AskForRetrial);
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.GetPile("rice").Count > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;

            List<string> prompt_list = new List<string> { "@midao", judge.Who.Name, string.Empty, Name, judge.Reason };
            string prompt = string.Join(":", prompt_list);

            room.SetTag(Name, data);
            List<int> ids = room.AskForExchange(player, Name, 1, 0, prompt, "rice", string.Empty, info.SkillPosition);
            room.RemoveTag(Name);
            if (ids.Count == 1)
            {
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.Retrial(room.GetCard(ids[0]), player, ref judge, Name, false, info.SkillPosition);
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

    public class Rangshang : TriggerSkill
    {
        public Rangshang() : base("rangshang")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Damaged };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && data is DamageStruct damage && damage.Nature == DamageStruct.DamageNature.Fire)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && player.GetMark("@flame") > 0 && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.AddPlayerMark(ask_who, "@flame", damage.Damage);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                room.LoseHp(ask_who, ask_who.GetMark("@flame"));
            }

            return false;
        }
    }

    public class Hanyong : TriggerSkill
    {
        public Hanyong() : base("hanyong")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed };
            skill_type = SkillType.Attack;
        }


        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && data is CardUseStruct use
                && (use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == SavageAssault.ClassName) && player.Hp < room.Round)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player player, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag(Name, data);
                bool invoke = room.AskForSkillInvoke(player, Name, "@hanyong:::" + use.Card.Name, info.SkillPosition);
                room.RemoveTag(Name);
                if (invoke)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player machao, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {

                LogMessage log = new LogMessage
                {
                    Type = "#hanyong",
                    From = machao.Name,
                    Arg = use.Card.Name,
                };
                room.SendLog(log);

                use.ExDamage++;
                data = use;
            }

            return false;
        }
    }

    public class Xionghuo : ZeroCardViewAsSkill
    {
        public Xionghuo() : base("xionghuo")
        {
            skill_type = SkillType.Attack;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(Name) > 0;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(XionghuoCard.ClassName) { Skill = Name };
        }
    }

    public class XionghuoCard : SkillCard
    {
        public static string ClassName = "XionghuoCard";
        public XionghuoCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && to_select.GetMark("xionghuo") == 0;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            player.RemoveMark("xionghuo");
            int count = player.GetMark("xionghuo");
            if (count > 0)
                room.SetPlayerStringMark(player, "cruel", player.GetMark("xionghuo").ToString());
            else
                room.RemovePlayerStringMark(player, "cruel");

            target.AddMark("xionghuo");
            room.SetPlayerStringMark(target, "cruel", target.GetMark("xionghuo").ToString());

            List<string> names =  target.ContainsTag("xionghuo") ? (List<string>)target.GetTag("xionghuo") : new List<string>();
            names.Add(player.Name);
            target.SetTag("xionghuo", names);
        }
    }

    public class XionghuoTri : TriggerSkill
    {
        public XionghuoTri() : base("#xionghuo")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.DamageCaused, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag("xionghuo_slash"))
                player.RemoveTag("xionghuo_slash");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.DamageCaused && base.Triggerable(player, room) && data is DamageStruct damage && damage.To.GetMark("xionghuo") > 0
                && damage.To.ContainsTag("xionghuo") && damage.To.GetTag("xionghuo") is List<string> xurongs && xurongs.Contains(player.Name))
            {
                triggers.Add(new TriggerStruct(Name, player));
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.Play && player.GetMark("xionghuo") > 0
                && player.ContainsTag("xionghuo") && player.GetTag("xionghuo") is List<string> names)
            {
                foreach (string str in names)
                {
                    Player p = room.FindPlayer(str);
                    if (base.Triggerable(p, room)) triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, "xionghuo");
            switch (triggerEvent)
            {
                case TriggerEvent.GameStart:
                    player.SetMark("xionghuo", 3);
                    room.SetPlayerStringMark(player, "cruel", player.GetMark("xionghuo").ToString());
                    break;
                case TriggerEvent.EventPhaseStart when player.Alive && ask_who.Alive:
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                        room.BroadcastSkillInvoke("xionghuo", ask_who, info.SkillPosition);

                        player.RemoveMark("xionghuo");
                        int count = player.GetMark("xionghuo");
                        if (count > 0)
                            room.SetPlayerStringMark(player, "cruel", player.GetMark("xionghuo").ToString());
                        else
                            room.RemovePlayerStringMark(player, "cruel");

                        List<int> ids = new List<int> { 0, 1, 2 };
                        Shuffle.shuffle(ref ids);
                        switch (ids[0])
                        {
                            case 0:
                                room.Damage(new DamageStruct("xionghuo", ask_who, player, 1, DamageStruct.DamageNature.Fire));
                                if (player.Alive)
                                {
                                    List<string> names = player.ContainsTag("xionghuo_slash") ? (List<string>)player.GetTag("xionghuo_slash") : new List<string>();
                                    names.Add(ask_who.Name);
                                    player.SetTag("xionghuo_slash", names);
                                }
                                break;
                            case 1:
                                room.LoseHp(player);
                                if (player.Alive) player.SetFlags("xionghuo");
                                break;
                            case 2:
                                List<int> get = new List<int>(), eq = new List<int>(), hands = new List<int>();
                                foreach (int id in player.GetEquips())
                                    if (RoomLogic.CanGetCard(room, ask_who, player, id)) eq.Add(id);
                                foreach (int id in player.GetCards("h"))
                                    if (RoomLogic.CanGetCard(room, ask_who, player, id)) hands.Add(id);
                                if (eq.Count > 0)
                                {
                                    Shuffle.shuffle(ref eq);
                                    get.Add(eq[0]);
                                }
                                if (hands.Count > 0)
                                {
                                    Shuffle.shuffle(ref hands);
                                    get.Add(hands[0]);
                                }
                                if (get.Count > 0)
                                    room.ObtainCard(ask_who, ref get, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, ask_who.Name, player.Name, Name, string.Empty), false);
                                break;
                        }

                        break;
                    }

                case TriggerEvent.DamageCaused when data is DamageStruct damage:
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, damage.To.Name);
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
                        break;
                    }
            }

            return false;
        }
    }
    public class XionghuoMax : MaxCardsSkill
    {
        public XionghuoMax() : base("#xionghuo-max")
        { }

        public override int GetExtra(Room room, Player target)
        {
            return target.HasFlag("xionghuo") ? -1 : 0;
        }
    }
    public class XionghuoPro : ProhibitSkill
    {
        public XionghuoPro() : base("#xionghuo-prohibit")
        {
        }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && to != null && card.Name.Contains(Slash.ClassName)
                && from.ContainsTag("xionghuo_slash") && from.GetTag("xionghuo_slash") is List<string> names && names.Contains(to.Name))
                return true;

            return false;
        }
    }

    public class Shajue : TriggerSkill
    {
        public Shajue() : base("shajue")
        {
            events.Add(TriggerEvent.Dying);
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Hp < 0)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    if (p != player)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);

            ask_who.SetMark("xionghuo", 3);
            room.SetPlayerStringMark(ask_who, "cruel", ask_who.GetMark("xionghuo").ToString());

            if (data is DyingStruct dying && dying.Damage.Card != null)
            {
                List<int> ids = new List<int>();
                foreach (int id in dying.Damage.Card.SubCards)
                    if (room.GetCardPlace(id) == Place.DiscardPile || room.GetCardPlace(id) == Place.PlaceTable)
                        ids.Add(id);

                if (ids.Count > 0)
                    room.ObtainCard(ask_who, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, ask_who.Name, Name, string.Empty));
            }

            return false;
        }
    }

    public class Shenxian : TriggerSkill
    {
        public Shenxian() : base("shenxian")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceHand)
                && move.To_place == Place.PlaceTable && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD)
            {
                bool basic = false;
                foreach (int id in move.Card_ids)
                {
                    if (Engine.GetFunctionCard(room.GetCard(id).Name) is BasicCard)
                    {
                        basic = true;
                        break;
                    }
                }
                if (basic)
                {
                    List<Player> xincai = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player p in xincai)
                        if (move.From != p && p.Phase == PlayerPhase.NotActive && !p.HasFlag(Name))
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
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
            ask_who.SetFlags(Name);
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    public class Yisuan : TriggerSkill
    {
        public Yisuan() : base("yisuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Masochism;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-yisuan");
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile && move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_USE)
            {
                Player target = room.FindPlayer(move.Reason.PlayerId);
                WrappedCard card = RoomLogic.ParseCard(room, move.Reason.CardString);
                if (card != null && Engine.GetFunctionCard(card.Name) is TrickCard && base.Triggerable(target, room) && target.Phase == PlayerPhase.Play && !target.HasFlag(Name))
                {
                    foreach (int id in move.Card_ids)
                        if (room.GetCardPlace(id) == Place.DiscardPile)
                            return new TriggerStruct(Name, target);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                WrappedCard card = RoomLogic.ParseCard(room, move.Reason.CardString);
                bool discard = false;
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCardPlace(id) == Place.DiscardPile)
                    {
                        discard = true;
                        break;
                    }
                }
                if (discard && ask_who.Alive)
                {
                    room.SetTag(Name, card);
                    bool invoke = room.AskForSkillInvoke(ask_who, Name, "@yisuan:::" + card.Name, info.SkillPosition);
                    room.RemoveTag(Name);
                    if (invoke)
                    {
                        room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                        room.LoseMaxHp(ask_who);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.Alive && data is CardsMoveOneTimeStruct move)
            {
                List<int> ids = new List<int>();
                foreach (int id in move.Card_ids)
                    if (room.GetCardPlace(id) == Place.DiscardPile) ids.Add(id);

                if (ids.Count > 0)
                {
                    ask_who.SetFlags(Name);
                    room.ObtainCard(ask_who, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, ask_who.Name, Name, string.Empty));
                }
            }

            return false;
        }
    }

    public class Langxi : PhaseChangeSkill
    {
        public Langxi() : base("langxi") { skill_type = SkillType.Attack; }
        public override bool CanPreShow() => false;
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Start;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Hp <= player.Hp) targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@langxi", true, true, info.SkillPosition);
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
            Player target = room.FindPlayer(player.GetTag(Name).ToString());
            player.RemoveTag(Name);
            Random ra = new Random();
            int result = ra.Next(0, 3);
            if (result > 0)
                room.Damage(new DamageStruct(Name, player, target, result));

            return false;
        }
    }

    public class Luanzhan : TriggerSkill
    {
        public Luanzhan() : base("luanzhan")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardTargetAnnounced, TriggerEvent.TargetChosen, TriggerEvent.Damage,
                TriggerEvent.GameStart, TriggerEvent.EventLoseSkill, TriggerEvent.EventAcquireSkill };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                room.SetPlayerStringMark(player, Name, "0");
            else if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && info.Info == Name)
            {
                player.SetMark(Name, 0);
                room.SetPlayerStringMark(player, Name, "0");
            }
            else if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct _info && _info.Info == Name)
                room.RemovePlayerStringMark(player, Name);
            else if (triggerEvent == TriggerEvent.Damage)
            {
                player.AddMark(Name);
                if (player.StringMarks.ContainsKey(Name))
                    room.SetPlayerStringMark(player, Name, player.GetMark(Name).ToString());
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && player.GetMark(Name) > 0 && use.Card.Name != Collateral.ClassName
                && use.To.Count < player.GetMark(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || (fcard is TrickCard && WrappedCard.IsBlack(use.Card.Suit) && !(fcard is DelayedTrick) && !(fcard is Nullification)))
                {
                    player.SetMark(Name, 0);
                    if (player.StringMarks.ContainsKey(Name))
                        room.SetPlayerStringMark(player, Name, "0");
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && use.Card.ExtraTarget
                && base.Triggerable(player, room) && player.GetMark(Name) > 0 && use.Card.Name != Collateral.ClassName)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || (fcard is TrickCard && WrappedCard.IsBlack(use.Card.Suit) && !(fcard is DelayedTrick) && !(fcard is Nullification)))
                {
                    return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                foreach (Player p in room.GetAlivePlayers())
                {
                    if ((fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, player, p))
                        || ((fcard is Slash || fcard is SavageAssault || fcard is ArcheryAttack) && p == player)
                        || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && (!RoomLogic.CanGetCard(room, player, p, "hej") || p == player))
                        || (fcard is Dismantlement && (!RoomLogic.CanDiscard(room, player, p, "hej") || p == player))) continue;

                    if (!use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, use.Card) == null)
                        targets.Add(p);
                }

                room.SetTag("extra_target_skill", data);                   //for AI
                List<Player> players = room.AskForPlayersChosen(player, targets, Name, 0, player.GetMark(Name),
                    string.Format("@extra_targets1:::{0}:{1}", use.Card.Name, player.GetMark(Name)), true, info.SkillPosition);
                room.RemoveTag("extra_target_skill");
                if (players.Count > 0)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    List<string> names = new List<string>();
                    foreach (Player p in players)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                        names.Add(p.Name);
                    }
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        Card_str = RoomLogic.CardToString(room, use.Card),
                        Arg = Name
                    };
                    log.SetTos(players);
                    room.SendLog(log);

                    player.SetTag("extra_targets", names);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<string> names = (List<string>)player.GetTag("extra_targets");
                player.RemoveTag("extra_targets");
                List<Player> targets = new List<Player>();
                foreach (string name in names)
                    targets.Add(room.FindPlayer(name));

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

    public class Falu : TriggerSkill
    {
        public Falu() : base("falu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.GameStart };
        }

        public static Dictionary<int, string> suits = new Dictionary<int, string>
        {
            { 0, "@zhiwei" },
            { 1, "@houtu" },
            { 2, "@yuqing" },
            { 3, "@gouchen" },
        };
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room) &&
                (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD && move.To_place == Place.PlaceTable)
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
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
                && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD && move.To_place == Place.DiscardPile)
            {
                foreach (int id in move.Card_ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (!card.HasFlag(Name)) continue;
                    string mark = suits[(int)card.Suit];
                    if (move.From.GetMark(mark) == 0)
                        return new TriggerStruct(Name, move.From);
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            if (triggerEvent == TriggerEvent.GameStart)
            {
                foreach (string mark in suits.Values)
                {
                    room.AddPlayerMark(player, mark);
                }
            }
            else if (data is CardsMoveOneTimeStruct move)
            {
                foreach (int id in move.Card_ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (!card.HasFlag(Name)) continue;
                    string mark = suits[(int)card.Suit];
                    if (ask_who.GetMark(mark) == 0)
                        room.AddPlayerMark(ask_who, mark);
                }
            }

            return false;
        }
    }

    public class FaluClear : TriggerSkill
    {
        public FaluClear() : base("#falu")
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
                    if (move.From_places[i] == Place.PlaceTable && card.HasFlag("falu"))
                        card.SetFlags("-falu");
                }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }

    public class Zhenyi : TriggerSkill
    {
        public Zhenyi() : base("zhenyi")
        {
            events = new List<TriggerEvent> { TriggerEvent.JudgeResult, TriggerEvent.DamageCaused, TriggerEvent.Damaged };
            view_as_skill = new ZhenyiVS();
            skill_type = SkillType.Alter;
        }
        public override bool CanPreShow() => true;
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.JudgeResult && data is JudgeStruct judge)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.GetMark(Falu.suits[0]) > 0)
                        triggers.Add(new TriggerStruct(Name, p));
            }
            else if (triggerEvent == TriggerEvent.DamageCaused && base.Triggerable(player, room) && player.GetMark(Falu.suits[2]) > 0)
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && player.GetMark(Falu.suits[3]) > 0
                && data is DamageStruct damage && damage.Nature != DamageStruct.DamageNature.Normal)
                triggers.Add(new TriggerStruct(Name, player));

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            object _data = data;
            string mark = string.Empty;
            Player target = null;
            switch (triggerEvent)
            {
                case TriggerEvent.JudgeResult when data is JudgeStruct judge:
                    _data = string.Format("@zhenyi-judge:{0}::{1}", player.Name, judge.Reason);
                    target = player;
                    mark = Falu.suits[0];
                    break;
                case TriggerEvent.DamageCaused when data is DamageStruct damage:
                    _data = target = damage.To;
                    mark = Falu.suits[2];
                    break;
                case TriggerEvent.Damaged:
                    mark = Falu.suits[3];
                    break;
            }

            room.SetTag(Name, data);
            bool invoke = room.AskForSkillInvoke(ask_who, Name, _data, info.SkillPosition);
            room.RemoveTag(Name);

            if (invoke)
            {
                room.SetPlayerMark(ask_who, mark, 0);
                if (target != null) room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, target.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            switch (triggerEvent)
            {
                case TriggerEvent.JudgeResult when data is JudgeStruct judge:
                    {
                        LogMessage log = new LogMessage
                        {
                            Type = "#zhenyi",
                            From = ask_who.Name,
                            To = new List<string> { player.Name },
                            Arg = judge.Reason
                        };

                        string choic = room.AskForChoice(ask_who, Name, "spade+heart", new List<string> { string.Format("@zhenyi:{0}::{1}", player.Name, judge.Reason) }, data);
                        if (choic == "spade")
                        {
                            judge.Card.SetSuit(WrappedCard.CardSuit.Spade);
                            log.Arg2 = "<color=black>♠</color>5";
                        }
                        else
                        {
                            judge.Card.SetSuit(WrappedCard.CardSuit.Heart);
                            log.Arg2 = "<color=red>♥</color>5";
                        }
                        judge.Card.SetNumber(5);
                        judge.Card.Modified = true;

                        room.UpdateJudgeResult(ref judge);
                        data = judge;

                        room.SendLog(log);
                    }
                    break;
                case TriggerEvent.DamageCaused when data is DamageStruct damage:
                    {
                        JudgeStruct judge = new JudgeStruct
                        {
                            Pattern = ".|black",
                            Negative = false,
                            Good = true,
                            Reason = Name,
                            PlayAnimation = true,
                            Who = player
                        };
                        room.Judge(ref judge);

                        if (judge.IsGood())
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
                    break;
                case TriggerEvent.Damaged:
                    {
                        int trick = -1;
                        int equip = -1;
                        int basic = -1;
                        List<int> ids = new List<int>();
                        foreach (int id in room.DrawPile)
                        {
                            WrappedCard card = room.GetCard(id);
                            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                            if (trick == -1 && fcard is TrickCard)
                            {
                                trick = id;
                                ids.Add(trick);
                            }
                            if (equip == -1 && fcard is EquipCard)
                            {
                                equip = id;
                                ids.Add(equip);
                            }
                            if (basic == -1 && fcard is BasicCard)
                            {
                                basic = id;
                                ids.Add(basic);
                            }

                            if (ids.Count >= 3) break;
                        }
                        if (ids.Count > 0)
                            room.ObtainCard(ask_who, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTCARD, ask_who.Name, Name, string.Empty), false);
                    }
                    break;
            }
            return false;
        }
    }

    public class ZhenyiVS : OneCardViewAsSkill
    {
        public ZhenyiVS() : base("zhenyi")
        {
            filter_pattern = ".";
            response_or_use = true;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            WrappedCard peach = new WrappedCard(Peach.ClassName);
            FunctionCard fcard = Peach.Instance;
            if (player.HasFlag("Global_Dying") && player.GetMark(Falu.suits[1]) > 0
                && room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE && fcard.IsAvailable(room, player, peach))
            {
                return Engine.MatchExpPattern(room, pattern, player, peach);
            }

            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard zy = new WrappedCard(ZhenyiCard.ClassName);
            zy.AddSubCard(card);
            return zy;
        }
    }
    public class ZhenyiCard : SkillCard
    {
        public static string ClassName = "ZhenyiCard";
        public ZhenyiCard() : base(ClassName) { target_fixed = true; }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player player = use.From;
            room.SetPlayerMark(player, Falu.suits[1], 0);
            room.BroadcastSkillInvoke("zhenyi", player, use.Card.SkillPosition);
            room.NotifySkillInvoked(player, "zhenyi");
            WrappedCard wrapped = new WrappedCard(Peach.ClassName) { Skill = "_zhenyi" };
            wrapped.AddSubCard(use.Card);
            return wrapped;
        }
    }

    public class Dianhua : PhaseChangeSkill
    {
        public Dianhua() : base("dianhua")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool Triggerable(Player target, Room room)
        {
            if (base.Triggerable(target, room) && (target.Phase == PlayerPhase.Finish || target.Phase == PlayerPhase.Start))
            {
                foreach (string mark in Falu.suits.Values)
                    if (target.GetMark(mark) > 0) return true;
            }

            return false;
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

            AskForMoveCardsStruct result = room.AskForMoveCards(zhuge, guanxing, new List<int>(), true, Name, 0, 0, false, true, new List<int>(), info.SkillPosition);
            List<int> top_cards = result.Top;
            if (top_cards.Count > 0)
            {
                LogMessage log1 = new LogMessage
                {
                    Type = "$GuanxingTop",
                    From = zhuge.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(top_cards))
                };
                room.SendLog(log1, zhuge);
            }
            room.ReturnToDrawPile(top_cards, false, zhuge);

            return false;
        }
        private int GetGuanxingNum(Room room, Player zhuge)
        {
            int count = 0;
            foreach (string mark in Falu.suits.Values)
                if (zhuge.GetMark(mark) > 0) count++;

            return count;
        }
    }

    public class Qiangwu : TriggerSkill
    {
        public Qiangwu() : base("qiangwu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Attack;
            view_as_skill = new QiangwuVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class QiangwuVS : ZeroCardViewAsSkill
    {
        public QiangwuVS() : base("qiangwu")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(QiangwuCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(QiangwuCard.ClassName)
            {
                Skill = Name
            };
            return card;
        }
    }

    public class QiangwuCard : SkillCard
    {
        public static string ClassName = "QiangwuCard";
        public QiangwuCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                PlayAnimation = false,
                Who = player,
                Reason = "qiangwu"
            };

            room.Judge(ref judge);

            player.SetMark("qiangwu", judge.Card.Number);
            room.SetPlayerStringMark(player, "qiangwu", judge.Card.Number.ToString());
        }
    }

    public class QiangwuTar : TargetModSkill
    {
        public QiangwuTar() : base("#qiangwu-tar", false) { }

        public override bool IgnoreCount(Room room, Player from, WrappedCard card)
        {
            return from.GetMark("qiangwu") > 0 && RoomLogic.GetCardNumber(room, card) > from.GetMark("qiangwu");
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from.GetMark("qiangwu") > 0 && RoomLogic.GetCardNumber(room, card) < from.GetMark("qiangwu");
        }
    }

    public class Xueji : OneCardViewAsSkill
    {
        public Xueji() : base("xueji")
        {
            filter_pattern = ".|red";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude() && !player.HasUsed(XuejiCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard xj = new WrappedCard(XuejiCard.ClassName)
            {
                Skill = Name
            };
            xj.AddSubCard(card);
            return xj;
        }
    }

    public class XuejiCard : SkillCard
    {
        public static string ClassName = "XuejiCard";
        public XuejiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count < Math.Max(1, Self.GetLostHp());
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = new List<Player>(card_use.To);
            room.SortByActionOrder(ref targets);

            bool hidden = (TypeID == CardType.TypeSkill && !WillThrow);
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            log.SetTos(targets);

            List<int> used_cards = new List<int>();
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            used_cards.AddRange(card_use.Card.SubCards);

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;

            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_THROW, card_use.From.Name, null,
                card_use.Card.Skill, null)
            {
                CardString = RoomLogic.CardToString(room, card_use.Card),
                General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
            };

            room.MoveCardTo(card_use.Card, card_use.From, null, Place.PlaceTable, reason, true);
            room.SendLog(log);

            List<int> table_cardids = room.GetCardIdsOnTable(card_use.Card);
            if (table_cardids.Count > 0)
            {
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, player, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
            }

            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = new List<Player>(card_use.To);
            foreach (Player p in targets)
                if (!p.Chained && RoomLogic.CanBeChainedBy(room, p, player))
                    room.SetPlayerChained(p, true);

            Player target = card_use.To[0];
            if (target.Alive)
                room.Damage(new DamageStruct("xeji", player, target, 1, DamageStruct.DamageNature.Fire));
        }
    }

    public class Huxiao : TriggerSkill
    {
        public Huxiao() : base("huxiao")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage };
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.To.Alive && base.Triggerable(player, room) && damage.Nature == DamageStruct.DamageNature.Fire)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (data is DamageStruct damage && damage.To.Alive)
            {
                room.DrawCards(damage.To, new DrawCardStruct(1, player, Name));
                if (room.Current == player)
                    player.SetFlags(string.Format("{0}_{1}", Name, damage.To.Name));
            }

            return false;
        }
    }

    public class HuxiaoTar : TargetModSkill
    {
        public HuxiaoTar() : base("#huxiao-tar", false)
        {
            pattern = ".Basic";
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card)
        {
            return from != null && to != null && from.HasFlag(string.Format("huxiao_{0}", to.Name));
        }
    }

    public class Wuji : TriggerSkill
    {
        public Wuji() : base("wuji")
        {
            frequency = Frequency.Wake;
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Damage, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && base.Triggerable(player, room) && player.GetMark(Name) == 0
                && data is DamageStruct damage && player.Phase != PlayerPhase.NotActive)
                player.AddMark("wuji_count", damage.Damage);
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark("wuji_count") > 0)
                player.SetMark("wuji_count", 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && player.GetMark(Name) == 0 && player.GetMark("wuji_count") >= 3)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);

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

            room.HandleAcquireDetachSkills(player, "-huxiao");

            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.Weapon.Value == ClassicBlade.ClassName)
                {
                    room.ObtainCard(player, p.Weapon.Key);
                    return false;
                }
            }
            foreach (int id in room.DrawPile)
            {
                if (room.GetCard(id).Name == ClassicBlade.ClassName)
                {
                    room.ObtainCard(player, id);
                    return false;
                }
            }
            foreach (int id in room.DiscardPile)
            {
                if (room.GetCard(id).Name == ClassicBlade.ClassName)
                {
                    room.ObtainCard(player, id);
                    break;
                }
            }

            return false;
        }
    }

    public class Liangzhu : TriggerSkill
    {
        public Liangzhu() : base("liangzhu")
        {
            events.Add(TriggerEvent.HpRecover);
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();

            if (player.Phase == PlayerPhase.Play)
            {
                List<Player> ssx = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in ssx)
                    triggers.Add(new TriggerStruct(Name, p));

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
            Player target = null;
            if (player == ask_who)
            {
                target = player;
            }
            else
            {
                List<string> prompts = new List<string> { string.Empty, "@liangzhu-let:" + player.Name };
                player.SetFlags(Name);
                string choice = room.AskForChoice(ask_who, Name, "let_draw+draw_self", prompts, player);
                player.SetFlags("-liangzhu");
                if (choice == "let_draw")
                {
                    target = player;
                    target.SetMark(Name, 1);
                }
                else
                    target = ask_who;
            }
            room.DrawCards(target, new DrawCardStruct(target == player ? 2 : 1, ask_who, Name));

            return false;
        }
    }

    public class Fanxiang : PhaseChangeSkill
    {
        public Fanxiang() : base("fanxiang")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark("liangzhu") > 0 && p.IsWounded())
                        return new TriggerStruct(Name, player);
                }
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

                room.HandleAcquireDetachSkills(player, "-liangzhu|xiaoji", false);
            }

            return false;
        }
    }

    public class Fengpo : TriggerSkill
    {
        public Fengpo() : base("fengpo")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
            priority = new Dictionary<TriggerEvent, double>
            {
                { TriggerEvent.TargetChosen, 3 },
                { TriggerEvent.CardUsedAnnounced, -1 },
                { TriggerEvent.EventPhaseChanging, 3 },
            };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && player == room.Current
                && (use.Card.Name == Duel.ClassName || use.Card.Name.Contains(Slash.ClassName)))
            {
                if (player.GetMark(Name) == 0)
                    player.SetFlags(RoomLogic.CardToString(room, use.Card));

                player.AddMark(Name);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && base.Triggerable(player, room) && data is CardUseStruct use
                && (use.Card.Name == Duel.ClassName || use.Card.Name.Contains(Slash.ClassName)) && use.To.Count == 1 && player.HasFlag(RoomLogic.CardToString(room, use.Card)))
            {
                return new TriggerStruct(Name, player, use.To);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player player, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag(Name, data);
                player.SetFlags(string.Format("-{0}", RoomLogic.CardToString(room, use.Card)));
                string choice = room.AskForChoice(player, Name, "draw+damage+cancel", new List<string> { "@fengpo:" + skill_target.Name }, skill_target);
                room.RemoveTag(Name);
                if (choice != "cancel")
                {
                    player.SetTag(Name, choice);
                    room.NotifySkillInvoked(player, Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player machao, TriggerStruct info)
        {
            string choice = machao.GetTag(Name).ToString();
            machao.RemoveTag(Name);
            int count = 0;
            foreach (int id in skill_target.GetCards("h"))
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond) count++;

            if (count > 0)
            {
                if (choice == "draw")
                    room.DrawCards(machao, count, Name);
                else if (data is CardUseStruct use)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#fengpo",
                        From = machao.Name,
                        Arg = use.Card.Name,
                        Arg2 = count.ToString()
                    };
                    room.SendLog(log);

                    use.ExDamage += count;
                    data = use;
                }
            }
            return false;
        }
    }

    public class Zhengnan : TriggerSkill
    {
        public Zhengnan() : base("zhengnan")
        {
            events.Add(TriggerEvent.Death);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            List<Player> caopis = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player caopi in caopis)
                if (caopi != player)
                    triggers.Add(new TriggerStruct(Name, caopi));

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            List<string> choices = new List<string> { "draw" };
            if (!caopi.GetAcquiredSkills().Contains("wusheng_jx")) choices.Add("wusheng_jx");
            if (!caopi.GetAcquiredSkills().Contains("dangxian")) choices.Add("dangxian");
            if (!caopi.GetAcquiredSkills().Contains("zhiman_jx")) choices.Add("zhiman_jx");
            choices.Add("cancel");

            string choice = room.AskForChoice(caopi, Name, string.Join("+", choices));
            if (choice != "cancel")
            {
                room.NotifySkillInvoked(caopi, Name);
                room.BroadcastSkillInvoke(Name, caopi, info.SkillPosition);
                caopi.SetTag(Name, choice);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            string choice = caopi.GetTag(Name).ToString();
            caopi.RemoveTag(Name);
            if (choice == "draw")
            {
                room.DrawCards(caopi, 3, Name);
            }
            else
            {
                room.HandleAcquireDetachSkills(caopi, choice, true);
            }


            return false;
        }
    }

    public class Xiefang : DistanceSkill
    {
        public Xiefang() : base("xiefang")
        {
        }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            int count = 0;
            if (RoomLogic.PlayerHasShownSkill(room, from, this))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (!p.IsMale())
                        count--;
            }

            return count;
        }
    }

    public class Wuniang : TriggerSkill
    {
        public Wuniang() : base("wuniang")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Card.Name.Contains(Slash.ClassName) && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (!p.IsNude() && RoomLogic.CanGetCard(room, player, p, "he"))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@wuniang", true, true, info.SkillPosition);
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
            Player target = room.FindPlayer(player.GetTag(Name).ToString());
            player.RemoveTag(Name);
            if (RoomLogic.CanGetCard(room, player, target, "he"))
            {
                int id = room.AskForCardChosen(player, target, "he", Name, false, FunctionCard.HandlingMethod.MethodGet);
                room.ObtainCard(player, room.GetCard(id), new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, Name, string.Empty), false);
                if (target.Alive)
                    room.DrawCards(target, new DrawCardStruct(1, player, Name));

                foreach (Player p in room.GetAlivePlayers())
                    if (p.ActualGeneral1 == "guansuo")
                        room.DrawCards(p, new DrawCardStruct(1, player, Name));
            }

            return false;
        }
    }

    public class Xushen : TriggerSkill
    {
        public Xushen() : base("xushen")
        {
            events = new List<TriggerEvent> { TriggerEvent.HpRecover, TriggerEvent.AskForPeachesDone };
            frequency = Frequency.Limited;
            limit_mark = "@xu";
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.HpRecover && data is RecoverStruct recover && recover.Who != null && player.HasFlag("Global_Dying")
                && recover.Who.Alive && !player.HasFlag(Name) && player.GetMark(limit_mark) > 0)
            {
                player.SetFlags(Name);
                if (recover.Who != null && recover.Who.IsMale() && recover.Who.Alive)
                    recover.Who.SetMark(Name, 1);
            }
            else if (triggerEvent == TriggerEvent.AskForPeachesDone && player.HasFlag(Name))
                player.SetFlags("-xushen");
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.AskForPeachesDone && base.Triggerable(player, room) && player.GetMark(limit_mark) > 0)
            {
                Player guansuo = null, target = null;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.GetMark(Name) > 0)
                        target = p;
                    if (p.ActualGeneral1 == "guansuo")
                        guansuo = p;
                }

                if (guansuo == null && target != null)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.RemovePlayerMark(player, limit_mark);
            room.NotifySkillInvoked(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);

            Player target = null;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.GetMark(Name) > 0)
                {
                    target = p;
                    target.SetMark(Name, 0);
                    break;
                }
            }

            if (room.AskForChoice(target, Name, "change+cancel") == "change")
            {
                if (target.GetMark("@duanchang") > 0)
                {
                    target.DuanChang = string.Empty;
                    room.BroadcastProperty(target, "DuanChang");
                    room.SetPlayerMark(target, "@duanchang", 0);
                }

                string from_general = target.ActualGeneral1;
                if (!from_general.Contains("sujiang"))
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_REMOVE, target.Name, true.ToString());
                    room.HandleUsedGeneral("-" + from_general);
                }

                room.HandleUsedGeneral("guansuo");
                target.ActualGeneral1 = target.General1 = "guansuo";
                target.HeadSkinId = 0;
                target.Kingdom = "shu";
                room.BroadcastProperty(target, "Kingdom");
                room.BroadcastProperty(target, "HeadSkinId");
                room.NotifyProperty(room.GetClient(target), target, "ActualGeneral1");
                room.BroadcastProperty(target, "General1");

                int max = 4;
                if (target.GetRoleEnum() == PlayerRole.Lord)
                    max = 4 + (room.Players.Count > 4 && target.GetRoleEnum() == PlayerRole.Lord ? 1 : 0);

                if (max > target.MaxHp)
                {
                    int count = max - target.MaxHp;
                    target.MaxHp = max;
                    room.BroadcastProperty(target, "MaxHp");
                    LogMessage log = new LogMessage
                    {
                        Type = "$GainMaxHp",
                        From = target.Name,
                        Arg = count.ToString()
                    };
                    room.SendLog(log);
                    room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, target);
                }
                else if (max < target.MaxHp)
                {
                    room.LoseMaxHp(target, target.MaxHp - max);
                }

                List<string> skills = target.GetSkills(true, false);
                foreach (string skill in skills)
                {
                    Skill _s = Engine.GetSkill(skill);
                    if (_s != null && !_s.Attached_lord_skill)
                        room.DetachSkillFromPlayer(target, skill, false, target.GetAcquiredSkills().Contains(skill), true);
                }

                foreach (string skill in Engine.GetGeneralRelatedSkills("guansuo", room.Setting.GameMode))
                {
                    if (!room.Skills.Contains(skill))
                    {
                        room.Skills.Add(skill);
                        Skill main = Engine.GetSkill(skill);
                        if (main is TriggerSkill tskill)
                            room.RoomThread.AddTriggerSkill(tskill);
                    }

                    foreach (Skill _skill in Engine.GetRelatedSkills(skill))
                    {
                        if (!room.Skills.Contains(_skill.Name))
                        {
                            room.Skills.Add(_skill.Name);
                            if (_skill is TriggerSkill tskill)
                                room.RoomThread.AddTriggerSkill(tskill);
                        }
                    }
                }

                foreach (string skill_name in Engine.GetGeneralSkills("guansuo", room.Setting.GameMode, true))
                {
                    if (!room.Skills.Contains(skill_name))
                    {
                        room.Skills.Add(skill_name);
                        Skill main = Engine.GetSkill(skill_name);
                        if (main is TriggerSkill tskill)
                            room.RoomThread.AddTriggerSkill(tskill);
                    }

                    foreach (Skill _skill in Engine.GetRelatedSkills(skill_name))
                    {
                        if (!room.Skills.Contains(_skill.Name))
                        {
                            room.Skills.Add(_skill.Name);
                            if (_skill is TriggerSkill tskill)
                                room.RoomThread.AddTriggerSkill(tskill);
                        }
                    }

                    room.AddPlayerSkill(target, skill_name);
                }

                room.SendPlayerSkillsToOthers(target);
                room.FilterCards(target, target.GetCards("he"), true);
            }

            if (player.Alive)
            {
                if (player.IsWounded())
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = player
                    };
                    room.Recover(player, recover, true);
                }

                room.HandleAcquireDetachSkills(player, "zhennan", true);
            }

            return false;
        }
    }

    public class Zhennan : TriggerSkill
    {
        public Zhennan() : base("zhennan")
        {
            events.Add(TriggerEvent.TargetConfirming);
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.Card.Name == SavageAssault.ClassName && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@zhennan", true, true, info.SkillPosition);
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
            Player target = room.FindPlayer(player.GetTag(Name).ToString());
            player.RemoveTag(Name);
            Random ra = new Random();
            int result = ra.Next(1, 4);
            room.Damage(new DamageStruct(Name, player, target, result));
            return false;
        }
    }

    public class Yuhua : TriggerSkill
    {
        public Yuhua() : base("yuhua")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change
                && change.To == PlayerPhase.Discard && player.HandcardNum > player.Hp)
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

    public class YuhuaMax : MaxCardsSkill
    {
        public YuhuaMax() : base("#yuhua-max") { }

        public override bool Ingnore(Room room, Player player, int card_id)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, "yuhua"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card_id).Name);
                return fcard is TrickCard || fcard is EquipCard;
            }

            return false;
        }
    }

    public class Qirang : TriggerSkill
    {
        public Qirang() : base("qirang")
        {
            events.Add(TriggerEvent.CardUsed);
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is EquipCard)
                    return new TriggerStruct(Name, player);
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
            List<int> ids = new List<int>(room.DrawPile);
            foreach (int id in ids)
            {
                FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                if (fcard is TrickCard)
                {
                    room.ObtainCard(player, id);
                    break;
                }
            }

            return false;
        }
    }


    public class FanghunVS : OneCardViewAsSkill
    {
        public FanghunVS() : base("fanghun")
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
            return player.GetMark(Name) > 0 && Slash.IsAvailable(room, player);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (player.GetMark(Name) == 0) return false;
            pattern = Engine.GetPattern(pattern).GetPatternString();
            return pattern == Jink.ClassName || pattern == Slash.ClassName;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard originalCard, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(originalCard.Name);
            if (fcard is Slash)
            {
                WrappedCard jink = new WrappedCard(FanghunCard.ClassName)
                {
                    UserString = Jink.ClassName
                };
                jink.AddSubCard(originalCard);
                return jink;
            }
            else if (fcard is Jink)
            {
                WrappedCard slash = new WrappedCard(FanghunCard.ClassName)
                {
                    UserString = Slash.ClassName
                };
                slash.AddSubCard(originalCard);
                return slash;
            }
            else
                return null;
        }
    }

    public class Fanghun : TriggerSkill
    {
        public Fanghun() : base("fanghun")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.Damaged };
            view_as_skill = new FanghunVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.AddMark(Name);
            room.SetPlayerStringMark(ask_who, "meiying", ask_who.GetMark(Name).ToString());
            room.NotifySkillInvoked(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            return false;
        }
    }

    public class FanghunDetach : DetachEffectSkill
    {
        public FanghunDetach() : base("fanghun", string.Empty) { }

        public override void OnSkillDetached(Room room, Player player, object data)
        {
            room.RemovePlayerStringMark(player, "meiying");
        }
    }

    public class FanghunCard : SkillCard
    {
        public static string ClassName = "FanghunCard";
        public FanghunCard() : base(ClassName)
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
            use.From.AddMark("fanghun", -1);
            if (use.From.GetMark("fanghun") > 0)
                room.SetPlayerStringMark(use.From, "meiying", use.From.GetMark("fanghun").ToString());
            else
                room.RemovePlayerStringMark(use.From, "meiying");

            use.From.AddMark("fuhan");
            room.BroadcastSkillInvoke("fanghun", use.From, use.Card.SkillPosition);
            WrappedCard wrapped = new WrappedCard(use.Card.UserString) { Skill = "longdan_jx", SkillPosition = use.Card.SkillPosition, Mute = true };
            wrapped.AddSubCard(use.Card);
            wrapped = RoomLogic.ParseUseCard(room, wrapped);
            room.DrawCards(use.From, 1, "fanghun");
            return wrapped;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            player.AddMark("fanghun", -1);
            if (player.GetMark("fanghun") > 0)
                room.SetPlayerStringMark(player, "meiying", player.GetMark("fanghun").ToString());
            else
                room.RemovePlayerStringMark(player, "meiying");

            player.AddMark("fuhan");
            room.BroadcastSkillInvoke("fanghun", player, card.SkillPosition);
            WrappedCard wrapped = new WrappedCard(card.UserString) { Skill = "longdan_jx", SkillPosition = card.SkillPosition, Mute = true };
            wrapped.AddSubCard(card);
            wrapped = RoomLogic.ParseUseCard(room, wrapped);
            room.DrawCards(player, 1, "fanghun");
            return wrapped;
        }
    }

    public class Fuhan : PhaseChangeSkill
    {
        public Fuhan() : base("fuhan")
        {
            frequency = Frequency.Limited;
            skill_type = SkillType.Wizzard;
            limit_mark = "@fuhan";
        }
        public override bool CanPreShow() => false;
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.RoundStart && target.GetMark(limit_mark) > 0 && target.GetMark(Name) + target.GetMark("fanghun") > 0;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.SetPlayerMark(player, limit_mark, 0);
                room.RemovePlayerStringMark(player, "meiying");
                player.AddMark(Name, player.GetMark("fanghun"));
                player.SetMark("fanghun", 0);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            int max = player.GetMark(Name);
            room.DrawCards(player, max, Name);

            List<string> available = new List<string>();
            foreach (string name in room.Generals)
                if (!name.StartsWith("lord_") && !room.UsedGeneral.Contains(name) && Engine.GetGeneral(name, room.Setting.GameMode).Kingdom == player.Kingdom)
                    available.Add(name);

            if (available.Count > 0)
            {
                string from_general = player.ActualGeneral1;
                room.DoAnimate(AnimateType.S_ANIMATE_REMOVE, player.Name, true.ToString());

                Shuffle.shuffle(ref available);
                List<string> generals = new List<string>();
                for (int i = 0; i < Math.Min(5, available.Count); i++)
                    generals.Add(available[i]);

                string general_name = room.AskForGeneral(player, generals, null, true, Name, null, true, true);
                room.HandleUsedGeneral(general_name);
                room.HandleUsedGeneral("-" + from_general);

                General general = Engine.GetGeneral(general_name, room.Setting.GameMode);
                player.ActualGeneral1 = player.General1 = general_name;
                player.HeadSkinId = 0;
                room.BroadcastProperty(player, "HeadSkinId");
                room.NotifyProperty(room.GetClient(player), player, "ActualGeneral1");
                room.BroadcastProperty(player, "General1");
                player.PlayerGender = general.GeneralGender;
                room.BroadcastProperty(player, "PlayerGender");

                List<string> skills = player.GetSkills(true, false);
                foreach (string skill in skills)
                {
                    Skill _s = Engine.GetSkill(skill);
                    if (_s != null && !_s.Attached_lord_skill)
                        room.DetachSkillFromPlayer(player, skill, false, player.GetAcquiredSkills().Contains(skill), true);
                }

                foreach (string skill in Engine.GetGeneralRelatedSkills("guansuo", room.Setting.GameMode))
                {
                    if (!room.Skills.Contains(skill))
                    {
                        room.Skills.Add(skill);
                        Skill main = Engine.GetSkill(skill);
                        if (main is TriggerSkill tskill)
                            room.RoomThread.AddTriggerSkill(tskill);
                    }

                    foreach (Skill _skill in Engine.GetRelatedSkills(skill))
                    {
                        if (!room.Skills.Contains(_skill.Name))
                        {
                            room.Skills.Add(_skill.Name);
                            if (_skill is TriggerSkill tskill)
                                room.RoomThread.AddTriggerSkill(tskill);
                        }
                    }
                }

                foreach (string skill_name in Engine.GetGeneralSkills(general_name, room.Setting.GameMode, true))
                {
                    Skill s = Engine.GetSkill(skill_name);
                    if (!room.Skills.Contains(skill_name))
                    {
                        room.Skills.Add(skill_name);
                        if (s is TriggerSkill tskill)
                            room.RoomThread.AddTriggerSkill(tskill);
                    }

                    foreach (Skill _skill in Engine.GetRelatedSkills(skill_name))
                    {
                        if (!room.Skills.Contains(_skill.Name))
                        {
                            room.Skills.Add(_skill.Name);
                            if (_skill is TriggerSkill tskill)
                                room.RoomThread.AddTriggerSkill(tskill);
                        }
                    }

                    if (s.LordSkill && player.GetRoleEnum() != PlayerRole.Lord) continue;
                    room.AddPlayerSkill(player, skill_name);
                    if (s != null && s.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(s.LimitMark))
                        room.SetPlayerMark(player, s.LimitMark, 1);

                    object data = new InfoStruct() { Info = skill_name, Head = true };
                    room.RoomThread.Trigger(TriggerEvent.EventAcquireSkill, room, player, ref data);
                }

                room.SendPlayerSkillsToOthers(player);
                room.FilterCards(player, player.GetCards("he"), true);
            }

            if (player.Alive)
            {
                if (max > player.MaxHp)
                {
                    int count = max - player.MaxHp;
                    player.MaxHp = max;
                    room.BroadcastProperty(player, "MaxHp");
                    LogMessage log = new LogMessage
                    {
                        Type = "$GainMaxHp",
                        From = player.Name,
                        Arg = count.ToString()
                    };
                    room.SendLog(log);

                    room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, player);
                }
                else if (max < player.MaxHp)
                {
                    room.LoseMaxHp(player, player.MaxHp - max);
                }

                int hp = 100;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.Hp < hp)
                        hp = p.Hp;
                }

                if (player.Hp == hp && player.IsWounded())
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = player
                    };
                    room.Recover(player, recover, true);
                }
            }

            return false;
        }
    }

    public class Zishu : TriggerSkill
    {
        public Zishu() : base("zishu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseStart };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && base.Triggerable(move.To, room)
                && move.To.Phase == PlayerPhase.NotActive && move.To_place == Place.PlaceHand)
            {
                List<int> ids = move.To.ContainsTag(Name) ? (List<int>)move.To.GetTag(Name) : new List<int>();
                foreach (int id in move.Card_ids)
                    if (!ids.Contains(id))
                        ids.Add(id);

                move.To.SetTag(Name, ids);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.ContainsTag(Name))
                        triggers.Add(new TriggerStruct(Name, p));
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && base.Triggerable(move.To, room)
                && move.To == room.Current && move.Reason.SkillName != Name && move.To_place == Place.PlaceHand)
                triggers.Add(new TriggerStruct(Name, move.To));

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.EventPhaseStart && ask_who.GetTag(Name) is List<int> ids)
            {
                ask_who.RemoveTag(Name);
                List<int> discard = new List<int>();
                foreach (int id in ids)
                {
                    if (room.GetCardOwner(id) == ask_who && room.GetCardPlace(id) == Place.PlaceHand)
                        discard.Add(id);
                }
                if (discard.Count > 0)
                {
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    CardsMoveStruct move = new CardsMoveStruct(discard, null, Place.DiscardPile, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, ask_who.Name, Name, string.Empty));
                    room.MoveCardsAtomic(move, true);
                }
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.DrawCards(ask_who, 1, Name);
            }


            return false;
        }
    }

    public class Yingyuan : TriggerSkill
    {
        public Yingyuan() : base("yingyuan")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile
                && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_USE)
            {
                Player from = room.FindPlayer(move.Reason.PlayerId);
                WrappedCard card = RoomLogic.ParseCard(room, move.Reason.CardString);
                string str = string.Format("{0}_{1}", Name, card.Name);
                if (base.Triggerable(from, room) && from == room.Current && !from.HasFlag(str))
                {
                    foreach (int id in move.Card_ids)
                        if (room.GetCardPlace(id) == Place.DiscardPile)
                            return new TriggerStruct(Name, from);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                List<int> ids = new List<int>();
                foreach (int id in move.Card_ids)
                    if (room.GetCardPlace(id) == Place.DiscardPile)
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    WrappedCard card = RoomLogic.ParseCard(room, move.Reason.CardString);
                    room.SetTag(Name, ids);
                    Player target = room.AskForPlayerChosen(ask_who, room.GetOtherPlayers(ask_who), Name, "@yingyuan:::" + card.Name, true, true, info.SkillPosition);
                    room.RemoveTag(Name);
                    if (target != null)
                    {
                        room.SetTag(Name, target);
                        room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            if (data is CardsMoveOneTimeStruct move)
            {
                List<int> ids = new List<int>();
                foreach (int id in move.Card_ids)
                    if (room.GetCardPlace(id) == Place.DiscardPile)
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    ResultStruct result = ask_who.Result;
                    result.Assist += ids.Count;
                    ask_who.Result = result;

                    WrappedCard card = RoomLogic.ParseCard(room, move.Reason.CardString);
                    string str = string.Format("{0}_{1}", Name, card.Name);
                    ask_who.SetFlags(str);

                    room.ObtainCard(target, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, ask_who.Name, target.Name, Name, string.Empty));
                }
            }

            return false;
        }
    }

    public class ZiyuanCard : SkillCard
    {
        public static string ClassName = "ZiyuanCard";
        public ZiyuanCard() : base(ClassName)
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
            return targets.Count == 1 && card.SubCards.Count > 0;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, card_use.From.Name, target.Name, "ziyuan", string.Empty);

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            room.ObtainCard(target, card_use.Card, reason, false);
            if (target.Alive && target.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = card_use.From
                };
                room.Recover(target, recover, true);
            }
        }
    }

    public class Ziyuan : ViewAsSkill
    {
        public Ziyuan() : base("ziyuan")
        {
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (player.HasEquip(to_select.Name)) return false;
            int number = 0;
            foreach (WrappedCard card in selected)
                number += card.Number;

            return number + to_select.Number <= 13;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ZiyuanCard.ClassName) && !player.IsKongcheng();
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                int number = 0;
                foreach (WrappedCard card in cards)
                    number += card.Number;

                if (number == 13)
                {
                    WrappedCard rende_card = new WrappedCard(ZiyuanCard.ClassName);
                    rende_card.AddSubCards(cards);
                    rende_card.Skill = Name;
                    rende_card.ShowSkill = Name;
                    return rende_card;
                }
            }

            return null;
        }
    }

    public class Jujia : TriggerSkill
    {
        public Jujia() : base("jujia")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change
                && change.To == PlayerPhase.Discard && RoomLogic.GetMaxCards(room, player) > player.Hp && player.HandcardNum > player.Hp)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.GameStart)
                return info;
            
            room.NotifySkillInvoked(player, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);

            room.DrawCards(player, player.MaxHp, Name);

            return false;
        }
    }

    public class JujiaMax : MaxCardsSkill
    {
        public JujiaMax() : base("#jujia-max") { }

        public override int GetExtra(Room room, Player target)
        {
            if (RoomLogic.PlayerHasSkill(room, target, "jujia"))
                return target.MaxHp;

            return 0;
        }
    }

    public class Hongyuan : DrawCardsSkill
    {
        public Hongyuan() : base("hongyuan")
        {
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = room.AskForPlayersChosen(player, room.GetOtherPlayers(player), Name, 0, 2, "@hongyuan", true, info.SkillPosition);
            if (targets.Count > 0 && data is int count)
            {
                count--;
                data = count;
                room.SetTag(Name, targets);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            List<Player> targets = (List<Player>)room.GetTag(Name);
            room.RemoveTag(Name);
            foreach (Player p in targets)
                room.DrawCards(p, new DrawCardStruct(1, player, Name));

            return n;
        }
    }

    public class Huanshi : TriggerSkill
    {
        public Huanshi() : base("huanshi")
        {
            events.Add(TriggerEvent.AskForRetrial);
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && !player.IsKongcheng())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is JudgeStruct judge && judge.Who.Alive)
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodResponse, true))
                        ids.Add(id);
                }

                if (ids.Count < player.HandcardNum)
                {
                    room.SetTag(Name, data);
                    bool invoke = room.AskForSkillInvoke(player, Name, judge.Who, info.SkillPosition);
                    room.RemoveTag(Name);
                    if (invoke)
                    {
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                        room.SetTag(Name, data);
                        int id = room.AskForCardChosen(judge.Who, player, "he", Name, true, HandlingMethod.MethodResponse, ids);
                        room.RemoveTag(Name);
                        room.Retrial(room.GetCard(id), player, ref judge, Name, false, info.SkillPosition);
                        data = judge;
                        return info;
                    }
                }
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

    public class Mingzhe : TriggerSkill
    {
        public Mingzhe() : base("mingzhe")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && (move.From_places.Contains(Place.PlaceEquip) || move.From_places.Contains(Place.PlaceHand))
                && base.Triggerable(move.From, room) && (move.To != move.From || move.To_place != Place.PlaceHand) && move.From.Phase == PlayerPhase.NotActive
                && ((move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_RESPONSE
                ||(move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_USE
                || (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD))
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if ((move.From_places[i] == Place.PlaceEquip || move.From_places[i] == Place.PlaceHand) && WrappedCard.IsRed(room.GetCard(move.Card_ids[i]).Suit))
                        return new TriggerStruct(Name, move.From);
                }
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
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    public class Aocai : TriggerSkill
    {
        public Aocai() : base("aocai")
        {
            skill_type = SkillType.Defense;
            view_as_skill = new AocaiVS();
            events.Add(TriggerEvent.DrawPileChanged);
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<Player> zhugeke = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player p in zhugeke)
                p.Piles["#aocai"] = new List<int>();
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class AocaiVS : ViewAsSkill
    {
        public AocaiVS() : base("aocai")
        {
            expand_pile = "#aocai";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (player.Phase != PlayerPhase.NotActive)
                return false;

            foreach (FunctionCard fcard in room.AvailableFunctionCards)
            {
                if (!(fcard is BasicCard)) continue;
                WrappedCard card = new WrappedCard(fcard.Name);
                if (Engine.MatchExpPattern(room, pattern, player, card))
                    return true;
            }

            return false;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count == 0 && player.GetPile(expand_pile).Contains(to_select.Id)
                && Engine.MatchExpPattern(room, room.GetRoomState().GetCurrentCardUsePattern(player), player, to_select);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (player.GetPile(expand_pile).Count == 0)
                return new WrappedCard(AocaiCard.ClassName) { Skill = Name, Mute = true };
            else if (cards.Count == 1)
                return cards[0];

            return null;
        }
    }

    public class AocaiCard : SkillCard
    {
        public static string ClassName = "AocaiCard";
        public AocaiCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            View(room, use.From, use.Card.SkillPosition);
            return null;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            View(room, player, card.SkillPosition);
            return null;
        }

        private void View(Room room, Player player, string head)
        {
            List<int> guanxing = room.GetNCards(2, false);
            room.NotifySkillInvoked(player, "aocai");
            room.BroadcastSkillInvoke("aocai", player, head);
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

            player.Piles["#aocai"] = guanxing;
            room.SetPromotSkill(player, "aocai", head, room.GetRoomState().GetCurrentCardUsePattern(), room.GetRoomState().GetCurrentCardUseReason());
        }
    }

    public class Duwu : TriggerSkill
    {
        public Duwu() : base("duwu")
        {
            events.Add(TriggerEvent.QuitDying);
            skill_type = SkillType.Attack;
            view_as_skill = new DuwuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is DyingStruct dying && dying.Damage.From != null && !string.IsNullOrEmpty(dying.Damage.Reason) && dying.Damage.Reason == Name && dying.Damage.From.Alive
                && !dying.Damage.Transfer && !dying.Damage.Chain)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#duwu-lose",
                    From = dying.Damage.From.Name
                };

                dying.Damage.From.SetFlags(Name);
                room.LoseHp(dying.Damage.From);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class DuwuVS : ViewAsSkill
    {
        public DuwuVS() : base("duwu")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasFlag(Name);
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, to_select.Id);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            WrappedCard dw = new WrappedCard(DuwuCard.ClassName)
            {
                Skill = Name
            };
            dw.AddSubCards(cards);
            return dw;
        }
    }

    public class DuwuCard : SkillCard
    {
        public static string ClassName = "DuwuCard";
        public DuwuCard() : base(ClassName)
        {
            will_throw = true;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != null && RoomLogic.InMyAttackRange(room, Self, to_select, card) && to_select.Hp == card.SubCards.Count;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            room.Damage(new DamageStruct("duwu", card_use.From, card_use.To[0]));
        }
    }

    public class Hongde : TriggerSkill
    {
        public Hongde() : base("hongde")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                if (move.To != null && base.Triggerable(move.To, room) && move.To_place == Place.PlaceHand && move.Card_ids.Count >= 2)
                    return new TriggerStruct(Name, move.To);

                if (move.From != null && (move.To != move.From || move.To_place == Place.PlaceTable || move.To_place == Place.PlaceSpecial) && base.Triggerable(move.From, room))
                {
                    int count = 0;
                    foreach (Place place in move.From_places)
                        if (place == Place.PlaceHand || place == Place.PlaceEquip)
                            count++;

                    if (count >= 2)
                        return new TriggerStruct(Name, move.From);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(ask_who, room.GetOtherPlayers(ask_who), Name, "@hongde", true, true, info.SkillPosition);
            if (target != null)
            {
                ask_who.SetTag(Name, target.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer(ask_who.GetTag(Name).ToString());
            ask_who.RemoveTag(Name);
            room.DrawCards(target, new DrawCardStruct(1, ask_who, Name));
            return false;
        }
    }

    public class Dingpan : ZeroCardViewAsSkill
    {
        public Dingpan() : base("dingpan")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            int count = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.GetRoleEnum() == PlayerRole.Rebel)
                    count++;

            return player.UsedTimes(DingpanCard.ClassName) < count;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(DingpanCard.ClassName) { Skill = Name };
        }
    }

    public class DingpanCard : SkillCard
    {
        public static string ClassName = "DingpanCard";
        public DingpanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.GetEquips().Count > 0;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];

            room.DrawCards(target, new DrawCardStruct(1, player, "dingpan"));
            if (target.Alive)
            {
                List<string> choices = new List<string>(), prompt = new List<string> { string.Format("@dingpan:{0}", player.Name) };
                foreach (int id in target.GetEquips())
                {
                    if (!choices.Contains("discard") && RoomLogic.CanDiscard(room, player, target, id))
                        choices.Add("discard");

                    if (!choices.Contains("get") && RoomLogic.CanGetCard(room, player, target, id))
                        choices.Add("get");
                }

                if (choices.Count > 0)
                {
                    string choice = room.AskForChoice(target, "dingpan", string.Join("+", choices), prompt, player);
                    if (choice == "discard")
                    {
                        int id = room.AskForCardChosen(player, target, "e", "dingpan", false, HandlingMethod.MethodDiscard);
                        room.ThrowCard(id, target, target != player ? player : null);
                    }
                    else
                    {
                        List<int> ids = new List<int>();
                        foreach (int id in target.GetEquips())
                            if (RoomLogic.CanGetCard(room, player, target, id))
                                ids.Add(id);

                        if (ids.Count > 0)
                            room.ObtainCard(player, ref ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, "dingpan", string.Empty));

                        if (target.Alive)
                            room.Damage(new DamageStruct("dingpan", player, target));
                    }
                }
            }
        }
    }

    public class Xingwu : TriggerSkill
    {
        public Xingwu() : base("xingwu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }
        

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Discard && base.Triggerable(player, room) && !player.IsKongcheng())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.AskForExchange(player, Name, 1, 0, "@xingwu", string.Empty, ".", info.SkillPosition);
            if (ids.Count > 0)
            {
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.AddToPile(player, Name, ids);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
            List<int> ids = player.GetPile(Name), discard = new List<int>();
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (!suits.Contains(card.Suit))
                {
                    suits.Add(card.Suit);
                    discard.Add(id);
                }
            }

            if (discard.Count >= 3)
            {
                room.ThrowCard(ref discard, player);
                Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@xingwu-target", false, true, info.SkillPosition);
                if (!target.IsAllNude())
                {
                    List<int> all = new List<int>();
                    foreach (int id in target.GetCards("e"))
                        if (RoomLogic.CanDiscard(room, player, target, id))
                            all.Add(id);

                    if (all.Count > 0)
                        room.ThrowCard(ref all, target, player);
                }

                if (player.Alive && target.Alive)
                    room.Damage(new DamageStruct(Name, player, target, target.IsMale() ? 2 : 1));
            }

            return false;
        }
    }

    public class XingwuClear : DetachEffectSkill
    {
        public XingwuClear() : base("xingwu", "xingwu") { }
    }

    public class Luoyan : ViewHasSkill
    {
        public Luoyan() : base("luoyan")
        {
            viewhas_skills = new List<string> { "liuli", "tianxiang_jx" };
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (player.Alive && RoomLogic.PlayerHasSkill(room, player, Name) && player.GetPile("xingwu").Count > 0)
                return true;
            return false;
        }
    }

    public class Meibu : TriggerSkill
    {
        public Meibu() : base("meibu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Wizzard;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.Play)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    if (p != player && RoomLogic.InMyAttackRange(room, player, p))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.AskForExchange(ask_who, Name, 1, 0, "@meibu:" + player.Name, string.Empty, "..!", info.SkillPosition);
            if (ids.Count == 1)
            {
                room.ThrowCard(ref ids, ask_who, null, Name);
                if (ids.Count == 1)
                {
                    WrappedCard card = room.GetCard(ids[0]);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!(fcard is Slash || (fcard is TrickCard && WrappedCard.IsBlack(card.Suit))))
                    {
                        if (player.Alive && ask_who.Alive)
                        {
                            player.SetFlags("MeibuFrom");
                            ask_who.SetFlags("MeibuTo");
                        }
                    }
                }
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive)
                room.HandleAcquireDetachSkills(player, "zhixi", true);

            return false;
        }
    }
    public class MeibuDistance : DistanceSkill
    {
        public MeibuDistance() : base("#meibu-dis")
        {
        }
        public override int GetFixed(Room room, Player from, Player to)
        {
            if (from.HasFlag("MeibuFrom") && to.HasFlag("MeibuTo"))
                return 1;
            else
                return 0;
        }
    }
    public class Mumu : PhaseChangeSkill
    {
        public Mumu() : base("mumu")
        {
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Play;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            if (player.HasEquip() && RoomLogic.CanGetCard(room, player, player, "e")) targets.Add(player);
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HasEquip() && (RoomLogic.CanDiscard(room, player, p, "e") || RoomLogic.CanGetCard(room, player, p, "e")))
                    targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@mumu", true, true, info.SkillPosition);
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
            Player target = room.FindPlayer(player.GetTag(Name).ToString());
            List<int> ids = new List<int>();
            if (target == player)
            {
                foreach (int id in target.GetEquips())
                    if (RoomLogic.CanGetCard(room, player, player, id)) ids.Add(id);

                List<int> get = room.AskForExchange(player, Name, 1, 1, "@mumu-get", string.Empty, string.Join("#", JsonUntity.IntList2StringList(ids)), info.SkillPosition);
                room.ObtainCard(player, ref get, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty));
                RoomLogic.SetPlayerCardLimitation(player, "use,response", "Slash", true);
            }
            else
            {
                foreach (int id in target.GetEquips())
                    if (!RoomLogic.CanGetCard(room, player, target, id) && !RoomLogic.CanDiscard(room, player, target, id)) ids.Add(id);

                int card_id = room.AskForCardChosen(player, target, "e", Name, false, HandlingMethod.MethodNone, ids);
                List<string> choices = new List<string>();
                if (RoomLogic.CanGetCard(room, player, target, card_id)) choices.Add("get");
                if (RoomLogic.CanDiscard(room, player, target, card_id)) choices.Add("discard");
                if (room.AskForChoice(player, Name, string.Join("+", choices), null, card_id) == "get")
                {
                    room.ObtainCard(player, card_id);
                    RoomLogic.SetPlayerCardLimitation(player, "use,response", "Slash", true);
                }
                else
                    room.ThrowCard(card_id, target, player);
            }

            return false;
        }
    }

    public class Zhixi : TriggerSkill
    {
        public Zhixi() : base("zhixi")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                {
                    player.AddMark(Name);
                    if (!player.HasFlag(Name) && (player.GetMark(Name) >= player.Hp || fcard is TrickCard))
                    {
                        RoomLogic.SetPlayerCardLimitation(player, "use", ".", true);
                        player.SetFlags(Name);
                    }
                }

            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && base.Triggerable(player, room))
            {
                player.AddMark(Name);
                if (!player.HasFlag(Name) && player.GetMark(Name) >= player.Hp)
                {
                    RoomLogic.SetPlayerCardLimitation(player, "use", ".", true);
                    player.SetFlags(Name);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play)
            {
                room.HandleAcquireDetachSkills(player, "-zhixi", true);
                if (player.HasFlag(Name))
                    RoomLogic.RemovePlayerCardLimitation(player, "use", ".$0");
                if (player.GetMark(Name) > 0)
                    player.SetMark(Name, 0);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
}
