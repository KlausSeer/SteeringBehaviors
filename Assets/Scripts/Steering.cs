using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behavior
{
    NONE,
    SEEK,
    FLEE,
    ARRIVAL,
}

public class Steering : MonoBehaviour
{

    [Range(2.0f, 7.0f)]
    [SerializeField]
    private float speed = 5.0f;

    [Range(0.0f, 2.0f)]
    [SerializeField]
    private float slowDownRadius = 1.0f;

    
    [SerializeField]
    private Behavior behavior;

    private Vector2 velocity = Vector2.zero;

    #region Behavoiurs

    Vector2 Seek(Vector2 desiredVelocity)
    {
        return desiredVelocity - velocity;
    }

    #endregion

    void Update()
    {
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 targetDistance = (targetPosition - (Vector2)transform.position);

        Vector2 desiredVelocity = targetDistance.normalized * speed;

        Vector2 steering = Vector2.zero;

        #region switch

        switch (behavior)
        {
            case Behavior.NONE:
                velocity = desiredVelocity;
                break;
            case Behavior.SEEK:
                steering = Seek(desiredVelocity);
                velocity += steering * Time.deltaTime;
                break;
            case Behavior.FLEE:
                steering = Seek(-desiredVelocity);
                velocity += steering * Time.deltaTime;
                break;
            case Behavior.ARRIVAL:
                float slowDownFactor = targetDistance.magnitude > slowDownRadius? 1.0f : targetDistance.magnitude/2;
                float stopFactor = targetDistance.magnitude > slowDownRadius/10 ? 1.0f : 0.0f;

                desiredVelocity *= slowDownFactor;

                steering = Seek(desiredVelocity);

                velocity *= stopFactor;

                velocity += steering * Time.deltaTime;

                break;
            default:
                break;
        }

        #endregion

        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
