using System.Runtime.Serialization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

[DataContract]
public class ServerMessage : BaseMessage
{
    public enum MessageKind
    {
        UNDEFINED,
        HELLO,
        NEW_OBSTICLE,
    }

    [DataMember]
    public MessageKind Kind;

    [DataMember]
    public Dictionary<string,string> commandsDictionary;

    public ServerMessage() : this(null)
    {

    }

    public ServerMessage(string data) : base(data)
    {

    }

    public static implicit operator ServerMessage(string data)
    {
        return new ServerMessage(data);
    }
}