using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class NaturefourAI : AIPackage
    {
        public NaturefourAI() : base("Naturefour")
        {
            events = new List<SkillEvent>
            {
                new LeijiJXAI(),
                new LiegongJXAI(),
                new KuangguJXAI(),
                new QimouAI(),
                new FenjiJXAI(),
                new ZhibaAI(),
                new TianxiangJXAI(),
            };

            use_cards = new List<UseCard>
            {
                new ZhibaCardAI(),
                new QimouCardAI(),
            };
        }
    }
    public class LeijiJXAI : SkillEvent
    {
        public LeijiJXAI() : base("leiji_jx")
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

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForAttack() || !ai.WillShowForDefence()) return new List<Player>();

            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in enemies)
            {
                if (ai.HasSkill("hongyan", p)) continue;
                DamageStruct damage = new DamageStruct(Name, player, p, 2, DamageStruct.DamageNature.Thunder);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score > 0 && score.DoDamage)
                    score.Score += ai.ChainDamage(damage);
                scores.Add(score);
            }

            if (scores.Count > 0)
            {
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 0)
                    return new List<Player> { scores[0].Damage.To };
            }

            return new List<Player>();
        }
    }

    public class LiegongJXAI : SkillEvent
    {
        public LiegongJXAI() : base("liegong_jx")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsEnemy(target))
                return true;

            return false;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (choice.Contains("nojink"))
                return "nojink";
            else if (choice.Contains("damage"))
                return "damage";

            return base.OnChoice(ai, player, choice, data);
        }
    }
    public class KuangguJXAI : SkillEvent
    {
        public KuangguJXAI() : base("kuanggu_jx")
        {}
        
        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (ai.HasCrossbowEffect(player))
            {
                foreach (Player p in ai.GetEnemies(player))
                    if (RoomLogic.InMyAttackRange(ai.Room, player, p))
                        return "draw";
            }

            return "recover";
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            Player from = damage.From;
            Player to = damage.To;
            Room room = ai.Room;
            ScoreStruct score = new ScoreStruct();
            if (from != null && ai.HasSkill(Name, from) && (to == from && from.Hp > damage.Damage || RoomLogic.DistanceTo(room, from, to) == 1))
            {
                double value = 0;
                if (to == from)
                    value = damage.Damage * 4;
                else
                {
                    int heal = Math.Min(from.GetLostHp(), damage.Damage);
                    value = heal * 4 + (damage.Damage - heal) * 1.5;
                }
                if (ai.IsFriend(from))
                    score.Score = value;
                else if (ai.IsEnemy(from))
                    score.Score = -value;
            }

            return score;
        }
    }
    public class QimouAI : SkillEvent
    {
        public QimouAI() : base("qimou") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("@mou") > 0 && player.Hp >= 2)
            {
                Room room = ai.Room;
                if (player.IsWounded() && ai.GetKnownCardsNums(Peach.ClassName, "he", player) > 0) return new List<WrappedCard>();
                double value = 0.6 * room.GetOtherPlayers(player).Count
                    * (ai.GetKnownCardsNums(SavageAssault.ClassName, "he", player) + ai.GetKnownCardsNums(ArcheryAttack.ClassName, "he", player));
                if (ai.GetEnemies(player).Count > 1 || ai.IsSituationClear())
                    value += ai.GetKnownCardsNums(Slash.ClassName, "he", player);
                if (value > 3.5)
                    return new List<WrappedCard> { new WrappedCard(QimouCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (player.GetMark("@mou") > 0 && (card.Name.Contains(Slash.ClassName) || card.Name == SavageAssault.ClassName || card.Name == ArcheryAttack.ClassName))
                return 1.5;
            if (player.GetMark("@mou") > 0 && card.Name == Peach.ClassName && player.IsWounded())
                return 2;

            return 0;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return (player.Hp - 1).ToString();
        }
    }

    public class QimouCardAI : UseCard
    {
        public QimouCardAI() : base(QimouCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class FenjiJXAI : SkillEvent
    {
        public FenjiJXAI() : base("fenji_jx")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Room room = ai.Room;
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag("fenji_target"))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsFriend(target) && (player.Hp > 1 || player.GetPile("buqu_jx").Count <= 4))
                return true;

            return false;
        }
    }

    public class ZhibaAI : SkillEvent
    {
        public ZhibaAI() : base("zhibavs")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return base.GetTurnUse(ai, player);
        }
    }

    public class ZhibaCardAI : UseCard
    {
        public ZhibaCardAI() : base(ZhibaCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return base.UsePriorityAdjust(ai, player, targets, card);
        }
    }

    public class TianxiangJXAI : SkillEvent
    {
        public TianxiangJXAI() : base("tianxiang_jx")
        { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            if (room.GetTag("TianxiangDamage") is DamageStruct damage)
            {
                if (ai.GetDamageScore(damage, DamageStruct.DamageStep.Done).Score < 0 && !ai.NeedToLoseHp(damage, false, false, DamageStruct.DamageStep.Done))
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("h"))
                        if (RoomLogic.CanDiscard(room, player, player, id) && room.GetCard(id).Suit == WrappedCard.CardSuit.Heart)
                            ids.Add(id);

                    ai.SortByKeepValue(ref ids, false);
                    if (ids.Count > 0)
                    {
                        WrappedCard tianxiangCard = new WrappedCard("TianxiangCard")
                        {
                            Skill = Name
                        };
                        tianxiangCard.AddSubCard(ids[0]);

                        foreach (Player p in ai.FriendNoSelf)
                        {
                            if (ai.HasSkill("buqu_jx", p) && p.GetPile("buqu_jx").Count < 3 && p.Hp <= 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }

                            if (ai.HasSkill("hunzi", p) && p.GetMark("hunzi") == 0 && p.Hp == 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }

                            if (ai.HasSkill("zhaxiang", p) && !ai.WillSkipPlayPhase(p) && p.Hp > 1)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        List<Player> enemies = ai.GetPrioEnemies();
                        ai.SortByDefense(ref enemies, false);
                        foreach (Player p in enemies)
                        {
                            DamageStruct _damage = new DamageStruct("tianxiang_jx", damage.From, p, 1);
                            if (ai.GetDamageScore(_damage).Score > 6 && !ai.CanResist(p, _damage.Damage))
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        foreach (Player p in enemies)
                        {
                            if (p.Hp <= 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        enemies = ai.GetEnemies(player);
                        if (enemies.Count > 0)
                        {
                            ai.SortByDefense(ref enemies, false);
                            use.Card = tianxiangCard;
                            use.To.Add(enemies[0]);
                            return use;
                        }
                    }
                }
            }

            return use;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            Player target = room.FindPlayer((string)player.GetTag("tianxiang_target"));
            if (room.GetTag("TianxiangDamage") is DamageStruct damage)
            {
                DamageStruct _damage = new DamageStruct(Name, damage.From, target);
                if (ai.IsFriend(target))
                {
                    if (ai.HasSkill("zhaxiang", target) && !ai.WillSkipPlayPhase(target) && target.Hp > 1)
                        return "losehp";

                    return "damage";
                }
                else
                {
                    if (ai.GetDamageScore(_damage).Score > 6 && !ai.CanResist(target, _damage.Damage))
                        return "damage";
                }
            }

            return "losehp";
        }
    }
}