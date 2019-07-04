using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class FormationAI : AIPackage
    {
        public FormationAI() : base("Formation")
        {
            events = new List<SkillEvent>
            {
                new TuntianAI(),
                new JixiAI(),
                new ZiliangAI(),
                new HuyuanAI(),
                new TiaoxinAI(),
                new YizhiAI(),
                new ShengxiAI(),
                new ShouchengAI(),
                new ShangyiAI(),
                new YichengAI(),
                new QianhuanAI(),
                new ZhenduAI(),
                new QiluanAI(),
                new ZhangwuAI(),
                new JizhaoAI()
            };

            use_cards = new List<UseCard>
            {
                new TiaoxinCardAI(),
                new ShangyiCardAI(),
            };
        }
    }

    public class TuntianAI : SkillEvent
    {
        public TuntianAI() : base("tuntian")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }
    public class JixiAI : SkillEvent
    {
        public JixiAI() : base("jixi")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> cards = new List<WrappedCard>();
            if (player.GetPile("field").Count > 0)
            {
                foreach (int id in player.GetPile("field"))
                {
                    WrappedCard shun = new WrappedCard("Snatch")
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    shun.AddSubCard(id);
                    shun = RoomLogic.ParseUseCard(ai.Room, shun);
                    cards.Add(shun);
                }
            }

            return cards;
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> cards = new List<WrappedCard>();
            if (pattern == "Snatch" && player.GetPile("field").Count > 0)
            {
                foreach (int id in player.GetPile("field"))
                {
                    WrappedCard shun = new WrappedCard("Snatch")
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    shun.AddSubCard(id);
                    shun = RoomLogic.ParseUseCard(ai.Room, shun);
                    cards.Add(shun);
                }
            }

            return cards;
        }
    }

    public class ZiliangAI : SkillEvent
    {
        public ZiliangAI() : base("ziliang")
        { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            if (player.ContainsTag("ziliang_aidata") && player.GetTag("ziliang_aidata") is string player_name)
            {
                Room room = ai.Room;
                Player target = room.FindPlayer(player_name);
                if (target != null)
                {
                    double best_v = 0;
                    int result = -1;
                    foreach (int id in player.GetPile("field"))
                    {
                        double value = Math.Max(ai.GetKeepValue(id, target, Player.Place.PlaceHand), ai.GetUseValue(id, target, Player.Place.PlaceHand));
                        if (value > best_v)
                        {
                            best_v = value;
                            result = id;
                        }

                        if (best_v > 2.5 && result >= 0)
                        {
                            use.Card = new WrappedCard("ZiliangCard") { Skill = Name, ShowSkill = Name };
                            use.Card.AddSubCard(result);
                        }
                    }
                }
            }

            return use;
        }
    }

    public class HuyuanAI : SkillEvent
    {
        public HuyuanAI() : base("huyuan")
        { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }
    }

    public class TiaoxinAI : SkillEvent
    {
        public TiaoxinAI() : base("tiaoxin")
        {
            key = new List<string> { "cardUsed" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices.Length >= 5 && choices[1] == "Slash" && choices[2] == "@tiaoxin-slash" && choices[4] == "nil")
                {
                    Player jiangwei = ai.Room.FindPlayer(choices[3]);
                    if (jiangwei != null && !ai.IsFriend(jiangwei, player))
                        ai.SetCardLack(player, "Slash");
                }
            }
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name) && ai.IsSituationClear())
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is DefensiveHorse || fcard is SpecialEquip)
                    return -2;
            }

            if (!isUse && ai.IsCard(card.GetEffectiveId(), "Slash", player))
            {
                Player jiangwei = ai.FindPlayerBySkill(Name);
                if (jiangwei != null && !ai.IsFriend(jiangwei, player) && RoomLogic.CanSlash(ai.Room, player, jiangwei))
                    return 2;
            }

            return 0;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("TiaoxinCard"))
                return new List<WrappedCard> { new WrappedCard("TiaoxinCard") { Skill = Name, ShowSkill = Name } };

            return null;
        }
    }

    public class TiaoxinCardAI : UseCard
    {
        public TiaoxinCardAI() : base("TiaoxinCard")
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!RoomLogic.InMyAttackRange(room, p, player) || p.IsNude()) continue;
                ScoreStruct score = ai.FindCards2Discard(player, p, "he", HandlingMethod.MethodDiscard);
                score.Players = new List<Player> { p };
                if (ai.IsEnemy(p))
                {
                    if (p.HandcardNum + p.GetPile("wooden_ox").Count < 3 || ai.IsLackCard(p, "Slash"))
                    {
                        score.Score += 3;
                    }
                    else
                    {
                        bool armor_ignore = false;
                        if (p.HasWeapon("QinggangSword") || ai.HasSkill("jianchu", p))
                            armor_ignore = true;
                        else if (ai.HasSkill("paoxiao|paoxiao_fz", p))
                        {
                            Player lord = ai.FindPlayerBySkill("shouyue");
                            if (lord != null && RoomLogic.PlayerHasShownSkill(room, lord, "shouyue") && RoomLogic.WillBeFriendWith(room, p, lord))
                                armor_ignore = true;
                        }

                        if (!armor_ignore && ai.HasArmorEffect(player, "RenwangShield"))
                            score.Score += 0.5;
                        if (!armor_ignore && ai.HasArmorEffect(player, "EightDiagram"))
                            score.Score += 0.5;
                        if (ai.HasSkill("wushang", p))
                            score.Score -= 0.5;
                        if (p.HasWeapon("DragonPhoenix"))
                            score.Score -= 0.5;
                        if (ai.HasSkill("jianchu", p))
                            score.Score = -0.5;
                        if (ai.HasSkill("tieqi|tieqi_fz", p))
                            score.Score = 1;
                        if (ai.GetKnownCardsNums("Jink", "he", player) == 0)
                            score.Score -= 2;
                    }
                }
                scores.Add(score);
            }

            if (scores.Count > 0)
            {
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 2)
                {
                    use.Card = card;
                    use.To = new List<Player>(scores[0].Players);
                }
            }
        }
    }
    public class YizhiAI : SkillEvent
    {
        public YizhiAI() : base("yizhi")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return base.OnSkillInvoke(ai, player, data);
        }
    }

    public class ShengxiAI : SkillEvent
    {
        public ShengxiAI() : base("shengxi")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.WillShowForDefence() || player.HandcardNum < player.Hp) return true;

            return false;
        }
    }
    public class ShouchengAI : SkillEvent
    {
        public ShouchengAI() : base("shoucheng")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                if (!ai.NeedKongcheng(player))
                    return true;
            }

            return false;
        }
    }

    public class ShangyiAI : SkillEvent
    {
        public ShangyiAI() : base("shangyi") { key = new List<string> { "skillChoice" }; }

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
                        if (p.HasFlag("shangyiTarget"))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (target != null)
                    {
                        if (choice == "hidden_general")
                            ai.SetKnown(target, "hd");
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
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("ShangyiCard") && !player.IsKongcheng())
            {
                WrappedCard c = new WrappedCard("ShangyiCard")
                {
                    Skill = Name,
                    ShowSkill = Name
                };

                return new List<WrappedCard> { c };
            }

            return null;
        }
        public override int OnPickAG(TrustedAI ai, Player player, List<int> card_ids, bool refusable)
        {
            if (player.ContainsTag(Name))
            {
                Room room = ai.Room;
                Player target = room.FindPlayer((string)player.GetTag(Name));
                if (target != null && !ai.IsFriend(target))
                {
                    double best = -1000;
                    int result = -1;
                    foreach (int id in card_ids)
                    {
                        double value = ai.GetKeepValue(id, target, Player.Place.PlaceHand);
                        if (value > best)
                        {
                            best = value;
                            result = id;
                        }
                    }
                    if (best > 0 && result != -1)
                        return result;
                }
            }

            return -1;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (ai.Choice[Name] != null)
                return ai.Choice[Name];

            return string.Empty;
        }
    }

    public class ShangyiCardAI : UseCard
    {
        public ShangyiCardAI() : base("ShangyiCard") { }
        
        public override double UsePriorityAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9.3;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Choice["shangyi"] = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (ai.GetPlayerTendency(p) == "unknown")
                {
                    ai.Choice["shangyi"] = "hidden_general";
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }

            List<Player> enemies = ai.GetPrioEnemies();
            ai.SortByDefense(ref enemies, false);
            foreach (Player p in enemies)
            {
                if (p.HandcardNum - ai.GetKnownCards(p).Count > 2)
                {
                    ai.Choice[Name] = "handcards";
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }

            enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            foreach (Player p in enemies)
            {
                if (p.HandcardNum - ai.GetKnownCards(p).Count > 2)
                {
                    ai.Choice["shangyi"] = "handcards";
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }
            foreach (Player p in enemies)
            {
                if (p.HandcardNum > ai.GetKnownCards(p).Count)
                {
                    ai.Choice["shangyi"] = "handcards";
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }
        }
    }

    public class YichengAI : SkillEvent
    {
        public YichengAI() : base("yicheng")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForDefence();
        }
    }

    public class QianhuanAI : SkillEvent
    {
        public QianhuanAI() : base("qianhuan")
        { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            if (ai.WillShowForDefence())
            {
                Room room = ai.Room;
                List<int> ids = new List<int>();
                foreach (int id in player.HandCards)
                {
                    WrappedCard card = room.GetCard(id);
                    bool add = true;
                    foreach (int _id in player.GetPile("sorcery"))
                    {
                        if (room.GetCard(_id).Suit == card.Suit)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add) ids.Add(id);
                }

                foreach (int id in player.GetEquips())
                {
                    WrappedCard card = room.GetCard(id);
                    bool add = true;
                    foreach (int _id in player.GetPile("sorcery"))
                    {
                        if (room.GetCard(_id).Suit == card.Suit)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add) ids.Add(id);
                }

                if (ids.Count > 0)
                {
                    ai.SortByKeepValue(ref ids, false);
                    foreach (int id in ids)
                    {
                        double keep = ai.GetKeepValue(id, player);
                        double use = ai.GetUseValue(id, player);
                        if (keep < 0 || (keep <= 5 && use <= 5))
                            return new List<int> { id };
                    }
                }
            }

            return new List<int>();
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct result = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };
            Room room = ai.Room;
            int sub = -1;
            foreach (int id in player.GetPile("sorcery"))
            {
                WrappedCard card = room.GetCard(id);
                if (card.Suit == WrappedCard.CardSuit.Club || card.Suit == WrappedCard.CardSuit.Spade)
                {
                    sub = id;
                    break;
                }
            }
            if (sub == -1)
                sub = player.GetPile("sorcery")[0];
            WrappedCard qianhuan = new WrappedCard("QianhuanCard")
            {
                Mute = true,
                Skill = Name
            };
            qianhuan.AddSubCard(sub);

            if (room.ContainsTag("qianhuan_data") && room.GetTag("qianhuan_data") is CardUseStruct use)
            {
                if (use.From != null && ai.IsEnemy(use.From) && !ai.IsCancelTarget(use.Card, use.To[0], use.From) && ai.IsCardEffect(use.Card, use.To[0], use.From))
                {
                    if (use.Card.Name == "SupplyShortage" || use.Card.Name == "Indulgence" || use.Card.Name == "Snatch")
                    {
                        result.Card = qianhuan;
                        return result;
                    }
                }
                else if (use.Card.Name.Contains("Slash"))
                {
                    DamageStruct damage = new DamageStruct(use.Card, use.From, use.To[0], 1 + use.Drank);
                    if (use.Card.Name == "FireSlash")
                        damage.Nature = DamageStruct.DamageNature.Fire;
                    else if (use.Card.Name == "ThunderSlash")
                        damage.Nature = DamageStruct.DamageNature.Thunder;

                    if (ai.IsWeak(use.To[0]) || ai.GetDamageScore(damage).Score < -5)
                    {
                        result.Card = qianhuan;
                        return result;
                    }
                }
                else if (use.Card.Name == "BurningCamps")
                {
                    DamageStruct damage = new DamageStruct(use.Card, use.From, use.To[0], 1, DamageStruct.DamageNature.Fire);
                    if (ai.GetDamageScore(damage).Score < -5)
                    {
                        result.Card = qianhuan;
                        return result;
                    }
                }
            }

            return result;
        }
    }

    public class ZhenduAI : SkillEvent
    {
        public ZhenduAI() : base("zhendu")
        { }

        public override List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip)
        {
            if (!ai.WillShowForAttack()) return new List<int>();

            Room room = ai.Room;
            Player target = room.Current;
            List<int> ids = new List<int>();
            
            foreach (int id in player.HandCards)
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);
            ai.SortByKeepValue(ref ids, false);
            double value = ai.GetKeepValue(ids[0], player);
            if (ai.GetOverflow(player) > 1)
                value /= 3;
            DamageStruct damage = new DamageStruct(Name, player, target);
            if (ai.IsFriend(target))
            {
                bool range = false;
                foreach (Player p in room.GetOtherPlayers(target))
                {
                    if (ai.GetPrioEnemies().Contains(p) && RoomLogic.InMyAttackRange(room, target, p))
                    {
                        range = true;
                        break;
                    }
                }

                if (range && target.HandcardNum + target.GetPile("wooden_ox").Count > 3)
                {
                    if (ai.GetDamageScore(damage).Score < -3 && ai.HasSkill("wushuang|jianchu", target) && value < 3)
                        return new List<int> { ids[0] };
                }
            }
            else
            {
                if (ai.GetDamageScore(damage).Score > 7 && value < 7)
                    return new List<int> { ids[0] };
                else if (ai.GetDamageScore(damage).Score > 4 && value < 3)
                {
                    if (ai.IsLackCard(target, "Slash") || target.HandcardNum + target.GetPile("wooden_ox").Count < 3)
                    {
                        return new List<int> { ids[0] };
                    }
                    else
                    {
                        bool range = false;
                        foreach (Player p in room.GetOtherPlayers(target))
                        {
                            if (ai.IsFriend(p) && RoomLogic.InMyAttackRange(room, target, p))
                            {
                                range = true;
                                break;
                            }
                        }

                        if (!range) return new List<int> { ids[0] };
                    }
                }
            }

            return new List<int>();
        }
    }

    public class QiluanAI : SkillEvent
    {
        public QiluanAI() : base("qiluan") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class ZhangwuAI : SkillEvent
    {
        public ZhangwuAI() : base("zhangwu")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class JizhaoAI : SkillEvent
    {
        public JizhaoAI() : base("jizhao") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            int count = 0;
            List<WrappedCard> analeptics = ai.GetCards("Analeptic", player, true);
            analeptics.AddRange(ai.GetCards("Peach", player, true));
            foreach (WrappedCard card in analeptics)
                if (!RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, player, player, card) == null)
                    count++;

            if (count >= 1 - player.Hp)
                return false;

            return true;
        }
    }
}