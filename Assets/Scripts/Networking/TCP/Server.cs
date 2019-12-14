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
    private Thread listeningThread;
    private TcpListener listener;
    private LinkedList<ClientHandler<T>> handlers = new LinkedList<ClientHandler<T>>();
    public ClientHandler<T>.MessageRceiver receiver = null;

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
                ClientHandler<T> handler = new ClientHandler<T>(client);
                handler.messageReceiver = receiver;
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
        foreach (ClientHandler<T> handler in handlers)
        {
            handler.SendMessage(message);
        }
    }

}