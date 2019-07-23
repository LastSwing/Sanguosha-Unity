using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace SanguoshaServer
{
    public class GameHall
    {
        private Dictionary<int, Client> UId2ClientTable;
        private Dictionary<MsgPackSession, Client> Session2ClientTable;
        private Dictionary<int, Room> RId2Room;
        private Dictionary<Room, Thread> Room2Thread;
        private Form1 form;
        private int room_serial = 0;

        public GameHall(Form1 form)
        {
            this.form = form;
            UId2ClientTable = new Dictionary<int, Client>();
            Session2ClientTable = new Dictionary<MsgPackSession, Client>();
            RId2Room = new Dictionary<int, Room>();
            Room2Thread = new Dictionary<Room, Thread>();

            new Engine();
        }
        public Room GetRoom(int room_id)
        {
            if (RId2Room.ContainsKey(room_id))
                return RId2Room[room_id];

            return null;
        }

        #region 客户端首次对话时
        public void OnConnected(MsgPackSession session) {
            Client new_client = new Client(this, session);
            Session2ClientTable.Add(session, new_client);

            OutPut(session.LocalEndPoint.ToString() + " 进行了连接");

            //Attach the Delegates
            new_client.Connected += OnConnected;
        }
        #endregion

        public Client GetClient(int uid) {
            if (UId2ClientTable.ContainsKey(uid))
            {
                return UId2ClientTable[uid];
            }
            else {
                return null;
            }
        }

        #region 客户端断线时操作
        internal void OnDisconnected(MsgPackSession session, CloseReason value)
        {
            OutPut(session.SessionID + " disconnected:" + value);

            Client client = Session2ClientTable[session];
            client.Connected -= OnConnected;
            //删除session对应
            Session2ClientTable.Remove(session);
            session = null;
            //删除uid对应
            if (client != null)
            {
                client.OnDisconnected();

                if (UId2ClientTable.ContainsValue(client))
                    UId2ClientTable.Remove(client.UserID);
                //广播离线信息
                ClientList.Instance().RemoveClient(client.UserID);
                MyData data = new MyData()
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = protocol.UpdateHallLeave,
                    Body = new List<string> { client.UserID.ToString() }
                };
                foreach (Client other in UId2ClientTable.Values)
                {
                    other.SendProfileReply(data);
                }
            }
        }
        #endregion

        #region 信息输出至form
        public delegate void OutDelegate(string text);
        public void OutPut(string message) {
            if (form.InvokeRequired) {
                OutDelegate outdelegate = new OutDelegate(OutPut);
                form.BeginInvoke(outdelegate, new object[] { message });
                return;
            }
            form.AddLog(message);
        }
        #endregion

        #region 同账号重复登录时
        public bool HandleSameAccount(int uid) {
            Client same = GetClient(uid);
            if (same != null)
            {
                //发送同账号登录信息
                MyData data = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = protocol.ClientMessage,
                    Body = new List<string> { ClientMessage.LoginDuplicated.ToString(), true.ToString() }
                };
                same.SendLoginReply(data);

                OutPut("重复登录");

                //断开连接
                same.Session.Close();

                //测试
                UId2ClientTable.Remove(uid);

                return true;
            }
            else
                return false;
        }
        #endregion

        #region 处理客户端请求
        public void OnRequesting(MsgPackSession session, BinaryRequestInfo requestInfo) {
            Client client = Session2ClientTable[session];

            MyData data = PacketTranslator.Unpack(requestInfo.Body);

            TransferType request = PacketTranslator.GetTransferType(requestInfo.Key);

            //OutPut(string.Format("请求协议为{0}, 内容为{1}", data.Protocol.ToString(), data.Body[0]));
            if (client.UserName == null && !request.Equals(TransferType.TypeLogin)) {
                OutPut("未登录客户端，关闭连接");
                session.Close();
                return;
            }

            if (data.Description == PacketDescription.Client2Hall)
            {
                switch (request)
                {
                    case TransferType.TypeLogin:                          //登录相关
                        switch (data.Protocol) {
                            case protocol.Login:
                                client.CheckUserName(data);
                                break;
                            case protocol.Register:                       //用户注册
                                client.Register(data);
                                break;
                            case protocol.PasswordChange:                 //密码修改
                                break;
                            default:
                                return;
                        }
                        break;
                    case TransferType.TypeMessage:                        //公共事件 比如 聊天
                        MessageForward(client, data);
                        break;
                    case TransferType.TypeSwitch:                           //切换room和hall
                        RoomSwitch(client, data);
                        break;
                    case TransferType.TypeUserProfile:                      //修改个人信息
                        client.UpDateProfile(data);
                        break;
                    default:
                        OutPut("对聊天大厅无效的请求");
                        break; ;
                }
            } else if (data.Description == PacketDescription.Client2Room)
            {
                switch (request)
                {
                    case TransferType.TypeMessage:                        //公共事件 比如 聊天
                        MessageForward(client, data);
                        break;
                    case TransferType.TypeGameControll:                   //游戏内各操作
                        client.ControlGame(data);
                        break;
                    case TransferType.TypeSwitch:                           //切换room和hall
                        RoomSwitch(client, data);
                        break;
                    default:
                        OutPut("对ROOM无效的请求");
                        break; ;
                }
            }
            else
            {
                OutPut("无效的客户端请求");
            }
        }
        #endregion

        #region 客户端登录成功时
        public void OnConnected(object sender, EventArgs e)
        {
            lock (this)
            {
                if (sender is Client temp)
                {
                    //同名已连接用户强制离线
                    bool same = HandleSameAccount(temp.UserID);

                    //Add the client to the Hashtable 
                    UId2ClientTable.Add(temp.UserID, temp);
                    OutPut("Client Connected:" + temp.UserID);

                    //check last game is over
                    bool reconnect = false;
                    if (temp.GameRoom > 0)
                    {
                        if (RId2Room.ContainsKey(temp.GameRoom) && RId2Room[temp.GameRoom].GameStarted)
                        {
                            reconnect = true;
                        }
                        else
                        {
                            //更新用户room id
                            DB.UpdateGameRoom(temp.UserName, -1);
                        }
                    }

                    InterHall(temp, reconnect);
                }
            }
        }

        public void InterHall(Client client, bool reconnect)
        {
            //update user profile
            //bring to hall
            bool proceed = client.GetProfile(!reconnect);
            if (!proceed) return;

            ClientList.Instance().AddClient(client.UserID, client.Profile.NickName);

            MyData data = new MyData();
            if (!reconnect)
            {
                //发送当前已登录的其他玩家信息和游戏房间信息
                DataSet ds = new DataSet();
                ds.Tables.Add(ClientList.Instance().GetUserList(0).Copy());
                ds.Tables.Add(RoomList.Instance().GetRoomList().Copy());

                data = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = protocol.GetProfile,
                    Body = new List<string> { JsonUntity.DataSet2Json(ds) }
                };
                client.SendProfileReply(data);
            }

            //通知其他客户端更新
            data = new MyData()
            {
                Description = PacketDescription.Hall2Cient,
                Protocol = protocol.UpdateHallJoin,
                Body = new List<string> { client.UserID.ToString(), client.Profile.NickName, "0" }
            };
            foreach (Client other in UId2ClientTable.Values)
            {
                if (other != client)
                    other.SendProfileReply(data);
            }
        }
        #endregion

        #region 消息转发
        private void MessageForward(Client sourcer, MyData data)
        {
            MyData message = new MyData
            {
                Description = PacketDescription.Hall2Cient,
                Protocol = data.Protocol
            };
            message.Body = data.Body;

            switch (data.Protocol) {
                case protocol.Message2Hall:
                    foreach (Client client in UId2ClientTable.Values)
                    {
                        if (client.GameRoom <= 0)
                            client.SendMessage(message);
                    }
                    break;
                case protocol.Message2Client:
                    Client destination = GetClient(int.Parse(data.Body[2]));
                    if (destination != null)
                    {
                        destination.SendMessage(message);
                    }
                    break;
                case protocol.Message2Room:
                    message.Description = PacketDescription.Room2Cient;
                    Room room = GetRoom(sourcer.GameRoom);
                    if (room != null)
                    {
                        if (!room.GameStarted || !room.Setting.SpeakForbidden || data.Body.Count == 3)
                        {
                            foreach (Client dest in room.Clients)
                                dest.SendMessage(message);
                        }
                    }
                    break;
                case protocol.MessageSystem:
                    if (sourcer.UserRight >= 3)
                    {
                        foreach (Client client in UId2ClientTable.Values)
                            client.SendMessage(message);
                    }
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region 更新roomlist、进入/退出游戏房间
        private void RoomSwitch(Client client, MyData data)
        {
            switch (data.Protocol)
            {
                case protocol.CreateRoom:
                    CreateRoom(client, JsonUntity.Json2Object<GameSetting>(data.Body[0]));
                    break;
                case protocol.JoinRoom:
                    int room_id = -1;
                    string pass = string.Empty;
                    if (data.Body.Count > 0)
                    {
                        room_id = int.Parse(data.Body[0]);
                        pass = data.Body[1].ToString();
                    }
                    else
                    {
                        room_id = client.GameRoom;
                    }

                    Room room = RId2Room.ContainsKey(room_id) ? RId2Room[room_id] : null;
                    bool result = false;
                    if (room != null)
                    {
                        //if (JoinRoom != null)
                        //{
                        //    Delegate[] delArray = JoinRoom.GetInvocationList();
                        //    foreach (Delegate del in delArray)
                        //    {
                        //        Room target = (Room)del.Target;
                        //        if (target != null && target.RoomId == room_id)
                        //        {
                        //            JoinRoomDelegate method = (JoinRoomDelegate)del;
                        //            result = method(client, room_id, pass);
                        //        }
                        //    }
                        //}
                        //JoinRoom?.Invoke(client, room_id, pass);
                        result = room.OnClientRequestInter(client, room_id, pass);
                    }
                    if (!result)
                    {
                        OutPut("join request fail at hall");
                        data = new MyData
                        {
                            Description = PacketDescription.Hall2Cient,
                            Protocol = protocol.JoinRoom,
                        };
                        client.SendSwitchReply(data);
                    }
                    break;
                case protocol.LeaveRoom:
                    if (RId2Room.ContainsKey(client.GameRoom))
                        client.RequestLeaveRoom();
                    break;
                case protocol.UpdateRoom:
                    client.RequstReady(bool.Parse(data.Body[0]));
                    break;
                case protocol.RoleReserved:
                    if (client.UserRight >= 2 && RId2Room.ContainsKey(client.GameRoom))
                    {
                        room = RId2Room[client.GameRoom];
                        if (!room.GameStarted && room.Setting.GameMode == "Classic")
                            client.RoleReserved = data.Body[0];
                    }
                    break;
                case protocol.KickOff:
                    room = RId2Room.ContainsKey(client.GameRoom) ? RId2Room[client.GameRoom] : null;
                    if (room != null && room.Host == client && !room.GameStarted)
                    {
                        int victim_id = int.Parse(data.Body[0]);
                        Client victim = GetClient(victim_id);
                        if (victim != null || victim_id < 0)
                            victim.RequestLeaveRoom(true);
                    }
                    break;
                case protocol.ConfigChange:
                    room = RId2Room.ContainsKey(client.GameRoom) ? RId2Room[client.GameRoom] : null;
                    if (room != null && room.Host == client && !room.GameStarted)
                    {
                        room.ChangeSetting(JsonUntity.Json2Object<GameSetting>(data.Body[0]));
                    }
                    break;
                default:
                    break;
                    
            }
        }

        private void CreateRoom(Client client, GameSetting setting)
        {
            lock (this)
            {
                //检查游戏设置是否正常
                GameMode mode = Engine.GetMode(setting.GameMode);
                if (!RId2Room.ContainsKey(client.GameRoom) && mode.Name == setting.GameMode)
                {
                    if (!mode.PlayerNum.Contains(setting.PlayerNum))
                        setting.PlayerNum = mode.PlayerNum[0];

                    List<string> general_p = new List<string>();
                    foreach (string general in setting.GeneralPackage)
                    {
                        if (mode.GeneralPackage.Contains(general))
                        {
                            setting.GeneralPackage = mode.GeneralPackage;
                            break;
                        }
                    }
                    foreach (string card in setting.CardPackage)
                    {
                        if (!mode.GeneralPackage.Contains(card))
                        {
                            setting.CardPackage = mode.CardPackage;
                            break;
                        }
                    }

                    OutPut(string.Format("创建room的hall当前线程为{0}", Thread.CurrentThread.ManagedThreadId));

                    int room_id = ++room_serial;
                    Room room = new Room(this, room_id, client, setting);
                    RId2Room.Add(room_id, room);
                }
                else
                {
                    //设置错误，通知客户端
                    MyData data = new MyData
                    {
                        Description = PacketDescription.Hall2Cient,
                        Protocol = protocol.CreateRoom,
                    };
                    client.SendSwitchReply(data);
                }
            }
        }

        public void StartGame(Room room)
        {
            //更新房间号至数据库，以备断线重连
            foreach (Client client in room.Clients)
            {
                if (client.UserID > 0)
                {
                    DB.UpdateGameRoom(client.UserName, client.GameRoom);
                }
            }
            
            Thread thread = new Thread(room.Run);
            Room2Thread.Add(room, thread);
            thread.Start();
        }

        //广播状态更改了的room
        public void BroadCastRoom(Room room)
        {
            lock (this)
            {
                RoomList.Instance().AddRoom(room.RoomId, room.Setting.Name, !string.IsNullOrEmpty(room.Setting.PassWord),
                                                room.Setting.GameMode, room.GameStarted, room.Clients.Count, room.Setting.PlayerNum);
                MyData data = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = protocol.UPdateRoomList,
                    Body = new List<string>
                    {
                        room.RoomId.ToString(),
                        room.Setting.Name,
                        (!string.IsNullOrEmpty(room.Setting.PassWord)).ToString(),
                        room.Setting.GameMode,
                        room.GameStarted.ToString(),
                        room.Clients.Count.ToString(),
                        room.Setting.PlayerNum.ToString()
                    }
                };
                foreach (Client client in UId2ClientTable.Values)
                {
                    client.SendProfileReply(data);
                }
            }
        }

        public void RemoveRoom(Room room, Client host, List<Client> clients)
        {
            lock (this)
            {
                OutPut("remove at " + Thread.CurrentThread.ManagedThreadId.ToString());

                //更新该room下用户的房间号为0
                foreach (Client client in clients)
                {
                    if (client.UserID > 0)
                        DB.UpdateGameRoom(client.UserName, room.RoomId);
                }

                //若房主存在，重建一个新的房间
                if (host != null)
                {
                    CreateRoom(host, room.Setting);
                    int id = host.GameRoom;

                    OutPut(string.Format("host {0} {1}", host.Profile.NickName, host.GameRoom));

                    if (id > 0 && id != room.RoomId)
                    {
                        Room new_room = GetRoom(id);
                        foreach (Client client in clients)
                        {
                            if (client == host || client.Status != Client.GameStatus.online) continue;
                            new_room.OnClientRequestInter(client, id, room.Setting.PassWord);
                        }
                    }
                }

                RId2Room.Remove(room.RoomId);
                if (Room2Thread.ContainsKey(room))
                {
                    Thread thread = Room2Thread[room];
                    thread.Abort();
                    Room2Thread.Remove(room);
                    thread = null;
                }
                RoomList.Instance().RemoveRoom(room.RoomId);
                int room_id = room.RoomId;
                room.Dispose();
                room = null;

                MyData data = new MyData
                {
                    Description = PacketDescription.Hall2Cient,
                    Protocol = protocol.UPdateRoomList,
                    Body = new List<string>
                    {
                        room_id.ToString()
                    }
                };
                foreach (Client client in UId2ClientTable.Values)
                {
                    client.SendProfileReply(data);
                }
            }
        }

        #endregion

        #region 添加AI相关
        private int bot_id = 0;
        public int GetBotId()
        {
            lock (this)
            {
                return --bot_id;
            }
        }
        public void AddBot(Client client)
        {
            UId2ClientTable.Add(client.UserID, client);
        }

        public void RemoveBot(Client client)
        {
            UId2ClientTable.Remove(client.UserID);
        }
        #endregion

        #region 更新用户游戏数据
        public void UpdateProfileGamePlay(int uid, int win, bool escaped)
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

                string new_sql = string.Format("update  profile set GamePlay = {0}, Win = {1}, Lose = {2}, Draw = {3}, Escape = {4} where uid = {5}",
                    profile.GamePlay, profile.Win, profile.Lose, profile.Draw, profile.Escape, uid);
                DB.UpdateData(new_sql);

                Client client = GetClient(uid);
                if (client != null)
                    client.UpdateProfileGamePlay(profile);
            }
        }
        #endregion
    }
}