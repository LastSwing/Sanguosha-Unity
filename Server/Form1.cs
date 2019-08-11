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
            LoginMessageBox.Items.Add(msg);
        }
        public void AddDebugMessage(string msg)
        {
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
            tabPage2 = new TabPage();
            DebugPage = new TabPage();
            LoginMessageBox = new ListBox();
            OnlineUsersBox = new ListBox();
            DebugBox = new ListBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            DebugPage.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.DebugPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(416, 294);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.LoginMessageBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "LoginPage";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(408, 268);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "登录信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.OnlineUsersBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "UserPage";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(408, 268);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "在线用户";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.DebugPage.Controls.Add(this.DebugBox);
            this.DebugPage.Location = new System.Drawing.Point(4, 22);
            this.DebugPage.Name = "DebugMesagePage";
            this.DebugPage.Padding = new System.Windows.Forms.Padding(3);
            this.DebugPage.Size = new System.Drawing.Size(408, 268);
            this.DebugPage.TabIndex = 2;
            this.DebugPage.Text = "调试信息";
            this.DebugPage.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.LoginMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoginMessageBox.FormattingEnabled = true;
            this.LoginMessageBox.ItemHeight = 12;
            this.LoginMessageBox.Location = new System.Drawing.Point(3, 3);
            this.LoginMessageBox.Name = "listBox1";
            this.LoginMessageBox.Size = new System.Drawing.Size(402, 262);
            this.LoginMessageBox.TabIndex = 0;
            // 
            // listBox2
            // 
            OnlineUsersBox.Dock = DockStyle.Fill;
            this.OnlineUsersBox.FormattingEnabled = true;
            this.OnlineUsersBox.ItemHeight = 12;
            this.OnlineUsersBox.Location = new System.Drawing.Point(3, 3);
            this.OnlineUsersBox.Name = "listBox2";
            this.OnlineUsersBox.Size = new System.Drawing.Size(402, 262);
            this.OnlineUsersBox.TabIndex = 0;
            // 
            // listBox3
            // 
            DebugBox.Dock = DockStyle.Fill;
            this.DebugBox.FormattingEnabled = true;
            this.DebugBox.ItemHeight = 12;
            this.DebugBox.Location = new System.Drawing.Point(3, 3);
            this.DebugBox.Name = "listBox3";
            this.DebugBox.Size = new System.Drawing.Size(402, 262);
            this.DebugBox.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(416, 294);
            Controls.Add(tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Sanguosha-Server";
            Closing += new System.ComponentModel.CancelEventHandler(Form1_Closing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.DebugPage.ResumeLayout(false);
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
    }
}
