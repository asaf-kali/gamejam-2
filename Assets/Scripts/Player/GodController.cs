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
    public GameObject button;
    public TextMeshProUGUI commanderCommand;

    private const int OPTIONS_NUM = 6;
    private HashSet<GameObject> buttons;

    private string answer;
    private ClientComponent cc;

    void Start()
    {
        button.GetComponent<Button>().onClick.AddListener(ButtonClicked);
        SearchForClientComp();
        CreateButtons();
    }

    private void CreateButtons()
    {
        buttons = new HashSet<GameObject>();
        int y = 750;
        int index = 0;
        for(int i = 0; i < OPTIONS_NUM; i++)
        {
            GameObject newButton = Instantiate(button) as GameObject;
            //newButton.GetComponent<Button>().onClick.AddListener(()=>ButtonClicked(newButton.GetComponentInChildren<TextMeshProUGUI>()));
            newButton.transform.SetParent(canvas.transform, false);
            Vector3 pos = newButton.transform.position;
            pos.y = y;
            pos.x = 275;
            newButton.transform.position = pos;
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = "button" + index.ToString();
            buttons.Add(newButton);
            index++;
            y -= 120;
        }
    }

    //private void ButtonClicked(TextMeshProUGUI buttonText)
    //{
    //    answer = buttonText.text;
    //    Debug.Log("CLICKED! answer = "+ answer);
    //    disableButtons();
    //    SendMessage();
    //}

        public void ButtonClicked()
    {
        Debug.Log("clicked");
    }

    private void SendMessage()
    {
        ClientMessage ans = new ClientMessage();
        ans.Kind = ClientMessage.MessageKind.ANSWER;
        ans.ChosenCommand = answer;
        cc.client.SendMessage(ans);//todo add client id?
    }

    private void disableButtons()
    {
        HashSet<GameObject>.Enumerator enumerator = buttons.GetEnumerator();
        while (enumerator.MoveNext())
            enumerator.Current.GetComponent<Button>().interactable = false;
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
        commanderCommand.gameObject.SetActive(true);
        hideButtons();
        commanderCommand.text = string.Join(",", answers);
        Debug.Log("Commands to show are: " + string.Join(",", answers));
    }

    

    private void DisplayAsGod(HashSet<string> commands)
    {
        commanderCommand.gameObject.SetActive(false);
        showButtons(commands);
        Debug.Log("Commands to show are: " + string.Join(",", commands));
    }

    private void showButtons(HashSet<string> commands)
    {
        HashSet<GameObject>.Enumerator buttonsEnumerator = buttons.GetEnumerator();
        HashSet<string>.Enumerator commandsEnumerator = commands.GetEnumerator();


        while (buttonsEnumerator.MoveNext() && commandsEnumerator.MoveNext())
        {
            GameObject currButton = buttonsEnumerator.Current;
            currButton.SetActive(true);
            currButton.GetComponent<Button>().interactable = true;
            currButton.GetComponentInChildren<TextMeshProUGUI>().text = commandsEnumerator.Current;
        }
    }

    private void hideButtons()
    {
        HashSet<GameObject>.Enumerator enumerator =  buttons.GetEnumerator();
        while (enumerator.MoveNext())
            enumerator.Current.SetActive(false);

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
