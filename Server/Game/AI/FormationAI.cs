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
                    WrappedCard shun = new WrappedCard(Snatch.ClassName)
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
            if (pattern == Snatch.ClassName && player.GetPile("field").Count > 0)
            {
                foreach (int id in player.GetPile("field"))
                {
                    WrappedCard shun = new WrappedCard(Snatch.ClassName)
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
                scores.Add(ai.FindCards2Discard(player, p, string.Empty, "he", HandlingMethod.MethodDiscard));
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0) return scores[0].Players;
            }

            return new List<Player>();
        }
    }

    public class TiaoxinAI : SkillEvent
    {
        public TiaoxinAI() : base("tiaoxin")
        {
            key = new List<string> { "cardUsed:Slash:@tiaoxin-slash" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices.Length >= 5 && choices[1] == Slash.ClassName && choices[2] == "@tiaoxin-slash" && choices[4] == "nil")
                {
                    Player jiangwei = ai.Room.FindPlayer(choices[3]);
                    if (jiangwei != null && !ai.IsFriend(jiangwei, player))
                        ai.SetCardLack(player, Slash.ClassName);
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

            if (!isUse && card.GetEffectiveId() >= 0 && ai.IsCard(card.GetEffectiveId(), Slash.ClassName, player))
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

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            string target_name = prompt.Split(':')[1];
            Room room = ai.Room;
            Player target = room.FindPlayer(target_name);
            if (target != null)
            {
                if (ai.IsFriend(target))
                {
                    foreach (int id in player.GetEquips())
                    {
                        if (ai.GetKeepValue(id, player) < 0 && RoomLogic.CanDiscard(room, target, player, id))
                            return use;
                    }

                    //System.Diagnostics.Debug.Assert(room.GetAI(target) == null, "ai挑衅目标错误");
                }
                
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, null, new List<Player> { target });
                if (scores.Count > 0 && scores[0].Score > -2 && scores[0].Card != null)
                {
                    use.Card = scores[0].Card;
                    use.To.Add(target);
                }
            }

            return use;
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
                ScoreStruct score = ai.FindCards2Discard(player, p, string.Empty, "he", HandlingMethod.MethodDiscard);
                score.Players = new List<Player> { p };
                if (ai.IsEnemy(p))
                {
                    if (p.HandcardNum + p.GetPile("wooden_ox").Count < 3 || ai.IsLackCard(p, Slash.ClassName))
                    {
                        score.Score += 3;
                    }
                    else
                    {
                        bool armor_ignore = false;
                        if (p.HasWeapon(QinggangSword.ClassName) || p.HasWeapon(Saber.ClassName) || (player.GetArmor() && ai.HasSkill("jianchu|moukui", p)))
                            armor_ignore = true;
                        else if (ai.HasSkill("paoxiao|paoxiao_fz", p))
                        {
                            Player lord = ai.FindPlayerBySkill("shouyue");
                            if (lord != null && RoomLogic.PlayerHasShownSkill(room, lord, "shouyue") && RoomLogic.WillBeFriendWith(room, p, lord))
                                armor_ignore = true;
                        }

                        if (!armor_ignore && ai.HasArmorEffect(player, RenwangShield.ClassName))
                            score.Score += 0.5;
                        if (!armor_ignore && ai.HasArmorEffect(player, EightDiagram.ClassName))
                            score.Score += 0.5;
                        if (ai.HasSkill("wushang", p))
                            score.Score -= 0.5;
                        if (p.HasWeapon(DragonPhoenix.ClassName))
                            score.Score -= 0.5;
                        if (ai.HasSkill("jianchu", p))
                            score.Score = -0.5;
                        if (ai.HasSkill("tieqi|tieqi_fz", p))
                            score.Score = 1;
                        if (ai.GetKnownCardsNums(Jink.ClassName, "he", player) == 0)
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
        public ShangyiAI() : base("shangyi") { key = new List<string> { "skillChoice:shangyi" }; }

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
                            foreach (int id in target.GetCards("h"))
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
                Player target = null;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.HasFlag("shangyi_target"))
                    {
                        target = p;
                        break;
                    }
                }

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
        
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
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
                //有君角在，不要用太平发动
                Player lord = RoomLogic.FindPlayerBySkillName(room, "wendao");

                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == PeaceSpell.ClassName && lord != null) continue;
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
                    if (card.Name == PeaceSpell.ClassName && lord != null) continue;
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

            if (room.ContainsTag("qianhuan_data"))
            {
                if (room.GetTag("qianhuan_data") is CardUseStruct use)
                {
                    player.SetFlags("target_confirming");
                    if (ai.IsCancelTarget(use.Card, use.To[0], use.From) || !ai.IsCardEffect(use.Card, use.To[0], use.From))
                    {
                        player.SetFlags("-target_confirming");
                        return result;
                    }

                    if (use.Card.Name == SupplyShortage.ClassName || use.Card.Name == Indulgence.ClassName || use.Card.Name == Snatch.ClassName)
                    {
                        if (use.From != null && ai.IsEnemy(use.From))
                        {
                            player.SetFlags("-target_confirming");
                            result.Card = qianhuan;
                            return result;
                        }
                    }
                    else if (use.Card.Name == BurningCamps.ClassName)
                    {
                        DamageStruct damage = new DamageStruct(use.Card, use.From, use.To[0], 1, DamageStruct.DamageNature.Fire);
                        if (player.GetPile("sorcery").Count >= 3 && ai.GetDamageScore(damage).Score < 0 || ai.GetDamageScore(damage).Score < -5)
                        {
                            player.SetFlags("-target_confirming");
                            result.Card = qianhuan;
                            return result;
                        }
                    }
                    else if (use.Card.Name.Contains(Slash.ClassName))
                    {
                        DamageStruct damage = new DamageStruct(use.Card, use.From, use.To[0], 1 + use.Drank + use.ExDamage);
                        if (use.Card.Name == FireSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Fire;
                        else if (use.Card.Name == ThunderSlash.ClassName) damage.Nature = DamageStruct.DamageNature.Thunder;
                        double value = ai.GetDamageScore(damage).Score;
                        if (value < -5 || (ai.IsWeak(use.To[0]) && value < 0) || (value < 0 && player.GetPile("sorcery").Count >= 3))
                        {
                            player.SetFlags("-target_confirming");
                            result.Card = qianhuan;
                            return result;
                        }
                    }
                    else if (use.Card.Name == ArcheryAttack.ClassName)
                    {
                        DamageStruct damage = new DamageStruct(use.Card, use.From, use.To[0]);
                        double value = ai.GetDamageScore(damage).Score;
                        if ((value < -5 || (ai.IsWeak(use.To[0]) && value < 0)) && player != use.To[0] ? ai.IsLackCard(use.To[0], Jink.ClassName) : ai.GetKnownCardsNums(Jink.ClassName, "he", use.To[0]) == 0)
                        {
                            player.SetFlags("-target_confirming");
                            result.Card = qianhuan;
                            return result;
                        }
                    }
                    else if (use.Card.Name == SavageAssault.ClassName)
                    {
                        Player menghuo = ai.FindPlayerBySkill("huoshou");
                        DamageStruct damage = new DamageStruct(use.Card, menghuo ?? use.From, use.To[0]);
                        double value = ai.GetDamageScore(damage).Score;
                        if ((value < -5 || (ai.IsWeak(use.To[0]) && value < 0)) && (player != use.To[0] ? ai.IsLackCard(use.To[0], Slash.ClassName)
                            : ai.GetKnownCardsNums(Slash.ClassName, "he", use.To[0]) == 0 || RoomLogic.IsHandCardLimited(room, use.To[0], HandlingMethod.MethodResponse)))
                        {
                            player.SetFlags("-target_confirming");
                            result.Card = qianhuan;
                            return result;
                        }
                    }
                    else if (use.Card.Name == Duel.ClassName)
                    {
                        DamageStruct damage = new DamageStruct(use.Card, use.From, use.To[0]);
                        double value = ai.GetDamageScore(damage).Score;
                        if (value < -5 || ((ai.IsWeak(use.To[0]) || player.GetPile("sorcery").Count >= 3) && value < 0))
                        {
                            player.SetFlags("-target_confirming");
                            result.Card = qianhuan;
                            return result;
                        }
                    }
                }
                else if (data is CardsMoveOneTimeStruct move)
                {
                    WrappedCard card = move.Reason.Card;
                    if (card.Name == SupplyShortage.ClassName || card.Name == Indulgence.ClassName)
                    {
                        player.SetFlags("-target_confirming");
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

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> cards, int min, int max, bool option)
        {
            if (!ai.WillShowForAttack()) return new List<int>();

            Room room = ai.Room;
            Player target = room.Current;
            List<int> ids = new List<int>();
            
            foreach (int id in player.GetCards("h"))
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
                    if (ai.IsLackCard(target, Slash.ClassName) || target.HandcardNum + target.GetPile("wooden_ox").Count < 3)
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
}