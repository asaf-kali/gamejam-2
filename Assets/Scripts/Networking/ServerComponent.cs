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
    private Server<GreekMessage> server;

    public Text someText;

    void Start()
    {
        server = new Server<GreekMessage>();
        server.receiver = MessageRecevied;
        server.ListenAsync();
    }

    void MessageRecevied(GreekMessage message)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            string text = "Client " + message.ShortId + " said: " + message.Data;
            someText.text = text;
            Debug.Log(text);
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

}