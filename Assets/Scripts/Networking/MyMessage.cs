using System.Runtime.Serialization;
using System.IO;

[DataContract]
public struct MyMessage
{
    [DataMember]
    public string Data;
}