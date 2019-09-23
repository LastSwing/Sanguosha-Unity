using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections.Generic;

namespace SanguoshaServer
{
    public class DB
    {
        private const int MaxPool = 10;//最大连接数
        private const int MinPool = 5;//最小连接数
        private const bool Asyn_Process = true;//设置异步访问数据库
        private const bool Mars = true;
        private const int Conn_Timeout = 15;//设置连接等待时间
        private const int Conn_Lifetime = 15;//设置连接的生命周期
        private DB()
        {
        }
        private static string GetConnString(bool athur = true)
        {
            return ConfigurationManager.AppSettings[athur ? "ConnectionString" : "DataConnectionString"]
            //+ "integrated security=sspi;"
            + "Max Pool Size=" + MaxPool + ";"
            + "Min Pool Size=" + MinPool + ";"
            + "Connect Timeout=" + Conn_Timeout + ";"
            + "Connection Lifetime=" + Conn_Lifetime + ";"
            + "Asynchronous Processing=" + Asyn_Process;
            //+ ";" + "MultipleActiveResultSets="+Mars+";";
        }

        public static DataTable GetData(string sql, bool athur = true)
        {
                using (SqlConnection SqlDrConn = new SqlConnection(GetConnString(athur)))
            {
                SqlDrConn.Open();
                SqlCommand Cmd = new SqlCommand(sql, SqlDrConn);
                SqlDataReader SqlDr = Cmd.ExecuteReader();
                DataTable dt = new DataTable();
                if (SqlDr.HasRows)
                {
                    //读取SqlDataReader里的内容
                    dt.Load(SqlDr);
                    //关闭对象和连接
                    SqlDr.Close();
                }
                //SqlDrConn.Close();
                return dt;
                }
        }

        public static int UpdateData(string sql)
        {
                using (SqlConnection SqlDrConn = new SqlConnection(GetConnString()))
                {
                    SqlDrConn.Open();
                    SqlCommand Cmd = new SqlCommand(sql, SqlDrConn);
                    return Cmd.ExecuteNonQuery();
                }
        }
        public static DataTable LoginDB(string Account)
        {
            string sql = string.Format("select * from account where account='{0}'", Account);
            return GetData(sql);
        }
        
        //启动时初始化数据库，清除所有玩家的状态
        public static int InitDB()
        {
            return UpdateData("update account set status = 0, inGame = 0, roomID = -1");
        }

        /*
        public static void UpdateDB(string DestinationDB, int uid, List<string> names, List<string> values)
        {
            if (names.Count != values.Count) return;

            List<string> types = new List<string>();
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]);
            myConnection.Open();


            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                string value = values[i];

                string search = string.Format("select {0} from {1} where uid={2}", name, DestinationDB, uid);
                SqlCommand Command = new SqlCommand(search, myConnection);
                SqlDataReader dr = Command.ExecuteReader(CommandBehavior.CloseConnection);

                string sql = string.Format("update {0} where uid = @uid set {1} = {2}", DestinationDB, name, value);
                //string sql = "update @DestinationDB where uid = @uid set @name = @value";

                SqlCommand myCommand = new SqlCommand(sql, myConnection);
                myCommand.ExecuteNonQuery();
            }
            myConnection.Close();
        }
        */
        //获取

        public static void UpdateScore(string UserID, int Score)
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]);
            string sql = "update PlayerList set Score=Score + @Score where UserID=@UserID";
            SqlCommand myCommand = new SqlCommand(sql, myConnection);

            SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.VarChar)
            {
                Value = UserID
            };
            myCommand.Parameters.Add(parameterUserID);

            SqlParameter parameterScore = new SqlParameter("@Score", SqlDbType.Int)
            {
                Value = Score
            };
            myCommand.Parameters.Add(parameterScore);

            myConnection.Open();
            myCommand.ExecuteNonQuery();
            myConnection.Close();
        }
    }
}