using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

public class GameControl : MonoBehaviour
{

    enum Players
    {
        Sisyphus,
        Gods
    }

    private const float TIME_FOR_OBSTICLE = 5f;
    private const int NUMBER_OF_REQUIRED_HITS = 3;
    public const int LINT_ITER_NUMER = 1;
    private const int GAME_TIMEOUT = 50;

    // Singletone
    public static GameControl instance;

    private static System.Random r = new System.Random();

    public GameObject player;
    public GameObject ball;
    public Transform ballPosition;
    public int obsticleTimeout;
    public GameObject lightning;
    public TextMeshProUGUI obsticleTimer;
    public TextMeshProUGUI gameTimer;
    public TextMeshProUGUI hitsNumber;

    private ServerComponent sc;
    private HashSet<string> clients = new HashSet<string>();
    private int godScore = 0;
    private float timeSinceLastObsticle = 0;
    private float gameTime = 0;
    private bool isObsticleActive = false;
    private bool isObsticleHitting = false;
    public bool gameOver { get; private set; }
    private float timeLeftForObsticle = TIME_FOR_OBSTICLE;
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

    void CurrentObsticleCheck()
    {
        if (!isObsticleActive)
            return;
        timeLeftForObsticle -= Time.deltaTime;
        obsticleTimer.text = ((int)timeLeftForObsticle).ToString();
        if (timeLeftForObsticle <= 0)
            DeactivateObsticle();
    }

    int TimeLeft()
    {
        return (int)(GAME_TIMEOUT - gameTime);
    }

    void UpdateTime()
    {
        gameTime += Time.deltaTime;
        gameTimer.text = TimeLeft().ToString();
    }

    void Update()
    {
        if (gameOver)
            return;
        UpdateTime();
        WinnerCheck();
        NewObsticleCheck();
        CurrentObsticleCheck();
    }

    private void WinnerCheck()
    {
        if (godScore == 3)
            GameOver(Players.Gods);
        else if (gameTime >= GAME_TIMEOUT)
            GameOver(Players.Sisyphus);
    }

    private IEnumerator GodsSucceeded()
    {
        Debug.Log("Gods hit!");
        AddHit();
        DeactivateObsticle();
        isObsticleHitting = true;
        lightning.SetActive(true);
        yield return new WaitForSeconds(1);

        MoveSisyphusDown();
        yield return new WaitForSeconds(2);

        isObsticleHitting = false;
        lightning.SetActive(false);
    }

    private void AddHit()
    {
        godScore++;
        hitsNumber.text = godScore.ToString() + "/" + NUMBER_OF_REQUIRED_HITS.ToString();
        WinnerCheck();
    }
    private void MoveSisyphusDown()
    {
        // TODO
    }

    private void SisyphusWins()
    {
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(-3, 1) * 1.5f;
    }

    private void SisyphusDies()
    {
        player.GetComponent<PolygonCollider2D>().enabled = false;
        player.GetComponent<SpriteRenderer>().sortingOrder = 2;
        player.GetComponent<Rigidbody2D>().velocity += new Vector2(0, -8);
        player.GetComponent<Rigidbody2D>().angularVelocity = 150;
    }

    private bool CheckCorrectAnswers()
    {
        if (receivedAnswers == null || correctAnswers == null)
            return false;
        return receivedAnswers.SetEquals(correctAnswers.Values);
    }

    void NewObsticleCheck()
    {
        if (isObsticleActive || isObsticleHitting)
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
        if (clients.Count < 2)
            return;
        isObsticleActive = true;

        string commnader = PickCommander();
        correctAnswers = CreateAnswersDict(commnader);
        receivedAnswers = new HashSet<string>();
        receivedAnswers.Add(Commands.COMMANDER); // Ugly but needed
        ServerMessage msg = new ServerMessage();
        msg.Kind = ServerMessage.MessageKind.NEW_OBSTICLE;
        msg.AnswersDict = correctAnswers;
        sc.server.SendMessage(msg);
        timeLeftForObsticle = TIME_FOR_OBSTICLE;
        ActivateObsticleTimer();
    }

    private void ActivateObsticleTimer()
    {
        obsticleTimer.gameObject.SetActive(true);
        obsticleTimer.text = ((int)timeLeftForObsticle).ToString();
    }

    void DeactivateObsticle()
    {
        isObsticleActive = false;
        obsticleTimer.gameObject.SetActive(false);
        timeSinceLastObsticle = 0;
        var msg = new ServerMessage();
        msg.Kind = ServerMessage.MessageKind.CLEAR;
        sc.server.SendMessage(msg);
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
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (!isObsticleActive)
                return;
            receivedAnswers.Add(message.ChosenCommand);
            if (CheckCorrectAnswers())
                StartCoroutine(GodsSucceeded());
        });

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

    private void GameOver(Players winner)
    {
        Debug.Log("Game over! The winner is: " + winner);
        if (winner == Players.Sisyphus)
            SisyphusWins();
        else SisyphusDies();
        gameOver = true;
        // TODO...
    }

}