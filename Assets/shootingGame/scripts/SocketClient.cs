using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketClient
{
    private static SocketClient _Instance;

    public static SocketClient socketClient
    {
        get {
            if (_Instance == null)
            {
                _Instance = new SocketClient();
                _Instance.SetClient("127.0.0.1",6000);
            }               
            return _Instance;
        }
    }
    public delegate void ClientCallBack(string msg);
    public event ClientCallBack Client_Ben;
    public event ClientCallBack Client_Suc;
    public event ClientCallBack Client_Fail;

    public bool isConnect = false;

    string Ip;
    int Port;

    public string GetIp
    {
        get { return Ip; }
    }
    public int GetPort
    {
        get { return Port; }
    }

    public void SetClient(string ip,int port)
    {
        Ip = ip;
        Port = port;
    }

    // Use this for initialization
    public void StartClient()
    {
        Thread c_thread = new Thread(bt_connect_Click);
        c_thread.IsBackground = true;
        c_thread.Start();
    }

    public void SendMsg(string msg)
    {
        bt_send_Click(msg);
    }

    Socket socketSend;
    private void bt_connect_Click()
    {
        //Client_Ben("开始");
        try
        {
            //创建客户端Socket，获得远程ip和端口号
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(Ip);
            IPEndPoint point = new IPEndPoint(ip, Port);

            socketSend.Connect(point);

            isConnect = true;
            //Client_Suc("连接成功!");
            Debug.Log("连接成功!");
            //开启新的线程，不停的接收服务器发来的消息
            Thread c_thread = new Thread(Received);
            c_thread.IsBackground = true;
            c_thread.Start();
        }
        catch (Exception)
        {
            //Client_Fail("IP或者端口号错误...");
            Debug.Log("IP或者端口号错误...");
        }

    }

    /// <summary>
    /// 接收服务端返回的消息
    /// </summary>
    void Received()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024 * 3];
                //实际接收到的有效字节数
                int len = socketSend.Receive(buffer);
                if (len == 0)
                {
                    break;
                }
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("客户端打印：" + socketSend.RemoteEndPoint + ":" + str);
            }
            catch { }
        }
    }

    /// <summary>
    /// 向服务器发送消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bt_send_Click(string str)
    {
        try
        {
            string msg = str;
            byte[] buffer = new byte[1024 * 1024 * 3];
            buffer = Encoding.UTF8.GetBytes(msg);
            socketSend.Send(buffer);
        }
        catch { }
    }
}