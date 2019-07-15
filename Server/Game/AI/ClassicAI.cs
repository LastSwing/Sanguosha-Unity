using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using SanguoshaServer.Scenario;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{

    public class StupidAI : TrustedAI
    {
        public StupidAI(Room room, Player player) : base(room, player)
        {
            foreach (string skill in room.Skills)
            {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null)
                    skill_events[skill] = e;
            }

            foreach (FunctionCard card in room.AvailableFunctionCards)
            {
                SkillEvent e = Engine.GetSkillEvent(card.Name);
                if (e != null)
                    skill_events[card.Name] = e;
            }
        }
    }
}