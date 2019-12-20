using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

[DataContract]
public class BaseMessage
{
    private const short SHORT_LENGTH = 6;
    
    [DataMember]
    public readonly string Identifier;
    [DataMember]
    public string Data;

    public BaseMessage()
    {
        Identifier = SystemInfo.deviceUniqueIdentifier;
    }

    public BaseMessage(string data) : this()
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
    public static implicit operator BaseMessage(string data)
    {
        return new BaseMessage(data);
    }
}