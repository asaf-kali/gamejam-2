using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    public ServerComponent sc;
    public GameObject player;
    public GameObject ball;

    private int score = 0;
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
    }

    void Update()
    {
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
