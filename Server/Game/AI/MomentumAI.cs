using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class MomentumAI : AIPackage
    {
        public MomentumAI() : base("Momentum")
        {
            events = new List<SkillEvent>
            {
                new XunxunAI(),
                new WangxiAI(),
                new HengjiangAI(),
                new QianxiAI(),
                new GuixiuAI(),
                new CunsiAI(),
                new YongjueAI(),
                new JiangAI(),
                new YingyangAI(),
                new HunshangAI(),
                new YinghunSCAI(),
                new DuanxieAI(),
                new FenmingAI(),
                new HengzhengAI(),
                new BenghuaiAI(),
                new ChuanxinAI(),
                new FengshiAI(),
                new WuxinAI(),
                new WendaoAI(),
                new HongfaAI(),
                new HongfaSlashAI(),
            };

            use_cards = new List<UseCard>
            {
                new CunsiCardAI(),
                new DuanxieCardAI(),
                new WendaoCardAI(),
            };
        }
    }

    public class XunxunAI : SkillEvent
    {
        public XunxunAI() : base("xunxun") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForAttack() || ai.WillShowForDefence();
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            List<int> ids = new List<int>(ups);
            ids.AddRange(downs);

            ai.SortByUseValue(ref ids);

            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Bottom = new List<int> { ids[0], ids[1] },
                Top = new List<int> { ids[2], ids[3] },
                Success = true
            };

            return move;
        }
    }

    public class WangxiAI : SkillEvent
    {
        public WangxiAI() : base("wangxi")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsEnemy(target))
                return !ai.HasCrossbowEffect(target);

            return true;
        }
    }

    public class HengjiangAI : SkillEvent
    {
        public HengjiangAI() : base("hengjiang") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
                return !ai.IsFriend(target);

            return true;
        }
    }

    public class QianxiAI : SkillEvent
    {
        public QianxiAI() : base("qianxi")
        { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.WillShowForAttack())
            {
                Room room = ai.Room;
                int adjust = RoomLogic.PlayerHasShownSkill(room, player, "mashu_madai") ? 0 : -1;

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (!ai.IsFriend(p) && RoomLogic.DistanceTo(room, player, p) + adjust <= 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            int id = LijianAI.FindLijianCard(ai, player);
            if (id > -1)
                return new List<int> { id };

            return new List<int>();
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            List<Player> targets = new List<Player>(target);
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
            {
                if (ai.GetPrioEnemies().Contains(p) && !p.IsKongcheng())
                    return new List<Player> { p };
            }

            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p) && !p.IsKongcheng())
                    return new List<Player> { p };
            }

            foreach (Player p in targets)
            {
                if (!ai.IsFriend(p))
                    return new List<Player> { p };
            }

            return new List<Player> { targets[0] };
        }
    }

    public class GuixiuAI : SkillEvent
    {
        public GuixiuAI() : base("guixiu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class CunsiAI : SkillEvent
    {
        public CunsiAI() : base("cunsi") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return new List<WrappedCard> {
                new WrappedCard("CunsiCard")
                {
                Skill = Name,
                ShowSkill = Name,
                Mute = true
                }
            };
        }
    }

    public class CunsiCardAI : UseCard
    {
        public CunsiCardAI() : base("CunsiCard") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 7;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;

            List<Player> friends = new List<Player>(ai.FriendNoSelf);
            ai.SortByDefense(ref friends, false);
            if (player.IsWounded())
            {
                foreach (Player p in friends)
                {
                    if (ai.IsWeak(p) && RoomLogic.WillBeFriendWith(room, player, p, "cunsi"))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }

                foreach (Player p in friends)
                {
                    if (RoomLogic.WillBeFriendWith(room, player, p, "cunsi"))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }

                if (ai.IsWeak(player))
                {
                    use.Card = card;
                    use.To = new List<Player> { player };
                    return;
                }
            }
            else
            {
                foreach (Player p in friends)
                {
                    if (ai.IsWeak(p) && RoomLogic.WillBeFriendWith(room, player, p, "cunsi"))
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }
            }
        }
    }

    public class YongjueAI : SkillEvent
    {
        public YongjueAI() : base("yongjue") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class JiangAI : SkillEvent
    {
        public JiangAI() : base("jiang") { }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            double value = 0;
            if ((card.Name.Contains(Slash.ClassName) && WrappedCard.IsRed(card.Suit) || card.Name == Duel.ClassName) && ai.HasSkill(Name, to))
            {
                value += ai.IsFriend(to) ? 2 : -2;
            }

            return value;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && (card.Name.Contains(Slash.ClassName) && WrappedCard.IsRed(card.Suit) || card.Name == Duel.ClassName) && isUse)
                return 1.5;

            return 0;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForAttack() || ai.WillShowForDefence();
        }
    }

    public class YingyangAI : SkillEvent
    {
        public YingyangAI() : base("yingyang") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if ((ai.WillShowForAttack() || ai.WillShowForDefence()) && data is PindianStruct pindian)
            {
                int number = 0;
                if (pindian.From == player)
                {
                    number = pindian.From_number;
                    if (number < 13) return true;
                }
                else
                {
                    int index = pindian.Tos.IndexOf(player);
                    number = pindian.To_numbers[index];
                    if (ai.IsFriend(pindian.From) && number > 1)
                        return true;
                    else if (ai.IsEnemy(pindian.From) && number < 13)
                        return true;
                }
            }

            return false;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is PindianStruct pindian)
            {
                int number = 0;
                if (pindian.From == player)
                {
                    number = pindian.From_number;
                    if (number < 13) return "jia3";
                }
                else
                {
                    int index = pindian.Tos.IndexOf(player);
                    number = pindian.To_numbers[index];
                    if (ai.IsFriend(pindian.From) && number > 1)
                        return "jian3";
                    else if (ai.IsEnemy(pindian.From) && number < 13)
                        return "jia3";
                }
            }

            return string.Empty;
        }
    }

    public class HunshangAI : SkillEvent
    {
        public HunshangAI() : base("hunshang") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class YinghunSCAI : SkillEvent
    {
        public YinghunSCAI() : base("yinghun_sunce") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Player tar = YinghunJAI.Choose(ai, player);
            if (tar != null)
                return new List<Player> { tar };

            return new List<Player>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return YinghunJAI.Choice(ai, player);
        }
    }

    public class DuanxieAI : SkillEvent
    {
        public DuanxieAI() : base("duanxie") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasUsed("DuanxieCard"))
            {
                return new List<WrappedCard> {
                    new WrappedCard("DuanxieCard")
            {
                Skill = Name,
                ShowSkill = Name
            }
            };
            }

            return new List<WrappedCard>();
        }
    }
    public class DuanxieCardAI : UseCard
    {
        public DuanxieCardAI() : base("DuanxieCard") { }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in ai.GetEnemies(player))
            {
                if (!p.Chained && RoomLogic.CanBeChainedBy(room, p, player))
                {
                    ScoreStruct score = ai.FindCards2Discard(player, p, string.Empty, "he", HandlingMethod.MethodDiscard);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }
            }

            if (scores.Count > 0)
            {
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 0)
                {
                    use.Card = card;
                    use.To = scores[0].Players;
                }
            }
        }
    }

    public class FenmingAI : SkillEvent
    {
        public FenmingAI() : base("fenming") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            double value = 0;
            Room room = ai.Room;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.Chained && RoomLogic.CanDiscard(room, player, p, "he"))
                {
                    if (p == player)
                    {
                        List<int> ids = new List<int>();
                        foreach (int id in player.GetCards("he"))
                            if (RoomLogic.CanDiscard(room, player, player, id))
                                ids.Add(id);

                        ai.SortByKeepValue(ref ids, false);
                        value += ai.GetKeepValue(ids[0], player);
                    }
                    else
                    {
                        value += ai.FindCards2Discard(player, p, string.Empty, "he", HandlingMethod.MethodDiscard).Score;
                    }
                }
            }

            return value > 3;
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetCards("he"))
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);

            ai.SortByKeepValue(ref ids, false);
            return new List<int> { ids[0] };
        }
    }

    public class HengzhengAI : SkillEvent
    {
        public HengzhengAI() : base("hengzheng") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (ai.WillSkipPlayPhase(player))
            {
                int count = 0;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (!p.IsAllNude() && RoomLogic.CanGetCard(room, player, p, "hej"))
                        count++;
                }

                Player erzhang = ai.FindPlayerBySkill("guzheng");
                if (erzhang != null && !ai.IsFriend(erzhang) && player.HandcardNum + count > player.MaxHp + 2)
                {
                    return false;
                }
            }

            double value = 0;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.IsAllNude() && RoomLogic.CanGetCard(room, player, p, "hej"))
                    value += ai.FindCards2Discard(player, p, string.Empty, "he", HandlingMethod.MethodGet).Score;
            }

            return value > 4;
        }
    }

    public class BenghuaiAI : SkillEvent
    {
        public BenghuaiAI() : base("benghuai") { }
        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return player.Hp < player.MaxHp ? "maxhp" : "hp";
        }

        public override double GetSkillAdjustValue(TrustedAI ai, Player player)
        {
            return player.Hp > 4 ? 1.5 : -1;
        }
    }

    public class ChuanxinAI : SkillEvent
    {
        public ChuanxinAI() : base("chuanxin") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.Room.GetTag("chuanxin_data") is DamageStruct damage)
            {
                double value = ai.GetDamageScore(damage).Score;
                if (ai.IsFriend(damage.To))
                    return value < -6;
                else if (ai.DamageEffect(damage, DamageStruct.DamageStep.Done) > 1 && value > 4)
                    return false;
                else
                    return value < 5.5;
            }

            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            List<int> ids = player.GetCards("e");
            if (ids.Count <= 2)
            {
                ai.SortByKeepValue(ref ids, false);
                if (!ai.IsWeak() || ai.GetKeepValue(ids[0], player) < 0)
                    return "discard";
            }

            return "remove";
        }
    }

    public class FengshiAI : SkillEvent
    {
        public FengshiAI() : base("fengshi") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            List<int> ids = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetEquips())
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);

            ai.SortByKeepValue(ref ids, false);
            use.Card = room.GetCard(ids[0]);
            return use;
        }
    }

    public class WuxinAI : SkillEvent
    {
        public WuxinAI() : base("wuxin")
        {
            key = new List<string> { "wuxinchose" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && player == ai.Self)
            {
                string[] choices = choice.Split(':');
                Player who = ai.Room.FindPlayer(choices[1]);
                if (who != null)
                {
                    List<int> ups = CommonClass.JsonUntity.StringList2IntList(new List<string>(choices[2].Split('+')));
                    ai.SetGuanxingResult(who, ups);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            ai.SortByUseValue(ref ups);
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Top = new List<int>(ups),
                Bottom = new List<int>(downs),
                Success = true
            };
            return move;
        }
    }

    public class WendaoAI : SkillEvent
    {
        public WendaoAI() : base("wendao") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(WendaoCard.ClassName) && !player.IsNude())
            {
                Room room = ai.Room;
                int pp = -1;
                foreach (int id in room.DiscardPile)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == PeaceSpell.ClassName)
                    {
                        pp = id;
                        break;
                    }
                }
                if (pp == -1)
                {
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        foreach (int id in p.GetEquips())
                        {
                            WrappedCard card = room.GetCard(id);
                            if (card.Name == PeaceSpell.ClassName)
                            {
                                pp = id;
                                break;
                            }
                        }
                        if (pp != -1)
                            break;
                    }
                }

                if (pp > -1)
                {
                    WrappedCard wd = new WrappedCard(WendaoCard.ClassName)
                    {
                        ShowSkill = Name,
                        Skill = Name
                    };
                    List<int> ids = player.GetCards("he");
                    ai.SortByKeepValue(ref ids, false);
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (WrappedCard.IsRed(card.Suit) && card.Name != PeaceSpell.ClassName && ai.GetKeepValue(id, player) < 0)
                        {
                            wd.AddSubCard(id);
                            return new List<WrappedCard> { wd };
                        }
                    }

                    ai.SortByUseValue(ref ids, false);
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (WrappedCard.IsRed(card.Suit) && card.Name != PeaceSpell.ClassName)
                        {
                            wd.AddSubCard(id);
                            return new List<WrappedCard> { wd };
                        }
                    }
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class WendaoCardAI : UseCard
    {
        public WendaoCardAI() : base(WendaoCard.ClassName)
        {}
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (player.HasArmor(PeaceSpell.ClassName))
                return 8;
            else
                return 6;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class HongfaAI : SkillEvent
    {
        public HongfaAI() : base("hongfa") { }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (targets.Count > 0) return 0;
            return 4;
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            if (player.GetMark(Name) == 1)
                return new List<int> { player.GetPile("heavenly_army")[0] };
            else
                return player.GetPile("heavenly_army");
        }
    }

    public class HongfaSlashAI : SkillEvent
    {
        public HongfaSlashAI() : base("hongfaslash")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            Player jiaozhu = ai.FindPlayerBySkill("hongfa");
            List<WrappedCard> result = new List<WrappedCard>();
            if (jiaozhu != null && RoomLogic.WillBeFriendWith(room, player, jiaozhu))
            {
                foreach (int id in jiaozhu.GetPile("heavenly_army"))
                {
                    WrappedCard hongfa = new WrappedCard("HongfaCard") { Skill = "hongfa" };
                    hongfa.AddSubCard(id);

                    WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "hongfa" };
                    slash.AddSubCard(id);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    slash.UserString = RoomLogic.CardToString(room, hongfa);
                    result.Add(slash);
                }
            }

            return result;
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            Room room = ai.Room;
            Player jiaozhu = ai.FindPlayerBySkill("hongfa");
            List<WrappedCard> result = new List<WrappedCard>();
            if (jiaozhu != null && RoomLogic.WillBeFriendWith(room, player, jiaozhu) && pattern == Slash.ClassName)
            {
                foreach (int id in jiaozhu.GetPile("heavenly_army"))
                {
                    WrappedCard hongfa = new WrappedCard("HongfaCard") { Skill = "hongfa" };
                    hongfa.AddSubCard(id);

                    WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "hongfa" };
                    slash.AddSubCard(id);
                    slash = RoomLogic.ParseUseCard(room, slash);
                    slash.UserString = RoomLogic.CardToString(room, hongfa);
                    result.Add(slash);
                }
            }

            return result;
        }
    }
}