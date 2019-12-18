using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public int width;
    public int height;
    private const float MAGIC = 200f;

    void Start()
    {
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = width / height;
        float scaleHeight = windowAspect / targetAspect;

        Debug.Log("Window aspect: " + windowAspect + ", Target aspect: " + targetAspect);

        if (windowAspect < targetAspect)
        {
            Camera.main.orthographicSize = (height / MAGIC) / scaleHeight;
        }
        else
        {
            Camera.main.orthographicSize = height / MAGIC;
        }
    }

    void Update()
    {

    }
}