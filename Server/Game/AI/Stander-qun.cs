using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;

namespace SanguoshaServer.AI
{
    public class StanderQunAI : AIPackage
    {
        public StanderQunAI() : base("Stander-qun")
        {
            events = new List<SkillEvent>
            {
                new LuanjiAI(),
                new LuanjiDrawAI(),
                new JijiuAI(),
                new ChuliAI(),
                new WushuangAI(),
                new BiyueAI(),
                new LijianAI(),
                new ShuangxiongAI(),
                new WeimuAI(),
                new MingshiAI(),
                new LirangAI(),
                new WanshaAI(),
                new LuanwuAI(),
                new SuishiAI(),
                new SijianAI(),
                new JianchuAI(),
                new LeijiAI(),
                new GuidaoAI(),
                new BeigeAI(),
                new XiongyiAI(),
                new ShuangrenAI(),
                new ShuangrenSlashAI(),
                new KuangfuAI(),
                new KuangfuJXAI(),
                new HuoshuiAI(),
                new QingchengAI()
            };

            use_cards = new List<UseCard>
            {
                new ChuliCardAI(),
                new LijianCardAI(),
                new LuanwuCardAI(),
                new XiongyiCardAI(),
                new HuoshuiCardAI(),
                new QingchengCardAI(),
                new KuangfuCardAI(),
            };
        }
    }

    public class LuanjiAI : SkillEvent
    {
        public LuanjiAI() : base("luanji") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;

            if (ai.WillShowForAttack() && (ai.IsSituationClear() || ai.GetOverflow(player) > 1))
            {
                List<int> ids = player.GetCards("he"), available = new List<int>();
                ids.AddRange(player.GetHandPile());
                List<int> suits = player.ContainsTag(Name) ? (List<int>)player.GetTag(Name) : new List<int>();

                double basic_value = Engine.GetCardUseValue(ArcheryAttack.ClassName);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (suits.Contains((int)card.Suit)) continue;

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

                    if (result.Count == 0)
                    {
                        WrappedCard aa = new WrappedCard(ArcheryAttack.ClassName) { Skill = Name, ShowSkill = Name };
                        aa.AddSubCard(available[0]);
                        aa.AddSubCard(available[1]);
                        result.Add(aa);
                    }
                }
            }

