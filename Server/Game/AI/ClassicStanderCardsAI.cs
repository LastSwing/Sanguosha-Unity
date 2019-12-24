using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System;
using System.Collections.Generic;


namespace SanguoshaServer.AI
{
    class ClassicStanderCardsAI : AIPackage
    {
        public ClassicStanderCardsAI() : base("ClassicStanderCards")
        {
            events = new List<SkillEvent>
            {
                new HoneyTrapAI2(),
            };
            use_cards = new List<UseCard>
            {
                new ClassicBladeAI(),
                new ClassicHalberdAI(),
                new SaberAI(),          //七宝刀
                new HiddenDaggerAI(),   //笑里藏刀
                new HoneyTrapAI(),      //美人计
            };
        }
    }

    public class ClassicBladeAI : UseCard
    {
        public ClassicBladeAI() : base(ClassicBlade.ClassName)
        {}
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return ai.AjustWeaponRangeValue(player, card);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            string[] strs = prompt.Split(':');

            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Player target = room.FindPlayer(strs[1]);
            if (target != null)
            {
                use.To.Add(target);

                List<ScoreStruct> values = ai.CaculateSlashIncome(player, null, new List<Player> { target });
                if (values.Count > 0 && values[0].Score > 0)
                {
                    bool will_slash = false;
                    if (values[0].Score >= 10)
                    {
                        will_slash = true;
                    }
                    else if (ai.GetEnemies(player).Count == 1 && ai.GetOverflow(player) > 0)
                    {
                        foreach (int id in values[0].Card.SubCards)
                        {
                            if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                            {
                                will_slash = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Player p in values[0].Players)
                        {
                            if (ai.GetPrioEnemies().Contains(p))
                            {
                                will_slash = true;
                                break;
                            }
                        }
                    }

                    //todo: adjust ai personality
                    if (!will_slash && ai.GetOverflow(player) > 0)
                    {
                        foreach (int id in values[0].Card.SubCards)
                        {
                            if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                            {
                                will_slash = true;
                                break;
                            }
                        }
                    }

                    if (will_slash)
                    {
                        use.Card = values[0].Card;
                    }
                }
            }
            return use;
        }
    }

    public class ClassicHalberdAI : UseCard
    {
        public ClassicHalberdAI() : base(ClassicHalberd.ClassName) { }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return ai.AjustWeaponRangeValue(player, card);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class SaberAI : UseCard
    {
        public SaberAI() : base(Saber.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            foreach (Player p in ai.GetEnemies(player))
            {
                if (RoomLogic.HasShownArmorEffect(ai.Room, p) && RoomLogic.DistanceTo(ai.Room, player, p, null, true) <= 2)
                    value += 0.5;
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class HiddenDaggerAI : UseCard
    {
        public HiddenDaggerAI() : base(HiddenDagger.ClassName)
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;

            if (positive)
            {
                ScoreStruct score = new ScoreStruct
                {
                    Score = 0
                };
                if (from.Alive)
                {
                    DamageStruct damage = new DamageStruct(trick, from, to);
                    score = ai.GetDamageScore(damage);
                }

                if (to.IsWounded())
                {
                    if (ai.IsFriend(to))
                        score.Score += 1 * to.GetLostHp();
                    else if (ai.IsEnemy(to))
                        score.Score -= 1 * to.GetLostHp();
                }

                if (score.Score <= -4) result.Null = true;
            }
            else
            {
                ScoreStruct score = new ScoreStruct
                {
                    Score = 0
                };
                if (from.Alive)
                {
                    DamageStruct damage = new DamageStruct(trick, from, to);
                    score = ai.GetDamageScore(damage);
                }

                if (to.IsWounded())
                {
                    if (ai.IsFriend(to))
                        score.Score += 1 * to.GetLostHp();
                    else if (ai.IsEnemy(to))
                        score.Score -= 1 * to.GetLostHp();
                }

                if (score.Score > 4) result.Null = true;
            }

            return result;
        }
    }

    public class HoneyTrapAI2 : SkillEvent
    {
        public HoneyTrapAI2() : base(HoneyTrap.ClassName)
        {
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = room.Current;
            List<int> ids = player.GetCards("h");
            ai.SortByKeepValue(ref ids, false);
            if (ai.IsFriend(target))
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { target });
                if (pair.Key != null && ids.Contains(pair.Value))
                    return new List<int> { pair.Value };
            }

            return new List<int> { ids[0] };
        }
    }

    public class HoneyTrapAI : UseCard
    {
        public HoneyTrapAI() : base(HoneyTrap.ClassName)
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;

            List<Player> players = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.IsFemale()) players.Add(p);

            int count = Math.Min(players.Count, to.HandcardNum);
            if (positive)
            {
                if (ai.IsFriend(to))
                {
                    double value = -count;
                    if (from.HandcardNum + count < to.HandcardNum)
                    {
                        DamageStruct damage = new DamageStruct(trick, from, to);
                        value += ai.GetDamageScore(damage).Score;
                    }

                    if (count <= -2) result.Null = true;
                }
            }
            else
            {
                if (ai.IsFriend(from))
                {
                    double value = count;
                    if (count > 0)
                    {
                        room.SortByActionOrder(ref players);
                        for (int i = 0; i < count; i++)
                            if (ai.IsFriend(players[i]) && players[i].HandcardNum > 0)
                                value += players[i].HandcardNum / 5;
                    }
                    if (from.HandcardNum + count < to.HandcardNum)
                    {
                        DamageStruct damage = new DamageStruct(trick, from, to);
                        value += ai.GetDamageScore(damage).Score;
                    }

                    if (count > 3) result.Null = true;
                }
            }

            return result;
        }
    }
}
