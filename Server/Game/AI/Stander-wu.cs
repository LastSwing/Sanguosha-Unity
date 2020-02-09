using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class StanderWuAI : AIPackage
    {
        public StanderWuAI() : base("Stander-wu")
        {
            events = new List<SkillEvent>
            {
                new ZhihengAI(),
                new QixiAI(),
                new KejiAI(),
                new MouduanAI(),
                new KurouAI(),
                new YingziZAI(),
                new FanjianAI(),
                new GuoseAI(),
                new LiuliAI(),
                new QianxunAI(),
                new DuoshiAI(),
                new YinghunJAI(),
                new TianxiangAI(),
                new TianyiAI(),
                new BuquAI(),
                new FenjiAI(),
                new XiaojiAI(),
                new JieyinAI(),
                new HaoshiAI(),
                new DimengAI(),
                new ZhijianAI(),
                new GuzhengAI(),
                new FenxunAI(),
                new DuanbingAI()
            };

            use_cards = new List<UseCard>
            {
                new ZhihengCardAI(),
                new KurouCardAI(),
                new FanjianCardAI(),
                new TianyiCardAI(),
                new JieyinCardAI(),
                new DimengCardAI(),
                new HaoshiCardAI(),
                new ZhijianCardAI(),
                new FenxunCardAI()
            };
        }
    }

    public class ZhihengAI : SkillEvent
    {
        public ZhihengAI() : base("zhiheng")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if ((ai.WillShowForAttack() || ai.WillShowForDefence()) && !player.HasUsed("ZhihengCard") && !player.IsNude())
                return new List<WrappedCard> { new WrappedCard("ZhihengCard") { Skill = Name, ShowSkill = Name } };

            return null;
        }
    }

    public class ZhihengCardAI : UseCard
    {
        public ZhihengCardAI() : base("ZhihengCard")
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4.2;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<int> unpreferedCards = new List<int>();
            Room room = ai.Room;
            List<int> cards = new List<int>();
	        bool unlimited = false;
            bool own = ai.HasSkill("zhiheng");
            if (own && player.HasTreasure(LuminouSpearl.ClassName))
                unlimited = true;

            foreach (int id in player.GetCards("h"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    cards.Add(id);

            foreach (int id in player.GetEquips())
                if (RoomLogic.CanDiscard(room, player, player, id))
                    cards.Add(id);

            ai.SortByKeepValue(ref cards, false);
            int equip = -1;
            if (ai.HasSkill(TrustedAI.LoseEquipSkill))
            {
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard _c = room.GetCard(id);
                    if (Engine.GetFunctionCard(_c.Name) is EquipCard)
                        return;
                }

                foreach (int id in cards)
                {
                    if (room.GetCardPlace(id) == Player.Place.PlaceEquip)
                    {
                        equip = id;
                        break;
                    }
                }

                foreach (int id in cards)
                {
                    bool ignore = false;
                    if (equip != -1 && equip != id) ignore = true;
                    double use_v = ai.GetUseValue(id, player);
                    double keep_v = ai.GetKeepValue(id, player, room.GetCardPlace(id), ignore);
                    if ((use_v <= 5 || keep_v < 0) && (unlimited || unpreferedCards.Count < player.MaxHp))
                        unpreferedCards.Add(id);
                }
            }
            else
            {
                Dictionary<int, double> values = new Dictionary<int, double>();
                List<int> equips = new List<int>();
                foreach (int id in cards)
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
                if (weapons.Count > 1)
                {
                    weapons.Sort((x, y) => { return values[x] > values[y] ? -1 : 1; });
                    for (int i = 1; i < weapons.Count; i++)
                        if (unlimited || unpreferedCards.Count < player.MaxHp)
                            unpreferedCards.Add(weapons[i]);
                }
                if (speacial.Count > 0)
                {
                    for (int i = 0; i < ohorse.Count; i++)
                        if (unlimited || unpreferedCards.Count < player.MaxHp)
                            unpreferedCards.Add(ohorse[i]);
                    for (int i = 0; i < dhorse.Count; i++)
                        if (unlimited || unpreferedCards.Count < player.MaxHp)
                            unpreferedCards.Add(dhorse[i]);
                }
                else
                {
                    if (ohorse.Count > 1)
                    {
                        ohorse.Sort((x, y) => { return values[x] > values[y] ? -1 : 1; });
                        for (int i = 1; i < ohorse.Count; i++)
                            if (unlimited || unpreferedCards.Count < player.MaxHp)
                                unpreferedCards.Add(ohorse[i]);
                    }
                    if (dhorse.Count > 1)
                    {
                        dhorse.Sort((x, y) => { return values[x] > values[y] ? -1 : 1; });
                        for (int i = 1; i < dhorse.Count; i++)
                            if (unlimited || unpreferedCards.Count < player.MaxHp)
                                unpreferedCards.Add(dhorse[i]);
                    }
                }
                if (treasure.Count > 1)
                {
                    treasure.Sort((x, y) => { return values[x] > values[y] ? -1 : 1; });
                    for (int i = 1; i < treasure.Count; i++)
                        if (unlimited || unpreferedCards.Count < player.MaxHp)
                            unpreferedCards.Add(treasure[i]);
                }

                foreach (int id in cards)
                    if (!equips.Contains(id) && (unlimited || unpreferedCards.Count < player.MaxHp) && values[id] < 5)
                        unpreferedCards.Add(id);

                if (ai.IsWeak() && (unlimited || unpreferedCards.Count < player.MaxHp))
                {
                    if (ohorse.Count > 0)
                        unpreferedCards.Add(ohorse[0]);
                    if (weapons.Count > 0 && (unlimited || unpreferedCards.Count < player.MaxHp))
                        unpreferedCards.Add(weapons[0]);
                }
            }

            if (unpreferedCards.Count > player.MaxHp || !own)
                unpreferedCards.Remove(player.Treasure.Key);

            if (unpreferedCards.Count > 0)
            {
                card.AddSubCards(unpreferedCards);
                if (player.HasTreasure(LuminouSpearl.ClassName) && (!own || (unpreferedCards.Count <= player.MaxHp && !RoomLogic.PlayerHasShownSkill(room, player, "zhiheng"))))
                    card.Mute = true;
                else
                    card.ShowSkill = "zhiheng";
                use.Card = card;
            }
        }
    }

    public class QixiAI : SkillEvent
    {
        public QixiAI() : base("qixi")
        { }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            if (WrappedCard.IsBlack(ai.Room.GetCard(id).Suit))
            {
                WrappedCard dismantlement = new WrappedCard(Dismantlement.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                dismantlement.AddSubCard(id);
                dismantlement = RoomLogic.ParseUseCard(ai.Room, dismantlement);
                return dismantlement;
            }

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && card.Name != Dismantlement.ClassName && !card.IsVirtualCard())
                return WrappedCard.IsBlack(RoomLogic.GetCardSuit(ai.Room, card)) ? 0.5 : 0;

            return 0;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.WillShowForAttack()) return null;
            Room room = ai.Room;
            List<int> ids = player.GetCards("he");
            ids.AddRange(player.GetHandPile());
            ai.SortByKeepValue(ref ids, false);
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (WrappedCard.IsBlack(card.Suit))
                {
                    double keep = ai.GetKeepValue(id, player);
                    if (keep < 0)
                    {
                        WrappedCard dismantlement = new WrappedCard(Dismantlement.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name
                        };
                        dismantlement.AddSubCard(card);
                        dismantlement = RoomLogic.ParseUseCard(room, dismantlement);
                        return new List<WrappedCard> { dismantlement };
                    }

                    List<WrappedCard> cards = ai.GetViewAsCards(player, id);
                    double value = 0;
                    WrappedCard _card = null;
                    foreach (WrappedCard _c in cards)
                    {
                        double card_value = ai.GetUseValue(_c, player, room.GetCardPlace(id));
                        if (card_value > value)
                        {
                            value = card_value;
                            _card = _c;
                        }
                    }

                    if (_card != null && _card.Name == Dismantlement.ClassName && _card.Skill == Name) return new List<WrappedCard> { _card };
                }
            }

            return null;
        }
    }

    public class KejiAI : SkillEvent
    {
        public KejiAI() : base("keji")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class MouduanAI : SkillEvent
    {
        public MouduanAI() : base("mouduan")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            List<int> result = new List<int>();
            if (flags == "ej")
            {
                int id = QiaobianAI.CardForQiaobian(ai, to).Key;
                if (id >= 0)
                    result.Add(id);
            }

            return null;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            List<Player> result = new List<Player>();
            if (ai.Room.GetTag("MouduanTarget") != null && ai.Room.GetTag("MouduanTarget") is Player from)
            {
                Player to = QiaobianAI.CardForQiaobian(ai, from).Value;
                if (to != null)
                    result.Add(to);
                else
                    ai.Room.Debug("谋断AI出错");
            }
            else
            {
                foreach (Player friend in ai.GetFriends(player))
                {
                    if (friend.JudgingArea.Count > 0 && QiaobianAI.CardForQiaobian(ai, friend).Key >= 0)
                    {
                        return new List<Player> { friend };
                    }
                }
                foreach (Player friend in ai.FriendNoSelf)
                {
                    if (friend.HasEquip() && ai.HasSkill(TrustedAI.LoseEquipSkill, friend) && QiaobianAI.CardForQiaobian(ai, friend).Key >= 0)
                    {
                        return new List<Player> { friend };
                    }
                }

                List<Player> enemies = ai.GetEnemies(player);
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                {
                    if (QiaobianAI.CardForQiaobian(ai, p).Key >= 0)
                    {
                        return new List<Player> { p };
                    }
                }
            }

            return result;
        }
    }

    public class KurouAI : SkillEvent
    {
        public KurouAI() : base("kurou")
        { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("KurouCard"))
            {
                Room room = ai.Room;
                bool use = false;
                if (player.GetLostHp() == 0 || (player.IsWounded() && player.HasArmor(SilverLion.ClassName))
                    || (player.Hp == 1 && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) > 0 && player.GetCards("he").Count > 1))
                    use = true;

                if (use)
                {
                    int id = KurouJXAI.FindKurouCard(ai, player);
                    if (id >= 0)
                    {
                        WrappedCard kurou = new WrappedCard("KurouCard") { Skill = Name, ShowSkill = Name };
                        kurou.AddSubCard(id);
                        return new List<WrappedCard> { kurou };
                    }
                }
            }

            return null;
        }
    }

    public class KurouCardAI : UseCard
    {
        public KurouCardAI() : base("KurouCard")
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

    public class YingziZAI : SkillEvent
    {
        public YingziZAI() : base("yingzi_zhouyu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.WillShowForAttack() || ai.WillShowForDefence())
            {
                Room room = ai.Room;
                if (ai.HasSkill("haoshi"))
                {
                    int count = player.HandcardNum + 4;
                    if (player.HasTreasure(JadeSeal.ClassName))
                        count++;

                    if (count <= 5 && count + 1 > 5)
                    {
                        int least = 1000;
                        foreach (Player p in room.GetOtherPlayers(player))
                            least = Math.Min(player.HandcardNum, least);

                        bool check = false;
                        foreach (Player p in ai.FriendNoSelf)
                        {
                            if (p.HandcardNum == least)
                            {
                                check = true;
                                break;
                            }
                        }
                        return check;
                    }
                }

                return true;
            }

            return false;
        }
    }

    public class FanjianAI : SkillEvent
    {
        public FanjianAI() : base("fanjian")
        {
        }
        
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("FanjianCard") && !player.IsKongcheng())
                return new List<WrappedCard> { new WrappedCard("FanjianCard") { Skill = Name, ShowSkill = Name } };

            return null;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (ai.HasSkill("zhaxiang")) return "losehp";

            if (player.GetTag(Name) is WrappedCard.CardSuit suit)
            {
                List<int> all = player.GetCards("he");
                double value = 0;
                int count = 0;

                if (player.Hp == 1 && ai.GetKnownCardsNums(Peach.ClassName, "he", player) == 0 && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) == 0
                    && !ai.HasSkill("buqu|buqu_jx"))
                    return "show";

                foreach (int id in all)
                {
                    if (room.GetCard(id).Suit == suit)
                    {
                        count++;
                        value += ai.GetKeepValue(id, player);
                    }
                }

                if (count == 0 || value > (ai.IsWeak(player) ? 6 : 4))
                    return "losehp";
            }

            return "show";
        }
    }

    public class FanjianCardAI : UseCard
    {
        public FanjianCardAI() : base("FanjianCard")
        {}

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai.Self != player && ai is SmartAI)
                {
                    if (!player.HasShownOneGeneral())
                    {
                        string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 100);
                    }
                    foreach (Player p in use.To)
                        ai.UpdatePlayerRelation(player, p, false);
                }
                else if (ai is StupidAI)
                {
                    int id = use.Card.GetEffectiveId();
                    foreach (Player p in use.To)
                    {
                        if (ai.IsCard(id, Analeptic.ClassName, p) || ai.IsCard(id, Peach.ClassName, p) || ai.HasSkill("zhaxiang", p))
                            ai.UpdatePlayerRelation(player, p, true);
                        else
                            ai.UpdatePlayerRelation(player, p, false);
                    }
                }
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> enemies = ai.GetPrioEnemies();
            ai.SortByDefense(ref enemies, false);
            List<int> cards = new List<int>();

            Player huanggai = ai.FindPlayerBySkill("zhaxiang");
            if (huanggai != null && ai.IsFriend(huanggai))
            {
                int help = -1;
                cards= player.GetCards("h");
                ai.SortByUseValue(ref cards, false);
                foreach (int id in cards)
                {
                    if (ai.IsCard(id, Peach.ClassName, huanggai) || ai.IsCard(id, Analeptic.ClassName, huanggai))
                    {
                        help = id;
                        break;
                    }
                }
                if (huanggai.Hp == 1 && help >= 0)
                {
                    card.AddSubCard(help);
                    use.Card = card;
                    use.To.Add(huanggai);
                    return;
                }
                else if (huanggai.Hp > 1 || ai.GetKnownCardsNums(Analeptic.ClassName, "he", huanggai) > 0 || ai.GetKnownCardsNums(Peach.ClassName, "he", huanggai) > 0)
                {
                    card.AddSubCard(cards[0]);
                    use.Card = card;
                    use.To.Add(huanggai);
                    return;
                }
            }

            foreach (int id in player.GetCards("h"))
            {
                if ((ai.GetKeepValue(id, player) < 5 || (ai.GetOverflow(player) > 0 && ai.GetUseValue(id, player) < 6))
                            && !ai.IsCard(id, Peach.ClassName, player) && !ai.IsCard(id, Analeptic.ClassName, player))
                    cards.Add(id);
            }

            if (cards.Count == 0) return;

            ai.SortByUseValue(ref cards, false);
            foreach (Player p in enemies)
            {
                if (ai.HasSkill("zhaxiang", p)) continue;
                //针对空城猪哥
                if (p.IsKongcheng() && RoomLogic.PlayerHasShownSkill(room, p, "kongcheng"))
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p))
                        {
                            if (p.GetArmor() && room.GetCard(p.Armor.Key).Suit == room.GetCard(id).Suit)
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                            if (p.GetSpecialEquip() && room.GetCard(p.Special.Key).Suit == room.GetCard(id).Suit)
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                            if (p.GetDefensiveHorse() && room.GetCard(p.DefensiveHorse.Key).Suit == room.GetCard(id).Suit)
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                            if (p.Hp > 1 && p.GetWeapon() && room.GetCard(p.Weapon.Key).Suit == room.GetCard(id).Suit)
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }
                //针对小乔
                if (RoomLogic.PlayerHasShownSkill(room, p, "hongyan"))
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p) && room.GetCard(id).Suit == WrappedCard.CardSuit.Spade)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }

            //下防具
            foreach (Player p in enemies)
            {
                if (p.GetArmor() && ai.GetKeepValue(p.Armor.Key, p, Player.Place.PlaceEquip) > 3)
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p) && room.GetCard(p.Armor.Key).Suit == room.GetCard(id).Suit)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }
            //下宝物
            foreach (Player p in enemies)
            {
                if (p.GetTreasure() && ai.GetKeepValue(p.Treasure.Key, p, Player.Place.PlaceEquip) > 3)
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p) && room.GetCard(p.Treasure.Key).Suit == room.GetCard(id).Suit)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }
            //下马车
            foreach (Player p in enemies)
            {
                if (p.GetSpecialEquip() && ai.GetKeepValue(p.Special.Key, p, Player.Place.PlaceEquip) > 3)
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p) && room.GetCard(p.Special.Key).Suit == room.GetCard(id).Suit)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }
            //下+1马
            foreach (Player p in enemies)
            {
                if (p.GetDefensiveHorse() && ai.GetKeepValue(p.DefensiveHorse.Key, p, Player.Place.PlaceEquip) > 3)
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p) && room.GetCard(p.DefensiveHorse.Key).Suit == room.GetCard(id).Suit)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }
            //给方片去闪
            foreach (Player p in enemies)
            {
                if (RoomLogic.CanSlash(room, player, p) && p.HandcardNum > 2)
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p) && !ai.IsCard(id, Jink.ClassName, p)
                            && room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }
            //下武器
            foreach (Player p in enemies)
            {
                if (p.GetWeapon() && ai.GetKeepValue(p.Weapon.Key, p, Player.Place.PlaceEquip) > 3)
                {
                    foreach (int id in cards)
                    {
                        if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p) && room.GetCard(p.Weapon.Key).Suit == room.GetCard(id).Suit)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }
            //手牌溢出就找一个手牌最多的敌方
            if (ai.GetOverflow(player) > 0)
            {
                ai.SortByHandcards(ref enemies);
                foreach (Player p in enemies)
                {
                    if (p.HandcardNum > 3)
                    {
                        foreach (int id in cards)
                        {
                            if (!ai.IsCard(id, Peach.ClassName, p) && !ai.IsCard(id, Analeptic.ClassName, p))
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    public class GuoseAI : SkillEvent
    {
        public GuoseAI() : base("guose")
        { }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            if (ai.Room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
            {
                WrappedCard indulgence = new WrappedCard(Indulgence.ClassName);
                indulgence.AddSubCard(id);
                indulgence.Skill = Name;
                indulgence.ShowSkill = Name;
                indulgence = RoomLogic.ParseUseCard(ai.Room, indulgence);
                return indulgence;
            }

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card.Id >= 0 && ai.HasSkill(Name, player) && card.Name != Indulgence.ClassName)
                return RoomLogic.GetCardSuit(ai.Room, card) == WrappedCard.CardSuit.Diamond ? 0.6 : 0;

            return 0;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.WillShowForAttack()) return null;
            Room room = ai.Room;
            List<int> ids = player.GetCards("he");
            ids.AddRange(player.GetHandPile());

            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids, false);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Suit == WrappedCard.CardSuit.Diamond)
                    {
                        double keep = ai.GetKeepValue(id, player);
                        if (keep < 0)
                        {
                            WrappedCard indulgence = new WrappedCard(Indulgence.ClassName);
                            indulgence.AddSubCard(id);
                            indulgence.Skill = Name;
                            indulgence.ShowSkill = Name;
                            indulgence = RoomLogic.ParseUseCard(ai.Room, indulgence);
                            return new List<WrappedCard> { indulgence };
                        }

                        List<WrappedCard> cards = ai.GetViewAsCards(player, id);
                        double value = 0;
                        WrappedCard _card = null;
                        foreach (WrappedCard _c in cards)
                        {
                            double card_value = ai.GetUseValue(_c, player, room.GetCardPlace(id));
                            if (card_value > value)
                            {
                                value = card_value;
                                _card = _c;
                            }
                        }

                        if (_card != null && _card.Name == Indulgence.ClassName && _card.Skill == Name) return new List<WrappedCard> { _card };
                    }
                }
            }

            return null;
        }
    }

    public class LiuliAI : SkillEvent
    {
        public LiuliAI() : base("liuli")
        { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>(), true);
            WrappedCard liuli = new WrappedCard("LiuliCard") { Skill = Name, Mute = true };
            int result = LijianAI.FindLijianCard(ai, player);
            Room room = ai.Room;
            CardUseStruct _use = (CardUseStruct)room.GetTag(Name);
            Player from = _use.From;
            WrappedCard slash = _use.Card;

            player.SetFlags("target_confirming");

            if (result >= 0)
            {
                liuli.AddSubCard(result);
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p == from || !RoomLogic.InMyAttackRange(room, player, p, liuli) || _use.To.Contains(p)) continue;
                    if (ai.IsEnemy(p) && !ai.IsCancelTarget(slash, p, from) && ai.IsCardEffect(slash, p, from) && !ai.NotSlashJiaozhu(from, p, slash))
                    {
                        DamageStruct damage = new DamageStruct(slash, from, p, _use.Drank + 1 + _use.ExDamage);
                        if (slash.Name.Contains("Fire"))
                            damage.Nature = DamageStruct.DamageNature.Fire;
                        else if (slash.Name.Contains("Thunder"))
                            damage.Nature = DamageStruct.DamageNature.Thunder;

                        if (ai.GetDamageScore(damage).Score > 0)
                        {
                            player.SetFlags("-target_confirming");
                            use.Card = liuli;
                            use.To.Add(p);
                            return use;
                        }
                    }
                }

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p == from || !RoomLogic.InMyAttackRange(room, player, p, liuli) || _use.To.Contains(p)) continue;
                    if (!ai.IsFriend(p) && !ai.IsCancelTarget(slash, p, from) && ai.IsCardEffect(slash, p, from) && !ai.NotSlashJiaozhu(from, p, slash))
                    {
                        DamageStruct damage = new DamageStruct(slash, from, p, _use.Drank + 1 + _use.ExDamage);
                        if (slash.Name.Contains("Fire"))
                            damage.Nature = DamageStruct.DamageNature.Fire;
                        else if (slash.Name.Contains("Thunder"))
                            damage.Nature = DamageStruct.DamageNature.Thunder;

                        if (ai.GetDamageScore(damage).Score > 0)
                        {
                            player.SetFlags("-target_confirming");
                            use.Card = liuli;
                            use.To.Add(p);
                            return use;
                        }
                    }
                }

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p == from || !RoomLogic.InMyAttackRange(room, player, p, liuli)) continue;
                    if (ai.IsCancelTarget(slash, p, from) || !ai.IsCardEffect(slash, p, from))
                    {
                        player.SetFlags("-target_confirming");
                        use.Card = liuli;
                        use.To.Add(p);
                        return use;
                    }
                    else if (!ai.NotSlashJiaozhu(from, p, slash))
                    {
                        DamageStruct damage = new DamageStruct(slash, from, p, _use.Drank + 1 + _use.ExDamage);
                        if (slash.Name.Contains("Fire"))
                            damage.Nature = DamageStruct.DamageNature.Fire;
                        else if (slash.Name.Contains("Thunder"))
                            damage.Nature = DamageStruct.DamageNature.Thunder;

                        if (ai.GetDamageScore(damage).Score > 0)
                        {
                            player.SetFlags("-target_confirming");
                            use.Card = liuli;
                            use.To.Add(p);
                            return use;
                        }
                    }
                }
            }
            else
            {
                if (ai.IsCardEffect(slash, player, from))
                {
                    DamageStruct _damage = new DamageStruct(slash, from, player, _use.Drank + 1 + _use.ExDamage);
                    if (ai.GetDamageScore(_damage).Score < -4)
                    {
                        List<int> ids = new List<int>();
                        foreach (int id in player.GetCards("he"))
                            if (RoomLogic.CanDiscard(room, player, player, id)) ids.Add(id);

                        if (ids.Count > 0)
                        {
                            List<double> values = ai.SortByKeepValue(ref ids, false);
                            foreach (int id in ids)
                            {
                                liuli.AddSubCard(id);
                                foreach (Player p in room.GetOtherPlayers(player))
                                {
                                    if (p == from || !RoomLogic.InMyAttackRange(room, player, p, liuli) || _use.To.Contains(p)) continue;
                                    if (ai.IsEnemy(p) && !ai.IsCancelTarget(slash, p, from) && ai.IsCardEffect(slash, p, from) && !ai.NotSlashJiaozhu(from, p, slash))
                                    {
                                        DamageStruct damage = new DamageStruct(slash, from, p, _use.Drank + 1 + _use.ExDamage);
                                        if (slash.Name.Contains("Fire"))
                                            damage.Nature = DamageStruct.DamageNature.Fire;
                                        else if (slash.Name.Contains("Thunder"))
                                            damage.Nature = DamageStruct.DamageNature.Thunder;

                                        if (ai.GetDamageScore(damage).Score > 0)
                                        {
                                            player.SetFlags("-target_confirming");
                                            use.Card = liuli;
                                            use.To.Add(p);
                                            return use;
                                        }
                                    }
                                }

                                foreach (Player p in room.GetOtherPlayers(player))
                                {
                                    if (p == from || !RoomLogic.InMyAttackRange(room, player, p, liuli) || _use.To.Contains(p)) continue;
                                    if (!ai.IsFriend(p) && !ai.IsCancelTarget(slash, p, from) && ai.IsCardEffect(slash, p, from) && !ai.NotSlashJiaozhu(from, p, slash))
                                    {
                                        DamageStruct damage = new DamageStruct(slash, from, p, _use.Drank + 1 + _use.ExDamage);
                                        if (slash.Name.Contains("Fire"))
                                            damage.Nature = DamageStruct.DamageNature.Fire;
                                        else if (slash.Name.Contains("Thunder"))
                                            damage.Nature = DamageStruct.DamageNature.Thunder;

                                        if (ai.GetDamageScore(damage).Score > 0)
                                        {
                                            player.SetFlags("-target_confirming");
                                            use.Card = liuli;
                                            use.To.Add(p);
                                            return use;
                                        }
                                    }
                                }

                                foreach (Player p in room.GetOtherPlayers(player))
                                {
                                    if (p == from || !RoomLogic.InMyAttackRange(room, player, p, liuli)) continue;
                                    if (ai.IsCancelTarget(slash, p, from) || !ai.IsCardEffect(slash, p, from))
                                    {
                                        player.SetFlags("-target_confirming");
                                        use.Card = liuli;
                                        use.To.Add(p);
                                        return use;
                                    }
                                    else if (!ai.NotSlashJiaozhu(from, p, slash))
                                    {
                                        DamageStruct damage = new DamageStruct(slash, from, p, _use.Drank + 1 + _use.ExDamage);
                                        if (slash.Name.Contains("Fire"))
                                            damage.Nature = DamageStruct.DamageNature.Fire;
                                        else if (slash.Name.Contains("Thunder"))
                                            damage.Nature = DamageStruct.DamageNature.Thunder;

                                        if (ai.GetDamageScore(damage).Score > 0)
                                        {
                                            player.SetFlags("-target_confirming");
                                            use.Card = liuli;
                                            use.To.Add(p);
                                            return use;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (use.To.Count == 0) use.Card = null;
            player.SetFlags("-target_confirming");
            return use;
        }
    }

    public class QianxunAI : SkillEvent
    {
        public QianxunAI() : base("qianxun")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardUseStruct use && ai.IsFriend(use.From) && ai.IsKnown(use.From, player))
                return false;

            return true;
        }

        public override bool IsCancelTarget(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            if (to != null && ai.HasSkill(Name, to) && card.Name == Snatch.ClassName && !RoomLogic.PlayerHasShownSkill(ai.Room, to, Name) && ai.IsFriend(from, to))
                return false;

            if (to != null && ai.HasSkill(Name, to) && (card.Name == Snatch.ClassName || card.Name == Indulgence.ClassName))
                return true;

            return false;
        }
    }
    public class DuoshiAI : SkillEvent
    {
        public DuoshiAI() : base("duoshi")
        {
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            if (WrappedCard.IsRed(ai.Room.GetCard(id).Suit) && (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand))
            {
                WrappedCard await = new WrappedCard(AwaitExhausted.ClassName);
                await.AddSubCard(id);
                await.Skill = Name;
                await.ShowSkill = Name;
                await = RoomLogic.ParseUseCard(ai.Room, await);
                return await;
            }

            return null;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.UsedTimes("ViewAsSkill_duoshiCard") < 4 && (ai.WillShowForAttack() || ai.WillShowForDefence()))
            {
                Room room = ai.Room;
                List<int> ids= player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                ai.SortByKeepValue(ref ids, false);

                bool need_lose_equip = false;
                foreach (int id in player.GetEquips())
                {
                    if (ai.GetKeepValue(id, player) < 0)
                    {
                        need_lose_equip = true;
                        break;
                    }
                }
                if (ai.IsWeak() && (player.GetWeapon() || player.GetOffensiveHorse()))
                    need_lose_equip = true;

                foreach (int id in ids)
                {
                    if (WrappedCard.IsRed(ai.Room.GetCard(id).Suit) && room.GetCard(id).Name != AwaitExhausted.ClassName
                        && (ai.GetKeepValue(id, player) < 3 || ai.GetOverflow(player) > 0 || need_lose_equip))
                    {
                        WrappedCard await = new WrappedCard(AwaitExhausted.ClassName);
                        await.AddSubCard(id);
                        await.Skill = Name;
                        await.ShowSkill = Name;
                        await = RoomLogic.ParseUseCard(ai.Room, await);
                        return new List<WrappedCard> { await };
                    }
                }
            }

            return null;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card.Skill == Name)
            {
                bool friend = false;
                foreach (Player p in ai.Room.GetAlivePlayers())
                {
                    if (RoomLogic.IsFriendWith(ai.Room, p, player))
                    {
                        friend = true;
                        break;
                    }
                }

                if (!friend) return -3.5;
            }

            return 0;
        }
    }

    public class BuquAI : SkillEvent
    {
        public BuquAI() : base("buqu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class FenjiAI : SkillEvent
    {
        public FenjiAI() : base("fenji")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                return ai.IsFriend(target);
            }

            return false;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name && choices[2] == "yes")
                {
                    Player target = ai.Room.Current;
                    if (!player.HasShownOneGeneral())
                    {
                        string role = (Scenario.Hegemony.WillbeRole(ai.Room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 200);
                    }
                    if (ai.GetPlayerTendency(target) == "unknown" && ai.IsKnown(player, target))
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
    }

    public class JieyinAI : SkillEvent
    {
        public JieyinAI() : base("jieyin")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            ai.Target[Name] = null;
            if (ai.WillShowForDefence() && player.HandcardNum >= 2 && !player.HasUsed("JieyinCard"))
            {
                Room room = ai.Room;
                Player target = null;
                foreach (Player p in ai.FriendNoSelf)
                {
                    if (p.HasShownOneGeneral() && p.IsMale() && p.IsWounded() && ai.IsWeak())
                    {
                        target = p;
                        break;
                    }
                }
                if (target == null)
                {
                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (p.HasShownOneGeneral() && p.IsMale() && p.IsWounded())
                        {
                            target = p;
                            break;
                        }
                    }
                }

                if (target != null)
                {
                    double value = 4;
                    if (ai.IsWeak(target))
                        value += 2;
                    if (player.IsWounded())
                    {
                        value += 4;
                        if (ai.HasSkill(TrustedAI.MasochismSkill))
                            value += 1.5;
                        if (player.Hp < 2)
                            value += 2;
                    }
                    if (target.Hp == 1 && ai.HasSkill("hunshang", target))
                        value -= 1.5;
                    if (ai.HasSkill(TrustedAI.MasochismSkill, target))
                        value += 1.5;

                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("h"))
                        if (RoomLogic.CanDiscard(room, player, player, id))
                            ids.Add(id);

                    if (ids.Count > 1)
                    {
                        double cost = 0;
                        List<int> subs = new List<int>();
                        ai.SortByUseValue(ref ids, false);
                        for (int i = 0; i < 2; i++)
                        {
                            double use_value = ai.GetUseValue(ids[i], player);
                            if (ai.GetOverflow(player) > i && use_value > 0)
                                use_value /= 4;
                            cost += use_value;
                            subs.Add(ids[i]);
                        }

                        if (cost < value)
                        {
                            ai.Target[Name] = target;
                            WrappedCard jieyin_card = new WrappedCard("JieyinCard")
                            {
                                Skill = Name,
                                ShowSkill = Name
                            };
                            jieyin_card.AddSubCards(subs);
                            return new List<WrappedCard> { jieyin_card };
                        }
                    }
                }
            }

            return null;
        }
    }
    public class JieyinCardAI : UseCard
    {
        public JieyinCardAI() : base("JieyinCard")
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            double cost = 0;
            foreach (int id in card.SubCards)
            {
                double use_value = ai.GetUseValue(id, player);
                if (ai.GetOverflow(player) > card.SubCards.IndexOf(id) && use_value > 0)
                    use_value /= 4;
                cost += use_value;
            }

            return cost > 8 ? 1 : 2.8;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
            use.To.Add(ai.Target["jieyin"]);
        }
    }

    public class XiaojiAI : SkillEvent
    {
        public XiaojiAI() : base("xiaoji")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (!isUse && place == Player.Place.PlaceEquip)
            {
                return -4;
            }

            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard is EquipCard)
                return 3;
            else if (card.IsVirtualCard())
            {
                foreach (int id in card.SubCards)
                {
                    if (ai.Room.GetCardPlace(id) == Player.Place.PlaceEquip)
                        return 3;
                }
            }

            return 0;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class YinghunJAI : SkillEvent
    {
        public YinghunJAI() : base("yinghun_sunjian")
        { }

        public static Player Choose(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() || ai.WillShowForDefence())
            {
                List<Player> friend = new List<Player>(ai.FriendNoSelf);
                if (friend.Count > 0)
                {
                    ai.SortByDefense(ref friend, false);
                    foreach (Player p in friend)
                        foreach (int id in p.GetEquips())
                            if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                return p;

                    return friend[0];
                }

                if (player.GetLostHp() > 1)
                {
                    List<Player> enemies = new List<Player>(ai.GetPrioEnemies());
                    if (enemies.Count > 0)
                    {
                        ai.SortByDefense(ref enemies, false);
                        foreach (Player p in enemies)
                        {
                            bool invoke = true;
                            foreach (int id in p.GetEquips())
                            {
                                if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                {
                                    invoke = false;
                                    break;
                                }
                            }
                            if (invoke)
                                return p;
                        }
                    }
                    else if (ai.IsSituationClear())
                    {
                        enemies = ai.GetEnemies(player);
                        ai.SortByDefense(ref enemies);
                        foreach (Player p in enemies)
                        {
                            bool invoke = true;
                            foreach (int id in p.GetEquips())
                            {
                                if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                {
                                    invoke = false;
                                    break;
                                }
                            }
                            if (invoke)
                                return p;
                        }
                    }
                }
            }

            return null;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Player tar = Choose(ai, player);
            if (tar != null)
                return new List<Player> { tar };

            return new List<Player>();
        }
        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return Choice(ai, player);
        }
        public static string Choice(TrustedAI ai, Player player)
        {
            Player target = null;
            foreach (Player p in ai.Room.GetAlivePlayers())
            {
                if (p.HasFlag("YinghunTarget"))
                {
                    target = p;
                    break;
                }
            }

            return ai.IsFriend(target) ? "dxt1" : "d1tx";
        }
    }

    public class TianxiangAI : SkillEvent
    {
        public TianxiangAI() : base("tianxiang")
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

                        foreach (Player p in ai.GetPrioEnemies())
                        {
                            DamageStruct _damage = new DamageStruct(damage.Card, damage.From, p, damage.Damage, damage.Nature)
                            {
                                Transfer = true,
                                TransferReason = Name
                            };

                            if (ai.GetDamageScore(_damage).Score > 4 && !ai.CanResist(p, _damage.Damage))
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        foreach (Player p in ai.FriendNoSelf)
                        {
                            DamageStruct _damage = new DamageStruct(damage.Card, damage.From, p, damage.Damage, damage.Nature)
                            {
                                Transfer = true,
                                TransferReason = Name
                            };

                            if (ai.GetDamageScore(_damage).Score > 4)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        foreach (Player p in ai.FriendNoSelf)
                        {
                            if (ai.CanResist(p, damage.Damage) && p.GetLostHp() >= 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        foreach (Player p in ai.FriendNoSelf)
                        {
                            if (ai.HasSkill("buqu", p) && p.GetPile("buqu").Count < 3 && p.Hp <= 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                            DamageStruct _damage = new DamageStruct(damage.Card, damage.From, p, damage.Damage, damage.Nature)
                            {
                                Transfer = true,
                                TransferReason = Name
                            };
                            int count = ai.DamageEffect(_damage, DamageStruct.DamageStep.Caused);
                            if (ai.HasSkill("hunshang", p) && !ai.IsWeak(p) && p.Hp == 2 && count == 1)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }
                    }
                }
            }

            return use;
        }
    }

    public class TianyiAI : SkillEvent
    {
        public TianyiAI() : base("tianyi")
        { }
        

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.IsKongcheng() && !player.HasUsed("TianyiCard"))
            {
                WrappedCard card = new WrappedCard("TianyiCard")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                return new List<WrappedCard> { card };
            }

            return null;
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && !isUse)
            {
                if (RoomLogic.GetCardNumber(ai.Room, card) > 11)
                    return 1;
                if (card.Name.Contains(Slash.ClassName))
                    return 0.3;
            }

            return 0;
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> player)
        {
            Room room = ai.Room;
            if (requestor == ai.Self)
            {
                List<int> ids = ai.Self.GetCards("h");
                if (ai.GetKnownCards(player[0]).Count == player[0].HandcardNum)
                {
                    ai.SortByUseValue(ref ids, false);
                    WrappedCard pindian = null;
                    int app = ai.HasSkill("yingyang") ? 3 : 0;
                    if (ai.IsFriend(player[0]))
                    {
                        pindian = ai.GetMinCard(ai.Self, ai.Self.GetCards("h"), new List<string> { Peach.ClassName, Analeptic.ClassName });
                        int min_number = 0;
                        if (pindian != null)
                        {
                            min_number = pindian.Number;
                            if (ai.HasSkill("yingyang", player[0]))
                                min_number = Math.Max(1, min_number - 3);

                            foreach (int id in ids)
                            {
                                if (!ai.IsCard(id, Peach.ClassName, requestor) && !ai.IsCard(id, Slash.ClassName, requestor) && !ai.IsCard(id, Analeptic.ClassName, requestor)
                                    && Math.Min(13, room.GetCard(id).Number + app) > min_number)
                                    return room.GetCard(id);
                            }

                            foreach (int id in ids)
                            {
                                if (Math.Min(13, room.GetCard(id).Number + app) > min_number)
                                    return room.GetCard(id);
                            }
                        }

                        pindian = ai.GetMinCard(ai.Self, ai.Self.GetCards("h"));
                        if (pindian != null)
                        {
                            foreach (int id in ids)
                            {
                                if (!ai.IsCard(id, Peach.ClassName, requestor) && !ai.IsCard(id, Slash.ClassName, requestor) && !ai.IsCard(id, Analeptic.ClassName, requestor)
                                    && Math.Min(13, room.GetCard(id).Number + app) > min_number)
                                    return room.GetCard(id);
                            }

                            foreach (int id in ids)
                            {
                                if (Math.Min(13, room.GetCard(id).Number + app) > min_number)
                                    return room.GetCard(id);
                            }
                        }
                    }
                    else
                    {
                        int max_enemy = 0;
                        pindian = ai.GetMaxCard(player[0]);
                        if (pindian != null)
                        {
                            max_enemy = pindian.Number;
                            if (ai.HasSkill("yingyang"))
                                max_enemy = Math.Min(13, max_enemy + 3);

                            foreach (int id in ids)
                            {
                                if (!ai.IsCard(id, Peach.ClassName, requestor) && !ai.IsCard(id, Slash.ClassName, requestor) && !ai.IsCard(id, Analeptic.ClassName, requestor)
                                    && Math.Min(13, room.GetCard(id).Number + app) > max_enemy)
                                    return room.GetCard(id);
                            }
                        }
                    }
                }
                else
                {
                    WrappedCard max = ai.GetMaxCard(ai.Self, ai.Self.GetCards("h"), new List<string> { Peach.ClassName, Analeptic.ClassName, Slash.ClassName });
                    int max_number = 0;
                    if (max != null)
                    {
                        max_number = max.Number;
                        if (ai.HasSkill("yingyang"))
                            max_number = Math.Min(13, max_number + 3);

                        if (ai.IsEnemy(player[0]) && max_number > 10 || ai.IsFriend(player[0]))
                            return max;
                    }
                }

                ai.SortByKeepValue(ref ids, false);
                return ai.Room.GetCard(ids[0]);
            }
            else
            {
                if (ai.IsEnemy(requestor))
                {
                    WrappedCard max = ai.GetMaxCard(ai.Self, ai.Self.GetCards("h"), new List<string> { Peach.ClassName, Analeptic.ClassName });
                    int max_number = 0;
                    if (max != null)
                    {
                        max_number = max.Number;
                        if (ai.HasSkill("yingyang"))
                            max_number = Math.Min(13, max_number + 3);
                        if (max_number > 10)
                            return max;
                    }

                    max = ai.GetMaxCard();
                    if (max != null)
                    {
                        max_number = max.Number;
                        if (ai.HasSkill("yingyang"))
                            max_number = Math.Min(13, max_number + 3);
                        if (max_number > 10)
                            return max;
                    }

                    List<int> ids = ai.Self.GetCards("h");
                    ai.SortByKeepValue(ref ids, false);
                    return ai.Room.GetCard(ids[0]);
                }
                else
                {
                    WrappedCard min = ai.GetMinCard(ai.Self, ai.Self.GetCards("h"), new List<string> { Peach.ClassName, Analeptic.ClassName });
                    int min_number = 0;
                    if (min != null)
                    {
                        min_number = min.Number;
                        if (ai.HasSkill("yingyang"))
                            min_number = Math.Max(1, min_number - 3);
                        if (min_number <= 7)
                            return min;
                    }

                    min = ai.GetMinCard(ai.Self, ai.Self.GetCards("h"));
                    if (min != null)
                        return min;
                }
            }

            return null;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag("extra_target_skill") is CardUseStruct use)
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in target)
                {
                    ScoreStruct score = ai.SlashIsEffective(use.Card, p);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 0)
                    return scores[0].Players;
            }

            return new List<Player>();
        }
    }

    public class TianyiCardAI : UseCard
    {
        public TianyiCardAI() : base("TianyiCard")
        { }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            WrappedCard max = ai.GetMaxCard(player, player.GetCards("h"), new List<string> { Peach.ClassName, Analeptic.ClassName, Slash.ClassName });
            int max_number = 0;
            if (max != null)
            {
                max_number = max.Number;
                if (ai.HasSkill("yingyang"))
                    max_number = Math.Min(13, max_number + 3);

                if (max_number >= 12) return 3.5;
                if (max_number >= 8 && ai.FriendNoSelf.Count > 0)
                {
                    List<Player> friends = new List<Player>(ai.FriendNoSelf);
                    ai.SortByHandcards(ref friends);
                    foreach (Player p in friends)
                    {
                        if (ai.IsWeak(p) || p.HandcardNum <= p.MaxHp) continue;
                        return 3.5;
                    }
                }
            }

            return 2;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetPrioEnemies();
            ai.SortByDefense(ref enemies, false);
            Room room = ai.Room;
            WrappedCard max = ai.GetMaxCard(player, player.GetCards("h"), new List<string> { Peach.ClassName, Analeptic.ClassName, Slash.ClassName });
            int max_number = 0;
            if (max != null)
            {
                max_number = max.Number;
                if (ai.HasSkill("yingyang"))
                    max_number = Math.Min(13, max_number + 3);

                List<Player> known = new List<Player>();
                foreach (Player p in enemies)
                {
                    if (!RoomLogic.CanBePindianBy(room, p, player) || ai.GetKnownCards(p).Count != p.HandcardNum) continue;
                    known.Add(p);
                    WrappedCard max_enemy = ai.GetMaxCard(p);
                    if (max_enemy != null)
                    {
                        int enemy = max_enemy.Number;
                        if (ai.HasSkill("yingyang", p))
                            enemy = Math.Min(13, enemy + 3);

                        if (enemy >= max_number)
                            continue;
                    }

                    use.Card = card;
                    use.To.Add(p);
                    return;
                }

                if (max_number >= 12)
                {
                    foreach (Player p in enemies)
                    {
                        if (!RoomLogic.CanBePindianBy(room, p, player) || known.Contains(p)) continue;

                        WrappedCard max_enemy = ai.GetMaxCard(p);
                        if (max_enemy != null)
                        {
                            int enemy = max_enemy.Number;
                            if (ai.HasSkill("yingyang", p))
                                enemy = Math.Min(13, enemy + 3);

                            if (enemy >= max_number)
                                continue;
                        }

                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
                else if (max_number >= 8)
                {
                    List<Player> friends = new List<Player>(ai.FriendNoSelf);
                    ai.SortByHandcards(ref friends);
                    foreach (Player p in friends)
                    {
                        if (!RoomLogic.CanBePindianBy(room, p, player) || ai.IsWeak(p) || p.HandcardNum <= p.MaxHp) continue;

                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                if (max_number > 10)
                {
                    foreach (Player p in enemies)
                    {
                        if (!RoomLogic.CanBePindianBy(room, p, player) || known.Contains(p)) continue;

                        WrappedCard max_enemy = ai.GetMaxCard(p);
                        if (max_enemy != null)
                        {
                            int enemy = max_enemy.Number;
                            if (ai.HasSkill("yingyang", p))
                                enemy = Math.Min(13, enemy + 3);

                            if (enemy >= max_number)
                                continue;
                        }

                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
                else if (ai.GetOverflow(player) > 0)
                {
                    foreach (Player p in enemies)
                    {
                        if (!RoomLogic.CanBePindianBy(room, p, player)) continue;
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }
        }
    }

    public class HaoshiAI : SkillEvent
    {
        public HaoshiAI() : base("haoshi")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack() && !ai.WillShowForDefence()) return false;

            Room room = ai.Room;
            int draw = (int)data;
            int count = player.HandcardNum + draw + 2;
            if (player.HasTreasure(JadeSeal.ClassName))
                count++;
            if (player.GetMark("@tangerine") > 0) count++;
            if (player.GetMark("duliang") > 0) count++;
            if (player.GetMark("@rob") > 0) count++;

            if (count > 5)
            {
                int least = 1000;
                foreach (Player p in room.GetOtherPlayers(player))
                    least = Math.Min(p.HandcardNum, least);

                foreach (Player p in ai.FriendNoSelf)
                {
                    if (p.HandcardNum == least)
                        return true;
                }
                return false;
            }
            else
                return true;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };

            Room room = ai.Room;
            int least = 1000;
            foreach (Player p in room.GetOtherPlayers(player))
                least = Math.Min(p.HandcardNum, least);
            
            List<Player> targets = new List<Player>();
            foreach (Player p in ai.FriendNoSelf)
                if (p.HandcardNum == least)
                    targets.Add(p);

            if (targets.Count > 0)
            {
                ai.SortByDefense(ref targets, false);
                use.Card = new WrappedCard(HaoshiCard.ClassName)
                {
                    Skill = "_haoshi",
                    Mute = true
                };
                List<int> ids= player.GetCards("h");
                ai.SortByUseValue(ref ids, false);
                for (int i = 0; i < player.HandcardNum / 2; i++)
                    use.Card.AddSubCard(ids[i]);

                use.To.Add(targets[0]);
            }

            return use;
        }
    }

    public class HaoshiCardAI : UseCard
    {
        public HaoshiCardAI() : base(HaoshiCard.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai is StupidAI && ai.GetPlayerTendency(target) != "unknown")
                {
                    ai.UpdatePlayerRelation(player, target, true);
                }
                if (ai is SmartAI) ai.UpdatePlayerRelation(player, target, true);
            }
        }
    }
    public class DimengAI : SkillEvent
    {
        public DimengAI() : base("dimeng")
        { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && ai.FriendNoSelf.Count > 0 && !player.HasUsed("DimengCard"))
                return new List<WrappedCard> { new WrappedCard("DimengCard") { Skill = Name, ShowSkill = Name } };

            return null;
        }
    }

    public class DimengCardAI : UseCard
    {
        public DimengCardAI() : base("DimengCard")
        { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target1 = use.To[0];
                Player target2 = use.To[1];
                Player target = null;
                Player enemy = null;
                if (ai.HasSkill("zishu", target1)) enemy = target1;
                if (ai.HasSkill("zishu", target2)) enemy = target2;
                if (target1.HandcardNum > target2.HandcardNum)
                {
                    if (enemy != target2)
                        target = target2;
                    if (enemy == null) enemy = target1;
                }
                else if (target2.HandcardNum > target1.HandcardNum)
                {
                    if (enemy != target1)
                        target = target1;
                    if (enemy == null) enemy = target2;
                }

                if (ai is StupidAI && target != null && ai.GetPlayerTendency(target) != "unknown")
                {
                    ai.UpdatePlayerRelation(player, target, true);
                    if (enemy != null) ai.UpdatePlayerRelation(player, enemy, false);
                }
                if (ai is SmartAI && target != null) ai.UpdatePlayerRelation(player, target, true);
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 2.8;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> targets = new List<Player>();
            List<int> subs = new List<int>();
            double best = 0;
            List<Player> enemies = ai.GetEnemies(player);
            foreach (Player enemy in enemies)
            {
                foreach (Player friend in ai.FriendNoSelf)
                {
                    if (ai.HasSkill("zishu", friend)) continue;
                    int count = enemy.HandcardNum - friend.HandcardNum;
                    if (count >= 0)
                    {
                        double good = count * 2;
                        if (ai.HasSkill("lianying", enemy))
                            good -= 1.5 * Math.Min(enemy.HandcardNum, enemies.Count);

                        if (ai.HasSkill("lianying", friend))
                            good += 1.5 * Math.Min(friend.HandcardNum, ai.GetFriends(player).Count);
                        if (ai.HasSkill("zishu", enemy)) good += 1.5 * friend.HandcardNum;

                        List<int> ids = new List<int>();
                        foreach (int id in player.GetCards("he"))
                            if (RoomLogic.CanDiscard(room, player, player, id))
                                ids.Add(id);

                        if (ids.Count >= count)
                        {
                            double cost = 0;
                            ai.SortByKeepValue(ref ids, false);
                            int over = ai.GetOverflow(player);
                            List<int> result = new List<int>();
                            for (int i = 0; i < count; i++)
                            {
                                double value = ai.GetKeepValue(ids[i], player);
                                if (value > 0 && room.GetCardPlace(ids[i]) == Player.Place.PlaceHand && over > 0)
                                {
                                    value /= 10;
                                    over--;
                                }
                                cost += value;
                                result.Add(ids[i]);
                            }

                            double v = good - cost;
                            if (v > best)
                            {
                                targets = new List<Player> { enemy, friend };
                                subs = new List<int>(result);
                            }
                        }
                    }
                }
            }

            if (targets.Count > 0)
            {
                card.AddSubCards(subs);
                use.Card = card;
                use.To = targets;
            }

            foreach (Player friend in ai.FriendNoSelf)
            {
                if (ai.HasSkill("lianying", friend))
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (friend == p) continue;
                        int count = Math.Abs(p.HandcardNum - friend.HandcardNum);
                        double good = 1.5 * Math.Min(friend.HandcardNum, ai.GetFriends(player).Count);
                        if (!ai.IsFriend(p)) good -= (friend.HandcardNum - p.HandcardNum) * 1.5;

                        List<int> ids = new List<int>();
                        foreach (int id in player.GetCards("he"))
                            if (RoomLogic.CanDiscard(room, player, player, id))
                                ids.Add(id);

                        if (ids.Count >= count)
                        {
                            double cost = 0;
                            ai.SortByKeepValue(ref ids, false);
                            int over = ai.GetOverflow(player);
                            List<int> result = new List<int>();
                            for (int i = 0; i < count; i++)
                            {
                                double value = ai.GetKeepValue(ids[i], player);
                                if (value > 0 && room.GetCardPlace(ids[i]) == Player.Place.PlaceHand && over > 0)
                                {
                                    value /= 10;
                                    over--;
                                }
                                cost += value;
                                result.Add(ids[i]);
                            }

                            double v = good - cost;
                            if (v > best)
                            {
                                targets = new List<Player> { p, friend };
                                subs = new List<int>(result);
                            }
                        }
                    }
                }
            }

            if (targets.Count > 0)
            {
                card.AddSubCards(subs);
                use.Card = card;
                use.To = targets;
            }
        }
    }

    public class ZhijianAI : SkillEvent
    {
        public ZhijianAI() : base("zhijian")
        {
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> ids = new List<WrappedCard>();
            foreach (int id in player.GetCards("h"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(ai.Room.GetCard(id).Name);
                if (fcard.Name == CrossBow.ClassName)
                {
                    List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player, true);
                    if (slashes.Count >= 4)
                    {
                        List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes);
                        if (scores.Count > 0 && scores[0].Score > 0)
                            continue;
                    }
                }
                if (fcard is EquipCard)
                {
                    WrappedCard card = new WrappedCard("ZhijianCard") { Skill = Name, ShowSkill = Name };
                    card.AddSubCard(id);
                    ids.Add(card);
                }
            }

            return ids;
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card.Id >= 0 && ai.HasSkill(Name, player) && !isUse && place == Player.Place.PlaceHand)
            {
                FunctionCard fcard = Engine.GetFunctionCard(ai.Room.GetCard(card.GetEffectiveId()).Name);
                if (fcard is EquipCard)
                {
                    return 0.5;
                }
            }

            return 0;
        }
    }

    public class ZhijianCardAI : UseCard
    {
        public ZhijianCardAI() : base("ZhijianCard")
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 8;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (!player.HasShownOneGeneral())
                {
                    string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                    ai.UpdatePlayerIntention(player, Scenario.Hegemony.WillbeRole(room, player), 100);
                }
                foreach (Player p in use.To)
                    ai.UpdatePlayerRelation(player, p, true);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            List<Player> friends = new List<Player>(ai.FriendNoSelf);
            ai.SortByDefense(ref friends, false);

            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card.GetEffectiveId()).Name);
            WrappedCard equip_card = room.GetCard(card.SubCards[0]);
            EquipCard equip = (EquipCard)fcard;
            int equip_index = (int)equip.EquipLocation();

            if (fcard is EquipCard)
            {
                foreach (Player p in friends)
                {
                    if ((ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill(TrustedAI.NeedEquipSkill)) && RoomLogic.CanPutEquip(p, room.GetCard(card.GetEffectiveId())) && p.GetEquip(equip_index) < 0)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                foreach (Player p in friends)
                {
                    if (RoomLogic.CanPutEquip(p, room.GetCard(card.GetEffectiveId())) && p.GetEquip(equip_index) < 0)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }
        }
    }

    public class GuzhengAI : SkillEvent
    {
        public GuzhengAI() : base("guzheng")
        {
            key = new List<string> { "cardExchange" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && ai is StupidAI && data is string str && player != ai.Self)
            {
                string[] strs = str.Split(':');
                Room room = ai.Room;
                if (strs[1] == Name && ai.GetPlayerTendency(room.Current) != "unknown" && string.IsNullOrEmpty(strs[2]))
                {
                    string role = ai.GetPlayerTendency(room.Current);
                    if (role == "lord" || role == "loyalist")
                        ai.UpdatePlayerIntention(player, "rebel", 50);
                    else
                        ai.UpdatePlayerIntention(player, "loyalist", 50);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetPile("#guzheng");
            Room room = ai.Room;
            Player target = room.Current;
            if (ai.IsFriend(target))
            {
                if ((ai.WillShowForDefence() || ai.IsWeak(target)) && (!ai.NeedKongcheng(target) || !target.IsKongcheng()))
                {
                    double value = 0;
                    int result = -1;
                    foreach (int id in ids)
                    {
                        double v = ai.GetKeepValue(id, target, Player.Place.PlaceHand);
                        if (v > value)
                        {
                            value = v;
                            result = id;
                        }
                    }
                    if (value > 0)
                        return new List<int> { result };
                }
            }
            else
            {
                if (ids.Count > 1)
                {
                    double value = 1000;
                    double all = 0;
                    int result = -1;
                    foreach (int id in ids)
                    {
                        double v = ai.GetKeepValue(id, target, Player.Place.PlaceHand);
                        all += ai.GetKeepValue(id, player, Player.Place.PlaceHand);
                        if (v < value)
                        {
                            value = v;
                            result = id;
                        }
                    }
                    if (all - value > 5 || value < 2)
                        return new List<int> { result };
                }
                else if (ai.NeedKongcheng(target) && target.IsKongcheng() && !ai.IsCard(ids[0], Jink.ClassName, target))
                {
                    return ids;
                }
            }

            return new List<int>();
        }
    }

    public class DuanbingAI : SkillEvent
    {
        public DuanbingAI() : base("duanbing")
        { }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            if (ai.WillShowForAttack() && room.GetTag("extra_target_skill") is CardUseStruct use)
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in target)
                {
                    ScoreStruct score = ai.SlashIsEffective(use.Card, p);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 0)
                    return scores[0].Players;
            }

            return new List<Player>();
        }
    }

    public class FenxunAI : SkillEvent
    {
        public FenxunAI() : base("fenxun") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("FenxunCard") && !player.IsNude())
            {
                WrappedCard first = new WrappedCard("FenxunCard")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                return new List<WrappedCard> { first };
            }

            return null;
        }
    }
    public class FenxunCardAI : UseCard
    {
        public FenxunCardAI() : base("FenxunCard") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 8;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);

            if (ids.Count > 0)
            {
                int sub = -1;
                ai.SortByKeepValue(ref ids, false);
                if (ai.GetKeepValue(ids[0], player) < 0)
                    sub = ids[0];

                double snatch_value = 0;
                Player snatch_target = null;
                List<WrappedCard> snatch = ai.GetCards(Snatch.ClassName, player);
                if (snatch.Count > 1)
                {
                    List<ScoreStruct> scores = new List<ScoreStruct>();
                    List<Player> target = new List<Player>();
                    foreach (WrappedCard s in snatch)
                    {
                        foreach (Player p in room.GetOtherPlayers(player))
                        {
                            if (!target.Contains(p) && RoomLogic.IsProhibited(room, player, p, card) == null && !ai.IsCancelTarget(s, p, player) && ai.IsCardEffect(s, p, player)
                                && RoomLogic.CanGetCard(room, player, p, "hej"))
                            {
                                ScoreStruct score = ai.FindCards2Discard(player, p, string.Empty, "he", FunctionCard.HandlingMethod.MethodGet);
                                score.Players = new List<Player> { p };
                                scores.Add(score);
                                target.Add(p);
                            }
                        }
                    }
                    if (scores.Count > 0)
                    {
                        ai.CompareByScore(ref scores);
                        if (scores[0].Score > 0 && RoomLogic.DistanceTo(room, player, scores[0].Players[0]) > 1)
                        {
                            snatch_value = scores[0].Score;
                            snatch_target = scores[0].Players[0];
                        }
                    }
                }
                double slash_value = 0;
                Player slash_target = null;
                List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player), _slahes = new List<WrappedCard>();
                List<ScoreStruct> slash_score = ai.CaculateSlashIncome(player, slashes);
                if (slash_score.Count > 0)
                    slash_value = slash_score[0].Score;

                foreach (WrappedCard sla in slashes)
                {
                    WrappedCard new_card = new WrappedCard(sla.Name)
                    {
                        DistanceLimited = false
                    };
                    new_card.AddSubCard(sla);
                    new_card = RoomLogic.ParseUseCard(room, new_card);
                }

                slash_score = ai.CaculateSlashIncome(player, slashes);
                if (slash_score.Count > 0 && slash_score[0].Score > slash_value && slash_score[0].Players.Count == 1
                        && RoomLogic.DistanceTo(room, player, slash_score[0].Players[0]) > 1)
                {
                    slash_value = slash_score[0].Score - slash_value;
                    slash_target = slash_score[0].Players[0];
                }
                else
                {
                    slash_value = 0;
                }

                Player target_fix = slash_value > snatch_value ? slash_target : snatch_target;

                if (sub > -1)
                {
                    card.AddSubCard(sub);
                    use.Card = card;
                    if (target_fix != null)
                        use.To.Add(target_fix);
                    else
                        use.To.Add(room.GetOtherPlayers(player)[0]);
                    return;
                }
                else if (target_fix != null)
                {
                    ai.SortByUseValue(ref ids, false);
                    double cost = ai.GetUseValue(ids[0], player);
                    if (cost < slash_value || cost < snatch_value)
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        use.To.Add(target_fix);
                    }
                }
            }
        }
    }
}