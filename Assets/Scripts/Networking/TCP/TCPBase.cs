using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

using System.Runtime.Serialization;

public class TCPBase<SEND, RCV>
{
    public delegate void MessageReceivedEvent(RCV message);
    public delegate void DiconnectedEvent(TCPBase<SEND, RCV> self);

    protected int id;
    protected TcpClient client;
    public MessageReceivedEvent MessagesHandler = null;
    public DiconnectedEvent DiconnectionHandler = null;

    private Byte[] buffer = new Byte[Constants.BUFFER_SIZE];

    public TCPBase(int id) : this(id, null, null, null)
    {

    }

    public TCPBase(int id, TcpClient client, MessageReceivedEvent messageReceiver, DiconnectedEvent disconnectHandler)
    {
        this.id = id;
        this.client = client;
        this.MessagesHandler = messageReceiver;
        this.DiconnectionHandler = disconnectHandler;
    }

    public void SendMessage(SEND message)
    {
        if (!IsConnected)
        {
            return;
        }
        try
        {
            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                var data = MessageConverter<SEND>.Instance.Serialize(message);
                stream.Write(data, 0, data.Length);
                // Debug.Log("Client " + id + " sent message - should be received by partner");
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
            // Debug.Log("Incoming data at client " + id + " length is " + length);
            var data = new byte[length];
            Array.Copy(buffer, 0, data, 0, length);
            try
            {
                RCV message = MessageConverter<RCV>.Instance.Desrialize(data);
                if (MessagesHandler != null)
                    MessagesHandler(message);
            }
            catch (SerializationException e)
            {
                Debug.LogError(e);
            }
        }
        Debug.Log("Client " + id + " done reading");
        Dispose();
    }

    public bool IsConnected
    {
        get
        {
            if (client == null)
                return false;
            return client.Connected;
        }
    }

    public void Dispose()
    {
        if (DiconnectionHandler != null)
            DiconnectionHandler(this);
        if (client != null)
        {
            client.Dispose();
        }
    }
}