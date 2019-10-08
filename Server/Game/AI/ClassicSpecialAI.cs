using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class ClassicSpecialAI : AIPackage
    {
        public ClassicSpecialAI() : base("ClassicSpecial")
        {
            events = new List<SkillEvent>
            {
                new XianfuAI(),
                new ChouceAI(),

                new XuejiAI(),
                new LiangzhuAI(),
                new ShenxianAI(),
                new QiangwuAI(),

                new HongyuanAI(),
                new HuanshiAI(),
                new MingzheAI(),
                new AocaiAI(),
                new DuwuAI(),
            };

            use_cards = new List<UseCard>
            {
                new XuejiCardAI(),
                new QiangwuCardAI(),
                new DuwuCardAI(),
            };
        }
    }

    public class XuejiAI : SkillEvent
    {
        public XuejiAI() : base("xueji") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(XuejiCard.ClassName) && !player.IsNude())
                return new List<WrappedCard> { new WrappedCard(XuejiCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class XuejiCardAI : UseCard
    {
        public XuejiCardAI() : base(XuejiCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            base.OnEvent(ai, triggerEvent, player, data);
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            int count = Math.Max(1, player.GetLostHp());
            List<ScoreStruct> scores = new List<ScoreStruct>();
            Room room = ai.Room;
            foreach (Player p in room.GetAllPlayers())
            {
                DamageStruct damage = new DamageStruct("xueji", player, p, 1, DamageStruct.DamageNature.Fire);
                ScoreStruct score = ai.GetDamageScore(damage);
                score.Players = new List<Player> { p };
                scores.Add(score);
            }

            scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
            if (scores[0].Score > 0)
            {
                double value = scores[0].Score;
                Player target = scores[0].Players[0];
                if (scores[0].DoDamage)
                {
                    int damama_count = scores[0].Damage.Damage;
                    foreach (Player p in room.GetOtherPlayers(target))
                    {
                        if (p.Chained)
                        {
                            DamageStruct damage = new DamageStruct("xueji", player, p, damama_count, DamageStruct.DamageNature.Fire)
                            {
                                Chain = true
                            };
                            value += ai.GetDamageScore(damage).Score;
                        }
                    }
                }

                if (value > 0)
                {
                    use.To.Add(target);
                    if (count > 1)
                    {
                        int damama_count = scores[0].Damage.Damage;
                        List<ScoreStruct> scores2 = new List<ScoreStruct>();
                        foreach (Player p in room.GetOtherPlayers(target))
                        {
                            if (!p.Chained)
                            {
                                DamageStruct damage = new DamageStruct("xueji", player, p, damama_count, DamageStruct.DamageNature.Fire)
                                {
                                    Chain = true
                                };
                                ScoreStruct score = ai.GetDamageScore(damage);
                                score.Players = new List<Player> { p };
                                scores2.Add(score);
                            }
                        }
                        scores2.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                        for (int i = 0; i < Math.Min(count - 1, scores2.Count); i++)
                        {
                            if (scores2[i].Score > 0)
                            {
                                use.To.AddRange(scores2[i].Players);
                                value += scores2[i].Score;
                            }
                            else
                                break;
                        }
                    }
                }

                if (use.To.Count > 0)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("he"))
                        {
                        if (WrappedCard.IsRed(room.GetCard(id).Suit) && RoomLogic.CanDiscard(room, player, player, id))
                            ids.Add(id);
                    }

                    if (ids.Count > 0)
                    {
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < 0)
                        {
                            card.AddSubCard(ids[0]);
                            use.Card = card;
                            return;
                        }

                        values = ai.SortByUseValue(ref ids, false);
                        if (values[0] < value)
                        {
                            card.AddSubCard(ids[0]);
                            use.Card = card;
                            return;
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 2;
        }
    }

    public class LiangzhuAI : SkillEvent
    {
        public LiangzhuAI() : base("liangzhu") { key = new List<string> { "skillChoice" }; }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && ai.Self != player)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Player target = null;
                    Room room = ai.Room;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        ai.UpdatePlayerRelation(player, target, strs[2] == "let_draw");
                    }
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player target && ai.IsFriend(target))
                return "let_draw";

            return "draw_self";
        }
    }

    public class ShenxianAI : SkillEvent
    {
        public ShenxianAI() : base("shenxian") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class QiangwuAI : SkillEvent
    {
        public QiangwuAI() : base("qiangwu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(QiangwuCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(QiangwuCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class QiangwuCardAI : UseCard
    {
        public QiangwuCardAI() : base(QiangwuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 8;
        }
    }

    public class XianfuAI : SkillEvent
    {
        public XianfuAI() : base("xianfu") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            Player p = RoomLogic.FindPlayerBySkillName(room, "shibei");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "huituo");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "rende");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "jieyin_jx");
            if (p != null) return new List<Player> { p };
            p = RoomLogic.FindPlayerBySkillName(room, "kuanggu_jx");
            if (p != null) return new List<Player> { p };

            foreach (Player _p in targets)
                if (_p.GetRoleEnum() == PlayerRole.Lord)
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class ChouceAI : SkillEvent
    {
        public ChouceAI() : base("chouce")
        {
            key = new List<string> { "playerChosen", "cardChosen" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                string[] strs = str.Split(':');
                Room room = ai.Room;
                if (strs[0] == "playerChosen" && strs[1] == Name && !player.HasFlag(Name))
                {
                    Player target = room.FindPlayer(strs[2]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
                else if (strs[0] == "cardChosen" && strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceJudge)
                    {
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
                    else if (room.GetCardPlace(card_id) == Place.PlaceHand && !(ai.HasSkill("qianxun_jx", target) && target.HandcardNum > 1))
                    {
                        ai.UpdatePlayerRelation(player, target, ai.HasSkill("kongcheng|kongcheng_jx") && target.HandcardNum == 1 ? true : false);
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                    {
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Player.Place.PlaceEquip) > 0 ? false : true);
                    }
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            if (player.HasFlag(Name))
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in targets)
                    if (RoomLogic.CanDiscard(room, player, p, "hej"))
                        scores.Add(ai.FindCards2Discard(player, p, Name, "hej", HandlingMethod.MethodDiscard));

                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 0)
                        return scores[0].Players;
                }
            }
            else
            {
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p) && p.ContainsTag("xianfu") && p.GetTag("xianfu") is List<string> names && names.Contains(player.Name))
                        return new List<Player> { p };
                }

                ai.SortByDefense(ref targets, false);
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p)) return new List<Player> { p };
                }
            }

            return new List<Player>();
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            Room room = ai.Room;
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            Player xizicai = RoomLogic.FindPlayerBySkillName(room, Name);
            if (xizicai != null && damage.Damage < damage.To.Hp && damage.Damage < xizicai.Hp
                && damage.To.ContainsTag("xianfu") && damage.To.GetTag("xianfu") is List<string> names && names.Contains(xizicai.Name)
                && (ai.Self == xizicai || damage.To.GetMark("@fu") > 0))
            {
                if (ai.IsFriend(xizicai))
                    score.Score += damage.Damage * 2;
                else if (ai.IsEnemy(xizicai))
                    score.Score -= damage.Damage * 2;
            }

            return score;
        }
    }

    public class HongyuanAI : SkillEvent
    {
        public HongyuanAI() : base("hongyuan")
        {
            key = new List<string> { "playerChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    List<Player> targets = new List<Player>();
                    foreach (string name in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(name));

                    foreach (Player p in targets)
                        if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> friends = ai.FriendNoSelf;
            List<Player> result = new List<Player>();
            if (friends.Count > 0 && ai.WillSkipPlayPhase(player) || friends.Count > 1)
            {
                ai.SortByDefense(ref friends, false);
                for (int i = 0; i < Math.Min(2, friends.Count); i++)
                    result.Add(friends[i]);
            }

            return result;
        }
    }

    public class MingzheAI : SkillEvent
    {
        public MingzheAI() : base("mingzhe") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class HuanshiAI : SkillEvent
    {
        public HuanshiAI() : base("huanshi")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && room.GetTag(Name) is JudgeStruct judge)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Player target = judge.Who;
                    if (player != ai.Self)
                    {
                        if (ai.GetPlayerTendency(target) != "unknown")
                        {
                            bool friendly = false;
                            if (judge.Reason == Lightning.ClassName)
                            {
                                friendly = judge.Card.Suit == WrappedCard.CardSuit.Spade && judge.Card.Number > 1 && judge.Card.Number < 10;
                                ai.UpdatePlayerRelation(player, target, friendly);
                            }
                            else if (judge.Reason == SupplyShortage.ClassName)
                            {
                                friendly = judge.Card.Suit != WrappedCard.CardSuit.Club;
                                ai.UpdatePlayerRelation(player, target, friendly);
                            }
                            else if (judge.Reason == Indulgence.ClassName)
                            {
                                friendly = judge.Card.Suit != WrappedCard.CardSuit.Heart;
                                ai.UpdatePlayerRelation(player, target, friendly);
                            }
                        }
                    }
                    else
                    {
                        ai.ClearKnownCards(target);
                        foreach (int id in target.GetCards("h"))
                            ai.SetPrivateKnownCards(target, id);
                    }
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (data is Player target && room.GetTag(Name) is JudgeStruct judge)
            {
                if (ai.IsFriend(target))
                {
                    if (ai.GetRetrialCardId(player.GetCards("h"), judge) != -1)
                        return true;
                }
                else if (ai.IsEnemy(target))
                {
                    if (judge.Reason == Lightning.ClassName && (judge.Card.Suit != WrappedCard.CardSuit.Spade || judge.Card.Number > 1 || judge.Card.Number < 10))
                    {
                        DamageStruct damage = new DamageStruct(Lightning.ClassName, null, judge.Who, 3, DamageStruct.DamageNature.Thunder);
                        if (ai.GetDamageScore(damage).Score > 6)
                        {
                            bool match = true;
                            foreach (int id in player.GetCards("h"))
                            {
                                WrappedCard card = room.GetCard(id);
                                if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodResponse, true)) continue;
                                if (card.Suit != WrappedCard.CardSuit.Spade || card.Number == 1 || card.Number > 9)
                                {
                                    match = false;
                                    break;
                                }
                            }

                            return match;
                        }
                    }
                    else if (judge.Reason == SupplyShortage.ClassName && judge.Card.Suit == WrappedCard.CardSuit.Club)
                    {
                        bool match = true;
                        foreach (int id in player.GetCards("h"))
                        {
                            WrappedCard card = room.GetCard(id);
                            if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodResponse, true)) continue;
                            if (card.Suit == WrappedCard.CardSuit.Club)
                            {
                                match = false;
                                break;
                            }
                        }

                        return match;
                    }
                    else if (judge.Reason == Indulgence.ClassName && judge.Card.Suit == WrappedCard.CardSuit.Heart)
                    {
                        bool match = true;
                        foreach (int id in player.GetCards("h"))
                        {
                            WrappedCard card = room.GetCard(id);
                            if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodResponse, true)) continue;
                            if (card.Suit == WrappedCard.CardSuit.Heart || (ai.HasSkill("hongyan", target) && card.Suit == WrappedCard.CardSuit.Spade))
                            {
                                match = false;
                                break;
                            }
                        }

                        return match;
                    }
                }
            }

            return false;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            Room room = ai.Room;
            List<int> ids = to.GetCards("h");
            if (disable_ids != null)
                foreach (int id in disable_ids)
                    ids.Remove(id);
            if (room.GetTag(Name) is JudgeStruct judge)
            {
                if (ai.IsFriend(to))
                {
                    ai.SortByKeepValue(ref ids, false);
                    int id = ai.GetRetrialCardId(ids, judge, false);
                    if (id != -1)
                        return new List<int> { id };
                }
                else
                    ai.SortByKeepValue(ref ids);

                if (judge.Reason == Lightning.ClassName)
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit != WrappedCard.CardSuit.Spade || card.Number == 1 || card.Number > 9)
                            return new List<int> { id };
                    }
                }
                else if (judge.Reason == SupplyShortage.ClassName && judge.Card.Suit != WrappedCard.CardSuit.Club)
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Club)
                            return new List<int> { id };
                    }
                }
                else if (judge.Reason == Indulgence.ClassName && judge.Card.Suit != WrappedCard.CardSuit.Heart)
                {
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == WrappedCard.CardSuit.Heart || (ai.HasSkill("hongyan", from) && card.Suit == WrappedCard.CardSuit.Spade))
                            return new List<int> { id };
                    }
                }

                return new List<int> { ids[0] };
            }

            return new List<int>();
        }
    }

    public class AocaiAI : SkillEvent
    {
        public AocaiAI() : base("aocai") { }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            Room room = ai.Room;
            if (player.Phase == PlayerPhase.NotActive && player.GetPile("#aocai").Count > 0
                && (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE
                || room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                && (pattern == Slash.ClassName || pattern == Peach.ClassName || pattern == Jink.ClassName || pattern == Analeptic.ClassName))
            {
                foreach (int id in player.GetPile("#aocai"))
                {
                    if (Engine.MatchExpPattern(room, pattern, player, room.GetCard(id)))
                    {
                        WrappedCard card = Engine.CloneCard(room.GetCard(id));
                        card.Skill = Name;
                        card.UserString = id.ToString();
                        result.Add(card);
                    }
                }
            }

            return result;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 20;
        }
    }

    public class DuwuAI : SkillEvent
    {
        public DuwuAI() : base("duwu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return base.GetTurnUse(ai, player);
        }

    }

    public class DuwuCardAI : UseCard
    {
        public DuwuCardAI() : base(DuwuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return base.UsePriorityAdjust(ai, player, targets, card);
        }
    }
}