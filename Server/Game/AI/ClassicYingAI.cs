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
    public class ClassicYingAI : AIPackage
    {
        public ClassicYingAI() : base("ClassicYing")
        {
            events = new List<SkillEvent>
            {
                new ZhengrongAI(),
                new QingceAI(),
                new HongjuAI(),
                new JianxiangAI(),
                new ShenshiAI(),
                new ZhenguAI(),

                new ChenglueAI(),
                new ShicaiJXAI(),
                new TusheAI(),
                new LimuAI(),
                new MingrenAI(),
                new ZhenliangAI(),
                new XiongluanAI(),
                new CongjianJXAI(),

                new ZuilunAI(),
                new JuzhanAI(),
                new FeijunAI(),
                new WanglieAI(),

                new LiangyinAI(),
                new KongshengAI(),
                new JueyanAI(),
                new HuairouAI(),
                new YiliAI(),
                new ZhenglunAI(),
                new KuizhuAI(),
                new ChezhengAI(),
                new LijunAI(),
            };

            use_cards = new List<UseCard>
            {
                 new ChenglueCardAI(),
                 new LimuCardAI(),
                 new JueyanCardAI(),
                 new HuairouCardAI(),
                 new FeijunCardAI(),
                 new QingceCardAI(),
                 new ShenshiCardAI(),
                 new ZhenliangCardAI(),
                 new XiongluanCardAI(),
                 new CongjianCardAI(),
            };
        }
    }

    public class ZhengrongAI : SkillEvent
    {
        public ZhengrongAI() : base("zhengrong"){}

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsFriend(target))
            {
                bool lose = false;
                foreach (int id in target.GetEquips())
                {
                    if (ai.GetKeepValue(id, target, Place.PlaceEquip) < 0)
                    {
                        lose = true;
                        break;
                    }
                }

                if (!lose) return false;
            }

            return true;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> result = new List<Player>();
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsEnemy(p))
                    return new List<Player> { p };

            foreach (Player p in targets)
                if (!ai.IsFriend(p))
                    return new List<Player> { p };

            return result;
        }
    }

    public class HongjuAI : SkillEvent
    {
        public HongjuAI() : base("hongju") { }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            int count = ups.Count;
            List<int> all = new List<int>(ups);
            all.AddRange(downs);
            ai.SortByUseValue(ref all);
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Top = new List<int>(),
                Bottom = new List<int>(),
                Success = true
            };
            for (int i = 0; i < all.Count; i++)
                if (i < count)
                    move.Top.Add(all[i]);
                else
                    move.Bottom.Add(all[i]);

            return move;
        }
    }

    public class QingceAI : SkillEvent
    {
        public QingceAI() : base("qingce")
        {
            key = new List<string> { "cardChosen:qingce" };
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
            List<int> ids = player.GetPile("zhengrong");
            if (ids.Count > 0)
            {
                WrappedCard qc = new WrappedCard(QingceCard.ClassName) { Skill = Name };
                qc.AddSubCard(ids[0]);
                return new List<WrappedCard> { qc };
            }

            return new List<WrappedCard>();
        }
    }

    public class QingceCardAI : UseCard
    {
        public QingceCardAI() : base(QingceCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (!RoomLogic.CanDiscard(room, player, p, "ej")) continue;
                ScoreStruct score = ai.FindCards2Discard(player, p, Name, "ej", HandlingMethod.MethodDiscard);
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 2)
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

    public class JianxiangAI : SkillEvent
    {
        public JianxiangAI() : base("jianxiang")
        {
            key = new List<string> { "playerChosen:jianxiang" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2], true);

                    if (ai.GetPlayerTendency(target) != "unknown" && !(ai.NeedKongcheng(target) && target.IsKongcheng()))
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsFriend(p) && ai.HasSkill("zishu", p) && p == room.Current)
                    return new List<Player> { p };

            foreach (Player p in targets)
                if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && !(ai.NeedKongcheng(p) && p.IsKongcheng() && !ai.MaySave(p)))
                    return new List<Player> { p };

            foreach (Player p in targets)
                if (ai.IsFriend(p) && !(ai.NeedKongcheng(p) && p.IsKongcheng() && !ai.MaySave(p)))
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class ShenshiAI : SkillEvent
    {
        public ShenshiAI() : base("shenshi")
        {
            key = new List<string> { "playerChosen:shenshi" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2], true);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.SortByHandcards(ref targets, false);
            Room room = ai.Room;
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && ai.HasSkill("zishu", p) && room.Current == p)
                    return new List<Player> { p };
            }

            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && !ai.HasSkill("zishu", p))
                    return new List<Player> { p };
            }

            foreach (Player p in targets)
            {
                if (ai.IsFriend(p))
                    return new List<Player> { p };
            }

            return new List<Player>();
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsNude() && !player.HasUsed(ShenshiCard.ClassName) && player.GetMark(Name) == 0)
                return new List<WrappedCard> { new WrappedCard(ShenshiCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("he");
            List<double> values = ai.SortByKeepValue(ref ids, false);
            if (values[0] < 0) return new List<int> { ids[0] };

            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HasFlag(Name))
                {
                    target = p;
                    break;
                }
            }

            if (ai.IsFriend(target))
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { target });
                if (pair.Key != null && pair.Value > -1)
                    return new List<int> { pair.Value };
            }
            else if (target.Phase == PlayerPhase.Play)
            {
                foreach (int id in ids)                     //技能弃牌、转化怎么判断？？？？
                {
                    if (room.GetCard(id).Name == Jink.ClassName)
                        return new List<int> { id };
                }
            }

            return new List<int> { ids[0] };
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.IsNude()) return true;

            if (data is Player target)
            {
                if (ai.IsFriend(target))
                    return true;
                else if (target.HandcardNum - 1 < 4)
                {
                    Room room = ai.Room;
                    if (ai.HasSkill("zishu", target) && room.Current == target) return false;

                    List<int> ids = player.GetCards("he");
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (values[0] < 0 || target.Phase != PlayerPhase.Play)
                        return true;

                    if (target.HandcardNum < RoomLogic.GetMaxCards(room, target))
                    {
                        if (ai.HasSkill("zhiheng_jx", target) && !target.HasUsed(ZhihengJCard.ClassName)) return false;
                        if (ai.HasSkill("quji", target) && !target.HasUsed(QujiCard.ClassName) && target.IsWounded()) return false;
                        if (ai.HasSkill(TrustedAI.ActiveCardneedSkill, target)) return false;

                        foreach (int id in ids)                     //技能弃牌、转化怎么判断？？？？
                        {
                            if (room.GetCard(id).Name == Jink.ClassName)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            Room room = ai.Room;
            if (player == ai.Self && player.ContainsTag(Name) && player.GetTag(Name) is Dictionary<int, string> names && card != null)
            {
                foreach (int id in names.Keys)
                {
                    if (card.SubCards.Contains(id))
                    {
                        Player target = room.FindPlayer(names[id]);
                        if (target != null && target.HandcardNum < 4)
                        {
                            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                            if (ai.IsFriend(target))
                            {
                                if (isUse && !(fcard is EquipCard))
                                    return -4;
                                else
                                    return 5;
                            }
                            else
                            {
                                if (isUse && !(fcard is EquipCard))
                                    return 3;
                                else
                                    return -8;
                            }
                        }
                    }
                }
            }

            return 0;
        }
    }

    public class ShenshiCardAI : UseCard
    {
        public ShenshiCardAI() : base(ShenshiCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            int count = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum > count) count = p.HandcardNum;
            List<Player> targets = new List<Player>();
            foreach (Player p in ai.GetEnemies(player))
                if (p.HandcardNum == count)
                    targets.Add(p);
            
            List<int> ids = player.GetCards("he");
            int sub = -1;
            List<double> values = ai.SortByKeepValue(ref ids, false);
            if (values[0] < 0) sub = ids[0];

            if (targets.Count > 0)
            {
                ai.SortByDefense(ref targets, false);
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in targets)
                {
                    DamageStruct damage = new DamageStruct("shenshi", player, p);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }

                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    if (sub > -1)
                    {
                        card.AddSubCard(sub);
                        use.Card = card;
                        use.To = scores[0].Players;
                        return;
                    }
                    else
                    {
                        foreach (int id in ids)
                        {
                            if (ai.GetKeepValue(id, scores[0].Players[0], Place.PlaceHand) < scores[0].Score && !ai.IsCard(id, Analeptic.ClassName, scores[0].Players[0])
                                && !ai.IsCard(id, Peach.ClassName, scores[0].Players[0]))
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To = scores[0].Players;
                                return;
                            }
                        }
                    }
                }
            }

            targets = ai.FriendNoSelf;
            if (targets.Count > 0)
            {
                ai.SortByDefense(ref targets);
                foreach (Player p in targets)
                {
                    if (p.HandcardNum == count)
                    {
                        DamageStruct damage = new DamageStruct("shenshi", player, p);
                        ScoreStruct score = ai.GetDamageScore(damage);
                        if (score.Score > 0)
                        {
                            if (sub > -1)
                            {
                                card.AddSubCard(sub);
                                use.Card = card;
                                use.To = new List<Player> { p };
                                return;
                            }
                            else
                            {
                                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { p });
                                if (pair.Key != null && pair.Value > -1)
                                {
                                    card.AddSubCard(pair.Value);
                                    use.Card = card;
                                    use.To = new List<Player> { p };
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 3;
        }
    }

    public class ZhenguAI : SkillEvent
    {
        public ZhenguAI() : base("zhengu")
        {
            key = new List<string> { "playerChosen:zhengu" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2], true);
                    if (ai.HasSkill("mingjian|rende|mizhao", target) && target.HandcardNum - player.HandcardNum <= 2)
                    {
                        ai.UpdatePlayerRelation(player, target, true);
                    }

                    bool friendly = target.HandcardNum < player.HandcardNum;
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, friendly);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Dictionary<Player, double> points = new Dictionary<Player, double>();
            Room room = ai.Room;
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && ai.HasSkill("rende|mingjian|mizhao", p) && !ai.WillSkipPlayPhase(p) && p.HandcardNum - player.HandcardNum <= 2)
                    return new List<Player> { p };
            }

            foreach (Player p in targets)
            {
                double value = 0;
                if (ai.IsFriend(p) && p.HandcardNum < player.HandcardNum && !ai.HasSkill("zishu", p))
                {
                    value = player.HandcardNum - p.HandcardNum * 1.5 - 1;
                    Player next = room.GetNextAlive(player, 1, false);
                    int count = 0;
                    while (next != p)
                    {
                        count++;
                        next = room.GetNextAlive(next, 1, false);
                    }
                    value -= 0.5 * count;
                }
                else if (ai.IsEnemy(p) && p.HandcardNum > player.HandcardNum)
                {
                    value = p.HandcardNum - player.HandcardNum * 1.5 - 1;
                }
                points[p] = value;
            }
            targets.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
            if (points[targets[0]] > 0)
                return new List<Player> { targets[0] };

            return new List<Player>();
        }
    }

    public class ChenglueAI : SkillEvent
    {
        public ChenglueAI() : base("chenglue") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(ChenglueCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(ChenglueCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return ai.AskForDiscard(player.GetCards("h"), Name, max, min, false);
        }
    }

    public class ChenglueCardAI : UseCard
    {
        public ChenglueCardAI() : base(ChenglueCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6;
        }
    }

    public class ShicaiJXAI : SkillEvent
    {
        public ShicaiJXAI() : base("shicai_jx") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class TusheAI : SkillEvent
    {
        public TusheAI() : base("tushe")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            if (ai.HasSkill(Name, player) && Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeBasic && !card.IsVirtualCard())
                return isUse ? 2 : -2;

            return 0;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card != null && Engine.GetFunctionCard(use.Card.Name).TypeID == CardType.TypeBasic && !use.Card.IsVirtualCard())
                return 4;

            return 0;
        }
    }

    public class LimuAI : SkillEvent
    {
        public LimuAI() : base("limu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            if (!RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName))
            {
                bool use = false;
                if ((player.HasWeapon(Spear.ClassName) && !RoomLogic.PlayerContainsTrick(room, player, Lightning.ClassName))
                    || player.Hp == 1 || (ai.GetOverflow(player) > 0 && player.IsWounded()))
                    use = true;

                if (!use)
                {
                    List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player);
                    if (slashes.Count > 1)
                    {
                        List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
                        if (scores.Count > 0 && scores[0].Score > 0)
                            use = true;
                    }
                }

                if (use)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("he"))
                    {
                        if (room.GetCard(id).Name == Lightning.ClassName) return new List<WrappedCard>();
                        if (room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
                            ids.Add(id);
                    }
                    foreach (int id in player.GetHandPile())
                    {
                        if (room.GetCard(id).Name == Lightning.ClassName) return new List<WrappedCard>();
                        if (room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
                            ids.Add(id);
                    }

                    if (ids.Count > 0)
                    {
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < 0)
                        {
                            WrappedCard lm = new WrappedCard(LimuCard.ClassName);
                            lm.AddSubCard(ids[0]);
                            lm.Skill = Name;
                            return new List<WrappedCard> { lm };
                        }

                        foreach (int id in ids)
                        {
                            if (room.GetCard(id).Name == Jink.ClassName)
                            {
                                WrappedCard lm = new WrappedCard(LimuCard.ClassName);
                                lm.AddSubCard(id);
                                lm.Skill = Name;
                                return new List<WrappedCard> { lm };
                            }
                        }

                        ai.SortByUseValue(ref ids, false);
                        WrappedCard _lm = new WrappedCard(LimuCard.ClassName);
                        _lm.AddSubCard(ids[0]);
                        _lm.Skill = Name;
                        return new List<WrappedCard> { _lm };
                    }
                }
            }

            return new List<WrappedCard>();
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (card != null && to != null && from != null && from.HasWeapon(Spear.ClassName) && ai.HasSkill("zhenlie", to) && !ai.IsFriend(from, to))
                return -8;
            return 0;
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (damage.From != null && ai.HasSkill(Name, damage.From) && damage.From.HasWeapon(Spear.ClassName) && ai.HasSkill("fankui|fankui_jx|duodao", damage.To)
                && damage.Damage < damage.To.Hp && !ai.IsFriend(damage.From, damage.To))
            {
                if (ai.IsFriend(damage.From)) score.Score = -7;
                else if (ai.IsEnemy(damage.From)) score.Score = 7;
            }
            return score;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            if (card.Name == Spear.ClassName)
                return 20;

            return 0;
        }
    }

    public class LimuCardAI : UseCard
    {
        public LimuCardAI() : base(LimuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6;
        }
    }

    public class MingrenAI : SkillEvent
    {
        public MingrenAI() : base("mingren")
        {}

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Top = new List<int>(),
                Bottom = new List<int>(),
                Success = true
            };
            List<int> ids = new List<int>(ups);
            ai.SortByKeepValue(ref ids, false);
            ids.AddRange(downs);
            if (player.GetMark("zhenliang") > 0)
            {
                Room room = ai.Room;
                int sub = -1;
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (Engine.GetFunctionCard(card.Name).TypeID == CardType.TypeBasic)
                    {
                        sub = id;
                        break;
                    }
                }
                if (sub == -1)
                {
                    sub = ids[0];
                }
                move.Bottom.Add(sub);
                ids.Remove(sub);
                move.Top = ids;
            }
            else
            {
                move.Bottom.Add(ids[0]);
                ids.RemoveAt(0);
                move.Top = ids;
            }

            return move;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("h");
            ai.SortByUseValue(ref ids, false);
            return new List<int> { ids[0] };
        }
    }

    public class ZhenliangAI : SkillEvent
    {
        public ZhenliangAI() : base("zhenliang")
        {
            key = new List<string> { "playerChosen:zhenliang" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2], true);

                    if (ai.GetPlayerTendency(target) != "unknown" && !(ai.NeedKongcheng(target) && target.IsKongcheng()))
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && ai.HasSkill("zishu", p) && p == room.Current) return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && ai.HasSkill("qingjian", p) && !p.HasFlag("qingjian") && ai.IsFriend(room.Current)) return new List<Player> { p };
            }
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && ai.HasSkill("rende", p) && !ai.WillSkipPlayPhase(p) && room.GetFront(player, p) == p) return new List<Player> { p };
            }

            return new List<Player> { player };
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(ZhenliangCard.ClassName) && player.GetMark(Name) == 0 && !player.IsNude())
                return new List<WrappedCard> { new WrappedCard(ZhenliangCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class ZhenliangCardAI : UseCard
    {
        public ZhenliangCardAI() : base(ZhenliangCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>(), available = new List<int>();
            List<int> pile = player.GetPile("mingren");
            bool black = WrappedCard.IsBlack(room.GetCard(pile[0]).Suit);

            foreach (int id in player.GetCards("he"))
                if (RoomLogic.CanDiscard(room, player, player, id) && WrappedCard.IsBlack(room.GetCard(id).Suit) == black) ids.Add(id);

            if (ids.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref ids, false);
                for (int i = 0; i < ids.Count; i++)
                {
                    if (values[i] < 0)
                        available.Add(ids[i]);
                    else
                        break;
                }
                ids.RemoveAll(t => available.Contains(t));
                values = ai.SortByUseValue(ref ids);
                available.AddRange(ids);

                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    int count = Math.Max(1, Math.Abs(player.Hp - p.Hp));
                    if (RoomLogic.InMyAttackRange(room, player, p) && available.Count >= count)
                    {
                        DamageStruct damage = new DamageStruct("zhenliang", player, p);
                        ScoreStruct score = ai.GetDamageScore(damage);
                        List<int> subs = new List<int>();
                        if (score.Score >= 0)
                        {
                            score.Players = new List<Player> { p };
                            double value = 0;
                            for (int i = 0; i < count; i++)
                            {
                                subs.Add(ids[i]);
                                double _value = ai.GetKeepValue(available[i], player);
                                if (_value < 0) value += _value;
                                else value += ai.GetUseValue(available[i], player);
                            }
                            if (value > 0) value /= 1.4;
                            score.Score -= value;
                            score.Card = new WrappedCard(DummyCard.ClassName);
                            score.Card.AddSubCards(subs);
                            scores.Add(score);
                        }
                    }
                }
                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0)
                    {
                        card.AddSubCard(scores[0].Card);
                        use.Card = card;
                        use.To = scores[0].Players;
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 3;
        }
    }

    public class XiongluanAI : SkillEvent
    {
        public XiongluanAI() : base("xiongluan")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("@luan") > 0)
            {
                ai.Target[Name] = null;
                List<Player> enemies = ai.GetPrioEnemies();
                if (enemies.Count > 0)
                {
                    ai.SortByDefense(ref enemies, false);
                    foreach (Player p in enemies)
                    {
                        double count = 0;
                        List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player);
                        foreach (WrappedCard card in slashes)
                        {
                            if (ai.IsCardEffect(card, p, player) && !ai.IsCancelTarget(card, p, player))
                                count++;
                        }
                        if (ai.HasArmorEffect(p, EightDiagram.ClassName)) count /= 2;

                        List<WrappedCard> duel = ai.GetCards(Duel.ClassName, player);
                        foreach (WrappedCard card in duel)
                        {
                            if (ai.IsCardEffect(card, p, player) && !ai.IsCancelTarget(card, p, player))
                                count++;
                        }

                        if (count - p.Hp > 1)
                        {
                            ai.Target[Name] = p;
                            return new List<WrappedCard> { new WrappedCard(XiongluanCard.ClassName) };
                        }
                    }
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class XiongluanCardAI : UseCard
    {
        public XiongluanCardAI() : base(XiongluanCard.ClassName) { }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (ai.Target["xiongluan"] != null)
            {
                use.Card = card;
                use.To = new List<Player> { ai.Target["xiongluan"] };
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class CongjianJXAI : SkillEvent
    {
        public CongjianJXAI() : base("congjian_jx") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            if (room.GetTag("congjian_jx") is CardUseStruct current_use && !player.IsNude())
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in use.To)
                    if (p != player && ai.IsFriend(p)) targets.Add(p);

                if (targets.Count > 0)
                {
                    ai.SortByDefense(ref targets, false);
                    List<int> ids = player.GetCards("he");
                    int sub = -1;
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (values[0] < 0) sub = ids[0];
                    if (sub == -1)
                    {
                        foreach (int id in ids)
                        {
                            WrappedCard card = room.GetCard(id);
                            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                            if (fcard is EquipCard)
                            {
                                if (player.GetMark("@luan") == 0 || fcard is Weapon || fcard is OffensiveHorse || player == room.Current)
                                {
                                    sub = -1;
                                    break;
                                }
                            }
                        }
                    }
                    if (sub == -1)
                    {
                        if (player.Phase == PlayerPhase.NotActive && values[0] < 2)
                            sub = ids[0];
                        else if (player == room.Current)
                        {
                            values = ai.SortByUseValue(ref ids, false);
                            if (values[0] < 3)
                                sub = ids[0];
                        }
                    }

                    if (sub > -1)
                    {
                        foreach (Player p in targets)
                        {
                            if (room.Current == p && ai.HasSkill("zishu", p))
                            {
                                use.Card = new WrappedCard(CongjianCard.ClassName) { Skill = Name };
                                use.Card.AddSubCard(sub);
                                use.To.Add(p);
                                return use;
                            }
                        }

                        foreach (Player p in targets)
                        {
                            if (ai.IsFriend(room.Current) && ai.HasSkill("qingjian", p))
                            {
                                use.Card = new WrappedCard(CongjianCard.ClassName) { Skill = Name };
                                use.Card.AddSubCard(sub);
                                use.To.Add(p);
                                return use;
                            }
                        }

                        foreach (Player p in targets)
                        {
                            if (!ai.HasSkill("zishu", p))
                            {
                                use.Card = new WrappedCard(CongjianCard.ClassName) { Skill = Name };
                                use.Card.AddSubCard(sub);
                                use.To.Add(p);
                                return use;
                            }
                        }
                    }
                    else
                    {
                        KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, targets, Place.PlaceHand);
                        if (pair.Key != null)
                        {
                            use.Card = new WrappedCard(CongjianCard.ClassName) { Skill = Name };
                            use.Card.AddSubCard(pair.Value);
                            use.To.Add(pair.Key);
                            return use;
                        }
                    }
                }
            }

            return use;
        }
    }

    public class CongjianCardAI : UseCard
    {
        public CongjianCardAI() : base(CongjianCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }
    }

    public class ZuilunAI : SkillEvent
    {
        public ZuilunAI() : base("zuilun")
        {
            key = new List<string> { "playerChosen:zuilun" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2], true);

                    if (ai.GetPlayerTendency(target) != "unknown" && (!ai.HasSkill("zhaxiang", target) || target.Hp <= 1))
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player zhuge, object data)
        {
            Room room = ai.Room;
            int count = 0;
            if (zhuge.HasFlag("zuilun_damage")) count++;
            if (!zhuge.HasFlag("zuilun_discard")) count++;
            int hand = 1000;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < hand) hand = p.HandcardNum;
            if (zhuge.HandcardNum == hand) count++;

            if (count > 0) return true;
            if (zhuge.Hp > 2)
            {
                foreach (Player p in room.GetOtherPlayers(zhuge))
                {
                    if (ai.IsFriend(p) && ai.HasSkill("zhaxiang", p) && p.Hp > 1) return true;
                    if (ai.IsEnemy(p) && p.Hp == 1 && !ai.HasSkill("buqu|buqu_jx", p)) return true;
                }
            }

            return false;
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            AskForMoveCardsStruct move = ai.GuanxingForNext(new List<int>(ups));
            if (move.Bottom.Count == min)
            {
                return move;
            }
            else if (move.Top.Count > 0)
            {
                if (move.Bottom.Count < min)
                {
                    while (move.Bottom.Count < min)
                    {
                        int id = move.Top[move.Top.Count - 1];
                        move.Top.Remove(id);
                        move.Bottom.Add(id);
                    }
                }
                else
                {
                    List<int> down = move.Bottom;
                    ai.SortByKeepValue(ref down, false);
                    while (move.Bottom.Count > min)
                    {
                        int id = down[0];
                        down.Remove(id);
                        move.Bottom.Remove(id);
                        move.Top.Add(id);
                    }
                }
            }
            else
            {
                move.Top.Clear();
                move.Bottom.Clear();
                ai.SortByKeepValue(ref ups);
                for (int i = 0; i < ups.Count; i++)
                {
                    if (i < min)
                        move.Bottom.Add(ups[i]);
                    else
                        move.Top.Add(ups[i]);
                }
            }

            Debug.Assert(move.Bottom.Count == min);
            return move;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies);
                foreach (Player p in enemies)
                    if (p.Hp == 1) return new List<Player> { p };

                foreach (Player p in enemies)
                    if (!ai.HasSkill("zhaxiang", p)) return new List<Player> { p };
            }

            return new List<Player> { targets[0] };
        }
    }

    public class JuzhanAI : SkillEvent
    {
        public JuzhanAI() : base("juzhan")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            ai.Number[Name] = -1;

            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use)
            {
                if (use.To.Contains(player)) return true;

                if (use.From == player && data is Player target)
                {
                    if (ai.IsEnemy(target))
                    {
                        if (target.GetArmor() && (!ai.IsCardEffect(use.Card, target, player) || target.HasArmor(EightDiagram.ClassName)) && RoomLogic.CanGetCard(room, player, target, target.Armor.Key))
                        {
                            ai.Number[Name] = target.Armor.Key;
                            return true;
                        }

                        if (!ai.IsLackCard(target, Jink.ClassName))
                            return true;
                    }
                }
            }

            return false;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            if (ai.Number.ContainsKey(Name) && ai.Number[Name] > -1)
                return new List<int> { (int)ai.Number[Name] };
            else
                return base.OnCardsChosen(ai, from, to, flags, min, max, disable_ids);
        }
    }

    public class FeijunAI : SkillEvent
    {
        public FeijunAI() : base("feijun")
        {
            key = new List<string> { "playerChosen:feijun" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choice.StartsWith("playerChosen") && choices[1] == Name)
                {
                    Player target = room.FindPlayer(choices[2]);
                    bool friendly = false;
                    foreach (int id in target.GetEquips())
                    {
                        if (ai.GetKeepValue(id, target, Place.PlaceEquip) < 0)
                        {
                            friendly = true;
                            break;
                        }
                    }

                    ai.UpdatePlayerRelation(player, target, friendly);
                }
            }
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(FeijunCard.ClassName) && !player.IsNude())
                return new List<WrappedCard> { new WrappedCard(FeijunCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class FeijunCardAI : UseCard
    {
        public FeijunCardAI() : base(FeijunCard.ClassName)
        {}

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            int sub = LijianAI.FindLijianCard(ai, player);
            if (sub > -1)
            {
                int hands = player.HandcardNum;
                int equips = player.GetEquips().Count;
                Room room = ai.Room;
                if (room.GetCardPlace(sub) == Place.PlaceHand) hands--;
                else equips--;

                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.HandcardNum < hands || p.GetEquips().Count < equips)
                        targets.Add(p);
                }

                ai.SortByDefense(ref targets, false);
                string mark = string.Format("feijun_{0}", player.Name);
                foreach (Player p in targets)
                {
                    if (p.GetMark(mark) == 0)
                    {
                        bool lose = false;
                        foreach (int id in p.GetEquips())
                        {
                            if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                            {
                                lose = true;
                                break;
                            }
                        }

                        if (lose && ai.IsFriend(p))
                        {
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                        else if (!lose && ai.IsEnemy(p))
                        {
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }
                foreach (Player p in targets)
                {
                    bool lose = false;
                    foreach (int id in p.GetEquips())
                    {
                        if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                        {
                            lose = true;
                            break;
                        }
                    }

                    if (lose && ai.IsFriend(p))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                    else if (!lose && ai.IsEnemy(p))
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
            return 3;
        }
    }

    public class WanglieAI : SkillEvent
    {
        public WanglieAI() : base("wanglie") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardUseStruct use)
            {
                if (use.Card.Name == FireAttack.ClassName || use.Card.Name == IronChain.ClassName || use.Card.Name == GodSalvation.ClassName
                    || use.Card.Name == AmazingGrace.ClassName || use.Card.Name.Contains(Nullification.ClassName))
                    return false;

                Room room = ai.Room;
                Player target = use.To[0];
                if (ai.NotSlashJiaozhu(use.From, target, use.Card)) return true;

                bool will_use = false;
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard.IsAvailable(room, player, card))
                    {
                        CardUseStruct dummy_use = new CardUseStruct(null, player, new List<Player>())
                        {
                            IsDummy = true
                        };
                        UseCard e = Engine.GetCardUsage(card.Name);
                        e.Use(ai, player, ref dummy_use, card);
                        if (use.Card != null)
                        {
                            will_use = true;
                            break;
                        }
                    }
                }
                if (use.Card.Name == Duel.ClassName && ai.IsEnemy(target) && (!ai.IsLackCard(target, Jink.ClassName)
                    || (ai.HasArmorEffect(target, EightDiagram.ClassName) && !target.HasWeapon(QinggangSword.ClassName) && !target.HasWeapon(Saber.ClassName))) && target.Hp <= 1 + use.Drank)
                    return true;
                if (use.Card.Name == Duel.ClassName && ai.IsEnemy(target) && (target.Hp == 1 || target.HandcardNum > 2))
                    return true;
                if (will_use) return false;
                if (use.Card.Name == SavageAssault.ClassName || use.Card.Name == ArcheryAttack.ClassName)
                {
                    foreach (Player p in use.To)
                    {
                        DamageStruct damage = new DamageStruct(use.Card, player, p, 1 + use.ExDamage);
                        if (ai.GetDamageScore(damage).Score < -6)
                            return false;
                    }

                    foreach (Player p in use.To)
                    {
                        DamageStruct damage = new DamageStruct(use.Card, player, p, 1 + use.ExDamage);
                        if (ai.IsEnemy(p) && ai.IsCardEffect(use.Card, p, player) && ai.GetDamageScore(damage).Score > 6)
                            return true;
                    }

                    return false;
                }

                return true;
            }

            return false;
        }
    }


    public class LiangyinAI : SkillEvent
    {
        public LiangyinAI() : base("liangyin")
        {
            key = new List<string> { "playerChosen:liangyin" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choice.StartsWith("playerChosen") && choices[1] == Name)
                {
                    Player target = room.FindPlayer(choices[2]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        if (target.HandcardNum > player.HandcardNum)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            bool friend = false;
                            if (target.HasEquip())
                            {
                                foreach (int id in target.GetEquips())
                                {
                                    if (ai.GetKeepValue(id, target) < 0)
                                    {
                                        friend = true;
                                        break;
                                    }
                                }
                            }
                            ai.UpdatePlayerRelation(player, target, friend);
                        }
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            bool draw = targets[0].HandcardNum > player.HandcardNum;
            if (draw)
            {
                Room room = ai.Room;
                Player current = room.Current;
                ai.SortByDefense(ref targets, false);
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p) && ai.HasSkill("zishu", p) && p == current)
                        return new List<Player> { p };
                }
                if (current != null && ai.IsFriend(current))
                {
                    foreach (Player p in targets)
                    {
                        if (ai.IsFriend(p) && ai.HasSkill("qingjian", p))
                            return new List<Player> { p };
                    }
                }
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p))
                        return new List<Player> { p };
                }
            }
            else
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in targets)
                {
                    if (!ai.IsFriend(p) && p.HasEquip())
                    {
                        bool skip = false;
                        foreach (int id in p.GetEquips())
                        {
                            if (ai.GetKeepValue(id, p) < 0)
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (skip) continue;
                    }
                    ScoreStruct score = ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodDiscard);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }
                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0)
                    {
                        return scores[0].Players;
                    }
                }
            }

            return new List<Player>();
        }
    }

    public class KongshengAI : SkillEvent
    {
        public KongshengAI(): base("kongsheng"){}
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> result = new List<int>();
            Room room = ai.Room;
            if (pattern.Contains(Name))
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetPile(Name))
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, player, card))
                        ids.Add(id);
                }

                if (player.IsWounded())
                    foreach (int id in ids)
                        if (room.GetCard(id).Name == SilverLion.ClassName)
                            return new List<int> { id };

                ai.SortByUseValue(ref ids, false);
                return new List<int> { ids[0] };
            }
            else
            {
                List<Player> friends = ai.FriendNoSelf;
                int count = 0, self = player.HandcardNum;
                if (!ai.WillSkipDrawPhase(player)) self += 2;
                if (friends.Count > 0)
                {
                    foreach (Player p in friends)
                    {
                        if (p.HandcardNum > count)
                            count = p.HandcardNum;
                    }
                }

                List<int> ids = player.GetCards("he");
                if (ids.Count > 0)
                {
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (values[0] < 0)
                        result.Add(ids[0]);

                    if (ai.WillSkipPlayPhase(player))
                    {
                        foreach (int id in player.GetCards("h"))
                            if (!result.Contains(id) && room.GetCard(id).Name != Nullification.ClassName)
                                result.Add(id);

                        Player zhongyao = RoomLogic.FindPlayerBySkillName(room, "zuoding");
                        if (zhongyao != null && ai.IsFriend(zhongyao))
                        {
                            foreach (int id in player.GetCards("e"))
                                if (!result.Contains(id) && room.GetCard(id).Suit == WrappedCard.CardSuit.Spade)
                                    result.Add(id);
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            CardUseStruct slash_use = new CardUseStruct(null, player, new List<Player>());
                            ai.FindSlashandTarget(ref slash_use, player);
                            foreach (int id in player.GetCards("h"))
                            {
                                if (result.Contains(id)) continue;
                                if (room.GetCard(id).Name == Jink.ClassName)
                                {
                                    result.Add(id);
                                    continue;
                                }
                                if (!player.IsWounded() && room.GetCard(id).Name == Peach.ClassName)
                                {
                                    result.Add(id);
                                    continue;
                                }
                                if (!RoomLogic.PlayerContainsTrick(room, player, SupplyShortage.ClassName) && room.GetCard(id).Name == Nullification.ClassName)
                                {
                                    result.Add(id);
                                    continue;
                                }
                                WrappedCard card = room.GetCard(id);
                                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                                if (card.Name == Lightning.ClassName)
                                {
                                    result.Add(id);
                                    continue;
                                }
                                if (fcard is Slash && !ai.HasCrossbowEffect(player) && (slash_use.Card == null || slash_use.Card.GetEffectiveId() != id))
                                {
                                    result.Add(id);
                                    continue;
                                }
                                if (fcard is EquipCard)
                                {
                                    if (fcard is Armor || fcard is DefensiveHorse)
                                    {
                                        result.Add(id);
                                        if (fcard is Armor && player.GetArmor() && !result.Contains(player.Armor.Key))
                                            result.Add(player.Armor.Key);
                                        continue;
                                    }
                                    if (fcard is OffensiveHorse && player.GetOffensiveHorse())
                                    {
                                        result.Add(id);
                                        continue;
                                    }
                                    if (fcard is Weapon && (ai.GetUseValue(id, player) < 1 || player.GetWeapon()))
                                    {
                                        result.Add(id);
                                        continue;
                                    }
                                }
                                else
                                {
                                    CardUseStruct _use = new CardUseStruct(null, player, new List<Player>());
                                    UseCard e = Engine.GetCardUsage(card.Name);
                                    if (e != null)
                                    {
                                        e.Use(ai, player, ref _use, card);
                                        if (_use.Card == null) result.Add(id);
                                    }
                                }
                            }
                        }

                        Player zhongyao = RoomLogic.FindPlayerBySkillName(room, "zuoding");
                        if (zhongyao != null && ai.IsFriend(zhongyao) && player.GetDefensiveHorse() && room.GetCard(player.DefensiveHorse.Key).Suit == WrappedCard.CardSuit.Spade
                            && !result.Contains(player.DefensiveHorse.Key))
                        {
                            result.Add(player.DefensiveHorse.Key);
                        }
                    }
                }
            }

            return result;
        }
    }

    public class JueyanAI : SkillEvent
    {
        public JueyanAI() : base("jueyan") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(JueyanCard.ClassName))
            {
                for (int i = 0; i < 5; i++)
                    if (!player.EquipIsBaned(i))
                        return new List<WrappedCard> { new WrappedCard(JueyanCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return ai.Choice[JueyanCard.ClassName];
        }
    }

    public class JueyanCardAI : UseCard
    {
        public JueyanCardAI() : base(JueyanCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 12;
            Room room = ai.Room;
            if (!player.EquipIsBaned(1) && ai.GetOverflow(player) == 0)
            {
                if (player.IsWounded() && ai.GetKnownCardsNums(SilverLion.ClassName, "he", player) > 0 && !player.HasArmor(SilverLion.ClassName))
                    return;

                use.Card = card;
                ai.Choice[Name] = "Armor";
                return;
            }

            if (!player.EquipIsBaned(0) && ai.GetKnownCardsNums(Slash.ClassName, "h", player) >= 2)
            {
                List<WrappedCard> cards = ai.GetCards(Slash.ClassName, player);
                int count = 0;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (RoomLogic.DistanceTo(room, player, p) == 1 && ai.IsEnemy(p))
                    {
                        foreach (WrappedCard slash in cards)
                        {
                            if (!ai.IsCancelTarget(slash, p, player) && ai.IsCardEffect(slash, p, player) && RoomLogic.IsProhibited(room, player, p, slash) == null)
                            {
                                count++;
                            }
                        }
                    }
                }
                if (count >= 2)
                {
                    use.Card = card;
                    ai.Choice[Name] = "Weapon";
                    return;
                }
            }

            if (!player.EquipIsBaned(4) && ai.GetKnownCardsNums("Trick", "h", player) >= 2 && !player.HasTreasure(ClassicWoodenOx.ClassName))
            {
                use.Card = card;
                ai.Choice[Name] = "Treasure";
                return;
            }

            if (!player.EquipIsBaned(2) || !player.EquipIsBaned(3))
            {
                if (player.GetMark("poshi") == 0 && player.EquipIsBaned(0) && player.EquipIsBaned(1) && player.EquipIsBaned(4))
                {
                    use.Card = card;
                    ai.Choice[Name] = "Horse";
                    return;
                }
                if (ai.GetKnownCardsNums("Snatch", "h", player) + ai.GetKnownCardsNums("Slash", "h", player) + ai.GetKnownCardsNums(SupplyShortage.ClassName, "h", player) > 2)
                {
                    use.Card = card;
                    ai.Choice[Name] = "Horse";
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class HuairouAI : SkillEvent
    {
        public HuairouAI() : base("huairou") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room; 
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is EquipCard equip && player.EquipIsBaned((int)equip.EquipLocation()))
                {
                    WrappedCard hr = new WrappedCard(HuairouCard.ClassName) { Skill = Name };
                    hr.AddSubCard(id);
                    return new List<WrappedCard> { hr };
                }
            }

            foreach (int id in player.GetCards("e"))
            {
                if (ai.GetKeepValue(id, player) < 0)
                {
                    WrappedCard hr = new WrappedCard(HuairouCard.ClassName) { Skill = Name };
                    hr.AddSubCard(id);
                    return new List<WrappedCard> { hr };
                }
            }

            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is EquipCard equip && ai.GetUseValue(id, player) < 1)
                {
                    WrappedCard hr = new WrappedCard(HuairouCard.ClassName) { Skill = Name };
                    hr.AddSubCard(id);
                    return new List<WrappedCard> { hr };
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class HuairouCardAI : UseCard
    {
        public HuairouCardAI() : base(HuairouCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 10;
        }
    }
    
    public class YiliAI : SkillEvent
    {
        public YiliAI() : base("yili")
        {
            key = new List<string> { "playerChosen:yili" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2], true);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            int count = ai.GetKnownCardsNums(Peach.ClassName, "he", player);
            if (player.GetMark("@tangerine") > 1 || player.Hp + count > 2)
            {
                List<Player> friends = ai.FriendNoSelf;
                if (friends.Count > 0)
                {
                    ai.SortByDefense(ref friends, false);
                    foreach (Player p in friends)
                        if (p.GetMark("@tangerine") == 0 && !ai.WillSkipDrawPhase(p) && !ai.WillSkipPlayPhase(p))
                            return new List<Player> { p };

                    foreach (Player p in friends)
                        if (p.GetMark("@tangerine") == 0 && !ai.WillSkipPlayPhase(p))
                            return new List<Player> { p };

                    foreach (Player p in friends)
                        if (p.GetMark("@tangerine") == 0)
                            return new List<Player> { p };

                    if (player.Hp + count > 3)
                        return new List<Player> { friends[0] };
                }
            }

            return new List<Player>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            int count = ai.GetKnownCardsNums(Peach.ClassName, "he", player);
            if (count + player.Hp > 3 || player.GetMark("@tangerine") == 1) return "losehp";

            return "remove";
        }
    }

    public class ZhenglunAI : SkillEvent
    {
        public ZhenglunAI() : base("zhenglun") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class KuizhuAI : SkillEvent
    {
        public KuizhuAI() : base("kuizhu")
        {

        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            int count = player.GetMark(Name);
            Room room = ai.Room;
            List<Player> targets = new List<Player>(), friends = ai.GetFriends(player), enemies = ai.GetEnemies(player);

            ai.SortByDefense(ref friends, false);
            ai.SortByDefense(ref enemies, false);

            List<List<Player>> uses = new List<List<Player>>(), available = new List<List<Player>>(); ;
            for (int i = 1; i <= enemies.Count; i++)
            {
                List<List<Player>> players = TrustedAI.GetCombinationList(new List<Player>(enemies), i);
                uses.AddRange(players);
            }
            
            foreach (List<Player> combine in uses)
            {
                int hp = 0;
                foreach (Player p in combine)
                {
                    hp += p.Hp;
                }

                if (hp == count)
                    available.Add(combine);
            }

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (List<Player> combine in available)
            {
                double value = 0;
                foreach (Player p in combine)
                    value += ai.GetDamageScore(new DamageStruct(Name, player, p)).Score;

                ScoreStruct score = new ScoreStruct
                {
                    Score = value,
                    Players = combine
                };
                scores.Add(score);
            }
            
            if (player.GetMark("@rob") > 0)
            {
                Player gn = RoomLogic.FindPlayerBySkillName(ai.Room, "jieying_gn");
                if (gn != null && !ai.IsFriend(gn)) friends.Remove(player);
            }
            for (int i = 0; i < Math.Min(count, friends.Count); i++)
                targets.Add(friends[i]);

            ScoreStruct friend = new ScoreStruct
            {
                Score = 1.5 * targets.Count,
                Players = targets
            };
            scores.Add(friend);

            if (scores.Count > 1)
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });

            if (scores[0].Score > 0)
            {
                use.Card = new WrappedCard(KuizhuCard.ClassName) { Skill = Name };
                use.To = scores[0].Players;
            }

            return use;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is List<Player> targets)
            {
                if (targets.Contains(player)) return "draw";
                foreach (Player p in targets)
                    if (ai.IsFriend(p)) return "draw";
            }

            return "damage";
        }
    }

    public class ChezhengAI : SkillEvent
    {
        public ChezhengAI() : base("chezheng")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodDiscard).Score > 0)
                    return new List<Player> { p };
            }

            foreach (Player p in targets)
            {
                if (ai.IsEnemy(p) && ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodDiscard).Score > 0)
                    return new List<Player> { p };
            }

            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p))
                    return new List<Player> { p };
            }

            Player target = null;
            int count = 0;
            foreach (Player p in targets)
            {
                if (p.GetCardCount(true) > count)
                {
                    count = p.GetCardCount(true);
                    target = p;
                }
            }

            return new List<Player> { target };
        }
    }

    public class LijunAI : SkillEvent
    {
        public LijunAI() : base("lijun")
        {
            key = new List<string> { "skillInvoke:lijun:yes" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && ai.Self != player && !ai.HasSkill(Name, player))
            {
                Player target = RoomLogic.FindPlayerBySkillName(room, Name);
                ai.UpdatePlayerRelation(player, target, true);
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsFriend(target))
                return true;
            else if (data is string prompt)
            {
                string[] strs = prompt.Split(':');
                Player who = ai.Room.FindPlayer(strs[1]);
                if (ai.GetPlayerTendency(who) != "rebel")
                    return true;
            }

            return false;
        }
    }
}