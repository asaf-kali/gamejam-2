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


    private void DisplayAsCommander(HashSet<string> answers)
    {
        Debug.Log("Commands to show are: " + string.Join(",", answers));
    }

    private void DisplayAsGod(HashSet<string> commands)
    {
        Debug.Log("Commands to show are: " + string.Join(",", commands));
    }

    private void DisplayOptions(string myAnswer, string[] allAnswers)
    {
        Debug.Log("My answer is: " + myAnswer);
        if (myAnswer == Commands.COMMANDER)
        {
            HashSet<string> commands = new HashSet<string>(allAnswers);
            commands.Remove(Commands.COMMANDER);
            DisplayAsCommander(commands);
        }
        else
        {
            HashSet<string> commands = Commands.RandomCommands(OPTIONS_NUM - 1, allAnswers);
            commands.Add(myAnswer);
            DisplayAsGod(commands);
        }
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
            if (entry.Key == Constants.SessionID)
            {
                DisplayOptions(entry.Value, message.AnswersDict.Values.ToArray());
            }
        }
    }

    void MessageReceived(ServerMessage message)
    {
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
