using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanguoshaServer.SQL_DB
{
    public class DBPool
    {
        private const int MaxPool = 10;//最大连接数
        private const int MinPool = 5;//最小连接数
        private const bool Asyn_Process = true;//设置异步访问数据库
        private const bool Mars = true;
        private const int Conn_Timeout = 15;//设置连接等待时间
        private const int Conn_Lifetime = 15;//设置连接的生命周期
        private string ConnString = "";//连接字符串
        private SqlConnection SqlDrConn = null;//连接对象
        public DBPool()//构造函数
        {
            ConnString = GetConnString();
            SqlDrConn = new SqlConnection(ConnString);
        }

        private string GetConnString()
        {
            return ConfigurationManager.AppSettings["connectionString"]
            + "integratedsecurity=sspi;"
            + "MaxPoolSize=" + MaxPool + ";"
            + "MinPoolSize=" + MinPool + ";"
            + "ConnectTimeout=" + Conn_Timeout + ";"
            + "ConnectionLifetime=" + Conn_Lifetime + ";"
            + "AsynchronousProcessing=" + Asyn_Process + ";";
            //+"MultipleActiveResultSets="+Mars+";";
        }
    }
}
