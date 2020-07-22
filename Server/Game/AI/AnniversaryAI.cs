using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class AnniversaryAI : AIPackage
    {
        public AnniversaryAI() : base("Anniversary")
        {
            events = new List<SkillEvent>
            {
                new YanjiaoAI(),
                new XingshenAI(),
                new AndongAI(),
                new YingshiAI(),
                new SanwenAI(),
                new QiaiAI(),
                new DenglouAI(),
                new LvliAI(),
                new QingjiaoAI(),
                new ZhenxingAI(),
                new WeiluAI(),
                new ZengdaoAI(),
                new YinjuAI(),
                new QianxinZGAI(),
                new ZhuilieAI(),

                new TunanAI(),

                new GuolunAI(),
                new SongSangAI(),
                new QinguoAI(),
                new YoudiAI(),
                new DuanfaAI(),
                new GongqingAI(),
                new BiaozhaoAI(),
                new YechouAI(),
                new ZhafuAI(),
                new FuhaiAI(),
                new SongshuAI(),
                new SibianAI(),

                new KannanAI(),
                new JiedaoAI(),
                new JixuAI(),
                new KuizhuLSAI(),
                new FenyueAI(),
                new XuheAI()
            };

            use_cards = new List<UseCard>
            {
                new GuolunCardAI(),
                new YanjiaoCardAI(),
                new DuanfaCardAI(),
                new TunanCardAI(),
                new JixuCardAI(),
                new ZengdaoCardAI(),
                new ZhafuCardAI(),
                new SongshuCardAI(),
                new FenyueCardAI(),
            };
        }
    }

    public class YanjiaoAI : SkillEvent
    {
        public YanjiaoAI() : base("yanjiao")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(YanjiaoCard.ClassName) && ai.FriendNoSelf.Count > 0)
                return new List<WrappedCard> { new WrappedCard(YanjiaoCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> cards, List<int> empty, int min, int max)
        {
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Top = new List<int>(),
                Bottom = new List<int>(),
                Success = false
            };

            Room room = ai.Room;
            Player target = room.Current;
            if (ai.IsEnemy(target) && ai.HasSkill("zishu"))
                return move;

            List<List<int>> tops = new List<List<int>>(), downs = new List<List<int>>();
            int half = cards.Count / 2;
            for (int i = 1; i <= half; i++)
            {
                List<List<int>> top = AI.TrustedAI.GetCombinationList(new List<int>(cards), i);
                foreach (List<int> combine in top)
                {
                    int top_count = 0;
                    foreach (int card_id in combine)
                        top_count += Engine.GetRealCard(card_id).Number;

                    List<int> others = new List<int>(cards);
                    others.RemoveAll(t => combine.Contains(t));
                    for (int i2 = 1; i2 < others.Count; i2++)
                    {
                        List<List<int>> down = AI.TrustedAI.GetCombinationList(new List<int>(others), i2);
                        foreach (List<int> combine2 in down)
                        {
                            int down_count = 0;
                            foreach (int card_id in combine2)
                                down_count += Engine.GetRealCard(card_id).Number;

                            if (top_count == down_count)
                            {
                                tops.Add(combine);
                                downs.Add(combine2);
                            }
                        }
                    }
                }
            }

            if (tops.Count > 0 && downs.Count > 0)
            {
                int best = -1;
                double good = -100;
                bool use = !ai.IsWeak();
                for (int i = 0; i < downs.Count; i++)
                {
                    double value = 0;
                    foreach (int id in downs[i])
                        value += use ? ai.GetUseValue(id, player, Place.PlaceHand) : ai.GetKeepValue(id, player, Place.PlaceHand);

                    if (value > good)
                    {
                        good = value;
                        best = i;
                    }
                }

                if (best >= 0)
                {
                    move.Top = tops[best];
                    move.Bottom = downs[best];
                    move.Success = true;
                }
            }
            /*
            if (tops.Count > 0 && downs.Count > 0)
            {
                for (int i = 0; i < tops.Count; i++)
                {
                    int top_count = 0;
                    List<string> downs_number = new List<string>(), tops_number = new List<string>();
                    foreach (int card_id in tops[i])
                    {
                        int number = Engine.GetRealCard(card_id).Number;
                        top_count += number;
                        tops_number.Add(number.ToString());
                    }

                    int down_count = 0;
                    foreach (int card_id in downs[i])
                    {
                        int number = Engine.GetRealCard(card_id).Number;
                        down_count += number;
                        downs_number.Add(number.ToString());
                    }

                    Debug(string.Format("组合{0} 合计{1}，上：cards:{3} number {2}", i + 1, top_count, string.Join("+", tops_number), string.Join("+", JsonUntity.IntList2StringList(tops[i]))));
                    Debug(string.Format("组合{0} 合计{1}，下：cards:{3} number {2}", i + 1, down_count, string.Join("+", downs_number), string.Join("+", JsonUntity.IntList2StringList(downs[i]))));
                }
            }
            */
            return move;
        }
    }

    public class YanjiaoCardAI : UseCard
    {
        public YanjiaoCardAI() : base(YanjiaoCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                use.Card = card;
                foreach (Player p in friends)
                {
                    if (ai.HasSkill("qingjian", p))
                    {
                        use.To = new List<Player> { p };
                        return;
                    }
                }
                foreach (Player p in friends)
                {
                    if (ai.HasSkill(TrustedAI.CardneedSkill, p))
                    {
                        use.To = new List<Player> { p };
                        return;
                    }
                }
                ai.SortByDefense(ref friends, false);
                foreach (Player p in friends)
                {
                    if (!ai.HasSkill("zishu", p))
                    {
                        use.To = new List<Player> { p };
                        return;
                    }
                }
                use.To = new List<Player> { friends[0] };
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
    }

    public class XingshenAI : SkillEvent
    {
        public XingshenAI() : base("xingshen") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.Damage < damage.To.Hp)
                score.Score += 1.5;

            return score;
        }
    }

    public class AndongAI : SkillEvent
    {
        public AndongAI() : base("andong")
        {
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.From != null && damage.From != damage.To && !ai.IsFriend(damage.To, damage.From)
                && !damage.To.IsKongcheng() && ai.Self == damage.From)
            {
                Room room = ai.Room;
                List<int> ids = new List<int>();
                foreach (int id in ai.Self.GetCards("h"))
                    if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart) ids.Add(id);

                if (damage.Card != null)
                    ids.RemoveAll(t => damage.Card.SubCards.Contains(t));

                double value = 0;
                if (ai.Self.Phase == PlayerPhase.Play)
                {
                    foreach (int id in ids)
                        value += ai.GetUseValue(id, ai.Self);

                }
                else
                {
                    foreach (int id in ids)
                        value += ai.GetKeepValue(id, ai.Self);
                }
                score.Score -= value / 2;
            }

            return score;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is DamageStruct damage)
            {
                Player target = damage.To;
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score > 0)
                    return "view";
                else
                    return "prevent";
            }

            return "view";
        }
    }

    public class YingshiAI : SkillEvent
    {
        public YingshiAI() : base("yingshi")
        {
            key = new List<string> { "playerChosen:hongde" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> enemies = ai.GetPrioEnemies();
            if (ai.GetEnemies(player).Count <= 1 && ai.GetOverflow(player) == 0) return new List<Player>();
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies, false);
                return new List<Player> { enemies[0] };
            }

            return new List<Player>();
        }
    }

    public class SanwenAI : SkillEvent
    {
        public SanwenAI() : base("sanwen") { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            List<string> patterns = new List<string>(pattern.Split('#'));
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                string remove = string.Empty;
                foreach (string p in patterns)
                {
                    if (Engine.MatchExpPattern(room, pattern, player, card))
                    {
                        ids.Add(id);
                        remove = p;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(remove)) patterns.Remove(remove);
            }

            return ids;
        }
    }

    public class QiaiAI : SkillEvent
    {
        public QiaiAI() : base("qiai") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("he");
            ai.SortByKeepValue(ref ids, false);
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag(Name))
                {
                    target = p;
                    break;
                }
            }

            if (ai.IsFriend(target))
            {
                foreach (int id in player.GetCards("h"))
                    if (room.GetCard(id).Name == Analeptic.ClassName) return new List<int> { id };

                foreach (int id in player.GetCards("h"))
                    if (room.GetCard(id).Name == Peach.ClassName) return new List<int> { id };
            }

            return new List<int> { ids[0] };
        }
    }

    public class DenglouAI : SkillEvent
    {
        public DenglouAI() : base("denglou") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<int> ids = player.GetPile("#denglou");

            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name == Analeptic.ClassName && Analeptic.Instance.IsAvailable(room, player, card))
                {
                    use.Card = card;
                    return use;
                }
                else if (card.Name == Peach.ClassName && Peach.Instance.IsAvailable(room, player, card))
                {
                    use.Card = card;
                    return use;
                }
            }

            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name.Contains(Slash.ClassName))
                {
                    List<WrappedCard> slashes = new List<WrappedCard> { card };
                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
                    if (scores.Count > 0 && scores[0].Score > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
                    {
                        use.Card = card;
                        use.To = scores[0].Players;
                        return use;
                    }
                }
            }

            return use;
        }
    }

    public class LvliAI : SkillEvent
    {
        public LvliAI() : base("lvli") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
    }

    public class QingjiaoAI : SkillEvent
    {
        public QingjiaoAI() : base("qingjiao") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => player.Hp <= 2 || player.HandcardNum < 4;
    }

    public class ZhenxingAI : SkillEvent
    {
        public ZhenxingAI() : base("zhenxing") { }
        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.Damage == 1)
                score.Score += ai.IsFriend(damage.To) ? 1.5 : -1.5;
            return score;
        }
    }

    public class WeiluAI : SkillEvent
    {
        public WeiluAI() : base("weilu") { }
        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.Damage == 1 && damage.From != null && damage.From != damage.To && damage.From.Hp > 1 && damage.Damage < damage.To.Hp)
            {
                if (ai.HasSkill("zhaxiang", damage.From))
                {
                    if (ai.IsFriend(damage.From)) score.Score += 5;
                    if (ai.IsEnemy(damage.From)) score.Score -= 5;
                    return score;
                }
                if (ai.IsFriend(damage.From)) score.Score -= 3;
                if (ai.IsEnemy(damage.From)) score.Score += 3;
            }

            return score;
        }
    }

    public class ZengdaoAI : SkillEvent
    {
        public ZengdaoAI() : base("zengdao") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetEquips().Count >= 2 && player.GetMark("@zengdao") > 0)
            {
                WrappedCard card = new WrappedCard(ZengdaoCard.ClassName) { Skill = Name, Mute = true };
                card.AddSubCards(player.GetEquips());
                return new List<WrappedCard> { card };
            }

            return null;
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.From != null && damage.From.Alive && damage.From.GetPile(Name).Count > 0)
                damage.Damage++;
        }
    }

    public class ZengdaoCardAI : UseCard
    {
        public ZengdaoCardAI() : base(ZengdaoCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                Room room = ai.Room;
                room.SortByActionOrder(ref friends);
                foreach (Player p in friends)
                {
                    if (!ai.WillSkipPlayPhase(p) && ai.HasSkill("liegong_jx|tieqi_jx|xueji|kurou_jx|ganglie_jx", p))
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                foreach (Player p in friends)
                {
                    if (!ai.WillSkipPlayPhase(p) && (p.GetWeapon() || p.GetOffensiveHorse()))
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card) => 1.5;
    }

    public class YinjuAI : SkillEvent
    {
        public YinjuAI() : base("yinju") { }
        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (card != null && from != null && to != null && Engine.GetFunctionCard(card.Name).TypeID != CardType.TypeSkill
                && to.HasFlag(string.Format("{0}_{1}", Name, from.Name)))
            {
                if (ai.IsFriend(from)) return 1.5;
                else if (ai.IsEnemy(from)) return -1.5;
            }
            return 0;
        }
    }

    public class YinjuCardAI : UseCard
    {
        public YinjuCardAI() : base(YinjuCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }
    }

    public class QianxinZGAI : SkillEvent
    {
        public QianxinZGAI() : base("qianxin_zg") { }
        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is List<Player> from)
            {
                foreach (Player p in from)
                {
                    if (ai.IsFriend(p)) return "draw";
                }
                foreach (Player p in from)
                {
                    if (ai.IsEnemy(p))
                    {
                        int max = RoomLogic.GetMaxCards(room, player);
                        if (player.HandcardNum - (max - 2) < 4 - p.HandcardNum) return "max";
                    }
                }
            }

            return "draw";
        }
    }

    public class ZhuilieAI : SkillEvent
    {
        public ZhuilieAI() : base("zhuilie") { }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            double value = 0;
            Room room = ai.Room;
            if (card != null && card.Name.Contains(Slash.ClassName) && from != null && ai.HasSkill(Name) && to != null && !RoomLogic.InMyAttackRange(room, from, to, card))
            {
                if (ai.IsFriend(from, to)) return -10;
                if (ai.IsEnemy(from, to)) return 2;
            }
            return value;
        }
    }

    public class TunanAI : SkillEvent
    {
        public TunanAI() : base("tunan") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return !player.HasUsed(TunanCard.ClassName) ? new List<WrappedCard> { new WrappedCard(TunanCard.ClassName) { Skill = Name } } : null;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            int id = player.GetMark(Name);

            if (player.HasFlag(Name))
            {
                WrappedCard card = new WrappedCard(Slash.ClassName) { Skill = "_tunan" };
                card.AddSubCard(id);
                card = RoomLogic.ParseUseCard(room, card);
                List<WrappedCard> slashes = new List<WrappedCard> { card };
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
                if (scores.Count > 0 && scores[0].Score > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
                {
                    use.Card = scores[0].Card;
                    use.To = scores[0].Players;
                }
            }
            else
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name.Contains(Slash.ClassName))
                {
                    List<WrappedCard> slashes = new List<WrappedCard> { card };
                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
                    if (scores.Count > 0 && scores[0].Score > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
                    {
                        use.Card = card;
                        use.To = scores[0].Players;
                        return use;
                    }
                }
                else
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_tunan" };
                    slash.AddSubCard(id);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    List<WrappedCard> slashes = new List<WrappedCard> { slash };
                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
                    if (scores.Count > 0 && scores[0].Score > 5 && scores[0].Players != null && scores[0].Players.Count > 0)
                        return use;

                    UseCard e = Engine.GetCardUsage(card.Name);
                    e.Use(ai, player, ref use, card);
                    if (use.Card == card)
                        return use;
                    else
                        use.Card = null;
                }
            }

            return use;
        }
    }

    public class TunanCardAI : UseCard
    {
        public TunanCardAI() : base(TunanCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                Room room = ai.Room;
                List<ScoreStruct> scores = new List<ScoreStruct>();
                WrappedCard slash = new WrappedCard(Slash.ClassName);

                foreach (Player p in friends)
                {
                    foreach (Player enemy in ai.GetEnemies(player))
                    {
                        if (RoomLogic.InMyAttackRange(room, p, enemy, slash) && RoomLogic.IsProhibited(room, p, enemy, slash) == null)
                        {
                            ScoreStruct score = new ScoreStruct
                            {
                                Players = new List<Player> { p },
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
                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0)
                    {
                        use.To = scores[0].Players;
                        use.Card = card;
                        return;
                    }
                }

                foreach (Player p in friends)
                {
                    if (ai.HasSkill("jiang|jizhi_jx|jizhi", p))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }

                use.Card = card;
                use.To = new List<Player> { friends[0] };
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
    }

    public class GuolunAI : SkillEvent
    {
        public GuolunAI() : base("guolun") { key = new List<string> { "cardExchange:guolun" }; }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && ai.Self != player)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Player target = null;
                    Room room = ai.Room;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }
                    int id = player.GetMark(Name);
                    if (int.TryParse(strs[2], out int give) && ai.GetPlayerTendency(target) != "unknown" && room.GetCard(id).Number < room.GetCard(give).Number)
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed(GuolunCard.ClassName))
            {
                return new List<WrappedCard> { new WrappedCard(GuolunCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            int id = player.GetMark(Name);
            List<int> ids = player.GetCards("h");
            ai.SortByUseValue(ref ids, false);
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag(Name))
                {
                    target = p;
                    break;
                }
            }

            if (ai.IsFriend(target))
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { target });
                if (pair.Key == target && (room.GetCard(pair.Value).Number != room.GetCard(id).Number || ai.HasSkill("zhanji")))
                {
                    return new List<int> { pair.Value };
                }

                double value = ai.GetKeepValue(id, target, Place.PlaceHand);
                foreach (int card in ids)
                {
                    double keep = ai.GetKeepValue(card, target, Place.PlaceHand);
                    if (room.GetCard(card).Number != room.GetCard(id).Number && keep >= value)
                        return new List<int> { card };
                }
                if (ai.HasSkill("zhanji"))
                {
                    foreach (int card in ids)
                    {
                        if (room.GetCard(card).Number != room.GetCard(id).Number)
                            return new List<int> { card };
                    }

                    return new List<int> { ids[0] };
                }
            }
            else
            {
                double value = ai.GetUseValue(id, player, Place.PlaceHand);
                foreach (int card in ids)
                {
                    double keep = ai.GetKeepValue(card, target, Place.PlaceHand);
                    if (room.GetCard(card).Number < room.GetCard(id).Number && keep < value)
                        return new List<int> { card };
                }

                if (ai.HasSkill("zhanji"))
                {
                    foreach (int card in ids)
                    {
                        double keep = ai.GetKeepValue(card, target, Place.PlaceHand);
                        if (room.GetCard(card).Number == room.GetCard(id).Number && keep < value)
                            return new List<int> { card };
                    }
                }
            }

            return new List<int>();
        }
    }

    public class GuolunCardAI : UseCard
    {
        public GuolunCardAI() : base(GuolunCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 2.9;
            List<int> ids = player.GetCards("h");
            if (ids.Count > 0)
            {
                Room room = ai.Room;
                ai.SortByUseValue(ref ids);
                int give = -1;
                foreach (int id in ids)
                {
                    if (ai.GetUseValue(id, player) < 2 && room.GetCard(id).Number < 5)
                    {
                        give = id;
                        break;
                    }
                }

                if (give >= 0)
                {
                    List<Player> enemies = ai.GetEnemies(player);
                    ai.SortByDefense(ref enemies, false);
                    foreach (Player p in enemies)
                    {
                        if (!p.IsKongcheng())
                        {
                            ai.Number[Name] = 9;
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }

                List<Player> friends = ai.FriendNoSelf;
                ai.SortByDefense(ref friends);
                foreach (Player p in friends)
                {
                    if (!p.IsKongcheng())
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }
            }
            foreach (Player p in ai.GetEnemies(player))
            {
                if (!p.IsKongcheng() && ai.GetKnownCards(p).Count < p.HandcardNum)
                {
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class SongSangAI : SkillEvent
    {
        public SongSangAI() : base("songsang") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class QinguoAI : SkillEvent
    {
        public QinguoAI() : base("qinguo") { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            return card != null && !card.IsVirtualCard() && Engine.GetFunctionCard(card.Name) is EquipCard ? 2 : 0;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            WrappedCard card = new WrappedCard(Slash.ClassName) { Skill = Name, ShowSkill = Name };
            List<WrappedCard> slashes = new List<WrappedCard> { card };
            List<ScoreStruct> scores = ai.CaculateSlashIncome(player, slashes, null, false);
            if (scores.Count > 0 && scores[0].Score > 0 && scores[0].Players != null && scores[0].Players.Count > 0)
            {
                use.Card = scores[0].Card;
                use.To = scores[0].Players;
            }

            return use;
        }
    }

    public class YoudiAI : SkillEvent
    {
        public YoudiAI() : base("youdi")
        {
            key = new List<string> { "cardChosen:youdi" };
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
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Place.PlaceEquip) > 0 ? false : true);
                    else
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<int> ids = player.GetCards("h");
            Room room = ai.Room;
            int slash = 0, black = 0;
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (WrappedCard.IsBlack(card.Suit)) black++;
                if (card.Name.Contains(Slash.ClassName)) slash++;
            }
            if ((double)(slash + black) / (ids.Count * 2) >= (double)1 / 3) return new List<Player>();

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in targets)
            {
                if (!RoomLogic.CanGetCard(room, player, p, "he")) continue;
                if (ai.IsFriend(p))
                {
                    bool lose = false;
                    foreach (int id in p.GetEquips())
                    {
                        if (ai.GetKeepValue(id, p, Place.PlaceEquip) < 0)
                        {
                            lose = true;
                            break;
                        }
                    }
                    if (lose)
                        scores.Add(ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodGet));
                }
                else
                {
                    scores.Add(ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodGet));
                }
            }

            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                    return scores[0].Players;
            }

            return new List<Player>();
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            double value = 0;
            if (ai.HasSkill(Name, player) && player.Phase == PlayerPhase.Discard && !isUse)
            {
                Room room = ai.Room;
                if (card.Name.Contains(Slash.ClassName) && room.GetCardPlace(card.GetEffectiveId()) == Place.PlaceHand) return value -=2;
                if (WrappedCard.IsBlack(card.Suit) && room.GetCardPlace(card.GetEffectiveId()) == Place.PlaceHand) value -= 2;
            }

            return value;
        }
    }

    public class DuanfaAI : SkillEvent
    {
        public DuanfaAI() : base("duanfa") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark(Name) < player.MaxHp)
            {
                Room room = ai.Room;
                List<int> sub = new List<int>();
                List<int> ids = player.GetCards("he");
                if (ids.Count > 0)
                {
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    for (int i = 0; i < ids.Count; i++)
                    {
                        if (values[i] < 0)
                        {
                            if (RoomLogic.CanDiscard(room, player, player, ids[i]) && WrappedCard.IsBlack(room.GetCard(ids[i]).Suit))
                                sub.Add(ids[i]);
                        }
                        else
                            break;

                        if (sub.Count + player.GetMark(Name) >= player.MaxHp)
                            break;
                    }
                }

                List<int> hands = player.GetCards("h");
                hands.RemoveAll(t => sub.Contains(t));
                if (hands.Count > 0)
                {
                    ai.SortByUseValue(ref hands, false);
                    foreach (int id in hands)
                    {
                        if (RoomLogic.CanDiscard(room, player, player, id) && WrappedCard.IsBlack(room.GetCard(id).Suit))
                            sub.Add(id);

                        if (sub.Count + player.GetMark(Name) >= player.MaxHp)
                            break;
                    }
                }

                if (sub.Count > 0)
                {
                    WrappedCard zhiheng_card = new WrappedCard(DuanfaCard.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name
                    };
                    zhiheng_card.AddSubCards(sub);
                    return new List<WrappedCard> { zhiheng_card };
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class DuanfaCardAI : UseCard
    {
        public DuanfaCardAI() : base(DuanfaCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 1;
        }
    }

    public class GuanweiAI : SkillEvent
    {
        public GuanweiAI() : base("guanwei")
        {
            key = new List<string> { "cardDiscard:guanwei" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (data is string choice && player != room.Current)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    if (ai.GetPlayerTendency(room.Current) != "unknown")
                        ai.UpdatePlayerRelation(player, room.Current, true);
                }
            }
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> cards, int min, int max, bool option)
        {
            Room room = ai.Room;
            if (ai.IsFriend(room.Current) && (ai.IsSituationClear() || ai.IsWeak(room.Current)))
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("he"))
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    List<double> values = new List<double>();
                    if (room.Current == player)
                        values = ai.SortByUseValue(ref ids, false);
                    else
                        values = ai.SortByKeepValue(ref ids, false);

                    return new List<int> { ids[0] };
                }
            }

            return new List<int>();
        }
    }

    public class GongqingAI : SkillEvent
    {
        public GongqingAI() : base("gongqing") { }
        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.From != null && damage.From.Alive && ai.HasSkill(Name, damage.To) && step == DamageStruct.DamageStep.Done)
            {
                bool weapon = true;
                if (damage.Card != null && damage.Card.SubCards.Contains(damage.From.Weapon.Key))
                    weapon = false;
                int range = RoomLogic.GetAttackRange(ai.Room, damage.From, weapon);
                if (range > 3) damage.Damage++;
            }
        }
    }

    public class BiaozhaoAI : SkillEvent
    {
        public BiaozhaoAI() : base("biaozhao")
        {
            key = new List<string> { "playerChosen:juedi" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            int count = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum > count) count = p.HandcardNum;
            
            double value = 0;
            Dictionary<Player, double> points = new Dictionary<Player, double>();
            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p) || ai.HasSkill("zishu", p)) continue;
                int draw = Math.Min(count - p.HandcardNum, 5);
                double _value = draw * 1.5;
                if (p.IsWounded()) _value += 3;
                if (ai.HasSkill(TrustedAI.CardneedSkill, p)) _value += draw;
                points[p] = _value;
            }

            if (points.Count > 0)
            {
                List<Player> frineds = new List<Player>(points.Keys);
                frineds.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                if (value < points[frineds[0]]) return new List<Player> { frineds[0] };
            }

            return new List<Player>();
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("he");
            if (ids.Count > 0)
            {
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 6) return new List<int> { ids[0] };
            }

            return new List<int>();
        }
    }

    public class YechouAI : SkillEvent
    {
        public YechouAI() : base("yechou") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            if (ai is StupidAI && (player.GetRoleEnum() == PlayerRole.Loyalist || player.GetRoleEnum() == PlayerRole.Rebel))
            {
                Room room = ai.Room;
                Dictionary<Player, double> points = new Dictionary<Player, double>();
                foreach (Player p in targets)
                {
                    if (!ai.IsEnemy(p)) continue;
                    double basic = 1;
                    if (player.GetRoleEnum() == PlayerRole.Rebel && p.GetRoleEnum() != PlayerRole.Lord) basic = 0.6;
                    int count = 1;
                    Player next = room.GetNextAlive(room.Current, 1, false);
                    while (next != p)
                    {
                        if (next.FaceUp)
                            count++;
                        next = room.GetNextAlive(next, 1, false);
                    }
                    points[p] = count * basic;
                }

                if (points.Count > 0)
                {
                    List<Player> players = new List<Player>(points.Keys);
                    players.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                    return new List<Player> { players[0] };
                }
            }

            return new List<Player>();
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai is StupidAI _ai && ai.HasSkill(Name, damage.To) && damage.Damage >= damage.To.Hp && ai.IsSituationClear())
            {
                Room room = ai.Room;
                if ((_ai.GetRolePitts(PlayerRole.Rebel) > 1 || _ai.GetRolePitts(PlayerRole.Renegade) > 0) && ai.GetPlayerTendency(damage.To) == "rebel")
                {
                    Player lord = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.GetRoleEnum() == PlayerRole.Lord && p.GetLostHp() > 1)
                        {
                            lord = p;
                            break;
                        }
                    }

                    if (lord != null)
                    {
                        int count = 1;
                        Player next = room.GetNextAlive(room.Current, 1, false);
                        while (next != lord)
                        {
                            if (next.FaceUp && next != damage.To)
                                count++;
                            next = room.GetNextAlive(next, 1, false);
                        }

                        if (count > lord.Hp)
                        {
                            switch (ai.Self.GetRoleEnum())
                            {
                                case PlayerRole.Lord:
                                case PlayerRole.Loyalist:
                                case PlayerRole.Renegade:
                                    score.Score = -40;
                                    break;
                                case PlayerRole.Rebel:
                                    score.Score = 20;
                                    break;
                            }
                        }
                    }
                }
            }

            return score;
        }
    }

    public class ZhafuAI : SkillEvent
    {
        public ZhafuAI() : base("zhafu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("@zhafu") > 0)
                return new List<WrappedCard> { new WrappedCard(ZhafuCard.ClassName) { Skill = Name, Mute = true } };
            return null;
        }
    }

    public class ZhafuCardAI : UseCard
    {
        public ZhafuCardAI() : base(ZhafuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                ai.SortByHandcards(ref enemies);
                foreach (Player p in enemies)
                {
                    if (!ai.WillSkipPlayPhase(p) && p.HandcardNum > 3)
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
            return 0;
        }
    }

    public class FuhaiAI : SkillEvent
    {
        public FuhaiAI() : base("fuhai") { }
        public override WrappedCard OnCardShow(TrustedAI ai, Player player, Player requestor, object data)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is int id)
            {
                int target = room.GetCard(id).Number;
                List<int> ids = player.GetCards("h");
                List<double> values = ai.SortByKeepValue(ref ids, false);
                for (int i = 0; i < ids.Count; i++)
                {
                    if (room.GetCard(ids[i]).Number > target && (values[i] < 4 || ai.IsFriend(room.Current) || room.Current.GetMark(Name) > 1))
                        return room.GetCard(ids[i]);
                }

                for (int i = 0; i < ids.Count; i++)
                {
                    if (room.GetCard(ids[i]).Number <= target)
                        return room.GetCard(ids[i]);
                }
            }

            return null;
        }
    }

    public class SongshuAI : SkillEvent
    {
        public SongshuAI() : base("songshu") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasFlag(Name) && !player.IsKongcheng())
                return new List<WrappedCard> { new WrappedCard(SongshuCard.ClassName) { Skill = Name } };

            return null;
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            if (ai.Self == requestor)
            {
                int id = (int)ai.Number[Name];
                if (id >= 0 && requestor.GetCards("h").Contains(id))
                    return ai.Room.GetCard((int)ai.Number[Name]);

                if (ai.IsFriend(players[0])) return ai.GetMinCard();
            }

            return ai.GetMaxCard();
        }
    }

    public class SongshuCardAI : UseCard
    {
        public SongshuCardAI() : base(SongshuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            ai.Number["songshu"] = -1; ai.Number[Name] = 1;
            if (friends.Count > 0)
            {
                Room room = ai.Room;
                List<int> ids = player.GetCards("h");
                List<double> values = ai.SortByUseValue(ref ids, false);
                for (int i = 0; i < ids.Count; i++)
                {
                    if (values[i] < 4)
                    {
                        foreach (Player p in friends)
                        {
                            if (!RoomLogic.CanBePindianBy(room, p, player) || ai.HasSkill("zishu", p)) continue;
                            foreach (int hand in ai.GetKnownCards(p))
                            {
                                if (room.GetCard(hand).Number >= room.GetCard(ids[i]).Number)
                                {
                                    ai.Number[Name] = 4;
                                    ai.Number["songshu"] = ids[i];
                                    use.Card = card;
                                    use.To.Add(p);
                                    return;
                                }
                            }

                            if (p.HandcardNum != ai.GetKnownCards(p).Count && room.GetCard(ids[i]).Number < 6)
                            {
                                ai.Number[Name] = 4;
                                ai.Number["songshu"] = ids[i];
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }

                if (ai.GetOverflow(player) > 0)
                {
                    values = ai.SortByKeepValue(ref ids, false);
                    for (int i = 0; i < ids.Count; i++)
                    {
                        if (values[i] < 6)
                        {
                            foreach (Player p in friends)
                            {
                                if (!RoomLogic.CanBePindianBy(room, p, player) || ai.HasSkill("zishu", p)) continue;
                                foreach (int hand in ai.GetKnownCards(p))
                                {
                                    if (room.GetCard(hand).Number >= room.GetCard(ids[i]).Number)
                                    {
                                        ai.Number["songshu"] = ids[i];
                                        use.Card = card;
                                        use.To.Add(p);
                                        return;
                                    }
                                }

                                if (p.HandcardNum != ai.GetKnownCards(p).Count && room.GetCard(ids[i]).Number < 6)
                                {
                                    ai.Number["songshu"] = ids[i];
                                    use.Card = card;
                                    use.To.Add(p);
                                    return;
                                }
                            }

                            foreach (Player p in ai.GetEnemies(player))
                            {
                                if (RoomLogic.CanBePindianBy(room, p, player) && ai.HasSkill("zishu", p))
                                {
                                    ai.Number["songshu"] = ids[i];
                                    use.Card = card;
                                    use.To.Add(p);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card) => ai.Number[Name];
    }

    public class SibianAI : SkillEvent
    {
        public SibianAI() : base("sibian") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            room.SortByActionOrder(ref targets);
            foreach (Player p in targets)
                if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p)) return new List<Player> { p };

            foreach (Player p in targets)
                if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p)) return new List<Player> { p };

            foreach (Player p in targets)
                if (ai.IsFriend(p)) return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class KannanAI : SkillEvent
    {
        public KannanAI() : base("kannan")
        {
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.From != null && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && damage.From.GetMark(Name) > 0)
            {
                Room room = ai.Room;
                List<CardUseStruct> use_list = room.GetUseList();

                if (use_list.Count == 0 || use_list[use_list.Count - 1].Card != damage.Card)
                    damage.Damage += damage.From.GetMark(Name);
            }
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            Room room = ai.Room;
            Player player = ai.Self;
            List<int> ids = player.GetCards("h");

            if (player == requestor)
            {
                Player target = players[0];
                ai.SortByUseValue(ref ids, false);
                if (!ai.IsFriend(target))
                {
                    return ai.GetMaxCard();
                }
                else
                    return room.GetCard(ids[0]);
            }
            else
            {
                ai.SortByKeepValue(ref ids, false);
                if (ai.IsFriend(requestor))
                {
                    return room.GetCard(ids[0]);
                }
                else
                    return ai.GetMaxCard();
            }
        }
    }

    public class JiedaoAI : SkillEvent
    {
        public JiedaoAI() : base("jiedao") { }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.From != null && ai.HasSkill(Name, damage.From) && damage.From.GetMark(Name) == 0 && damage.From.IsWounded()
                && step <= DamageStruct.DamageStep.Caused && !ai.IsEnemy(damage.From, damage.To))
                damage.Damage += damage.From.GetLostHp();
        }
    }

    public class JixuAI : SkillEvent
    {
        public JixuAI() : base("jixu") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (data is CardUseStruct use)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.HasFlag(Name) && ai.IsFriend(p) && ai.SlashIsEffective(use.Card, p).Score < 0)
                        return false;
                }
            }
            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            foreach (int id in ai.GetKnownCards(player))
            {
                if (room.GetCard(id).Name.Contains(Slash.ClassName))
                    return "has";
            }
            if (Shuffle.random(player.HandcardNum, 4)) return "has";
            return "nohas";
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(JixuCard.ClassName) && !player.IsKongcheng())
                return new List<WrappedCard> { new WrappedCard(JixuCard.ClassName) { Skill = Name } };
            return null;
        }
    }

    public class JixuCardAI : UseCard
    {
        public JixuCardAI() : base(JixuCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, false);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                Dictionary<Player, int> count = new Dictionary<Player, int>();
                foreach (Player p in enemies)
                {
                    int number = 1;
                    foreach (Player p2 in enemies)
                    {
                        if (p == p2) continue;
                        if (p.Hp == p2.Hp) number++;
                    }
                    count[p] = number;
                }
                enemies.Sort((x, y) => { return count[x] > count[y] ? -1 : 1; });
                use.Card = card;
                foreach (Player p in enemies)
                    if (p.Hp == enemies[0].Hp) use.To.Add(p);
            }            
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card) => 5;
    }

    public class KuizhuLSAI : SkillEvent
    {
        public KuizhuLSAI() : base("kuizhu_ls") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (player.HasFlag(Name))
            {
                Player target = null;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.HasFlag("kuizhu_target"))
                    {
                        target = p;
                        break;
                    }
                }

                foreach (Player p in targets)
                {
                    DamageStruct damage = new DamageStruct(Name, target, p);
                    if (ai.GetDamageScore(damage).Score > 6)
                        return new List<Player> { p };
                }
            }
            else
            {
                ai.SortByHandcards(ref targets);
                foreach (Player p in targets)
                    if (ai.IsFriend(p) && Math.Min(5, p.HandcardNum) - Math.Min(5, targets[0].HandcardNum) >= -1) return new List<Player> { p };

                return new List<Player> { targets[0] };
            }

            return new List<Player>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = room.Current;
            if (ai.IsFriend(target) && player.GetCardCount(false) >= 2)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                    if (RoomLogic.InMyAttackRange(room, player, p)) targets.Add(p);

                bool discard = false;
                if (targets.Count > 0)
                {
                    foreach (Player p in targets)
                    {
                        DamageStruct damage = new DamageStruct(Name, player, p);
                        if (ai.GetDamageScore(damage).Score > 6)
                            discard = true;
                    }
                }

                if (discard)
                {
                    List<int> ids = player.GetCards("h"), subs = new List<int>();
                    ai.SortByKeepValue(ref ids, false);
                    foreach (int id in ids)
                    {
                        if (RoomLogic.CanDiscard(room, player, player, id)) subs.Add(id);
                        if (subs.Count >= 2) break;
                    }

                    if (subs.Count == 2) return subs;
                }
            }
            else if (!ai.IsFriend(target) && room.GetTag(Name) is List<int> ids && !ai.HasSkill("zishu"))
            {
                List<int> hands = new List<int>();
                foreach (int id in player.GetCards("h"))
                    if (RoomLogic.CanDiscard(room, player, player, id)) hands.Add(id);

                if (hands.Count > 0)
                {
                    List<double> hand_values = ai.SortByKeepValue(ref hands, false);
                    List<double> target_values = ai.SortByKeepValue(ref ids);
                    if (hand_values[0] <= target_values[0])
                        return new List<int> { hands[0] };
                }
            }

            return new List<int>();
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            List<double> target_values = ai.SortByKeepValue(ref ups);
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Top = new List<int>(),
                Bottom = new List<int>(),
                Success = true
            };
            for (int i = 0; i < min; i++)
                move.Bottom.Add(ups[i]);
            ups.RemoveAll(t => move.Bottom.Contains(t));
            move.Top = ups;
            return move;
        }
    }

    public class FenyueAI : SkillEvent
    {
        public FenyueAI() : base("fenyue") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng())
            {
                Room room = ai.Room;
                int count = 0;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    PlayerRole role = p.GetRoleEnum();
                    if (role == PlayerRole.Lord || role == PlayerRole.Loyalist)
                    {
                        if (player.GetRoleEnum() != PlayerRole.Lord && player.GetRoleEnum() != PlayerRole.Loyalist)
                            count++;
                    }
                    else if (player.GetRoleEnum() != p.GetRoleEnum())
                        count++;
                }
                if (player.UsedTimes(FenyueCard.ClassName) < count)
                {
                    WrappedCard card = new WrappedCard(FenyueCard.ClassName) { Skill = Name };
                    return new List<WrappedCard> { card };
                }
            }

            return null;
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            if (ai.Self == requestor)
            {
                List<int> ids = requestor.GetCards("h");
                ai.SortByUseValue(ref ids, false);
                return ai.Room.GetCard(ids[0]);
            }
            else
            {
                return ai.GetMaxCard();
            }
        }
    }

    public class FenyueCardAI : UseCard
    {
        public FenyueCardAI() : base(FenyueCard.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, false);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                {
                    if (RoomLogic.CanBePindianBy(ai.Room, p, player))
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
            return 4;
        }
    }

    public class XuheAI : SkillEvent
    {
        public XuheAI() : base("xuhe") { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            double draw = 0;
            double discard = 0;
            Room room = ai.Room;
            if (RoomLogic.CanDiscard(room, player, player, "he")) discard += ai.FindCards2Discard(player, player, Name, "he", HandlingMethod.MethodDiscard).Score;
            draw += 1.5;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (RoomLogic.DistanceTo(room, player, p) == 1)
                {
                    if (RoomLogic.CanDiscard(room, player, p, "he"))
                        discard += ai.FindCards2Discard(player, p, Name, "he", HandlingMethod.MethodDiscard).Score;

                    draw += ai.HasSkill("zishu", p) ? 0 : ai.IsFriend(p) ? 1.5 : -1.5;
                }
            }
            if (draw >= discard && draw > 2)
                return "draw";
            else if (discard >= draw && discard > 2)
                return "discard";

            return "cancle";
        }
    }
}