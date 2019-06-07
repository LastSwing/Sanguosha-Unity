using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class TransformationAI : AIPackage
    {
        public TransformationAI() : base("Transformation")
        {
            events = new List<SkillEvent>
            {
                new JiliAI(),
                new HuashenAI(),
                new XinshengAI(),
            };

            use_cards = new List<UseCard>
            {
            };
        }
    }

    public class JiliAI : SkillEvent
    {
        public JiliAI() : base("jili")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForAttack() || ai.WillShowForDefence();
        }
    }

    public class HuashenAI : SkillEvent
    {
        public HuashenAI() : base("huashen")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }
    public class XinshengAI : SkillEvent
    {
        public XinshengAI() : base("xinsheng")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }
}