using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientHandler : TCPBase
{
    private static int listenersCounter = 0;
    private TcpClient connectedTcpClient;
    private Thread handleThread;

    public ClientHandler(TcpClient connectedTcpClient) : base(Interlocked.Increment(ref listenersCounter))
    {
        this.connectedTcpClient = connectedTcpClient;
        Debug.Log("Listener " + id + " is connected to a client");
    }

    public void Handle()
    {
        while (!connectedTcpClient.Connected)
        {
            Debug.Log("Connecting...");
        }
        using (NetworkStream stream = connectedTcpClient.GetStream())
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

    public void SendMessage()
    {
        if (connectedTcpClient == null)
        {
            // Debug.Log("No client connected, can't send message");
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                string serverMessage = "This is a message from your server.";
                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

}