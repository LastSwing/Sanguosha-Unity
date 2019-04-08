using CommonClass.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanguoshaServer.Game
{
    public class AchieveCollector
    {
        public static void Event(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (room.Setting.GameMode != "Classic") return;
        }
    }
}
