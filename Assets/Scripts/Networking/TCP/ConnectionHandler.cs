using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ConnectionHandler<T> : TCPBase<T>
{
    private static int listenersCounter = 0;
    private Thread handleThread;

    public ConnectionHandler(TcpClient client) : this(client, null)
    {

    }

    public ConnectionHandler(TcpClient client, MessageRceiver receiver) : base(Interlocked.Increment(ref listenersCounter), client, receiver)
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
        Debug.Log("Listener " + id + " done listening");
    }

    public void HandleAsync()
    {
        handleThread = new Thread(Handle);
        handleThread.IsBackground = true;
        handleThread.Start();
    }

}