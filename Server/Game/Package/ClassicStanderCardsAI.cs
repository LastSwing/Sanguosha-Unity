using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System.Collections.Generic;


namespace SanguoshaServer.AI
{
    class ClassicStanderCardsAI : AIPackage
    {
        public ClassicStanderCardsAI() : base("ClassicStanderCards")
        {
            use_cards = new List<UseCard>
            {
                new ClassicBladeAI(),
                new ClassicHalberdAI(),
            };
        }
    }

    public class ClassicBladeAI : UseCard
    {
        public ClassicBladeAI() : base(ClassicBlade.ClassName)
        {}
        public override double UsePriorityAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
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
        public override double UsePriorityAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
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
}
