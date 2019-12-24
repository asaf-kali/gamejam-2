using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

[DataContract]
public class ClientMessage : BaseMessage
{
    public enum MessageKind
    {
        HelloResponse,
        Answer,
        SisyphusClick
    }

    [DataMember]
    public MessageKind Kind;

    [DataMember]
    public string ChosenCommand;

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