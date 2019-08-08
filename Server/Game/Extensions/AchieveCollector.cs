using CommonClass.Game;
using SanguoshaServer.Game;
using System.Collections.Generic;

namespace SanguoshaServer.Extensions
{
    public class AchieveCollector
    {
        public static void Event(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            foreach (Title title in Engine.GetTitleCollector())
                if (title.EventList.Contains(triggerEvent))
                    title.OnEvent(triggerEvent, room, player, data);
        }
    }

    public abstract class Title
    {
        public int TitleId { get; private set; }
        public int MarkId { get; private set; }
        public List<TriggerEvent> EventList { get; protected set; }
        public Title(int id, int mark)
        {
            TitleId = id;
            MarkId = mark;
        }

        public abstract void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data);
    }
}
