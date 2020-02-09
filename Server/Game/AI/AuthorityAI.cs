using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class AuthorityAI : AIPackage
    {
        public AuthorityAI() : base("Authority")
        {
            events = new List<SkillEvent>
            {
                new ImperialOrderAI(),
                new BeltsheChaoAI(),
                new SurrenderAI(),
                new PayTributeAI(),
                new StopFightingAI(),
                new ChangeGeneralAI(),

                new JieyueAI(),
                new ZhengpiAI(),
                new FengyingAI(),
                new FudiAI(),
                new CongjianAI(),
                new WeidiAI(),
                new JianglueAI(),
                new EnyuanEAI(),
                new EnyuanYAI(),
                new XuanhuoAI(),
                new GanluAI(),
                new BuyiAI(),
                new KeshouAI(),
                new ZhuweiAI(),
                new ZhuweiMaxAI(),
                new HuibianAI(),
                new ZongyuAI(),
                new JiananAI(),

                new WushengFZAI(),
                new PaoxiaoFZAI(),
                new LongdanFZAI(),
                new TieqiFZAI(),
                new LiegongFZAI(),
                new KuangguFZAI(),
            };

            use_cards = new List<UseCard>
            {
                new FengyingCardAI(),
                new WeidiCardAI(),
                new JianglueCardAI(),
                new XuanhuoCardAI(),
                new GanluCardAI(),
                new HuibianCardAI(),
            };
        }
    }

    public class ImperialOrderAI : SkillEvent
    {
        public ImperialOrderAI() : base("imperialorder") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            List<WrappedCard> cards = ImperialOrderVS.GetImperialOrders(room, player);
            if (prompt == "@jianglue")
            {
                WrappedCard card = cards.Find(t => t.Name == ChangeGeneral.ClassName);
                if (card != null)
                {
                    use.Card = card;
                    return use;
                }
                card = cards.Find(t => t.Name == PayTribute.ClassName);
                if (card != null)
                {
                    use.Card = card;
                    return use;
                }
                card = cards.Find(t => t.Name == BeltsheChao.ClassName);
                if (card != null)
                {
                    use.Card = card;
                    return use;
                }
                card = cards.Find(t => t.Name == Surrender.ClassName);
                if (card != null)
                {
                    use.Card = card;
                    return use;
                }
            }
            else
            {
                Player target = room.FindPlayer(cards[0].UserString);
                if (ai.IsEnemy(target))
                {
                    if (player.GetCards("he").Count > player.MaxHp)
                    {
                        WrappedCard surrender = cards.Find(t => t.Name == Surrender.ClassName);
                        if (surrender != null)
                        {
                            use.Card = surrender;
                            return use;
                        }
                    }

                    if (target.HandcardNum > 2)
                    {
                        WrappedCard change = cards.Find(t => t.Name == ChangeGeneral.ClassName);
                        if (change != null)
                        {
                            use.Card = change;
                            return use;
                        }
                    }
                    WrappedCard card = cards.Find(t => t.Name == PayTribute.ClassName);
                    if (card != null)
                    {
                        use.Card = card;
                        return use;
                    }

                    card = cards.Find(t => t.Name == StopFighting.ClassName);
                    if (card != null)
                    {
                        use.Card = card;
                        return use;
                    }
                }
            }

            use.Card = cards[0];
            return use;
        }
    }

    public class BeltsheChaoAI : SkillEvent
    {
        public BeltsheChaoAI() : base(BeltsheChao.ClassName) { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is Player from)
            {
                string reason = (string)from.GetTag("order_reason");
                if (ai.IsFriend(from))
                {
                    if (reason == "jianglue") return "accept";
                }
                else
                {
                    if (ai.IsWeak() && reason == "weidi" && player.HandcardNum <= 2)
                        return "cancel";

                    return "accept";
                }
            }

            return "cancel";
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            if (room.GetTag("belt_killer") is Player killer)
            {
                List<Player> enemies = ai.GetEnemies(player);
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                    if (target.Contains(p)) return new List<Player> { p };
            }

            ai.SortByDefense(ref target);
            return new List<Player> { target[0] };
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            string target_name = prompt.Split(':')[1];
            Room room = ai.Room;
            Player target = room.FindPlayer(target_name);
            if (target != null)
            {
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

    public class SurrenderAI : SkillEvent
    {
        public SurrenderAI() : base("surrender") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            if (Accept(ai, player))
            {
                use.Card = new WrappedCard(SurrenderCard.ClassName);
                List<int> ids = player.GetEquips();
                ai.SortByKeepValue(ref ids);
                ids.Add(ids[0]);
            }

            return use;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return Accept(ai, player) ? "throw" : "cancel";
        }

        private bool Accept(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            if (room.GetTag(Surrender.ClassName) is Player from)
            {
                string reason = (string)from.GetTag("order_reason");
                if (reason == "jianglue")
                    return true;

                if (ai.IsEnemy(from))
                {
                    int count = Math.Max(0, player.GetEquips().Count - 1);
                    if (count > 0 && ai.HasSkill(TrustedAI.LoseEquipSkill))
                        count--;
                    count += player.HandcardNum;
                    if (count <= player.MaxHp) return true;
                }
            }

            return false;
        }
    }

    public class PayTributeAI : SkillEvent
    {
        public PayTributeAI() : base(PayTribute.ClassName){}

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            List<int> result = new List<int>();
            if (room.GetTag(Name) is Player from)
            {
                string reason = (string)from.GetTag("order_reason");
                if (reason != "buyi" || ai.IsEnemy(from))
                {
                    foreach (int id in player.GetEquips())
                    {
                        if (ai.GetKeepValue(id, player) < 0)
                        {
                            result.Add(id);
                            break;
                        }
                    }
                    if (ai.IsEnemy(from) && result.Count > 0)
                        return result;
                    else if (ai.IsFriend(from))
                    {
                        List<int> ids = player.GetCards("he");
                        ids.RemoveAll(t => result.Contains(t));
                        if (player.HasTreasure(WoodenOx.ClassName) && player.GetPile("wooden_ox").Count > 0)
                            ids.Remove(player.Treasure.Key);

                        KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { from });
                        if (pair.Key == from && pair.Value >= 0)
                            result.Add(pair.Value);

                        ids.RemoveAll(t => result.Contains(t));
                        if (result.Count < 2 && ids.Count > 0)
                        {
                            ai.SortByKeepValue(ref ids, false);
                            result.Add(ids[0]);
                        }
                    }
                    else
                    {
                        List<int> ids = player.GetCards("he");
                        ai.SortByKeepValue(ref ids, false);
                        result.Add(ids[0]);
                    }
                }
            }

            return result;
        }
    }

    public class StopFightingAI : SkillEvent
    {
        public StopFightingAI() : base(StopFighting.ClassName) { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is Player from)
            {
                string reason = (string)from.GetTag("order_reason");
                if (reason == "jianglue")
                    return "skillinvalid";

                if (ai.HasSkill(TrustedAI.MasochismSkill) && ai.IsWeak(player))
                    return "cancel";
            }

            return "skillinvalid";
        }
    }

    public class ChangeGeneralAI : SkillEvent
    {
        public ChangeGeneralAI() : base(ChangeGeneral.ClassName) { }
        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is Player from)
            {
                string reason = (string)from.GetTag("order_reason");
                if (reason == "jianglue")
                    return "accept";
                if (ai.IsFriend(from) && reason == "buyi")
                    return "cancel";
                if (reason == "weidi" && player.HandcardNum <= 2)
                    return "cancel";
                if (player.Phase == Player.PlayerPhase.NotActive && ai.IsWeak() && player.HandcardNum > player.Hp)
                    return "cancel";
            }

            return "accept";
        }
    }

    public class JieyueAI : SkillEvent
    {
        public JieyueAI() : base("jieyue")
        {
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                To = new List<Player>(),
                From = player
            };

            if (!ai.WillShowForAttack()) return use;

            List<int> ids = player.GetCards("h");
            int id = -1;
            ai.SortByUseValue(ref ids, false);
            if (ai.GetUseValue(ids[0], player) < 5)
                id = ids[0];

            if (id >= 0)
            {
                List<Player> targets = ai.GetPrioEnemies();
                if (targets.Count > 0)
                {
                    ai.SortByDefense(ref targets, false);
                    foreach (Player p in targets)
                    {
                        if (!p.HasShownOneGeneral() || p.Kingdom != "wei")
                        {
                            use.Card = new WrappedCard("JieyueCard");
                            use.Card.AddSubCard(id);
                            use.To.Add(p);
                            return use;
                        }
                    }
                }

                targets = ai.GetEnemies(player);
                ai.SortByDefense(ref targets, false);
                foreach (Player p in targets)
                {
                    if (!p.HasShownOneGeneral() || p.Kingdom != "wei")
                    {
                        use.Card = new WrappedCard("JieyueCard");
                        use.Card.AddSubCard(id);
                        use.To.Add(p);
                        return use;
                    }
                }
            }

            return use;
        }
    }
    
    public class ZhengpiAI : SkillEvent
    {
        public ZhengpiAI() : base("zhengpi")
        { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };

            if (player.HasFlag(Name))
            {

            }
            else
            {
                if (!ai.WillShowForAttack()) return use;

                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (Engine.GetFunctionCard(card.Name) is BasicCard)
                        ids.Add(id);
                }

                int sub = -1;
                if (ids.Count > 0)
                {
                    foreach (Player p in ai.FriendNoSelf)
                    {
                        if (p.HasShownOneGeneral() && p.HasEquip())
                        {
                            foreach (int id in p.GetEquips())
                            {
                                if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                {
                                    use.Card = new WrappedCard(ZhengpiCard.ClassName) { Skill = Name };
                                    use.To.Add(p);
                                    KeyValuePair<Player, int> result = ai.GetCardNeedPlayer(ids, new List<Player> { p });
                                    if (result.Key == p && ids.Contains(result.Value))
                                    {
                                        use.Card.AddSubCard(result.Value);
                                        return use;
                                    }

                                    ai.SortByUseValue(ref ids, false);
                                    use.Card.AddSubCard(ids[0]);
                                    return use;
                                }
                            }
                        }
                    }
                    ai.SortByUseValue(ref ids, false);
                    if (ai.GetUseValue(ids[0], player) < 4)
                        sub = ids[0];
                }

                foreach (Player p in ai.GetPrioEnemies())
                {
                    if (!p.HasShownOneGeneral())
                    {
                        use.Card = new WrappedCard(ZhengpiCard.ClassName) { Skill = Name };
                        use.To.Add(p);
                        return use;
                    }
                }

                if (sub >= 0)
                {
                    List<Player> targets = ai.GetPrioEnemies();
                    ai.SortByDefense(ref targets, false);
                    foreach (Player p in targets)
                    {
                        if (p.IsNude()) continue;
                        bool check = true;
                        foreach (int id in p.GetEquips())
                        {
                            if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check)
                        {
                            use.Card = new WrappedCard(ZhengpiCard.ClassName) { Skill = Name };
                            use.Card.AddSubCard(sub);
                            use.To.Add(p);
                            return use;
                        }
                    }

                    foreach (Player p in ai.GetEnemies(player))
                    {
                        if (!p.HasShownOneGeneral())
                        {
                            use.Card = new WrappedCard(ZhengpiCard.ClassName) { Skill = Name };
                            use.To.Add(p);
                            return use;
                        }
                    }
                }
            }

            return use;
        }
    }

    public class FengyingAI : SkillEvent
    {
        public FengyingAI() : base("fengying")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (player.GetMark("@lord") == 0 || player.IsKongcheng() || !ai.IsSituationClear()) return result;

            Room room = ai.Room;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodUse))
                    return result;

                CardUseStruct use = new CardUseStruct(null, player, new List<Player>())
                {
                    IsDummy = true
                };
                UseCard e = Engine.GetCardUsage(card.Name);
                if (e != null) e.Use(ai, player, ref use, card);
                if (use.Card != null) return result;
            }

            int count = 0;
            double draw = 0;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.IsFriendWith(room, p, player))
                {
                    count++;
                    draw += Math.Max(p.MaxHp - p.HandcardNum, 0);
                }
            }

            if (draw / count >= 2 || draw > 5)
            {
                WrappedCard card = new WrappedCard("FengyingCard")
                {
                    Skill = Name,
                    ShowSkill = Name,
                };
                result.Add(card);
            }

            return result;
        }
    }

    public class FengyingCardAI : UseCard
    {
        public FengyingCardAI() : base("FengyingCard") { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class FudiAI : SkillEvent
    {
        public FudiAI() : base("fudi") { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            ai.Target[Name] = null;
            if (ai.WillShowForAttack() && room.ContainsTag(Name) && room.GetTag(Name) is DamageStruct damage && ai.IsEnemy(damage.From))
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (RoomLogic.IsFriendWith(room, p, damage.From) && p.Hp >= player.Hp)
                        targets.Add(p);
                }

                if (targets.Count > 0)
                {
                    double best = 0;
                    Player target = null;
                    List<int> ids = player.GetCards("h");
                    ai.SortByKeepValue(ref ids, false);
                    double value = ai.GetKeepValue(ids[0], player);
                    ai.SortByDefense(ref targets, false);
                    foreach (Player p in targets)
                    {
                        DamageStruct _damage = new DamageStruct(Name, player, p);
                        ScoreStruct score = ai.GetDamageScore(_damage, DamageStruct.DamageStep.Caused);
                        if (score.Score > value && score.Score > best)
                        {
                            best = score.Score;
                            target = p;
                        }
                    }

                    if (target != null)
                    {
                        ai.Target[Name] = target;
                        return new List<int> { ids[0] };
                    }
                }
            }

            return new List<int>();
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (ai.Target[Name] != null && target.Contains(ai.Target[Name]))
                return new List<Player> { ai.Target[Name] };

            return new List<Player>();
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct();
            Room room = ai.Room;
            if (damage.From != null && damage.From.Alive && ai.HasSkill(Name, damage.To) && ai.IsEnemy(damage.To)
                && !damage.To.IsKongcheng())
            {
                int count = damage.To.Phase == Player.PlayerPhase.NotActive ? 2 : 1;
                double s = 1;
                if (damage.Damage >= damage.To.Hp)
                {
                    s = 1.5;
                    for (int i = 0; i < damage.Damage - damage.To.Hp; i++)
                        s += 1;
                }
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (RoomLogic.IsFriendWith(room, p, damage.From) && ai.IsFriend(p) && p.Hp >= damage.To.Hp && !ai.CanResist(p, count))
                    {
                        score.Score -= count * 4.5;
                        score.Score /= s;
                        return score;
                    }
                }
            }

            if (damage.From != null && ai.IsFriend(damage.From) && ai.HasSkill(Name, damage.From) && room.Current == damage.From
                && ai.IsEnemy(damage.To) && ai.HasSkill("ganglie", damage.To))
            {
                score.Score -= 4;
            }

            return score;
        }
    }

    public class CongjianAI :SkillEvent
    {
        public CongjianAI() : base("congjian") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.Phase != Player.PlayerPhase.NotActive)
                return false;

            if (data is DamageStruct damage && ai.IsFriend(damage.To))
                return false;

            return true;
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (step <= DamageStruct.DamageStep.Caused && damage.From != null && damage.From.Alive && ai.HasSkill(Name, damage.From) && damage.From.Phase == Player.PlayerPhase.NotActive)
                damage.Damage++;

            if (step != DamageStruct.DamageStep.Caused && step <= DamageStruct.DamageStep.Done && ai.HasSkill(Name, damage.To) && ai.Room.Current == damage.To)
                damage.Damage++;
        }
    }

    public class WeidiAI : SkillEvent
    {
        public WeidiAI() : base("weidi") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return base.GetTurnUse(ai, player);
        }
    }

    public class WeidiCardAI : UseCard
    {
        public WeidiCardAI() : base("WeidiCard") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
        }
    }

    public class JianglueAI : SkillEvent
    {
        public JianglueAI() : base("jianglue") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (player.GetMark("@jiang") > 0)
            {
                WrappedCard card = new WrappedCard("JianglueCard")
                {
                    Skill = Name,
                    ShowSkill = Name,
                    Mute = true
                };
                result.Add(card);
            }

            return result;
        }
    }

    public class JianglueCardAI : UseCard
    {
        public JianglueCardAI() : base("JianglueCard") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 7;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class EnyuanEAI : SkillEvent
    {
        public EnyuanEAI() : base("enyuan_en") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
                return ai.IsFriend(target);

            return true;
        }
    }

    public class EnyuanYAI : SkillEvent
    {
        public EnyuanYAI() : base("enyuan_yuan")
        {}

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
                return !ai.IsFriend(target);

            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("h");
            ai.SortByKeepValue(ref ids, false);
            foreach (int id in ids)
                if (!ai.IsCard(id, Peach.ClassName, player) && !ai.IsCard(id, Analeptic.ClassName, player))
                    return new List<int> { id };

            return new List<int>();
        }
    }

    public class XuanhuoAI : SkillEvent
    {
        public XuanhuoAI() : base("xuanhuovs"){}

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (!player.HasUsed("XuanhuoCard") && ai.GetOverflow(player) >= 2)
            {
                List<Player> fazhengs = RoomLogic.FindPlayersBySkillName(ai.Room, "xuanhuo");
                foreach (Player p in fazhengs)
                {
                    if (RoomLogic.PlayerHasShownSkill(ai.Room, p, "xuanhuo") && RoomLogic.IsFriendWith(ai.Room, player, p))
                    {
                        WrappedCard card = new WrappedCard("XuanhuoCard");
                        result.Add(card);
                        break;
                    }
                }
            }

            return result;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (!string.IsNullOrEmpty(ai.Choice[Name]) && choice.Contains(ai.Choice[Name]))
                return ai.Choice[Name];

            if (choice.Contains("paoxiao_fz")) return "paoxiao_fz";
            if (choice.Contains("tieqi_fz")) return "tieqi_fz";
            if (choice.Contains("kuanggu_fz")) return "kuanggu_fz";
            if (choice.Contains("longdan_fz")) return "longdan_fz";
            return string.Empty;
        }
        public override int OnPickAG(TrustedAI ai, Player player, List<int> card_ids, bool refusable)
        {
            Player target = null;
            List<Player> fazhengs = RoomLogic.FindPlayersBySkillName(ai.Room, "xuanhuo");
            foreach (Player p in fazhengs)
            {
                if (RoomLogic.PlayerHasShownSkill(ai.Room, p, "xuanhuo") && RoomLogic.IsFriendWith(ai.Room, player, p))
                {
                    target = p;
                    break;
                }
            }

            int result = -1;
            double best = 0;
            foreach (int id in card_ids)
            {
                double value = ai.GetKeepValue(id, target, Player.Place.PlaceHand);
                if (value > best)
                {
                    best = value;
                    result = id;
                }
            }
            return result;
        }
    }

    public class XuanhuoCardAI : UseCard
    {
        public XuanhuoCardAI() : base("XuanhuoCard") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5.5;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Choice["xuanhuovs"] = null;
            Room room = ai.Room;
            List<string> skills = new List<string> { "wusheng_fz", "paoxiao_fz", "tieqi_fz", "longdan_fz", "liegong_fz", "kuanggu_fz" };
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.PlayerHasShownSkill(room, p, "wusheng|wusheng_fz"))
                    skills.Remove("wusheng_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "paoxiao|paoxiao_fz"))
                    skills.Remove("paoxiao_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "tieqi|tieqi_fz"))
                    skills.Remove("tieqi_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "longdan|longdan_fz"))
                    skills.Remove("longdan_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "liegong|liegong_fz"))
                    skills.Remove("liegong_fz");
                if (RoomLogic.PlayerHasShownSkill(room, p, "kuanggu|kuanggu_fz"))
                    skills.Remove("kuanggu_fz");
            }
            
            if (!ai.HasCrossbowEffect(player) && skills.Contains("paoxiao_fz")) ai.Choice["xuanhuovs"] = "paoxiao_fz";
            if (ai.HasCrossbowEffect(player))
            {
                if (!ai.HasSkill("wusheng") && skills.Contains("wusheng_fz"))
                    ai.Choice["xuanhuovs"] = "wusheng_fz";
                else if (!ai.HasSkill("kuanggu") && skills.Contains("kuanggu_fz"))
                    ai.Choice["xuanhuovs"] = "kuanggu_fz";
            }

            List<int> ids = player.GetCards("h");
            ai.SortByUseValue(ref ids, false);
            card.AddSubCard(ids[0]);
            card.AddSubCard(ids[1]);
            use.Card = card;
        }
    }
    public class WushengFZAI : SkillEvent
    {
        public WushengFZAI() : base("wusheng_fz")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = player.GetCards("he");
            ids.AddRange(player.GetHandPile()); Player lord = RoomLogic.GetLord(room, player.Kingdom);
            bool any = true;
            if (lord == null || !RoomLogic.PlayerHasSkill(room, lord, "shouyue") || !lord.General1Showed)
            {
                any = false;
            }

            foreach (int id in ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if (any || WrappedCard.IsRed(card.Suit))
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return new List<WrappedCard> { slash };
                }
            }
            return null;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (targets.Count > 0) return 0;
            return -1;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            Player lord = RoomLogic.GetLord(room, player.Kingdom);
            bool any = true;
            if (lord == null || !RoomLogic.PlayerHasSkill(room, lord, "shouyue") || !lord.General1Showed)
            {
                any = false;
            }
            if (any || WrappedCard.IsRed(card.Suit))
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                slash.AddSubCard(card);
                slash = RoomLogic.ParseUseCard(room, slash);
                return slash;
            }

            return null;
        }
    }

    public class PaoxiaoFZAI : SkillEvent
    {
        public PaoxiaoFZAI() : base("paoxiao_fz")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class LongdanFZAI : SkillEvent
    {
        public LongdanFZAI() : base("longdan_fz")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = player.GetCards("h");
            ids.AddRange(player.GetHandPile());

            foreach (int id in ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if (card.Name == Jink.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return new List<WrappedCard> { slash };
                }
            }
            return null;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            Room room = ai.Room;
            WrappedCard card = room.GetCard(id);
            if (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand)
            {
                if (card.Name == Jink.ClassName)
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    slash.AddSubCard(card);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    return slash;
                }
                else if (card.Name.Contains(Slash.ClassName))
                {
                    WrappedCard jink = new WrappedCard(Jink.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    jink.AddSubCard(card);
                    jink = RoomLogic.ParseUseCard(room, jink);
                    return jink;
                }
            }

            return null;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            double value = 0;
            if (card.Name.Contains(Slash.ClassName) && ai.HasSkill(Name, to) && to.HandcardNum + to.GetPile("wooden_ox").Count >= 3 && !ai.IsLackCard(to, Jink.ClassName))
            {
                foreach (Player p in ai.Room.GetOtherPlayers(ai.Self))
                {
                    if (p == to) continue;
                    if (ai.IsFriend(p, to) && p.IsWounded())
                    {
                        value += ai.IsFriend(p) ? 3 : -3;
                        break;
                    }
                }
            }

            return value;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (player.ContainsTag(Name) && player.GetTag(Name) is string user_name)
            {
                Room room = ai.Room;
                Player user = room.FindPlayer(user_name);
                if (user != null)
                {
                    if (user == player)
                    {
                        double value = 0;
                        Player tar = null;
                        foreach (Player p in target)
                        {
                            DamageStruct damage = new DamageStruct(Name, player, p);
                            ScoreStruct score = ai.GetDamageScore(damage);
                            if (score.Score > value)
                            {
                                value = score.Score;
                                tar = p;
                            }
                        }
                        if (tar != null)
                            return new List<Player> { tar };
                    }
                    else
                    {
                        ai.SortByDefense(ref target, false);
                        foreach (Player p in target)
                            if (ai.IsFriend(p))
                                return new List<Player> { p };
                    }
                }
            }

            return new List<Player>();
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            Room room = ai.Room;
            if (targets.Count == 0)
            {
                Player lord = ai.FindPlayerBySkill("shouyue");
                if (lord != null && (lord == player || lord.General1Showed))
                    return 1.2;
            }

            if (card.Name.Contains(Slash.ClassName) && targets.Count > 0)
            {
                List<Player> enemies = ai.GetEnemies(player);
                if (enemies.Count > 1)
                {
                    foreach (Player p in targets)
                        if (!ai.IsFriend(p) && !ai.IsCancelTarget(card, p, player) && ai.IsCardEffect(card, p, player))
                            return 1;
                }
            }
            else if (card.Name == Jink.ClassName)
            {
                if (ai.Room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
                    && ai.Room.GetRoomState().GetCurrentCardUsePattern() == Jink.ClassName)
                {
                    foreach (Player p in ai.FriendNoSelf)
                        if (p.IsWounded())
                            return 1.5;
                }
            }

            return 0;
        }
    }

    public class TieqiFZAI : SkillEvent
    {
        public TieqiFZAI() : base("tieqi_fz")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player p)
                return ai.IsEnemy(p);

            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            string[] choices = choice.Split('+');
            Room room = ai.Room;
            if (data is Player p)
            {
                if (p.GetMark("@tieqi1") > 0)
                    return choices[1];
                else if (p.GetMark("@tieqi2") > 0)
                    return choices[0];
                else
                {
                    General general1 = Engine.GetGeneral(p.General1, room.Setting.GameMode);
                    if (general1.HasSkill(TrustedAI.MasochismSkill, room.Setting.GameMode, true))
                        return choices[0];

                    General general2 = Engine.GetGeneral(p.General2, room.Setting.GameMode);
                    if (general2.HasSkill(TrustedAI.MasochismSkill, room.Setting.GameMode, false))
                        return choices[0];

                    if (general1.HasSkill(TrustedAI.DefenseSkill, room.Setting.GameMode, true))
                        return choices[0];

                    if (general2.HasSkill(TrustedAI.DefenseSkill, room.Setting.GameMode, false))
                        return choices[0];
                }
            }

            return choices[0];
        }
    }
    public class LiegongFZAI : SkillEvent
    {
        public LiegongFZAI() : base("liegong_fz")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player p)
            {
                return ai.IsEnemy(p);
            }

            return true;
        }
    }
    public class KuangguFZAI : SkillEvent
    {
        public KuangguFZAI() : base("kuanggu_fz")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (ai.HasCrossbowEffect(player))
            {
                foreach (Player p in ai.GetEnemies(player))
                    if (RoomLogic.InMyAttackRange(ai.Room, player, p))
                        return "draw";
            }

            return "recover";
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            Player from = damage.From;
            Player to = damage.To;
            Room room = ai.Room;
            ScoreStruct score = new ScoreStruct();
            if (from != null && ai.HasSkill(Name, from) && (to == from && from.Hp > damage.Damage || RoomLogic.DistanceTo(room, from, to) == 1))
            {
                double value = 0;
                if (to == from)
                    value = damage.Damage * 4;
                else
                {
                    int heal = Math.Min(from.GetLostHp(), damage.Damage);
                    value = heal * 4 + (damage.Damage - heal) * 1.5;
                }
                if (ai.IsFriend(from))
                    score.Score = value;
                else if (ai.IsEnemy(from))
                    score.Score = -value;
            }

            return score;
        }
    }

    public class GanluAI : SkillEvent
    {
        public GanluAI() : base("ganlu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if ((ai.WillShowForAttack() || ai.WillShowForDefence()) && !player.HasUsed("GanluCard"))
            {
                WrappedCard card = new WrappedCard("GanluCard")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                result.Add(card);
            }


            return result;
        }
    }

    public class GanluCardAI : UseCard
    {
        public GanluCardAI() : base("GanluCard") { }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 7.5;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            int count = player.GetLostHp();
            Room room = ai.Room;

            if (count > 0)
            {
                double best = 0;
                List<Player> targets = null;
                foreach (Player friend in ai.GetFriends(player))
                {
                    int f_count = friend.GetEquips().Count;
                    foreach (Player enemy in ai.GetEnemies(player))
                    {
                        int e_count = enemy.GetEquips().Count;
                        int dif = f_count - e_count;
                        if (dif > 0 && dif <= count)
                        {
                            double value = dif * 2;
                            if (ai.HasSkill(TrustedAI.LoseEquipSkill, friend))
                            {
                                value++;
                                if (f_count > 0) value += 0.5;
                            }
                            if (ai.HasSkill(TrustedAI.LoseEquipSkill, enemy))
                            {
                                value -= 0.5; ;
                                if (f_count > 0) value--;
                            }
                            if (friend.GetTreasure())
                                value--;
                            if (enemy.GetTreasure())
                                value++;
                            foreach (int id in friend.GetEquips())
                            {
                                if (ai.GetKeepValue(id, friend) < 0)
                                {
                                    value++;
                                    break;
                                }
                            }
                            foreach (int id in enemy.GetEquips())
                            {
                                if (ai.GetKeepValue(id, friend) < 0)
                                {
                                    value--;
                                    break;
                                }
                            }

                            if (value > best)
                            {
                                best = value;
                                targets = new List<Player> { friend, enemy };
                            }
                        }
                    }
                }
                if (targets != null && targets.Count == 2)
                {
                    use.Card = card;
                    use.To = targets;
                    return;
                }
            }

            double _best = 0;
            List<Player> _targets = null;
            foreach (Player friend in ai.GetFriends(player))
            {
                int f_count = friend.GetEquips().Count;
                foreach (Player enemy in ai.GetFriends(player))
                {
                    if (enemy == friend) continue;
                    int e_count = enemy.GetEquips().Count;
                    int dif = f_count - e_count;
                    if (Math.Abs(dif) <= count)
                    {
                        double value = 0;
                        if (ai.HasSkill(TrustedAI.LoseEquipSkill, friend) && e_count > 0)
                            value++;

                        if (ai.HasSkill(TrustedAI.LoseEquipSkill, enemy) && f_count > 0)
                            value++;

                        foreach (int id in friend.GetEquips())
                        {
                            if (ai.GetKeepValue(id, friend) < 0)
                            {
                                value++;
                                break;
                            }
                        }
                        foreach (int id in enemy.GetEquips())
                        {
                            if (ai.GetKeepValue(id, friend) < 0)
                            {
                                value++;
                                break;
                            }
                        }

                        if (value > _best)
                        {
                            _best = value;
                            _targets = new List<Player> { friend, enemy };
                        }
                    }
                }
            }
            if (_targets != null && _targets.Count == 2)
            {
                use.Card = card;
                use.To = _targets;
            }
        }
    }

    public class BuyiAI : SkillEvent
    {
        public BuyiAI() : base("buyi") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class KeshouAI : SkillEvent
    {
        public KeshouAI() : base("keshou") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };
            if (!ai.WillShowForDefence()) return use;

            Room room = ai.Room;

            DamageStruct damage = (DamageStruct)room.GetTag("keshou_data");
            if ((damage.Nature != DamageStruct.DamageNature.Normal && RoomLogic.HasArmorEffect(room, player, PeaceSpell.ClassName))
                || (damage.Damage >= player.Hp && RoomLogic.HasArmorEffect(room, player, BreastPlate.ClassName)))
                return use;

            double bad = 4 * damage.Damage;
            if (ai.IsWeak()) bad *= 1.5;
            if (damage.Damage <= player.Hp) bad += 4;

            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);

            if (ids.Count > 1)
            {
                ai.SortByKeepValue(ref ids, false);
                for (int i = 0; i < ids.Count; i++)
                {
                    double value = ai.GetKeepValue(ids[i], player);
                    if (ai.IsCard(ids[i], Peach.ClassName, player) || (damage.Damage >= player.Hp && ai.IsCard(ids[i], Analeptic.ClassName, player))) continue;
                    bool include = ai.HasSkill(TrustedAI.LoseEquipSkill) && room.GetCardPlace(ids[i]) == Player.Place.PlaceEquip;
                    for (int j = i + 1; j < ids.Count; j++)
                    {

                        if (WrappedCard.IsBlack(room.GetCard(ids[i]).Suit) == WrappedCard.IsBlack(room.GetCard(ids[j]).Suit)
                            && !(ai.IsCard(ids[j], Peach.ClassName, player) || (damage.Damage >= player.Hp && ai.IsCard(ids[j], Analeptic.ClassName, player))))
                        {
                            value += ai.GetKeepValue(ids[i], player, room.GetCardPlace(ids[i]), include);
                            if (value < bad)
                            {
                                use.Card = new WrappedCard("KeshouCard");
                                use.Card.AddSubCard(ids[i]);
                                use.Card.AddSubCard(ids[j]);
                                return use;
                            }
                        }
                    }
                }
            }

            return use;
        }
    }

    public class ZhuweiAI : SkillEvent
    {
        public ZhuweiAI() : base("zhuwei") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
    }

    public class ZhuweiMaxAI : SkillEvent
    {
        public ZhuweiMaxAI() : base("zhuwei_max") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is string prompt)
            {
                string target_name = prompt.Split(':')[1];
                Player target = ai.Room.FindPlayer(target_name);
                return target != null && ai.IsFriend(target);
            }

            return false;
        }
    }

    public class HuibianAI : SkillEvent
    {
        public HuibianAI() : base("huibian") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(HuibianCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(HuibianCard.ClassName) { Skill = Name, ShowSkill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class HuibianCardAI : UseCard
    {
        public HuibianCardAI() : base("HuibianCard") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> wounds = new List<Player>(), all = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.IsFriendWith(room, player, p))
                {
                    all.Add(p);
                    if (p.IsWounded())
                        wounds.Add(p);
                }
            }

            if (all.Count > 1 && wounds.Count > 0)
            {
                Player wounded = null;
                ai.SortByDefense(ref wounds, false);
                foreach (Player p in wounds)
                {
                    if (!p.Removed && ai.HasSkill(TrustedAI.MasochismSkill, p))
                        {
                        wounded = p;
                        break;
                    }
                }

                if (wounded == null)
                {
                    foreach (Player p in wounds)
                    {
                        if (!p.Removed)
                        {
                            wounded = p;
                            break;
                        }
                    }
                }
                if (wounded == null) wounded = wounds[0];

                Dictionary<Player, double> points = new Dictionary<Player, double>();
                foreach (Player p in all)
                {
                    if (wounded == p) continue;
                    DamageStruct damage = new DamageStruct(Name, player, p);
                    points[p] = ai.GetDamageScore(damage).Score;
                }

                List<Player> hurts = new List<Player>(points.Keys);
                hurts.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                if (points[hurts[0]] > -6)
                {
                    use.Card = card;
                    use.To.Add(hurts[0]);
                    use.To.Add(wounded);
                }
            }
        }
    }

    public class ZongyuAI : SkillEvent
    {
        public ZongyuAI() : base("zongyu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                return !ai.IsFriend(move.To);
            }
            else
            {
                foreach (Player p in ai.Room.GetOtherPlayers(player))
                    if (p.GetSpecialEquip())
                        return !ai.IsFriend(p);

                return true;
            }
        }
    }

    public class JiananAI : SkillEvent
    {
        public JiananAI() : base("jianan") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return base.OnSkillInvoke(ai, player, data);
        }
        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> ids, int min, int max, bool option)
        {
            return base.OnDiscard(ai, player, ids, min, max, option);
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return base.OnChoice(ai, player, choice, data);
        }
    }
}