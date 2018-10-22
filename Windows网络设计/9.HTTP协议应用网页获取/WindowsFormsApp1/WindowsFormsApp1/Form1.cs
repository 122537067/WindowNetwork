using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //定义消息常数 
        public const int TRAN_FINISHED = 0x500;

        public static IntPtr main_wnd_handle;
        public static int recv_str_len;
        public static string recv_str;

        public static MemoryStream ms_recv;
        //动态链接库引入
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        IntPtr hWnd, // handle to destination window 
        int Msg, // message 
        int wParam, // first message parameter 
        int lParam // second message parameter 
        );


        protected override void DefWndProc(ref Message m)
        {//窗体消息处理重载
            switch (m.Msg)
            {
                case TRAN_FINISHED:

                    //Encoding gb2312 = Encoding.GetEncoding("gb2312");
                    Encoding utf8Char = Encoding.GetEncoding("utf-8");


                    //recv_str = Encoding.UTF8.GetString(ms_recv.GetBuffer(), 0, recv_str_len);
                    recv_str = utf8Char.GetString(ms_recv.GetBuffer(), 0, recv_str_len);
                    textBox1.Text = recv_str;
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        static void thread_GET_html()
        {
            IPAddress ipadd_dest = Dns.GetHostEntry(url_str).AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipadd_dest, Int32.Parse("80"));
            Socket client_sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //client_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1); 
            try
            {
                client_sock.Blocking = true;
                client_sock.Connect(remoteEP);
                try
                {
                    byte[] SendDataBuffer = new byte[1024];//数据发送缓存
                    byte[] ReadDataBuffer = new byte[1024];//数据接收缓存
                    //byte[] b_cmd=Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\n\r\n");
                    byte[] b_cmd = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\n" +

"Host: www.zsc.edu.cn\r\n" +
"Connection: keep-alive\r\n" +
"Upgrade-Insecure-Requests: 1\r\n" +
"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.3427.400 QQBrowser/9.6.12088.400\r\n" +
"Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n" +
"Accept-Encoding: gzip, deflate, sdch\r\n" +
"Accept-Language: zh-CN,zh;q=0.8\r\n" +
"Cookie: UM_distinctid=15c13b3a89bac-05ade50a24d1a38-791f5833-12c000-15c13b3a89c6d7; PHPSESSID=vf4ttro53u0p4euffijekbd2k7; CNZZDATA4257938=cnzz_eid%3D1583364785-1494976462-http%253A%252F%252F192.168.5.200%252F%26ntime%3D1506383348; Hm_lvt_407473d433e871de861cf818aa1405a1=1505779029,1505964120,1506303243,1506385176; Hm_lpvt_407473d433e871de861cf818aa1405a1=1506385176\r\n" +
                   "\r\n");
                    Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                    client_sock.Send(SendDataBuffer, SocketFlags.None);
                    ms_recv.Seek(0, SeekOrigin.Begin);
                    recv_str_len = 0;
                    int recv_package_len = 0;
                    recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                    while (recv_package_len != 0)
                    {
                        recv_str_len += recv_package_len;
                        ms_recv.Write(ReadDataBuffer, 0, recv_package_len);
                        recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                    }
                    client_sock.Close();
                    SendMessage(main_wnd_handle, TRAN_FINISHED, 100, 100);
                }
                catch (SocketException se3)
                {
                    MessageBox.Show("客户端异常" + se3.Message);
                }
            }
            catch (SocketException se1)
            {
                MessageBox.Show("SocketException 客户端连接不到服务器呢" + se1.Message);
            }
            catch (Exception se2)
            {
                MessageBox.Show("客户端异常" + se2.Message);
            }
        }

        public static string url_str;

        private void Form1_Load(object sender, EventArgs e)
        {
            main_wnd_handle = this.Handle;
            ms_recv = new MemoryStream(5000000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            url_str = textBox2.Text;
            //启动线程			
            Thread workThread = new Thread(new ThreadStart(thread_GET_html));
            workThread.IsBackground = true;
            workThread.Start();
        }
    }
}
