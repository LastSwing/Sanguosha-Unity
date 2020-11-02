using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static CommonClass.Game.Player;
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
                new AmazingGraceAI(),
                new GodSalvationAI(),
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
                new HegNullificationAI(),
            };

            events = new List<SkillEvent>
            {
                new DoubleSwordSkillAI(),
                new SpeakSKillAI(),
                new CompanionAI(),
                new MegatamaAI(),
                new PioneerAI(),
            };
        }
    }
    public class SlashAI : UseCard
    {
        public SlashAI() : base(Slash.ClassName)
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
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && (use.Reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY || use.Pattern == "@@rende"))
            {
                if (ai is SmartAI && ai.Self != player)
                {
                    foreach (Player p in use.To)
                        if (!ai.IsCancelTarget(use.Card, p, player) && ai.IsCardEffect(use.Card, p, player) &&
                            (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p)))
                            ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
                }
                else if (ai is StupidAI _ai)
                {
                    foreach (Player p in use.To)
                    {
                        if (ai.GetPlayerTendency(p) != "unknown" && !ai.IsCancelTarget(use.Card, p, player) && ai.IsCardEffect(use.Card, p, player))
                        {
                            if (ai.HasSkill("leiji|leiji_jx", p) && !ai.IsLackCard(p, "Jink") && (ai.HasArmorEffect(p, EightDiagram.ClassName) || p.HandcardNum >= 3)
                                && !ai.HasSkill("tieqi_jx|liegong_jx"))
                            {
                                ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 60);
                            }
                            else if (ai.HasSkill(TrustedAI.MasochismSkill, p) && ai.HasSkill("jueqing|gangzhi_classic", player))
                            {
                                ai.UpdatePlayerRelation(player, p, false);
                            }
                            else
                            {
                                int count = 1 + use.Drank + use.ExDamage;
                                if (ai.HasSkill("liegong_jx", player) && player.Hp <= p.Hp) count++;
                                DamageStruct damage = new DamageStruct(use.Card, player, p, count);
                                if (use.Card.Name == FireSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Fire;
                                else if (use.Card.Name == ThunderSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Thunder;

                                if ((ai.HasSkill("zhiman_jx", player) || player.HasWeapon(IceSword.ClassName)) && ai.GetPlayerTendency(player) != "unknown")
                                    continue;

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

                                        continue;
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

                                        continue;
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

            if (ai.Self == player) return;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split('%'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_")
                    {
                        string prompt = strs[3];
                        if (!prompt.StartsWith("@multi-jink") || strs[strs.Count - 2] == "1")
                        {
                            List<CardUseStruct> uses = room.GetUseList();
                            use = uses[uses.Count - 1];
                            if (use.Card != null && use.Card.Name.Contains(Slash.ClassName))
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, player);
                                if (use.Card.Name == FireSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Fire;
                                else if (use.Card.Name == ThunderSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Thunder;

                                ScoreStruct score = ai.GetDamageScore(damage);
                                bool lack = true;
                                if (ai.IsFriend(use.From, player) && score.Score > 0)
                                    lack = false;
                                else if (ai.IsEnemy(player) && score.Score < 0)
                                    lack = false;

                                if (lack) ai.SetCardLack(player, Jink.ClassName);
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
                DamageStruct damage = new DamageStruct(effect.Slash, effect.From, effect.To, 1 + effect.Drank + effect.ExDamage);
                if (effect.Slash.Name == FireSlash.ClassName)
                    damage.Nature = DamageStruct.DamageNature.Fire;
                else if (effect.Slash.Name == ThunderSlash.ClassName)
                    damage.Nature = DamageStruct.DamageNature.Thunder;
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Damage.Damage > 0 && player.Chained && score.Damage.Nature != DamageStruct.DamageNature.Normal)
                {
                    score.Score += ai.ChainDamage(damage);
                }

                CardUseStruct use = new CardUseStruct
                {
                    From = player
                };
                List<WrappedCard> cards = ai.GetCards(Jink.ClassName, player);
                if (score.Score >= 0 || cards.Count == 0) return use;

                int rest = 1;
                if (prompt.StartsWith("@multi-jink"))
                {
                    List<string> strs = new List<string>(prompt.Split(':'));
                    rest = int.Parse(strs[strs.Count - 2]);
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
                if (result == null) return use;

                int rate = 1;
                if (ai.WillSkipPlayPhase(player) || !ai.WillShowForAttack() || ai.GetOverflow(player) > 0) rate = 2;
                double value = -ai.GetKeepValue(result, player);
                if (value < 0)
                    value /= rate;
                else
                    value *= rate;

                if (player.GetMark("@tangerine") > 0) value += 4;
                if (ai.HasSkill("leiji|leiji_jx|shicai_jx|tushe", player)) value += 3;
                if (player.Phase != PlayerPhase.NotActive) value += 3;
                if (result.Skill == "longdan")
                {
                    foreach (Player p in ai.Room.GetOtherPlayers(player))
                    {
                        if (p.IsWounded() && ai.IsFriend(p, player) && p != effect.From)
                        {
                            value += 3;
                            break;
                        }
                    }
                }

                if (result.Skill == "longdan_jx" && ai.HasSkill("chongzhen", player) && !ai.IsFriend(player, effect.From) && !effect.From.IsKongcheng())
                    value += 2;

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

                if (damage.Damage > 0 && score.Score < -2 && ai.HasSkill("tianxiang_jx", player))
                {
                    SkillEvent tianxiang = Engine.GetSkillEvent("tianxiang_jx");
                    if (tianxiang != null)
                    {
                        CardUseStruct tianxiang_use = tianxiang.OnResponding(ai, player, "@@tianxiang_jx", string.Empty, damage);
                        if (tianxiang_use.Card != null && tianxiang_use.To != null && tianxiang_use.To.Count > 0)
                            score.Score = -2;
                    }
                }

                if (rest > 1)
                {
                    double need = rest;
                    if (ai.HasArmorEffect(player, EightDiagram.ClassName))
                        need = (rest - 1) * 0.6 + 1;

                    if (available >= need)
                        value -= 4 * (int)(need - 1);
                    else
                        value += score.Score * Math.Min(1, need - 1);
                }
                if (ai.MaySave(player) && player.FaceUp) value += 1.5;
                if (available > rest && ai.GetOverflow(player) > 0 || !ai.IsSituationClear()) value += 2;

                if (value >= score.Score) use.Card = result;

                return use;
            }

            return new CardUseStruct();
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.FindSlashandTarget(ref use, player);
        }

        public static double Value(TrustedAI ai, Player player, WrappedCard card, Player.Place place)
        {
            if (card.Name.Contains(Slash.ClassName) && (ai.GetKnownCardsNums(CrossBow.ClassName, "he", player, ai.Self) > 0 || ai.HasSkill("paoxiao", player)))
            {
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { card }, null, false);
                if (scores.Count > 0 && scores[0].Score > 0)
                    return 1.5;
            }

            return 0;
        }

        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return Value(ai, player, card, place);
        }

        public static double Priority(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            Room room = ai.Room;
            double value = 0;
            if (card.Skill == Spear.ClassName)
                value -= 0.1;

            if (WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card)))
                value -= 0.05;

            if (player.GetMark("yongjue") == 0 && player.Phase == Player.PlayerPhase.Play)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (RoomLogic.PlayerHasShownSkill(room, p, "yongjue") && RoomLogic.IsFriendWith(room, p, player))
                    {
                        value += 20;
                        break;
                    }
                }
            }

            if (card.Skill == "wusheng")
            {
                WrappedCard c = room.GetCard(card.GetEffectiveId());
                double prio = Engine.GetCardPriority(card.Name) - Engine.GetCardPriority(c.Name);
                value += prio < 0 ? prio : 0;
            }

            return value;
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return Priority(ai, player, targets, card);
        }
    }

    public class FireSlashAI : UseCard
    {
        public FireSlashAI() : base(FireSlash.ClassName)
        {
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.Steped < DamageStruct.DamageStep.Caused && step >= DamageStruct.DamageStep.Caused && damage.From != null && damage.From.Alive)
                damage.Damage += damage.From.GetMark("drank");
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.FindSlashandTarget(ref use, player);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return SlashAI.Value(ai, player, card, place);
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return SlashAI.Priority(ai, player, targets, card);
        }
    }
    public class ThunderSlashAI : UseCard
    {
        public ThunderSlashAI() : base(ThunderSlash.ClassName)
        {
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.Steped < DamageStruct.DamageStep.Caused && step >= DamageStruct.DamageStep.Caused && damage.From != null && damage.From.Alive)
                damage.Damage += damage.From.GetMark("drank");
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.FindSlashandTarget(ref use, player);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return SlashAI.Value(ai, player, card, place);
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return SlashAI.Priority(ai, player, targets, card);
        }
    }

    public class AnalepticAI : UseCard
    {
        public AnalepticAI() : base(Analeptic.ClassName)
        {
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            bool hand = false;
            //当酒为手牌时且溢出并且不在保留手牌中时，若敌方有二张，则应将其用掉
            foreach (int id in card.SubCards)
            {
                if (ai.Room.GetCardOwner(id) == player && ai.Room.GetCardPlace(id) == Place.PlaceHand)
                {
                    hand = true;
                    break;
                }
            }
            if (hand && player.Phase == PlayerPhase.Play && ai.GetOverflow(player) >= 2)
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
                bool should_use = false;
                if (enemy_erzhang)
                {
                    List<int> hands= player.GetCards("h");
                    ai.SortByKeepValue(ref hands, false);
                    foreach (int id in card.SubCards)
                    {
                        if (hands.IndexOf(id) < ai.GetOverflow(player) - 1)
                        {
                            should_use = true;
                            break;
                        }
                    }

                }
                if (!should_use && ai.HasSkill("zishu")) should_use = true;
                if (should_use)
                {
                    use.Card = card;
                    return;
                }
            }
            //刘焉清基本牌
            if (ai.HasSkill("tushe") && Slash.IsAvailable(ai.Room, player))
            {
                use.Card = card;
                return;
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
        public PeachAI() : base(Peach.ClassName)
        {
            key = new List<string> { "peach" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            //判断出桃子救援的行为
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                Player dying = room.FindPlayer(strs[1]);
                if (player != dying)
                {
                    if (ai is SmartAI && ai.Self != player)
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
                    else if (ai is StupidAI && ai.GetPlayerTendency(dying) != "unknown" && !ai.HasSkill("yechou", dying))
                    {
                        ai.UpdatePlayerRelation(player, dying, true);
                    }
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
            foreach (int id in player.GetCards("h"))
            {
                if (ai.IsCard(id, Peach.ClassName, player))
                    peaches++;
            }
            foreach (int id in card.SubCards)
            {
                if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == player)
                    hand = true;
                if (player.GetPile("wooden_ox").Contains(id))
                    wooden = true;
            }

            if (hand && ai.HasSkill("zhiheng_jx") && !player.HasUsed(ZhihengJCard.ClassName))
            {
                use.Card = card;
                return;
            }
            if (player.GetMark("@rob") > 0 && !ai.HasSkill("jieying_gn"))           //被劫营一定吃桃
            {
                Player gn = RoomLogic.FindPlayerBySkillName(room, "jieying_gn");
                if (gn != null && !ai.IsFriend(gn))
                {
                    use.Card = card;
                    return;
                }
            }

            if (ai.HasSkill("tushe") && hand)           //刘焉必定吃光桃子
            {
                use.Card = card;
                return;
            }

            //进入鏖战前将桃吃完
            if (room.AliveCount() <= room.Players.Count / 2 && room.Setting.GameMode == "Hegemony" && !card.IsVirtualCard())
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
                        if ((ai.GetKnownCardsNums(Dismantlement.ClassName, "he", enemy, player) > 0 || ai.HasSkill("qixi", enemy))
                            || (RoomLogic.DistanceTo(room, enemy, player) == 1 && (ai.GetKnownCardsNums(Snatch.ClassName, "he", enemy, player) > 0 || ai.HasSkill("jixi", enemy)))
                            || (ai.HasSkill("jianchu", enemy) && RoomLogic.CanSlash(room, enemy, player))
                            || (ai.HasSkill("tiaoxin", enemy) && RoomLogic.InMyAttackRange(room, player, enemy)
                            && (ai.GetKnownCardsNums(Slash.ClassName, "he", player) == 0 || !RoomLogic.CanSlash(room, player, enemy))))
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
                List<int> hands= player.GetCards("h");
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

            //奇策前用掉

            if (hand)
            {
                /*
                if (ai.HasSkill("qice"))
                {
                    SkillEvent e = Engine.GetSkillEvent("qice");
                    if (e != null)
                    {
                        List<WrappedCard> cards = e.GetTurnUse(ai, player);
                        foreach (WrappedCard qice in cards)
                        {
                            CardUseStruct _use = new CardUseStruct(null, player, new List<Player>())
                            {
                                IsDummy = true
                            };

                            UseCard u = Engine.GetCardUsage(qice.Name);
                            if (u != null)
                            {
                                u.Use(ai, player, ref _use, qice);
                                if (_use.Card != null)
                                {
                                    use.Card = card;
                                    return;
                                }
                            }
                        }
                    }
                }
                */
                if (ai.HasSkill("qice_jx"))
                {
                    SkillEvent e = Engine.GetSkillEvent("qice_jx");
                    if (e != null)
                    {
                        List<WrappedCard> cards = e.GetTurnUse(ai, player);
                        foreach (WrappedCard qice in cards)
                        {
                            CardUseStruct _use = new CardUseStruct(null, player, new List<Player>())
                            {
                                IsDummy = true
                            };

                            UseCard u = Engine.GetCardUsage(qice.Name);
                            if (u != null)
                            {
                                u.Use(ai, player, ref _use, qice);
                                if (_use.Card != null)
                                {
                                    use.Card = card;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //为队友保留桃子
            foreach (Player p in ai.FriendNoSelf)
            {
                if (ai.HasSkill("jiuyuan", p) && p.Hp < player.Hp && player.Kingdom == p.Kingdom)
                {
                    use.Card = card;
                    return;
                }
                if (ai.IsWeak(p) && (peaches == 1 || !hand || !overflow))
                    return;
            }

            use.Card = card;
        }
    }

    public class AmazingGraceAI : UseCard
    {
        public AmazingGraceAI() : base(AmazingGrace.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            double value = 2, good = 0, rate = 1;
            Room room = ai.Room;
            double adjust = 0;
            int target_num = 0;
            List<Player> targets = room.GetAlivePlayers();
            room.SortByActionOrder(ref targets);
            Player dongyun = RoomLogic.FindPlayerBySkillName(room, "sheyan");
            if (dongyun != null && RoomLogic.IsProhibited(room, player, dongyun, card) != null) dongyun = null;
            double best_good = 0, best_bad = 0;
            foreach (Player p in targets)
            {
                if (RoomLogic.IsProhibited(room, player, p, card) == null && !ai.IsCancelTarget(card, p, player))
                    target_num++;
            }
            if (ai.HasSkill("tushe"))
            {
                bool basic = false;
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard wrapped = room.GetCard(id);
                    if (Engine.GetFunctionCard(wrapped.Name).TypeID == FunctionCard.CardType.TypeBasic)
                    {
                        basic = true;
                        break;
                    }
                }
                if (!basic) adjust = 1.5 * target_num;
            }
            if (!card.IsVirtualCard() && ai.HasSkill("jizhi|jizhi_jx")) adjust += 2;

            foreach (Player p in targets)
            {
                if (RoomLogic.IsProhibited(room, player, p, card) == null && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                {
                    if (ai.IsFriend(p))
                    {
                        value *= rate;
                        rate = 1;
                        good += value;
                        if (best_good == 0) best_good = value;
                        value *= 0.8;
                    }
                    else
                    {
                        rate *= 0.8;
                        if (ai.IsEnemy(p))
                        {
                            if (best_bad == 0) best_bad = value;
                            good -= value;
                        }
                        else if (ai.IsSituationClear())
                        {
                            if (best_bad == 0) best_bad = value / 2;
                            good -= value / 2;
                        }
                    }
                }
            }

            if (dongyun != null)
            {
                if (ai.IsFriend(dongyun))
                    good += best_bad;
                else if (ai.IsEnemy(dongyun))
                    good -= best_good;
            }
            if (player.HasFlag("qiaoshui") && player.GetMark("qiaoshui") == 0)
                good += best_bad;

            good += adjust;
            if (good > 0) use.Card = card;
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;
            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);
            delete = new List<Player>(targets);

            double before = 0, after = 0;
            List<int> ag_card = new List<int>((List<int>)room.GetTag(Name));
            foreach (Player p in targets)
            {
                double best = -100;
                int best_id = -1;
                foreach (int id in ag_card)
                {
                    double value;
                    if (room.Current == p)
                        value = ai.GetUseValue(id, p, Place.PlaceHand);
                    else
                        value = ai.GetKeepValue(id, p, Place.PlaceHand);

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
                    if (room.NullTimes == 0 && ai.GetKnownCardsNums(HegNullification.ClassName, "he", player) > 0 || room.HegNull)
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
                ag_card = new List<int>((List<int>)room.GetTag(Name));
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

                    ag_card = new List<int>((List<int>)room.GetTag(Name));
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
        public GodSalvationAI() : base(GodSalvation.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            double good = 0, bad = 0;
            int wounded_friend = 0;
            if ((!card.IsVirtualCard() || card.SubCards.Count == 0) && ai.HasSkill("jizhi|jizhi_jx", player))
                good += 6;
            if ((ai.HasSkill("kongcheng|kongcheng_jx", player) || ai.HasLoseHandcardEffective()) && player.IsLastHandCard(card, true))
                good += 5;

            Player dongyun = RoomLogic.FindPlayerBySkillName(room, "sheyan");
            if (dongyun != null && RoomLogic.IsProhibited(room, player, dongyun, card) != null) dongyun = null;
            double best_good = 0, best_bad = 0;

            foreach (Player friend in ai.GetFriends(player))
            {
                good += 10 * ai.GetKnownCardsNums(Nullification.ClassName, "he", friend, player);
                if (!ai.IsCancelTarget(card, friend, player) && ai.IsCardEffect(card, friend, player) && RoomLogic.IsProhibited(room, player, friend, card) == null)
                {
                    if (friend.IsWounded())
                    {
                        double value = 10;
                        wounded_friend++;
                        if (ai.HasSkill(TrustedAI.MasochismSkill, friend))
                            value += 5;
                        if (ai.IsWeak(friend) && friend.Hp <= 1)
                            value += 5;
                        if (ai.NeedToLoseHp(new DamageStruct(string.Empty, null, friend), true, true))
                            value -= 3;
                        if (ai.HasSkill("qingxian|liexian|hexian", friend))
                            value += 6;

                        good += value;
                        if (value > best_good) best_good = value;
                    }
                }
            }

            foreach (Player enemy in ai.GetEnemies(player))
            {
                good -= 10 * ai.GetKnownCardsNums(Nullification.ClassName, "he", enemy, player);
                if (!ai.IsCancelTarget(card, enemy, player) && ai.IsCardEffect(card, enemy, player)
                    && ai.IsCardEffect(card, enemy, player) && RoomLogic.IsProhibited(room, player, enemy, card) == null)
                {
                    if (enemy.IsWounded())
                    {
                        double value = 10;
                        if (ai.HasSkill(TrustedAI.MasochismSkill, enemy))
                            value += 5;
                        if (ai.IsWeak(enemy) && enemy.Hp <= 1)
                            value += 5;
                        if (ai.NeedToLoseHp(new DamageStruct(string.Empty, null, enemy), true, true))
                            value -= 3;
                        if (ai.HasSkill("qingxian|liexian|hexian", enemy))
                            value += 8;

                        bad += value;
                        if (value > best_bad) best_bad = value;
                    }
                }
            }

            if (dongyun != null)
            {
                if (ai.IsFriend(dongyun))
                    bad -= best_bad;
                else if (ai.IsEnemy(dongyun))
                    good -= best_good;
            }
            if (player.HasFlag("qiaoshui") && player.GetMark("qiaoshui") == 0)
                bad -= best_bad;

            if (good - bad > 5 && wounded_friend > 0)
            {
                use.Card = card;
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;

            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            double good = 0;
            if (positive)
            {
                if (ai.IsEnemy(to))
                {
                    int count = 0;
                    good += 4;
                    if (ai.HasSkill(TrustedAI.MasochismSkill, to))
                        good += 3;
                    if (ai.IsWeak(to) && to.Hp <= 1)
                        good += 5;
                    if (ai.HasSkill("qingxian|liexian|hexian", to))
                        good += 10;

                    if (ai.GetCards(HegNullification.ClassName, player).Count > 0 || (room.NullTimes > 0 && room.HegNull))
                    {
                        foreach (Player p in targets)
                        {
                            if (RoomLogic.IsFriendWith(room, p, to) && p.IsWounded())
                            {
                                count++;
                                if (p != to)
                                {
                                    good += 4;
                                    if (ai.HasSkill(TrustedAI.MasochismSkill, p))
                                        good += 3;
                                    if (ai.IsWeak(p) && to.Hp <= 1)
                                        good += 5;
                                }
                            }
                        }
                    }

                    if (good > 8)
                    {
                        result.Null = true;
                        if (count > 1)
                            result.Heg = !room.HegNull;
                    }

                    return result;
                }
            }
            else
            {
                if (ai.IsFriend(to))
                {
                    int count = 0;
                    good += 4;
                    if (ai.HasSkill(TrustedAI.MasochismSkill, to))
                        good += 3;
                    if (ai.IsWeak(to) && to.Hp <= 1)
                        good += 5;
                    if (ai.HasSkill("qingxian|liexian|hexian", to))
                        good += 10;

                    if (room.NullTimes > 0 && room.HegNull)
                    {
                        foreach (Player p in targets)
                        {
                            if (RoomLogic.IsFriendWith(room, p, to) && p.IsWounded())
                            {
                                count++;
                                if (p != to)
                                {
                                    good += 4;
                                    if (ai.HasSkill(TrustedAI.MasochismSkill, p))
                                        good += 3;
                                    if (ai.IsWeak(p) && to.Hp <= 1)
                                        good += 5;
                                }
                            }
                        }
                    }
                    if (good > 8)
                        result.Null = true;
                }
            }
            
            return result;
        }
    }

    public class DismantlementAI : UseCard
    {
        public DismantlementAI() : base(Dismantlement.ClassName)
        {
            key = new List<string> { "cardChosen:Dismantlement" };
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            FunctionCard fcard = Engine.GetFunctionCard(Name);
            List<Player> targets = ai.Exclude(room.GetOtherPlayers(player), card, player);
            foreach (Player p in targets)
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, Name, "hej", FunctionCard.HandlingMethod.MethodDiscard);
                if (ai.HasSkill("sheyan", p) && score.Score > 0)
                {
                    if (ai.IsFriend(p))
                        score.Score += 8;
                    else
                        score.Score /= 2;
                }
                if (ai.HasSkill("qianya", p) && score.Score > 0)
                {
                    if (ai.IsFriend(p))
                        score.Score += 1;
                    else
                        score.Score /= 2;
                }
                foreach (string skill in ai.GetKnownSkills(p))
                {
                    SkillEvent ev = Engine.GetSkillEvent(skill);
                    if (ev != null)
                        score.Score += ev.TargetValueAdjust(ai, card, player, new List<Player>{ p }, p);
                }
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
                if (player.ContainsTag("xianzhen") && player.GetTag("xianzhen") is List<string> names)
                {
                    bool good = false;
                    foreach (ScoreStruct score in scores)
                    {
                        if (names.Contains(score.Players[0].Name) && score.Score > 0)
                        {
                            good = true;
                            break;
                        }
                    }

                    if (good)
                    {
                        foreach (ScoreStruct score in scores)
                        {
                            if (score.Score > 0 && !names.Contains(score.Players[0].Name))
                            {
                                use.Card = card;
                                use.To = score.Players;
                                return;
                            }
                        }
                    }
                }

                if (scores[0].Score > 4 || (scores[0].Score > 0 && hand && ai.GetOverflow(player) > 0))
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                }
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;

            NulliResult result = new NulliResult();
            int num = ai.GetKnownCardsNums(Nullification.ClassName, "he", ai.Self);
            if (!from.Alive || !to.Alive || to.IsAllNude() || (keep && num == 1)) return result;

            if (positive)
            {
                if ((RoomLogic.PlayerContainsTrick(room, to, Indulgence.ClassName) || RoomLogic.PlayerContainsTrick(room, to, SupplyShortage.ClassName))
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
                    if (to.HasTreasure(JadeSeal.ClassName))
                        use = true;
                    else if (to.HasTreasure(WoodenOx.ClassName))
                        use = true;
                    else if (to.HasArmor(PeaceSpell.ClassName) && !RoomLogic.PlayerHasSkill(room, to, "wendao"))
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
                if (ai.IsFriend(from) && ai.Self == room.Current && num > 1 && ai.GetOverflow(ai.Self) > 0)
                {
                    result.Null = true;
                    return result;
                }
            }

            return result;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai is StupidAI && triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && use.Reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                foreach (Player p in use.To)
                {
                    if (ai.HasSkill("qianxun_jx", p) && ai.GetPlayerTendency(p) != "unknown" && p.HandcardNum > 1 && ai.GetFriends(p).Count > 1)
                        ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 80);
                }
            }

            //针对所选择的卡牌判断敌友
            if (ai.Self == player && !(ai is StupidAI)) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);
                    if (room.GetCardPlace(card_id) == Place.PlaceJudge)
                    {
                        if (target.HasFlag("sheyan") && player.GetCards("he").Count == 0) return;
                        if (room.GetCard(card_id).Name != Lightning.ClassName)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            Player winner = ai.GetWizzardRaceWinner(room.GetCard(card_id).Name, target, target);
                            if (winner != null && ai.IsFriend(winner, target))
                                ai.UpdatePlayerRelation(player, target, false);
                            else
                                ai.UpdatePlayerRelation(player, target, true);
                        }
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceHand && !(ai.HasSkill("qianxun_jx", target) && target.HandcardNum > 1) && !target.HasFlag("sheyan"))
                    {
                        ai.UpdatePlayerRelation(player, target, ai.HasSkill("kongcheng|kongcheng_jx") && target.HandcardNum == 1 ? true : false);
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                    {
                        if (target.HasFlag("sheyan") && player.GetCards("h").Count == 0) return;
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Player.Place.PlaceEquip) > 0 ? false : true);
                    }
                }
            }
        }
    }
    public class SnatchAI : UseCard
    {
        public SnatchAI() : base(Snatch.ClassName)
        {
            key = new List<string> { "cardChosen:Snatch" };
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (card.Skill == "jixi")
                return IsRed(RoomLogic.GetCardSuit(ai.Room, card)) ? -0.2 : 0;

            return 0;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            FunctionCard fcard = Engine.GetFunctionCard(Name);
            List<Player> targets = ai.Exclude(room.GetOtherPlayers(player), card, player);
            foreach (Player p in targets)
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, Name, "hej", FunctionCard.HandlingMethod.MethodGet);
                if (ai.HasSkill("sheyan", p))
                {
                    if (score.Score > 0)
                    {
                        if (ai.IsFriend(p))
                            score.Score += 8;
                        else
                            score.Score /= 2;
                    }
                    else if (ai.IsFriend(p) && score.Score > -1.5)
                        score.Score = 1;
                }
                if (ai.HasSkill("qianya", p) && score.Score > 0)
                {
                    if (ai.IsFriend(p))
                        score.Score += 1;
                    else
                        score.Score /= 2;
                }
                foreach (string skill in ai.GetKnownSkills(p))
                {
                    SkillEvent ev = Engine.GetSkillEvent(skill);
                    if (ev != null)
                        score.Score += ev.TargetValueAdjust(ai, card, player, new List<Player> { p }, p);
                }
                scores.Add(score);
            }

            bool hand = false;
            foreach (int id in card.SubCards)
            {
                if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == player)
                {
                    hand = true;
                    break;
                }
            }

            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (player.ContainsTag("xianzhen") && player.GetTag("xianzhen") is List<string> names)
                {
                    bool good = false;
                    foreach (ScoreStruct score in scores)
                    {
                        if (names.Contains(score.Players[0].Name) && score.Score > 0)
                        {
                            good = true;
                            break;
                        }
                    }

                    if (good)
                    {
                        foreach (ScoreStruct score in scores)
                        {
                            if (score.Score > 0 && !names.Contains(score.Players[0].Name))
                            {
                                use.Card = card;
                                use.To = score.Players;
                                return;
                            }
                        }
                    }
                }

                if (scores[0].Score > 4 || (scores[0].Score > 0 && hand && ai.GetOverflow(player) > 0))
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                    return;
                }
            }

            if (ai.HasSkill("zishu") && hand && player.Phase == PlayerPhase.Play && ai.GetOverflow(player) > 0)
            {
                foreach (Player p in targets)
                {
                    if (!ai.IsFriend(p))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }

                foreach (Player p in targets)
                {
                    foreach (int id in p.GetEquips())
                    {
                        WrappedCard equip = room.GetCard(id);
                        if (ai.GetSameEquip(card, player) == null)
                        {
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }

                foreach (Player p in targets)
                {
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            NulliResult result = new NulliResult();
            int num = ai.GetKnownCardsNums(Nullification.ClassName, "he", ai.Self);
            if (!from.Alive || !to.Alive || to.IsAllNude() || (keep && num == 1 && ai.Self != to)) return result;

            if (positive)
            {
                if ((RoomLogic.PlayerContainsTrick(room, to, Indulgence.ClassName) || RoomLogic.PlayerContainsTrick(room, to, SupplyShortage.ClassName))
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
                    else if (to.HasTreasure(JadeSeal.ClassName) || to.HasTreasure(WoodenOx.ClassName) || to.HasTreasure(ClassicWoodenOx.ClassName))
                        use = true;
                    else if (to.HasArmor(PeaceSpell.ClassName) && !RoomLogic.PlayerHasSkill(room, to, "wendao"))
                        use = true;
                    else if (ai.IsWeak(to))
                        use = true;
                    else if (ai.GetEnemies(ai.Self).Count == 1)
                        use = true;
                    else if (!ai.IsFriend(from) && ai.Self == to && num > 0 && !to.GetWeapon() && !to.GetDefensiveHorse() && !ai.HasSkill("qianxun_jx", to))
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
                if (ai.IsFriend(from) && ai.Self == room.Current && num > 1 && ai.GetOverflow(ai.Self) > 0)
                {
                    result.Null = true;
                    return result;
                }
            }

            return result;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai is StupidAI && triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && use.Reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                foreach (Player p in use.To)
                {
                    if (ai.HasSkill("qianxun_jx", p) && ai.GetPlayerTendency(p) != "unknown" && p.HandcardNum > 1 && ai.GetFriends(p).Count > 1)
                        ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 70);
                }
            }

            //针对所选择的卡牌判断敌友
            if (ai.Self == player && !(ai is StupidAI)) return;
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceJudge)
                    {
                        if (target.HasFlag("sheyan") && player.GetCards("he").Count == 0) return;
                        if (room.GetCard(card_id).Name != Lightning.ClassName)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            Player winner = ai.GetWizzardRaceWinner(room.GetCard(card_id).Name, target, target);
                            if (winner != null && ai.IsFriend(winner, target))
                                ai.UpdatePlayerRelation(player, target, false);
                            else
                                ai.UpdatePlayerRelation(player, target, true);
                        }
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceHand && !(ai.HasSkill("qianxun_jx", target) && target.HandcardNum > 1) && !target.HasFlag("sheyan"))
                    {
                        ai.UpdatePlayerRelation(player, target, ai.HasSkill("kongcheng|kongcheng_jx") && target.HandcardNum == 1 ? true : false);
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                    {
                        if (target.HasFlag("sheyan") && player.GetCards("h").Count == 0) return;
                        bool friendly = ai.GetKeepValue(card_id, target, Place.PlaceEquip) < 0;

                        if (ai is StupidAI)
                        {
                            if (target != ai.Self || ai.Self.GetRoleEnum() == PlayerRole.Lord)
                            {
                                if (target.GetRoleEnum() == PlayerRole.Lord)
                                {
                                    ai.UpdatePlayerIntention(player, friendly ? "loyalist" : "rebel", friendly ? 80 : 40);
                                }
                                else if (ai.GetPlayerTendency(target) == "loyalist" && player.GetRoleEnum() != PlayerRole.Lord)
                                {
                                    ai.UpdatePlayerIntention(player, friendly ? "loyalist" : "rebel", friendly ? 80 : 40);
                                }
                                else if (ai.GetPlayerTendency(target) == "rebel" && player.GetRoleEnum() != PlayerRole.Lord)
                                {
                                    ai.UpdatePlayerIntention(player, friendly ? "rebel" : "loyalist", friendly ? 80 : 40);
                                }
                            }
                            else if (ai.Self == target && ai.GetPlayerTendency(ai.Self) == "rebel")
                            {
                                ai.UpdatePlayerIntention(player, "loyalist", friendly ? 60 : 30);
                            }
                            else if (ai.Self == target && ai.GetPlayerTendency(ai.Self) == "loyalist")
                            {
                                ai.UpdatePlayerIntention(player, "rebel", friendly ? 60 : 30);
                            }
                        }
                        else
                            ai.UpdatePlayerRelation(player, target, friendly);
                    }
                }
            }
        }
    }

    public class DuelAI : UseCard
    {
        public DuelAI() : base(Duel.ClassName)
        {
            key = new List<string> { "cardResponded" };
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard duel)
        {
            List<Player> enemies = ai.Exclude(ai.GetEnemies(player), duel);
            List<Player> friends = ai.Exclude(ai.FriendNoSelf, duel);
            int n1 = ai.GetKnownCardsNums(Slash.ClassName, "he", player);
            Room room = ai.Room;

            bool benxi = false;
            if (ai.HasSkill("benxi"))
            {
                bool check = true;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    int distance = RoomLogic.DistanceTo(room, player, p, duel, true);
                    if (distance != 1)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                    benxi = true;
            }

            if (ai.HasSkill("wushuang")) n1*= 2;
            Dictionary<Player, double> scores = new Dictionary<Player, double>();
            foreach (Player p in friends)
            {
                bool fuyin = false;
                if (ai.HasSkill("fuyin", p) && p.GetMark("fuyin") == 0)
                {
                    int count = player.HandcardNum;
                    foreach (int id in duel.SubCards)
                        if (room.GetCardPlace(id) == Place.PlaceHand)
                            count--;

                    if (count > p.HandcardNum)
                    {
                        scores[p] = 0;
                        fuyin = true;
                    }
                }
                if (!fuyin)
                {
                    scores[p] = ai.GetDamageScore(new DamageStruct(duel, player, p)).Score;
                    foreach (string skill in ai.GetKnownSkills(p))
                    {
                        SkillEvent skill_e = Engine.GetSkillEvent(skill);
                        if (skill_e != null)
                            scores[p] += skill_e.TargetValueAdjust(ai, duel, player, new List<Player> { p }, p);
                    }
                }
            }
            foreach (Player p in enemies)
            {
                bool fuyin = false;
                if (ai.HasSkill("fuyin", p) && p.GetMark("fuyin") == 0)
                {
                    int count = player.HandcardNum;
                    foreach (int id in duel.SubCards)
                        if (room.GetCardPlace(id) == Place.PlaceHand)
                            count--;

                    if (count > p.HandcardNum)
                    {
                        scores[p] = 1;
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

                    if (!fuqi && !benxi && !ai.IsLackCard(p, Slash.ClassName))
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

                    if (benxi || fuqi)
                        scores[p] = ai.GetDamageScore(new DamageStruct(duel, player, p)).Score;
                    else if (n2 > n1)
                        scores[p] = ai.GetDamageScore(new DamageStruct(duel, p, player)).Score;
                    else
                        scores[p] = ai.GetDamageScore(new DamageStruct(duel, player, p)).Score - (n2 - 1) * 0.4;

                    foreach (string skill in ai.GetKnownSkills(p))
                    {
                        SkillEvent skill_e = Engine.GetSkillEvent(skill);
                        if (skill_e != null)
                            scores[p] += skill_e.TargetValueAdjust(ai, duel, player, new List<Player> { p }, p);
                    }
                }
            }
            List<Player> targets = new List<Player>(friends);
            targets.AddRange(enemies);
            targets.Sort((x, y) => { return scores[x] > scores[y] ? -1 : 1; });
            bool hand = false;
            foreach (int id in duel.SubCards)
            {
                if (ai.Room.GetCardPlace(id) == Place.PlaceHand && ai.Room.GetCardOwner(id) == player)
                    hand = true;
            }
            if (targets.Count > 0)
            {
                int targets_num = 1 + Engine.CorrectCardTarget(ai.Room, TargetModSkill.ModType.ExtraMaxTarget, player, duel);
                if (player.ContainsTag("xianzhen") && player.GetTag("xianzhen") is List<string> names)
                {
                    bool good = false;
                    foreach (Player p in targets)
                    {
                        if (names.Contains(p.Name) && scores[p] > 0)
                        {
                            good = true;
                            break;
                        }
                    }

                    if (good)
                    {
                        foreach (Player p in targets)
                        {
                            if (scores[p] > 0 && !names.Contains(p.Name))
                            {
                                use.Card = duel;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }

                for (int i = 0; i < targets_num; i++)
                    if (scores[targets[i]] > 4 || (scores[targets[i]] > 0 && hand && ai.GetOverflow(player) > 0))
                        use.To.Add(targets[i]);

                if (use.To.Count > 0)
                {
                    use.Card = duel;
                    return;
                }
            }

            if (ai.HasSkill("jizhi|jizhi_jx") && !duel.IsVirtualCard() || ai.HasSkill("jiang"))
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
            string[] strs = prompt.Split(':');
            Room room = ai.Room;
            Player target = room.FindPlayer(strs[1]);

            int damage_count = 1 + effect.ExDamage;
            if (effect.From == target)
                damage_count += effect.BasicEffect.Effect1;

            DamageStruct damage = new DamageStruct(effect.Card, target, player, damage_count);
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player> { target }
            };

            if (!ai.CardAskNullifilter(damage) || player.GetMark("@tangerine") > 0)
            {
                if (ai.IsFriend(target))
                {
                    DamageStruct _damage = new DamageStruct(effect.Card, player, target);
                    if (ai.GetDamageScore(_damage).Score < ai.GetDamageScore(damage).Score + 2)
                        return use;
                }

                bool shuangxiong = false;
                if (effect.Card.Skill == "shuangxiong_jx")
                {
                    if (player != effect.From)
                    {
                        shuangxiong = true;
                    }
                    else if (player.Hp > 1 && damage.Damage == 1 && player.ContainsTag(RoomLogic.CardToString(room, effect.Card))
                        && player.GetTag(RoomLogic.CardToString(room, effect.Card)) is List<int> ids && ids.Count > 0)
                    {
                        int num = 0;
                        foreach (int id in ids)
                        {
                            if (room.GetCardPlace(id) == Player.Place.DiscardPile)
                                num++;
                        }

                        if (num >= 2) return use;                                               //若决斗为界双雄，且对方已经打出至少2张牌，则选择受伤获得这些牌
                    }
                }

                List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player);
                List<WrappedCard> delete = new List<WrappedCard>(slashes);
                foreach (WrappedCard slash in delete)
                {
                    foreach (int id in slash.SubCards)
                    {
                        if (ai.IsCard(id, Peach.ClassName, player) || (ai.IsCard(id, Analeptic.ClassName, player) && ai.IsWeak()))
                        {
                            slashes.Remove(slash);
                            break;
                        }
                    }
                }

                int count = int.Parse(strs[3]);
                if (slashes.Count >= count)
                {
                    if (shuangxiong)
                    {
                        bool red = IsRed(effect.Card.Suit);                                     //若决斗为界双雄，在体力健康时不应用与决斗颜色相同的杀进行反抗
                        foreach (WrappedCard slash in slashes)
                        {
                            if (slash.SubCards.Count == 0 || (slash.SubCards.Count == 1 && IsRed(slash.Suit) != red))
                            {
                                use.Card = slash;
                                break;
                            }
                        }

                        if (use.Card == null && (player.Hp == 1 || effect.From.Hp == 1))       //若没有合适的杀且自身或对方1血时，则坚决反抗              
                            use.Card = slashes[0];
                    }
                    else
                        use.Card = slashes[0];
                }
            }

            return use;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && use.Reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                if (ai is SmartAI && ai.Self != player)
                {
                    foreach (Player p in use.To)
                        if (!ai.IsCancelTarget(use.Card, p, player) && ai.IsCardEffect(use.Card, p, player) 
                            && (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p)))
                            ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
                }
                else if (ai is StupidAI _ai)
                {
                    foreach (Player p in use.To)
                    {
                        if (ai.GetPlayerTendency(p) != "unknown" && !ai.IsCancelTarget(use.Card, p, player) && ai.IsCardEffect(use.Card, p, player))
                        {
                            DamageStruct damage = new DamageStruct(use.Card, player, p, 1 + use.ExDamage);
                            if (ai.HasSkill("zhiman_jx", player))
                            {
                                bool good = p.JudgingArea.Count > 0;
                                if (!good)
                                {
                                    foreach (int id in p.GetEquips())
                                    {
                                        if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
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

                                    continue;
                                }
                            }

                            if (_ai.NeedDamage(damage))
                            {
                                ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 60);
                                continue;
                            }

                            ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
                        }
                    }
                }
            }

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split('%'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_" && strs[3].StartsWith("duel-slash"))
                    {
                        string prompt = strs[3];
                        if (strs[strs.Count - 2] == "1")
                        {
                            List<CardUseStruct> uses = ai.Room.GetUseList();
                            use = uses[uses.Count - 1];
                            if (use.Card != null && use.Card.Name == Name)
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, player, 1 + use.ExDamage);
                                ScoreStruct score = ai.GetDamageScore(damage);
                                bool lack = true;
                                if (ai.IsFriend(use.From, player) && score.Score > 0)
                                    lack = false;
                                else if (ai.IsEnemy(player) && score.Score < 0)
                                    lack = false;

                                if (lack) ai.SetCardLack(player, Slash.ClassName);
                            }
                        }
                    }
                }
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;

            DamageStruct damage = new DamageStruct(trick, from, to, 1 + effect.ExDamage + effect.BasicEffect.Effect1);
            ScoreStruct score = ai.GetDamageScore(damage);
            if (ai is SmartAI)
            {
                if (positive)
                {
                    if (ai.IsFriend(to) && score.Score < -3)
                    {
                        if (ai.IsEnemy(from) && ai.IsFriend(to))
                        {
                            int wushuang_from = RoomLogic.PlayerHasShownSkill(ai.Room, from, "wushuang") ? 2 : 1;
                            int wushuang_to = RoomLogic.PlayerHasShownSkill(ai.Room, to, "wushuang") ? 2 : 1;
                            if (ai.GetKnownCardsNums(Slash.ClassName, "he", to, ai.Self) * wushuang_to > ai.GetKnownCardsNums(Slash.ClassName, "he", from, ai.Self) * wushuang_from)
                                return result;
                        }

                        if (keep && score.Score > -6)
                            return result;

                        result.Null = true;
                    }
                }
                else if (ai.IsEnemy(to) && ai.IsFriend(damage.From) && score.Score > 5 && !keep)
                {
                    result.Null = true;
                }
            }
            else
            {
                if (positive)
                {
                    if (score.Score < -3)
                    {
                        int wushuang_from = RoomLogic.PlayerHasShownSkill(ai.Room, from, "wushuang") ? 2 : 1;
                        int wushuang_to = RoomLogic.PlayerHasShownSkill(ai.Room, to, "wushuang") ? 2 : 1;
                        if (ai.GetKnownCardsNums(Slash.ClassName, "he", to, ai.Self) * wushuang_to > ai.GetKnownCardsNums(Slash.ClassName, "he", from, ai.Self) * wushuang_from)
                            return result;

                        if (keep && score.Score > -4)
                            return result;

                        result.Null = true;
                    }
                }
                else if (ai.IsEnemy(to) && score.Score > 5 && !keep)
                {
                    result.Null = true;
                }
            }

            return result;
        }
    }

    public class ExNihiloAI : UseCard
    {
        public ExNihiloAI() : base(ExNihilo.ClassName)
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (keep)
                return result;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            bool last = false;
            WrappedCard nul = ai.GetCards(Nullification.ClassName, ai.Self, true)[0];
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
        public AwaitExhaustedAI() : base(AwaitExhausted.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (card.Skill == "duoshi")
                return 2 - Engine.GetCardPriority(ai.Room.GetCard(card.GetEffectiveId()).Name);

            return 0;
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
            if (!card.IsVirtualCard() || card.SubCards.Count == 0)
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

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            if (keep)
                return result;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            bool last = false;
            WrappedCard nul = ai.GetCards(Nullification.ClassName, ai.Self, true)[0];
            if (ai.Self.IsLastHandCard(nul) && ai.HasLoseHandcardEffective(ai.Self))
                last = true;

            if (positive)
            {
                if (ai.IsEnemy(to) && ai.PlayersLevel[to] >= 3 && last)
                {
                    result.Null = true;

                    List<Player> targets = new List<Player>(effect.StackPlayers);
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
        public BefriendAttackingAI() : base(BefriendAttacking.ClassName)
        {
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            NulliResult result = new NulliResult();
            if (keep)
                return result;

            bool last = false;
            WrappedCard nul = ai.GetCards(Nullification.ClassName, ai.Self, true)[0];
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
                if ((!ai.HasSkill("luanji", p) || p.HandcardNum <= 3) && p.GetRoleEnum() == Player.PlayerRole.Careerist)
                {
                    use.To.Add(p);
                    use.Card = card;
                    return;
                }
            }
            
            //计算人数最少的势力
            List<string> kingdoms = new List<string> { "wei", "shu", "wu", "qun" };
            if (player.GetRoleEnum() != Player.PlayerRole.Careerist)
                kingdoms.Remove(player.Kingdom);
            Dictionary<string, int> kingdom_count = new Dictionary<string, int>();
            foreach (string kingdom in kingdoms)
            {
                int count = RoomLogic.GetPlayerNumWithSameKingdom(room, player, kingdom);
                if (count > 0)
                    kingdom_count[kingdom] = count;
            }
            kingdoms = new List<string>(kingdom_count.Keys);

            if (kingdoms.Count > 0)
            {
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
            else if (players.Count == 1 && room.GetOtherPlayers(player).Count == 1)
            {
                use.To = players;
                use.Card = card;
            }
        }
    }

    public class FireAttackAI : UseCard
    {
        private readonly Dictionary<string, string> convert = new Dictionary<string, string> { { ".S", "spade" }, { ".D", "diamond" }, { ".H", "heart" }, { ".C", "club" } };
        public FireAttackAI() : base(FireAttack.ClassName)
        {
            key = new List<string> { "cardShow", "cardResponded" };
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            NulliResult result = new NulliResult();
            if (keep || !from.Alive || to.IsKongcheng() || from.IsKongcheng() || ai.Self == from
                || from.HasFlag("FireAttackFailedPlayer_" + to.Name) || ai.IsFriend(from)) return result;

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
                    if (from.HandcardNum > 2 || ai.IsWeak(to) || ai.HasArmorEffect(to, Vine.ClassName))
                    {
                        result.Null = true;
                        return result;
                    }
                }
            }

            return result;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (card.Skill == "huoji" || card.Skill == "huoji_sm")
            {
                Room room = ai.Room;
                WrappedCard c = room.GetCard(card.GetEffectiveId());
                double prio = Engine.GetCardPriority(card.Name) - Engine.GetCardPriority(c.Name);
                return prio < 0 ? prio : 0;
            }

            return 0;
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
            FunctionCard fire = Engine.GetFunctionCard(card.Name);

            foreach (Player p in room.GetAlivePlayers())
            {
                if (!fire.TargetFilter(room, new List<Player>(), p, player, card)) continue;
                double adjust = 0;
                if (ai.HasSkill("jizhi|jizhi_jx", player) && !card.IsVirtualCard())
                    adjust += 3.5;

                if (!ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                {
                    DamageStruct damage = new DamageStruct(card, player, p, 1, DamageStruct.DamageNature.Fire);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    score.Players = new List<Player> { p };
                    if (ai.IsFriend(p)) score.Score -= 2.5;

                    if (score.DoDamage && score.Score > 0 && p.Chained)
                        score.Score += ai.ChainDamage(damage);

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
                        bool fail = false;
                        foreach (int id in ids)
                        {
                            WrappedCard c = room.GetCard(id);
                            if (player.HasFlag("FireAttackFailed_" + GetSuitString(c.Suit)) && lack[c.Suit] == true)
                            {
                                fail = true;
                                break;
                            }
                        }
                        if (fail)
                            rate = 0;
                        else
                        {
                            if (ids.Count == p.HandcardNum)
                            {
                                List<CardSuit> _lack = new List<CardSuit>(), suits = new List<CardSuit>();
                                foreach (int id in ids)
                                {
                                    WrappedCard c = room.GetCard(id);
                                    if (!suits.Contains(c.Suit))
                                        suits.Add(c.Suit);

                                    if (lack[c.Suit] == true && !_lack.Contains(c.Suit))
                                        _lack.Add(c.Suit);
                                }
                                rate = 1 - (double)_lack.Count / suits.Count;
                            }
                            else
                                rate = canDis.Count / 4;
                        }
                    }

                    score.Score *= rate;
                    if (player == p)
                        score.Score -= 2;
                    if (ai.IsFriend(p) && score.Score > 0)              //尽量不要火攻自己人
                        score.Score /= 3;

                    if (adjust > 0 && score.Score < 0)
                        score.Score = 0;
                    score.Score += adjust;

                    foreach (string skill in ai.GetKnownSkills(p))
                    {
                        SkillEvent skill_e = Engine.GetSkillEvent(skill);
                        if (skill_e != null)
                        {
                            double points = skill_e.TargetValueAdjust(ai, card, player, new List<Player> { p }, p);
                            if (points > 0 && score.Score < 0 && ai.IsFriend(p))
                                score.Score = 0;
                            score.Score += points;
                        }
                    }

                    scores.Add(score);
                }
                else
                {
                    ScoreStruct score = new ScoreStruct
                    {
                        Players = new List<Player> { p },
                        Score = adjust
                    };
                    scores.Add(score);
                }
            }

            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                bool hand = false;
                foreach (int id in card.SubCards)
                {
                    if (ai.Room.GetCardPlace(id) == Place.PlaceHand && ai.Room.GetCardOwner(id) == player)
                    {
                        hand = true;
                        break;
                    }
                }

                if (scores[0].Score > 4)
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                }
                else if (hand && ai.GetOverflow(player) > 0)
                {
                    foreach (ScoreStruct score in scores)
                    {
                        Player target = score.Players[0];
                        if (score.Score > 0 && !player.HasFlag("FireAttackFailedPlayer_" + target.Name))
                        {
                            use.Card = card;
                            use.To = score.Players;
                            return;
                        }
                    }

                    foreach (ScoreStruct score in scores)
                    {
                        Player target = score.Players[0];
                        if (!player.HasFlag("FireAttackFailedPlayer_" + target.Name) && !ai.IsFriend(target))
                        {
                            use.Card = card;
                            use.To = score.Players;
                            return;
                        }
                    }
                }
            }
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && use.Reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                if (ai is SmartAI && ai.Self != player)
                {
                    foreach (Player p in use.To)
                        if (player != p && !ai.IsCancelTarget(use.Card, p, player) && ai.IsCardEffect(use.Card, p, player) &&
                            (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p)))
                            ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
                }
                else if (ai is StupidAI _ai)
                {
                    foreach (Player p in use.To)
                    {
                        if (player != p && ai.GetPlayerTendency(p) != "unknown" && !ai.IsCancelTarget(use.Card, p, player) && ai.IsCardEffect(use.Card, p, player))
                        {
                            DamageStruct damage = new DamageStruct(use.Card, player, p, 1, DamageStruct.DamageNature.Fire);
                            if (_ai.NeedDamage(damage))
                                continue;

                            double value = 0;
                            foreach (string skill in ai.GetKnownSkills(p))
                            {
                                SkillEvent skill_e = Engine.GetSkillEvent(skill);
                                if (skill_e != null)
                                    value += skill_e.TargetValueAdjust(ai, use.Card, player, use.To, p);
                            }
                            if (value > 1.5)
                            {
                                ai.UpdatePlayerIntention(player, ai.GetPlayerTendency(p), 60);
                                return;
                            }

                            ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
                        }
                    }
                }
            }

            if (ai.Self == player) return;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                if (str.StartsWith("cardResponded"))
                {
                    List<string> strs = new List<string>(str.Split('%'));
                    if (strs[1] == Name && strs[strs.Count - 1] == "_nil_")
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
                }
                else if (str.StartsWith("cardShow"))
                {
                    List<string> strs = new List<string>(str.Split(':'));
                    if (strs[1] == Name)
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
                List<int> ids= player.GetCards("h");
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
            List<int> ids= player.GetCards("h");
            ai.SortByUseValue(ref ids, false);

            CardEffectStruct effect = (CardEffectStruct)data;
            Player target = effect.To;
            WrappedCard fire = effect.Card;

            DamageStruct damage = new DamageStruct(fire, player, target, 1, DamageStruct.DamageNature.Fire);
            ScoreStruct score = ai.GetDamageScore(damage);
            if (score.DoDamage)
            {
                score.Score += ai.ChainDamage(damage);
            }
            if (score.Score > 0)
            {
                foreach (int id in ids)
                {
                    if (WrappedCard.GetSuitString(room.GetCard(id).Suit) == convert[pattern])
                    {
                        double value = score.Score;
                        if (ai.IsCard(id, Peach.ClassName, player))
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
        public KnownBothAI() : base(KnownBoth.ClassName)
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
                            foreach (int id in target.GetCards("h"))
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

            if (can_recast && (!ai.HasSkill("jizhi|jizhi_jx", player) || (card.IsVirtualCard() && card.SubCards.Count != 1)))
            {
                use.Card = card;
                return;
            }

            foreach (Player p in players)
            {
                if (ai.IsEnemy(p) && (p.HandcardNum > 3 || !p.HasShownAllGenerals()))
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
        public IronChainAI() : base(IronChain.ClassName)
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            Room room = ai.Room;
            if (card.Skill == "lianhuan" || card.Skill == "lianhuan_sm")
            {
                WrappedCard c = room.GetCard(card.GetEffectiveId());
                double prio = Engine.GetCardPriority(card.Name) - Engine.GetCardPriority(c.Name);
                return prio < 0 ? prio : 0;
            }

            return 0;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player && !(ai is StupidAI)) return;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is SmartAI)
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
                else if (ai is StupidAI)
                {

                    foreach (Player p in use.To)
                    {
                        if (p == player) continue;
                        if (ai.GetPlayerTendency(p) != "unknown")
                        {
                            if (!p.Chained)
                            {
                                double value = 0;
                                foreach (string skill in ai.GetKnownSkills(p))
                                {
                                    SkillEvent skill_e = Engine.GetSkillEvent(skill);
                                    if (skill_e != null)
                                        value += skill_e.TargetValueAdjust(ai, use.Card, player, use.To, p);
                                }
                                if (value > 1)
                                    ai.UpdatePlayerRelation(player, p, true);
                                else
                                    ai.UpdatePlayerRelation(player, p, false);
                            }
                            else
                                ai.UpdatePlayerRelation(player, p, true);
                        }
                    }
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> enemis = ai.GetEnemies(player), friends = ai.GetFriends(player);
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            bool canRecast = fcard.CanRecast(room, player, card);
            if (canRecast) use.Card = card;

            Player luxun = ai.FindPlayerBySkill("qianxun_jx+lianying");
            if (ai is StupidAI _ai && luxun != null && luxun != player && luxun.HandcardNum > 1
                && fcard.TargetFilter(room, new List<Player>(), luxun, player, card) && !ai.IsCancelTarget(card, luxun, player) && ai.IsCardEffect(card, luxun, player))
            {
                SkillEvent e = Engine.GetSkillEvent("qianxun_jx");
                if (e != null)
                {
                    double value = e.TargetValueAdjust(ai, card, player, new List<Player> { luxun }, luxun);
                    if (value > 1)
                    {
                        use.Card = card;
                        use.To = new List<Player> { luxun };
                        return;
                    }
                }
            }

            List<Player> friendtargets = new List<Player>(), friendtargets2 = new List<Player>();
            List<Player> enemytargets = new List<Player>();
            ai.SortByDefense(ref friends, false);
            foreach (Player p in friends)
            {
                if (ai.HasSkill("jieying", p)) continue;
                if (fcard.TargetFilter(room, new List<Player>(), p, player, card) && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                {
                    DamageStruct damage_f = new DamageStruct(string.Empty, null, p, 1, DamageStruct.DamageNature.Fire);
                    DamageStruct damage_t = new DamageStruct(string.Empty, null, p, 1, DamageStruct.DamageNature.Thunder);
                    double damage_value = Math.Min(ai.GetDamageScore(damage_f).Score, ai.GetDamageScore(damage_t).Score);
                    double max_value_f = ai.ChainDamage(damage_f);
                    double max_value_t = ai.ChainDamage(damage_t);
                    damage_value += Math.Max(max_value_f, max_value_t);
                    if (p.Chained && damage_value < 0)
                    {
                        if (RoomLogic.PlayerContainsTrick(room, p, Lightning.ClassName))
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
                    if (!p.Chained && damage_value > 0 && RoomLogic.CanBeChainedBy(room, p, player))
                    {
                        enemytargets.Add(p);
                    }
                }
            }
            int targets_num = 2 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, player, card);
            friendtargets.AddRange(friendtargets2);
            friendtargets.AddRange(enemytargets);
            for (int i = 0; i < Math.Min(targets_num, friendtargets.Count); i++)
                use.To.Add(friendtargets[i]);

            if (use.To.Count < targets_num && !player.Chained && RoomLogic.CanBeChainedBy(room, player, player)
                && fcard.TargetFilter(room, new List<Player>(), player, player, card) && !ai.IsCancelTarget(card, player, player) && ai.IsCardEffect(card, player, player))
            {
                List<WrappedCard> fires = ai.GetCards(FireAttack.ClassName, player, true);
                if (fires.Count > 0)
                {
                    WrappedCard fire = fires[0];
                    if (!player.IsLastHandCard(fire) && player.HandcardNum > 2 && !ai.IsWeak(player))
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

            double use_value = use.To.Count > 0 && ai.HasSkill("jizhi|jizhi_jx") && !card.IsVirtualCard() ? 2 : 0;
            foreach (Player p in use.To)
            {
                foreach (string skill in ai.GetKnownSkills(p))
                {
                    SkillEvent ev = Engine.GetSkillEvent(skill);
                    if (ev != null)
                        use_value += ev.TargetValueAdjust(ai, card, player, use.To, p);
                }
            }
            if (use_value < 0) return;

            if (use.To.Count > 0 && use.Card == null) use.Card = card;
            if (use.To.Count == 1)
            {
                if (ai.HasSkill("zishu") && player.Phase == PlayerPhase.Play && canRecast)
                {
                    use.To.Clear();
                    return;
                }

                if (use_value > 0) return;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.Chained) return;

                if (canRecast)
                {
                    use.To = new List<Player>();
                    return;
                }

                else if(ai.GetOverflow(player) > 0)
                {
                    foreach (int id in card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == player)
                            return;
                    }
                }

                use.Card = null;
                use.To = new List<Player>();
            }
        }
        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            if (keep) return result;
            Room room = ai.Room;

            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
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
                        if (ai.HasArmorEffect(p, Vine.ClassName) && (p.Chained || targets.Contains(p)))
                        {
                            invoke = true;
                        }
                        if (RoomLogic.PlayerContainsTrick(room, p, Lightning.ClassName))
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

                    if (invoke && ai.GetKnownCardsNums(HegNullification.ClassName, "he", ai.Self, ai.Self) > 0)
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
                    WrappedCard nul = ai.GetCards(Nullification.ClassName, ai.Self, true)[0];
                    if (ai.Self.IsLastHandCard(nul) && ai.HasLoseHandcardEffective(ai.Self))
                        result.Null = true;
                }
            }

            return result;
        }
    }

    public class SavageAssaultAI : UseCard
    {
        public SavageAssaultAI() : base(SavageAssault.ClassName)
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
            if (!ai.CardAskNullifilter(damage) || player.GetMark("@tangerine") > 0)
            {
                List<WrappedCard> slashes = ai.GetCards(Slash.ClassName, player);
                List<WrappedCard> delete = new List<WrappedCard>(slashes);
                foreach (WrappedCard slash in delete)
                {
                    foreach (int id in slash.SubCards)
                    {
                        if ((ai.IsCard(id, Peach.ClassName, player) || (ai.IsCard(id, Analeptic.ClassName, player) && ai.IsWeak())) && slash.SubCards.Count > 1)
                        {
                            slashes.Remove(slash);
                            break;
                        }
                    }
                }

                if (slashes.Count > 0)
                {
                    foreach (WrappedCard card in slashes)
                    {
                        if (!RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodResponse))
                        {
                            use.Card = card;
                            return use;
                        }
                    }
                }
            }

            return use;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split('%'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_" && strs[3].StartsWith("savage-assault-slash"))
                    {
                        List<CardUseStruct> uses = ai.Room.GetUseList();
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

                            if (lack) ai.SetCardLack(player, Slash.ClassName);
                        }
                    }
                }
            }
        }

        private bool HasSlash(TrustedAI ai, Player who)
        {
            if (!ai.IsLackCard(who, Slash.ClassName))
                return false;
            else
            {
                foreach (WrappedCard card in ai.GetCards(Slash.ClassName, who))
                    if (!RoomLogic.IsCardLimited(ai.Room, who, card, FunctionCard.HandlingMethod.MethodResponse))
                        return true;

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

                double count = (RoomLogic.IsHandCardLimited(ai.Room, who, FunctionCard.HandlingMethod.MethodResponse) ? 0 : (who.HandcardNum - ai.GetKnownCards(who).Count)) / (rate - fix)
                    + (who.GetHandPile().Count - ai.GetKnownHandPileCards(who).Count) / (4 - fix);
                return count >= 1 ? true : false;

            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;
            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            DamageStruct damage = new DamageStruct(trick, from, to);
            ScoreStruct score = ai.GetDamageScore(damage);

            if (positive)
            {
                if (score.Score < 0 && !ai.IsEnemy(to))
                {
                    bool nulli = !HasSlash(ai, to);
                    double value = score.Score;
                    if (room.NullTimes == 0 && ai.GetKnownCardsNums(HegNullification.ClassName, "he", player) > 0 || room.HegNull)
                    {
                        foreach (Player p in targets)
                        {
                            if (p != to && RoomLogic.IsFriendWith(room, to, p) && ai.IsCardEffect(trick, p, from))
                            {
                                DamageStruct _damage = new DamageStruct(trick, from, p);
                                double _value = ai.GetDamageScore(_damage).Score;
                                if (_value < 0 && !HasSlash(ai, p))
                                {
                                    value += _value;
                                    result.Heg = room.NullTimes == 0;
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
                if (ai.IsEnemy(to))
                {
                    if (score.Score > 4 && !keep || score.Score > 8)
                        result.Null = true;
                }
            }

            return result;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            //foreach (Player p in room.GetOtherPlayers(player))
            //    if (ai.HasSkill("zhennan", p) && !ai.IsFriend(p)) return;

            if (ai.GetAoeValue(card))
                use.Card = card;
        }
    }

    public class ArcheryAttackAI : UseCard
    {
        public ArcheryAttackAI() : base(ArcheryAttack.ClassName)
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
            if (!ai.CardAskNullifilter(damage) || player.GetMark("@tangerine") > 0)
            {
                List<WrappedCard> jinks = ai.GetCards(Jink.ClassName, player);
                if (jinks.Count > 0)
                {
                    foreach (WrappedCard card in jinks)
                    {
                        if (!RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodResponse))
                        {
                            use.Card = card;
                            return use;
                        }
                    }
                }
            }

            return use;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> strs = new List<string>(str.Split('%'));
                if (strs[1] == Name)
                {
                    if (strs[strs.Count - 1] == "_nil_" && strs[3].StartsWith("archery-attack-jink"))
                    {
                        List<CardUseStruct> uses = ai.Room.GetUseList();
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

                            if (lack) ai.SetCardLack(player, Jink.ClassName);
                        }
                    }
                }
            }
        }

        private bool HasJink(TrustedAI ai, Player who)
        {
            bool no_red = who.GetMark("@qianxi_red") > 0;
            bool no_black = who.GetMark("@qianxi_black") > 0;
            if (ai.HasArmorEffect(who, EightDiagram.ClassName) && ai.HasSkill("qingguo+tiandu") && !no_black)
                return true;

            double basic = 0;
            if (ai.HasArmorEffect(who, EightDiagram.ClassName))
                basic = 0.35;

            if (!ai.IsLackCard(who, Jink.ClassName))
                return false;
            else
            {
                foreach (WrappedCard card in ai.GetCards(Jink.ClassName, who))
                    if (!RoomLogic.IsCardLimited(ai.Room, who, card, FunctionCard.HandlingMethod.MethodResponse))
                        return true;

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

                double count = (RoomLogic.IsHandCardLimited(ai.Room, who, FunctionCard.HandlingMethod.MethodResponse) ? 0 : (who.HandcardNum - ai.GetKnownCards(who).Count)) / (rate - fix)
                    + (who.GetHandPile().Count - ai.GetKnownHandPileCards(who).Count) / (5 - fix) + basic;
                return count >= 1 ? true : false;

            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Player player = ai.Self;
            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            DamageStruct damage = new DamageStruct(trick, from, to);
            ScoreStruct score = ai.GetDamageScore(damage);

            if (positive)
            {
                if (score.Score < 0 && !ai.IsEnemy(to))
                {
                    bool nulli = !HasJink(ai, to);
                    double value = score.Score;
                    if (room.NullTimes == 0 && ai.GetKnownCardsNums(HegNullification.ClassName, "he", player) > 0 || room.HegNull)
                    {
                        foreach (Player p in targets)
                        {
                            if (p != to && RoomLogic.IsFriendWith(room, to, p) && ai.IsCardEffect(trick, p, from))
                            {
                                DamageStruct _damage = new DamageStruct(trick, from, p);
                                double _value = ai.GetDamageScore(_damage).Score;
                                if (_value < 0 && !HasJink(ai, p))
                                {
                                    value += _value;
                                    result.Heg = room.NullTimes == 0;
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
                if (ai.IsEnemy(to))
                {
                    if (score.Score > 4 && !keep || score.Score > 8)
                        result.Null = true;
                }
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
        public IndulgenceAI() : base(Indulgence.ClassName)
        { }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Room room = ai.Room;

            if (room.DrawPile.Count > 0)
            {
                int id = room.DrawPile[0];
                RoomCard card = room.GetCard(id);
                if ((card.HasFlag("visible") || card.HasFlag("visible2" + ai.Self.Name))
                    && (card.Suit == CardSuit.Heart || (ai.HasSkill("hongyan", to) && card.Suit == CardSuit.Spade)))
                    return result;
            }

            if (positive)
            {
                if (ai.IsFriend(to) && !ai.IsGuanxingEffected(to, false, trick))
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
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is SmartAI && ai.Self != player)
                {
                    foreach (Player p in use.To)
                        if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                            ai.UpdatePlayerRelation(player, p, false);          //若使用者和目标中的一方身份已判明，则更新双方关系为敌对
                }
                else if (ai is StupidAI)
                {
                    foreach (Player p in use.To)
                        if (ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, false);          //若使用者和目标中的一方身份已判明，则更新双方关系为敌对
                }
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
            Player caoren = ai.FindPlayerBySkill("jiewei");
            int caoren_seat = (caoren != null && !caoren.FaceUp && !caoren.IsKongcheng() && !ai.IsFriend(caoren)) ? caoren.Seat : 0;
            if (caoren != null && caoren_seat > 0 && (zhanghe == null || room.GetFront(caoren, zhanghe) == caoren))
            {
                zhanghe = caoren;
                zhanghe_seat = caoren_seat;
            }

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
            if (ai.HasSkill("qiaobian") && (!RoomLogic.PlayerContainsTrick(room, enemy, SupplyShortage.ClassName) || !enemy.IsKongcheng()))
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
            if (ai.HasSkill("lijian|fanjian|dimeng|jijiu|jieyin|zhiheng|rende|jieyin_jx|zhiheng_jx", enemy)) value += 10;
            if (ai.HasSkill("qixi|guose|duanliang|luoshen|jizhi|wansha|guose_jx")) value += 5;
            if (ai.HasSkill("guzheng|duoshi")) value += 3;
            if (ai.IsWeak(enemy)) value += 3;
            if (ai.PlayersLevel[enemy] < 3) value -= 10;
            if (ai.HasSkill("keji|shensu|keji_jx", enemy)) value -= enemy.HandcardNum;
            if (ai.HasSkill("lirang|guanxing|yizhi", enemy)) value -= 5;
            if (ai.HasSkill("tiandu", enemy)) value -= 2;
            if (enemy.GetPile("tuifeng").Count > 0) value += enemy.GetPile("tuifeng").Count * 2;

            //if not sgs.isGoodTarget(enemy, self.enemies, self) then value = value - 1 end

            if (ai.GetKnownCardsNums(Dismantlement.ClassName, "he", enemy, ai.Self) > 0) value += 2;
            value += (room.AliveCount() - ai.PlayerGetRound(enemy)) / 2;

            foreach (string skill in ai.GetKnownSkills(enemy))
            {
                SkillEvent ev = Engine.GetSkillEvent(skill);
                if (ev != null)
                    value += ev.TargetValueAdjust(ai, card, null, new List<Player> { enemy }, enemy);
            }

            return value;
        }
    }

    public class SupplyShortageAI : UseCard
    {
        public SupplyShortageAI() : base(SupplyShortage.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            double value = 0;
            Room room = ai.Room;
            if (card.Skill == "duanliang" || card.Skill == "duanliang_jx")
            {
                WrappedCard c = room.GetCard(card.GetEffectiveId());
                double prio = Engine.GetCardPriority(card.Name) - Engine.GetCardPriority(c.Name);
                value += prio < 0 ? prio : 0;
            }

            if (RoomLogic.DistanceTo(room, player, targets[0], card) > 1 && ai.HasSkill("duanliang", player))
                value -= 2;

            return value;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is SmartAI && ai.Self != player)
                {
                    foreach (Player p in use.To)
                        if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                            ai.UpdatePlayerRelation(player, p, false);          //若使用者和目标中的一方身份已判明，则更新双方关系为敌对
                }
                else if (ai is StupidAI)
                {
                    foreach (Player p in use.To)
                        if (ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, false);          //若使用者和目标中的一方身份已判明，则更新双方关系为敌对
                }
            }
        }
        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Room room = ai.Room;
            if (room.DrawPile.Count > 0)
            {
                int id = room.DrawPile[0];
                RoomCard card = room.GetCard(id);
                if ((card.HasFlag("visible") || card.HasFlag("visible2" + ai.Self.Name)) && card.Suit == CardSuit.Club)
                    return result;
            }

            if (positive)
            {
                if (ai.IsFriend(to) && !ai.IsGuanxingEffected(to, false, trick))
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
            if (ai.HasSkill("qiaobian") && (!RoomLogic.PlayerContainsTrick(room, enemy, Indulgence.ClassName) || !enemy.IsKongcheng()))
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

            if (ai.HasSkill("haoshi|tuxi|tuxi_jx|lijian|fanjian|dimeng|jijiu|jieyin|beige", enemy)
                || (ai.HasSkill("zaiqi", enemy) && enemy.GetLostHp() > 1))
                value += value + 10;

            if (ai.HasSkill(TrustedAI.CardneedSkill + "|tianxiang"))
                value += 5;

            if (ai.HasSkill("yingzi_zhouyu|yingzi_sunce|duoshi", enemy)) value += 1;
            if (enemy.GetMark("@tangerine") > 0) value += 1;
            if (ai.HasSkill("zhenglun", enemy) && enemy.GetMark("@tangerine") == 0) value += 1.5;
            if (ai.IsWeak(enemy)) value += 3;
            if (ai.PlayersLevel[enemy] < 3) value -= 10;
            if (ai.WillSkipPlayPhase(enemy)) value -= 10;
            if (ai.HasSkill("shensu", enemy)) value -= 1;
            if (ai.HasSkill("guanxing|yizhi|guanxing_jx", enemy)) value -= 5;
            if (ai.HasSkill("tiandu", enemy)) value -= 5;
            if (ai.HasSkill("guidao", enemy)) value -= 3;
            if (ai.HasSkill("tiandu", enemy)) value -= 3;
            if (ai.NeedKongcheng(enemy)) value -= 1;

            foreach (string skill in ai.GetKnownSkills(enemy))
            {
                SkillEvent ev = Engine.GetSkillEvent(skill);
                if (ev != null)
                    value += ev.TargetValueAdjust(ai, card, null, new List<Player> { enemy }, enemy);
            }

            return value;
        }
    }

    public class LightningAI : UseCard
    {
        public LightningAI() : base(Lightning.ClassName)
        {
        }
        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            DamageStruct damage = new DamageStruct(trick, null, to, 3, DamageStruct.DamageNature.Thunder);
            double value = ai.GetDamageScore(damage).Score;
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            if (room.DrawPile.Count > 0)
            {
                int id = room.DrawPile[0];
                RoomCard card = room.GetCard(id);
                if (card.HasFlag("visible") || card.HasFlag("visible2" + ai.Self.Name))
                {
                    if (card.Suit != CardSuit.Club || card.Number == 1 || card.Number > 9)
                    {
                        return result;
                    }
                    else
                    {
                        if ((positive && ai.IsFriend(to) && value < -5) || (!positive && ai.IsEnemy(to) && value > 5))
                        {
                            result.Null = true;
                            return result;
                        }
                    }
                }
            }

            if (positive)
            {
                if (ai.IsFriend(to) && ai.IsGuanxingEffected(to, true, trick))
                {
                    value += ai.ChainDamage(damage);
                    if (value < -5)
                    {
                        Player wizzard = ai.GetWizzardRaceWinner(Name, to, to);
                        if (wizzard != null && ai.IsEnemy(wizzard) && ai.PlayersLevel[wizzard] >= 3)
                        {
                            result.Null = true;
                        }
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
            if (!ai.IsCardEffect(card, player, player) || (damage.Damage == 0 && player.GetMark("@tangerine") == 0))
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
                bool effect = (_damage.Damage > 1 || p.GetMark("@tangerine") > 0) && !ai.IsCardEffect(card, p, null) && ai.IsCancelTarget(card, p, null);
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
                        if (!ai.HasSkill("guanxing|yizhi|guanxing_jx|dianhua", p))
                            friends++;
                        Player last = room.GetLastAlive(p);
                        if (ai.IsEnemy(last) && ai.HasSkill("guanxing|yizhi|guanxing_jx|dianhua", last))
                        {
                            no_use = true;
                            break;
                        }
                    }
                    else
                    {
                        Player wizzard = ai.GetWizzardRaceWinner(Name, p, p);
                        if (!ai.HasSkill("guanxing|yizhi|guanxing_jx|dianhua", p) || wizzard == null || !ai.IsFriend(p, wizzard))
                            enemies++;
                    }
                }
            }

            if (!no_use)
            {
                if (ai.HasSkill("limu"))
                    shouldUse = true;
                else if (friends == 0 && enemies > 1)
                {
                    shouldUse = true;
                }
                else if (ai.IsSituationClear() && (double)enemies / friends > 1.5)
                {
                    shouldUse = true;
                }
                if (shouldUse)
                    use.Card = card;
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (ai.HasSkill("limu", player)) return 9;

            return 0;
        }
    }

    public class HegNullificationAI : UseCard
    {
        public HegNullificationAI() : base(HegNullification.ClassName) { }
        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            if (!string.IsNullOrEmpty(ai.Choice[Name]))
                return ai.Choice[Name];

            return "single";
        }
    }

    public class CrossBowAI : UseCard
    {
        public CrossBowAI() : base(CrossBow.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (!ai.HasSkill("paoxiao", player))
            {
                foreach (Player p in ai.GetEnemies(player))
                {
                    if ((RoomLogic.DistanceTo(ai.Room, player, p, null, true) == 1 || ai.HasSkill("tianyi", player))  && !ai.HasArmorEffect(p, Vine.ClassName)
                        && !ai.HasArmorEffect(p, RenwangShield.ClassName))
                        value += 0.5;
                }

                if (value > 0)
                {
                    if (ai.HasSkill("luoshen", player))
                        value += 0.5;
                    if (ai.HasSkill("wusheng", player))
                        value += 0.5;
                    if (ai.HasSkill("zhiheng", player))
                        value += 0.5;
                }
            }

            return value;
        }
    }

    public class DoubleSwordAI : UseCard
    {
        public DoubleSwordAI() : base(DoubleSword.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
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

    public class DoubleSwordSkillAI : SkillEvent
    {
        public DoubleSwordSkillAI() : base(DoubleSword.ClassName) { }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use && !ai.IsFriend(use.From))
            {
                List<int> cards = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        cards.Add(id);
                }

                if (cards.Count > 0)
                {
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (ids.Count > 4 || values[0] < 4)
                        return new List<int> { cards[0] };
                }
            }

            return new List<int>();
        }
    }

    public class QinggangSwordAI : UseCard
    {
        public QinggangSwordAI() : base(QinggangSword.ClassName)
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
                if (RoomLogic.HasShownArmorEffect(ai.Room, p))
                    value += 0.5;
            }

            return value;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class SpeakSKillAI : SkillEvent
    {
        public SpeakSKillAI() : base(Spear.ClassName)
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<int> ids= player.GetCards("h");
            ids.AddRange(player.GetHandPile());
            if (ids.Count >= 2)
            {
                List<double> values = ai.SortByUseValue(ref ids, false);
                double value = values[0] + values[1];
                if (value < Engine.GetCardUseValue(Slash.ClassName))
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(ids[0]);
                    slash.AddSubCard(ids[1]);
                    slash = RoomLogic.ParseUseCard(ai.Room, slash);
                    return new List<WrappedCard> { slash };
                }

                if (ai.HasSkill("tushe") && player.JudgingArea.Count > 0)
                {
                    Room room = ai.Room;
                    List<int> basic = new List<int>(), subs = new List<int>(); ;
                    foreach (int id in player.GetCards("h"))
                    {
                        WrappedCard card = room.GetCard(id);
                        if (Engine.GetFunctionCard(card.Name).TypeID == FunctionCard.CardType.TypeBasic)
                            basic.Add(id);
                    }

                    if (basic.Count > 0)
                    {
                        ai.SortByUseValue(ref basic, false);
                        for (int i = 0; i < Math.Min(2, basic.Count); i++)
                            subs.Add(basic[i]);
                    }

                    if (subs.Count < 2)
                    {
                        for (int i = 0; i < ids.Count; i++)
                        {
                            int id = ids[i];
                            if (!subs.Contains(id))
                                subs.Add(id);
                            else
                                continue;

                            if (subs.Count >= 2)
                                break;
                        }
                    }

                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCards(subs);
                    slash = RoomLogic.ParseUseCard(ai.Room, slash);
                    return new List<WrappedCard> { slash };
                }
            }

            return null;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (targets.Count > 0) return 0;

            Room room = ai.Room;
            double value = 0;
            foreach (int id in card.SubCards)
            {
                double _value = 0;
                List<WrappedCard> cards = ai.GetViewAsCards(player, id);
                foreach (WrappedCard c in cards)
                {
                    double card_value = ai.GetUseValue(c, player, room.GetCardPlace(id));
                    if (card_value > _value)
                        _value = card_value;
                }
                value += _value;
            }

            return Math.Min(0, 4-value);
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            if (pattern == Slash.ClassName)
            {
                List<int> ids= player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                CardUseStruct.CardUseReason reason = ai.Room.GetRoomState().GetCurrentCardUseReason();
                if (reason != CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY && reason != CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                {
                    foreach (int id in ids)
                        if (ai.Room.GetCard(id).Name == Slash.ClassName)
                            return new List<WrappedCard>();
                }

                if (ids.Count >= 2)
                {
                    List<double> values = ai.SortByUseValue(ref ids, false);
                    if (ai.Self == player)
                    {
                        bool will_use = false;
                        if (reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
                        {
                            if (ai.HasSkill("tushe") && player.JudgingArea.Count > 0)
                                will_use = true;
                            else
                            {
                                double value = values[0] + values[1];
                                if (value < Engine.GetCardUseValue(Slash.ClassName))
                                    will_use = true;
                            }
                        }
                        else
                        {
                            ai.SortByKeepValue(ref ids, false);
                            double value = ai.GetKeepValue(ids[0], player);
                            value += ai.GetKeepValue(ids[1], player);
                            if (value < Engine.GetCardKeepValue(Slash.ClassName))
                                will_use = true;
                        }

                        if (will_use)
                        {
                            if (ai.HasSkill("tushe"))
                            {
                                Room room = ai.Room;
                                List<int> basic = new List<int>(), subs = new List<int>(); ;
                                foreach (int id in player.GetCards("h"))
                                {
                                    WrappedCard card = room.GetCard(id);
                                    if (Engine.GetFunctionCard(card.Name).TypeID == FunctionCard.CardType.TypeBasic)
                                        basic.Add(id);
                                }

                                if (basic.Count > 0)
                                {
                                    ai.SortByUseValue(ref basic, false);
                                    for (int i = 0; i < Math.Min(2, basic.Count); i++)
                                        subs.Add(basic[i]);
                                }

                                if (subs.Count < 2)
                                {
                                    for (int i = 0; i < ids.Count; i++)
                                    {
                                        int id = ids[i];
                                        if (!subs.Contains(id))
                                            subs.Add(id);
                                        else
                                            continue;

                                        if (subs.Count >= 2)
                                            break;
                                    }
                                }

                                WrappedCard slash = new WrappedCard(Slash.ClassName)
                                {
                                    Skill = Name,
                                    ShowSkill = Name
                                };
                                slash.AddSubCards(subs);
                                slash = RoomLogic.ParseUseCard(ai.Room, slash);
                                return new List<WrappedCard> { slash };
                            }
                            else
                            {
                                WrappedCard slash = new WrappedCard(Slash.ClassName)
                                {
                                    Skill = Name,
                                    ShowSkill = Name
                                };
                                slash.AddSubCard(ids[0]);
                                slash.AddSubCard(ids[1]);
                                slash = RoomLogic.ParseUseCard(ai.Room, slash);
                                return new List<WrappedCard> { slash };
                            }
                        }
                    }
                    else
                    {
                        WrappedCard slash = new WrappedCard(Slash.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name
                        };
                        slash.AddSubCard(ids[0]);
                        slash.AddSubCard(ids[1]);
                        slash = RoomLogic.ParseUseCard(ai.Room, slash);
                        return new List<WrappedCard> { slash };
                    }
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class SpearAI : UseCard
    {
        public SpearAI() : base(Spear.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
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
        public AxeAI() : base(Axe.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            if (data is SlashEffectStruct effect)
            {
                List<int> ids = player.GetCards("he");
                ids.Remove(player.Weapon.Key);
                Room room = ai.Room;
                DamageStruct damage = new DamageStruct(effect.Slash, effect.From, effect.To, effect.Drank + 1 + effect.ExDamage);
                if (effect.Slash.Name == FireSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Fire;
                else if (effect.Slash.Name == "Thunder") damage.Nature = DamageStruct.DamageNature.Thunder;
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score > 0 && ids.Count > 1)
                {
                    List<int> subs = new List<int>();
                    double discard_value = 0;
                    ai.SortByKeepValue(ref ids, false);
                    double keep = ai.GetKeepValue(ids[0], player);
                    if (ai.GetOverflow(player) > 0 && room.GetCardPlace(ids[0]) == Player.Place.PlaceHand && keep > 0)
                        keep /= 2;
                    subs.Add(ids[0]);
                    discard_value += keep;
                    bool equip = room.GetCardPlace(ids[0]) == Player.Place.PlaceEquip;
                    double keep2 = ai.GetKeepValue(ids[1], player, Player.Place.PlaceUnknown, equip);
                    if (ai.GetOverflow(player) > 1 && room.GetCardPlace(ids[1]) == Player.Place.PlaceHand && keep2 > 0)
                        keep2 /= 2;
                    discard_value += keep2;
                    subs.Add(ids[1]);

                    if (discard_value / 1.5 < score.Score)
                    {
                        WrappedCard card = new WrappedCard(DummyCard.ClassName)
                        {
                            Skill = Name
                        };
                        card.AddSubCards(subs);
                        use.Card = card;
                    }
                }
            }

            return use;
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
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
        public KylinBowAI() : base(KylinBow.ClassName)
        {
        }
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

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            ai.Choice[Name] = null;
            if (data is Player target && ai.IsEnemy(target))
            {
                Room room = ai.Room;
                List<int> disable_equiplist = new List<int>(), equiplist = new List<int>();
                for (int i = 2; i < 6; i++)
                {
                    if ((i == 2 || i == 3 || i == 5) && target.GetEquip(i) >= 0 && RoomLogic.CanDiscard(room, player, target, target.GetEquip(i)))
                    {
                        equiplist.Add(target.GetEquip(i));
                    }
                }

                double value = 0;
                int result = -1;
                foreach (int id in equiplist)
                {
                    double _value = ai.GetKeepValue(id, target, Player.Place.PlaceEquip);
                    if (_value > value)
                    {
                        value = _value;
                        result = id;
                    }
                }

                if (result >= 0)
                {
                    string choice = null;
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(result).Name);
                    if (fcard is OffensiveHorse)
                        choice = "ohorse";
                    else if (fcard is DefensiveHorse)
                        choice = "dhorse";
                    else
                        choice = "shorse";
                    ai.Choice[Name] = choice;
                }
                else
                    return false;
            }

            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choices, object data)
        {
            if (!string.IsNullOrEmpty(ai.Choice[Name]))
                return ai.Choice[Name];

            return choices.Split('+')[0];
        }
    }
    public class EightDiagramAI : UseCard
    {
        public EightDiagramAI() : base(EightDiagram.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.HasSkill("leiji|leiji_jx")) return true;

            Room room = ai.Room;
            CardAskStruct asked = (CardAskStruct)data;
            string prompt = asked.Prompt;
            if (prompt.StartsWith("slash-jink") || prompt.StartsWith("@multi-jink") || prompt.StartsWith("archery-attack-jink"))
            {
                List<CardUseStruct> uses = room.GetUseList();
                CardUseStruct use = uses[uses.Count - 1];
                DamageStruct damage = new DamageStruct(use.Card, use.From, player);
                if (use.Card.Name == FireSlash.ClassName)
                    damage.Nature = DamageStruct.DamageNature.Fire;
                else if (use.Card.Name == ThunderSlash.ClassName)
                    damage.Nature = DamageStruct.DamageNature.Thunder;

                if (ai.GetDamageScore(damage).Score > 0)
                    return false;
            }
            else if (asked.Reason == "hujia")
            {
                string caocao_name = prompt.Split(':')[1];
                Player caocao = room.FindPlayer(caocao_name);
                if (caocao != null && ai.IsFriend(caocao))
                    return true;

                return false;
            }

            return true;
        }

        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (ai.HasSkill("bazhen", player))
                value -= 2.5;

            if (ai.HasSkill("tiandu", player))
            {
                value += 2;
                Player lord = RoomLogic.FindPlayerBySkillName(ai.Room, "hujia");
                if (lord != null && ai.IsFriend(player, lord)) value += 1;
            }

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
        public IceSwordAI() : base(IceSword.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            DamageStruct damage = (DamageStruct)data;
            damage.Steped = DamageStruct.DamageStep.Caused;

            ScoreStruct d = ai.GetDamageScore(damage);
            return !d.DoDamage;
        }

        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = ai.AjustWeaponRangeValue(player, card);
            string skills = TrustedAI.MasochismSkill + "|tianxiang|tianxiang_jx";
            foreach (Player p in ai.GetEnemies(player))
            {
                if (ai.HasSkill(skills, p) && RoomLogic.DistanceTo(ai.Room, player, p, null, true) <= 2)
                    value += 2;
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
        public RenwangShieldAI() : base(RenwangShield.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (ai.HasSkill("bazhen|linglong", player))
                value -= 2;

            if (place == Place.PlaceEquip)
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
        public FanAI() : base(Fan.ClassName)
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardUseStruct use)
            {
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { use.Card }, new List<Player>(use.To), false);
                if (scores.Count > 0 && (scores[0].Card.Name == FireSlash.ClassName ||
                    (ai.HasSkill("lihuo") && scores.Count > 1 && scores[1].Card.Name == FireSlash.ClassName || scores[1].Score > 0 && scores[0].Score - scores[1].Score < 1)))
                {
                    WrappedCard fire = new WrappedCard(FireSlash.ClassName) { Skill = use.Card.Skill, UserString = use.Card.UserString };
                    fire.AddSubCard(use.Card);
                    fire = RoomLogic.ParseUseCard(ai.Room, fire);
                    foreach (Player p in use.To)
                    {
                        DamageStruct damage = new DamageStruct(fire, player, p, 1 + use.Drank + use.ExDamage, DamageStruct.DamageNature.Fire);
                        if (ai.GetDamageScore(damage).Score + ai.ChainDamage(damage) < 0)
                            return false;
                    }

                    return true;
                }
            }

            return false;
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
                if (ai.HasArmorEffect(p, Vine.ClassName) && RoomLogic.DistanceTo(ai.Room, player, p, null, true) <= 4)
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
        public SixSwordsAI() : base(SixSwords.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
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
        public TribladeAI() : base(Triblade.ClassName)
        {
        }
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
            CardUseStruct use = new CardUseStruct
            {
                From = player
            };
            Room room = ai.Room;
            
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("h"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);
            ai.SortByKeepValue(ref ids, false);
            double value = ai.GetUseValue(ids[0], player);

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HasFlag("TribladeCanBeSelected"))
                {
                    DamageStruct damage = new DamageStruct(Name, player, p);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }
            }
            if (scores.Count > 0)
            {
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 0 && scores[0].Score > value)
                {
                    WrappedCard card = new WrappedCard(TribladeSkillCard.ClassName)
                    {
                        Skill = Name
                    };
                    card.AddSubCard(ids[0]);
                    use.Card = card;
                    use.To = scores[0].Players;

                    Debug.Assert(use.To.Count == 1 && use.To[0] != null);
                }
            }

            return use;
        }
    }
    public class VineAI : UseCard
    {
        public VineAI() : base(Vine.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            Room room = ai.Room;
            if (ai.HasSkill("gangzhi|shixin", player)) return 10;
            List<Player> enemies = ai.GetEnemies(player);
            if (ai.GetFriends(player).Count + enemies.Count == room.AliveCount() && enemies.Count == 1 && ai.HasSkill("jueqing|gangzhi_classic", enemies[0])) return 8;
            if (ai.HasSkill("liangying|fangzhu|jieming|jieming_jx|benyu|jianxiong|jianxiong_jx|huituo|chengxiang", player))
                value -= 10;

            if (ai.HasSkill("bazhen|linglong", player))
                value -= 5;

                
            if (ai.Self == player)
            {
                int count = 0;
                List<int> ids = player.GetCards("he");
                ids.AddRange(player.GetHandPile());
                foreach (int id in ids)
                    if (ai.IsCard(id, "Jink", player))
                        count++;

                if (count == 0)
                    value -= 6;
            }
            else if (ai.IsLackCard(player, Jink.ClassName))
                value -= 6;

            foreach (Player p in ai.GetFriends(player))
                if (p != player && p.Chained)
                    value -= 0.7;

            if (player.Chained) value -= 10;

            if (!RoomLogic.PlayerHasSkill(ai.Room, player, "kongcheng|kongcheng_jx") || !(ai.Room.Current == player && player.IsLastHandCard(card)) || !player.IsKongcheng())
            {
                foreach (Player p in ai.GetEnemies(player))
                {
                    if (p.HasWeapon(Fan.ClassName) || RoomLogic.PlayerHasSkill(ai.Room, p, "huoji") || ai.GetKnownCardsNums(FireAttack.ClassName, "he", player, p) > 0)
                        value -= 3;
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
        public SilverLionAI() : base(SilverLion.ClassName)
        {
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            if (player.GetLostHp() > 0)
            {
                if (!use && place == Place.PlaceEquip && !ai.HasSkill("dingpan", player) && (player != ai.Self || !player.ArmorIsNullifiedBy(ai.Self)))
                   return -8;
                if (use && place != Place.PlaceEquip)
                    value += 3;
            }
            if (ai.HasSkill("kurou|duanliang|xiongsuan|dingpan|kurou_jx", player))
                value += 1.2;

            if (player.Chained) value += 0.5;

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
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return ai.AjudstDHorseValue(player, card, place);
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
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return ai.AjustOHorseValue(player, card, place);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }
    }

    public class CollateralAI : UseCard
    {
        public CollateralAI() : base(Collateral.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> fromList = new List<Player>();
            double basic_value = 0;
            if (ai.HasSkill("jizhi|jizhi_jx", player))
                basic_value += 4;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.GetWeapon() && RoomLogic.IsProhibited(room, player, p, card) == null && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                    fromList.Add(p);
            }

            bool needCrossbow = false;
            if (!ai.HasSkill("paoxiao|paoxiao_jx|kuangcai") && ai.GetCards(Slash.ClassName, player).Count > 2)
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
                    if (ai.IsLackCard(from, Slash.ClassName) || (ai.GetKnownCardsNums(Slash.ClassName, "he", from, player) == 0 && from.HandcardNum - ai.GetKnownCards(from).Count < 2))
                        from_value += 3;
                    else if (from.HasWeapon("Halberg"))
                        from_value -= 3;
                }

                if (from.HasWeapon(CrossBow.ClassName) && needCrossbow)
                    from_value += 4;

                if (needWeapon)
                    from_value += 2;

                foreach (string skill in ai.GetKnownSkills(from))
                {
                    SkillEvent ev = Engine.GetSkillEvent(skill);
                    if (ev != null)
                        from_value += ev.TargetValueAdjust(ai, card, player, new List<Player> { from }, from);
                }


                Dictionary<Player, double> to_values = new Dictionary<Player, double>();
                foreach (Player to in room.GetOtherPlayers(from))
                {
                    double to_value = -100;
                    if (RoomLogic.CanSlash(room, from, to))
                    {
                        to_value = 0;
                        if (ai.IsEnemy(to) && !ai.NotSlashJiaozhu(from, to, card))
                        {
                            to_value += 3;
                            if (ai.IsWeak(to))
                                to_value += 5;
                        }
                        if (ai.JiaozhuneedSlash(from, to, card))
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
            //to do
            //
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player
            };
            Room room = ai.Room;
            List<WrappedCard> all = ai.GetCards(Slash.ClassName, player), slashes = new List<WrappedCard>();
            foreach (WrappedCard slash in all)
                if (Slash.IsAvailable(room, player))
                    slashes.Add(slash);

            if (slashes.Count == 0) return use;
            List<string> strs = new List<string>(prompt.Split(':'));
            Player target = room.FindPlayer(strs[2]);
            Player target2 = room.FindPlayer(strs[1]);
            if (ai.IsFriend(target) && player.HasWeapon(CrossBow.ClassName) && ai.GetKnownCardsNums(Slash.ClassName, "he", target, player) > 1)
                return use;

            if (ai.JiaozhuneedSlash(player, target2, new WrappedCard(Slash.ClassName)))
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
            else if (ai.NotSlashJiaozhu(player, target2, new WrappedCard(Slash.ClassName)))
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
            if (scores.Count > 0 && scores[0].Score >= -2)
            {
                use.Card = scores[0].Card;
                use.To = scores[0].Players;
                return use;
            }

            //todo adjust halberg

            return use;
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            if (positive && ai.Self == to && ai.IsEnemy(from) && ai.HasSkill("jiewei"))
                result.Null = true;

            return result;
        }
    }

    public class MegatamaCardAI : UseCard
    {
        public MegatamaCardAI() : base(MegatamaCard.ClassName)
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
                WrappedCard card = new WrappedCard(MegatamaCard.ClassName)
                {
                    Skill = Name
                };
                use.Card = card;
            }

            return use;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.HandcardNum < RoomLogic.GetMaxCards(ai.Room, player))
            {
                return new List<WrappedCard> { new WrappedCard(MegatamaCard.ClassName) { Skill = Name } };
            }
            return null;
        }
    }

    public class CompanionCardAI : UseCard
    {
        public CompanionCardAI() : base(CompanionCard.ClassName)
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
            if (player.GetMark("@companion") > 0 && pattern == Peach.ClassName)
            {
                WrappedCard peach = new WrappedCard(Peach.ClassName);
                WrappedCard card = new WrappedCard(CompanionCard.ClassName)
                {
                    Skill = Name
                };
                peach.UserString = RoomLogic.CardToString(ai.Room, card);
                result.Add(peach);
            }

            return result;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("companion") == 0) return null;

            if (player.IsWounded() || (ai.GetOverflow(player) == 0 && !(player.IsKongcheng() && ai.NeedKongcheng(player))))
            {
                foreach (Player p in ai.FriendNoSelf)
                {
                    if (ai.IsWeak(p))
                        return null;
                }

                return new List<WrappedCard> { new WrappedCard(CompanionCard.ClassName) { Skill = Name } };
            }

            return null;
        }
    }

    public class PioneerCardAI : UseCard
    {
        public PioneerCardAI() : base(PioneerCard.ClassName)
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

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
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

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.MaxHp - player.HandcardNum >= 2 && !(player.IsKongcheng() && ai.NeedKongcheng(player)))
            {
                return new List<WrappedCard> { new WrappedCard(PioneerCard.ClassName) { Skill = Name } };
            }

            return null;
        }
    }
}
