using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

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