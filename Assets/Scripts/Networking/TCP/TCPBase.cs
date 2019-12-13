using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPBase
{
    protected int id;
    private Byte[] buffer = new Byte[Constants.BUFFER_SIZE];

    public TCPBase(int id)
    {
        this.id = id;
    }

    public void ReadStream(NetworkStream stream)
    {
        if (!stream.CanRead)
        {
            Debug.Log("Can't read from stream: " + stream);
            return;
        }
        int length;
        // Read incomming stream into byte arrary. 					
        while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            var incommingData = new byte[length];
            Debug.Log("Incoming data length is " + length);
            Array.Copy(buffer, 0, incommingData, 0, length);
            string message = Encoding.ASCII.GetString(incommingData);
            Debug.Log("Message received at " + this.id + ": " + message);
        }
    }
}