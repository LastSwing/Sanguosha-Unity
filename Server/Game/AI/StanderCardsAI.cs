using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;

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
                if (strs[1] == "Slash")
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
                if (ai.IsCardEffect(card, p, player))
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
                if (ai.IsCardEffect(card, friend, player))
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
                if (ai.IsCardEffect(card, enemy, player))
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
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card))
                    scores.Add(ai.FindCards2Discard(player, p, "hej", FunctionCard.HandlingMethod.MethodDiscard));
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 4 || (scores[0].Score > 0 && ai.GetOverflow(player) > 0))
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
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card))
                    scores.Add(ai.FindCards2Discard(player, p, "hej", FunctionCard.HandlingMethod.MethodGet));
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 4 || (scores[0].Score > 0 && ai.GetOverflow(player) > 0))
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
}
