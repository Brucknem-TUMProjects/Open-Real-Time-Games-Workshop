using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public float renderScale = 1.0f;
    private InputDevice inputDevice;

    void Start ()
    {
        inputDevice = GameObject.FindObjectOfType<InputDevice>();
        resetCamera();
        UnityEngine.VR.VRSettings.renderScale = renderScale;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(inputDevice.resetCamera)
        {
            Debug.Log("Resetting view");
            resetCamera();
            UnityEngine.VR.VRSettings.renderScale = renderScale;
        }
	}

    public void resetCamera ()
    {
        UnityEngine.VR.InputTracking.Recenter();
        UnityEngine.VR.VRDevice.SetTrackingSpaceType(UnityEngine.VR.TrackingSpaceType.Stationary);
    }
}
