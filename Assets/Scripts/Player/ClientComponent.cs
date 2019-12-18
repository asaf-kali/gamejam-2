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
    private Client<GreekMessage> client;

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
        client = new Client<GreekMessage>(GetInstanceID(), ip);
        client.onConnect = OnConnect;
        client.ConnetToServerAsync();
    }

    public void SendMessage()
    {
        GreekMessage message = "push";
        client.SendMessage(message);
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