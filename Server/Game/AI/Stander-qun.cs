using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class StanderQunAI : AIPackage
    {
        public StanderQunAI() : base("Stander-qun")
        {
            events = new List<SkillEvent>
            {
            };

            use_cards = new List<UseCard>
            {
            };
        }
    }
}