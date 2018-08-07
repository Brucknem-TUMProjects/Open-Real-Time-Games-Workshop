using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatVibration : MonoBehaviour
{
    public AudioSource vibration;
    public float volumeBlendSpeed;

    private static float targetVolume;

	// Use this for initialization
	void Start ()
    {
        vibration.volume = targetVolume = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(vibration.volume != targetVolume)
        {
            float blend = volumeBlendSpeed * Time.deltaTime;
            //Debug.Log(blend);
            vibration.volume = 0.75f * (blend * targetVolume + (1 - blend) * vibration.volume);
        }
	}

    public static void setVolume (float volume)
    {
        targetVolume = volume;
    }
}
