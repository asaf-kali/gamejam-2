using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(new Vector3(1, 1, 15) * Time.deltaTime);
    }
}
