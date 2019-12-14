using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientComponent : MonoBehaviour
{
    private Client<MyMessage> client;

    public void Connect(string ip)
    {
        client = new Client<MyMessage>(GetInstanceID(), ip);
        client.ConnetToServerAsync();
    }

    public void SendMessage()
    {
        MyMessage message = new MyMessage();
        message.Data = "Action form " + SystemInfo.deviceUniqueIdentifier.ToString();
        client.SendMessage(message);
    }

    void Update()
    {

    }
}