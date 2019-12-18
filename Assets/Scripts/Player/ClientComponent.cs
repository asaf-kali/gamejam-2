using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientComponent : MonoBehaviour
{
    private AsyncSceneLoader loader;
    private Client<MessageClient, MessageServer> client;

    void Start()
    {
        loader = GetComponent<AsyncSceneLoader>();
    }

    public void Connect(string ip)
    {
        if (client != null)
        {
            client.Dispose();
        }
        client = new Client<MessageClient, MessageServer>(GetInstanceID(), ip);
        client.ConnectionHandler = OnConnect;
        client.MessagesHandler = MessageReceived;
        client.ConnetToServerAsync();
    }

    public void SendMessage()
    {
        MessageClient message = "dudi";
        client.SendMessage(message);
    }

    public void MessageReceived(MessageServer message)
    {
        Debug.Log("Message from " + message.ShortId + ": " + message.Data);
    }

    void OnConnect()
    {
        loader.LoadNext();
    }

    void OnDestroy()
    {
        if (client != null)
            client.Dispose();
    }
}