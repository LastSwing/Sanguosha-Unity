using System;
using System.Collections;
using System.Data;

namespace SanguoshaServer
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
            col = clientTable.Columns.Add("UserName", typeof(string));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("Image1", typeof(int));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("Image2", typeof(int));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("Image3", typeof(int));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("IsOK", typeof(string));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("RoomNumber", typeof(int));
            col.AllowDBNull = false;
            col = clientTable.Columns.Add("RoomPosition", typeof(int));
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

        public DataRow FindUserRow(string UserID)
        {
            return clientTable.Rows.Find(UserID);
        }

        public void SomeOneIsOK(string UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);
            if (row != null)
            {
                row["IsOK"] = "Y";
            }
        }

        public int IsOKCount(string UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);
            int DeskNumber = (int)row["RoomNumber"];
            int HallNumber = (int)row["HallNumber"];

            return clientTable.Select("HallNumber=" + HallNumber + " and RoomNumber=" + DeskNumber + " and IsOK='Y'").Length;
        }

        public void LeaveDesk(string UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);
            if (row != null)
            {
                row["RoomNumber"] = 0;
                row["RoomPosition"] = 0;
            }
        }

        public void JoinDesk(string UserID, int newDeskNumber, int newDeskPosition)
        {
            DataRow row = clientTable.Rows.Find(UserID);
            if (row != null)
            {
                row["RoomNumber"] = newDeskNumber;
                row["RoomPosition"] = newDeskPosition;
            }
        }

        public void AddClient(Client client)
        {
            lock (clientTable)
            {
                if (!FindUser(client.UserID))
                {
                    DataRow newRow = clientTable.NewRow();
                    newRow["UserID"] = client.UserID;
                    newRow["UserName"] = client.Profile.NickName;
                    newRow["IsOK"] = "N";
                    newRow["Image1"] = client.Profile.Image1;
                    newRow["Image2"] = client.Profile.Image2;
                    newRow["Image3"] = client.Profile.Image3;
                    newRow["RoomNumber"] = 0;
                    newRow["RoomPosition"] = 0;

                    clientTable.Rows.Add(newRow);
                }
            }
        }

        public void RemoveClient(string UserID)
        {
            lock (clientTable)
            {
                DataRow row = clientTable.Rows.Find(UserID);
                if (row != null)
                {
                    clientTable.Rows.Remove(row);
                }
            }
        }

        public bool FindUser(int UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);
            if (row == null)
                return false;
            else
                return true;
        }

        public bool PositionExist(int HallNumber, int DeskNumber, int DeskPosition)
        {
            DataRow[] rows = clientTable.Select("HallNumber=" + HallNumber + " and DeskNumber=" + DeskNumber + " and DeskPosition=" + DeskPosition);
            if (rows.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable GetUserList(string UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);

            int HallNumber = (int)row["HallNumber"];

            return GetUserList(HallNumber);
        }

        public DataRow[] GetDeskUserList(string UserID)
        {
            DataRow row = clientTable.Rows.Find(UserID);

            int HallNumber = (int)row["HallNumber"];
            int DeskNumber = (int)row["DeskNumber"];

            return GetUserList(HallNumber, DeskNumber);
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
            string filter = "";
            if (HallNumber > 0)
            {
                filter = "HallNumber=" + HallNumber;
            }

            DataRow[] rows = clientTable.Select(filter, "DeskNumber,DeskPosition");

            DataTable dt = clientTable.Clone();
            foreach (DataRow row in rows)
            {
                dt.Rows.Add(row.ItemArray);
            }

            return dt;
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
    }
}