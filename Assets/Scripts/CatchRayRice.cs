using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchRayRice : MonoBehaviour
{
    public AudioClip thanksForTheRescue, catchMe, wellDone, tryToLandBackThere;
    public AudioSource source;

    private float timeUntilCatchMe = 5.0f;
    private bool catchMePlayed = false, wellDonePlayed = false;

    // Use this for initialization
    void Start ()
    {
        GameData.Instance.objective = "Catch Ray Rice";
        transform.GetComponentInChildren<Animator>().SetTrigger("fall");
        source.PlayOneShot(thanksForTheRescue);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(!catchMePlayed)
        {
            if(timeUntilCatchMe <= 0)
            {
                source.PlayOneShot(catchMe);
                catchMePlayed = true;
            }

            timeUntilCatchMe -= Time.deltaTime;
        }
	}

    void OnTriggerEnter (Collider c)
    {
        if(c.CompareTag("EdgarMustang") && !wellDonePlayed)
        { 
            wellDonePlayed = true;
            source.PlayOneShot(wellDone);
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
            StartCoroutine(playLandVoice());
            GameData.Instance.objective = "Land on the runway\nYellow marker";
            GameObject.Find("Marker").GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("UFO Spawner").SetActive(false);
        }
    }

    IEnumerator playLandVoice ()
    {
        yield return new WaitForSeconds(3.0f);
        source.PlayOneShot(tryToLandBackThere, 1.0f);
        Destroy(gameObject, 5.0f);
    }
}
