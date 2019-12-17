using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public Vector2 moveDirection;
    public float moveSpeed;

    private readonly float endY = -10f;
    private readonly Vector3 startPoint = new Vector3(-19.4f, 10.1f, 0);

    // Start is called before the first frame update

    void Start()
    {
        // var v = Camera.main.camera.orthographicSize;

        Debug.Log(tag + " y pos = " + transform.localPosition.y.ToString() + "x pos = " + transform.position.x.ToString());

    }
    
    void Update()
    {
        if (transform.localPosition.y <= endY)
            transform.localPosition = startPoint;
        else
            transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
    }

    private void FixedUpdate()
    {
        // rigidbody.MovePosition(rigidbody.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

}
