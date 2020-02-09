using System.Collections.Generic;

//传输协议封装数据包相关
namespace CommonClassLibrary
{
    public enum TransferType {
        TypeLogin,              //Key == "LOGN"
        TypeMessage,            //Key == "MESS"
        TypeUserProfile,        //Key == "UDATA"
        TypeSwitch,             //Key == "SWTH"
        TypeGameControll,       //Key == "GAME"
        TypeUnknown
    }

    public enum Protocol
    {
        Unknown,

        //登录相关              //type == TypeLogin
        ClientMessage,          //通知客户端各种连接信息 与ClientMessage相关
        #region
        //协议格式：
        //Body[0] = 用户名
        //Body[1] = 密码
        //仅为客户端发送
        #endregion
        Login,                  //登录请求
        #region
        //协议格式：
        //客户端发送格式：
        //Body[0] = 用户名
        //Body[1] = 密码
        #endregion
        Register,               //注册
        #region
        //协议格式：
        //客户端发送格式：
        //Body[0] = 老密码
        //Body[1] = 新密码
        //服务器发送格式
        //Body[0] = "true"为成功，"false"为失败
        #endregion
        PasswordChange,         //密码修改

        #region 发送信息              type == TypeMessage
        //发送格式：
        //Body[0] = 发言人uid
        //Body[1] = 信息内容
        //Body[2] = 接收人uid（Message2Client专有）
        #endregion
        Message2Hall, 
        Message2Client,
        Message2Room,
        MessageSystem,

        //ServerData            //type == TypeUserProfile
        NickName,               //昵称命名
        UserProfile,            //更新用户信息
        GetProfile,             //获取他人的信息

        //Hall                  //type = TypeSwitch
        UpdateUser,             //508 发送给新人某大厅UserList
        UPdateRoomList,         //更新房间信息
        UpdateHallLeave,        //512 发给某大厅的所有人，有他人Leave该大厅
        UpdateHallJoin,         //514 发给某大厅的所有人，有他人Join 该大厅

        //Room
        CreateRoom,            //创建房间
        ConfigChange,           //修改游戏设置
        #region 客户端申请进入房间
        //客户端格式 Body[0] = room_id, Body[1] = password
        //服务器端格式 PacketDescription = Hall2Cient为失败， PacketDescription = Room2Cient为成功
        #endregion
        JoinRoom,              //515 加入房间
        #region 离开房间
        //客户端请求Body为空
        //服务器发送Body为空，被房主踢出
        #endregion
        LeaveRoom,             //519 离开房间
        KickOff,                //踢出房间
        #region 客户端向服务器发送room的ready信息，服务器端通知客户端更新room内clients
        //客户端格式 Body[0] = "true" or "false"
        //服务器端格式 Body[0] = host id, Body[1] = clients[0].profile, Body[2] = clients[0].status ..以此类推
        #endregion
        UpdateRoom,
        #region 客户端向服务器发送身份点选信息
        //客户端格式 Body[0] = "lord" or "rebel"
        #endregion
        RoleReserved,
        #region 客户端向服务器发送点将信息
        //客户端格式 Body = new List<string>{ "caocao" , "liubei"}
        #endregion
        GeneralReserved,

        //Game                  type == TypeGameControll
        GameRequest,
        GameReply,
        GameNotification
    }

    public enum ProcessInstanceType
    {
        S_SERVER_INSTANCE,
        S_CLIENT_INSTANCE
    }

    public enum ClientMessage
    {
        #region
        //用于服务器通知客户端显示连接信息
        //也用于客户端告知用户
        //Body[0] = 信息类型
        //Body[1] = "true" 为现实确定按钮
        #endregion

        ConncetionError,            //连接失败
        Connecting,                 //连接中
        Connected,                  //已连接
        Asking,                     //请求中
        Disconnected,               //断开连接
        LoginDuplicated,            //重复登录
        NoAccount,                  //账号不存在
        PasswordWrong,              //密码错误
        RegisterSuccesful,          //注册
        AccountDuplicated,          //帐号申请重复
        PasswordChangeSuccesful,    //密码修改
        WrongMessage,               //协议错误
        VersionNotMatch             //版本号不匹配
    }

    public enum CheatCode
    {
        S_CHEAT_GET_ONE_CARD,
        S_CHEAT_KILL_PLAYER,
        S_CHEAT_REVIVE_PLAYER,
        S_CHEAT_MAKE_DAMAGE,
        S_CHEAT_RUN_SCRIPT
    };

