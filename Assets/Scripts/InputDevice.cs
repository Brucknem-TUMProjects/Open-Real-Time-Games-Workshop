using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// http://www.thrustmaster.com/de_DE/produkte/hotas-warthog-dual-throttles
// http://www.thrustmaster.com/de_DE/produkte/hotas-warthog-flight-stick
public class InputDevice : MonoBehaviour
{
    public InputMethod inputMethod = InputMethod.Keyboard;
    [Range(-1,1)]
    public float sideRudder, elevator, aileron;
    [Range(0,1)]
    public float throttle;
    public bool fire, resetCamera;
    public enum InputMethod
    {
        ThrottleAndFlightStick, Keyboard, XBOX
    }

    private InputMethod oldInputMethod;

    // Use this for initialization
    void Start ()
    {
        oldInputMethod = inputMethod = (InputMethod)PlayerPrefs.GetInt("InputMethod", 1);
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch(inputMethod)
        {
            case InputMethod.ThrottleAndFlightStick:
                aileron = Input.GetAxis("Horizontal");
                elevator = Input.GetAxis("Vertical");
                if (Input.GetAxis("Jump") != 0)
                {
                    sideRudder = aileron;
                    aileron = 0;
                }
                else
                {
                    sideRudder = 0;
                }
                fire = Input.GetAxis("Fire1") != 0;
                throttle = (-Input.GetAxis("Throttle") + 1) / 2.0f;
                resetCamera = Input.GetKeyDown(KeyCode.JoystickButton1);
                break;

            case InputMethod.XBOX:
                aileron = Input.GetAxis("RightStick X");
                elevator = -Input.GetAxis("RightStick Y");
                sideRudder = Input.GetAxis("Horizontal");
                fire = Input.GetKey(KeyCode.JoystickButton5);
                throttle = Mathf.Clamp01(throttle - Time.deltaTime * Input.GetAxis("Throttle"));
                resetCamera = Input.GetKeyDown(KeyCode.JoystickButton6);
                break;

            case InputMethod.Keyboard:
                aileron = Input.GetAxis("Horizontal");
                elevator = Input.GetAxis("Vertical");
                sideRudder = Input.GetAxis("Side Rudder Keyboard");
                fire = Input.GetKey(KeyCode.Space);
                //throttle = Mathf.Clamp01(throttle - Time.deltaTime * Input.GetAxis("Throttle Keyboard"));
                throttle = Input.GetKey(KeyCode.LeftShift) ? Mathf.Clamp01(throttle + 0.5f * Time.deltaTime) : Mathf.Clamp01(throttle - 0.5f * Time.deltaTime);
                resetCamera = Input.GetKeyDown(KeyCode.Return);
                break;
        }

        if(inputMethod != oldInputMethod)
        {
            PlayerPrefs.SetInt("InputMethod", (int)inputMethod);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Menu")
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        oldInputMethod = inputMethod;
	}
}
