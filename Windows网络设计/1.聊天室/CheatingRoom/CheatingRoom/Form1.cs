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

namespace CheatingRoom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region UDP发送线程
        //发送  相关变量
        public static Socket skUdpSend;
        public static byte[] udpSendDataBuf;
        public static IPEndPoint remoteIPEp;  //IP地址和端口的绑定
        public static int CLIENT_UDP_PORT = 0x5555;  //分配给 Port属性,例子用ipv4

        public static ManualResetEvent mreUdpShutDown;//UDP结束命令
        public static ManualResetEvent mreUdpSend;//UDP发送命令
        public static WaitHandle[] whUdp;  //UDP相关句柄

        public static string strSendTxt;  //发送内容

        //获取当前IPv4地址
        private string GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
                                                                    //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];

            return localaddr.ToString();
        }

        public void UdpSend()
        {
            string ip = textBox_ip.Text.Trim();  //获取ip地址
            //Parse()把十进制iP转化成IPAddress类，绑定IP地址和端口
            remoteIPEp = new IPEndPoint(IPAddress.Parse(ip), CLIENT_UDP_PORT);
            //创建发送数据 Socket 对象与数据缓冲区
            udpSendDataBuf = new byte[1024];
            skUdpSend = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);

            //为false，等待执行
            mreUdpShutDown = new ManualResetEvent(false);
            mreUdpSend = new ManualResetEvent(false);
            whUdp = new WaitHandle[2];
            whUdp[0] = mreUdpShutDown;//UDP结束命令
            whUdp[1] = mreUdpSend; //UDP服务结束命令 

            int iWaitRetCode;
            iWaitRetCode = WaitHandle.WaitAny(whUdp, 1000);   //等待指定数组中的任意元素接收信号，同时使用 TimeSpan 指定时间间隔
            byte[] b_txt;
            int iByteLen;

            //满足条件
            while (iWaitRetCode != 0)
            {
                switch (iWaitRetCode)
                {
                    case 1://发送数据 
                        b_txt = Encoding.UTF8.GetBytes(strSendTxt);
                        iByteLen = b_txt.Length;
                        //初始化缓存数据
                        Array.Clear(udpSendDataBuf, iWaitRetCode, iByteLen);
                        //复制数组
                        Array.Copy(b_txt, udpSendDataBuf, iByteLen);
                        //发送到指定IP
                        skUdpSend.SendTo(udpSendDataBuf, remoteIPEp);
                        //消费掉一次事件
                        mreUdpSend.Reset();
                        break;
                    case WaitHandle.WaitTimeout://超时
                        break;
                }
                //继续下次事件检测
                iWaitRetCode = WaitHandle.WaitAny(whUdp, 1000);
            }
            skUdpSend.Close();
            skUdpSend = null;
        }

        private void button_ip_Click(object sender, EventArgs e)
        {
            if(button_ip.Text.Trim() == "确定")
            {
                //发送线程
                ThreadStart theStart = new ThreadStart(UdpSend);
                Thread theThr = new Thread(theStart);
                theThr.Start();

                //接收线程
                mainWndHandle = this.Handle;
                ThreadStart cheatStart = new ThreadStart(UdpRecv);
                Thread cheatThr = new Thread(cheatStart);
                cheatThr.Start();


                //按钮可用
                button_ip.Text = "解绑";
                textBox_send.ReadOnly = false;
                button_send.Enabled = true;
                textBox_ip.ReadOnly = true;

            }
            else if(button_ip.Text.Trim() == "解绑")
            {
                //按钮不可用
                button_ip.Text = "确定";
                textBox_ip.ReadOnly = false;
                textBox_send.ReadOnly = true;
                button_send.Enabled = false;
                mreUdpRecvShutDown.Set();
                mreUdpShutDown.Set();
            }
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            //发送
            strSendTxt = GetIpAddress() + "：" + textBox_send.Text + "\r\n";
            mreUdpSend.Set();
        }
        #endregion



        #region 接收
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

        public static int iUdpRecvPkgLen;
        public static byte[] udpRecvDataBuf;
        public static Socket skUdpRecv;
        public static IPEndPoint remoteIPEp_r;
        public static EndPoint remoteEp;
        public static string strRecvStr;

        protected override void DefWndProc(ref Message m)
        {//窗体消息处理重载
            switch (m.Msg)
            {
                case TRAN_UDP_IN:
                    {
                        textBox_cheat.Text +=  strRecvStr;
                        break;
                    }
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        public static void UdpReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                if (skUdpRecv == null)
                {
                    return;
                }

                EndPoint tmpRemoteEp = remoteIPEp_r;
                remoteIPEp_r = (IPEndPoint)tmpRemoteEp;
                //准备远方设备的IP地址和端口号，准备回复数据
                IPEndPoint iep = new IPEndPoint(remoteIPEp_r.Address, CLIENT_UDP_PORT);
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
            remoteIPEp_r = new IPEndPoint(IPAddress.Any, 0);
            remoteEp = (EndPoint)remoteIPEp_r;
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
        public static IntPtr mainWndHandle;
        #endregion    UDP UDP 接收线程

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(button_ip.Text.Trim()=="解绑")
            {
                mreUdpShutDown.Set();
                mreUdpRecvShutDown.Set();
            }
        }
    }
}


