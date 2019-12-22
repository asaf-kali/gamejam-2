using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

[DataContract]
public struct GreekMessage
{
    private const short SHORT_LENGTH = 6;

    [DataMember]
    public string Identifier;
    [DataMember]
    public string Data;

    public string ShortId
    {
        get
        {
            if (Identifier.Length <= SHORT_LENGTH)
                return Identifier;
            return Identifier.Substring(Identifier.Length - SHORT_LENGTH);
        }
    }

    public static implicit operator GreekMessage(string data)
    {
        return new GreekMessage() { Identifier = SystemInfo.deviceUniqueIdentifier, Data = data };
    }
}