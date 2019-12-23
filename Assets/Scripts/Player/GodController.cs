using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GodController : MonoBehaviour
{
    private const int OPTIONS_NUM = 6;

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

    private void DisplayOptions(string correct, string[] allAnswers)
    {
        Debug.Log("My answer is: " + correct);
        HashSet<string> commands = Commands.RandomCommands(OPTIONS_NUM - 1, allAnswers);
        commands.Add(correct);
        Debug.Log("All commands are: " + string.Join(",", commands));
    }

    private void HandleHello(ServerMessage message)
    {
        Debug.Log("Hello message, responding");
        ClientMessage msg = new ClientMessage("Hi!");
        msg.Kind = ClientMessage.MessageKind.HELLO_RESPONSE;
        cc.client.SendMessage(msg);
    }

    private void HandleNewObsticle(ServerMessage message)
    {
        foreach (var entry in message.AnswersDict)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
            if (entry.Key == SystemInfo.deviceUniqueIdentifier)
            {
                DisplayOptions(entry.Value, message.AnswersDict.Values.ToArray());
            }
        }
    }

    void MessageReceived(ServerMessage message)
    {
        Debug.Log("Message from server " + message.ShortId);
        Debug.Log(message.Kind + ": " + message.Data);
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (message.Kind == ServerMessage.MessageKind.HELLO)
                HandleHello(message);
            else if (message.Kind == ServerMessage.MessageKind.NEW_OBSTICLE)
                HandleNewObsticle(message);
        });
    }

    void Update()
    {

    }
}
