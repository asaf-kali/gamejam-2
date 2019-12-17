using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientComponent : MonoBehaviour
{
    private Client<GreekMessage> client;

    public void Connect(string ip)
    {
        if (client != null)
        {
            client.Dispose();
        }
        client = new Client<GreekMessage>(GetInstanceID(), ip);
        client.ConnetToServerAsync();
    }

    public void SendMessage()
    {
        GreekMessage message = "push";
        client.SendMessage(message);
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        client.Dispose();
    }

}