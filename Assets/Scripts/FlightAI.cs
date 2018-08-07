using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
// Moves between random points on a sphere in around its target
public class FlightAI : MonoBehaviour
{
    public enum AIState { Attack, Escape};
    public AIState state = AIState.Attack;
    public Transform follow;
    public float minHeight, maxHeight;
    [Header("Attack state properties:")]
    [Range(1, 10)]
    public float returnSpeed;
    public float movementSpeed;
    public float basicTurnSpeed;
    public float velocitySmooth;
    public Vector3 attackDistance;
    public float attackRadius = 50;
    public bool debugMovement = false;
    private Vector3 oldPoint, currentPoint, movementVelocity, targetVelocity;     // points around origin
    [Header("Escape state properties")]
    public float escapeDistance;
    public float escapeRadius;
    public float escapeSmooth;
    public float speedUpDistance;
    private Vector3 escapePoint, escapeStart;
    private bool escapePointReached;

    private Rigidbody rb;
    private float speed;
    private AIState oldState;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        oldPoint = calculatePoint();
        currentPoint = calculatePoint();
        calculateNextPoint();
        oldState = state = AIState.Attack;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

    void Update ()
    {
        // Clamp Y coordinate
        Vector3 tmp = transform.position;
        tmp.y = Mathf.Clamp(tmp.y, minHeight, maxHeight);
        transform.position = tmp;

        // Calculate a new point to escape to
        if(oldState != AIState.Escape && state == AIState.Escape)
        {
            escapePoint = follow.position + follow.forward * escapeDistance + Random.Range(-escapeRadius, escapeRadius) * follow.right + Random.Range(-escapeRadius, escapeRadius) * follow.up;
            escapePoint.y = Mathf.Clamp(escapePoint.y, minHeight, maxHeight);
            escapeStart = transform.position;
            escapePointReached = false;
        }

        if (state == AIState.Attack)
        {
            // Transform points into local space
            Vector3 transformedOldPoint = follow.localToWorldMatrix * oldPoint;
            Vector3 transformedCurrentPoint = follow.localToWorldMatrix * currentPoint;
            // Transform points into world space
            Vector3 posOffset = attackDistance.x * follow.right + attackDistance.y * follow.up + attackDistance.z * follow.forward;
            Vector3 fromPosition = follow.position + posOffset + transformedOldPoint;
            Vector3 toPosition = follow.position + posOffset + transformedCurrentPoint;
            Vector3 direction = (toPosition - transform.position).normalized;
            speed = Mathf.Clamp(Mathf.Pow((toPosition - transform.position).magnitude / attackRadius, returnSpeed), 1, 100);
            speed *= movementSpeed;
            // Calculate turn degrees from the distance to current point -> current point will always be met
            float turnDegrees = Mathf.Clamp01((transform.position - fromPosition).magnitude / (toPosition - fromPosition).magnitude);
            turnDegrees = Mathf.Pow(turnDegrees, 10) + speed * basicTurnSpeed;
            movementVelocity = Vector3.RotateTowards(movementVelocity.normalized, direction, turnDegrees, 0) * speed;
            targetVelocity = Vector3.Lerp(targetVelocity, follow.GetComponent<Rigidbody>().velocity, velocitySmooth * Time.fixedDeltaTime);
            rb.velocity = targetVelocity + movementVelocity;
            if (movementVelocity.magnitude == 0)
                movementVelocity = direction;

            if (debugMovement)
            {
                // Draw quater of a sphere
                for (float phi = 0; phi < Mathf.PI; phi += 0.05f)
                {
                    for (float theta = 0; theta < Mathf.PI / 2; theta += 0.05f)
                    {
                        Vector3 p = getPointOnSphere(attackRadius, theta, phi);
                        p = follow.localToWorldMatrix * p;
                        Debug.DrawLine(follow.position + posOffset + 0.99f * p, follow.position + posOffset + p, Color.red, Time.fixedDeltaTime);
                    }
                }

                // Draw connection between points
                Debug.DrawLine(follow.position + posOffset + transformedOldPoint, follow.position + posOffset + transformedCurrentPoint, Color.green, Time.fixedDeltaTime);

                // Draw movement velocity
                Debug.DrawLine(transform.position, transform.position + movementVelocity, Color.blue, Time.fixedDeltaTime);
            }

            // Calculate the next point when the transform is close to the target point
            float distance = ((follow.position + posOffset + transformedCurrentPoint) - transform.position).magnitude;
            if (distance < 1)
                calculateNextPoint();
        }
        else if (state == AIState.Escape)
        {
            // Fly fast towards a point in front of the target, slow down, become faster when target gets closer
            Vector3 flyDirection;
            float speedFactor;
            if (!escapePointReached)
            {
                // Fly away towards a certain point
                escapePointReached = (escapePoint - transform.position).magnitude < 10;
                flyDirection = (escapePoint - transform.position).normalized;
                speedFactor = 0.5f + 2 * Mathf.Sqrt((escapePoint - transform.position).magnitude / (escapePoint - escapeStart).magnitude);
            }
            else
            {
                // Fly slowly in a constant direction, speed up if the target comes closer
                flyDirection = (escapePoint - escapeStart).normalized;
                speedFactor = 0.85f + Mathf.Clamp01(1 - ((transform.position - follow.position).magnitude / speedUpDistance));
            }
            Vector3 toVelocity = flyDirection * speedFactor * (10 + follow.GetComponent<Rigidbody>().velocity.magnitude);
            // TODO: maybe add random movement here
            rb.velocity = Vector3.Lerp(rb.velocity, toVelocity, escapeSmooth * Time.deltaTime);
            Debug.DrawLine(transform.position, escapePoint, Color.green, Time.deltaTime);
        }

        oldState = state;
    }

    private void calculateNextPoint ()
    {
        // Get next random point with a max distance to the current point
        oldPoint = currentPoint;
        do
        {
            currentPoint = calculatePoint();
        } while ((currentPoint - oldPoint).magnitude > attackRadius);
    }

    private Vector3 calculatePoint ()
    {
        // Get random point on a fourth of a sphere
        float theta = Random.Range(0, Mathf.PI / 2);
        float phi = Random.Range(0, Mathf.PI);
        return getPointOnSphere(attackRadius, theta, phi);
    }

    private Vector3 getPointOnSphere (float radius, float theta, float phi)
    {
        return new Vector3(radius * Mathf.Sin(theta) * Mathf.Cos(phi), radius * Mathf.Sin(theta) * Mathf.Sin(phi), radius * Mathf.Cos(theta));
    }

    public bool isEscapePointReached ()
    {
        return escapePointReached;
    }
}
