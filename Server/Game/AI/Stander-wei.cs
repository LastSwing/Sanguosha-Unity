using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

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
        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            Room room = ai.Room;
            double value = 0;
            if (!card.IsVirtualCard())
            {
                foreach (int id in card.SubCards)
                {
                    if (ai.IsCard(id, Peach.ClassName, to))
                        value += 1;
                    if (ai.IsCard(id, Analeptic.ClassName, to))
                        value += 0.6;
                }

                if (ai.IsEnemy(to))
                    value = -value;
            }

            return value;
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
                        score.Score += value * 0.7;
                    }

                    if (ai.WillSkipPlayPhase(damage.To))
                        score.Score /= 3;

                    if (damage.Damage >= damage.To.Hp)
                        score.Score /= 2;

                    if (damage.From != null && damage.From == damage.To && score.Score > 0)
                        score.Score /= 4;    
                }
                if (damage.Damage > 1) score.Score -= 1;
                if (ai.WillSkipPlayPhase(damage.To)) score.Score /= 1.5;

                if (ai.IsEnemy(damage.To))
                    score.Score = -score.Score;
                else
                    score.Score -= 1.5;
            }

            return score;
        }
    }

    public class FankuiAI : SkillEvent
    {
        public FankuiAI() : base("fankui")
        {
            key = new List<string> { "cardChosen:fankui" };
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is DamageStruct damage)
            {
                Player to = damage.To;
                List<int> disable = new List<int>();
                foreach (int id in to.GetCards("he"))
                {
                    if (!RoomLogic.CanGetCard(ai.Room, player, to, id))
                        disable.Add(id);
                }

                if (ai.FindCards2Discard(player, damage.From, string.Empty, "he", FunctionCard.HandlingMethod.MethodGet, 1, true, disable).Score > 0)
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
            if (damage.To != null && damage.From != null && !damage.From.IsNude() && damage.Damage <= damage.To.Hp && ai.HasSkill(Name, damage.To))
            {
                if (ai.IsFriend(damage.From, damage.To) && damage.From.HasEquip() && damage.From.IsWounded())
                {
                    if (ai.HasArmorEffect(damage.From, SilverLion.ClassName)) score.Score += 2;
                }
                else if (!ai.IsFriend(damage.From, damage.To))
                {
                    score.Score += 1;
                    if (damage.From.HasWeapon(CrossBow.ClassName))
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

                    if (ai.GetPlayerTendency(target) == "unknown" && ai.HasArmorEffect(target, SilverLion.ClassName) && target.IsWounded() && id == target.Armor.Key)
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
            key = new List<string> { "cardResponded%guicai" };
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
                if (ai.IsEnemy(judge.Who) && (!ai.IsSituationClear() || !ai.GetPrioEnemies().Contains(judge.Who))) return use;
                int id = ai.GetRetrialCardId(player.GetCards("h"), judge);
                if (id >= 0)
                    use.Card = ai.Room.GetCard(id);
            }

            return use;
        }

        public static bool HasSpade(TrustedAI ai, Player player)
        {
            Room room = ai.Room;

            List<int> ids = ai.GetKnownCards(player);
            ids.AddRange(ai.GetKnownHandPileCards(player));
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Spade && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                    return true;
            }

            int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
            if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                count += player.HandcardNum - ai.GetKnownCards(player).Count;

            return count > 2;
        }

        public static bool HasClub(TrustedAI ai, Player player)
        {
            Room room = ai.Room;

            List<int> ids = ai.GetKnownCards(player);
            ids.AddRange(ai.GetKnownHandPileCards(player));
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Club && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                    return true;
            }

            int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
            if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                count += player.HandcardNum - ai.GetKnownCards(player).Count;

            return count > 3;
        }
        public static bool HasHeart(TrustedAI ai, Player player)
        {
            Room room = ai.Room;

            List<int> ids = ai.GetKnownCards(player);
            ids.AddRange(ai.GetKnownHandPileCards(player));
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                    return true;
            }

            int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
            if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                count += player.HandcardNum - ai.GetKnownCards(player).Count;

            return count > 3;
        }

        public override bool CanRetrial(TrustedAI ai, string pattern, Player player, Player judge_who)
        {
            Room room = ai.Room;
            if (pattern == "leiji")
            {
                if (ai.IsFriend(player, judge_who))
                {
                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Spade && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 1) return true;
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
                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Club && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 1) return true;
                }
            }
            else if (pattern == Indulgence.ClassName)
            {
                if (ai.IsFriend(player, judge_who))
                    return HasHeart(ai, player);
                else
                {
                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Heart && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 1) return true;
                }
            }
            else if (pattern == Lightning.ClassName)
            {
                if (ai.IsFriend(player, judge_who))
                {
                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if ((card.Suit != WrappedCard.CardSuit.Spade || card.Number == 1 || card.Number >= 10)
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 1) return true;
                }
                else
                {
                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Spade && card.Number > 1 && card.Number < 10
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 4) return true;
                }
            }

            return false;
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
                else if (damage.From == null)
                    return true;
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

                        if (ai.HasSkill("fankui", target) && player.HasArmor(SilverLion.ClassName))
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

                        if (ai.HasSkill("fankui", target) && player.HasWeapon(CrossBow.ClassName))
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

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> cards, int min, int max, bool option)
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

            if (ai.DamageEffect(damage, DamageStruct.DamageStep.Done) == 0 || player.Removed)
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
                    if (xiahou.HasArmor(SilverLion.ClassName) && ai.IsFriend(xiahou))
                        invoke = true;
                    else if (xiahou.HasWeapon(CrossBow.ClassName) && !ai.IsFriend(xiahou))
                        invoke = true;
                }
                if (ai.HasSkill("wangxi", player) && ai.IsFriend(xiahou))
                    invoke = true;

                if (invoke && (player.Hp > 1 || ai.GetCards(Peach.ClassName, player).Count > 0 || ai.GetCards(Analeptic.ClassName, player).Count > 0))
                    return result;
            }

            if (player.Hp == 1 && (ai.HasArmorEffect(player, BreastPlate.ClassName) || ai.GetCards(Peach.ClassName, player).Count > 0 && ai.GetCards(Analeptic.ClassName, player).Count > 0))
                return result;

            if (room.Current == player)
            {
                if (ai.GetCards(Peach.ClassName, player).Count > 0) return result;
                if (player.Hp - 1 > player.HandcardNum) return result;
            }

            List<int> ids= player.GetCards("h");
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
            key = new List<string> { "playerChosen:tuxi" };
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
                    foreach (string general in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(general, true));
                    
                    if (!player.HasShownOneGeneral())
                    {
                        string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 100);
                    }
                    foreach (Player p in targets)
                    {
                        if (ai.IsKnown(player, p) && ai.GetPlayerTendency(p) == "unknown")
                            ai.UpdatePlayerRelation(player, p, false);
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (player.HasFlag("jieyue_draw") || player.HasTreasure(JadeSeal.ClassName)) return new List<Player>();

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
            Player zhugeliang = ai.FindPlayerBySkill("kongcheng|kongcheng_jx");

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
                        if (!ai.IsCard(id, Jink.ClassName, zhugeliang) && !!ai.IsCard(id, Peach.ClassName, zhugeliang) && !ai.IsCard(id, Analeptic.ClassName, zhugeliang))
                            result.Add(zhugeliang);
                    }
                }
            }

            foreach (Player p in enemies)
            {
                if (result.Contains(p) || !RoomLogic.CanGetCard(room, player, p, "h")) continue;
                List<int> ids = ai.GetKnownCards(p);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == Peach.ClassName || card.Name == Nullification.ClassName || card.Name.Contains(Nullification.ClassName))
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
                    if (result.Contains(p) || !RoomLogic.CanGetCard(room, player, p, "h")) continue;
                    if (ai.HasSkill("jijiu|qingnang|leiji|jieyin|beige|kanpo|liuli|qiaobian|zhiheng|guidao|tianxiang|lijian", p))
                        result.Add(p);

                    if (result.Count >= 2) break;
                }
            }
            if (result.Count < 2)
            {
                foreach (Player p in enemies)
                {
                    if (result.Contains(p) || !RoomLogic.CanGetCard(room, player, p, "h")) continue;
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
                if (use.To.Count > 1 || ai.GetCards("Halberg", player).Count > 0 || ai.GetCards(QinggangSword.ClassName, player).Count > 0
                    || !ai.HasArmorEffect(use.To[0], SilverLion.ClassName))
                    return true;
            }

            List<WrappedCard> duels = ai.GetCards(Duel.ClassName, player);
            foreach (WrappedCard card in duels)
            {
                foreach (Player p in enemies)
                {
                    if (RoomLogic.IsProhibited(room, player, p, card) == null && ai.GetCards(Slash.ClassName, player).Count >= ai.GetCards(Slash.ClassName, p).Count
                        && !ai.HasArmorEffect(p, SilverLion.ClassName))
                        return true;
                }
            }

            return false;
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (step == DamageStruct.DamageStep.Caused && damage.From != null
                && damage.From.Alive && damage.From.HasFlag(Name) && damage.Card != null && (damage.Card.Name == Duel.ClassName || damage.Card.Name.Contains(Slash.ClassName)
                && !damage.Chain && !damage.Transfer))
            {
                damage.Damage++;
            }
        }
    }

    public class TianduAI : SkillEvent
    {
        public TianduAI() : base("tiandu")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && card.Name == EightDiagram.ClassName)
            {
                return 1;
            }

            return 0;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack()) return false;
            if (data is JudgeStruct judge)
            {
                if (judge.Reason == "tuntian" && judge.Card.Suit != WrappedCard.CardSuit.Heart)
                {
                    if (ai.IsCard(judge.Card.Id, Peach.ClassName, player))
                        return !ai.NeedKongcheng(player) || !player.IsKongcheng();

                    if (ai.IsCard(judge.Card.Id, Analeptic.ClassName, player) && ai.IsWeak())
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
            key = new List<string> { "Yiji:yiji" };
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

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override Player OnYiji(TrustedAI ai, Player player, List<int> ids, ref int id)
        {
            KeyValuePair<Player, int> key = ai.GetCardNeedPlayer(ids, null, Player.Place.PlaceHand, Name);
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
                if (n < damage.To.Hp || damage.To.Removed)
                    score.Score = 4;

                double numerator = 0.6;
                if (!ai.WillSkipPlayPhase(damage.To)) numerator += 0.3;
                foreach (Player p in ai.GetFriends(damage.To))
                    if (p != damage.To && !ai.WillSkipPlayPhase(p))
                        numerator += 0.1;
                score.Score *= numerator;

                if (damage.Damage > 1) score.Score /= 1.5;
                if (ai.IsEnemy(damage.To))
                    score.Score = -score.Score;
                else
                    score.Score -= 1.5;
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
        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            int id = card.GetEffectiveId();
            Room room = ai.Room;
            List<WrappedCard> cards = ai.GetViewAsCards(player, id);
            double value = 0;
            foreach (WrappedCard c in cards)
            {
                if (c.Skill == Name) continue;
                double card_value = ai.GetUseValue(c, player, room.GetCardPlace(id));
                if (card_value > value)
                    value = card_value;
            }

            return Math.Max(0, 4 - value);
        }
        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (card != null && WrappedCard.IsBlack(card.Suit) && (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand))
            {
                WrappedCard jink = new WrappedCard(Jink.ClassName)
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
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;

            if (prompt.EndsWith("1"))
            {
                if (!ai.WillShowForAttack()) return use;
                if (RoomLogic.PlayerContainsTrick(room, player, Lightning.ClassName) && player.JudgingArea.Count == 1)
                {
                    Player wizzard = ai.GetWizzardRaceWinner(Lightning.ClassName, player);
                    if (wizzard == null || !ai.IsEnemy(wizzard))
                        return use;
                }
                if (!RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName) && !RoomLogic.PlayerContainsTrick(room, player, SupplyShortage.ClassName))
                {
                    if (player.HasTreasure(JadeSeal.ClassName) || player.GetPile("yijipile").Count > 0) return use;
                }

                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    Skill = "_shensu",
                    DistanceLimited = false
                };
                List<WrappedCard> cards = new List<WrappedCard> { slash };
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, cards, null, false);
                if (scores.Count > 0)
                {
                    foreach (Player p in scores[0].Players)
                    {
                        if (ai.IsEnemy(p) && scores[0].Score > 4)
                        {
                            if (p.Hp <= 1 && (!RoomLogic.InMyAttackRange(room, player, p) || ai.GetCards(Slash.ClassName, player).Count == 0 || RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName)))
                            {
                                use.Card = new WrappedCard(ShensuCard.ClassName) { Skill = Name };
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

                if (RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName))
                {
                    Player wizzard = ai.GetWizzardRaceWinner(Indulgence.ClassName, player);
                    if (!Nullification && (wizzard == null || ai.IsEnemy(wizzard)))
                    {
                        if (scores.Count > 0 && scores[0].Score > 3)
                        {
                            use.Card = new WrappedCard(ShensuCard.ClassName) { Skill = Name };
                            use.To = scores[0].Players;
                            return use;
                        }
                        else
                        {
                            double value = 0;
                            foreach (int id in player.GetCards("h"))
                                value += ai.GetUseValue(id, player);

                            foreach (int id in player.GetHandPile())
                                value += ai.GetUseValue(id, player);

                            if (value > 16 || ai.GetOverflow(player) > 0)
                            {
                                if (scores.Count > 0 && scores[0].Score > 0)
                                {
                                    use.Card = new WrappedCard(ShensuCard.ClassName) { Skill = Name };
                                    use.To = scores[0].Players;
                                    return use;
                                }
                                else
                                {
                                    FunctionCard fcard = Slash.Instance;
                                    foreach (Player p in room.GetOtherPlayers(player))
                                    {
                                        if (fcard.TargetFilter(room, new List<Player>(), p, player, slash)
                                            && (!ai.IsFriend(p) || ai.IsCancelTarget(slash, p, player) || !ai.IsCardEffect(slash, p, player)))
                                        {
                                            use.Card = new WrappedCard(ShensuCard.ClassName) { Skill = Name };
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
                double value = 0;
                List<WrappedCard> cards = ai.GetCards(Slash.ClassName, player, true);
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, cards, null, false);
                if (scores.Count > 0 && scores[0].Score > 0)
                {
                    if (ai.GetCards(CrossBow.ClassName, player).Count > 0 && cards.Count > 0)
                        return use;
                    value = scores[0].Score;
                }

                List<WrappedCard> slashes = new List<WrappedCard>();
                foreach (int id in player.GetCards("he"))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    if (fcard is EquipCard && RoomLogic.CanDiscard(room, player, player, id))
                    {
                        WrappedCard slash = new WrappedCard(Slash.ClassName)
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
                    scores = ai.CaculateSlashIncome(player, slashes, null, false);
                    if (scores.Count > 0)
                    {
                        for (int i = 0; i < scores.Count; i++)
                        {
                            ScoreStruct score = scores[i];
                            WrappedCard equip = room.GetCard(score.Card.SubCards[0]);
                            if (equip.Name == SilverLion.ClassName && player.HasArmor(equip.Name) && player.IsWounded())
                                score.Score += 2.5;
                            else if (player.HasEquip(equip.Name) || ai.GetSameEquip(equip, player) == null)
                                score.Score -= 2;
                        }

                        ai.CompareByScore(ref scores);
                        if (scores[0].Score > value && scores[0].Score > 4)
                        {
                            use.Card = new WrappedCard(ShensuCard.ClassName) { Skill = Name };
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
            key = new List<string> { "cardChosen:qiaobian" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    int id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);

                    if (player != target)
                    {
                        if (room.GetCardPlace(id) == Player.Place.PlaceDelayedTrick)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            bool friend = ai.GetKeepValue(id, target, Player.Place.PlaceEquip) < 0;
                            ai.UpdatePlayerRelation(player, target, friend);
                        }
                    }
                }
            }
        }

        public static KeyValuePair<int, Player> CardForQiaobian(TrustedAI ai, Player who)
        {
            int result_id = -1;
            Player player = ai.Self;
            Room room = ai.Room;
            if (ai.IsFriend(who))
            {
                List<int> judges = new List<int>(who.JudgingArea);
                foreach (int id in judges)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == Indulgence.ClassName)
                    {
                        foreach (Player enemy in ai.GetEnemies(player))
                        {
                            if (!RoomLogic.PlayerContainsTrick(room, enemy, card.Name) && !ai.IsCancelTarget(card, enemy, null)
                                && RoomLogic.IsProhibited(room, null, enemy, card) == null && enemy.JudgingAreaAvailable)
                                return new KeyValuePair<int, Player>(id, enemy);
                        }
                    }
                }

                foreach (int id in judges)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == SupplyShortage.ClassName)
                    {
                        foreach (Player enemy in ai.GetEnemies(player))
                        {
                            if (!RoomLogic.PlayerContainsTrick(room, enemy, card.Name) && !ai.IsCancelTarget(card, enemy, null)
                                && RoomLogic.IsProhibited(room, null, enemy, card) == null && enemy.JudgingAreaAvailable)
                                return new KeyValuePair<int, Player>(id, enemy);
                        }
                    }
                }

                if (ai.HasSkill(TrustedAI.LoseEquipSkill, who, true) && who.HasEquip())
                {
                    List<int> equips = player.GetEquips();
                    if (who.GetArmor() && ai.GetKeepValue(who.Armor.Key, who) < 0)
                        result_id = who.Armor.Key;
                    if (result_id < 0 && who.GetSpecialEquip())
                        result_id = who.Special.Key;
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
                            if (ai.GetSameEquip(equip, p) == null && (ai.HasSkill(TrustedAI.NeedEquipSkill, who) || ai.HasSkill(TrustedAI.LoseEquipSkill, who))
                                && RoomLogic.CanPutEquip(p, equip))
                                return new KeyValuePair<int, Player>(result_id, p);
                        }
                        List<Player> friends = ai.GetFriends(player);
                        ai.SortByDefense(ref friends, false);
                        foreach (Player p in room.GetOtherPlayers(who))
                        {
                            if (p != who && ai.GetSameEquip(equip, p) == null && RoomLogic.CanPutEquip(p, equip))
                                return new KeyValuePair<int, Player>(result_id, p);
                        }
                    }
                }
            }
            else
            {
                if (!who.HasEquip() || (ai.HasSkill(TrustedAI.LoseEquipSkill, who) && !who.GetTreasure())) return new KeyValuePair<int, Player>(-1, null);

                int id = ai.AskForCardChosen(who, "e", Snatch.ClassName, FunctionCard.HandlingMethod.MethodNone, new List<int>());
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
                            if (ai.GetSameEquip(card, p) == null && (ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill(TrustedAI.NeedEquipSkill, p)))
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
                            if (ai.GetSameEquip(card, p) == null && (ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill(TrustedAI.NeedEquipSkill, p)))
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

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            List<int> to_discard = new List<int>();
            if (!ai.WillShowForAttack()) return to_discard;
            
            List<int> cards= player.GetCards("h");
            ai.SortByKeepValue(ref cards, false);
            Room room = ai.Room;
            Player stealer = null;

            foreach (Player ap in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill("tuxi|tuxi_jx", ap) && ai.IsEnemy(ap))
                {
                    stealer = ap;
                    break;
                }
            }
            int card = -1;
            for (int i = 0; i < cards.Count; i++)
            {
                if (ai.IsCard(cards[i], Peach.ClassName, player))
                {
                    if (stealer != null && player.HandcardNum <= 2 && player.Hp > 2 && !RoomLogic.PlayerContainsTrick(room, stealer, SupplyShortage.ClassName))
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
                if (RoomLogic.PlayerContainsTrick(room, player, Lightning.ClassName))
                {
                    Player wizzard = ai.GetWizzardRaceWinner(Lightning.ClassName, player);
                    if ((wizzard != null && ai.IsEnemy(wizzard)) || ai.GetFriends(player).Count > ai.GetEnemies(player).Count)
                        return to_discard;
                }
                else if (RoomLogic.PlayerContainsTrick(room, player, SupplyShortage.ClassName))
                {
                    if (player.Hp > player.HandcardNum) return to_discard;
                    List<Player> targets = TuxiAI.FindTuxiTargets(ai, player);
                    if (targets.Count == 2) return to_discard;
                }
                else if (RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName))
                {
                    if (player.HandcardNum > 3 || player.HandcardNum > player.Hp - 1) return to_discard;
                    foreach (Player friend in ai.FriendNoSelf)
                        if (RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName) || RoomLogic.PlayerContainsTrick(room, player, SupplyShortage.ClassName))
                            return to_discard;
                }
            }
            else if (phase == Player.PlayerPhase.Draw && !player.IsSkipped(Player.PlayerPhase.Draw) && !ai.HasSkill("tuxi|tuxi_jx"))
            {
                ai.Target["qiaobian1"] = null;
                ai.Target["qiaobian2"] = null;

                if (player.HasFlag("jieyue_draw") || player.HasTreasure(JadeSeal.ClassName) || player.GetMark("@tangerine") > 0)
                    return new List<int>();
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
                //cards= player.GetCards("h");
                //ai.SortByKeepValue(ref cards, false);
                //to_discard = new List<int>(cards[0]);

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

                cards= player.GetCards("h");
                ai.SortByKeepValue(ref cards, false);
                //to_discard = new List<int>(cards[0]);
                double top_value = 0;
                foreach (int id in cards)
                {
                    if (!ai.IsCard(id, Jink.ClassName, player))
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
            CardUseStruct use = new CardUseStruct { From = player, Card = new WrappedCard(QiaobianCard.ClassName) { Mute = true, Skill = Name }, To = new List<Player>() };
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
                    ai.Room.Debug("巧变3 AI出错");
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
                WrappedCard shortage = new WrappedCard(SupplyShortage.ClassName)
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

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.WillShowForAttack() || player.HasFlag(Name)) return null;
            Room room = ai.Room;
            List<int> ids= player.GetCards("h");
            ids.AddRange(player.GetEquips());
            ids.AddRange(player.GetHandPile());
            ai.SortByUseValue(ref ids);
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (WrappedCard.IsBlack(card.Suit) && (fcard is BasicCard || fcard is EquipCard))
                {
                    WrappedCard shortage = new WrappedCard(SupplyShortage.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    shortage.AddSubCard(card);
                    shortage = RoomLogic.ParseUseCard(room, shortage);
                    return new List<WrappedCard> { shortage };
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
            if (ai.Room.Setting.GameMode == "Hegemony")
            {
                List<string> kingdoms = new List<string>();
                foreach (Player p in ai.Room.GetAlivePlayers())
                    if (p.HasShownOneGeneral() && !kingdoms.Contains(p.Kingdom))
                        kingdoms.Add(p.Kingdom);

                if (kingdoms.Count <= 2)
                    return true;

                if (ai.IsWeak(player)) return true;

                return false;
            }

            return true;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, player);
            WrappedCard jushou = new WrappedCard(JushouCard.ClassName)
            {
                Mute = true,
                Skill = Name
            };
            CardUseStruct use = new CardUseStruct(jushou, player, new List<Player>());
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is DefensiveHorse || fcard is Armor || fcard is SpecialEquip) && RoomLogic.CanPutEquip(player, card) && ai.GetSameEquip(card, player) == null)
                {
                    jushou.AddSubCard(card);
                    return use;
                }
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is Weapon || fcard is OffensiveHorse || fcard is Treasure) && RoomLogic.CanPutEquip(player, card) && ai.GetSameEquip(card, player) == null)
                {
                    jushou.AddSubCard(card);
                    return use;
                }
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is Armor && player.GetArmor() && RoomLogic.CanPutEquip(player, card) && ai.GetKeepValue(player.Armor.Key, player) < ai.GetUseValue(card.Id, player))
                {
                    jushou.AddSubCard(card);
                    return use;
                }
            }

            List<int> subs = ai.AskForDiscard(player.GetCards("h"), string.Empty, 1, 1, false);
            jushou.AddSubCards(subs);
            return use;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return "use";
        }
    }

    public class QiangxiAI : SkillEvent
    {
        public QiangxiAI() : base("qiangxi")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(QiangxiCard.ClassName))
            {
                WrappedCard card = new WrappedCard(QiangxiCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                return new List<WrappedCard> { card };
            }
            else
                return null;
        }
    }

    public class QiangxiCardAI : UseCard
    {
        public QiangxiCardAI() : base(QiangxiCard.ClassName)
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return player.GetWeapon() ? 3 : 0;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    ai.UpdatePlayerRelation(player, p, false);
            }
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
                        DamageStruct damage = new DamageStruct("qiangxi", player, enemy);
                        if (RoomLogic.InMyAttackRange(room, player, enemy) && hand_weapon >= 0 && ai.GetDamageScore(damage).Score > 4)
                        {
                            card.AddSubCard(hand_weapon);
                            use.Card = card;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                        else if (RoomLogic.InMyAttackRange(room, player, enemy, room.GetCard(player.Weapon.Key)) && ai.GetDamageScore(damage).Score > 4)
                        {
                            card.AddSubCard(player.Weapon.Key);
                            use.Card = card;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                    }
                }
            }
            else
            {
                bool analeptic = player.Hp == 1 && ai.GetCards(Analeptic.ClassName, player).Count > 0;
                bool peach = ai.GetCards(Peach.ClassName, player).Count > 0;
                foreach (Player enemy in enemies)
                {
                    if (ai.PlayersLevel[enemy] > 3)
                    {
                        DamageStruct damage = new DamageStruct("qiangxi", player, enemy);
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

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("QuhuCard") && !player.IsKongcheng())
                return new List<WrappedCard>{ new WrappedCard("QuhuCard"){ Skill = Name, ShowSkill = Name }};
            else
                return null;
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> to)
        {
            Player player = ai.Self;
            Room room = ai.Room;
            List<int> cards= player.GetCards("h");
            ai.SortByKeepValue(ref cards, false);

            if (player == requestor)
            {
                if (ai.Target[Name] != null)
                {
                    if (RoomLogic.InMyAttackRange(room, to[0], ai.Target[Name]))
                        return ai.GetMaxCard(player, null, new List<string> { Peach.ClassName });
                }
                else
                {
                    WrappedCard card = ai.GetMinCard(player, null, new List<string> { Peach.ClassName });
                    if (card != null)
                        return card;
                    else return ai.GetMinCard();
                }
            }
            else
            {
                if (ai.IsFriend(requestor))
                {
                    return ai.GetMinCard(player, null, new List<string> { Peach.ClassName });
                }
                else
                {
                    if (!ai.HasSkill("zhiman|zhiman_jx"))
                    {
                        foreach (Player p in room.GetOtherPlayers(player))
                        {
                            DamageStruct damage = new DamageStruct(Name, player, p);
                            if (RoomLogic.InMyAttackRange(room, player, p) && ai.IsFriend(p) && p.Hp == 1 && !ai.CanResist(p, 1))
                            {
                                return ai.GetMaxCard(player, null, new List<string> { Peach.ClassName });
                            }
                        }

                        DamageStruct _damage = new DamageStruct(Name, player, requestor);
                        ScoreStruct score = ai.GetDamageScore(_damage);
                        if (score.Score < 0)
                            return ai.GetMinCard(player, null, new List<string> { Peach.ClassName });
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
            WrappedCard max_card = ai.GetMaxCard(player, null, new List<string> { Peach.ClassName });
            int peaches = ai.GetKnownCardsNums(Peach.ClassName, "he", player);
            if (max_card == null || peaches - player.GetLostHp() < 2) return;

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
                        if (p != target && p.Hp > player.Hp && RoomLogic.CanBePindianBy(room, p, player)
                            && RoomLogic.InMyAttackRange(room, p, target) && ai.IsEnemy(p) && RoomLogic.IsFriendWith(room, p, target)
                            && !ai.HasSkill("zhiman|zhiman_jx", p))
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
                            if (ai.HasSkill("congjian", p) && !ai.HasArmorEffect(player, SilverLion.ClassName))
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
                        if (p != target && p.Hp > player.Hp && RoomLogic.CanBePindianBy(room, p, player)
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
                        WrappedCard min_no_peach = ai.GetMinCard(player, null, new List<string> { Peach.ClassName });
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
                            if (target.Hp > player.Hp && RoomLogic.CanBePindianBy(room, target, player))
                            {
                                if ((min_number_no_peach < 14 && min_number_no_peach < 7)
                                    || (min_number < 14 && min_number < 5)
                                    || (RoomLogic.InMyAttackRange(room, target, player) && (!ai.HasSkill("congjian", target) || ai.HasArmorEffect(player, SilverLion.ClassName))))
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
            key = new List<string> { "playerChosen:jieming" };
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
                    {
                        string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 100);
                    }
                    Player target = room.FindPlayer(choices[2]);
                    if (target != null && target.HandcardNum < target.MaxHp)
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            double value = 0;
            if (ai.HasSkill(Name, damage.To))
            {
                Room room = ai.Room;
                Dictionary<Player, double> point = new Dictionary<Player, double>();
                foreach (Player p in ai.GetFriends(damage.To))
                {
                    int count = p.HandcardNum;
                    if (damage.Card != null)
                    {
                        foreach (int id in damage.Card.SubCards)
                            if (room.GetCardOwner(id) == damage.From && room.GetCardPlace(id) == Player.Place.PlaceHand)
                                count--;
                    }
                    if (p.MaxHp - count > 0)
                    {
                        double v = (p.MaxHp - count) * 1.5;
                        if (ai.HasSkill(TrustedAI.CardneedSkill, p)) v *= 1.5;
                        if (ai.Room.Current == p)
                            v += 2;
                        if (ai.WillSkipPlayPhase(p))
                            v /= 1.5;

                        point.Add(p, v);
                    }
                }
                List<Player> targets = new List<Player>(point.Keys);
                if (targets.Count > 0)
                {
                    targets.Sort((x, y) => { return point[x] > point[y] ? -1 : 1; });
                    value = point[targets[0]];
                    if (damage.Damage > 1) value /= 1.5;
                    if (damage.Damage >= damage.To.Hp) value /= 2;
                    if (ai.IsEnemy(targets[0]))
                        value = -value;
                    else
                        value -= 1.5;
                }
            }

            ScoreStruct score = new ScoreStruct
            {
                Score = value
            };
            return score;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForMasochism()) return new List<Player>();

            Dictionary<Player, double> point = new Dictionary<Player, double>();
            foreach (Player p in ai.GetFriends(player))
            {
                if (p.MaxHp - p.HandcardNum > 0)
                {
                    double value = Math.Min(5, p.MaxHp - p.HandcardNum);
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
            key = new List<string> { "playerChosen:fangzhu" };
        }
        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            double value = 0;
            if (ai.HasSkill(Name, damage.To))
            {
                Room room = ai.Room;
                foreach (Player p in ai.GetFriends(damage.To))
                {
                    if (p != damage.To && !p.FaceUp)
                    {
                        value = 5;
                        break;
                    }
                }
                if (value == 0)
                {
                    foreach (Player p in ai.GetEnemies(damage.To))
                    {
                        if (!ai.WillSkipPlayPhase(p) && p.FaceUp && p.HandcardNum > 3)
                        {
                            value = 6;
                            break;
                        }
                    }
                }

                if (value == 0)
                {
                    foreach (Player p in ai.GetEnemies(damage.To))
                    {
                        if (!ai.WillSkipPlayPhase(p) && p.FaceUp)
                        {
                            value = 5;
                            break;
                        }
                    }
                }
                if (value == 0)
                {
                    foreach (Player p in ai.GetEnemies(damage.To))
                    {
                        if (p.FaceUp)
                        {
                            value = 3;
                            break;
                        }
                    }
                }
                if (ai.GetFriends(damage.To).Count == 1)
                {
                    value /= 1.5;
                    if (ai.WillSkipPlayPhase(damage.To))
                        value /= 2;
                }

                if (damage.Damage > 1) value /= 1.5;
                if (damage.Damage >= damage.To.Hp) value /= 2;
                if (!ai.IsFriend(damage.To)) value = -value;
            }

            ScoreStruct score = new ScoreStruct
            {
                Score = value
            };
            return score;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                Room room = ai.Room;
                string[] choices = choice.Split(':');
                Player target = room.FindPlayer(choices[2]);
                if (ai is SmartAI && ai.Self != player)
                {
                    if (!player.HasShownOneGeneral())
                    {
                        string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 100);
                    }
                    int count = player.GetLostHp();
                    if (count >= 2 && ai.HasCrossbowEffect(target)) return; 

                    if (target != null && !target.FaceUp)
                        ai.UpdatePlayerRelation(player, target, true);
                    if (ai.GetPlayerTendency(target) == "unknown" && target.FaceUp && ai.IsKnown(player, target))
                    {
                        if (ai.HasSkill("jushou", target))
                        {
                            List<string> kingdoms = new List<string>();
                            foreach (Player p in room.GetAlivePlayers())
                                if (p.HasShownOneGeneral() && !kingdoms.Contains(p.Kingdom))
                                    kingdoms.Add(p.Kingdom);

                            if (kingdoms.Count > 2)
                                ai.UpdatePlayerRelation(player, target, true);
                        }
                        else
                            ai.UpdatePlayerRelation(player, target, false);
                    }
                }
                else if (ai is StupidAI)
                {
                    int count = player.GetLostHp();
                    if (count >= 2 && ai.HasCrossbowEffect(target) || (ai.HasSkill("luanji_jx", target) && target == room.Current)) return;

                    if (!target.FaceUp) ai.UpdatePlayerRelation(player, target, true);
                    if (ai.GetPlayerTendency(target) == "unknown" && target.FaceUp)
                    {
                        if (ai.HasSkill("jushou_jx|shensu_jx", target))
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                            ai.UpdatePlayerRelation(player, target, false);
                    }
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForMasochism()) return new List<Player>();

            Room room = ai.Room;
            int count = player.GetLostHp();
            foreach (Player p in ai.GetFriends(player))
            {
                if (p != player && !p.FaceUp && ai.HasSkill("jushou", p) && p == room.Current)
                {
                    List<string> kingdoms = new List<string>();
                    foreach (Player _p in room.GetAlivePlayers())
                        if (_p.HasShownOneGeneral() && !kingdoms.Contains(_p.Kingdom))
                            kingdoms.Add(_p.Kingdom);

                    if (!kingdoms.Contains(player.Kingdom))
                        kingdoms.Add(player.Kingdom);

                    if (kingdoms.Count > 2) continue;
                    else return new List<Player> { p };
                }
                else if (p != player && !p.FaceUp && (!ai.HasSkill("jushou_jx|shensu_jx", p) || p != room.Current))
                    return new List<Player> { p };
            }

            if (room.Current != player && ai.IsFriend(room.Current) && ai.HasSkill("jushou", room.Current) && room.Current.FaceUp)
            {
                List<string> kingdoms = new List<string>();
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasShownOneGeneral() && !kingdoms.Contains(p.Kingdom))
                        kingdoms.Add(p.Kingdom);

                if (!kingdoms.Contains(player.Kingdom))
                    kingdoms.Add(player.Kingdom);

                if (kingdoms.Count > 2)
                    return new List<Player> { room.Current };
            }
            if (room.Current != player && ai.IsFriend(room.Current) && ai.HasSkill("jushou_jx|shensu_jx", room.Current) && room.Current.FaceUp)
                return new List<Player> { room.Current };
            if (count >= 3 && ai.IsFriend(room.Current) && ai.HasCrossbowEffect(room.Current) && !ai.HasSkill("kuangcai", room.Current))
                return new List<Player> { room.Current };
            if (ai is StupidAI && count >= 2 && ai.IsFriend(room.Current) && ai.HasSkill("luanji_jx", room.Current)) return new List<Player> { room.Current };

            Dictionary<Player, double> strenth = new Dictionary<Player, double>();
            List<Player> enemis = ai.GetEnemies(player);
            if (ai is SmartAI smart)
            {
                foreach (Player p in enemis)
                {
                    double value = 0;
                    if (count >= 2 && (ai.HasCrossbowEffect(p) || ai.HasSkill("luanji|luanji_jx", p)) && p == room.Current)
                    {
                        value = -10;
                    }
                    else
                    {
                        value = smart.EvaluatePlayerStrength(p);
                        if (room.Current == p) value -= 1.5;
                    }
                    strenth.Add(p, value);
                }
            }
            else if (ai is StupidAI stupid)
            {
                foreach (Player p in enemis)
                {
                    double value = 0;
                    if (count >= 2 && (ai.HasCrossbowEffect(p) || ai.HasSkill("luanji|luanji_jx", p)) && p == room.Current)
                    {
                        value = -10;
                    }
                    else
                    {
                        value = stupid.EvaluatePlayerStrength(p);
                        if (room.Current == p) value -= 1.5;
                    }
                    strenth.Add(p, value);
                }
            }

            enemis.Sort((x, y) => { return strenth[x] > strenth[y] ? -1 : 1; });
            foreach (Player p in enemis)
                if (p.FaceUp && !ai.WillSkipPlayPhase(p) && strenth[p] >= 0)
                    return new List<Player> { p };

            foreach (Player p in enemis)
                if (p.FaceUp && strenth[p] >= 0)
                    return new List<Player> { p };

            if (count >= 2 && ai.IsFriend(room.Current) && ai.HasCrossbowEffect(room.Current) && !ai.HasSkill("kuangcai", room.Current))
                return new List<Player> { room.Current };

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
                if (ai.GetPrioEnemies().Contains(current))
                    min_value *= 1.3;
            }

            if (throw_card)
            {
                List<int> ids= player.GetCards("h");
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
}