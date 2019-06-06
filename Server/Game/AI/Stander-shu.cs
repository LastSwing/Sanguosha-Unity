using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class StanderShuAI : AIPackage
    {
        public StanderShuAI() : base("Stander-shu")
        {
            events = new List<SkillEvent>
            {
                new RendeAI(),
                new NiepanAI(),
            };

            use_cards = new List<UseCard>
            {
                new RendeCardAI(),
            };
        }
    }

    public class RendeAI : SkillEvent
    {
        public RendeAI() : base("rende")
        {
        }

        public bool ShouldUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<Player> friends = ai.FriendNoSelf;
            friends.RemoveAll(t => t.HasFlag("rende_" + player.Name));

            string card_name = player.HasFlag("rende_judged") && player.GetMark(Name) < 2 ? ai.Choice[Name] : string.Empty;
            if (!string.IsNullOrEmpty(card_name))
            {
                return true;
            }
            else if (player.GetMark(Name) < 2 && player.HandcardNum >= 2 - player.GetMark(Name))
            {
                double peach_value = 0;
                double analeptic_value = 0;
                double slash_value = 0;
                bool can_slash = false;
                WrappedCard drink = null;
                WrappedCard slash = null;

                List<WrappedCard> slashes = ai.GetCards("Slash", player);

                List<WrappedCard> all_cards = new List<WrappedCard>();
                List<string> names = ViewAsSkill.GetGuhuoCards(room, "b");
                foreach (string name in names)
                {
                    if (name == "Jink") continue;
                    WrappedCard card = new WrappedCard(name)
                    {
                        Skill = "_rende"
                    };
                    FunctionCard fcard = Engine.GetFunctionCard(name);
                    if (fcard.IsAvailable(room, player, card))
                    {
                        all_cards.Add(card);
                        if (name.Contains("Slash"))
                        {
                            can_slash = true;
                            slashes.Add(card);
                        }
                        if (name == "Analeptic")
                        {
                            drink = card;
                        }
                    }
                }

                if (player.IsWounded())
                {
                    peach_value = 3;
                    if (!player.HasFlag("ShengxiDamageInPlayPhase"))
                        peach_value += 4;
                }
                if (can_slash)
                {
                    List<ScoreStruct> values = ai.CaculateSlashIncome(player, slashes);
                    if (values.Count > 0)
                    {
                        if (values[0].Card.Skill == "_rende")
                        {
                            slash_value = values[0].Score;
                            slash = values[0].Card;
                        }
                        foreach (ScoreStruct score in values)
                        {
                            if (!score.Card.Skill.Contains("rende") && score.Score - slash_value < 1)
                            {
                                slash_value = 0;
                                slash = score.Card;
                                break;
                            }
                        }

                        if (slash_value == 0 && drink != null)
                        {
                            int hand = 0;
                            foreach (int id in slash.SubCards)
                                if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                                    hand++;
                            if (player.HandcardNum - hand > 2 - player.GetMark(Name))
                            {
                                ai.SetPreDrink(drink);
                                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { values[0].Card }, values[0].Players);
                                ai.RemovePreDrink();
                                if (scores.Count > 0)
                                    analeptic_value = scores[0].Score - values[0].Score;
                            }
                        }
                    }
                }

                if (analeptic_value > 0 && analeptic_value > peach_value)
                {
                    player.SetFlags("rende_judged");
                    ai.Choice[Name] = "Analeptic";
                    return true;
                }
                else if (slash_value > 0 && slash_value > peach_value)
                {
                    player.SetFlags("rende_judged");
                    ai.Choice[Name] = slash.Name;
                    return true;
                }
                else if (peach_value > 0)
                {
                    player.SetFlags("rende_judged");
                    ai.Choice[Name] = "Peach";
                    return true;
                }
            }
            else
            {
                ai.Choice[Name] = null;
                if (ai.NeedKongcheng(player))
                    return true;
                else
                {
                    if (friends.Count == 0) return false;

                    if (ai.GetCardNeedPlayer(new List<int>(player.HandCards), friends).Key != null)
                        return true;
                }
            }

            return false;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };
            Room room = ai.Room;
            string card_name = player.HasFlag("rende_judged") && player.GetMark(Name) < 2 ? ai.Choice[Name] : string.Empty;
            if (!string.IsNullOrEmpty(card_name))
            {
                WrappedCard card = new WrappedCard(card_name)
                {
                    Skill = "_rende"
                };
                FunctionCard fcard = Engine.GetFunctionCard(card_name);
                if (fcard.IsAvailable(room, player, card))
                {
                    if (fcard is Slash)
                    {
                        List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { card });
                        if (scores.Count > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
                        {
                            use.Card = card;
                            use.To = scores[0].Players;
                        }
                    }
                    else
                    {
                        use.Card = card;
                    }
                }
            }

            return use;
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && ShouldUse(ai, player))
                return new WrappedCard("RendeCard") { Skill = Name, ShowSkill = Name };

            return null;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    ai.UpdatePlayerRelation(player, p, true);
            }
        }
    }

    public class RendeCardAI : UseCard
    {
        public RendeCardAI() : base("RendeCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            string card_name = player.HasFlag("rende_judged") && player.GetMark(Name) < 2 ? ai.Choice[Name] : string.Empty;
            Room room = ai.Room;
            List<Player> friends = ai.FriendNoSelf;
            friends.RemoveAll(t => t.HasFlag("rende_" + player.Name));
            List<Player> _friends = new List<Player>(friends);
            List<int> ids = new List<int>(player.HandCards);

            if (!string.IsNullOrEmpty(card_name) && player.HandcardNum >= 2 - player.GetMark("rende"))
            {
                if (card_name.Contains("Slash"))
                {
                    List<WrappedCard> ana = ai.GetCards("Analeptic", player, true);
                    if (ana.Count > 0)
                    {
                        ai.SortByUseValue(ref ana);
                        int hand = 0;
                        foreach (int id in ana[0].SubCards)
                            if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                                hand++;
                        FunctionCard fcard = Engine.GetFunctionCard("Analeptic");
                        if (fcard.IsAvailable(room, player, ana[0]) && player.HandcardNum - hand > 2 - player.GetMark("rende") && friends.Count > 0)
                        {
                            use.Card = ana[0];
                            use.To = new List<Player>();
                            return;
                        }
                    }
                }

                if (card_name == "Analeptic")
                {
                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player);
                    if (scores.Count > 0)
                    {
                        foreach (int id in scores[0].Card.SubCards)
                            ids.Remove(id);
                    }
                }
            }

            if (friends.Count > 0)
            {
                List<int> _ids = new List<int>(ids);
                while (true)
                {
                    KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(_ids, _friends);
                    if (pair.Key != null)
                    {
                        _friends = new List<Player> { pair.Key };
                        _ids.Remove(pair.Value);
                        card.AddSubCard(pair.Value);
                    }
                    else
                        break;
                }
                if (card.SubCards.Count > 0 && _friends.Count == 1)
                {
                    //如只有1位友方，则须至少给够2张牌
                    if (friends.Count == 1 && card.SubCards.Count < 2 - player.GetMark("rende"))
                    {
                        ai.SortByUseValue(ref ids, false);
                        for (int i = card.SubCards.Count - 2 + player.GetMark("rende"); i >= 0; i--)
                        {
                            card.AddSubCard(ids[0]);
                        }
                    }

                    use.Card = card;
                    use.To = _friends;
                    return;
                }
            }

            if (card_name == "Peach" && player.HandcardNum >= 2 - player.GetMark("rende"))
            {
                if (friends.Count > 0)
                {
                    ai.SortByDefense(ref friends, false);
                    ai.SortByUseValue(ref ids, false);
                    for (int i = 2 - player.GetMark("rende"); i >= 0; i--)
                        card.AddSubCard(ids[0]);

                    use.Card = card;
                    use.To = new List<Player> { friends[0] };
                    return;
                }
                else if (ai.HasSkill("kongcheng"))
                {
                    List<Player> enemies = ai.GetEnemies(player);
                    ai.SortByDefense(ref enemies, false);
                    if (enemies.Count <= 2)
                    {
                        foreach (int id in ids)
                            if (ai.GetUseValue(id, player) > 3)
                                return;

                        //把手有手牌交给敌人空城回血
                        card.AddSubCards(ids);
                        use.Card = card;
                        use.To = new List<Player> { enemies[0] };
                        return;
                    }
                }
            }

            if (ai.HasSkill("kongcheng") && ai.NeedKongcheng(player))
            {
                if (friends.Count > 0)
                {
                    ai.SortByDefense(ref friends, false);
                    card.AddSubCards(ids);

                    use.Card = card;
                    use.To = new List<Player> { friends[0] };
                    return;
                }
            }
        }
    }

    public class NiepanAI : SkillEvent
    {
        public NiepanAI() : base("niepan")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.CanSave(player, 1 - player.Hp))
                return false;

            return true;
        }
    }
}