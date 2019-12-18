using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

[DataContract]
public class MessageBase
{
    private const short SHORT_LENGTH = 6;

    [DataMember]
    public readonly string Identifier;
    [DataMember]
    public string Data;

    public MessageBase()
    {
        Identifier = SystemInfo.deviceUniqueIdentifier;
    }

    public MessageBase(string data) : this()
    {
        Data = data;
    }

    public string ShortId
    {
        get
        {
            if (Identifier.Length <= SHORT_LENGTH)
                return Identifier;
            return Identifier.Substring(Identifier.Length - SHORT_LENGTH);
        }
    }
    public static implicit operator MessageBase(string data)
    {
        return new MessageBase(data);
    }
}