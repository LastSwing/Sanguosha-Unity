using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CommonClass.Game
{
    //Class to maintain the Userlist 
    public class RoomList
    {
        private static RoomList mySingleton;
        private static ConcurrentDictionary<int, RoomInfoStruct> RoomTable;

        private RoomList()
        {
        }

        public static RoomList Instance()
        {
            if (mySingleton == null)
            {
                mySingleton = new RoomList();
                mySingleton.GenTable();
            }
            return mySingleton;
        }

        private void GenTable()
        {
            RoomTable = new ConcurrentDictionary<int, RoomInfoStruct>();
        }


        public RoomInfoStruct FindRoomRow(int id)
        {
            RoomTable.TryGetValue(id, out RoomInfoStruct info);
            return info;
        }

        public void AddRoom(int id, string name, bool pass, string gameMode, bool status, int current, int max)
        {
            RoomTable.TryRemove(id, out RoomInfoStruct old);
            RoomInfoStruct Info = new RoomInfoStruct()
            {
                Id = id,
                Name = name,
                PassWord = pass,
                Mode = gameMode,
                Started = status,
                CurrentPlayers = current,
                MaxPlayres = max,
            };
            RoomTable.TryAdd(id, Info);
        }

        public void RemoveRoom(int room_id)
        {
            RoomTable.TryRemove(room_id, out RoomInfoStruct old);
        }

        public Dictionary<int, RoomInfoStruct> GetRoomList()
        {
            var myDictionary = RoomTable.ToDictionary(entry => entry.Key,
                                                       entry => entry.Value);
            return myDictionary;
        }
        public void Init(ConcurrentDictionary<int, RoomInfoStruct> db)
        {
            RoomTable = db;
        }
    }
}