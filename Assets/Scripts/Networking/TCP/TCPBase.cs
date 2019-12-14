using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPBase<T>
{
    public delegate void MessageRceiver(T message);

    protected int id;
    protected TcpClient client;
    public MessageRceiver messageReceiver = null;

    private Byte[] buffer = new Byte[Constants.BUFFER_SIZE];

    public TCPBase(int id) : this(id, null)
    {

    }

    public TCPBase(int id, TcpClient client)
    {
        this.id = id;
        this.client = client;
    }

    public void SendMessage(T message)
    {
        if (client == null)
        {
            return;
        }
        try
        {
            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                var data = MessageConverter<T>.Instance.Serialize(message);
                stream.Write(data, 0, data.Length);
                Debug.Log("Client " + id + " sent message - should be received by partner");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void ReadStream(NetworkStream stream)
    {
        if (!stream.CanRead)
        {
            Debug.Log("Can't read from stream: " + stream);
            return;
        }
        int length;
        while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            Debug.Log("Incoming data at listener " + id + " length is " + length);
            var data = new byte[length];
            Array.Copy(buffer, 0, data, 0, length);
            T message = MessageConverter<T>.Instance.Desrialize(data);
            if (messageReceiver != null)
            {
                messageReceiver(message);
            }
        }
    }
}