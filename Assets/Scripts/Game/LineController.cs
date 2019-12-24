using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public Vector2 moveDirection;
    public float moveSpeed;

    private readonly float endY = -18.72f;
    private readonly Vector3 startPoint = new Vector3(-46.783f, 27.521f, 0);

    // Start is called before the first frame update

    void Start()
    {
        // var v = Camera.main.camera.orthographicSize;
        Debug.Log(tag + " y pos = " + transform.localPosition.y.ToString() + "x pos = " + transform.position.x.ToString());

    }
    
    void Update()
    {
        if (GameControl.iterations < GameControl.numberOfLineIterations)
        {
            if (transform.localPosition.y <= endY)
            {
                transform.localPosition = startPoint;
                GameControl.iterations+=1;
            }
            else
                transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
        }
        else
            GameControl.lastIteration = true;
    }

    private void FixedUpdate()
    {
        // rigidbody.MovePosition(rigidbody.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

}
