using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class AuthorityAI : AIPackage
    {
        public AuthorityAI() : base("Authority")
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