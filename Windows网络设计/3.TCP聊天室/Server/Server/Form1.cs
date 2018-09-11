using System;
using System.Collections;
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

namespace Server
{
    public partial class Form1 : Form
    {
        //文件发送服务端
        //主窗体句柄
        public static IntPtr main_wnd_handle;
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        IntPtr hWnd, // handle to destination window 
        int Msg, // message 
        int wParam, // first message parameter 
        int lParam // second message parameter 
        );
        //定义消息常数 
        public const int BEGIN_LISTEN = 0x500;
        public const int END_LISTEN = 0x501;

        public const int TRAN_CLIENT_ACCEPT = 0x502;
        public const int TRAN_CLIENT_TRAN = 0x503;

        //用于设置传输进度
        public const int RECV_TEXT = 0x504;

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
            User_Terminate_listen = new ManualResetEvent(false);




            socket_list = new ArrayList();
        }

        public static ManualResetEvent User_Terminate_listen;
        public static ArrayList socket_list;
        public static Socket S_Listen_sock;
        public static Socket S_client_sock;
        public static String tran_file_name;

        public static byte[] RecvDataBuf;
        public class Accep_Object { }

        static void thread_listen()
        { //监听线程
            //1.开始监听
            //2.等待用户关闭命令，但是设置为各子线程必须完成状态才结束 

            //监听线程入口
            //线程流程
            //1.获取主机信息
            //2.启动listen
            //3.使用begin_accept完成异步
            //4.检查全局变量，等待停止信号到来
            //5.检查所有已经连接的客启端，向每个客户端发送close命令
            //6.等待客启端关闭...比较困难
            //6.如果所有连接客启端已经关闭，则发出close命令

            IPAddress[] host_ip = Dns.GetHostAddresses(Dns.GetHostName());

            S_Listen_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LingerOption _lingerOption = new LingerOption(true, 3);
            S_Listen_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, _lingerOption);
            S_Listen_sock.Blocking = false;//设定其为异步
            //IPAddress.Parse("127.0.0.1");
            //IPEndPoint host_end = new IPEndPoint(host_ip[0], 8128);
            IPEndPoint host_end = new IPEndPoint(IPAddress.Parse("172.16.136.29"), Int32.Parse("8131"));

            User_Terminate_listen.Reset();
            S_Listen_sock.Bind(host_end);//开始绑定
            S_Listen_sock.Listen(3);//开始监听

            Accep_Object Ac_state = new Accep_Object();
            S_Listen_sock.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    Ac_state);
            SendMessage(main_wnd_handle, BEGIN_LISTEN, 100, 200);

            User_Terminate_listen.WaitOne();
            //关闭所有的子socket，结束监听 
            S_Listen_sock.Close();
            SendMessage(main_wnd_handle, END_LISTEN, 100, 200);
        }




        //负责接收TCP数据的的Receive回调函数 
        public static void ReceiveCallback(IAsyncResult ar)
        {
            Accep_Object oneState = (Accep_Object)ar.AsyncState;
            int readBytesLen = S_client_sock.EndReceive(ar);
            strChatText = Encoding.UTF8.GetString(RecvDataBuf, 0, readBytesLen);
            SendMessage(main_wnd_handle, RECV_TEXT, 0, 0);
            Accep_Object Ac_state = new Accep_Object();
            S_client_sock.BeginReceive(
                RecvDataBuf, 0, 1024, SocketFlags.None,
                new AsyncCallback(ReceiveCallback),
                Ac_state);
        }


        //负责接收连接的回调函数 
        public static void AcceptCallback(IAsyncResult ar)
        {
            //有新的客户端连接 
            // Get the socket that handles the client request.

            if (S_Listen_sock == null)
            {   //在此增加此语句有两个原因
                //1.---Callback是在消息队列里面循环调用的，当发生了主监听不存在的时候，
                //callback仍然会被调用。所以没有真正的我们理解上的客户到来，但是此函数被正实的调用了
                //2.在dotnet平台中资源的回收虽然是由自动回收机制完成的，但是对于socket来说，是调用的
                //底层的socket接口，而这些方法本身具有资源回收的功能，所以在socket方法调用Close方法
                //的时候，socket已经变成空值，也就是NULL，但是Callback却实际上会被调用，
                //因此发生ObjectDisposedException 异常
                //MessageBox.Show("listen Socket is null，监听已经停止");
                //int closesocket(SOCKET s)的作用是关闭指定的socket，并且回收其所有的资源。
                //int shutdown(SOCKET s,  int how)则是禁止在指定的socket s上禁止进行由how指定的操作，但并不对资源进行回收，shutdown之后而closesocket之前s还不能再次connect或者WSAConnect.
                //通过上面的说明，socket.Close方法实际上就是调用了closesocket ，资源当然就不存在了
                //
                S_client_sock = S_Listen_sock.EndAccept(ar);
                MessageBox.Show("新客户已经开始连接服务器");
            }
            else
            {
                //MessageBox.Show("新客户已经连接到服务器");
                //EndAccept方法返回的是一个新的Socket对象，该对象由系统创建，
                //并且其属性由系统设置，与客户端Socket对象配对，例如其远端IP和端口即是
                //对应发起请求的客户端Socket对象的值。
                S_client_sock = S_Listen_sock.EndAccept(ar);
                S_client_sock.Blocking = true;
                RecvDataBuf = new byte[1024];
                Accep_Object Ac_state = new Accep_Object();
                S_client_sock.BeginReceive(
                    RecvDataBuf, 0, 1024, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    Ac_state);

                //每次新的Client到来则启动一个新的线程，利用新的Socket与客户交互
                ThreadStart clientWorkStart = new ThreadStart(thrSendChatText);
                Thread clientThread = new Thread(clientWorkStart);
                clientThread.IsBackground = true;
                clientThread.Start();
            }

        }

        static void thrSendChatText()
        {
            try
            {
                int iRetValue;
                iRetValue = WaitHandle.WaitAny(mreArray, 500);
                byte[] SendDataBuffer = new byte[1024];
                while (iRetValue != 0)
                {
                    switch (iRetValue)
                    {
                        case 1://有数据要发送
                            //消费本次事件状态
                            mreDataSend.Reset();
                            //1将数组值清空
                            Array.Clear(SendDataBuffer, 0, 1024);
                            byte[] bChatText = Encoding.UTF8.GetBytes(strChatText);
                            //client_sock 
                            Array.Copy(bChatText, 0, SendDataBuffer, 0, bChatText.Length);
                            S_client_sock.Send(SendDataBuffer, bChatText.Length, SocketFlags.None);
                            break;
                        default:
                            break;
                    }
                    iRetValue = WaitHandle.WaitAny(mreArray, 500);
                }
            }
            catch (SocketException Se1)
            {
                MessageBox.Show("SocketException:" + Se1.Message);
            }
            catch (Exception Se2)
            {
                MessageBox.Show("服务器端" + Se2.Message);
            }

        }

        private void button_start_Click(object sender, EventArgs e)
        {
            //启动监听线程
            ThreadStart workStart = new ThreadStart(thread_listen);
            Thread workThread = new Thread(workStart);
            workThread.IsBackground = true;
            workThread.Start();
        }

        /// 重写窗体的消息处理函数 
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                //接收自定义消息 ，并显示其参数 
                case BEGIN_LISTEN:
                    //m.WParam, m.LParam; 
                    label_status.Text = "正在监听";
                    break;
                case END_LISTEN:
                    //m.WParam, m.LParam;  
                    label_status.Text = "结束监听";
                    break;
                case TRAN_CLIENT_ACCEPT:
                    //m.WParam, m.LParam; 
                    label_status.Text = "新客户到达";
                    break;
                case TRAN_CLIENT_TRAN:
                    //m.WParam, m.LParam; 
                    label_status.Text = "正在传输中";
                    break;
                case RECV_TEXT:
                    //显示一行聊天内容 
                    textBox_content.AppendText(strChatText + "\r\n");
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            //用户按下按钮停止监听
            User_Terminate_listen.Set();
        }
        public static String strChatText;

        private void button_send_Click(object sender, EventArgs e)
        {
            strChatText = textBox_send.Text.Trim();
            mreDataSend.Set();
        }

        private void button_finish_Click(object sender, EventArgs e)
        {
            mreTerminateThread.Set();
        }
    }
}
