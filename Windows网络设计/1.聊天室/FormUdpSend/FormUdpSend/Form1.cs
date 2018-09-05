using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormUdpSend
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region  UDP发送线程
        public static int iUdpRecvPkgLen;
        public static Socket skUdpSend;
        public static byte[] udpSendDataBuf;
        public static IPEndPoint remoteIPEp;  //IP地址和端口的绑定
        public static int CLIENT_UDP_PORT = 0x5555;  //分配给 Port属性,例子用ipv4

        public static ManualResetEvent mreUdpShutDown;//UDP结束命令
        public static ManualResetEvent mreUdpSend;//UDP发送命令
        public static WaitHandle[] whUdp;  //UDP相关句柄

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
            //Parse()把十进制iP转化成IPAddress类，绑定IP地址和端口
            remoteIPEp = new IPEndPoint(IPAddress.Parse("192.168.191.1"), CLIENT_UDP_PORT);
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
        #endregion    UDP发送线程线程 
        public static string strSendTxt;
        private void button1_Click(object sender, EventArgs e)
        {
            mreUdpShutDown.Set();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            strSendTxt = GetIpAddress() +"："+ textBox1.Text + "\r\n";
            mreUdpSend.Set();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //线程开始
            ThreadStart theStart = new ThreadStart(UdpSend);
            Thread theThr = new Thread(theStart);
            theThr.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mreUdpShutDown.Set();
        }
    }
}
