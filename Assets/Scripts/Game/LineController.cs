using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public Vector2 moveDirection;
    public float moveSpeed;

    private readonly float endY = -18.72f;
    private readonly Vector3 startPoint = new Vector3(-46.783f, 27.521f, 0);

    void Start()
    {
        Debug.Log(tag + " y pos = " + transform.localPosition.y.ToString() + ", x pos = " + transform.position.x.ToString());
    }

    void Update()
    {
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
        if (transform.localPosition.y <= endY)
            transform.localPosition = startPoint;
    }
}
