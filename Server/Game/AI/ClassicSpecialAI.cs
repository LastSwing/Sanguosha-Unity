using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                new ChongzhenAI(),
                new MizhaoAI(),
                new TianmingAI(),
                new WeidiJXAI(),
                new LihunAI(),
                new ZongkuiAI(),
                new CanshiAI(),
                new ShichouAI(),
                new QiaomengAI(),
                new JiqiaoAI(),
                new JiaoziAI(),
                new YisheAI(),
                new BushiAI(),
                new MidaoAI(),
                new MoukuiAI(),

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
        public LiangzhuAI() : base("liangzhu") { key = new List<string> { "skillChoice" }; }

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
            key = new List<string> { "playerChosen" };
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

            foreach (Player _p in targets)
                if (_p.GetRoleEnum() == PlayerRole.Lord)
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class ChouceAI : SkillEvent
    {
        public ChouceAI() : base("chouce")
        {
            key = new List<string> { "playerChosen", "cardChosen" };
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
            key = new List<string> { "playerChosen" };
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
            for (int i = 0; i < 4; i++)
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

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> result = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetEquips())
            {
                if (RoomLogic.CanDiscard(room, player,player, id) && ai.GetKeepValue(id, player) < 0)
                    result.Add(id);
                if (result.Count >= min)
                    break;
            }

            if (result.Count < min)
            {
                foreach (int id in player.GetEquips())
                {
                    if (!result.Contains(id) && RoomLogic.CanDiscard(room, player, player, id)
                        && ai.FindSameEquipCards(room.GetCard(id), true, false).Count > 0)
                        result.Add(id);

                    if (result.Count >= min)
                        break;
                }
            }

            if (result.Count < min)
            {
                List<int> ids = player.GetCards("h");
                ai.SortByUseValue(ref ids, false);
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
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    double value = ai.GetUseValue(id, player);
                    value += (13 - card.Number) / 13;
                    if (card.Number == player.GetMark("@she"))
                        value -= 1;

                    points[id] = value;
                }

                List<int> ids = new List<int>(points.Keys);
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
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && ai.Self != player)
            {
                foreach (Player p in use.To)
                    if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, false);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player), targets = new List<Player>();
            ai.SortByDefense(ref enemies, false);
            Room room = ai.Room;
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
        { key = new List<string> { "cardChosen" }; }
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
                if (ai.IsFriend(p))
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
                    scores.Add(score);
                }
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0 )
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
        public WeikuiAI() : base("weikui") {}

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(WeikuiCard.ClassName) && (player.Hp > 2 || (player.Hp == 0 && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) > 0)))
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
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                ai.UpdatePlayerRelation(player, use.To[0], false);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemes = ai.GetEnemies(player);
            ai.SortByDefense(ref enemes, false);
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            Room room = ai.Room;
            foreach (Player p in enemes)
            {
                if (!p.IsKongcheng() && RoomLogic.IsProhibited(room, player, p, slash) != null && ai.IsCardEffect(slash, p, player))
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

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4;
        }
    }

    public class LizhanAI : SkillEvent
    {
        public LizhanAI() : base("lizhan")
        {
            key = new List<string> { "playerChosen" };
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
            foreach (Player p in targets)
                if (ai.IsFriend(p)) result.Add(p);

            return result;
        }
    }

    public class ZhenweiAI : SkillEvent
    {
        public ZhenweiAI() : base("zhenwei")
        {
            key = new List<string> { "cardDiscard" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                if (choices[1] == Name && room.GetTag(Name) is List<CardUseStruct> uses)
                {
                    Player target = uses[uses.Count - 1].To[0];
                    if (ai.GetPlayerTendency(target) != "unknown" && ai.Self != target)
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

    public class ChongzhenAI : SkillEvent
    {
        public ChongzhenAI() : base("chongzhen")
        {
            key = new List<string> { "skillInvoke" };
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
                return new List<WrappedCard> { new WrappedCard(MizhaoCard.ClassName) { Skill = Name } };

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
                if (RoomLogic.IsProhibited(room, player, target, slash) == null && RoomLogic.IsProhibited(room, target, player, slash) == null)
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
                    if (!big)
                    {
                        return room.GetCard(ids[0]);
                    }
                    else
                    {
                        WrappedCard card = ai.GetMaxCard(player);
                        return card;
                    }
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

            return base.OnPindian(ai, requestor, players);
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

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in friends)
            {
                if (ai.HasSkill("liegong_jx|tieqi_jx|wushuang|jianchu"))
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

                if (player.GetCardCount(true) >= 2 || ai.GetKnownCardsNums(Jink.ClassName, "he", player) > 0)
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
                if (p.HandcardNum > p.Hp)
                    points.Add(p, p.HandcardNum - p.Hp);

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
                        if (ai.IsFriend(p))
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
            key = new List<string> { "cardChosen" };
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
            return base.OnExchange(ai, player, pattern, min, max, pile);
        }
    }

    public class BushiAI : SkillEvent
    {
        public BushiAI() : base("bushi") { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return base.OnExchange(ai, player, pattern, min, max, pile);
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            return base.OnMoveCards(ai, player, ups, downs, min, max);
        }
    }

    public class MidaoAI : SkillEvent
    {
        public MidaoAI() : base("midao")
        {
            key = new List<string> { "cardResponded" };
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

                    if (judge.Reason == "beige")
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
                if (target.GetArmor() && (!ai.IsCardEffect(use.Card, target, player) || target.HasArmor(EightDiagram.ClassName)))
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

    public class HongyuanAI : SkillEvent
    {
        public HongyuanAI() : base("hongyuan")
        {
            key = new List<string> { "playerChosen" };
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
                for (int i = 0; i < Math.Min(2, friends.Count); i++)
                    result.Add(friends[i]);
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
            key = new List<string> { "skillInvoke" };
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
            return base.GetTurnUse(ai, player);
        }

    }

    public class DuwuCardAI : UseCard
    {
        public DuwuCardAI() : base(DuwuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return base.UsePriorityAdjust(ai, player, targets, card);
        }
    }

    public class HongdeAI : SkillEvent
    {
        public HongdeAI() : base("hongde")
        {
            key = new List<string> { "playerChosen" };
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
                if (ai.IsFriend(from) && (player.Hp > 1 || player.HasArmor(SilverLion.ClassName)))
                    return "get";
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
            foreach (Player p in ai.FriendNoSelf)
            {
                if (p.HasArmor(SilverLion.ClassName))
                {
                    use.Card = card;
                    use.To = new List<Player> { p };
                    ai.Number[Name] = 15;
                    return;
                }
            }

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasEquip() && RoomLogic.CanDiscard(room, player, p, "e"))
                    scores.Add(ai.FindCards2Discard(player, p, "dingpan", "e", HandlingMethod.MethodDiscard));
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
            return ai.Number[Name];
        }
    }

    public class MeibuAI : SkillEvent
    {
        public MeibuAI() : base("meibu")
        {
            key = new List<string> { "cardDiscard" };
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
            key = new List<string> { "cardChosen" };
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
            key = new List<string> { "playerChosen" };
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
}