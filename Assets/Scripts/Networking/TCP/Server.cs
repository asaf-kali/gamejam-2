using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server<T>
{
    public delegate void ClientConnectedHandler();

    private Thread listeningThread;
    private TcpListener listener;
    private LinkedList<ConnectionHandler<T>> handlers = new LinkedList<ConnectionHandler<T>>();
    public TCPBase<T>.MessageRceiver messageReceiver = null;
    public ClientConnectedHandler clientConnectedHandler = null;

    public void Listen()
    {
        try
        {
            listener = new TcpListener(IPAddress.Any, Constants.PORT);
            listener.Start();
            Debug.Log("Server started listening");
            while (true)
            {
                Debug.Log("Server is open to new connections...");
                TcpClient client = listener.AcceptTcpClient();
                if (clientConnectedHandler != null)
                    clientConnectedHandler();
                ConnectionHandler<T> handler = new ConnectionHandler<T>(client, messageReceiver);
                handlers.AddLast(handler);
                handler.HandleAsync();
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
        Debug.Log("Server done listeneing");
    }
    public void ListenAsync()
    {
        listeningThread = new Thread(Listen);
        listeningThread.IsBackground = true;
        listeningThread.Start();
    }

    public void SendMessage(T message)
    {
        foreach (var handler in handlers)
        {
            handler.SendMessage(message);
        }
    }

    public void Dispose()
    {
        listener.Stop();
        foreach (var handler in handlers)
        {
            handler.Dispose();
        }
    }

}