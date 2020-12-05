using System;
using System.Collections.Generic;
using CommonClass;
using CommonClassLibrary;
using SanguoshaServer.Game;
using CommandType = CommonClassLibrary.CommandType;

namespace SanguoshaServer
{
    public class ClientEventArgs : EventArgs
    {
        public int Id { set; get; }
        public bool Kicked { set; get; }
    }

    public class Client
    {
        public enum GameStatus {
            Normal,
            Ready,
            Online,
            Offline,
            Bot,
        }

        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<ClientEventArgs> LeaveRoom;
        public event EventHandler<ClientEventArgs> GetReady;

        private GameHall hall;
        private Profile _profile;

        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserRight { get; set; }
        public bool IsLogOn { get; set; }
        public Profile Profile => _profile;

        public string IP { get; }

        public GameStatus Status { get; set; }
        public int GameRoom
        {
            get => game_room;
            set
            {
                game_room = value;
                if (room != null)
                    room = null;
            }
        }

        public MsgPackSession Session { get; }
        public string RoleReserved { get; set; }
        public List<string> GeneralReserved { get; set; }
        private int game_room = 0;
        private Room room = null;

        //构造函数
        public Client(GameHall hall, MsgPackSession session, Profile profile = new Profile())
        {
            this.hall = hall;
            Session = session;
            IsLogOn = false;
            if (session != null)
            {
                IP = session.RemoteEndPoint.ToString();
            }
            this._profile = profile;
            if (profile.UId < 0)
                UserId = profile.UId;
        }

        //bot only
        public Client(GameHall hall, Profile profile = new Profile())
        {
            this.hall = hall;
            Session = null;
            UserName = profile.NickName;
            IsLogOn = false;
            this._profile = profile;
            if (profile.UId < 0)
                UserId = profile.UId;
        }

        public override bool Equals(object obj)
        {
            Client other = (Client)obj;
            return UserId == other.UserId && UserName == other.UserName;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() * UserName.GetHashCode();
        }

