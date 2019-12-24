using System.Runtime.Serialization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

[DataContract]
public class ServerMessage : BaseMessage
{
    public enum MessageKind
    {
        Hello,
        NewObsticle,
        Clear,
        GameOver,
    }

    [DataMember]
    public MessageKind Kind;

    [DataMember]
    public Dictionary<string, string> AnswersDict;

    [DataMember]
    public GameControl.Players Winner;

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