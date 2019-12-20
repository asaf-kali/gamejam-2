using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodController : MonoBehaviour
{
    private ClientComponent cc;

    void Start()
    {
        SearchForClientComp();
    }

    void SearchForClientComp()
    {
        ClientComponent[] ccs = FindObjectsOfType<ClientComponent>();
        if (ccs.Length != 1)
        {
            Debug.LogError("Wrong number of ClientComponent: " + ccs.Length + " (should be 1).");
            return;
        }
        cc = ccs[0];
        cc.client.MessagesHandler = MessageReceived;
    }

    void MessageReceived(ServerMessage message)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            Debug.Log("Message from server " + message.ShortId);
            Debug.Log(message.Kind + ": " + message.Data);
            if (message.Kind == ServerMessage.MessageKind.HELLO)
            {
                Debug.Log("Hello message, responding");
                ClientMessage msg = new ClientMessage("Hi!");
                msg.Kind = ClientMessage.MessageKind.HELLO_RESPONSE;
                cc.client.SendMessage(msg);
            }
        });
    }

    void Update()
    {

    }
}
