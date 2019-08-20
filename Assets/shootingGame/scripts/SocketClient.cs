using Frankfort.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketClient
{
    private static SocketClient _Instance;
    private ThreadPoolScheduler myThreadScheduler;
    public static SocketClient socketClient
    {
        get {
            if (_Instance == null)
            {
                _Instance = new SocketClient();
                _Instance.myThreadScheduler = Loom.CreateThreadPoolScheduler();
                IPManager.GetInstance.GetMasseage();
                _Instance.SetClient(IPManager.GetInstance.GetValue("Ip"), int.Parse(IPManager.GetInstance.GetValue("Port")));
            }               
            return _Instance;
        }
    }
    public delegate void ClientCallBack(string msg);

    public event ClientCallBack ClientStartCallBack;

    public event ClientCallBack ClientSucCallBack;

    public event ClientCallBack ClientFailCallBack;

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
        Loom.StartSingleThread(bt_connect_Click, System.Threading.ThreadPriority.Normal, true);
    }

    public void SendMsg(string msg)
    {
        bt_send_Click(msg);
    }



    Socket socketSend;
    private void bt_connect_Click()
    {
        Loom.DispatchToMainThread(() => {
            ClientStartCallBack("开始连接");
        });
        
        try
        {
            //创建客户端Socket，获得远程ip和端口号
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(Ip);
            IPEndPoint point = new IPEndPoint(ip, Port);

            socketSend.Connect(point);

            isConnect = true;
            Loom.DispatchToMainThread(() => {
                ClientSucCallBack("The connection is successful.(连接成功)");
            });
           
            Debug.Log("连接成功!");
            //开启新的线程，不停的接收服务器发来的消息
            Thread c_thread = new Thread(Received);
            c_thread.IsBackground = true;
            c_thread.Start();
        }
        catch (Exception)
        {
            Loom.DispatchToMainThread(() => {
                ClientFailCallBack("The connection failed. Please check the IP number and port number.(连接失败，请检查ip号与端口号是否正确)");
            });
           
            Debug.Log("IP或者端口号错误...");
        }
        //isRun = true;
        //while(isRun)
        //Loom.WaitForSeconds(1);

        //EndClient();

    }
    //public bool isRun = false;
    public void EndClient()
    {
        if (socketClient.isConnect)
        {
            socketSend.Shutdown(SocketShutdown.Both);
            socketSend.Close();
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
                if (str == "Indesign start")
                {
                    Loom.DispatchToMainThread(() => {
                        ShootGame._Instance.ClientStart();
                    });
                    bt_send_Click("indesign ok");
                }
               
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