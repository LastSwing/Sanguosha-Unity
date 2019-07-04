using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.Scenario
{
    public class GuanduWarfare : GameScenario
    {
        public GuanduWarfare()
        {
            mode_name = "GuanduWarfare";
        }
        public override void Assign(Room room)
        {
            throw new NotImplementedException();
        }

        public override List<string> GetWinners(Room room)
        {
            throw new NotImplementedException();
        }

        public override bool IsFriendWith(Room room, Player player, Player other)
        {
            throw new NotImplementedException();
        }

        public override void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile)
        {
            throw new NotImplementedException();
        }

        public override bool WillBeFriendWith(Room room, Player player, Player other, string skill_name = null)
        {
            throw new NotImplementedException();
        }
    }
}