    public enum CheatCategory
    {
        S_CHEAT_FIRE_DAMAGE,
        S_CHEAT_THUNDER_DAMAGE,
        S_CHEAT_NORMAL_DAMAGE,
        S_CHEAT_HP_RECOVER,
        S_CHEAT_HP_LOSE,
        S_CHEAT_MAX_HP_LOSE,
        S_CHEAT_MAX_HP_RESET
    };

    public enum CommandType
    {
        S_COMMAND_UNKNOWN,
        S_COMMAND_CHOOSE_CARD,
        S_COMMAND_PLAY_CARD,
        S_COMMAND_RESPONSE_CARD,
        S_COMMAND_SHOW_CARD,
        S_COMMAND_EXCHANGE_CARD,
        S_COMMAND_DISCARD_CARD,
        S_COMMAND_INVOKE_SKILL,
        S_COMMAND_MOVE_FOCUS,
        S_COMMAND_CHOOSE_GENERAL,
        S_COMMAND_SELECT_GENERAL,
        S_COMMAND_CHOOSE_KINGDOM,
        S_COMMAND_CHOOSE_SUIT,
        S_COMMAND_CHOOSE_DIRECTION,
        S_COMMAND_CHOOSE_PLAYER,
        S_COMMAND_CHOOSE_ORDER,
        S_COMMAND_ASK_PEACH,
        S_COMMAND_SET_MARK,
        S_COMMAND_SET_FLAG,
        S_COMMAND_SET_STRINGMARK,
        S_COMMAND_NULLIFICATION,
        S_COMMAND_MULTIPLE_CHOICE,
        S_COMMAND_PINDIAN,
        S_COMMAND_AMAZING_GRACE,
        S_COMMAND_SKILL_YIJI,
        S_COMMAND_SKILL_GONGXIN,
        S_COMMAND_SET_PROPERTY,
        S_COMMAND_CHANGE_HP,
        S_COMMAND_CHAIN_REMOVE,
        S_COMMAND_OPERATE,
        S_COMMAND_CHEAT,
        S_COMMAND_SURRENDER,
        S_COMMAND_ENABLE_SURRENDER,
        S_COMMAND_GAME_OVER,
        S_COMMAND_GAME_START,
        S_COMMAND_MOVE_CARD,
        S_COMMAND_GET_CARD,
        S_COMMAND_LOSE_CARD,
        S_COMMAND_LOG_EVENT,
        S_COMMAND_LOG_SKILL,
        S_COMMAND_UPDATE_CARD,
        S_COMMAND_SET_EMOTION,
        S_COMMAND_FILL_AMAZING_GRACE,
        S_COMMAND_CLEAR_AMAZING_GRACE,
        S_COMMAND_TAKE_AMAZING_GRACE,
        S_COMMAND_KILL_PLAYER,
        S_COMMAND_REVIVE_PLAYER,
        S_COMMAND_ATTACH_SKILL,
        S_COMMAND_NULLIFICATION_ASKED,
        S_COMMAND_EXCHANGE_KNOWN_CARDS,
        S_COMMAND_SET_KNOWN_CARDS,
        S_COMMAND_UPDATE_PILE,
        S_COMMAND_UPDATE_ROUND,
        S_COMMAND_RESET_PILE,
        S_COMMAND_UPDATE_STATE_ITEM,
        S_COMMAND_SPEAK,
        S_COMMAND_ARRANGE_GENERAL,
        S_COMMAND_FILL_GENERAL,
        S_COMMAND_TAKE_GENERAL,
        S_COMMAND_RECOVER_GENERAL,
        S_COMMAND_REVEAL_GENERAL,
        S_COMMAND_ANIMATE,
        S_COMMAND_LUCK_CARD,
        S_COMMAND_VIEW_GENERALS,
        S_COMMAND_SET_DASHBOARD_SHADOW,
        S_COMMAND_PRESHOW,
        S_COMMAND_INIT_CARDS,
        S_COMMAND_ADD_ROBOT,
        S_COMMAND_FILL_ROBOTS,
        S_COMMAND_TRUST,
        S_COMMAND_INTELSELECT,
        S_COMMAND_NETWORK_DELAY_TEST,
        S_COMMAND_CHECK_VERSION,
        S_COMMAND_USE_VIRTUAL_CARD,
        S_COMMAND_ARRANGE_SEATS,
        S_COMMAND_WARN,
        S_COMMAND_GAMEMODE_BLOODBATTLE,
        S_COMMAND_DISABLE_SHOW,
        S_COMMAND_TRIGGER_ORDER,
        S_COMMAND_CHANGE_SKIN,
        S_COMMAND_SKILL_MOVECARDS,
        S_COMMAND_SKILL_SORTCARDS,
        S_COMMAND_MIRROR_MOVECARDS_STEP,
        S_COMMAND_SET_VISIBLE_CARDS,
        S_COMMAND_GLOBAL_CHOOSECARD,
        S_COMMAND_CHOOSE_EXTRA_TARGET,
        S_COMMAND_BAN_PICK,
        S_COMMAND_MAPPING_PLAYER,
        S_COMMAND_OWNER_CHANGE,
        S_COMMAND_UPDATE_PRIVATE_PILE,
        S_COMMAND_UPDATE_CARD_FOOTNAME,
        S_COMMAND_SHOWDISTANCE
    };

