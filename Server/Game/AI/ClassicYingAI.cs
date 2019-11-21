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

                new ChenglueAI(),
                new ShicaiJXAI(),
                new TusheAI(),
                new LimuAI(),

                new ZuilunAI(),
                new JuzhanAI(),
                new FeijunAI(),

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
                if (!RoomLogic.CanDiscard(room, player, p, "hej")) continue;
                ScoreStruct score = ai.FindCards2Discard(player, p, Name, "hej", HandlingMethod.MethodDiscard);
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
    }

    public class LimuAI : SkillEvent
    {
        public LimuAI() : base("limu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            if (!RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName)
                && ((player.HasWeapon(Spear.ClassName) && !RoomLogic.PlayerContainsTrick(room, player, Lightning.ClassName))
                || player.Hp == 1 || (ai.GetOverflow(player) > 0 && player.IsWounded())))
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

                    ai.SortByUseValue(ref ids, false);
                    WrappedCard _lm = new WrappedCard(LimuCard.ClassName);
                    _lm.AddSubCard(ids[0]);
                    _lm.Skill = Name;
                    return new List<WrappedCard> { _lm };
                }
            }

            return new List<WrappedCard>();
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

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && ai.Self != player && data is CardUseStruct use)
            {
                Player target = use.To[0];
                bool friendly = false;
                foreach (int id in target.GetEquips())
                {
                    if (ai.GetKeepValue(id, player, Place.PlaceEquip) < 0)
                    {
                        friendly = true;
                        break;
                    }
                }

                ai.UpdatePlayerRelation(player, target, friendly);
            }
        }

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
                            if (ai.GetKeepValue(id, player, Place.PlaceEquip) < 0)
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
                        if (ai.GetKeepValue(id, player, Place.PlaceEquip) < 0)
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


    public class LiangyinAI : SkillEvent
    {
        public LiangyinAI() : base("liangyin")
        {
            key = new List<string> { "playerChosen:liangyin", "cardChosen:liangyin" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choice.StartsWith("playerChosen") && choices[1] == Name)
                {
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown" && target.HandcardNum > player.HandcardNum)
                        ai.UpdatePlayerRelation(player, target, true);
                }
                else if (choice.StartsWith("cardChosen") && choices[1] == Name)
                {
                    int card_id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceHand && target.HandcardNum > 1)
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

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            bool draw = targets[0].HandcardNum > player.HandcardNum;
            if (draw)
            {
                ai.SortByDefense(ref targets, false);
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
                    ScoreStruct score = ai.FindCards2Discard(player, p, Name, "hej", FunctionCard.HandlingMethod.MethodDiscard);
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
                        foreach (int id in player.GetCards("h"))
                        {
                            if (!result.Contains(id) && room.GetCard(id).Name == Jink.ClassName)
                                result.Add(id);
                            if (!result.Contains(id) && !player.IsWounded() && room.GetCard(id).Name == Peach.ClassName)
                                result.Add(id);
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
                if (ai.GetUseValue(id, player) < 1)
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