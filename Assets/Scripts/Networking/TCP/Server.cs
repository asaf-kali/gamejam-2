using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server
{
    private Thread listeningThread;
    private TcpListener tcpListener;
    private LinkedList<ClientHandler> handlers = new LinkedList<ClientHandler>();

    public void Listen()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Parse(Constants.LOCALHOST), Constants.PORT);
            tcpListener.Start();
            Debug.Log("Server started listening");
            while (true)
            {
                Debug.Log("Server is open to new connections...");
                TcpClient connectedTcpClient = tcpListener.AcceptTcpClient();
                ClientHandler handler = new ClientHandler(connectedTcpClient);
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

    public void SendMessage()
    {
        foreach (ClientHandler handler in handlers)
        {
            handler.SendMessage();
        }
    }

}