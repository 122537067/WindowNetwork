using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormUDPRecv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //动态链接库引入
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        IntPtr hWnd, // handle to destination window 
        int Msg, // message 
        int wParam, // first message parameter 
        int lParam // second message parameter 
        );
        //定义消息常数 
        public const int TRAN_UDP_IN = 0x500;



        protected override void DefWndProc(ref Message m)
        {//窗体消息处理重载
            switch (m.Msg)
            {
                case TRAN_UDP_IN:
                    {
                        label1.Text = strRecvStr;
                        break;
                    }
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        #region  UDP 接收线程
        public static int iUdpRecvPkgLen;
        public static Socket skUdpRecv;
        public static Socket skUdpSend;
        public static byte[] udpRecvDataBuf;
        public static byte[] udpSendDataBuf;
        public static IPEndPoint remoteIPEp;
        public static EndPoint remoteEp;
        public static int CLIENT_UDP_PORT = 0x5555;
        public static string strRecvStr;


        public static void UdpReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                if (skUdpRecv == null)
                {
                    return;
                }

                EndPoint tmpRemoteEp = remoteIPEp;
                remoteIPEp = (IPEndPoint)tmpRemoteEp;
                //准备远方设备的IP地址和端口号，准备回复数据
                IPEndPoint iep = new IPEndPoint(remoteIPEp.Address, CLIENT_UDP_PORT);
                iUdpRecvPkgLen = skUdpRecv.EndReceiveFrom(ar, ref tmpRemoteEp);
                strRecvStr = Encoding.UTF8.GetString(udpRecvDataBuf, 0, iUdpRecvPkgLen);
                SendMessage(mainWndHandle, TRAN_UDP_IN, 0, 0);
                skUdpRecv.BeginReceiveFrom(udpRecvDataBuf, 0, 1024,
                SocketFlags.None, ref remoteEp, UdpReceiveCallBack, new object());
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }
        public static ManualResetEvent mreUdpRecvShutDown;//UDP服务结束命令  
        public static void UdpRecv()
        {
            //绑定接收 UDP 数据回调函数
            udpRecvDataBuf = new byte[1024];
            remoteIPEp = new IPEndPoint(IPAddress.Any, 0);
            remoteEp = (EndPoint)remoteIPEp;
            skUdpRecv = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, CLIENT_UDP_PORT);
            skUdpRecv.Bind(iep);
            string ss = string.Format("{0:x}", iep.Port);
            skUdpRecv.BeginReceiveFrom(udpRecvDataBuf, 0, 1024,
            SocketFlags.None, ref remoteEp, UdpReceiveCallBack, new object());
            //创建发送数据 Socket 对象与数据缓冲区
            udpSendDataBuf = new byte[1024];
            skUdpSend = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
            mreUdpRecvShutDown = new ManualResetEvent(false);
            while (!mreUdpRecvShutDown.WaitOne(1000)) ;
            skUdpRecv.Close();
            skUdpRecv = null;
        }
        #endregion    UDP UDP 接收线程

        public static IntPtr mainWndHandle;
        private void Form1_Load(object sender, EventArgs e)
        {
            mainWndHandle = this.Handle;
            ThreadStart theStart = new ThreadStart(UdpRecv);
            Thread theThr = new Thread(theStart);
            theThr.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mreUdpRecvShutDown.Set();
        }
    }
}
