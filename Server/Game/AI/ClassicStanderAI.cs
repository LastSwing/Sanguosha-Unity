using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;

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
                new QingjianAI(),
                new LuoyiJXAI(),

                new JijiangAI(),
                new WushengJXAI(),
                new YijueAI(),
                new PaoxiaoJXAI(),
                new TishenAI(),
                new JizhiJXAI(),
                new LongdanJXAI(),
                new YajiaoAI(),
                new GuanxingJXAI(),
                new TieqiJXAI(),
                new JijieAI(),
                new JiyuanAI(),

                new LiyuAI(),
                new BiyueJXAI(),

                new ZhihengJXAI(),
                new JiuyuanAI(),
                new GuoseJXAI(),
                new KurouJXAI(),
                new QianxunJXAI(),
                new LianyingAI(),
                new JieyinJXAI(),
                new KejiJxAI(),
                new GongxinAI(),
                new BotuAI(),
                new FenweiAI(),
            };

            use_cards = new List<UseCard>
            {
                new YijueCardAI(),
                new ZhihengJXCardAI(),
                new GuoseCardAI(),
                new KurouJCardAI(),
                new JieyinJXCardAI(),
                new GongxinCardAI(),
                new YijiJCardAI(),
                new JijieCardAI(),
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
        public HujiaAI() : base("hujia")
        {
            key = new List<string> { "cardResponded%hujia", "skillInvoke:EightDiagram" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                Room room = ai.Room;
                if (choice.StartsWith("cardResponded"))
                {
                    string[] choices = choice.Split('%');
                    if (choices[1] == Name && ai.GetPlayerTendency(player) == "unknown" && choices[4] != "_nil_")
                    {
                        string prompt = choices[3];
                        Player cc = room.FindPlayer(choices[3].Split(':')[1]);
                        ai.UpdatePlayerRelation(player, cc, true);
                    }
                }
                else if (choice.StartsWith("skillInvoke"))
                {
                    string[] choices = choice.Split(':');
                    if (choices[1] == EightDiagram.ClassName && ai.GetPlayerTendency(player) == "unknown"
                        && choices[2] == "yes" && room.GetTag(EightDiagram.ClassName) is List<string> strs && strs[2] == Name)
                    {
                        string prompt = strs[1];
                        string caocao_name = prompt.Split(':')[1];
                        Player cc = room.FindPlayer(caocao_name);
                        ai.UpdatePlayerRelation(player, cc, true);
                    }
                }
            }
        }
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
                    damage.Damage = slash.Drank + 1 + slash.ExDamage;
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
                    damage.Damage = 1 + effect.ExDamage;
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
            key = new List<string> { "playerChosen:tuxi_jx" };
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
            List<int> ids= player.GetCards("h");
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            KeyValuePair<Player, int> key = ai.GetCardNeedPlayer(ids, null, Player.Place.PlaceHand, Name);
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

    public class YijiJCardAI : UseCard
    {
        public YijiJCardAI() : base(YijiJCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && ai.Self != player && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, true);
            }
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
            key = new List<string> { "cardChosen:fankui_jx" };
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
            key = new List<string> { "cardResponded%guicai_jx" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
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
            else if (pattern == "leiji_jx")
            {
                if (ai.IsFriend(player, judge_who))
                {
                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Spade && card.Suit != WrappedCard.CardSuit.Club
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 1) return true;
                }
                else
                    return HasSpade(ai, player) || HasClub(ai, player);
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

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            Room room = ai.Room;
            if (damage.From != null && damage.From.Alive && ai.HasSkill(Name, damage.To))
            {
                if (damage.Damage < damage.To.Hp)
                {
                    double value = 0;
                    if (ai is StupidAI _ai && _ai.NeedDamage(new DamageStruct(Name, damage.To, damage.From)) && damage.Damage == 1 && damage.From != damage.To)
                    {
                        value += 0.4 * 4;
                        if (ai.IsFriend(damage.From, damage.To))
                        {
                            bool lose = false;
                            foreach (int id in damage.From.GetEquips())
                            {
                                if (ai.GetKeepValue(id, damage.From) < 0)
                                {
                                    lose = true;
                                    break;
                                }
                            }
                            if (lose) value += 0.6 * 4;
                            else
                                value = 0;
                        }
                    }
                    else if (!ai.IsEnemy(damage.From, damage.To))
                    {
                        int count = damage.From.GetCardCount(true);
                        if (damage.Card != null)
                        {
                            foreach (int id in damage.Card.SubCards)
                                if (room.GetCardOwner(id) == damage.From && (room.GetCardPlace(id) == Player.Place.PlaceHand || room.GetCardPlace(id) == Player.Place.PlaceEquip))
                                    count--;
                        }

                        if (damage.Damage > damage.From.Hp)
                            value += 6;
                        else
                            value += 4 * 0.4 * damage.Damage;

                        if (count >= damage.Damage)
                            value += Math.Min(damage.Damage, count) * 0.6 * 2;

                        if (ai.IsFriend(damage.From))
                            score.Score -= value;
                        else if (ai.IsFriend(damage.To))
                            score.Score += value;
                    }
                }
            }

            return score;
        }
    }

    public class QingjianAI : SkillEvent
    {
        public QingjianAI() : base("qingjian")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<int> ids = player.GetCards("he");
            Room room = ai.Room;
            if (player.HasFlag("bingzheng") && ai.IsFriend(room.Current) && player.HandcardNum == player.Hp) return use;
            if (player.HasFlag("xiashu") && ai.IsEnemy(room.Current))
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, null, Player.Place.PlaceHand, Name);
                if (pair.Key != null && pair.Value > -1)
                {
                    use.Card = new WrappedCard(QingjianCard.ClassName) { Skill = Name };
                    use.Card.AddSubCards(player.GetCards("h"));
                    use.To.Add(pair.Key);
                    return use;
                }

                List<Player> friends = ai.FriendNoSelf;
                if (friends.Count > 0)
                {
                    ai.SortByDefense(ref friends, false);
                    foreach (Player p in friends)
                    {
                        if (!ai.HasSkill("zishu", p) && !ai.WillSkipPlayPhase(p))
                        {
                            use.Card = new WrappedCard(QingjianCard.ClassName) { Skill = Name };
                            use.Card.AddSubCards(player.GetCards("h"));
                            use.To.Add(p);
                            return use;
                        }
                    }
                    foreach (Player p in friends)
                    {
                        if (!ai.HasSkill("zishu", p))
                        {
                            use.Card = new WrappedCard(QingjianCard.ClassName) { Skill = Name };
                            use.Card.AddSubCards(player.GetCards("h"));
                            use.To.Add(p);
                            return use;
                        }
                    }

                    use.Card = new WrappedCard(QingjianCard.ClassName) { Skill = Name };
                    use.Card.AddSubCards(player.GetCards("h"));
                    use.To.Add(friends[0]);
                    return use;
                }
            }

            if (ids.Count == 0 || (room.Current != null && !ai.IsFriend(room.Current) && ai.GetOverflow(room.Current) > 0)) return use;
            
            Player target = null;
            do
            {
                List<Player> players = null;
                if (target != null)
                    players = new List<Player> { target };

                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, players, Player.Place.PlaceHand, Name);
                if (pair.Key != null && pair.Value > -1)
                {
                    ids.Remove(pair.Value);
                    target = pair.Key;
                    if (use.Card == null) use.Card = new WrappedCard(QingjianCard.ClassName) { Skill = Name };
                    use.Card.AddSubCard(pair.Value);
                    if (use.To.Count == 0) use.To.Add(target);
                }
                else
                    break;
            }
            while (ids.Count > 0);

            if (ids.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 0)
                {
                    if (target != null)
                        use.Card.AddSubCard(ids[0]);
                    else
                    {
                        List<Player> friends = ai.FriendNoSelf;
                        if (friends.Count > 0)
                        {
                            room.SortByActionOrder(ref friends);
                            use.Card = new WrappedCard(QingjianCard.ClassName) { Skill = Name };
                            use.Card.AddSubCard(ids[0]);
                            use.To.Add(friends[0]);
                        }
                    }
                }
            }
 
            return use;
        }
    }

    public class QingjianCardAI : UseCard
    {
        public QingjianCardAI() : base(QingjianCard.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
            }
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

    public class JijiangAI : SkillEvent
    {
        public JijiangAI() : base("jijiang")
        {
            key = new List<string> { "cardResponded%jijiang" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                Room room = ai.Room;
                string[] choices = choice.Split('%');
                if (choices[1] == Name && ai.GetPlayerTendency(player) == "unknown" && choices[4] != "_nil_")
                {
                    string prompt = choices[3];
                    Player cc = room.FindPlayer(choices[3].Split(':')[1]);
                    ai.UpdatePlayerRelation(player, cc, true);
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            List<Player> friends = ai.FriendNoSelf;
            return friends.Count == 0 ? 1 : 0;
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;
            if (pattern != Slash.ClassName) return result;

            CardUseStruct.CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
            if (reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                if (!Slash.IsAvailable(room, player) || player.HasFlag(string.Format("jijiang_activate_{0}", room.GetRoomState().GlobalActivateID)))
                    return result;
            }
            else
            {
                if (player.HasFlag(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID())))
                    return result;
            }

            bool check = false;
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                int count = 0;
                foreach (int id in player.GetCards("h"))
                    if (room.GetCard(id).Name.Contains(Slash.ClassName)) count++;
                foreach (int id in player.GetPile("wooden_ox"))
                    if (room.GetCard(id).Name.Contains(Slash.ClassName)) count++;

                foreach (Player p in friends)
                {
                    if (p.Kingdom == "shu" && ((ai.HasSkill("yajiao", p) && room.Current != p) || count == 0 || !ai.IsWeak(p)))
                    {
                        check = true;
                        break;
                    }
                }
            }
            else if (ai.FriendNoSelf.Count == 0)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.Kingdom == "shu")
                    {
                        check = true;
                        break;
                    }
                }
            }

            if (check)
            {
                WrappedCard jj = new WrappedCard(JijiangCard.ClassName) { Skill = Name, Mute = true };
                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    UserString = RoomLogic.CardToString(room, jj)
                };
                result.Add(slash);
                if (reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY || reason == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                {
                    WrappedCard f_slash = new WrappedCard(FireSlash.ClassName)
                    {
                        UserString = RoomLogic.CardToString(room, jj)
                    };
                    result.Add(f_slash);

                    WrappedCard t_slash = new WrappedCard(ThunderSlash.ClassName)
                    {
                        UserString = RoomLogic.CardToString(room, jj)
                    };
                    result.Add(t_slash);
                }
            }

            return result;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;
            if (!Slash.IsAvailable(room, player) || player.HasFlag(string.Format("jijiang_activate_{0}", room.GetRoomState().GlobalActivateID)))
                return result;

            bool check = false;

            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                int count = 0;
                foreach (int id in player.GetCards("h"))
                    if (room.GetCard(id).Name.Contains(Slash.ClassName)) count++;
                foreach (int id in player.GetPile("wooden_ox"))
                    if (room.GetCard(id).Name.Contains(Slash.ClassName)) count++;

                foreach (Player p in friends)
                {
                    if (p.Kingdom == "shu" && ((ai.HasSkill("yajiao", p) && room.Current != p) || count == 0 || !ai.IsWeak(p)))
                    {
                        check = true;
                        break;
                    }
                }
            }
            else if (ai.FriendNoSelf.Count == 0)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.Kingdom == "shu")
                    {
                        check = true;
                        break;
                    }
                }
            }

            if (check)
            {
                WrappedCard jj = new WrappedCard(JijiangCard.ClassName) { Skill = Name, Mute = true };
                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    UserString = RoomLogic.CardToString(room, jj)
                };
                result.Add(slash);
            }

            return result;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            if (prompt.StartsWith("@jijiang-target"))
            {
                Player liubei = room.FindPlayer(prompt.Split(':')[1], true);
                if (ai.IsFriend(liubei))
                {
                    List<Player> targets = new List<Player>();
                    foreach (string str in prompt.Split(':')[2].Split('+'))
                    {
                        Player target = room.FindPlayer(str);
                        if (target != null)
                            targets.Add(target);
                    }

                    if (targets.Count > 0)
                    {
                        List<ScoreStruct> scores = new List<ScoreStruct>();
                        foreach (WrappedCard slash in ai.GetCards(Slash.ClassName, player))
                        {
                            if (RoomLogic.IsCardLimited(room, player, slash, FunctionCard.HandlingMethod.MethodResponse)) continue;
                            foreach (Player enemy in targets)
                            {
                                if (ai.IsEnemy(enemy) && RoomLogic.IsProhibited(room, liubei, enemy, slash) == null
                                    && !ai.IsCancelTarget(slash, enemy, liubei) && ai.IsCardEffect(slash, enemy, liubei))
                                {
                                    ScoreStruct score = new ScoreStruct
                                    {
                                        Card = slash,
                                    };

                                    DamageStruct damage = new DamageStruct(slash, liubei, enemy);
                                    if (ai.HasArmorEffect(enemy, Vine.ClassName) && slash.Name == Slash.ClassName && liubei.HasWeapon(Fan.ClassName))
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
                                        ScoreStruct effect = ai.SlashIsEffective(damage.Card, liubei, enemy);
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

                        if (scores.Count > 0)
                        {
                            scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                            double adjust = ai.HasSkill("yajiao") && room.Current != player ? 2 : 0;
                            if (scores[0].Score + adjust > 0)
                            {
                                use.Card = scores[0].Card;
                                return use;
                            }
                        }
                    }
                }
            }
            else
            {
                Player liubei = room.FindPlayer(prompt.Split(':')[1], true);
                if (ai.IsFriend(liubei))
                {
                    object reason = room.GetTag("current_Slash");
                    DamageStruct damage = new DamageStruct();
                    if (reason is CardEffectStruct effect)
                    {
                        damage.From = effect.From;
                        damage.To = effect.To;
                        damage.Card = effect.Card;
                        damage.Damage = 1 + effect.ExDamage;
                        damage.Nature = DamageStruct.DamageNature.Normal;
                    }

                    List<WrappedCard> slashs = ai.GetCards(Slash.ClassName, player);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    if (score.Score < 0 && slashs.Count > 0 && room.Current != player && ai.HasSkill("yajiao"))
                        use.Card = slashs[0];
                    else if (score.Score < -5 && slashs.Count > 0)
                        use.Card = slashs[0];
                    else if (score.Score < 0 && slashs.Count > 1)
                        use.Card = slashs[0];
                }
            }

            return use;
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

    public class YijueAI : SkillEvent
    {
        public YijueAI() : base("yijue") { key = new List<string> { "skillInvoke:yijue:yes" }; }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(YijueCard.ClassName) && !player.IsNude())
                return new List<WrappedCard> { new WrappedCard(YijueCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                List<string> choices = new List<string>(choice.Split(':'));
                if (choices[1] == Name && ai.Self != player && choices[2] == "yes")
                {
                    Room room = ai.Room;
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.HasFlag(Name) && ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, true);
                }
            }
        }

        public override WrappedCard OnCardShow(TrustedAI ai, Player player, Player requestor, object data)
        {
            Room room = ai.Room;
            if (ai.IsFriend(requestor))
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    if (WrappedCard.IsRed(room.GetCard(id).Suit))
                        ids.Add(id);
                }

                if (ids.Count > 0 && ai.IsFriend(requestor))
                {
                    KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { requestor });
                    if (pair.Key != null) return room.GetCard(pair.Value);

                    ai.SortByKeepValue(ref ids, false);
                    return room.GetCard(ids[0]);
                }

                if (!ai.IsFriend(requestor))
                {
                    if (!RoomLogic.InMyAttackRange(room, requestor, player) && requestor.HandcardNum < 3 || !ai.IsWeak())
                    {
                        foreach (int id in player.GetCards("h"))
                        {
                            if (!WrappedCard.IsRed(room.GetCard(id).Suit))
                                return room.GetCard(id);
                        }
                    }
                    else if (ids.Count > 0)
                    {
                        ai.SortByKeepValue(ref ids, false);
                        return room.GetCard(ids[0]);
                    }
                }
            }

            return base.OnCardShow(ai, player, requestor, data);
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return data is Player target && ai.IsFriend(target);
        }
    }

    public class YijueCardAI : UseCard
    {
        public YijueCardAI() : base(YijueCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            int sub = -1;
            Room room = ai.Room;
            List<int> ids = player.GetCards("he");
            if (ids.Count > 0)
            {
                List<double> scores = ai.SortByKeepValue(ref ids, false);
                sub = ids[0];
                if (player.HandcardNum > 3)
                {
                    List<Player> enemies = ai.GetEnemies(player);
                    ai.SortByDefense(ref enemies, false);
                    foreach (Player p in enemies)
                    {
                        if (!p.IsKongcheng())
                        {
                            use.Card = card;
                            use.To = new List<Player> { p };
                            card.AddSubCard(sub);
                            return;
                        }
                    }
                }

                foreach (Player p in ai.GetFriends(player))
                {
                    if (player != p && p.IsWounded() && ai.GetOverflow(p) > 2)
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        card.AddSubCard(sub);
                        return;
                    }
                }

                if (scores[0] < 0)
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (!p.IsKongcheng())
                        {
                            use.Card = card;
                            use.To = new List<Player> { p };
                            card.AddSubCard(sub);
                            return;
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4.5;
        }
    }

    public class PaoxiaoJXAI : SkillEvent
    {
        public PaoxiaoJXAI() : base("paoxiao_jx") { }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (step == DamageStruct.DamageStep.Caused && damage.From != null
                && damage.From.Alive && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && damage.From.GetMark(Name) > 0)
            {
                damage.Damage += damage.From.GetMark(Name);
            }
        }
    }

    public class TishenAI : SkillEvent
    {
        public TishenAI() : base("tishen") { }
        /*
        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (card.Name.Contains(Slash.ClassName) && to != null && to.GetMark(Name) > 0 && !ai.IsCardEffect(card, to, from))
            {
                return ai.IsFriend(to) ? 1 : -1;
            }

            return 0;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
        */
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return player.GetLostHp() >= 2;
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
            List<int> ids= player.GetCards("h");
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
                else if (card.Name == Analeptic.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Peach.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return new List<WrappedCard> { slash };
                }
                else if (card.Name == Peach.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Analeptic.ClassName)
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
                else if (card.Name == Analeptic.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Peach.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return slash;
                }
                else if (card.Name == Peach.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Analeptic.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return slash;
                }
            }

            return null;
        }
    }

    public class YajiaoAI : SkillEvent
    {
        public YajiaoAI() : base("yajiao")
        {
            key = new List<string> { "playerChosen:yajiao", "cardChosen:yajiao" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                if (choice.Contains("playerChosen:yajiao") && choices[1] == Name && !player.HasFlag(Name))
                {
                    Player target = room.FindPlayer(choices[2]);
                    if (target != player && ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(target), 80);
                }
                else if (choice.Contains("cardChosen:yajiao") && ai.Self != player)
                {
                    int card_id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);
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
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            if (!player.HasFlag(Name))
            {
                return new List<Player> { player };
            }
            else
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in targets)
                {
                    ScoreStruct score = ai.FindCards2Discard(player, p, Name, "hej", FunctionCard.HandlingMethod.MethodDiscard);
                    scores.Add(score);
                }
                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0) return scores[0].Players;
                }
            }
            return new List<Player>();
        }
    }

    public class GuanxingJXAI : SkillEvent
    {
        public GuanxingJXAI() : base("guanxing_jx")
        {
            key = new List<string> { "guanxing_jxchose" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                List<int> ups = JsonUntity.StringList2IntList(new List<string>(choices[2].Split('+')));
                List<int> downs = JsonUntity.StringList2IntList(new List<string>(choices[3].Split('+')));
                ai.SetGuanxingResult(player, ups);
                if (ai.Self == player)
                {
                    Room room = ai.Room;
                    foreach (int id in ups)
                        room.GetCard(id).SetFlags("visible2" + player.Name);
                    foreach (int id in downs)
                        room.GetCard(id).SetFlags("visible2" + player.Name);
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
            if (player.Phase == Player.PlayerPhase.Start)
                return ai.Guanxing(ups);
            else
                return ai.GuanxingForNext(ups);
        }
    }

    public class TieqiJXAI : SkillEvent
    {
        public TieqiJXAI() : base("tieqi_jx")
        {
            key = new List<string> { "skillInvoke:tieqi_jx:yes" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                Room room = ai.Room;
                string[] choices = choice.Split(':');
                if (choices[1] == Name && choices[2] == "yes")
                {
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag("TieqiTarget"))
                        {
                            target = p;
                            break;
                        }
                    }
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player p)
                return !ai.IsFriend(p);

            return true;
        }
    }

    public class JijieAI : SkillEvent
    {
        public JijieAI() : base("jijie")
        {
            key = new List<string> { "Yiji:jijie" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                string[] strs = str.Split(':');
                Player target = room.FindPlayer(strs[3]);
                if (target == player) return;

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

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(JijieCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(JijieCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override Player OnYiji(TrustedAI ai, Player player, List<int> ids, ref int id)
        {
            List<Player> friends = ai.FriendNoSelf;
            id = ids[0];
            if (friends.Count > 0)
            {
                ai.Room.SortByActionOrder(ref friends);
                foreach (Player p in friends)
                    if (ai.HasSkill("qingjian", p)) return p;

                foreach (Player p in friends)
                    if (!ai.WillSkipPlayPhase(player) && !ai.HasSkill("zishu", p) && ai.HasSkill(TrustedAI.CardneedSkill, p)) return p;

                foreach (Player p in friends)
                    if (!ai.WillSkipPlayPhase(player) && !ai.HasSkill("zishu", p)) return p;

                foreach (Player p in friends)
                    if (!ai.HasSkill("zishu", p)) return p;
            }

            return null;
        }
    }

    public class JijieCardAI : UseCard
    {
        public JijieCardAI() : base(JijieCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 10;
        }
    }

    public class JiyuanAI : SkillEvent
    {
        public JiyuanAI() : base("jiyuan")
        {
            key = new List<string> { "skillInvoke:jiyuan:yes" };
        }
        
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                List<string> choices = new List<string>(choice.Split(':'));
                if (choices[1] == Name && choices[2] == "yes")
                {
                    Room room = ai.Room;
                    foreach (Player p in room.GetAlivePlayers())
                        if (p.HasFlag(Name) && ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, true);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return data is Player target && ai.IsFriend(target);
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
                    if (ai.FindCards2Discard(player, target, Name, "hej", FunctionCard.HandlingMethod.MethodGet).Score <= 0)
                        return false;

                    if (target.HandcardNum > 0 && RoomLogic.CanGetCard(room, player, target, "h")) return true;

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

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            if (ai.IsFriend(to) && to.JudgingArea.Count > 0)
                return base.OnCardsChosen(ai, from, to, flags, min, max, disable_ids);

            Room room = ai.Room;
            List<Player> targets = room.GetOtherPlayers(from);
            targets.Remove(from);

            WrappedCard card = new WrappedCard(Duel.ClassName);
            double worst = 0;
            foreach (Player p in targets)
            {
                if (!ai.IsCancelTarget(card, p, from) && ai.IsCardEffect(card, p, from))
                {
                    DamageStruct damage = new DamageStruct(card, from, p);
                    double value = ai.GetDamageScore(damage).Score;
                    if (value < worst)
                        worst = value;
                }
            }

            List<int> ids = to.GetCards("h");
            if (worst < -2 && ids.Count > 0)
            {
                Shuffle.shuffle(ref ids);
                return new List<int> { ids[0] };
            }

            return base.OnCardsChosen(ai, from, to, flags, min, max, disable_ids);
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

    public class ZhihengJXAI : SkillEvent
    {
        public ZhihengJXAI() : base("zhiheng_jx")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(ZhihengJCard.ClassName) && !player.IsNude())
                return new List<WrappedCard> { new WrappedCard(ZhihengJCard.ClassName) { Skill = Name, ShowSkill = Name } };

            return null;
        }
    }

    public class JiuyuanAI : SkillEvent
    {
        public JiuyuanAI() : base("jiuyuan")
        {
            key = new List<string> { "skillInvoke:jiuyuan:yes" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                List<string> choices = new List<string>(choice.Split(':'));
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player lord = null;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.GetRoleEnum() == Player.PlayerRole.Lord)
                        {
                            lord = p;
                            break;
                        }
                    }
                    ai.UpdatePlayerRelation(player, lord, true);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Player lord = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.GetRoleEnum() == Player.PlayerRole.Lord)
                {
                    if (ai.IsFriend(p))
                        return true;
                    else
                    {
                        lord = p;
                    }
                    break;
                }
            }
            if (player.GetRoleEnum() == Player.PlayerRole.Renegade && !ai.IsEnemy(lord) && lord.Hp == 1 && ai is StupidAI _ai && _ai.GetRolePitts(Player.PlayerRole.Rebel) > 0)
                return true;

            return false;
        }
    }

    public class ZhihengJXCardAI : UseCard
    {
        public ZhihengJXCardAI() : base(ZhihengJCard.ClassName)
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 0.3;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<int> cards = new List<int>();
            foreach (int id in player.GetEquips())
                if (ai.GetKeepValue(id, player) < 0 && RoomLogic.CanDiscard(room, player, player, id))
                    cards.Add(id);

            foreach (int id in player.GetCards("h"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    cards.Add(id);

            if (cards.Count > 0)
            {
                card.AddSubCards(cards);
                use.Card = card;
            }
        }
    }

    public class GuoseJXAI : SkillEvent
    {
        public GuoseJXAI() : base("guose_jx")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && card.Name != Indulgence.ClassName && card.GetEffectiveId() >= 0)
                return RoomLogic.GetCardSuit(ai.Room, card) == WrappedCard.CardSuit.Diamond ? 0.6 : 0;

            return 0;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.HasUsed(GuoseCard.ClassName)) return null;
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
                    ids.Add(id);

            foreach (int id in player.GetHandPile())
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
                    ids.Add(id);

            if (ids.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref ids, false);
                WrappedCard guose = new WrappedCard(GuoseCard.ClassName)
                {
                    Skill = Name
                };

                int sub = -1;
                if (values[0] < 0)
                    sub = ids[0];
                else
                {
                    values = ai.SortByUseValue(ref ids, false);
                    sub = ids[0];
                }

                foreach (Player p in ai.FriendNoSelf)
                {
                    if (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName))
                    {
                        guose.AddSubCard(sub);
                        return new List<WrappedCard> { guose };
                    }
                }

                if (Engine.GetCardUseValue(room.GetCard(sub).Name) < Engine.GetCardUseValue(Indulgence.ClassName))
                {
                    guose.AddSubCard(sub);
                    WrappedCard indulgence = new WrappedCard(Indulgence.ClassName);
                    indulgence.AddSubCard(sub);
                    indulgence = RoomLogic.ParseUseCard(ai.Room, indulgence);
                    indulgence.UserString = RoomLogic.CardToString(ai.Room, guose);
                    return new List<WrappedCard> { indulgence };
                }
            }

            return null;
        }
    }

    public class GuoseCardAI : UseCard
    {
        public GuoseCardAI() : base(GuoseCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is StupidAI _ai)
                {
                    Player target = use.To[0];
                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        if (RoomLogic.PlayerContainsTrick(room, target, Indulgence.ClassName))
                            ai.UpdatePlayerRelation(player, target, true);
                    }
                }
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            ai.SortByDefense(ref friends, false);
            foreach (Player p in friends)
            {
                if (RoomLogic.PlayerContainsTrick(ai.Room, p, Indulgence.ClassName))
                {
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class KurouJXAI : SkillEvent
    {
        public KurouJXAI() : base("kurou_jx")
        { }

        public static int FindKurouCard(TrustedAI ai, Player player)
        {
            int result = -1;

            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                if ((ai.IsCard(id, Peach.ClassName, player) && player.IsWounded())
                    || (player.Hp == 1 && ai.IsCard(id, Analeptic.ClassName, player) && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) == 1)) continue;
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);
            }
            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids, false);
                if (ai.GetKeepValue(ids[0], player) < 0)
                    return ids[0];

                Dictionary<int, double> values = new Dictionary<int, double>();
                List<int> equips = new List<int>();
                foreach (int id in ids)
                {
                    WrappedCard _c = room.GetCard(id);
                    if (Engine.GetFunctionCard(_c.Name) is EquipCard)
                        equips.Add(id);
                    else
                        values[id] = ai.GetKeepValue(id, player);
                }
                List<int> weapons = new List<int>(), armors = new List<int>(), ohorse = new List<int>(), dhorse = new List<int>(), speacial = new List<int>(), treasure = new List<int>();
                foreach (int id in equips)
                {
                    WrappedCard _c = room.GetCard(id);
                    double basic = Engine.GetCardUseValue(_c.Name);
                    UseCard card_event = Engine.GetCardUsage(_c.Name);
                    if (card_event != null)
                        basic += card_event.CardValue(ai, player, true, _c, Player.Place.PlaceEquip);
                    values[id] = basic;
                    if (Engine.GetFunctionCard(_c.Name) is Weapon)
                        weapons.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is Armor)
                        armors.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is OffensiveHorse)
                        ohorse.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is DefensiveHorse)
                        dhorse.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is SpecialEquip)
                        speacial.Add(id);
                    else
                        treasure.Add(id);
                }

                if (speacial.Count > 0)
                {
                    if (ohorse.Count > 0)
                        return ohorse[0];
                    if (dhorse.Count > 0)
                        return dhorse[0];
                }
                else
                {
                    if (ohorse.Count > 1)
                    {
                        ohorse.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                        return ohorse[0];
                    }
                    if (dhorse.Count > 1)
                    {
                        dhorse.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                        return dhorse[0];
                    }
                }

                if (weapons.Count > 1)
                {
                    weapons.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                    return weapons[0];
                }

                if (treasure.Count > 1)
                {
                    treasure.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                    return treasure[0];
                }

                ids.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                if (ai.GetUseValue(ids[0], player) < 6)
                    return ids[0];
            }


            return result;
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(KurouJCard.ClassName))
            {
                Room room = ai.Room;
                bool use = false;
                if (player.GetLostHp() <= 1 || (player.IsWounded() && player.HasArmor(SilverLion.ClassName))
                    || (player.Hp == 1 && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) > 0 && player.GetCards("he").Count > 1))
                    use = true;

                if (use)
                {
                    int id = FindKurouCard(ai, player);
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
                    if (fcard is Duel && ai.IsFriend(from) && !ai.HasSkill("wuyan", from)) return 0;
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
            key = new List<string> { "playerChosen:lianying" };
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

    public class JieyinJXAI : SkillEvent
    {
        public JieyinJXAI() : base("jieyin_jx") { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is StupidAI _ai)
                {
                    Player target = use.To[0];
                    if (ai.GetPlayerTendency(target) != "unknown"
                        && (target.GetLostHp() > 0 || Engine.GetFunctionCard(room.GetCard(use.Card.GetEffectiveId()).Name) is EquipCard))
                    {
                        ai.UpdatePlayerRelation(player, target, true);
                    }
                }
            }
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsNude() && !player.HasUsed(JieyinJCard.ClassName))
            {
                return new List<WrappedCard> { new WrappedCard(JieyinJCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return "put";
        }
    }

    public class JieyinJXCardAI : UseCard
    {
        public JieyinJXCardAI() : base(JieyinJCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            ai.SortByDefense(ref friends, false);
            Room room = ai.Room;
            foreach (Player p in friends)
            {
                if (p.IsMale() && p.Hp != player.Hp && (p.GetLostHp() > 0 || player.GetLostHp() > 0))
                {
                    foreach (int id in player.GetEquips())
                    {
                        WrappedCard equip = room.GetCard(id);
                        EquipCard fcard = (EquipCard)Engine.GetFunctionCard(equip.Name);
                        if (p.GetEquip((int)fcard.EquipLocation()) == -1 && RoomLogic.CanPutEquip(p, equip))
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }
            }

            foreach (Player p in friends)
            {
                if (p.IsMale() && p.Hp != player.Hp)
                {
                    foreach (int id in player.GetEquips())
                    {
                        WrappedCard equip = room.GetCard(id);
                        EquipCard fcard = (EquipCard)Engine.GetFunctionCard(equip.Name);
                        if (p.GetEquip((int)fcard.EquipLocation()) == -1 && RoomLogic.CanPutEquip(p, equip))
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }
            }

            List<int> cards = player.GetCards("h");
            if (cards.Count > 0)
            {
                ai.SortByUseValue(ref cards, false);
                foreach (int id in cards)
                    if (Engine.GetFunctionCard(room.GetCard(id).Name) is EquipCard)
                        return;

                foreach (Player p in friends)
                {
                    if (p.IsMale() && p.Hp != player.Hp && (p.GetLostHp() > 0 || player.GetLostHp() > 0))
                    {
                        card.AddSubCard(cards[0]);
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!ai.IsEnemy(p) && p.IsMale() && p.Hp > player.Hp && p.GetLostHp() > 0)
                {
                    foreach (int id in player.GetEquips())
                    {
                        WrappedCard equip = room.GetCard(id);
                        EquipCard fcard = (EquipCard)Engine.GetFunctionCard(equip.Name);
                        if (p.GetEquip((int)fcard.EquipLocation()) == -1 && RoomLogic.CanPutEquip(p, equip))
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }
            }
            
            if (cards.Count > 0)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (!ai.IsEnemy(p) && p.IsMale() && p.Hp > player.Hp && p.GetLostHp() > 0)
                    {
                        card.AddSubCard(cards[0]);
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }
            }
        }
    }

    public class KejiJxAI : SkillEvent
    {
        public KejiJxAI() : base("keji_jx") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class GongxinAI : SkillEvent
    {
        public GongxinAI() : base("gongxin") { key = new List<string> { "gongxin:gongxin" }; }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    Player target = room.FindPlayer(strs[3], true);
                    int id = int.Parse(strs[4]);
                    List<int> ids = ai.GetKnownCards(target);

                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        if (id >= 0)
                            ai.UpdatePlayerRelation(player, target, false);
                        else if (ids.Count > 0)
                        {
                            bool friend = false;
                            foreach (int card_id in ids)
                            {
                                if (room.GetCard(card_id).Suit == WrappedCard.CardSuit.Heart)
                                {
                                    friend = true;
                                    break;
                                }
                            }
                            if (friend)
                                ai.UpdatePlayerRelation(player, target, true);
                        }
                    }
                }
            }
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(GongxinCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(GongxinCard.ClassName) { Skill = Name } };

            return null;
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
                List<int> ids = new List<int>();
                foreach (int id in card_ids)
                    if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart)
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    if (ids.Count > 1)
                        ids.Sort((x, y) => { return ai.GetKeepValue(x, target) > ai.GetKeepValue(y, target) ? -1 : 1; });

                    return ids[0];
                }
            }

            return -1;
        }
        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            if (data is int id && ai.GetKnownCardsNums(ExNihilo.ClassName, "he", player) > 0)
            {
                if (ai.GetUseValue(id, player) > 5)
                    return "piletop";
            }

            return "disacard";
        }
    }

    public class GongxinCardAI : UseCard
    {
        public GongxinCardAI() : base(GongxinCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is StupidAI _ai)
                {
                    Player target = use.To[0];
                    if (target.GetRoleEnum() == Player.PlayerRole.Lord || ai.GetPlayerTendency(target) == "loyalist")
                    {
                        _ai.UpdatePlayerIntention(player, "reble", 50);
                    }
                    else if (ai.GetPlayerTendency(target) == "rebel")
                        _ai.UpdatePlayerIntention(player, "loyalist", 50);
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            foreach (Player p in enemies)
            {
                if (p.HandcardNum == 0) continue;
                List<int> ids = ai.GetKnownCards(p);
                if (p.HandcardNum == ids.Count)
                {
                    foreach (int id in ids)
                    {
                        if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart && ai.GetKeepValue(id, p) > 3)
                        {
                            use.To = new List<Player> { p };
                            use.Card = card;
                            return;
                        }
                    }

                    continue;
                }

                use.To = new List<Player> { p };
                use.Card = card;
                return;
            }

            List<Player> targets = room.GetOtherPlayers(player);
            ai.SortByHandcards(ref targets);
            foreach (Player p in targets)
            {
                if (p.HandcardNum == 0) continue;
                List<int> ids = ai.GetKnownCards(p);
                if (p.HandcardNum != ids.Count)
                {
                    use.To = new List<Player> { p };
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

    public class BotuAI : SkillEvent
    {
        public BotuAI() : base("botu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
    }

    public class FenweiAI : SkillEvent
    {
        public FenweiAI() : base("fenwei")
        {
            key = new List<string> { "playerChosen:fenwei" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                Room room = ai.Room;
                List<string> choices = new List<string>(choice.Split(':'));
                if (choices[1] == Name && ai.Self != player && room.GetTag(Name) is CardUseStruct use)
                {
                    List<Player> targets = new List<Player>();
                    foreach (string name in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(name));

                    if (use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == SavageAssault.ClassName)
                    {
                        foreach (Player p in targets)
                            if (ai.GetPlayerTendency(p) != "unknown") ai.UpdatePlayerRelation(player, p, true);
                    }
                    else if (use.Card.Name == GodSalvation.ClassName || use.Card.Name == AmazingGrace.ClassName)
                    {
                        foreach (Player p in targets)
                            if (ai.GetPlayerTendency(p) != "unknown") ai.UpdatePlayerRelation(player, p, false);
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            List<Player> result = new List<Player>();
            if (ai.IsSituationClear() && room.GetTag(Name) is CardUseStruct use)
            {
                if (use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == SavageAssault.ClassName)
                {
                    foreach (Player p in use.To)
                        if (ai.IsFriend(p)) result.Add(p);
                }
                else if (use.Card.Name == GodSalvation.ClassName || use.Card.Name == AmazingGrace.ClassName)
                {
                    foreach (Player p in use.To)
                        if (!ai.IsFriend(p)) result.Add(p);
                }
            }

            return result;
        }
    }
}