using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public Text serverIpInput;
    public ClientComponent client;

    private const float CLICK_TIMEOUT = 3f;
    private float timeSinceClick = 0f;

    void Update()
    {
        timeSinceClick -= Time.deltaTime;
    }

    public void Click()
    {
        if (timeSinceClick > 0)
            return;
        timeSinceClick = CLICK_TIMEOUT;
        Debug.Log("Trying to connect to " + serverIpInput.text);
        client.Connect(serverIpInput.text);
    }
}
