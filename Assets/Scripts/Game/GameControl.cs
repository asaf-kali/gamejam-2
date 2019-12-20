using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    public GameObject player;
    public GameObject ball;
    public int obsticleTimeout;

    private ServerComponent sc;
    private HashSet<string> clients = new HashSet<string>();
    private int score = 0;
    private float timeSinceLastObsticle = 0f;
    private bool isObsticleActive = false;
    public bool gameOver { get; private set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        gameOver = false;
    }

    void Start()
    {
        SearchForServerComp();
        ListClients();
    }

    void ListClients()
    {
        ServerMessage hi = new ServerMessage();
        hi.Kind = ServerMessage.MessageKind.HELLO;
        sc.server.SendMessage(hi);
    }

    void SearchForServerComp()
    {
        ServerComponent[] scs = FindObjectsOfType<ServerComponent>();
        if (scs.Length != 1)
        {
            Debug.LogError("Wrong number of ServerComponents: " + scs.Length + " (should be 1).");
            return;
        }
        sc = scs[0];
        sc.server.MessagesHandler = MessageReceived;
    }

    void Update()
    {
        NewObsticleCheck();
    }

    void NewObsticleCheck()
    {
        if (isObsticleActive)
            return;
        timeSinceLastObsticle += Time.deltaTime;
        if (timeSinceLastObsticle >= obsticleTimeout)
        {
            ActivateObsticle();
        }
    }

    void ActivateObsticle()
    {
        isObsticleActive = true;
        // sc.server.SendMessage();
    }

    public void MessageReceived(ClientMessage message)
    {
        Debug.Log("Message from " + message.ShortId);
        if (message.Kind == ClientMessage.MessageKind.HELLO_RESPONSE)
        {
            Debug.Log("Hello response, logging");
            clients.Add(message.Identifier);
        }
    }
    public void NotifyClients()
    {
        // **** Implement here command to clients like this: ****
        ServerMessage command = new ServerMessage("Do something");
        sc.server.SendMessage(command);
    }

    public void PushBall()
    {
        ball.GetComponent<Rigidbody2D>().velocity += new Vector2(-5f, 5f);
    }

    public void GameOver()
    {
        Debug.Log("Game over");
        gameOver = true;
    }

}
