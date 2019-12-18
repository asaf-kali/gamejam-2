using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public ServerComponent server;
    public TextMeshProUGUI connectedGods;
    private int godsCount = 0;

    void Start()
    {
        server.Init();
        server.server.newClientsHandler = NewClient;
        server.server.disconnectHandler = ClientDiconnect;
    }

    void NewClient()
    {
        godsCount++;
        UpdateText();
    }

    void ClientDiconnect(TCPBase<MessageServer, MessageClient> client)
    {
        godsCount--;
        UpdateText();
    }

    void UpdateText()
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            string text = "";
            if (godsCount == 0)
                text = "0 אלים מחוברים...";
            else if (godsCount == 1)
                text = "אל אחד מחובר...";
            else
                text = godsCount + " אלים מחוברים!";
            connectedGods.text = text;
        });
    }

    public void StartClicked()
    {
        server.MoveToGame();
    }

    void Update()
    {

    }
}
