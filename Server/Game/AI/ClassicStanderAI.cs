using System;
using System.Collections.Generic;
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
                new WushengJXAI(),
                new HujiaAI(),
                new TuxiJXAI(),
                new YijiJXAI(),
            };
        }
    }

    public class JianxiongJXAI : SkillEvent
    {
        public JianxiongJXAI() : base("jianxiong_jx")
        {
        }
        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player to)
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
}