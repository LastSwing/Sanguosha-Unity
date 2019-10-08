using System;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using CommonClassLibrary;
using System.Collections.Generic;

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
        private Button auto_run_button;
        private Label auto_test_label;
        private NumericUpDown classic_number;
        private NumericUpDown guandu_number;
        private NumericUpDown hegemony_number;
        private Label class_label;
        private Label guandu_label;
        private Label hegemony_label;
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
                LogHelper.WriteLog(null, e);
            }

            if (init)
            {
                hall = new GameHall(this);

                serverThread = new Thread(new ThreadStart(StartListen));
                serverThread.Start();

                AddLoginMessage("Socket Server Started");
                AddLoginMessage(string.Format("Server Version {0}", GameHall.Version));
            }
        }


        public void StartListen()
        {
            string ip_address = ConfigurationManager.AppSettings["ServerIP"];
            string port = ConfigurationManager.AppSettings["ServerPort"];
            serverListener = new MsgPackServer();
            var serverConfig = new SuperSocket.SocketBase.Config.ServerConfig
            {
                Port = int.Parse(port), //set the listening port
                Ip = ip_address,
                Name = "GameServer",
                MaxConnectionNumber = 200,
                SendBufferSize = 1024 * 4,
                ReceiveBufferSize = 1024 * 4,
                SendingQueueSize = 128,
                SendTimeOut = 10000
            };
            if (!serverListener.Setup(serverConfig))
            {
                AddDebugMessage("server setup failed!");
            }
            /*
            AddDebugMessage(serverListener.Config.SendTimeOut.ToString());
            AddDebugMessage(serverListener.Config.SendBufferSize.ToString());
            AddDebugMessage(serverListener.Config.SendingQueueSize.ToString());
            AddDebugMessage(serverListener.Config.SyncSend.ToString());
            AddDebugMessage(serverListener.Config.MaxConnectionNumber.ToString());
            AddDebugMessage(serverListener.Config.IdleSessionTimeOut.ToString());
            */
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

        public void UpdateUser(Dictionary<int, string> list)
        {
            OnlineUsersBox.Items.Clear();
            foreach (int uid in list.Keys)
            {
                Client client = hall.GetClient(uid);
                if (client == null) continue;
                string nick_name = list[uid];
                string message = string.Format("uid {0}:账号{2} 昵称 {1}", uid, nick_name, client.UserName);
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LoginMessageBox = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.OnlineUsersBox = new System.Windows.Forms.ListBox();
            this.DebugPage = new System.Windows.Forms.TabPage();
            this.DebugBox = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.guandu_number = new System.Windows.Forms.NumericUpDown();
            this.hegemony_number = new System.Windows.Forms.NumericUpDown();
            this.classic_number = new System.Windows.Forms.NumericUpDown();
            this.guandu_label = new System.Windows.Forms.Label();
            this.hegemony_label = new System.Windows.Forms.Label();
            this.class_label = new System.Windows.Forms.Label();
            this.auto_test_label = new System.Windows.Forms.Label();
            this.auto_run_button = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.DebugPage.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guandu_number)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hegemony_number)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.classic_number)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.DebugPage);
            this.tabControl1.Controls.Add(this.tabPage3);
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
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(408, 268);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "登录信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LoginMessageBox
            // 
            this.LoginMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoginMessageBox.FormattingEnabled = true;
            this.LoginMessageBox.ItemHeight = 12;
            this.LoginMessageBox.Location = new System.Drawing.Point(3, 3);
            this.LoginMessageBox.Name = "LoginMessageBox";
            this.LoginMessageBox.Size = new System.Drawing.Size(402, 262);
            this.LoginMessageBox.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.OnlineUsersBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(408, 268);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "在线用户";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // OnlineUsersBox
            // 
            this.OnlineUsersBox.Dock = System.Windows.Forms.DockStyle.Fill;
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
            this.DebugPage.Padding = new System.Windows.Forms.Padding(3);
            this.DebugPage.Size = new System.Drawing.Size(408, 268);
            this.DebugPage.TabIndex = 2;
            this.DebugPage.Text = "调试信息";
            this.DebugPage.UseVisualStyleBackColor = true;
            // 
            // DebugBox
            // 
            this.DebugBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DebugBox.FormattingEnabled = true;
            this.DebugBox.ItemHeight = 12;
            this.DebugBox.Location = new System.Drawing.Point(3, 3);
            this.DebugBox.Name = "DebugBox";
            this.DebugBox.Size = new System.Drawing.Size(402, 262);
            this.DebugBox.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.guandu_number);
            this.tabPage3.Controls.Add(this.hegemony_number);
            this.tabPage3.Controls.Add(this.classic_number);
            this.tabPage3.Controls.Add(this.guandu_label);
            this.tabPage3.Controls.Add(this.hegemony_label);
            this.tabPage3.Controls.Add(this.class_label);
            this.tabPage3.Controls.Add(this.auto_test_label);
            this.tabPage3.Controls.Add(this.auto_run_button);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(408, 268);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "功能";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // guandu_number
            // 
            this.guandu_number.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.guandu_number.Location = new System.Drawing.Point(24, 120);
            this.guandu_number.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.guandu_number.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.guandu_number.Name = "guandu_number";
            this.guandu_number.Size = new System.Drawing.Size(59, 21);
            this.guandu_number.TabIndex = 7;
            this.guandu_number.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // hegemony_number
            // 
            this.hegemony_number.AutoSize = true;
            this.hegemony_number.Font = new System.Drawing.Font("宋体", 9F);
            this.hegemony_number.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.hegemony_number.Location = new System.Drawing.Point(24, 86);
            this.hegemony_number.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.hegemony_number.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.hegemony_number.Name = "hegemony_number";
            this.hegemony_number.Size = new System.Drawing.Size(59, 21);
            this.hegemony_number.TabIndex = 6;
            this.hegemony_number.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // classic_number
            // 
            this.classic_number.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.classic_number.Location = new System.Drawing.Point(24, 53);
            this.classic_number.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.classic_number.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.classic_number.Name = "classic_number";
            this.classic_number.Size = new System.Drawing.Size(59, 21);
            this.classic_number.TabIndex = 5;
            this.classic_number.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // guandu_label
            // 
            this.guandu_label.AutoSize = true;
            this.guandu_label.Font = new System.Drawing.Font("宋体", 12F);
            this.guandu_label.Location = new System.Drawing.Point(98, 91);
            this.guandu_label.Name = "guandu_label";
            this.guandu_label.Size = new System.Drawing.Size(40, 16);
            this.guandu_label.TabIndex = 4;
            this.guandu_label.Text = "官渡";
            // 
            // hegemony_label
            // 
            this.hegemony_label.AutoSize = true;
            this.hegemony_label.Font = new System.Drawing.Font("宋体", 12F);
            this.hegemony_label.Location = new System.Drawing.Point(98, 125);
            this.hegemony_label.Name = "hegemony_label";
            this.hegemony_label.Size = new System.Drawing.Size(40, 16);
            this.hegemony_label.TabIndex = 8;
            this.hegemony_label.Text = "国战";
            // 
            // class_label
            // 
            this.class_label.AutoSize = true;
            this.class_label.Font = new System.Drawing.Font("宋体", 12F);
            this.class_label.Location = new System.Drawing.Point(98, 58);
            this.class_label.Name = "class_label";
            this.class_label.Size = new System.Drawing.Size(40, 16);
            this.class_label.TabIndex = 2;
            this.class_label.Text = "身份";
            // 
            // auto_test_label
            // 
            this.auto_test_label.Location = new System.Drawing.Point(89, 11);
            this.auto_test_label.Name = "auto_test_label";
            this.auto_test_label.Size = new System.Drawing.Size(311, 26);
            this.auto_test_label.TabIndex = 1;
            this.auto_test_label.Text = "根据下列数字，自动生成对应的游戏房间进行压力测试，循环由AI自动进行游戏";
            // 
            // button1
            // 
            this.auto_run_button.Location = new System.Drawing.Point(8, 14);
            this.auto_run_button.Name = "button1";
            this.auto_run_button.Size = new System.Drawing.Size(75, 23);
            this.auto_run_button.TabIndex = 0;
            this.auto_run_button.Text = "自动运行";
            this.auto_run_button.UseVisualStyleBackColor = true;
            this.auto_run_button.Click += new System.EventHandler(this.TestButtonClick);
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
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guandu_number)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hegemony_number)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.classic_number)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        [STAThread]
        static void Main()
        {
            BindExceptionHandler();//绑定程序中的异常处理
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());
        }

        private static void BindExceptionHandler()
        {
            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            //处理未捕获的异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        /// <summary>
        /// 处理UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogHelper.WriteLog(null, e.Exception as Exception);
        }
        /// <summary>
        /// 处理未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelper.WriteLog(null, e.ExceptionObject as Exception);
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serverListener != null)
                serverListener.Stop();
        }

        private void TestButtonClick(object sender, EventArgs e)
        {
            hall.AutoTest((int)classic_number.Value, (int)guandu_number.Value, (int)hegemony_number.Value);
        }
    }
}
