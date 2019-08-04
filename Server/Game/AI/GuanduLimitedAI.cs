using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class GuanduLimitedAI : AIPackage
    {
        public GuanduLimitedAI() : base("GuanduLimited")
        {
            events = new List<SkillEvent>
            {
                new LuanjiJXAI(),
                new ShicaiAI(),
                new ChenggongAI(),
                new ZhezhuAI(),
                new XiyingAI(),
                new ShushouAI(),
                new LiangyingAI(),
                new GangzhiAI(),
                new BeizhanAI(),
                new YuanlueAI(),
                new FenglueAI(),
                new MoushiAI(),
                new ShibeiAI(),
                new JianyingAI(),
                new BifaAI(),
                new SongciAI(),
                new JigongAI(),
                new ShifeiAI(),

                new JushouJXAI(),
                new JieweiAI(),
                new DuanliangJXAI(),
                new ShefuAI(),
                new BenyuAI(),
                new JiemingJXAI(),
                new ChoulueAI(),
                new PoluAI(),
                new YuanhuAI(),
                new ZhenjunAI(),
                new ShenduanAI(),
                new YonglueAI(),
                new QiceJXAI(),
            };
            use_cards = new List<UseCard>
            {
                new ShicaiCardAI(),
                new ZhezhuCardAI(),
                new YuanlueCardAI(),
                new MoushiCardAI(),
                new SongciCardAI(),
            };
        }
    }

    public class LuanjiJXAI : SkillEvent
    {
        public LuanjiJXAI() : base("luanji_jx") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;

            List<int> ids = player.GetCards("he"), available = new List<int>();
            ids.AddRange(player.GetHandPile());
            double basic_value = Engine.GetCardUseValue(ArcheryAttack.ClassName);

            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                CardUseStruct dummy_use = new CardUseStruct(null, player, new List<Player>())
                {
                    IsDummy = true
                };
                UseCard e = Engine.GetCardUsage(card.Name);
                if (e != null) e.Use(ai, player, ref dummy_use, card);
                if (dummy_use.Card != null) return result;

                double value = ai.GetUseValue(id, player);
                if (value < basic_value)
                    available.Add(id);
            }

            if (available.Count > 1)
            {
                ai.SortByUseValue(ref available, false);
                List<WrappedCard.CardSuit> used_suits = new List<WrappedCard.CardSuit>();
                for (int i = 0; i < available.Count; i++)
                {
                    if (used_suits.Contains(room.GetCard(available[i]).Suit)) continue;
                    int first = available[i];
                    for (int y = i + 1; y < available.Count; y++)
                    {
                        if (room.GetCard(first).Suit == room.GetCard(available[y]).Suit)
                        {
                            WrappedCard aa = new WrappedCard(ArcheryAttack.ClassName) { Skill = Name, ShowSkill = Name };
                            aa.AddSubCard(first);
                            aa.AddSubCard(available[y]);
                            used_suits.Add(room.GetCard(first).Suit);
                            result.Add(aa);
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }

    public class ShicaiAI : SkillEvent
    {
        public ShicaiAI() : base("shicai") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark(Name) == 0)
            {
                Room room = ai.Room;
                List<int> ids = player.GetCards("he");
                ai.SortByKeepValue(ref ids, false);
                if (ai.GetKeepValue(ids[0], player) < 0)
                {
                    WrappedCard sc = new WrappedCard(ShicaiCard.ClassName) { Skill = Name };
                    sc.AddSubCard(ids[0]);
                    return new List<WrappedCard> { sc };
                }

                int id = player.GetPile(Name)[0];
                double value = ai.GetUseValue(id, player, Player.Place.PlaceHand);
                CardUseStruct dummy_use = new CardUseStruct(null, player, new List<Player>()) { IsDummy = true };
                UseCard e = Engine.GetCardUsage(room.GetCard(id).Name);
                if (e != null)
                {
                    e.Use(ai, player, ref dummy_use, room.GetCard(id));
                    if (dummy_use.Card != null)
                    {
                        ai.SortByUseValue(ref ids, false);
                        foreach (int i in ids)
                        {
                            if (ai.GetUseValue(i, player) < value)
                            {
                                WrappedCard sc = new WrappedCard(ShicaiCard.ClassName) { Skill = Name };
                                sc.AddSubCard(i);
                                return new List<WrappedCard> { sc };
                            }
                        }
                    }
                }
            }

            return null;
        }
    }

    public class ShicaiCardAI : UseCard
    {
        public ShicaiCardAI() : base(ShicaiCard.ClassName) { }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class ChenggongAI : SkillEvent
    {
        public ChenggongAI() : base("chenggong") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return data is Player p && ai.IsFriend(p);
        }
    }

    public class ZhezhuAI : SkillEvent
    {
        public ZhezhuAI() : base("zhezhu") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.HasUsed(ZhezhuCard.ClassName)) return new List<WrappedCard>();
            bool invoke = true;
            foreach (Player p in ai.Room.GetOtherPlayers(player))
            {
                if (p.GetRoleEnum() == Player.PlayerRole.Lord && !ai.IsFriend(p) && p.IsKongcheng())
                {
                    invoke = false;
                    break;
                }
            }

            return invoke ? new List<WrappedCard> { new WrappedCard(ZhezhuCard.ClassName) { Skill = Name } } : new List<WrappedCard>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = (Player)room.GetTag("zhezhu_target");
            List<int> ids = player.GetCards("he");
            if (ai.IsFriend(target))
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { target });
                if (pair.Key != null)
                    return new List<int> { pair.Value };
            }

            ai.SortByKeepValue(ref ids, false);
            foreach (int id in ids)
            {
                if (!ai.IsCard(id, Peach.ClassName, target) && !ai.IsCard(id, Analeptic.ClassName, target))
                    return new List<int> { id };
            }

            return new List<int> { ids[0] };
        }
    }

    public class ZhezhuCardAI : UseCard
    {
        public ZhezhuCardAI() : base(ZhezhuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 8;
        }
    }

    public class XiyingAI : SkillEvent
    {
        public XiyingAI() : base("xiying") { }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            List<int> cards = player.GetCards("he");
            if (cards.Count == 0) return new List<int>();
            ai.SortByKeepValue(ref cards, false);
            if (ai.GetKeepValue(cards[0], player) < 0)
                return new List<int> { cards[0] };

            if (player.HasFlag(Name))
            {
                bool invoke = false;
                if (ai.GetOverflow(player) > 0)
                {
                    invoke = true;
                }
                else if (cards.Count > 2 && ai.GetKnownCardsNums(Slash.ClassName, "he", player) > 0)
                {
                    foreach (Player p in ai.GetEnemies(player))
                    {
                        if (ai.IsWeak(p) && RoomLogic.InMyAttackRange(ai.Room, player, p))
                        {
                            invoke = true;
                            break;
                        }
                    }
                }

                if (invoke)
                {
                    ai.SortByUseValue(ref cards, false);
                    return new List<int> { cards[0] };
                }
            }
            else
            {
                Player from = null;
                foreach (Player p in ai.Room.GetOtherPlayers(player))
                {
                    if (p.HasFlag(Name))
                    {
                        from = p;
                        break;
                    }
                }

                if (ai.IsEnemy(from) && from.HandcardNum >= 2)
                {
                    Player help = null;
                    foreach (Player p in ai.GetFriends(player))
                    {
                        if (p.Hp == 1)
                        {
                            help = p;
                            break;
                        }
                    }

                    if (help != null)
                    {
                        if (player == help && RoomLogic.InMyAttackRange(ai.Room, from, player))
                        {
                            return new List<int> { cards[0] };
                        }
                        else if (ai.GetKnownCardsNums("Peach", "he", player) > 0 && cards.Count > 1)
                        {
                            return new List<int> { cards[0] };
                        }
                    }
                }
            }

            return new List<int>();
        }
    }

    public class ShushouAI : SkillEvent
    {
        public ShushouAI() : base("shushou") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<int> cards = ai.GetOverflow(player) > 0 ? player.GetCards("h") : player.GetCards("he");
            foreach (int id in cards)
            {
                if (room.GetCard(id).HasFlag(Name))
                    ids.Add(id);
            }
            List<Player> targets = new List<Player>();
            foreach (Player p in ai.FriendNoSelf)
                if (p.HasFlag(Name)) targets.Add(p);

            if (ids.Count > 0 && targets.Count > 0)
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, targets);
                if (pair.Key != null)
                {
                    use.Card = new WrappedCard(ShushouCard.ClassName) { Skill = Name };
                    use.Card.AddSubCard(pair.Value);
                    use.To.Add(pair.Key);
                    return use;
                }

                if (ai.GetOverflow(player) > 0)
                {
                    ai.SortByDefense(ref targets, false);
                    ai.SortByUseValue(ref ids);
                    use.Card = new WrappedCard(ShushouCard.ClassName) { Skill = Name };
                    use.Card.AddSubCard(ids[0]);
                    use.To.Add(targets[0]);
                    return use;
                }
            }

            return use;
        }
    }

    public class LiangyingAI : SkillEvent
    {
        public LiangyingAI() : base("liangying") { }
        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct();
            if (ai.HasSkill(Name, damage.To) && damage.To.GetMark("@gd_liang") > 0 && damage.Nature == DamageStruct.DamageNature.Fire)
            {
                int count = Math.Min(damage.Damage, damage.To.GetMark("@gd_liang"));
                double value = 4 * count;
                if (count >= damage.To.GetMark("@gd_liang")) value += 10;
                if (ai.IsFriend(damage.To))
                    value = -value;

                score.Score = value;
            }

            return score;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card.Name == Vine.ClassName && ai.HasSkill(Name, player) && player.GetMark("@gd_liang") > 0)
                return -8;

            return 0;
        }
    }

    public class GangzhiAI : SkillEvent
    {
        public GangzhiAI() : base("gangzhi") { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card.Name == Vine.ClassName && ai.HasSkill(Name, player))
                return 10;

            return 0;
        }
    }

    public class BeizhanAI : SkillEvent
    {
        public BeizhanAI() : base("beizhan") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            int count = 0;
            Player target = null;

            foreach (Player p in ai.GetFriends(player))
            {
                if (p.MaxHp - p.HandcardNum > count)
                {
                    count = p.MaxHp - p.HandcardNum;
                    target = p;
                }
            }

            if (target != null) return new List<Player> { target };
            count = 0;
            foreach (Player p in ai.GetEnemies(player))
            {
                if (!ai.WillSkipPlayPhase(p) && p.HandcardNum > p.MaxHp)
                {
                    count = p.HandcardNum - p.MaxHp;
                    target = p;
                }
            }

            if (target != null) return new List<Player> { target };

            return new List<Player>();
        }
    }

    public class YuanlueAI : SkillEvent
    {
        public YuanlueAI() : base("yuanlue") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return !player.HasUsed(YuanlueCard.ClassName)
                ? new List<WrappedCard> { new WrappedCard(YuanlueCard.ClassName) { Skill = Name } } : new List<WrappedCard>();
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            string[] strs = prompt.Split(':');
            Room room = ai.Room;
            Player zhanghe = room.FindPlayer(strs[1]);
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            if (zhanghe != null && ai.IsFriend(zhanghe))
            {
                int id = int.Parse(pattern);
                WrappedCard card = room.GetCard(id);
                if (card.Name == "Peach")
                {
                    use.Card = card;
                    return use;
                }

                UseCard e = Engine.GetCardUsage(card.Name);
                e.Use(ai, player, ref use, card);
                if (use.Card != null && RoomLogic.ParseUseCard(room, use.Card) != card)
                    use.Card = null;
            }

            return use;
        }
    }

    public class YuanlueCardAI : UseCard
    {
        public YuanlueCardAI() : base(YuanlueCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 0;
            List<int> cards = new List<int>();
            Room room = ai.Room;
            List<Player> targets = ai.FriendNoSelf, enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref targets, false);
            ai.SortByDefense(ref enemies, false);

            List<int> hs = new List<int>();
            foreach (int id in player.HandCards)
                if (room.GetCard(id).Name == HoardUp.ClassName)
                    hs.Add(id);
            if (hs.Count > 0)
            {
                ai.Number[Name] = 15;
                card.AddSubCard(hs[0]);
                use.Card = card;
                use.To.Add(targets[0]);
                return;
            }
            List<int> rf = new List<int>();
            foreach (int id in player.HandCards)
                if (room.GetCard(id).Name == Reinforcement.ClassName)
                    rf.Add(id);
            if (rf.Count > 0)
            {
                bool will_use = player.IsWounded();
                Player user = null;
                foreach (Player p in targets)
                {
                    if (p.IsWounded())
                        will_use = true;
                    else
                        user = p;
                }

                if (will_use && user != null)
                {
                    ai.Number[Name] = 15;
                    card.AddSubCard(rf[0]);
                    use.Card = card;
                    use.To.Add(user);
                    return;
                }
            }

            List<WrappedCard> peaches = ai.GetCards(Peach.ClassName, player);
            if (peaches.Count > 0)
            {
                foreach (Player p in targets)
                {
                    if (ai.IsWeak(p))
                    {
                        ai.Number[Name] = 7;
                        card.AddSubCard(peaches[0]);
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }

            foreach (int id in player.HandCards)
            {
                WrappedCard trick = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(trick.Name);
                if (fcard is TrickCard)
                {
                    if (fcard is DelayedTrick)
                    {
                        if (fcard.Name == Lightning.ClassName) continue;
                        foreach (Player enemy in enemies)
                        {
                            if (!RoomLogic.PlayerContainsTrick(room, enemy, trick.Name))
                            {
                                ai.Number[Name] = 9;
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(targets[0]);
                                return;
                            }
                        }
                    }
                    else if (trick.Name == ExNihilo.ClassName)
                    {
                        ai.Number[Name] = 15;
                        card.AddSubCard(id);
                        use.Card = card;
                        use.To.Add(targets[0]);
                        return;
                    }
                    else if (trick.Name == Dismantlement.ClassName)
                    {
                        foreach (Player p in targets)
                        {
                            if (p.JudgingArea.Count == 0)
                            {
                                ai.Number[Name] = 9;
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                    else if (trick.Name == Snatch.ClassName)
                    {
                        foreach (Player p in targets)
                        {
                            if (p.JudgingArea.Count > 0)
                            {
                                foreach (Player _p in targets)
                                {
                                    if (p != _p && RoomLogic.DistanceTo(room, _p, p) == 1)
                                    {
                                        ai.Number[Name] = 9;
                                        card.AddSubCard(id);
                                        use.Card = card;
                                        use.To.Add(_p);
                                        return;
                                    }
                                }
                            }
                        }

                        foreach (Player p in targets)
                        {
                            foreach (Player enemy in ai.GetEnemies(player))
                            {
                                if (!enemy.IsNude() && RoomLogic.DistanceTo(room, p, enemy) == 1)
                                {
                                    ai.Number[Name] = 9;
                                    card.AddSubCard(id);
                                    use.Card = card;
                                    use.To.Add(p);
                                    return;
                                }
                            }
                        }
                    }
                    else if (trick.Name == IronChain.ClassName)
                    {
                        foreach (Player p in targets)
                        {
                            if (p.Chained)
                            {
                                ai.Number[Name] = 9;
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }

                        if (enemies.Count > 1)
                        {
                            foreach (Player p in enemies)
                            {
                                if (!p.Chained)
                                {
                                    ai.Number[Name] = 9;
                                    card.AddSubCard(id);
                                    use.Card = card;
                                    use.To.Add(targets[0]);
                                    return;
                                }
                            }
                        }
                    }
                    else if (trick.Name == FireAttack.ClassName)
                    {
                        foreach (Player p in targets)
                        {
                            if (p.HandcardNum > 4)
                            {
                                ai.Number[Name] = 9;
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }
            }

            List<WrappedCard> slashes = new List<WrappedCard>();
            foreach (int id in player.HandCards)
            {
                WrappedCard slash = room.GetCard(id);
                if (Engine.GetFunctionCard(slash.Name) is Slash)
                    slashes.Add(slash);
            }
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (WrappedCard slash in slashes)
            {
                foreach (Player p in targets)
                {
                    foreach (Player enemy in ai.GetEnemies(player))
                    {
                        if (RoomLogic.InMyAttackRange(room, p, enemy, slash) && RoomLogic.IsProhibited(room, p, enemy, slash) == null)
                        {
                            ScoreStruct score = new ScoreStruct
                            {
                                Players = new List<Player>{p},
                                Card = slash,
                            };

                            DamageStruct damage = new DamageStruct(slash, p, enemy);
                            if (ai.HasArmorEffect(enemy, Vine.ClassName) && slash.Name == Slash.ClassName && p.HasWeapon(Fan.ClassName))
                            {
                                WrappedCard fan = new WrappedCard(FireSlash.ClassName);
                                fan.AddSubCard(slash);
                                fan = RoomLogic.ParseUseCard(room, fan);
                                damage.Card = fan;
                            }

                            if (damage.Card.Name == FireSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Fire;
                            else if (damage.Card.Name == ThunderSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Thunder;

                            ScoreStruct damage_score = ai.GetDamageScore(damage);
                            if (damage_score.Score > 0)
                            {
                                ScoreStruct effect = ai.SlashIsEffective(damage.Card, p, enemy);
                                if (effect.Score > 0)
                                {
                                    score.Score = effect.Score;
                                    if (effect.Rate > 0)
                                    {
                                        score.Score += Math.Min(1, effect.Rate) * damage_score.Score;
                                        scores.Add(score);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    ai.Number[Name] = 4;
                    card.AddSubCard(scores[0].Card);
                    use.To = new List<Player>(scores[0].Players);
                    use.Card = card;
                    return;
                }
            }

            foreach (int id in player.HandCards)
            {
                WrappedCard c = room.GetCard(id);
                if (!(Engine.GetFunctionCard(c.Name) is EquipCard))
                    cards.Add(id);
            }

            KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(cards, ai.FriendNoSelf, Player.Place.PlaceHand);
            if (pair.Key != null)
            {
                ai.Number[Name] = 0;
                card.AddSubCard(pair.Value);
                use.To.Add(pair.Key);
                use.Card = card;
                return;
            }

            if (ai.GetOverflow(player) > 0)
            {
                List<int> ids = new List<int>(player.HandCards);
                ai.SortByUseValue(ref ids);
                ai.Number[Name] = 0;
                card.AddSubCard(ids[0]);
                use.To.Add(targets[0]);
                use.Card = card;
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name]; ;
        }
    }

    public class FenglueAI : SkillEvent
    {
        public FenglueAI() : base("fenglue") { }

        private int GetPindianCard(TrustedAI ai, Player p)
        {
            if (ai.Self.Camp == p.Camp)
            {
                int min = 13;
                List<int> ids = new List<int>(p.HandCards);
                foreach (int id in ids)
                {
                    int num = ai.Room.GetCard(id).Number;
                    if (num < min)
                        min = num;
                }
                return min;
            }
            else
            {
                if (ai.GetKnownCards(p).Count == p.HandcardNum)
                {
                    WrappedCard max = ai.GetMaxCard(p);
                    if (max != null) return max.Number;
                }

                return 10;
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            WrappedCard max_card = ai.GetMaxCard(player) ;
            int number = 0;
            if (max_card != null)
                number = max_card.Number;


                foreach (Player p in ai.FriendNoSelf)
                {
                    if (!p.IsKongcheng() && (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName) || RoomLogic.PlayerContainsTrick(room, p, SupplyShortage.ClassName))
                        && number > GetPindianCard(ai, p))
                    {
                        return new List<Player> { p };
                    }
                }

            foreach (Player p in ai.FriendNoSelf)
            {
                if (!p.IsKongcheng() && p.HasArmor(SilverLion.ClassName) && number > GetPindianCard(ai, p))
                {
                    return new List<Player> { p };
                }
            }
            
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            foreach (Player enemy in enemies)
            {
                if (enemy.HandcardNum > 1 && number > GetPindianCard(ai, enemy) && (!enemy.IsWounded() || !enemy.HasArmor(SilverLion.ClassName)))
                {
                    return new List<Player> { enemy };
                }
            }

            if (ai.GetOverflow(player) > 0)
            {
                foreach (Player enemy in enemies)
                {
                    if (!enemy.IsKongcheng() && number > GetPindianCard(ai, enemy) && (!enemy.IsWounded() || !enemy.HasArmor(SilverLion.ClassName)))
                    {
                        return new List<Player> { enemy };
                    }
                }
            }

            return new List<Player>();
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.Room.GetTag(Name) is Player target && ai.IsFriend(target))
                return true;

            return false;
        }
        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> player)
        {
            Room room = ai.Room;
            if (ai.Self == requestor)
            {
                List<int> ids = new List<int>(requestor.HandCards);
                ai.SortByUseValue(ref ids, false);
                if (ai.IsFriend(player[0]))
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Number > GetPindianCard(ai, player[0]) && card.Number > 6)
                            return card;
                    }
                }
                else
                {
                    if (ai.GetKnownCards(player[0]).Count == player[0].HandcardNum)
                    {
                        foreach (int id in ids)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Number > GetPindianCard(ai, player[0]) && card.Number > GetPindianCard(ai, player[0]))
                                return card;
                        }
                    }
                }                    
            }
            else
            {
                if (ai.IsFriend(requestor))
                    return ai.GetMinCard(ai.Self);
            }

            return ai.GetMaxCard(ai.Self);
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (RoomLogic.PlayerHasSkill(ai.Room, player, Name) && card.Number > 9)
            {
                double basic = (card.Number - 9) / 3, adjust = 0.5;
                for (int i = 10; i <= card.Number; i++)
                    adjust *= i;

                return basic + adjust;
            }

            return 0;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            if (pattern == ".")
            {
                List<int> ids = new List<int>(player.HandCards);
                ai.SortByKeepValue(ref ids, false);
                return new List<int> { ids[0] };
            }
            else if (pattern == "..")
            {
                List<int> ids = player.GetCards("he");
                ai.SortByKeepValue(ref ids, false);
                return new List<int> { ids[0] };
            }
            else
            {
                List<int> ids = player.GetCards("e");
                ai.SortByKeepValue(ref ids, false);
                return new List<int> { ids[0] };
            }
        }
    }

    public class MoushiAI : SkillEvent
    {
        public MoushiAI() : base("moushi") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed(MoushiCard.ClassName))
            {
                return new List<WrappedCard> { new WrappedCard(MoushiCard.ClassName) { Skill = Name, Mute = true } };
            }

            return new List<WrappedCard>();
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct();
            if (damage.From != null && damage.From.GetMark(Name) > 0 && damage.From.Phase == Player.PlayerPhase.Play)
            {
                List<Player> damages = ai.Room.ContainsTag("moushi_targets") ? (List<Player>)ai.Room.GetTag("moushi_targets") : new List<Player>();
                if (!damages.Contains(damage.To))
                    score.Score = ai.IsFriend(damage.From) ? 1.5 : -1.5;
            }

            return score;
        }
    }

    public class MoushiCardAI : UseCard
    {
        public MoushiCardAI() : base(MoushiCard.ClassName) {}

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 4.5;
            KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer();
            if (pair.Key != null)
            {
                card.AddSubCard(pair.Value);
                use.Card = card;
                use.To.Add(pair.Key);
                return;
            }

            if (ai.GetOverflow(player) > 0)
            {
                ai.Number[Name] = 0;
                List<int> ids = player.GetCards("h");
                ai.SortByUseValue(ref ids);
                card.AddSubCard(ids[0]);
                use.Card = card;
                use.To.Add(ai.FriendNoSelf[0]);
            }
        }
    }

    public class ShibeiAI : SkillEvent
    {
        public ShibeiAI() : base("shibei") { }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct();
            if (ai.HasSkill(Name, damage.To))
            {
                if (damage.To.GetMark(Name) == 0)
                    score.Score = ai.IsFriend(damage.To) ? 2 : -2;
                else
                    score.Score = ai.IsFriend(damage.To) ? -4 : 4;
            }

            return score;
        }
    }

    public class BifaAI : SkillEvent
    {
        public BifaAI() : base("bifa") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<int> ids = new List<int>(player.HandCards);
            ai.SortByKeepValue(ref ids, false);
            List<Player> targets = ai.GetEnemies(player);
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
            {
                if (p.GetPile(Name).Count == 0)
                {
                    foreach (int id in ids)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                        if (fcard is EquipCard || fcard is Slash || fcard is Jink || fcard is Lightning)
                        {
                            WrappedCard card = new WrappedCard(BifaCard.ClassName) { Skill = Name };
                            card.AddSubCard(id);
                            use.Card = card;
                            use.To.Add(p);
                            return use;
                        }
                    }
                }
            }

            return use;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name == Peach.ClassName || (player.Hp == 1 && card.Name == Analeptic.ClassName) || !Engine.MatchExpPattern(room, pattern, player, card))
                    continue;

                ids.Add(id);
            }

            if (ids.Count > 0)
            {
                ai.SortByUseValue(ref ids, false);
                return new List<int> { ids[0] };
            }

            return new List<int>();
        }
    }

    public class SongciAI : SkillEvent
    {
        public SongciAI() : base("songci") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            foreach (Player p in ai.Room.GetAlivePlayers())
                if (p.GetMark("songci_" + player.Name) == 0 && p.HandcardNum != p.Hp)
                    return new List<WrappedCard> { new WrappedCard(SongciCard.ClassName) { Skill = Name, Mute = true } };

            return new List<WrappedCard>();
        }
    }

    public class SongciCardAI : UseCard
    {
        public SongciCardAI() : base(SongciCard.ClassName) { }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            foreach (Player p in ai.Room.GetAlivePlayers())
            {
                if (p.GetMark("songci_" + player.Name) == 0 && p.HandcardNum != p.Hp)
                {
                    if (ai.IsFriend(p) && p.HandcardNum < p.Hp)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                    else if (ai.IsEnemy(p) && p.HandcardNum > p.Hp)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class JigongAI : SkillEvent
    {
        public JigongAI() : base("jigong") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.HandcardNum <= 1) return true;
            Room room = ai.Room;
            if (ai.GetEnemies(player).Count > 1 && ai.GetKnownCardsNums(SavageAssault.ClassName, "he", player) + ai.GetKnownCardsNums(ArcheryAttack.ClassName, "he", player) > 0)
                    return true;

            foreach (Player enemy in ai.GetEnemies(player))
                if (ai.IsWeak(enemy) && RoomLogic.CanSlash(room, player, enemy))
                    return true;

            return false;                        
        }
    }

    public class ShifeiAI : SkillEvent
    {
        public ShifeiAI() : base("shifei") { }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            Room room = ai.Room;
            List<WrappedCard> result = new List<WrappedCard>();
            string response_id = room.GetRoomState().GetCurrentResponseID().ToString();
            if (pattern == "Jink" && !player.HasFlag("shifei_" + response_id) && room.Current != null)
            {
                WrappedCard shifei = new WrappedCard(ShifeiCard.ClassName) { Skill = Name };
                if (ai.IsFriend(room.Current))
                {
                    WrappedCard jink = new WrappedCard(Jink.ClassName);
                    jink.UserString = RoomLogic.CardToString(room, shifei);
                    result.Add(jink);
                    return result;
                }

                int max = room.Current.HandcardNum + 1;
                foreach (Player p in room.GetOtherPlayers(room.Current))
                    if (p.HandcardNum > max)
                        max = p.HandcardNum;

                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HandcardNum == max && ai.IsEnemy(p))
                    {
                        WrappedCard jink = new WrappedCard(Jink.ClassName);
                        jink.UserString = RoomLogic.CardToString(room, shifei);
                        result.Add(jink);
                        return result;
                    }
                }

                Player most = null;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HandcardNum == max && ai.IsFriend(p))
                    {
                        most = p;
                        List<int> ids = p.GetEquips();
                        if (ids.Count > 0)
                        {
                            foreach (int id in ids)
                            {
                                if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                {
                                    WrappedCard jink = new WrappedCard(Jink.ClassName);
                                    jink.UserString = RoomLogic.CardToString(room, shifei);
                                    result.Add(jink);
                                    return result;
                                }
                            }
                        }
                    }
                }
                if (most != null && ai.IsWeak())
                {
                    WrappedCard jink = new WrappedCard(Jink.ClassName);
                    jink.UserString = RoomLogic.CardToString(room, shifei);
                    result.Add(jink);
                }
            }

            return result;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 8;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player to)
        {
            Player player = ai.Self;
            Room room = ai.Room;
            double value = 0;
            if ((card.Name.Contains(Slash.ClassName) || card.Name == ArcheryAttack.ClassName)
                && !ai.IsCancelTarget(card, to, player) || ai.IsCardEffect(card, to, ai.Self) && room.Current != null)
            {
                if (ai.IsFriend(to, room.Current))
                    value++;

                int max = room.Current.HandcardNum + 1;
                foreach (Player p in room.GetOtherPlayers(room.Current))
                    if (p.HandcardNum > max)
                        max = p.HandcardNum;

                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HandcardNum == max && ai.IsEnemy(to, p))
                    {
                        value += 1;
                        break;
                    }
                }
            }

            return value;
        }
    }

    public class JushouJXAI : SkillEvent
    {
        public JushouJXAI() : base("jushou_jx")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, player);
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is DefensiveHorse || fcard is Armor) && RoomLogic.CanPutEquip(player, card) && ai.GetSameEquip(card, player) == null)
                    return new List<int> { card.Id };
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is Weapon || fcard is OffensiveHorse || fcard is Treasure) && RoomLogic.CanPutEquip(player, card) && ai.GetSameEquip(card, player) == null)
                    return new List<int> { card.Id };
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is Armor && player.GetArmor() && RoomLogic.CanPutEquip(player, card) && ai.GetKeepValue(player.Armor.Key, player) < ai.GetUseValue(card.Id, player))
                    return new List<int> { card.Id };
            }

            return ai.AskForDiscard(player.GetCards("h"), string.Empty, 1, 1, false);
        }
    }

    public class JieweiAI : SkillEvent
    {
        public JieweiAI() : base("jiewei") { }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            if (ai.HasSkill(Name, player) && (place == Player.Place.PlaceEquip || room.GetCardPlace(id) == Player.Place.PlaceEquip))
            {
                WrappedCard nulli = new WrappedCard(Nullification.ClassName) { Skill = Name };
                nulli.AddSubCard(id);
                return nulli;
            }

            return null;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            Room room = ai.Room;
            double value = ai.GetKeepValue(card.GetEffectiveId(), player);
            if (value < 0) return -value;

            WrappedCard equip = room.GetCard(card.GetEffectiveId());
            FunctionCard fcard = Engine.GetFunctionCard(equip.Name);
            if (ai.FindSameEquipCards(equip).Count > 0) return 1;

            if (fcard is OffensiveHorse) return -0.5;
            if (fcard is Armor || fcard is DefensiveHorse) return -1.5;
            return -1;
        }
    }

    public class JianyingAI : SkillEvent
    {
        public JianyingAI() : base("jianying") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            WrappedCard card = use.Card;
            if (!(Engine.GetFunctionCard(card.Name) is SkillCard) && player.GetMark("JianyingNumber") > 0)
            {
                if ((card.Name == IronChain.ClassName || card.Name == GDFighttogether.ClassName) && use.To.Count == 0) return 0;
                int suit = player.GetMark("JianyingSuit"), number = player.GetMark("JianyingNumber");
                if ((suit > 0 && (int)card.Suit + 1 == suit) || (number > 0 && card.Number == number))
                    return 3.5;
            }

            return 0;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class DanjiAI : SkillEvent
    {
        public DanjiAI() : base("danji") { }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.To.GetMark(Name) == 0 && damage.To.HandcardNum > damage.To.MaxHp - 1 && damage.To.GetLostHp() == 0)
            {
                if (damage.Damage == 1)
                    score.Score = ai.IsFriend(damage.To) ? 4 : -4;
            }

            return score;
        }
    }

    public class DuanliangJXAI : SkillEvent
    {
        public DuanliangJXAI() : base("duanliang_jx")
        {
        }
        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            string pattern = "BasicCard,EquipCard|black";
            if (!player.HasFlag(Name) && card != null && Engine.MatchExpPattern(room, pattern, player, card))
            {
                WrappedCard shortage = new WrappedCard(SupplyShortage.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                shortage.AddSubCard(card);
                shortage = RoomLogic.ParseUseCard(room, shortage);
                return shortage;
            }

            return null;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>(player.HandCards);
            ids.AddRange(player.GetEquips());
            ids.AddRange(player.GetHandPile());
            ai.SortByUseValue(ref ids);
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (WrappedCard.IsBlack(card.Suit) && (fcard is BasicCard || fcard is EquipCard))
                {
                    WrappedCard shortage = new WrappedCard(SupplyShortage.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    shortage.AddSubCard(card);
                    shortage = RoomLogic.ParseUseCard(room, shortage);
                    return new List<WrappedCard> { shortage };
                }
            }

            return null;
        }
    }

    public class ShefuAI : SkillEvent
    {
        public ShefuAI() : base("shefu") { }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return base.OnExchange(ai, player, pattern, min, max, pile);
        }
    }

    public class BenyuAI : SkillEvent
    {
        public BenyuAI() : base("benyu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            if (room.GetTag(Name) is DamageStruct damage)
            {
                double value = 0;
                DamageStruct _damage = new DamageStruct(Name, player, damage.From);
                ScoreStruct score = ai.GetDamageScore(_damage);

                List<int> ids = new List<int>(), discount = new List<int>();
                foreach (int id in player.GetCards("h"))
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        ids.Add(id);

                ai.SortByKeepValue(ref ids, false);
                for (int i = 0; i <= damage.From.HandcardNum; i++)
                {
                    value += ai.GetKeepValue(ids[i], player);
                    discount.Add(ids[i]);
                }
                if (value > 0) value /= 2;

                if (value < score.Score)
                {
                    WrappedCard card = new WrappedCard(BenyuCard.ClassName) { Skill = Name };
                    card.AddSubCards(discount);
                    use.Card = card;
                }
            }

            return use;
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (damage.From != null && damage.To != null && ai.HasSkill(Name, damage.To))
            {
                Room room = ai.Room;
                int from = damage.From.HandcardNum;
                int to = damage.To.HandcardNum;
                if (damage.Card != null)
                {
                    foreach (int id in damage.Card.SubCards)
                        if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                            from--;
                }

                if (from > to)
                {
                    score.Score = Math.Min(5, from - to) * 1.5;
                    if (ai.WillSkipPlayPhase(damage.To)) score.Score /= 1.5;
                    if (ai.IsEnemy(damage.To))
                        score.Score = -score.Score;
                    else
                        score.Score -= 1.5;
                }
                else
                {
                    if (ai.IsFriend(damage.From) && !ai.IsFriend(damage.From, damage.To))
                    {
                        if (to - from <= 2 || ai.IsWeak(damage.From))
                        {
                            score.Score = -3;
                        }
                    }
                }

                if (damage.Damage > 1) score.Score /= 1.5;
                if (damage.Damage >= damage.To.Hp) score.Score /= 2;
            }

            return score;
        }
    }

    public class JiemingJXAI : SkillEvent
    {
        public JiemingJXAI() : base("jieming_jx")
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
                }
            }
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            double value = 0;
            if (ai.HasSkill(Name, damage.To))
            {
                Room room = ai.Room;
                Dictionary<Player, double> point = new Dictionary<Player, double>();
                foreach (Player p in ai.GetFriends(damage.To))
                {
                    int count = p.HandcardNum;
                    if (damage.Card != null)
                    {
                        foreach (int id in damage.Card.SubCards)
                            if (room.GetCardOwner(id) == damage.From && room.GetCardPlace(id) == Player.Place.PlaceHand)
                                count--;
                    }
                    if (p.MaxHp - count > 0)
                    {
                        double v = (p.MaxHp - count) * 1.2;
                        if (ai.HasSkill(TrustedAI.CardneedSkill, p)) v *= 1.3;
                        if (ai.Room.Current == p)
                            v += 2;
                        if (ai.WillSkipPlayPhase(p))
                            v /= 1.5;

                        point.Add(p, v);
                    }
                }
                List<Player> targets = new List<Player>(point.Keys);
                if (targets.Count > 0)
                {
                    targets.Sort((x, y) => { return point[x] > point[y] ? -1 : 1; });
                    for (int i = 0; i < Math.Min(targets.Count, damage.Damage); i++)
                        value += point[targets[i]];

                    if (damage.Damage >= damage.To.Hp) value /= 2;
                    if (ai.IsEnemy(targets[0]))
                        value = -value;
                    else
                        value -= 1.5;
                }
            }

            ScoreStruct score = new ScoreStruct
            {
                Score = value
            };
            return score;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Dictionary<Player, double> point = new Dictionary<Player, double>();
            foreach (Player p in ai.GetFriends(player))
            {
                if (p.MaxHp - p.HandcardNum > 0)
                {
                    double value = Math.Min(5, p.MaxHp - p.HandcardNum) * 1.2;
                    if (ai.HasSkill(TrustedAI.CardneedSkill, p)) value *= 1.2;
                    if (ai.Room.Current == p)
                        value += 2;

                    point.Add(p, value);
                }
            }
            List<Player> targets = new List<Player>(point.Keys);
            if (targets.Count > 0)
            {
                targets.Sort((x, y) => { return point[x] > point[y] ? -1 : 1; });
                return new List<Player> { targets[0] };
            }

            if (ai.NeedShowImmediately())
                return new List<Player> { player };

            return new List<Player>();
        }
    }

    public class ChoulueAI : SkillEvent
    {
        public ChoulueAI() : base("choulue") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            return base.OnPlayerChosen(ai, player, target, min, max);
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return base.OnExchange(ai, player, pattern, min, max, pile);
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }
    }

    public class PoluAI : SkillEvent
    {
        public PoluAI() : base("polu") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class YuanhuAI : SkillEvent
    {
        public YuanhuAI() : base("yuanhu") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }
    }

    public class ZhenjunAI : SkillEvent
    {
        public ZhenjunAI() : base("zhenjun") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            return base.OnPlayerChosen(ai, player, target, min, max);
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return base.OnExchange(ai, player, pattern, min, max, pile);
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            return base.OnDiscard(ai, player, ids, min, max, option);
        }
    }

    public class ShenduanAI : SkillEvent
    {
        public ShenduanAI() : base("shenduan") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }
    }

    public class YonglueAI : SkillEvent
    {
        public YonglueAI() : base("yonglue")
        {}

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return base.OnSkillInvoke(ai, player, data);
        }
    }

    public class QiceJXAI : SkillEvent
    {
        public QiceJXAI() : base("qice_jx") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return base.GetTurnUse(ai, player);
        }
    }
}