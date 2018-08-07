using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFlaps : MonoBehaviour
{
    public InputDevice input;

    public Transform leftRoll;
    private Vector3 leftRollNeutralRotation, leftRollNeutralPosition;
    private Vector3[] leftRollRotPos = 
    {
        new Vector3(109.805f, 1.399994f, -1.399994f),   // Local rotation up
        new Vector3(-4.964774f, -2.475126f, 1.787082f), // Local position up
        new Vector3(70.195f, -1.4f, 1.4f),              // Local rotation down
        new Vector3(-4.964812f, -2.5051f, 1.666993f)    // Local position down
    };

    public Transform rightRoll;
    private Vector3 rightRollNeutralRotation, rightRollNeutralPosition;
    private Vector3[] rightRollRotPos =
    {
        new Vector3(109.996f, -1.919983f, -1.515991f),  // Local rotation up
        new Vector3(6.104464f, -1.758109f, 1.891932f),  // Local position up
        new Vector3(70.00401f, -1.516f, -1.92f),        // Local rotation down
        new Vector3(6.104416f, -1.808028f, 1.772011f)   // Local position down
    };

    public Transform leftElevator;
    private Vector3 leftElevatorNeutralRotation, leftElevatorNeutralPosition;
    private Vector3[] leftElevatorRotPos =
    {
        new Vector3(109.715f, -0.02600098f, -3.409973f), // Local rotation up
        new Vector3(-0.8104912f, 3.515409f, 1.220851f),  // Local position up
        new Vector3(70.285f, -3.41f, -0.026f),           // Local rotation down
        new Vector3(-0.8104863f, 3.43538f, 1.130441f)    // Local position down
    };

    public Transform rightElevator;
    private Vector3 rightElevatorNeutralRotation, rightElevatorNeutralPosition;
    private Vector3[] rightElevatorRotPos =
    {
        new Vector3(109.951f, -2.422974f, -1.013f),    // Local rotation up
        new Vector3(1.328603f, 3.655604f, 1.248676f),  // Local position up
        new Vector3(70.049f, -1.013f, -2.423f),        // Local rotation down
        new Vector3(1.328622f, 3.575498f, 1.128072f)   // Local position down
    };

    public Transform rudder;
    private Vector3 rudderNeutralRotation, rudderNeutralPosition;
    private Vector3[] rudderRotPos =
    {
        new Vector3(70.957f, 89.638f, 83.495f),    // Local rotation left
        new Vector3(0.07403338f, 4.668703f, 1.42572f),  // Local position left
        new Vector3(70.957f, -96.505f, -90.36301f),        // Local rotation right
        new Vector3(0.2040648f, 4.688649f, 1.42555f)   // Local position right
    };

    [Header("Use this to calibrate the flap rot/pos in inspector")]
    public bool calibrateFlap = false;
    public Transform flap;
    public Vector3 rotationAxis, height;
    private Vector3 startPos;

    // Use this for initialization
    void Start ()
    {
        if (calibrateFlap)
        {
            startPos = flap.position;
            Vector3 axis = rotationAxis.x * transform.right + rotationAxis.y * transform.up + rotationAxis.z * transform.forward;
            flap.RotateAround(flap.position, axis, -20.0f);
        }

        leftRollNeutralRotation = leftRoll.localRotation.eulerAngles;
        leftRollNeutralPosition = leftRoll.localPosition;
        rightRollNeutralRotation = rightRoll.localRotation.eulerAngles;
        rightRollNeutralPosition = rightRoll.localPosition;
        leftElevatorNeutralRotation = leftElevator.localRotation.eulerAngles;
        leftElevatorNeutralPosition = leftElevator.localPosition;
        rightElevatorNeutralRotation = rightElevator.localRotation.eulerAngles;
        rightElevatorNeutralPosition = rightElevator.localPosition;
        rudderNeutralRotation = rudder.localRotation.eulerAngles;
        rudderNeutralPosition = rudder.localPosition;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (calibrateFlap)
        {
            Vector3 axis = rotationAxis.x * transform.right + rotationAxis.y * transform.up + rotationAxis.z * transform.forward;
            Debug.DrawLine(flap.position, flap.position + axis, Color.yellow, Time.deltaTime);
            flap.position = startPos + height.x * transform.right + height.y * transform.up + height.z * transform.forward;
        }

        float aileron = input.aileron;
        if (aileron > 0)
        {
            leftRoll.localRotation = Quaternion.Lerp(Quaternion.Euler(leftRollNeutralRotation), Quaternion.Euler(leftRollRotPos[2]), aileron);
            leftRoll.localPosition = Vector3.Lerp(leftRollNeutralPosition, leftRollRotPos[3], aileron);
            rightRoll.localRotation = Quaternion.Lerp(Quaternion.Euler(rightRollNeutralRotation), Quaternion.Euler(rightRollRotPos[0]), aileron);
            rightRoll.localPosition = Vector3.Lerp(rightRollNeutralPosition, rightRollRotPos[1], aileron);
        }
        else
        {
            leftRoll.localRotation = Quaternion.Lerp(Quaternion.Euler(leftRollNeutralRotation), Quaternion.Euler(leftRollRotPos[0]), -aileron);
            leftRoll.localPosition = Vector3.Lerp(leftRollNeutralPosition, leftRollRotPos[1], -aileron);
            rightRoll.localRotation = Quaternion.Lerp(Quaternion.Euler(rightRollNeutralRotation), Quaternion.Euler(rightRollRotPos[2]), -aileron);
            rightRoll.localPosition = Vector3.Lerp(rightRollNeutralPosition, rightRollRotPos[3], -aileron);
        }

        float elevator = input.elevator;
        if(elevator > 0)
        {
            leftElevator.localRotation = Quaternion.Lerp(Quaternion.Euler(leftElevatorNeutralRotation), Quaternion.Euler(leftElevatorRotPos[2]), elevator);
            leftElevator.localPosition = Vector3.Lerp(leftElevatorNeutralPosition, leftElevatorRotPos[3], elevator);
            rightElevator.localRotation = Quaternion.Lerp(Quaternion.Euler(rightElevatorNeutralRotation), Quaternion.Euler(rightElevatorRotPos[2]), elevator);
            rightElevator.localPosition = Vector3.Lerp(rightElevatorNeutralPosition, rightElevatorRotPos[3], elevator);
        }
        else
        {
            leftElevator.localRotation = Quaternion.Lerp(Quaternion.Euler(leftElevatorNeutralRotation), Quaternion.Euler(leftElevatorRotPos[0]), -elevator);
            leftElevator.localPosition = Vector3.Lerp(leftElevatorNeutralPosition, leftElevatorRotPos[1], -elevator);
            rightElevator.localRotation = Quaternion.Lerp(Quaternion.Euler(rightElevatorNeutralRotation), Quaternion.Euler(rightElevatorRotPos[0]), -elevator);
            rightElevator.localPosition = Vector3.Lerp(rightElevatorNeutralPosition, rightElevatorRotPos[1], -elevator);
        }

        float sideRudder = input.sideRudder;
        if(sideRudder > 0)
        {
            rudder.localRotation = Quaternion.Lerp(Quaternion.Euler(rudderNeutralRotation), Quaternion.Euler(rudderRotPos[2]), sideRudder);
            rudder.localPosition = Vector3.Lerp(rudderNeutralPosition, rudderRotPos[3], sideRudder);
        }
        else
        {
            rudder.localRotation = Quaternion.Lerp(Quaternion.Euler(rudderNeutralRotation), Quaternion.Euler(rudderRotPos[0]), -sideRudder);
            rudder.localPosition = Vector3.Lerp(rudderNeutralPosition, rudderRotPos[1], -sideRudder);
        }
    }
}
