using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class StanderWeiAI : AIPackage
    {
        public StanderWeiAI() : base("Stander-wei")
        {
            events = new List<SkillEvent>
            {
                //wei
                new JianxiongAI(),
                new GuicaiAI(),
                new FankuiAI(),
                new GanglieAI(),
                new TuxiAI(),
                new LuoyiAI(),
                new TianduAI(),
                new YijiAI(),
                new LuoshenAI(),
                new QingguoAI(),
                new ShensuAI(),
                new QiaobianAI(),
                new DuanliangAI(),
                new JushouAI(),
                new QiangxiAI(),
                new QuhuAI(),
                new JiemingAI(),
                new XingshangAI(),
                new FangzhuAI(),
                new XiaoguoAI()
            };

            use_cards = new List<UseCard>
            {
                //wei
                new QiangxiCardAI(),
                new QuhuCardAI(),
            };
        }
    }

    public class JianxiongAI : SkillEvent
    {
        public JianxiongAI() : base("jianxiong")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForMasochism()) return false;

            return !ai.NeedKongcheng(player);
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };

            if (damage.To != null && ai.HasSkill(Name, damage.To) && damage.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (!(fcard is SkillCard))
                {
                    foreach (int id in damage.Card.SubCards)
                    {
                        double value = Math.Max(ai.GetUseValue(id, damage.To, Player.Place.PlaceHand), ai.GetKeepValue(id, damage.To, Player.Place.PlaceHand));
                        score.Score += value;
                    }

                    if (ai.WillSkipPlayPhase(damage.To))
                        score.Score /= 3;

                    if (damage.Damage >= damage.To.Hp)
                        score.Score /= 2;
                }

                if (ai.IsEnemy(damage.To))
                    score.Score = -score.Score;
            }

            return score;
        }
    }

    public class FankuiAI : SkillEvent
    {
        public FankuiAI() : base("fankui")
        {
            key = new List<string> { "cardChosen" };
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is DamageStruct damage)
            {
                Player to = damage.To;
                List<int> disable = new List<int>();
                foreach (int id in player.GetCards("he"))
                {
                    if (!RoomLogic.CanGetCard(ai.Room, player, to, id))
                        disable.Add(id);
                }

                if (ai.FindCards2Discard(player, damage.From, "he", FunctionCard.HandlingMethod.MethodGet, 1, true, disable).Score > 0)
                    return true;
            }

            return false;
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (damage.To != null && damage.From != null && !damage.From.IsNude() && damage.Damage <= damage.To.Hp)
            {
                if (ai.IsFriend(damage.From, damage.To) && damage.From.HasEquip() && damage.From.IsWounded())
                {
                    if (ai.HasArmorEffect(damage.From, "SilverLion")) score.Score += 2;
                }
                else if (!ai.IsFriend(damage.From, damage.To))
                {
                    score.Score += 1;
                    if (damage.From.HasWeapon("CrossBow"))
                        score.Score += 5;
                }

                if (damage.Damage == damage.To.Hp)
                    score.Score /= 4;

                if (ai.IsEnemy(damage.To))
                {
                    score.Score = -score.Score;
                    if (ai.IsFriend(damage.From))
                        score.Score -= 1;
                }
            }
            return score;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    int id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);

                    if (ai.GetPlayerTendency(target) == "unknown" && ai.HasArmorEffect(target, "SilverLion") && id == target.Armor.Key)
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        //public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        //{
        //    return ai.FindCards2Discard(from, to, "he", FunctionCard.HandlingMethod.MethodGet, 1, true, disable_ids).Ids;
        //}
    }

    public class GuicaiAI : SkillEvent
    {
        public GuicaiAI() : base("guicai")
        {
            key = new List<string> { "cardResponded" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && ai.Self != player)
            {
                Room room = ai.Room;
                string[] choices = choice.Split(':');
                if (choices[2] == Name && room.GetTag(Name) is JudgeStruct judge && ai.GetPlayerTendency(judge.Who) == "unknown" && choices[4] != "_nil_")
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
                    else if (judge.Reason == "Lightning")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Spade && judge.Card.Number > 1 && judge.Card.Number <= 9
                            && (suit != WrappedCard.CardSuit.Spade || number > 9 || number == 1))
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                        else if (suit == WrappedCard.CardSuit.Spade && number > 1 && number <= 9)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                    }
                    else if (judge.Reason == "SupplyShortage")
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
                    else if (judge.Reason == "Indulgence")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Heart && suit != WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (suit == WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "EightDiagram")
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
                }
            }
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && place != Player.Place.PlaceEquip && !isUse)
            {
                WrappedCard.CardSuit suit = RoomLogic.GetCardSuit(ai.Room, card);
                if (suit == WrappedCard.CardSuit.Heart)
                    return 1;
                else if (suit == WrappedCard.CardSuit.Club)
                    return 0.5;
                else
                    return 0.2;
            }
            return 0;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            if (data is JudgeStruct judge)
            {
                int id = ai.GetRetrialCardId(player.GetCards("h"), judge);
                if (id >= 0)
                    use.Card = ai.Room.GetCard(id);
            }

            return use;
        }
    }

    public class GanglieAI : SkillEvent
    {
        public GanglieAI() : base("ganglie")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForMasochism()) return false;

            if (data is DamageStruct damage)
            {
                Player target = damage.From;
                if (target == player || (damage.From == null && !ai.HasSkill("tiandu", player)))
                    return false;
                else if (ai.IsFriend(target))
                {
                    if (target.Hp > 1)
                    {
                        double value = 0;
                        if (ai.HasSkill("jieming", target))
                        {
                            double max = 0;
                            foreach (Player p in ai.GetFriends(player))
                            {
                                int count = p.MaxHp - p.HandcardNum;
                                if (count > max)
                                    max = count;
                            }
                            value += max * 0.6;
                        }

                        if (ai.HasSkill("yiji", target))
                        {
                            value += 1.5;
                        }

                        if (ai.HasSkill("fangzhu", target))
                        {
                            value += 2;
                        }

                        if (ai.HasSkill("fankui", target) && player.HasArmor("SilverLion"))
                        {
                            value += 0.5;
                        }
                        if (ai.HasSkill("wangxi", target))
                        {
                            value += 1.5;
                        }

                        if (value > 3) return true;
                    }

                    return false;
                }
                else if (!ai.IsFriend(target))
                {
                    if (target.Hp > 1)
                    {
                        double value = 0;
                        if (ai.HasSkill("jieming", target))
                        {
                            double max = 0;
                            foreach (Player p in ai.GetFriends(target))
                            {
                                int count = p.MaxHp - p.HandcardNum;
                                if (count > max)
                                    max = count;
                            }
                            value += max * 0.6;
                        }

                        if (ai.HasSkill("yiji", target))
                        {
                            value += 1.5;
                        }

                        if (ai.HasSkill("fangzhu", target))
                        {
                            value += 4;
                        }

                        if (ai.HasSkill("fankui", target) && player.HasWeapon("CrossBow"))
                        {
                            value += 3;
                        }

                        if (ai.HasSkill("ganglie", target))
                        {
                            value += 0.5;
                        }

                        if (value >= 3) return false;
                    }
                }
            }

            return true;
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip)
        {
            List<int> result = new List<int>();
            Room room = ai.Room;
            DamageStruct current = (DamageStruct)room.GetTag("CurrentDamageStruct");
            Player xiahou = current.To;
            DamageStruct damage = new DamageStruct
            {
                From = xiahou,
                To = player
            };

            if (ai.DamageEffect(damage, DamageStruct.DamageStep.None) == 0)
                return result;
            else
            {
                bool invoke = false;
                if (ai.HasSkill("jieming", player))
                {
                    int _max = 0;
                    foreach (Player p in ai.GetFriends(player))
                    {
                        int count = p.MaxHp - p.HandcardNum;
                        if (count > _max)
                            _max = count;
                    }
                    if (_max >= 2)
                        invoke = true;
                }

                if (ai.HasSkill("yiji", player))
                    invoke = true;

                if (ai.HasSkill("fangzhu", player))
                    invoke = true;

                if (ai.HasSkill("fankui", player))
                {
                    if (xiahou.HasArmor("SilverLion") && ai.IsFriend(xiahou))
                        invoke = true;
                    else if (xiahou.HasWeapon("CrossBow") && !ai.IsFriend(xiahou))
                        invoke = true;
                }
                if (ai.HasSkill("wangxi", player) && ai.IsFriend(xiahou))
                    invoke = true;

                if (invoke && (player.Hp > 1 || ai.GetCards("Peach", player).Count > 0 || ai.GetCards("Analeptic", player).Count > 0))
                    return result;
            }

            if (player.Hp == 1 && (ai.HasArmorEffect(player, "BreastPlate") || ai.GetCards("Peach", player).Count > 0 && ai.GetCards("Analeptic", player).Count > 0))
                return result;

            if (room.Current == player)
            {
                if (ai.GetCards("Peach", player).Count > 0) return result;
                if (player.Hp - 1 > player.HandcardNum) return result;
            }

            List<int> ids = new List<int>(player.HandCards);
            if (room.Current == player)
                ai.SortByUseValue(ref ids, false);
            else
                ai.SortByKeepValue(ref ids, false);

            result.Add(ids[0]);
            result.Add(ids[1]);

            return result;
        }
    }

    public class TuxiAI : SkillEvent
    {
        public TuxiAI() : base("tuxi")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (player.HasTreasure("JadeSeal")) return new List<Player>();

            List<Player> result = FindTuxiTargets(ai, player);
            if (result.Count == 1 && result[0].HandcardNum != 1)
                return new List<Player>();

            return result;
        }

        public static List<Player> FindTuxiTargets(TrustedAI ai, Player player)
        {
            List<Player> result = new List<Player>();
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByHandcards(ref enemies, false);
            Room room = ai.Room;
            Player zhugeliang = ai.FindPlayerBySkill("kongcheng");

            if (zhugeliang != null && ai.IsFriend(zhugeliang) && zhugeliang.HandcardNum == 1 && ai.GetEnemisBySeat(zhugeliang) > 0 && zhugeliang != player)
            {
                if (ai.IsWeak(zhugeliang))
                    result.Add(zhugeliang);
                else
                {
                    List<int> ids = ai.GetKnownCards(zhugeliang);
                    if (ids.Count == 1)
                    {
                        int id = ids[0];
                        if (!ai.IsCard(id, "Jink", zhugeliang) && !!ai.IsCard(id, "Peach", zhugeliang) && !ai.IsCard(id, "Analeptic", zhugeliang))
                            result.Add(zhugeliang);
                    }
                }
            }

            foreach (Player p in enemies)
            {
                List<int> ids = ai.GetKnownCards(p);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == "Peach" || card.Name == "Nullification" || card.Name.Contains("Nullification"))
                    {
                        result.Add(p);
                        break;
                    }
                }
                if (result.Count >= 2) break;
            }

            if (result.Count < 2)
            {
                foreach (Player p in enemies)
                {
                    if (ai.HasSkill("jijiu|qingnang|leiji|jieyin|beige|kanpo|liuli|qiaobian|zhiheng|guidao|tianxiang|lijian", p))
                        result.Add(p);

                    if (result.Count >= 2) break;
                }
            }
            if (result.Count < 2)
            {
                foreach (Player p in enemies)
                {
                    int x = p.HandcardNum;
                    bool good = true;
                    if (x == 1 && ai.NeedKongcheng(p))
                        good = false;
                    if (x >= 2 && ai.HasSkill("tuntian", p))
                        good = false;

                    if (good)
                        result.Add(p);

                    if (result.Count >= 2) break;
                }
            }

            return result;
        }
    }

    public class LuoyiAI : SkillEvent
    {
        public LuoyiAI() : base("luoyi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.IsSkipped(Player.PlayerPhase.Play)) return false;

            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByHp(ref enemies, false);

            CardUseStruct use = new CardUseStruct
            {
                To = new List<Player>(),
                IsDummy = true
            };
            ai.FindSlashandTarget(ref use, player);
            if (use.Card != null && use.To.Count > 0)
            {
                if (use.To.Count > 1 || ai.GetCards("Halberg", player).Count > 0 || ai.GetCards("QinggangSword", player).Count > 0
                    || !ai.HasArmorEffect(use.To[0], "SilverLion"))
                    return true;
            }

            List<WrappedCard> duels = ai.GetCards("Duel", player);
            foreach (WrappedCard card in duels)
            {
                foreach (Player p in enemies)
                {
                    if (RoomLogic.IsProhibited(room, player, p, card) == null && ai.GetCards("Slash", player).Count >= ai.GetCards("Slash", p).Count
                        && !ai.HasArmorEffect(p, "SilverLion"))
                        return true;
                }
            }

            return false;
        }
    }

    public class TianduAI : SkillEvent
    {
        public TianduAI() : base("tiandu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack()) return false;
            if (data is JudgeStruct judge)
            {
                if (judge.Reason == "tuntian" && judge.Card.Suit != WrappedCard.CardSuit.Heart)
                {
                    if (ai.IsCard(judge.Card.Id, "Peach", player))
                        return !ai.NeedKongcheng(player) || !player.IsKongcheng();

                    if (ai.IsCard(judge.Card.Id, "Analeptic", player) && ai.IsWeak())
                        return !ai.NeedKongcheng(player) || !player.IsKongcheng();

                    return false;
                }
            }

            return !ai.NeedKongcheng(player) || !player.IsKongcheng();
        }
    }

    public class YijiAI : SkillEvent
    {
        public YijiAI() : base("yiji")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override Player OnYiji(TrustedAI ai, Player player, List<int> ids, ref int id)
        {
            KeyValuePair<Player, int> key = ai.GetCardNeedPlayer(ids);
            if (key.Key != null && key.Value >= 0 && ai.Room.Current == key.Key)
            {
                id = key.Value;
                return key.Key;
            }
            if (player.HandcardNum <= 2)
                return null;

            if (key.Key != null && key.Value >= 0)
            {
                id = key.Value;
                return key.Key;
            }

            return null;
        }
        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };

            if (damage.To != null && ai.HasSkill(Name, damage.To))
            {
                int n = ai.DamageEffect(damage, DamageStruct.DamageStep.Done);
                if (n < damage.To.Hp)
                {
                    score.Score = 4;
                }

                if (ai.IsEnemy(damage.To))
                    score.Score = -score.Score;
            }

            return score;
        }
    }

    public class LuoshenAI : SkillEvent
    {
        public LuoshenAI() : base("luoshen")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack() && !ai.WillShowForDefence()) return false;
            Player erzhang = ai.FindPlayerBySkill("guzheng");
            if (erzhang != null && ai.IsEnemy(erzhang) && ai.WillSkipPlayPhase(player) && !ai.HasSkill("qiaobian"))
            {
                List<int> card_list = player.ContainsTag("luoshen") ? (List<int>)player.GetTag("luoshen") : new List<int>();
                List<int> subcards = new List<int>();
                foreach (int id in card_list)
                    if (ai.Room.GetCardPlace(id) == Player.Place.PlaceTable && !subcards.Contains(id))
                        subcards.Add(id);
                if (RoomLogic.GetMaxCards(ai.Room, player) - player.HandcardNum - subcards.Count < 0)
                    return false;
            }

            return true;
        }
    }

    public class QingguoAI : SkillEvent
    {
        public QingguoAI() : base("qingguo")
        {
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (card != null && WrappedCard.IsBlack(card.Suit) && (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand))
            {
                WrappedCard jink = new WrappedCard("Jink")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                jink.AddSubCard(card);
                jink = RoomLogic.ParseUseCard(room, jink);
                return jink;
            }

            return null;
        }
    }

    public class ShensuAI : SkillEvent
    {
        public ShensuAI() : base("shensu")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                Card = null
            };
            Room room = ai.Room;

            if (prompt.EndsWith("1"))
            {
                if (!ai.WillShowForAttack()) return use;
                if (RoomLogic.PlayerContainsTrick(room, player, "Lightning") && player.JudgingArea.Count == 1)
                {
                    Player wizzard = ai.GetWizzardRaceWinner("Lightning", player);
                    if (wizzard == null || !ai.IsEnemy(wizzard))
                        return use;
                }
                if (!RoomLogic.PlayerContainsTrick(room, player, "Indulgence") && !RoomLogic.PlayerContainsTrick(room, player, "SupplyShortage"))
                {
                    if (player.HasTreasure("JadeSeal") || player.GetPile("yijipile").Count > 0) return use;
                }

                WrappedCard slash = new WrappedCard("Slash")
                {
                    Skill = "_shensu",
                    DistanceLimited = false
                };
                List<WrappedCard> cards = new List<WrappedCard> { slash };
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, cards);
                if (scores.Count > 0)
                {
                    foreach (Player p in scores[0].Players)
                    {
                        if (ai.IsEnemy(p) && scores[0].Score > 4)
                        {
                            if (p.Hp <= 1 && (!RoomLogic.InMyAttackRange(room, player, p) || ai.GetCards("Slash", player).Count == 0 || RoomLogic.PlayerContainsTrick(room, player, "Indulgence")))
                            {
                                use.Card = new WrappedCard("ShensuCard");
                                use.To = scores[0].Players;
                                return use;
                            }
                        }
                    }
                }
                bool Nullification = false;
                foreach (Player p in ai.GetFriends(player))
                {
                    if (ai.GetKnownCardsNums("Nullification", "he", p) > 0)
                    {
                        Nullification = true;
                        break;
                    }
                }

                if (RoomLogic.PlayerContainsTrick(room, player, "Indulgence"))
                {
                    Player wizzard = ai.GetWizzardRaceWinner("Indulgence", player);
                    if (!Nullification && (wizzard == null || ai.IsEnemy(wizzard)))
                    {
                        if (scores.Count > 0 && scores[0].Score > 3)
                        {
                            use.Card = new WrappedCard("ShensuCard");
                            use.To = scores[0].Players;
                            return use;
                        }
                        else
                        {
                            double value = 0;
                            foreach (int id in player.HandCards)
                                value += ai.GetUseValue(id, player);

                            foreach (int id in player.GetHandPile())
                                value += ai.GetUseValue(id, player);

                            if (value > 16 || ai.GetOverflow(player) > 0)
                            {
                                if (scores.Count > 0 && scores[0].Score > 0)
                                {
                                    use.Card = new WrappedCard("ShensuCard");
                                    use.To = scores[0].Players;
                                    return use;
                                }
                                else
                                {
                                    FunctionCard fcard = Engine.GetFunctionCard("Slash");
                                    foreach (Player p in room.GetOtherPlayers(player))
                                    {
                                        if (fcard.TargetFilter(room, new List<Player>(), p, player, slash)
                                            && (!ai.IsFriend(p) || ai.IsCancelTarget(slash, p, player) || !ai.IsCardEffect(slash, p, player)))
                                        {
                                            use.Card = new WrappedCard("ShensuCard");
                                            use.To.Add(p);
                                            return use;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (prompt.EndsWith("2"))
            {
                if (!ai.WillShowForAttack()) return use;
                ((SmartAI)ai).UpdatePlayers();
                double value = 0;
                List<WrappedCard> cards = ai.GetCards("Slash", player, true);
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, cards);
                if (scores.Count > 0 && scores[0].Score > 0)
                {
                    if (ai.GetCards("CrossBow", player).Count > 0 && cards.Count > 0)
                        return use;
                    value = scores[0].Score;
                }

                List<WrappedCard> slashes = new List<WrappedCard>();
                foreach (int id in player.GetCards("he"))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    if (fcard is EquipCard && RoomLogic.CanDiscard(room, player, player, id))
                    {
                        WrappedCard slash = new WrappedCard("Slash")
                        {
                            Skill = "_shensu",
                            DistanceLimited = false,
                        };
                        slash.AddSubCard(id);
                    }
                }
                if (slashes.Count > 0)
                {
                    scores.Clear();
                    scores = ai.CaculateSlashIncome(player, slashes);
                    if (scores.Count > 0)
                    {
                        for (int i = 0; i < scores.Count; i++)
                        {
                            ScoreStruct score = scores[i];
                            WrappedCard equip = room.GetCard(score.Card.SubCards[0]);
                            if (equip.Name == "SilverLion" && player.HasArmor(equip.Name) && player.IsWounded())
                                score.Score += 2.5;
                            else if (player.HasEquip(equip.Name) || ai.GetSameEquip(equip, player) == null)
                                score.Score -= 2;
                        }

                        ai.CompareByScore(ref scores);
                        if (scores[0].Score > value && scores[0].Score > 4)
                        {
                            use.Card = new WrappedCard("ShensuCard");
                            use.Card.AddSubCards(scores[0].Card.SubCards);
                            use.To = scores[0].Players;
                            return use;
                        }
                    }
                }
            }

            return use;
        }
    }

    public class QiaobianAI : SkillEvent
    {
        public QiaobianAI() : base("qiaobian")
        {
        }

        private KeyValuePair<int, Player> CardForQiaobian(TrustedAI ai, Player who)
        {
            int result_id = -1;
            Player player = ai.Self;
            Room room = ai.Room;
            if (ai.IsFriend(who))
            {
                List<int> judges = who.JudgingArea;
                foreach (int id in judges)
                {
                    WrappedCard card = room.GetCard(id);
                    foreach (Player enemy in ai.GetEnemies(player))
                    {
                        if (!RoomLogic.PlayerContainsTrick(room, enemy, card.Name) && !ai.IsCancelTarget(card, enemy, null) && ai.IsCardEffect(card, enemy, null))
                        {
                            return new KeyValuePair<int, Player>(id, enemy);
                        }
                    }
                }

                if (ai.HasSkill(TrustedAI.LoseEquipSkill, who, true) && who.HasEquip())
                {
                    List<int> equips = player.GetEquips();
                    if (who.GetArmor() && ai.GetKeepValue(who.Armor.Key, who) < 0)
                        result_id = who.Armor.Key;
                    if (result_id < 0 && who.GetOffensiveHorse())
                        result_id = who.OffensiveHorse.Key;
                    if (result_id < 0 && who.GetDefensiveHorse() && !ai.IsWeak(who))
                        result_id = who.DefensiveHorse.Key;
                    if (result_id < 0 && who.GetArmor() && !ai.IsWeak(who))
                        result_id = who.Armor.Key;

                    if (result_id >= 0)
                    {
                        WrappedCard equip = room.GetCard(result_id);
                        foreach (Player p in room.GetOtherPlayers(who))
                        {
                            if (ai.GetSameEquip(equip, p) == null && ai.HasSkill(TrustedAI.LoseEquipSkill, who))
                                return new KeyValuePair<int, Player>(result_id, p);
                        }
                        List<Player> friends = ai.GetFriends(player);
                        ai.SortByDefense(ref friends, false);
                        foreach (Player p in room.GetOtherPlayers(who))
                        {
                            if (p != who && ai.GetSameEquip(equip, p) == null)
                                return new KeyValuePair<int, Player>(result_id, p);
                        }
                    }
                }
            }
            else
            {
                if (!who.HasEquip() || (ai.HasSkill(TrustedAI.LoseEquipSkill, who) && !who.GetTreasure())) return new KeyValuePair<int, Player>(-1, null);

                int id = ai.AskForCardChosen(who, "e", "Snatch", FunctionCard.HandlingMethod.MethodNone, new List<int>());
                if (id >= 0 && who.GetEquips().Contains(id))
                {
                    List<Player> friends = ai.GetFriends(player);
                    result_id = id;
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is Armor || fcard is DefensiveHorse)
                    {
                        ai.SortByDefense(ref friends, false);
                        foreach (Player p in friends)
                        {
                            if (ai.GetSameEquip(card, p) == null && (ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill("shensu", p)))
                                return new KeyValuePair<int, Player>(result_id, p);
                        }

                        foreach (Player p in friends)
                        {
                            if (ai.GetSameEquip(card, p) == null)
                                return new KeyValuePair<int, Player>(result_id, p);
                        }
                    }
                    else if (fcard is Treasure || fcard is Weapon)
                    {
                        foreach (Player p in friends)
                        {
                            if (ai.GetSameEquip(card, p) == null && (ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill("shensu", p)))
                                return new KeyValuePair<int, Player>(result_id, p);
                        }

                        foreach (Player p in friends)
                        {
                            if (ai.GetSameEquip(card, p) == null)
                                return new KeyValuePair<int, Player>(result_id, p);
                        }
                    }
                }
            }

            return new KeyValuePair<int, Player>(-1, null);
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip)
        {
            List<int> to_discard = new List<int>();
            if (!ai.WillShowForAttack()) return to_discard;

            ((SmartAI)ai).UpdatePlayers();
            List<int> cards = new List<int>(player.HandCards);
            ai.SortByKeepValue(ref cards, false);
            Room room = ai.Room;
            Player stealer = null;

            foreach (Player ap in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill("tuxi", ap) && ai.IsEnemy(ap))
                {
                    stealer = ap;
                    break;
                }
            }
            int card = -1;
            for (int i = 0; i < cards.Count; i++)
            {
                if (ai.IsCard(cards[i], "Peach", player))
                {
                    if (stealer != null && player.HandcardNum <= 2 && player.Hp > 2 && !RoomLogic.PlayerContainsTrick(room, stealer, "SupplyShortage"))
                    {
                        card = cards[i];
                        break;
                    }
                    bool to_discard_peach = true;
                    foreach (Player fd in ai.GetFriends(player))
                    {
                        if (fd.Hp <= 2 && (!ai.HasSkill("niepan", fd) || fd.GetMark("@nirvana") == 0))
                        {
                            to_discard_peach = false;
                            break;
                        }
                    }
                    if (to_discard_peach)
                    {
                        card = cards[i];
                        break;
                    }
                }
                else
                {
                    card = cards[i];
                    break;
                }
            }

            if (card == -1)
                return to_discard;

            to_discard.Add(card);
            Player.PlayerPhase phase = (Player.PlayerPhase)player.GetMark("qiaobianPhase");

            if (phase == Player.PlayerPhase.Judge && !player.IsSkipped(Player.PlayerPhase.Judge))
            {
                if (RoomLogic.PlayerContainsTrick(room, player, "Lightning"))
                {
                    Player wizzard = ai.GetWizzardRaceWinner("Lightning", player);
                    if ((wizzard != null && ai.IsEnemy(wizzard)) || ai.GetFriends(player).Count > ai.GetEnemies(player).Count)
                        return to_discard;
                }
                else if (RoomLogic.PlayerContainsTrick(room, player, "SupplyShortage"))
                {
                    if (player.Hp > player.HandcardNum) return to_discard;
                    List<Player> targets = TuxiAI.FindTuxiTargets(ai, player);
                    if (targets.Count == 2) return to_discard;
                }
                else if (RoomLogic.PlayerContainsTrick(room, player, "Indulgence"))
                {
                    if (player.HandcardNum > 3 || player.HandcardNum > player.Hp - 1) return to_discard;
                    foreach (Player friend in ai.FriendNoSelf)
                        if (RoomLogic.PlayerContainsTrick(room, player, "Indulgence") || RoomLogic.PlayerContainsTrick(room, player, "SupplyShortage"))
                            return to_discard;
                }
            }
            else if (phase == Player.PlayerPhase.Draw && !player.IsSkipped(Player.PlayerPhase.Draw) && !ai.HasSkill("tuxi"))
            {
                ai.Target["qiaobian1"] = null;
                ai.Target["qiaobian2"] = null;

                if (player.HasTreasure("JadeSeal") || player.GetPile("yijipile").Count > 0) return new List<int>();
                List<Player> targets = TuxiAI.FindTuxiTargets(ai, player);
                if (targets.Count == 2)
                {
                    ai.Target["qiaobian1"] = targets[0];
                    ai.Target["qiaobian2"] = targets[1];
                    return to_discard;
                }
            }
            else if (phase == Player.PlayerPhase.Play && !player.IsSkipped(Player.PlayerPhase.Play))
            {
                cards = new List<int>(player.HandCards);
                ai.SortByKeepValue(ref cards, false);
                to_discard = new List<int>(cards[0]);

                foreach (Player friend in ai.GetFriends(player))
                {
                    if (friend.JudgingArea.Count > 0 && CardForQiaobian(ai, friend).Key >= 0)
                        return to_discard;
                }

                foreach (Player friend in ai.FriendNoSelf)
                {
                    if (friend.HasEquip() && ai.HasSkill(TrustedAI.LoseEquipSkill, friend) && CardForQiaobian(ai, friend).Key >= 0)
                        return to_discard;
                }


                double top_value = 0;
                foreach (int id in cards)
                {
                    if (!ai.IsCard(id, "Jink", player))
                    {
                        double value = ai.GetUseValue(id, player);
                        if (value > top_value)
                            top_value = value;
                    }
                }

                if (top_value >= 3.7 && ai.GetTurnUse().Count > 0) return new List<int>();
                foreach (Player p in ai.GetEnemies(player))
                {
                    if (CardForQiaobian(ai, p).Key > 0)
                        return to_discard;
                }
            }
            else if (phase == Player.PlayerPhase.Discard && !player.IsSkipped(Player.PlayerPhase.Discard) && player.HandcardNum > RoomLogic.GetMaxCards(room, player) + 1)
            {
                player.SetFlags("AI_ConsideringQiaobianSkipDiscard");
                return to_discard;
            }

            return new List<int>();
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Player.PlayerPhase phase = (Player.PlayerPhase)player.GetMark("qiaobianPhase");
            CardUseStruct use = new CardUseStruct { From = player, Card = new WrappedCard("QiaobianCard"), To = new List<Player>() };
            if (prompt == "@qiaobian-2" && ai.Target["qiaobian1"] != null && ai.Target["qiaobian2"] != null)
            {
                use.To.Add(ai.Target["qiaobian1"]);
                use.To.Add(ai.Target["qiaobian2"]);
                return use;
            }
            else
            {
                foreach (Player friend in ai.GetFriends(player))
                {
                    if (friend.JudgingArea.Count > 0 && CardForQiaobian(ai, friend).Key >= 0)
                    {
                        use.To.Add(friend);
                        return use;
                    }
                }
                foreach (Player friend in ai.FriendNoSelf)
                {
                    if (friend.HasEquip() && ai.HasSkill(TrustedAI.LoseEquipSkill, friend) && CardForQiaobian(ai, friend).Key >= 0)
                    {
                        use.To.Add(friend);
                        return use;
                    }
                }

                List<Player> enemies = ai.GetEnemies(player);
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                {
                    if (CardForQiaobian(ai, p).Key >= 0)
                    {
                        use.To.Add(p);
                        return use;
                    }
                }
            }

            return new CardUseStruct();
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            List<int> result = new List<int>();
            if (flags == "ej")
            {
                int id = CardForQiaobian(ai, to).Key;
                if (id >= 0)
                    result.Add(id);
            }

            return null;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            List<Player> result = new List<Player>();
            if (ai.Room.GetTag("QiaobianTarget") != null && ai.Room.GetTag("QiaobianTarget") is Player from)
            {
                Player to = CardForQiaobian(ai, from).Value;
                if (to != null)
                    result.Add(to);
                else
                    ai.Room.OutPut("巧变3 AI出错");
            }

            return result;
        }
    }

    public class DuanliangAI : SkillEvent
    {
        public DuanliangAI() : base("duanliang")
        {
        }
        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            string pattern = "BasicCard,EquipCard|black";
            if (!player.HasFlag(Name) && card != null && Engine.MatchExpPattern(room, pattern, player, card))
            {
                WrappedCard shortage = new WrappedCard("SupplyShortage")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                shortage.AddSubCard(card);
                shortage = RoomLogic.ParseUseCard(room, shortage);
                return shortage;
            }

            return null;
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.WillShowForAttack() || player.HasFlag(Name)) return null;
            Room room = ai.Room;
            List<int> ids = new List<int>(player.HandCards);
            ids.AddRange(player.GetEquips());
            ids.AddRange(player.GetHandPile());
            ai.SortByUseValue(ref ids);
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (WrappedCard.IsBlack(card.Suit) && (fcard is BasicCard || fcard is EquipCard)
                    && ai.GetDynamicUsePriority(card) < Engine.GetCardUseValue("SupplyShortage"))
                {
                    WrappedCard shortage = new WrappedCard("SupplyShortage")
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    shortage.AddSubCard(card);
                    shortage = RoomLogic.ParseUseCard(room, shortage);
                    return shortage;
                }
            }

            return null;
        }
    }

    public class JushouAI : SkillEvent
    {
        public JushouAI() : base("jushou")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            List<string> kingdoms = new List<string>();
            foreach (Player p in ai.Room.Players)
                if (p.HasShownOneGeneral() && !kingdoms.Contains(p.Kingdom))
                    kingdoms.Add(p.Kingdom);

            if (kingdoms.Count <= 2)
                return true;

            return false;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, player);
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is DefensiveHorse || fcard is Armor) && ai.GetSameEquip(card, player) == null)
                    return new List<int>(card.Id);
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is Weapon || fcard is OffensiveHorse || fcard is Treasure) && ai.GetSameEquip(card, player) == null)
                    return new List<int>(card.Id);
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is Armor && player.GetArmor() && ai.GetKeepValue(player.Armor.Key, player) < ai.GetUseValue(card.Id, player))
                    return new List<int>(card.Id);
            }

            return ai.AskForDiscard(string.Empty, 1, 1, false, false);
        }
    }

    public class QiangxiAI : SkillEvent
    {
        public QiangxiAI() : base("Qiangxi")
        {
        }
        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed("QiangxiCard"))
                return new WrappedCard("QiangxiCard")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
            else
                return null;
        }
    }

    public class QiangxiCardAI : UseCard
    {
        public QiangxiCardAI() : base("QiangxiCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);

            if (player.GetWeapon())
            {
                int hand_weapon = -1;
                foreach (WrappedCard _card in RoomLogic.GetPlayerHandcards(room, player))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(_card.Name);
                    if (fcard is Weapon)
                    {
                        hand_weapon = _card.Id;
                        break;
                    }
                }

                foreach (Player enemy in enemies)
                {
                    if (ai.PlayersLevel[enemy] > 3)
                    {
                        DamageStruct damage = new DamageStruct { From = player, To = enemy };
                        if (RoomLogic.InMyAttackRange(room, player, enemy) && hand_weapon >= 0 && ai.GetDamageScore(damage).Score > 4)
                        {
                            WrappedCard qiangxi = new WrappedCard("QiangxiCard")
                            {
                                Skill = Name,
                                ShowSkill = Name
                            };
                            qiangxi.AddSubCard(hand_weapon);

                            use.Card = qiangxi;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                        else if (RoomLogic.InMyAttackRange(room, player, enemy, room.GetCard(player.Weapon.Key)) && ai.GetDamageScore(damage).Score > 4)
                        {
                            WrappedCard qiangxi = new WrappedCard("QiangxiCard")
                            {
                                Skill = Name,
                                ShowSkill = Name
                            };
                            qiangxi.AddSubCard(player.Weapon.Key);

                            use.Card = qiangxi;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                    }
                }
            }
            else
            {
                bool analeptic = player.Hp == 1 && ai.GetCards("Analeptic", player).Count > 0;
                bool peach = ai.GetCards("Peach", player).Count > 0;
                foreach (Player enemy in enemies)
                {
                    if (ai.PlayersLevel[enemy] > 3)
                    {
                        DamageStruct damage = new DamageStruct { From = player, To = enemy };
                        if (RoomLogic.InMyAttackRange(room, player, enemy)
                            && ((ai.GetDamageScore(damage).Score > 4 && (!player.IsWounded() && peach || analeptic))
                            || (ai.GetDamageScore(damage).Score > 7 && (player.Hp > 1 || peach || analeptic))))
                        {
                            use.Card = card;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                    }
                }
            }
        }
    }

    public class QuhuAI : SkillEvent
    {
        public QuhuAI() : base("quhu")
        {
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("QuhuCard") && !player.IsKongcheng())
                return new WrappedCard("QuhuCard")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
            else
                return null;
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> to)
        {
            Player player = ai.Self;
            Room room = ai.Room;
            List<int> cards = new List<int>(player.HandCards);
            ai.SortByKeepValue(ref cards, false);

            if (player == requestor)
            {
                if (ai.Target[Name] != null)
                {
                    if (RoomLogic.InMyAttackRange(room, to[0], ai.Target[Name]))
                        return ai.GetMaxCard(player, null, new List<string> { "Peach" });
                }
                else
                {
                    WrappedCard card = ai.GetMinCard(player, null, new List<string> { "Peach" });
                    if (card != null)
                        return card;
                    else return ai.GetMinCard();
                }
            }
            else
            {
                if (ai.IsFriend(requestor))
                {
                    return ai.GetMinCard(player, null, new List<string> { "Peach" });
                }
                else
                {
                    if (!ai.HasSkill("zhiman"))
                    {
                        foreach (Player p in room.GetOtherPlayers(player))
                        {
                            DamageStruct damage = new DamageStruct(Name, player, p);
                            if (RoomLogic.InMyAttackRange(room, player, p) && ai.IsFriend(p) && p.Hp == 1 && !ai.CanResist(p, 1))
                            {
                                return ai.GetMaxCard(player, null, new List<string> { "Peach" });
                            }
                        }

                        DamageStruct _damage = new DamageStruct(Name, player, requestor);
                        ScoreStruct score = ai.GetDamageScore(_damage);
                        if (score.Score < 0)
                            return ai.GetMinCard(player, null, new List<string> { "Peach" });
                    }
                }
            }
            return room.GetCard(cards[0]);
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Player winner = (Player)ai.Room.GetTag(Name);
            if (ai.Target[Name] != null && target.Contains(ai.Target[Name]))
                return new List<Player> { ai.Target[Name] };
            else
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in target)
                {
                    DamageStruct damage = new DamageStruct(Name, winner, p);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    scores.Add(score);
                }
                ai.CompareByScore(ref scores);
                if (scores[0].Damage.To != null)
                    return new List<Player> { scores[0].Damage.To };
                else if (scores[0].Players != null && scores[0].Players.Count == 1 && target.Contains(scores[0].Players[0]))
                    return scores[0].Players;
            }

            return new List<Player>();
        }
    }

    public class QuhuCardAI : UseCard
    {
        public QuhuCardAI() : base("QuhuCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Target["quhu"] = null;
            WrappedCard max_card = ai.GetMaxCard(player, null, new List<string> { "Peach" });
            int peaches = ai.GetKnownCardsNums("Peach", "he", player);
            if (max_card == null && peaches - player.GetLostHp() < 2) return;

            int max_point = max_card.Number;
            if (ai.HasSkill("yingyang"))
                max_point = Math.Min(max_point + 3, 13);
            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            //优先杀敌血量敌方
            List<Player> targets = new List<Player>();
            foreach (Player enemy in enemies)
                if (!enemy.Removed)
                    targets.Add(enemy);
            ai.SortByHp(ref targets, false);
            if (targets.Count > 0)
            {
                //优先寻找敌人拼点
                foreach (Player target in targets)
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p != target && p.Hp > player.Hp && !p.IsKongcheng()
                            && RoomLogic.InMyAttackRange(room, p, target) && ai.IsEnemy(p) && RoomLogic.IsFriendWith(room, p, target)
                            && !ai.HasSkill("zhiman", p))
                        {
                            int max = 0;
                            WrappedCard enemy_max = ai.GetMaxCard(p);
                            if (enemy_max != null && p.HandcardNum == ai.GetKnownCards(p).Count)
                            {
                                max = enemy_max.Number;
                                if (ai.HasSkill("yingyang", p))
                                    max = Math.Min(13, max + 3);
                            }
                            //碰到张绣没把握就不拼
                            if (ai.HasSkill("congjian", p) && !ai.HasArmorEffect(player, "SilverLion"))
                            {
                                if (max == 0 && max_point < 13 || max_point < max)
                                    continue;
                            }

                            if (max > 0 && max_point > max || max_point == 13 || (max_point > 10 && !ai.HasSkill("yingyang", p)))
                            {
                                use.Card = card;
                                use.To = new List<Player> { p };
                                ai.Target["quhu"] = target;
                                return;
                            }
                        }
                    }
                }

                //寻找友方杀残血的
                foreach (Player target in targets)
                {
                    if (target.Hp != 1)
                        break;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p != target && p.Hp > player.Hp && !p.IsKongcheng()
                            && RoomLogic.InMyAttackRange(room, p, target) && ai.IsFriend(p))
                        {
                            int min = 0;
                            WrappedCard friend_min = ai.GetMinCard(p);
                            if (friend_min != null && p.HandcardNum == ai.GetKnownCards(p).Count)
                            {
                                min = friend_min.Number;
                                if (ai.HasSkill("yingyang", p))
                                    min = Math.Max(1, min - 3);
                            }
                            if ((min < 14 && max_point > min)
                                || max_point == 13
                                || (max_point > 10 && (ai.HasSkill("yingyang", p) || p.HandcardNum - ai.GetKnownCards(p).Count >= 2)))
                            {
                                use.Card = card;
                                use.To = new List<Player> { p };
                                ai.Target["quhu"] = target;
                                return;
                            }
                        }
                    }
                }

                //桃子溢出，需要掉血补牌
                if (peaches - player.GetLostHp() >= 2)
                {
                    bool pindian = false;
                    foreach (Player friend in ai.GetFriends(player))
                    {
                        if (friend.MaxHp - friend.HandcardNum >= 2)
                        {
                            pindian = true;
                            break;
                        }
                    }

                    if (pindian)
                    {
                        int min_number_no_peach = 14;
                        WrappedCard min_no_peach = ai.GetMinCard(player, null, new List<string> { "Peach" });
                        if (min_no_peach != null)
                        {
                            min_number_no_peach = min_no_peach.Number;
                            if (ai.HasSkill("yingyang", player))
                                min_number_no_peach = Math.Max(1, min_number_no_peach - 3);
                        }

                        int min_number = 14;
                        WrappedCard min = ai.GetMinCard();
                        if (min != null)
                        {
                            min_number = min.Number;
                            if (ai.HasSkill("yingyang", player))
                                min_number = Math.Max(1, min_number - 3);
                        }
                        enemies = ai.GetEnemies(player);
                        ai.SortByHandcards(ref enemies, false);
                        foreach (Player target in enemies)
                        {
                            if (target.Hp > player.Hp && !target.IsKongcheng())
                            {
                                if ((min_number_no_peach < 14 && min_number_no_peach < 7)
                                    || (min_number < 14 && min_number < 5)
                                    || (RoomLogic.InMyAttackRange(room, target, player) && (!ai.HasSkill("congjian", target) || ai.HasArmorEffect(player, "SilverLion"))))
                                {
                                    use.Card = card;
                                    use.To = new List<Player> { target };
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class JiemingAI : SkillEvent
    {
        public JiemingAI() : base("jieming")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForMasochism()) return new List<Player>();

            Dictionary<Player, double> point = new Dictionary<Player, double>();
            foreach (Player p in ai.GetFriends(player))
            {
                if (p.MaxHp - p.HandcardNum > 0)
                {
                    double value = p.MaxHp - p.HandcardNum;
                    if (ai.HasSkill(TrustedAI.CardneedSkill, p)) value *= 1.5;
                    if (ai.Room.Current == p)
                        value += 2;

                    point.Add(p, value);
                }
            }
            List<Player> targets = new List<Player>(point.Keys);
            if (targets.Count > 0)
            {
                targets.Sort((x, y) => { return point[x] > point[y] ? -1 : 1; });
                return new List<Player> { targets[0] };
            }

            if (ai.NeedShowImmediately())
                return new List<Player> { player };

            return new List<Player>();
        }
    }

    public class XingshangAI : SkillEvent
    {
        public XingshangAI() : base("xingshang")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class FangzhuAI : SkillEvent
    {
        public FangzhuAI() : base("fangzhu")
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
                    if (!player.HasShownOneGeneral())
                        ai.UpdatePlayerIntention(player, Scenario.Hegemony.WillbeRole(room, player), 100);
                    Player target = room.FindPlayer(choices[2]);
                    if (target != null && !target.FaceUp)
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForMasochism()) return new List<Player>();

            Room room = ai.Room;
            foreach (Player p in ai.GetFriends(player))
                if (p != player && !p.FaceUp)
                    return new List<Player> { p };

            if (room.Current != player && ai.IsFriend(room.Current) && ai.HasSkill("jushou", room.Current))
            {
                List<string> kingdoms = new List<string>();
                foreach (Player p in room.Players)
                    if (p.HasShownOneGeneral() && !kingdoms.Contains(p.Kingdom))
                        kingdoms.Add(p.Kingdom);

                if (!kingdoms.Contains(player.Kingdom))
                    kingdoms.Add(player.Kingdom);

                if (kingdoms.Count > 2)
                    return new List<Player> { room.Current };
            }

            Dictionary<Player, double> strenth = new Dictionary<Player, double>();
            List<Player> enemis = ai.GetEnemies(player);
            foreach (Player p in enemis)
                strenth.Add(p, ((SmartAI)ai).EvaluatePlayerStrength(p));

            enemis.Sort((x, y) => { return strenth[x] > strenth[y] ? -1 : 1; });
            foreach (Player p in enemis)
                if (p.FaceUp && !ai.WillSkipPlayPhase(p))
                    return new List<Player> { p };

            foreach (Player p in enemis)
                if (p.FaceUp)
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class XiaoguoAI : SkillEvent
    {
        public XiaoguoAI() : base("xiaoguo")
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
            }

            if (throw_card)
            {
                List<int> ids = new List<int>(player.HandCards);
                ai.SortByKeepValue(ref ids, false);

                foreach (int id in ids)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    if (fcard is BasicCard && RoomLogic.CanDiscard(room, player, player, id))
                    {
                        if (min_value - ai.GetKeepValue(id, player) > 3)
                            return new List<int> { id };
                    }
                }
            }

            return new List<int>();
        }
    }
}