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
                new XueyiAI(),
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
                new HengjiangJXAI(),
                new QigongAI(),
                new LiehouAI(),
            };
            use_cards = new List<UseCard>
            {
                new ShicaiCardAI(),
                new ZhezhuCardAI(),
                new YuanlueCardAI(),
                new MoushiCardAI(),
                new SongciCardAI(),
                new LiehouCardAI(),
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

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && room.GetTag(Name) is CardUseStruct use)
            {
                string[] choices = choice.Split(':');
                Player target = room.FindPlayer(choices[2]);
                if (target != player && ai.GetPlayerTendency(target) != "unknown")
                {
                    if (use.Card.Name == ArcheryAttack.ClassName && ai.HasSkill("leiji|leiji_jx", target)) return;
                    ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use)
            {
                if (use.Card.Name == ArcheryAttack.ClassName)
                    foreach (Player p in targets)
                        if (!ai.IsFriend(p) && ai.NotSlashJiaozhu(use.From, p, use.Card)) return new List<Player> { p };

                ai.SortByDefense(ref targets, false);
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p) && use.Card.Name == ArcheryAttack.ClassName && ai.JiaozhuneedSlash(use.From, p, use.Card)) continue;
                    if (ai.IsCardEffect(use.Card, p, use.From) && !ai.IsCancelTarget(use.Card, p, use.From))
                    {
                        DamageStruct damage = new DamageStruct(use.Card, use.From, p, 1 + use.ExDamage);
                        ScoreStruct score = ai.GetDamageScore(damage);
                        score.Players = new List<Player> { p };
                        scores.Add(score);
                    }
                }

                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score < y.Score ? -1 : 1; });
                    if (scores[0].Score < 0)
                        return scores[0].Players;
                }
            }

            return new List<Player>();
        }
    }

    public class XueyiAI : SkillEvent
    {
        public XueyiAI() : base("xueyi") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillSkipPlayPhase(player) && player.HandcardNum < 5)
            {
                return true;
            }

            return false;
        }
    }

    public class ShicaiAI : SkillEvent
    {
        public ShicaiAI() : base("shicai") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark(Name) == 0 && !player.IsNude())
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
            List<int> ids = player.GetPile("#shushou");
            List<int> hands = player.GetCards("h");
            hands.RemoveAll(t => !ids.Contains(t));
            if (ai.GetOverflow(player) > player.HandcardNum - hands.Count)
                ids.RemoveAll(t => !hands.Contains(t));

            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());

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
            if (ai.HasSkill(Name, player) && player.GetMark("@gd_liang") > 0 && !isUse)
                if (card.Name == Jink.ClassName) return 2;

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

                if (card.Name.Contains(Slash.ClassName))
                {
                    ai.FindSlashandTarget(ref use, player, new List<WrappedCard> { card });
                    if (use.Card != null)
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
            foreach (int id in player.GetCards("h"))
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
            foreach (int id in player.GetCards("h"))
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

            foreach (int id in player.GetCards("h"))
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
            foreach (int id in player.GetCards("h"))
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

            foreach (int id in player.GetCards("h"))
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
                List<int> ids= player.GetCards("h");
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
                List<int> ids = p.GetCards("h");
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
                    if (RoomLogic.CanBePindianBy(room, p, player) && (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName) || RoomLogic.PlayerContainsTrick(room, p, SupplyShortage.ClassName))
                        && number > GetPindianCard(ai, p))
                    {
                        return new List<Player> { p };
                    }
                }

            foreach (Player p in ai.FriendNoSelf)
            {
                if (RoomLogic.CanBePindianBy(room, p, player) && p.HasArmor(SilverLion.ClassName) && number > GetPindianCard(ai, p))
                {
                    return new List<Player> { p };
                }
            }
            
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            foreach (Player enemy in enemies)
            {
                if (RoomLogic.CanBePindianBy(room, enemy, player) && enemy.HandcardNum > 1 && number > GetPindianCard(ai, enemy) && (!enemy.IsWounded() || !enemy.HasArmor(SilverLion.ClassName)))
                {
                    return new List<Player> { enemy };
                }
            }

            if (ai.GetOverflow(player) > 0)
            {
                foreach (Player enemy in enemies)
                {
                    if (RoomLogic.CanBePindianBy(room, enemy, player) && number > GetPindianCard(ai, enemy) && (!enemy.IsWounded() || !enemy.HasArmor(SilverLion.ClassName)))
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
                List<int> ids = requestor.GetCards("h");
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
                List<int> ids= player.GetCards("h");
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
            List<int> ids= player.GetCards("h");
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
                    WrappedCard jink = new WrappedCard(Jink.ClassName)
                    {
                        UserString = RoomLogic.CardToString(room, shifei)
                    };
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
                        WrappedCard jink = new WrappedCard(Jink.ClassName)
                        {
                            UserString = RoomLogic.CardToString(room, shifei)
                        };
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
                                    WrappedCard jink = new WrappedCard(Jink.ClassName)
                                    {
                                        UserString = RoomLogic.CardToString(room, shifei)
                                    };
                                    result.Add(jink);
                                    return result;
                                }
                            }
                        }
                    }
                }
                if (most != null && ai.IsWeak())
                {
                    WrappedCard jink = new WrappedCard(Jink.ClassName)
                    {
                        UserString = RoomLogic.CardToString(room, shifei)
                    };
                    result.Add(jink);
                }
            }

            return result;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 8;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            Player player = ai.Self;
            Room room = ai.Room;
            double value = 0;
            if ((card.Name.Contains(Slash.ClassName) || card.Name == ArcheryAttack.ClassName) && room.Current != null
                && !ai.IsCancelTarget(card, to, player) && ai.IsCardEffect(card, to, ai.Self))
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
            if (player.GetMark("@rob") > 0)
            {
                Player gn = RoomLogic.FindPlayerBySkillName(ai.Room, "jieying_gn");
                if (gn != null && !ai.IsFriend(gn)) return false;
            }

            return true;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, player);
            WrappedCard jushou = new WrappedCard(JushouCard.ClassName)
            {
                Mute = true,
                Skill = Name
            };
            CardUseStruct use = new CardUseStruct(jushou, player, new List<Player>());
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is DefensiveHorse || fcard is Armor || fcard is SpecialEquip) && RoomLogic.CanPutEquip(player, card) && ai.GetSameEquip(card, player) == null)
                {
                    jushou.AddSubCard(card);
                    return use;
                }
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if ((fcard is Weapon || fcard is OffensiveHorse || fcard is Treasure) && RoomLogic.CanPutEquip(player, card) && ai.GetSameEquip(card, player) == null)
                {
                    jushou.AddSubCard(card);
                    return use;
                }
            }
            foreach (WrappedCard card in cards)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is Armor && player.GetArmor() && RoomLogic.CanPutEquip(player, card) && ai.GetKeepValue(player.Armor.Key, player) < ai.GetUseValue(card.Id, player))
                {
                    jushou.AddSubCard(card);
                    return use;
                }
            }

            List<int> subs = ai.AskForDiscard(player.GetCards("h"), string.Empty, 1, 1, false);
            jushou.AddSubCards(subs);
            return use;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return "use";
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
            if (ai.FindSameEquipCards(equip, false, false).Count > 0) return 1;

            if (fcard is OffensiveHorse) return -0.5;
            if (fcard is Armor || fcard is DefensiveHorse) return -1.5;
            return -1;
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
                    ai.Room.Debug("解围AI出错");
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

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            List<int> to_discard = new List<int>();

            List<int> cards = player.GetCards("he");
            ai.SortByKeepValue(ref cards, false);
            Room room = ai.Room;
            Player stealer = null;

            foreach (Player ap in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill("tuxi|tuxi_jx", ap) && ai.IsEnemy(ap))
                {
                    stealer = ap;
                    break;
                }
            }

            int card = -1;
            for (int i = 0; i < cards.Count; i++)
            {
                if (ai.IsCard(cards[i], Peach.ClassName, player))
                {
                    if (stealer != null && player.HandcardNum <= 2 && player.Hp > 2 && !RoomLogic.PlayerContainsTrick(room, stealer, SupplyShortage.ClassName))
                    {
                        card = cards[i];
                        break;
                    }
                    bool to_discard_peach = true;
                    foreach (Player fd in ai.GetFriends(player))
                    {
                        if (fd.Hp <= 2 && (!ai.HasSkill("niepan", fd) || fd.GetMark("@nirvana") == 0))
                        {
                            to_discard_peach = false;
                            break;
                        }
                    }
                    if (to_discard_peach)
                    {
                        card = cards[i];
                        break;
                    }
                }
                else
                {
                    card = cards[i];
                    break;
                }
            }

            if (card == -1)
                return to_discard;

            to_discard.Add(card);
            foreach (Player friend in ai.GetFriends(player))
            {
                if (friend.JudgingArea.Count > 0 && QiaobianAI.CardForQiaobian(ai, friend).Key >= 0)
                    return to_discard;
            }

            foreach (Player friend in ai.FriendNoSelf)
            {
                if (friend.HasEquip() && ai.HasSkill(TrustedAI.LoseEquipSkill, friend) && QiaobianAI.CardForQiaobian(ai, friend).Key >= 0)
                    return to_discard;
            }
            
            foreach (Player p in ai.GetEnemies(player))
            {
                if (QiaobianAI.CardForQiaobian(ai, p).Key > 0)
                    return to_discard;
            }

            return new List<int>();
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && Engine.GetFunctionCard(card.Name) is EquipCard)
                return 1.5;

            return 0;
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
            List<int> ids= player.GetCards("h");
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
        public ShefuAI() : base("shefu") { key = new List<string> { "cardExchange:shefu" }; }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                string[] strs = str.Split(':');
                if (!string.IsNullOrEmpty(strs[2]))
                {
                    Room room = ai.Room;
                    if (room.GetTag("shefu_data") is CardUseStruct use)
                    {
                        if (ai.GetPlayerTendency(use.From) != "unknown")
                            ai.UpdatePlayerRelation(player, use.From, false);
                    }
                }
            }
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<int> ids = player.GetCards("h");
            ai.SortByKeepValue(ref ids);
            int id = -1;
            foreach (int i in ids)
            {
                if (!ai.IsCard(i, Peach.ClassName, player) && !ai.IsCard(i, Analeptic.ClassName, player))
                {
                    id = i;
                    break;
                }
            }
            if (id == -1) return use;

            if (!player.ContainsTag(string.Format("shefu_{0}", Slash.ClassName)))
            {
                WrappedCard shefu = new WrappedCard(ShefuCard.ClassName) { Skill = Name, UserString = Slash.ClassName };
                shefu.AddSubCard(id);
                use.Card = shefu;
            }
            else if (!player.ContainsTag(string.Format("shefu_{0}", Peach.ClassName)))
            {
                WrappedCard shefu = new WrappedCard(ShefuCard.ClassName) { Skill = Name, UserString = Peach.ClassName };
                shefu.AddSubCard(id);
                use.Card = shefu;
            }
            else if (!player.ContainsTag(string.Format("shefu_{0}", Jink.ClassName)))
            {
                WrappedCard shefu = new WrappedCard(ShefuCard.ClassName) { Skill = Name, UserString = Jink.ClassName };
                shefu.AddSubCard(id);
                use.Card = shefu;
            }
            else if (!player.ContainsTag(string.Format("shefu_{0}", Nullification.ClassName)))
            {
                WrappedCard shefu = new WrappedCard(ShefuCard.ClassName) { Skill = Name, UserString = Nullification.ClassName };
                shefu.AddSubCard(id);
                use.Card = shefu;
            }
            else if (room.AvailableFunctionCards.Contains(Reinforcement.Instance) && !player.ContainsTag(string.Format("shefu_{0}", Reinforcement.ClassName)))
            {
                WrappedCard shefu = new WrappedCard(ShefuCard.ClassName) { Skill = Name, UserString = Reinforcement.ClassName };
                shefu.AddSubCard(id);
                use.Card = shefu;
            }

            return use;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            int id = int.Parse(pattern.Split('|')[0]);
            
            if (room.GetTag("shefu_data") is CardUseStruct use)
            {
                if (ai is StupidAI _ai && player.GetRoleEnum() == Player.PlayerRole.Renegade)
                {
                    if (use.Card.Name == Peach.ClassName && use.To[0].GetRoleEnum() == Player.PlayerRole.Lord && use.To[0].HasFlag("Global_Dying") && room.AliveCount() > 2)
                        return new List<int>();

                    if (use.Card.Name.Contains(Slash.ClassName))
                    {
                        foreach (Player p in use.To)
                        {
                            if (!ai.IsCancelTarget(use.Card, p, use.From) && ai.IsCardEffect(use.Card, p, use.From))
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, p, 1 + use.Drank + use.ExDamage);
                                if (use.Card.Name == FireSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Fire;
                                else if (damage.Card.Name == ThunderSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Thunder;

                                ScoreStruct score = ai.GetDamageScore(damage);
                                if (score.Score < -10)
                                    return new List<int> { id };
                            }
                        }
                    }
                }

                if (ai.IsEnemy(use.From))
                {
                    if (use.Card.Name.Contains(Slash.ClassName))
                    {
                        foreach (Player p in use.To)
                        {
                            if (ai.IsFriend(p) && !ai.IsCancelTarget(use.Card, p, use.From) && ai.IsCardEffect(use.Card, p, use.From))
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, p, 1 + use.Drank + use.ExDamage);
                                if (use.Card.Name == FireSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Fire;
                                else if (damage.Card.Name == ThunderSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Thunder;

                                ScoreStruct score = ai.GetDamageScore(damage);
                                if ((ai.IsWeak(p) && score.Score < 0) || score.Score < -5)
                                    return new List<int> { id };
                            }
                        }

                        return new List<int>();
                    }
                    else if (use.Card.Name == Peach.ClassName)
                    {
                        foreach (Player p in use.To)
                            if (ai.IsFriend(p)) return new List<int>();
                    }

                    return new List<int> { id };
                }
            }
            else if (room.GetTag("shefu_data") is CardResponseStruct resp && ai.IsEnemy(resp.From))
            {
                return new List<int> { id };
            }

            return new List<int>();
        }
    }

    public class BenyuAI : SkillEvent
    {
        public BenyuAI() : base("benyu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> all, int min, int max, bool option)
        {
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
                    return discount;
                }
            }

            return new List<int>();
        }
        /*
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
        */
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
            key = new List<string> { "playerChosen:jieming_jx" };
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
        public ChoulueAI() : base("choulue") { key = new List<string> { "cardExchange:choulue" }; }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                string[] strs = str.Split(':');
                if (!string.IsNullOrEmpty(strs[2]))
                {
                    if (ai.GetPlayerTendency(ai.Room.Current) != "unknown")
                        ai.UpdatePlayerRelation(player, ai.Room.Current, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            List<Player> targets = ai.FriendNoSelf;
            if (targets.Count > 0)
            {
                ai.SortByDefense(ref targets);
                foreach (Player p in targets)
                {
                    if (player.HasEquip())
                    {
                        List<int> ids = player.GetCards("e");
                        foreach (int id in ids)
                        {
                            if (ai.GetKeepValue(id, p) < 0)
                            {
                                return new List<Player> { p };
                            }
                        }
                    }
                }
                foreach (Player p in targets)
                {
                    if (ai.GetOverflow(p) > 0)
                        return new List<Player> { p };
                }
                foreach (Player p in targets)
                {
                    if (ai.WillSkipPlayPhase(p) && !p.IsNude())
                        return new List<Player> { p };
                }

                if (player.ContainsTag(Name) && player.GetTag(Name) is string card_name && !string.IsNullOrEmpty(card_name))
                {
                    foreach (Player p in targets)
                    {
                        if (!p.IsNude() && !ai.IsWeak(p))
                            return new List<Player> { p };
                    }
                }
            }
            else
            {
                Room room = ai.Room;
                List<Player> players = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                    if (!ai.IsEnemy(p) && !ai.IsWeak(p) && !p.IsNude())
                        players.Add(p);

                if (players.Count > 0)
                {
                    ai.SortByDefense(ref players);
                    return new List<Player> { players[0] };
                }
            }

            return new List<Player>();
            
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            if (room.GetTag("choulue_source") is Player from && ai.IsFriend(from))
            {
                if (player.HasEquip())
                {
                    List<int> ids = player.GetCards("e");
                    ai.SortByUseValue(ref ids, false);
                    if (ai.GetKeepValue(ids[0], player) < 0)
                        return new List<int> { ids[0] };
                }

                List<int> cards = player.GetCards("he");
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(cards, new List<Player> { from });
                if (pair.Key != null && pair.Value > -1)
                    return new List<int> { pair.Value };

                if (from.ContainsTag(Name) && from.GetTag(Name) is string card_name && !string.IsNullOrEmpty(card_name))
                {
                    ai.SortByKeepValue(ref cards, false);
                    return new List<int> { cards[0] };
                }
            }

            return new List<int>();
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());

            Room room = ai.Room; if (player.ContainsTag(Name) && player.GetTag(Name) is string card_name && !string.IsNullOrEmpty(card_name))
            {
                WrappedCard card = new WrappedCard(card_name) { Skill = "_choulue" };
                if (card.Name.Contains(Slash.ClassName))
                {
                    ai.FindSlashandTarget(ref use, player, new List<WrappedCard> { card });
                    if (use.Card != null)
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
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            if (!ai.WillShowForAttack()) return use;

            List<int> ids = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetCards("he"))
            {
                WrappedCard card = room.GetCard(id);
                if (Engine.GetFunctionCard(card.Name) is EquipCard)
                    ids.Add(id);
            }

            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids, false);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    EquipCard equip = (EquipCard)Engine.GetFunctionCard(card.Name);
                    if (equip is Horse)
                    {
                        int index = (int)equip.EquipLocation();
                        List<Player> targets = ai.FriendNoSelf;
                        ai.SortByDefense(ref targets, false);
                        foreach (Player p in targets)
                        {
                            if (p.IsWounded() && p.GetEquip(index) == -1 && RoomLogic.CanPutEquip(p, card))
                            {
                                use.Card = new WrappedCard(HuyuanCard.ClassName) { Skill = Name, Mute = true };
                                use.Card.AddSubCard(id);
                                use.To.Add(p);
                                return use;
                            }
                        }
                    }
                }

                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    EquipCard equip = (EquipCard)Engine.GetFunctionCard(card.Name);
                    if (equip is Armor)
                    {
                        int index = (int)equip.EquipLocation();
                        List<Player> targets = ai.FriendNoSelf;
                        ai.SortByDefense(ref targets, false);
                        foreach (Player p in targets)
                        {
                            if (p.GetEquip(index) == -1 && RoomLogic.CanPutEquip(p, card))
                            {
                                use.Card = new WrappedCard(HuyuanCard.ClassName) { Skill = Name, Mute = true };
                                use.Card.AddSubCard(id);
                                use.To.Add(p);
                                return use;
                            }
                        }
                    }
                }

                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    EquipCard equip = (EquipCard)Engine.GetFunctionCard(card.Name);
                    int index = (int)equip.EquipLocation();
                    List<Player> targets = ai.FriendNoSelf;
                    ai.SortByDefense(ref targets, false);
                    foreach (Player p in targets)
                    {
                        if (p.GetEquip(index) == -1 && RoomLogic.CanPutEquip(p, card))
                        {
                            use.Card = new WrappedCard(HuyuanCard.ClassName) { Skill = Name, Mute = true };
                            use.Card.AddSubCard(id);
                            use.To.Add(p);
                            return use;
                        }
                    }
                }
            }

            return use;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && Engine.GetFunctionCard(card.Name) is EquipCard)
                return 1.5;

            return 0;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in targets)
            {
                scores.Add(ai.FindCards2Discard(player, p, string.Empty, "he", FunctionCard.HandlingMethod.MethodDiscard));
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0) return scores[0].Players;
            }

            return new List<Player>();
        }
    }

    public class ZhenjunAI : SkillEvent
    {
        public ZhenjunAI() : base("zhenjun") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {

            foreach(Player p in targets)
            {
                if (ai.IsFriend(p))
                {
                    if (player.HasEquip())
                    {
                        List<int> ids = player.GetCards("e");
                        ai.SortByUseValue(ref ids, false);
                        if (ai.GetKeepValue(ids[0], player) < 0)
                            return new List<Player> { p };
                    }
                }
            }

            double value = 0;
            Player target = null;
            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p))
                {
                    int count = player.HandcardNum - player.Hp;
                    double _value = count * 0.1;

                    List<int> equips = player.GetEquips();
                    for (int i = 0; i < Math.Min(equips.Count, count); i++)
                        if (ai.GetKeepValue(equips[i], p) > 0)
                            _value += 1;

                    if (_value > value)
                    {
                        value = _value;
                        target = p;
                    }
                }
            }

            double self_value = 0;
            if (targets.Contains(player))
            {
                List<int> ids = player.GetCards("he");
                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    if (ai.GetUseValue(id, player) < 3.5)
                        self_value += 0.5;
                }
            }

            if (self_value > value) return new List<Player> { player };
            if (target != null) return new List<Player> { target };

            foreach (Player p in targets)
            {
                if (ai.IsFriend(p) && p.HandcardNum - p.Hp == 1 && ai.HasSkill("tuntian", p))
                {
                    return new List<Player> { p };
                }
            }

            return new List<Player>();
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            if (ai.IsEnemy(to))
            {
                List<int> equips = to.GetEquips();
                foreach (int id in equips)
                    if (!disable_ids.Contains(id) && ai.GetKeepValue(id, to) > 0)
                        return new List<int> { id };
            }

            return ai.FindCards2Discard(from, to, string.Empty, flags, FunctionCard.HandlingMethod.MethodDiscard, 1, false, disable_ids).Ids;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("he"), result = new List<int>();
            ai.SortByKeepValue(ref ids, false);
            for (int i = 0; i < Math.Min(max, ids.Count); i++)
            {
                if (ai.GetKeepValue(ids[i], player) < 0)
                    result.Add(ids[i]);
                else
                    break;
            }
            ids.RemoveAll(t => result.Contains(t));
            ai.SortByUseValue(ref ids, false);
            for (int i = 0; i < Math.Min(max - result.Count, ids.Count); i++)
                result.Add(ids[i]);

            return result;
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            List<int> cards = new List<int>(ids), result = new List<int>();
            ai.SortByKeepValue(ref cards, false);
            for (int i = 0; i < Math.Min(min, cards.Count); i++)
            {
                if (ai.GetKeepValue(cards[i], player) < 0)
                    result.Add(cards[i]);
                else
                    break;
            }
            cards.RemoveAll(t => result.Contains(t));
            ai.SortByUseValue(ref cards, false);
            for (int i = 0; i < Math.Min(min - result.Count, cards.Count); i++)
                result.Add(cards[i]);

            return result;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (room.GetTag("zhenjun_target") is Player target && ai.IsFriend(target))
                return "draw";

            if (choice.Contains("cancel"))
                return "cancel";

            return "disacard";
        }
    }

    public class ShenduanAI : SkillEvent
    {
        public ShenduanAI() : base("shenduan") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            List<int> ids = player.GetPile("#shenduan");
            foreach (int id in ids)
            {
                WrappedCard ss = new WrappedCard(SupplyShortage.ClassName);
                ss.AddSubCard(id);
                ss.DistanceLimited = false;
                ss = RoomLogic.ParseUseCard(room, ss);
                foreach (Player enemy in ai.GetEnemies(player))
                {
                    if (SupplyShortage.Instance.TargetFilter(room, new List<Player>(), enemy, player, ss))
                    {
                        WrappedCard sd = new WrappedCard(ShenduanCard.ClassName) { Skill = Name };
                        sd.AddSubCard(id);
                        use.Card = sd;
                        use.To.Add(enemy);
                        return use;
                    }
                }

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (SupplyShortage.Instance.TargetFilter(room, new List<Player>(), p, player, ss) && RoomLogic.InMyAttackRange(room, player, p))
                    {
                        WrappedCard sd = new WrappedCard(ShenduanCard.ClassName) { Skill = Name };
                        sd.AddSubCard(id);
                        use.Card = sd;
                        use.To.Add(p);
                        return use;
                    }
                }
            }

            return use;
        }
    }

    public class YonglueAI : SkillEvent
    {
        public YonglueAI() : base("yonglue")
        {}

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            Player target = room.Current;
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            ScoreStruct score = new ScoreStruct();
            if (!RoomLogic.InMyAttackRange(room, player, target))
            {
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { slash }, new List<Player> { target }, false);
                if (scores.Count > 0)
                    score = scores[0];
            }

            if (ai.IsFriend(target))
            {
                if (RoomLogic.InMyAttackRange(room, player, target)) return true;

                if (score.Score >= 0) return true;
                if (RoomLogic.PlayerContainsTrick(room, target, Indulgence.ClassName))
                    if (score.Score > -5) return true;
                if (RoomLogic.PlayerContainsTrick(room, target, SupplyShortage.ClassName))
                    if (score.Score > -4) return true;

                if (RoomLogic.PlayerContainsTrick(room, target, Lightning.ClassName))
                {
                    Player wizzard = ai.GetWizzardRaceWinner(Lightning.ClassName, target);
                    if (wizzard != null && ai.IsEnemy(wizzard))
                    {
                        WrappedCard lightning = null;
                        foreach (int id in target.JudgingArea)
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name == Lightning.ClassName)
                            {
                                lightning = card;
                                break;
                            }
                        }

                        DamageStruct light_damage = new DamageStruct(lightning, null, target, 3, DamageStruct.DamageNature.Thunder);
                        ScoreStruct _score = ai.GetDamageScore(light_damage);
                        if (score.Score > _score.Score) return true;
                    }
                }
            }
            else
            {
                if (score.Score > 8) return true;
                if (RoomLogic.PlayerContainsTrick(room, target, SupplyShortage.ClassName))
                    if (score.Score >= 5) return true;
                if (RoomLogic.PlayerContainsTrick(room, target, Lightning.ClassName))
                {
                    Player wizzard = ai.GetWizzardRaceWinner(Lightning.ClassName, target);
                    if (wizzard != null || !ai.IsFriend(wizzard))
                        return score.Score > 0;
                }
            }

            return false;
        }
    }

    public class QiceJXAI : SkillEvent
    {
        public QiceJXAI() : base("qice_jx") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            if (!player.IsKongcheng() && !player.HasUsed("ViewAsSkill_qice_jxCard"))
            {
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodUse))
                        return new List<WrappedCard>();
                }

                if (room.AvailableFunctionCards.Contains(Reinforcement.Instance))
                {
                    int wound = 0;
                    foreach (Player p in ai.FriendNoSelf)
                        if (p.IsWounded()) wound++;

                    if (wound > 1)
                    {
                        WrappedCard re = new WrappedCard(Reinforcement.ClassName) { Skill = Name };
                        re.AddSubCards(player.GetCards("h"));
                        return new List<WrappedCard> { re };
                    }
                }
                if (room.AvailableFunctionCards.Contains(GodSalvation.Instance))
                {
                    WrappedCard gs = new WrappedCard(GodSalvation.ClassName) { Skill = Name };
                    gs.AddSubCards(player.GetCards("h"));
                    UseCard e = Engine.GetCardUsage(GodSalvation.ClassName);
                    if (e != null)
                    {
                        CardUseStruct use = new CardUseStruct(null, player, new List<Player>())
                        {
                            IsDummy = true
                        };
                        e.Use(ai, player, ref use, gs);
                        if (use.Card != null)
                        {
                            return new List<WrappedCard> { gs };
                        }
                    }
                }
                if (room.AvailableFunctionCards.Contains(ExNihilo.Instance))
                {
                    double value = 0;
                    foreach (int id in player.GetCards("h"))
                        value += ai.GetKeepValue(id, player);

                    if (value < Engine.GetCardUseValue(ExNihilo.ClassName))
                    {
                        WrappedCard ex = new WrappedCard(ExNihilo.ClassName) { Skill = Name };
                        ex.AddSubCards(player.GetCards("h"));
                        return new List<WrappedCard> { ex };
                    }
                }

                if (ai.GetOverflow(player) > 0)
                {
                    Player caocao = ai.FindPlayerBySkill("jianxiong|jianxiong_jx");
                    if (caocao != null && ai.IsFriend(caocao) && !ai.IsWeak(caocao))
                    {
                        WrappedCard aa = new WrappedCard(ArcheryAttack.ClassName) { Skill = Name };
                        aa.AddSubCards(player.GetCards("h"));
                        if (RoomLogic.IsProhibited(room, player, caocao, aa) == null && !ai.IsCancelTarget(aa, caocao, player)
                            && ai.IsCardEffect(aa, caocao, player))
                        {
                            UseCard e = Engine.GetCardUsage(ArcheryAttack.ClassName);
                            if (e != null)
                            {
                                CardUseStruct use = new CardUseStruct(null, player, new List<Player>())
                                {
                                    IsDummy = true
                                };
                                e.Use(ai, player, ref use, aa);
                                if (use.Card != null)
                                {
                                    return new List<WrappedCard> { aa };
                                }
                            }
                        }
                    }
                }
            }

            return new List<WrappedCard>();
        }
        
        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card.Skill == Name)
                return -10;

            return 0;
        }
    }

    public class QigongAI : SkillEvent
    {
        public QigongAI() : base("qigong") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            string[] strs = prompt.Split(':');

            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HasFlag("SlashAssignee")) targets.Add(p);

            List<ScoreStruct> values = ai.CaculateSlashIncome(player, null, targets);
            if (values.Count > 0 && values[0].Score > 0)
            {
                use.Card = values[0].Card;
                use.To = values[0].Players;
            }

            return use;
        }
    }

    public class LiehouAI : SkillEvent
    {
        public LiehouAI() : base("liehou"){}

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(LiehouCard.ClassName))
            {
                return new List<WrappedCard> { new WrappedCard(LiehouCard.ClassName) { Skill = Name } };
            }
            return new List<WrappedCard>();
        }
        public override Player OnYiji(TrustedAI ai, Player player, List<int> ids, ref int id)
        {
            List<Player> friends = new List<Player>(), others = new List<Player>();
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.HasFlag(Name) && RoomLogic.InMyAttackRange(room, player, p))
                {
                    if (ai.IsFriend(p))
                        friends.Add(p);
                    else
                        others.Add(p);
                }
            }

            if (friends.Count > 0)
            {
                KeyValuePair<Player, int> key = ai.GetCardNeedPlayer(ids, friends, Player.Place.PlaceHand, Name);
                if (key.Key != null && key.Value >= 0 && ai.Room.Current == key.Key)
                {
                    id = key.Value;
                    return key.Key;
                }
                if (key.Key != null && key.Value >= 0)
                {
                    id = key.Value;
                    return key.Key;
                }
            }

            ai.SortByUseValue(ref ids, false);
            id = ids[0];
            return others[0];
        }
    }

    public class LiehouCardAI : UseCard
    {
        public LiehouCardAI() : base(LiehouCard.ClassName)
        {
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class HengjiangJXAI : SkillEvent
    {
        public HengjiangJXAI() : base("hengjiang_jx") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
                return !ai.IsFriend(target);

            return true;
        }
    }
}