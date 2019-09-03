using System.Data;

namespace CommonClass.Game
{
    //Class to maintain the Userlist 
    public class ClientList
    {
        private static ClientList mySingleton;
        private static DataTable clientTable;

        private ClientList()
        {
        }

        public static ClientList Instance()
        {
            if (mySingleton == null)
            {
                mySingleton = new ClientList();
                mySingleton.GenTable();
            }
            return mySingleton;
        }

        private void GenTable()
        {
            clientTable = new DataTable("UserList");
            DataColumn col;

            col = clientTable.Columns.Add("UserID", typeof(string));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("NickName", typeof(string));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("RoomNumber", typeof(int));
            col.AllowDBNull = false;

            clientTable.PrimaryKey = new DataColumn[] { clientTable.Columns["UserID"] };
        }

        public int LeaveHallAndReturnHallNumber(string UserID)
        {
            int leaveHallNumber = 0;
            foreach (DataRow row in clientTable.Rows)
            {
                if ((string)row["UserID"] == UserID)
                {
                    leaveHallNumber = (int)row["HallNumber"];
                    row["HallNumber"] = 0;
                    break;
                }
            }
            return leaveHallNumber;
        }

        public DataRow JoinHallAndReturnUserRow(string UserID, int newHallNumber)
        {
            DataRow userRow = null;
            foreach (DataRow row in clientTable.Rows)
            {
                if ((string)row["UserID"] == UserID)
                {
                    row["HallNumber"] = newHallNumber;
                    userRow = row;
                    break;
                }
            }

            return userRow;
        }

        public int FindUserId(string nick_name)
        {
            DataRow[] rows = clientTable.Select(string.Format("NickName = '{0}'", nick_name));
            if (rows.Length == 1)
                return int.Parse(rows[0]["UserID"].ToString());

            return 0;
        }

        public void AddClient(int id, string nick_name, int room_number = 0)
        {
            lock (clientTable.Rows.SyncRoot)
            {
                DataRow[] rows = clientTable.Select(string.Format("UserID = {0}", id));
                foreach (DataRow row in rows)
                    clientTable.Rows.Remove(row);

                DataRow newRow = clientTable.NewRow();
                newRow["UserID"] = id;
                newRow["NickName"] = nick_name;
                newRow["RoomNumber"] = room_number;

                clientTable.Rows.Add(newRow);
            }
        }

        public void RemoveClient(int UserID)
        {
            lock (clientTable.Rows.SyncRoot)
            {
                DataRow[] rows = clientTable.Select(string.Format("UserID = {0}", UserID));
                foreach (DataRow row in rows)
                    clientTable.Rows.Remove(row);
            }
        }

        public DataRow[] GetHallExceptDeskUserList(string UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);

            int HallNumber = (int)row["HallNumber"];
            int DeskNumber = (int)row["DeskNumber"];

            return clientTable.Select("HallNumber=" + HallNumber + " and DeskNumber<>" + DeskNumber, "DeskNumber,DeskPosition");
        }

        public DataRow[] GetDeskExceptThisUserList(string UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);

            int HallNumber = (int)row["HallNumber"];
            int DeskNumber = (int)row["DeskNumber"];

            string filter = "HallNumber=" + HallNumber.ToString() + " and DeskNumber=" + DeskNumber.ToString() + " and UserID<>'" + UserID + "'";
            return clientTable.Select(filter, "DeskNumber,DeskPosition");
        }

        public DataTable GetUserList(int HallNumber)
        {
            //string filter = "";
            //if (HallNumber > 0)
            //{
            //    filter = "HallNumber=" + HallNumber;
            //}

            //DataRow[] rows = clientTable.Select(filter, "DeskNumber,DeskPosition");

            //DataTable dt = clientTable.Clone();
            //foreach (DataRow row in rows)
            //{
            //    dt.Rows.Add(row.ItemArray);
            //}

            return clientTable;
        }

        public DataRow[] GetUserList(int HallNumber, int DeskNumber)
        {
            string filter = "HallNumber=" + HallNumber;
            if (DeskNumber > 0)
            {
                filter += " and DeskNumber=" + DeskNumber;
            }

            return clientTable.Select(filter, "DeskNumber,DeskPosition");
        }

        public void Init(DataTable db)
        {
            if (db.Rows.Count == 0)
            {
                GenTable();
                return;
            }

            clientTable = db.Copy();
        }
    }
}