using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    public GameObject player;
    public GameObject ball;
    public int obsticleTimeout;
    public GameObject lighting;
    public Camera camera;
    public Transform playerTransform;

    private ServerComponent sc;
    private HashSet<string> clients = new HashSet<string>();
    private int score = 0;
    private float timeSinceLastObsticle = 0f;
    private bool isObsticleActive = false;
    public bool gameOver { get; private set; }

    private readonly float TIME_FOR_OBSTICLE = 2f;
    private readonly int ZOOM_IN = 5;
    private readonly int ZOOM_OUT = 20;
    private readonly Vector3 MOUNTAIN_TOP;//todo
    private readonly Vector3 MOUNTAIN_BOTTOM;//todo

    private int numOfPlayers;
    private ArrayList allCommands;
    private string sisyphus = "";
    private float timeLeftForObsticle;
    private Dictionary<string,string> correctAnswers;
    private Dictionary<string,string> receivedAnswers;

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
        createComamnds();
        numOfPlayers = clients.Count;
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
        checkWinner();
        if (isObsticleActive)
        {
            bool success=checkCorrectAnswers();
            if (success || timeLeftForObsticle <= 0)
            {
                isObsticleActive = false;
                if (success)
                    GodsSucceeded();
            }
            else
                timeLeftForObsticle -= Time.deltaTime;
        }
        NewObsticleCheck();
    }

    private void checkWinner()
    {
        if (playerTransform.position == MOUNTAIN_TOP)
            GameOver();
        if (playerTransform.position == MOUNTAIN_BOTTOM)
            GameOver();

    }

    private void GodsSucceeded()
    {
        //todo lighting
        lighting.SetActive(true);

        //zoom out
        camera.orthographicSize = ZOOM_OUT;

        //todo wait a few seconds
        //todo move sisyphus back
        moveSisyphusDown();

        //zoom in
        camera.orthographicSize = ZOOM_IN;
        lighting.SetActive(false);

    }

    private void moveSisyphusDown()
    {
        //todo
    }

    private bool checkCorrectAnswers()
    {
       return correctAnswers.OrderBy(kvp => kvp.Key)
           .SequenceEqual(receivedAnswers.OrderBy(kvp => kvp.Key));
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

        System.Random r = new System.Random();
        ArrayList indices = new ArrayList();
        for (int i = 0; i < numOfPlayers-1; i++) {
            int index = r.Next(0, allCommands.Count);
            while(!indices.Contains(index))
                index = r.Next(0, allCommands.Count);
            indices.Add(index);
        }
         correctAnswers = new Dictionary<string, string>();
        HashSet<string>.Enumerator em = clients.GetEnumerator();
        int k = 0;
        while (em.MoveNext())
        {
            string val = em.Current;
            if (Equals(val, sisyphus))
                continue;
             correctAnswers.Add(val, (string)allCommands[(int)indices[k]]);
        }

        ServerMessage obsticle = new ServerMessage();
        obsticle.Kind = ServerMessage.MessageKind.NEW_OBSTICLE;
        obsticle.commandsDictionary = correctAnswers;
        sc.server.SendMessage(obsticle);
        timeLeftForObsticle = TIME_FOR_OBSTICLE;
    }

    public void MessageReceived(ClientMessage message)
    {
        Debug.Log("Message from " + message.ShortId);
        if (message.Kind == ClientMessage.MessageKind.HELLO_RESPONSE)
        {
            //selects which player is sisyphus
            if (!Equals("", sisyphus))
                sisyphus = message.Identifier;

            Debug.Log("Hello response, logging");
            clients.Add(message.Identifier);
        }
        if (isObsticleActive && message.Kind == ClientMessage.MessageKind.ANSWER)
        {
            if (!Equals(message.Identifier, sisyphus))
                receivedAnswers.Add(message.Identifier, message.ChosenCommand);
        }
        if (message.Kind == ClientMessage.MessageKind.SISYPHUS_CLICK)
        {
            moveSisyphusUp();
        }

    }

    private void moveSisyphusUp()
    {
        //todo
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
        //todo
        Debug.Log("Game over");
        gameOver = true;
    }



    private void createComamnds()
    {
        allCommands = new ArrayList();
        allCommands.Add("אודיסיאה");
        allCommands.Add("אוהמרוס");
       //todo...

    }


}
