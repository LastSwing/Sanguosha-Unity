using CommonClassLibrary;
using System;
using System.Collections.Generic;

namespace SanguoshaServer.Game
{
    public class RoomMessage
    {
        public static void NotifyPlayerJoinorLeave(Room room, Client client, bool join)
        {
            try
            {
                string message = string.Format("{0}:{1}", join ? "@join_game" : "@leave_game", client.Profile.NickName);
                MyData data = new MyData
                {
                    Description = PacketDescription.Room2Cient,
                    Protocol = Protocol.Message2Room,
                    Body = new List<string> { string.Empty, message },
                };

                List<Client> clients = new List<Client>(room.Clients);
                foreach (Client c in clients)
                    if (c.GameRoom == room.RoomId)
                        c.SendMessage(data);
            }
            catch (Exception e)
            {
                room.Debug(string.Format("error on NotifyPlayerJoinorLeave {0} {1} {2}", e.Message, e.Source, e.HelpLink));
            }
        }

        public static void NotifyPlayerDisconnected(Room room, Client client)
        {
            try
            {
                string message = string.Format("@disconnected:{0}", client.Profile.NickName);
                MyData data = new MyData
                {
                    Description = PacketDescription.Room2Cient,
                    Protocol = Protocol.Message2Room,
                    Body = new List<string> { string.Empty, message },
                };

                List<Client> clients = new List<Client>(room.Clients);
                foreach (Client c in clients)
                    if (c.GameRoom == room.RoomId)
                        c.SendMessage(data);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
                room.Debug(string.Format("error on NotifyPlayerDisconnected {0} {1} {2}", e.Message, e.Source, e.HelpLink));
            }
        }
    }
}
