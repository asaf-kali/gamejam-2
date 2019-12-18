using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

[DataContract]
public class MessageClient : MessageBase
{
    public bool IsClient = true;

    public MessageClient(string data) : base(data)
    {

    }

    public static implicit operator MessageClient(string data)
    {
        return new MessageClient(data);
    }
}