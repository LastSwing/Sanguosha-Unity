using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class FormationAI : AIPackage
    {
        public FormationAI() : base("Formation")
        {
            events = new List<SkillEvent>
            {
                new ShengxiAI(),
                new ShouchengAI(),
            };

            use_cards = new List<UseCard>
            {
            };
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
}