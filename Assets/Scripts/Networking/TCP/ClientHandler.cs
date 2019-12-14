using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientHandler<T> : TCPBase<T>
{
    private static int listenersCounter = 0;
    private Thread handleThread;

    public ClientHandler(TcpClient connectedTcpClient) : base(Interlocked.Increment(ref listenersCounter), connectedTcpClient)
    {
        Debug.Log("Listener " + id + " is connected to a client");
    }

    public void Handle()
    {
        while (!client.Connected)
        {
            Debug.Log("Connecting...");
        }
        using (NetworkStream stream = client.GetStream())
        {
            Debug.Log("Listener " + id + " is connected to client stream");
            ReadStream(stream);
        }
    }

    public void HandleAsync()
    {
        handleThread = new Thread(Handle);
        handleThread.IsBackground = true;
        handleThread.Start();
    }

}