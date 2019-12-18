using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public Text serverIpInput;
    public ClientComponent client;

    void Start()
    {

    }

    public void Click()
    {
        Debug.Log("Trying to connect to " + serverIpInput.text);
        client.Connect(serverIpInput.text);
    }
}
