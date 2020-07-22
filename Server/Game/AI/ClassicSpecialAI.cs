using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class ClassicSpecialAI : AIPackage
    {
        public ClassicSpecialAI() : base("ClassicSpecial")
        {
            events = new List<SkillEvent>
            {
                new XianfuAI(),
                new ChouceAI(),
                new ChenqingAI(),
                new MoshiAI(),
                new ShanjiaAI(),
                new GusheAI(),
                new JiciAI(),
                new QizhiAI(),
                new JinquAI(),
                new KangkaiAI(),
                new XiaoguoJXAI(),
                new FenkunAI(),
                new WeikuiAI(),
                new LizhanAI(),
                new ZhenweiAI(),
                new TuifengAI(),
                new QujiAI(),
                new JunbingAI(),
                new JianshuAI(),
                new YongdiAI(),
                new LingrenAI(),
                new JuesiAI(),
                new QingzhongAI(),
                new WeijingAI(),

                new ChongzhenAI(),
                new MizhaoAI(),
                new TianmingAI(),
                new WeidiJXAI(),
                new LihunAI(),
                new ZongkuiAI(),
                new CanshiAI(),
                new BingzhaoAI(),
                new ShichouAI(),
                new QiaomengAI(),
                new JiqiaoAI(),
                new JiaoziAI(),
                new YisheAI(),
                new BushiAI(),
                new MidaoAI(),
                new MoukuiAI(),
                new RangshangAI(),
                new HanyongAI(),
                new XionghuoAI(),
                new YisuanAI(),
                new LangxiAI(),
                new ZhenyiAI(),
                new DianhuaAI(),
                new LuanzhanAI(),
                new FuqiAI(),
                new BiluanAI(),
                new LixiaAI(),
                new WenjiAI(),
                new TunjiangAI(),
                new HuojiSMAI(),
                new LianhuanSMAI(),
                new JianjieAI(),
                new YinshiAI(),
                new LuemingAI(),
                new TunjunAI(),
                new XingluanAI(),
                new SidaoAI(),
                new TanbeiAI(),
                new LianjiAI(),
                new LianzhuAI(),
                new ZhoufuAI(),
                new NeifaAI(),
                new ZhenduJXAI(),
                new QiluanJXAI(),
                new BeizhanCAI(),

                new XuejiAI(),
                new LiangzhuAI(),
                new ShenxianAI(),
                new QiangwuAI(),
                new FengpoAI(),
                new ZhengnanAI(),
                new ZhennanAI(),
                new XushenAI(),
                new QirangAI(),
                new FanghunAI(),
                new FuhanAI(),
                new ZishuAI(),
                new YingyuanAI(),
                new ZiyuanAI(),
                new FumanAI(),
                new ShuimengAI(),
                new ZhuhaiAI(),
                new JianyanAI(),
                new BaobianAI(),
                new BingzhengAI(),
                new SheyanAI(),
                new DianhuAI(),
                new JianjiAI(),
                new YuxuAI(),
                new ShijianAI(),

                new HongyuanAI(),
                new HuanshiAI(),
                new MingzheAI(),
                new AocaiAI(),
                new DuwuAI(),
                new HongdeAI(),
                new DingpanAI(),
                new MeibuAI(),
                new MumuAI(),
                new ZhixiAI(),
                new YinbingAI(),
                new JuediAI(),
                new CanshiSHAI(),
                new ChouhaiAI(),
                new XiashuAI(),
                new QizhouAI(),
                new ShanxiAI(),
            };

            use_cards = new List<UseCard>
            {
                new XuejiCardAI(),
                new QiangwuCardAI(),
                new DuwuCardAI(),
                new MizhaoCardAI(),
                new LihunCardAI(),
                new GusheCardAI(),
                new DingpanCardAI(),
                new WeikuiCardAI(),
                new ZiyuanCardAI(),
                new QujiCardAI(),
                new XionghuoCardAI(),
                new JianshuCardAI(),
                new FumanCardAI(),
                new QianyaCardAI(),
                new JianyanCardAI(),
                new JuesiCardAI(),
                new ShanxiCardAI(),
                new LuemingCardAI(),
                new TunjunCardAI(),
                new TanbeiCardAI(),
                new LianzhuCardAI(),
                new ZhoufuCardAI(),
                new JianjiCardAI(),
            };
        }
    }

    public class XuejiAI : SkillEvent
    {
        public XuejiAI() : base("xueji") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(XuejiCard.ClassName) && !player.IsNude())
                return new List<WrappedCard> { new WrappedCard(XuejiCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class XuejiCardAI : UseCard
    {
        public XuejiCardAI() : base(XuejiCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            base.OnEvent(ai, triggerEvent, player, data);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            int count = Math.Max(1, player.GetLostHp());
            List<ScoreStruct> scores = new List<ScoreStruct>();
            Room room = ai.Room;
            foreach (Player p in room.GetAllPlayers())
            {
                DamageStruct damage = new DamageStruct("xueji", player, p, 1, DamageStruct.DamageNature.Fire);
                ScoreStruct score = ai.GetDamageScore(damage);
                score.Players = new List<Player> { p };
                scores.Add(score);
            }

            scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
            if (scores[0].Score > 0)
            {
                double value = scores[0].Score;
                Player target = scores[0].Players[0];
                if (scores[0].DoDamage)
                {
                    int damama_count = scores[0].Damage.Damage;
                    foreach (Player p in room.GetOtherPlayers(target))
                    {
                        if (p.Chained)
                        {
                            DamageStruct damage = new DamageStruct("xueji", player, p, damama_count, DamageStruct.DamageNature.Fire)
                            {
                                Chain = true
                            };
                            value += ai.GetDamageScore(damage).Score;
                        }
                    }
                }

                if (value > 0)
                {
                    use.To.Add(target);
                    if (count > 1)
                    {
                        int damama_count = scores[0].Damage.Damage;
                        List<ScoreStruct> scores2 = new List<ScoreStruct>();
                        foreach (Player p in room.GetOtherPlayers(target))
                        {
                            if (!p.Chained)
                            {
                                DamageStruct damage = new DamageStruct("xueji", player, p, damama_count, DamageStruct.DamageNature.Fire)
                                {
                                    Chain = true
                                };
                                ScoreStruct score = ai.GetDamageScore(damage);
                                score.Players = new List<Player> { p };
                                scores2.Add(score);
                            }
                        }
                        scores2.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                        for (int i = 0; i < Math.Min(count - 1, scores2.Count); i++)
                        {
                            if (scores2[i].Score > 0)
                            {
                                use.To.AddRange(scores2[i].Players);
                                value += scores2[i].Score;
                            }
                            else
                                break;
                        }
                    }
                }

                if (use.To.Count > 0)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("he"))
                    {
                        if (WrappedCard.IsRed(room.GetCard(id).Suit) && RoomLogic.CanDiscard(room, player, player, id))
                            ids.Add(id);
                    }

                    if (ids.Count > 0)
                    {
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < 0)
                        {
                            card.AddSubCard(ids[0]);
                            use.Card = card;
                            return;
                        }

                        values = ai.SortByUseValue(ref ids, false);
                        if (values[0] < value)
                        {
                            card.AddSubCard(ids[0]);
                            use.Card = card;
                            return;
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 2;
        }
    }

    public class LiangzhuAI : SkillEvent
    {
        public LiangzhuAI() : base("liangzhu") { key = new List<string> { "skillChoice:liangzhu" }; }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && ai.Self != player)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Player target = null;
                    Room room = ai.Room;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        ai.UpdatePlayerRelation(player, target, strs[2] == "let_draw");
                    }
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player target && ai.IsFriend(target))
                return "let_draw";

            return "draw_self";
        }
    }

    public class ShenxianAI : SkillEvent
    {
        public ShenxianAI() : base("shenxian") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.HasFlag("DimengTarget"))
            {
                foreach (Player p in ai.Room.GetOtherPlayers(player))
                    if (p.HasFlag("DimengTarget"))
                        return ai.IsFriend(p);
            }

            return true;
        }
    }

    public class QiangwuAI : SkillEvent
    {
        public QiangwuAI() : base("qiangwu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(QiangwuCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(QiangwuCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class QiangwuCardAI : UseCard
    {
        public QiangwuCardAI() : base(QiangwuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 8;
        }
    }

    public class FengpoAI : SkillEvent
    {
        public FengpoAI() : base("fengpo") { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player target)
            {
                Room room = ai.Room;
                if (ai.IsEnemy(target) && room.GetTag(Name) is CardUseStruct use)
                {
                    if (ai.IsCancelTarget(use.Card, target, player) && !ai.HasSkill("zhenlie", target)
                        && !target.HasArmor(SilverLion.ClassName) && !(target.Hp == 1 && ai.HasSkill("buqu_jx", target))
                        && ((use.Card.Name.Contains(Slash.ClassName) && (ai.IsLackCard(target, Jink.ClassName) || (player.HasWeapon(Axe.ClassName) && player.GetCardCount(true) > 2)))
                        || (use.Card.Name == Duel.ClassName && ai.IsLackCard(target, Slash.ClassName))))
                        return "damage";
                }
            }

            return "draw";
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (ai.HasSkill(Name, from) && from.Phase != PlayerPhase.NotActive && from.GetMark(Name) == 0 && (card.Name.Contains(Slash.ClassName) || card.Name == Duel.ClassName)
                && targets.Count <= 0)
            {
                return ai.IsFriend(from) ? to.HandcardNum * 0.15 : -to.HandcardNum * 0.15;
            }

            return 0;
        }
    }

    public class ZhengnanAI : SkillEvent
    {
        public ZhengnanAI() : base("zhengnan") { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (player.HandcardNum < 5)
                return "draw";

            if (choice.Contains("dangxian"))
                return "dangxian";
            if (choice.Contains("zhiman_jx"))
                return "zhiman_jx";
            if (choice.Contains("wusheng_jx"))
                return "wusheng_jx";

            return "draw";
        }
    }

    public class WuniangAI : SkillEvent
    {
        public WuniangAI() : base("wuniang") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            Player guansuo = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.ActualGeneral1 == "guansuo")
                {
                    if (!ai.IsFriend(p))
                        return new List<Player>();
                    else
                        guansuo = p;
                }
            }

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in targets)
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, Name, "he", FunctionCard.HandlingMethod.MethodGet);
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                if (scores[0].Score > 0)
                {
                    return scores[0].Players;
                }
            }
            if (guansuo != null && targets.Contains(guansuo)) return new List<Player> { guansuo };

            return new List<Player>();
        }
    }

    public class XushenAI : SkillEvent
    {
        public XushenAI() : base("xushen") { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (player.GetMark("@duanchang") > 0) return "change";
            if (ai.HasSkill("xianfu"))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.ContainsTag("xianfu"))
                        return "cancel";

                return "change";
            }
            if (room.AliveCount() > 5) return "change";
            return "cancel";
        }
    }

    public class ZhennanAI : SkillEvent
    {
        public ZhennanAI() : base("zhennan")
        {
            key = new List<string> { "playerChosen:zhennan" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsEnemy(p))
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class QirangAI : SkillEvent
    {
        public QirangAI() : base("qirang")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class FanghunAI : SkillEvent
    {
        public FanghunAI() : base("fanghun")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("meiying") == 0) return new List<WrappedCard>();
            Room room = ai.Room;
            List<int> ids = player.GetCards("h");
            ids.AddRange(player.GetHandPile());
            WrappedCard fh = new WrappedCard(FanghunCard.ClassName) { Skill = Name };
            foreach (int id in ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if (card.Name == Jink.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name,
                        UserString = Slash.ClassName
                    };
                    slash.AddSubCard(card);
                    fh.ClearSubCards();
                    fh.AddSubCard(id);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    slash.UserString = RoomLogic.CardToString(room, fh);
                    return new List<WrappedCard> { slash };
                }
            }
            return null;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            if (player.GetMark("meiying") == 0) return null;

            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            WrappedCard fh = new WrappedCard(FanghunCard.ClassName) { Skill = Name };

            if (player.GetHandPile().Contains(id) || place == Place.PlaceHand)
            {
                if (card.Name == Jink.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    fh.AddSubCard(card);
                    fh.UserString = Slash.ClassName;
                    slash = RoomLogic.ParseUseCard(room, slash);
                    slash.UserString = RoomLogic.CardToString(room, fh);
                    return slash;
                }
                else if (card.Name.Contains(Slash.ClassName))
                {
                    WrappedCard jink = new WrappedCard(Jink.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    fh.UserString = Jink.ClassName;
                    jink.AddSubCard(card);
                    fh.AddSubCard(card);
                    jink = RoomLogic.ParseUseCard(room, jink);
                    jink.UserString = RoomLogic.CardToString(room, fh);
                    return jink;
                }
            }

            return null;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return -2;
        }
    }

    public class FuhanAI : SkillEvent
    {
        public FuhanAI() : base("fuhan") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return player.GetMark("meiying") > 3;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            string[] generals = choice.Split('+');
            Player lord = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetRoleEnum() == PlayerRole.Lord)
                {
                    lord = p;
                    break;
                }
            }

            General lord_general = Engine.GetGeneral(lord.General1, room.Setting.GameMode);
            //对将面国籍、技能设置身份倾向

            Dictionary<string, double> points = new Dictionary<string, double>();
            foreach (string general in generals)
            {
                double value = Engine.GetGeneralValue(general, room.Setting.GameMode);
                value += Engine.GetRoleTendency(general, player.GetRoleEnum());
                points[general] = value;
            }

            List<string> best = new List<string>(points.Keys);
            best.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });

            return best[0];
        }
    }

    public class ZishuAI : SkillEvent
    {
        public ZishuAI() : base("zishu") { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            Room room = ai.Room;
            if (ai.HasSkill(Name, player) && player.Phase == PlayerPhase.NotActive && place == Place.PlaceHand && !card.IsVirtualCard())
            {
                int id = card.GetEffectiveId();
                if (room.GetCardOwner(id) == player)
                {
                    if (player.ContainsTag(Name) && player.GetTag(Name) is List<int> ids && ids.Contains(id))
                        return isUse ? 2 : -1;
                }
                else
                {
                    return -0.8;
                }
            }

            return 0;
        }
    }

    public class YingyuanAI : SkillEvent
    {
        public YingyuanAI() : base("yingyuan")
        {
            key = new List<string> { "playerChosen:yingyuan" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                string[] strs = str.Split(':');
                Room room = ai.Room;
                if (strs[0] == "playerChosen" && strs[1] == Name)
                {
                    Player target = room.FindPlayer(strs[2]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is List<int> ids)
            {
                List<Player> friends = ai.FriendNoSelf;
                if (friends.Count > 0)
                {
                    foreach (Player p in friends)
                        if (ai.HasSkill("qingjian", p) && !p.HasFlag("qingjian")) return new List<Player> { p };

                    KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, friends, Place.PlaceHand);
                    if (pair.Key != null)
                        return new List<Player> { pair.Key };

                    ai.SortByDefense(ref friends, false);
                    return new List<Player> { friends[0] };
                }
            }

            return new List<Player>();
        }
    }

    public class ZiyuanAI : SkillEvent
    {
        public ZiyuanAI() : base("ziyuan") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(ZiyuanCard.ClassName) && !player.IsKongcheng())
            {
                return new List<WrappedCard> { new WrappedCard(ZiyuanCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }
    }

    public class ZiyuanCardAI : UseCard
    {
        public ZiyuanCardAI() : base(ZiyuanCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 3.5;
            List<int> cards = player.GetCards("he");
            List<List<int>> uses = new List<List<int>>(), available = new List<List<int>>(); ;
            for (int i = 1; i <= cards.Count; i++)
            {
                List<List<int>> players = TrustedAI.GetCombinationList(new List<int>(cards), i);
                uses.AddRange(players);
            }

            Room room = ai.Room;
            foreach (List<int> combine in uses)
            {
                int number = 0;
                foreach (int id in combine)
                {
                    number += room.GetCard(id).Number;
                }

                if (number == 13)
                    available.Add(combine);
            }

            if (available.Count > 0)
            {
                int over = ai.GetOverflow(player);
                available.Sort((x, y) => { return x.Count > y.Count ? -1 : 1; });
                List<Player> friends = ai.FriendNoSelf;

                if (friends.Count > 0)
                {
                    ai.SortByDefense(ref friends, false);
                    foreach (Player p in friends)
                    {
                        if (p.IsWounded() && !ai.WillSkipPlayPhase(p))
                        {
                            card.AddSubCards(available[0]);
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }

                    foreach (Player p in friends)
                    {
                        if (p.IsWounded())
                        {
                            card.AddSubCards(available[0]);
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }

                    if (over > 0)
                    {
                        ai.Number[Name] = 1;
                        room.SortByActionOrder(ref friends);
                        foreach (Player p in friends)
                        {
                            if (!ai.WillSkipPlayPhase(p))
                            {
                                card.AddSubCards(available[0]);
                                use.Card = card;
                                use.To = new List<Player> { p };
                                return;
                            }
                        }

                        card.AddSubCards(available[0]);
                        use.Card = card;
                        use.To = new List<Player> { friends[0] };
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class FumanAI : SkillEvent
    {
        public FumanAI() : base("fuman") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng())
            {
                Room room = ai.Room;
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                    if (room.GetCard(id).Name.Contains(Slash.ClassName))
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    ai.SortByKeepValue(ref ids, false);
                    WrappedCard fm = new WrappedCard(FumanCard.ClassName) { Skill = Name };
                    fm.AddSubCard(ids[0]);
                    return new List<WrappedCard> { fm };
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class FumanCardAI : UseCard
    {
        public FumanCardAI() : base(FumanCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown" && !(target.IsKongcheng() && ai.HasSkill("kongcheng_jx", target)))
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> targets = new List<Player>();
            Room room = ai.Room;
            foreach (Player p in ai.FriendNoSelf)
            {
                if (ai.HasSkill("zishu", p) || p.HasFlag("fuman_" + player.Name)) continue;
                if (p.IsKongcheng() && ai.HasSkill("kongcheng_jx", p) && !ai.MaySave(p)) continue;
                targets.Add(p);
            }

            if (targets.Count > 0)
            {
                foreach (Player p in targets)
                {
                    if (ai.HasSkill("qingjian", p) || ai.HasSkill(TrustedAI.CardneedSkill, p))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }

                room.SortByActionOrder(ref targets);
                foreach (Player p in targets)
                {
                    if (!ai.WillSkipPlayPhase(p))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }

                if (ai.GetOverflow(player) > 0)
                {
                    use.Card = card;
                    use.To = new List<Player> { targets[0] };
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 2;
        }
    }

    public class QianyaCardAI : UseCard
    {
        public QianyaCardAI() : base(QianyaCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown" && !(target.IsKongcheng() && ai.HasSkill("kongcheng_jx", target)))
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }
    }
    public class ShuimengAI : SkillEvent
    {
        public ShuimengAI() : base("shuimeng") { }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            if (ai.Self == requestor)
            {
                return ai.GetMaxCard();
            }
            else
            {
                if (ai.IsFriend(requestor))
                    return ai.GetMinCard();
                else
                    return ai.GetMaxCard();
            }
        }
    }

    public class ZhuhaiAI : SkillEvent
    {
        public ZhuhaiAI() : base("zhuhai") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            string[] strs = prompt.Split(':');

            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HasFlag("SlashAssignee")) targets.Add(p);

            List<ScoreStruct> values = ai.CaculateSlashIncome(player, null, targets);
            if (values.Count > 0 && values[0].Score > 0)
            {
                use.Card = values[0].Card;
                use.To = values[0].Players;
            }

            return use;
        }
    }

    public class JianyanAI : SkillEvent
    {
        public JianyanAI() : base("jianyan") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            if (player.GetTag(Name) is int id)
            {
                Room room = ai.Room;
                room.SortByActionOrder(ref targets);
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (card.Name == Jink.ClassName || card.Name == Peach.ClassName || card.Name == Analeptic.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && ai.IsWeak(p) && !ai.HasSkill("zishu", p))
                            return new List<Player> { p };
                }
                if (fcard is EquipCard)
                {
                    if (card.Name == Vine.ClassName)
                    {
                        foreach (Player p in targets)
                            if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("shixin", p))
                                return new List<Player> { p };
                    }
                    else if (card.Name == EightDiagram.ClassName)
                    {
                        foreach (Player p in targets)
                            if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("tiandu", p))
                                return new List<Player> { p };
                    }
                    else if (card.Name == Spear.ClassName)
                    {
                        foreach (Player p in targets)
                            if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("tushe", p))
                                return new List<Player> { p };
                    }
                    else if (card.Name == SilverLion.ClassName)
                    {
                        foreach (Player p in targets)
                            if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("dingpan", p))
                                return new List<Player> { p };
                    }

                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && ai.HasSkill(TrustedAI.LoseEquipSkill, p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                            return new List<Player> { p };
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && ai.HasSkill(TrustedAI.NeedEquipSkill, p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                            return new List<Player> { p };

                    if (ai.FindSameEquipCards(card, false, false).Count > 0)
                    {
                        foreach (Player p in targets)
                            if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p) && ai.GetSameEquip(card, p) == null)
                                return new List<Player> { p };
                    }
                }
                if (fcard is TrickCard)
                {
                    bool use = fcard.IsAvailable(room, player, card);
                    if (use)
                    {
                        CardUseStruct _use = new CardUseStruct(null, player, new List<Player>());
                        UseCard e = Engine.GetCardUsage(card.Name);
                        e.Use(ai, player, ref _use, card);
                        if (_use.Card == null)
                            use = false;
                    }
                    if (!use)
                    {
                        foreach (Player p in targets)
                            if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                                return new List<Player> { p };
                    }
                }
                if (fcard is Slash)
                {
                    bool use = ai.GetCards(Slash.ClassName, player).Count == 0 && fcard.IsAvailable(room, player, card);
                    if (use)
                    {
                        CardUseStruct _use = new CardUseStruct(null, player, new List<Player>());
                        UseCard e = Engine.GetCardUsage(card.Name);
                        e.Use(ai, player, ref _use, card);
                        if (_use.Card == null)
                            use = false;
                    }
                    if (!use && ai.GetOverflow(player) > 0)
                    {
                        foreach (Player p in targets)
                            if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                                return new List<Player> { p };
                    }
                }
                if (fcard is BasicCard && ai.GetCards(card.Name, player).Count > 0)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill(TrustedAI.CardneedSkill, p))
                            return new List<Player> { p };
                }
            }

            if (targets.Contains(player))
                return new List<Player> { player };

            return new List<Player> { targets[0] };
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            List<Player> friends = ai.GetFriends(player);
            ai.SortByDefense(ref friends, false);
            foreach (Player p in friends)
                if (p.IsMale() && ai.IsWeak(p) && !ai.HasSkill("zishu", p))
                    return "red";

            foreach (Player p in friends)
                if (p.IsMale() && ai.HasSkill(TrustedAI.LoseEquipSkill, p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                    return "equip";

            foreach (Player p in friends)
                if (p.IsMale() && ai.HasSkill(TrustedAI.NeedEquipSkill, p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                    return "equip";

            if (player.GetEquips().Count < 2) return "equip";

            return "trick";
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(JianyanCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(JianyanCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class JianyanCardAI : UseCard
    {
        public JianyanCardAI() : base(JianyanCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            foreach (Player p in ai.GetFriends(player))
            {
                if (p.IsMale())
                {
                    use.Card = card;
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
    }

    public class BaobianAI : SkillEvent
    {
        public BaobianAI() : base("baobian")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(TiaoxinJXCard.ClassName) && player.Hp <= 3)
                return new List<WrappedCard> { new WrappedCard(TiaoxinJXCard.ClassName) { Skill = Name, ShowSkill = Name } };

            return null;
        }
    }

    public class BingzhengAI : SkillEvent
    {
        public BingzhengAI() : base("bingzheng")
        {
            key = new List<string> { "playerChosen:bingzheng", "skillChoice:bingzheng" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                if (choice.StartsWith("playerChosen:bingzheng"))
                {
                    Player target = room.FindPlayer(choices[2]);
                    if (ai.GetPlayerTendency(target) != "unknown" && target.HandcardNum == 0)
                        ai.UpdatePlayerRelation(player, target, true);
                }
                else
                {
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }
                    bool friendly = choices[2] == "draw";
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, friendly);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && p.HandcardNum + 1 == player.Hp) return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && p.HandcardNum + 1 == player.Hp) return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p) && p.HandcardNum - 1 == player.Hp && !ai.HasSkill("tuntian", p)) return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p) && p.HandcardNum - 1 == player.Hp) return new List<Player> { p };
            }

            Room room = ai.Room;
            room.SortByActionOrder(ref targets);
            foreach (Player p in targets)
                if (ai.IsFriend(p) && !ai.HasSkill("zishu", p)) return new List<Player> { p };
            foreach (Player p in targets)
                if (!ai.IsFriend(p) && !ai.HasSkill("tuntian", p)) return new List<Player> { p };

            return new List<Player>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player target)
                return ai.IsFriend(target) ? "draw" : "discard";

            return "discard";
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag(Name))
                {
                    target = p;
                    break;
                }
            }
            List<int> ids = player.GetCards("he"), hands = player.GetCards("h");
            if (ids.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 0 && !ai.IsEnemy(target))
                    return new List<int> { ids[0] };

                if (ai.IsFriend(target) && !ai.HasSkill("zishu", target) && (ai.GetOverflow(player) > 0 || ai.MaySave(target)))
                {
                    if (ai.MaySave(target) && !ai.WillSkipPlayPhase(target))
                    {
                        if (ai.GetOverflow(player) > 0)
                        {
                            ai.SortByUseValue(ref hands);
                            return new List<int> { hands[0] };
                        }
                        else
                        {
                            ai.SortByUseValue(ref ids);
                            return new List<int> { ids[0] };
                        }
                    }

                    if (ai.GetOverflow(player) > 0)
                    {
                        ai.SortByKeepValue(ref hands);
                        return new List<int> { hands[0] };
                    }
                }
            }

            return new List<int>();
        }
    }

    public class SheyanAI : SkillEvent
    {
        public SheyanAI() : base("sheyan")
        {
            key = new List<string> { "playerChosen:sheyan" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && room.GetTag(Name) is CardUseStruct use)
            {
                string[] choices = choice.Split(':');
                Player target = room.FindPlayer(choices[2]);
                if (target != player && ai.GetPlayerTendency(target) != "unknown")
                {
                    switch (use.Card.Name)
                    {
                        case "IronChain":
                            {
                                if ((use.To.Contains(target) && !target.Chained) || (!use.To.Contains(target) && !target.Chained))
                                    ai.UpdatePlayerRelation(player, target, true);
                            }
                            break;
                        case "SavageAssault":
                        case "ArcheryAttack":
                            {
                                if (use.Card.Name == ArcheryAttack.ClassName && ai.HasSkill("leiji|leiji_jx", target)) return;
                                ai.UpdatePlayerRelation(player, target, true);
                            }
                            break;
                        case "GodSalvation":
                            if (target.IsWounded()) ai.UpdatePlayerRelation(player, target, false);
                            break;
                        case "AmazingGrace":
                            ai.UpdatePlayerRelation(player, target, false);
                            break;
                        case "ExNihilo":
                            ai.UpdatePlayerRelation(player, target, true);
                            break;
                    }
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use)
            {
                switch (use.Card.Name)
                {
                    case "IronChain":
                        {
                            foreach (Player p in targets)
                            {
                                if (ai.IsFriend(p) && ((!p.Chained && use.To.Contains(p)) || (p.Chained && !use.To.Contains(p) && !ai.HasSkill("jieying", p))))
                                    return new List<Player> { p };
                            }
                            foreach (Player p in targets)
                            {
                                if (ai.IsEnemy(p) && !p.Chained && !use.To.Contains(p)) return new List<Player> { p };
                            }
                        }
                        break;
                    case "Dismantlement":
                        {
                            if (ai.IsFriend(use.From))
                            {
                                List<ScoreStruct> scores = new List<ScoreStruct>();
                                foreach (Player p in targets)
                                {
                                    if (!use.To.Contains(p))
                                    {
                                        ScoreStruct score = ai.FindCards2Discard(use.From, p, Dismantlement.ClassName, "hej", HandlingMethod.MethodDiscard);
                                        scores.Add(score);
                                    }
                                }
                                if (scores.Count > 0)
                                {
                                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                                    if (scores[0].Score > 0)
                                        return scores[0].Players;
                                }
                            }
                            else
                            {
                                foreach (Player p in targets)
                                {
                                    if (ai.IsFriend(p) && use.To.Contains(p)) return new List<Player> { p };
                                }
                                foreach (Player p in targets)
                                {
                                    if (ai.IsEnemy(p) && !use.To.Contains(p))
                                    {
                                        ScoreStruct score = ai.FindCards2Discard(use.From, p, Dismantlement.ClassName, "hej", HandlingMethod.MethodDiscard);
                                        if (score.Score >= 0)
                                            return new List<Player> { p };
                                    }
                                }
                            }
                        }
                        break;
                    case "Snatch":
                        {
                            if (ai.IsFriend(use.From))
                            {
                                List<ScoreStruct> scores = new List<ScoreStruct>();
                                foreach (Player p in targets)
                                {
                                    if (!use.To.Contains(p))
                                    {
                                        ScoreStruct score = ai.FindCards2Discard(use.From, p, Dismantlement.ClassName, "hej", HandlingMethod.MethodGet);
                                        scores.Add(score);
                                    }
                                }
                                if (scores.Count > 0)
                                {
                                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                                    if (scores[0].Score >= 0)
                                        return scores[0].Players;
                                }
                            }
                            else
                            {
                                foreach (Player p in targets)
                                {
                                    if (ai.IsFriend(p) && use.To.Contains(p)) return new List<Player> { p };
                                }
                                if (ai.HasSkill("zishu", use.From) && use.From.Phase != PlayerPhase.NotActive) return new List<Player>();
                                foreach (Player p in targets)
                                {
                                    if (ai.IsEnemy(p) && !use.To.Contains(p))
                                    {
                                        ScoreStruct score = ai.FindCards2Discard(use.From, p, Dismantlement.ClassName, "hej", HandlingMethod.MethodGet);
                                        if (score.Score <= 0)
                                            return new List<Player> { p };
                                    }
                                }
                            }
                        }
                        break;
                    case "FireAttack":
                        {
                            if (ai.IsFriend(use.From))
                            {
                                ai.SortByDefense(ref targets, false);
                                foreach (Player p in targets)
                                {
                                    if (ai.IsEnemy(p) && !use.To.Contains(p))
                                        return new List<Player> { p };
                                }
                            }
                        }
                        break;
                    case "SavageAssault":
                    case "ArcheryAttack":
                        {
                            if (use.Card.Name == ArcheryAttack.ClassName)
                                foreach (Player p in targets)
                                    if (!ai.IsFriend(p) && ai.NotSlashJiaozhu(use.From, p, use.Card)) return new List<Player> { p };

                            ai.SortByDefense(ref targets, false);
                            List<ScoreStruct> scores = new List<ScoreStruct>();
                            foreach (Player p in targets)
                            {
                                if (ai.IsFriend(p) && use.Card.Name == ArcheryAttack.ClassName && ai.JiaozhuneedSlash(use.From, p, use.Card)) continue;
                                if (ai.IsCardEffect(use.Card, p, use.From) && !ai.IsCancelTarget(use.Card, p, use.From))
                                {
                                    DamageStruct damage = new DamageStruct(use.Card, use.From, p, 1 + use.ExDamage);
                                    ScoreStruct score = ai.GetDamageScore(damage);
                                    score.Players = new List<Player> { p };
                                    scores.Add(score);
                                }
                            }

                            if (scores.Count > 0)
                            {
                                scores.Sort((x, y) => { return x.Score < y.Score ? -1 : 1; });
                                if (scores[0].Score < 0)
                                    return scores[0].Players;
                            }
                        }
                        break;
                    case "GodSalvation":
                        {
                            ai.SortByDefense(ref targets, false);
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && p.IsWounded() && ai.HasSkill("qingxian|liexian|hexian", p))
                                    return new List<Player> { p };

                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && p.IsWounded())
                                    return new List<Player> { p };
                        }
                        break;
                    case "AmazingGrace":
                        {
                            room.SortByActionOrder(ref targets);
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && (ai.HasSkill("zishu", p) && p.Phase != PlayerPhase.NotActive || ai.HasSkill("qianya|qingjian")))
                                    return new List<Player> { p };
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && !ai.HasSkill("zishu", p)) return new List<Player> { p };
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p)) return new List<Player> { p };
                        }
                        break;
                    case "ExNihilo":
                        {
                            foreach (Player p in targets)
                            {
                                if (use.To.Contains(p)) continue;
                                if (ai.IsFriend(p) && (!ai.HasSkill("zishu", p) || p.Phase != PlayerPhase.NotActive)) return new List<Player> { p };
                            }
                        }
                        break;
                    case "Duel":
                        {
                            if (!ai.IsFriend(use.From) && targets.Contains(player)) return new List<Player> { player };

                            List<ScoreStruct> scores = new List<ScoreStruct>();
                            foreach (Player p in targets)
                            {
                                if (use.To.Contains(p)) continue;
                                if (ai.IsCardEffect(use.Card, p, use.From) && !ai.IsCancelTarget(use.Card, p, use.From))
                                {
                                    if (ai.HasSkill("hunzi+jiang", p) && p.GetMark("hunzi") == 0 && p.Hp - 1 - use.ExDamage == 1)
                                    {
                                        ScoreStruct score = new ScoreStruct
                                        {
                                            Players = new List<Player> { p },
                                            Score = ai.IsFriend(p) ? 5 : -5
                                        };
                                        scores.Add(score);
                                    }
                                    else
                                    {
                                        DamageStruct damage = new DamageStruct(use.Card, use.From, p, 1 + use.ExDamage);
                                        ScoreStruct score = ai.GetDamageScore(damage);
                                        score.Players = new List<Player> { p };
                                        scores.Add(score);
                                    }
                                }
                            }
                            if (scores.Count > 0)
                            {
                                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                                if (scores[0].Score > 0)
                                    return scores[0].Players;
                            }
                        }
                        break;
                }
            }

            return new List<Player>();
        }
    }

    public class DianhuAI : SkillEvent
    {
        public DianhuAI() : base("dianhu") { }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (player.GetRoleEnum() == PlayerRole.Rebel || player.GetRoleEnum() == PlayerRole.Renegade)
            {
                foreach (Player _p in room.GetOtherPlayers(player))
                    if (_p.GetRoleEnum() == PlayerRole.Lord && !ai.HasSkill("lixun", _p)) return new List<Player> { _p };
            }

            Player p = RoomLogic.FindPlayerBySkillName(room, "shibei");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "huituo");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "rende");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "jieyin_jx");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "kuanggu_jx");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "buqu_jx");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "buqu");
            if (p != null) return new List<Player> { p };

            foreach (Player _p in targets)
                if (_p.GetRoleEnum() != PlayerRole.Lord && !ai.HasSkill("lixun", _p))
                    return new List<Player> { _p };

            return new List<Player>();
        }
    }

    public class JianjiAI : SkillEvent
    {
        public JianjiAI() : base("jianji") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(JianjiCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(JianjiCard.ClassName) { Skill = Name } };
            return null;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            int id = int.Parse(pattern);
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());

            WrappedCard card = room.GetCard(id);
            if (card.Name.Contains(Slash.ClassName))
            {
                List<WrappedCard> slashes = new List<WrappedCard> { card };
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
                if (scores.Count > 0 && scores[0].Score > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                    return use;
                }
            }
            else
            {
                UseCard e = Engine.GetCardUsage(card.Name);
                e.Use(ai, player, ref use, card);
                if (use.Card == card)
                    return use;
                else
                    use.Card = null;
            }

            return use;
        }
    }

    public class JianjiCardAI : UseCard
    {
        public JianjiCardAI() : base(JianjiCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, true);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                Room room = ai.Room;
                room.SortByActionOrder(ref friends);
                foreach (Player p in friends)
                {
                    if (!ai.HasSkill("zishu", p))
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                use.Card = card;
                use.To.Add(friends[0]);
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
    }

    public class YuxuAI : SkillEvent
    {
        public YuxuAI() : base("yuxu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class ShijianAI : SkillEvent
    {
        public ShijianAI() : base("shijian")
        {
            key = new List<string> { "cardDiscard:shijian" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                if (choices[1] == Name)
                {
                    Player target = room.Current;
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            Room room = ai.Room;
            Player current = room.Current;
            if (ai.IsFriend(current) && (ai.HasSkill("zhanji|zishu|chenglue|tushe", current) || (ai.HasSkill(TrustedAI.LoseEquipSkill, current) && current.HasEquip())))
            {
                List<int> discards = new List<int>();
                foreach (int id in ids)
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        discards.Add(id);

                if (discards.Count > 0)
                {
                    ai.SortByKeepValue(ref discards, false);
                    return new List<int> { discards[0] };
                }
            }
            return new List<int>();
        }
    }

    public class XianfuAI : SkillEvent
    {
        public XianfuAI() : base("xianfu") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            Player p = RoomLogic.FindPlayerBySkillName(room, "shibei");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "huituo");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "rende");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "jieyin_jx");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "kuanggu_jx");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "buqu_jx");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "buqu");
            if (p != null) return new List<Player> { p };

            foreach (Player _p in targets)
                if (_p.GetRoleEnum() == PlayerRole.Lord && !ai.HasSkill("lixun", _p))
                    return new List<Player> { _p };

            foreach (Player _p in targets)
                if (!ai.HasSkill("lixun", _p))
                    return new List<Player> { _p };

            return new List<Player>();
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (damage.To.ContainsTag(Name) && damage.To.GetTag(Name) is List<string> xizhicai)
            {
                Room room = ai.Room;
                List<Player> targets = new List<Player>();
                foreach (string name in xizhicai)
                {
                    Player p = room.FindPlayer(name);
                    if (p != null) targets.Add(p);
                }

                if (damage.To.GetMark("@fu") > 0 || targets.Contains(ai.Self))
                {
                    foreach (Player p in targets)
                    {
                        DamageStruct _damage = new DamageStruct(Name, null, p, damage.Damage);
                        score.Score += ai.GetDamageScore(_damage).Score;
                    }
                }
            }

            return score;
        }
    }

    public class ChouceAI : SkillEvent
    {
        public ChouceAI() : base("chouce")
        {
            key = new List<string> { "playerChosen:chouce", "cardChosen:chouce" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                string[] strs = str.Split(':');
                Room room = ai.Room;
                if (strs[0] == "playerChosen" && strs[1] == Name && !player.HasFlag(Name))
                {
                    Player target = room.FindPlayer(strs[2]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
                else if (strs[0] == "cardChosen" && strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceJudge)
                    {
                        if (room.GetCard(card_id).Name != Lightning.ClassName)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            Player winner = ai.GetWizzardRaceWinner(room.GetCard(card_id).Name, target, target);
                            if (winner != null && ai.IsFriend(winner, target))
                                ai.UpdatePlayerRelation(player, target, false);
                            else
                                ai.UpdatePlayerRelation(player, target, true);
                        }
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceHand && !(ai.HasSkill("qianxun_jx", target) && target.HandcardNum > 1))
                    {
                        ai.UpdatePlayerRelation(player, target, ai.HasSkill("kongcheng|kongcheng_jx") && target.HandcardNum == 1 ? true : false);
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                    {
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Player.Place.PlaceEquip) > 0 ? false : true);
                    }
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (player.HasFlag(Name))
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in targets)
                    if (RoomLogic.CanDiscard(room, player, p, "hej"))
                        scores.Add(ai.FindCards2Discard(player, p, Name, "hej", HandlingMethod.MethodDiscard));

                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0)
                        return scores[0].Players;
                }
            }
            else
            {
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p) && p.ContainsTag("xianfu") && p.GetTag("xianfu") is List<string> names && names.Contains(player.Name))
                        return new List<Player> { p };
                }

                ai.SortByDefense(ref targets, false);
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p)) return new List<Player> { p };
                }
            }

            return new List<Player>();
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            Room room = ai.Room;
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            Player xizicai = RoomLogic.FindPlayerBySkillName(room, Name);
            if (xizicai != null && damage.Damage < damage.To.Hp && damage.Damage < xizicai.Hp
                && damage.To.ContainsTag("xianfu") && damage.To.GetTag("xianfu") is List<string> names && names.Contains(xizicai.Name)
                && (ai.Self == xizicai || damage.To.GetMark("@fu") > 0))
            {
                if (ai.IsFriend(xizicai))
                    score.Score += damage.Damage * 2;
                else if (ai.IsEnemy(xizicai))
                    score.Score -= damage.Damage * 2;
            }

            return score;
        }
    }

    public class ChenqingAI : SkillEvent
    {
        public ChenqingAI() : base("chenqing")
        {
            key = new List<string> { "playerChosen:chenqing" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.SortByHandcards(ref targets);
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p)) return new List<Player> { p };
            }

            return new List<Player>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HasFlag("Global_Dying"))
                {
                    target = p;
                    break;
                }
            }

            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);
            ai.SortByKeepValue(ref ids, false);

            List<int> worst = new List<int>(), club = new List<int>(), spade = new List<int>(), heart = new List<int>(), diamond = new List<int>();
            for (int i = 0; i < Math.Min(4, ids.Count); i++)
                worst.Add(ids[i]);

            if (ai.IsFriend(target))
            {

                List<WrappedCard> peaches = ai.GetCards(Peach.ClassName, player);
                if (peaches.Count > 0)
                {
                    foreach (WrappedCard peach in peaches)
                    {
                        bool check = true;
                        foreach (int id in peach.SubCards)
                        {
                            if (worst.Contains(id))
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check) return worst;
                    }
                }

                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    switch (card.Suit)
                    {
                        case WrappedCard.CardSuit.Club:
                            club.Add(id);
                            break;
                        case WrappedCard.CardSuit.Spade:
                            spade.Add(id);
                            break;
                        case WrappedCard.CardSuit.Diamond:
                            diamond.Add(id);
                            break;
                        case WrappedCard.CardSuit.Heart:
                            heart.Add(id);
                            break;
                    }
                }

                if (club.Count > 0 && spade.Count > 0 && diamond.Count > 0 && heart.Count > 0)
                {
                    List<int> result = new List<int> { club[0], spade[0], diamond[0], heart[0] };
                    return result;
                }
            }

            return worst;
        }
    }

    public class MoshiAI : SkillEvent
    {
        public MoshiAI() : base("moshi") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            if (player.GetTag(Name) is List<string> cards)
            {
                string card_name = cards[0];
                WrappedCard card = new WrappedCard(card_name) { Skill = Name };
                if (card_name.Contains(Slash.ClassName))
                {
                    List<WrappedCard> slashes = new List<WrappedCard>();
                    foreach (int id in player.GetCards("h"))
                    {
                        if (!RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodUse, true))
                        {
                            card.AddSubCard(id);
                            WrappedCard slash = RoomLogic.ParseUseCard(room, card);
                            slashes.Add(slash);
                        }
                    }

                    if (slashes.Count > 0)
                    {
                        List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
                        if (scores.Count > 0 && scores[0].Score > 3)
                        {
                            use.Card = scores[0].Card;
                            use.To = scores[0].Players;
                            return use;
                        }
                    }
                }
                else if (card_name != IronChain.ClassName && card_name != Collateral.ClassName)
                {
                    UseCard e = Engine.GetCardUsage(card_name);
                    if (e != null)
                    {
                        foreach (int id in player.GetCards("h"))
                        {
                            if (!RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodUse, true))
                            {
                                use = new CardUseStruct(null, player, new List<Player>());
                                card.AddSubCard(id);
                                card = RoomLogic.ParseUseCard(room, card);
                                e.Use(ai, player, ref use, card);
                                if (use.Card == card)
                                    return use;
                            }
                        }
                    }
                }
            }

            return use;
        }
    }

    public class ShanjiaAI : SkillEvent
    {
        public ShanjiaAI() : base("shanjia") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            if (ai.HasSkill(Name, player) && player.GetMark(Name) < 3 && card != null && !isUse
                && Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeEquip && place == Place.PlaceEquip)
                return -2;

            return 0;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> result = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetEquips())
            {
                if (RoomLogic.CanDiscard(room, player, player, id) && ai.GetKeepValue(id, player) < 0)
                    result.Add(id);
                if (result.Count >= min)
                    break;
            }

            if (result.Count < min)
            {
                foreach (int id in player.GetEquips())
                {
                    if (!result.Contains(id) && RoomLogic.CanDiscard(room, player, player, id)
                        && ai.FindSameEquipCards(room.GetCard(id), false, false).Count > 0)
                        result.Add(id);

                    if (result.Count >= min)
                        break;
                }
            }

            List<int> ids = player.GetCards("h"), reserved = new List<int>();
            if (result.Count < min)
            {
                ai.SortByUseValue(ref ids);

                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (Engine.GetFunctionCard(card.Name) is EquipCard) reserved.Add(id);
                    if (reserved.Count >= min - result.Count) break;
                }

                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    if (!result.Contains(id) && RoomLogic.CanDiscard(room, player, player, id) && !reserved.Contains(id))
                        result.Add(id);

                    if (result.Count >= min)
                        break;
                }
            }

            if (result.Count < min)
            {
                foreach (int id in ids)
                {
                    if (!result.Contains(id) && RoomLogic.CanDiscard(room, player, player, id))
                        result.Add(id);

                    if (result.Count >= min)
                        break;
                }
            }

            return result;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            WrappedCard card = new WrappedCard(Slash.ClassName) { Skill = "_shanjia" };
            List<WrappedCard> slashes = new List<WrappedCard> { card };
            List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
            if (scores.Count > 0 && scores[0].Score > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
            {
                use.Card = scores[0].Card;
                use.To = scores[0].Players;
            }

            return use;
        }
    }

    public class GusheAI : SkillEvent
    {
        public GusheAI() : base("gushe")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && player.UsedTimes(GusheCard.ClassName) < 1 + player.GetMark("jici"))
                return new List<WrappedCard> { new WrappedCard(GusheCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            List<double> values = ai.SortByKeepValue(ref ids, false);
            if (values[0] < 1)
                return new List<int> { ids[0] };

            return new List<int>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            foreach (int id in player.GetEquips())
                if (RoomLogic.CanDiscard(room, player, player, id) && ai.GetKeepValue(id, player) < 0 && ai.FindSameEquipCards(room.GetCard(id), true, false).Count == 0)
                    return new List<int> { id };

            return new List<int>();
        }
        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            Player player = ai.Self;
            Room room = ai.Room;
            if (player == requestor)
            {
                Dictionary<int, double> points = new Dictionary<int, double>();
                List<int> ids = new List<int>();
                if (player.GetMark("@she") >= 4)
                {
                    List<int> available = new List<int>();
                    foreach (int id in player.GetCards("h"))
                    {
                        int number = room.GetCard(id).Number;
                        if (number >= 12 || (number < player.GetMark("@she") && number + player.GetMark("@she") >= 12))
                            available.Add(id);
                    }

                    if (available.Count > 0)
                    {
                        foreach (int id in available)
                        {
                            WrappedCard card = room.GetCard(id);
                            double value = ai.GetUseValue(id, player);
                            points[id] = value;
                        }

                        ids = new List<int>(points.Keys);
                        ids.Sort((x, y) => { return points[x] < points[y] ? -1 : 1; });
                        return room.GetCard(ids[0]);
                    }
                }

                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    double value = ai.GetUseValue(id, player);
                    value += (13 - card.Number) / 13;
                    if (card.Number == player.GetMark("@she"))
                        value -= 1;

                    points[id] = value;
                }

                ids = new List<int>(points.Keys);
                ids.Sort((x, y) => { return points[x] < points[y] ? -1 : 1; });
                return room.GetCard(ids[0]);
            }
            else if (!ai.IsFriend(requestor))
            {
                Dictionary<int, double> points = new Dictionary<int, double>();
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    double value = ai.GetUseValue(id, player);
                    value -= card.Number / 12;

                    points[id] = value;
                }

                List<int> ids = new List<int>(points.Keys);
                ids.Sort((x, y) => { return points[x] < points[y] ? -1 : 1; });
                return room.GetCard(ids[0]);
            }
            else
            {
                List<int> ids = player.GetCards("h");
                ai.SortByKeepValue(ref ids, false);
                return room.GetCard(ids[0]);
            }
        }
    }

    public class GusheCardAI : UseCard
    {
        public GusheCardAI() : base(GusheCard.ClassName)
        { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, false);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<int> ids = player.GetCards("h"); Room room = ai.Room;
            bool big = false;
            if (player.GetMark("@she") >= 4)
            {
                foreach (int id in ids)
                {
                    int number = room.GetCard(id).Number;
                    if (number >= 12 || (number < player.GetMark("@she") && number + player.GetMark("@she") >= 12))
                    {
                        big = true;
                        break;
                    }
                }
                if (!big) return;
            }

            List<Player> enemies = ai.GetEnemies(player), targets = new List<Player>();
            ai.SortByDefense(ref enemies, false);
            foreach (Player p in enemies)
            {
                if (RoomLogic.CanBePindianBy(room, p, player))
                    targets.Add(p);

                if (targets.Count >= 3)
                    break;
            }
            if (targets.Count > 0)
            {
                use.Card = card;
                use.To = targets;
            }
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 3.2;
        }
    }

    public class JiciAI : SkillEvent
    {
        public JiciAI() : base("jici") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class QizhiAI : SkillEvent
    {
        public QizhiAI() : base("qizhi")
        { key = new List<string> { "cardChosen:qizhi" }; }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
            if (ai.Self == player && !(ai is StupidAI)) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                    {
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Place.PlaceEquip) > 0 ? false : true);
                    }
                    else if (ai.HasSkill("tuntian", target) && target.Phase == PlayerPhase.NotActive)
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && !ai.HasSkill("zishu", p))
                {
                    foreach (int id in p.GetEquips())
                    {
                        if (RoomLogic.CanDiscard(room, player, p, id) && ai.GetKeepValue(id, p) < 0)
                            return new List<Player> { p };
                    }
                }
            }

            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && ai.HasSkill("tuntian", p) && !p.IsKongcheng() && p.Phase == PlayerPhase.NotActive)
                {
                    return new List<Player> { p };
                }
            }

            if (targets.Contains(player))
            {
                List<int> ids = player.GetCards("he");
                if (ids.Count > 0)
                {
                    List<double> values = ai.SortByUseValue(ref ids, false);
                    if (values[0] < 2.5)
                        return new List<Player> { player };
                }
            }

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p) && !ai.HasSkill("tuntian", p))
                {
                    ScoreStruct score = ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodDiscard);
                    score.Players = new List<Player> { p };
                    if (ai.HasSkill("zishu")) score.Score += 1.5;
                    scores.Add(score);
                }
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    return scores[0].Players;
                }
            }

            return new List<Player>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("he");
            List<double> values = ai.SortByKeepValue(ref ids, false);
            if (values[0] < 0) return new List<int> { ids[0] };
            ai.SortByUseValue(ref ids, false);
            return new List<int> { ids[0] };
        }
    }

    public class JinquAI : SkillEvent
    {
        public JinquAI() : base("jinqu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.GetMark("@rob") > 0)
            {
                Player gn = RoomLogic.FindPlayerBySkillName(ai.Room, "jieying_gn");
                if (gn != null && !ai.IsFriend(gn))
                {
                    if (player.GetMark("qizhi") < player.HandcardNum) return true;
                    else return false;
                }
            }

            return player.HandcardNum <= player.GetMark("qizhi");
        }
    }

    public class KangkaiAI : SkillEvent
    {
        public KangkaiAI() : base("kangkai")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                if (ai.IsFriend(target))
                    return true;
            }

            return false;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use)
            {
                if (pattern == "..")
                {
                    foreach (int id in player.GetEquips())
                        if (ai.GetKeepValue(id, player) < 0)
                            return new List<int> { id };

                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }

                    List<int> armor = new List<int>(), ohorse = new List<int>();
                    foreach (int id in player.GetCards("he"))
                    {
                        WrappedCard card = room.GetCard(id);
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        if (fcard is Armor && fcard.IsAvailable(room, target, card))
                            armor.Add(id);


                        if (fcard is DefensiveHorse && fcard.IsAvailable(room, target, card))
                            ohorse.Add(id);
                    }

                    if (!target.GetArmor() || (use.Card.Name == FireSlash.ClassName && target.HasArmor(Vine.ClassName)) && ai.IsCardEffect(use.Card, target, use.From)
                         && ai.IsFriend(target))
                    {
                        foreach (int id in armor)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name == RenwangShield.ClassName && WrappedCard.IsBlack(card.Suit))
                                return new List<int> { id };
                        }

                        foreach (int id in armor)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name == Vine.ClassName && use.Card.Name == Slash.ClassName && !target.Chained)
                                return new List<int> { id };
                        }

                        foreach (int id in armor)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name != Vine.ClassName)
                                return new List<int> { id };
                        }
                    }

                    if (ai.IsCardEffect(use.Card, target, use.From) && ai.IsWeak(target) && ai.IsFriend(target) && ai.IsLackCard(target, Jink.ClassName))
                    {
                        foreach (int id in player.GetCards("he"))
                        {
                            if (ai.IsCard(id, Jink.ClassName, target))
                                return new List<int> { id };
                        }
                    }

                    KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(player.GetCards("he"), new List<Player> { target });
                    if (pair.Key == target)
                        return new List<int> { pair.Value };
                }
                else
                {
                    List<int> armor = new List<int>(), ohorse = new List<int>();
                    foreach (int id in player.GetCards("h"))
                    {
                        WrappedCard card = room.GetCard(id);
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        if (fcard is Armor && fcard.IsAvailable(room, player, card))
                            armor.Add(id);


                        if (fcard is DefensiveHorse && fcard.IsAvailable(room, player, card))
                            ohorse.Add(id);
                    }

                    if (!player.GetArmor() || (use.Card.Name == FireSlash.ClassName && player.HasArmor(Vine.ClassName)) && ai.IsCardEffect(use.Card, player, use.From))
                    {
                        foreach (int id in armor)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name == RenwangShield.ClassName && WrappedCard.IsBlack(card.Suit))
                                return new List<int> { id };
                        }

                        foreach (int id in armor)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name == Vine.ClassName && use.Card.Name == Slash.ClassName && !player.Chained)
                                return new List<int> { id };
                        }

                        foreach (int id in armor)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name != Vine.ClassName)
                                return new List<int> { id };
                        }
                    }

                    if (player.GetDefensiveHorse() && ohorse.Count > 0)
                        return new List<int> { ohorse[0] };

                    return new List<int>();
                }
            }

            return new List<int>();
        }
    }

    public class XiaoguoJXAI : SkillEvent
    {
        public XiaoguoJXAI() : base("xiaoguo_jx")
        {
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            if (!ai.WillShowForAttack()) return new List<int>();

            List<int> result = new List<int>();
            Room room = ai.Room;
            Player current = room.Current;

            if (room.Round < 2 && !player.HasShownOneGeneral() && ai.GetOverflow(player) < 1 && !current.HasShownOneGeneral()) return result;

            DamageStruct damage = new DamageStruct(Name, player, current);
            ScoreStruct score = ai.GetDamageScore(damage);

            double min_value = 10000;
            foreach (int id in current.GetEquips())
            {
                double v = ai.GetKeepValue(id, current);
                if (v < min_value)
                {
                    min_value = v;
                }
            }

            bool throw_card = false;
            if (ai.IsFriend(current))
            {
                if (score.Score > 3 || min_value < 0)
                {
                    throw_card = true;
                    min_value = Math.Max(score.Score, -min_value);
                }
            }
            else if (score.Score > 4 && min_value >= 0)
            {
                throw_card = true;
                min_value = Math.Min(score.Score, min_value);
                if (ai.GetPrioEnemies().Contains(current))
                    min_value *= 1.3;
            }

            if (throw_card)
            {
                List<int> ids = player.GetCards("h");
                ai.SortByKeepValue(ref ids, false);

                foreach (int id in ids)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    if (fcard is BasicCard && RoomLogic.CanDiscard(room, player, player, id))
                    {
                        double value = ai.GetKeepValue(id, player);
                        if (ai.GetOverflow(player) > 0) value *= 0.65;
                        if (min_value - value > 3)
                            return new List<int> { id };
                    }
                }
            }

            return new List<int>();
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            Room room = ai.Room;
            if (room.GetTag(Name) is Player from)
            {
                DamageStruct damage = new DamageStruct(Name, from, player);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score >= -1)
                    return use;

                List<int> ids = player.GetEquips();
                foreach (int id in ids)
                {
                    double value = ai.GetKeepValue(id, player, Player.Place.PlaceEquip);
                    if (value <= 0)
                    {
                        use.Card = room.GetCard(id);
                        return use;
                    }
                }
                foreach (int id in player.GetCards("h"))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    if (fcard is EquipCard)
                    {
                        ids.Add(id);
                    }
                }

                if (ids.Count > 0)
                {
                    ai.SortByKeepValue(ref ids, false);
                    double value = ai.GetKeepValue(ids[0], player);
                    if (value < -score.Score)
                    {
                        use.Card = room.GetCard(ids[0]);
                        return use;
                    }
                }
            }

            return use;
        }
    }

    public class FenkunAI : SkillEvent
    {
        public FenkunAI() : base("fenkun") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return player.Hp > 1;
        }
    }

    public class WeikuiAI : SkillEvent
    {
        public WeikuiAI() : base("weikui") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(WeikuiCard.ClassName) && (player.Hp > 2 || (player.Hp == 1 && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) > 0)))
            {
                return new List<WrappedCard> { new WrappedCard(WeikuiCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }

        public override int OnPickAG(TrustedAI ai, Player player, List<int> card_ids, bool refusable)
        {
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag("gongxin_target"))
                {
                    target = p;
                    break;
                }
            }

            if (ai.IsEnemy(target))
            {
                List<int> ids = new List<int>(card_ids);
                if (ids.Count > 1)
                    ids.Sort((x, y) => { return ai.GetKeepValue(x, target) > ai.GetKeepValue(y, target) ? -1 : 1; });

                return ids[0];
            }

            return -1;
        }
    }

    public class WeikuiCardAI : UseCard
    {
        public WeikuiCardAI() : base(WeikuiCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
                ai.UpdatePlayerRelation(player, use.To[0], false);
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemes = ai.GetEnemies(player);
            if (enemes.Count > 0)
            {
                ai.SortByDefense(ref enemes, false);
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                Room room = ai.Room;
                foreach (Player p in enemes)
                {
                    if (!p.IsKongcheng() && RoomLogic.IsProhibited(room, player, p, slash) == null && ai.IsCardEffect(slash, p, player) && !ai.IsCancelTarget(slash, p, player))
                    {
                        List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { slash }, new List<Player> { p }, false);
                        if (scores.Count > 0 && scores[0].Score > 0)
                        {
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4;
        }
    }

    public class LizhanAI : SkillEvent
    {
        public LizhanAI() : base("lizhan")
        {
            key = new List<string> { "playerChosen:lizhan" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    List<Player> targets = new List<Player>();
                    foreach (string name in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(name));

                    foreach (Player p in targets)
                        if (ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> result = new List<Player>();

            bool self = true;
            if (player.GetMark("@rob") > 0)
            {
                Player gn = RoomLogic.FindPlayerBySkillName(ai.Room, "jieying_gn");
                if (gn != null && !ai.IsFriend(gn)) self = false;
            }

            foreach (Player p in targets)
            {
                if (!self && p == player) continue;
                if (ai.IsFriend(p)) result.Add(p);
            }

            return result;
        }
    }

    public class ZhenweiAI : SkillEvent
    {
        public ZhenweiAI() : base("zhenwei")
        {
            key = new List<string> { "cardDiscard:zhenwei" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                if (choices[1] == Name && room.GetTag(Name) is List<CardUseStruct> uses)
                {
                    Player target = uses[uses.Count - 1].To[0];
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            Room room = ai.Room;
            if (player.IsNude()) return new List<int>();
            if (room.GetTag(Name) is List<CardUseStruct> uses)
            {
                player.SetFlags("target_confirming");
                CardUseStruct use = uses[uses.Count - 1];
                Player target = use.To[0];
                if (ai.IsFriend(target) && ai.IsEnemy(use.From))
                {
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (values[0] < 0)
                    {
                        player.SetFlags("-target_confirming");
                        return new List<int> { ids[0] };
                    }

                    if (ai.IsCardEffect(use.Card, target, use.From))
                    {
                        if (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == Duel.ClassName || use.Card.Name == FireAttack.ClassName)
                        {
                            DamageStruct damage = new DamageStruct(use.Card, use.From, target, 1 + use.Drank + use.ExDamage);
                            if (use.Card.Name == FireSlash.ClassName || use.Card.Name == FireAttack.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Fire;
                            else if (damage.Card.Name == ThunderSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Thunder;

                            ScoreStruct score = ai.GetDamageScore(damage);
                            if (score.Score < -5)
                            {
                                player.SetFlags("-target_confirming");
                                return new List<int> { ids[0] };
                            }
                        }

                        if (ai.IsWeak(target) && values[0] < 3)
                        {
                            player.SetFlags("-target_confirming");
                            return new List<int> { ids[0] };
                        }
                    }
                }
            }
            player.SetFlags("-target_confirming");

            return new List<int>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (data is CardUseStruct use)
            {
                if (!ai.IsCardEffect(use.Card, player, use.From) || use.Card.Name == IronChain.ClassName
                    || use.Card.Name == Snatch.ClassName || use.Card.Name == Dismantlement.ClassName)
                    return "transfer";

                if (use.Card.Name == FireAttack.ClassName)
                {
                    DamageStruct damage = new DamageStruct(use.Card, use.From, player, 1, DamageStruct.DamageNature.Fire);
                    if (ai.GetDamageScore(damage).Score < -5) return "transfer";
                }
                if (use.Card.Name == Duel.ClassName)
                {
                    DamageStruct damage = new DamageStruct(use.Card, use.From, player, 1);
                    if (ai.GetDamageScore(damage).Score < -5) return "transfer";
                }
                if (use.Card.Name.Contains(Slash.ClassName) && ai.GetKnownCardsNums(Jink.ClassName, "he", player) > 0 && !ai.HasSkill("liegong_jx|tieqi_jx|jianchu|wushuang", use.From))
                {
                    if (ai.HasSkill("fuqi", use.From) && RoomLogic.DistanceTo(room, use.From, player) == 1) return "nullfy";
                    if (ai.HasSkill("chongzhen", use.From) && use.Card.Skill == "longdan_jx") return "nullfy";

                    DamageStruct damage = new DamageStruct(use.Card, use.From, player, 1 + use.Drank);
                    if (use.Card.Name == FireSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Fire;
                    else if (use.Card.Name == ThunderSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Thunder;
                    if (ai.GetDamageScore(damage).Score < -5) return "transfer";
                }
            }

            return "nullfy";
        }
    }

    public class TuifengAI : SkillEvent
    {
        public TuifengAI() : base("tuifeng")
        { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("he");
            if (ids.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 0) return new List<int> { ids[0] };
                Room room = ai.Room;
                foreach (int id in ids)
                {
                    if (ai.IsCard(id, Peach.ClassName, player) || (player.Hp == 1 && ai.IsCard(id, Analeptic.ClassName, player))) continue;
                    if (ai.MaySave(player) && ai.GetKeepValue(id, player) < 3) return new List<int> { id };
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (room.GetCardPlace(id) != Place.PlaceEquip || !(fcard is Armor)) return new List<int> { id };
                }
            }

            return new List<int>();
        }
    }

    public class JunbingAI : SkillEvent
    {
        public JunbingAI() : base("junbing")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.IsKongcheng() && ai.NeedKongcheng(player)) return false;
            if (data is string str && str.StartsWith("@junbing"))
            {
                string[] strs = str.Split(':');
                Room room = ai.Room;
                Player who = room.FindPlayer(strs[1]);
                if (!ai.IsEnemy(who) && !player.IsKongcheng())
                {
                    List<int> ids = player.GetCards("h");
                    foreach (int id in who.GetEquips())
                        if (ai.GetKeepValue(id, who, Place.PlaceEquip) < 0)
                            return false;

                    if (ai.GetKeepValue(ids[0], player) > 6) return false;
                }
            }

            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return ai.AskForDiscard(player.GetCards("he"), Name, min, min, false);
        }
    }

    public class QujiAI : SkillEvent
    {
        public QujiAI() : base("quji") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(QujiCard.ClassName) && player.IsWounded() && player.GetCards("he").Count >= player.GetLostHp())
                return new List<WrappedCard> { new WrappedCard(QujiCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class QujiCardAI : UseCard
    {
        public QujiCardAI() : base(QujiCard.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && ai.Self != player && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, true);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<int> ids = player.GetCards("he"), subs = new List<int>();
            List<double> values = ai.SortByKeepValue(ref ids, false);
            bool black = false;
            double value = 0;
            for (int i = 0; i < player.GetLostHp(); i++)
            {
                if (values[i] < 0)
                {
                    subs.Add(ids[i]);
                    value += values[i];
                    if (WrappedCard.IsBlack(room.GetCard(ids[i]).Suit))
                        black = true;
                }
                else
                    break;
            }
            ids.RemoveAll(t => subs.Contains(t));
            values = ai.SortByKeepValue(ref ids, false);
            for (int i = 0; i < ids.Count; i++)
            {
                if (black || WrappedCard.IsRed(room.GetCard(ids[i]).Suit))
                {
                    subs.Add(ids[i]);
                    value += values[i];
                }

                if (subs.Count >= player.GetLostHp()) break;
            }
            if (subs.Count < player.GetLostHp()) return;

            List<Player> friends = ai.GetFriends(player), targets = new List<Player>(); ;
            ai.SortByDefense(ref friends, false);
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.IsWounded()) targets.Add(p);
                if (targets.Count >= player.GetLostHp()) break;
            }

            double heal_value = 4 * targets.Count;
            if (ai.GetOverflow(player) > 0 || heal_value * 1.2 > value)
            {
                use.To = targets;
                use.Card = card;
                use.Card.AddSubCards(subs);
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 2;
        }
    }

    public class JianshuAI : SkillEvent
    {
        public JianshuAI() : base("jianshu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("@jian") > 0)
                return new List<WrappedCard> { new WrappedCard(JianshuCard.ClassName) };

            return new List<WrappedCard>();
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            return base.OnPindian(ai, requestor, players);
        }
    }

    public class JianshuCardAI : UseCard
    {
        public JianshuCardAI() : base(JianshuCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    ai.UpdatePlayerRelation(player, p, true);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 1)
            {
                Room room = ai.Room;
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                {
                    if (p.HandcardNum < 2 && p.Hp > 1 || p.IsKongcheng()) continue;
                    foreach (Player p2 in enemies)
                    {
                        if (p == p2 || (p2.HandcardNum < 2 && p.Hp > 1) || !RoomLogic.CanBePindianBy(room, p2, p)) continue;
                        use.Card = card;
                        use.To = new List<Player> { p, p2 };
                        return;
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 7;
        }
    }

    public class YongdiAI : SkillEvent
    {
        public YongdiAI() : base("yongdi")
        {
            key = new List<string> { "playerChosen:yongdi" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                Player p = room.FindPlayer(choices[2]);
                if (ai.GetPlayerTendency(p) != "unknown")
                    ai.UpdatePlayerRelation(player, p, true);
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && !p.IsDuanchang(true) && ai.HasSkill(TrustedAI.MasochismGood, p)) return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && !p.IsDuanchang(true) && (ai.HasSkill("miji|shangshi|yinghun_sunjian", p) || p.General1 == "sunce"))
                    return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && !p.IsDuanchang(true) && (ai.HasSkill("yingzi_zhouyu", p) || p.General1 == "sunce"))
                    return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && p.GetRoleEnum() != PlayerRole.Lord && !p.IsDuanchang(true))
                {
                    if (p.General1 == "liubei" || p.General1 == "liushan" || p.General1 == "zhangjiao" || p.General1 == "caocao_jx" || p.General1 == "sunquan"
                        || p.General1 == "caopi" || p.General1 == "caorui" || p.General1 == "sunliang" || p.General1 == "yuanshu" || p.General1 == "yuanshao"
                        || p.General1 == "dongzhuo")
                        return new List<Player> { p };
                }
            }
            foreach (Player p in targets)
                if (ai.IsFriend(p))
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class LingrenAI : SkillEvent
    {
        public LingrenAI() : base("lingren")
        {
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (ai.HasSkill(Name, from) && from.Phase == PlayerPhase.Play && !from.HasFlag(Name) && to.IsKongcheng() && (card.Name.Contains(Slash.ClassName)
                || card.Name == Duel.ClassName || card.Name == FireAttack.ClassName || card.Name == SavageAssault.ClassName || card.Name == ArcheryAttack.ClassName))
            {
                if (ai.IsEnemy(from, to)) return 4;
                if (!ai.IsCardEffect(card, to, from)) return 3;
            }

            return 0;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use)
            {
                ai.SortByDefense(ref targets, false);
                foreach (Player p in targets)
                    if (ai.IsEnemy(p) && p.IsKongcheng()) return new List<Player> { p };

                foreach (Player p in targets)
                    if (ai.IsEnemy(p) && ai.GetKnownCards(p).Count == p.HandcardNum) return new List<Player> { p };

                foreach (Player p in targets)
                    if (ai.IsEnemy(p) && ai.GetKnownCards(p).Count > 0 && ai.IsCardEffect(use.Card, p, player)) return new List<Player> { p };

                foreach (Player p in targets)
                    if (!ai.IsEnemy(p) && (p.IsKongcheng() || ai.GetKnownCards(p).Count == p.HandcardNum) && !ai.IsCardEffect(use.Card, p, player)) return new List<Player> { p };

                foreach (Player p in targets)
                    if (ai.IsEnemy(p)) return new List<Player> { p };
            }

            return new List<Player>();
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.From != null && ai.HasSkill(Name, damage.From) && damage.From.Phase == PlayerPhase.Play && ai.IsEnemy(damage.From, damage.To) && !damage.From.HasFlag(Name)
                && damage.Card != null && !damage.Chain && !damage.Transfer && (damage.Card.Name.Contains(Slash.ClassName) || damage.Card.Name == Duel.ClassName
                || damage.Card.Name == FireAttack.ClassName || damage.Card.Name == SavageAssault.ClassName || damage.Card.Name == ArcheryAttack.ClassName))
            {
                Room room = ai.Room;
                List<CardUseStruct> use_list = room.GetUseList();
                if (use_list.Count == 0 || use_list[use_list.Count - 1].Card != damage.Card)
                    damage.Damage++;
            }
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player target)
            {
                Room room = ai.Room;
                bool basic = true, equip = false, trick = false;
                List<int> ids = ai.GetKnownCards(target);
                foreach (int id in ids)
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

                if (player.HasFlag("lingren_basic") && !basic && ids.Count != player.HandcardNum)
                    basic = true;

                if (player.HasFlag("lingren_basic"))
                    return basic ? "yes" : "no";

                if (player.HasFlag("lingren_equip"))
                    return equip ? "yes" : "no";

                if (player.HasFlag("lingren_trick"))
                    return trick ? "yes" : "no";
            }

            return "no";
        }
    }

    public class QingzhongAI : SkillEvent
    {
        public QingzhongAI() : base("qingzhong") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            int less = 100000;
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HandcardNum < less) less = p.HandcardNum;

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HandcardNum == less && ((ai.IsFriend(p) && !ai.HasSkill("zishu", p)) || (!ai.IsFriend(p) && ai.HasSkill("zishu", p)))) return true;

            if (player.HandcardNum <= 2) return true;

            return false;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            foreach (Player p in targets)
                if (!ai.IsFriend(p) && ai.HasSkill("zishu", p)) return new List<Player> { p };

            foreach (Player p in targets)
                if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("tuntian|lianying", p)) return new List<Player> { p };

            if (targets.Contains(player)) return new List<Player>();
            foreach (Player p in targets)
                if (!ai.HasSkill("tuntian|lianying", p)) return new List<Player> { p };

            return new List<Player> { targets[0] };
        }
    }

    public class WeijingAI : SkillEvent
    {
        public WeijingAI() : base("weijing") { }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;
            if (player.GetMark(Name) < room.Round && (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY
                || room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE))
            {
                if (pattern == Slash.ClassName && player.GetMark(Name) < room.Round)
                {
                    WrappedCard card = new WrappedCard(WeijingCard.ClassName) { UserString = Slash.ClassName };
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        UserString = RoomLogic.CardToString(room, card)
                    };
                    result.Add(slash);
                }
                else if (pattern == Jink.ClassName && player.GetMark(Name) < room.Round)
                {
                    WrappedCard card = new WrappedCard(WeijingCard.ClassName) { UserString = Jink.ClassName };
                    WrappedCard jink = new WrappedCard(Jink.ClassName)
                    {
                        UserString = RoomLogic.CardToString(room, card)
                    };
                    result.Add(jink);
                }
            }

            return result;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            if (player.GetMark(Name) < room.Round)
            {
                List<int> ids = player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                bool slash = false;
                foreach (int id in ids)
                {
                    if (room.GetCard(id).Name.Contains(Slash.ClassName))
                    {
                        slash = true;
                        break;
                    }
                }

                if (!slash)
                {
                    List<WrappedCard> result = new List<WrappedCard>();
                    WrappedCard card = new WrappedCard(WeijingCard.ClassName) { UserString = Slash.ClassName };
                    WrappedCard slas = new WrappedCard(Slash.ClassName)
                    {
                        UserString = RoomLogic.CardToString(room, card)
                    };
                    result.Add(slas);
                    return result;
                }
            }

            return null;
        }
    }

    public class ChongzhenAI : SkillEvent
    {
        public ChongzhenAI() : base("chongzhen")
        {
            key = new List<string> { "skillInvoke:chongzhen:yes" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && ai.Self != player)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }
                    if (ai.GetPlayerTendency(target) != "unknown" && target.HandcardNum < 6) ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsFriend(target) && target.HandcardNum < 6)
            {
                return false;
            }

            return true;
        }
    }

    public class MizhaoAI : SkillEvent
    {
        public MizhaoAI() : base("mizhao") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && ai.FriendNoSelf.Count > 0 && !player.HasUsed(MizhaoCard.ClassName))
            {
                WrappedCard mz = new WrappedCard(MizhaoCard.ClassName) { Skill = Name };
                mz.AddSubCards(player.GetCards("h"));
                return new List<WrappedCard> { mz };
            }

            return new List<WrappedCard>();
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            Room room = ai.Room;
            Player player = ai.Self;
            List<int> ids = player.GetCards("h");
            ai.SortByKeepValue(ref ids, false);
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            if (player == requestor)
            {
                Player target = players[0];
                bool big = true;
                if (RoomLogic.IsProhibited(room, player, target, slash) != null && RoomLogic.IsProhibited(room, target, player, slash) != null)
                    big = false;
                if (big && !ai.IsCardEffect(slash, player, target) && !ai.IsCardEffect(slash, target, player))
                {
                    if (ai.IsFriend(target))
                        big = false;
                    else
                    {
                        big = false;
                        WrappedCard fire = new WrappedCard(FireSlash.ClassName);
                        if (player.HasWeapon(Fan.ClassName))
                        {
                            if (ai.IsCardEffect(fire, player, target))
                                big = true;
                        }
                        if (!big && target.HasWeapon(Fan.ClassName))
                        {
                            if (ai.IsCardEffect(fire, target, player))
                                big = true;
                        }
                    }
                }
                if (!big)
                    return room.GetCard(ids[0]);
                else
                {
                    WrappedCard card = ai.GetMaxCard(player);
                    return card;
                }
            }
            else
            {
                bool big = true;
                if (RoomLogic.IsProhibited(room, requestor, player, slash) != null)
                    big = false;
                if (big && !ai.IsCardEffect(slash, player, requestor))
                {
                    if (ai.IsFriend(requestor))
                        big = false;
                    else if (requestor.HasWeapon(Fan.ClassName))
                    {
                        WrappedCard fire = new WrappedCard(FireSlash.ClassName);
                        if (!ai.IsCardEffect(fire, player, requestor))
                            big = false;
                    }
                }
                if (!big)
                {
                    return room.GetCard(ids[0]);
                }
                else
                {
                    WrappedCard card = ai.GetMaxCard(player);
                    if (card.Number < 10)
                        return room.GetCard(ids[0]);
                    else
                        return card;
                }
            }
        }
    }

    public class MizhaoCardAI : UseCard
    {
        public MizhaoCardAI() : base(MizhaoCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 2;
            Room room = ai.Room;
            List<Player> friends = ai.FriendNoSelf, enemies = ai.GetEnemies(player);
            ai.SortByHandcards(ref friends);
            ai.SortByDefense(ref enemies, false);
            WrappedCard slash = new WrappedCard(Slash.ClassName);

            if (player.ContainsTag("zhengu") && player.GetTag("zhengu") is string haozhao)
            {
                Player owner = room.FindPlayer(haozhao);
                if (owner != null && ai.IsFriend(owner) && owner.HandcardNum < 5)
                {
                    foreach (Player enemy in enemies)
                    {
                        if (!RoomLogic.CanBePindianBy(room, enemy, owner)) continue;

                        if (RoomLogic.IsProhibited(room, owner, enemy, slash) == null)
                        {
                            DamageStruct damage = new DamageStruct(slash, owner, enemy);
                            if (ai.HasArmorEffect(enemy, Vine.ClassName) && slash.Name == Slash.ClassName && owner.HasWeapon(Fan.ClassName))
                            {
                                WrappedCard fan = new WrappedCard(FireSlash.ClassName);
                                fan.AddSubCard(slash);
                                fan = RoomLogic.ParseUseCard(room, fan);
                                damage.Card = fan;
                            }

                            if (damage.Card.Name == FireSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Fire;
                            else if (damage.Card.Name == ThunderSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Thunder;

                            ScoreStruct damage_score = ai.GetDamageScore(damage);
                            if (damage_score.Score > 0)
                            {
                                if (player.HandcardNum == 2) ai.Number[Name] = 2;
                                card.AddSubCards(player.GetCards("h"));
                                use.Card = card;
                                use.To = new List<Player> { owner, enemy };
                                return;
                            }
                        }
                    }
                }
            }

            Player fazheng = ai.FindPlayerBySkill("enyuan_jx");
            if (fazheng != null && ai.IsFriend(fazheng) && player.HandcardNum >= 2 && !ai.WillSkipPlayPhase(fazheng))
            {
                foreach (Player enemy in enemies)
                {
                    if (!RoomLogic.CanBePindianBy(room, enemy, fazheng)) continue;

                    if (RoomLogic.IsProhibited(room, fazheng, enemy, slash) == null)
                    {
                        DamageStruct damage = new DamageStruct(slash, fazheng, enemy);
                        if (ai.HasArmorEffect(enemy, Vine.ClassName) && slash.Name == Slash.ClassName && fazheng.HasWeapon(Fan.ClassName))
                        {
                            WrappedCard fan = new WrappedCard(FireSlash.ClassName);
                            fan.AddSubCard(slash);
                            fan = RoomLogic.ParseUseCard(room, fan);
                            damage.Card = fan;
                        }

                        if (damage.Card.Name == FireSlash.ClassName)
                            damage.Nature = DamageStruct.DamageNature.Fire;
                        else if (damage.Card.Name == ThunderSlash.ClassName)
                            damage.Nature = DamageStruct.DamageNature.Thunder;

                        ScoreStruct damage_score = ai.GetDamageScore(damage);
                        if (damage_score.Score > 0)
                        {
                            card.AddSubCards(player.GetCards("h"));
                            use.Card = card;
                            use.To = new List<Player> { fazheng, enemy };
                            return;
                        }
                    }
                }
            }


            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in friends)
            {
                if (ai.HasSkill("liegong_jx|tieqi_jx|wushuang|jianchu|moukui|fuqi") || (ai.HasSkill("jiedao", p) && p.IsWounded()))
                {
                    foreach (Player enemy in enemies)
                    {
                        if (!RoomLogic.CanBePindianBy(room, enemy, p)) continue;

                        if (RoomLogic.IsProhibited(room, p, enemy, slash) == null)
                        {
                            ScoreStruct score = new ScoreStruct
                            {
                                Players = new List<Player> { p, enemy },
                                Card = slash,
                            };

                            DamageStruct damage = new DamageStruct(slash, p, enemy);
                            if (ai.HasArmorEffect(enemy, Vine.ClassName) && slash.Name == Slash.ClassName && p.HasWeapon(Fan.ClassName))
                            {
                                WrappedCard fan = new WrappedCard(FireSlash.ClassName);
                                fan.AddSubCard(slash);
                                fan = RoomLogic.ParseUseCard(room, fan);
                                damage.Card = fan;
                            }

                            if (damage.Card.Name == FireSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Fire;
                            else if (damage.Card.Name == ThunderSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Thunder;

                            ScoreStruct damage_score = ai.GetDamageScore(damage);
                            if (damage_score.Score > 0)
                            {
                                ScoreStruct effect = ai.SlashIsEffective(damage.Card, p, enemy);
                                if (effect.Score > 0)
                                {
                                    score.Score = effect.Score;
                                    if (effect.Rate > 0)
                                    {
                                        score.Score += Math.Min(1, effect.Rate) * damage_score.Score;
                                        scores.Add(score);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (scores.Count > 0)
            {
                bool big = false;
                foreach (int id in player.GetCards("h"))
                {
                    if (room.GetCard(id).Number > 11)
                    {
                        big = true;
                        break;
                    }
                }

                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                foreach (ScoreStruct score in scores)
                {
                    if (score.Score > 0 && (big || score.Players[0].HandcardNum > 2))
                    {
                        Debug.Assert(score.Players.Count == 2);
                        use.To = score.Players;
                        use.Card = card;
                        if (player.HandcardNum == 1)
                            ai.Number[Name] = 4;

                        return;
                    }
                }
            }

            scores.Clear();
            foreach (Player p in friends)
            {
                if (!ai.HasSkill("zishu", p) || (player.HandcardNum == 1 && p.IsKongcheng()))
                {
                    foreach (Player enemy in enemies)
                    {
                        if (!RoomLogic.CanBePindianBy(room, enemy, p)) continue;

                        if (RoomLogic.IsProhibited(room, p, enemy, slash) == null)
                        {
                            ScoreStruct score = new ScoreStruct
                            {
                                Players = new List<Player> { p, enemy },
                                Card = slash,
                            };

                            DamageStruct damage = new DamageStruct(slash, p, enemy);
                            if (ai.HasArmorEffect(enemy, Vine.ClassName) && slash.Name == Slash.ClassName && p.HasWeapon(Fan.ClassName))
                            {
                                WrappedCard fan = new WrappedCard(FireSlash.ClassName);
                                fan.AddSubCard(slash);
                                fan = RoomLogic.ParseUseCard(room, fan);
                                damage.Card = fan;
                            }

                            if (damage.Card.Name == FireSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Fire;
                            else if (damage.Card.Name == ThunderSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Thunder;

                            ScoreStruct damage_score = ai.GetDamageScore(damage);
                            if (damage_score.Score > 0)
                            {
                                ScoreStruct effect = ai.SlashIsEffective(damage.Card, p, enemy);
                                if (effect.Score > 0)
                                {
                                    score.Score = effect.Score;
                                    if (effect.Rate > 0)
                                    {
                                        score.Score += Math.Min(1, effect.Rate) * damage_score.Score;
                                        scores.Add(score);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (scores.Count > 0)
            {
                bool big = false;
                foreach (int id in player.GetCards("h"))
                {
                    if (room.GetCard(id).Number > 11)
                    {
                        big = true;
                        break;
                    }
                }

                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                foreach (ScoreStruct score in scores)
                {
                    if (score.Score > 0 && (big || score.Players[0].HandcardNum > 2))
                    {
                        Debug.Assert(score.Players.Count == 2);
                        use.To = score.Players;
                        use.Card = card;
                        if (player.HandcardNum == 1)
                            ai.Number[Name] = 4;

                        return;
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class TianmingAI : SkillEvent
    {
        public TianmingAI() : base("tianming") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardUseStruct use)
            {
                if (!ai.IsCardEffect(use.Card, player, use.From) && player.GetCardCount(true) <= 2) return false;

                if (player.GetCardCount(true) >= 2 && ai.GetKnownCardsNums(Jink.ClassName, "he", player) > 0)
                {
                    Room room = ai.Room;
                    int hp = 0;
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.Hp > hp)
                            hp = p.Hp;

                    List<Player> players = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.Hp == hp) players.Add(p);

                    if (players.Count == 1 && players[0] != player && !ai.IsFriend(players[0]))
                        return false;
                }
            }

            return true;
        }
    }

    public class WeidiJXAI : SkillEvent
    {
        public WeidiJXAI() : base("weidi_jx") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<int> cardsToGet = player.GetPile("#weidi_jx");

            List<Player> friends = new List<Player>();
            foreach (Player p in ai.FriendNoSelf)
                if (!p.HasFlag(Name) && p.Kingdom == player.Kingdom)
                    friends.Add(p);

            if (friends.Count > 0)
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(cardsToGet, friends);
                if (pair.Key != null)
                {
                    WrappedCard card = new WrappedCard(WeidiJXCard.ClassName);
                    card.AddSubCard(pair.Value);
                    use.To = new List<Player> { pair.Key };
                    use.Card = card;
                    return use;
                }

                ai.SortByDefense(ref friends, false);
                ai.SortByKeepValue(ref cardsToGet);
                WrappedCard wd = new WrappedCard(WeidiJXCard.ClassName);
                wd.AddSubCard(cardsToGet[0]);
                use.To = new List<Player> { friends[0] };
                use.Card = wd;
            }

            return use;
        }
    }

    public class LihunAI : SkillEvent
    {
        public LihunAI() : base("lihun") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed(LihunCard.ClassName))
            {
                return new List<WrappedCard> { new WrappedCard(LihunCard.ClassName) { Skill = Name, Mute = true } };
            }
            return new List<WrappedCard>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("he");
            ai.SortByKeepValue(ref ids, false);
            List<int> result = new List<int>();
            for (int i = 0; i < min; i++)
                result.Add(ids[i]);

            return result;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (ai.Self == from && from.ContainsTag(Name) && from.GetTag(Name).ToString() == to.Name)
            {
                if ((card.Name.Contains(Slash.ClassName) || card.Name == Duel.ClassName || card.Name == ArcheryAttack.ClassName || card.Name == SavageAssault.ClassName)
                    && ai.IsCardEffect(card, to, from))
                    return 1.5;
            }

            return 0;
        }
    }

    public class LihunCardAI : UseCard
    {
        public LihunCardAI() : base(LihunCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Dictionary<Player, int> points = new Dictionary<Player, int>();
            foreach (Player p in ai.GetEnemies(player))
            {
                if (ai.HasSkill("zishu", p) && !p.IsKongcheng())
                    points.Add(p, p.HandcardNum);
                else if (p.HandcardNum > p.Hp)
                    points.Add(p, p.HandcardNum - p.Hp);
            }

            if (points.Count > 0)
            {
                List<Player> targets = new List<Player>(points.Keys);
                targets.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                if (points[targets[0]] >= 2)
                {
                    Room room = ai.Room;
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("he"))
                        if (RoomLogic.CanDiscard(room, player, player, id))
                            ids.Add(id);


                    int sub = LijianAI.FindLijianCard(ai, player);
                    if (sub >= 0)
                    {
                        card.AddSubCard(sub);
                        use.Card = card;
                        use.To = new List<Player> { targets[0] };
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6.8;
        }
    }

    public class ZongkuiAI : SkillEvent
    {
        public ZongkuiAI() : base("zongkui") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            return new List<Player> { targets[0] };
        }
    }

    public class CanshiAI : SkillEvent
    {
        public CanshiAI() : base("canshi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use)
            {
                player.SetFlags("target_confirming");
                if (ai.IsCardEffect(use.Card, player, use.From))
                {
                    if (use.Card.Name.Contains(Slash.ClassName) && !ai.IsFriend(use.From))
                    {
                        if (ai.HasSkill("wushuang|jianchu|tieqi_jx", use.From))
                        {
                            player.SetFlags("-target_confirming");
                            return true;
                        }

                        if (ai.GetKnownCardsNums(Jink.ClassName, "he", player) < 2)
                        {
                            player.SetFlags("-target_confirming");
                            return true;
                        }
                    }
                    else if (use.Card.Name == Duel.ClassName)
                    {
                        player.SetFlags("-target_confirming");
                        return true;
                    }
                    else if (ai.IsWeak(player) && (use.Card.Name == SavageAssault.ClassName || use.Card.Name == ArcheryAttack.ClassName))
                    {
                        player.SetFlags("-target_confirming");
                        return true;
                    }
                    else if (ai.IsWeak(player) && !ai.IsFriend(use.From) && use.Card.Name == FireAttack.ClassName && use.From.HandcardNum > 3)
                    {
                        player.SetFlags("-target_confirming");
                        return true;
                    }
                    else if (!ai.IsFriend(use.From) && (use.Card.Name == Snatch.ClassName || use.Card.Name == Dismantlement.ClassName))
                    {
                        player.SetFlags("-target_confirming");
                        return true;
                    }
                }
            }
            player.SetFlags("-target_confirming");
            return false;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag("extra_target_skill") is CardUseStruct use)
            {
                List<Player> result = new List<Player>();
                if (use.Card.Name == Peach.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && p.IsWounded())
                            result.Add(p);
                }
                else if (use.Card.Name == ExNihilo.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && (!ai.HasSkill("zishu", p) || p.Phase != PlayerPhase.NotActive))
                            result.Add(p);
                }
                else if (use.Card.Name.Contains(Slash.ClassName))
                {
                    foreach (Player p in targets)
                    {
                        List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { use.Card }, new List<Player> { p }, false);
                        if (scores.Count > 0 && scores[0].Score > 2)
                            result.Add(p);
                    }
                }
                else if (use.Card.Name == Snatch.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.FindCards2Discard(player, p, use.Card.Name, "hej", HandlingMethod.MethodGet).Score > 0)
                            result.Add(p);
                    }
                }
                else if (use.Card.Name == Dismantlement.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.FindCards2Discard(player, p, use.Card.Name, "hej", HandlingMethod.MethodDiscard).Score > 0)
                            result.Add(p);
                    }
                }

                return result;
            }

            return new List<Player>();
        }
    }

    public class BingzhaoAI : SkillEvent
    {
        public BingzhaoAI() : base("bingzhao")
        {
            key = new List<string> { "skillInvoke:bingzhao" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    Player lord = null;
                    bool friend = strs[2] == "yes";
                    foreach (Player p in ai.Room.GetAlivePlayers())
                    {
                        if (p.GetRoleEnum() == PlayerRole.Lord)
                        {
                            lord = p;
                            break;
                        }
                    }

                    ai.UpdatePlayerRelation(player, lord, friend);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is string str)
            {
                string[] strs = str.Split(':');
                Player lord = ai.Room.FindPlayer(strs[1]);
                return ai.IsFriend(lord);
            }

            return false;
        }
    }

    public class ShichouAI : SkillEvent
    {
        public ShichouAI() : base("shichou") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag("extra_target_skill") is CardUseStruct use)
            {
                List<Player> players = new List<Player>();
                foreach (Player p in targets)
                {
                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { use.Card }, new List<Player> { p }, false);
                    if (scores.Count > 0 && scores[0].Score > 0)
                        players.Add(p);
                }

                return players;
            }

            return new List<Player>();
        }
    }

    public class QiaomengAI : SkillEvent
    {
        public QiaomengAI() : base("qiaomeng")
        {
            key = new List<string> { "cardChosen:qiaomeng" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
            if (ai.Self == player && !(ai is StupidAI)) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Place.PlaceEquip) > 0 ? false : true);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                Room room = ai.Room;
                List<int> ids = new List<int>();
                foreach (int id in target.GetEquips())
                    if (RoomLogic.CanDiscard(room, player, target, id)) ids.Add(id);

                if (ai.IsFriend(target))
                {
                    foreach (int id in ids)
                        if (ai.GetKeepValue(id, target, Place.PlaceEquip) < 0) return true;
                }
                else
                    foreach (int id in ids)
                        if (ai.GetKeepValue(id, target, Place.PlaceEquip) >= 0) return true;
            }

            return false;
        }
    }

    public class JiqiaoAI : SkillEvent
    {
        public JiqiaoAI() : base("jiqiao") { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>(), use = new List<int>();
            foreach (int id in player.GetCards("h"))
            {
                if (!RoomLogic.CanDiscard(room, player, player, id)) continue;
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is Armor && (card.Name != SilverLion.ClassName || !player.IsWounded()))
                    ids.Add(id);
                else if (fcard is EquipCard && ai.GetSameEquip(card, player) != null)
                {
                    if (ai.GetUseValue(id, player) <= 0)
                        ids.Add(id);
                    else
                        use.Add(id);
                }
            }

            foreach (int id in player.GetCards("e"))
            {
                if (!RoomLogic.CanDiscard(room, player, player, id)) continue;
                if (ai.GetKeepValue(id, player) < 0)
                    ids.Add(id);
            }

            if (use.Count > 0)
            {
                foreach (int id in use)
                {
                    WrappedCard card = room.GetCard(id);
                    WrappedCard same = ai.GetSameEquip(card, player);
                    if (!ids.Contains(same.Id) && RoomLogic.CanDiscard(room, player, player, same.Id))
                        ids.Add(same.Id);
                }
            }

            return ids;
        }
    }

    public class JiaoziAI : SkillEvent
    {
        public JiaoziAI() : base("jiaozi") { }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            Room room = ai.Room;
            int max = 0;
            bool same = false;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HandcardNum > max)
                {
                    max = p.HandcardNum;
                    same = false;
                }
                else if (p.HandcardNum == max)
                    same = true;
            }

            if (!same)
            {
                if (damage.From != null && ai.HasSkill(Name, damage.From) && damage.From.HandcardNum == max && step == DamageStruct.DamageStep.Caused)
                    damage.Damage++;

                if (ai.HasSkill(Name, damage.To) && damage.To.HandcardNum == max && step == DamageStruct.DamageStep.Done)
                    damage.Damage++;
            }
        }
    }

    public class YisheAI : SkillEvent
    {
        public YisheAI() : base("yishe") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> cards = player.GetCards("he"), result = new List<int>(); ;
            List<double> values = ai.SortByKeepValue(ref cards, false);
            if (values[0] < 0)
            {
                result.Add(cards[0]);
                cards.RemoveAt(0);
            }

            List<Player> friends = ai.FriendNoSelf;
            Room room = ai.Room;
            room.SortByActionOrder(ref friends);
            foreach (Player p in friends)
            {
                if (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName))
                {
                    bool hongyan = ai.HasSkill("hongyan", p);
                    foreach (int id in cards)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Heart || (card.Suit == WrappedCard.CardSuit.Spade && hongyan))
                        {
                            result.Add(id);
                            break;
                        }
                    }

                    cards.RemoveAll(t => result.Contains(t));
                    if (result.Count >= 2) return result;
                }
                if (RoomLogic.PlayerContainsTrick(room, p, SupplyShortage.ClassName))
                {
                    foreach (int id in cards)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Club)
                        {
                            result.Add(id);
                            break;
                        }
                    }

                    cards.RemoveAll(t => result.Contains(t));
                    if (result.Count >= 2) return result;
                }
            }
            for (int i = 0; i < 2 - result.Count; i++)
                result.Add(cards[i]);

            return result;
        }
    }

    public class BushiAI : SkillEvent
    {
        public BushiAI() : base("bushi") { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> cards = player.GetPile("rice"), result = new List<int>();
            if (cards.Count == 1)
                return cards;

            List<Player> friends = ai.GetFriends(player);
            Room room = ai.Room;
            room.SortByActionOrder(ref friends);

            foreach (Player p in friends)
            {
                if (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName))
                {
                    bool hongyan = ai.HasSkill("hongyan", p);
                    foreach (int id in cards)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Heart || (card.Suit == WrappedCard.CardSuit.Spade && hongyan))
                        {
                            result.Add(id);
                            break;
                        }
                    }

                    if (result.Count > 0)
                        break;
                }
                if (RoomLogic.PlayerContainsTrick(room, p, SupplyShortage.ClassName))
                {
                    foreach (int id in cards)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Club)
                        {
                            result.Add(id);
                            break;
                        }
                    }

                    if (result.Count > 0)
                        break;
                }
            }
            cards.RemoveAll(t => result.Contains(t));
            if (cards.Count > 0)
            {
                ai.SortByKeepValue(ref cards);
                return new List<int> { cards[0] };
            }

            return new List<int>();
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            Room room = ai.Room;
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Top = new List<int>(ups),
                Bottom = new List<int>()
            };

            if (room.GetTag(Name) is Player target)
            {
                if (!ai.IsFriend(target))
                {
                    if (target.IsWounded() && ups.Count == 1 && ai.HasSkill("zishu")) return move;

                    ai.SortByKeepValue(ref ups);
                    move.Bottom = new List<int> { ups[0] };
                    ups.RemoveAt(0);
                    move.Top = ups;
                    move.Success = true;
                }
                else
                {
                    List<Player> friends = ai.GetFriends(player);
                    room.SortByActionOrder(ref friends);
                    List<int> result = new List<int>();
                    foreach (Player p in friends)
                    {
                        if (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName))
                        {
                            bool hongyan = ai.HasSkill("hongyan", p);
                            foreach (int id in ups)
                            {
                                WrappedCard card = room.GetCard(id);
                                if (card.Suit == WrappedCard.CardSuit.Heart || (card.Suit == WrappedCard.CardSuit.Spade && hongyan))
                                {
                                    result.Add(id);
                                    break;
                                }
                            }

                            if (result.Count > 0)
                                break;
                        }
                        if (RoomLogic.PlayerContainsTrick(room, p, SupplyShortage.ClassName))
                        {
                            foreach (int id in ups)
                            {
                                WrappedCard card = room.GetCard(id);
                                if (card.Suit == WrappedCard.CardSuit.Club)
                                {
                                    result.Add(id);
                                    break;
                                }
                            }

                            if (result.Count > 0)
                                break;
                        }
                    }

                    ups.RemoveAll(t => result.Contains(t));
                    if (ups.Count > 0)
                    {
                        ai.SortByKeepValue(ref ups);
                        move.Bottom = new List<int> { ups[0] };
                        ups.RemoveAt(0);
                        move.Top = ups;
                        move.Success = true;
                    }
                }
            }

            return move;
        }
    }

    public class MidaoAI : SkillEvent
    {
        public MidaoAI() : base("midao")
        {
            key = new List<string> { "cardResponded%midao" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && ai.Self != player)
            {
                Room room = ai.Room;
                string[] choices = choice.Split('%');
                if (choices[1] == Name && room.GetTag(Name) is JudgeStruct judge && ai.GetPlayerTendency(judge.Who) == "unknown" && choices[4] != "_nil_")
                {
                    string str = choices[4].Substring(1, choices[4].Length - 2);
                    WrappedCard card = RoomLogic.ParseCard(room, str);
                    WrappedCard.CardSuit suit = RoomLogic.GetCardSuit(room, card);
                    int number = RoomLogic.GetCardNumber(room, card);

                    if (judge.Reason == "beige" || judge.Reason == "beige_jx")
                    {
                        if (!judge.Who.FaceUp && suit == WrappedCard.CardSuit.Spade)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == Lightning.ClassName)
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Spade && judge.Card.Number > 1 && judge.Card.Number <= 9
                            && (suit != WrappedCard.CardSuit.Spade || number > 9 || number == 1))
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                        else if (suit == WrappedCard.CardSuit.Spade && number > 1 && number <= 9)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                    }
                    else if (judge.Reason == SupplyShortage.ClassName)
                    {
                        if (suit != WrappedCard.CardSuit.Club)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (suit == WrappedCard.CardSuit.Club)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "ganglie")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Heart && suit != WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (suit == WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == Indulgence.ClassName)
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Heart && suit != WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (suit == WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == EightDiagram.ClassName)
                    {
                        if (WrappedCard.IsRed(judge.Card.Suit) && WrappedCard.IsBlack(suit))
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (WrappedCard.IsRed(suit))
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "leiji")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Spade && suit != WrappedCard.CardSuit.Spade)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                        else if (suit == WrappedCard.CardSuit.Spade)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                    }
                    else if (judge.Reason == "wuhun")
                    {
                        if (judge.Card.Name != Peach.ClassName && judge.Card.Name != GodSalvation.ClassName && (card.Name == Peach.ClassName || card.Name == GodSalvation.ClassName))
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                        else if (card.Name != Peach.ClassName && card.Name != GodSalvation.ClassName)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                    }
                }
            }
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is JudgeStruct judge)
            {
                if (ai.IsEnemy(judge.Who) && (!ai.IsSituationClear() || !ai.GetPrioEnemies().Contains(judge.Who))) return new List<int>();
                int id = ai.GetRetrialCardId(player.GetPile("rice"), judge);
                if (id >= 0) return new List<int> { id };
            }

            return new List<int>(); ;
        }

        public static bool HasSpade(TrustedAI ai, Player player)
        {
            Room room = ai.Room;

            List<int> ids = player.GetPile("rice");
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Spade && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                    return true;
            }

            return false;
        }

        public static bool HasClub(TrustedAI ai, Player player)
        {
            Room room = ai.Room;

            List<int> ids = player.GetPile("rice");
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Club && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                    return true;
            }
            return false;
        }
        public static bool HasHeart(TrustedAI ai, Player player)
        {
            Room room = ai.Room;

            List<int> ids = player.GetPile("rice");
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                    return true;
            }

            return false;
        }

        public override bool CanRetrial(TrustedAI ai, string pattern, Player player, Player judge_who)
        {
            Room room = ai.Room;
            List<int> ids = player.GetPile("rice");
            if (pattern == "leiji")
            {
                if (ai.IsFriend(player, judge_who))
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Spade && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                            return true;
                    }
                }
                else
                    return HasSpade(ai, player);
            }
            else if (pattern == SupplyShortage.ClassName)
            {
                if (ai.IsFriend(player, judge_who))
                    return HasClub(ai, player);
                else
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Club && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                            return true;
                    }
                }
            }
            else if (pattern == Indulgence.ClassName)
            {
                if (ai.IsFriend(player, judge_who))
                    return HasHeart(ai, player);
                else
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Heart && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                            return true;
                    }
                }
            }
            else if (pattern == Lightning.ClassName)
            {
                if (ai.IsFriend(player, judge_who))
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if ((card.Suit != WrappedCard.CardSuit.Spade || card.Number == 1 || card.Number >= 10)
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                            return true;
                    }
                }
                else
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Spade && card.Number > 1 && card.Number < 10
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodResponse))
                            return true;
                    }
                }
            }

            return false;
        }
    }

    public class MoukuiAI : SkillEvent
    {
        public MoukuiAI() : base("moukui")
        {
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (from != null && card != null && card.Name.Contains(Slash.ClassName) && ai.HasSkill(Name, from) && !ai.IsCardEffect(card, to, from))
            {
                return ai.IsFriend(from) ? 1 : 0;
            }

            return 0;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            ai.Number[Name] = -1;
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag("moukui_target"))
                {
                    target = p;
                    break;
                }
            }

            if (ai.IsEnemy(target) && choice.Contains("discard") && data is CardUseStruct use)
            {
                if (target.GetArmor() && (!ai.IsCardEffect(use.Card, target, player) || target.HasArmor(EightDiagram.ClassName)) && RoomLogic.CanDiscard(room, player, target, target.Armor.Key))
                {
                    ai.Number[Name] = target.Armor.Key;
                    return "discard";
                }

                if (!ai.IsLackCard(target, Jink.ClassName))
                    return "discard";
            }

            return "draw";
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            if (ai.Number.ContainsKey(Name) && ai.Number[Name] > -1)
                return new List<int> { (int)ai.Number[Name] };
            else
                return base.OnCardsChosen(ai, from, to, flags, min, max, disable_ids);
        }
    }

    public class RangshangAI : SkillEvent
    {
        public RangshangAI() : base("rangshang") { }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.Nature == DamageStruct.DamageNature.Fire)
            {
                if (ai.IsFriend(damage.To))
                    score.Score -= 2 * damage.Damage;
                else
                    score.Score = 2 * damage.Damage;
            }

            return score;
        }
    }

    public class HanyongAI : SkillEvent
    {
        public HanyongAI() : base("hanyong") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class XionghuoAI : SkillEvent
    {
        public XionghuoAI() : base("xionghuo") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark(Name) > 0)
                return new List<WrappedCard> { new WrappedCard(XionghuoCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.From != null && ai.HasSkill(Name, damage.From) && damage.To.GetMark(Name) > 0
                && damage.To.ContainsTag(Name) && damage.To.GetTag(Name) is List<string> names && names.Contains(damage.From.Name))
                damage.Damage++;
        }
    }

    public class XionghuoCardAI : UseCard
    {
        public XionghuoCardAI() : base(XionghuoCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                ai.UpdatePlayerRelation(player, target, false);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            Room room = ai.Room;
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies, false);
                if (!player.HasUsed(XionghuoCard.ClassName) && Slash.IsAvailable(room, player))
                {
                    CardUseStruct _use = new CardUseStruct(null, player, new List<Player>())
                    {
                        IsDummy = true
                    };
                    ai.FindSlashandTarget(ref _use, player);

                    if (_use.Card != null && _use.Card.Name.Contains(Slash.ClassName))
                    {
                        foreach (Player p in _use.To)
                        {
                            if (ai.IsEnemy(p) && p.GetMark("xionghuo") == 0)
                            {
                                use.Card = card;
                                use.To = new List<Player> { p };
                                return;
                            }
                        }
                    }
                }

                foreach (Player p in enemies)
                {
                    if (p.Hp == 1 && p.GetMark("xionghuo") == 0)
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class YisuanAI : SkillEvent
    {
        public YisuanAI() : base("yisuan") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.IsWounded() && player.MaxHp > 3)
            {
                Room room = ai.Room;
                WrappedCard card = (WrappedCard)room.GetTag(Name);

                if (card.Name == SavageAssault.ClassName || card.Name == ArcheryAttack.ClassName || card.Name == Duel.ClassName)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    CardUseStruct use = new CardUseStruct(null, player, new List<Player>())
                    {
                        IsDummy = true
                    };
                    if (fcard.IsAvailable(room, player, card))
                    {
                        UseCard e = Engine.GetCardUsage(card.Name);
                        e.Use(ai, player, ref use, card);
                    }

                    if (use.Card != null)
                    {
                        List<Player> enemies = ai.GetEnemies(player);
                        bool kill = false;
                        foreach (Player p in enemies)
                        {
                            if (p.Hp == 1 && ai.IsCardEffect(card, p, player) && !ai.IsCancelTarget(card, p, player) && RoomLogic.IsProhibited(room, player, p, card) == null)
                            {
                                kill = true;
                                break;
                            }
                        }

                        if ((card.Name == SavageAssault.ClassName || card.Name == ArcheryAttack.ClassName) && (enemies.Count > 1 || kill))
                        {
                            return true;
                        }
                        else if (card.Name == Duel.ClassName && kill)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class LangxiAI : SkillEvent
    {
        public LangxiAI() : base("langxi")
        {
            key = new List<string> { "playerChosen:langxi" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in targets)
            {
                DamageStruct damage = new DamageStruct(Name, player, p, 2);
                ScoreStruct score = ai.GetDamageScore(damage);
                score.Players = new List<Player> { p };
                scores.Add(score);
            }

            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0) return scores[0].Players;
            }

            return new List<Player>();
        }
    }

    public class ZhenyiAI : SkillEvent
    {
        public ZhenyiAI() : base("zhenyi") { }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.Nature != DamageStruct.DamageNature.Normal && damage.To.GetMark(Falu.suits[3]) > 0)
            {
                if (damage.Damage < damage.To.Hp || (damage.To.GetMark("@houtu") > 0 && !damage.To.IsKongcheng()))
                    score.Score += ai.IsFriend(damage.To) ? 4.5 : -4.5;
            }

            return score;
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            if (pattern == Peach.ClassName && ai.HasSkill(Name, player) && player.GetMark("@houtu") > 0 && player.HasFlag("Global_Dying") && !player.IsKongcheng())
            {
                if (ai.Self != player)
                    return new List<WrappedCard> { new WrappedCard(Peach.ClassName) };
                else
                {
                    List<int> ids = player.GetCards("h");
                    ai.SortByKeepValue(ref ids, false);
                    WrappedCard peach = new WrappedCard(Peach.ClassName);
                    foreach (int id in ids)
                    {
                        if (!RoomLogic.IsHandCardLimited(ai.Room, player, HandlingMethod.MethodUse))
                        {
                            WrappedCard zy = new WrappedCard(ZhenyiCard.ClassName);
                            zy.AddSubCard(id);
                            peach.UserString = RoomLogic.CardToString(ai.Room, zy);
                            return new List<WrappedCard> { peach };
                        }
                    }
                }
            }

            return new List<WrappedCard>();
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (data is DamageStruct)
            {
                return true;
            }
            else if (data is string && room.GetTag(Name) is JudgeStruct judge)
            {
                string reason = judge.Reason;
                Player who = judge.Who;
                bool good = judge.IsGood();

                if ((ai.IsFriend(who) && good) || (ai.IsEnemy(who) && !good)) return false;
                if (string.IsNullOrEmpty(judge.Pattern))
                {
                    if (reason == "beige" || reason == "beige_jx")
                    {
                        DamageStruct damage = (DamageStruct)room.GetTag("CurrentDamageStruct");
                        if (damage.From != null)
                        {
                            if (ai.IsFriend(damage.From))
                            {
                                if (!ai.ToTurnOver(damage.From) && judge.Card.Suit != WrappedCard.CardSuit.Spade)
                                {
                                    ai.Choice[Name] = "spade";
                                    return true;
                                }
                            }
                            else if (ai.IsEnemy(damage.From) && ai.IsFriend(damage.To))
                            {
                                if (!ai.ToTurnOver(damage.From) && judge.Card.Suit == WrappedCard.CardSuit.Spade)
                                {
                                    ai.Choice[Name] = "heart";
                                    return true;
                                }
                            }
                        }
                    }
                }
                else if (reason == Name)
                {
                    ai.Choice[Name] = "spade";
                    return true;
                }
                else if (reason == Lightning.ClassName)
                {
                    DamageStruct _damage = new DamageStruct(new WrappedCard(Lightning.ClassName), null, judge.Who, 3, DamageStruct.DamageNature.Thunder);
                    ScoreStruct score = ai.GetDamageScore(_damage);
                    if (ai.IsEnemy(who) && score.Score > 6)
                    {
                        ai.Choice[Name] = "spade";
                        return true;
                    }
                    else if (ai.IsFriend(who) && score.Score < -4)
                    {
                        ai.Choice[Name] = "heart";
                        return true;
                    }
                }
                else if (reason == Indulgence.ClassName)
                {
                    if (ai.IsEnemy(who) && (who.HandcardNum >= who.Hp || who.HandcardNum > 2))
                    {
                        ai.Choice[Name] = "spade";
                        return true;
                    }
                    else if (ai.IsFriend(who) && (!ai.WillSkipDrawPhase(who) || who.HandcardNum > 2))
                    {
                        ai.Choice[Name] = "heart";
                        return true;
                    }
                }
                else if (reason == "leiji_jx" && ai.IsEnemy(who))
                {
                    ai.Choice[Name] = "spade";
                    return true;
                }
            }
            else if (data is Player target && room.GetTag(Name) is DamageStruct damage && ai.IsEnemy(target))
            {
                ScoreStruct score = ai.GetDamageScore(damage);
                damage.Damage++;
                if (ai.GetDamageScore(damage).Score - score.Score > 3) return true;
            }

            return false;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return ai.Choice[Name];
        }

        public override bool CanRetrial(TrustedAI ai, string pattern, Player player, Player judge_who)
        {
            if (player.GetMark("@zhiwei") == 0 && (pattern == "leiji" || pattern == "leiji_jx" || pattern == Indulgence.ClassName || pattern == Lightning.ClassName))
                return true;

            return false;
        }
    }

    public class DianhuaAI : SkillEvent
    {
        public DianhuaAI() : base("dianhua")
        {
            key = new List<string> { "dianhuachose" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                List<int> ups = JsonUntity.StringList2IntList(new List<string>(choices[2].Split('+')));
                ai.SetGuanxingResult(player, ups);
                if (ai.Self == player)
                {
                    Room room = ai.Room;
                    foreach (int id in ups)
                        room.GetCard(id).SetFlags("visible2" + player.Name);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            if (ups.Count == 1)
            {
                AskForMoveCardsStruct move = new AskForMoveCardsStruct
                {
                    Success = true,
                    Top = ups,
                    Bottom = new List<int>()
                };
                return move;
            }

            if (player.Phase == PlayerPhase.Start)
            {
                AskForMoveCardsStruct move = ai.Guanxing(ups);
                move.Top.AddRange(move.Bottom);
                move.Bottom.Clear();
                return move;
            }
            else
            {
                AskForMoveCardsStruct move = ai.GuanxingForNext(ups);
                move.Top.AddRange(move.Bottom);
                move.Bottom.Clear();
                return move;
            }
        }
    }

    public class LuanzhanAI : SkillEvent
    {
        public LuanzhanAI() : base("luanzhan")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag("extra_target_skill") is CardUseStruct use)
            {
                List<Player> result = new List<Player>();
                if (use.Card.Name == ExNihilo.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.IsFriend(p)) result.Add(p);
                        if (result.Count >= max) continue;
                    }
                }
                else if (use.Card.Name.Contains(Slash.ClassName))
                {
                    foreach (Player p in targets)
                    {
                        List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { use.Card }, new List<Player> { p }, false);
                        if (scores.Count > 0 && scores[0].Score > 2)
                            result.Add(p);

                        if (result.Count >= max) continue;
                    }
                }
                else if (use.Card.Name == Snatch.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.FindCards2Discard(player, p, use.Card.Name, "hej", HandlingMethod.MethodGet).Score > 0)
                            result.Add(p);

                        if (result.Count >= max) continue;
                    }
                }
                else if (use.Card.Name == Dismantlement.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.FindCards2Discard(player, p, use.Card.Name, "hej", HandlingMethod.MethodDiscard).Score > 0)
                            result.Add(p);

                        if (result.Count >= max) continue;
                    }
                }
                else if (use.Card.Name == FireAttack.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.IsEnemy(p)) result.Add(p);

                        if (result.Count >= max) continue;
                    }
                }
                else if (use.Card.Name == IronChain.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if ((ai.IsEnemy(p) && !p.Chained) || (ai.IsFriend(p) && p.Chained)) result.Add(p);

                        if (result.Count >= max) continue;
                    }
                }

                return result;
            }

            return new List<Player>();
        }
    }

    public class FuqiAI : SkillEvent
    {
        public FuqiAI() : base("fuqi") { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            if (ai.HasSkill(Name, player) && !card.IsVirtualCard())
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is OffensiveHorse && ((isUse && !player.GetOffensiveHorse()) || (!isUse && place == Place.PlaceEquip)))
                    return 4;
            }

            return 0;
        }
    }

    public class BiluanAI : SkillEvent
    {
        public BiluanAI() : base("biluan") { }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            List<int> cards = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetCards("he"))
            {
                if (RoomLogic.CanDiscard(room, player, player, id))
                    cards.Add(id);
            }
            if (cards.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref cards, false);
                if (values[0] < 4)
                    return new List<int> { cards[0] };
            }
            return new List<int>();
        }
    }

    public class LixiaAI : SkillEvent
    {
        public LixiaAI() : base("lixia") { key = new List<string> { "skillChoice:lixia" }; }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && ai.Self != player)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name && strs[2] == "letdraw")
                {
                    Player target = ai.Room.Current;
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player target)
            {
                if (ai.IsFriend(target))
                {
                    if (ai.HasSkill("zishu", target) || ai.IsWeak(target) || ai.WillSkipPlayPhase(player))
                        return "letdraw";
                }
            }

            return "draw";
        }
    }

    public class WenjiAI : SkillEvent
    {
        public WenjiAI() : base("wenji") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                ai.SortByDefense(ref friends);
                foreach (Player p in friends)
                {
                    if (ai.WillSkipPlayPhase(p) && ai.GetOverflow(p) > 0)
                    {
                        return new List<Player> { p };
                    }
                }

                foreach (Player p in friends)
                {
                    foreach (int id in p.GetEquips())
                    {
                        if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                        {
                            return new List<Player> { p };
                        }
                    }
                }

                foreach (Player p in friends)
                {
                    if (p.HandcardNum > 4)
                    {
                        return new List<Player> { p };
                    }
                }
            }

            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                {
                    if (p.IsNude()) continue;
                    bool lose = false;
                    foreach (int id in p.GetEquips())
                    {
                        if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                        {
                            lose = true;
                            break;
                        }
                    }

                    if (!lose)
                    {
                        return new List<Player> { p };
                    }
                }
            }
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!ai.IsFriend(p) && !ai.IsEnemy(p) && !p.IsNude())
                {
                    return new List<Player> { p };
                }
            }
            return new List<Player>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = room.Current;
            List<int> ids = player.GetCards("he");
            List<double> values = ai.SortByKeepValue(ref ids, false);
            if (values[0] < 0) return new List<int> { ids[0] };
            if (ai.IsFriend(target))
            {
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name.Contains(Slash.ClassName) || card.Name == Duel.ClassName)
                        return new List<int> { id };
                }

                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == Dismantlement.ClassName)
                        return new List<int> { id };
                }

                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { target }, Place.PlaceHand, Name);
                if (pair.Key != null)
                    return new List<int> { pair.Value };
            }
            else
            {
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == Jink.ClassName || card.Name == Lightning.ClassName)
                        return new List<int> { id };
                }
            }

            return new List<int> { ids[0] };
        }
    }

    public class TunjiangAI : SkillEvent
    {
        public TunjiangAI() : base("tunjiang") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class HuojiSMAI : SkillEvent
    {
        public HuojiSMAI() : base("huoji_sm")
        {
        }
        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return -2;
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card.Name == FireAttack.ClassName && use.Card.Skill == Name)
                return -2;

            return 0;
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.HasSkill("huoji") && player.GetMark("@dragon") > 0 && player.UsedTimes("ViewAsSkill_huoji_smCard") < 3)
            {
                Room room = ai.Room;
                List<int> ids = player.GetCards("h");
                ids.AddRange(player.GetHandPile());

                foreach (int id in ids)
                {
                    WrappedCard card = ai.Room.GetCard(id);
                    if (WrappedCard.IsRed(card.Suit) && card.Name != FireAttack.ClassName)
                    {
                        WrappedCard slash = new WrappedCard(FireAttack.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name
                        };
                        slash.AddSubCard(card);
                        slash = RoomLogic.ParseUseCard(room, slash);
                        return new List<WrappedCard> { slash };
                    }
                }
            }

            return null;
        }
    }

    public class LianhuanSMAI : SkillEvent
    {
        public LianhuanSMAI() : base("lianhuan_sm")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.HasSkill("lianhuan") && player.GetMark("@phenix") > 0 && player.UsedTimes("ViewAsSkill_lianhuan_smCard") < 3)
            {
                Room room = ai.Room;
                List<int> ids = player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Suit == WrappedCard.CardSuit.Club)
                    {
                        List<WrappedCard> cards = ai.GetViewAsCards(player, id);
                        double value = 0;
                        WrappedCard _card = null;
                        foreach (WrappedCard _c in cards)
                        {
                            double card_value = ai.GetUseValue(_c, player, room.GetCardPlace(id));
                            if (card_value > value)
                            {
                                value = card_value;
                                _card = _c;
                            }
                        }

                        if (_card != null && _card.Name == IronChain.ClassName && _card.Skill == Name) return new List<WrappedCard> { _card };
                    }
                }
            }

            return null;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (card != null && card.Suit == WrappedCard.CardSuit.Club && (player.GetHandPile().Contains(id) || place == Place.PlaceHand))
            {
                WrappedCard ic = new WrappedCard(IronChain.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                ic.AddSubCard(card);
                ic = RoomLogic.ParseUseCard(room, ic);
                return ic;
            }

            return null;
        }
    }
    public class JianjieAI : SkillEvent
    {
        public JianjieAI() : base("jianjie")
        {
            key = new List<string> { "Yiji:jianjie" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                string[] strs = str.Split(':');
                Player target = room.FindPlayer(strs[3]);
                ai.UpdatePlayerRelation(player, target, true);
                if (player == ai.Self)
                {
                    List<string> cards = new List<string>(strs[4].Split('+'));
                    List<int> ids = JsonUntity.StringList2IntList(cards);
                    foreach (int id in ids)
                        ai.SetPrivateKnownCards(target, id);
                }
            }
        }
    }
    public class YinshiAI : SkillEvent
    {
        public YinshiAI() : base("yinshi") { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            if (ai.HasSkill(Name, player) && player.GetMark("@dragon") == 0 && player.GetMark("@phenix") == 0 && Engine.GetFunctionCard(card.Name) is Armor)
                return -6;

            return 0;
        }
    }

    public class JuesiAI : SkillEvent
    {
        public JuesiAI() : base("juesi")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("h"))
                if (room.GetCard(id).Name.Contains(Slash.ClassName) && RoomLogic.CanDiscard(room, player, player, id)) ids.Add(id);

            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids, false);
                WrappedCard js = new WrappedCard(JuesiCard.ClassName) { Skill = Name };
                js.AddSubCard(ids[0]);
                return new List<WrappedCard> { js };
            }

            return null;
        }
    }

    public class JuesiCardAI : UseCard
    {
        public JuesiCardAI() : base(JuesiCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            Room room = ai.Room;
            if (enemies.Count > 0)
            {
                WrappedCard duel = new WrappedCard(Duel.ClassName);
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                {
                    if (p.IsNude() || (p.HandcardNum == 1 && ai.HasSkill("kongcheng|kongcheng_jx", p))) continue;
                    bool lose = false;
                    foreach (int id in p.GetEquips())
                    {
                        if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                        {
                            lose = true;
                            break;
                        }
                    }
                    if (lose) continue;
                    if (p.Hp >= player.Hp && ai.IsWeak(p) && RoomLogic.IsProhibited(room, player, p, duel) == null)
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }

                if (ai.GetOverflow(player) > 0)
                {
                    foreach (Player p in enemies)
                    {
                        if (p.IsNude()) continue;
                        bool lose = false;
                        foreach (int id in p.GetEquips())
                        {
                            if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                            {
                                lose = true;
                                break;
                            }
                        }
                        if (lose) continue;
                        if (p.Hp < player.Hp)
                        {
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 2;
        }
    }

    public class LuemingAI : SkillEvent
    {
        public LuemingAI() : base("lueming") { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            List<string> choices = new List<string> { "A", "K", "Q" };
            if (player.GetPile("incantation").Count > 0)
            {
                int id = player.GetPile("incantation")[0];
                switch (ai.Room.GetCard(id).Number)
                {
                    case 1:
                        choices.Remove("A");
                        break;
                    case 12:
                        choices.Remove("Q");
                        break;
                    case 13:
                        choices.Remove("K");
                        break;
                }
            }
            Shuffle.shuffle(ref choices);
            return choices[0];
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(LuemingCard.ClassName))
            {
                return new List<WrappedCard> { new WrappedCard(LuemingCard.ClassName) { Skill = Name } };
            }

            return null;
        }
    }

    public class LuemingCardAI : UseCard
    {
        public LuemingCardAI() : base(LuemingCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, false);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies);
            foreach (Player p in enemies)
            {
                if (!p.IsNude() && p.GetEquips().Count < player.GetEquips().Count)
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }

            if (enemies.Count > 0)
            {
                use.Card = card;
                use.To.Add(enemies[0]);
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
    }

    public class TunjunAI : SkillEvent
    {
        public TunjunAI() : base("tunjun") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("@tunjun") > 0 && player.GetMark("lueming") >= 3)
                return new List<WrappedCard> { new WrappedCard(TunjunCard.ClassName) { Mute = true } };

            return null;
        }
    }

    public class TunjunCardAI : UseCard
    {
        public TunjunCardAI() : base(TunjunCard.ClassName)
        {
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (player != target && ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, false);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            int count = player.GetMark("lueming");
            List<Player> friends = ai.GetFriends(player);
            foreach (Player p in friends)
            {
                if (p.GetEquips().Count >= 3 || ai.HasSkill("xiongluan|jueyan")) continue;
                if (ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill(TrustedAI.NeedEquipSkill, p))
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }
            foreach (Player p in friends)
            {
                if (p.GetEquips().Count >= 3 || ai.HasSkill("xiongluan|jueyan")) continue;
                if (count - p.GetEquips().Count > 3)
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
    }

    public class XingluanAI : SkillEvent
    {
        public XingluanAI() : base("xingluan") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
    }

    public class SidaoAI : SkillEvent
    {
        public SidaoAI() : base("sidao") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("h"))
                if (!RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodUse, true)) ids.Add(id);

            foreach (int id in player.GetHandPile())
                if (!RoomLogic.IsCardLimited(room, player, room.GetCard(id), HandlingMethod.MethodUse)) ids.Add(id);

            if (ids.Count > 0)
            {
                List<double> values = ai.SortByUseValue(ref ids, false);
                if (values[0] < Engine.GetCardUseValue(Snatch.ClassName) && player.GetTag("sidao_target") is List<string> names)
                {
                    WrappedCard card = new WrappedCard(Snatch.ClassName);
                    card.AddSubCard(ids[0]);
                    card = RoomLogic.ParseUseCard(room, card);

                    List<ScoreStruct> scores = new List<ScoreStruct>();
                    FunctionCard fcard = Engine.GetFunctionCard(Name);
                    List<Player> targets = ai.Exclude(room.GetOtherPlayers(player), card, player);
                    targets.RemoveAll(t => !names.Contains(t.Name));
                    foreach (Player p in targets)
                    {
                        ScoreStruct score = ai.FindCards2Discard(player, p, Name, "hej", HandlingMethod.MethodGet);
                        if (ai.HasSkill("sheyan", p))
                        {
                            if (score.Score > 0)
                            {
                                if (ai.IsFriend(p))
                                    score.Score += 8;
                                else
                                    score.Score /= 2;
                            }
                            else if (ai.IsFriend(p) && score.Score > -1.5)
                                score.Score = 1;
                        }
                        if (ai.HasSkill("qianya", p) && score.Score > 0)
                        {
                            if (ai.IsFriend(p))
                                score.Score += 1;
                            else
                                score.Score /= 2;
                        }
                        foreach (string skill in ai.GetKnownSkills(p))
                        {
                            SkillEvent ev = Engine.GetSkillEvent(skill);
                            if (ev != null)
                                score.Score += ev.TargetValueAdjust(ai, card, player, new List<Player> { p }, p);
                        }
                        scores.Add(score);
                    }

                    bool hand = false;
                    foreach (int id in card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == player)
                        {
                            hand = true;
                            break;
                        }
                    }

                    if (scores.Count > 0)
                    {
                        scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                        if (scores[0].Score > 4 || (scores[0].Score > 0 && hand && ai.GetOverflow(player) > 0))
                        {
                            use.Card = new WrappedCard(SidaoCard.ClassName);
                            use.Card.AddSubCard(ids[0]);
                            use.To = scores[0].Players;
                            return use;
                        }
                    }

                    if (ai.HasSkill("zishu") && hand && player.Phase == PlayerPhase.Play && ai.GetOverflow(player) > 0)
                    {
                        foreach (Player p in targets)
                        {
                            if (!ai.IsFriend(p))
                            {
                                use.Card = new WrappedCard(SidaoCard.ClassName);
                                use.Card.AddSubCard(ids[0]);
                                use.To = new List<Player> { p };
                                return use;
                            }
                        }

                        foreach (Player p in targets)
                        {
                            foreach (int id in p.GetEquips())
                            {
                                WrappedCard equip = room.GetCard(id);
                                if (ai.GetSameEquip(card, player) == null)
                                {
                                    use.Card = new WrappedCard(SidaoCard.ClassName);
                                    use.Card.AddSubCard(ids[0]);
                                    use.To = new List<Player> { p };
                                    return use;
                                }
                            }
                        }

                        foreach (Player p in targets)
                        {
                            use.Card = new WrappedCard(SidaoCard.ClassName);
                            use.Card.AddSubCard(ids[0]);
                            use.To = new List<Player> { p };
                            return use;
                        }
                    }
                }
            }

            return use;
        }
    }

    public class TanbeiAI : SkillEvent
    {
        public TanbeiAI() : base("tanbei") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(TanbeiCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(TanbeiCard.ClassName) { Skill = Name } };

            return null;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            Player target = room.Current;
            if (player.IsNude()) return "got";
            if (ai.IsFriend(target))
                return "infi";
            else
            {
                if (target.HandcardNum < 3 && !ai.IsWeak()) return "infi";
                if (player.GetCards("hej").Count > 5) return "got";
            }

            return "infi";
        }
    }

    public class TanbeiCardAI : UseCard
    {
        public TanbeiCardAI() : base(TanbeiCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> friends = ai.FriendNoSelf;
            foreach (Player p in friends)
            {
                if (p.IsNude() && (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName) || RoomLogic.PlayerContainsTrick(room, p, SupplyShortage.ClassName)))
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }

            foreach (Player p in friends)
            {
                if (p.IsKongcheng() && ai.HasSkill(TrustedAI.LoseEquipSkill, p) && p.HasEquip())
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }

            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0) ai.SortByDefense(ref enemies, false);
            if (ai.GetKnownCardsNums(Snatch.ClassName, "he", player) > 0)
            {
                foreach (Player p in friends)
                {
                    if (RoomLogic.DistanceTo(room, player, p) > 1 && ai.FindCards2Discard(player, p, Snatch.ClassName, "hej", HandlingMethod.MethodGet).Score > 3)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                foreach (Player p in enemies)
                {
                    if (RoomLogic.DistanceTo(room, player, p) > 1 && ai.FindCards2Discard(player, p, Snatch.ClassName, "hej", HandlingMethod.MethodGet).Score > 3)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }
            foreach (Player p in enemies)
            {
                if (!p.IsNude())
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }
            foreach (Player p in enemies)
            {
                if (p.JudgingArea.Count == 0)
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6;
        }
    }
    
    public class LianjiAI : SkillEvent
    {
        public LianjiAI() : base("lianji")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            string target_name = prompt.Split(':')[2];
            Room room = ai.Room;
            Player target = room.FindPlayer(target_name);
            if (target != null)
            {
                double not_use = 0;
                if (ai.HasSkill(TrustedAI.LoseEquipSkill)) not_use += 3;

                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, null, new List<Player> { target });
                if (scores.Count > 0 && scores[0].Score > not_use && scores[0].Card != null)
                {
                    use.Card = scores[0].Card;
                    use.To.Add(target);
                }
            }

            return use;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            if (player.HasFlag("lianji_weapon"))
            {
                List<Player> friends = new List<Player>();
                foreach (Player p in ai.GetEnemies(player))
                    if (targets.Contains(p)) friends.Add(p);

                Room room = ai.Room;
                room.SortByActionOrder(ref friends);
                foreach (Player p in friends)
                    if (ai.HasSkill(TrustedAI.LoseEquipSkill, p)) return new List<Player> { p };

                foreach (Player p in friends)
                    if (!p.GetWeapon()) return new List<Player> { p };

                if (friends.Count > 0) return new List<Player> { friends[0] };
            }
            else
            {
                List<Player> enemies = new List<Player>();
                foreach (Player p in ai.GetEnemies(player))
                    if (targets.Contains(p)) enemies.Add(p);

                if (enemies.Count > 1) ai.SortByDefense(ref enemies);
                if (enemies.Count > 0) return new List<Player> { enemies[0] };
            }

            return new List<Player> { targets[0] };
        }
    }

    public class LianzhuAI : SkillEvent
    {
        public LianzhuAI() : base("lianzhu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(LianzhuCard.ClassName) && !player.IsKongcheng())
            {
                Room room = ai.Room;
                List<int> ids = player.GetCards("h");
                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    if (WrappedCard.IsBlack(room.GetCard(id).Suit))
                    {
                        WrappedCard lz = new WrappedCard(LianzhuCard.ClassName) { Skill = Name };
                        lz.AddSubCard(id);
                        return new List<WrappedCard> { lz };
                    }
                }

                if (ai.GetOverflow(player) > 0 && ai.FriendNoSelf.Count > 0)
                {
                    WrappedCard lz = new WrappedCard(LianzhuCard.ClassName) { Skill = Name };
                    lz.AddSubCard(ids[0]);
                    return new List<WrappedCard> { lz };
                }
            }
            return null;
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            Room room = ai.Room;
            Player target = room.Current;
            if (ai.IsEnemy(target))
            {
                List<int> cards = new List<int>();
                foreach (int id in player.GetCards("he"))
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        cards.Add(id);

                if (cards.Count >= 2)
                {
                    List<double> values = ai.SortByKeepValue(ref cards, false);
                    if (values[0] + values[1] < 4)
                        return new List<int> { ids[0], ids[1] };
                }
            }

            return new List<int>();
        }
    }

    public class LianzhuCardAI : UseCard
    {
        public LianzhuCardAI() : base(LianzhuCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown" && !ai.HasSkill("qingjian|rende|mingjian|mizhao", target))
                    ai.UpdatePlayerRelation(player, target, false);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            Player target = room.Current;

            List<Player> friends = ai.FriendNoSelf;
            bool black = WrappedCard.IsBlack(room.GetCard(card.GetEffectiveId()).Suit);
            if (black)
            {
                ai.Number[Name] = 4;
                foreach (Player p in friends)
                {
                    if (ai.HasSkill("qingjian|rende|mingjian|mizhao", p))
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
                List<Player> enemies = ai.GetEnemies(player);
                ai.SortByHandcards(ref enemies, false);
                foreach (Player p in enemies)
                {
                    if (p.GetCardCount(true) < 2)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                foreach (Player p in enemies)
                {
                    if (!(ai.HasSkill(TrustedAI.LoseEquipSkill, p) && p.HasEquip()) && !(p.IsWounded() && p.HasArmor(SilverLion.ClassName)))
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }
            else
            {
                ai.Number[Name] = 1;
                KeyValuePair<Player, int> key = ai.GetCardNeedPlayer();
                if (key.Key != null)
                {
                    card.ClearSubCards();
                    card.AddSubCard(key.Value);
                    use.To.Add(key.Key);
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class ZhoufuAI : SkillEvent
    {
        public ZhoufuAI() : base("zhoufu")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(ZhoufuCard.ClassName) && !player.IsKongcheng())
                return new List<WrappedCard> { new WrappedCard(ZhoufuCard.ClassName) { Skill = Name } };

            return null;
        }
    }

    public class ZhoufuCardAI : UseCard
    {
        public ZhoufuCardAI() : base(ZhoufuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            List<int> ids = player.GetCards("h");
            ai.SortByUseValue(ref ids, false);
            Room room = ai.Room;
            ai.Number[Name] = 4;
            foreach (Player p in enemies)
            {
                if (ai.HasArmorEffect(p, EightDiagram.ClassName))
                {
                    foreach (int id in ids)
                    {
                        WrappedCard wrapped = room.GetCard(id);
                        if (WrappedCard.IsBlack(wrapped.Suit) && (!ai.HasSkill("hongyan", p) || wrapped.Suit != WrappedCard.CardSuit.Spade))
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }

            ai.Number[Name] = 1;
            foreach (Player p in enemies)
            {
                if (ai.HasSkill("luoshen|luoshen_jx", p))
                {
                    foreach (int id in ids)
                    {
                        WrappedCard wrapped = room.GetCard(id);
                        if (WrappedCard.IsRed(wrapped.Suit))
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }

            foreach (Player p in enemies)
            {
                if (ai.HasSkill("luoshen|luoshen_jx", p))
                {
                    foreach (int id in ids)
                    {
                        WrappedCard wrapped = room.GetCard(id);
                        if (WrappedCard.IsRed(wrapped.Suit))
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }

            foreach (Player p in enemies)
            {
                if (p.JudgingArea.Count > 0)
                {
                    int judge = p.JudgingArea[0];
                    WrappedCard trick = room.GetCard(judge);
                    if (trick.Name == Lightning.ClassName)
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                    else if (trick.Name == SupplyShortage.ClassName)
                    {
                        foreach (int id in ids)
                        {
                            WrappedCard wrapped = room.GetCard(id);
                            if (wrapped.Suit != WrappedCard.CardSuit.Club)
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                    else if (trick.Name == Indulgence.ClassName)
                    {
                        foreach (int id in ids)
                        {
                            WrappedCard wrapped = room.GetCard(id);
                            if (wrapped.Suit != WrappedCard.CardSuit.Heart && (!ai.HasSkill("hongyan", p) || wrapped.Suit != WrappedCard.CardSuit.Spade))
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }
            }

            if (enemies.Count > 0)
            {
                ai.SortByHandcards(ref enemies);
                foreach (Player p in enemies)
                {
                    if (!ai.WillSkipPlayPhase(p))
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                card.AddSubCard(ids[0]);
                use.Card = card;
                use.To.Add(enemies[0]);
                return;
            }
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, false);
            }
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4;
        }
    }

    public class NeifaAI : SkillEvent
    {
        public NeifaAI() : base("neifa") { }

        /*
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            List<int> ids = player.GetCards("he");
            List<double> values = ai.SortByKeepValue(ref ids, false);
            ai.Number[Name] = -1;
            if (values[0] < 0)
            {
                ai.Number[Name] = ids[0];
                return true;
            }

            Room room = ai.Room;
            List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player, true);
            if (slashes.Count > 1)
            {
                int count = 0;
                int discard = -1;
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard))
                    {
                        count++;
                    }
                    else
                    {
                        if (card.Name == Jink.ClassName || (card.Name == Peach.ClassName && !player.IsWounded()))
                            discard = id;
                    }
                }

                int max = Math.Max(5, slashes.Count);
                if (count > 0 && count >= max - 2 && discard > -1)
                {
                    ai.Number[Name] = discard;
                    return true;
                }
            }
            else
            {
                int count = 0;
                ids.Clear();
                foreach (int id in player.GetHandPile())
                {
                    WrappedCard card = room.GetCard(id);
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard))
                    {
                        count++;
                    }
                }

                foreach (int id in player.GetCards("he"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard) && RoomLogic.CanDiscard(room, player, player, id))
                    {
                        ids.Add(id);
                    }
                }

                if (ids.Count > 0 && count > 1)
                {
                    ai.SortByUseValue(ref ids, false);
                    ai.Number[Name] = ids[0];
                    return true;
                }
            }

            return false;
        }
        */

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (player.HasFlag("neifa_invoke"))
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    ScoreStruct score = ai.FindCards2Discard(player, p, Name, "ej", HandlingMethod.MethodGet);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }
                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 3) return scores[0].Players;
                }

                return new List<Player> { player };
            }
            else if (room.GetTag(Name) is CardUseStruct use)
            {
                List<Player> result = new List<Player>();
                switch (use.Card.Name)
                {
                    case "IronChain":
                        {
                            foreach (Player p in targets)
                            {
                                if (ai.IsFriend(p) && p.Chained && !use.To.Contains(p) && !ai.HasSkill("jieying", p))
                                    return new List<Player> { p };
                            }
                            foreach (Player p in targets)
                            {
                                if (ai.IsEnemy(p) && !p.Chained && !use.To.Contains(p)) return new List<Player> { p };
                            }
                        }
                        break;
                    case "Dismantlement":
                        {
                            List<ScoreStruct> scores = new List<ScoreStruct>();
                            foreach (Player p in targets)
                            {
                                if (!use.To.Contains(p))
                                {
                                    ScoreStruct score = ai.FindCards2Discard(use.From, p, Dismantlement.ClassName, "hej", HandlingMethod.MethodDiscard);
                                    scores.Add(score);
                                }
                            }
                            if (scores.Count > 0)
                            {
                                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                                if (scores[0].Score > 0)
                                    return scores[0].Players;
                            }
                        }
                        break;
                    case "Snatch":
                        {
                            List<ScoreStruct> scores = new List<ScoreStruct>();
                            foreach (Player p in targets)
                            {
                                if (!use.To.Contains(p))
                                {
                                    ScoreStruct score = ai.FindCards2Discard(use.From, p, Dismantlement.ClassName, "hej", HandlingMethod.MethodGet);
                                    scores.Add(score);
                                }
                            }
                            if (scores.Count > 0)
                            {
                                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                                if (scores[0].Score > 0)
                                    return scores[0].Players;
                            }
                        }
                        break;
                    case "FireAttack":
                        {
                            ai.SortByDefense(ref targets, false);
                            foreach (Player p in targets)
                            {
                                if (ai.IsEnemy(p) && !use.To.Contains(p))
                                    return new List<Player> { p };
                            }
                        }
                        break;
                    case "SavageAssault":
                    case "ArcheryAttack":
                        {
                            if (use.Card.Name == ArcheryAttack.ClassName)
                                foreach (Player p in targets)
                                    if (!ai.IsFriend(p) && ai.NotSlashJiaozhu(use.From, p, use.Card)) return new List<Player> { p };

                            ai.SortByDefense(ref targets, false);
                            List<ScoreStruct> scores = new List<ScoreStruct>();
                            foreach (Player p in targets)
                            {
                                if (ai.IsFriend(p) && use.Card.Name == ArcheryAttack.ClassName && ai.JiaozhuneedSlash(use.From, p, use.Card)) continue;
                                if (ai.IsCardEffect(use.Card, p, use.From) && !ai.IsCancelTarget(use.Card, p, use.From))
                                {
                                    DamageStruct damage = new DamageStruct(use.Card, use.From, p, 1 + use.ExDamage);
                                    ScoreStruct score = ai.GetDamageScore(damage);
                                    score.Players = new List<Player> { p };
                                    scores.Add(score);
                                }
                            }

                            if (scores.Count > 0)
                            {
                                scores.Sort((x, y) => { return x.Score < y.Score ? -1 : 1; });
                                if (scores[0].Score < 0)
                                    return scores[0].Players;
                            }
                        }
                        break;
                    case "GodSalvation":
                        {
                            ai.SortByDefense(ref targets, false);
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && p.IsWounded() && ai.HasSkill("qingxian|liexian|hexian", p))
                                    return new List<Player> { p };

                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && p.IsWounded())
                                    return new List<Player> { p };
                        }
                        break;
                    case "AmazingGrace":
                        {
                            room.SortByActionOrder(ref targets);
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && (ai.HasSkill("zishu", p) && p.Phase != PlayerPhase.NotActive || ai.HasSkill("qianya|qingjian")))
                                    return new List<Player> { p };
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p) && !ai.HasSkill("zishu", p)) return new List<Player> { p };
                            foreach (Player p in targets)
                                if (ai.IsEnemy(p)) return new List<Player> { p };
                        }
                        break;
                    case "ExNihilo":
                        {
                            foreach (Player p in targets)
                            {
                                if (use.To.Contains(p)) continue;
                                if (ai.IsFriend(p) && (!ai.HasSkill("zishu", p) || p.Phase != PlayerPhase.NotActive)) return new List<Player> { p };
                            }
                        }
                        break;
                    case "Duel":
                        {
                            List<ScoreStruct> scores = new List<ScoreStruct>();
                            foreach (Player p in targets)
                            {
                                if (use.To.Contains(p)) continue;
                                if (ai.IsCardEffect(use.Card, p, use.From) && !ai.IsCancelTarget(use.Card, p, use.From))
                                {
                                    if (ai.HasSkill("hunzi+jiang", p) && p.GetMark("hunzi") == 0 && p.Hp - 1 - use.ExDamage == 1)
                                    {
                                        ScoreStruct score = new ScoreStruct
                                        {
                                            Players = new List<Player> { p },
                                            Score = ai.IsFriend(p) ? 5 : -5
                                        };
                                        scores.Add(score);
                                    }
                                    else
                                    {
                                        DamageStruct damage = new DamageStruct(use.Card, use.From, p, 1 + use.ExDamage);
                                        ScoreStruct score = ai.GetDamageScore(damage);
                                        score.Players = new List<Player> { p };
                                        scores.Add(score);
                                    }
                                }
                            }
                            if (scores.Count > 0)
                            {
                                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                                if (scores[0].Score > 0)
                                    return scores[0].Players;
                            }
                        }
                        break;
                }

                return result;
            }
            return new List<Player>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return "draw";
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int _max, string pile)
        {
            List<int> ids = player.GetCards("he");
            List<double> values = ai.SortByKeepValue(ref ids, false);
            if (values[0] < 0) return new List<int> { ids[0] };

            Room room = ai.Room;
            List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player, true);
            if (slashes.Count > 1)
            {
                int count = 0;
                int discard = -1;
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard))
                    {
                        count++;
                    }
                    else
                    {
                        if (card.Name == Jink.ClassName || (card.Name == Peach.ClassName && !player.IsWounded()))
                            discard = id;
                    }
                }

                int max = Math.Max(5, slashes.Count);
                if (count > 0 && count >= max - 2 && discard > -1)
                {
                    return new List<int> { discard };
                }
            }
            else
            {
                int count = 0;
                ids.Clear();
                foreach (int id in player.GetHandPile())
                {
                    WrappedCard card = room.GetCard(id);
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard))
                    {
                        count++;
                    }
                }

                foreach (int id in player.GetCards("he"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard) && RoomLogic.CanDiscard(room, player, player, id))
                    {
                        ids.Add(id);
                    }
                }

                if (ids.Count > 0 && count > 1)
                {
                    ai.SortByUseValue(ref ids, false);
                    return new List<int> { ids[0] };
                }
            }

            return new List<int>();
        }
    }

    public class ZhenduJXAI : SkillEvent
    {
        public ZhenduJXAI() : base("zhendu_jx")
        { }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> cards, int min, int max, bool option)
        {
            if (!ai.WillShowForAttack()) return new List<int>();

            Room room = ai.Room;
            Player target = room.Current;
            List<int> ids = new List<int>();

            foreach (int id in player.GetCards("h"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);
            ai.SortByKeepValue(ref ids, false);
            double value = ai.GetKeepValue(ids[0], player);
            if (ai.GetOverflow(player) > 1)
                value /= 3;
            DamageStruct damage = new DamageStruct(Name, player, target);
            if (ai.IsFriend(target))
            {
                bool range = false;
                foreach (Player p in room.GetOtherPlayers(target))
                {
                    if (ai.GetPrioEnemies().Contains(p) && RoomLogic.InMyAttackRange(room, target, p))
                    {
                        range = true;
                        break;
                    }
                }

                if (range && target.HandcardNum + target.GetPile("wooden_ox").Count > 3)
                {
                    if (value < 3 && target == player) return new List<int> { ids[0] };
                    if (ai.GetDamageScore(damage).Score < -3 && ai.HasSkill("wushuang_jx|jianchu", target) && value < 3)
                        return new List<int> { ids[0] };
                }
            }
            else
            {
                if (ai.GetDamageScore(damage).Score > 7 && value < 7)
                    return new List<int> { ids[0] };
                else if (ai.GetDamageScore(damage).Score > 4 && value < 3)
                {
                    if (ai.IsLackCard(target, Slash.ClassName) || target.HandcardNum + target.GetPile("wooden_ox").Count < 3)
                    {
                        return new List<int> { ids[0] };
                    }
                    else
                    {
                        bool range = false;
                        foreach (Player p in room.GetOtherPlayers(target))
                        {
                            if (ai.IsFriend(p) && RoomLogic.InMyAttackRange(room, target, p))
                            {
                                range = true;
                                break;
                            }
                        }

                        if (!range) return new List<int> { ids[0] };
                    }
                }
            }

            return new List<int>();
        }
    }

    public class QiluanJXAI : SkillEvent
    {
        public QiluanJXAI() : base("qiluan_jx") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class BeizhanCAI : SkillEvent
    {
        public BeizhanCAI() : base("beizhan_classic") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            int count = 0;
            Player target = null;

            foreach (Player p in ai.GetFriends(player))
            {
                if (p.MaxHp - p.HandcardNum > count && p.HandcardNum < 5)
                {
                    count = p.MaxHp - p.HandcardNum;
                    target = p;
                }
            }

            if (target != null) return new List<Player> { target };

            foreach (Player p in ai.GetEnemies(player))
            {
                if (!ai.WillSkipPlayPhase(p) && (p.HandcardNum > p.MaxHp ||  p.HandcardNum >= 5))
                {
                    return new List<Player> { p };
                }
            }

            return new List<Player>();
        }
    }

    public class HongyuanAI : SkillEvent
    {
        public HongyuanAI() : base("hongyuan")
        {
            key = new List<string> { "playerChosen:hongyuan" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    List<Player> targets = new List<Player>();
                    foreach (string name in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(name));

                    foreach (Player p in targets)
                        if (ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> friends = ai.FriendNoSelf;
            List<Player> result = new List<Player>();
            if (friends.Count > 0 && ai.WillSkipPlayPhase(player) || friends.Count > 1)
            {
                ai.SortByDefense(ref friends, false);
                foreach (Player p in friends)
                {
                    if (ai.HasSkill("zishu", p)) continue;
                    if (result.Count < 2)
                        result.Add(p);
                    else
                        break;
                }

                if (result.Count == 1)
                {
                    foreach (Player p in friends)
                    {
                        if (result.Count < 2)
                        {
                            if (!result.Contains(p))
                                result.Add(p);
                        }
                        else
                            break;
                    }
                }
            }

            return result;
        }
    }

    public class MingzheAI : SkillEvent
    {
        public MingzheAI() : base("mingzhe") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class HuanshiAI : SkillEvent
    {
        public HuanshiAI() : base("huanshi")
        {
            key = new List<string> { "skillInvoke:huanshi:yes" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && room.GetTag(Name) is JudgeStruct judge)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Player target = judge.Who;
                    if (player != ai.Self)
                    {
                        if (ai.GetPlayerTendency(target) != "unknown")
                        {
                            bool friendly = false;
                            if (judge.Reason == Lightning.ClassName)
                            {
                                friendly = judge.Card.Suit == WrappedCard.CardSuit.Spade && judge.Card.Number > 1 && judge.Card.Number < 10;
                                ai.UpdatePlayerRelation(player, target, friendly);
                            }
                            else if (judge.Reason == SupplyShortage.ClassName)
                            {
                                friendly = judge.Card.Suit != WrappedCard.CardSuit.Club;
                                ai.UpdatePlayerRelation(player, target, friendly);
                            }
                            else if (judge.Reason == Indulgence.ClassName)
                            {
                                friendly = judge.Card.Suit != WrappedCard.CardSuit.Heart;
                                ai.UpdatePlayerRelation(player, target, friendly);
                            }
                        }
                    }
                    else
                    {
                        ai.ClearKnownCards(target);
                        foreach (int id in target.GetCards("h"))
                            ai.SetPrivateKnownCards(target, id);
                    }
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (data is Player target && room.GetTag(Name) is JudgeStruct judge)
            {
                if (ai.IsFriend(target))
                {
                    if (ai.GetRetrialCardId(player.GetCards("h"), judge) != -1)
                        return true;
                }
                else if (ai.IsEnemy(target))
                {
                    if (judge.Reason == Lightning.ClassName && (judge.Card.Suit != WrappedCard.CardSuit.Spade || judge.Card.Number > 1 || judge.Card.Number < 10))
                    {
                        DamageStruct damage = new DamageStruct(Lightning.ClassName, null, judge.Who, 3, DamageStruct.DamageNature.Thunder);
                        if (ai.GetDamageScore(damage).Score > 6)
                        {
                            bool match = true;
                            foreach (int id in player.GetCards("h"))
                            {
                                WrappedCard card = room.GetCard(id);
                                if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodResponse, true)) continue;
                                if (card.Suit != WrappedCard.CardSuit.Spade || card.Number == 1 || card.Number > 9)
                                {
                                    match = false;
                                    break;
                                }
                            }

                            return match;
                        }
                    }
                    else if (judge.Reason == SupplyShortage.ClassName && judge.Card.Suit == WrappedCard.CardSuit.Club)
                    {
                        bool match = true;
                        foreach (int id in player.GetCards("h"))
                        {
                            WrappedCard card = room.GetCard(id);
                            if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodResponse, true)) continue;
                            if (card.Suit == WrappedCard.CardSuit.Club)
                            {
                                match = false;
                                break;
                            }
                        }

                        return match;
                    }
                    else if (judge.Reason == Indulgence.ClassName && judge.Card.Suit == WrappedCard.CardSuit.Heart)
                    {
                        bool match = true;
                        foreach (int id in player.GetCards("h"))
                        {
                            WrappedCard card = room.GetCard(id);
                            if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodResponse, true)) continue;
                            if (card.Suit == WrappedCard.CardSuit.Heart || (ai.HasSkill("hongyan", target) && card.Suit == WrappedCard.CardSuit.Spade))
                            {
                                match = false;
                                break;
                            }
                        }

                        return match;
                    }
                }
            }

            return false;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            Room room = ai.Room;
            List<int> ids = to.GetCards("h");
            if (disable_ids != null)
                foreach (int id in disable_ids)
                    ids.Remove(id);
            if (room.GetTag(Name) is JudgeStruct judge)
            {
                if (ai.IsFriend(to))
                {
                    ai.SortByKeepValue(ref ids, false);
                    int id = ai.GetRetrialCardId(ids, judge, false);
                    if (id != -1)
                        return new List<int> { id };
                }
                else
                    ai.SortByKeepValue(ref ids);

                if (judge.Reason == Lightning.ClassName)
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Spade || card.Number == 1 || card.Number > 9)
                            return new List<int> { id };
                    }
                }
                else if (judge.Reason == SupplyShortage.ClassName && judge.Card.Suit != WrappedCard.CardSuit.Club)
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Club)
                            return new List<int> { id };
                    }
                }
                else if (judge.Reason == Indulgence.ClassName && judge.Card.Suit != WrappedCard.CardSuit.Heart)
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Heart || (ai.HasSkill("hongyan", from) && card.Suit == WrappedCard.CardSuit.Spade))
                            return new List<int> { id };
                    }
                }

                return new List<int> { ids[0] };
            }

            return new List<int>();
        }
    }

    public class AocaiAI : SkillEvent
    {
        public AocaiAI() : base("aocai") { }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;
            if (player.Phase == PlayerPhase.NotActive && player.GetPile("#aocai").Count > 0
                && (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE
                || room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                && (pattern == Slash.ClassName || pattern == Peach.ClassName || pattern == Jink.ClassName || pattern == Analeptic.ClassName))
            {
                foreach (int id in player.GetPile("#aocai"))
                {
                    if (Engine.MatchExpPattern(room, pattern, player, room.GetCard(id)))
                    {
                        WrappedCard card = Engine.CloneCard(room.GetCard(id));
                        card.Skill = Name;
                        card.UserString = id.ToString();
                        result.Add(card);
                    }
                }
            }

            return result;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 20;
        }
    }

    public class DuwuAI : SkillEvent
    {
        public DuwuAI() : base("duwu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.HasFlag(Name))
                return new List<WrappedCard> { new WrappedCard(DuwuCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

    }

    public class DuwuCardAI : UseCard
    {
        public DuwuCardAI() : base(DuwuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<int> ids = player.GetCards("he");
            List<Player> enemes = ai.GetEnemies(player);
            if (enemes.Count == 0) return;
            if (ids.Count > 0)
            {
                List<int> subs = new List<int>();
                List<double> values = ai.SortByKeepValue(ref ids, false);
                for (int i = 0; i < values.Count; i++)
                    if (values[i] < 0) subs.Add(ids[i]);

                ids.RemoveAll(t => subs.Contains(t));
                ai.SortByUseValue(ref ids, false);
                ids.InsertRange(0, subs);
            }
            Room room = ai.Room;
            ai.SortByDefense(ref enemes, false);
            foreach (Player p in enemes)
            {
                if (p.Hp == 0)
                {
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }
            foreach (Player p in enemes)
            {
                int count = 0;
                for (int i = p.Hp; i > 0; i--)
                    count += i;

                if (!RoomLogic.InMyAttackRange(room, player, p) || count > ids.Count) continue;
                DamageStruct damage = new DamageStruct(Name, player, p, 1);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score > 0)
                {
                    DamageStruct damage2 = new DamageStruct(Name, player, p, p.Hp);
                    ScoreStruct score2 = ai.GetDamageScore(damage2);
                    if (score2.Score > 7)
                    {
                        List<int> subs = new List<int>();
                        for (int i = 0; i < count; i++)
                            subs.Add(ids[i]);

                        card.AddSubCards(subs);
                        if (RoomLogic.InMyAttackRange(room, player, p, card))
                        {
                            card.SubCards.Clear();
                            subs.Clear();
                            for (int i = 0; i < p.Hp; i++)
                                subs.Add(ids[i]);

                            card.AddSubCards(subs);
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 1.5;
        }
    }

    public class HongdeAI : SkillEvent
    {
        public HongdeAI() : base("hongde")
        {
            key = new List<string> { "playerChosen:hongde" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsFriend(p))
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class DingpanAI : SkillEvent
    {
        public DingpanAI() : base("dingpan") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            int count = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.GetRoleEnum() == PlayerRole.Rebel)
                    count++;

            if (player.UsedTimes(DingpanCard.ClassName) < count)
                return new List<WrappedCard> { new WrappedCard(DingpanCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player from)
            {
                DamageStruct damage = new DamageStruct(Name, from, player);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score > 0) return "get";

                if (ai.HasSkill(TrustedAI.LoseEquipSkill) && score.Score > -4) return "get";
                if (player.IsWounded() && player.HasArmor(SilverLion.ClassName)) return "get";
            }

            return "discard";
        }
    }

    public class DingpanCardAI : UseCard
    {
        public DingpanCardAI() : base(DingpanCard.ClassName)
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 3.2;
            if (player.HasArmor(SilverLion.ClassName) && player.GetEquips().Count > 1 && ai.FriendNoSelf.Count > 0)
            {
                use.Card = card;
                use.To = new List<Player> { player };
                ai.Number[Name] = 15;
                return;
            }
            Room room = ai.Room;

            Dictionary<Player, double> points = new Dictionary<Player, double>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasEquip())
                {
                    DamageStruct damage = new DamageStruct("dingpan", player, p);
                    double damage_v = ai.GetDamageScore(damage).Score;
                    double discard_v = 0;
                    if (RoomLogic.CanDiscard(room, player, p, "e"))
                        discard_v = ai.FindCards2Discard(player, p, "dingpan", "e", HandlingMethod.MethodDiscard).Score;

                    if (ai.IsFriend(p))
                    {
                        double value = Math.Max(damage_v, discard_v);
                        points[p] = value;
                    }
                    else
                    {
                        double value = Math.Min(damage_v, discard_v);
                        points[p] = value;
                    }
                }
            }
            if (points.Count > 0)
            {
                List<Player> targets = new List<Player>(points.Keys);
                targets.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                if (points[targets[0]] > 0)
                {
                    use.Card = card;
                    use.To = new List<Player> { targets[0] };
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class MeibuAI : SkillEvent
    {
        public MeibuAI() : base("meibu")
        {
            key = new List<string> { "cardDiscard:meibu" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                if (choices[1] == Name)
                {
                    Player target = room.Current;
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }
        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            Room room = ai.Room;
            Player target = room.Current;

            if (!target.ContainsTag("sidi") && ai.IsEnemy(target))
            {
                if (target.HandcardNum > 3 || target.HandcardNum > target.Hp)
                {
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (values[0] < 3)
                        return new List<int> { ids[0] };

                    if (target.HandcardNum - target.Hp > 2 && values[0] < 5)
                        return new List<int> { ids[0] };
                }
            }

            return new List<int>();
        }
    }

    public class MumuAI : SkillEvent
    {
        public MumuAI() : base("mumu")
        {
            key = new List<string> { "cardChosen:mumu" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
            if (ai.Self == player && !(ai is StupidAI)) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);
                    ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Place.PlaceEquip) > 0 ? false : true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in targets)
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, Name, "e", HandlingMethod.MethodDiscard);
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                    return scores[0].Players;
            }

            CardUseStruct use = new CardUseStruct(null, player, new List<Player>())
            {
                IsDummy = true
            };

            ai.FindSlashandTarget(ref use, player);
            if (use.Card == null)
            {
                scores.Clear();
                foreach (Player p in targets)
                {
                    ScoreStruct score = ai.FindCards2Discard(player, p, Name, "e", HandlingMethod.MethodGet);
                    scores.Add(score);
                }
                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0)
                        return scores[0].Players;
                }
            }

            return new List<Player>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>())
            {
                IsDummy = true
            };

            ai.FindSlashandTarget(ref use, player);
            if (use.Card == null)
                return "get";

            return "discard";
        }
    }

    public class ZhixiAI : SkillEvent
    {
        public ZhixiAI() : base("zhixi") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card.Name == Peach.ClassName)
                return 4;
            if (Engine.GetFunctionCard(use.Card.Name).TypeID == CardType.TypeTrick)
                return -5;

            return 0;
        }
    }

    public class XingwuAI : SkillEvent
    {
        public XingwuAI() : base("xingwu")
        {
            key = new List<string> { "playerChosen:xingwu" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> piles = player.GetPile(Name);
            Room room = ai.Room;
            List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
            foreach (int id in piles)
            {
                WrappedCard card = room.GetCard(id);
                if (!suits.Contains(card.Suit))
                    suits.Add(card.Suit);
            }

            if (ai.GetOverflow(player) > 0 || (piles.Count == 0 && player.HandcardNum > 1))
            {
                List<int> ids = player.GetCards("h");
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (ai.GetOverflow(player) > 0 && piles.Count > 0)
                {
                    for (int i = 0; i < ai.GetOverflow(player); i++)
                    {
                        WrappedCard card = room.GetCard(ids[i]);
                        if (!suits.Contains(card.Suit))
                            return new List<int> { ids[i] };
                    }
                }

                return new List<int> { ids[0] };
            }

            if (suits.Count == 2)
            {
                List<int> ids = player.GetCards("h");
                List<double> values = ai.SortByKeepValue(ref ids, false);
                int sub = -1;
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (!suits.Contains(card.Suit))
                    {
                        sub = id;
                        break;
                    }
                }

                if (sub > -1)
                {
                    List<ScoreStruct> scores = new List<ScoreStruct>();
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        DamageStruct damage = new DamageStruct(Name, player, p, p.IsMale() ? 2 : 1);
                        scores.Add(ai.GetDamageScore(damage));
                    }

                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 7)
                        return new List<int> { sub };
                }
            }

            return new List<int>();
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in targets)
            {
                DamageStruct damage = new DamageStruct(Name, player, p, p.IsMale() ? 2 : 1);
                ScoreStruct score = ai.GetDamageScore(damage);
                score.Players = new List<Player> { p };
                scores.Add(score);
            }

            scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
            return scores[0].Players;
        }
    }

    public class YinbingAI : SkillEvent
    {
        public YinbingAI() : base("yinbing") { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = new List<int>(), result = new List<int>(); ;
            Room room = ai.Room;
            foreach (int id in player.GetCards("he"))
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (!(fcard is BasicCard))
                    ids.Add(id);
            }
            if (ids.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 0)
                {
                    result.Add(ids[0]);
                    ids.RemoveAt(0);
                }
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is TrickCard && card.Name != Nullification.ClassName)
                        result.Add(id);
                    if (fcard is EquipCard && (room.GetCardPlace(id) == Place.PlaceHand || fcard is OffensiveHorse
                        || ai.GetKeepValue(id, player) < 3 || (fcard is Weapon && ai.FriendNoSelf.Count > 0)))
                        result.Add(id);
                }
            }
            return result;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if ((card.Name.Contains(Slash.ClassName) || card.Name == Duel.ClassName) && ai.IsCardEffect(card, to, from) && to.GetPile(Name).Count > 0)
            {
                if (ai.IsFriend(to))
                    return -3;
                else
                    return 3;
            }

            return 0;
        }
    }

    public class JuediAI : SkillEvent
    {
        public JuediAI() : base("juedi")
        {
            key = new List<string> { "playerChosen:juedi" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            int equip = 0;
            List<int> ids = player.GetPile("yinbing");
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                CardType type = Engine.GetFunctionCard(card.Name).TypeID;
                if (type == CardType.TypeEquip) equip++;
                if (card.Name == Vine.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("shixin", p))
                            return new List<Player> { p };
                }
                if (card.Name == SilverLion.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("dingpan", p))
                            return new List<Player> { p };
                }
                if (card.Name == Spear.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("tushe", p))
                            return new List<Player> { p };
                }
                if (card.Name == EightDiagram.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("tiandu", p))
                            return new List<Player> { p };
                }
            }

            double value = (player.MaxHp - player.HandcardNum) * 1.5;
            Dictionary<Player, double> points = new Dictionary<Player, double>();
            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p) || ai.HasSkill("zishu", p)) continue;
                double _value = ids.Count * 1.5;
                if (p.IsWounded()) _value += 3;
                if (ai.HasSkill(TrustedAI.CardneedSkill, p)) _value += ids.Count;
                if (ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill(TrustedAI.NeedEquipSkill, p) && !ai.WillSkipPlayPhase(p))
                    _value += equip * 2;
                points[p] = _value;
            }

            if (points.Count > 0)
            {
                List<Player> frineds = new List<Player>(points.Keys);
                frineds.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                if (value < points[frineds[0]]) return new List<Player> { frineds[0] };
            }

            return new List<Player>();
        }
    }

    public class ChouhaiAI : SkillEvent
    {
        public ChouhaiAI() : base("chouhai") { }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (ai.HasSkill(Name, damage.To) && damage.To.IsKongcheng() && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName))
                damage.Damage++;
        }
    }

    public class CanshiSHAI : SkillEvent
    {
        public CanshiSHAI() : base("canshi_sh") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is string str)
            {
                string[] strs = str.Split(':');
                int count = int.Parse(strs[strs.Length - 1]);
                if (count >= 2) return true;
                if (ai.WillSkipPlayPhase(player) && count > 2) return true;
                if (count > 0 && count + 2 + player.HandcardNum <= player.MaxHp) return true;
            }

            return false;
        }
    }

    public class XiashuAI : SkillEvent
    {
        public XiashuAI() : base("xiashu")
        {
            key = new List<string> { "playerChosen:xiashu" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        bool friend = target.HandcardNum < player.HandcardNum || ai.HasSkill("qingjian", target);
                        ai.UpdatePlayerRelation(player, target, friend);
                    }
                }
            }
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            Room room = ai.Room;
            AskForMoveCardsStruct move = new AskForMoveCardsStruct();
            move.Success = true;
            move.Top = new List<int>(ups);
            move.Bottom = new List<int>();
            if (room.GetTag(Name) is Player target)
            {
                if (ups.Count < player.HandcardNum)
                {
                    move.Success = false;
                    move.Top = new List<int>();
                }
            }

            return move;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = room.Current;
            if (ai.IsFriend(target))
            {
                List<int> hands = player.GetCards("h"), ids = new List<int>();
                ai.SortByUseValue(ref hands);
                bool jink = false, slash = false; ;
                foreach (int id in hands)
                {
                    WrappedCard card = room.GetCard(id);
                    if (Engine.GetFunctionCard(card.Name).IsAvailable(room, target, card) && (!card.Name.Contains(Slash.ClassName) || !slash || ai.HasCrossbowEffect(player))
                        && (card.Name != Jink.ClassName || !jink))
                    {
                        ids.Add(id);
                        if (card.Name.Contains(Slash.ClassName)) slash = true;
                        if (card.Name == Jink.ClassName) jink = true;
                    }
                }

                return ids;
            }
            else
            {
                int count = player.HandcardNum / 2;
                List<int> hands = player.GetCards("h");
                List<List<int>> all = TrustedAI.GetCombinationList(hands, count);
                List<List<int>> others = new List<List<int>>();
                List<int> good = new List<int>();
                double best = 100;
                foreach (List<int> combin in all)
                {
                    List<int> rest = new List<int>(hands);
                    rest.RemoveAll(t => combin.Contains(t));
                    double v1 = 0, v2 = 0;
                    foreach (int id in combin)
                        v1 += ai.GetKeepValue(id, player);

                    foreach (int id in rest)
                        v2 += ai.GetKeepValue(id, player);

                    double real = Math.Abs(v1 - v2);
                    if (real < best)
                    {
                        good = combin;
                        best = real;
                    }
                }

                return good;
            }
        }
    }

    public class QizhouAI : SkillEvent
    {
        public QizhouAI() : base("qizhou") { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            return card != null && !card.IsVirtualCard() && Engine.GetFunctionCard(card.Name) is EquipCard ? 2 : 0;
        }
    }

    public class ShanxiAI : SkillEvent
    {
        public ShanxiAI() : base("shanxi")
        {
            key = new List<string> { "cardChosen:shanxi" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
            if (ai.Self == player && !(ai is StupidAI)) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceJudge)
                    {
                        if (room.GetCard(card_id).Name != Lightning.ClassName)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            Player winner = ai.GetWizzardRaceWinner(room.GetCard(card_id).Name, target, target);
                            if (winner != null && ai.IsFriend(winner, target))
                                ai.UpdatePlayerRelation(player, target, false);
                            else
                                ai.UpdatePlayerRelation(player, target, true);
                        }
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceHand)
                    {
                        ai.UpdatePlayerRelation(player, target, ai.HasSkill("kongcheng|kongcheng_jx") && target.HandcardNum == 1 ? true : false);
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                    {
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Place.PlaceEquip) > 0 ? false : true);
                    }
                }
            }
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(ShanxiCard.ClassName))
            {
                int jink = 0, sub = -1;
                Room room = ai.Room;
                foreach (int id in player.GetCards("h"))
                {
                    if (room.GetCard(id).Name == Jink.ClassName) jink++;
                }
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is BasicCard && WrappedCard.IsRed(card.Suit))
                    {
                        if ((room.GetCard(id).Name == Jink.ClassName && jink > 1) || (fcard is Slash && (!Slash.IsAvailable(room, player, card) || ai.GetOverflow(player) > 0)))
                        {
                            sub = id;
                            break;
                        }
                    }
                }
                if (sub > -1)
                {
                    WrappedCard sx = new WrappedCard(ShanxiCard.ClassName) { Skill = Name };
                    sx.AddSubCard(sub);
                    return new List<WrappedCard> { sx };
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class ShanxiCardAI : UseCard
    {
        public ShanxiCardAI() : base(ShanxiCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!RoomLogic.CanDiscard(room, player, p, "he")) continue;
                ScoreStruct score = ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodDiscard);
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                }
            }
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 3.1;
        }
    }
}