using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static CommonClass.Game.WrappedCard;
namespace SanguoshaServer.AI
{
    class StrategicAdvantageCardsAI : AIPackage
    {
        public StrategicAdvantageCardsAI() : base("StrategicAdvantageCardsAI")
        {
            events = new List<SkillEvent>
            {
                new TransferAI(),
                new WoodenAI(),
                new JadeAI(),
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
                new ImperialOrderAI(),
            };
        }
    }

    public class TransferCardAI : UseCard
    {
        public TransferCardAI() : base("TransferCard")
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
            if (ai.Target["transfer"] != null)
            {
                use.Card = card;
                use.To.Add(ai.Target["transfer"]);
            }
        }
    }

    public class TransferAI : SkillEvent
    {
        public TransferAI() : base("transfer")
        {
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.HandCards)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Transferable)
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
                    KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, friend);
                    if (pair.Key != null)
                    {
                        WrappedCard card = new WrappedCard("TransferCard");
                        card.AddSubCard(pair.Value);
                        ai.Target[Name] = pair.Key;
                        return card;
                    }
                }

                List<int> cards = new List<int>();
                bool oneJink = ai.HasSkill("kongcheng");
                foreach (int id in ids)
                {
                    if (!ai.IsCard(id, "Peach", player) || friend.Count > 0)
                    {
                        bool insert = true;
                        if (!oneJink && ai.IsCard(id, "Jink", player))
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

                if (cards.Count == 0) return null;
                if (friend_other.Count > 0)
                {
                    KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(cards, friend_other);
                    if (pair.Key != null)
                    {
                        WrappedCard card = new WrappedCard("TransferCard");
                        card.AddSubCard(pair.Value);
                        ai.Target[Name] = pair.Key;
                        return card;
                    }
                }

                if (!player.HasShownOneGeneral()) return null;

                foreach (int id in cards)
                {
                    WrappedCard c = room.GetCard(id);
                    if (c.Name == "ThreatenEmperor")
                    {
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
                            if (big_kingdom.StartsWith("sgs"))
                                maxNum = 99;
                            else
                                maxNum = RoomLogic.GetPlayerNumWithSameKingdom(room, player, big_kingdom);
                        }
                        else
                            maxNum = 99;

                        foreach (Player p in targets)
                        {
                            if (p.HasShownOneGeneral() && p.Name != big_kingdom && (!big_kingdoms.Contains(p.Kingdom) || p.Role == "careerist")
                                && (maxNum == 99 || RoomLogic.GetPlayerNumWithSameKingdom(room, p) + anjiang < maxNum))
                            {
                                WrappedCard card = new WrappedCard("TransferCard");
                                card.AddSubCard(id);
                                ai.Target[Name] = p;
                                return card;
                            }
                        }
                    }
                    else if (c.Name == "BurningCamps")
                    {
                        string gameProcess = ai.Process;
                        if (gameProcess.Contains(player.Kingdom + ">"))
                        {
                            foreach (Player p in targets)
                            {
                                if (p.HasShownOneGeneral() && (ai.IsFriend(p) || ai.WillSkipPlayPhase(p)))
                                {
                                    WrappedCard card = new WrappedCard("TransferCard");
                                    card.AddSubCard(id);
                                    ai.Target[Name] = p;
                                    return card;
                                }
                            }
                        }
                        else
                        {
                            FunctionCard fcard = Engine.GetFunctionCard(c.Name);
                            foreach (Player p in targets)
                            {
                                if (p.HasShownOneGeneral() && fcard.IsAvailable(room, p, c))
                                {
                                    Player np = room.GetNextAlive(p);
                                    DamageStruct damage = new DamageStruct(c, p, np, 1, DamageStruct.DamageNature.Fire);
                                    damage.Damage = ai.DamageEffect(damage, DamageStruct.DamageStep.None);
                                    if (!ai.IsFriend(np) && (!np.Chained || !ai.IsGoodSpreadStarter(damage, false)))
                                    {
                                        WrappedCard card = new WrappedCard("TransferCard");
                                        card.AddSubCard(id);
                                        ai.Target[Name] = p;
                                        return card;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }


    //装备
    public class BladeAI : UseCard
    {
        public BladeAI() : base("Blade")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
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
        public HalberdAI() : base("Halberd")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            List<string> kingdoms = new List<string>();
            foreach (Player p in ai.Room.GetOtherPlayers(player))
            {
                if (ai.IsEnemy(p) && (!p.HasShownOneGeneral() || p.Role == "careerist" || !kingdoms.Contains(p.Kingdom)))
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
        public HalberdChooseAI() : base("Halberd")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> players, int min, int max, object data)
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

            foreach (Player p in room.GetAlivePlayers())
            {
                if (p != player && (p.HasShownOneGeneral() && p.Role != "careerist" && !kingdoms.Contains(p.Kingdom) || !p.HasShownOneGeneral()))
                    targets.Add(p);
            }

            if (targets.Count == 0) return targets;
            Player target = ai.AddExtraSlashTarget(targets, use);
            while (target != null)
            {
                result.Add(target);
                use.To.Add(target);
                targets.Clear();
                kingdoms.Clear();

                foreach (Player p in use.To)
                {
                    if (p.HasShownOneGeneral() && p.Role != "careerist")
                        kingdoms.Add(p.Kingdom);
                }

                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p != player && (p.HasShownOneGeneral() && p.Role != "careerist" && !kingdoms.Contains(p.Kingdom) || !p.HasShownOneGeneral()))
                        targets.Add(p);
                }

                if (targets.Count == 0) return result;
                target = ai.AddExtraSlashTarget(targets, use);
            }
            return result;
        }
    }

    public class BreastPlateAI : UseCard
    {
        public BreastPlateAI() : base("BreastPlate")
        { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
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
        public IronArmorAI() : base("IronArmor")
        {
        }
        
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class WoodenOxCardAI : UseCard
    {
        public WoodenOxCardAI() : base("WoodenOxCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class WoodenAI : SkillEvent
    {
        public WoodenAI() : base("WoodenOx")
        {
            key = new List<string> { "playerChosen" };
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
                    if (to != null)
                    {
                        if (ai.GetPossibleId(player).Count == 1 && ai.GetPlayerTendency(to) == "unknown" && ai.IsKnown(player, to) && !to.HasShownOneGeneral())
                            ai.UpdatePlayerRelation(player, to, true);
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max, object data)
        {
            if (ai.Target[Name] != null)
                return new List<Player> { ai.Target[Name] };

            return new List<Player>();
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            ai.Target[Name] = null;
            if (player.HasUsed("WoodenOxCard") || player.IsKongcheng()) return null;
            List<int> cards = new List<int>(player.HandCards);
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
                {
                    if (p.Treasure.Key == -1)
                        targets.Add(p);
                }

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
                    KeyValuePair<Player, int> result = ai.GetCardNeedPlayer(cards, new List<Player> { next });
                    if (result.Key != null)
                    {
                        ai.Target[Name] = next;
                        WrappedCard card = new WrappedCard("WoodenOxCard");
                        card.AddSubCard(result.Value);
                        return card;
                    }
                }
                else if (ai.HasSkill(TrustedAI.LoseEquipSkill) && targets.Count > 0)
                {
                    ai.Target[Name] = targets[0];
                    ai.SortByUseValue(ref cards, false);
                    WrappedCard card = new WrappedCard("WoodenOxCard");
                    card.AddSubCard(cards[0]);
                    return card;
                }
                else if (targets.Count > 0)
                {
                    List<Player> weaks = new List<Player>();
                    foreach (Player p in targets)
                    {
                        if (ai.IsWeak(p))
                            weaks.Add(p);
                    }

                    KeyValuePair<Player, int> result = ai.GetCardNeedPlayer(cards, weaks);
                    if (result.Key != null)
                    {
                        ai.Target[Name] = result.Key;
                        WrappedCard card = new WrappedCard("WoodenOxCard");
                        card.AddSubCard(result.Value);
                        return card;
                    }
                }
            }

            if (ai.GetOverflow(player) > 0)
            {
                ai.SortByKeepValue(ref cards, false);
                WrappedCard card = new WrappedCard("WoodenOxCard");
                card.AddSubCard(cards[0]);
                return card;
            }

            return null;
        }
    }

    public class WoodenOxAI : UseCard
    {
        public WoodenOxAI() : base("WoodenOx")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
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
        public JadeAI() : base("JadeSeal")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            WrappedCard kb = new WrappedCard("KnownBoth");
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                IsDummy = true
            };

            UseCard u = Engine.GetCardUsage("KnownBoth");
            u.Use(ai, player, ref use, kb);

            return use;
        }
    }

    public class JadeSealAI : UseCard
    {
        public JadeSealAI() : base("JadeSeal")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
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
        public DrowningAI() : base("Drowning")
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

            if (target != null)
            {
                use.Card = card;
                use.To = new List<Player> { target };
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            return base.OnNullification(ai, from, to, trick, positive, keep);
        }
    }

    public class BurningCampsAI : UseCard
    {
        public BurningCampsAI() : base("BurningCamps")
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player player = ai.Self;

            List<Player> targets = (List<Player>)room.GetTag("targets" + RoomLogic.CardToString(room, trick));
            List<Player> delete = new List<Player>(targets);
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
                if (ai.IsGoodSpreadStarter(damage, false))
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
                    if (!chained && ai.IsGoodSpreadStarter(damage))
                    {
                        chained = true;
                        foreach (Player _p in room.GetOtherPlayers(p))
                        {
                            if (p.Chained)
                            {
                                DamageStruct _damage = new DamageStruct(trick, from, _p, 1, DamageStruct.DamageNature.Fire);
                                value += ai.GetDamageScore(damage).Score;
                            }
                        }
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
            Player np = room.GetNextAlive(player, 1, false);
            if (ai.IsFriend(np)) return;

            List<Player> targets = RoomLogic.GetFormation(room, np);
            if (targets.Count > 0)
            {
                double value = 0;
                bool damage_done = false;
                foreach (Player p in targets)
                {
                    if (!ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                    {
                        DamageStruct damage = new DamageStruct(card, player, p, 1, DamageStruct.DamageNature.Fire);
                        ScoreStruct score = ai.GetDamageScore(damage);
                        value += score.Score;
                        if (score.DoDamage)
                            damage_done = true;
                    }
                }

                if (value > 4)
                {
                    if (damage_done)
                    {
                        CardUseStruct new_use = new CardUseStruct
                        {
                            From = player,
                            IsDummy = true
                        };

                        List<WrappedCard> wrappeds = ai.GetCards("IronChain", player);
                        UseCard ic = Engine.GetCardUsage("IronChain");
                        foreach (WrappedCard c in wrappeds)
                        {
                            ic.Use(ai, player, ref new_use, c);
                            if (new_use.Card != null)
                                return;
                        }

                        List<WrappedCard> _wrappeds = ai.GetCards("FightTogether", player);
                        UseCard ft = Engine.GetCardUsage("FightTogether");
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
        public LureTigerAI() : base("LureTiger")
        {
        }
        
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = ai.GetCards("BurningCamps", player);
            FunctionCard lure = Engine.GetFunctionCard(Name);
            List<Player> players = new List<Player>();
            if (cards.Count > 0)
            {
                ai.SortByUseValue(ref cards);
                WrappedCard burn = cards[0];
                FunctionCard fcard = Engine.GetFunctionCard("BurningCamps");
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
                        use.Card = card;
                        use.To = players;
                        return;
                    }
                }
            }

            players.Clear();
            cards = ai.GetCards("ArcheryAttack", player);
            if (cards.Count > 0)
            {
                ai.SortByUseValue(ref cards);
                WrappedCard aa = cards[0];
                FunctionCard fcard = Engine.GetFunctionCard("ArcheryAttack");
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
            cards = ai.GetCards("SavageAssault", player);
            if (cards.Count > 0)
            {
                ai.SortByUseValue(ref cards);
                WrappedCard aa = cards[0];
                FunctionCard fcard = Engine.GetFunctionCard("SavageAssault");
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

            cards = ai.GetCards("Slash", player);
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
                    if (!RoomLogic.CanSlash(room, player, to, result.Damage.Card) && RoomLogic.CanSlash(room, player, to, result.Damage.Card, -2))
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
            cards = ai.GetCards("GodSalvation", player);
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
        }
    }

    public class FightTogetherAI : UseCard
    {
        public FightTogetherAI() : base("FightTogether")
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player player = ai.Self;

            List<Player> targets = (List<Player>)room.GetTag("targets" + RoomLogic.CardToString(room, trick));
            List<Player> delete = new List<Player>(targets);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            int ed = 0, no = 0;
            bool single = true;
            if (positive)
            {
                if (to.Chained)
                {
                    if (ai.IsEnemy(to) && (ai.HasSkill(TrustedAI.CardneedSkill, to) || ai.HasSkill(TrustedAI.PrioritySkill, to)
                            || ai.HasSkill(TrustedAI.WizardHarmSkill, to)))
                    {
                        foreach (Player p in targets)
                            if (p.Chained)
                                ed++;
                            else
                                no++;
                    }

                    if (targets.Count > 0 && ed > no)
                        single = false;

                    result.Null = true;
                    result.Heg = single;
                }
                else
                {
                    Player vine = null;
                    bool use = false;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (ai.HasArmorEffect(p, "Vine") && (p.Chained || targets.Contains(p)))
                        {
                            vine = p;
                            break;
                        }
                    }

                    bool heg_null_card = ai.GetCards("HegNullification", player).Count > 0
                        || (room.NullTimes > 0 && room.HegNull);
                    if (ai.IsFriend(to) && vine != null)
                    {
                        if (to == vine || vine.Chained || heg_null_card)
                            use = true;
                        foreach (Player p in targets)
                        {
                            if (p.Chained)
                                ed++;
                            else
                                no++;
                        }
                        if (targets.Count > 0 && no >= ed)
                            single = false;

                    }
                    else if (RoomLogic.IsFriendWith(room, player, to) && heg_null_card)
                    {
                        no = 1;
                        foreach (Player p in targets)
                        {
                            if (p.Chained)
                                ed++;
                            else
                                no++;
                        }
                        if (targets.Count > 0 && no >= ed)
                            use = true;

                    }
                    result.Null = use;
                    result.Heg = single;
                }
            }
            else
            {
                if (RoomLogic.IsFriendWith(room, player, to) && to.Chained && !keep)
                    result.Null = true;
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
                        kingdom = p.Role == "careerist" ? p.Name : p.Kingdom;

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
                    if (win == v_big)
                    {
                        ai.Choice[Name] = "big";
                        use.To.Add(bigs[0]);
                    }
                    else if (win == v_small)
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
        public AllianceFeastAI() : base("AllianceFeast")
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (!ai.IsFriend(to) && !ai.IsEnemy(to)) return result;
            Room room = ai.Room;
            Player player = ai.Self;

            List<Player> targets = (List<Player>)room.GetTag("targets" + RoomLogic.CardToString(room, trick));
            List<Player> delete = new List<Player>(targets);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            if (from == to)
            {
                double from_value = from.GetMark(Name);
                if (ai.HasSkill(TrustedAI.CardneedSkill, from))
                    from_value *= 1.5;
                if ((ai.IsFriend(to) && positive) || (ai.IsEnemy(to) && !positive))
                    from_value = 0 - from_value;

                if (from_value > 2)
                    result.Null = true;
            }
            else
            {
                List<WrappedCard> cards = ai.GetCards("HegNullification", player);
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
                            if (p.IsWounded())
                            {
                                value += 1.8;
                                if (ai.HasSkill(TrustedAI.MasochismSkill, p))
                                    value++;
                            }
                            else
                            {
                                if (ai.HasSkill(TrustedAI.CardneedSkill, p)) value++;
                                if (p.Chained) value++;
                            }
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
                        if (to.IsWounded())
                        {
                            value += 1.8;
                            if (ai.HasSkill(TrustedAI.MasochismSkill, to))
                                value++;
                        }
                        else
                        {
                            if (ai.HasSkill(TrustedAI.CardneedSkill, to)) value++;
                            if (to.Chained) value++;
                        }
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
            List<WrappedCard> hegnullcards = ai.GetCards("HegNullification", player);
            List<string> effect_kingdoms = new List<string>();
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!RoomLogic.IsFriendWith(room, player, p) && p.HasShownOneGeneral() && ai.IsCardEffect(card, p, player))
                {
                    if (p.Role == "careerist")
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
                if (kingdom.Contains("sgs"))
                {
                    value++;
                    if (ai.HasSkill(TrustedAI.CardneedSkill, player)) value += 0.5;
                    Player target = room.FindPlayer(kingdom);
                    if (ai.IsFriend(target))
                    {
                        value++;
                        if (target.IsWounded())
                        {
                            value += 1.8;
                            if (ai.HasSkill(TrustedAI.MasochismSkill, target))
                                value++;
                        }
                        else
                        {
                            if (ai.HasSkill(TrustedAI.CardneedSkill, target)) value++;
                            if (target.Chained) value++;
                        }
                    }
                    else if (ai.IsEnemy(target))
                    {
                        value--;
                        if (target.IsWounded())
                        {
                            value -= 1.8;
                            if (ai.HasSkill(TrustedAI.MasochismSkill, target))
                                value--;
                        }
                        else
                        {
                            if (ai.HasSkill(TrustedAI.CardneedSkill, target)) value--;
                            if (target.Chained) value--;
                        }
                    }
                }
                else
                {
                    double self_value = 0;
                    double enemy_value = 0;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.HasShownOneGeneral() && p.Role != "careerist" && p.Kingdom == kingdom)
                        {
                            self_value++;
                            if (ai.HasSkill(TrustedAI.CardneedSkill, player)) self_value += 0.5;
                            if (ai.IsFriend(p) && ai.IsCardEffect(card, p, player))
                            {
                                self_value++;
                                if (p.IsWounded())
                                {
                                    self_value += 1.8;
                                    if (ai.HasSkill(TrustedAI.MasochismSkill, p)) self_value++;
                                }
                                else
                                {
                                    if (ai.HasSkill(TrustedAI.CardneedSkill, p)) self_value++;
                                    if (p.Chained) self_value++;
                                }
                            }
                            else if (ai.IsEnemy(p) && ai.IsCardEffect(card, p, player))
                            {
                                enemy_value++;
                                if (p.IsWounded())
                                {
                                    enemy_value += 1.8;
                                    if (ai.HasSkill(TrustedAI.MasochismSkill, p)) enemy_value++;
                                }
                                else
                                {
                                    if (ai.HasSkill(TrustedAI.CardneedSkill, p)) enemy_value++;
                                    if (p.Chained) enemy_value++;
                                }
                            }
                        }
                    }
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
                if (winner.StartsWith("sgs"))
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
        public ThreatenEmperorAI() : base("ThreatenEmperor")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            List<int> ids = player.GetCards("he");
            if (ids.Count == 0) return use;
            Room room = ai.Room;
            if (player.FaceUp)
            {
                ai.SortByKeepValue(ref ids, false);
                foreach (int id in ids)
                {
                    if (!ai.IsCard(id, "JadeSeal", player) && !ai.IsCard(id, "JadeSeal", player))
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

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (positive)
            {
                if (ai.IsEnemy(from) && !from.IsNude()) result.Null = true;
            }
            else
            {
                if (from == ai.Self)
                {
                    List<WrappedCard> cards = ai.GetCards("Nullification", ai.Self);
                    if (cards.Count == 1)
                    {
                        List<int> ids = ai.Self.GetCards("he");
                        foreach (int id in cards[0].SubCards)
                            ids.Remove(id);

                        if (ids.Count == 0 && (RoomLogic.IsVirtualCard(ai.Room, cards[0]) || !ai.HasSkill("jizhi", ai.Self)))
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
                List<int> ids = player.GetCards("he");
                foreach (int id in card.SubCards)
                    ids.Remove(id);

                if (ai.HasSkill("jizhi") && !RoomLogic.IsVirtualCard(ai.Room, card) || ids.Count > 0)
                {
                    use.Card = card;
                }
            }
        }
    }

    public class ImperialOrderAI : UseCard
    {
        public ImperialOrderAI() : base("ImperialOrder")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            if (ai.HasArmorEffect(player, "SilverLion") && player.IsWounded())
            {
                use.Card = ai.Room.GetCard(player.Armor.Key);
            }
            return use;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }
}