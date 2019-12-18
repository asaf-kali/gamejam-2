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
    public delegate void NewClientsHandler();

    private Thread listeningThread;
    private TcpListener listener;
    private LinkedList<ConnectionHandler<T>> handlers = new LinkedList<ConnectionHandler<T>>();
    public NewClientsHandler newClientsHandler = null;
    public TCPBase<T>.MessagesHandler messagesHandler = null;
    public TCPBase<T>.DisconnectHandler disconnectHandler = null;

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
                if (newClientsHandler != null)
                    newClientsHandler();
                ConnectionHandler<T> handler = new ConnectionHandler<T>(client, MessageReceived, DisconnectHandler);
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

    private void MessageReceived(T message)
    {
        if (messagesHandler != null)
            messagesHandler(message);
    }
    private void DisconnectHandler(TCPBase<T> client)
    {
        if (disconnectHandler != null)
            disconnectHandler(client);
    }

    public void SendMessage(T message)
    {
        foreach (var handler in handlers)
        {
            try
            {
                handler.SendMessage(message);
            }
            catch (InvalidOperationException)
            {

            }
        }
        RemoveDisconnectedHandlers();
    }

    private void RemoveDisconnectedHandlers()
    {
        LinkedList<ConnectionHandler<T>> diconnected = new LinkedList<ConnectionHandler<T>>();
        foreach (var handler in handlers)
        {
            if (!handler.IsConnected)
                diconnected.AddLast(handler);
        }
        if (diconnected.Count > 0)
        {
            Debug.Log(diconnected.Count + " disconnected handlers found");
            foreach (var handler in diconnected)
            {
                handlers.Remove(handler);
            }
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