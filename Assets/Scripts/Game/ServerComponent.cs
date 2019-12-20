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
    private AsyncSceneLoader loader;
    public Server<ServerMessage, ClientMessage> server;
    private bool initCalled = false;

    void Start()
    {
        loader = GetComponent<AsyncSceneLoader>();
        Init();
    }

    public void Init()
    {
        if (initCalled)
            return;
        initCalled = true;
        server = new Server<ServerMessage, ClientMessage>();
        server.ListenAsync();
        // server.messageReceiver = MessageRecevied;
        // server.clientConnectedHandler = () => { Debug.Log("New connection arrived!"); };
    }

    void MessageRecevied(ClientMessage message)
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

    public void MoveToGame()
    {
        loader.LoadNext();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ServerMessage message = "Hi from server!";
            server.SendMessage(message);
        }
    }

    void OnDestroy()
    {
        server.Dispose();
    }

}