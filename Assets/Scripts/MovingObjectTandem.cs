using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectTandem : MonoBehaviour
{
    [Range(2.0f, 7.0f)]
    [SerializeField]
    private float speed = 5.0f;

    [SerializeField]
    private bool facingRight;

    private Vector2 velocity; 

    public Vector2 Velocity
    {
        get { return velocity; }
    }

    [SerializeField]
    private float leftBound= -5.0f;

    [SerializeField]
    private float rightBound = 5.0f;

    void Start()
    {
        velocity = Vector2.zero;
        facingRight = true;
    }

    void Update()
    {
        velocity = facingRight? Vector2.right * speed : Vector2.left * speed;
        this.transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if(Bounds())
        {
            Flip();
        }
    }

    private bool Bounds()
    {
        if(facingRight)
        {
            return this.transform.position.x >= rightBound;
        }
        else
        {
            return this.transform.position.x <= leftBound;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        this.transform.position = new Vector3(this.transform.position.x, -this.transform.position.y, this.transform.position.z);
    }
}