            return result;
        }
    }

    public class LuanjiDrawAI : SkillEvent
    {
        public LuanjiDrawAI() : base("luanji-draw") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class JijiuAI : SkillEvent
    {
        public JijiuAI() : base("jijiu")
        {
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            if (player.Phase == Player.PlayerPhase.NotActive && (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand || place == Player.Place.PlaceEquip)
                && WrappedCard.IsRed(room.GetCard(id).Suit) && (room.BloodBattle || room.GetCard(id).Name != Peach.ClassName))
            {
                WrappedCard peach = new WrappedCard(Peach.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                peach.AddSubCard(id);
                peach = RoomLogic.ParseUseCard(room, peach);
                return peach;
            }

            return null;
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

            return Math.Min(0, 5 - value);
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            Room room = ai.Room;
            if (!isUse && ai.HasSkill(Name, player) && !card.IsVirtualCard()
                && (player.GetHandPile().Contains(card.GetEffectiveId()) || place == Player.Place.PlaceHand || place == Player.Place.PlaceEquip)
                && WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card)) && (ai.Room.BloodBattle || card.Name != Peach.ClassName))
            {
                return 0.7;
            }

            return 0;
        }
    }

    public class ChuliAI : SkillEvent
    {
        public ChuliAI() : base("chuli")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("ChuliCard"))
                return new List<WrappedCard> { new WrappedCard("ChuliCard") { Skill = Name, ShowSkill = Name } };

            return null;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> result = new List<int>();
            List<int> ids = player.GetCards("he");
            double value = 10000;
            int _id = -1;
            foreach (int id in ids)
            {
                double _value = ai.GetUseValue(id, player);
                if (ai.Room.GetCard(id).Suit == WrappedCard.CardSuit.Spade)
                    _value -= 1.5;

                if (_value < value)
                {
                    value = _value;
                    _id = id;
                }
            }
            if (_id >= 0)
                result.Add(_id);

            return result;
        }
    }

    public class ChuliCardAI : UseCard
    {
        public ChuliCardAI() : base("ChuliCard")
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6.7;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            FunctionCard fcard = Engine.GetFunctionCard(Name);
            List<Player> selected = new List<Player>();

            while (selected.Count < 3)
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.IsNude() || selected.Contains(p) || !fcard.TargetFilter(room, selected, p, player, card)) continue;

                    ScoreStruct score = ai.FindCards2Discard(player, p, string.Empty, "he", FunctionCard.HandlingMethod.MethodDiscard);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }

                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0)
                        selected.AddRange(scores[0].Players);
                    else
                        break;
                }
                else
                    break;
            }

            if (selected.Count > 0)
            {
                use.Card = card;
                use.To = selected;
            }
        }
    }

    public class WushuangAI : SkillEvent
    {
        public WushuangAI() : base("wushuang")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack()) return false;
            if (data is Player p)
                return !ai.IsFriend(p);

            return true;
        }
    }

    public class BiyueAI : SkillEvent
    {
        public BiyueAI() : base("biyue")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForDefence();
        }
    }

    public class LijianAI : SkillEvent
    {
        public LijianAI() : base("lijian")
        {
        }

        public static int FindLijianCard(TrustedAI ai, Player player)
        {
            int result = -1;

            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);
            }
            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids, false);
                if (ai.GetKeepValue(ids[0], player) < 0)
                    return ids[0];

                Dictionary<int, double> values = new Dictionary<int, double>();
                List<int> equips = new List<int>();
                foreach (int id in ids)
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

                if (speacial.Count > 0)
                {
                    if (ohorse.Count > 0)
                        return ohorse[0];
                    if (dhorse.Count > 0)
                        return dhorse[0];
                }
                else
                {
                    if (ohorse.Count > 1)
                    {
                        ohorse.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                        return ohorse[0];
                    }
                    if (dhorse.Count > 1)
                    {
                        dhorse.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                        return dhorse[0];
                    }
                }

                if (weapons.Count > 1)
                {
                    weapons.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                    return weapons[0];
                }
                
                if (treasure.Count > 1)
                {
                    treasure.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                    return treasure[0];
                }

                ids.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                if (ai.GetUseValue(ids[0], player) < 6)
                    return ids[0];
            }


            return result;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("LijianCard"))
            {
                int id = FindLijianCard(ai, player);
                if (id >= 0)
                {
                    WrappedCard lijian = new WrappedCard("LijianCard") { Skill = Name, ShowSkill = Name, Mute = true };
                    lijian.AddSubCard(id);
                    return new List<WrappedCard> { lijian };
                }
            }

            return null;
        }
    }

    public class LijianCardAI : UseCard
    {
        public LijianCardAI() : base("LijianCard")
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is StupidAI _ai)
                {
                    Player target = use.To[0];
                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        DamageStruct damage = new DamageStruct(new WrappedCard(Duel.ClassName) , use.To[1], target);
                        if (_ai.NeedDamage(damage) || (ai.IsFriend(use.To[1], target) && ai.HasSkill("zhiman_jx", use.To[1])))
                        {
                            ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(target), 60);
                            return;
                        }

                        ai.UpdatePlayerRelation(player, target, false);
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return Engine.GetCardPriority(Duel.ClassName) + 1;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<ScoreStruct> scores = new List<ScoreStruct>();
            Room room = ai.Room;
            WrappedCard duel = new WrappedCard(Duel.ClassName);
            FunctionCard fcard = Engine.GetFunctionCard(Duel.ClassName);
            foreach (Player target in room.GetOtherPlayers(player))
            {
                if (target.IsMale() && !target.Removed && ai.IsEnemy(target))
                {
                    int n1 = ai.GetKnownCardsNums(Slash.ClassName, "he", target);
                    if (ai.HasSkill("wushuang", target))
                        n1 *= 2;
                    foreach (Player user in room.GetOtherPlayers(player))
                    {
                        if (user != target && user.IsMale() && !user.Removed && fcard.IsAvailable(room, user, duel)
                            && fcard.TargetFilter(room, new List<Player>(), target, user, duel) && !ai.IsCancelTarget(duel, target, user)
                            && ai.IsCardEffect(duel, target, user))
                        {
                            DamageStruct damage = new DamageStruct(duel, user, target);
                            ScoreStruct score = ai.GetDamageScore(damage);
                            if (ai.IsFriend(user) && n1 >= 1 && !ai.IsLackCard(target, Slash.ClassName))
                                score.Score -= 3;

                            if (ai.HasSkill("jiang", user))
                                score.Score += ai.IsFriend(user) ? 2 : -2;

                            foreach (string skill in ai.GetKnownSkills(target))
                            {
                                SkillEvent ev = Engine.GetSkillEvent(skill);
                                if (ev != null)
                                    score.Score += ev.TargetValueAdjust(ai, card, user, new List<Player> { target }, target);
                            }
                            score.Players = new List<Player> { target, user };
                            scores.Add(score);
                        }
                    }
                }
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score >= 4)
                {
                    use.Card = card;
                    use.To = new List<Player>(scores[0].Players);
                }
            }
        }
    }

    public class WeimuAI : SkillEvent
    {
        public WeimuAI() : base("weimu")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardUseStruct use)
            {
                if  (use.Card.Name == Edict.ClassName || (use.Card.Name == FightTogether.ClassName && player.Chained))
                    return false;
            }
            return true;
        }

        public override bool IsCancelTarget(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            Room room = ai.Room;
            if (card != null && ai.HasSkill(Name, to) && Engine.GetFunctionCard(card.Name) is TrickCard)
                return WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, card));

            return false;
        }
    }

    public class WanshaAI : SkillEvent
    {
        public WanshaAI() : base("wansha")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is DyingStruct dying)
                return ai.WillShowForAttack() && !ai.IsFriend(dying.Who);

            return false;
        }
    }

    public class LuanwuAI : SkillEvent
    {
        public LuanwuAI() : base("luanwu")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && player.GetMark("@chaos") > 0)
            {
                Room room = ai.Room;
                double good = 0, bad = 0;
                if (RoomLogic.PlayerHasShownSkill(room, player, "baoling"))
                    good += 0.8;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (!p.Removed && ai.IsWeak(p))
                    {
                        if (ai.IsFriend(p)) bad += 1.5;
                        else if (p.HasShownOneGeneral()) good += 0.8;
                        else good += 0.4;
                    }
                }

                if (good < ((double)room.AliveCount() / 4)) return null;

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.Removed) continue;

                    int hp = Math.Max(p.Hp, 1);
                    if (ai.GetKnownCardsNums(Analeptic.ClassName, "he", p, player) > 0)
                    {
                        if (ai.IsFriend(p))
                            good += 1 / hp;
                        else
                            bad += 1 / hp;
                    }
                    bool slash = ai.GetKnownCardsNums(Slash.ClassName, "he", p, player) > 0;
                    bool can_slash = false;
                    foreach (Player _p in room.GetOtherPlayers(p))
                    {
                        if (RoomLogic.InMyAttackRange(room, p, _p))
                        {
                            can_slash = true;
                            break;
                        }
                    }

                    if (!slash || !can_slash)
                    {
                        if (ai.IsFriend(p))
                            good += Math.Max(ai.GetKnownCardsNums(Peach.ClassName, "he", p, player), 1);
                        else
                            bad += Math.Max(ai.GetKnownCardsNums(Peach.ClassName, "he", p, player), 1);
                    }

                    if (ai.GetKnownCardsNums(Jink.ClassName, "he", p, player) > 0)
                    {
                        double lost_value = 0;
                        if (ai.HasSkill(TrustedAI.MasochismSkill, p))
                            lost_value = p.Hp / 2;
                        hp = Math.Max(p.Hp, 1);
                        if (ai.IsFriend(p))
                            bad += (lost_value + 1) / hp;
                        else
                            good += (lost_value + 1) / hp;

                    }
                }

                if (good > bad)
                    return new List<WrappedCard> { new WrappedCard("LuanwuCard") { Skill = Name, ShowSkill = Name, Mute = true } };
            }

            return null;
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HasFlag("SlashAssignee"))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, null, targets);
                if (scores.Count > 0 && scores[0].Score > -2 && scores[0].Card != null)
                {
                    use.Card = scores[0].Card;
                    use.To = scores[0].Players;
                }
            }

            return use;
        }
    }

    public class LuanwuCardAI : UseCard
    {
        public LuanwuCardAI() : base("LuanwuCard")
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class ShuangxiongAI : SkillEvent
    {
        public ShuangxiongAI() : base("shuangxiong")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack() || ai.WillSkipPlayPhase(player) ||
                ai.GetKnownCardsNums(Slash.ClassName, "he", player) < 2 || player.HandcardNum + player.GetPile("wooden_ox").Count < 3)
                return false;

            WrappedCard duel = new WrappedCard(Duel.ClassName);
            FunctionCard fcard = Engine.GetFunctionCard(Duel.ClassName);
            if (fcard.IsAvailable(ai.Room, player, duel))
            {
                CardUseStruct use = new CardUseStruct(duel)
                {
                    From = player,
                    IsDummy = true,
                    To = new List<Player>()
                };
                UseCard useCard = Engine.GetCardUsage(Duel.ClassName);
                useCard.Use(ai, player, ref use, duel);
                if (use.Card != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark(Name) > 0)
            {
                Room room = ai.Room;
                List<int> ids= player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if ((player.GetMark(Name) == 1 && WrappedCard.IsBlack(card.Suit)) || (player.GetMark(Name) == 2 && WrappedCard.IsRed(card.Suit)))
                    {
                        WrappedCard duel = new WrappedCard(Duel.ClassName);
                        duel.AddSubCard(card);
                        duel.Skill = Name;
                        duel.ShowSkill = Name;
                        duel = RoomLogic.ParseUseCard(room, duel);
                        return new List<WrappedCard> { duel };
                    }
                }
            }

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card.Id <= 0) return 0;
            if (player.GetMark(Name) == 1)
                return WrappedCard.IsBlack(RoomLogic.GetCardSuit(ai.Room, card)) ? 1 : 0;
            if (player.GetMark(Name) == 2)
                return WrappedCard.IsRed(RoomLogic.GetCardSuit(ai.Room, card)) ? 1 : 0;

            if (ai.HasSkill(Name, player))
                return 0.5;

            return 0;
        }
    }

    public class JianchuAI : SkillEvent
    {
        public JianchuAI() : base("jianchu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack()) return false;
            if (data is Player target)
            {
                if (ai.IsFriend(target)) return false;

                Room room = ai.Room;
                List<CardUseStruct> list = room.GetUseList();
                CardUseStruct use = list[list.Count - 1];
                int damage_count = 1 + use.Drank + use.ExDamage;

                if (target.GetCards("he").Count == 1 && !player.HasWeapon(QinggangSword.ClassName) && !player.HasWeapon(Saber.ClassName)
                    && target.GetArmor() && RoomLogic.CanDiscard(room, player, target, target.Armor.Key))
                {
                    if (target.HasArmor(Vine.ClassName) && use.Card.Name == FireSlash.ClassName)
                        return false;
                    if (target.HasArmor(SilverLion.ClassName) && damage_count == 1 && target.IsWounded())
                        return false;
                    if (target.HasArmor(RenwangShield.ClassName) && WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, use.Card)))
                        return true;
                }

                if (ai.HasSkill(TrustedAI.LoseEquipSkill, target) && target.HandcardNum == 0)
                {
                    foreach (int id in target.GetEquips())
                        if (ai.GetKeepValue(id, target, Player.Place.PlaceEquip) > -2)
                            return true;

                    return false;
                }
            }

            return true;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            Room room = ai.Room;
            List<CardUseStruct> list = room.GetUseList();
            CardUseStruct use = list[list.Count - 1];
            int damage_count = 1 + use.Drank + use.ExDamage;

            if ((WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, use.Card)) && to.HasArmor(RenwangShield.ClassName) && RoomLogic.CanDiscard(room, from, to, to.Armor.Key))
                || (use.Card.Name != Slash.ClassName && to.HasArmor(PeaceSpell.ClassName))
                || (damage_count > 1 && to.HasArmor(SilverLion.ClassName)))
            {
                return new List<int> { to.Armor.Key };
            }

            if (to.HasTreasure(WoodenOx.ClassName) && to.GetPile("wooden_ox").Count > 0 && RoomLogic.CanDiscard(room, from, to, to.Treasure.Key))
                return new List<int> { to.Treasure.Key };

            if (to.HandcardNum == 1 && RoomLogic.CanDiscard(room, from, to, "h"))
            {
                bool discard_hand = true;
                int id = to.GetCards("h")[0];
                if (ai.GetKnownCards(to).Count == 1)
                {
                    if (ai.GetUseValue(id, to, Player.Place.PlaceHand) < 5 && ai.GetKeepValue(id, to, Player.Place.PlaceHand) < 5)
                        discard_hand = false;
                }

                if (discard_hand)
                    return to.GetCards("h");
            }
            List<int> ids = new List<int>();
            foreach (int id in to.GetEquips())
                if (RoomLogic.CanDiscard(room, from, to, id))
                    ids.Add(id);

            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids);
                return new List<int> { ids[0] };
            }

            return null;
        }
    }

    public class SuishiAI : SkillEvent
    {
        public SuishiAI() : base("suishi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            TriggerEvent e = (TriggerEvent)(int)data;
            if (e == TriggerEvent.Dying)
                return true;

            return false;
        }
    }

    public class SijianAI : SkillEvent
    {
        public SijianAI() : base("sijian")
        {
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            ai.Choice[Name] = null;
            List<CardUseStruct> list = room.GetUseList();
            if (list.Count > 0)
            {
                CardUseStruct use = list[list.Count - 1];
                if (use.Card != null && use.Card.Name.Contains(Slash.ClassName) && use.From != null && use.From.Alive && ai.IsEnemy(use.From)
                    && use.To.Contains(player) && use.From.HasWeapon(Axe.ClassName) && use.From.GetCards("he").Count > 2)
                {
                    ai.Choice[Name] = use.From.Weapon.Key.ToString();
                    return new List<Player> { use.From };
                }
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, string.Empty, "he", FunctionCard.HandlingMethod.MethodDiscard);
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    return scores[0].Players;
                }
            }

            return null;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            if (!string.IsNullOrEmpty(ai.Choice[Name]) && int.TryParse(ai.Choice[Name], out int id) && to.GetCards("he").Contains(id))
                return new List<int> { id };

            return null;
        }
    }

    public class LeijiAI : SkillEvent
    {
        public LeijiAI() : base("leiji")
        {
            key = new List<string> { "playerChosen:leiji" };
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
                    if (ai.GetPlayerTendency(target) == "unknown" && ai.IsKnown(player, target))
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForAttack() || !ai.WillShowForDefence()) return new List<Player>();

            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in enemies)
            {
                if (ai.HasSkill("hongyan", p)) continue;
                DamageStruct damage = new DamageStruct(Name, player, p, 2, DamageStruct.DamageNature.Thunder);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score > 0 && score.DoDamage)
                    score.Score += ai.ChainDamage(damage);
                scores.Add(score);
            }

            if (scores.Count > 0)
            {
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 0)
                    return new List<Player> { scores[0].Damage.To };
            }

            return new List<Player>();
        }
    }

    public class GuidaoAI : SkillEvent
    {
        public GuidaoAI() : base("guidao")
        {
            key = new List<string> { "cardResponded%guidao" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && ai.Self != player)
            {
                Room room = ai.Room;
                string[] choices = choice.Split('%');
                if (choices[1] == Name && room.GetTag(Name) is JudgeStruct judge && ai.GetPlayerTendency(judge.Who) == "unknown" && choices[4] != "_nil_")
                {
                    string str = choices[4].Substring(1, choices[4].Length - 2);
                    WrappedCard _card = RoomLogic.ParseCard(room, str);
                    WrappedCard.CardSuit suit = RoomLogic.GetCardSuit(room, _card);
                    int number = RoomLogic.GetCardNumber(room, _card);
                    WrappedCard card = new WrappedCard(_card.Name, _card.GetEffectiveId(), suit, number);
                    if (suit == WrappedCard.CardSuit.Spade && RoomLogic.PlayerHasShownSkill(room, judge.Who, "hongyan"))
                        card.SetSuit(WrappedCard.CardSuit.Heart);

                    bool effected = judge.Good == Engine.MatchExpPattern(room, judge.Pattern, judge.Who, card);
                    bool change = judge.IsEffected() == effected;

                    if (change)
                    {
                        if (judge.Reason == "beige" || judge.Reason == "beige_jx")
                        {
                            if (!judge.Who.FaceUp && suit == WrappedCard.CardSuit.Spade)
                                ai.UpdatePlayerRelation(player, judge.Who, true);
                        }
                        else if (judge.Reason == Lightning.ClassName)
                        {
                            if (judge.Card.Suit == WrappedCard.CardSuit.Spade && judge.Card.Number > 1 && judge.Card.Number <= 9
                                && (suit != WrappedCard.CardSuit.Spade || number > 9 || number == 1))
                                ai.UpdatePlayerRelation(player, judge.Who, true);
                            else if (suit == WrappedCard.CardSuit.Spade && number > 1 && number <= 9)
                                ai.UpdatePlayerRelation(player, judge.Who, false);
                        }
                        else if (judge.Reason == SupplyShortage.ClassName)
                        {
                            if (suit != WrappedCard.CardSuit.Club)
                                ai.UpdatePlayerRelation(player, judge.Who, false);
                            else if (suit == WrappedCard.CardSuit.Club)
                                ai.UpdatePlayerRelation(player, judge.Who, true);
                        }
                        else if (judge.Reason == "ganglie")
                        {
                            if (judge.Card.Suit == WrappedCard.CardSuit.Heart && suit != WrappedCard.CardSuit.Heart)
                                ai.UpdatePlayerRelation(player, judge.Who, false);
                        }
                        else if (judge.Reason == EightDiagram.ClassName)
                        {
                            if (WrappedCard.IsRed(judge.Card.Suit) && WrappedCard.IsBlack(suit))
                                ai.UpdatePlayerRelation(player, judge.Who, false);
                        }
                        else if (judge.Reason == "leiji")
                        {
                            if (judge.Card.Suit == WrappedCard.CardSuit.Spade && suit != WrappedCard.CardSuit.Spade)
                                ai.UpdatePlayerRelation(player, judge.Who, true);
                            else if (suit == WrappedCard.CardSuit.Spade)
                                ai.UpdatePlayerRelation(player, judge.Who, false);
                        }
                        else if (judge.Reason == "wuhun")
                        {
                            if (judge.Card.Name != Peach.ClassName && judge.Card.Name != GodSalvation.ClassName && (card.Name == Peach.ClassName || card.Name == GodSalvation.ClassName))
                                ai.UpdatePlayerRelation(player, judge.Who, true);
                            else if (card.Name != Peach.ClassName && card.Name != GodSalvation.ClassName)
                                ai.UpdatePlayerRelation(player, judge.Who, false);
                        }
                    }
                }
            }
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card.Id <= 0) return 0;

            if (ai.HasSkill(Name, player) && card.Name == EightDiagram.ClassName)
            {
                return 4;
            }

            if (ai.HasSkill(Name, player) && !isUse)
            {
                if (ai.IsCard(card.GetEffectiveId(), Jink.ClassName, player)) return 1.2;

                WrappedCard.CardSuit suit = RoomLogic.GetCardSuit(ai.Room, card);
                if (suit == WrappedCard.CardSuit.Spade)
                    return 0.5;
                else if (suit == WrappedCard.CardSuit.Club)
                    return 0.2;
            }

            return 0;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            if (data is JudgeStruct judge)
            {
                Room room = ai.Room;
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (WrappedCard.IsBlack(card.Suit) && !RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodResponse, true))
                        ids.Add(id);
                }
                foreach (int id in player.GetEquips())
                {
                    WrappedCard card = room.GetCard(id);
                    if (!card.HasFlag("using") && WrappedCard.IsBlack(card.Suit) && !RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodResponse, false))
                        ids.Add(id);
                }
                foreach (int id in player.GetHandPile())
                {
                    WrappedCard card = room.GetCard(id);
                    if (WrappedCard.IsBlack(card.Suit) && !RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodResponse, false))
                        ids.Add(id);
                }

                int result = ai.GetRetrialCardId(ids, judge);
                if (result >= 0)
                {
                    use.Card = ai.Room.GetCard(result);
                    return use;
                }

                if (ai.GetKnownCardsNums(Jink.ClassName, "he", player) == 0 && ai.IsCard(judge.Card.Id, Jink.ClassName, player) && ids.Count > 0)
                {
                    foreach (int id in ids)
                    {
                        if (ai.GetUseValue(id, player) < 5 && ai.GetKeepValue(id, player) < 5)
                        {
                            result = id;
                            break;
                        }
                    }

                    //在不改变判定结果的情况下拿判定闪
                    if (result >= 0)
                    {
                        WrappedCard card = room.GetCard(result);
                        WrappedCard _card = new WrappedCard(card.Name, card.Id, card.Suit, card.Number);
                        if (card.Suit == WrappedCard.CardSuit.Spade && RoomLogic.PlayerHasShownSkill(room, judge.Who, "hongyan"))
                            _card.SetSuit(WrappedCard.CardSuit.Heart);

                        bool effected = judge.Good == Engine.MatchExpPattern(room, judge.Pattern, judge.Who, _card);
                        if (judge.IsEffected() == effected)
                        {
                            use.Card = ai.Room.GetCard(result);
                            return use;
                        }
                    }
                }
            }

            return use;
        }

        public static bool HasSpade(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            foreach (int id in player.GetEquips())
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Spade && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                    return true;

            List<int> ids = ai.GetKnownCards(player);
            ids.AddRange(ai.GetKnownHandPileCards(player));
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Spade && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                    return true;
            }

            int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
            if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                count += player.HandcardNum - ai.GetKnownCards(player).Count;

            return count > 2;
        }

        public static bool HasClub(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            foreach (int id in player.GetEquips())
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Club && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                    return true;

            List<int> ids = ai.GetKnownCards(player);
            ids.AddRange(ai.GetKnownHandPileCards(player));
            foreach (int id in ids)
            {
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Club && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                    return true;
            }

            int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
            if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                count += player.HandcardNum - ai.GetKnownCards(player).Count;

            return count > 3;
        }

        public override bool CanRetrial(TrustedAI ai, string pattern, Player player, Player judge_who)
        {
            Room room = ai.Room;
            if (pattern == "leiji")
            {
                if (ai.IsFriend(player, judge_who))
                    return HasClub(ai, player);
                else
                    return HasSpade(ai, player);
            }
            if (pattern == "leiji_jx")
            {
                if (!ai.IsFriend(player, judge_who))
                    return HasSpade(ai, player) || HasClub(ai, player);
            }
            else if (pattern == SupplyShortage.ClassName)
            {
                if (ai.IsFriend(player, judge_who))
                    return HasClub(ai, player);
                else
                    return HasSpade(ai, player);
            }
            else if (pattern == Indulgence.ClassName && ai.IsEnemy(player, judge_who))
            {
                foreach (int id in player.GetEquips())
                {
                    WrappedCard card = room.GetCard(id);
                    if (WrappedCard.IsBlack(card.Suit) && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                        return true;
                }

                List<int> ids = ai.GetKnownCards(player);
                ids.AddRange(ai.GetKnownHandPileCards(player));
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if (WrappedCard.IsBlack(card.Suit) && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                        return true;
                }

                int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                    count += player.HandcardNum - ai.GetKnownCards(player).Count;

                if (count > 2) return true;
            }
            else if (pattern == Lightning.ClassName)
            {
                if (ai.IsFriend(player, judge_who))
                {
                    foreach (int id in player.GetEquips())
                    {
                        WrappedCard card = room.GetCard(id);
                        if (WrappedCard.IsBlack(card.Suit) && !(card.Suit == WrappedCard.CardSuit.Spade && card.Number == 1 && card.Number >= 10)
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (WrappedCard.IsBlack(card.Suit) && (card.Suit != WrappedCard.CardSuit.Spade || card.Number == 1 || card.Number >= 10)
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 2) return true;
                }
                else
                {
                    foreach (int id in player.GetEquips())
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Spade && card.Number > 1 && card.Number < 10
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    List<int> ids = ai.GetKnownCards(player);
                    ids.AddRange(ai.GetKnownHandPileCards(player));
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Spade && card.Number > 1 && card.Number < 10
                            && !RoomLogic.IsCardLimited(room, player, room.GetCard(id), FunctionCard.HandlingMethod.MethodResponse))
                            return true;
                    }

                    int count = player.GetHandPile(true).Count - ai.GetKnownHandPileCards(player).Count;
                    if (!RoomLogic.IsHandCardLimited(room, player, FunctionCard.HandlingMethod.MethodResponse))
                        count += player.HandcardNum - ai.GetKnownCards(player).Count;

                    if (count > 4) return true;
                }
            }

            return false;
        }
    }

    public class BeigeAI : SkillEvent
    {
        public BeigeAI() : base("beige")
        {
            key = new List<string> { "cardDiscard:beige" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                Room room = ai.Room;
                if (choices[1] == Name && room.GetTag("beige_data") is DamageStruct damage)
                {
                    if (!player.HasShownOneGeneral())
                    {
                        string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 100);
                    }

                    if (ai is SmartAI && ai.GetPlayerTendency(damage.To) == "unknown" && ai.Self != damage.To)
                        ai.UpdatePlayerRelation(player, damage.To, true);
                    else if (ai is StupidAI && ai.GetPlayerTendency(damage.To) != "unknown" && ai.Self != damage.To)
                        ai.UpdatePlayerRelation(player, damage.To, true);
                }
            }
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> cards, int min, int max, bool option)
        {
            Room room = ai.Room;
            if (room.GetTag("beige_data") is DamageStruct damage && ai.IsFriend(damage.To) && damage.From != null && damage.From.Alive && !ai.IsFriend(damage.From))
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("he"))
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    if (room.Current == player)
                        ai.SortByUseValue(ref ids, false);
                    else
                        ai.SortByKeepValue(ref ids, false);

                    int result = -1;
                    if (room.Current == player && ai.GetUseValue(ids[0], player) < 6)
                        result = ids[0];
                    else if (room.Current != player && ai.GetKeepValue(ids[0], player) < 6)
                        result = ids[0];

                    if (result >= 0)
                        return new List<int> { ids[0] };
                }
            }

            return new List<int>();
        }
    }

    public class XiongyiAI : SkillEvent
    {
        public XiongyiAI() : base("xiongyi")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("@arise") > 0)
            {
                WrappedCard card = new WrappedCard(XiongyiCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name,
                    Mute = true
                };
                Room room = ai.Room;
                int count = 1;
                int shown = 0;
                int unknown = 0;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (RoomLogic.WillBeFriendWith(room, player, p, "xiongyi"))
                        count++;
                }
                if (count > (double)room.Setting.PlayerNum / 2 - 1)
                    return new List<WrappedCard> { card };

                foreach (Player p in room.Players)
                {
                    if (p.HasShownOneGeneral())
                        shown++;
                    if (ai.GetPlayerTendency(p) == "unknown")
                        unknown++;
                }

                if (count > 1 && unknown == 0)
                    return new List<WrappedCard> { card };

                if ((room.AliveCount() < (double)room.Setting.PlayerNum / 2 - 1 || shown > (double)room.Setting.PlayerNum * 2 / 3) && ai.IsWeak(player))
                    return new List<WrappedCard> { card };
            }

            return null;
        }

    }

    public class XiongyiCardAI : UseCard
    {
        public XiongyiCardAI() : base(XiongyiCard.ClassName)
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class MingshiAI : SkillEvent
    {
        public MingshiAI() : base("mingshi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class LirangAI : SkillEvent
    {
        public LirangAI() : base("lirang")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };

            ((SmartAI)ai).UpdatePlayers();
            List<Player> friends = ai.FriendNoSelf;
            if (ai.WillShowForAttack() && friends.Count > 0)
            {
                WrappedCard card = new WrappedCard("LirangCard")
                {
                    ShowSkill = Name,
                    Skill = Name
                };
                List<int> card_ids = player.GetPile("#lirang");
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(card_ids, friends);
                if (pair.Key != null && pair.Value >= 0)
                {
                    card.AddSubCard(pair.Value);
                    use.Card = card;
                    use.To.Add(pair.Key);
                }
                else
                {
                    ai.SortByDefense(ref friends, false);
                    card.AddSubCards(card_ids);
                    use.Card = card;
                    use.To.Add(friends[0]);
                }
            }

            return use;
        }
    }

    public class ShuangrenAI : SkillEvent
    {
        public ShuangrenAI() : base("shuangren")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForAttack()) return new List<Player>();

            Room room = ai.Room;
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                DistanceLimited = false,
                Skill = Name
            };

            WrappedCard max_card = ai.GetMaxCard();
            int max_num = max_card.Number;
            if (ai.HasSkill("yingyang")) max_num = Math.Min(max_num + 3, 13);

            List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { slash }, null, false);
            if (scores.Count > 0)
            {
                foreach (ScoreStruct score in scores)
                {
                    if (score.Score > 0 && score.Players.Count > 1 && ai.IsEnemy(score.Players[0]))
                    {
                        List<Player> targets = new List<Player>();
                        foreach (Player p in room.GetOtherPlayers(player))
                        {
                            if (RoomLogic.IsFriendWith(room, score.Players[0], p) && RoomLogic.CanBePindianBy(room, p, player))
                                targets.Add(p);
                        }

                        if (targets.Count > 0)
                        {
                            foreach (Player p in targets)
                            {
                                int enemy_number = ai.GetOverflow(player) > 2 ? 12 : 10;
                                if (p.HandcardNum == ai.GetKnownCards(p).Count)
                                {
                                    enemy_number = ai.GetMaxCard(p).Number;
                                }
                                if (ai.HasSkill("yingyang", p))
                                    enemy_number = Math.Min(enemy_number + 3, 13);

                                if (max_num > enemy_number)
                                    return new List<Player> { p };
                            }
                        }
                    }
                }
            }

            return new List<Player>();
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> player)
        {
            if (ai.Self != requestor)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    DistanceLimited = false,
                    Skill = Name
                };

                bool effect = false;
                List<Player> targets = new List<Player>();
                Room room = ai.Room;
                foreach (Player p in room.GetOtherPlayers(requestor))
                {
                    if (RoomLogic.IsFriendWith(room, ai.Self, p))
                    {
                        targets.Add(p);
                    }
                }

                WrappedCard fire = null;
                if (requestor.HasWeapon(Fan.ClassName))
                    fire = new WrappedCard(FireSlash.ClassName)
                    {
                        DistanceLimited = false,
                        Skill = Name
                    };

                foreach (Player p in targets)
                {
                    if (!ai.IsCancelTarget(slash, p, requestor) && ai.IsCardEffect(slash, p, requestor) && RoomLogic.IsProhibited(room, requestor, p, slash) == null)
                    {
                        DamageStruct damage = new DamageStruct(slash, requestor, p);
                        if (ai.GetDamageScore(damage).Score < 0)
                        {
                            effect = true;
                            break;
                        }
                    }

                    if (fire != null && !ai.IsCancelTarget(fire, p, requestor) && ai.IsCardEffect(fire, p, requestor) && RoomLogic.IsProhibited(room, requestor, p, fire) == null)
                    {
                        DamageStruct damage = new DamageStruct(slash, requestor, p, 1, DamageStruct.DamageNature.Fire); if (ai.GetDamageScore(damage).Score < 0)
                        {
                            effect = true;
                            break;
                        }
                    }
                }

                if (!effect)
                {
                    List<int> cards = ai.Self.GetCards("h");
                    ai.SortByKeepValue(ref cards, false);
                    return room.GetCard(cards[0]);
                }
            }

            return ai.GetMaxCard();
        }
    }

    public class ShuangrenSlashAI : SkillEvent
    {
        public ShuangrenSlashAI() : base("shuangren-slash")
        { }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                DistanceLimited = false,
                Skill = Name
            };
            List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { slash }, target, false);
            if (scores.Count > 0 && scores[0].Players.Count == 1)
                return scores[0].Players;

            return null;
        }
    }


    public class KuangfuAI : SkillEvent
    {
        public KuangfuAI() : base("kuangfu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            ai.Choice[Name] = null;
            if (data is Player target)
            {
                Room room = ai.Room;
                List<int> disable_equiplist = new List<int>(), equiplist = new List<int>();
                for (int i = 0; i < 6; i++)
                {
                    if (target.GetEquip(i) >= 0)
                    {
                        if (!RoomLogic.CanDiscard(room, player, target, target.GetEquip(i)))
                        {
                            if (player.GetEquip(i) >= 0 || ((i == 2 || i == 3) && player.GetEquip(5) >= 0))
                            {
                                disable_equiplist.Add(target.GetEquip(i));
                                continue;
                            }
                        }
                        equiplist.Add(target.GetEquip(i));
                    }
                }

                double value = 0;
                int result = -1;
                foreach (int id in equiplist)
                {
                    double _value = ai.GetKeepValue(id, target, Player.Place.PlaceEquip);
                    if (ai.GetSameEquip(room.GetCard(id), player) == null)
                        _value += Engine.GetCardUseValue(room.GetCard(id).Name);

                    if (_value > value)
                    {
                        value = _value;
                        result = id;
                    }
                }

                if (result >= 0)
                    ai.Choice[Name] = result.ToString();
                else
                    return false;
            }


            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return "move";
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            if (!string.IsNullOrEmpty(ai.Choice[Name]) && int.TryParse(ai.Choice[Name], out int id))
                return new List<int> { id };

            return null;
        }
    }

    public class KuangfuJXAI : SkillEvent
    {
        public KuangfuJXAI() : base("kuangfu_jx")
        {
            key = new List<string> { "cardChosen:kuangfu_jx", "playerChosen:kuangfu_jx" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                Room room = ai.Room;
                string[] choices = choice.Split(':');
                if (choices[1] == Name && choice.Contains("cardChosen:kuangfu_jx"))
                {
                    int id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);
                    if (target != player)
                    {
                        bool friend = ai.GetKeepValue(id, target, Player.Place.PlaceEquip) < 0;
                        ai.UpdatePlayerRelation(player, target, friend);
                    }
                }
                else if (choice.Contains("playerChosen:kuangfu_jx") && player.HasFlag(Name))
                {
                    Player target = room.FindPlayer(choices[2]);
                    WrappedCard slash = new WrappedCard(Slash.ClassName);
                    if (ai is SmartAI && ai.Self != player)
                    {
                            if (!ai.IsCancelTarget(slash, target, player) && ai.IsCardEffect(slash, target, player) &&
                                (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(target).Count == 1 || ai.IsKnown(player, target)))
                                ai.UpdatePlayerRelation(player, target, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
                    }
                    else if (ai is StupidAI _ai)
                    {
                        Player p = target;
                            if (ai.GetPlayerTendency(p) != "unknown" && !ai.IsCancelTarget(slash, p, player) && ai.IsCardEffect(slash, p, player))
                            {
                                if (ai.HasSkill("leiji|leiji_jx", p) && !ai.IsLackCard(p, "Jink") && (ai.HasArmorEffect(p, EightDiagram.ClassName) || p.HandcardNum >= 3)
                                    && !ai.HasSkill("tieqi_jx|liegong_jx"))
                                {
                                    ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 60);
                                }
                                else if (ai.HasSkill(TrustedAI.MasochismSkill, p) && ai.HasSkill("jueqing", player))
                                {
                                    ai.UpdatePlayerRelation(player, p, false);
                                }
                                else
                                {
                                    int count = 1;
                                    if (ai.HasSkill("liegong_jx", player) && player.Hp <= p.Hp) count++;
                                    DamageStruct damage = new DamageStruct(slash, player, p, count);
                                    if (slash.Name == FireSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Fire;
                                    else if (slash.Name == ThunderSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Thunder;

                                    if ((ai.HasSkill("zhiman_jx", player) || player.HasWeapon(IceSword.ClassName)) && ai.GetPlayerTendency(player) != "unknown")
                                        return;

                                    if (ai.HasSkill("zhiman_jx", player))
                                    {
                                        bool good = p.JudgingArea.Count > 0;
                                        if (!good)
                                        {
                                            foreach (int id in p.GetEquips())
                                            {
                                                if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                                                {
                                                    good = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (good)
                                        {
                                            if (ai.GetPlayerTendency(p) == "lord" || ai.GetPlayerTendency(p) == "loyalist")
                                                ai.UpdatePlayerIntention(player, "rebel", 20);
                                            else
                                                ai.UpdatePlayerIntention(player, "loyalist", 20);

                                        return;
                                        }
                                    }

                                    if (player.HasWeapon(IceSword.ClassName))
                                    {
                                        int good = 0;
                                        foreach (int id in p.GetEquips())
                                        {
                                            if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                            {
                                                good++;
                                            }
                                        }
                                        if (good > 0 && (good > 1 || p.HandcardNum == 0))
                                        {
                                            if (ai.GetPlayerTendency(p) == "lord" || ai.GetPlayerTendency(p) == "loyalist")
                                                ai.UpdatePlayerIntention(player, "rebel", 20);
                                            else
                                                ai.UpdatePlayerIntention(player, "loyalist", 20);

                                        return;
                                        }

                                        if (_ai.NeedDamage(damage) && player.GetCardCount(true) - good > 1)
                                        {
                                            if (ai.GetPlayerTendency(p) == "lord" || ai.GetPlayerTendency(p) == "loyalist")
                                                ai.UpdatePlayerIntention(player, "rebel", 40);
                                            else
                                                ai.UpdatePlayerIntention(player, "loyalist", 40);
                                        }
                                    }

                                    if (_ai.NeedDamage(damage))
                                        ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 60);
                                    else
                                        ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
                                }
                            }
                        }
                }
            }
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            if (ai.WillShowForAttack() && !player.HasUsed(KuangfuCard.ClassName))
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasEquip() && RoomLogic.CanDiscard(room, player, p, "e"))
                        return new List<WrappedCard> { new WrappedCard(KuangfuCard.ClassName) { Skill = Name, ShowSkill = Name } };
            }
            return new List<WrappedCard>();
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            if (player.HasFlag(Name))
            {
                if (ai.Target[Name] != null)
                    return new List<Player> { ai.Target[Name] };
            }
            else
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    Skill = "_kuangfu_jx",
                    DistanceLimited = false
                };
                List<WrappedCard> cards = new List<WrappedCard> { slash };
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, cards, null, false);
                if (scores.Count > 0)
                {
                    return new List<Player> { scores[0].Players[0] };
                }
            }

            return new List<Player>();
        }
    }

    public class KuangfuCardAI : UseCard
    {
        public KuangfuCardAI() : base(KuangfuCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Target["kuangfu_jx"] = null;
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasEquip() && RoomLogic.CanDiscard(room, player, p, "e"))
                {
                    ScoreStruct score = ai.FindCards2Discard(player, p, "kuangfu_jx", "e", FunctionCard.HandlingMethod.MethodDiscard);
                    score.Players = new List<Player> { p };
                }
            }

            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    ai.Target["kuangfu_jx"] = scores[0].Players[0];
                    use.Card = card;
                    return;
                }
            }
            else if (player.HasEquip() && RoomLogic.CanDiscard(room, player, player, "e"))
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    Skill = "_kuangfu_jx",
                    DistanceLimited = false
                };
                List<WrappedCard> cards = new List<WrappedCard> { slash };
                scores = ai.CaculateSlashIncome(player, cards, null, false);
                if (scores.Count > 0 && scores[0].Score >= 4)
                {
                    ai.Target["kuangfu_jx"] = player;
                    use.Card = card;
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class HuoshuiAI : SkillEvent
    {
        public HuoshuiAI() : base("huoshui")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !RoomLogic.PlayerHasShownSkill(ai.Room, player, Name))
                return new List<WrappedCard> { new WrappedCard("HuoshuiCard") { Skill = Name, ShowSkill = Name, Mute = true } };

            return null;
        }

    }

    public class HuoshuiCardAI : UseCard
    {
        public HuoshuiCardAI() : base("HuoshuiCardAI")
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 10;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            foreach (Player p in ai.GetEnemies(player))
            {
                if (!p.HasShownAllGenerals())
                {
                    use.Card = card;
                    return;
                }
            }
        }
    }

    public class QingchengAI : SkillEvent
    {
        public QingchengAI() : base("qingcheng")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            ai.Target[Name] = null;
            if (ai.WillShowForAttack() && !player.HasUsed("QingchengCard"))
            {
                Room room = ai.Room;
                List<Player> enemies = ai.GetPrioEnemies();
                if (enemies.Count > 0)
                {
                    Player target = null;
                    ai.SortByDefense(ref enemies, false);
                    foreach (Player p in enemies)
                    {
                        if (p.HasShownAllGenerals() &&
                            RoomLogic.PlayerHasShownSkill(room, p, string.Format("{0}|{1}|{2}", TrustedAI.MasochismSkill, TrustedAI.DefenseSkill, TrustedAI.SaveSkill)))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (target != null)
                    {
                        List<int> ids = new List<int>();
                        foreach (int id in player.GetCards("he"))
                            if (WrappedCard.IsBlack(room.GetCard(id).Suit) && RoomLogic.CanDiscard(room, player, player, id))
                                ids.Add(id);

                        if (ids.Count > 0)
                        {
                            ai.SortByUseValue(ref ids, false);
                            if (ai.GetUseValue(ids[0], player) < 5)
                            {
                                ai.Target[Name] = target;
                                WrappedCard first = new WrappedCard("QingchengCard");
                                first.AddSubCard(ids[0]);
                                first.ShowSkill = Name;
                                first.Skill = Name;
                                return new List<WrappedCard> { first };
                            }
                        }
                    }
                }
            }

            return null;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            bool head = false;
            bool deputy = false;
            bool first = true;
            string[] generals = choice.Split('+');
            string[] skills = string.Format("{0}|{1}|{2}", TrustedAI.MasochismSkill, TrustedAI.DefenseSkill, TrustedAI.SaveSkill).Split('|');
            foreach (string general in generals)
            {
                General g = Engine.GetGeneral(general, ai.Room.Setting.GameMode);
                foreach (string skill in skills)
                {
                    if (g.HasSkill(skill, ai.Room.Setting.GameMode, first))
                    {
                        if (first)
                            head = true;
                        else
                            deputy = true;
                        break;
                    }
                }
                first = false;
            }

            if (head && deputy)
            {
                return Engine.GetGeneralValue(generals[0], ai.Room.Setting.GameMode) > Engine.GetGeneralValue(generals[1], ai.Room.Setting.GameMode)
                    ? generals[0] : generals[1];
            }

            return head ? generals[0] : generals[1];
        }


        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            List<Player> enemies = ai.GetPrioEnemies();
            foreach (Player p in enemies)
            {
                if (p.HasShownAllGenerals() && target.Contains(p) &&
                    RoomLogic.PlayerHasShownSkill(room, p, string.Format("{0}|{1}|{2}", TrustedAI.MasochismSkill, TrustedAI.DefenseSkill, TrustedAI.SaveSkill)))
                {
                    return new List<Player> { p };
                }
            }

            foreach (Player p in target)
                if (ai.IsEnemy(p))
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class QingchengCardAI : UseCard
    {
        public QingchengCardAI() : base("QingchengCard")
        { }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (ai.Target["qingcheng"] != null)
            {
                use.Card = card;
                use.To = new List<Player> { ai.Target["qingcheng"] };
            }
        }
    }
}