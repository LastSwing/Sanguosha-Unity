using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System;
using System.Collections.Generic;
using static CommonClass.Game.WrappedCard;
namespace SanguoshaServer.AI
{
    public class StrategicAdvantageCardsAI : AIPackage
    {
        public StrategicAdvantageCardsAI() : base("StrategicAdvantageCardsAI")
        {
            events = new List<SkillEvent>
            {
                new TransferAI(),
                new WoodenAI(),
                new JadeAI(),
                new HalberdChooseAI(),
            };

            use_cards = new List<UseCard>
            {
                new TransferCardAI(),
                new WoodenOxCardAI(),

                new BladeAI(),
                new HalberdAI(),
                new BreastPlateAI(),
                new IronArmorAI(),
                new WoodenOxAI(),
                new JadeSealAI(),

                new DrowningAI(),
                new BurningCampsAI(),
                new LureTigerAI(),
                new FightTogetherAI(),
                new AllianceFeastAI(),
                new ThreatenEmperorAI(),
                new EdictAI(),
                new DHorseAI("Jingfan"),
            };
        }
    }

    public class TransferCardAI : UseCard
    {
        public TransferCardAI() : base(TransferCard.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && use.From != player)
            {
                foreach (Player p in use.To)
                    if (ai.GetPossibleId(player).Count == 1 && ai.GetPlayerTendency(p) == "unknown" && ai.IsKnown(player, p) && !p.HasShownOneGeneral())
                        ai.UpdatePlayerRelation(player, p, true);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard c = room.GetCard(id);
                if (c.Transferable)
                    ids.Add(id);
            }

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.HasShownOneGeneral() || (player.HasShownOneGeneral() && !RoomLogic.IsFriendWith(room, p, player)))
                    targets.Add(p);
            }

            if (ids.Count > 0 && targets.Count > 0)
            {
                List<Player> friend = new List<Player>();
                List<Player> friend_other = new List<Player>();
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p))
                    {
                        friend_other.Add(p);
                        if (p.HasShownOneGeneral() && player.HasShownOneGeneral())
                            friend.Add(p);
                    }
                }
                if (friend.Count > 0)                               //可以摸牌的友方
                {
                    List<int> to_transfer = new List<int>();
                    while (ids.Count > 0)
                    {
                        KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, friend);
                        if (pair.Key != null)
                        {
                            ids.Remove(pair.Value);
                            to_transfer.Add(pair.Value);
                            friend = new List<Player> { pair.Key };
                            if (to_transfer.Count >= 3)
                                break;
                        }
                        else
                            break;
                    }
                    if (to_transfer.Count > 0 && friend.Count == 1)
                    {
                        card.ClearSubCards();
                        card.AddSubCards(to_transfer);
                        use.Card = card;
                        use.To = friend;
                        return;
                    }
                }

                List<int> cards = new List<int>();
                bool oneJink = ai.HasSkill("kongcheng|kongcheng_jx");
                foreach (int id in ids)
                {
                    if (!ai.IsCard(id, Peach.ClassName, player) || friend.Count > 0)
                    {
                        bool insert = true;
                        if (!oneJink && ai.IsCard(id, Jink.ClassName, player))
                        {
                            oneJink = true;
                            insert = false;
                        }
                        else if (room.GetCard(id).Number > 10 && ai.HasSkill("tianyi|quhu|shuangren|lieren"))
                            insert = false;

                        if (insert)
                            cards.Add(id);
                    }
                }

                if (cards.Count == 0) return; ;
                if (friend_other.Count > 0)
                {
                    List<int> to_transfer = new List<int>();
                    while (cards.Count > 0)
                    {
                        KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(cards, friend_other);
                        if (pair.Key != null)
                        {
                            cards.Remove(pair.Value);
                            to_transfer.Add(pair.Value);
                            friend_other = new List<Player> { pair.Key };
                            if (to_transfer.Count >= 3)
                                break;
                        }
                        else
                            break;
                    }
                    if (to_transfer.Count > 0 && friend_other.Count == 1)
                    {
                        card.ClearSubCards();
                        card.AddSubCards(to_transfer);
                        use.Card = card;
                        use.To = friend_other;
                        return;
                    }

                }

                if (!player.HasShownOneGeneral()) return;

                foreach (int id in cards)
                {
                    WrappedCard c = room.GetCard(id);
                    if (c.Name == ThreatenEmperor.ClassName)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(c.Name);
                        if (fcard.IsAvailable(room, player, c))
                        {
                            UseCard e = Engine.GetCardUsage(c.Name);
                            if (e != null)
                            {
                                CardUseStruct dummy_use = new CardUseStruct
                                {
                                    From = player,
                                    IsDummy = true
                                };
                                e.Use(ai, player, ref dummy_use, c);
                                if (dummy_use.Card != null && dummy_use.Card == c)
                                    continue;
                            }
                        }

                        int anjiang = 0;
                        foreach (Player p in room.GetOtherPlayers(player))
                        {
                            if (!p.HasShownOneGeneral())
                                anjiang++;
                        }

                        List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
                        string big_kingdom = string.Empty;
                        if (big_kingdoms.Count > 0)
                            big_kingdom = big_kingdoms[0];

                        int maxNum = 0;
                        if (big_kingdom != string.Empty)
                        {
                            if (big_kingdom.StartsWith("SGS"))
                                maxNum = 99;
                            else
                                maxNum = RoomLogic.GetPlayerNumWithSameKingdom(room, player, big_kingdom);
                        }
                        else
                            maxNum = 99;

                        foreach (Player p in targets)
                        {
                            if (p.HasShownOneGeneral() && p.Name != big_kingdom && (!big_kingdoms.Contains(p.Kingdom) || p.GetRoleEnum() == Player.PlayerRole.Careerist)
                                && (maxNum == 99 || RoomLogic.GetPlayerNumWithSameKingdom(room, p) + anjiang < maxNum))
                            {
                                card.ClearSubCards();
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To = new List<Player> { p };
                                return;
                            }
                        }
                    }
                    else if (c.Name == BurningCamps.ClassName)
                    {
                        string gameProcess = ai.Process;
                        if (gameProcess.Contains(player.Kingdom + ">"))
                        {
                            foreach (Player p in targets)
                            {
                                if (p.HasShownOneGeneral() && (ai.IsFriend(p) || ai.WillSkipPlayPhase(p)))
                                {
                                    card.ClearSubCards();
                                    card.AddSubCard(id);
                                    use.Card = card;
                                    use.To = new List<Player> { p };
                                    return;
                                }
                            }
                        }
                        else
                        {
                            FunctionCard fcard = Engine.GetFunctionCard(c.Name);
                            foreach (Player p in targets)
                            {
                                if (p.HasShownOneGeneral() && fcard.IsAvailable(room, p, c) && !ai.GetPrioEnemies().Contains(p))
                                {
                                    Player np = room.GetNextAlive(p);
                                    DamageStruct damage = new DamageStruct(c, p, np, 1, DamageStruct.DamageNature.Fire);
                                    damage.Damage = ai.DamageEffect(damage, DamageStruct.DamageStep.None);
                                    if (!ai.IsFriend(np) && (!np.Chained || ai.ChainDamage(damage) >= 0))
                                    {
                                        card.ClearSubCards();
                                        card.AddSubCard(id);
                                        use.Card = card;
                                        use.To = new List<Player> { p };
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class TransferAI : SkillEvent
    {
        public TransferAI() : base("transfer")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.HasFlag(Name)) return null;

            Room room = ai.Room;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (card.Transferable)
                    return new List<WrappedCard> { new WrappedCard(TransferCard.ClassName) { Skill = Name, Mute = true } };
            }

            return null;
        }
    }


    //装备
    public class BladeAI : UseCard
    {
        public BladeAI() : base(Blade.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            foreach (Player p in ai.Room.GetOtherPlayers(player))
            {
                if (!p.HasShownAllGenerals() && !ai.IsFriend(p))
                    value += 0.15;
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class HalberdAI : UseCard
    {
        public HalberdAI() : base(Halberd.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            List<string> kingdoms = new List<string>();
            foreach (Player p in ai.Room.GetOtherPlayers(player))
            {
                if (ai.IsEnemy(p) && (!p.HasShownOneGeneral() || p.GetRoleEnum() == Player.PlayerRole.Careerist || !kingdoms.Contains(p.Kingdom)))
                {
                    if (p.HasShownOneGeneral() && p.Role != "careerist")
                        kingdoms.Add(p.Kingdom);

                    value += 0.5;
                }
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class HalberdChooseAI : SkillEvent
    {
        public HalberdChooseAI() : base(Halberd.ClassName)
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> players, int min, int max)
        {
            Room room = ai.Room;
            CardUseStruct use = (CardUseStruct)room.GetTag("extra_target_skill");
            List<string> kingdoms = new List<string>();
            List<Player> result = new List<Player>();
            List<Player> targets = new List<Player>();
            foreach (Player p in use.To)
            {
                if (p.HasShownOneGeneral() && p.Role != "careerist")
                    kingdoms.Add(p.Kingdom);
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (use.To.Contains(p)) continue;
                if (!p.HasShownOneGeneral() || p.GetRoleEnum() == Player.PlayerRole.Careerist || !kingdoms.Contains(p.Kingdom))
                    targets.Add(p);
            }

            if (targets.Count == 0) return targets;
            Player target = ai.AddExtraSlashTarget(targets, use);
            while (target != null)
            {
                result.Add(target);
                if (target.HasShownOneGeneral() && target.Role != "careerist")
                    kingdoms.Add(target.Kingdom);

                target = null;
                targets.Clear();

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (use.To.Contains(p) || result.Contains(p)) continue;
                    if (!p.HasShownOneGeneral() || p.GetRoleEnum() == Player.PlayerRole.Careerist || !kingdoms.Contains(p.Kingdom))
                        targets.Add(p);
                }

                if (targets.Count == 0) return result;
                target = ai.AddExtraSlashTarget(targets, use, result);
            }

            return result;
        }
    }

    public class BreastPlateAI : UseCard
    {
        public BreastPlateAI() : base(BreastPlate.ClassName)
        { }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (ai.IsWeak(player))
            {
                value += 3;
            }

            return value;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class IronArmorAI : UseCard
    {
        public IronArmorAI() : base(IronArmor.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class WoodenOxCardAI : UseCard
    {
        public WoodenOxCardAI() : base(WoodenOxCard.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (ai.HasSkill(TrustedAI.LoseEquipSkill, player) && ai.GetFriends(player).Count > 1)
                return 6;

            return 2;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class WoodenAI : SkillEvent
    {
        public WoodenAI() : base(WoodenOx.ClassName)
        {
            key = new List<string> { "playerChosen:WoodenOx" };
        }
        
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    string name = choices[2];
                    Player to = ai.Room.FindPlayer(name);
                    if (to != null && ai.IsKnown(player, to))
                        ai.UpdatePlayerRelation(player, to, true);
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
            if (player.HasUsed(WoodenOxCard.ClassName) || player.IsKongcheng()) return null;
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
                        WrappedCard card = new WrappedCard(WoodenOxCard.ClassName) { Skill = Name };
                        card.AddSubCard(sub);
                        return new List<WrappedCard> { card };
                    }
                    else
                    {
                        KeyValuePair<Player, int> result = ai.GetCardNeedPlayer(cards, new List<Player> { next });
                        if (result.Key != null)
                        {
                            ai.Target[Name] = next;
                            WrappedCard card = new WrappedCard(WoodenOxCard.ClassName) { Skill = Name };
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
                        WrappedCard card = new WrappedCard(WoodenOxCard.ClassName)
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
                        WrappedCard card = new WrappedCard(WoodenOxCard.ClassName)
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
                        ai.Target[Name] = weaks[0]; WrappedCard card = new WrappedCard(WoodenOxCard.ClassName)
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
                        WrappedCard card = new WrappedCard(WoodenOxCard.ClassName)
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
                WrappedCard card = new WrappedCard(WoodenOxCard.ClassName)
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

            if (player.HandcardNum == 1 && ai.HasSkill("kongcheng"))
            {
                WrappedCard card = new WrappedCard(WoodenOxCard.ClassName)
                {
                    Skill = Name
                };
                card.AddSubCards(player.GetCards("h"));
                return new List<WrappedCard> { card };
            }

            return null;
        }
    }

    public class WoodenOxAI : UseCard
    {
        public WoodenOxAI() : base(WoodenOx.ClassName)
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

    public class JadeAI : SkillEvent
    {
        public JadeAI() : base(JadeSeal.ClassName)
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            WrappedCard kb = new WrappedCard(KnownBoth.ClassName);
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                IsDummy = true
            };

            UseCard u = Engine.GetCardUsage(KnownBoth.ClassName);
            u.Use(ai, player, ref use, kb);

            return use;
        }
    }

    public class JadeSealAI : UseCard
    {
        public JadeSealAI() : base(JadeSeal.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;

            return value;
        }
        
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class DrowningAI : UseCard
    {
        public DrowningAI() : base(Drowning.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                {
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                    {
                        bool friendly = false;
                        List<int> equips = p.GetCards("e");
                        double throw_value = equips.Count * 2.5;
                        if (ai.HasSkill(TrustedAI.LoseEquipSkill, p))
                        {
                            throw_value -= 6;
                            if (equips.Count > 1)
                                throw_value += 3;
                        }

                        if (p.GetArmor())
                        {
                            double throw_armor = ai.GetKeepValue(p.Armor.Key, p, Player.Place.PlaceEquip);
                            throw_value += throw_armor;
                        }

                        if (throw_value < 0)
                            friendly = true;

                        ai.UpdatePlayerRelation(player, p, friendly);
                    }
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> targets = ai.Exclude(room.GetOtherPlayers(player), card, player);
            Dictionary<Player, double> values = new Dictionary<Player, double>();
            foreach (Player p in targets)
            {
                List<int> equips = p.GetCards("e");
                double throw_value = equips.Count * 2.5;
                if (ai.HasSkill(TrustedAI.LoseEquipSkill, p))
                {
                    throw_value -= 6;
                    if (equips.Count > 1)
                        throw_value += 3;
                }

                if (ai.IsFriend(p))
                    throw_value = 0 - throw_value;

                if (p.GetArmor())
                {
                    double throw_armor = ai.GetKeepValue(p.Armor.Key, p, Player.Place.PlaceEquip);
                    if (throw_armor < 0)
                    {
                        if (ai.IsFriend(p))
                            throw_value -= throw_value;
                        else
                            throw_value += throw_value;
                    }
                }

                if (ai.IsFriend(p))
                {
                    values.Add(p, throw_value);
                }
                else
                {
                    DamageStruct damage = new DamageStruct(card, player, p, 1, DamageStruct.DamageNature.Thunder);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    double value = Math.Min(throw_value, score.Score);
                    values.Add(p, value);
                }
            }

            Player target = null;
            double v = -100;
            foreach (Player p in values.Keys)
            {
                if (values[p] > v)
                {
                    v = values[p];
                    target = p;
                }
            }

            if (ai.HasSkill("jizhi"))
                v += 2;

            if (target != null && v > 0)
            {
                use.Card = card;
                use.To = new List<Player> { target };
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            DamageStruct damage = new DamageStruct(effect.Card, from, to, 1, DamageStruct.DamageNature.Thunder);

            ScoreStruct score = ai.GetDamageScore(damage);
            if (positive)
            {
                if (score.Score < -4 && ai.IsFriend(to))
                {
                    double value = 0;
                    List<int> ids = to.GetEquips();
                    foreach (int id in ids)
                        value += ai.GetKeepValue(id, to);

                    if (value > Math.Abs(score.Score) && ai.IsWeak(to))
                        result.Null = true;
                }
            }
            else
            {
                if (score.Score > 6 && ai.IsEnemy(to))
                {
                    double value = 0;
                    List<int> ids = to.GetEquips();
                    foreach (int id in ids)
                        value += ai.GetKeepValue(id, to);

                    if (value > 6 && !keep)
                        result.Null = true;
                }
            }

            return result;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            if (data is CardEffectStruct effect)
            {
                DamageStruct damage = new DamageStruct(effect.Card, effect.From, player, 1, DamageStruct.DamageNature.Thunder);
                double value = 0;
                List<int> ids = player.GetEquips();
                foreach (int id in ids)
                    value += ai.GetKeepValue(id, player);

                ScoreStruct score = ai.GetDamageScore(damage, DamageStruct.DamageStep.None);
                if (score.DoDamage && score.Damage.Damage >= player.Hp && !ai.CanResist(player, score.Damage.Damage)
                    && ai.GetKnownCardsNums(Peach.ClassName, "he", player) + ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) < score.Damage.Damage - player.Hp)
                    return "throw";

                if (score.Score > -value / 2)
                    return "damage";
            }

            return "throw";
        }
    }

    public class BurningCampsAI : UseCard
    {
        public BurningCampsAI() : base(BurningCamps.ClassName)
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player player = ai.Self;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            if (positive)
            {
                if (from == player) return result;
                List<Player> chained = new List<Player>();
                bool dangerous = false;

                DamageStruct damage = new DamageStruct(trick, from, to, 1, DamageStruct.DamageNature.Fire);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (ai.ChainDamage(damage) < 0)
                {
                    foreach (Player p in room.GetOtherPlayers(to))
                    {
                        if (p.Chained && ai.IsFriend(p))
                        {
                            chained.Add(p);
                            if (ai.IsWeak(p))
                                dangerous = true;
                        }
                    }
                }

                if (ai.DamageEffect(damage, DamageStruct.DamageStep.None) > 1)
                    dangerous = true;
                
                List<Player> friends = new List<Player>();
                if (ai.IsFriend(to))
                {
                    foreach (Player p in targets)
                    {
                        DamageStruct _damage = new DamageStruct(trick, from, p, 1, DamageStruct.DamageNature.Fire);
                        int count = ai.DamageEffect(damage, DamageStruct.DamageStep.None);
                        if (count > 0)
                            friends.Add(p);
                        if (count > 1)
                            dangerous = true;
                    }
                }

                if (chained.Count + friends.Count > 2 || dangerous)
                {
                    result.Null = true;
                    result.Heg = room.NullTimes == 0 && friends.Count > 1;
                    return result;
                }

                if (keep) return result;
                if (RoomLogic.IsFriendWith(room, player, to) && ai.IsEnemy(from))
                {
                    result.Null = true;
                    result.Heg = room.NullTimes == 0 && friends.Count > 1;
                }
            }
            else
            {
                if (!ai.IsFriend(from)) return result;
                double value = 0;
                bool isHegNullification = room.HegNull;
                targets.Insert(0, to);

                bool chained = false;
                foreach (Player p in targets)
                {

                    DamageStruct damage = new DamageStruct(trick, from, p, 1, DamageStruct.DamageNature.Fire);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    value = score.Score;
                    if (!chained && ai.IsSpreadStarter(damage))
                    {
                        chained = true;
                        value += ai.ChainDamage(damage);
                    }

                    if (!isHegNullification)
                        break;
                }

                if (value > 6)
                    result.Null = true;
            }

            return result;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                {
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                        ai.UpdatePlayerRelation(player, p, false);
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            Player np = room.GetNextAlive(player, 1);
            if (ai.IsFriend(np)) return;

            List<Player> targets = RoomLogic.GetFormation(room, np);
            if (targets.Count > 0)
            {
                double value = 0;
                bool damage_done = false;
                foreach (Player p in targets)
                {
                    if (!ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player) && RoomLogic.IsProhibited(room, player, p, card) == null)
                    {
                        DamageStruct damage = new DamageStruct(card, player, p, 1, DamageStruct.DamageNature.Fire);
                        ScoreStruct score = ai.GetDamageScore(damage);
                        value += score.Score;
                        if (score.DoDamage)
                            damage_done = true;
                    }
                }

                int hand = 0;
                foreach (int id in card.SubCards)
                    if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == player)
                        hand++;


                if (value > 4 || (value > 0 && hand > 0 && hand <= ai.GetOverflow(player)))
                {
                    if (damage_done)
                    {
                        CardUseStruct new_use = new CardUseStruct
                        {
                            From = player,
                            IsDummy = true,
                            To = new List<Player>()
                        };

                        List<WrappedCard> wrappeds = ai.GetCards(IronChain.ClassName, player);
                        UseCard ic = Engine.GetCardUsage(IronChain.ClassName);
                        foreach (WrappedCard c in wrappeds)
                        {
                            ic.Use(ai, player, ref new_use, c);
                            if (new_use.Card != null)
                                return;
                        }

                        List<WrappedCard> _wrappeds = ai.GetCards(FightTogether.ClassName, player);
                        UseCard ft = Engine.GetCardUsage(FightTogether.ClassName);
                        foreach (WrappedCard c in _wrappeds)
                        {
                            ft.Use(ai, player, ref new_use, c);
                            if (new_use.Card != null)
                                return;
                        }
                    }
                    use.Card = card;
                }
            }
        }
    }

    public class LureTigerAI : UseCard
    {
        public LureTigerAI() : base(LureTiger.ClassName)
        {
        }
        
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = ai.GetCards(BurningCamps.ClassName, player);
            FunctionCard lure = Engine.GetFunctionCard(Name);
            List<Player> players = new List<Player>();
            if (cards.Count > 0)
            {
                ai.SortByUseValue(ref cards);
                WrappedCard burn = cards[0];
                FunctionCard fcard = Engine.GetFunctionCard(BurningCamps.ClassName);
                if (fcard.IsAvailable(room, player, burn))
                {
                    Player nextp = room.GetNextAlive(player);
                    Player first = null;
                    while (nextp != null && nextp != player)
                    {
                        if (lure.TargetFilter(room, new List<Player>(), nextp, player, card) && ai.IsCardEffect(card, nextp, player))
                        {
                            if (first == null)
                            {
                                if (ai.IsEnemy(nextp))
                                    first = nextp;
                                else
                                    players.Add(nextp);
                            }
                            else
                            {
                                if (!RoomLogic.IsFriendWith(room, player, nextp))
                                    players.Add(nextp);
                            }
                            nextp = room.GetNextAlive(nextp);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (first != null && players.Count > 0)
                    {
                        for (int i = 0; i < 2 && i < players.Count; i++)
                            use.To.Add(players[i]);
                        use.Card = card;
                        return;
                    }
                }
            }

            players.Clear();
            cards = ai.GetCards(ArcheryAttack.ClassName, player);
            if (cards.Count > 0)
            {
                ai.SortByUseValue(ref cards);
                WrappedCard aa = cards[0];
                FunctionCard fcard = Engine.GetFunctionCard(ArcheryAttack.ClassName);
                if (fcard.IsAvailable(room, player, aa) && ai.GetAoeValue(aa))
                {
                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (RoomLogic.IsFriendWith(room, player, p) && ai.IsWeak(p) && !ai.IsCancelTarget(aa, p, player) && ai.IsCardEffect(aa, p, player)
                            && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player) && lure.TargetFilter(room, new List<Player>(), p, player, card))
                        {
                            players.Add(p);
                        }
                    }

                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (players.Contains(p)) continue;
                        if (RoomLogic.IsFriendWith(room, player, p) && !ai.IsCancelTarget(aa, p, player) && ai.IsCardEffect(aa, p, player)
                            && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player) && lure.TargetFilter(room, new List<Player>(), p, player, card))
                        {
                            players.Add(p);
                        }
                    }


                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (players.Contains(p)) continue;
                        if (ai.IsWeak(p) && !ai.IsCancelTarget(aa, p, player) && ai.IsCardEffect(aa, p, player)
                            && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player) && lure.TargetFilter(room, new List<Player>(), p, player, card))
                        {
                            players.Add(p);
                        }
                    }
                }

                if (players.Count > 0)
                {
                    use.Card = card;
                    for (int i = 0; i < 2 && i < players.Count; i++)
                        use.To.Add(players[i]);

                    return;
                }
            }
            players.Clear();
            cards = ai.GetCards(SavageAssault.ClassName, player);
            if (cards.Count > 0)
            {
                ai.SortByUseValue(ref cards);
                WrappedCard aa = cards[0];
                FunctionCard fcard = Engine.GetFunctionCard(SavageAssault.ClassName);
                if (fcard.IsAvailable(room, player, aa) && ai.GetAoeValue(aa))
                {
                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (RoomLogic.IsFriendWith(room, player, p) && ai.IsWeak(p) && !ai.IsCancelTarget(aa, p, player) && ai.IsCardEffect(aa, p, player)
                            && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player) && lure.TargetFilter(room, new List<Player>(), p, player, card))
                        {
                            players.Add(p);
                        }
                    }

                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (players.Contains(p)) continue;
                        if (RoomLogic.IsFriendWith(room, player, p) && !ai.IsCancelTarget(aa, p, player) && ai.IsCardEffect(aa, p, player)
                            && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player) && lure.TargetFilter(room, new List<Player>(), p, player, card))
                        {
                            players.Add(p);
                        }
                    }


                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (players.Contains(p)) continue;
                        if (ai.IsWeak(p) && !ai.IsCancelTarget(aa, p, player) && ai.IsCardEffect(aa, p, player)
                            && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player) && lure.TargetFilter(room, new List<Player>(), p, player, card))
                        {
                            players.Add(p);
                        }
                    }
                }

                if (players.Count > 0)
                {
                    use.Card = card;
                    for (int i = 0; i < 2 && i < players.Count; i++)
                        use.To.Add(players[i]);

                    return;
                }
            }

            cards = ai.GetCards(Slash.ClassName, player);
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (WrappedCard c in cards)
            {
                if (Slash.IsAvailable(room, player, c))
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (!ai.IsCancelTarget(c, p, player) && ai.IsCardEffect(c, p, player))
                        {
                            DamageStruct damage = new DamageStruct(c, player, p);
                            if (c.Name.Contains("Fire"))
                                damage.Nature = DamageStruct.DamageNature.Fire;
                            else if (c.Name.Contains("Thunder"))
                                damage.Nature = DamageStruct.DamageNature.Thunder;

                            ScoreStruct score = ai.GetDamageScore(damage);
                            scores.Add(score);
                        }
                    }
                }

                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    ScoreStruct result = scores[0];
                    Player to = result.Damage.To;
                    if (to == null && result.Players != null && result.Players.Count > 0)
                        to = result.Players[0];

                    if (to != null && !RoomLogic.CanSlash(room, player, to, result.Damage.Card) && RoomLogic.CanSlash(room, player, to, result.Damage.Card, -2))
                    {
                        int right = RoomLogic.OriginalRightDistance(room, player, to);
                        int left = room.AliveCount(false) - right;
                        if (right < left)
                        {
                            int i = 0;
                            Player nextp = room.GetNextAlive(player);
                            while (i < 2 && nextp != to)
                            {
                                players.Add(nextp);
                                nextp = room.GetNextAlive(nextp);
                                i++;
                            }
                        }
                        else
                        {
                            int i = 0;
                            Player lastp = room.GetLastAlive(player);
                            while (i < 2 && lastp != to)
                            {
                                players.Add(lastp);
                                lastp = room.GetLastAlive(lastp);
                                i++;
                            }
                        }
                        if (players.Count > 0)
                        {
                            use.Card = card;
                            use.To = players;
                            return;
                        }
                    }
                }
            }

            players.Clear();
            cards = ai.GetCards(GodSalvation.ClassName, player);
            if (cards.Count > 0)
            {
                foreach (Player p in ai.GetEnemies(player))
                {
                    if (ai.IsWeak(p) && lure.TargetFilter(room, new List<Player>(), p, player, card) && ai.IsCardEffect(card, p, player))
                        players.Add(p);
                }

                foreach (Player p in ai.GetEnemies(player))
                {
                    if (p.IsWounded() && !players.Contains(p) && lure.TargetFilter(room, new List<Player>(), p, player, card) && ai.IsCardEffect(card, p, player))
                        players.Add(p);
                }

                if (players.Count > 0)
                {
                    use.Card = card;
                    for (int i = 0; i < 2 && i < players.Count; i++)
                        use.To.Add(players[i]);

                    return;
                }
            }
            /*
            players.Clear();
            if (player == room.Current)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (lure.TargetFilter(room, new List<Player>(), p, player, card) && ai.IsCardEffect(card, p, player))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }
            }
            */
        }
    }

    public class FightTogetherAI : UseCard
    {
        public FightTogetherAI() : base(FightTogether.ClassName)
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player player = ai.Self;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            double ed = 0, no = 0;
            bool single = true;
            if (positive)
            {
                if (to.Chained)
                {
                    if (ai.IsEnemy(to))
                    {
                        int count = 0;
                        foreach (Player p in targets)
                        {
                            if (RoomLogic.IsFriendWith(room, to, p) && ai.IsCardEffect(trick, p, from))
                            {
                                if (p.Chained)
                                {
                                    count++;
                                    ed++;
                                    if (ai.HasSkill(TrustedAI.CardneedSkill, to) || ai.HasSkill(TrustedAI.PrioritySkill, to)
                                            || ai.HasSkill(TrustedAI.WizardHarmSkill, to))
                                        ed += 0.5;
                                }
                                else if (RoomLogic.CanBeChainedBy(room, p, from))
                                    no++;
                            }
                        }
                        
                        if (count > 1 && ed > no && (ai.GetCards(HegNullification.ClassName, player).Count > 0 || (room.NullTimes > 0 && room.HegNull)))
                        {
                            result.Null = true;
                            result.Heg = room.NullTimes == 0;
                        }
                    }
                }
                else
                {
                    Player vine = null;
                    bool use = false;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (ai.HasArmorEffect(p, Vine.ClassName) && (p.Chained || (targets.Contains(p) && ai.IsCardEffect(trick, p, from) && RoomLogic.CanBeChainedBy(room, p, from))))
                        {
                            vine = p;
                            break;
                        }
                    }

                    bool heg_null_card = ai.GetCards(HegNullification.ClassName, player).Count > 0
                        || (room.NullTimes > 0 && room.HegNull);
                    if (ai.IsFriend(to) && vine != null)
                    {
                        if (to == vine || vine.Chained || heg_null_card)
                            use = true;

                        int count = 0;
                        foreach (Player p in targets)
                        {
                            if (RoomLogic.IsFriendWith(room, to, p) && ai.IsCardEffect(trick, p, from))
                            {
                                if (p.Chained)
                                    ed++;
                                else if (RoomLogic.CanBeChainedBy(room, p, from))
                                {
                                    no++;
                                    count++;
                                }
                            }
                        }

                        if (count > 1 && no >= ed && room.NullTimes == 0)
                            single = false;

                    }
                    else if (RoomLogic.IsFriendWith(room, player, to) && heg_null_card)
                    {
                        no = 1;
                        int count = 0;
                        foreach (Player p in targets)
                        {
                            if (!RoomLogic.IsFriendWith(room, to, p) || !ai.IsCardEffect(trick, p, from)) continue;
                            if (p.Chained)
                                ed++;
                            else if (RoomLogic.CanBeChainedBy(room, p, from))
                            {
                                count++;
                                no++;
                            }
                        }
                        if (count > 1 && no >= ed)
                        {
                            use = true;
                            single = room.NullTimes == 0;
                        }

                    }
                    result.Null = use;
                    result.Heg = single;
                }
            }
            else
            {
                if (RoomLogic.IsFriendWith(room, player, to) && to.Chained && !keep && room.NullTimes > 0 && room.HegNull)
                {
                    int count = 0;
                    foreach (Player p in targets)
                    {
                        if (RoomLogic.IsFriendWith(room, to, p))
                        {
                            if (p.Chained)
                            {
                                ed++;
                                count++;
                            }
                            else
                            {
                                no++;
                            }
                        }
                    }
                    if (count > 1 && ed >= no)
                        result.Null = true;
                }
            }

            return result;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.To = new List<Player>();
            ai.Choice[Name] = string.Empty;
            Room room = ai.Room;
            List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
            List<Player> bigs = new List<Player>(), smalls = new List<Player>();
            if (big_kingdoms.Count > 0)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    string kingdom = string.Empty;
                    if (p.HasShownOneGeneral())
                        kingdom = p.GetRoleEnum() == Player.PlayerRole.Careerist ? p.Name : p.Kingdom;

                    if (ai.IsCardEffect(card, p, player))
                    {
                        if (big_kingdoms.Contains(kingdom))
                        {
                            bigs.Add(p);
                        }
                        else
                        {
                            if (RoomLogic.CanBeChainedBy(room, p, player))
                            {
                                smalls.Add(p);
                            }
                        }
                    }
                }
            }
            List<string> choices = new List<string>();
            if (bigs.Count > 0) choices.Add("big");
            if (smalls.Count > 0) choices.Add("small");
            if (choices.Count > 0)
            {
                int v_big = 0, v_small = 0;
                if (choices.Contains("big"))
                {
                    foreach (Player p in bigs)
                    {
                        if (ai.IsFriend(p))
                        {
                            if (p.Chained) v_big++;
                            else
                                v_big--;
                        }
                        else if (ai.IsEnemy(p))
                        {
                            if (p.Chained)
                                v_big--;
                            else
                            {
                                string gameProcess = ai.Process;
                                if (p.HasShownOneGeneral() && gameProcess.Contains(p.Kingdom + ">>"))
                                    v_big += 2;
                                else
                                    v_big++;
                            }
                        }
                    }
                }

                if (choices.Contains("small"))
                {
                    foreach (Player p in smalls)
                    {
                        if (ai.IsFriend(p))
                        {
                            if (p.Chained) v_small++;
                            else
                                v_small--;
                        }
                        else if (ai.IsEnemy(p))
                        {
                            if (p.Chained)
                                v_small--;
                            else
                            {
                                string gameProcess = ai.Process;
                                if (p.HasShownOneGeneral() && gameProcess.Contains(p.Kingdom + ">>"))
                                    v_small += 2;
                                else
                                    v_small++;
                            }
                        }
                    }
                }
                int win = Math.Max(v_small, v_big);

                if (win > 1)
                {
                    if (win == v_big && bigs.Count > 1)             //目标只有1个还不如重铸
                    {
                        ai.Choice[Name] = "big";
                        use.To.Add(bigs[0]);
                    }
                    else if (win == v_small && smalls.Count > 1)
                    {
                        ai.Choice[Name] = "small";
                        use.To.Add(smalls[0]);
                    }
                }
            }

            FunctionCard ft = Engine.GetFunctionCard(Name);
            if (ai.Choice[Name] == string.Empty && ft.CanRecast(room, player, card))
            {
                ai.Choice[Name] = "recast";
            }
            if (ai.Choice[Name] != string.Empty)
            {
                use.Card = card;
            }
        }
    }

    public class AllianceFeastAI : UseCard
    {
        public AllianceFeastAI() : base(AllianceFeast.ClassName)
        {
        }

        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            string[] strs = choices.Split('+');
            return strs[0];
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            if (!ai.IsFriend(to) && !ai.IsEnemy(to)) return result;
            Room room = ai.Room;
            Player player = ai.Self;

            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            if (from == to)
            {
                int draw = from.GetMark(Name);
                double from_value = 0;
                if (from.IsWounded())
                {
                    int heal = Math.Min(draw, from.GetLostHp());
                    from_value += 1.5 * heal;
                    draw -= heal;
                    if (ai.HasSkill(TrustedAI.MasochismSkill, to))
                        from_value += heal;
                }

                from_value += draw;
                if (draw > 0 && ai.HasSkill(TrustedAI.CardneedSkill, from))
                    from_value += 1.5 * draw;

                if ((ai.IsFriend(to) && positive) || (ai.IsEnemy(to) && !positive))
                    from_value = 0 - from_value;

                if (from_value > 2)
                    result.Null = true;
            }
            else
            {
                List<WrappedCard> cards = ai.GetCards(HegNullification.ClassName, player);
                bool hegnull = (cards.Count > 0 && room.NullTimes == 0)
                    || (room.NullTimes > 0 && room.HegNull);

                double value = 0;
                int count = 0;
                if (hegnull)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.IsCardEffect(trick, p, player))
                        {
                            count++;
                            value++;
                            //if (p.IsWounded())
                            //{
                            //    value += 1.8;
                            //    if (ai.HasSkill(TrustedAI.MasochismSkill, p))
                            //        value++;
                            //}
                            //else
                            //{
                                if (ai.HasSkill(TrustedAI.CardneedSkill, p)) value++;
                                if (p.Chained) value++;
                           //}
                        }
                    }

                    if (positive)
                    {
                        if (ai.IsEnemy(to) && (value > 3 && !keep || value > 6))
                        {
                            result.Null = true;
                            if (count > 1)
                                result.Heg = true;
                        }
                    }
                    else
                    {
                        if (ai.IsFriend(to) && (value > 3 && !keep || value > 6))
                            result.Null = true;
                    }
                }
                else
                {
                    if (ai.IsCardEffect(trick, to, player))
                    {
                        value++;
                        //if (to.IsWounded())
                        //{
                        //    value += 1.8;
                        //    if (ai.HasSkill(TrustedAI.MasochismSkill, to))
                        //        value++;
                        //}
                        //else
                        //{
                            if (ai.HasSkill(TrustedAI.CardneedSkill, to)) value++;
                            if (to.Chained) value++;
                        //}
                    }

                    if (value > 3 && !keep)
                    {
                        if ((positive && ai.IsEnemy(to)) || (!positive && ai.IsFriend(to)))
                            result.Null = true;
                    }
                }
            }
            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<WrappedCard> hegnullcards = ai.GetCards(HegNullification.ClassName, player);
            List<string> effect_kingdoms = new List<string>();
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!RoomLogic.IsFriendWith(room, player, p) && p.HasShownOneGeneral() && ai.IsCardEffect(card, p, player))
                {
                    if (p.GetRoleEnum() == Player.PlayerRole.Careerist)
                        effect_kingdoms.Add(p.Name);
                    else if (!effect_kingdoms.Contains(p.Kingdom))
                        effect_kingdoms.Add(p.Kingdom);
                }
            }
            if (effect_kingdoms.Count == 0) return;
            double max_v = 0;
            string winner = string.Empty;

            foreach (string kingdom in effect_kingdoms)
            {
                double value = 0;
                if (kingdom.Contains("SGS"))
                {
                    value++;
                    if (ai.HasSkill(TrustedAI.CardneedSkill, player)) value += 0.5;
                    Player target = room.FindPlayer(kingdom);
                    if (ai.IsFriend(target))
                    {
                        value++;
                        //if (target.IsWounded())
                        //{
                        //    value += 1.8;
                        //    if (ai.HasSkill(TrustedAI.MasochismSkill, target))
                        //        value++;
                        //}
                        //else
                        //{
                            if (ai.HasSkill(TrustedAI.CardneedSkill, target)) value++;
                            if (target.Chained) value++;
                        //}
                    }
                    else if (ai.IsEnemy(target))
                    {
                        value--;
                        //if (target.IsWounded())
                        //{
                        //    value -= 1.8;
                        //    if (ai.HasSkill(TrustedAI.MasochismSkill, target))
                        //        value--;
                        //}
                        //else
                        //{
                            if (ai.HasSkill(TrustedAI.CardneedSkill, target)) value--;
                            if (target.Chained) value--;
                        //}
                    }
                }
                else
                {
                    double self_value = 0;
                    double enemy_value = 0;
                    int draw = 0;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.HasShownOneGeneral() && p.Role != "careerist" && p.Kingdom == kingdom)
                        {
                            draw++;
                            if (ai.HasSkill(TrustedAI.CardneedSkill, player)) self_value += 0.5;
                            if (ai.IsFriend(p) && ai.IsCardEffect(card, p, player))
                            {
                                self_value++;
                                //if (p.IsWounded())
                                //{
                                //    self_value += 1.8;
                                //    if (ai.HasSkill(TrustedAI.MasochismSkill, p)) self_value++;
                                //}
                                //else
                                //{
                                    if (ai.HasSkill(TrustedAI.CardneedSkill, p)) self_value++;
                                    if (p.Chained) self_value++;
                                //}
                            }
                            else if (ai.IsEnemy(p) && ai.IsCardEffect(card, p, player))
                            {
                                enemy_value++;
                                //if (p.IsWounded())
                                //{
                                //    enemy_value += 1.8;
                                 //   if (ai.HasSkill(TrustedAI.MasochismSkill, p)) enemy_value++;
                                //}
                                //else
                                //{
                                    if (ai.HasSkill(TrustedAI.CardneedSkill, p)) enemy_value++;
                                    if (p.Chained) enemy_value++;
                                //}
                            }
                        }
                    }

                    if (player.IsWounded())
                    {
                        int heal = Math.Min(draw, player.GetLostHp());
                        self_value += 1.5 * heal;
                        draw -= heal;
                        if (ai.HasSkill(TrustedAI.MasochismSkill, player))
                            self_value += heal;
                    }
                    self_value += draw;

                    if (self_value >= 3 && enemy_value > 5 && hegnullcards.Count > 0)
                        enemy_value = enemy_value / 2;
                    value = self_value - enemy_value;
                }

                if (value > max_v)
                {
                    winner = kingdom;
                    max_v = value;
                }
            }

            if (winner != string.Empty)
            {
                Player target = null;
                if (winner.StartsWith("SGS"))
                    target = room.FindPlayer(winner);
                else
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.HasShownOneGeneral() && p.Role != "careerist" && p.Kingdom == winner && ai.IsCardEffect(card, p, player))
                        {
                            target = p;
                            break;
                        }
                    }
                }
                if (target != null)
                {
                    use.Card = card;
                    use.To.Add(target);
                }
            }
        }
    }

    public class ThreatenEmperorAI : UseCard
    {
        public ThreatenEmperorAI() : base(ThreatenEmperor.ClassName)
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            List<int> ids = player.GetCards("h");
            if (ids.Count == 0) return use;
            Room room = ai.Room;
            if (player.FaceUp)
            {
                ai.SortByKeepValue(ref ids, false);
                foreach (int id in ids)
                {
                    if (!ai.IsCard(id, JadeSeal.ClassName, player))
                    {
                        use.Card = room.GetCard(id);
                        break;
                    }
                }

                if (use.Card == null)
                    use.Card = room.GetCard(ids[0]);
            }
            else
            {
                ai.SortByKeepValue(ref ids, false);
                use.Card = room.GetCard(ids[0]);
            }

            return use;
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            if (positive)
            {
                if (ai.IsEnemy(from) && !from.IsNude()) result.Null = true;
            }
            else
            {
                if (from == ai.Self)
                {
                    List<WrappedCard> cards = ai.GetCards(Nullification.ClassName, ai.Self);
                    if (cards.Count == 1)
                    {
                        List<int> ids = ai.Self.GetCards("he");
                        foreach (int id in cards[0].SubCards)
                            ids.Remove(id);

                        if (ids.Count == 0 && (cards[0].IsVirtualCard() || !ai.HasSkill("jizhi", ai.Self)))
                            return result;
                    }
                    result.Null = true;
                }
                else if (ai.IsFriend(from) && !from.IsNude())
                    result.Null = true;
            }
            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (ai.IsCardEffect(card, player, player) && !ai.IsCancelTarget(card, player, player))
            {
                List<int> ids= player.GetCards("h");
                ids.RemoveAll(t => card.SubCards.Contains(t));

                if (ai.HasSkill("jizhi") && !card.IsVirtualCard() || ids.Count > 0)
                {
                    use.Card = card;
                }
            }
        }
    }

    public class EdictAI : UseCard
    {
        public EdictAI() : base(Edict.ClassName)
        {
        }

        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            return "show";
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            if (ai.HasArmorEffect(player, SilverLion.ClassName) && player.IsWounded())
                use.Card = ai.Room.GetCard(player.Armor.Key);

            return use;
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (ai.IsSituationClear() && player.HasShownOneGeneral())
                return -8;

            return 0;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            bool will_use = !ai.IsSituationClear();
            if (!will_use)
            {
                if (!player.HasShownOneGeneral())
                    will_use = true;
                else if (ai.GetOverflow(player) > 0)
                {
                    Room room = ai.Room;
                    foreach (int id in card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == player)
                        {
                            will_use = true;
                            break;
                        }
                    }
                }
            }

            if (will_use)
                use.Card = card;
        }
    }
}