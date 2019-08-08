using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using System.Data;

/*
此类专门处理Client类与数据库交互
 */

namespace SanguoshaServer
{
    public class ClientDBOperation
    {
        #region 登录用户名密码检查
        public static ClientMessage CheckUserName(Client client, string UserName, string Password)
        {
            DataTable dr = DB.LoginDB(UserName);
            if (dr.Rows.Count == 1)
            {
                if (dr.Rows[0]["password"].ToString() == Password)
                {
                    client.IsLogin = true;
                    client.UserID = int.Parse(dr.Rows[0]["uid"].ToString());
                    client.UserName = UserName;
                    client.UserRight = int.Parse(dr.Rows[0]["User_Right"].ToString());
                    client.GameRoom = int.TryParse(dr.Rows[0]["roomID"].ToString(), out int result) ? result : -1;

                    //更新LastIP
                    string sql = string.Format("update account set lastIP = '{1}', status = {2}, login_date = GETDATE() where uid = {0}", client.UserID, client.IP, 1);
                    DB.UpdateData(sql);
                    
                    return ClientMessage.Connected;
                }
                //密码错误
                else
                {
                    return ClientMessage.PasswordWrong;
                    //关闭连接
                }
            }
            //账号不存在
            else
            {
                return ClientMessage.NoAccount;
            }
        }
        #endregion

        #region 从数据库读取个人信息并发送给客户端
        public static Profile GetProfile(Client client, bool to_hall = true)
        {
            string sql = string.Format("select * from profile where uid = {0}", client.UserID);
            DataTable dt = DB.GetData(sql);

            //首次登录个人信息为空
            if (dt.Rows.Count == 0)
            {
                //向客户端发送起名请求
                //MyData data = new MyData
                //{
                //    Description = PacketDescription.Hall2Cient,
                //    Protocol = protocol.NickName
                //};
                //SendProfileReply(data);

                Profile profile = new Profile { UId = -1 };
                return profile;
            }
            else
            {
                string title_sql = string.Format("select * from title where uid = {0}", client.UserID);
                DataTable title_dt = DB.GetData(title_sql);
                Dictionary<int, string> titles = new Dictionary<int, string>();
                foreach (DataRow row in title_dt.Rows)
                    titles.Add(int.Parse(row["title_id"].ToString()), row["date"].ToString());

                string achieve_sql = string.Format("select * from achieve where uid = {0}", client.UserID);
                DataTable achieve_dt = DB.GetData(achieve_sql);
                Dictionary<int, string> achieves = new Dictionary<int, string>();
                foreach (DataRow row in achieve_dt.Rows)
                    achieves.Add(int.Parse(row["achieve_id"].ToString()), row["date"].ToString());

                Profile profile = new Profile
                {
                    UId = int.Parse(dt.Rows[0]["uid"].ToString()),
                    NickName = dt.Rows[0]["NickName"].ToString(),
                    Right = client.UserRight,
                    Avatar = int.Parse(dt.Rows[0]["avatar"].ToString()),
                    Frame = int.Parse(dt.Rows[0]["frame"].ToString()),
                    Bg = int.Parse(dt.Rows[0]["bg"].ToString()),
                    GamePlay = int.Parse(dt.Rows[0]["GamePlay"].ToString()),
                    Win = int.Parse(dt.Rows[0]["Win"].ToString()),
                    Lose = int.Parse(dt.Rows[0]["Lose"].ToString()),
                    Draw = int.Parse(dt.Rows[0]["GamePlay"].ToString()),
                    Escape = int.Parse(dt.Rows[0]["Escape"].ToString()),
                    Title = int.Parse(dt.Rows[0]["Title_id"].ToString()),
                    Titles = titles,
                    Achievements = achieves
                };
                
                return profile;
            }
        }
        #endregion

        #region 用户注册
        public static bool Register(Client client, string id, string pwd)
        {
            string sql = string.Format("select * from account where account = '{0}'", id);
            DataTable dt = DB.GetData(sql);
            if (dt.Rows.Count == 0)
            {
                string new_sql = string.Format("insert into account (account, password, User_Right, status, lastIP, login_date, inGame) values ('{0}', '{1}', 0, 1, '{2}', GETDATE(), 0)", id, pwd, client.IP);
                //hall.OutPut(new_sql);
                DB.UpdateData(new_sql);

                DataTable dr = DB.LoginDB(id);
                if (dr.Rows.Count == 1)
                {
                    client.IsLogin = true;
                    client.UserID = int.Parse(dr.Rows[0]["uid"].ToString());
                    client.UserName = id;

                    return true;
                }
            }

            //用户名已注册
            return false;
        }
        #endregion

