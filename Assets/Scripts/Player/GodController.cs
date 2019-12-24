using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;

public class GodController : MonoBehaviour
{
    public GameObject canvas;
    public GameObject buttonBase;
    public TextMeshProUGUI commandsText;
    public TextMeshProUGUI guidance;

    private const int OPTIONS_NUM = 6;
    private const float BUTTON_Y_DIFF = 100;

    private HashSet<GameObject> buttons = new HashSet<GameObject>();
    private ClientComponent cc;

    void Start()
    {
        SearchForClientComp();
        CreateButtons();
    }

    private void CreateButtons()
    {
        for (int i = 0; i < OPTIONS_NUM; i++)
        {
            GameObject clone = Instantiate(buttonBase) as GameObject;
            clone.SetActive(false);
            clone.transform.SetParent(canvas.transform, false);
            Vector3 newPos = clone.transform.position;
            newPos.y -= i * BUTTON_Y_DIFF;
            clone.transform.position = newPos;
            clone.GetComponentInChildren<TextMeshProUGUI>().text = "";
            clone.GetComponent<Button>().onClick.AddListener(() => { ButtonClicked(clone); });
            buttons.Add(clone);
        }
    }

    public void ButtonClicked(GameObject button)
    {
        String command = button.GetComponentInChildren<TextMeshProUGUI>().text;
        Debug.Log("CLICKED! Answer is: " + command);
        DisableButtons();
        SendAnswer(command);
    }

    private void SendAnswer(string command)
    {
        ClientMessage msg = new ClientMessage();
        msg.Kind = ClientMessage.MessageKind.ANSWER;
        msg.ChosenCommand = command;
        cc.client.SendMessage(msg);
    }

    private void DisableButtons()
    {
        foreach (var button in buttons)
            button.GetComponent<Button>().interactable = false;
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
        HideButtons();
        Debug.Log("Commands to show are: " + string.Join(",", answers));
        guidance.text = "צעק לחבריך!";
        guidance.gameObject.SetActive(true);
        commandsText.text = string.Join("\n", answers);
        commandsText.gameObject.SetActive(true);
    }

    private void ShowCommands(HashSet<string> commands)
    {
        IEnumerator<string> shuffled = commands.ToArray().OrderBy(x => UnityEngine.Random.value).GetEnumerator();
        foreach (var button in buttons)
        {
            if (!shuffled.MoveNext())
                Debug.LogError("Shuffled set not big enough!");
            button.SetActive(true);
            button.GetComponent<Button>().interactable = true;
            button.GetComponentInChildren<TextMeshProUGUI>().text = shuffled.Current;
        }
    }

    private void HideButtons()
    {
        foreach (var button in buttons)
            button.SetActive(false);
    }

    private void DisplayAsGod(HashSet<string> commands)
    {
        guidance.gameObject.SetActive(false);
        commandsText.gameObject.SetActive(false);
        ShowCommands(commands);
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
        guidance.text = "המשחק החל!";
    }

    private void HandleNewObsticle(ServerMessage message)
    {
        foreach (var entry in message.AnswersDict)
            if (entry.Key == Constants.SessionID)
            {
                DisplayOptions(entry.Value, message.AnswersDict.Values.ToArray());
                break;
            }
    }

    private void HandleClear(ServerMessage message)
    {
        HideButtons();
        commandsText.gameObject.SetActive(false);
        guidance.gameObject.SetActive(false);
    }

    void MessageReceived(ServerMessage message)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (message.Kind == ServerMessage.MessageKind.HELLO)
                HandleHello(message);
            else if (message.Kind == ServerMessage.MessageKind.NEW_OBSTICLE)
                HandleNewObsticle(message);
            else if (message.Kind == ServerMessage.MessageKind.CLEAR)
                HandleClear(message);
        });
    }

    void Update()
    {

    }
}
