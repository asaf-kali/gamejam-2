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

    [DataMember]
    public MessageKind Kind;

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