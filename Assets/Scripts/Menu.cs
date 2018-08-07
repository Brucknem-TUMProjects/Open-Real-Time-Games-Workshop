using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Camera camera;
    public SpriteRenderer controls;
    public UnityEngine.UI.Image keyboard, xbox, throttleAndJoystick;
    public UnityEngine.Sprite controlsKeyboard, controlsXBOX, controlsThrottleAndJoystick;
    public UnityEngine.UI.Text highscore;

    private Color selected = new Color(1, 1, 1, 0.23f), unselected = new Color (0, 0, 0, 0.23f);

	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false;
        InputDevice.InputMethod m = (InputDevice.InputMethod) PlayerPrefs.GetInt("InputMethod", 0);
        switch(m)
        {
            case InputDevice.InputMethod.Keyboard:
                selectKeyboard();
                break;
            case InputDevice.InputMethod.ThrottleAndFlightStick:
                selectThrottleAndJoystick();
                break;
            case InputDevice.InputMethod.XBOX:
                selectXbox();
                break;
        }
        highscore.text = "Highscore\n" + PlayerPrefs.GetInt("Highscore", 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(!UnityEngine.VR.VRDevice.isPresent)
        {
            Vector3 look = new Vector3((Input.mousePosition.x / Screen.width) - 0.5f, (Input.mousePosition.y / Screen.height) - 0.5f, 0.5f);
            look *= 10;
            camera.transform.LookAt(look);
        }
	}

    public void selectKeyboard ()
    {
        PlayerPrefs.SetInt("InputMethod", (int)InputDevice.InputMethod.Keyboard);
        GameObject.FindObjectOfType<InputDevice>().inputMethod = InputDevice.InputMethod.Keyboard;
        resetImages();
        keyboard.color = selected;
        controls.sprite = controlsKeyboard;
    }

    public void selectXbox ()
    {
        PlayerPrefs.SetInt("InputMethod", (int)InputDevice.InputMethod.XBOX);
        GameObject.FindObjectOfType<InputDevice>().inputMethod = InputDevice.InputMethod.XBOX;
        resetImages();
        xbox.color = selected;
        controls.sprite = controlsXBOX;
    }

    public void selectThrottleAndJoystick ()
    {
        PlayerPrefs.SetInt("InputMethod", (int)InputDevice.InputMethod.ThrottleAndFlightStick);
        GameObject.FindObjectOfType<InputDevice>().inputMethod = InputDevice.InputMethod.ThrottleAndFlightStick;
        resetImages();
        throttleAndJoystick.color = selected;
        controls.sprite = controlsThrottleAndJoystick;
    }

    private void resetImages ()
    {
        keyboard.color = unselected;
        xbox.color = unselected;
        throttleAndJoystick.color = unselected;
    }

    public void Play ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    public void Exit ()
    {
        Application.Quit();
    }
}
