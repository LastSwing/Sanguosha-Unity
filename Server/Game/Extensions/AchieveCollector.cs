using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;

namespace SanguoshaServer.Extensions
{
    public class AchieveCollector
    {
        public static void Event(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            try
            {
                foreach (Title title in Engine.GetTitleCollector())
                    if (title.EventList.Contains(triggerEvent))
                        title.OnEvent(triggerEvent, room, player, data);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
                room.Debug(string.Format("error when proceeing title {0} {1}", e.Message, e.TargetSite));
            }
        }
    }

    public abstract class Title
    {
        public int TitleId { get; private set; }
        public int MarkId { get; protected set; }
        public List<TriggerEvent> EventList { get; protected set; }
        public Title(int id)
        {
            TitleId = id;
        }

        public abstract void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data);
    }
}
