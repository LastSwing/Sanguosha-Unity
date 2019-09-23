using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class NaturefourAI : AIPackage
    {
        public NaturefourAI() : base("Naturefour")
        {
            events = new List<SkillEvent>
            {
                new LiegongJXAI(),
            };
        }
    }
    

    public class LiegongJXAI : SkillEvent
    {
        public LiegongJXAI() : base("liegong_jx")
        {
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (step <= DamageStruct.DamageStep.Caused && damage.From != null && damage.From.Alive && ai.HasSkill(Name, damage.From) && damage.To.Hp >= damage.From.Hp
                && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && !damage.Transfer && !damage.Chain)
                damage.Damage++;
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsEnemy(target))
                return true;

            return false;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (choice.Contains("nojink"))
                return "nojink";
            else if (choice.Contains("damage"))
                return "damage";

            return base.OnChoice(ai, player, choice, data);
        }
    }
}