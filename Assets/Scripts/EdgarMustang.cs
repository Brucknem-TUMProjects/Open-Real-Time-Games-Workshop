using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgarMustang : MonoBehaviour
{
    public float maxSpeed, accelerationSpeed, slowDownSpeed, aileronSpeed, elevatorSpeed, sideRudderSpeed;
    public bool canShoot = false;
    public Gun leftGun, rightGun;
    public float dropAcceleration, timeBetweenFire, maxRotateRadians;
    public AudioSource wind, music;
    public float timeUntilEnabled = 0;

    private Rigidbody rb;
    private InputDevice inputDevice;
    private bool leftGunFireTurn = false;
    private float timeUntilNextFire;
    private bool isEnabled = false;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        inputDevice = GameObject.FindObjectOfType<InputDevice>();
	}

    void Update ()
    {
        if(!isEnabled)
        {
            if (timeUntilEnabled <= 0)
            {
                isEnabled = true;
                if(music)
                    music.enabled = true;
            }
            else
                timeUntilEnabled -= Time.deltaTime;
        }

        GameData.Instance.timePassed = Time.time;
        GameData.Instance.velocity = rb.velocity.magnitude;
        if(timeUntilNextFire <= 0)
        {
            if (canShoot && inputDevice.fire)
            {
                if (leftGunFireTurn)
                    leftGun.Fire();
                else
                    rightGun.Fire();
                leftGunFireTurn = !leftGunFireTurn;
                timeUntilNextFire = timeBetweenFire;
            }
        }
        else
        {
            timeUntilNextFire -= Time.deltaTime;
        }

        wind.pitch = 0.3f + (rb.velocity.magnitude / maxSpeed);
        wind.volume = 0.5f * (0.2f + (rb.velocity.magnitude / maxSpeed));
        SeatVibration.setVolume(Mathf.Min(1, 0.1f + (rb.velocity.magnitude / maxSpeed)));
    }
	
	void FixedUpdate ()
    {
        if (isEnabled)
        {
            // Propeller speed
            Vector3 accelerationForce = (-0.3f + inputDevice.throttle) * transform.forward * accelerationSpeed;
            if (rb.velocity.magnitude > maxSpeed && accelerationForce.magnitude > 0)
                accelerationForce = Vector3.zero;
            rb.AddForce(accelerationForce, ForceMode.Acceleration);
            // Elevator speed
            rb.AddTorque(inputDevice.elevator * transform.right * elevatorSpeed, ForceMode.Acceleration);
            // Adjust velocity direction
            rb.velocity = Vector3.RotateTowards(rb.velocity, transform.forward, Mathf.Abs(inputDevice.elevator) * maxRotateRadians, 0);
            rb.velocity = Vector3.RotateTowards(rb.velocity, transform.forward, Mathf.Abs(inputDevice.sideRudder) * maxRotateRadians, 0);
            // Generally make velocity face forward
            rb.velocity = Vector3.RotateTowards(rb.velocity, transform.forward, 0.05f * maxRotateRadians, 0);

            // Aileron
            rb.AddTorque(inputDevice.aileron * -transform.forward * aileronSpeed, ForceMode.Acceleration);
            // Side rudder
            rb.AddTorque(inputDevice.sideRudder * transform.up * sideRudderSpeed, ForceMode.Acceleration);

            float dropFactorRoll = Mathf.Pow(Mathf.Abs(Vector3.Angle(transform.right, Vector3.up) - 90) / 90, 1);
            float dropFactorElevator = Mathf.Pow(Mathf.Clamp01(Vector3.Angle(transform.forward, Vector3.up) - 90) / 90, 1);
            Vector3 dropForce = Vector3.down * (dropFactorRoll + dropFactorElevator) * dropAcceleration;
            rb.AddForce(dropForce, ForceMode.Acceleration);

            if (rb.velocity.magnitude > maxSpeed)
            {
                float slowDownFactor = Mathf.Pow(1 - (dropFactorRoll + dropFactorElevator), 16);
                rb.velocity -= slowDownSpeed * slowDownFactor * (rb.velocity - rb.velocity * (maxSpeed / rb.velocity.magnitude));
            }
        }

        // Draw velocity
        Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.yellow, Time.fixedDeltaTime);
    }
}
