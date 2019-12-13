using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class ServerComponent : MonoBehaviour
{
    private Server server;

    void Start()
    {
        server = new Server();
        server.ListenAsync();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            server.SendMessage();
        }
    }

}