        #region 建立新的个人信息表
        public static bool InsertNewProfile(Client client, string NickName)
        {
            string sql = string.Format("select * from profile where NickName = '{0}'", NickName);
            DataTable dt = DB.GetData(sql);
            //昵称重名
            if (Engine.GetBotsNames().Contains(NickName) || dt.Rows.Count > 0)
                return false;
            else
            {
                string new_sql = string.Format("insert into profile (uid, NickName) values ({0}, '{1}') insert into title values({0}, 0, GETDATE())", client.UserID, NickName);
                DB.UpdateData(new_sql);
                return true;
            }
        }
        #endregion

        #region 更新用户信息
        public static void UpDateProfileAvatar(Profile profile)
        {
            string new_sql = string.Format("update  profile set avatar = {0}, frame = {1}, bg = {2}, Title_id = {3} where uid = {4}",
                profile.Avatar, profile.Frame, profile.Bg, profile.Title, profile.UId);

            DB.UpdateData(new_sql);
        }
        #endregion

        #region 更新用户游戏数据
        public static void UpdateProfileGamePlay(Client client, int uid, int win, bool escaped)
        {
            string sql = string.Format("select * from profile where uid = {0}", uid);
            DataTable dt = DB.GetData(sql);

            if (dt.Rows.Count == 1)
            {
                Profile profile = new Profile
                {
                    UId = uid,
                    GamePlay = int.Parse(dt.Rows[0]["GamePlay"].ToString()),
                    Win = int.Parse(dt.Rows[0]["Win"].ToString()),
                    Lose = int.Parse(dt.Rows[0]["Lose"].ToString()),
                    Draw = int.Parse(dt.Rows[0]["GamePlay"].ToString()),
                    Escape = int.Parse(dt.Rows[0]["Escape"].ToString()),
                    Title = int.Parse(dt.Rows[0]["Title_id"].ToString())
                };

                profile.GamePlay++;
                if (win > 0)
                    profile.Win++;
                else if (win < 0)
                    profile.Lose++;
                else
                    profile.Draw++;

                if (escaped)
                    profile.Escape++;

                string new_sql = string.Format("update profile set GamePlay = {0}, Win = {1}, Lose = {2}, Draw = {3}, [Escape] = {4} where uid = {5}",
                    profile.GamePlay, profile.Win, profile.Lose, profile.Draw, profile.Escape, uid);
                DB.UpdateData(new_sql);

                if (client != null)
                    client.UpdateProfileGamePlay(profile);
            }
        }
        #endregion

        //更新用户状态
        public static void UpdateStatus(string UserID, bool is_online)
        {
            string sql = string.Format("update account set status = {0} where account = '{1}'", is_online ? 1 : 0, UserID);
            DB.UpdateData(sql);
        }
        //更新用户所在游戏房间信息
        public static void UpdateGameRoom(string UserID, int RoomId)
        {
            string sql = string.Format("update account set inGame = {0}, roomID = {1} where account = '{2}'", RoomId > 0 ? 1 : 0, RoomId, UserID);
            DB.UpdateData(sql);
        }

        public static bool CheckTitle(int uid, int title_id)
        {
            string sql = string.Format("select * from title where uid = {0} and title_id = {1}", uid, title_id);
            DataTable dt = DB.GetData(sql);
            if (dt.Rows.Count == 0)
                return false;

            return true;
        }

        public static void SetTitle(int uid, int title_id, bool get = true)
        {
            string sql = get ? string.Format("insert into title (uid, title_id) values ({0}, {1})", uid, title_id)
                : string.Format("delete form title where uid = {0} and title_id = {1}", uid, title_id);
            DB.UpdateData(sql);
        }

        public static int GetTitleMark(int uid, int mark_id)
        {
            string sql = string.Format("select * from title_mark where uid = {0} and mark_id = {1}", uid, mark_id);
            DataTable dt = DB.GetData(sql);
            if (dt.Rows.Count == 1)
                return int.Parse(dt.Rows[0]["value"].ToString());

            return 0;
        }

        public static void SetTitleMark(int uid, int mark_id, int value)
        {
            string sql = string.Format("delete from title_mark where uid = {0} and mark_id = {1} insert into title_mark values({0}, {1}, {2})", uid, mark_id, value);
            DB.UpdateData(sql);
        }
    }
}
