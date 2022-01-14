using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behavior
{
    NONE,
    SEEK,
    FLEE,
    ARRIVAL,
    PURSUIT,
    EVADE,
    WANDER,
    PATH_FOLLOW,
}

public class Steering : MonoBehaviour
{

    [Range(2.0f, 7.0f)]
    [SerializeField]
    private float speed = 5.0f;

    [Range(0.0f, 2.0f)]
    [SerializeField]
    private float slowDownRadius = 1.0f;

    [Range(1, 3)]
    [SerializeField]
    private int initialTimePrediction = 2;

    [Range(1.0f, 3.0f)]
    [SerializeField]
    private float wanderTime = 2.0f;

    private float wanderElapsedTime = 0.0f;

    [SerializeField]
    private float wanderAngle = 30.0f;

    [Range(10.0f, 30.0f)]
    [SerializeField]
    private float wanderChange = 10.0f;

    private int timePrediction = 0;

    [SerializeField]
    private bool mouseTarget = true;

    [SerializeField]
    private GameObject movingTarget;

    [SerializeField]
    private Behavior behavior;

    private Vector2 velocity = Vector2.zero;

    private Vector2 futurePosition = Vector2.zero;

    private Vector2 nodePosition = Vector2.zero;

    [SerializeField]
    private Path path;

    [SerializeField]
    private int currentNode = 0;

    [SerializeField]
    [Range(0.5f, 2.0f)]
    private float pathRadius = 1.0f;

    private List<Vector2> nodes;

    [SerializeField]
    private Vector2 target;

    [SerializeField]
    Vector2 circleCenter;

    [SerializeField]
    Vector2 displacement;

    [SerializeField]
    Vector2 wanderForce;

    #region Behavoiurs

    Vector2 Seek(Vector2 desiredVelocity)
    {
        return desiredVelocity - velocity;
    }

    Vector2 Pursuit(Vector2 targetPosition, Vector2 velocity)
    {
        Vector2 distance = targetPosition - (Vector2) this.transform.position;
        timePrediction = (int) (distance.magnitude * initialTimePrediction / speed);
        Vector2 futurePosition = targetPosition + velocity * timePrediction;
        return futurePosition;
    }

    Vector2 Wander()
    {
        circleCenter = velocity.normalized * speed;

        displacement = new Vector2(0, -1);
        displacement *= speed;

        SetAngle(displacement, wanderAngle);

        wanderAngle += Random.value * wanderChange -  0.5f *wanderChange;

        wanderForce = circleCenter + displacement;

        return wanderForce;
    }

    Vector2 PathFollowing()
    {
        target = Vector2.zero;
        if (path != null)
        {          
            target = nodes[currentNode];

            if(Vector2.Distance(this.transform.position, target) <= pathRadius)
            {
                currentNode++;

                if (currentNode >= nodes.Count)
                    currentNode = nodes.Count - 1;
            }

        }

        return target;
    }

    #endregion

    private void SetAngle(Vector2 displacementVector, float angle)
    {
        float magnitude = displacementVector.magnitude;
        float x = Mathf.Cos(angle) * magnitude;
        float y = Mathf.Sin(angle) * magnitude;

        displacementVector = new Vector2(x, y);
    }

    private void Start()
    {
        if (behavior == Behavior.WANDER)
            velocity = Vector2.one * speed;

        nodes = path.Nodes;
    }

    private void FixedUpdate()
    {
        if(behavior == Behavior.WANDER)
            wanderElapsedTime += Time.fixedDeltaTime;
    }

    void Update()
    {
        Vector2 targetPosition = mouseTarget? Camera.main.ScreenToWorldPoint(Input.mousePosition): movingTarget.transform.position;

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
            case Behavior.PURSUIT:
                if (mouseTarget)
                    break;
                futurePosition = Pursuit(targetPosition, movingTarget.GetComponent<MovingObjectTandem>().Velocity);

                targetDistance = (futurePosition - (Vector2)transform.position);
                desiredVelocity = targetDistance.normalized * speed;

                steering = Seek(desiredVelocity);
                velocity += steering * Time.deltaTime;

                break;
            case Behavior.EVADE:
                if (mouseTarget)
                    break;
                futurePosition = Pursuit(targetPosition, movingTarget.GetComponent<MovingObjectTandem>().Velocity);

                targetDistance = (futurePosition - (Vector2)transform.position);
                desiredVelocity = targetDistance.normalized * speed;

                steering = Seek(-desiredVelocity);
                velocity += steering * Time.deltaTime;

                break;

            case Behavior.WANDER:
                if(wanderElapsedTime >= wanderTime)
                {
                    wanderElapsedTime = 0.0f;
                    steering = Wander();
                    velocity += steering;
                }

                break;

            case Behavior.PATH_FOLLOW:

                nodePosition = PathFollowing();


                break;
            default:
                break;
        }

        #endregion

        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
