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
    private readonly Vector3 MOUNTAIN_TOP; // TODO
    private readonly Vector3 MOUNTAIN_BOTTOM; //  TODO

    // Singletone
    public static GameControl instance;
    private static System.Random r = new System.Random();

    public GameObject player;
    public GameObject ball;
    public int obsticleTimeout;
    public GameObject lighting;

    private ServerComponent sc;
    private HashSet<string> clients = new HashSet<string>();
    private int score = 0;
    private float timeSinceLastObsticle = 0f;
    private bool isObsticleActive = false;
    public bool gameOver { get; private set; }
    private float timeLeftForObsticle;
    private Dictionary<string, string> correctAnswers;
    private HashSet<string> receivedAnswers;

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
        Debug.Log("Good job gods!");
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
        if (receivedAnswers == null || correctAnswers == null)
            return false;
        return receivedAnswers.SetEquals(correctAnswers.Values);
    }

    void NewObsticleCheck()
    {
        if (isObsticleActive)
            return;
        timeSinceLastObsticle += Time.deltaTime;
        if (timeSinceLastObsticle >= obsticleTimeout)
            ActivateObsticle();
    }

    Dictionary<string, string> CreateAnswersDict(string commander)
    {
        Dictionary<string, string> answersDict = new Dictionary<string, string>();
        HashSet<string> correct = Commands.RandomCommands(clients.Count - 1);

        HashSet<string>.Enumerator client = clients.GetEnumerator();
        HashSet<string>.Enumerator answer = correct.GetEnumerator();
        string value;
        while (client.MoveNext())
        {
            if (client.Current.Equals(commander))
                value = Commands.COMMANDER;
            else
            {
                if (!answer.MoveNext())
                    Debug.LogAssertion("Clients were " + clients.Count + ", answers were " + correct.Count);
                value = answer.Current;
            }
            answersDict[client.Current] = value;
        }
        return answersDict;
    }

    string PickCommander()
    {
        return clients.ElementAt(r.Next(clients.Count));
    }

    void ActivateObsticle()
    {
        isObsticleActive = true;
        string commnader = PickCommander();
        correctAnswers = CreateAnswersDict(commnader);
        receivedAnswers = new HashSet<string>();
        ServerMessage msg = new ServerMessage();
        msg.Kind = ServerMessage.MessageKind.NEW_OBSTICLE;
        msg.AnswersDict = correctAnswers;
        sc.server.SendMessage(msg);
        timeLeftForObsticle = TIME_FOR_OBSTICLE;
    }

    private void HandleHello(ClientMessage message)
    {
        Debug.Log("Hello response, logging");
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            // This has to be thread safe
            clients.Add(message.Identifier);
        });
    }

    private void HandleAnswer(ClientMessage message)
    {
        Debug.Log(message.ShortId + ": " + message.ChosenCommand);
        if (isObsticleActive)
            receivedAnswers.Add(message.ChosenCommand);
    }

    private void HandleSisyphus(ClientMessage message)
    {
        // Do not implement. No time for this.
    }

    public void MessageReceived(ClientMessage message)
    {
        // Debug.Log("Message from " + message.ShortId);
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

}
