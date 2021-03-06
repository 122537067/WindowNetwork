﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsFormsApp2
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

        public static IntPtr main_wnd_handle;

        public static string send_str;
        public static string recv_str;
        public static string mailbox_str;
        public static int recv_octects = 0;
        public static MemoryStream ms_recv;

        //定义消息常数 
        public const int TRAN_SEND_INFO = 0x500;
        public const int TRAN_REPLY_INFO = 0x501;
        public const int TRAN_MAIL_CONTENT = 0x502;
        public const int MAIL_COUNT = 0x503;


        public static ManualResetEvent MRE_check_end;



        /// 重写窗体的消息处理函数 
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case TRAN_SEND_INFO:
                    textBox1.Text += send_str;
                    break;
                case TRAN_REPLY_INFO:
                    textBox2.Text += recv_str;
                    break;
                case TRAN_MAIL_CONTENT:
                    recv_str = Encoding.UTF8.GetString(ms_recv.GetBuffer(), 0, recv_octects);
                    textBox2.Text += recv_str;
                    break;
                case MAIL_COUNT:
                    label1.Text = mailbox_str;
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        public static string userID = "账号@163.com";
        public static string userPass = "密码";
        static void thread_pop_con()
        {
            //线程流程 
            try
            {
                IPAddress ipadd_dest = Dns.GetHostEntry("pop.163.com").AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipadd_dest, Int32.Parse("110"));
                //连接服务器 
                // Create a TCP/IP socket.
                Socket client_sock = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                client_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                client_sock.Blocking = true;
                client_sock.Connect(remoteEP);
                try
                {
                    //连接逻辑  
                    byte[] SendDataBuffer = new byte[1024];//数据发送缓存
                    byte[] ReadDataBuffer = new byte[1024];//数据接收缓存
                    int recv_package_len = 0;

                    //POP服务器会先发送一些响应字符串到客户端，显示客户端登录正常
                    recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                    recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                    //通知窗体显示收到的字符串
                    SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);


                    //send   CMD: user
                    send_str = "user " + userID + "\r\n";
                    //通知窗体显示发出的字符串
                    SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                    byte[] b_cmd = Encoding.ASCII.GetBytes(send_str);
                    Array.Clear(SendDataBuffer, 0, 1024);
                    Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                    client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                    //接收服务器响应
                    recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                    recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                    //通知窗体显示收到的字符串
                    SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);

                    //send   CMD: pass
                    send_str = "pass " + userPass + "\r\n";
                    SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                    b_cmd = Encoding.ASCII.GetBytes(send_str);
                    Array.Clear(SendDataBuffer, 0, 1024);
                    Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                    client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                    //接收服务器响应
                    recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                    recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                    //通知窗体显示收到的字符串
                    SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);


                    Regex str_num;
                    Match m;
                    Int32 mail_len;
                    String[] result_num;
                    Int32 old_total_mail_len, new_total_mail_len;

                    send_str = "list\r\n";
                    SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                    b_cmd = Encoding.ASCII.GetBytes(send_str);
                    Array.Clear(SendDataBuffer, 0, 1024);
                    Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                    client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                    //接收服务器响应
                    recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                    recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                    result_num = recv_str.Split("\r\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    old_total_mail_len = Int32.Parse(result_num[2]);
                    do
                    {
                        //send   CMD: quit
                        send_str = "quit\r\n";
                        SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                        b_cmd = Encoding.ASCII.GetBytes(send_str);
                        Array.Clear(SendDataBuffer, 0, 1024);
                        Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                        client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                        //接收服务器响应
                        recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                        recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);

                        //检验新邮件 --需要重新TCP连接，否则无法接收到新邮件
                        client_sock.Close();//Socket资源已经释放，要重新构造Socket对象
                        client_sock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);
                        client_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                        client_sock.Blocking = true;
                        client_sock.Connect(remoteEP);


                        //POP服务器会先发送一些响应字符串到客户端，显示客户端登录正常
                        recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                        recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                        //通知窗体显示收到的字符串
                        //SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);


                        //send   CMD: user
                        send_str = "user " + userID + "\r\n";
                        //通知窗体显示发出的字符串
                        SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                        b_cmd = Encoding.ASCII.GetBytes(send_str);
                        Array.Clear(SendDataBuffer, 0, 1024);
                        Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                        client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                        //接收服务器响应
                        recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                        recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                        //通知窗体显示收到的字符串
                        //SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);

                        //send   CMD: pass
                        send_str = "pass " + userPass + "\r\n";
                        SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                        b_cmd = Encoding.ASCII.GetBytes(send_str);
                        Array.Clear(SendDataBuffer, 0, 1024);
                        Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                        client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                        //接收服务器响应
                        recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                        recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                        //通知窗体显示收到的字符串
                        //SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100); 

                        send_str = "list\r\n";
                        SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                        b_cmd = Encoding.ASCII.GetBytes(send_str);
                        Array.Clear(SendDataBuffer, 0, 1024);
                        Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                        client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                        //接收服务器响应
                        recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                        recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                        result_num = recv_str.Split("\r\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        new_total_mail_len = Int32.Parse(result_num[2]);
                        mailbox_str = "你邮件个数是" + result_num[1];
                        SendMessage(main_wnd_handle, MAIL_COUNT, 100, 100);
                        if (new_total_mail_len != old_total_mail_len)
                        {
                            old_total_mail_len = new_total_mail_len;
                            //send   CMD: retr 1  Must handle more than 1024 bytes
                            send_str = "retr 1\r\n";
                            SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                            b_cmd = Encoding.ASCII.GetBytes(send_str);
                            Array.Clear(SendDataBuffer, 0, 1024);
                            Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                            client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                            //接收服务器响应--邮件本身大小
                            //+OK 835 octects
                            recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                            recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                            //SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);
                            str_num = new Regex(@"\D+(?<num_val>\d+)\D+");
                            m = str_num.Match(recv_str);
                            mail_len = Int32.Parse(m.Groups["num_val"].Value);
                            recv_str = "要接收的邮件长度为" + mail_len.ToString() + "字节\r\n";
                            SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);
                            //通知窗体显示收到的邮件长度

                            //Mail body

                            ms_recv.Seek(0, SeekOrigin.Begin);
                            do
                            {
                                recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                                ms_recv.Write(ReadDataBuffer, 0, recv_package_len);
                                recv_octects += recv_package_len;
                            }
                            while (recv_octects < mail_len);
                            //通知窗体显示收到的字符串
                            SendMessage(main_wnd_handle, TRAN_MAIL_CONTENT, 100, 100);
                            //Mail body
                        }

                        Thread.Sleep(5000);
                    } while (MRE_check_end.WaitOne(1) == false);
                    //检验新邮件


                    //send   CMD: quit
                    send_str = "quit\r\n";
                    SendMessage(main_wnd_handle, TRAN_SEND_INFO, 100, 100);
                    b_cmd = Encoding.ASCII.GetBytes(send_str);
                    Array.Clear(SendDataBuffer, 0, 1024);
                    Array.Copy(b_cmd, SendDataBuffer, b_cmd.Length);
                    client_sock.Send(SendDataBuffer, b_cmd.Length, SocketFlags.None);
                    //接收服务器响应
                    recv_package_len = client_sock.Receive(ReadDataBuffer, 1024, SocketFlags.None);
                    recv_str = Encoding.UTF8.GetString(ReadDataBuffer, 0, recv_package_len);
                    //通知窗体显示收到的字符串
                    SendMessage(main_wnd_handle, TRAN_REPLY_INFO, 100, 100);

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

        private void button1_Click(object sender, EventArgs e)
        {
            //启动线程			
            Thread workThread = new Thread(new ThreadStart(thread_pop_con));
            workThread.IsBackground = true;
            workThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //退出
            MRE_check_end.Set();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            main_wnd_handle = this.Handle;
            ms_recv = new MemoryStream(5000000);
            MRE_check_end = new ManualResetEvent(false);
        }
    }
}
