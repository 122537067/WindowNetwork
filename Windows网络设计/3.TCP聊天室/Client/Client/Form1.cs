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

namespace Client
{
    public partial class Form1 : Form
    {
        //文件发送客户端，负责与服务器连接，传送文件
        //主窗体句柄
        public static IntPtr main_wnd_handle;
        public static IntPtr main_label2_handle;
        public static String tran_file_name;
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(
        IntPtr hWnd, // handle to destination window 
        int Msg, // message 
        int wParam, // first message parameter 
        int lParam // second message parameter 
        );
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        IntPtr hWnd, // handle to destination window 
        int Msg, // message 
        int wParam, // first message parameter 
        int lParam // second message parameter 
        );
        //定义消息常数 
        public const int RECV_TEXT = 0x500;
        public class Accep_Object { }
        public static byte[] RecvDataBuf;

        //负责接收TCP数据的的Receive回调函数 
        public static void ReceiveCallback(IAsyncResult ar)
        {
            int readBytesLen = client_sock.EndReceive(ar);
            strChatText = Encoding.UTF8.GetString(RecvDataBuf, 0, readBytesLen);
            SendMessage(main_wnd_handle, RECV_TEXT, 0, 0);
            Accep_Object Ac_state = new Accep_Object();
            client_sock.BeginReceive(
                RecvDataBuf, 0, 1024, SocketFlags.None,
                new AsyncCallback(ReceiveCallback),
                Ac_state);
        }

        public static Socket client_sock;


        static void thrDataSend()
        {//线程入口
            //线程流程 
            IPEndPoint remoteEP = new IPEndPoint(ServerIP, Int32.Parse("8131"));

            // Create a TCP/IP socket.
            client_sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client_sock.Blocking = true;
                client_sock.Connect(remoteEP);
                RecvDataBuf = new byte[1024];
                Accep_Object Ac_state = new Accep_Object();
                client_sock.BeginReceive(
                    RecvDataBuf, 0, 1024, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    Ac_state);
                //数据暂存缓存，开始数据打包
                byte[] SendDataBuffer = new byte[1024];

                try
                {
                    int iRetValue;
                    iRetValue = WaitHandle.WaitAny(mreArray, 500);

                    while (iRetValue != 0)
                    {
                        switch (iRetValue)
                        {
                            case 1://有数据要发送
                                //消费本次发送事件
                                mreDataSend.Reset();
                                //1将数组值清空
                                Array.Clear(SendDataBuffer, 0, 1024);
                                byte[] bChatText = Encoding.UTF8.GetBytes(strChatText);
                                //client_sock 
                                Array.Copy(bChatText, 0, SendDataBuffer, 0, bChatText.Length);
                                client_sock.Send(SendDataBuffer, bChatText.Length, SocketFlags.None);
                                break;
                            default:
                                break;
                        }
                        iRetValue = WaitHandle.WaitAny(mreArray, 500);
                    }
                }
                catch (SocketException se3)
                {
                    MessageBox.Show("客户端异常3" + se3.Message);

                }
            }
            catch (SocketException se1)
            {
                MessageBox.Show("SocketException" + se1.Message);
                MessageBox.Show(se1.ErrorCode.ToString());
                //10061
                //Winsock Reference 
                //WSAECONNREFUSED 10061
                //Winsock2.h  
            }

            catch (Exception se2)
            {
                MessageBox.Show("客户端异常1" + se2.Message);
            }
        }
        /// 重写窗体的消息处理函数 
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case RECV_TEXT:
                    textBox_content.AppendText(strChatText + "\r\n");
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        public static ManualResetEvent mreTerminateThread;
        public static ManualResetEvent mreDataSend;
        public static ManualResetEvent[] mreArray;

        private void Form1_Load(object sender, EventArgs e)
        {
            main_wnd_handle = this.Handle;
            mreArray = new ManualResetEvent[2];
            mreTerminateThread = new ManualResetEvent(false);
            mreDataSend = new ManualResetEvent(false);
            mreArray[0] = mreTerminateThread;
            mreArray[1] = mreDataSend;
        }
        public static string strServerIP;
        public static IPAddress ServerIP;

        private void button_connect_Click(object sender, EventArgs e)
        {
            strServerIP = textBox_ip.Text.Trim();
            if (IPAddress.TryParse(strServerIP, out ServerIP))
            {
                //2.开始文件传输线程
                ThreadStart workStart = new ThreadStart(thrDataSend);
                Thread workThread = new Thread(workStart);
                workThread.IsBackground = true;
                workThread.Start();
            }
            else
            {
                label_message.Text = "IP地址格式有误。";
            }
        }
        public static String strChatText;
        private void button_send_Click(object sender, EventArgs e)
        {
            strChatText = textBox_send.Text;
            mreDataSend.Set();
        }

        private void button_interrupt_Click(object sender, EventArgs e)
        {
            mreTerminateThread.Set();
        }
    }
}
