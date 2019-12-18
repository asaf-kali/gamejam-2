using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

[DataContract]
public class MessageServer : MessageBase
{
    public bool IsServer = true;

    public MessageServer(string data) : base(data)
    {

    }

    public static implicit operator MessageServer(string data)
    {
        return new MessageServer(data);
    }
}