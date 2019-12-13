using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : TCPBase
{
    public string serverIp = "localhost";
    private TcpClient socketConnection;
    private Thread listeningThread;

    public Client(int id) : base(id)
    {

    }

    public void ConnetToServerAsync()
    {
        listeningThread = new Thread(new ThreadStart(ConnectToServer));
        listeningThread.IsBackground = true;
        listeningThread.Start();
    }

    private void ConnectToServer()
    {
        try
        {
            socketConnection = new TcpClient(serverIp, Constants.PORT);
            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    ReadStream(stream);
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {		
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = "This is a message from clients " + id;       
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}