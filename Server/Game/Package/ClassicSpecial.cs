using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
                new Juesi(),
                new Mashu("pangde_sp"),
                new Qingzhong(),
                new QingzhongExchange(),
                new Weijing(),
                new Xingzhao(),
                new XingzhaoVH(),

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
                new Bingzhao(),
                new Qiaomeng(),
                new Yicong(),
                new YicongDis(),
                new Jiqiao(),
                new Linglong(),
                new LinglongVH(),
                new LinglongTar(),
                new LinglongMax(),
                new LinglongFix(),
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
                new Biluan(),
                new BiluanDistance(),
                new Lixia(),
                new LixiaDistance(),
                new HuojiSM(),
                new LianhuanSM(),
                new YeyanSM(),
                new Jianjie(),
                new JianjieMove(),
                new Chenghao(),
                new Yinshi(),
                new Lueming(),
                new Tunjun(),
                new Xingluan(),
                new Sidao(),
                new SidaoRecord(),
                new Tanbei(),
                new TanbeiPro(),
                new TanbeiTar(),
                new Wenji(),
                new WenjiEffect(),
                new Tunjiang(),
                new Zhidao(),
                new ZhidaoPro(),
                new JiliYbh(),
                new Lianji(),
                new Moucheng(),
                new Jingong(),
                new Zhoufu(),
                new ZhoufuEffect(),
                new Yingbing(),
                new Lianzhu(),
                new Xiahui(),
                new XiahuiMax(),
                new Neifa(),
                new NeifaTar(),
                new NeifaPro(),
                new ZhenduJX(),
                new QiluanJX(),
                new QiluanJXClear(),
                new BeizhanClassic(),
                new BeizhanCProhibit(),
                new GangzhiClassic(),

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
                new Zhuhai(),
                new ZhuhaiTag(),
                new Qianxin(),
                new Jianyan(),
                new Qianya(),
                new Shuimeng(),
                new Fuman(),
                new Bingzheng(),
                new Sheyan(),
                new Baobian(),
                new BaobianVH(),
                new Dianhu(),
                new Jianji(),
                new Yuxu(),
                new YuxuDiscard(),
                new Shijian(),

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
                new Lianpian(),
                new LianpianRecord(),
                new Yinbing(),
                new YinbingDamage(),
                new Juedi(),
                new CanshiSH(),
                new CanshiDiscard(),
                new Chouhai(),
                new Guiming(),
                new Xiashu(),
                new Kuanshi(),
                new KuanshiIm(),
                new Qizhou(),
                new QizhouVH(),
                new Mashu("heqi"),
                new Yingzi("heqi", false),
                new YingziMax("heqi"),
                new Shanxi(),
                new Bizheng(),
                new Yidian(),
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
                new JianyanCard(),
                new QianyaCard(),
                new FumanCard(),
                new JuesiCard(),
                new YeyanSMCard(),
                new JianjieCard(),
                new LuemingCard(),
                new TunjunCard(),
                new SidaoCard(),
                new TanbeiCard(),
                new WeijingCard(),
                new ShanxiCard(),
                new LianjiCard(),
                new JianjiCard(),
                new ZhoufuCard(),
                new LianzhuCard(),
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
                { "linglong", new List<string>{ "#linglong-max", "#linglong-tar", "#linglongvh", "#linglong-fix" } },
                { "xingwu", new List<string>{ "#xingwu-clear" } },
                { "yicong", new List<string>{ "#yicong" } },
                { "weikui", new List<string>{ "#weikui-dis" } },
                { "meibu", new List<string>{ "#meibu-dis" } },
                { "moukui", new List<string>{ "#moukui" } },
                { "jujia", new List<string>{ "#jujia-max" } },
                { "xionghuo", new List<string>{ "#xionghuo", "#xionghuo-max", "#xionghuo-prohibit" } },
                { "tuifeng", new List<string>{ "#tuifeng", "#tuifeng-tar" } },
                { "falu", new List<string>{ "#falu" } },
                { "zhuhai", new List<string>{ "#zhuhai" } },
                { "biluan", new List<string>{ "#biluan" } },
                { "lixia", new List<string>{ "#lixia" } },
                { "lianpian", new List<string>{ "#lianpian" } },
                { "yinbing", new List<string>{ "#yinbing" } },
                { "jianjie", new List<string>{ "#jianjie" } },
                { "canshi_sh", new List<string>{ "#canshi-discard" } },
                { "kuanshi", new List<string>{ "#kuanshi" } },
                { "sidao", new List<string>{ "#sidao" } },
                { "tanbei", new List<string>{ "#tanbei-target", "#tanbei-prohibit" } },
                { "qingzhong", new List<string>{ "#qingzhong" } },
                { "baobian", new List<string>{ "#baobian" } },
                { "qizhou", new List<string>{ "#qizhou" } },
                { "zhidao", new List<string>{ "#zhidao" } },
                { "xingzhao", new List<string>{ "#xingzhao" } },
                { "zhoufu", new List<string>{ "#zhoufu" } },
                { "xiahui", new List<string>{ "#xiahui-max" } },
                { "wenji", new List<string>{ "#wenji" } },
                { "zhenlue", new List<string>{ "#zhenlue" } },
                { "yuxu", new List<string>{ "#yuxu" } },
                { "neifa", new List<string>{ "#neifa-tar", "#neifa-pro" } },
                { "qiluan_jx", new List<string>{ "#qiluan_jx" } },
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
                room.AskForUseCard(player, "@@shefu", "@shefu", null, -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
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
                        CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, p.Name, string.Empty, Name, string.Empty)
                        {
                            General = gsk
                        };
                        room.MoveCardTo(room.GetCard(id), p, null, Place.DiscardPile, string.Empty, reason, true);

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
                if (p.GetRoleEnum() == PlayerRole.Lord && p.General1.Contains("liubei")) return false;

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
            WrappedCard card = room.AskForUseCard(player, "@@yuanhu", "@huyuan-equip", null, -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
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
                            int card_id = room.AskForCardChosen(caohong, to_dismantle, "hej", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
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
                        CardMoveReason reasonG = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, chenlin.Name, Name, string.Empty);
                        room.ObtainCard(chenlin, ref to_give, reasonG, false);
                        CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty);
                        List<int> card_ids = new List<int> { card_id };
                        room.ObtainCard(player, ref card_ids, reason, false);
                    }
                }

                room.ClearAG(player);
                if (to_give.Count == 0)
                {
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, string.Empty, Name, string.Empty);
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
            WrappedCard card = room.AskForUseCard(player, "@@bifa", "@bifa-remove", null, -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
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
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name, new List<Player> { player });
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

            if (WrappedCard.IsBlack(judge.JudgeSuit))
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
            events = new List<TriggerEvent> { TriggerEvent.Dying };
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && player.HasFlag("Global_Dying"))
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p != player && room.Round > p.GetMark(Name))
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player dyer, ref object data, Player player, TriggerStruct info)
        {
            if (data is DyingStruct dying && room.Round > player.GetMark(Name))
            {
                List<Player> targets = room.GetOtherPlayers(player);
                targets.Remove(dyer);
                if (targets.Count > 0)
                {
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
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player dyer, ref object data, Player player, TriggerStruct info)
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
                    room.AskForUseCard(player, "@@moshi", string.Format("@moshi:::{0}", card_name), null, -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
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
                if (move.From != null && move.From_places.Contains(Place.PlaceEquip) && move.From.GetMark(Name) < 3)
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
                    room.AskForUseCard(player, "@@shanjia", "@shanjia-slash", null, -1, HandlingMethod.MethodUse, false, info.SkillPosition);
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
                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, ask_who.Name, player.Name, Name, string.Empty));
                if (ids.Count == 1 && player.Alive)
                {
                    WrappedCard card = room.GetCard(ids[0]);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, player, card))
                        room.AskForUseCard(player, ids[0].ToString(), "@kangkai-use", null);
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

                        room.ObtainCard(p, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, p.Name, Name, string.Empty));
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
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
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
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0 && player.MaxHp > room.AliveCount() && player.IsWounded())
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
                room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, ask_who.Name, Name, string.Empty), false);
                int count = ids.Count;

                if (count > 0 && ask_who.Alive && player.Alive)
                {
                    List<int> give = room.AskForExchange(ask_who, Name, count, count, string.Format("@junbing-give:{0}::{1}", player.Name, count), string.Empty, "..", info.SkillPosition);
                    room.ObtainCard(player, ref give, new CardMoveReason(MoveReason.S_REASON_GIVE, ask_who.Name, player.Name, Name, string.Empty), false);
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
            CardsMoveStruct move1 = new CardsMoveStruct(ids, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, player.Name, "tuifeng", string.Empty));
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
            WrappedCard js = new WrappedCard(JianshuCard.ClassName);
            js.AddSubCard(card);
            return js;
        }
    }

    public class JianshuCard : SkillCard
    {
        public static string ClassName = "JianshuCard";
        public JianshuCard() : base(ClassName) { will_throw = false; }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (to_select == Self || targets.Count >= 2) return false;
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
            room.ObtainCard(from, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, from.Name, "mizhao", string.Empty), false);

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
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Recover;
            frequency = Frequency.Limited;
            limit_mark = "@yongdi";
        }
        
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.RoundStart && player.GetMark(limit_mark) > 0)
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

                if (target.Alive && target.IsWounded())
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = player,
                        Recover = 1
                    };
                    room.Recover(target, recover, true);
                }

                if (target.Alive && target.GetRoleEnum() != PlayerRole.Lord)
                {
                    foreach (string skill_name in Engine.GetGeneralSkills(target.General1, room.Setting.GameMode, true))
                    {
                        Skill s = Engine.GetSkill(skill_name);
                        if (s.LordSkill)
                        {
                            room.AddPlayerSkill(target, skill_name);
                            if (s != null && s.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(s.LimitMark))
                                room.SetPlayerMark(target, s.LimitMark, 1);

                            object _data = new InfoStruct() { Info = skill_name, Head = true };
                            room.RoomThread.Trigger(TriggerEvent.EventAcquireSkill, room, target, ref _data);
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
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.RoundStart && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.HandleAcquireDetachSkills(player, "-jianxiong_jx|-xingshang", true);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-lingren");
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
                    string basic_choice = room.AskForChoice(player, Name, "has+nohas", new List<string> { "@lingren-basic:" + target.Name }, target);
                    player.SetFlags("-lingren_basic");

                    if ((basic_choice == "has" && basic) || (basic_choice != "has" && !basic))
                        correct++;

                    player.SetFlags("lingren_equip");
                    string equip_choice = room.AskForChoice(player, Name, "has+nohas", new List<string> { "@lingren-equip:" + target.Name }, target);
                    player.SetFlags("-lingren_equip");

                    if ((equip_choice == "has" && equip) || (equip_choice != "has" && !equip))
                        correct++;

                    player.SetFlags("lingren_trick");
                    string trick_choice = room.AskForChoice(player, Name, "has+nohas", new List<string> { "@lingren-trick:" + target.Name }, target);
                    player.SetFlags("-lingren_trick");

                    if ((trick_choice == "has" && trick) || (trick_choice != "has" && !trick))
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
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            int min = 1000;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < min) min = p.HandcardNum;

            if (min > 0)
            {
                List<Player> targets = room.GetOtherPlayers(player);
                Shuffle.shuffle(ref targets);
                Player target = targets[0];

                List<int> ids = target.GetCards("h"), views = new List<int>(); ;
                Shuffle.shuffle(ref ids);
                for (int i = 0; i < Math.Min(min, ids.Count); i++)
                    views.Add(ids[i]);

                room.DoGongxin(player, target, views, new List<int>(), Name, "@to-player:" + target.Name, info.SkillPosition);
            }

            return false;
        }
    }

    public class Juesi : OneCardViewAsSkill
    {
        public Juesi() : base("juesi") { skill_type = SkillType.Attack; filter_pattern = "Slash!"; }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard js = new WrappedCard(JuesiCard.ClassName) { Skill = Name };
            js.AddSubCard(card);
            return js;
        }
    }

    public class JuesiCard : SkillCard
    {
        public static string ClassName = "JuesiCard";
        public JuesiCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && RoomLogic.InMyAttackRange(room, Self, to_select, card) && RoomLogic.CanDiscard(room, to_select, to_select, "he");
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = target.GetCards("he");
            if (ids.Count > 1)
                ids = room.AskForExchange(target, "juesi", 1, 1, "@juesi:" + player.Name, string.Empty, "..!", string.Empty);
            
            if (ids.Count > 0)
            {
                bool slash = room.GetCard(ids[0]).Name.Contains(Slash.ClassName);
                room.ThrowCard(ref ids, target);

                if (!slash && player.Alive && target.Alive && player.Hp <= target.Hp)
                {
                    WrappedCard duel = new WrappedCard(Duel.ClassName) { Skill = "_juesi" };
                    if (RoomLogic.IsProhibited(room, player, target, duel) == null)
                        room.UseCard(new CardUseStruct(duel, player, target));
                }
            }
        }
    }

    public class Qingzhong : TriggerSkill
    {
        public Qingzhong() : base("qingzhong")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Replenish;
        }
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Play;
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

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetTag(Name, info.SkillPosition);
            player.SetFlags(Name);
            room.DrawCards(player, 2, Name);
            return false;
        }
    }
    public class QingzhongExchange : TriggerSkill
    {
        public QingzhongExchange() : base("#qingzhong")
        {
            events.Add(TriggerEvent.EventPhaseEnd);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Play && player.HasFlag("qingzhong") && player.GetTag("qingzhong") is string position)
            {
                if (!player.IsKongcheng())
                {
                    TriggerStruct trigger = new TriggerStruct(Name, player)
                    {
                        SkillPosition = position
                    };
                    return trigger;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags("-qingzhong");
            player.RemoveTag("qingzhong");
            int less = 100000;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < less) less = p.HandcardNum;

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HandcardNum == less) targets.Add(p);

            if (targets.Count > 0)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "qingzhong", info.SkillPosition);
                room.BroadcastSkillInvoke("qingzhong", "male", 2, gsk.General, gsk.SkinId);

                bool option = player.HandcardNum == less;
                Player target = room.AskForPlayerChosen(player, targets, "qingzhong", "@qingzhong", option, true, info.SkillPosition);
                if (target != null)
                {
                    CardsMoveStruct move1 = new CardsMoveStruct(player.GetCards("h"), target, Place.PlaceHand,
                        new CardMoveReason(MoveReason.S_REASON_SWAP, player.Name, target.Name, "qingzhong", null));
                    CardsMoveStruct move2 = new CardsMoveStruct(target.GetCards("h"), player, Place.PlaceHand,
                        new CardMoveReason(MoveReason.S_REASON_SWAP, target.Name, player.Name, "qingzhong", null));
                    List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct> { move1, move2 };
                    room.MoveCards(exchangeMove, false);
                }
            }

            return false;
        }
    }

    public class Weijing : ZeroCardViewAsSkill
    {
        public Weijing() : base("weijing") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(Name) < room.Round && Slash.IsAvailable(room, player);
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (player.GetMark(Name) >= room.Round || room.GetRoomState().GetCurrentCardUseReason() != CardUseReason.CARD_USE_REASON_RESPONSE_USE) return false;
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            WrappedCard jink = new WrappedCard(Jink.ClassName);
            return Engine.MatchExpPattern(room, pattern, player, slash) || Engine.MatchExpPattern(room, pattern, player, jink);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY)
                return new WrappedCard(WeijingCard.ClassName) { UserString = Slash.ClassName };
            else
            {
                string pattern = room.GetRoomState().GetCurrentCardUsePattern(player);
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                if (Engine.MatchExpPattern(room, pattern, player, slash))
                    return new WrappedCard(WeijingCard.ClassName) { UserString = Slash.ClassName };
                else if (Engine.MatchExpPattern(room, pattern, player, jink))
                    return new WrappedCard(WeijingCard.ClassName) { UserString = Jink.ClassName };
            }

            return null;
        }
    }

    public class WeijingCard : SkillCard
    {
        public static string ClassName = "WeijingCard";
        public WeijingCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard real = new WrappedCard(card.UserString);
            FunctionCard fcard = Engine.GetFunctionCard(real.Name);
            return fcard.TargetFilter(room, targets, to_select, Self, real);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            WrappedCard real = new WrappedCard(card.UserString);
            FunctionCard fcard = Engine.GetFunctionCard(real.Name);
            return fcard.TargetsFeasible(room, targets, Self, real);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            WrappedCard real = new WrappedCard(use.Card.UserString) { Skill = "weijing", ShowSkill = "weijing" };
            use.From.SetMark("weijing", room.Round);
            return real;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            WrappedCard real = new WrappedCard(card.UserString) { Skill = "weijing", ShowSkill = "weijing" };
            player.SetMark("weijing", room.Round);
            return real;
        }
    }

    public class Xingzhao : TriggerSkill
    {
        public Xingzhao() : base("xingzhao")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is EquipCard)
                {
                    int count = 0;
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.IsWounded()) count++;

                    if (count >= 2)
                        return new TriggerStruct(Name, player);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.Discard && base.Triggerable(player, room))
            {
                int count = 0;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.IsWounded()) count++;

                if (count >= 3)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (triggerEvent == TriggerEvent.CardUsed)
                room.DrawCards(player, 1, Name);
            else
                room.SkipPhase(player, PlayerPhase.Discard, true);

            return false;
        }
    }

    public class XingzhaoVH : ViewHasSkill
    {
        public XingzhaoVH() : base("#xingzhao") { viewhas_skills = new List<string> { "xunxun" }; }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (RoomLogic.PlayerHasSkill(room, player, "xingzhao"))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.IsWounded()) return true;
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
        public MizhaoCard() : base(ClassName) { will_throw = false; }

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
            room.ObtainCard(from, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, from.Name, "mizhao", string.Empty), false);

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
                && move.From == room.Current && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD
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
                WrappedCard card = room.AskForUseCard(player, "@@weidi_jx", "@weidi-distribute", null, -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
                player.PileChange("#" + Name, cards, false);

                if (card != null && card.SubCards.Count > 0)
                {
                    Player target = room.FindPlayer((string)player.GetTag("lirang_target"), true);
                    target.SetFlags(Name);
                    player.RemoveTag("lirang_target");
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_PREVIEWGIVE, player.Name, target.Name, Name, null);
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
            room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, "lihun", string.Empty), false);

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

                    room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);
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
            room.ObtainCard(ask_who, room.GetCard(id), new CardMoveReason(MoveReason.S_REASON_EXTRACTION, ask_who.Name, player.Name, Name, string.Empty), false);

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
            int count = 1;

            if (ask_who.ContainsTag("bingzhao") && ask_who.GetTag("bingzhao") is string kingdom && kingdom == player.Kingdom && room.AskForSkillInvoke(player, "bingzhao", "@bingzhao:" + ask_who.Name))
                count++;

            room.DrawCards(ask_who, count, Name);
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
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.Reason.Reason == MoveReason.S_REASON_DRAW
                && move.Reason.SkillName == "guqu" && move.To.GetMark(Name) == 0)
                move.To.AddMark("baijia_got", move.Card_ids.Count);
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
                if (fcard is BasicCard || fcard.IsNDTrick())
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct _use && base.Triggerable(player, room) && _use.Card.ExtraTarget)
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                if (fcard is BasicCard || (fcard.IsNDTrick() && _use.Card.Name != Collateral.ClassName && !_use.Card.Name.Contains(Nullification.ClassName)))
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

    public class Bingzhao : TriggerSkill
    {
        public Bingzhao() : base("bingzhao")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart };
            lord_skill = true;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            List<string> kingdoms = new List<string>();
            foreach (Player p in room.GetAlivePlayers())
                if (!kingdoms.Contains(p.Kingdom))
                    kingdoms.Add(p.Kingdom);

            string choice = room.AskForChoice(player, Name, string.Join("+", kingdoms));
            room.SetPlayerStringMark(player, Name, choice);
            player.SetTag(Name, choice);
            foreach (Player p in room.GetAlivePlayers())
                if (p.Kingdom == choice)
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

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
                if (Engine.GetFunctionCard(room.GetCard(id).Name).TypeID != CardType.TypeEquip)
                    gets.Add(id);
                else
                    drops.Add(id);
                room.MoveCardTo(room.GetCard(id), player, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name, Name, null), false);
                Thread.Sleep(400);
            }
            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(gets, player, Place.PlaceHand, new CardMoveReason(MoveReason.S_REASON_GOTBACK, player.Name, Name, null)) },
                true);
            room.MoveCards(new List<CardsMoveStruct>{
                new CardsMoveStruct(drops, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, null, Name, null)) },
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
            pattern = "Slash#TrickCard";
            skill_type = SkillType.Wizzard;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeTrick && RoomLogic.PlayerHasSkill(room, from, "linglong") && !from.GetTreasure() ? true : false;
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            return Engine.GetFunctionCard(card.Name) is Slash && !from.GetWeapon() && RoomLogic.PlayerHasSkill(room, from, "linglong") ? 1 : 0;
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

    public class LinglongFix : FixCardSkill
    {
        public LinglongFix() : base("#linglong-fix") { }

        public override bool IsCardFixed(Room room, Player from, Player to, string flags, HandlingMethod method)
        {
            if (to != null && from != null && from != to && (flags == "t" || flags == "a")
                && method == HandlingMethod.MethodDiscard && RoomLogic.PlayerHasSkill(room, to, "linglong") && !to.GetTreasure())
                return true;

            return false;
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
            if (base.Triggerable(player, room) && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash && use.Card.ExtraTarget)
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
                List<Player> players = room.AskForPlayersChosen(player, targets, Name, 0, Math.Max(1, player.GetLostHp()),
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
                if (use.Card.Name.Contains(Slash.ClassName))
                {
                    foreach (Player p in use.To)
                        if (p != player && RoomLogic.DistanceTo(room, p, player, null, true) == 1)
                            return new TriggerStruct(Name, player);

                }
                if (Engine.GetFunctionCard(use.Card.Name).IsNDTrick())
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (RoomLogic.DistanceTo(room, p, player, null, true) == 1)
                            return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && base.Triggerable(effect.From, room) && player != effect.From
                && RoomLogic.DistanceTo(room, player, effect.From, null, true) == 1)
            {
                return new TriggerStruct(Name, effect.From);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                if (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == Duel.ClassName || use.Card.Name == Collateral.ClassName
                    || use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == SavageAssault.ClassName)
                {
                    for (int i = 0; i < use.EffectCount.Count; i++)
                    {
                        CardBasicEffect effect = use.EffectCount[i];
                        if (RoomLogic.DistanceTo(room, effect.To, player) == 1)
                        {
                            effect.Effect2 = 0;
                            if (!targets.Contains(effect.To))
                                targets.Add(effect.To);
                        }
                    }
                }

                if (Engine.GetFunctionCard(use.Card.Name).IsNDTrick())
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (RoomLogic.DistanceTo(room, p, player, null, true) == 1 && !targets.Contains(p))
                            targets.Add(p);

                if (targets.Count > 0)
                {
                    room.SortByActionOrder(ref targets);
                    foreach (Player p in targets)
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

                    room.SendCompulsoryTriggerLog(ask_who, Name);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
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
            string choice = room.AskForChoice(ask_who, Name, string.Join("+", choices), new List<string> { "@to-player:" + player.Name }, data);
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
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventLoseSkill };
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
            else if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct _info && _info.Info == Name)
            {
                room.ClearOnePrivatePile(player, "rice");
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
                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty));
            else if (data is DamageStruct damage)
                room.ObtainCard(damage.To, ref ids, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, damage.To.Name, Name, string.Empty));

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
                                    room.ObtainCard(ask_who, ref get, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, ask_who.Name, player.Name, Name, string.Empty), false);
                                break;
                        }

                        break;
                    }

                case TriggerEvent.DamageCaused when data is DamageStruct damage:
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, damage.To.Name);
                        room.BroadcastSkillInvoke("xionghuo", ask_who, info.SkillPosition);
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

            ask_who.AddMark("xionghuo");
            room.SetPlayerStringMark(ask_who, "cruel", ask_who.GetMark("xionghuo").ToString());

            if (data is DyingStruct dying && dying.Damage.Card != null)
            {
                List<int> ids = new List<int>();
                foreach (int id in dying.Damage.Card.SubCards)
                    if (room.GetCardPlace(id) == Place.DiscardPile || room.GetCardPlace(id) == Place.PlaceTable)
                        ids.Add(id);

                if (ids.Count > 0)
                    room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, ask_who.Name, Name, string.Empty));
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
                && move.To_place == Place.PlaceTable && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
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
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile && move.Reason.Reason == MoveReason.S_REASON_USE
                && move.Reason.Card != null && move.Reason.Card.SubCards.Count > 0)
            {
                Player target = room.FindPlayer(move.Reason.PlayerId);
                WrappedCard card = move.Reason.Card;
                List<int> ids = room.GetSubCards(card);
                if (card != null && Engine.GetFunctionCard(card.Name) is TrickCard && base.Triggerable(target, room) && target.Phase == PlayerPhase.Play && !target.HasFlag(Name)
                    && ids.SequenceEqual(card.SubCards) && ids.SequenceEqual(move.Card_ids))
                {
                    bool check = true;
                    foreach (int id in card.SubCards)
                    {
                        if (room.GetCardPlace(id) != Place.DiscardPile)
                        {
                            check = false;
                            break;
                        }
                    }

                    if (check) return new TriggerStruct(Name, target);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                WrappedCard card = move.Reason.Card;
                if (card.SubCards.Count > 0 && ask_who.Alive)
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
                List<int> ids = new List<int>(move.Reason.Card.SubCards);
                room.RemoveSubCards(move.Reason.Card);

                if (ids.Count > 0)
                {
                    ask_who.SetFlags(Name);
                    room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, ask_who.Name, Name, string.Empty));
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
            List<int> ids = new List<int> { 0, 0, 1, 1, 1, 2 };
            Shuffle.shuffle(ref ids);
            int result = ids[0];
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
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD && move.To_place == Place.DiscardPile)
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
                            room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_GOTCARD, ask_who.Name, Name, string.Empty), false);
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
            if (!RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse))
            {
                WrappedCard zy = new WrappedCard(ZhenyiCard.ClassName);
                zy.AddSubCard(card);
                return zy;
            }

            return null;
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

    public class Biluan : TriggerSkill
    {
        public Biluan() : base("biluan")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who)
        {
            if (base.Triggerable(lidian, room) && lidian.Phase == PlayerPhase.Finish && !lidian.IsNude())
                foreach (Player p in room.GetOtherPlayers(lidian))
                    if (RoomLogic.DistanceTo(room, p, lidian) == 1) return new TriggerStruct(Name, lidian);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = Math.Min(4, room.GetAlivePlayers().Count);
            if (room.AskForDiscard(lidian, Name, 1, 1, true, true, "@biluan:::" + count.ToString(), true, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, lidian, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = Math.Min(4, room.GetAlivePlayers().Count);
            lidian.AddMark(Name, count);
            int change = lidian.GetMark(Name) - lidian.GetMark("lixia");
            room.SetPlayerStringMark(lidian, "distance", change.ToString());
            return true;
        }
    }

    public class BiluanDistance : DistanceSkill
    {
        public BiluanDistance() : base("#biluan")
        {
        }
        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            if (RoomLogic.PlayerHasSkill(room, to, "biluan"))
                return to.GetMark("biluan");

            return 0;
        }
    }

    public class Lixia : TriggerSkill
    {
        public Lixia() : base("lixia")
        {
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
            events.Add(TriggerEvent.EventPhaseStart);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && player.Phase == PlayerPhase.Finish)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (player != p && !RoomLogic.InMyAttackRange(room, player, p)) triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);

            string choice = room.AskForChoice(ask_who, Name, "draw+letdraw", null, player);
            if (choice == "draw")
                room.DrawCards(ask_who, 1, Name);
            else
                room.DrawCards(player, new DrawCardStruct(2, ask_who, Name));

            if (ask_who.Alive)
            {
                ask_who.AddMark(Name);
                int change = ask_who.GetMark("biluan") - ask_who.GetMark(Name);
                room.SetPlayerStringMark(ask_who, "distance", change.ToString());
            }

            return false;
        }
    }

    public class LixiaDistance : DistanceSkill
    {
        public LixiaDistance() : base("#lixia")
        {
        }
        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            if (RoomLogic.PlayerHasSkill(room, to, "lixia"))
                return -to.GetMark("lixia");

            return 0;
        }
    }

    public class HuojiSM : OneCardViewAsSkill
    {
        public HuojiSM() : base("huoji_sm")
        {
            filter_pattern = ".|red|.|hand";
            response_or_use = true;
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => player.GetMark("@dragon") > 0 && player.UsedTimes("ViewAsSkill_huoji_smCard") < 3;
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard fire_attack = new WrappedCard(FireAttack.ClassName);
            fire_attack.AddSubCard(card);
            fire_attack.Skill = Name;
            fire_attack.ShowSkill = Name;
            fire_attack = RoomLogic.ParseUseCard(room, fire_attack);
            return fire_attack;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class LianhuanSM : OneCardViewAsSkill
    {
        public LianhuanSM() : base("lianhuan_sm")
        {
            filter_pattern = ".|club|.|hand";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => player.GetMark("@phenix") > 0 && player.UsedTimes("ViewAsSkill_lianhuan_smCard") < 3;
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

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class YeyanSM : ViewAsSkill
    {
        public YeyanSM() : base("yeyan_sm")
        {
            frequency = Frequency.Limited;
        }

        public override bool IsEnabledAtPlay(Room room, Player player) => player.GetMark("@phenix") > 0 && player.GetMark("@dragon") > 0;

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 3 || room.GetCardPlace(to_select.Id) != Place.PlaceHand || !RoomLogic.CanDiscard(room, player, player, to_select.Id))
                return false;

            if (selected.Count > 0)
            {
                List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
                foreach (WrappedCard card in selected)
                    suits.Add(card.Suit);

                return !suits.Contains(to_select.Suit);
            }

            return true;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0 || cards.Count == 4)
            {
                WrappedCard yy = new WrappedCard(YeyanSMCard.ClassName) { Skill = Name };
                yy.AddSubCards(cards);
                return yy;
            }

            return null;
        }
    }

    public class YeyanSMCard : SkillCard
    {
        public static string ClassName = "YeyanSMCard";
        public YeyanSMCard() : base(ClassName)
        {
            votes = true;
            will_throw = true;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count >= 3) return false;
            if (card.SubCards.Count == 0)
                return !targets.Contains(to_select);
            else if (card.SubCards.Count == 4 && targets.Count == 2)
                return targets.Contains(to_select) || targets[0] == targets[1];

            return true;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (targets.Count == 0) return false;
            if (card.SubCards.Count == 0)
                return targets.Count <= 3;
            else if (card.SubCards.Count == 4 && targets.Count > 1 && targets.Count <= 3)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Player p1 = targets[i];
                    for (int x = i + 1; x < targets.Count; x++)
                    {
                        Player p2 = targets[x];
                        if (p2 == p1)
                            return true;
                    }
                }
            }

            return false;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@phenix", 0);
            room.SetPlayerMark(card_use.From, "@dragon", 0);
            room.BroadcastSkillInvoke("yeyan", card_use.From, card_use.Card.SkillPosition);

            List<Player> targets = new List<Player>();
            for (int i = 0; i < card_use.To.Count; i++)
            {
                int count = 1;
                Player p1 = card_use.To[i];
                if (targets.Contains(p1)) continue;
                for (int x = i + 1; x < card_use.To.Count; x++)
                {
                    Player p2 = card_use.To[x];
                    if (p2 == p1)
                    {
                        count++;
                    }
                }

                p1.SetMark("yeyan_sm", count);
                targets.Add(p1);
            }

            card_use.To = targets;
            card_use.Card.Mute = true;
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            if (card_use.Card.SubCards.Count > 0)
                room.LoseHp(card_use.From, 3);

            base.Use(room, card_use);
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, target = effect.To;
            int count = target.GetMark("yeyan_sm");
            target.SetMark("yeyan_sm", 0);

            if (target.Alive)
                room.Damage(new DamageStruct("yeyan_sm", player, target, count, DamageStruct.DamageNature.Fire));
        }
    }

    public class JianjieVS : ZeroCardViewAsSkill
    {
        public JianjieVS() : base("jianjie")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => !player.HasUsed(JianjieCard.ClassName) && room.Round > 1;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern) => pattern.StartsWith("@@jianjie");
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JianjieCard.ClassName) { Skill = Name, Mute = true };
        }
    }

    public class Jianjie : TriggerSkill
    {
        public Jianjie() : base("jianjie")
        {
            skill_type = SkillType.Wizzard;
            events = new List<TriggerEvent> { TriggerEvent.EventLoseSkill, TriggerEvent.Death };
            view_as_skill = new JianjieVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct info && info.Info == Name)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark("@dragon") > 0 || p.GetMark("@phenix") > 0)
                    {
                        room.SetPlayerMark(p, "@dragon", 0);
                        room.SetPlayerMark(p, "@phenix", 0);
                        room.HandleAcquireDetachSkills(p, "-huoji_sm|-lianhuan_sm|-yeyan_sm", true);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.Death && RoomLogic.PlayerHasSkill(room, player, Name))
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark("@dragon") > 0 || p.GetMark("@phenix") > 0)
                    {
                        room.SetPlayerMark(p, "@dragon", 0);
                        room.SetPlayerMark(p, "@phenix", 0);
                        room.HandleAcquireDetachSkills(p, "-huoji_sm|-lianhuan_sm|-yeyan_sm", true);
                    }
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class JianjieMove : TriggerSkill
    {
        public JianjieMove() : base("#jianjie")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Death };
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.Death && (player.GetMark("@dragon") > 0 || player.GetMark("@phenix") > 0))
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p != player) triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                if (room.AskForUseCard(player, "@@jianjie!", "@jianjie-add", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition) == null)
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.GetMark("@dragon") == 0 && p.GetMark("@phenix") == 0)
                            targets.Add(p);

                        if (targets.Count >= 2) break;
                    }

                    if (targets.Count == 2)
                    {
                        WrappedCard jj = new WrappedCard(JianjieCard.ClassName) { Skill = "jianjie", Mute = true };
                        room.UseCard(new CardUseStruct(jj, player, targets));
                    }
                }
            }
            else
            {
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, "jianjie", info.SkillPosition);
            Random ra = new Random();
            int index = ra.Next(1, 3);
            room.BroadcastSkillInvoke("jianjie", "male", index, gsk.General, gsk.SkinId);

            if (player.GetMark("@dragon") > 0)
            {
                Player target = room.AskForPlayerChosen(ask_who, room.GetOtherPlayers(player), "jianjie", "@jianjie-dragon:" + player.Name, false, true, info.SkillPosition);
                room.BroadcastSkillInvoke("jianjie", player, info.SkillPosition);
                room.SetPlayerMark(player, "@dragon", 0);
                room.SetPlayerMark(target, "@dragon", 1);
                string skill = "huoji_sm";
                if (target.GetMark("@phenix") > 0)
                {
                    Thread.Sleep(1000);
                    skill = "huoji_sm|yeyan_sm";
                    room.BroadcastSkillInvoke("jianjie", "male", 3, gsk.General, gsk.SkinId);
                }
                room.HandleAcquireDetachSkills(target, skill, true);
            }
            if (player.GetMark("@phenix") > 0)
            {
                Player target = room.AskForPlayerChosen(ask_who, room.GetOtherPlayers(player), "jianjie", "@jianjie-phenix:" + player.Name, false, true, info.SkillPosition);
                room.BroadcastSkillInvoke("jianjie", player, info.SkillPosition);
                room.SetPlayerMark(player, "@phenix", 0);
                room.SetPlayerMark(target, "@phenix", 1);
                string skill = "lianhuan_sm";
                if (target.GetMark("@dragon") > 0)
                {
                    Thread.Sleep(1000);
                    skill = "lianhuan_sm|yeyan_sm";
                    room.BroadcastSkillInvoke("jianjie", "male", 3, gsk.General, gsk.SkinId);
                }
                room.HandleAcquireDetachSkills(target, skill, true);
            }

            return false;
        }
    }

    public class JianjieCard : SkillCard
    {
        public static string ClassName = "JianjieCard";
        public JianjieCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 1) return false;
            if (Self.Phase == PlayerPhase.Start)
                return to_select.GetMark("@dragon") == 0 && to_select.GetMark("@phenix") == 0 && to_select != Self;
            else
            {
                if (targets.Count == 0)
                    return to_select.GetMark("@dragon") > 0 || to_select.GetMark("@phenix") > 0;
                else
                    return (targets[0].GetMark("@dragon") > 0 && to_select.GetMark("@dragon") == 0)|| (targets[0].GetMark("@phenix") > 0 && to_select.GetMark("@phenix") == 0);
            }
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card) => targets.Count == 2;
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = new List<Player>(card_use.To);
            room.SortByActionOrder(ref targets);

            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            log.SetTos(targets);

            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "jianjie", card_use.Card.SkillPosition);
            Random ra = new Random();
            int index = ra.Next(1, 3);
            room.BroadcastSkillInvoke("jianjie", "male", index, gsk.General, gsk.SkinId);

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;

            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            if (player.Phase == PlayerPhase.Start)
            {
                player.AddMark("#jianjie");
                for (int i = 0; i < 2; i++)
                {
                    Player target = card_use.To[i];
                    if (i == 0)
                    {
                        room.AddPlayerMark(target, "@dragon");
                        room.HandleAcquireDetachSkills(target, "huoji_sm", true);
                    }
                    else
                    {
                        room.AddPlayerMark(target, "@phenix");
                        room.HandleAcquireDetachSkills(target, "lianhuan_sm", true);
                    }
                }
            }
            else
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "jianjie", card_use.Card.SkillPosition);
                Player from = card_use.To[0], to = card_use.To[1];
                List<string> choices = new List<string>();
                if (from.GetMark("@dragon") > 0 && to.GetMark("@dragon") == 0) choices.Add("@dragon");
                if (from.GetMark("@phenix") > 0 && to.GetMark("@phenix") == 0) choices.Add("@phenix");
                string mark = room.AskForChoice(player, "jianjie", string.Join("+", choices), new List<string> { string.Format("@jianjie-from:{0}:{1}", from.Name, to.Name) });
                if (mark == "@dragon")
                {
                    room.SetPlayerMark(from, "@dragon", 0);
                    room.SetPlayerMark(to, "@dragon", 1);
                    room.HandleAcquireDetachSkills(from, "-huoji_sm|-yeyan_sm", true);
                    string skill = to.GetMark("@phenix") > 0 ? "huoji_sm|yeyan_sm" : "huoji_sm";
                    if (to.GetMark("@phenix") > 0)
                    {
                        Thread.Sleep(1000);
                        skill = "huoji_sm|yeyan_sm";
                        room.BroadcastSkillInvoke(Name, "male", 3, gsk.General, gsk.SkinId);
                    }
                    room.HandleAcquireDetachSkills(to, skill, true);
                }
                else
                {
                    room.SetPlayerMark(from, "@phenix", 0);
                    room.SetPlayerMark(to, "@phenix", 1);
                    room.HandleAcquireDetachSkills(from, "-lianhuan_sm|-yeyan_sm", true);
                    string skill = "lianhuan_sm";
                    if (to.GetMark("@dragon") > 0)
                    {
                        Thread.Sleep(1000);
                        skill = "lianhuan_sm|yeyan_sm";
                        room.BroadcastSkillInvoke(Name, "male", 3, gsk.General, gsk.SkinId);
                    }
                    room.HandleAcquireDetachSkills(to, skill, true);
                }
            }
        }
    }

    public class Chenghao : TriggerSkill
    {
        public Chenghao() : base("chenghao")
        {
            skill_type = SkillType.Replenish;
            events.Add(TriggerEvent.Damaged);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && data is DamageStruct damage && damage.Nature != DamageStruct.DamageNature.Normal && damage.ChainStarter && !damage.Chain)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
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
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player guojia, TriggerStruct info)
        {
            int count = 1;
            foreach (Player p in room.GetAlivePlayers())
                if (p.Chained) count++;

            List<int> yiji_cards = room.GetNCards(count);
            List<int> origin_yiji = new List<int>(yiji_cards);

            while (guojia.Alive && yiji_cards.Count > 0)
            {
                guojia.PileChange("#chenghao", yiji_cards);
                if (!room.AskForYiji(guojia, yiji_cards, Name, true, false, true, -1, room.GetOtherPlayers(guojia), null, null, "#chenghao", false, info.SkillPosition))
                    break;

                guojia.Piles["#chenghao"].Clear();
                foreach (int id in origin_yiji)
                    if (room.GetCardPlace(id) != Place.DrawPile)
                        yiji_cards.Remove(id);
            }
            if (guojia.GetPile("#chenghao").Count > 0) guojia.Piles["#chenghao"].Clear();
            if (yiji_cards.Count > 0 && guojia.Alive)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, guojia.Name);
                room.ObtainCard(guojia, ref yiji_cards, reason, false);
                yiji_cards.Clear();
            }

            if (yiji_cards.Count > 0)
                room.ReturnToDrawPile(yiji_cards, false);

            return false;
        }
    }
    public class Yinshi : TriggerSkill
    {
        public Yinshi() : base("yinshi")
        {
            events.Add(TriggerEvent.DamageDefined);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && base.Triggerable(player, room) && player.GetMark("@dragon") == 0 && player.GetMark("@phenix") == 0
                && !player.EquipIsBaned(1) && !player.GetArmor()
                && (damage.Card != null && Engine.GetFunctionCard(damage.Card.Name).TypeID == CardType.TypeTrick || damage.Nature != DamageStruct.DamageNature.Normal))
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

    public class Lueming :ZeroCardViewAsSkill
    {
        public Lueming() : base("lueming")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(LuemingCard.ClassName) && player.HasEquip();
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(LuemingCard.ClassName) { Skill = Name };
        }
    }

    public class LuemingCard : SkillCard
    {
        public static string ClassName = "LuemingCard";
        public LuemingCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && Self != to_select && to_select.GetEquips().Count < Self.GetEquips().Count; 
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            player.AddMark("lueming");
            room.SetPlayerStringMark(player, "lueming", player.GetMark("lueming").ToString());
            string number = room.AskForChoice(target, "lueming", "A+2+3+4+5+6+7+8+9+10+J+Q+K", new List<string> { "@lueming-judge:" + player.Name });

            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                Negative = false,
                PlayAnimation = false,
                Who = player,
                Reason = "lueming",
                Pattern = ".|.|" + number
            };
            target.SetFlags("lueming");
            room.Judge(ref judge);
            target.SetFlags("-lueming");

            if (player.Alive && target.Alive)
            {
                if (judge.IsGood())
                {
                    room.Damage(new DamageStruct("lueming", player, target, 2));
                }
                else if (!target.IsAllNude() && RoomLogic.CanGetCard(room, player, target, "hej"))
                {
                    List<int> ids = target.GetCards("hej");
                    Shuffle.shuffle(ref ids);
                    foreach (int card in ids)
                    {
                        if (RoomLogic.CanGetCard(room, player, target, card))
                        {
                            room.ObtainCard(player, ids[0], false);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class Tunjun : ZeroCardViewAsSkill
    {
        public Tunjun() : base("tunjun") { limit_mark = "@tunjun"; frequency = Frequency.Limited; }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(limit_mark) > 0 && player.GetMark("lueming") > 0;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(TunjunCard.ClassName) { Mute = true };
        }
    }

    public class TunjunCard : SkillCard
    {
        public static string ClassName = "TunjunCard";
        public TunjunCard() : base(ClassName)
        {}

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (!to_select.EquipIsBaned(i) && to_select.GetEquip(i) == -1)
                        return true;
                }
            }

            return false;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;

            room.SetPlayerMark(card_use.From, "@tunjun", 0);
            room.BroadcastSkillInvoke("tunjun", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "tunjun");

            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            int count = player.GetMark("lueming");

            List<int> ids = new List<int>();
            foreach (int id in room.DrawPile)
            {
                WrappedCard card = room.GetCard(id);
                if (Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeEquip)
                    ids.Add(id);
            }

            if (ids.Count > 0)
            {
                Shuffle.shuffle(ref ids);
                for (int i = 0; i < Math.Min(ids.Count, count); i++)
                {
                    WrappedCard card = room.GetCard(ids[i]);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    EquipCard equip = (EquipCard)fcard;
                    int index = (int)equip.EquipLocation();
                    if (target.Alive && target.GetEquip(index) == -1 && fcard.IsAvailable(room, target, card))
                        room.UseCard(new CardUseStruct(card, target, new List<Player>(), false));
                }
            }
        }
    }

    public class Xingluan : TriggerSkill
    {
        public Xingluan() : base("xingluan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.CardFinished };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-xingluan");
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && base.Triggerable(player, room) && !player.HasFlag(Name) && player.Phase == PlayerPhase.Play
                && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill && use.To.Count == 1)
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
            player.SetFlags(Name);
            foreach (int id in room.DrawPile)
            {
                if (room.GetCard(id).Number == 6)
                {
                    room.ObtainCard(player, id, true);
                    break;
                }
            }
            return false;
        }
    }

    public class Sidao : TriggerSkill
    {
        public Sidao() : base("sidao")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Alter;
            view_as_skill = new SidaoVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.ContainsTag(Name))
            {
                player.RemoveTag(Name);
                player.SetFlags("-sidao");
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play
                && player.ContainsTag(Name) && player.GetTag(Name) is List<string> names && !player.HasFlag(Name) && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill)
            {
                foreach (Player p in use.To)
                    if (p != player && names.Contains(p.Name))
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && player.GetTag(Name) is List<string> names)
            {
                List<string> targets = new List<string>();
                foreach (Player p in use.To)
                    if (names.Contains(p.Name))
                        targets.Add(p.Name);

                player.SetTag("sidao_target", targets);
                room.AskForUseCard(player, "@@sidao", "@sidao", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
                player.RemoveTag("sidao_target");
            }
            return new TriggerStruct();
        }
    }

    public class SidaoVS : OneCardViewAsSkill
    {
        public SidaoVS() : base("sidao")
        {
            response_pattern = "@@sidao";
            filter_pattern = ".";
            response_or_use = true;
        }

        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard sd = new WrappedCard(SidaoCard.ClassName);
            sd.AddSubCard(card);
            return sd;
        }
    }

    public class SidaoCard : SkillCard
    {
        public static string ClassName = "SidaoCard";
        public SidaoCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (Self.GetTag("sidao_target") is List<string> names)
            {
                WrappedCard snatch = new WrappedCard(Snatch.ClassName);
                snatch.AddSubCard(card);
                return names.Contains(to_select.Name) && Snatch.Instance.TargetFilter(room, targets, to_select, Self, snatch);
            }
            return false;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            WrappedCard snatch = new WrappedCard(Snatch.ClassName);
            snatch.AddSubCard(card);
            return Snatch.Instance.TargetsFeasible(room, targets, Self, snatch);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            use.From.SetFlags("sidao");
            WrappedCard snatch = new WrappedCard(Snatch.ClassName) {Skill =  "sidao", ShowSkill = "sidao" };
            snatch.AddSubCard(use.Card);
            return snatch;
        }
    }

    public class SidaoRecord : TriggerSkill
    {
        public SidaoRecord() : base("#sidao")
        {
            events.Add(TriggerEvent.CardFinished);
        }
        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardUseStruct use && player.Phase == PlayerPhase.Play && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill)
            {
                if (use.To.Count > 0)
                {
                    List<string> names = new List<string>();
                    foreach (Player p in use.To)
                        names.Add(p.Name);

                    player.SetTag("sidao", names);
                }
                else if (player.ContainsTag("sidao"))
                    player.RemoveTag("sidao");
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class TanbeiCard : SkillCard
    {
        public static string ClassName = "TanbeiCard";
        public TanbeiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            bool infi = true;
            if (!target.IsAllNude() && RoomLogic.CanGetCard(room, player, target, "hej"))
                infi = room.AskForChoice(target, "tanbei", "got+infi", new List<string> { "@tanbei:" + player.Name }, player) == "infi";

            if (infi)
            {
                player.SetFlags("tanbei_" + target.Name);
                LogMessage log = new LogMessage("#infi-inturn")
                {
                    From = player.Name,
                    To = new List<string> { target.Name },
                    Arg = "tanbei"
                };
                room.SendLog(log);
            }
            else
            {
                List<int> ids = target.GetCards("hej");
                Shuffle.shuffle(ref ids);
                foreach (int card in ids)
                {
                    if (RoomLogic.CanGetCard(room, player, target, card))
                    {
                        room.ObtainCard(player, ids[0], false);
                        break;
                    }
                }

                player.SetFlags("tanbeipro_" + target.Name);

                LogMessage log = new LogMessage("#prohibi-inturn")
                {
                    From = player.Name,
                    To = new List<string> { target.Name },
                    Arg = "tanbei"
                };
                room.SendLog(log);
            }
        }
    }

    public class Tanbei : ZeroCardViewAsSkill
    {
        public Tanbei() : base("tanbei") { skill_type = SkillType.Attack; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(TanbeiCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(TanbeiCard.ClassName) { Skill = Name };
        }
    }

    
    public class TanbeiTar : TargetModSkill
    {
        public TanbeiTar() : base("#tanbei-target", false)
        {
            pattern = ".";
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
        {
            if (to != null && from.HasFlag("tanbei_" + to.Name))
                return true;

            return false;
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (to != null && from.HasFlag("tanbei_" + to.Name))
                return true;

            return false;
        }
    }

    public class TanbeiPro : ProhibitSkill
    {
        public TanbeiPro() : base("#tanbei-prohibit") { }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && to != null && Engine.GetFunctionCard(card.Name).TypeID != CardType.TypeSkill && from.HasFlag("tanbeipro_" + to.Name))
                return true;

            return false;
        }
    }

    public class Wenji : TriggerSkill
    {
        public Wenji() : base("wenji")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                player.RemoveTag(Name);
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
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (!p.IsNude()) targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@wenji-ask", true, true, info.SkillPosition);
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

            List<int> ids = target.GetCards("he");
            if (ids.Count > 1)
                ids = room.AskForExchange(target, Name, 1, 1, "@wenji:" + player.Name, string.Empty, "..", string.Empty);
            string card_name = string.Empty;
            WrappedCard card = room.GetCard(ids[0]);
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard is Slash)
                card_name = Slash.ClassName;
            else if (fcard is TrickCard && !(fcard is DelayedTrick))
                card_name = card.Name;

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, null);
            room.ObtainCard(player, ref ids, reason, true);
            if (!string.IsNullOrEmpty(card_name))
            {
                List<string> names = player.ContainsTag(Name) ? (List<string>)player.GetTag(Name) : new List<string>();
                names.Add(card_name);
                player.SetTag(Name, names);
            }
            return false;
        }
    }
    public class WenjiEffect : TriggerSkill
    {
        public WenjiEffect() : base("#wenji")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.TrickCardCanceling };
            frequency = Frequency.Compulsory;
        }


        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && use.To.Count > 0 && player.ContainsTag("wenji") && player.GetTag("wenji") is List<string> names
                && (names.Contains(use.Card.Name) || (use.Card.Name.Contains(Slash.ClassName) && names.Contains(Slash.ClassName))))
            {
                return new TriggerStruct(Name, player, use.To);
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && player != effect.From && effect.From != null && effect.From.Alive
                && effect.From.ContainsTag("wenji") && effect.From.GetTag("wenji") is List<string> _names && _names.Contains(effect.Card.Name))
            {
                return new TriggerStruct(Name, effect.From);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
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

    public class Tunjiang : TriggerSkill
    {
        public Tunjiang() : base("tunjiang")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardTargetAnnounced };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && player.Phase == PlayerPhase.Play && data is CardUseStruct use && use.To.Count > 0 && !player.HasFlag(Name))
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

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && !player.HasFlag(Name))
            {
                bool check = true;
                foreach (PhaseStruct phase in player.PhasesState)
                {
                    if (phase.Phase == PlayerPhase.Play && phase.Skipped)
                    {
                        check = false;
                        break;
                    }
                }
                if (check) return new TriggerStruct(Name, player);
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
            room.DrawCards(player, Fuli.GetKingdoms(room), Name);
            return false;
        }
    }

    public class Zhidao : TriggerSkill
    {
        public Zhidao() : base("zhidao")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && player.Phase == PlayerPhase.Play)
                player.AddMark(Name);
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);

        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && player.GetMark(Name) == 1 && damage.To.Alive && !damage.To.IsAllNude()
                && base.Triggerable(player, room) && RoomLogic.CanGetCard(room, player, damage.To, "hej") && damage.To != player)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                Player target = damage.To;
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.SendCompulsoryTriggerLog(player, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                List<int> ids = new List<int>();
                if (!target.IsKongcheng() && RoomLogic.CanGetCard(room, player, target, "h"))
                {
                    int id = room.AskForCardChosen(player, target, "h", Name, false, HandlingMethod.MethodGet);
                    ids.Add(id);
                }
                if (target.HasEquip() && RoomLogic.CanGetCard(room, player, target, "e"))
                {
                    int id = room.AskForCardChosen(player, target, "e", Name, false, HandlingMethod.MethodGet);
                    ids.Add(id);
                }
                if (target.JudgingArea.Count > 0 && RoomLogic.CanGetCard(room, player, target, "j"))
                {
                    int id = room.AskForCardChosen(player, target, "j", Name, false, HandlingMethod.MethodGet);
                    ids.Add(id);
                }
                if (ids.Count > 0)
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, Name, string.Empty), false);

                if (player.Alive)
                    player.SetFlags(Name);
            }
            return false;
        }
    }

    public class ZhidaoPro : ProhibitSkill
    {
        public ZhidaoPro() : base("#zhidao") { }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && Engine.GetFunctionCard(card.Name).TypeID != CardType.TypeSkill && from.HasFlag("zhidao"))
                return from != to;

            return false;
        }
    }

    public class JiliYbh : TriggerSkill
    {
        public JiliYbh() : base("jili_ybh")
        {
            events.Add(TriggerEvent.TargetConfirming);
            skill_type = SkillType.Wizzard;
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardUseStruct use && use.To.Contains(player) && player.Alive && use.Card.Name != Collateral.ClassName && WrappedCard.IsRed(use.Card.Suit))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is BasicCard || fcard.IsNDTrick())
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    {
                        if (p != use.From && !use.To.Contains(p) && RoomLogic.DistanceTo(room, player, p) == 1
                            && RoomLogic.IsProhibited(room, use.From, p, use.Card) == null)
                        {
                            if ((fcard is Peach && !p.IsWounded()) || (fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, use.From, p))
                                || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && !RoomLogic.CanGetCard(room, use.From, p, "hej"))
                                || (fcard is Dismantlement && !RoomLogic.CanDiscard(room, use.From, p, "hej"))) continue;

                            triggers.Add(new TriggerStruct(Name, p));
                        }
                    }
                }
            }
            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            if (data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                use.To.Add(ask_who);
                use.EffectCount.Add(fcard.FillCardBasicEffct(room, ask_who));
                room.SortByActionOrder(ref use);
                data = use;

                LogMessage log = new LogMessage
                {
                    Type = "$jili_ybh",
                    From = ask_who.Name,
                    Card_str = RoomLogic.CardToString(room, use.Card),
                    Arg = Name
                };
                room.SendLog(log);
            }
            return false;
        }
    }    

    public class Lianji : TriggerSkill
    {
        public Lianji() : base("lianji")
        {
            skill_type = SkillType.Wizzard;
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.SwapPile };
            view_as_skill = new LianjiVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.SwapPile && room.ContainsTag("saber_change") && !room.ContainsTag("saber_changed"))
            {
                for (int i = 0; i < room.DrawPile.Count; i++)
                {
                    int card_id = room.DrawPile[i];
                    WrappedCard wrapped = room.GetCard(card_id);
                    if (wrapped.Name == QinggangSword.ClassName && Shuffle.random(1, 3))
                    {
                        int replace = -1;
                        foreach (int real in Engine.GetEngineCards())
                        {
                            WrappedCard real_card = Engine.GetRealCard(real);
                            if (real_card.Name == Saber.ClassName)
                            {
                                replace = real;
                                break;
                            }
                        }

                        room.ReplaceRoomCard(card_id, replace);
                        room.SetTag("saber_changed", true);
                    }
                }

                room.RemoveTag("saber_change");
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class LianjiVS : OneCardViewAsSkill
    {
        public LianjiVS() : base("lianji")
        {
            filter_pattern = ".!";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(LianjiCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard lj = new WrappedCard(LianjiCard.ClassName) { Skill = Name };
            lj.AddSubCard(card);
            return lj;
        }
    }

    public class LianjiCard : SkillCard
    {
        public static string ClassName = "LianjiCard";
        public LianjiCard() : base(ClassName)
        {
            will_throw = true;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            room.SetTag("saber_change", true);
            Player player = card_use.From, target = card_use.To[0];
            player.AddMark("lianji");

            if (target.Alive)
            {
                for (int i = 0; i < room.DrawPile.Count; i++)
                {
                    int card_id = room.DrawPile[i];
                    WrappedCard wrapped = room.GetCard(card_id);
                    FunctionCard functionCard = Engine.GetFunctionCard(wrapped.Name);
                    if (functionCard is Weapon)
                    {
                        if (wrapped.Name == QinggangSword.ClassName)
                        {
                            int replace = -1;
                            foreach (int real in Engine.GetEngineCards())
                            {
                                WrappedCard real_card = Engine.GetRealCard(real);
                                if (real_card.Name == Saber.ClassName)
                                {
                                    replace = real;
                                    break;
                                }
                            }

                            room.ReplaceRoomCard(card_id, replace);
                            room.SetTag("saber_changed", true);
                            card_id = replace;
                        }
                        wrapped = room.GetCard(card_id);
                        functionCard = Engine.GetFunctionCard(wrapped.Name);
                        if (functionCard.IsAvailable(room, target, wrapped))
                            room.UseCard(new CardUseStruct(wrapped, target, new List<Player>()));
                        break;
                    }
                }
            }

            if (target.Alive && player.Alive)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(target))
                {
                    if (RoomLogic.CanSlash(room, target, p))
                        targets.Add(p);
                }

                bool slash = true;
                if (targets.Count > 0)
                {
                    Player victim = room.AskForPlayerChosen(player, targets, "lianji", "@lianji-victim:" + target.Name, false, false, card_use.Card.SkillPosition);
                    if (victim != null)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, victim.Name);
                        LogMessage log = new LogMessage
                        {
                            Type = "#lianji-target",
                            From = player.Name,
                            To = new List<string> { victim.Name }
                        };
                        room.SendLog(log);

                        slash = room.AskForUseSlashTo(target, victim, string.Format("@lianji-slash:{0}:{1}", player.Name, victim), null) != null;
                    }
                }

                if (!slash && target.GetWeapon())
                {
                    WrappedCard card = room.GetCard(target.Weapon.Key);
                    player.SetFlags("lianji_weapon");
                    Player holder = room.AskForPlayerChosen(player, room.GetOtherPlayers(target), Name, string.Format("@lianji-weapon:{0}::{1}", target.Name, card.Name),
                        false, false, card_use.Card.SkillPosition);
                    player.SetFlags("-lianji_weapon");

                    room.ObtainCard(holder, card, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, holder.Name, "lianji", string.Empty));
                }
            }
        }
    }


    public class Moucheng : PhaseChangeSkill
    {
        public Moucheng() : base("moucheng")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.GetMark("lianji") >= 3)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DoSuperLightbox(ask_who, info.SkillPosition, Name);
            ask_who.SetMark(Name, 1);
            room.SendCompulsoryTriggerLog(ask_who, Name);

            room.HandleAcquireDetachSkills(ask_who, "-lianji", false);
            room.HandleAcquireDetachSkills(ask_who, "jingong", true);

            return false;
        }
    }

    public class Jingong : TriggerSkill
    {
        public Jingong() : base("jingong")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.Damage, TriggerEvent.CardUsedAnnounced };
            skill_type = SkillType.Alter;
            view_as_skill = new JingongVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class JingongVS : ViewAsSkill
    {
        public JingongVS() : base("jingong")
        {
            response_or_use = true;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("ViewAsSkill_jingongCard");
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count == 0)
            {
                FunctionCard fcard = Engine.GetFunctionCard(to_select.Name);
                return !RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodUse) && (fcard is Slash || fcard is EquipCard);
            }
            return false;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (cards.Count == 1)
            {
                if (player.ContainsTag(Name) && player.GetTag(Name) is List<string> vcards)
                {
                    foreach (string card_name in vcards)
                    {
                        WrappedCard card = new WrappedCard(card_name) { Skill = Name };
                        card.AddSubCard(cards[0]);
                        result.Add(card);
                    }
                }
                else
                {
                    List<string> v_cards = GetGuhuoCards(room, "t");
                    v_cards.RemoveAll(t => t.Contains(Nullification.ClassName));
                    List<string> specials = new List<string> { HoneyTrap.ClassName, HiddenDagger.ClassName };
                    Shuffle.shuffle(ref v_cards);
                    Shuffle.shuffle(ref specials);

                    List<string> alls = new List<string> { specials[0] };
                    for (int i = 0; i < 2; i++)
                        alls.Add(v_cards[i]);
                    player.SetTag(Name, alls);
                    foreach (string card_name in alls)
                    {
                        WrappedCard card = new WrappedCard(card_name) { Skill = Name };
                        card.AddSubCard(cards[0]);
                        result.Add(card);
                    }
                }
            }

            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
            {
                WrappedCard card = RoomLogic.ParseUseCard(room, cards[0]);
                return card;
            }

            return null;
        }
    }

    public class ZhoufuEffect : TriggerSkill
    {
        public ZhoufuEffect() : base("#zhoufu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.StartJudge };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.HasFlag(Name)) return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.StartJudge && player.GetPile("incantation").Count > 0)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {

            if (triggerEvent == TriggerEvent.StartJudge && data is JudgeStruct judge_struct)
            {
                List<int> ids = player.GetPile("incantation");
                judge_struct.Card = room.GetCard(ids[0]);
                player.SetFlags(Name);

                LogMessage log = new LogMessage
                {
                    Type = "$InitialJudge",
                    From = judge_struct.Who.Name,
                    Card_str = ids[0].ToString()
                };
                room.SendLog(log);

                room.MoveCardTo(judge_struct.Card, null, judge_struct.Who, Place.PlaceJudge,
                    new CardMoveReason(MoveReason.S_REASON_JUDGE, judge_struct.Who.Name, null, null, judge_struct.Reason), true);

                Thread.Sleep(500);
                bool effected = judge_struct.Good == Engine.MatchExpPattern(room, judge_struct.Pattern, judge_struct.Who, judge_struct.Card);
                judge_struct.UpdateResult(effected);
                data = judge_struct;

                return true;
            }
            else
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasFlag(Name)) targets.Add(p);

                if (targets.Count > 0)
                {
                    room.SortByActionOrder(ref targets);
                    foreach (Player p in targets)
                        if (p.Alive)
                            room.LoseHp(p);
                }

                return false;
            }

        }
    }

    public class Zhoufu : OneCardViewAsSkill
    {
        public Zhoufu() : base("zhoufu")
        { }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ZhoufuCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard zf = new WrappedCard(ZhoufuCard.ClassName) { Skill = Name };
            zf.AddSubCard(card);
            return zf;
        }
    }

    public class ZhoufuCard : SkillCard
    {
        public static string ClassName = "ZhoufuCard";
        public ZhoufuCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && to_select.GetPile("incantation").Count == 0;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            room.AddToPile(card_use.To[0], "incantation", card_use.Card.GetEffectiveId(), false, new List<Player> { card_use.From });
        }
    }

    public class Yingbing : TriggerSkill
    {
        public Yingbing() : base("yingbing")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceSpecial)
                && move.From_pile_names.Contains("incantation") && move.From.GetMark(Name) > 0)
                move.From.SetMark(Name, 0);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && player.GetPile("incantation").Count > 0 && use.Card.Suit == room.GetCard(player.GetPile("incantation")[0]).Suit)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    triggers.Add(new TriggerStruct(Name, p));
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use
               && player.GetPile("incantation").Count > 0 && resp.Card.Suit == room.GetCard(player.GetPile("incantation")[0]).Suit)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.AddMark(Name);
            if (ask_who.Alive)
                room.DrawCards(ask_who, 1, Name);

            if (player.GetMark(Name) > 1)
            {
                List<int> ids = player.GetPile("incantation");
                if (ids.Count > 0)
                {
                    CardsMoveStruct move = new CardsMoveStruct(ids, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, player.Name, Name, string.Empty));
                    room.MoveCardsAtomic(move, true);
                }
            }

            return false;
        }
    }

    public class Lianzhu : OneCardViewAsSkill
    {
        public Lianzhu() : base("lianzhu")
        {
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(LianzhuCard.ClassName) && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard zf = new WrappedCard(LianzhuCard.ClassName) { Skill = Name };
            zf.AddSubCard(card);
            return zf;
        }
    }

    public class LianzhuCard : SkillCard
    {
        public static string ClassName = "LianzhuCard";
        public LianzhuCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = new List<int>(card_use.Card.SubCards);
            bool black = WrappedCard.IsBlack(room.GetCard(ids[0]).Suit);

            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "lianzhu", string.Empty));

            if (target.Alive && black)
            {
                string pattern = "@lianzhu:" + player.Name;
                if (!player.Alive)
                    pattern = "@lianzhu-discard:" + player.Name;
                if (!room.AskForDiscard(target, "lianzhu", 2, 2, player.Alive, true, pattern) && player.Alive)
                    room.DrawCards(player, 2, "lianzhu");
            }
        }
    }

    public class Xiahui : TriggerSkill
    {
        public Xiahui() : base("xiahui")
        {
            events = new List<TriggerEvent> { TriggerEvent.BeforeCardsMove, TriggerEvent.CardsMoveOneTime, TriggerEvent.PostHpReduced };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.BeforeCardsMove && data is CardsMoveOneTimeStruct move && move.To != null && base.Triggerable(move.From, room)
                && move.To != move.From && move.To.Alive && move.To_place == Place.PlaceHand)
            {
                List<int> blacks = new List<int>();
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int id = move.Card_ids[i];
                    if ((move.From_places[i] == Place.PlaceHand || move.From_places[i] == Place.PlaceEquip) && WrappedCard.IsBlack(room.GetCard(id).Suit))
                        blacks.Add(id);
                }

                if (blacks.Count > 0)
                    move.To.SetTag(string.Format("{0}_{1}", Name, move.To.Name), blacks);
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct _move && _move.From != null && _move.From_places.Contains(Place.PlaceHand)
                && _move.From.ContainsTag(Name) && _move.From.GetTag(Name) is List<int> ids)
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
            else if (triggerEvent == TriggerEvent.PostHpReduced && player.ContainsTag(Name) && player.GetTag(Name) is List<int> remove)
            {
                player.RemoveTag(Name);
                RoomLogic.RemovePlayerCardLimitation(player, Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && base.Triggerable(move.From, room) && move.To != null && move.To.Alive
                && move.To != move.From && move.To_place == Place.PlaceHand
                && move.To.ContainsTag(string.Format("{0}_{1}", Name, move.To.Name)) && move.To.GetTag(string.Format("{0}_{1}", Name, move.To.Name)) is List<int> ids)
            {
                foreach (int id in move.Card_ids)
                    if (ids.Contains(id))
                        return new TriggerStruct(Name, move.From);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To.GetTag(string.Format("{0}_{1}", Name, move.To.Name)) is List<int> ids)
            {
                move.To.RemoveTag(string.Format("{0}_{1}", Name, move.To.Name));
                //room.SendCompulsoryTriggerLog(ask_who, Name);
                //room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, move.To.Name);

                List<int> fix = move.Card_ids.FindAll(t => ids.Contains(t));
                move.To.SetTag(Name, fix);
                foreach (int id in fix)
                    RoomLogic.SetPlayerCardLimitation(move.To, Name, "use,response,discard", id.ToString());
            }
            return false;
        }
    }

    public class XiahuiMax : MaxCardsSkill
    {
        public XiahuiMax() : base("#xiahui-max") { }

        public override bool Ingnore(Room room, Player player, int card_id) => RoomLogic.PlayerHasSkill(room, player, "xiahui") && WrappedCard.IsBlack(room.GetCard(card_id).Suit);
    }

    public class Neifa : TriggerSkill
    {
        public Neifa() : base("neifa")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardUsed, TriggerEvent.EventPhaseChanging, TriggerEvent.CardTargetAnnounced };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                if (player.GetMark(Name) > 0)
                    player.SetMark(Name, 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardUsed && player.HasFlag("neifa_not_basic") && data is CardUseStruct use && Engine.GetFunctionCard(use.Card.Name) is EquipCard
                && player.Alive && player.GetMark(Name) > 0 && player.GetMark("neifa_draw") < 2)
            {
                return new TriggerStruct(Name, player);
            }
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct _use && player.HasFlag("neifa_not_basic")
                && Engine.GetFunctionCard(_use.Card.Name).IsNDTrick() && _use.Card.Name != Collateral.ClassName)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if ((p.HasEquip() || p.JudgingArea.Count > 0) && RoomLogic.CanGetCard(room, player, p, "ej"))
                        targets.Add(p);
                }
                targets.Add(player);
                player.SetFlags("neifa_invoke");
                Player target = room.AskForPlayerChosen(player, targets, Name, "@neifa-choose", true, true, info.SkillPosition);
                player.SetFlags("-neifa_invoke");
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }
            else if (triggerEvent == TriggerEvent.CardUsed)
            {
                return info;
            }
            else if (data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (use.To.Contains(p))
                    {
                        if (use.To.Count > 1)
                            targets.Add(p);
                    }
                    else if (RoomLogic.IsProhibited(room, use.From, p, use.Card) == null)
                    {
                        if ((fcard is Slash && p == use.From) || (fcard is Peach && !p.IsWounded())
                            || (fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, player, p))
                            || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && (!RoomLogic.CanGetCard(room, player, p, "hej") || p == use.From))
                            || (fcard is Dismantlement && (!RoomLogic.CanDiscard(room, player, p, "hej") || p == use.From))
                            || (fcard is Duel && p == use.From) || ((fcard is ArcheryAttack || fcard is SavageAssault) && p == use.From)) continue;
                        targets.Add(p);
                    }
                }

                if (targets.Count > 0)
                {
                    room.SetTag(Name, data);
                    Player target = room.AskForPlayerChosen(player, targets, Name, string.Format("@neifa-trick:::{0}", use.Card.Name), true, true, info.SkillPosition);
                    room.RemoveTag(Name);
                    if (target != null)
                    {
                        room.SetTag(Name, target);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                Player target = (Player)room.GetTag(Name);
                room.RemoveTag(Name);
                bool draw = target == player;
                if (target == player && (player.HasEquip() || player.JudgingArea.Count > 0) && RoomLogic.CanGetCard(room, player, player, "ej"))
                {
                    string choice = room.AskForChoice(player, Name, "draw+get");
                    draw = choice == "draw";
                }

                if (draw)
                    room.DrawCards(player, 2, Name);
                else
                {
                    int id = room.AskForCardChosen(player, target, "ej", Name, false, HandlingMethod.MethodGet);
                    room.ObtainCard(player, room.GetCard(id),
                        new CardMoveReason(room.GetCardPlace(id) == Place.PlaceEquip ? MoveReason.S_REASON_EXTRACTION : MoveReason.S_REASON_GOTCARD, player.Name, target.Name,
                        Name, string.Empty));
                }

                if (player.Alive && RoomLogic.CanDiscard(room, player, player, "he"))
                {
                    List<int> ids = room.AskForExchange(player, Name, 1, 1, "@neifa", string.Empty, "..!", info.SkillPosition);
                    if (ids.Count > 0)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(ids[0]).Name);
                        if (fcard is BasicCard)
                        {
                            player.SetFlags("neifa_basic");
                            int count = 0;
                            foreach (int id in player.GetCards("h"))
                            {
                                if (!(Engine.GetFunctionCard(room.GetCard(id).Name) is BasicCard))
                                    count++;
                            }
                            count = Math.Min(5, count);
                            player.SetMark(Name, count);
                        }
                        else
                        {
                            player.SetFlags("neifa_not_basic");
                            int count = 0;
                            foreach (int id in player.GetCards("h"))
                            {
                                if (Engine.GetFunctionCard(room.GetCard(id).Name) is BasicCard)
                                    count++;
                            }
                            count = Math.Min(5, count);
                            player.SetMark(Name, count);
                            player.SetMark("neifa_draw", 0);
                        }

                        room.ThrowCard(ref ids, new CardMoveReason(MoveReason.S_REASON_THROW, player.Name, Name, string.Empty), player);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.CardUsed)
            {
                player.AddMark("neifa_draw");
                room.DrawCards(player, player.GetMark(Name), Name);
            }
            else if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = (Player)room.GetTag(Name);
                room.RemoveTag(Name);
                if (!use.To.Contains(target))
                {
                    use.To.Add(target);
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        To = new List<string> { target.Name },
                        Card_str = RoomLogic.CardToString(room, use.Card),
                        Arg = Name
                    };
                    room.SendLog(log);
                }
                else
                {
                    use.To.Remove(target);
                    LogMessage log = new LogMessage
                    {
                        Type = "$CancelTarget",
                        From = use.From.Name,
                        To = new List<string> { target.Name },
                        Arg = use.Card.Name
                    };
                    room.SendLog(log);
                    room.SetEmotion(target, "cancel");
                    Thread.Sleep(400);
                }

                room.SortByActionOrder(ref use);
                data = use;
            }

            return false;
        }
    }

    public class NeifaTar : TargetModSkill
    {
        public NeifaTar() : base("#neifa-tar", false)
        {
        }
        public override int GetExtraTargetNum(Room room, Player from, WrappedCard card)
        {
            if (card.Name.Contains(Slash.ClassName) && from.HasFlag("neifa_basic"))
                return 1;
            return 0;
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            int count = 0;
            if (from.HasFlag("neifa_basic") && card.Name.Contains(Slash.ClassName))
            {
                count = from.GetMark("neifa");
            }
            return count;
        }
    }

    public class NeifaPro : ProhibitSkill
    {
        public NeifaPro() : base("#neifa-pro")
        {
        }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                bool basic = fcard is BasicCard;
                if ((from.HasFlag("neifa_basic") && !basic) || (from.HasFlag("neifa_not_basic") && basic))
                    return true;
            }

            return false;
        }
    }

    public class ZhenduJX : TriggerSkill
    {
        public ZhenduJX() : base("zhendu_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player != null && triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play)
            {
                List<Player> hetaihous = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player hetaihou in hetaihous)
                {
                    if (RoomLogic.CanDiscard(room, hetaihou, hetaihou, "h"))
                        skill_list.Add(new TriggerStruct(Name, hetaihou));
                }
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player hetaihou, TriggerStruct info)
        {
            string prompt = "@zhendu-discard:" + player.Name;
            if (hetaihou == player) prompt = "@zhendu-self-discard";
            if (room.AskForDiscard(hetaihou, Name, 1, 1, true, false, prompt, true, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, hetaihou.Name, player.Name);
                room.BroadcastSkillInvoke(Name, hetaihou, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player hetaihou, TriggerStruct info)
        {
            WrappedCard analeptic = new WrappedCard(Analeptic.ClassName)
            {
                Skill = "_zhendu_jx"
            };
            room.UseCard(new CardUseStruct(analeptic, player, new List<Player>(), true), true, true);
            if (player.Alive && player != hetaihou)
                room.Damage(new DamageStruct(Name, hetaihou, player));

            return false;
        }
    }
    public class QiluanJX : TriggerSkill
    {
        public QiluanJX() : base("qiluan_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death)
            {
                Player current = room.Current;
                foreach (Player p in room.GetOtherPlayers(death.Who))
                {
                    if (death.Damage.From != null && p == death.Damage.From)
                        p.AddMark(Name, 3);
                    else
                        p.AddMark(Name);
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.GetMark(Name) > 0) skill_list.Add(new TriggerStruct(Name, p));
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player hetaihou, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(hetaihou, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, hetaihou, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, ask_who.GetMark(Name), Name);
            return false;
        }
    }

    public class QiluanJXClear : TriggerSkill
    {
        public QiluanJXClear() : base("#qiluan_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }
        public override int GetPriority() => 2;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark("qiluan_jx") > 0) p.SetMark("qiluan_jx", 0);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class BeizhanClassic : PhaseChangeSkill
    {
        public BeizhanClassic() : base("beizhan_classic")
        {
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.GetMark(Name) > 0 && player.Phase == Player.PlayerPhase.Start)
            {
                player.SetMark(Name, 0);
                int max_cout = 0;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum > max_cout)
                        max_cout = p.HandcardNum;

                if (player.HandcardNum == max_cout)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#beizhan",
                        From = player.Name,
                        Arg = Name
                    };
                    room.SendLog(log);

                    player.SetFlags(Name);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Finish)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player to = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@beizhan_classic", true, true, info.SkillPosition);
            if (to != null)
            {
                room.SetTag("beizhan_target", to);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player to = (Player)room.GetTag("beizhan_target");
            room.RemoveTag("beizhan_target");
            if (to != null)
            {
                to.SetMark(Name, 1);
                if (to.HandcardNum < to.MaxHp && to.HandcardNum < 5)
                    room.DrawCards(to, new DrawCardStruct(Math.Min(5 - to.HandcardNum, to.MaxHp - to.HandcardNum), player, Name));
            }

            return false;
        }
    }

    public class BeizhanCProhibit : ProhibitSkill
    {
        public BeizhanCProhibit() : base("#beizhan-c-prohibit")
        {
        }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && from.HasFlag("beizhan_classic") && card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                return !(fcard is SkillCard) && to != null && to != from;
            }
            return false;
        }
    }

    public class GangzhiClassic : TriggerSkill
    {
        public GangzhiClassic() : base("gangzhi_classic")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageForseen, TriggerEvent.Predamage };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageForseen && data is DamageStruct damage && base.Triggerable(player, room) && damage.From != player)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.Predamage && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (data is DamageStruct damage)
            {
                if (triggerEvent == TriggerEvent.DamageForseen)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#gangzhi",
                        From = ask_who.Name,
                        Arg = Name,
                        Arg2 = damage.Damage.ToString()
                    };
                    room.SendLog(log);

                    room.LoseHp(ask_who, damage.Damage);
                }
                else
                {
                    room.LoseHp(damage.To, damage.Damage);
                }
                return true;
            }

            return false;
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

            player.SetMark("qiangwu", judge.JudgeNumber);
            room.SetPlayerStringMark(player, "qiangwu", judge.JudgeNumber.ToString());
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

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_THROW, card_use.From.Name, null,
                card_use.Card.Skill, null)
            {
                Card = card_use.Card,
                General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
            };

            room.RecordSubCards(card_use.Card);
            room.MoveCardTo(card_use.Card, card_use.From, null, Place.PlaceTable, reason, true);
            room.SendLog(log);

            List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card_use.Card));
            if (table_cardids.Count > 0)
            {
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, player, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
                room.RemoveSubCards(card_use.Card);
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

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
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
                target.SetMark(Name, 1);
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
                string choice = room.AskForChoice(player, Name, "draw+damage+cancel", new List<string> { "@to-player:" + skill_target.Name }, skill_target);
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
                room.ObtainCard(player, room.GetCard(id), new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, Name, string.Empty), false);
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
            CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
            switch (reason)
            {
                case CardUseReason.CARD_USE_REASON_PLAY:
                    return card.Name == Jink.ClassName && !RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse);
                case CardUseReason.CARD_USE_REASON_RESPONSE:
                case CardUseReason.CARD_USE_REASON_RESPONSE_USE:
                    if (RoomLogic.IsCardLimited(room, player, card, reason == CardUseReason.CARD_USE_REASON_RESPONSE ? HandlingMethod.MethodResponse : HandlingMethod.MethodUse))
                        return false;

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
            if (data is DamageStruct damage)
            {
                ask_who.AddMark(Name, damage.Damage);
                room.SetPlayerStringMark(ask_who, "meiying", ask_who.GetMark(Name).ToString());
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            }

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
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE)
                return false;

            WrappedCard wrapped = new WrappedCard(card.UserString);
            FunctionCard fcard = Engine.GetFunctionCard(card.UserString);
            return fcard.TargetFilter(room, targets, to_select, Self, wrapped);
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE && targets.Count == 0)
                return true;

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

                List<string> skills = player.GetSkills(false, false);
                foreach (string skill in skills)
                {
                    Skill _s = Engine.GetSkill(skill);
                    if (_s != null && !_s.Attached_lord_skill)
                        room.DetachSkillFromPlayer(player, skill, false, player.GetAcquiredSkills().Contains(skill), true);
                }

                foreach (string skill in Engine.GetGeneralRelatedSkills(general_name, room.Setting.GameMode))
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
                max = Math.Min(room.Players.Count, max);
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
                    CardsMoveStruct move = new CardsMoveStruct(discard, null, Place.DiscardPile, new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, ask_who.Name, Name, string.Empty));
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
            events.Add(TriggerEvent.CardFinished);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player from, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(from, room) && from == room.Current)
            {
                WrappedCard card = use.Card;
                List<int> ids = room.GetSubCards(card);
                string card_name = card.Name;
                if (card_name.Contains(Slash.ClassName)) card_name = Slash.ClassName;
                string str = string.Format("{0}_{1}", Name, card_name);
                if (!from.HasFlag(str) && ids.Count > 0 && ids.SequenceEqual(card.SubCards))
                {
                    bool check = true;
                    foreach (int id in card.SubCards)
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

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<int> ids = new List<int>(use.Card.SubCards);
                if (ids.Count > 0)
                {
                    WrappedCard card = use.Card;
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
            if (data is CardUseStruct use)
            {
                List<int> ids = new List<int>(use.Card.SubCards);
                if (ids.Count > 0)
                {
                    room.RemoveSubCards(use.Card);
                    ResultStruct result = ask_who.Result;
                    result.Assist += ids.Count;
                    ask_who.Result = result;

                    WrappedCard card = use.Card;
                    string card_name = card.Name;
                    if (card_name.Contains(Slash.ClassName)) card_name = Slash.ClassName;
                    string str = string.Format("{0}_{1}", Name, card_name);
                    ask_who.SetFlags(str);

                    room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, ask_who.Name, target.Name, Name, string.Empty));
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
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, card_use.From.Name, target.Name, "ziyuan", string.Empty);

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

    public class Zhuhai : TriggerSkill
    {
        public Zhuhai() : base("zhuhai")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.EventPhaseStart, TriggerEvent.CardUsedAnnounced };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && player.Alive && room.Current == player)
                player.SetFlags(Name);
            else if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Pattern == "Slash:zhuhai")
            {
                room.ShowSkill(player, Name, string.Empty);
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.Finish && player.HasFlag(Name))
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (player != p) triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive)
            {
                ask_who.SetFlags("slashTargetFix");
                player.SetFlags("SlashAssignee");

                WrappedCard used = room.AskForUseCard(ask_who, "Slash:zhuhai", "@zhuhai-slash:" + player.Name, null, -1, HandlingMethod.MethodUse, false);
                if (used == null)
                {
                    ask_who.SetFlags("-slashTargetFix");
                    player.SetFlags("-SlashAssignee");
                }
            }

            return new TriggerStruct();
        }
    }

    public class ZhuhaiTag : TargetModSkill
    {
        public ZhuhaiTag() : base("#zhuhai", false) { }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseStruct.CardUseReason reason, string pattern)
        {
            if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && to.HasFlag("SlashAssignee")
                && (room.GetRoomState().GetCurrentResponseSkill() == "zhuhai" || pattern == "Slash:zhuhai"))
                return true;

            return false;
        }
    }

    public class Qianxin : TriggerSkill
    {
        public Qianxin() : base("qianxin")
        {
            frequency = Frequency.Wake;
            events.Add(TriggerEvent.Damage);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.IsWounded())
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
            if (player.Alive) room.HandleAcquireDetachSkills(player, "jianyan", true);

            return false;
        }
    }

    public class Jianyan : ZeroCardViewAsSkill
    {
        public Jianyan() : base("jianyan")
        {
            skill_type = SkillType.Replenish;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(JianyanCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JianyanCard.ClassName) { Skill = Name };
        }
    }

    public class JianyanCard : SkillCard
    {
        public static string ClassName = "JianyanCard";
        public JianyanCard() : base(ClassName) { target_fixed = true; }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            string choice = room.AskForChoice(player, "jianyan", "basic+equip+trick+red+black", new List<string> { "@jianyan" });
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
                else if (WrappedCard.IsBlack(card.Suit) && choice == "black")
                {
                    ids.Add(id);
                    break;
                }
                else if (WrappedCard.IsRed(card.Suit) && choice == "red")
                {
                    ids.Add(id);
                    break;
                }
            }
            if (ids.Count > 0)
            {
                room.MoveCardTo(room.GetCard(ids[0]), player, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name, "jianyan", null), false);
                Thread.Sleep(500);
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if (p.IsMale()) targets.Add(p);

                if (targets.Count > 0)
                {
                    player.SetTag("jianyan", ids[0]);
                    Player target = room.AskForPlayerChosen(player, targets, "jianyan", "@jianyan-give", false, false, card_use.Card.SkillPosition);
                    player.RemoveTag("jianyan");
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);

                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, target.Name, "jianyan", string.Empty);
                    room.ObtainCard(target, ref ids, reason, true);
                }
            }
        }
    }

    public class Qianya : TriggerSkill
    {
        public Qianya() : base("qianya")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            view_as_skill = new QianyaVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && Engine.GetFunctionCard(use.Card.Name).TypeID == CardType.TypeTrick && !player.IsKongcheng() && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetTag(Name, data);
            if (room.AskForUseCard(player, "@@qianya", "@qianya", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition) == null)
                room.RemoveTag(Name);

            return new TriggerStruct();
        }
    }

    public class QianyaVS : ViewAsSkill
    {
        public QianyaVS() : base("qianya") { response_pattern = "@@qianya"; }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => false;

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard qy = new WrappedCard(QianyaCard.ClassName) { Skill = Name };
                qy.AddSubCards(cards);
                return qy;
            }

            return null;
        }
    }

    public class QianyaCard : SkillCard
    {
        public static string ClassName = "QianyaCard";
        public QianyaCard() : base(ClassName) { will_throw = false; }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            room.RemoveTag("qianya");
            Player player = card_use.From, target = card_use.To[0];

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "qianya", string.Empty), false);
        }
    }

    public class Shuimeng : TriggerSkill
    {
        public Shuimeng() : base("shuimeng") { events.Add(TriggerEvent.EventPhaseEnd); }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Play && base.Triggerable(player, room) && !player.IsKongcheng())
                return new TriggerStruct(Name, player);

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
            if (targets.Count > 0)
            {
                Player victim = room.AskForPlayerChosen(jiling, targets, Name, "@shuimeng", true, true, info.SkillPosition);
                if (victim != null)
                {
                    room.SetTag(Name, victim);
                    room.BroadcastSkillInvoke(Name, jiling, info.SkillPosition);
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            PindianStruct pd = room.PindianSelect(player, target, Name);
            room.Pindian(ref pd);
            if (pd.Success)
            {
                WrappedCard ex = new WrappedCard(ExNihilo.ClassName) { Skill = "_shuimeng" };
                FunctionCard fcard = Engine.GetFunctionCard(ex.Name);
                if (fcard.IsAvailable(room, player, ex))
                    room.UseCard(new CardUseStruct(ex, player, new List<Player>()));
            }
            else
            {
                WrappedCard ex = new WrappedCard(Dismantlement.ClassName) { Skill = "_shuimeng" };
                FunctionCard fcard = Engine.GetFunctionCard(ex.Name);
                if (fcard.IsAvailable(room, target, ex) && RoomLogic.IsProhibited(room, target, player, ex) == null)
                    room.UseCard(new CardUseStruct(ex, target, player));
            }

            return false;
        }
    }

    public class Fuman : TriggerSkill
    {
        public Fuman() : base("fuman")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.CardUsed };
            view_as_skill = new FumanVS();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change)
            {
                if (change.From == PlayerPhase.Play)
                {
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.HasFlag("fuman_" + player.Name))
                            p.SetFlags("-fuman_" + player.Name);
                }
                else if (change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                    player.RemoveTag(Name);
            }
            else if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill
                && player.ContainsTag(Name) && player.GetTag(Name) is Dictionary<string, List<int>> record)
            {
                foreach (int id in use.Card.SubCards)
                {
                    foreach (string genreal in record.Keys)
                    {
                        if (record[genreal].Contains(id))
                        {
                            Player target = room.FindPlayer(genreal);
                            if (target != null) room.DrawCards(target, 1, Name);
                        }
                    }
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who) => new TriggerStruct();
    }

    public class FumanVS : OneCardViewAsSkill
    {
        public FumanVS() : base("fuman") { filter_pattern = "Slash"; }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard fm = new WrappedCard(FumanCard.ClassName) { Skill = Name };
            fm.AddSubCard(card);
            return fm;
        }
    }

    public class FumanCard : SkillCard
    {
        public static string ClassName = "FumanCard";
        public FumanCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && !to_select.HasFlag("fuman_" + Self.Name);
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

            Dictionary<string, List<int>> record = target.ContainsTag("fuman") ? (Dictionary<string, List<int>>)target.GetTag("fuman") : new Dictionary<string, List<int>>();
            if (record.ContainsKey(card_use.From.Name))
                record[card_use.From.Name].Add(card_use.Card.GetEffectiveId());
            else
                record[card_use.From.Name] = new List<int> { card_use.Card.GetEffectiveId() };
            target.SetTag("fuman", record);

            target.SetFlags("fuman_" + card_use.From.Name);
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, card_use.From.Name, target.Name, "fuman", null);
            room.ObtainCard(target, card_use.Card, reason, true);
        }
    }

    public class Bingzheng : TriggerSkill
    {
        public Bingzheng() : base("bingzheng")
        {
            events.Add(TriggerEvent.EventPhaseEnd);
        }
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Play;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum != p.Hp) targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@bingzheng", true, true, info.SkillPosition);
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
            List<string> choices = new List<string> { "draw" };
            if (!target.IsKongcheng() && RoomLogic.CanDiscard(room, target, target, "h"))
                choices.Add("discard");

            target.SetFlags(Name);
            string choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { "@to-player:" + target.Name }, target);
            target.SetFlags("-bingzheng");
            if (choice == "draw")
            {
                target.SetFlags(Name);
                room.DrawCards(target, new DrawCardStruct(1, player, Name));
                target.SetFlags("-bingzheng");
            }
            else
            {
                room.AskForDiscard(target, Name, 1, 1, false, false, "@bingzheng-discard", false, info.SkillPosition);
            }
            if (target.Alive && target.HandcardNum == target.Hp && player.Alive)
            {
                room.DrawCards(player, 1, Name);
                if (player.Alive && player != target && target.Alive)
                {
                    target.SetFlags(Name);
                    List<int> ids = room.AskForExchange(player, Name, 1, 0, "@bingzheng-give:" + target.Name, string.Empty, "..", info.SkillPosition);
                    target.SetFlags("-bingzheng");
                    if (ids.Count > 0)
                        room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);
                }
            }

            return false;
        }
    }

    public class Sheyan : TriggerSkill
    {
        public Sheyan() : base("sheyan")
        {
            events.Add(TriggerEvent.TargetConfirming);
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room) && use.To.Contains(player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.IsNDTrick() && use.Card.Name != Collateral.ClassName)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                if (use.To.Contains(player) && use.To.Count > 1) targets.Add(player);

                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (use.To.Contains(p))
                        targets.Add(p);
                    else if (RoomLogic.IsProhibited(room, use.From, p, use.Card) == null)
                    {
                        if ((fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, player, p))
                        || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && (!RoomLogic.CanGetCard(room, player, p, "hej") || p == use.From))
                        || (fcard is Dismantlement && (!RoomLogic.CanDiscard(room, player, p, "hej") || p == use.From))
                        || (fcard is Duel && p == use.From) || ((fcard is ArcheryAttack || fcard is SavageAssault) && p == use.From)) continue;
                        targets.Add(p);
                    }
                }

                if (targets.Count > 0)
                {
                    room.SetTag(Name, data);
                    Player target = room.AskForPlayerChosen(player, targets, Name, string.Format("@sheyan:{0}::{1}", use.From.Name, use.Card.Name), true, true, info.SkillPosition);
                    room.RemoveTag(Name);
                    if (target != null)
                    {
                        room.SetTag(Name, target);
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
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
            if (data is CardUseStruct use)
            {
                if (use.To.Contains(target))
                {
                    if (target != player && (use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == SavageAssault.ClassName))
                    {
                        ResultStruct result = player.Result;
                        result.Assist++;
                        ask_who.Result = result;
                    }

                    room.CancelTarget(ref use, target);
                    data = use;
                    if (target == player) return true;
                }
                else
                {
                    if (use.Card.Name == ExNihilo.ClassName)
                    {
                        ResultStruct result = player.Result;
                        result.Assist++;
                        ask_who.Result = result;
                    }

                    use.To.Add(target);
                    use.EffectCount.Add(new CardBasicEffect(target, 0, 1, 0));
                    room.SortByActionOrder(ref use);
                    data = use;
                }
            }

            return false;
        }
    }

    public class Baobian : TriggerSkill
    {
        public Baobian() : base("baobian")
        {
            events = new List<TriggerEvent> { TriggerEvent.HpChanged, TriggerEvent.GameStart, TriggerEvent.EventAcquireSkill, TriggerEvent.EventLoseSkill };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
            view_as_skill = new BaobianVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                player.SetMark(Name, player.Hp);
            else if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && info.Info == Name)
                player.SetMark(Name, player.Hp);
            else if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct _info && _info.Info == Name)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room)) return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Hp != player.GetMark(Name))
            {
                if ((player.Hp < player.GetMark(Name) && player.Hp <= 3 && player.GetMark(Name) > 1) || (player.Hp > player.GetMark(Name) && player.GetMark(Name) <= 3))
                room.SendCompulsoryTriggerLog(player, Name);
            }
            player.SetMark(Name, player.Hp);
            return false;
        }
    }

    public class BaobianVS : ZeroCardViewAsSkill
    {
        public BaobianVS() : base("baobian")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(TiaoxinJXCard.ClassName) && player.Hp <= 3;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(TiaoxinJXCard.ClassName)
            {
                Skill = "tiaoxin_jx",
                ShowSkill = Name
            };
            return card;
        }
    }

    public class BaobianVH : ViewHasSkill
    {
        public BaobianVH() : base("#baobian")
        {
            viewhas_skills = new List<string> { "shensu_jx", "paoxiao_jx" };
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (player.Alive && RoomLogic.PlayerHasSkill(room, player, Name))
            {
                if (skill_name == "shensu_jx" && player.Hp == 1) return true;
                if (skill_name == "paoxiao_jx" && player.Hp <= 2) return true;
            }
            return false;
        }
    }

    public class Dianhu : TriggerSkill
    {
        public Dianhu() : base("dianhu")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.Damage, TriggerEvent.HpRecover };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && damage.To.ContainsTag(Name) && damage.To.GetTag(Name) is List<string> names
                && names.Contains(player.Name))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.HpRecover && player.ContainsTag(Name) && player.GetTag(Name) is List<string> _names)
            {
                foreach (string genreal in _names)
                {
                    Player target = room.FindPlayer(genreal);
                    if (target != null) triggers.Add(new TriggerStruct(Name, target));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.GameStart)
            {
                Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@dianhu", false, true, info.SkillPosition);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);

                List<string> names = target.ContainsTag(Name) ? (List<string>)target.GetTag(Name) : new List<string>();
                names.Add(player.Name);
                target.SetTag(Name, names);
                room.SetPlayerStringMark(target, Name, string.Empty);
            }
            else
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                room.DrawCards(ask_who, 1, Name);
            }

            return false;
        }
    }

    public class Jianji : ZeroCardViewAsSkill
    {
        public Jianji() : base("jianji")
        { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(JianjiCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JianjiCard.ClassName) { Skill = Name };
        }
    }

    public class JianjiCard : SkillCard
    {
        public static string ClassName = "JianjiCard";
        public JianjiCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && Self != to_select;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            List<int> ids = room.DrawCards(target, new DrawCardStruct(1, card_use.From, "jianji"));
            if (ids.Count == 1)
            {
                int id = ids[0];
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (target.Alive && room.GetCardOwner(id) == target && room.GetCardPlace(id) == Place.PlaceHand && fcard.IsAvailable(room, target, card))
                    room.AskForUseCard(target, id.ToString(), "@jianji", null);
            }
        }
    }

    public class Yuxu : TriggerSkill
    {
        public Yuxu() : base("yuxu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.CardFinished };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill
                && base.Triggerable(player, room) && (player.GetMark(Name) == 0 || player.GetMark(Name) % 2 == 0) && player.Phase == PlayerPhase.Play
                && !use.Card.HasFlag("#yuxu"))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, "@yuxu-draw", info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.DrawCards(ask_who, 1, Name);
                ask_who.AddMark(Name);
                use.Card.SetFlags(Name);
            }
            return false;
        }
    }

    public class YuxuDiscard : TriggerSkill
    {
        public YuxuDiscard() : base("#yuxu")
        {
            events.Add(TriggerEvent.CardFinished);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && (player.GetMark("yuxu") == 1 || player.GetMark("yuxu") % 2 == 1) && !use.Card.HasFlag("yuxu") && player.Phase == PlayerPhase.Play
                && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke("yuxu", "male", 2, gsk.General, gsk.SkinId);

                use.Card.SetFlags(Name);
                player.AddMark("yuxu");
                room.AskForDiscard(player, "yuxu", 1, 1, false, true, "@yuxu-discard", false, info.SkillPosition);
            }
            return false;
        }
    }

    public class Shijian : TriggerSkill
    {
        public Shijian() : base("shijian")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.EventPhaseChanging };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                if (player.GetMark(Name) > 0) player.SetMark(Name, 0);
                if (player.HasFlag(Name))
                {
                    player.SetFlags("-shijian");
                    room.HandleAcquireDetachSkills(player, "-yuxu", true);
                }
            }
            else if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill && player.Phase == PlayerPhase.Play)
            {
                player.AddMark(Name);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardFinished && player.GetMark(Name) == 2 && !player.HasFlag(Name) && player.Phase == PlayerPhase.Play)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p != player && !p.IsKongcheng())
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForDiscard(ask_who, Name, 1, 1, true, true, string.Format("@shijian:{0}", player.Name), true, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive)
            {
                player.SetFlags(Name);
                room.HandleAcquireDetachSkills(player, "yuxu", true);
            }

            return false;
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
                && ((move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_RESPONSE
                ||(move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_USE
                || (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD))
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
                        List<int> ids = target.GetEquips();
                        if (ids.Count > 0)
                            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, target.Name, "dingpan", string.Empty));

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
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
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
                room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, Name, string.Empty));
                RoomLogic.SetPlayerCardLimitation(player, Name, "use,response", "Slash", true);
            }
            else
            {
                foreach (int id in target.GetEquips())
                    if (!RoomLogic.CanGetCard(room, player, target, id) && !RoomLogic.CanDiscard(room, player, target, id)) ids.Add(id);

                int card_id = room.AskForCardChosen(player, target, "e", Name, false, HandlingMethod.MethodNone, ids);
                List<string> choices = new List<string>();
                if (RoomLogic.CanGetCard(room, player, target, card_id) && Engine.GetFunctionCard(room.GetCard(card_id).Name) is Armor) choices.Add("get");
                if (RoomLogic.CanDiscard(room, player, target, card_id)) choices.Add("discard");
                if (room.AskForChoice(player, Name, string.Join("+", choices), null, card_id) == "get")
                {
                    room.ObtainCard(player, card_id);
                    RoomLogic.SetPlayerCardLimitation(player, Name, "use,response", "Slash", true);
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
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                {
                    player.AddMark(Name);
                    if (!player.HasFlag(Name) && (player.GetMark(Name) >= player.Hp || fcard is TrickCard))
                    {
                        RoomLogic.SetPlayerCardLimitation(player, Name, "use", ".", true);
                        player.SetFlags(Name);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play)
            {
                room.HandleAcquireDetachSkills(player, "-zhixi", true);
                if (player.HasFlag(Name)) RoomLogic.RemovePlayerCardLimitation(player, Name);
                if (player.GetMark(Name) > 0) player.SetMark(Name, 0);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Lianpian : TriggerSkill
    {
        public Lianpian() : base("lianpian")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChoosing, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.ContainsTag(Name))
            {
                player.RemoveTag(Name);
                player.SetMark(Name, 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChoosing && data is CardUseStruct use && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play
                && player.ContainsTag(Name) && player.GetTag(Name) is List<string> names && player.GetMark(Name) < 3)
            {
                foreach (Player p in use.To)
                    if (names.Contains(p.Name))
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
            player.AddMark(Name);
            List<int> ids = room.DrawCards(player, 1, Name);
            if (player.Alive && ids.Count > 0)
            {
                if (data is CardUseStruct use && player.ContainsTag(Name) && player.GetTag(Name) is List<string> names)
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player p in use.To)
                        if (p != player && names.Contains(p.Name) && p.Alive)
                            targets.Add(p);

                    if (targets.Count > 0)
                    {
                        Player target = room.AskForPlayerChosen(player, targets, Name, "@lianpian:::" + room.GetCard(ids[0]).Name, true, false, info.SkillPosition);
                        if (target != null)
                        {
                            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);
                        }
                    }
                }
            }

            return false;
        }
    }

    public class LianpianRecord : TriggerSkill
    {
        public LianpianRecord() : base("#lianpian")
        {
            events.Add(TriggerEvent.TargetChoosing);
        }
        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardUseStruct use && player.Phase == PlayerPhase.Play)
            {
                if (use.To.Count > 0)
                {
                    List<string> names = new List<string>();
                    foreach (Player p in use.To)
                        names.Add(p.Name);

                    player.SetTag("lianpian", names);
                }
                else if (player.ContainsTag("lianpian"))
                    player.RemoveTag("lianpian");
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Yinbing : TriggerSkill
    {
        public Yinbing() : base("yinbing")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                List<int> ids = room.AskForExchange(player, Name, 400, 0, "@yinbing", string.Empty, "TrickCard,EquipCard", info.SkillPosition);
                if (ids.Count > 0)
                {
                    room.NotifySkillInvoked(player, Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.AddToPile(player, Name, ids);
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

    public class YinbingDamage : TriggerSkill
    {
        public YinbingDamage() : base("#yinbing")
        {
            events.Add(TriggerEvent.Damaged);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && player.GetPile("yinbing").Count > 0 && damage.Card != null
                && (damage.Card.Name.Contains(Slash.ClassName) || damage.Card.Name == Duel.ClassName))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            List<int> maps = player.GetPile("yinbing");
            if (maps.Count > 0)
            {
                int card_id;
                if (maps.Count == 1)
                    card_id = maps[0];
                else
                {
                    room.FillAG("yinbing", maps, player);
                    card_id = room.AskForAG(player, maps, false, "yinbing");
                    room.ClearAG(player);
                }

                LogMessage log = new LogMessage
                {
                    Type = "$RemoveFromPile",
                    From = player.Name,
                    Arg = "yinbing",
                    Card_str = card_id.ToString()
                };
                room.SendLog(log);

                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, null, "yinbing", null);
                List<int> ids = new List<int> { card_id };
                room.ThrowCard(ref ids, reason, null);
                room.ClearAG(player);
            }
            return false;
        }
    }

    public class Juedi : PhaseChangeSkill
    {
        public Juedi() : base("juedi")
        {
            frequency = Frequency.Compulsory;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Start && target.GetPile("yinbing").Count > 0;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Hp <= player.Hp) targets.Add(p);

            room.RemoveTag(Name);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@juedi", true, true, info.SkillPosition);
                if (target != null) room.SetTag(Name, target);
            }
            return info;
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            List<int> ids = player.GetPile("yinbing");
            if (room.ContainsTag(Name) && room.GetTag(Name) is Player target)
            {
                room.RemoveTag(Name);
                int count = ids.Count;
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTCARD, target.Name, Name, string.Empty);
                room.ObtainCard(target, ref ids, reason);
                if (target.Alive && target.IsWounded())
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = player
                    };
                    room.Recover(target, recover, true);
                }
                if (target.Alive) room.DrawCards(target, new DrawCardStruct(count, target, Name));
            }
            else
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, null, Name, null);
                room.ThrowCard(ref ids, reason, null);
                if (player.Alive && player.HandcardNum < player.MaxHp)
                    room.DrawCards(player, player.MaxHp - player.HandcardNum, Name);
            }

            return false;
        }
    }

    public class CanshiSH : DrawCardsSkill
    {
        public CanshiSH() : base("canshi_sh")
        {
            frequency = Frequency.Frequent;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw && (int)data >= 0)
            {
                int count = 0;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.IsWounded() || (RoomLogic.PlayerHasSkill(room, player, "guiming") && p.Kingdom == "wu" && p != player))
                        count++;
                }

                if (count > 0)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = 0;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.IsWounded() || (RoomLogic.PlayerHasSkill(room, lidian, "guiming") && p.Kingdom == "wu" && p != lidian))
                    count++;
            }
            if (room.AskForSkillInvoke(lidian, Name, "@canshi_sh:::" + count.ToString(), info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, lidian, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player lidian, int n)
        {
            lidian.SetFlags(Name);
            int count = 0;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.IsWounded() || (RoomLogic.PlayerHasSkill(room, lidian, "guiming") && p.Kingdom == "wu" && p != lidian))
                    count++;
            }
            return n + count;
        }
    }

    public class CanshiDiscard : TriggerSkill
    {
        public CanshiDiscard() : base("#canshi-discard")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && player.HasFlag("canshi_sh"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || fcard.TypeID == CardType.TypeTrick)
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && player.HasFlag("canshi_sh"))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, "canshi_sh");
            room.AskForDiscard(player, "canshi_sh", 1, 1, false, true, "@canshi-discard", false, info.SkillPosition);
            return false;
        }
    }

    public class Chouhai : TriggerSkill
    {
        public Chouhai() : base("chouhai")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && base.Triggerable(player, room) && player.IsKongcheng() && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName))
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
                Type = "#AddDamaged",
                From = player.Name,
                Arg = Name,
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);

            data = damage;

            return false;
        }
    }

    public class Guiming : TriggerSkill
    {
        public Guiming() : base("guiming")
        {
            lord_skill = true;
            frequency = Frequency.Compulsory;
            events.Add(TriggerEvent.EventPhaseStart);
        }

        public override bool Triggerable(Player target, Room room)
        {
            if (base.Triggerable(target, room) && target.Phase == PlayerPhase.RoundStart)
            {
                foreach (Player p in room.GetOtherPlayers(target))
                    if (p.Kingdom == "wu") return true;
            }
            return false;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "wu")
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

            return false;
        }
    }

    public class Xiashu : TriggerSkill
    {
        public Xiashu() : base("xiashu")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Wizzard;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Play && !target.IsKongcheng();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@xiashu-target", true, true, info.SkillPosition);
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
            List<int> ids = player.GetCards("h");
            target.SetFlags(Name);
            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);
            target.SetFlags("-xiashu");
            if (target.Alive && !target.IsKongcheng() && player.Alive)
            {
                List<int> showed = target.GetCards("h");
                if (showed.Count > 1) showed = room.AskForExchange(target, Name, showed.Count, 1, "@xiashu-shown:" + player.Name, string.Empty, ".", string.Empty);
                room.ShowCards(target, showed, Name);
                /*
                room.SetTag(Name, showed);
                room.FillAG(Name, showed, player, null, null);
                string choice = room.AskForChoice(player, Name, "shown+hide", null, target);
                room.ClearAG(player);
                room.RemoveTag(Name);
                if (choice == "shown")
                {
                    room.ObtainCard(player, ref showed, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, target.Name, string.Empty));
                }
                else
                {
                    List<int> hands = target.GetCards("h");
                    hands.RemoveAll(t => showed.Contains(t));
                    if (hands.Count > 0)
                        room.ObtainCard(player, ref hands, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, target.Name, string.Empty), false);
                }
                */

                AskForMoveCardsStruct move = room.AskForMoveCards(player, showed, new List<int>(), false, Name, 0, 0, true, false, new List<int>(), info.SkillPosition);
                room.RemoveTag(Name);
                if (move.Success)
                {
                    room.ObtainCard(player, ref showed, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, target.Name, string.Empty));
                }
                else
                {
                    List<int> hands = target.GetCards("h");
                    hands.RemoveAll(t => showed.Contains(t));
                    if (hands.Count > 0)
                        room.ObtainCard(player, ref hands, new CardMoveReason(MoveReason.S_REASON_GOTCARD, player.Name, target.Name, string.Empty), false);
                }
            }

            return false;
        }
    }

    public class Kuanshi : TriggerSkill
    {
        public Kuanshi() : base("kuanshi")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Defense;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == PlayerPhase.RoundStart && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                foreach (Player p in room.GetAlivePlayers())
                    if (p.ContainsTag(Name) && p.GetTag(Name) is string name && name == player.Name)
                        p.RemoveTag(Name);

                if (player.GetMark("kuanshi_skip") > 0)
                {
                    player.SetMark("kuanshi_skip", 0);
                    room.SkipPhase(player, PlayerPhase.Draw);
                }
            }
        }
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (!p.ContainsTag(Name)) targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@kuanshi", true, false, info.SkillPosition);
                if (target != null)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name, new List<Player> { player });
                    room.NotifySkillInvoked(player, Name);
                    room.SetTag(Name, target);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            player.AddMark(Name);
            target.SetTag(Name, player.Name);

            return false;
        }
    }

    public class KuanshiIm : TriggerSkill
    {
        public KuanshiIm() : base("#kuanshi")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageDefined };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.Damage >= 2 && player.ContainsTag("kuanshi") && player.GetTag("kuanshi") is string name)
            {
                Player target = room.FindPlayer(name);
                if (target != null) return new TriggerStruct(Name, target);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
            player.RemoveTag("kuanshi");
            ask_who.AddMark("kuanshi_skip");
            room.SendCompulsoryTriggerLog(ask_who, "kuanshi");
            if (RoomLogic.PlayerHasSkill(room, ask_who, Name))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, "kuanshi", info.SkillPosition);
                room.BroadcastSkillInvoke("kuanshi", "male", 2, gsk.General, gsk.SkinId);
            }
            LogMessage log = new LogMessage
            {
                Type = "#damaged-prevent",
                From = player.Name,
                Arg = "kuanshi"
            };
            room.SendLog(log);
            return true;
        }
    }

    public class Qizhou : TriggerSkill
    {
        public Qizhou() : base("qizhou") { events.Add(TriggerEvent.GameStart); frequency = Frequency.Compulsory; }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (base.Triggerable(player, room))
                room.AddPlayerMark(player, "@fenwei");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class QizhouVH : ViewHasSkill
    {
        public QizhouVH() : base("#qizhou")
        {
            viewhas_skills = new List<string> { "fenwei", "mashu_heqi", "duanbing", "yingzi_heqi" };
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (player.Alive && RoomLogic.PlayerHasSkill(room, player, "qizhou") && player.HasEquip())
            {
                List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
                foreach (int id in player.GetEquips())
                {
                    WrappedCard.CardSuit suit = room.GetCard(id).Suit;
                    if (!suits.Contains(suit)) suits.Add(suit);
                }

                if (skill_name == "mashu_heqi" && suits.Count >= 1) return true;
                if (skill_name == "yingzi_heqi" && suits.Count >= 2) return true;
                if (skill_name == "duanbing" && suits.Count >= 3) return true;
                if (skill_name == "fenwei" && suits.Count >= 4) return true;
            }
            return false;
        }
    }

    public class Shanxi : TriggerSkill
    {
        public Shanxi() : base("shanxi")
        {
            events = new List<TriggerEvent> {  TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Attack;
            view_as_skill = new ShanxiVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.PlaceTable
                && move.From != null && move.Reason.Reason == MoveReason.S_REASON_DISMANTLE && move.Reason.SkillName == Name
                && move.From.Name == move.Reason.TargetId && move.Card_ids.Count == 1)
            {
                bool equip = room.GetCard(move.Card_ids[0]).Name == Jink.ClassName;
                string tag_name = string.Format("{0}_{1}", Name, move.Reason.TargetId);
                Player pangde = room.FindPlayer(move.Reason.PlayerId, true);
                pangde.SetTag(tag_name, equip);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class ShanxiVS : OneCardViewAsSkill
    {
        public ShanxiVS() : base("shanxi") { filter_pattern = "BasicCard|red!"; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ShanxiCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard sx = new WrappedCard(ShanxiCard.ClassName) { Skill = Name };
            sx.AddSubCard(card);
            return sx;
        }
    }

    public class ShanxiCard : SkillCard
    {
        public static string ClassName = "ShanxiCard";
        public ShanxiCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && RoomLogic.InMyAttackRange(room, Self, to_select, card)
                && !to_select.IsNude() && RoomLogic.CanDiscard(room, Self, to_select, "he");
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player pangde = card_use.From, target = card_use.To[0];
            int to_throw = room.AskForCardChosen(pangde, target, "he", "shanxi", false, HandlingMethod.MethodDiscard);
            List<int> ids = new List<int> { to_throw };
            string tag_name = string.Format("{0}_{1}", "shanxi", target.Name);
            pangde.RemoveTag(tag_name);
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISMANTLE, pangde.Name, target.Name, "shanxi", string.Empty)
            {
                General = RoomLogic.GetGeneralSkin(room, pangde, "shanxi", card_use.Card.SkillPosition)
            };
            room.ThrowCard(ref ids, reason, target, pangde);
            if (ids.Count > 0)
            {
                Debug.Assert(pangde.ContainsTag(tag_name), "shanxi tag error!");
                if ((bool)pangde.GetTag(tag_name))
                    room.ShowAllCards(target, pangde, "shanxi", card_use.Card.SkillPosition);
                else
                    room.ShowAllCards(pangde, target, "shanxi", card_use.Card.SkillPosition);
            }
        }
    }

    public class Bizheng : TriggerSkill
    {
        public Bizheng() : base("bizheng")
        {
            events.Add(TriggerEvent.EventPhaseEnd);
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
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@bizheng", true, true, info.SkillPosition);
            if (target != null)
            {
                room.SetTag(Name, target);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.GetTag(Name) is Player target)
            {
                room.RemoveTag(Name);
                room.DrawCards(target, new DrawCardStruct(2, player, Name));

                if (player.Alive && player.HandcardNum > player.MaxHp)
                    room.AskForDiscard(player, Name, 2, 2, false, true, "@bizheng-discard");
                if (target.Alive && target.HandcardNum > target.MaxHp)
                    room.AskForDiscard(target, Name, 2, 2, false, true, "@bizheng-discard");
            }

            return false;
        }
    }

    public class Yidian : TriggerSkill
    {
        public Yidian() : base("yidian")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardTargetAnnounced };
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct _use && base.Triggerable(player, room) && _use.Card.ExtraTarget)
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                if (fcard is BasicCard || (fcard.IsNDTrick() && _use.Card.Name != Collateral.ClassName && !_use.Card.Name.Contains(Nullification.ClassName)))
                {
                    bool same = false;
                    string card_name = _use.Card.Name;
                    if (card_name.Contains("Slash")) card_name = "Slash";
                    foreach (int id in room.DiscardPile)
                    {
                        if (room.GetCard(id).Name.Contains(card_name))
                        {
                            same = true;
                            break;
                        }
                    }

                    if (!same)
                    {
                        foreach (Player p in room.GetOtherPlayers(player))
                            if (!_use.To.Contains(p))
                                return new TriggerStruct(Name, player);
                    }
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced)
            {
                List<Player> targets = new List<Player>();
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if ((fcard is Peach && !p.IsWounded()) || (fcard is IronChain && !p.Chained && !RoomLogic.CanBeChainedBy(room, player, p))
                        || (fcard is FireAttack && p.IsKongcheng()) || (fcard is Snatch && !RoomLogic.CanGetCard(room, player, p, "hej"))
                        || (fcard is Dismantlement && !RoomLogic.CanDiscard(room, player, p, "hej"))) continue;

                    if (!use.To.Contains(p) && RoomLogic.IsProhibited(room, player, p, use.Card) == null)
                        targets.Add(p);
                }

                room.SetTag("extra_target_skill", data);                   //for AI
                Player target = room.AskForPlayerChosen(player, targets, Name, "@yidian:::" + use.Card.Name, true, true, info.SkillPosition);
                room.RemoveTag("extra_target_skill");
                if (target != null)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "$extra_target",
                        From = player.Name,
                        To = new List<string> { target.Name},
                        Card_str = RoomLogic.CardToString(room, use.Card),
                        Arg = Name
                    };
                    room.SendLog(log);

                    room.SetTag("extra_targets", target);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            Player target = (Player)room.GetTag("extra_targets");
            room.RemoveTag("extra_targets");

            use.To.Add(target);
            room.SortByActionOrder(ref use);
            data = use;

            return false;
        }
    }
}
