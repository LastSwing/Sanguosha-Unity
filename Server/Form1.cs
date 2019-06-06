using System;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using CommonClassLibrary;

namespace SanguoshaServer
{
    public class Form1 : System.Windows.Forms.Form
    {
        private Thread serverThread;
        private MsgPackServer serverListener;
        private GameHall hall;

        public Form1()
        {
            InitializeComponent();
            
            int result = DB.InitDB();
            AddLog(String.Format("{0} Users' Status Inited", result));            //初始化用户状态

            hall = new GameHall(this);

            serverThread = new Thread(new ThreadStart(StartListen));
            serverThread.Start();

            AddLog("Socket Server Started");
        }
        

        public void StartListen()
        {
            string[] ip_address = ConfigurationManager.AppSettings["ServerIP"].Split(':');
            serverListener = new MsgPackServer();
            if (!serverListener.Setup(int.Parse(ip_address[1])))
            {
                AddLog("server setup failed!");
            }

            serverListener.NewSessionConnected += new SessionHandler<MsgPackSession>(Server_NewSessionConnected);
            serverListener.NewRequestReceived += new RequestHandler<MsgPackSession, BinaryRequestInfo>(Server_NewRequestReceived);
            serverListener.SessionClosed += new SessionHandler<MsgPackSession, SuperSocket.SocketBase.CloseReason>(Server_SessionClosed);

            if (!serverListener.Start())
            {
                AddLog("server started failed!");
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

        public void AddLog(string msg)
        {
            logBox.Items.Add(msg);
        }

        #region Windows 窗体设计器生成的代码
        private System.Windows.Forms.ListBox logBox;
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
            this.logBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.ItemHeight = 12;
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(416, 294);
            this.logBox.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(416, 294);
            this.Controls.Add(this.logBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Sanguosha-Server";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
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
            serverListener.Stop();
        }
    }
}
