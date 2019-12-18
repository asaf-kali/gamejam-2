using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientComponent : MonoBehaviour
{
    public string sceneName;
    private Client<GreekMessage> client;

    public void Connect(string ip)
    {
        if (client != null)
        {
            client.Dispose();
        }
        client = new Client<GreekMessage>(GetInstanceID(), ip);
        client.onConnect = OnConnect;
        client.ConnetToServerAsync();
    }

    public void SendMessage()
    {
        GreekMessage message = "push";
        client.SendMessage(message);
    }

    void OnConnect()
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            StartCoroutine(LoadYourAsyncScene());
        });
    }

    IEnumerator LoadYourAsyncScene()
    {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(this.sceneName, LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(sceneName));
        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }

    void OnDestroy()
    {
        if (client != null)
            client.Dispose();
    }

}