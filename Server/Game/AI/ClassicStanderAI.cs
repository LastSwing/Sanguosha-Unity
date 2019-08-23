using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class ClassicStanderAI : AIPackage
    {
        public ClassicStanderAI() : base("ClassicStander")
        {
            events = new List<SkillEvent>
            {
                new JianxiongJXAI(),
                new HujiaAI(),
                new TuxiJXAI(),
                new YijiJXAI(),
                new LuoshenJXAI(),
                new GuicaiJXAI(),
                new FankuiJXAI(),
                new GanglieJXAI(),
                new LuoyiJXAI(),

                new WushengJXAI(),
                new JizhiJXAI(),
                new LongdanJXAI(),
                new YajiaoAI(),
                new GuanxingJXAI(),
                new TieqiJXAI(),

                new LiyuAI(),
                new BiyueJXAI(),

                new KurouJXAI(),
                new QianxunJXAI(),
                new LianyingAI(),
            };

            use_cards = new List<UseCard>
            {
                new KurouJCardAI(),
            };
        }
    }

    public class JianxiongJXAI : SkillEvent
    {
        public JianxiongJXAI() : base("jianxiong_jx")
        {
        }
        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            Room room = ai.Room;
            double value = 0;
            if (!RoomLogic.IsVirtualCard(room, card))
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
                score.Score = 1.2;
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (!(fcard is SkillCard))
                {
                    foreach (int id in damage.Card.SubCards)
                    {
                        double value = Math.Max(ai.GetUseValue(id, damage.To, Player.Place.PlaceHand), ai.GetKeepValue(id, damage.To, Player.Place.PlaceHand));
                        score.Score += value * 0.45;
                    }

                    if (ai.WillSkipPlayPhase(damage.To))
                        score.Score /= 3;

                    if (damage.Damage >= damage.To.Hp)
                        score.Score /= 2;

                    if (damage.From != null && damage.From == damage.To && score.Score > 0)
                        score.Score /= 4;
                }

                if (damage.Damage > 1) score.Score /= 1.5;
                if (ai.WillSkipPlayPhase(damage.To)) score.Score /= 1.5;

                if (ai.IsEnemy(damage.To))
                    score.Score = -score.Score;
                else
                    score.Score -= 1.5;
            }

            return score;
        }
    }

    public class HujiaAI : SkillEvent
    {
        public HujiaAI() : base("hujia") { }

        public override double GetSkillAdjustValue(TrustedAI ai, Player player)
        {
            return 5;
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;
            WrappedCard hujia = new WrappedCard(HujiaCard.ClassName) { Skill = Name };
            if (pattern == "Jink" && !player.HasFlag(string.Format("hujia_{0}", room.GetRoomState().GetCurrentResponseID())))
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.Kingdom == "wei" && ai.HasArmorEffect(p, EightDiagram.ClassName))
                    {
                        WrappedCard jink = new WrappedCard(Jink.ClassName) { Skill = Name, UserString = RoomLogic.CardToString(room, hujia) };
                        return new List<WrappedCard> { jink };
                    }
                }

                List<int> ids = player.GetCards("he");
                ids.AddRange(player.GetHandPile());
                int jink_count = 0;
                foreach (int id in ids)
                    if (ai.IsCard(id, pattern, player))
                        jink_count++;

                if (jink_count == 0)
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (ai.IsFriend(p) && p.Kingdom == "wei")
                        {
                            WrappedCard jink = new WrappedCard(Jink.ClassName) { Skill = Name, UserString = RoomLogic.CardToString(room, hujia) };
                            return new List<WrappedCard> { jink };
                        }
                    }
                }
            }
            return result;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            Player caocao = room.FindPlayer(prompt.Split(':')[1]);
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            if (caocao != null && ai.IsFriend(caocao))
            {
                object reason = room.GetTag("current_Jink");
                DamageStruct damage = new DamageStruct();
                if (reason is SlashEffectStruct slash)
                {
                    damage.From = slash.From;
                    damage.To = slash.To;
                    damage.Card = slash.Slash;
                    damage.Damage = slash.Drank + 1;
                    damage.Nature = DamageStruct.DamageNature.Normal;
                    if (damage.Card.Name == FireSlash.ClassName)
                        damage.Nature = DamageStruct.DamageNature.Fire;
                    else if (damage.Card.Name == ThunderSlash.ClassName)
                        damage.Nature = DamageStruct.DamageNature.Thunder;
                }
                else if (reason is CardEffectStruct effect)
                {
                    damage.From = effect.From;
                    damage.To = effect.To;
                    damage.Card = effect.Card;
                    damage.Damage = 1;
                    damage.Nature = DamageStruct.DamageNature.Normal;
                }

                List<WrappedCard> jinks = ai.GetCards("Jink", player);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score < -5 && jinks.Count > 0)
                    use.Card = jinks[0];
                else if (score.Score < 0 && jinks.Count > 1)
                    use.Card = jinks[0];
            }

            return use;
        }
    }

    public class TuxiJXAI : SkillEvent
    {
        public TuxiJXAI() : base("tuxi_jx")
        {
            key = new List<string> { "playerChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    List<Player> targets = new List<Player>();
                    foreach (string general in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(general, true));

                    foreach (Player p in targets)
                        if (ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, false);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            List<Player> result = FindTuxiTargets(ai, player, max);
            return result;
        }

        public static List<Player> FindTuxiTargets(TrustedAI ai, Player player, int max)
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
                if (result.Count >= max) break;
            }

            if (result.Count < max)
            {
                foreach (Player p in enemies)
                {
                    if (result.Contains(p) || !RoomLogic.CanGetCard(room, player, p, "h")) continue;
                    if (ai.HasSkill("jijiu|qingnang|leiji|jieyin|beige|kanpo|liuli|qiaobian|zhiheng|guidao|tianxiang|lijian", p))
                        result.Add(p);

                    if (result.Count >= max) break;
                }
            }
            if (result.Count < max)
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

                    if (result.Count >= max) break;
                }
            }

            return result;
        }
    }

    public class YijiJXAI : SkillEvent
    {
        public YijiJXAI() : base("yiji_jx")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            List<int> ids = new List<int>(player.HandCards);
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            KeyValuePair<Player, int> key = ai.GetCardNeedPlayer(ids);
            if (key.Key != null && key.Value >= 0 && ai.Room.Current == key.Key)
            {
                WrappedCard wrapped = new WrappedCard(YijiJCard.ClassName) { Skill = Name };
                wrapped.AddSubCard(key.Value);
                use.Card = wrapped;
                use.To.Add(key.Key);
                return use;
            }
            if (player.HandcardNum <= 2)
                return use;

            if (key.Key != null && key.Value >= 0)
            {
                WrappedCard wrapped = new WrappedCard(YijiJCard.ClassName) { Skill = Name };
                wrapped.AddSubCard(key.Value);
                use.Card = wrapped;
                use.To.Add(key.Key);
                return use;
            }

            return use;
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
                    score.Score = 4 * damage.Damage;

                double numerator = 0.6;
                if (!ai.WillSkipPlayPhase(damage.To)) numerator += 0.3;
                foreach (Player p in ai.GetFriends(damage.To))
                    if (p != damage.To && !ai.WillSkipPlayPhase(p))
                        numerator += 0.1;
                score.Score *= numerator;

                if (ai.IsEnemy(damage.To))
                    score.Score = -score.Score;
                else
                    score.Score -= 1.5;
            }

            return score;
        }
    }

    public class LuoshenJXAI : SkillEvent
    {
        public LuoshenJXAI() : base("luoshen_jx") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class FankuiJXAI : SkillEvent
    {
        public FankuiJXAI() : base("fankui_jx")
        {
            key = new List<string> { "cardChosen" };
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
                else if (!ai.IsFriend(damage.From, damage.To) && damage.From.GetCardCount(true) >= damage.Damage)
                {
                    score.Score += damage.Damage;
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
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    int id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);

                    if (target != player && ai.GetPlayerTendency(target) != "unknown" && ai.HasArmorEffect(target, SilverLion.ClassName) && id == target.Armor.Key && target.IsWounded())
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        //public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        //{
        //    return ai.FindCards2Discard(from, to, "he", FunctionCard.HandlingMethod.MethodGet, 1, true, disable_ids).Ids;
        //}
    }

    public class GuicaiJXAI : SkillEvent
    {
        public GuicaiJXAI() : base("guicai_jx")
        {
            key = new List<string> { "cardResponded" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
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
                int id = ai.GetRetrialCardId(player.GetCards("he"), judge);
                if (id >= 0)
                    use.Card = ai.Room.GetCard(id);
            }

            return use;
        }

        public static bool HasSpade(TrustedAI ai, Player player)
        {
            Room room = ai.Room;

            List<int> ids = ai.GetKnownCards(player);
            ids.AddRange(player.GetEquips());
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

    public class GanglieJXAI : SkillEvent
    {
        public GanglieJXAI() : base("ganglie_jx")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is DamageStruct damage)
            {
                Player target = damage.From;
                if (target == null || target == player)
                    return false;

                return !ai.IsFriend(target);
            }

            return false;
        }
    }

    public class LuoyiJXAI : SkillEvent
    {
        public LuoyiJXAI() : base("luoyi_jx")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is string str && str == "@luoyi-get" && player.GetTag(Name) is List<int> ids)
            {
                Room room = ai.Room;
                int count = 0;
                bool weapon = false;
                bool slash = false;
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!weapon && fcard is Weapon && ai.GetKnownCardsNums("Weapon", "he", player) == 0)
                    {
                        count++;
                        weapon = true;
                    }
                    else if (!slash && fcard is Slash && ai.GetKnownCardsNums("Slash", "he", player) == 0)
                    {
                        count++;
                        slash = true;
                    }
                    else if (fcard is Duel)
                        count++;
                }

                if (count == 0)
                    return false;
                else if (count == 1 && ai.IsWeak(player))
                    return false;
            }

            return true;
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (step == DamageStruct.DamageStep.Caused && damage.From != null
                && damage.From.Alive && damage.From.GetMark("@luoyi") > 0 && damage.Card != null && (damage.Card.Name == Duel.ClassName || damage.Card.Name.Contains(Slash.ClassName)
                && !damage.Chain && !damage.Transfer))
            {
                damage.Damage++;
            }
        }
    }


    public class WushengJXAI : SkillEvent
    {
        public WushengJXAI() : base("wusheng_jx")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = player.GetCards("he");
            ids.AddRange(player.GetHandPile());

            foreach (int id in ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if (WrappedCard.IsRed(card.Suit))
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return new List<WrappedCard> { slash };
                }
            }
            return null;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            Room room = ai.Room;
            if (ai.HasSkill("nuzhan", player) && targets.Count > 0)
            {
                WrappedCard wrapped = room.GetCard(card.GetEffectiveId());
                FunctionCard fcard = Engine.GetFunctionCard(wrapped.Name);
                if (fcard is TrickCard) return 2;
                if (fcard is EquipCard) return 1.5;
            }
            else if (targets.Count > 0)
                return 0;

            return -1;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (card.HasFlag("using")) return null;
            if (WrappedCard.IsRed(card.Suit))
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                slash.AddSubCard(card);
                slash = RoomLogic.ParseUseCard(room, slash);
                return slash;
            }

            return null;
        }
    }

    public class JizhiJXAI : SkillEvent
    {
        public JizhiJXAI() : base("jizhi_jx")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            int id = int.Parse(pattern);
            string class_name = room.GetCard(id).Name;
            if (class_name.Contains(Slash.ClassName)) class_name = Slash.ClassName;

            if (ai.GetOverflow(player) > 0)
            {
                int count = ai.GetKnownCardsNums(class_name, "he", player);
                if (class_name == Peach.ClassName && count < player.GetLostHp())
                    return new List<int>();

                if (count > 1)
                {
                    return new List<int> { id };
                }
            }

            return new List<int>();
        }
    }

    public class LongdanJXAI : SkillEvent
    {
        public LongdanJXAI() : base("longdan_jx")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>(player.HandCards);
            ids.AddRange(player.GetHandPile());

            foreach (int id in ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if (card.Name == Jink.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return new List<WrappedCard> { slash };
                }
            }
            return null;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand)
            {
                if (card.Name == Jink.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return slash;
                }
                else if (card.Name.Contains(Slash.ClassName))
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
            }

            return null;
        }
    }

    public class YajiaoAI : SkillEvent
    {
        public YajiaoAI() : base("yajiao")
        {
            key = new List<string> { "playerChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);
                    if (target != player && ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(target), 80);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            return new List<Player> { player };
        }
    }

    public class GuanxingJXAI : SkillEvent
    {
        public GuanxingJXAI() : base("guanxing_jx")
        {
            key = new List<string> { "guanxingchose" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                Player who = ai.Room.FindPlayer(choices[1]);
                if (who != null)
                {
                    List<int> ups = JsonUntity.StringList2IntList(new List<string>(choices[2].Split('+')));
                    ai.SetGuanxingResult(who, ups);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack() && !ai.WillShowForDefence() && player.JudgingArea.Count == 0) return false;
            return true;
        }
        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            return ai.Guanxing(ups);
        }
    }

    public class TieqiJXAI : SkillEvent
    {
        public TieqiJXAI() : base("tieqi_jx")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player p)
                return !ai.IsFriend(p);

            return true;
        }
    }

    public class LiyuAI : SkillEvent
    {
        public LiyuAI() : base("liyu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                Room room = ai.Room;
                if (ai.IsFriend(target))
                    return true;
                else
                {
                    List<Player> targets = room.GetOtherPlayers(target);
                    targets.Remove(player);

                    WrappedCard card = new WrappedCard(Duel.ClassName);
                    double worst = 0;
                    foreach (Player p in targets)
                    {
                        if (!ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                        {
                            DamageStruct damage = new DamageStruct(card, player, p);
                            double value = ai.GetDamageScore(damage).Score;
                            if (value < worst)
                                worst = value;
                        }
                    }

                    if (worst < -7)
                        return false;
                }
            }

            return true;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsEnemy(p)) return new List<Player> { p };

            ai.SortByDefense(ref targets);
            return new List<Player> { targets[0] };
        }
    }

    public class BiyueJXAI : SkillEvent
    {
        public BiyueJXAI() : base("biyue_jx")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class KurouJXAI : SkillEvent
    {
        public KurouJXAI() : base("kurou_jx")
        { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(KurouJCard.ClassName))
            {
                Room room = ai.Room;
                bool use = false;
                if (player.GetLostHp() == 0 || (player.IsWounded() && player.HasArmor(SilverLion.ClassName))
                    || (player.Hp == 1 && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) > 0 && player.GetCards("he").Count > 1))
                    use = true;

                if (use)
                {
                    int id = LijianAI.FindLijianCard(ai, player);
                    if (id >= 0)
                    {
                        WrappedCard kurou = new WrappedCard(KurouJCard.ClassName) { Skill = Name, ShowSkill = Name };
                        kurou.AddSubCard(id);
                        return new List<WrappedCard> { kurou };
                    }
                }
            }

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            double value = 0;
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard is Slash && WrappedCard.IsRed(card.Suit))
            {
                if (ai.HasSkill("zhaxiang"))
                    value++;
                if (player.GetMark("zhaxiang") > 0)
                    value++;
            }

            return value;
        }
    }

    public class KurouJCardAI : UseCard
    {
        public KurouJCardAI() : base(KurouJCard.ClassName)
        { }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 3;
        }
    }


    public class QianxunJXAI : SkillEvent
    {
        public QianxunJXAI() : base("qianxun_jx") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardEffectStruct effect)
            {
                if (player.Hp == 1 && effect.From != null && ai.IsEnemy(effect.From))
                {
                    if (effect.Card.Name == Duel.ClassName || effect.Card.Name == SavageAssault.ClassName)
                    {
                        if (player.HasWeapon(Spear.ClassName) && player.HandcardNum > 1) return false;
                        foreach (int id in player.GetCards("h"))
                            if (ai.IsCard(id, Slash.ClassName, player)) return false;
                    }

                    if (ai.FriendNoSelf.Count == 0 && player.HandcardNum > 2 && RoomLogic.InMyAttackRange(ai.Room, effect.From, player))
                    {
                        foreach (int id in player.GetCards("h"))
                            if (ai.IsCard(id, Peach.ClassName, player) || ai.IsCard(id, Peach.ClassName, player)) return false;
                    }
                }
            }

            return true;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            double value = 0;
            if (fcard is TrickCard && from != to && targets.Count == 1 && ai.HasSkill("lianying", to) && to.HandcardNum > 1 && ai is StupidAI _ai)
            {
                if (ai.IsFriend(to))
                {
                    value = 1.5 * Math.Min(ai.GetFriends(to).Count, to.HandcardNum);
                }
                else if (ai.IsEnemy(to))
                {
                    value -= 1.5 * Math.Min(ai.GetFriends(to).Count, to.HandcardNum);
                }
                else if (ai.GetPlayerTendency(to) == "unknown" && ai.Self == from && from.GetRoleEnum() != Player.PlayerRole.Renegade)
                {
                    if (from.GetRoleEnum() == Player.PlayerRole.Rebel)
                    {
                        int unknown_enemies = _ai.GetRolePitts(Player.PlayerRole.Loyalist) + 1 - ai.GetEnemies(from).Count;
                        int unknown_friends = _ai.GetRolePitts(Player.PlayerRole.Rebel) - ai.GetFriends(from).Count;
                        if (_ai.Process.Contains("<") && _ai.GetRolePitts(Player.PlayerRole.Renegade) > 0) unknown_enemies++;
                        return unknown_friends - unknown_enemies;
                    }
                    else
                    {
                        int unknown_friends = _ai.GetRolePitts(Player.PlayerRole.Loyalist) + 1 - ai.GetFriends(from).Count;
                        int unknown_enemies = _ai.GetRolePitts(Player.PlayerRole.Rebel) - ai.GetEnemies(from).Count;
                        if (_ai.Process.Contains(">") && _ai.GetRolePitts(Player.PlayerRole.Renegade) > 0) unknown_enemies++;
                        return unknown_friends - unknown_enemies;
                    }
                }
            }
            return value;
        }
    }

    public class LianyingAI : SkillEvent
    {
        public LianyingAI() : base("lianying")
        {
            key = new List<string> { "playerChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    List<Player> targets = new List<Player>();
                    foreach (string general in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(general, true));

                    foreach (Player p in targets)
                    {
                        if (p != player && ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 80);
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> friends = ai.FriendNoSelf, result = new List<Player> { player };
            ai.SortByDefense(ref friends, false);
            for (int i = 0; i < Math.Min(friends.Count, max - 1); i++)
                result.Add(friends[i]);

            return result;
        }
    }
}