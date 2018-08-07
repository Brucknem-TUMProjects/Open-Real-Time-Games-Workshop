using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : UfoAI
{
    public GameObject rayRicePilotPrefab;
    public AudioClip[] alienVoices;
    public AudioClip[] humanVoices;
    public AudioSource source;
    public float minTimeBetweenVoice, maxTimeBetweenVoice;

    private float timeUntilNextVoice;
    private int[] lastClips = { 4, 7, 2, 6 };

	// Use this for initialization
	public override void initializeVoiceActing ()
    {
        timeUntilNextVoice = Random.Range(minTimeBetweenVoice, maxTimeBetweenVoice);
	}

    public override void loopVoiceActing ()
    {
        if (timeUntilNextVoice <= 0)
        {
            int clip = Random.Range(0, 8);
            while (clip == lastClips[0] || clip == lastClips[1] || clip == lastClips[2] || clip == lastClips[3])
                clip = Random.Range(0, 8);
            lastClips[3] = lastClips[2];
            lastClips[2] = lastClips[1];
            lastClips[1] = lastClips[0];
            lastClips[0] = clip;

            if (clip < 4)
                source.clip = alienVoices[clip];
            else
                source.clip = humanVoices[clip - 4];
            source.Play();

            timeUntilNextVoice = Random.Range(minTimeBetweenVoice, maxTimeBetweenVoice);
        }
        else if (!source.isPlaying)
        {
            timeUntilNextVoice -= Time.deltaTime;
        }
    }

    public override IEnumerator Die()
    {
        // Try to escape
        Destroy(GameObject.FindObjectOfType<EdgarMustang>().GetComponent<RestrictPosition>());
        Destroy(GameObject.FindObjectOfType<UfoSpawner>());
        flightAI.minHeight = 200;
        flightAI.escapeRadius = 50;
        flightAI.escapeDistance = 4000;
        flightAI.state = FlightAI.AIState.Escape;
        ufoLight.gameObject.SetActive(false);
        yield return new WaitForSeconds(5.0f);

        float timeUntilDestroy = 10.0f;
        GameData.Instance.killedEnemies++;
        GameData.Instance.score += 3000;

        flightAI.enabled = false;
        transform.SetParent(null);
        rb.useGravity = true;
        GameObject s = Instantiate(smoke, transform.position, Quaternion.identity);
        GameObject e = Instantiate(explosion, transform.position, Quaternion.identity);
        GameObject g = Instantiate(rayRicePilotPrefab, transform.position, rayRicePilotPrefab.transform.rotation, null);
        g.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        while (timeUntilDestroy > 0)
        {
            rb.AddForce(Vector3.down * 20 * Time.deltaTime, ForceMode.Acceleration);
            s.transform.position = transform.position;
            e.transform.position = transform.position;
            timeUntilDestroy -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(s);
        Destroy(e);
        Destroy(gameObject);
    }
}
