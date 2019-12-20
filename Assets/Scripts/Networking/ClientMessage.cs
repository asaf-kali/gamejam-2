using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

[DataContract]
public class ClientMessage : BaseMessage
{
    public enum MessageKind
    {
        UNDEFINED,
        HELLO_RESPONSE,
        ANSWER,
    }

    public MessageKind Kind = MessageKind.UNDEFINED;

    public ClientMessage() : this(null)
    {

    }

    public ClientMessage(string data) : base(data)
    {

    }

    public static implicit operator ClientMessage(string data)
    {
        return new ClientMessage(data);
    }
}