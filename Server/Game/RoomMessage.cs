using CommonClassLibrary;
using System.Collections.Generic;

namespace SanguoshaServer.Game
{
    public class RoomMessage
    {
        public static void NotifyPlayerJoinorLeave(Room room, Client client, bool join)
        {
            string message = string.Format("{0}:{1}", join ? "@join_game" : "@leave_game", client.Profile.NickName);
            MyData data = new MyData
            {
                Description = PacketDescription.Room2Cient,
                Protocol = protocol.Message2Room,
                Body = new List<string> { string.Empty, message },
            };

            foreach (Client c in room.Clients)
                if (c.GameRoom == room.RoomId)
                    c.SendMessage(data);
        }

        public static void NotifyPlayerDisconnected(Room room, Client client)
        {
            string message = string.Format("@disconnected:{0}", client.Profile.NickName);
            MyData data = new MyData
            {
                Description = PacketDescription.Room2Cient,
                Protocol = protocol.Message2Room,
                Body = new List<string> { string.Empty, message },
            };

            foreach (Client c in room.Clients)
                if (c.GameRoom == room.RoomId)
                    c.SendMessage(data);
        }
    }
}
