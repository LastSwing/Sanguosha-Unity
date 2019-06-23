using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

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

        public override double UsePriorityAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return WrappedCard.IsRed(RoomLogic.GetCardSuit(ai.Room, card)) ? -0.2 : 0;
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
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            base.OnEvent(ai, triggerEvent, player, data);
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
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
        public ShangyiAI() : base("shangyi") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return base.GetTurnUse(ai, player);
        }
    }

    public class ShangyiCardAI : UseCard
    {
        public ShangyiCardAI() : base("ShangyiCard") { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            base.OnEvent(ai, triggerEvent, player, data);
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
        }
    }

    public class YichengAI : SkillEvent
    {
        public YichengAI() : base("yicheng")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return base.OnSkillInvoke(ai, player, data);
        }
    }

    public class QianhuanAI : SkillEvent
    {
        public QianhuanAI() : base("qianhuan")
        { }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return base.OnExchange(ai, player, pattern, min, max, pile);
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }
    }

    public class ZhenduAI : SkillEvent
    {
        public ZhenduAI() : base("zhendu")
        { }

        public override List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip)
        {
            return base.OnDiscard(ai, player, min, max, option, include_equip);
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
            return true;
        }
    }
}