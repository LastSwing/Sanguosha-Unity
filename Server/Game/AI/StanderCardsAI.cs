using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static CommonClass.Game.WrappedCard;

namespace SanguoshaServer.AI
{
    class StanderCardsAI : AIPackage
    {
        public StanderCardsAI() : base("StanderCards")
        {
            use_cards = new List<UseCard>
            {
                new SlashAI(),
                new FireSlashAI(),
                new ThunderSlashAI(),
                new PeachAI(),
                new AnalepticAI(),
                new DismantlementAI(),
                new SnatchAI(),
                new DuelAI(),
                new CollateralAI(),
                new ExNihiloAI(),
                new AwaitExhaustedAI(),
                new BefriendAttackingAI(),
                new FireAttackAI(),
                new KnownBothAI(),
                new IronChainAI(),
                new SavageAssaultAI(),
                new ArcheryAttackAI(),
                new IndulgenceAI(),
                new SupplyShortageAI(),
                new LightningAI(),
                new CrossBowAI(),
                new DoubleSwordAI(),
                new QinggangSwordAI(),
                new IceSwordAI(),
                new SpearAI(),
                new AxeAI(),
                new KylinBowAI(),
                new FanAI(),
                new SixSwordsAI(),
                new TribladeAI(),
                new SilverLionAI(),
                new EightDiagramAI(),
                new RenwangShieldAI(),
                new VineAI(),
                new DHorseAI("Jueying"),
                new DHorseAI("Dilu"),
                new DHorseAI("Zhuahuangfeidian"),
                new OHorseAI("Chitu"),
                new OHorseAI("Dayuan"),
                new OHorseAI("Zixing"),
                new PioneerCardAI(),
                new CompanionCardAI(),
                new MegatamaCardAI(),
            };

            events = new List<SkillEvent>
            {
                new CompanionAI(),
                new MegatamaAI(),
                new PioneerAI(),
            };
        }
    }
    public class SlashAI : UseCard
    {
        public SlashAI() : base("Slash")
        {
            key = new List<string> { "cardResponded" };
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.Steped < DamageStruct.DamageStep.Caused && step >= DamageStruct.DamageStep.Caused && damage.From != null && damage.From.Alive)
                damage.Damage += damage.From.GetMark("drank");
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                        ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
            }

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_")
                    {
                        string prompt = strs[3];
                        if (!prompt.StartsWith("@multi-jink") || strs[strs.Count - 2] == "1")
                        {
                            List<CardUseStruct> uses = (List<CardUseStruct>)room.GetTag("card_proceeing");
                            use = uses[uses.Count - 1];
                            if (use.Card != null && use.Card.Name.Contains("Slash"))
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, player);
                                if (use.Card.Name == "FireSlash")
                                    damage.Nature = DamageStruct.DamageNature.Fire;
                                else if (use.Card.Name == "ThunderSlash")
                                    damage.Nature = DamageStruct.DamageNature.Thunder;

                                ScoreStruct score = ai.GetDamageScore(damage);
                                bool lack = true;
                                if (ai.IsFriend(use.From, player) && score.Score > 0)
                                    lack = false;
                                else if (ai.IsEnemy(player) && score.Score < 0)
                                    lack = false;

                                if (lack) ai.SetCardLack(player, "Jink");
                            }
                        }
                    }
                }
            }
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            if (prompt.StartsWith("slash-jink") || prompt.StartsWith("@multi-jink"))
            {
                SlashEffectStruct effect = (SlashEffectStruct)data;
                DamageStruct damage = new DamageStruct(effect.Slash, effect.From, effect.To);
                if (effect.Slash.Name == "FireSlash")
                    damage.Nature = DamageStruct.DamageNature.Fire;
                else if (effect.Slash.Name == "ThunderSlash")
                    damage.Nature = DamageStruct.DamageNature.Thunder;
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Damage.Damage > 0 && player.Chained && score.Damage.Nature != DamageStruct.DamageNature.Normal)
                {
                    if (ai.IsGoodSpreadStarter(damage))
                        score.Score += 6;
                    else if (ai.IsGoodSpreadStarter(damage, false))
                        score.Score -= 8;
                }

                CardUseStruct use = new CardUseStruct
                {
                    From = player
                };
                List<WrappedCard> cards = ai.GetCards("Jink", player);
                if (score.Score == 0 || cards.Count == 0) return use;

                int rest = 1;
                if (prompt.StartsWith("@multi-jink"))
                {
                    List<string> strs = new List<string>(prompt.Split(':'));
                    rest = int.Parse(strs[strs.Count - 1]);
                }
                WrappedCard result = null;
                int available = 0;
                foreach (WrappedCard jink in cards)
                {
                    if (!RoomLogic.IsCardLimited(ai.Room, player, jink, FunctionCard.HandlingMethod.MethodUse))
                    {
                        available++;
                        result = jink;
                    }
                }
                if (result == null) result = cards[0];

                int rate = 1;
                if (ai.WillSkipPlayPhase(player) || !ai.WillShowForAttack() || ai.GetOverflow(player) > 0) rate = 2;
                double value = -ai.GetKeepValue(result, player);
                if (value < 0) value /= rate;
                if (ai.HasSkill("leiji", player)) value += 2;
                if (result.Skill == "longdan")
                {
                    if (ai.HasSkill("chongzhen", player) && !ai.IsFriend(player, effect.From) && !effect.From.IsKongcheng())
                        value += 2;

                    foreach (Player p in ai.Room.GetOtherPlayers(player))
                    {
                        if (p.IsWounded() && ai.IsFriend(p, player))
                            value += 3;
                    }
                }

                if (damage.Damage > 0 && score.Score < -2 && ai.HasSkill("tianxiang", player))
                {
                    SkillEvent tianxiang = Engine.GetSkillEvent("tianxiang");
                    if (tianxiang != null)
                    {
                        CardUseStruct tianxiang_use = tianxiang.OnResponding(ai, player, "@@tianxiang", string.Empty, damage);
                        if (tianxiang_use.Card != null && tianxiang_use.To != null && tianxiang_use.To.Count > 0)
                            score.Score = -2;
                    }
                }

                if (rest > 1)
                {
                    double need = rest;
                    if (ai.HasArmorEffect(player, "EightDiagram"))
                        need = (rest - 1) * 0.6 + 1;

                    if (available >= need)
                        value -= 4 * (int)(need - 1);
                    else
                        value += score.Score * Math.Min(1, need - 1);
                }

                if (value >= score.Score) use.Card = result;

                return use;
            }

            return new CardUseStruct();
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use = ai.FindSlashandTarget(player);
        }
    }

    public class FireSlashAI : UseCard
    {
        public FireSlashAI() : base("FireSlash")
        {
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.Steped < DamageStruct.DamageStep.Caused && step >= DamageStruct.DamageStep.Caused && damage.From != null && damage.From.Alive)
                damage.Damage += damage.From.GetMark("drank");
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use = ai.FindSlashandTarget(player);
        }
    }
    public class ThunderSlashAI : UseCard
    {
        public ThunderSlashAI() : base("ThunderSlash")
        {
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.Steped < DamageStruct.DamageStep.Caused && step >= DamageStruct.DamageStep.Caused && damage.From != null && damage.From.Alive)
                damage.Damage += damage.From.GetMark("drank");
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use = ai.FindSlashandTarget(player);
        }
    }

    public class AnalepticAI : UseCard
    {
        public AnalepticAI() : base("Analeptic")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            bool hand = false;
            //当酒为手牌时且溢出并且不在保留手牌中时，若敌方有二张，则应将其用掉
            foreach (int id in card.SubCards)
            {
                if (ai.Room.GetCardOwner(id) == player && ai.Room.GetCardPlace(id) == Player.Place.PlaceHand)
                {
                    hand = true;
                    break;
                }
            }
            if (hand && player.Phase == Player.PlayerPhase.Play && ai.GetOverflow(player) >= 2)
            {
                bool enemy_erzhang = false;
                foreach (Player p in ai.Room.GetOtherPlayers(player))
                {
                    if (ai.HasSkill("guzheng", p))
                    {
                        if (ai.IsEnemy(p, player))
                        {
                            enemy_erzhang = true;
                            break;
                        }
                    }
                }
                if (enemy_erzhang)
                {
                    bool should_use = false;
                    List<int> hands = new List<int>(player.HandCards);
                    ai.SortByKeepValue(ref hands, false);
                    foreach (int id in card.SubCards)
                    {
                        if (hands.IndexOf(id) < ai.GetOverflow(player) - 1)
                        {
                            should_use = true;
                            break;
                        }
                    }

                    if (should_use)
                    {
                        use.Card = card;
                        return;
                    }
                }
            }

            //若差一张牌可发动吉利则使用该酒
            if (ai.HasSkill("jili", player))
            {
                int range = RoomLogic.GetAttackRange(ai.Room, player, true);
                if (range - player.GetMark("jili") == 1)
                {
                    use.Card = card;
                    return;
                }
            }
        }
    }

    public class PeachAI : UseCard
    {
        public PeachAI() : base("Peach")
        {
            key = new List<string> { "peach" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //判断出桃子救援的行为
            if (triggerEvent == TriggerEvent.ChoiceMade && ai.Self != player && data is string str)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                Player dying = room.FindPlayer(strs[1]);
                if (player != dying)
                {
                    if (ai.IsKnown(ai.Self, player) || ai.IsKnown(ai.Self, dying))
                    {
                        if (ai.IsKnown(ai.Self, player) && !ai.IsKnown(ai.Self, dying))
                            ai.UpdatePlayerIntention(dying, player.Kingdom, 50);
                        else if (ai.IsKnown(ai.Self, dying) && !ai.IsKnown(ai.Self, player))
                            ai.UpdatePlayerIntention(player, dying.Kingdom, 50);
                    }
                    else
                        ai.UpdatePlayerRelation(player, dying, true);
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (!player.IsWounded()) return;

            bool mustusepeach = false;
            bool hand = false;
            bool wooden = false;
            int peaches = 0;
            Room room = ai.Room;
            foreach (int id in player.HandCards)
            {
                if (ai.IsCard(id, "Peach", player))
                    peaches++;
            }
            foreach (int id in card.SubCards)
            {
                if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == player)
                    hand = true;
                if (player.GetPile("wooden_ox").Contains(id))
                    wooden = true;
            }

            //进入鏖战前将桃吃完
            if (room.AliveCount() <= room.Players.Count / 2 && room.Setting.GameMode == "Hegemony" && !RoomLogic.IsVirtualCard(room, card))
            {
                bool check = true;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (RoomLogic.GetPlayerNumWithSameKingdom(room, p) > 1)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    use.Card = card;
                    return;
                }
            }

            //if (ai.HasSkill("rende", player) && ai.FindFriendsByType(sgs.Friend_Draw)) return;
            //有偷桃、拆、顺的敌人则不留
            if (hand && player.HandcardNum < 3)
            {
                foreach (Player enemy in ai.GetEnemies(player))
                {
                    if (hand && ai.HasSkill("qiaobian|tuxi", enemy) && !ai.WillSkipDrawPhase(enemy))
                    {
                        mustusepeach = true;
                        break;
                    }
                    if ((hand || wooden) && !ai.WillSkipPlayPhase(enemy))
                    {
                        if ((ai.GetKnownCardsNums("Dismantlement", "he", enemy, player) > 0 || ai.HasSkill("qixi", enemy))
                            || (RoomLogic.DistanceTo(room, enemy, player) == 1 && (ai.GetKnownCardsNums("Snatch", "he", enemy, player) > 0 || ai.HasSkill("jixi", enemy)))
                            || (ai.HasSkill("jianchu", enemy) && RoomLogic.CanSlash(room, enemy, player))
                            || (ai.HasSkill("tiaoxin", enemy) && RoomLogic.InMyAttackRange(room, player, enemy)
                            && (ai.GetKnownCardsNums("Slash", "he", player) == 0 || !RoomLogic.CanSlash(room, player, enemy))))
                        {
                            mustusepeach = true;
                            break;
                        }
                    }
                }
            }
            int maxCards = ai.GetOverflow(player);
            bool overflow = maxCards > 0;
            if (ai.HasSkill("nos_buqu") && player.Hp < 1 && maxCards == 0)
            {
                use.Card = card;
                return;
            }

            if (mustusepeach || peaches > maxCards || player.Hp == 1)
            {
                use.Card = card;
                return;
            }
            //如果桃子不是保留的卡牌则吃掉
            if (hand && overflow)
            {
                List<int> hands = new List<int>(player.HandCards);
                ai.SortByKeepValue(ref hands, false);
                foreach (int id in card.SubCards)
                {
                    if (hands.Contains(id) && hands.IndexOf(id) < maxCards - 1)
                    {
                        use.Card = card;
                        return;
                    }
                }
            }
            //为队友保留桃子
            foreach (Player p in ai.FriendNoSelf)
            {
                if (ai.IsWeak(p))
                    return;
            }

            use.Card = card;
        }
    }

    public class AmazingGraceAI : UseCard
    {
        public AmazingGraceAI() : base("AmazingGrace")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            double value = 1;
            double suf = 0.8, coeff = 0.8;
            if (ai.NeedKongcheng(player) && player.IsLastHandCard(card, true))
            {
                suf = 0.6;
                coeff = 0.6;
            }
            foreach (Player p in ai.Room.GetOtherPlayers(player))
            {
                int index = 0;
                if (!ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                {
                    if (ai.IsFriend(p, player))
                        index = 1;
                    else
                        index = -1;
                    value += index * suf;
                }
                if (value < 0) return;
                suf *= coeff;
            }

            use.Card = card;
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
            delete = new List<Player>(targets);

            double before = 0, after = 0;
            List<int> ag_card = (List<int>)room.GetTag("AmazingGrace");
            foreach (Player p in targets)
            {
                double best = -100;
                int best_id = -1;
                foreach (int id in ag_card)
                {
                    double value;
                    if (room.Current == p)
                        value = ai.GetUseValue(id, p, Player.Place.PlaceHand);
                    else
                        value = ai.GetKeepValue(id, p, Player.Place.PlaceHand);

                    if (value > best)
                    {
                        best_id = id;
                        best = value;
                    }
                }
                ag_card.Remove(best_id);
                if (ai.IsFriend(p))
                    before += best;
                else
                    before -= best;
            }

            if (positive)
            {
                if (ai.IsEnemy(to))
                {
                    if (room.NullTimes == 0 && ai.GetKnownCardsNums("HegNullification", "he", player) > 0 || room.HegNull)
                    {
                        foreach (Player p in delete)
                        {
                            if (ai.IsEnemy(p) && RoomLogic.IsFriendWith(room, p, to))
                            {
                                targets.Remove(p);
                                if (to != p)
                                    result.Heg = true;
                            }
                        }
                    }
                }
                ag_card = (List<int>)room.GetTag("AmazingGrace");
                foreach (Player p in targets)
                {
                    double best = -100;
                    int best_id = -1;
                    foreach (int id in ag_card)
                    {
                        double value;
                        if (room.Current == p)
                            value = ai.GetUseValue(id, p, Player.Place.PlaceHand);
                        else
                            value = ai.GetKeepValue(id, p, Player.Place.PlaceHand);

                        if (value > best)
                        {
                            best_id = id;
                            best = value;
                        }
                    }
                    ag_card.Remove(best_id);
                    if (ai.IsFriend(p))
                        after += best;
                    else
                        after -= best;
                }

                if (after - before > 5 && !keep || after - before > 8)
                    result.Null = true;
            }
            else
            {
                if (ai.IsFriend(to))
                {
                    targets.Remove(to);
                    foreach (Player p in delete)
                    {
                        if (room.HegNull && RoomLogic.IsFriendWith(room, p, to))
                            targets.Remove(p);
                    }

                    ag_card = (List<int>)room.GetTag("AmazingGrace");
                    foreach (Player p in targets)
                    {
                        double best = -100;
                        int best_id = -1;
                        foreach (int id in ag_card)
                        {
                            double value;
                            if (room.Current == p)
                                value = ai.GetUseValue(id, p, Player.Place.PlaceHand);
                            else
                                value = ai.GetKeepValue(id, p, Player.Place.PlaceHand);

                            if (value > best)
                            {
                                best_id = id;
                                best = value;
                            }
                        }
                        ag_card.Remove(best_id);
                        if (ai.IsFriend(p))
                            after += best;
                        else
                            after -= best;
                    }

                    if (before - after > 5 && !keep || before - after > 8)
                        result.Null = true;
                }
            }

            return result;
        }
    }

    public class GodSalvationAI : UseCard
    {
        public GodSalvationAI() : base("GodSalvation")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            double good = 0, bad = 0;
            int wounded_friend = 0;
            if ((!RoomLogic.IsVirtualCard(room, card) || card.SubCards.Count == 0) && ai.HasSkill("jizhi", player))
                good += 6;
            if ((ai.HasSkill("kongcheng", player) || ai.HasLoseHandcardEffective()) && player.IsLastHandCard(card, true))
                good += 5;
            foreach (Player friend in ai.GetFriends(player))
            {
                good += 10 * ai.GetKnownCardsNums("Nullification", "he", friend, player);
                if (!ai.IsCancelTarget(card, friend, player) && ai.IsCardEffect(card, friend, player))
                {
                    if (friend.IsWounded())
                    {
                        wounded_friend++;
                        good += 10;
                        if (ai.HasSkill(TrustedAI.MasochismSkill, friend))
                            good += 5;
                        if (ai.IsWeak(friend) && friend.Hp <= 1)
                            good += 5;
                        if (ai.NeedToLoseHp(new DamageStruct(string.Empty, null, friend), true, true))
                            good -= 3;
                    }
                }
            }

            foreach (Player enemy in ai.GetEnemies(player))
            {
                good += 10 * ai.GetKnownCardsNums("Nullification", "he", enemy, player);
                if (!ai.IsCancelTarget(card, enemy, player) && ai.IsCardEffect(card, enemy, player))
                {
                    if (enemy.IsWounded())
                    {
                        bad += 10;
                        if (ai.HasSkill(TrustedAI.MasochismSkill, enemy))
                            bad += 5;
                        if (ai.IsWeak(enemy) && enemy.Hp <= 1)
                            bad += 5;
                        if (ai.NeedToLoseHp(new DamageStruct(string.Empty, null, enemy), true, true))
                            bad -= 3;
                    }
                }
            }

            if (good - bad > 5 && wounded_friend > 0)
            {
                use.Card = card;
            }
        }
    }

    public class DismantlementAI : UseCard
    {
        public DismantlementAI() : base("Dismantlement")
        {
            key = new List<string> { "cardChosen" };
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            FunctionCard fcard = Engine.GetFunctionCard(Name);
            List<Player> targets = ai.Exclude(room.GetOtherPlayers(player), card, player);
            foreach (Player p in targets)
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, "hej", FunctionCard.HandlingMethod.MethodDiscard);
                room.OutPut(card.Name + p.SceenName + " is " + score.Score.ToString() + " friend ? " + ai.IsFriend(p).ToString());
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                bool hand = false;
                foreach (int id in card.SubCards)
                {
                    if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == player)
                        hand = true;
                }

                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 4 || (scores[0].Score > 0 && hand && ai.GetOverflow(player) > 0))
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                }
            }
        }
        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids, object data)
        {
            return ai.FindCards2Discard(from, to, "hej", FunctionCard.HandlingMethod.MethodDiscard).Ids;
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            Room room = ai.Room;
            NulliResult result = new NulliResult();
            if (!from.Alive || !to.Alive || to.IsAllNude() || keep) return result;

            if (positive)
            {
                if ((RoomLogic.PlayerContainsTrick(room, to, "Indulgence") || RoomLogic.PlayerContainsTrick(room, to, "SupplyShortage"))
                        && ai.IsFriend(to) && to.IsNude()) return result;

                if (ai.IsEnemy(from) && ai.IsFriend(from, to) && RoomLogic.GetPlayerCards(room, to, "j").Count > 0)
                {
                    result.Null = true;
                    return result;
                }
                else if (ai.IsFriend(from) && ai.IsFriend(to))
                    return result;

                else if (ai.IsFriend(to))
                {
                    bool use = false;
                    if (to.HasTreasure("JadeSeal"))
                        use = true;
                    else if (to.HasTreasure("WoodenOx"))
                        use = true;
                    else if (to.HasArmor("PeaceSpell") && !RoomLogic.PlayerHasSkill(room, to, "wendao"))
                        use = true;
                    else if (ai.IsWeak(to))
                        use = true;
                    else if (ai.GetEnemies(ai.Self).Count == 1)
                        use = true;

                    if (use)
                    {
                        result.Null = true;
                        return result;
                    }
                }
            }
            else
            {
                if (ai.IsFriend(from) && ai.Self == room.Current && ai.GetKnownCardsNums("Nullification", "he", ai.Self) > 1 && ai.GetOverflow(ai.Self) > 0)
                {
                    result.Null = true;
                    return result;
                }
            }

            return result;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                }
            }
        }
    }
    public class SnatchAI : UseCard
    {
        public SnatchAI() : base("Snatch")
        {
            key = new List<string> { "cardChosen" };
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            FunctionCard fcard = Engine.GetFunctionCard(Name);
            List<Player> targets = ai.Exclude(room.GetOtherPlayers(player), card, player);
            foreach (Player p in targets)
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, "hej", FunctionCard.HandlingMethod.MethodGet);
                room.OutPut(card.Name + p.SceenName + " is " + score.Score.ToString() + " friend ? " + ai.IsFriend(p).ToString());
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                bool hand = false;
                foreach (int id in card.SubCards)
                {
                    if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == player)
                        hand = true;
                }

                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 4 || (scores[0].Score > 0 && hand && ai.GetOverflow(player) > 0))
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                }
            }
        }
        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids, object data)
        {
            return ai.FindCards2Discard(from, to, "hej", FunctionCard.HandlingMethod.MethodGet).Ids;
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            Room room = ai.Room;
            NulliResult result = new NulliResult();
            if (!from.Alive || !to.Alive || to.IsAllNude()) return result;

            if (positive)
            {
                if ((RoomLogic.PlayerContainsTrick(room, to, "Indulgence") || RoomLogic.PlayerContainsTrick(room, to, "SupplyShortage"))
                        && ai.IsFriend(to) && to.IsNude()) return result;

                if (ai.IsEnemy(from) && ai.IsFriend(from, to) && RoomLogic.GetPlayerCards(room, to, "j").Count > 0)
                {
                    result.Null = true;
                    return result;
                }
                else if (ai.IsFriend(from) && ai.IsFriend(to))
                    return result;

                else if (ai.IsFriend(to))
                {
                    bool use = false;
                    if (to == ai.Self)
                        use = true;
                    else if (keep)
                        return result;
                    else if (to.HasTreasure("JadeSeal"))
                        use = true;
                    else if (to.HasTreasure("WoodenOx"))
                        use = true;
                    else if (to.HasArmor("PeaceSpell") && !RoomLogic.PlayerHasSkill(room, to, "wendao"))
                        use = true;
                    else if (ai.IsWeak(to))
                        use = true;
                    else if (ai.GetEnemies(ai.Self).Count == 1)
                        use = true;

                    if (use)
                    {
                        result.Null = true;
                        return result;
                    }
                }
            }
            else
            {
                if (ai.IsFriend(from) && ai.Self == room.Current && ai.GetKnownCardsNums("Nullification", "he", ai.Self) > 1 && ai.GetOverflow(ai.Self) > 0)
                {
                    result.Null = true;
                    return result;
                }
            }

            return result;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                }
            }
        }
    }

    public class DuelAI : UseCard
    {
        public DuelAI() : base("Duel")
        {
            key = new List<string> { "cardResponded" };
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard duel)
        {
            List<Player> enemies = ai.Exclude(ai.GetEnemies(player), duel);
            List<Player> friends = ai.Exclude(ai.FriendNoSelf, duel);
            duel.SetFlags("AI_Using");
            int n1 = ai.GetKnownCardsNums("Slash", "he", player);
            duel.SetFlags("-AI_Using");

            if (ai.HasSkill("wushuang")) n1*= 2;

            Dictionary<Player, double> scores = new Dictionary<Player, double>();
            foreach (Player p in friends)
            {
                scores[p] = ai.GetDamageScore(new DamageStruct(duel, player, p)).Score;
                if (ai.HasSkill("jiang", p)) scores[p] += 3;
            }
            foreach (Player p in enemies)
            {
                bool no_red = p.GetMark("@qianxi_red") > 0;
                bool no_black = p.GetMark("@qianxi_black") > 0;
                double n2 = ai.GetKnownCardsNums("Slash", "he", p, player);
                if (!ai.IsLackCard(p, "Slash"))
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
                if (n2 > n1)
                    scores[p] = ai.GetDamageScore(new DamageStruct(duel, p, player)).Score;
                else
                    scores[p] = ai.GetDamageScore(new DamageStruct(duel, player, p)).Score - (n2 - 1) * 0.4;

                if (ai.HasSkill("jiang", p)) scores[p] -= 3;
            }
            List<Player> targets = new List<Player>(friends);
            targets.AddRange(enemies);
            targets.Sort((x, y) => { return scores[x] > scores[y] ? -1 : 1; });
            bool hand = false;
            foreach (int id in duel.SubCards)
            {
                if (ai.Room.GetCardPlace(id) == Player.Place.PlaceHand && ai.Room.GetCardOwner(id) == player)
                    hand = true;
            }
            if (targets.Count > 0)
            {
                int targets_num = 1 + Engine.CorrectCardTarget(ai.Room, TargetModSkill.ModType.ExtraMaxTarget, player, duel);
                for (int i = 0; i < targets_num; i++)
                    if (scores[targets[i]] > 4 || (scores[targets[i]] > 0 && hand && ai.GetOverflow(player) > 0))
                        use.To.Add(targets[i]);

                if (use.To.Count > 0)
                {
                    use.Card = duel;
                    return;
                }
            }

            if (ai.HasSkill("jijzhi") || ai.HasSkill("jiang"))
            {
                if (targets.Count > 0 && scores[targets[0]] > 0)
                {
                    use.To.Add(targets[0]);
                }
                else if (hand && ai.GetOverflow(player) > 0)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(Name);
                    foreach (Player p in ai.Room.GetOtherPlayers(player))
                    {
                        if (fcard.TargetFilter(ai.Room, new List<Player>(), p, player, duel) && (ai.IsCancelTarget(duel, p, player) || !ai.IsCardEffect(duel, p, player)))
                        {
                            use.To.Add(p);
                            break;
                        }
                    }
                }

                if (use.To.Count > 0)
                    use.Card = duel;
            }
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardEffectStruct effect = (CardEffectStruct)data;
            Player from, to;
            if (effect.From == player)
            {
                from = player;
                to = effect.To;
            }
            else
            {
                from = effect.From;
                to = player;
            }
            DamageStruct damage = new DamageStruct(effect.Card, from, to);
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player> { from == player ? to : from }
            };
            if (!ai.CardAskNullifilter(damage))
            {
                List<string> strs = new List<string>(prompt.Split(':'));
                int n = int.Parse(strs[strs.Count - 1]);
                List<WrappedCard> slashes = ai.GetCards("Slash", player, true);
                if (slashes.Count >= n)
                {
                    use.Card = slashes[0];
                }
            }

            return use;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                        ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
            }

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_" && strs[3].StartsWith("duel-slash"))
                    {
                        string prompt = strs[3];
                        if (strs[strs.Count - 2] == "1")
                        {
                            List<CardUseStruct> uses = (List<CardUseStruct>)ai.Room.GetTag("card_proceeing");
                            use = uses[uses.Count - 1];
                            if (use.Card != null && use.Card.Name == Name)
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, player);
                                ScoreStruct score = ai.GetDamageScore(damage);
                                bool lack = true;
                                if (ai.IsFriend(use.From, player) && score.Score > 0)
                                    lack = false;
                                else if (ai.IsEnemy(player) && score.Score < 0)
                                    lack = false;

                                if (lack) ai.SetCardLack(player, "Slash");
                            }
                        }
                    }
                }
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();

            DamageStruct damage = new DamageStruct(trick, from, to);
            ScoreStruct score = ai.GetDamageScore(damage);
            if (score.Score < -3)
            {
                if (ai.IsEnemy(from) && ai.IsFriend(to))
                {
                    int wushuang_from = RoomLogic.PlayerHasShownSkill(ai.Room, from, "wushuang") ? 2 : 1;
                    int wushuang_to = RoomLogic.PlayerHasShownSkill(ai.Room, to, "wushuang") ? 2 : 1;
                    if (ai.GetKnownCardsNums("Slash", "he", to, ai.Self) * wushuang_to > ai.GetKnownCardsNums("Slash", "he", from, ai.Self) * wushuang_from)
                        return result;
                }

                if (keep && score.Score > -6)
                    return result;

                result.Null = true;
                return result;
            }

            return result;
        }
    }

    public class ExNihiloAI : UseCard
    {
        public ExNihiloAI() : base("ExNihilo")
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (keep)
                return result;

            bool last = false;
            WrappedCard nul = ai.GetCards("Nullification", ai.Self, true)[0];
            if (ai.Self.IsLastHandCard(nul) && ai.HasLoseHandcardEffective(ai.Self))
                last = true;

            if (positive)
            {
                if (ai.IsEnemy(to) && ai.PlayersLevel[to] >= 3)
                {
                    if (last || to.HandcardNum < 2)
                        result.Null = true;
                }
            }
            else
            {
                if (ai.IsFriend(to) && (last || (to == ai.Self && ai.Self.IsLastHandCard(nul))))
                    result.Null = true;
            }

            return result;
        }
    }

    public class AwaitExhaustedAI : UseCard
    {
        public AwaitExhaustedAI() : base("AwaitExhausted")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            bool hand = false;
            foreach (int id in card.SubCards)
            {
                if (ai.Room.GetCardPlace(id) == Player.Place.PlaceHand && ai.Room.GetCardOwner(id) == player)
                    hand = true;
            }
            if (!RoomLogic.IsVirtualCard(room, card) || card.SubCards.Count == 0)
                use.Card = card;
            else
            {
                double value = ai.GetUseValue(card, player), use_value = 0, keep_value = 0;
                if (!hand)
                    value += 3;
                foreach (int id in card.SubCards)
                {
                    use_value += ai.GetUseValue(id, player);
                    keep_value += ai.GetKeepValue(id, player);
                }

                if (use_value < value && keep_value < value)
                    use.Card = card;
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (keep)
                return result;

            bool last = false;
            WrappedCard nul = ai.GetCards("Nullification", ai.Self, true)[0];
            if (ai.Self.IsLastHandCard(nul) && ai.HasLoseHandcardEffective(ai.Self))
                last = true;

            if (positive)
            {
                if (ai.IsEnemy(to) && ai.PlayersLevel[to] >= 3 && last)
                {
                    result.Null = true;

                    List<Player> targets = (List<Player>)ai.Room.GetTag("targets" + RoomLogic.CardToString(ai.Room, trick));
                    List<Player> delete = new List<Player>(targets);
                    foreach (Player p in delete)
                        if (delete.IndexOf(p) < delete.IndexOf(to))
                            targets.Remove(p);

                    int count = 0;
                    foreach (Player p in targets)
                    {
                        if (RoomLogic.IsFriendWith(ai.Room, p, to))
                        {
                            count++;
                        }
                    }
                    if (count > 2)
                        result.Heg = true;
                }
            }
            else
            {
                if (ai.IsFriend(to) && (last || (to == ai.Self && ai.Self.IsLastHandCard(nul))))
                    result.Null = true;
            }

            return result;
        }
    }

    public class BefriendAttackingAI : UseCard
    {
        public BefriendAttackingAI() : base("BefriendAttacking")
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (keep)
                return result;

            bool last = false;
            WrappedCard nul = ai.GetCards("Nullification", ai.Self, true)[0];
            if (ai.Self.IsLastHandCard(nul) && ai.HasLoseHandcardEffective(ai.Self))
                last = true;

            if (positive)
            {
                if (ai.IsEnemy(from) && ai.PlayersLevel[to] >= 3)
                {
                    if (last || from.HandcardNum < 2)
                        result.Null = true;
                }
            }
            else
            {
                if (ai.IsFriend(from) && (last || (from == ai.Self && ai.Self.IsLastHandCard(nul))))
                    result.Null = true;
            }

            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> players = ai.Exclude(room.GetOtherPlayers(player), card);
            ai.SortByDefense(ref players, false);
            //对友方使用
            foreach (Player p in players)
            {
                if (ai.IsFriend(p) && (!ai.NeedKongcheng(p) || !p.IsKongcheng()))
                {
                    use.To.Add(p);
                    use.Card = card;
                    return;
                }
            }
            //野心家
            foreach (Player p in players)
            {
                if ((!ai.HasSkill("luanji", p) || p.HandcardNum <= 3) && p.Role == "careerist")
                {
                    use.To.Add(p);
                    use.Card = card;
                    return;
                }
            }

            //计算人数最少的势力
            List<string> kingdoms = new List<string> { "wei", "shu", "wu", "qun" };
            kingdoms.Remove(player.Kingdom);
            Dictionary<string, int> kingdom_count = new Dictionary<string, int>();
            foreach (string kingdom in kingdoms)
            {
                int count = RoomLogic.GetPlayerNumWithSameKingdom(room, player, kingdom);
                if (count > 0)
                    kingdom_count[kingdom] = count;
            }
            kingdoms = new List<string>(kingdom_count.Keys);
            kingdoms.Sort((x, y) => { return kingdom_count[x] < kingdom_count[y] ? -1 : 1; });

            foreach (Player p in players)
            {
                if (p.Kingdom == kingdoms[0] && ai.NeedKongcheng(p) && p.IsKongcheng())
                {
                    use.To.Add(p);
                    use.Card = card;
                    return;
                }
            }
            foreach (Player p in players)
            {
                if (p.Kingdom == kingdoms[0] && ai.WillSkipPlayPhase(p))
                {
                    use.To.Add(p);
                    use.Card = card;
                    return;
                }
            }
            //从行动顺序最末尾的开始寻找
            room.SortByActionOrder(ref players);
            for (int i = players.Count - 1; i >= 0; i--)
            {
                Player p = players[i];
                if (p.Kingdom == kingdoms[0])
                {
                    use.To.Add(p);
                    use.Card = card;
                    return;
                }
            }
        }
    }

    public class FireAttackAI : UseCard
    {
        private readonly Dictionary<string, string> convert = new Dictionary<string, string> { { ".S", "spade" }, { ".D", "diamond" }, { ".H", "heart" }, { ".C", "club" } };
        public FireAttackAI() : base("FireAttack")
        {
            key = new List<string> { "cardShow", "cardResponded" };
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            Room room = ai.Room;
            NulliResult result = new NulliResult();
            if (keep || !from.Alive || to.IsKongcheng() || from.IsKongcheng() || ai.Self == from) return result;

            DamageStruct damage = new DamageStruct(trick, from, to, 1, DamageStruct.DamageNature.Fire);
            if (ai.DamageEffect(damage, DamageStruct.DamageStep.Done) == 0 || from.HandcardNum < 3
                || (RoomLogic.PlayerHasShownSkill(room, from, "hongyan") && to.HandcardNum > 3))
                return result;

            if (positive)
            {
                if (to.Chained && !(ai.IsFriend(from) && ai.IsEnemy(to)))
                {
                    double value = 0;
                    foreach (Player p in room.GetOtherPlayers(to))
                    {
                        if (p.Chained)
                            damage = new DamageStruct(trick, from, p, 1, DamageStruct.DamageNature.Fire);
                        if (ai.IsFriend(p))
                            value += ai.GetDamageScore(damage).Score;
                    }
                    if (value < -4)
                    {
                        result.Null = true;
                        return result;
                    }
                }

                if (ai.IsEnemy(from) && ai.IsFriend(to))
                {
                    if (from.HandcardNum > 2 || ai.IsWeak(to) || ai.HasArmorEffect(to, "Vine"))
                    {
                        result.Null = true;
                        return result;
                    }
                }
            }

            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            Dictionary<CardSuit, bool> lack = new Dictionary<CardSuit, bool>
            {
                { CardSuit.Spade, true },
                { CardSuit.Club, true },
                { CardSuit.Diamond, true },
                { CardSuit.Heart, true },
            };
            List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, player);
            List<CardSuit> canDis = new List<CardSuit>();
            foreach (WrappedCard c in cards)
            {
                if (!card.SubCards.Contains(c.Id) && RoomLogic.CanDiscard(room, player, player, c.Id) && !canDis.Contains(c.Suit))
                {
                    lack[c.Suit] = false;
                    canDis.Add(c.Suit);
                }
            }
            List<ScoreStruct> scores = new List<ScoreStruct>();
            List<Player> players = ai.Exclude(room.GetAlivePlayers(), card, player);
            foreach (Player p in players)
            {
                DamageStruct damage = new DamageStruct(card, player, p, 1, DamageStruct.DamageNature.Fire);
                ScoreStruct score = ai.GetDamageScore(damage);
                score.Players = new List<Player> { p };
                if (score.DoDamage && score.Score > 0 && p.Chained)
                {
                    if (ai.IsGoodSpreadStarter(damage))
                        score.Score += 6;
                    else
                        score.Score -= 6;
                }
                //计算命中率
                double rate = 0;
                if (p == player)
                {
                    if (player.IsLastHandCard(card, true))
                        rate = 0;
                    else
                        rate = 1;
                }
                else
                {
                    List<int> ids = ai.GetKnownCards(p);
                    if (ids.Count == p.HandcardNum)
                    {
                        List<CardSuit> _lack = new List<CardSuit>(), suits = new List<CardSuit>();
                        foreach (int id in ids)
                        {
                            WrappedCard c = room.GetCard(id);
                            if (!suits.Contains(c.Suit))
                                suits.Add(c.Suit);

                            if (lack[c.Suit] == true && !_lack.Contains(c.Suit))
                            {
                                _lack.Add(c.Suit);
                            }
                        }
                        rate = 1 - (double)_lack.Count / suits.Count;
                    }
                    else
                        rate = canDis.Count / 4;
                }

                score.Score *= rate;
                scores.Add(score);
            }

            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                bool hand = false;
                foreach (int id in card.SubCards)
                {
                    if (ai.Room.GetCardPlace(id) == Player.Place.PlaceHand && ai.Room.GetCardOwner(id) == player)
                        hand = true;
                }
                if (scores[0].Score > 4 || (scores[0].Score > 0 && hand && ai.GetOverflow(player) > 0))
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                }
            }
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    if (strs[0] == "cardResponded" && strs[strs.Count - 1] == "_nil_")
                    {
                        //{ ".S", "spade" }, { ".D", "diamond" }, { ".H", "heart" }, { ".C", "club" }
                        string pattern = strs[strs.Count - 2];
                        switch (pattern)
                        {
                            case ".S":
                                if (ai.GetKnownCardsNums(pattern, "h", player, ai.Self, false) == 0)
                                    ai.SetCardLack(player, CardSuit.Spade.ToString());
                                break;
                            case ".D":
                                if (ai.GetKnownCardsNums(pattern, "h", player, ai.Self, false) == 0)
                                    ai.SetCardLack(player, CardSuit.Diamond.ToString());
                                break;
                            case ".H":
                                if (ai.GetKnownCardsNums(pattern, "h", player, ai.Self, false) == 0)
                                    ai.SetCardLack(player, CardSuit.Heart.ToString());
                                break;
                            case ".C":
                                if (ai.GetKnownCardsNums(pattern, "h", player, ai.Self, false) == 0)
                                    ai.SetCardLack(player, CardSuit.Club.ToString());
                                break;
                        }
                    }

                    if (strs[0] == "cardShow")
                    {
                        string id_str = strs[strs.Count - 1];
                        id_str = id_str.Substring(1, id_str.Length - 2);
                        int id = int.Parse(id_str);
                        ai.SetPublicKnownCards(player, id);
                        ai.ClearCardLack(player, id);
                    }
                }
            }
        }

        public override WrappedCard OnCardShow(TrustedAI ai, Player player, Player request, object data)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = RoomLogic.GetPlayerCards(room, player, "h");
            if (request == player)
            {
                List<int> ids = new List<int>(player.HandCards);
                ai.SortByUseValue(ref ids, false);
                return room.GetCard(ids[0]);
            }

            if (RoomLogic.PlayerHasShownSkill(room, request, "hongyan"))
            {
                if (ai.IsEnemy(request))
                {
                    foreach (WrappedCard card in cards)
                    {
                        if (card.Suit == CardSuit.Spade)
                            return card;
                    }
                }
                else if (ai.IsFriend(request, player))
                {
                    foreach (WrappedCard card in cards)
                    {
                        if (card.Suit != CardSuit.Spade && !ai.IsLackCard(request, card.Suit.ToString()))
                            return card;
                    }
                }
            }

            if (ai.IsEnemy(request))
            {
                foreach (WrappedCard card in cards)
                {
                    if (ai.IsLackCard(request, card.Suit.ToString()))
                        return card;
                }

                foreach (WrappedCard card in cards)
                {
                    if (card.Suit == CardSuit.Heart)
                        return card;
                }

                foreach (WrappedCard card in cards)
                {
                    if (card.Suit == CardSuit.Diamond)
                        return card;
                }
            }
            else if (ai.IsFriend(request, player))
            {
                foreach (WrappedCard card in cards)
                {
                    if (!ai.IsLackCard(request, card.Suit.ToString()))
                        return card;
                }
            }

            return null;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            List<WrappedCard> cards = RoomLogic.GetPlayerCards(room, player, "h");
            WrappedCard card = null;
            List<int> ids = new List<int>(player.HandCards);
            ai.SortByUseValue(ref ids, false);

            CardEffectStruct effect = (CardEffectStruct)data;
            Player target = effect.To;
            WrappedCard fire = effect.Card;

            DamageStruct damage = new DamageStruct(fire, player, target, 1, DamageStruct.DamageNature.Fire);
            ScoreStruct score = ai.GetDamageScore(damage);
            if (score.DoDamage && target.Chained)
            {
                if (ai.IsGoodSpreadStarter(damage))
                    score.Score += 6;
                else
                    score.Score -= 6;
            }
            if (score.Score > 0)
            {
                foreach (int id in ids)
                {
                    if (WrappedCard.GetSuitString(room.GetCard(id).Suit) == convert[pattern])
                    {
                        double value = score.Score;
                        if (ai.IsCard(id, "Peach", player))
                            value -= 4;
                        if (ai.GetOverflow(player) <= 0)
                            value -= 1;
                        if (value > 0)
                        {
                            card = room.GetCard(id);
                            break;
                        }
                    }
                }
            }

            CardUseStruct use = new CardUseStruct(card, player, target);
            return use;
        }
    }

    public class KnownBothAI : UseCard
    {
        public KnownBothAI() : base("KnownBoth")
        {
            key = new List<string> { "skillChoice" };
        }

        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            if (ai.Choice.ContainsKey(Name) && choices.Contains(ai.Choice[Name]))
                return ai.Choice[Name];

            if (choices.Contains("head_general"))
                return "head_general";
            else if (choices.Contains("deputy_general"))
                return "deputy_general";
            else if (choices.Contains("handcards"))
                return "handcards";

            return base.OnChoice(ai, player, choices, data);
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && ai.Self == player)
                {
                    string choice = strs[2];
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag("KnownBothTarget"))
                        {
                            target = p;
                            break;
                        }
                    }
                    if (target != null)
                    {
                        if (choice == "head_general")
                        {
                            ai.SetKnown(target, "h");
                        }
                        else if (choice == "deputy_general")
                            ai.SetKnown(target, "d");
                        else if (choice == "handcards")
                        {
                            ai.ClearKnownCards(target);
                            foreach (int id in target.HandCards)
                                ai.SetPrivateKnownCards(target, id);
                        }
                    }
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Choice[Name] = string.Empty;
            Room room = ai.Room;
            List<string> knownboth_choice = new List<string>();
            FunctionCard fcard = Engine.GetFunctionCard(Name);
            bool can_recast = fcard.CanRecast(room, player, card);
            List<Player> targets = new List<Player>(), players = ai.Exclude(room.GetOtherPlayers(player), card);
            foreach (Player p in players)
            {
                if (ai.GetPlayerTendency(p) == "unknown")
                    targets.Add(p);
            }
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, player, card);
            room.SortByActionOrder(ref targets);
            if (targets.Count > 0)
            {
                for (int i = 0; i < total_num; i++)
                    use.To.Add(targets[i]);

                ai.Choice[Name] = "head_general";
                use.Card = card;
                return;
            }

            if (can_recast && (!ai.HasSkill("jizhi", player) || (RoomLogic.IsVirtualCard(room, card) && card.SubCards.Count != 1)))
            {
                use.Card = card;
                return;
            }

            foreach (Player p in players)
            {
                if (ai.IsEnemy(p) && (p.HandCards.Count > 3 || !p.HasShownAllGenerals()))
                    targets.Add(p);

                if (targets.Count > 0)
                {
                    for (int i = 0; i < total_num; i++)
                        use.To.Add(targets[i]);

                    use.Card = card;
                    return;
                }
            }

            if (can_recast)
            {
                use.Card = card;
            }
        }
    }

    public class IronChainAI : UseCard
    {
        public IronChainAI() : base("IronChain")
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                {
                    if (p == player) continue;
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                    {
                        if (!p.Chained)
                            ai.UpdatePlayerRelation(player, p, false);
                        else
                            ai.UpdatePlayerRelation(player, p, true);
                    }
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            use.Card = card;
            List<Player> enemis = ai.GetEnemies(player), friends = ai.GetFriends(player);
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            bool canRecast = fcard.CanRecast(room, player, card);

            if (ai.GetFriends(player).Count == 1 && ai.GetChainedFriends(player).Count <= 1 && !canRecast)
            {
                use.Card = null;
                return;
            }

            List<Player> friendtargets = new List<Player>(), friendtargets2 = new List<Player>();
            List<Player> enemytargets = new List<Player>();
            ai.SortByDefense(ref friends, false);
            foreach (Player p in friends)
            {
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card) && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                {
                    DamageStruct damage_f = new DamageStruct(string.Empty, null, p, 1, DamageStruct.DamageNature.Fire);
                    DamageStruct damage_t = new DamageStruct(string.Empty, null, p, 1, DamageStruct.DamageNature.Thunder);
                    double damage_value = Math.Min(ai.GetDamageScore(damage_f).Score, ai.GetDamageScore(damage_t).Score);
                    if (ai.IsGoodSpreadStarter(damage_f) || ai.IsGoodSpreadStarter(damage_t))
                        damage_value += 4;
                    if (p.Chained && damage_value < 0)
                    {
                        if (RoomLogic.PlayerContainsTrick(room, p, "Lightning"))
                            friendtargets.Add(p);
                        else
                            friendtargets2.Add(p);
                    }
                }
            }

            ai.SortByDefense(ref enemis, false);
            foreach (Player p in enemis)
            {
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card) && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                {
                    DamageStruct damage_f = new DamageStruct(string.Empty, null, p, 1, DamageStruct.DamageNature.Fire);
                    DamageStruct damage_t = new DamageStruct(string.Empty, null, p, 1, DamageStruct.DamageNature.Thunder);
                    double damage_value = Math.Min(ai.GetDamageScore(damage_f).Score, ai.GetDamageScore(damage_t).Score);
                    if (!p.Chained && damage_value < 0 && RoomLogic.CanBeChainedBy(room, p, player))
                    {
                        enemytargets.Add(p);
                    }
                }
            }
            int targets_num = 2 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, player, card);
            friendtargets.AddRange(friendtargets2);
            friendtargets.AddRange(enemytargets);
            for (int i = 0; i < Math.Min(targets_num, friendtargets.Count); i++)
            {
                use.To.Add(friendtargets[i]);
            }
            if (use.To.Count < targets_num && !player.Chained && RoomLogic.CanBeChainedBy(room, player, player)
                && fcard.TargetFilter(room, new List<Player>(), player, player, card) && !ai.IsCancelTarget(card, player, player) && ai.IsCardEffect(card, player, player))
            {
                List<WrappedCard> fires = ai.GetCards("FireAttack", player, true);
                if (fires.Count > 0)
                {
                    WrappedCard fire = fires[0];
                    if (!player.IsLastHandCard(fire) && player.HandcardNum > 2)
                    {
                        DamageStruct damage_f = new DamageStruct(fire, player, player, 1, DamageStruct.DamageNature.Fire);
                        ScoreStruct score = ai.GetDamageScore(damage_f);
                        if (score.DoDamage)
                        {
                            damage_f.Damage = ai.DamageEffect(damage_f, DamageStruct.DamageStep.Done);
                            damage_f.Steped = DamageStruct.DamageStep.Done;
                            double value = score.Score;
                            foreach (Player p in room.GetOtherPlayers(player))
                            {
                                bool chained = (!p.Chained && use.To.Contains(p)) || (p.Chained && !use.To.Contains(p));
                                if (chained)
                                {
                                    DamageStruct damage_copy = damage_f;
                                    damage_copy.Transfer = true;
                                    damage_copy.Chain = true;
                                    damage_copy.To = p;

                                    value += ai.GetDamageScore(damage_copy).Score;
                                }
                            }
                            if (value > 6)
                                use.To.Add(player);
                        }
                    }
                }
            }
        }
        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (keep) return result;
            Room room = ai.Room;

            List<Player> targets = (List<Player>)room.GetTag("targets" + RoomLogic.CardToString(room, trick));
            List<Player> delete = new List<Player>(targets);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            if (positive)
            {
                if (ai.IsEnemy(from) && ai.IsFriend(to))
                {
                    bool invoke = false;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (ai.HasArmorEffect(p, "Vine") && (p.Chained || targets.Contains(p)))
                        {
                            invoke = true;
                        }
                        if (RoomLogic.PlayerContainsTrick(room, p, "Lightning"))
                        {
                            List<Player> friends = new List<Player>();
                            foreach (Player p2 in ai.GetFriends(ai.Self))
                            {
                                if (p2.Chained || targets.Contains(p2))
                                    friends.Add(p2);
                            }
                            if (friends.Count > 2)
                            {
                                invoke = true;
                                break;
                            }
                        }

                        if (invoke)
                        {
                            result.Null = true;
                            break;
                        }
                    }

                    if (invoke && ai.GetKnownCardsNums("HegNullification", "he", ai.Self, ai.Self) > 0)
                    {
                        if (room.NullTimes == 0)
                        {
                            targets.Remove(to);
                            foreach (Player p in targets)
                            {
                                if (RoomLogic.IsFriendWith(room, to, p))
                                {
                                    result.Heg = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (ai.IsEnemy(to))
                {
                    WrappedCard nul = ai.GetCards("Nullification", ai.Self, true)[0];
                    if (ai.Self.IsLastHandCard(nul) && ai.HasLoseHandcardEffective(ai.Self))
                        result.Null = true;
                }
            }

            return result;
        }
    }

    public class SavageAssaultAI : UseCard
    {
        public SavageAssaultAI() : base("SavageAssault")
        {
            key = new List<string> { "cardResponded" };
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardEffectStruct effect = (CardEffectStruct)data;
            Player menghuo = ai.FindPlayerBySkill("huoshou");
            Player from = effect.From;
            if (menghuo != null && RoomLogic.PlayerHasShownSkill(room, menghuo, "huoshou"))
                from = menghuo;
            DamageStruct damage = new DamageStruct(effect.Card, from, player);
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player> { effect.From }
            };
            if (!ai.CardAskNullifilter(damage))
            {
                List<WrappedCard> slashes = ai.GetCards("Slash", player);
                if (slashes.Count > 0)
                    use.Card = slashes[0];
            }

            return use;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_" && strs[3].StartsWith("savage-assault-slash"))
                    {
                        List<CardUseStruct> uses = (List<CardUseStruct>)ai.Room.GetTag("card_proceeing");
                        CardUseStruct use = uses[uses.Count - 1];
                        if (use.Card != null && use.Card.Name == Name)
                        {
                            Player source = ai.FindPlayerBySkill("huoshou");
                            if (source != null && !RoomLogic.PlayerHasShownSkill(ai.Room, source, "huoshou"))
                                source = null;
                            DamageStruct damage = new DamageStruct(use.Card, use.From, source ?? player);
                            ScoreStruct score = ai.GetDamageScore(damage);

                            bool lack = true;
                            if (ai.IsFriend(use.From, player) && score.Score > 0)
                                lack = false;
                            else if (ai.IsEnemy(player) && score.Score < 0)
                                lack = false;

                            if (lack) ai.SetCardLack(player, "Slash");
                        }
                    }
                }
            }
        }

        private bool HasSlash(TrustedAI ai, Player who)
        {
            if (!ai.IsLackCard(who, "Slash"))
                return false;
            else
            {
                if (ai.GetKnownCardsNums("Slash", "he", who) > 0)
                    return true;
                else
                {
                    double rate = 4;
                    bool no_red = who.GetMark("@qianxi_red") > 0;
                    bool no_black = who.GetMark("@qianxi_black") > 0;
                    if (no_black)
                        rate = 9;
                    if (no_red)
                        rate = 5;
                    double fix = 0;
                    if (ai.HasSkill("wusheng", who) && !no_red)
                        fix += 2;
                    if (ai.HasSkill("longdan", who) && !no_red)
                        fix += 2.5;

                    double count = (who.HandcardNum - ai.GetKnownCards(who).Count) / (rate - fix)
                        + (who.GetHandPile().Count - ai.GetKnownHandPileCards(who).Count) / (4 - fix);
                    return count >= 1 ? true : false;
                }
            }
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

            DamageStruct damage = new DamageStruct(trick, from, to);
            ScoreStruct score = ai.GetDamageScore(damage);

            if (positive)
            {
                if (score.Score < 0 && ai.IsFriend(to))
                {
                    bool nulli = !HasSlash(ai, to);
                    double value = score.Score;
                    if (room.NullTimes == 0 && ai.GetKnownCardsNums("HegNullification", "he", player) > 0 || room.HegNull)
                    {
                        foreach (Player p in targets)
                        {
                            if (p != to && RoomLogic.IsFriendWith(room, p, to))
                            {
                                DamageStruct _damage = new DamageStruct(trick, from, p);
                                double _value = ai.GetDamageScore(_damage).Score;
                                if (_value < 0 && !HasSlash(ai, p))
                                {
                                    value += _value;
                                    result.Heg = true;
                                    nulli = true;
                                }
                            }
                        }
                    }

                    if (nulli && (score.Score < -5 && !keep || score.Score < -8))
                        result.Null = true;
                }
            }
            else
            {
                if (score.Score > 0 && !keep || score.Score > 8)
                    result.Null = true;
            }

            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (ai.GetAoeValue(card))
                use.Card = card;
        }
    }

    public class ArcheryAttackAI : UseCard
    {
        public ArcheryAttackAI() : base("ArcheryAttack")
        {
            key = new List<string> { "cardResponded" };
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardEffectStruct effect = (CardEffectStruct)data;
            Player from = effect.From;
            DamageStruct damage = new DamageStruct(effect.Card, from, player);
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player> { effect.From }
            };
            if (!ai.CardAskNullifilter(damage))
            {
                List<WrappedCard> jinks = ai.GetCards("Jink", player);
                if (jinks.Count > 0)
                    use.Card = jinks[0];
            }

            return use;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_" && strs[3].StartsWith("savage-assault-slash"))
                    {
                        List<CardUseStruct> uses = (List<CardUseStruct>)ai.Room.GetTag("card_proceeing");
                        CardUseStruct use = uses[uses.Count - 1];
                        if (use.Card != null && use.Card.Name == Name)
                        {
                            DamageStruct damage = new DamageStruct(use.Card, use.From, player);
                            ScoreStruct score = ai.GetDamageScore(damage);

                            bool lack = true;
                            if (ai.IsFriend(use.From, player) && score.Score > 0)
                                lack = false;
                            else if (ai.IsEnemy(player) && score.Score < 0)
                                lack = false;

                            if (lack) ai.SetCardLack(player, "Jink");
                        }
                    }
                }
            }
        }

        private bool HasJink(TrustedAI ai, Player who)
        {
            bool no_red = who.GetMark("@qianxi_red") > 0;
            bool no_black = who.GetMark("@qianxi_black") > 0;
            if (ai.HasArmorEffect(who, "EightDiagram") && ai.HasSkill("qingguo+tiandu") && !no_black)
                return true;

            double basic = 0;
            if (ai.HasArmorEffect(who, "EightDiagram"))
                basic = 0.35;

            if (!ai.IsLackCard(who, "Jink"))
                return false;
            else
            {
                if (ai.GetKnownCardsNums("Jink", "he", who) > 0)
                    return true;
                else
                {
                    double rate = 5;
                    if (no_red)
                        rate = 0;
                    double fix = 0;
                    if (ai.HasSkill("qingguo", who) && !no_black)
                        fix += 2.5;
                    if (ai.HasSkill("longdan", who))
                        fix += 1.5;
                    if (fix > 0 && rate == 0)
                        rate = 7;

                    double count = (who.HandcardNum - ai.GetKnownCards(who).Count) / (rate - fix)
                        + (who.GetHandPile().Count - ai.GetKnownHandPileCards(who).Count) / (5 - fix) + basic;
                    return count >= 1 ? true : false;
                }
            }
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

            DamageStruct damage = new DamageStruct(trick, from, to);
            ScoreStruct score = ai.GetDamageScore(damage);

            if (positive)
            {
                if (score.Score < 0 && ai.IsFriend(to))
                {
                    double value = score.Score;
                    if (room.NullTimes == 0 && ai.GetKnownCardsNums("HegNullification", "he", player) > 0 || room.HegNull)
                    {
                        result.Heg = true;
                        foreach (Player p in targets)
                        {
                            if (p != to && RoomLogic.IsFriendWith(room, p, to))
                            {
                                DamageStruct _damage = new DamageStruct(trick, from, p);
                                value += ai.GetDamageScore(_damage).Score;
                            }
                        }
                    }

                    if (score.Score < -5 && !keep || score.Score < -8)
                        result.Null = true;
                }
            }
            else
            {
                if (score.Score > 0 && !keep || score.Score > 8)
                    result.Null = true;
            }

            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (ai.GetAoeValue(card))
                use.Card = card;
        }
    }

    public class IndulgenceAI : UseCard
    {
        public IndulgenceAI() : base("Indulgence")
        { }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (positive)
            {
                if (ai.IsFriend(to))
                {
                    if (!ai.WillSkipDrawPhase(to) || to.HandcardNum >= 2 || ai.GetOverflow(to) > 0 || to.GetMark("@pioneer") > 0)
                        result.Null = true;
                }
            }
            else
            {
                if (ai.IsEnemy(to) && ai.PlayersLevel[to] >= 3)
                {
                    if (ai.GetOverflow(to) > 0 || ai.IsWeak(to))
                        result.Null = true;
                }
            }
            return result;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                        ai.UpdatePlayerRelation(player, p, false);          //若使用者和目标中的一方身份已判明，则更新双方关系为敌对
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count == 0 && room.Round <= 1 && ai.GetPlayerTendency(room.GetNextAlive(player)) == "unknown")
                enemies = new List<Player> { room.GetNextAlive(player) };
            enemies = ai.Exclude(enemies, card, player);

            if (enemies.Count == 0) return;
            Player zhanghe = ai.FindPlayerBySkill("qiaobian");
            int zhanghe_seat = (zhanghe != null && zhanghe.FaceUp && !zhanghe.IsKongcheng() && !ai.IsFriend(zhanghe)) ? zhanghe.Seat : 0;

            Dictionary<Player, double> points = new Dictionary<Player, double>();
            foreach (Player p in enemies)
                points[p] = Getvalue(ai, p, card, zhanghe, zhanghe_seat);
            enemies.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
            if (points[enemies[0]] > -100)
            {
                use.Card = card;
                use.To = new List<Player> { enemies[0] };
            }
        }

        private double Getvalue(TrustedAI ai, Player enemy, WrappedCard card, Player zhanghe, int zhanghe_seat)
        {
            Room room = ai.Room;
            if (ai.HasSkill("jgjiguan_qinglong|jgjiguan_baihu|jgjiguan_zhuque|jgjiguan_xuanwu", enemy)) return -101;
            if (ai.HasSkill("jgjiguan_bian|jgjiguan_suanni|jgjiguan_chiwen|jgjiguan_yazi")) return -101;
            if (ai.HasSkill("qiaobian") && (!RoomLogic.PlayerContainsTrick(room, enemy, "SupplyShortage") || !enemy.IsKongcheng()))
            {
                foreach (Player p in ai.GetFriends(ai.Self))
                {
                    if (!ai.IsCancelTarget(card, p, null) && ai.IsCardEffect(card, p, null))
                        return -101;
                }
            }
            if (zhanghe_seat > 0 && (ai.PlayerGetRound(zhanghe) <= ai.PlayerGetRound(enemy) && enemy.JudgingArea.Count <= 1 || !enemy.FaceUp))
                return -101;
            double value = enemy.HandcardNum - enemy.Hp;
            if (enemy.HandcardNum <= 2 && ai.WillSkipDrawPhase(enemy)) value -= 3;
            if (ai.WillSkipPlayPhase(enemy)) value -= 10;
            if (ai.HasSkill("lijian|fanjian|dimeng|jijiu|jieyin|zhiheng|rende", enemy)) value += 10;
            if (ai.HasSkill("qixi|guose|duanliang|luoshen|jizhi|wansha")) value += 5;
            if (ai.HasSkill("guzheng|duoshi")) value += 3;
            if (ai.IsWeak(enemy)) value += 3;
            if (ai.PlayersLevel[enemy] < 3) value -= 10;
            if (ai.HasSkill("keji|shensu", enemy)) value -= enemy.HandcardNum;
            if (ai.HasSkill("lirang|guanxing|yizhi", enemy)) value -= 5;
            if (ai.HasSkill("tiandu", enemy)) value -= 3;

            //if not sgs.isGoodTarget(enemy, self.enemies, self) then value = value - 1 end

            if (ai.GetKnownCardsNums("Dismantlement", "he", enemy, ai.Self) > 0) value += 2;
            value += (room.AliveCount() - ai.PlayerGetRound(enemy)) / 2;

            return value;
        }
    }

    public class SupplyShortageAI : UseCard
    {
        public SupplyShortageAI() : base("SupplyShortage")
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                        ai.UpdatePlayerRelation(player, p, false);          //若使用者和目标中的一方身份已判明，则更新双方关系为敌对
            }
        }
        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (positive)
            {
                if (ai.IsFriend(to))
                {
                    if (!ai.WillSkipPlayPhase(to) && !keep)
                        result.Null = true;
                }
            }
            else
            {
                if (ai.IsEnemy(to) && ai.PlayersLevel[to] >= 3)
                {
                    if (ai.IsWeak(to))
                        result.Null = true;
                }
            }
            return result;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.Exclude(ai.GetEnemies(player), card, player);
            if (enemies.Count == 0) return;
            Player zhanghe = ai.FindPlayerBySkill("qiaobian");
            int zhanghe_seat = (zhanghe != null && zhanghe.FaceUp && !zhanghe.IsKongcheng() && !ai.IsFriend(zhanghe)) ? zhanghe.Seat : 0;

            Dictionary<Player, double> points = new Dictionary<Player, double>();
            foreach (Player p in enemies)
                points[p] = GetValue(ai, p, card, zhanghe, zhanghe_seat);
            enemies.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
            if (points[enemies[0]] > -100)
            {
                use.Card = card;
                use.To = new List<Player> { enemies[0] };
            }
        }
        private double GetValue(TrustedAI ai, Player enemy, WrappedCard card, Player zhanghe, int zhanghe_seat)
        {
            Room room = ai.Room;
            if (ai.HasSkill("qiaobian") && (!RoomLogic.PlayerContainsTrick(room, enemy, "Indulgence") || !enemy.IsKongcheng()))
            {
                foreach (Player p in ai.GetFriends(ai.Self))
                {
                    if (!ai.IsCancelTarget(card, p, null) && ai.IsCardEffect(card, p, null))
                        return -101;
                }
            }
            if (zhanghe_seat > 0 && (ai.PlayerGetRound(zhanghe) <= ai.PlayerGetRound(enemy) && enemy.JudgingArea.Count <= 1 || !enemy.FaceUp))
                return -101;
            double value = 0 - enemy.HandcardNum;

            if (ai.HasSkill("haoshi|tuxi|lijian|fanjian|dimeng|jijiu|jieyin|beige", enemy)
                || (ai.HasSkill("zaiqi", enemy) && enemy.GetLostHp() > 1))
                value += value + 10;

            if (ai.HasSkill(TrustedAI.CardneedSkill + "|tianxiang"))
                value += 5;

            if (ai.HasSkill("yingzi_zhouyu|yingzi_sunce|duoshi", enemy)) value += value + 1;
            if (ai.IsWeak(enemy)) value += 3;
            if (ai.PlayersLevel[enemy] < 3) value -= 10;
            if (ai.HasSkill("tuxi", enemy)) value += 2;
            if (ai.WillSkipPlayPhase(enemy)) value -= 10;
            if (ai.HasSkill("shensu", enemy)) value -= 1;
            if (ai.HasSkill("guanxing|yizhi", enemy)) value -= 5;
            if (ai.HasSkill("tiandu", enemy)) value -= 5;
            if (ai.HasSkill("guidao", enemy)) value -= 3;
            if (ai.HasSkill("tiandu", enemy)) value -= 3;
            if (ai.NeedKongcheng(enemy)) value -= 1;
            return value;
        }
    }

    public class LightningAI : UseCard
    {
        public LightningAI() : base("Lightning")
        {
        }
        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            DamageStruct damage = new DamageStruct(trick, null, to, 3, DamageStruct.DamageNature.Thunder);
            NulliResult result = new NulliResult();
            if (positive)
            {
                if (ai.IsFriend(to))
                {
                    double value = ai.GetDamageScore(damage).Score;
                    if (ai.IsGoodSpreadStarter(damage, false))
                        value -= 6;
                    if (value < -5)
                    {
                        //wizzard skill
                        Player wizzard = ai.GetWizzardRaceWinner(Name, to, to);
                        if (wizzard != null && ai.IsEnemy(wizzard) && ai.PlayersLevel[wizzard] >= 3)
                        {
                            result.Null = true;
                        }
                        //guanxin to do
                    }
                }
            }
            else
            {
                if (ai.IsEnemy(to) && ai.PlayersLevel[to] > 3)
                {
                    //guanxin to do
                }
            }

            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            bool shouldUse = false;
            Room room = ai.Room;
            DamageStruct damage = new DamageStruct(card, null, player, 3, DamageStruct.DamageNature.Thunder);
            damage.Damage = ai.DamageEffect(damage, DamageStruct.DamageStep.Done);
            if (ai.IsCancelTarget(card, player, player) || !ai.IsCardEffect(card, player, player) || damage.Damage == 0)
            {
                shouldUse = true;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    DamageStruct _damage = new DamageStruct(card, null, p, 3, DamageStruct.DamageNature.Thunder);
                    if (ai.GetPlayerTendency(p) == ai.GetPlayerTendency(player)
                        && !ai.IsCancelTarget(card, player, player) && ai.IsCardEffect(card, player, player) && ai.GetDamageScore(_damage).Score < 0)
                    {
                        shouldUse = false;
                        break;
                    }
                }
            }
            if (shouldUse)
            {
                use.Card = card;
                return;
            }

            int friends = 0, enemies = 0;
            bool no_use = false;
            foreach (Player p in room.GetAlivePlayers())
            {
                DamageStruct _damage = new DamageStruct(card, null, p, 3, DamageStruct.DamageNature.Thunder)
                {
                    Damage = ai.DamageEffect(damage, DamageStruct.DamageStep.Done),
                    Steped = DamageStruct.DamageStep.Done
                };
                bool effect = _damage.Damage == 0 || ai.IsCancelTarget(card, player, player) || !ai.IsCardEffect(card, player, player);
                if (effect)
                {
                    if (ai.IsFriend(p))
                    {
                        Player wizzard = ai.GetWizzardRaceWinner(Name, p, p);
                        if (wizzard != null && !ai.IsFriend(wizzard))
                        {
                            no_use = true;
                            break;
                        }
                        if (!ai.HasSkill("guanxing|yizhi", p))
                            friends++;
                        Player last = room.GetLastAlive(p);
                        if (ai.IsEnemy(last) && ai.HasSkill("guanxing|yizhi", last))
                        {
                            no_use = true;
                            break;
                        }
                    }
                    else
                    {
                        Player wizzard = ai.GetWizzardRaceWinner(Name, p, p);
                        if (!ai.HasSkill("guanxing|yizhi", p) || wizzard == null || !ai.IsFriend(p, wizzard))
                            enemies++;
                    }
                }
            }

            if (!no_use)
            {
                if (friends == 0 && enemies > 1)
                {
                    shouldUse = true;
                }
                else if ((double)enemies / friends > 1.5)
                {
                    shouldUse = true;
                }
                if (shouldUse)
                    use.Card = card;
            }
        }
    }

    public class CrossBowAI : UseCard
    {
        public CrossBowAI() : base("CrossBow")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class DoubleSwordAI : UseCard
    {
        public DoubleSwordAI() : base("DoubleSword")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            Player.Gender target_gender = player.PlayerGender == Player.Gender.Male ? Player.Gender.Female :
                player.PlayerGender == Player.Gender.Female ? Player.Gender.Male : Player.Gender.Sexless;
            if (target_gender == Player.Gender.Sexless) return value;
            foreach (Player p in ai.GetEnemies(player))
            {
                if (p.PlayerGender == target_gender)
                {
                    value += 0.5;
                    if (RoomLogic.DistanceTo(ai.Room, player, p, null, true) <= 2)
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
    public class QinggangSwordAI : UseCard
    {
        public QinggangSwordAI() : base("QinggangSword")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
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
    public class SpearAI : UseCard
    {
        public SpearAI() : base("Spear")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            if (ai.HasSkill("paoxiao", player))
                value += 0.3;
            if (ai.HasSkill("tianyi", player))
                value += 0.2;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class AxeAI : UseCard
    {
        public AxeAI() : base("Axe")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            if (ai.HasSkill("luoyi", player))
                value += 1;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class KylinBowAI : UseCard
    {
        public KylinBowAI() : base("KylinBow")
        {
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            return ai.AjustWeaponRangeValue(player, card);
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class EightDiagramAI : UseCard
    {
        public EightDiagramAI() : base("EightDiagram")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.JiaozhuneedSlash(player))
                return true;

            Room room = ai.Room;
            List<string> strs = (List<string>)data;
            string prompt = strs[1];
            if (prompt.StartsWith("slash-jink") || prompt.StartsWith("@multi-jink") || prompt.StartsWith("archery-attack-jink"))
            {
                List<CardUseStruct> uses = (List<CardUseStruct>)room.GetTag("card_proceeing");
                CardUseStruct use = uses[uses.Count - 1];
                DamageStruct damage = new DamageStruct(use.Card, use.From, player);
                if (use.Card.Name == "FireSlash")
                    damage.Nature = DamageStruct.DamageNature.Fire;
                else if (use.Card.Name == "ThunderSlash")
                    damage.Nature = DamageStruct.DamageNature.Thunder;

                if (ai.GetDamageScore(damage).Score > 0)
                    return false;
            }

            return true;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (ai.HasSkill("bazhen", player))
                value -= 2.5;

            if (ai.HasSkill("tiandu", player))
                value += 2;

            if (ai.HasSkill("leiji", player))
                value += 2;

            if (place == Player.Place.PlaceEquip && !ai.HasSkill("bazhen", player))
                value += 4;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class IceSwordAI : UseCard
    {
        public IceSwordAI() : base("IceSword")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            DamageStruct damage = (DamageStruct)data;
            damage.Steped = DamageStruct.DamageStep.Caused;

            ScoreStruct d = ai.GetDamageScore(damage);
            return !d.DoDamage;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids, object data)
        {
            ScoreStruct d = ai.FindCards2Discard(from, to, "he", FunctionCard.HandlingMethod.MethodDiscard, 1);
            if (d.Ids.Count > 0)
                return d.Ids;

            return base.OnCardsChosen(ai, from, to, flags, min, max, disable_ids, data);
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            string skills = TrustedAI.MasochismSkill + "|tianxiang";
            foreach (Player p in ai.GetEnemies(player))
            {
                if (ai.HasSkill(skills, p) && RoomLogic.DistanceTo(ai.Room, player, p, null, true) <= 2)
                    value += 1;
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class RenwangShieldAI : UseCard
    {
        public RenwangShieldAI() : base("RenwangShield")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (ai.HasSkill("bazhen", player))
                value -= 2;

            if (place == Player.Place.PlaceEquip)
                value += 4;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class FanAI : UseCard
    {
        public FanAI() : base("Fan")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            foreach (Player p in ai.GetEnemies(player))
            {
                if (ai.HasArmorEffect(p, "Vine") && RoomLogic.DistanceTo(ai.Room, player, p, null, true) <= 4)
                    value += 1;
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class SixSwordsAI : UseCard
    {
        public SixSwordsAI() : base("SixSwords")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            value += (ai.GetFriends(player).Count - 1) * 0.1;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class TribladeAI : UseCard
    {
        public TribladeAI() : base("Triblade")
        {
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            return ai.AjustWeaponRangeValue(player, card);
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class VineAI : UseCard
    {
        public VineAI() : base("Vine")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (ai.HasSkill("bazhen", player))
                value -= 2;

            if (place == Player.Place.PlaceEquip)
                value += 2.5;

            foreach (Player p in ai.GetFriends(player))
                if (p.Chained)
                    value -= 0.7;

            if (!RoomLogic.PlayerHasSkill(ai.Room, player, "kongcheng") || !(ai.Room.Current == player && player.IsLastHandCard(card)) || !player.IsKongcheng())
            {
                foreach (Player p in ai.GetEnemies(player))
                {
                    if (p.HasWeapon("Fan") || RoomLogic.PlayerHasSkill(ai.Room, p, "huoji") || ai.GetKnownCardsNums("FireAttack", "he", player, p) > 0)
                        value -= 1.5;
                }
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }
    public class SilverLionAI : UseCard
    {
        public SilverLionAI() : base("SilverLion")
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (player.GetLostHp() > 0)
            {
                if (place == Player.Place.PlaceEquip)
                    value -= 7;
                else
                    value += 3;
            }
            if (ai.HasSkill("kurou|duanliang|xiongshuan", player))
                value += 1.2;

            if (place == Player.Place.PlaceEquip)
                value += 2.6;

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class DHorseAI : UseCard
    {
        public DHorseAI(string name) : base(name)
        {
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            return ai.AjudstDHorseValue(player, card);
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class OHorseAI : UseCard
    {
        public OHorseAI(string name) : base(name)
        {
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            return ai.AjustOHorseValue(player, card);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class CollateralAI : UseCard
    {
        public CollateralAI() : base("Collateral")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> fromList = new List<Player>();
            double basic_value = 0;
            if (ai.HasSkill("jizhi", player))
                basic_value += 4;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.GetWeapon() && RoomLogic.IsProhibited(room, player, p, card) == null && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                    fromList.Add(p);
            }

            bool needCrossbow = false;
            if (!ai.HasSkill("paoxiao") && ai.GetCards("Slash", player).Count > 2)
            {
                foreach (Player p in ai.GetEnemies(player))
                {
                    if (ai.PlayersLevel[p] > 3 && RoomLogic.DistanceTo(room, player, p) == 1)
                    {
                        needCrossbow = true;
                    }
                }
            }
            bool needWeapon = false;
            if (!player.GetWeapon())
            {
                foreach (Player p in ai.GetEnemies(player))
                {
                    if (!RoomLogic.InMyAttackRange(room, player, p, null))
                        needWeapon = true;
                }
            }

            //todo adjust halberg
            Dictionary<Player, Player> from_to = new Dictionary<Player, Player>();
            Dictionary<Player, double> from_values = new Dictionary<Player, double>();
            foreach (Player from in fromList)
            {
                double from_value = 0;
                if (ai.IsEnemy(from))
                {
                    from_value += 3;
                    if (ai.IsLackCard(from, "Slash") || (ai.GetKnownCardsNums("Slash", "he", from, player) == 0 && from.HandcardNum - ai.GetKnownCards(from).Count < 2))
                        from_value += 3;
                    else if (from.HasWeapon("Halberg"))
                        from_value -= 3;
                }

                if (from.HasWeapon("CrossBow") && needCrossbow)
                    from_value += 4;

                if (needWeapon)
                    from_value += 2;
                

                Dictionary<Player, double> to_values = new Dictionary<Player, double>();
                foreach (Player to in room.GetOtherPlayers(from))
                {
                    double to_value = -100;
                    if (RoomLogic.CanSlash(room, from, to))
                    {
                        to_value = 0;
                        if (ai.IsEnemy(to) && !ai.NotSlashJiaozhu(to))
                        {
                            to_value += 3;
                            if (ai.IsWeak(to))
                                to_value += 5;
                        }
                        if (ai.JiaozhuneedSlash(to))
                            to_value += 6;
                    }
                    to_values[to] = to_value;
                }

                List<Player> tos = new List<Player>(to_values.Keys);
                tos.Sort((x, y) => { return to_values[x] > to_values[y] ? -1 : 1; });
                if (to_values[tos[0]] >= 0)
                {
                    from_value += to_values[tos[0]];
                    from_to[from] = tos[0];
                    from_values[from] = from_value;
                }
            }

            List<Player> froms = new List<Player>(from_values.Keys);
            if (froms.Count > 0)
            {
                froms.Sort((x, y) => { return from_values[x] > from_values[y] ? -1 : 1; });
                use.To.Add(froms[0]);
                use.To.Add(from_to[froms[0]]);
                use.Card = card;
            }
            else if (basic_value > 0)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    use.To = new List<Player>();
                    if (p.GetWeapon() && RoomLogic.IsProhibited(room, player, p, card) == null && (ai.IsCancelTarget(card, p, player) || !ai.IsCardEffect(card, p, player)))
                    {
                        use.To.Add(p);
                        foreach (Player to in room.GetOtherPlayers(p))
                        {
                            if (RoomLogic.CanSlash(room, p, to))
                            {
                                use.To.Add(to);
                                use.Card = card;
                                return;
                            }
                        }
                    }
                }
            }
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            base.OnEvent(ai, triggerEvent, player, data);
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player
            };
            Room room = ai.Room;
            List<WrappedCard> slashes = ai.GetCards("Slash", player);
            if (slashes.Count == 0) return use;
            List<string> strs = new List<string>(prompt.Split(':'));
            Player target = room.FindPlayer(strs[2]);
            Player target2 = room.FindPlayer(strs[1]);
            if (ai.IsFriend(target) && player.HasWeapon("CrossBow") && ai.GetKnownCardsNums("Slash", "he", target, player) > 1)
                return use;

            if (ai.JiaozhuneedSlash(target2))
            {
                foreach (WrappedCard slash in slashes)
                {
                    if (!ai.IsCancelTarget(slash, target2, player) && ai.IsCardEffect(slash, target2, player))
                    {
                        use.Card = slash;
                        use.To = new List<Player> { target2 };
                        return use;
                    }
                }
            }
            else if (ai.NotSlashJiaozhu(target2))
            {
                if (ai.IsEnemy(target))
                {
                    foreach (WrappedCard slash in slashes)
                    {
                        if (ai.IsCancelTarget(slash, target2, player) || !ai.IsCardEffect(slash, target2, player))
                        {
                            use.Card = slash;
                            use.To = new List<Player> { target2 };
                            return use;
                        }
                    }
                }
                else
                    return use;
            }

            if (ai.ProcessPublic == "===" && slashes.Count <= 2)
                return use;

            double keep_value = ai.GetKeepValue(player.Weapon.Key, player, Player.Place.PlaceEquip);
            if (keep_value < 0)
                return use;

            List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, new List<Player> { target2 });
            if (scores[0].Score >= -2)
            {
                use.Card = scores[0].Card;
                use.To = scores[0].Players;
                return use;
            }

            //todo adjust halberg

            return use;
        }

        public override NulliResult OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();

            return result;
        }
    }

    public class MegatamaCardAI : UseCard
    {
        public MegatamaCardAI() : base("MegatamaCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
                use.Card = card;
        }
    }

    public class MegatamaAI : SkillEvent
    {
        public MegatamaAI() : base("megatama")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player
            };
            if (ai.GetOverflow(player) >= 2)
            {
                WrappedCard card = new WrappedCard("MegatamaCard");
                use.Card = card;
            }

            return use;
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.HandcardNum < RoomLogic.GetMaxCards(ai.Room, player))
            {
                return new WrappedCard("MegatamaCard");
            }
            return null;
        }
    }

    public class CompanionCardAI : UseCard
    {
        public CompanionCardAI() : base("CompanionCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class CompanionAI : SkillEvent
    {
        public CompanionAI() : base("companion")
        {
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (player.IsKongcheng() && ai.NeedKongcheng(player))
                return "peach";

            if (RoomLogic.GetMaxCards(ai.Room, player) >= player.HandcardNum + 2)
                return "draw";

            return "peach";
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (pattern == "Peach")
            {
                WrappedCard card = new WrappedCard("Peach");
                WrappedCard _card = new WrappedCard("CompanionCard");
                card.UserString = RoomLogic.CardToString(ai.Room, _card);
            }

            return result;
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.IsWounded() || (ai.GetOverflow(player) == 0 && !(player.IsKongcheng() && ai.NeedKongcheng(player))))
            {
                foreach (Player p in ai.FriendNoSelf)
                {
                    if (ai.IsWeak(p))
                        return null;
                }

                return new WrappedCard("CompanionCard");
            }

            return null;
        }
    }

    public class PioneerCardAI : UseCard
    {
        public PioneerCardAI() : base("PioneerCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class PioneerAI : SkillEvent
    {
        public PioneerAI() : base("pioneer")
        {
        }

        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            if (ai.Choice.ContainsKey(Name) && choices.Contains(ai.Choice[Name]))
                return ai.Choice[Name];

            if (choices.Contains("head_general"))
                return "head_general";
            else if (choices.Contains("deputy_general"))
                return "deputy_general";

            return base.OnChoice(ai, player, choices, data);
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && ai.Self == player)
                {
                    string choice = strs[2];
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag("KnownBothTarget"))
                        {
                            target = p;
                            break;
                        }
                    }
                    if (target != null)
                    {
                        if (choice == "head_general")
                        {
                            ai.SetKnown(target, "h");
                        }
                        else if (choice == "deputy_general")
                            ai.SetKnown(target, "d");
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max, object data)
        {
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (ai.GetPlayerTendency(p) == "unknown")
                    return new List<Player> { p };
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.HasShownOneGeneral() && !ai.IsKnown(player, p, "h") && !ai.IsKnown(player, p, "d"))
                {
                    if (ai.IsKnown(player, p, "h"))
                        ai.Choice[Name] = "deputy_general";
                    else
                        ai.Choice[Name] = "head_general";

                    return new List<Player> { p };
                }
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.HasShownAllGenerals())
                    return new List<Player> { p };
            }

            return null;
        }

        public override WrappedCard GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.MaxHp - player.HandcardNum >= 2 && !(player.IsKongcheng() && ai.NeedKongcheng(player)))
            {
                return new WrappedCard("PioneerCard");
            }

            return null;
        }
    }
}
