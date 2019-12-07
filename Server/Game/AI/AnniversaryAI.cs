using System;
using System.Collections.Generic;
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

                new TunanAI(),

                new GuolunAI(),
                new SongSangAI(),
                new QinguoAI(),
                new YoudiAI(),
                new DuanfaAI(),
                new GongqingAI(),

                new KannanAI(),
                new JiedaoAI(),

            };

            use_cards = new List<UseCard>
            {
                new GuolunCardAI(),
                new YanjiaoCardAI(),
                new DuanfaCardAI(),
                new TunanCardAI(),
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
            if (ai.Self == player && !(ai is StupidAI)) return;
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
            if ((slash + black) / (ids.Count * 2) >= 1 / 3) return new List<Player>();

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
            base.Use(ai, player, ref use, card);
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
                List<CardUseStruct> use_list = room.ContainsTag("card_proceeing") ? (List<CardUseStruct>)room.GetTag("card_proceeing") : new List<CardUseStruct>();

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
}