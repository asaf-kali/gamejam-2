using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientComponent : MonoBehaviour
{
    private Client client;

    void Start()
    {
        client = new Client(GetInstanceID());
        client.ConnetToServerAsync();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            client.SendMessage();
        }
    }
}