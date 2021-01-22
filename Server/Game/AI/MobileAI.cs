﻿using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class MobileAI : AIPackage
    {
        public MobileAI() : base("Mobile")
        {
            events = new List<SkillEvent>
            {
                new YingjianAI(),

                new DaigongAI(),
                new ZhaoxinAI(),
                //new ZhongzuoAI(),
                new TongquAI(),
                new WanlanAI(),
                new KuangcaiAI(),

                new YixiangAI(),
                new YirangAI(),
                new RensheAI(),
                new ZhiyiAI(),
                new RangjieAI(),
                new ZhouxuanAI(),
                new ChengzhaoAI(),
                new WeifengAI(),

                new WuyuanAI(),

                new FenyinAI(),
            };

            use_cards = new List<UseCard>
            {
                new WuyuanCardAI(),
                new YizhengCardAI(),
            };
        }
    }

    public class YingjianAI : SkillEvent
    {
        public YingjianAI() : base("yingjian") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            WrappedCard card = new WrappedCard(Slash.ClassName) { Skill = Name, ShowSkill = Name, DistanceLimited = false };
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

    public class DaigongAI : SkillEvent
    {
        public DaigongAI() : base("daigong") { }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct();
            score.Score = 0;
            if (damage.From != null && damage.From != damage.To && ai.HasSkill(Name, damage.To) && !damage.To.HasFlag(Name) && !ai.IsFriend(damage.From, damage.To) && !damage.To.IsKongcheng())
            {
                if (ai.IsFriend(damage.To))
                    score.Score += 3;
                else if (ai.IsFriend(damage.From))
                    score.Score -= 3;
            }
            return score;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is DamageStruct damage)
            {
                ScoreStruct score = ai.GetDamageScore(damage, DamageStruct.DamageStep.Done);
                if (ai.IsEnemy(damage.To))
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("h"))
                    {
                        WrappedCard card = room.GetCard(id);
                        if (Engine.MatchExpPattern(room, pattern, player, card) && (card.Name != Peach.ClassName && card.Name != Analeptic.ClassName || damage.Damage > 1))
                            ids.Add(id);
                    }
                    if (ids.Count > 0)
                    {
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < score.Score)
                            return new List<int> { ids[0] };
                        if (player == room.Current)
                        {
                            values = ai.SortByUseValue(ref ids, false);
                            if (values[0] < score.Score)
                                return new List<int> { ids[0] };
                        }
                    }
                }
            }

            return new List<int>();
        }
    }

    public class ZhaoxinAI : SkillEvent
    {
        public ZhaoxinAI() : base("zhaoxin")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is string str)
            {
                string[] strs = str.Split(':');
                Player who = ai.Room.FindPlayer(strs[1]);

                DamageStruct damage = new DamageStruct(Name, who, player);
                if (ai.IsFriend(who) || ai.GetDamageScore(damage).Score > 0)
                    return true;
            }
            else if (data is Player target)
            {
                DamageStruct damage = new DamageStruct(Name, player, target);
                if (ai.GetDamageScore(damage).Score > 0)
                    return true;
            }

            return false;
        }
    }
    /*
    public class ZhongzuoAI : SkillEvent
    {
        public ZhongzuoAI() : base("zhongzuo")
        {
            key = new List<string> { "playerChosen:zhongzuo" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (target != null && ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> players = ai.FriendNoSelf;
            if (players.Count > 0)
            {
                ai.SortByDefense(ref players, false);
                foreach (Player p in players)
                    if (p.IsWounded())
                        return new List<Player> { p };

                players = ai.GetFriends(player);
                Room room = ai.Room;
                room.SortByActionOrder(ref players);
                foreach (Player p in players)
                {
                    if (p == room.Current || !p.FaceUp) continue;
                    return new List<Player> { p };
                }
            }
            return new List<Player> { player };
        }
    }
    */

    public class TongquAI : SkillEvent
    {
        public TongquAI() : base("tongqu")
        {
            key = new List<string> { "playerChosen:tongqu" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(strs[2]);
                    if (player != target && ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            room.SortByActionOrder(ref targets);
            foreach (Player p in targets)
                if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p) && !ai.WillSkipPlayPhase(p))
                    return new List<Player> { p };

            foreach (Player p in targets)
                if (ai.IsFriend(p))
                    return new List<Player> { p };

            return new List<Player>();
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(new WrappedCard(TongquCard.ClassName), player, new List<Player>());
            List<Player> targets = new List<Player>();
            Room room = ai.Room;
            List<int> ids = player.GetCards("he");
            ai.SortByKeepValue(ref ids, false);
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.GetMark(Name) > 0 && ai.IsFriend(p)) targets.Add(p);

            if (targets.Count > 0)
            {
                KeyValuePair<Player, int> keys = ai.GetCardNeedPlayer(null, targets, Player.Place.PlaceHand);
                if (keys.Key != null && keys.Value > -1)
                {
                    use.Card.AddSubCard(keys.Value);
                    use.To.Add(keys.Key);
                    return use;
                }
                else
                {
                    use.Card.AddSubCard(ids[0]);
                    use.To.Add(targets[0]);
                    return use;
                }
            }

            foreach (int id in ids)
            {
                if (RoomLogic.CanDiscard(room, player, player, id))
                {
                    use.Card.AddSubCard(id);
                    break;
                }
            }

            return use;
        }
    }

    public class WanlanAI : SkillEvent
    {
        public WanlanAI() : base("wanlan")
        {
            key = new List<string> { "skillInvoke:wanlan" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Room room = ai.Room;
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (player != target && ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (data is Player target)
                return ai.IsFriend(target);

            return false;
        }
    }

    public class YixiangAI : SkillEvent
    {
        public YixiangAI() : base("yixiang") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class YirangAI : SkillEvent
    {
        public YirangAI() : base("yirang")
        {
            key = new List<string> { "playerChosen:yirang" };
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
            bool equip = false, trick = false;
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                WrappedCard card = room.GetCard(id);
                CardType type = Engine.GetFunctionCard(card.Name).TypeID;
                if (type == CardType.TypeBasic) continue;
                ids.Add(id);
                if (type == CardType.TypeTrick)
                    trick = true;
                else if (type == CardType.TypeEquip)
                    equip = true;

                if (card.Name == Vine.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("shixin", p))
                            return new List<Player> { p };
                }
                if (card.Name == SilverLion.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("dingpan", p))
                            return new List<Player> { p };
                }
                if (card.Name == Spear.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("tushe", p))
                            return new List<Player> { p };
                }
                if (card.Name == EightDiagram.ClassName)
                {
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && !ai.HasSkill("zishu", p) && ai.HasSkill("tiandu", p))
                            return new List<Player> { p };
                }
            }

            if (equip)
            {
                foreach (Player p in targets)
                    if (ai.IsFriend(p) && ai.HasSkill(TrustedAI.LoseEquipSkill, p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                        return new List<Player> { p };
                foreach (Player p in targets)
                    if (ai.IsFriend(p) && ai.HasSkill(TrustedAI.NeedEquipSkill, p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                        return new List<Player> { p };
            }
            if (trick)
            {
                foreach (Player p in targets)
                    if (ai.IsFriend(p) && ai.HasSkill("jizhi|jizhi_jx", p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                        return new List<Player> { p };
            }
            foreach (Player p in targets)
                if (ai.IsFriend(p) && ai.HasSkill(TrustedAI.CardneedSkill, p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                    return new List<Player> { p };

            if (player.MaxHp == 1)
            {
                foreach (Player p in targets)
                    if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p) && !ai.HasSkill("zishu", p))
                        return new List<Player> { p };

                foreach (Player p in targets)
                    if (ai.IsFriend(p) && !ai.HasSkill("zishu", p))
                        return new List<Player> { p };

                if (ids.Count <= 2)
                {
                    int max_hp = 0;
                    foreach (Player p in targets)
                    {
                        if (p.MaxHp > max_hp)
                            max_hp = p.MaxHp;
                    }
                    foreach (Player p in targets)
                        if (p.MaxHp == max_hp && p.MaxHp - 1 >= ids.Count) return new List<Player> { p };
                }
            }

            return new List<Player>();
        }
    }

    public class RensheAI : SkillEvent
    {
        public RensheAI() : base("renshe")
        {
            key = new List<string> { "playerChosen:renshe" };
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
    }

    public class WuyuanAI : SkillEvent
    {
        public WuyuanAI() : base("wuyuan") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(WuyuanCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(WuyuanCard.ClassName) { Skill = Name } };

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card != null && !card.IsVirtualCard() && ai.HasSkill(Name, player) && card.Name.Contains(Slash.ClassName))
            {
                double value = 2;
                if (WrappedCard.IsRed(card.Suit)) value += 1;
                if (card.Name != Slash.ClassName) value += 1.5;
                return value;
            }
            return 0;
        }
    }

    public class WuyuanCardAI : UseCard
    {
        public WuyuanCardAI() : base(WuyuanCard.ClassName) { }

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
            List<int> slashes = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard wrapped = room.GetCard(id);
                if (wrapped.Name.Contains(Slash.ClassName)) slashes.Add(id);
            }

            if (slashes.Count > 0)
            {
                List<Player> friends = ai.FriendNoSelf;
                room.SortByActionOrder(ref friends);
                foreach (int id in slashes)
                {
                    WrappedCard slash = room.GetCard(id);
                    if (WrappedCard.IsRed(slash.Suit) && slash.Name != Slash.ClassName)
                    {
                        foreach (Player p in friends)
                        {
                            if (p.IsWounded() && !ai.HasSkill("zishu", p) && !ai.WillSkipPlayPhase(player))
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }

                foreach (int id in slashes)
                {
                    WrappedCard slash = room.GetCard(id);
                    if (!WrappedCard.IsRed(slash.Suit) && slash.Name != Slash.ClassName)
                    {
                        foreach (Player p in friends)
                        {
                            if (!ai.HasSkill("zishu", p) && !ai.WillSkipPlayPhase(player))
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }

                foreach (int id in slashes)
                {
                    WrappedCard slash = room.GetCard(id);
                    if (WrappedCard.IsRed(slash.Suit))
                    {
                        foreach (Player p in friends)
                        {
                            if (p.IsWounded() && !ai.HasSkill("zishu", p))
                            {
                                card.AddSubCard(id);
                                use.Card = card;
                                use.To.Add(p);
                                return;
                            }
                        }
                    }
                }

                ai.SortByUseValue(ref slashes, false);
                {
                    foreach (Player p in friends)
                    {
                        if (!ai.HasSkill("zishu", p) && !ai.WillSkipPlayPhase(player))
                        {
                            card.AddSubCard(slashes[0]);
                            use.Card = card;
                            use.To.Add(p);
                            return;
                        }
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4;
        }
    }

    public class ZhiyiAI : SkillEvent
    {
        public ZhiyiAI() : base("zhiyi") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            if (player.GetTag(Name) is List<string> cards)
            {
                List<WrappedCard> slashes = new List<WrappedCard>();
                foreach (string card_name in cards)
                {
                    if (card_name.Contains(Slash.ClassName))
                    {
                        WrappedCard card = new WrappedCard(card_name);
                        card.Skill = "_zhiyi";
                        slashes.Add(card);
                    }
                }
                if (slashes.Count > 0)
                {
                    List<ScoreStruct> values = ai.CaculateSlashIncome(player, slashes);
                    if (values.Count > 0 && values[0].Score > 3)
                    {
                        use.From = player;
                        use.Card = values[0].Card;
                        use.To = values[0].Players;
                        return use;
                    }
                }

                if (cards.Contains(Peach.ClassName))
                {
                    WrappedCard card = new WrappedCard(Peach.ClassName);
                    card.Skill = "_zhiyi";
                    if (Peach.Instance.IsAvailable(ai.Room, player, card))
                    {
                        use.From = player;
                        use.Card = card;
                        return use;
                    }
                }
            }

            return use;
        }
    }

    public class RangjieAI : SkillEvent
    {
        public RangjieAI() : base("rangjie")
        {
            key = new List<string> { "cardChosen:rangjie" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    int id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);

                    if (player != target && ai.GetPlayerTendency(target) != "unknown")
                    {
                        if (room.GetCardPlace(id) == Player.Place.PlaceDelayedTrick)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            bool friend = ai.GetKeepValue(id, target, Player.Place.PlaceEquip) < 0;
                            ai.UpdatePlayerRelation(player, target, friend);
                        }
                    }
                }
            }
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (choice.Contains("get"))
                return "get";
            else
                return "basic";
        }
    }

    public class YizhengCardAI : UseCard
    {
        public YizhengCardAI() : base("yizheng")
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
    }

    public class ZhouxuanAI : SkillEvent
    {
        public ZhouxuanAI() : base("zhouxuan")
        {
            key = new List<string> { "Yiji:zhouxuan" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                string[] strs = str.Split(':');
                Player target = room.FindPlayer(strs[3]);
                ai.UpdatePlayerRelation(player, target, true);
                if (player == ai.Self)
                {
                    List<string> cards = new List<string>(strs[4].Split('+'));
                    List<int> ids = JsonUntity.StringList2IntList(cards);
                    foreach (int id in ids)
                        ai.SetPrivateKnownCards(target, id);
                }
            }
        }
    }

    public class ChengzhaoAI : SkillEvent
    {
        public ChengzhaoAI() : base("chengzhao")
        {
            key = new List<string> { "playerChosen:chengzhao" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);

                    if (target != null && ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> players)
        {
            return ai.GetMaxCard();
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                    if (targets.Contains(p))
                        return new List<Player> { p };
            }

            return new List<Player>();
        }
    }

    public class WeifengAI : SkillEvent
    {
        public WeifengAI() : base("weifeng")
        { }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.Card != null && damage.To.ContainsTag(Name) && damage.To.GetTag(Name) is KeyValuePair<string, string> wei)
            {
                string card = damage.Card.Name.Contains(Slash.ClassName) ? Slash.ClassName : damage.Card.Name;
                if (card == wei.Value)
                    damage.Damage++;
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsEnemy(p)) return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class JinfanAI : SkillEvent
    {
        public JinfanAI() : base("jinfan") { }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());

            Room room = ai.Room;
            List<string> patterns = new List<string> { "spade", "heart", "club", "diamond" };
            foreach (int id in player.GetPile("&ring"))
                patterns.Remove(WrappedCard.GetSuitString(room.GetCard(id).Suit));

            List<int> result = new List<int>();
            List<int> hands = player.GetCards("h");
            ai.SortByKeepValue(ref hands);
            foreach (int id in hands)
            {
                string suit = WrappedCard.GetSuitString(room.GetCard(id).Suit);
                if (patterns.Contains(suit))
                {
                    patterns.Remove(suit);
                    result.Add(id);
                }
            }

            if (result.Count > 0)
            {
                WrappedCard card = new WrappedCard(JinfanCard.ClassName) { Skill = Name };
                card.AddSubCards(result);
                use.Card = card;
            }

            return use;
        }
    }

    public class ShequeAI : SkillEvent
    {
        public ShequeAI() : base("sheque") { }
        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            Room room = ai.Room;

            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (p.HasFlag("SlashAssignee")) targets.Add(p);

            List<ScoreStruct> values = ai.CaculateSlashIncome(player, null, targets);
            if (values.Count > 0 && values[0].Score > 0)
            {
                use.Card = values[0].Card;
                use.To = values[0].Players;
            }

            return use;
        }
    }

    public class ZhiyanXHAI : SkillEvent
    {
        public ZhiyanXHAI() : base("zhiyan_xh") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            ai.Choice[Name] = string.Empty;
            List<WrappedCard> result = new List<WrappedCard>();
            if (player.GetMark("zhiyan_used") < 2)
            {
                int mark = player.GetMark(Name);
                List<string> choices = new List<string>();
                if ((mark == 2 || mark == 0) && player.HandcardNum < player.MaxHp)
                {
                    ai.Choice[Name] = "draw";
                    result.Add(new WrappedCard(ZhiyanCard.ClassName) { Skill = Name });
                }
                else if (mark <= 1 && player.HandcardNum - player.Hp > 0)
                {
                    List<Player> friends = ai.GetFriends(player);
                    if (friends.Count > 0)
                    {
                        ai.Choice[Name] = "give";
                        result.Add(new WrappedCard(ZhiyanCard.ClassName) { Skill = Name });
                    }
                }
            }

            return result;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            List<Player> friends = ai.GetFriends(player);
            if (friends.Count > 0)
            {
                List<int> hands = player.GetCards("h"), give = new List<int>();
                Player target = null;
                while (give.Count < player.HandcardNum - player.Hp)
                {
                    KeyValuePair<Player, int> card = ai.GetCardNeedPlayer(hands, target == null ? friends : new List<Player> { target });
                    if (card.Key != null && card.Value >= 0)
                    {
                        target = card.Key;
                        give.Add(card.Value);
                        hands.Remove(card.Value);
                    }
                    else
                    {
                        if (target == null)
                        {
                            target = friends[0];
                        }

                        int count = player.HandcardNum - player.Hp - give.Count;
                        ai.SortByKeepValue(ref hands, false);
                        for (int i = 0; i < count; i++)
                        {
                            give.Add(hands[0]);
                        }
                    }
                }
                if (give.Count > 0)
                {
                    WrappedCard card = new WrappedCard(ZhiyanCard.ClassName) { Skill = Name };
                    card.AddSubCards(give);
                    use.To.Add(target);
                    use.Card = card;
                }
            }

            return use;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return ai.Choice[Name];
        }
    }

    public class ZhiyanCardAI : UseCard
    {
        public ZhiyanCardAI() : base(ZhiyanCard.ClassName)
        {
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is CardUseStruct use && use.To.Count > 0 && use.Card.SubCards.Count > 0)
            {
                ai.UpdatePlayerRelation(player, use.To[0], true);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (!string.IsNullOrEmpty(ai.Choice["zhiyan_xh"]))
            {
                use.Card = card;
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 0;
        }
    }

    public class ZhilueAI : SkillEvent
    {
        public ZhilueAI() : base("zhilue")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (!player.HasUsed(ZhilueCard.ClassName) && player.Hp >= 2)
            {
                result.Add(new WrappedCard(ZhilueCard.ClassName) { Skill = Name });
            }
            return result;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return "draw";
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                Skill = Name
            };
            List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { slash }, targets, false);
            if (scores.Count > 0 && scores[0].Players.Count == 1)
                return scores[0].Players;

            return null;
        }
    }

    public class ZhilueCardAI : UseCard
    {
        public ZhilueCardAI() : base(ZhilueCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4;
        }
    }

    public class KuangcaiAI : SkillEvent
    {
        public KuangcaiAI() : base("kuangcai") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
    }

    public class FenyinAI : SkillEvent
    {
        public FenyinAI() : base("fenyin") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            WrappedCard card = use.Card;
            if (!(Engine.GetFunctionCard(card.Name) is SkillCard) && player.GetMark(Name) > 0)
            {
                if ((card.Name == IronChain.ClassName || card.Name == GDFighttogether.ClassName) && use.To.Count == 0) return 0;
                int color = 1;
                if (WrappedCard.IsBlack(card.Suit))
                    color = 2;
                else if (WrappedCard.IsRed(card.Suit))
                    color = 2;

                if (color != player.GetMark(Name))
                    return 3.5;
            }

            return 0;
        }
    }
}