using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client<SEND, RCV> : TCPBase<SEND, RCV>
{
    public delegate void OnConnect();

    private string serverIp;
    private Thread listeningThread;
    public OnConnect onConnect;

    public Client(int id, string serverIp) : base(id)
    {
        this.serverIp = serverIp;
    }

    public void ConnetToServerAsync()
    {
        Debug.Log("Starting new thread...");
        listeningThread = new Thread(new ThreadStart(ConnectToServer));
        listeningThread.IsBackground = true;
        listeningThread.Start();
    }

    private void ConnectToServer()
    {
        try
        {
            Debug.Log("Client opening socket...");
            client = new TcpClient(serverIp, Constants.PORT);
            while (true)
            {
                using (NetworkStream stream = client.GetStream())
                {
                    Debug.Log("Client connected to a stream");
                    if (onConnect != null)
                        onConnect();
                    ReadStream(stream);
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

}