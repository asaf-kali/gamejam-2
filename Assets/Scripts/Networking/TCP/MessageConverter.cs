using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System;
using System.Runtime.Serialization.Json;

public class MessageConverter<T>
{
    private static MessageConverter<T> _instance;
    public static MessageConverter<T> Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MessageConverter<T>();
            return _instance;
        }
    }

    private DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

    public byte[] Serialize(object obj)
    {
        using (var ms = new MemoryStream())
        {
            serializer.WriteObject(ms, obj);
            return ms.ToArray();
        }
    }

    public T Desrialize(byte[] data)
    {
        T obj;
        using (var ms = new MemoryStream(data))
        {
            obj = (T)serializer.ReadObject(ms);
        }
        return obj;
    }
}