using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using UnityEngine;

public class ServerComponent : MonoBehaviour
{
    private Server<MyMessage> server;

    public Text someText;

    void Start()
    {
        server = new Server<MyMessage>();
        server.receiver = MessageRecevied;
        server.ListenAsync();
    }

    void MessageRecevied(MyMessage message)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            someText.text = message.Data;
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyMessage message = new MyMessage();
            message.Data = "Action form " + SystemInfo.deviceUniqueIdentifier.ToString();
            server.SendMessage(message);
        }
    }

}