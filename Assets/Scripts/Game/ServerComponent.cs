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
    public Server<GreekMessage> server;
    private bool initCalled = false;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (initCalled)
            return;
        initCalled = true;
        server = new Server<GreekMessage>();
        server.ListenAsync();
        // server.messageReceiver = MessageRecevied;
        // server.clientConnectedHandler = () => { Debug.Log("New connection arrived!"); };
    }

    void MessageRecevied(GreekMessage message)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            string text = "Client " + message.ShortId + " said: " + message.Data;
            Debug.Log(text);
            if (message.Data == "push")
            {
                GameControl.instance.PushBall();
            }
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GreekMessage message = "Server brodcast!";
            server.SendMessage(message);
        }
    }

    void OnDestroy()
    {
        server.Dispose();
    }

}