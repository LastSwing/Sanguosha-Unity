using System;
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

                new YixiangAI(),
                new YirangAI(),
            };

            use_cards = new List<UseCard>
            {
                new WuyuanCardAI(),
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
            if (data is string choice && ai.Self != player)
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
    }
}