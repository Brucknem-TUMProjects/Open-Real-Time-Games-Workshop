using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePropeller : MonoBehaviour
{
    public Vector3 rotationAxis;
    public float speed;
    public float maxSpeed;
    public float volume;
    public AudioSource engineSound;
    public AudioSource propellerSound;
    public AudioSource propellerSlowSound;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawLine(transform.position, transform.position + rotationAxis, Color.red, Time.deltaTime);
        float input = GameObject.FindObjectOfType<InputDevice>().throttle;
        speed = -Time.deltaTime * (400 + maxSpeed * input);
        transform.Rotate(rotationAxis, speed, Space.Self);
        engineSound.volume = volume * (0.01f + input * 0.04f);
        engineSound.pitch = 0.3f + input * 0.7f;
        propellerSlowSound.volume = volume * 0.5f * (1-input);
        propellerSlowSound.pitch = 0.6f + 0.4f * input;
        propellerSound.volume = volume * input;
        propellerSound.pitch = 0.5f + 1.5f * input;
	}
}
