using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System.Collections.Generic;


namespace SanguoshaServer.AI
{
    class ManeuveringCardsAI : AIPackage
    {
        public ManeuveringCardsAI() : base("ManeuveringCards")
        {
            use_cards = new List<UseCard>
            {
                new GudingBladeAI(),
                new ClassicWoodenOxAI(),
                new ClassicWoodenOxCardAI(),

                new DHorseAI("Hualiu"),
            };

            events = new List<SkillEvent>
            {
                new ClassicWoodenAI(),
            };
        }
    }

    class GudingBladeAI : UseCard
    {
        public GudingBladeAI() : base(GudingBlade.ClassName) { }
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
    public class ClassicWoodenOxCardAI : UseCard
    {
        public ClassicWoodenOxCardAI() : base(ClassicWoodenOxCard.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (ai.HasSkill(TrustedAI.LoseEquipSkill, player) && ai.GetFriends(player).Count > 1)
                return 6;

            if (player.GetMark("@luan") > 0 && ai.GetFriends(player).Count > 1)
                return 6;

            return 2;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class ClassicWoodenAI : SkillEvent
    {
        public ClassicWoodenAI() : base(ClassicWoodenOx.ClassName)
        {
            key = new List<string> { "playerChosen:ClassicWoodenOx" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    string name = choices[2];
                    Player to = ai.Room.FindPlayer(name);
                    if (to != null)
                    {
                        if (ai.GetPlayerTendency(to) != "unknown")
                            ai.UpdatePlayerRelation(player, to, true);
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (ai.Target[Name] != null)
                return new List<Player> { ai.Target[Name] };

            return new List<Player>();
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            ai.Target[Name] = null;
            if (player.HasUsed(ClassicWoodenOxCard.ClassName) || player.IsKongcheng() || player.GetPile("wooden_ox").Count >= 5) return null;
            List<int> cards= player.GetCards("h");

            int sub = -1;
            foreach (int id in cards)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name == DragonPhoenix.ClassName)
                {
                    Player target = null;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (ai.HasSkill("zhangwu", p))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (target != null && !ai.IsFriend(target, player) || !ai.HasSkill(TrustedAI.LoseEquipSkill, player))
                    {
                        sub = id;
                        break;
                    }
                }
                else if (card.Name == PeaceSpell.ClassName)
                {
                    Player target = null;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (ai.HasSkill("wendao", p))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (target != null && !ai.IsFriend(target, player))
                    {
                        sub = id;
                        break;
                    }
                }
                else if (card.Name == LuminouSpearl.ClassName)
                {
                    Player target = null;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (ai.HasSkill("jubao", p))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (target != null && !ai.IsFriend(target, player))
                    {
                        sub = id;
                        break;
                    }
                }
            }

            ai.SortByUseValue(ref cards);
            bool keep = false;
            if (ai.HasSkill("jijiu"))
            {
                foreach (Player p in ai.GetFriends(player))
                {
                    if (ai.IsWeak(p))
                    {
                        keep = true;
                        break;
                    }
                }
            }

            if (!keep)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in ai.FriendNoSelf)
                    if (RoomLogic.CanPutEquip(p, room.GetCard(player.Treasure.Key)) && ai.GetSameEquip(room.GetCard(player.Treasure.Key), p) == null)
                        targets.Add(p);

                Player next = null;
                room.SortByActionOrder(ref targets);
                foreach (Player p in targets)
                {
                    if (!ai.WillSkipPlayPhase(p))
                    {
                        next = p;
                        break;
                    }
                }
                if (next != null)
                {
                    if (sub != -1)
                    {
                        ai.Target[Name] = next;
                        WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName) { Skill = Name };
                        card.AddSubCard(sub);
                        return new List<WrappedCard> { card };
                    }
                    else
                    {
                        KeyValuePair<Player, int> result = ai.GetCardNeedPlayer(cards, new List<Player> { next });
                        if (result.Key != null)
                        {
                            ai.Target[Name] = next;
                            WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName) { Skill = Name };
                            card.AddSubCard(result.Value);
                            return new List<WrappedCard> { card };
                        }
                    }
                }
                else if (ai.HasSkill(TrustedAI.LoseEquipSkill) && targets.Count > 0)
                {
                    if (sub != -1)
                    {
                        ai.Target[Name] = targets[0];
                        WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName)
                        {
                            Skill = Name
                        };
                        card.AddSubCard(sub);
                        return new List<WrappedCard> { card };
                    }
                    else
                    {
                        ai.Target[Name] = targets[0];
                        ai.SortByUseValue(ref cards, false);
                        WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName)
                        {
                            Skill = Name
                        };
                        card.AddSubCard(cards[0]);
                        return new List<WrappedCard> { card };
                    }
                }
                else if (targets.Count > 0)
                {
                    List<Player> weaks = new List<Player>();
                    foreach (Player p in targets)
                    {
                        if (ai.IsWeak(p))
                            weaks.Add(p);
                    }

                    if (sub != -1 && weaks.Count > 0)
                    {
                        ai.SortByDefense(ref weaks, false);
                        ai.Target[Name] = weaks[0]; WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName)
                        {
                            Skill = Name
                        };
                        card.AddSubCard(sub);
                        return new List<WrappedCard> { card };
                    }

                    KeyValuePair<Player, int> result = ai.GetCardNeedPlayer(cards, weaks);
                    if (result.Key != null)
                    {
                        ai.Target[Name] = result.Key;
                        WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName)
                        {
                            Skill = Name
                        };
                        card.AddSubCard(result.Value);
                        return new List<WrappedCard> { card };
                    }
                }
            }

            if (ai.GetOverflow(player) > 0)
            {
                WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName)
                {
                    Skill = Name
                };
                if (sub != -1)
                {
                    card.AddSubCard(sub);
                }
                else
                {
                    ai.SortByKeepValue(ref cards, false);
                    card.AddSubCard(cards[ai.GetOverflow(player) - 1]);
                }
                return new List<WrappedCard> { card };
            }

            if (player.HandcardNum == 1 && ai.HasSkill("kongcheng|kongcheng_jx"))
            {
                WrappedCard card = new WrappedCard(ClassicWoodenOxCard.ClassName)
                {
                    Skill = Name
                };
                card.AddSubCards(player.GetCards("h"));
                return new List<WrappedCard> { card };
            }

            return null;
        }
    }

    public class ClassicWoodenOxAI : UseCard
    {
        public ClassicWoodenOxAI() : base(ClassicWoodenOx.ClassName)
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            value += player.GetPile("wooden_ox").Count * 0.3;
            foreach (Player p in ai.FriendNoSelf)
                if (p.Treasure.Key == -1)
                    value += 0.5;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
}