    public enum GameEventType
    {
        S_GAME_EVENT_PLAYER_DYING,
        S_GAME_EVENT_PLAYER_QUITDYING,
        S_GAME_EVENT_PLAY_EFFECT,
        S_GAME_EVENT_JUDGE_RESULT,
        S_GAME_EVENT_DETACH_SKILL,
        S_GAME_EVENT_ACQUIRE_SKILL,
        S_GAME_EVENT_ADD_SKILL,
        S_GAME_EVENT_LOSE_SKILL,
        S_GAME_EVENT_UPDATE_SKILL,
        S_GAME_EVENT_UPDATE_PRESHOW,
        S_GAME_EVENT_CHANGE_GENDER,
        S_GAME_EVENT_CHANGE_HERO,
        S_GAME_EVENT_PLAYER_REFORM,
        S_GAME_EVENT_SKILL_INVOKED,
        S_GAME_EVENT_PAUSE,
        S_GAME_EVENT_REVEAL_PINDIAN,
        S_GAME_EVENT_ALTER_PINDIAN,
        S_GAME_EVENT_BIG_KINGDOM,
        S_GAME_EVENT_CLIENT_TIP,
        S_GAME_EVENT_CHAIN_ANIMATION,
        S_GAME_EVENT_SHEFU,
        S_GAME_EVENT_SHOWDISTANCE,
        S_GAME_EVENT_SKILL_TURN,
        S_GAME_EVENT_EQUIP_ABOLISH,
        S_GAME_EVENT_HUASHEN,
    };

    public enum MoveCardType
    {
        S_TYPE_MOVE,
        S_TYPE_SORT,
    }

    public enum RequestType
    {
        S_REQUEST_CARD,
        S_REQUEST_DOUBLE_CLICK,
        S_REQUEST_SKILL,
        S_REQUEST_TARGET,
        S_REQUEST_SYS_BUTTON,
        S_REQUEST_SWITCH_CARDS,
        S_REQUEST_HUASHEN,
        S_REQUEST_MOVECARDS,
        S_REQUEST_DASHBOARD_CHANGE
    };

    public enum AnimateType
    {
        S_ANIMATE_NULL,
        S_ANIMATE_INDICATE,
        S_ANIMATE_LIGHTBOX,
        S_ANIMATE_NULLIFICATION,
        S_ANIMATE_FIRE,
        S_ANIMATE_LIGHTNING,
        S_ANIMATE_HUASHEN,
        S_ANIMATE_BATTLEARRAY,
        S_ANIMATE_REMOVE,
        S_ANIMATE_ABUSE,
    };

    public enum Game3v3ChooseOrderCommand
    {
        S_REASON_CHOOSE_ORDER_TURN,
        S_REASON_CHOOSE_ORDER_SELECT
    };

    //所属冷/暖
    public enum Game3v3Camp
    {
        S_CAMP_NONE,
        S_CAMP_WARM,
        S_CAMP_COOL
    };

    //观星步骤
    public enum GuanxingStep
    {
        S_GUANXING_START,
        S_GUANXING_MOVE,
        S_GUANXING_FINISH
    };

    //传输方向
    public enum PacketDescription
    {
        Unknown,
        Client2Hall,
        Client2Room,
        Room2Cient,
        Hall2Cient
    };

    //传输协议用的封装包
    public class MyData
    {
        public PacketDescription Description { get; set; }
        public Protocol Protocol { get; set; }
        public List<string> Body { get; set; }
    }
    public struct Profile
    {
        public int UId { get; set; }
        public string NickName { get; set; }
        public int Right { set; get; }
        public int Avatar { get; set; }
        public int Frame { get; set; }
        public int Bg { get; set; }
        public int GamePlay { get; set; }
        public int Lose { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Escape { get; set; }
        public int Title { get; set; }
        public Dictionary<int, string> Titles { get; set; }
        public Dictionary<int, string> Achievements { get; set; }
    }
}