        #region  向客户端发送信息相关
        //登录
        public void SendLoginReply(MyData data)
        {
            try
            {
                byte[] bytes = PacketTranslator.Data2byte(data, PacketTranslator.GetTypeString(TransferType.TypeLogin));
                if (Session != null && Session.Connected)
                    Session.Send(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
            }
        }

        //聊天大厅
        public void SendProfileReply(MyData data)
        {
            try
            {
                byte[] bytes = PacketTranslator.Data2byte(data, PacketTranslator.GetTypeString(TransferType.TypeUserProfile));
                if (Session != null && Session.Connected)
                {
                    Session.Send(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
            }
        }

        //切换
        public void SendSwitchReply(MyData data)
        {
            try
            {
                byte[] bytes = PacketTranslator.Data2byte(data, PacketTranslator.GetTypeString(TransferType.TypeSwitch));
                if (Session != null && Session.Connected)
                {
                    Session.Send(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
            }
        }

        //发送游戏信息/操作请求给客户端
        public void SendRoomNotify(List<string> message_body)
        {
            try
            {
                if (Session != null && Session.Connected)
                {
                    MyData data = new MyData
                    {
                        Description = PacketDescription.Room2Cient,
                        Protocol = Protocol.GameNotification,
                        Body = message_body
                    };
                    byte[] bytes = PacketTranslator.Data2byte(data, PacketTranslator.GetTypeString(TransferType.TypeGameControll));

                    Session.Send(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
            }
        }
        public void SendRoomRequest(CommandType command, List<string> message_body)
        {
            try
            {
                if (Session != null && Session.Connected)
                {
                    message_body.Insert(0, command.ToString());
                    MyData data = new MyData
                    {
                        Description = PacketDescription.Room2Cient,
                        Protocol = Protocol.GameRequest,
                        Body = message_body
                    };
                    byte[] bytes = PacketTranslator.Data2byte(data, PacketTranslator.GetTypeString(TransferType.TypeGameControll));
                    Session.Send(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
            }
        }

        //发送聊天信息给客户端
        public void SendMessage(MyData data)
        {
            try
            {
                byte[] bytes = PacketTranslator.Data2byte(data, PacketTranslator.GetTypeString(TransferType.TypeMessage));
                if (Session != null && Session.Connected)
                    Session.Send(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
            }
        }

        #endregion

        #region 登录相关
        //用户名检查
        public void CheckUserName(MyData data)
        {
            try
            {
                string UserName = data.Body[0], Password = data.Body[1];
                if (!string.IsNullOrEmpty(GameHall.Version) && (data.Body.Count < 3 || data.Body[2] != GameHall.Version))        //检查版本号
                {
                    MyData wrong = new MyData
                    {
                        Description = PacketDescription.Hall2Cient,
                        Protocol = Protocol.ClientMessage,
                        Body = new List<string> { ClientMessage.VersionNotMatch.ToString(), true.ToString() }
                    };
                    SendLoginReply(wrong);
                    return;
                }

                ClientMessage message = ClientDBOperation.CheckUserName(this, UserName, Password);
                if (message == ClientMessage.Connected)
                    //Raise the Connected Event 
                    Connected?.Invoke(this, new EventArgs());
                else if (message == ClientMessage.PasswordWrong)
                {
                    //通知客户端
                    MyData wrong = new MyData
                    {
                        Description = PacketDescription.Hall2Cient,
                        Protocol = Protocol.ClientMessage,
                        Body = new List<string> { ClientMessage.PasswordWrong.ToString(), true.ToString() }
                    };
                    SendLoginReply(wrong);
                    //关闭连接
                }
                else
                {
                    //账号不存在
                    //通知客户端
                    MyData wrong = new MyData
                    {
                        Description = PacketDescription.Hall2Cient,
                        Protocol = Protocol.ClientMessage,
                        Body = new List<string> { ClientMessage.NoAccount.ToString(), true.ToString() }
                    };
                    SendLoginReply(wrong);
                    //关闭连接
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
                hall.Debug(string.Format("error on check user name {0} {1}", e.Message, e.TargetSite));
            }
        }

        //从数据库读取个人信息并发送给客户端
        public bool GetClientProfile(bool to_hall = true)
        {
            try
            {
                _profile = ClientDBOperation.GetProfile(this, to_hall);
                //首次登录个人信息为空
                if (Profile.UId <= 0)
                {
                    //向客户端发送起名请求
                    //MyData data = new MyData
                    //{
                    //    Description = PacketDescription.Hall2Cient,
                    //    Protocol = protocol.NickName
                    //};
                    //SendProfileReply(data);

                    MyData data = new MyData
                    {
                        Description = PacketDescription.Hall2Cient,
                        Protocol = Protocol.UserProfile,
                        Body = new List<string> { JsonUntity.Object2Json(Profile) }
                    };

                    SendProfileReply(data);
                    return false;
                }
                else
                {
                    //发送至客户端
                    MyData data = new MyData
                    {
                        Description = PacketDescription.Hall2Cient,
                        Protocol = Protocol.UserProfile,
                        Body = new List<string> { JsonUntity.Object2Json(Profile), to_hall.ToString() }
                    };

                    SendProfileReply(data);
                    return true;
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
                hall.Debug(string.Format("error on get profile {0} {1}", e.Message, e.TargetSite));
            }
            return false;
        }
        #endregion
        //账号注册和个人信息更新
        public void Register(MyData data)
        {
            string id = data.Body[0];
            string pwd = data.Body[1];
            string version = data.Body[2];
            if (!string.IsNullOrEmpty(GameHall.Version) && (data.Body.Count < 3 || data.Body[2] != GameHall.Version))        //检查版本号
            {
                MyData wrong = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = Protocol.ClientMessage,
                    Body = new List<string> { ClientMessage.VersionNotMatch.ToString(), true.ToString() }
                };
                SendLoginReply(wrong);
                return;
            }

            if (ClientDBOperation.Register(this, id, pwd))
            {
                hall.OutPut(UserId + "：注册成功");

                MyData message = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = Protocol.ClientMessage,
                    Body = new List<string> { ClientMessage.RegisterSuccesful.ToString(), false.ToString() }
                };
                SendLoginReply(message);

                //初始化与room的交互方法

                if (Connected != null)
                {
                    EventArgs e = new EventArgs();
                    Connected(this, e);
                }
            }
            else
            {
                //用户名已注册
                hall.OutPut(id + "已注册");
                //发送信息通知客户端
                MyData message = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = Protocol.ClientMessage,
                    Body = new List<string> { ClientMessage.AccountDuplicated.ToString(), true.ToString() }
                };
                SendLoginReply(message);
            }
        }
        //建立新的个人信息表
        public void InsertNewProfile(string NickName)
        {
            bool succe = ClientDBOperation.InsertNewProfile(this, NickName);
            if (!succe)
            {
                //昵称重名
                //通知客户端
                MyData data = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = Protocol.NickName,
                    Body = new List<string>() { false.ToString() }
                };
                SendProfileReply(data);
            }
            else
            {
                MyData data = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = Protocol.NickName,
                    Body = new List<string>() { true.ToString() }
                };
                SendProfileReply(data);

                hall.InterHall(this, false);
            }
        }
        //更新个人信息
        public void UpDateProfile(MyData data)
        {
            try
            {
                switch (data.Protocol) {
                    case Protocol.NickName:
                        InsertNewProfile(data.Body[0]);
                        break;
                    case Protocol.UserProfile:
                        Profile profile = JsonUntity.Json2Object<Profile>(data.Body[0]);
                        UpDateProfileAvatar(profile);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(null, e);
                hall.Debug(string.Format("error at update profile {0} {1} {2}", e.Message, e.Source, e.HelpLink));
            }
        }
        private void UpDateProfileAvatar(Profile profile)
        {
            if (profile.UId == Profile.UId && Engine.CheckShwoAvailable(profile) && CheckTitle(profile.Title))
            {
                this._profile.Avatar = profile.Avatar;
                this._profile.Title = profile.Title;
                this._profile.Frame = profile.Frame;
                this._profile.Bg = profile.Bg;

                ClientDBOperation.UpDateProfileAvatar(profile);

                SendProfile2Client();
            }
        }

        public void UpdateProfileGamePlay(Profile profile)
        {
            if (profile.UId == this._profile.UId)
            {
                this._profile.GamePlay = profile.GamePlay;
                this._profile.Win = profile.Win;
                this._profile.Lose = profile.Lose;
                this._profile.Escape = profile.Escape;
                this._profile.Draw = profile.Draw;
                
                SendProfile2Client();
            }
        }

        public void AddProfileTitle(int title_id)
        {
            if (!_profile.Titles.ContainsKey(title_id))
            {
                _profile.Titles.Add(title_id, DateTime.Now.ToString());
                SendProfile2Client();
            }
        }

        private void SendProfile2Client()
        {
            MyData data = new MyData
            {
                Description = PacketDescription.Hall2Cient,
                Protocol = Protocol.UserProfile,
                Body = new List<string> { JsonUntity.Object2Json(_profile) }
            };

            SendProfileReply(data);
        }
        private bool CheckTitle(int id)
        {
            return _profile.Titles.ContainsKey(id);
        }
        public void OnDisconnected()
        {
            ClientDBOperation.UpdateStatus(UserName, false);
            Disconnected?.Invoke(this, new EventArgs());
        }

        public void RequestLeaveRoom(bool kicked = false)
        {
            GameRoom = 0;
            ClientDBOperation.UpdateGameRoom(UserName, 0);

            ClientEventArgs args = new ClientEventArgs
            {
                Id = GameRoom,
                Kicked = kicked
            };
            LeaveRoom?.Invoke(this, args);
        }

        public void RequstReady(bool ready)
        {
            ClientEventArgs arg = new ClientEventArgs
            {
                Kicked = ready
            };
            GetReady?.Invoke(this, arg);
        }
    }
}
