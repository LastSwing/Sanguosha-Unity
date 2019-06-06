using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class StanderWuAI : AIPackage
    {
        public StanderWuAI() : base("Stander-wu")
        {
            events = new List<SkillEvent>
            {
                new BuquAI(),
            };

            use_cards = new List<UseCard>
            {
            };
        }
    }

    public class BuquAI : SkillEvent
    {
        public BuquAI() : base("buqu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class FenjiAI : SkillEvent
    {
        public FenjiAI() : base("fenji")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                return ai.IsFriend(target);
            }

            return false;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name && choices[2] == "yes")
                {
                    Player target = ai.Room.Current;
                    if (!player.HasShownOneGeneral())
                        ai.UpdatePlayerIntention(player, Scenario.Hegemony.WillbeRole(ai.Room, player), 200);
                    if (ai.GetPlayerTendency(target) == "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
    }
}