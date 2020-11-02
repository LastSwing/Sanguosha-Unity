using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System;
using System.Collections.Generic;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    class ClassicStanderCardsAI : AIPackage
    {
        public ClassicStanderCardsAI() : base("ClassicStanderCards")
        {
            events = new List<SkillEvent>
            {
                new HoneyTrapAI2(),
                new QuenchedKnifeSkillAI(),
            };
            use_cards = new List<UseCard>
            {
                new ClassicBladeAI(),
                new ClassicHalberdAI(),
                new SaberAI(),          //七宝刀
                new HiddenDaggerAI(),   //笑里藏刀
                new HoneyTrapAI(),      //美人计
                new LanceAI(),
                new PosionedDaggerAI(),
                new QuenchedKnifeAI(),
                new LightningSummonerAI(),
                new WaterSwordAI(),
            };
        }
    }

    public class ClassicBladeAI : UseCard
    {
        public ClassicBladeAI() : base(ClassicBlade.ClassName)
        {}
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
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return ai.AjustWeaponRangeValue(player, card) - (8 - ai.Room.AliveCount()) * 0.5;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class SaberAI : UseCard
    {
        public SaberAI() : base(Saber.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            foreach (Player p in ai.GetEnemies(player))
            {
                if (RoomLogic.HasShownArmorEffect(ai.Room, p) && RoomLogic.DistanceTo(ai.Room, player, p, null, true) <= 2)
                    value += 0.5;
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class HiddenDaggerAI : UseCard
    {
        public HiddenDaggerAI() : base(HiddenDagger.ClassName)
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;

            if (positive)
            {
                ScoreStruct score = new ScoreStruct
                {
                    Score = 0
                };
                if (from.Alive)
                {
                    DamageStruct damage = new DamageStruct(trick, from, to);
                    score = ai.GetDamageScore(damage);
                }

                if (to.IsWounded())
                {
                    if (ai.IsFriend(to))
                        score.Score += 1 * to.GetLostHp();
                    else if (ai.IsEnemy(to))
                        score.Score -= 1 * to.GetLostHp();
                }

                if (score.Score <= -4) result.Null = true;
            }
            else
            {
                ScoreStruct score = new ScoreStruct
                {
                    Score = 0
                };
                if (from.Alive)
                {
                    DamageStruct damage = new DamageStruct(trick, from, to);
                    score = ai.GetDamageScore(damage);
                }

                if (to.IsWounded())
                {
                    if (ai.IsFriend(to))
                        score.Score += 1 * to.GetLostHp();
                    else if (ai.IsEnemy(to))
                        score.Score -= 1 * to.GetLostHp();
                }

                if (score.Score > 4) result.Null = true;
            }

            return result;
        }
    }

    public class HoneyTrapAI2 : SkillEvent
    {
        public HoneyTrapAI2() : base(HoneyTrap.ClassName)
        {
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = room.Current;
            List<int> ids = player.GetCards("h");
            ai.SortByKeepValue(ref ids, false);
            if (ai.IsFriend(target))
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { target });
                if (pair.Key != null && ids.Contains(pair.Value))
                    return new List<int> { pair.Value };
            }

            return new List<int> { ids[0] };
        }
    }

    public class HoneyTrapAI : UseCard
    {
        public HoneyTrapAI() : base(HoneyTrap.ClassName)
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;

            List<Player> players = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.IsFemale()) players.Add(p);

            int count = Math.Min(players.Count, to.HandcardNum);
            if (positive)
            {
                if (ai.IsFriend(to))
                {
                    double value = -count;
                    if (from.HandcardNum + count < to.HandcardNum)
                    {
                        DamageStruct damage = new DamageStruct(trick, from, to);
                        value += ai.GetDamageScore(damage).Score;
                    }

                    if (count <= -2) result.Null = true;
                }
            }
            else
            {
                if (ai.IsFriend(from))
                {
                    double value = count;
                    if (count > 0)
                    {
                        room.SortByActionOrder(ref players);
                        for (int i = 0; i < count; i++)
                            if (ai.IsFriend(players[i]) && players[i].HandcardNum > 0)
                                value += players[i].HandcardNum / 5;
                    }
                    if (from.HandcardNum + count < to.HandcardNum)
                    {
                        DamageStruct damage = new DamageStruct(trick, from, to);
                        value += ai.GetDamageScore(damage).Score;
                    }

                    if (count > 3) result.Null = true;
                }
            }

            return result;
        }
    }

    public class LanceAI : UseCard
    {
        public LanceAI() : base(Lance.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            if (player.IsWounded()) value += 1;
            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class PosionedDaggerAI : UseCard
    {
        public PosionedDaggerAI() : base(PosionedDagger.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            foreach (Player p in ai.GetEnemies(player))
                if (ai.HasSkill(TrustedAI.MasochismSkill, p)) value += 1;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
        /*
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (data is Player target && room.GetTag(Name) is CardUseStruct use)
            {
                if (ai.HasSkill("zhaxiang", target))
                {
                    if (ai.IsFriend(target) && target.Hp > 1 && !ai.WillSkipPlayPhase(target))
                        return true;
                    else if (target.Hp > 1)
                        return false;
                }

                if (ai.IsEnemy(target) && ai.IsCardEffect(use.Card, target, player))
                {
                    if (use.EffectCount[0].Effect2 == 1 && ai.IsLackCard(target, Jink.ClassName) && ai.HasSkill("qingguo|longdan|longdan_jx", target))
                    {
                        DamageStruct damage = new DamageStruct(use.Card, player, target, 1 + use.Drank + use.EffectCount[0].Effect1);
                        if (use.Card.Name == ThunderSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Thunder;
                        if (use.Card.Name == FireSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Fire;
                        if (ai.DamageEffect(damage, DamageStruct.DamageStep.Caused) > 1)
                            return false;
                    }
                }

                return ai.IsEnemy(target);
            }

            return false;
        }
        */
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target) return ai.IsEnemy(target);
            return false;
        }
    }

    public class QuenchedKnifeSkillAI : SkillEvent
    {
        public QuenchedKnifeSkillAI() : base(QuenchedKnife.ClassName) { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is DamageStruct damage)
            {
                ScoreStruct score = ai.GetDamageScore(damage);
                damage.Damage++;
                ScoreStruct _score = ai.GetDamageScore(damage);
                if (_score.Score - score.Score > 3)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("h"))
                    {
                        if (!RoomLogic.CanDiscard(room, player, player, id)) continue;
                        WrappedCard card = room.GetCard(id);
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        if (fcard is Slash || fcard is Weapon)
                            ids.Add(id);
                    }

                    if (ids.Count > 0)
                    {
                        ai.SortByUseValue(ref ids, false);
                        return new List<int> { ids[0] };
                    }
                }
            }

            return new List<int>();
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.From != null && damage.Card != null && damage.From.HasWeapon(Name) && damage.Card.Name.Contains(Slash.ClassName) && !damage.Transfer && !damage.Chain)
            {
                Room room = ai.Room;
                List<int> ids = ai.GetKnownCards(damage.From);
                foreach (int id in ids)
                {
                    if (damage.Card.SubCards.Contains(id)) continue;
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is Slash || fcard is Weapon)
                    {
                        damage.Damage++;
                        break;
                    }
                }
            }
        }
    }

    public class QuenchedKnifeAI : UseCard
    {
        public QuenchedKnifeAI() : base(QuenchedKnife.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class LightningSummonerAI : UseCard
    {
        public LightningSummonerAI() : base(LightningSummoner.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            foreach (Player p in ai.GetFriends(player))
                if (ai.HasSkill("guidao|guicai_jx|zhenyi", p)) value += 5;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                if (ai.IsEnemy(target))
                {
                    DamageStruct damage = new DamageStruct(Name, null, target, 3, DamageStruct.DamageNature.Thunder);
                    if (ai.GetDamageScore(damage).Score > 0) return true;
                }
            }

            return false;
        }
    }
    public class WaterSwordAI : UseCard
    {
        public WaterSwordAI() : base(WaterSword.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            return value;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag("extra_target_skill") is CardUseStruct use)
            {
                List<Player> result = new List<Player>();
                if (use.Card.Name == ExNihilo.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p))
                            return new List<Player> { p };
                }
                else if (use.Card.Name.Contains(Slash.ClassName))
                {
                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { use.Card }, targets, false);
                    if (scores.Count > 0 && scores[0].Score > 0)
                        return scores[0].Players;
                }
                else if (use.Card.Name == Snatch.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.FindCards2Discard(player, p, use.Card.Name, "hej", HandlingMethod.MethodGet).Score > 0)
                            return new List<Player> { p };
                    }
                }
                else if (use.Card.Name == Dismantlement.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.FindCards2Discard(player, p, use.Card.Name, "hej", HandlingMethod.MethodDiscard).Score > 0)
                            return new List<Player> { p };
                    }
                }
                else if (use.Card.Name == IronChain.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.IsFriend(p) && !ai.HasSkill("jieying", p) && p.Chained)
                            return new List<Player> { p };
                    }
                    foreach (Player p in targets)
                    {
                        if (ai.IsEnemy(p) && !p.Chained)
                            return new List<Player> { p };
                    }
                }
                else if (use.Card.Name == FireAttack.ClassName)
                {
                    foreach (Player p in targets)
                    {
                        if (ai.IsEnemy(p)) return new List<Player> { p };
                    }
                }
                else if (use.Card.Name == Duel.ClassName)
                {
                    WrappedCard duel = use.Card;
                    List<Player> enemies = ai.Exclude(ai.GetEnemies(player), duel);
                    List<Player> friends = ai.Exclude(ai.FriendNoSelf, duel);
                    int n1 = ai.GetKnownCardsNums(Slash.ClassName, "he", player);

                    if (ai.HasSkill("wushuang")) n1 *= 2;
                    List<ScoreStruct> scores = new List<ScoreStruct>();
                    foreach (Player p in friends)
                    {
                        if (!targets.Contains(p)) continue;
                        bool fuyin = false;
                        if (ai.HasSkill("fuyin", p) && p.GetMark("fuyin") == 0)
                        {
                            int count = player.HandcardNum;
                            if (count > p.HandcardNum)
                                fuyin = true;

                        }
                        if (!fuyin)
                        {
                            ScoreStruct score = ai.GetDamageScore(new DamageStruct(duel, player, p));
                            score.Players = new List<Player> { p };
                            foreach (string skill in ai.GetKnownSkills(p))
                            {
                                SkillEvent skill_e = Engine.GetSkillEvent(skill);
                                if (skill_e != null)
                                    score.Score += skill_e.TargetValueAdjust(ai, duel, player, new List<Player> { p }, p);
                            }
                            scores.Add(score);
                        }
                    }
                    foreach (Player p in enemies)
                    {
                        if (!targets.Contains(p)) continue;
                        bool fuyin = false;
                        if (ai.HasSkill("fuyin", p) && p.GetMark("fuyin") == 0)
                        {
                            int count = player.HandcardNum;
                            foreach (int id in duel.SubCards)
                                if (room.GetCardPlace(id) == Place.PlaceHand)
                                    count--;

                            if (count > p.HandcardNum)
                            {
                                ScoreStruct score = new ScoreStruct
                                {
                                    Score = 1,
                                    Players = new List<Player> { p }
                                };
                                scores.Add(score);
                                fuyin = true;
                            }
                        }

                        if (!fuyin)
                        {
                            bool no_red = p.GetMark("@qianxi_red") > 0;
                            bool no_black = p.GetMark("@qianxi_black") > 0;
                            double n2 = ai.GetKnownCardsNums(Slash.ClassName, "he", p, player);

                            bool fuqi = false;
                            if (player.ContainsTag("wenji") && player.GetTag("wenji") is List<string> names && names.Contains(Name))
                                fuqi = true;
                            if (ai.HasSkill("fuqi", player) && RoomLogic.DistanceTo(room, player, p) == 1)
                                fuqi = true;

                            if (!fuqi && !ai.IsLackCard(p, Slash.ClassName))
                            {
                                int rate = 4;
                                if (ai.GetKnownCards(p).Count != p.HandcardNum)
                                {
                                    rate = 5;
                                    if (ai.HasSkill("longdan", p))
                                    {
                                        rate -= 2;
                                        if (no_black || no_red)
                                            rate += 1;
                                    }
                                    if (ai.HasSkill("wusheng", p) && !no_red)
                                        rate -= 2;
                                    int count = p.HandcardNum - ai.GetKnownCards(p).Count;
                                    count += p.GetHandPile(true).Count - ai.GetKnownHandPileCards(p).Count;
                                    if (no_red)
                                    {
                                        rate += 1;
                                    }
                                    if (no_black)
                                        rate += 2;
                                    n2 += ((double)count / rate);
                                }
                                if (ai.HasSkill("wushuang", p)) n2 *= 2;
                            }
                            ScoreStruct score = new ScoreStruct
                            {
                                Players = new List<Player> { p }
                            };
                            if (fuqi)
                            {
                                score.Score = ai.GetDamageScore(new DamageStruct(duel, player, p)).Score;
                            }
                            else if (n2 > n1)
                            {
                                score.Score = ai.GetDamageScore(new DamageStruct(duel, p, player)).Score;
                            }
                            else
                            {
                                score.Score = ai.GetDamageScore(new DamageStruct(duel, p, player)).Score - (n2 - 1) * 0.4;
                            }

                            foreach (string skill in ai.GetKnownSkills(p))
                            {
                                SkillEvent skill_e = Engine.GetSkillEvent(skill);
                                if (skill_e != null)
                                    score.Score += skill_e.TargetValueAdjust(ai, duel, player, new List<Player> { p }, p);
                            }
                            scores.Add(score);
                        }
                    }
                    if (scores.Count > 0)
                    {
                        scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                        if (scores[0].Score > 1)
                            return scores[0].Players;
                    }
                    
                }
            }
            return new List<Player>();
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
}
