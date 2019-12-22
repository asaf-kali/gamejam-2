using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameControl : MonoBehaviour
{
    private const float TIME_FOR_OBSTICLE = 2f;
    private const int ZOOM_IN = 5;
    private const int ZOOM_OUT = 20;
    private readonly Vector3 MOUNTAIN_TOP;  // TODO
    private readonly Vector3 MOUNTAIN_BOTTOM;  //  TODO

    public static GameControl instance;

    public GameObject player;
    public GameObject ball;
    public int obsticleTimeout;
    public GameObject lighting;

    private ServerComponent sc;
    private HashSet<string> clients = new HashSet<string>();
    private ArrayList allCommands;
    private int score = 0;
    private float timeSinceLastObsticle = 0f;
    private bool isObsticleActive = false;
    public bool gameOver { get; private set; }
    private string sisyphus = "";
    private float timeLeftForObsticle;
    private Dictionary<string, string> correctAnswers;
    private Dictionary<string, string> receivedAnswers;

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
        CreateComamnds();
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
        CheckWinner();
        if (isObsticleActive)
        {
            bool success = CheckCorrectAnswers();
            timeLeftForObsticle -= Time.deltaTime;
            if (success || timeLeftForObsticle <= 0)
            {
                isObsticleActive = false;
                timeSinceLastObsticle = 0;
                if (success)
                    GodsSucceeded();
            }
        }
        NewObsticleCheck();
    }

    private void CheckWinner()
    {
        if (player.transform.position == MOUNTAIN_TOP)  // TODO: Replace == with .distance() < some_distance
            GameOver();
        if (player.transform.position == MOUNTAIN_BOTTOM)
            GameOver();
    }

    private void GodsSucceeded()
    {
        // TODO lighting
        lighting.SetActive(true);

        // Zoom out
        Camera.main.orthographicSize = ZOOM_OUT;

        // TODO wait a few seconds
        // TODO move sisyphus back
        MoveSisyphusDown();

        // Zoom in
        Camera.main.orthographicSize = ZOOM_IN;
        lighting.SetActive(false);

    }

    private void MoveSisyphusDown()
    {
        // TODO
    }

    private bool CheckCorrectAnswers()
    {
        return Enumerable.SequenceEqual(correctAnswers, receivedAnswers);
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

    HashSet<string> RandomAnswers()
    {
        HashSet<string> answers = new HashSet<string>();
        System.Random r = new System.Random();
        while (answers.Count < clients.Count - 1)
        {
            int index = r.Next(allCommands.Count);
            answers.Add((string)allCommands[index]);
        }
        return answers;
    }

    Dictionary<string, string> CreateAnswersDict()
    {
        Dictionary<string, string> answersDict = new Dictionary<string, string>();
        HashSet<string> correct = RandomAnswers();

        HashSet<string>.Enumerator client = clients.GetEnumerator();
        HashSet<string>.Enumerator answer = correct.GetEnumerator();
        while (client.MoveNext())
        {
            if (client.Current.Equals(sisyphus))
                continue;
            if (!answer.MoveNext())
                Debug.LogAssertion("Clients were longer then answers!");
            answersDict[client.Current] = answer.Current;
        }
        return answersDict;
    }

    void ActivateObsticle()
    {
        isObsticleActive = true;
        correctAnswers = CreateAnswersDict();
        receivedAnswers = new Dictionary<string, string>();
        ServerMessage msg = new ServerMessage();
        msg.Kind = ServerMessage.MessageKind.NEW_OBSTICLE;
        msg.AnswersDict = correctAnswers;
        sc.server.SendMessage(msg);
        timeLeftForObsticle = TIME_FOR_OBSTICLE;
    }

    private void HandleHello(ClientMessage message)
    {
        // Selects which player is sisyphus
        if (!Equals("", sisyphus))
            sisyphus = message.Identifier;

        Debug.Log("Hello response, logging");
        clients.Add(message.Identifier);
    }

    private void HandleAnswer(ClientMessage message)
    {
        if (isObsticleActive)
        {
            if (!message.Identifier.Equals(sisyphus))
                receivedAnswers.Add(message.Identifier, message.ChosenCommand);
        }
    }

    private void HandleSisyphus(ClientMessage message)
    {
        // TODO
    }

    public void MessageReceived(ClientMessage message)
    {
        Debug.Log("Message from " + message.ShortId);
        if (message.Kind == ClientMessage.MessageKind.HELLO_RESPONSE)
            HandleHello(message);
        else if (message.Kind == ClientMessage.MessageKind.ANSWER)
            HandleAnswer(message);
        else if (message.Kind == ClientMessage.MessageKind.SISYPHUS_CLICK)
            HandleSisyphus(message);
        else Debug.LogWarning("Uknown message kind: " + message.Kind);
    }

    public void PushBall()
    {
        ball.GetComponent<Rigidbody2D>().velocity += new Vector2(-5f, 5f);
    }

    public void GameOver()
    {
        Debug.Log("Game over");
        gameOver = true;
        // TODO...
    }

    private void CreateComamnds()
    {
        allCommands = new ArrayList();
        allCommands.Add("אודיסיאה");
        allCommands.Add("אוהמרוס");
        // TODO...

    }

}
