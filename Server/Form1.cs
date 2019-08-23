using System;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using CommonClassLibrary;
using System.Data;

namespace SanguoshaServer
{
    public class Form1 : Form
    {
        private Thread serverThread;
        private MsgPackServer serverListener;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ListBox LoginMessageBox;
        private ListBox OnlineUsersBox;
        private TabPage DebugPage;
        private ListBox DebugBox;
        private TabPage tabPage3;
        private Button button1;
        private Label label1;
        private GameHall hall;

        public Form1()
        {
            InitializeComponent();

            bool init = false;
            try
            {
                int result = DB.InitDB();
                AddLoginMessage(string.Format("{0} Users' Status Inited", result));            //初始化用户状态
                init = true;
            }
            catch (Exception e)
            {
                AddLoginMessage("数据库初始化失败");
                AddDebugMessage(string.Format("{0} : {1} {2}", e.Message, e.TargetSite, e.Source));
            }

            if (init)
            {
                hall = new GameHall(this);

                serverThread = new Thread(new ThreadStart(StartListen));
                serverThread.Start();

                AddLoginMessage("Socket Server Started");
            }
        }
        

        public void StartListen()
        {
            string[] ip_address = ConfigurationManager.AppSettings["ServerIP"].Split(':');
            serverListener = new MsgPackServer();
            if (!serverListener.Setup(int.Parse(ip_address[1])))
            {
                AddDebugMessage("server setup failed!");
            }

            serverListener.NewSessionConnected += new SessionHandler<MsgPackSession>(Server_NewSessionConnected);
            serverListener.NewRequestReceived += new RequestHandler<MsgPackSession, BinaryRequestInfo>(Server_NewRequestReceived);
            serverListener.SessionClosed += new SessionHandler<MsgPackSession, SuperSocket.SocketBase.CloseReason>(Server_SessionClosed);

            if (!serverListener.Start())
            {
                AddDebugMessage("server started failed!");
            }

        }

        private void Server_SessionClosed(MsgPackSession session, SuperSocket.SocketBase.CloseReason value)
        {
            hall.OnDisconnected(session, value);
        }

        void Server_NewSessionConnected(MsgPackSession session)
        {
            hall.OnConnected(session);
        }

        /// <summary>
        ///客户端请求处理
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="requestInfo">请求信息</param>
        void Server_NewRequestReceived(MsgPackSession session, BinaryRequestInfo requestInfo)
        {
            hall.OnRequesting(session, requestInfo);
        }

        public void AddLoginMessage(string msg)
        {
            if (LoginMessageBox.Items.Count >= 100)
                LoginMessageBox.Items.RemoveAt(0);

            LoginMessageBox.Items.Add(msg);
        }
        public void AddDebugMessage(string msg)
        {
            if (DebugBox.Items.Count >= 100)
                DebugBox.Items.RemoveAt(0);

            DebugBox.Items.Add(msg);
        }
        public void UpdateUser(DataTable list)
        {
            OnlineUsersBox.Items.Clear();
            foreach (DataRow row in list.Rows)
            {
                int uid = int.Parse(row["UserID"].ToString());
                Client client = hall.GetClient(uid);
                if (client == null) continue;
                string nick_name = row["NickName"].ToString();
                int room_id = int.Parse(row["RoomNumber"].ToString());
                string message = string.Format("uid {0}:账号{3} 昵称 {1}  {2}", uid, nick_name, room_id > 0 ? string.Format("房间{0}游戏中", room_id) : "空闲", client.UserName);
                OnlineUsersBox.Items.Add(message);
            }
        }

        #region Windows 窗体设计器生成的代码
        private System.ComponentModel.Container components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (serverListener != null)
                    serverListener.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            LoginMessageBox = new ListBox();
            tabPage2 = new TabPage();
            OnlineUsersBox = new ListBox();
            DebugPage = new TabPage();
            DebugBox = new ListBox();
            tabPage3 = new TabPage();
            label1 = new Label();
            button1 = new Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.DebugPage.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(DebugPage);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(416, 294);
            tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(this.LoginMessageBox);
            tabPage1.Location = new System.Drawing.Point(4, 22);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new System.Drawing.Size(408, 268);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "登录信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LoginMessageBox
            // 
            LoginMessageBox.Dock = DockStyle.Fill;
            LoginMessageBox.FormattingEnabled = true;
            LoginMessageBox.ItemHeight = 12;
            LoginMessageBox.Location = new System.Drawing.Point(3, 3);
            LoginMessageBox.Name = "LoginMessageBox";
            LoginMessageBox.Size = new System.Drawing.Size(402, 262);
            LoginMessageBox.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.OnlineUsersBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(408, 268);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "在线用户";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // OnlineUsersBox
            // 
            OnlineUsersBox.Dock = DockStyle.Fill;
            this.OnlineUsersBox.FormattingEnabled = true;
            this.OnlineUsersBox.ItemHeight = 12;
            this.OnlineUsersBox.Location = new System.Drawing.Point(3, 3);
            this.OnlineUsersBox.Name = "OnlineUsersBox";
            this.OnlineUsersBox.Size = new System.Drawing.Size(402, 262);
            this.OnlineUsersBox.TabIndex = 0;
            // 
            // DebugPage
            // 
            this.DebugPage.Controls.Add(this.DebugBox);
            this.DebugPage.Location = new System.Drawing.Point(4, 22);
            this.DebugPage.Name = "DebugPage";
            DebugPage.Padding = new Padding(3);
            this.DebugPage.Size = new System.Drawing.Size(408, 268);
            this.DebugPage.TabIndex = 2;
            this.DebugPage.Text = "调试信息";
            this.DebugPage.UseVisualStyleBackColor = true;
            // 
            // DebugBox
            // 
            DebugBox.Dock = DockStyle.Fill;
            this.DebugBox.FormattingEnabled = true;
            this.DebugBox.ItemHeight = 12;
            this.DebugBox.Location = new System.Drawing.Point(3, 3);
            this.DebugBox.Name = "DebugBox";
            this.DebugBox.Size = new System.Drawing.Size(402, 262);
            this.DebugBox.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(label1);
            tabPage3.Controls.Add(button1);
            tabPage3.Location = new System.Drawing.Point(4, 22);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new System.Drawing.Size(408, 268);
            tabPage3.TabIndex = 3;
            tabPage3.Text = "功能";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(89, 11);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(311, 26);
            label1.TabIndex = 1;
            label1.Text = "自动生成游戏进行压力测试，每点击一次生成30个游戏房间循环由AI自动进行游戏";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "自动运行";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.TestButtonClick);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(416, 294);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Sanguosha-Server";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.DebugPage.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serverListener != null)
                serverListener.Stop();
        }

        private void TestButtonClick(object sender, EventArgs e)
        {
            hall.AutoTest();
        }
    }
}
