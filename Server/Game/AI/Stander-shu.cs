using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class StanderShuAI : AIPackage
    {
        public StanderShuAI() : base("Stander-shu")
        {
            events = new List<SkillEvent>
            {
                new RendeAI(),
                new WushengAI(),
                new PaoxiaoAI(),
                new LongdanAI(),
                new TieqiAI(),
                new NiepanAI(),
                new LianhuanAI(),
                new BazhenAI(),
                new HuojiAI(),
                new KanpoAI(),
                new KongchengAI(),
                new GuanxingAI(),
                new KuangguAI(),
                new XiangleAI(),
                new FangquanAI(),
                new JizhiAI(),
                new LiegongAI(),
                new HuoshouAI(),
                new ZaiqiAI(),
                new SAAvoid1AI(),
                new SAAvoid2AI(),
                new JuxiangAI(),
                new LierenAI(),
                new ShushenAI(),
                new ShenzhiAI(),
            };

            use_cards = new List<UseCard>
            {
                new RendeCardAI(),
                new FangquanCardAI(),
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
            Player shenpei = ai.FindPlayerBySkill("beizhan");

            if (player.ContainsTag("zhengu") && player.GetTag("zhengu") is string haozhao)
            {
                Player owner = room.FindPlayer(haozhao);
                if (owner != null && (ai.IsFriend(owner) || player.HandcardNum > owner.HandcardNum))
                    return true;
            }
            Player fazheng = ai.FindPlayerBySkill("enyuan_jx");
            if (fazheng != null && ai.IsFriend(fazheng) && player.HandcardNum >= 2 && !fazheng.HasFlag("rende_" + player.Name))
                return true;

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

                List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player);

                List<WrappedCard> all_cards = new List<WrappedCard>();
                List<string> names = ViewAsSkill.GetGuhuoCards(room, "b");
                foreach (string name in names)
                {
                    if (name == Jink.ClassName) continue;
                    WrappedCard card = new WrappedCard(name)
                    {
                        Skill = "_rende"
                    };
                    FunctionCard fcard = Engine.GetFunctionCard(name);
                    if (fcard.IsAvailable(room, player, card))
                    {
                        all_cards.Add(card);
                        if (name.Contains(Slash.ClassName))
                        {
                            can_slash = true;
                            slashes.Add(card);
                        }
                        if (name == Analeptic.ClassName)
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
                    if (values.Count > 0 && values[0].Score > 2)
                    {
                        if (values[0].Card.Skill == "_rende")
                        {
                            slash_value = values[0].Score;
                            slash = values[0].Card;
                        }

                        foreach (ScoreStruct score in values)
                        {
                            if (!score.Card.Skill.Contains("rende") && slash_value - score.Score < 1)
                            {
                                slash = score.Card;
                                break;
                            }
                        }

                        if (slash != null && !slash.Skill.Contains("rende") && drink != null && player.HandcardNum > 2 - player.GetMark(Name))
                        {
                            int hand = 0;
                            foreach (int id in slash.SubCards)
                                if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                                    hand++;
                            if (player.HandcardNum - hand > 2 - player.GetMark(Name))
                            {
                                ai.SetPreDrink(drink);
                                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { slash });
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
                    ai.Choice[Name] = Analeptic.ClassName;
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
                    ai.Choice[Name] = Peach.ClassName;
                    return true;
                }
                else if (friends.Count > 0
                        && (ai.GetCardNeedPlayer(player.GetCards("h"), friends).Key != null
                        || ai.GetOverflow(player) > 0
                        || (shenpei != null && ai.IsFriend(shenpei))))
                    return true;
            }
            else
            {
                ai.Choice[Name] = null;
                if (ai.NeedKongcheng(player))
                    return true;
                else
                {
                    if (friends.Count == 0) return false;
                    if (ai.GetCardNeedPlayer(player.GetCards("h"), friends).Key != null || ai.GetOverflow(player) > 0 || (shenpei != null && ai.IsFriend(shenpei)))
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
            string card_name = player.HasFlag("rende_judged") ? ai.Choice[Name] : string.Empty;
            bool use_slash = false;
            if (!string.IsNullOrEmpty(card_name))
            {
                if (card_name.Contains(Slash.ClassName))
                {
                    use_slash = true;
                }
                else
                {
                    WrappedCard card = new WrappedCard(card_name) { Skill = "_rende" };
                    FunctionCard fcard = Engine.GetFunctionCard(card_name);
                    if (fcard.IsAvailable(room, player, card))
                    {
                        use.Card = card;
                        return use;
                    }
                }
            }
            else
            {
                use_slash = true;
            }

            if (use_slash)
            {
                WrappedCard card = new WrappedCard(Slash.ClassName) { Skill = "_rende" };
                FunctionCard fcard = Slash.Instance;
                if (fcard.IsAvailable(room, player, card))
                {

                    WrappedCard fire = new WrappedCard(FireSlash.ClassName)
                    {
                        Skill = "_rende"
                    };
                    WrappedCard thunder = new WrappedCard(ThunderSlash.ClassName)
                    {
                        Skill = "_rende"
                    };
                    List<WrappedCard> slashes = new List<WrappedCard>{ card, fire, thunder};

                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes);
                    if (scores.Count > 0 && scores[0].Score > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
                    {
                        use.Card = scores[0].Card;
                        use.To = scores[0].Players;
                        return use;
                    }
                }
            }

            WrappedCard peach = new WrappedCard(Peach.ClassName) { Skill = "_rend" };
            if (Peach.Instance.IsAvailable(room, player, peach))
            {
                use.Card = peach;
                return use;
            }
            WrappedCard ana = new WrappedCard(Analeptic.ClassName) { Skill = "_rend" };
            if (Analeptic.Instance.IsAvailable(room, player, peach))
            {
                use.Card = ana;
                return use;
            }

            return use;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && ShouldUse(ai, player))
                return new List<WrappedCard> { new WrappedCard("RendeCard") { Skill = Name, ShowSkill = Name } };

            return null;
        }
        
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is SmartAI && ai.Self != player)
                {
                    foreach (Player p in use.To)
                        ai.UpdatePlayerRelation(player, p, true);
                }
                else if (ai is StupidAI)
                {
                    foreach (Player p in use.To)
                        if (ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, true);
                }
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
            List<int> ids= player.GetCards("h");
            if (ids.Count == 0) return;

            Room room = ai.Room;
            if (player.ContainsTag("zhengu") && player.GetTag("zhengu") is string haozhao)
            {
                Player owner = room.FindPlayer(haozhao);
                if (owner != null && ai.IsFriend(owner) && owner.HandcardNum < 5 && !owner.HasFlag("rende_" + player.Name))
                {
                    List<int> sub = new List<int>();
                    ai.SortByKeepValue(ref ids, false);
                    for (int i = 0; i < Math.Min(ids.Count, 5 - owner.HandcardNum); i++)
                        sub.Add(ids[i]);

                    card.AddSubCards(sub);
                    use.Card = card;
                    use.To = new List<Player> { owner };
                    return;
                }
            }

            Player fazheng = ai.FindPlayerBySkill("enyuan_jx");
            if (fazheng != null && ai.IsFriend(fazheng) && player.HandcardNum >= 2 && !fazheng.HasFlag("rende_" + player.Name))
            {
                List<int> sub = new List<int>();
                ai.SortByUseValue(ref ids, false);
                for (int i = 0; i < 2; i++)
                    sub.Add(ids[i]);

                card.AddSubCards(sub);
                use.Card = card;
                use.To = new List<Player> { fazheng };
                return;
            }

            string card_name = player.HasFlag("rende_judged") && player.GetMark("rende") < 2 ? ai.Choice["rende"] : string.Empty;
            List<Player> friends = ai.FriendNoSelf;
            friends.RemoveAll(t => t.HasFlag("rende_" + player.Name));
            List<Player> _friends = new List<Player>(friends);

            if (!string.IsNullOrEmpty(card_name) && player.HandcardNum >= 2 - player.GetMark("rende"))
            {
                if (card_name.Contains(Slash.ClassName))
                {
                    List<WrappedCard> ana = ai.GetCards(Analeptic.ClassName, player, true);
                    if (ana.Count > 0)
                    {
                        ai.SortByUseValue(ref ana);
                        int hand = 0;
                        foreach (int id in ana[0].SubCards)
                            if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                                hand++;
                        FunctionCard fcard = Analeptic.Instance;
                        if (fcard.IsAvailable(room, player, ana[0]) && player.HandcardNum - hand > 2 - player.GetMark("rende") && friends.Count > 0)
                        {
                            use.Card = ana[0];
                            use.To = new List<Player>();
                            return;
                        }
                    }
                }

                if (card_name == Analeptic.ClassName)
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
                while (_ids.Count > 0)
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
                    ids.RemoveAll(t => card.SubCards.Contains(t));
                    //如只有1位友方，则须至少给够2张牌
                    if (friends.Count == 1 && card.SubCards.Count < 2 - player.GetMark("rende") && ids.Count > 0)
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

            if (card_name == Peach.ClassName && player.HandcardNum >= 2 - player.GetMark("rende"))
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

                        //把所有手牌交给敌人空城回血
                        card.AddSubCards(ids);
                        use.Card = card;
                        use.To = new List<Player> { enemies[0] };
                        return;
                    }
                }
            }

            if (friends.Count > 0)
            {
                room.SortByActionOrder(ref friends);
                Player target = null;
                foreach (Player p in friends)
                {
                    if (!ai.WillSkipPlayPhase(p))
                    {
                        target = p;
                        break;
                    }
                }
                if (target == null)
                {
                    ai.SortByDefense(ref friends, false);
                    target = friends[0];
                }

                Player shenpei = ai.FindPlayerBySkill("beizhan");
                Player gn = RoomLogic.FindPlayerBySkillName(ai.Room, "jieying_gn");

                if (ai.HasSkill("kongcheng") && ai.NeedKongcheng(player))
                {
                    card.AddSubCards(ids);

                    use.Card = card;
                    use.To.Add(target);
                }
                else if (shenpei != null && ai.IsFriend(shenpei))
                {
                    if (ai.MaySave(shenpei))
                    {
                        card.AddSubCards(ids);
                        use.Card = card;
                        use.To.Add(target);
                    }
                    else
                    {
                        List<WrappedCard> jinks = ai.GetCards(Jink.ClassName, player);
                        int id = jinks.Count > 0 ? jinks[0].GetEffectiveId() : -1;
                        foreach (int i in ids)
                            if (id != i)
                                card.AddSubCard(id);

                        if (card.SubCards.Count > 0)
                        {
                            use.Card = card;
                            use.To.Add(target);
                        }
                    }
                }
                else if (player.GetMark("@rob") > 0 && gn != null && !ai.IsFriend(gn))
                {
                    card.AddSubCards(ids);
                    use.Card = card;
                    use.To.Add(target);
                }
                else if (ai.GetOverflow(player) > 0)
                {
                    ai.SortByUseValue(ref ids, false);
                    for (int i = 0; i < ai.GetOverflow(player); i++)
                        card.AddSubCard(ids[i]);

                    use.Card = card;
                    use.To.Add(target);
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            string card_name = player.HasFlag("rende_judged") && player.GetMark("rende") < 2 ? ai.Choice["rende"] : string.Empty;
            if (!string.IsNullOrEmpty(card_name))
                return 8;

            return 0;
        }
    }

    public class NiepanAI : SkillEvent
    {
        public NiepanAI() : base("niepan")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            int count = 0;
            List<WrappedCard> analeptics = ai.GetCards(Analeptic.ClassName, player, true);
            analeptics.AddRange(ai.GetCards(Peach.ClassName, player, true));
            foreach (WrappedCard card in analeptics)
                if (!RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, player, player, card) == null)
                    count++;

            if (count >= 1 - player.Hp)
                return false;

            return true;
        }
    }

    public class LianhuanAI : SkillEvent
    {
        public LianhuanAI() : base("lianhuan")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.WillShowForAttack()) return null;
            Room room = ai.Room;
            List<int> ids= player.GetCards("h");
            ids.AddRange(player.GetHandPile());
            ai.SortByUseValue(ref ids, false);
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Suit == WrappedCard.CardSuit.Club)
                {
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

                    if (_card != null && _card.Name == IronChain.ClassName && _card.Skill == Name) return new List<WrappedCard> { _card }; 
                }
            }

            return null;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (card != null && card.Suit == WrappedCard.CardSuit.Club && (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand))
            {
                WrappedCard ic = new WrappedCard(IronChain.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                ic.AddSubCard(card);
                ic = RoomLogic.ParseUseCard(room, ic);
                return ic;
            }

            return null;
        }
    }

    public class WushengAI : SkillEvent
    {
        public WushengAI() : base("wusheng")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = player.GetCards("he");
            ids.AddRange(player.GetHandPile());
            Player lord = RoomLogic.GetLord(room, player.Kingdom);
            bool any = true;
            if (lord == null || !RoomLogic.PlayerHasSkill(room, lord, "shouyue") || !lord.General1Showed)
            {
                any = false;
            }

            foreach (int id in ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if (any || WrappedCard.IsRed(card.Suit))
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
            if (targets.Count > 0) return 0;
            return -1;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (card.HasFlag("using")) return null;
            Player lord = RoomLogic.GetLord(room, player.Kingdom);
            bool any = true;
            if (lord == null || !RoomLogic.PlayerHasSkill(room, lord, "shouyue") || !lord.General1Showed)
            {
                any = false;
            }
            if (any || WrappedCard.IsRed(card.Suit))
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

    public class PaoxiaoAI : SkillEvent
    {
        public PaoxiaoAI() : base("paoxiao")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class LongdanAI : SkillEvent
    {
        public LongdanAI() : base("longdan")
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
            }

            return null;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            double value = 0;
            if (card.Name.Contains(Slash.ClassName) && to.HandcardNum + to.GetPile("wooden_ox").Count >= 3 && !ai.IsLackCard(to, Jink.ClassName))
            {
                foreach (Player p in ai.Room.GetOtherPlayers(ai.Self))
                {
                    if (p == to) continue;
                    if (ai.IsFriend(p, to) && p.IsWounded())
                    {
                        value += ai.IsFriend(p) ? 3 : -3;
                        break;
                    }
                }
            }

            return value;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (player.ContainsTag(Name) && player.GetTag(Name) is string user_name)
            {
                Room room = ai.Room;
                Player user = room.FindPlayer(user_name);
                if (user != null)
                {
                    if (user == player)
                    {
                        double value = 0;
                        Player tar = null;
                        foreach (Player p in target)
                        {
                            DamageStruct damage = new DamageStruct(Name, player, p);
                            ScoreStruct score = ai.GetDamageScore(damage);
                            if (score.Score > value)
                            {
                                value = score.Score;
                                tar = p;
                            }
                        }
                        if (tar != null)
                            return new List<Player> { tar };
                    }
                    else
                    {
                        ai.SortByDefense(ref target, false);
                        foreach (Player p in target)
                            if (ai.IsFriend(p))
                                return new List<Player> { p };
                    }
                }
            }

            return new List<Player>();
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            Room room = ai.Room;
            if (targets.Count == 0)
            {
                Player lord = ai.FindPlayerBySkill("shouyue");
                if (lord != null && (lord == player || lord.General1Showed))
                    return 1.2;
            }

            if (card.Name.Contains(Slash.ClassName) && targets.Count > 0)
            {
                List<Player> enemies = ai.GetEnemies(player);
                if (enemies.Count > 1)
                {
                    foreach (Player p in targets)
                        if (!ai.IsFriend(p) && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                            return 1;
                }
            }
            else if (card.Name == Jink.ClassName)
            {
                if (ai.Room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
                    && ai.Room.GetRoomState().GetCurrentCardUsePattern() == Jink.ClassName)
                {
                    foreach (Player p in ai.FriendNoSelf)
                        if (p.IsWounded())
                            return 1.5;
                }
            }

            return 0;
        }
    }

    public class TieqiAI : SkillEvent
    {
        public TieqiAI() : base("tieqi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player p)
                return ai.IsEnemy(p);

            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            string[] choices = choice.Split('+');
            Room room = ai.Room;
            if (data is Player p)
            {
                if (p.GetMark("@tieqi1") > 0)
                    return choices[1];
                else if (p.GetMark("@tieqi2") > 0)
                    return choices[0];
                else
                {
                    General general1 = Engine.GetGeneral(p.General1, room.Setting.GameMode);
                    if (general1.HasSkill(TrustedAI.MasochismSkill, room.Setting.GameMode, true))
                        return choices[0];

                    General general2 = Engine.GetGeneral(p.General2, room.Setting.GameMode);
                    if (general2.HasSkill(TrustedAI.MasochismSkill, room.Setting.GameMode, false))
                        return choices[0];

                    if (general1.HasSkill(TrustedAI.DefenseSkill, room.Setting.GameMode, true))
                        return choices[0];

                    if (general2.HasSkill(TrustedAI.DefenseSkill, room.Setting.GameMode, false))
                        return choices[0];
                }
            }

            return choices[0];
        }
    }

    public class BazhenAI : SkillEvent
    {
        public BazhenAI() : base("bazhen")
        {
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && Engine.GetFunctionCard(card.Name) is Armor)
                return -Engine.GetCardKeepValue(EightDiagram.ClassName);

            return 0;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class KanpoAI : SkillEvent
    {
        public KanpoAI() : base("kanpo")
        {
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (card != null && WrappedCard.IsBlack(card.Suit) && (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand))
            {
                WrappedCard nulli = new WrappedCard(Nullification.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                nulli.AddSubCard(id);
                nulli = RoomLogic.ParseUseCard(room, nulli);
                return nulli;
            }

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (!isUse && ai.HasSkill(Name, player) && place == Player.Place.PlaceHand && !card.IsVirtualCard())
            {
                if (WrappedCard.IsBlack(ai.Room.GetCard(card.GetEffectiveId()).Suit))
                    return 1.5;
            }

            return 0;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            int id = card.GetEffectiveId();
            Room room = ai.Room;
            List<WrappedCard> cards = ai.GetViewAsCards(player, id);
            double value = 0;
            foreach (WrappedCard c in cards)
            {
                if (c.Skill == Name) continue;
                double card_value = ai.GetUseValue(c, player, room.GetCardPlace(id));
                if (card_value > value)
                    value = card_value;
            }

            return Math.Min(0, 4 - value);
        }
    }

    public class HuojiAI : SkillEvent
    {
        public HuojiAI() : base("huoji")
        {
        }
        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return -2;
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card.Name == FireAttack.ClassName && use.Card.Skill == Name)
                return -2;

            return 0;
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids= player.GetCards("h");
            ids.AddRange(player.GetHandPile());

            foreach (int id in ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if (WrappedCard.IsRed(card.Suit) && card.Name != FireAttack.ClassName)
                {
                    WrappedCard slash = new WrappedCard(FireAttack.ClassName)
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
    }

    public class GuanxingAI : SkillEvent
    {
        public GuanxingAI() : base("guanxing")
        {
            key = new List<string> { "guanxingchose" };
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
            return true;
        }
        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            return ai.Guanxing(ups);
        }
    }
    public class KongchengAI : SkillEvent
    {
        public KongchengAI() : base("kongcheng")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardUseStruct)
                return true;
            else if (player.IsKongcheng())
                return true;

            return false;
        }

        public override bool IsCancelTarget(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            if (to != null && ai.HasSkill(Name, to) && (card.Name == Duel.ClassName || card.Name.Contains(Slash.ClassName)) && to.IsKongcheng())
                return true;

            return false;
        }
    }

    public class KuangguAI : SkillEvent
    {
        public KuangguAI() : base("kuanggu")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

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

    public class XiangleAI : SkillEvent
    {
        public XiangleAI() : base("xiangle")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardUseStruct use)
            {
                if (use.From != null && RoomLogic.IsFriendWith(ai.Room, use.From, player))
                {
                    DamageStruct damage = new DamageStruct(use.Card, use.From, player, 1 + use.Drank + use.ExDamage,
                        use.Card.Name.Contains("Fire") ? DamageStruct.DamageNature.Fire : use.Card.Name.Contains("Thunder") ? DamageStruct.DamageNature.Thunder : DamageStruct.DamageNature.Normal);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    if (score.DoDamage)
                        score.Score += ai.ChainDamage(damage);

                    if (score.Score > 0)
                        return false;
                }
            }

            return true;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct result = new CardUseStruct();
            Room room = ai.Room;
            if (data is CardUseStruct use)
            {
                int index = (int)player.GetTag(Name);
                Player target = use.To[index];
                CardBasicEffect effect = use.EffectCount[index];

                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard c = ai.Room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(c.Name);
                    if (RoomLogic.CanDiscard(ai.Room, player, player, id) && fcard is BasicCard)
                        ids.Add(id);
                }

                if (ids.Count > 0)
                {
                    DamageStruct damage = new DamageStruct(use.Card, player, target, 1 + use.Drank + use.ExDamage + effect.Effect1,
                        use.Card.Name.Contains("Fire") ? DamageStruct.DamageNature.Fire : use.Card.Name.Contains("Thunder") ? DamageStruct.DamageNature.Thunder : DamageStruct.DamageNature.Normal);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    if (score.DoDamage)
                        score.Score += ai.ChainDamage(damage);
                    
                    double value = 0;
                    if (room.Current == player)
                    {
                        ai.SortByKeepValue(ref ids, false);
                        value = ai.GetUseValue(ids[0], player);
                        if (ai.GetOverflow(player) > 0)
                            value /= 2;
                    }
                    else
                    {
                        ai.SortByKeepValue(ref ids, false);
                        value = ai.GetKeepValue(ids[0], player);
                    }

                    if (score.Score - 5 > (value / 2))
                        result.Card = room.GetCard(ids[0]);
                }
            }

            return result;
        }

        public override bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            if (card.Name.Contains(Slash.ClassName) && ai.HasSkill(Name, to) && from != null)
            {
                if (from.HandcardNum == 0) return false;
                Room room = ai.Room;
                if (!ai.IsFriend(from, to))
                {
                    List<int> known = ai.GetKnownCards(from);
                    int count = from.HandcardNum - known.Count;
                    foreach (int id in card.SubCards)
                    {
                        if (room.GetCardOwner(id) == from && room.GetCardPlace(id) == Player.Place.PlaceHand)
                        {
                            if (known.Contains(id))
                                known.Remove(id);
                            else
                                count--;
                        }
                    }

                    foreach (int id in ai.GetKnownCards(from))
                    {
                        WrappedCard c = ai.Room.GetCard(id);
                        FunctionCard fcard = Engine.GetFunctionCard(c.Name);
                        if (RoomLogic.CanDiscard(ai.Room, from, from, id) && fcard is BasicCard)
                            return true;
                    }

                    if (count > 2) return true;
                }

                return false;
            }

            return true;
        }
        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            Room room = ai.Room;
            Player player = ai.Self;
            double value = 0;
            if (card.Name.Contains(Slash.ClassName))
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    if (card.SubCards.Contains(id)) continue;
                    WrappedCard c = ai.Room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(c.Name);
                    if (RoomLogic.CanDiscard(ai.Room, player, player, id) && fcard is BasicCard)
                        ids.Add(id);
                }

                if (ids.Count == 0) return -1000;
                else
                {
                    if (room.Current == player)
                    {
                        ai.SortByKeepValue(ref ids, false);
                        value = ai.GetUseValue(ids[0], player);
                        if (ai.GetOverflow(player) > 0)
                            value /= 2;
                    }
                    else
                    {
                        ai.SortByKeepValue(ref ids, false);
                        value = ai.GetKeepValue(ids[0], player);
                    }
                    value /= 2;
                }

                if (ai.IsFriend(to) && RoomLogic.PlayerHasShownSkill(room, to, Name) || ai.IsEnemy(to))
                    value = -value;
            }

            return value;
        }
    }

    public class FangquanCardAI : UseCard
    {
        public FangquanCardAI() : base(FangquanCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && ai is StupidAI && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
            }
        }
    }

    public class FangquanAI : SkillEvent
    {
        public FangquanAI() : base("fangquan")
        {
        }
       
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            ai.Target[Name] = null;

            Room room = ai.Room;
            if (ai.HasSkill("rende|jili")) return false;
            if (ai.HasSkill("jizhi|jizhi_jx"))
            {
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is TrickCard && !(fcard is DelayedTrick) && !(fcard is Nullification))
                        return false;
                }
            }

            if (ai.FriendNoSelf.Count > 0 && player.HandcardNum > 0)
            {
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, null, null, false);
                if (scores.Count > 0 && scores[0].Card != null && scores[0].Score > 6) return false;

                List<int> cards = new List<int>();
                foreach (int id in player.GetCards("h"))
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        cards.Add(id);

                if (cards.Count > 0)
                {
                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (!ai.WillSkipPlayPhase(p, player))
                        {
                            if (ai.HasSkill("jizhi|jizhi_jx|jili|rende", p) || player.HandcardNum >= player.HandcardNum)
                            {
                                ai.Target[Name] = p;
                                break;
                            }
                        }
                    }

                    if (ai.Target[Name] != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player
            };

            if (ai.Target[Name] != null)
            {
                List<int> cards = new List<int>();
                foreach (int id in player.GetCards("h"))
                    if (RoomLogic.CanDiscard(ai.Room, player, player, id))
                        cards.Add(id);

                ai.SortByKeepValue(ref cards, false);

                use.Card = new WrappedCard(FangquanCard.ClassName)
                {
                    Skill = Name
                };
                use.Card.AddSubCard(cards[0]);
                use.To = new List<Player> { ai.Target[Name] };
            }

            return use;
        }
    }

    public class JizhiAI : SkillEvent
    {
        public JizhiAI() : base("jizhi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (player.Phase == Player.PlayerPhase.Judge)
            {
                List<WrappedCard> cards = new List<WrappedCard>();
                for (int i = player.JudgingArea.Count; i > 0; i--)
                {
                    WrappedCard card = room.GetCard(player.JudgingArea[i - 1]);
                    cards.Add(card);
                }

                foreach (WrappedCard judge in cards)
                    if (ai.IsGuanxingEffected(player, false, judge))
                        return false;
            }

            return true;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is TrickCard && !(fcard is DelayedTrick) && !card.IsVirtualCard())
                    return 2;
            }

            return 0;
        }
    }

    public class LiegongAI : SkillEvent
    {
        public LiegongAI() : base("liegong")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player p)
            {
                return ai.IsEnemy(p);
            }

            return true;
        }
    }

    public class HuoshouAI : SkillEvent
    {
        public HuoshouAI() : base("huoshou")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForAttack() || ai.WillShowForDefence();
        }
        public override bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            if (ai.HasSkill(Name, to) && card != null && card.Name == SavageAssault.ClassName)
                return false;

            return true;
        }
    }

    public class ZaiqiAI : SkillEvent
    {
        public ZaiqiAI() : base("zaiqi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            int count = player.GetLostHp();
            if (count == 2 && ai.HasSkill("rende") && ai.FriendNoSelf.Count > 0 && !ai.WillSkipPlayPhase(player))
                return false;

            if (count == 1 && RoomLogic.GetMaxCards(ai.Room, player) <= player.HandcardNum && player.IsSkipped(Player.PlayerPhase.Play))
                return true;

            return count >= 2;
        }

        public override double GetSkillAdjustValue(TrustedAI ai, Player player)
        {
            double value = player.MaxHp - 4;
            if (ai.Room.BloodBattle)
                value += 1.5;

            return value;
        }
    }

    public class SAAvoid1AI : SkillEvent
    {
        public SAAvoid1AI() : base("#sa_avoid_huoshou")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }
    public class SAAvoid2AI : SkillEvent
    {
        public SAAvoid2AI() : base("#sa_avoid_juxiang")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class JuxiangAI : SkillEvent
    {
        public JuxiangAI() : base("juxiang") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            if (ai.HasSkill(Name, to) && card != null && card.Name == SavageAssault.ClassName)
                return false;

            return true;
        }
    }

    public class LierenAI : SkillEvent
    {
        public LierenAI() : base("lieren") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsEnemy(target))
            {
                bool can_pindian = false;
                if (target.HandcardNum != 1 || !ai.NeedKongcheng(target))
                {
                    if (target.HandcardNum == 1)
                    {
                        foreach (int id in target.GetEquips())
                        {
                            if (RoomLogic.CanGetCard(ai.Room,player, target, id) && ai.GetKeepValue(id, target, Player.Place.PlaceEquip) > 0)
                            {
                                can_pindian = true;
                                break;
                            }
                        }
                    }
                }

                if (can_pindian)
                {
                    List<int> ids = new List<int>();
                    if (ai.Room.Current == player)
                    {
                        foreach (int id in player.GetCards("h"))
                        {
                            if (ai.GetUseValue(id, player, Player.Place.PlaceHand) < 6)
                                ids.Add(id);
                        }
                    }
                    else
                    {
                        foreach (int id in player.GetCards("h"))
                        {
                            if (ai.GetKeepValue(id, player, Player.Place.PlaceHand) < 5)
                                ids.Add(id);
                        }
                    }

                    if (ids.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> player)
        {
            if (ai.Self == requestor)
            {
                List<int> ids = new List<int>();
                if (ai.Room.Current == requestor)
                {
                    foreach (int id in requestor.GetCards("h"))
                    {
                        if (ai.GetUseValue(id, requestor, Player.Place.PlaceHand) < 6)
                            ids.Add(id);
                    }
                }
                else
                {
                    foreach (int id in requestor.GetCards("h"))
                    {
                        if (ai.GetKeepValue(id, requestor, Player.Place.PlaceHand) < 5)
                            ids.Add(id);
                    }
                }

                if (ids.Count > 0)
                {
                    return ai.GetMaxCard(requestor, ids);
                }
                else
                {
                    if (player[0].GetCards("he").Count > 1)
                        return ai.GetMaxCard(requestor);
                    else
                    {
                        ids = requestor.GetCards("h");
                        if (ai.Room.Current == requestor)
                        {
                            ai.SortByUseValue(ref ids, false);
                        }
                        else
                        {
                            ai.SortByKeepValue(ref ids, false);
                        }

                        return ai.Room.GetCard(ids[0]);
                    }
                }
            }
            else
            {
                return ai.GetMaxCard(ai.Self);
            }
        }
    }

    public class ShushenAI : SkillEvent
    {
        public ShushenAI() : base("shushen")
        {
            key = new List<string> { "playerChosen:shushen" };
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
                    if (!player.HasShownOneGeneral())
                    {
                        string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 100);
                    }
                    Player target = room.FindPlayer(choices[2]);
                    if (target != null && ai.GetPlayerTendency(target) == "unknown" && ai.IsKnown(player, target))
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            ai.SortByDefense(ref target, false);
            foreach (Player p in target)
            {
                if (ai.IsFriend(p))
                {
                    if (!p.IsKongcheng() || !ai.NeedKongcheng(p))
                    {
                        if (ai.HasSkill(TrustedAI.NeedEquipSkill, p))
                            return new List<Player> { p };
                    }
                }
            }
            foreach (Player p in target)
            {
                if (ai.IsFriend(p))
                    if (!p.IsKongcheng() || !ai.NeedKongcheng(p))
                        return new List<Player> { p };
            }
            foreach (Player p in target)
                if (ai.IsEnemy(p) && p.IsKongcheng() && ai.NeedKongcheng(p))
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class ShenzhiAI : SkillEvent
    {
        public ShenzhiAI() : base("shenzhi")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            int count = 2;
            if (room.BloodBattle) count++;
            if (player.Hp == 1) count++;
            if (ai.FriendNoSelf.Count > 0 && ai.HasSkill("shushen")) count++;
            if (player.HandcardNum >= player.GetLostHp() && player.HandcardNum < count)
            {
                if (!ai.WillSkipPlayPhase(player))
                {
                    foreach (int id in player.GetCards("h"))
                        if (ai.IsCard(id, Peach.ClassName, player)) return false;
                }

                return true;
            }

            return false;
        }
    }

}