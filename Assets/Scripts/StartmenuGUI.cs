using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartmenuGUI : MonoBehaviour {

    [Header("Buttons")]
    public Button start;
    public Button quit;

    [Header("Input Method")]
    public Dropdown inputMethod;

	// Use this for initialization
	void Start () {
        start.onClick.AddListener(OnStart);
        quit.onClick.AddListener(OnQuit);
        inputMethod.onValueChanged.AddListener(OnChangedInputMethod);
        inputMethod.value = PlayerPrefs.GetInt("InputMethod", 1);
        inputMethod.RefreshShownValue();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnStart()
    {
        Debug.Log("OnStart");
        SceneManager.LoadScene(1);
    }

    void OnQuit()
    {
        Debug.Log("OnQuit");

        Application.Quit();
    }

    void OnChangedInputMethod(int method)
    {
        Debug.Log("OnChangedInputMethod: " + method);

        switch (method)
        {
            case 1:
                PlayerPrefs.SetInt("InputMethod", (int)InputDevice.InputMethod.Keyboard);
                break;
            case 2:
                PlayerPrefs.SetInt("InputMethod", (int)InputDevice.InputMethod.XBOX);
                break;
            default:
                PlayerPrefs.SetInt("InputMethod", (int)InputDevice.InputMethod.ThrottleAndFlightStick);
                break;
        }
    }
}
