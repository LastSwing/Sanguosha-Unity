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
                new ExNihiloAI(),
                new AwaitExhaustedAI(),
                new BefriendAttackingAI(),
                new FireAttackAI(),
                new KnownBothAI(),
            };
        }
    }
    public class SlashAI : UseCard
    {
        public SlashAI() : base("Slash")
        {
            key = new List<string> { "cardResponded" };
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage)
        {
            if (damage.From != null && damage.From.Alive)
                damage.Damage += damage.From.GetMark("drank");
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
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

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data, FunctionCard.HandlingMethod method)
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
                        CardUseStruct tianxiang_use = tianxiang.OnResponding(ai, player, "@@tianxiang", string.Empty, damage, FunctionCard.HandlingMethod.MethodUse);
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

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage)
        {
            if (damage.From != null && damage.From.Alive)
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

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage)
        {
            if (damage.From != null && damage.From.Alive)
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
                foreach (Player p in room.AlivePlayers)
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

        public override bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
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
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card) && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                    scores.Add(ai.FindCards2Discard(player, p, "hej", FunctionCard.HandlingMethod.MethodDiscard));
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

        public override bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
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
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card) && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                    scores.Add(ai.FindCards2Discard(player, p, "hej", FunctionCard.HandlingMethod.MethodGet));
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

        public override bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //针对所选择的卡牌判断敌友
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

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
        }

        public override bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
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

        public override bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
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

        public override bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
        }
    }

    public class BefriendAttackingAI : UseCard
    {
        public BefriendAttackingAI() : base("BefriendAttacking")
        {
        }

        public override bool OnNullification(TrustedAI ai, Player from, Player to, WrappedCard trick)
        {
            return false;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> players = ai.Exclude(room.GetOtherPlayers(player), card), targets = new List<Player>();
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
            //对会取消卡牌效果的使用
            FunctionCard fcard = Engine.GetFunctionCard(Name);
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card) && !ai.IsCardEffect(card, p, player))
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
        public FireAttackAI() : base("FireAttack")
        {
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
            List<Player> players = ai.Exclude(room.AlivePlayers, card, player);
            foreach (Player p in players)
            {
                DamageStruct damage = new DamageStruct(card, player, p, 1, DamageStruct.DamageNature.Fire);
                damage.Damage = ai.DamageEffect(damage);
                ScoreStruct score = ai.GetDamageScore(damage);
                score.Players = new List<Player> { p };
                if (score.DoDamage && p.Chained)
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
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data, FunctionCard.HandlingMethod method)
        {
            return new CardUseStruct();
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
                    foreach (Player p in room.AlivePlayers)
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
}
