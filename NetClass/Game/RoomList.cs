using System.Data;

namespace CommonClass.Game
{
    //Class to maintain the Userlist 
    public class RoomList
    {
        private static RoomList mySingleton;
        private static DataTable RoomTable;

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
            RoomTable = new DataTable("RoomList");
            DataColumn col;

            col = RoomTable.Columns.Add("RoomId", typeof(int));
            col.AllowDBNull = false;
            col = RoomTable.Columns.Add("Name", typeof(string));
            col.AllowDBNull = false;
            col = RoomTable.Columns.Add("PassWord", typeof(bool));
            col.AllowDBNull = false;
            col = RoomTable.Columns.Add("Mode", typeof(string));
            col.AllowDBNull = false;
            col = RoomTable.Columns.Add("Started", typeof(bool));
            col.AllowDBNull = false;
            col = RoomTable.Columns.Add("CurrentPlayers", typeof(int));
            col.AllowDBNull = false;
            col = RoomTable.Columns.Add("MaxPlayers", typeof(int));
            col.AllowDBNull = false;

            RoomTable.PrimaryKey = new DataColumn[] { RoomTable.Columns["RoomId"] };
        }


        public DataRow FindRoomRow(int id)
        {
            DataRow[] rows = RoomTable.Select(string.Format("RoomId = {0}", id));
            if (rows.Length == 1)
                return rows[0];
            else
                return null;
        }

        public void LeaveDesk(int id)
        {
            DataRow row = FindRoomRow(id);
            if (row != null)
            {
                row["RoomNumber"] = 0;
                row["RoomPosition"] = 0;
            }
        }

        public void JoinDesk(int id, int newDeskNumber, int newDeskPosition)
        {
            DataRow row = FindRoomRow(id);
            if (row != null)
            {
                row["RoomNumber"] = newDeskNumber;
                row["RoomPosition"] = newDeskPosition;
            }
        }

        public void AddRoom(int id, string name, bool pass, string gameMode, bool status, int current, int max)
        {
            lock (RoomTable)
            {
                RemoveRoom(id);
                DataRow newRow = RoomTable.NewRow();
                newRow["RoomId"] = id;
                newRow["Name"] = name;
                newRow["PassWord"] = pass;
                newRow["Mode"] = gameMode;
                newRow["Started"] = status;
                newRow["CurrentPlayers"] = current;
                newRow["MaxPlayers"] = max;

                RoomTable.Rows.Add(newRow);

                DataView dView = RoomTable.DefaultView;
                dView.Sort = "RoomId asc";               //按RoomId升序
                RoomTable = dView.ToTable();
            }
        }

        public void RemoveRoom(int room_id)
        {
            lock (RoomTable)
            {
                DataRow row = FindRoomRow(room_id);
                if (row != null)
                {
                    RoomTable.Rows.Remove(row);
                }
            }
        }

        public bool FindRoom(int room_id)
        {
            DataRow row = FindRoomRow(room_id);
            if (row == null)
                return false;
            else
                return true;
        }

        public DataTable GetRoomList()
        {
            return RoomTable;
        }
        public void Init(DataTable db)
        {
            if (db.Rows.Count == 0)
            {
                GenTable();
                return;
            }

            RoomTable = db.Copy();
        }
    }
}