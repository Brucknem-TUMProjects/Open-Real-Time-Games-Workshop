using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayRice : Player
{
    public Transform ufoLight;
    public Transform pilot;
    public GameObject UFOSpawner;
    public float suckInSpeed;
    public VoiceActing voiceActing;
    public AudioClip iAmBeingSuckedIn, whatDoYouWantFromMe, shootThem;

    private bool suckIn = false, suckedIn = false, whatDoYouWantFromMePlayed = false, destroyOnImpact = false;
    private float startDistance;
    private float distance = 0;

    public override void Die()
    {
        if (!suckedIn)
        {
            // Get sucked into the light
            if (suckIn)
            {
                if (distance >= 0)
                {
                    pilot.position = ufoLight.position + distance * ufoLight.forward;
                    pilot.localScale = Vector3.one * Mathf.Sqrt(distance / startDistance);
                    distance -= suckInSpeed * Time.deltaTime;

                    if(!whatDoYouWantFromMePlayed && (distance/startDistance) <= 0.5f)
                    {
                        whatDoYouWantFromMePlayed = true;
                        voiceActing.GetComponent<AudioSource>().PlayOneShot(whatDoYouWantFromMe, voiceActing.volumeScale);
                    }
                }
                else
                {
                    suckedIn = true;
                    destroyOnImpact = true;
                    pilot.gameObject.SetActive(false);
                    ufoLight.GetComponentInParent<FlightAI>().state = FlightAI.AIState.Escape;
                    // Destroy ufo
                    Destroy(ufoLight.parent.gameObject, 10.0f);
                    // Spawn other ufos
                    UFOSpawner.SetActive(true);
                    // Activate guns
                    GameObject.FindObjectOfType<EdgarMustang>().canShoot = true;
                    // Play voice acting
                    voiceActing.GetComponent<AudioSource>().PlayOneShot(shootThem, voiceActing.volumeScale);
                    // Set objective
                    GameData.Instance.objective = "Shoot the UFOs";
                }
            }
            else
            {
                // Make the plane fall to the ground
                gameObject.AddComponent<BoxCollider>(); // TODO: use this to destroy the gameObject when it reaches the ground
                GetComponent<Rigidbody>().useGravity = true;
                // Start animation
                GetComponentInChildren<Animator>().SetTrigger("fall");
                startDistance = distance = (ufoLight.position - pilot.position).magnitude;
                suckIn = true;
                // Play voice acting
                voiceActing.GetComponent<AudioSource>().PlayOneShot(iAmBeingSuckedIn, voiceActing.volumeScale);
            }
        }
    }

    public override void OnCollisionEnter (Collision c)
    {
        if(destroyOnImpact)
        {
            Destroy(Instantiate(explosionPrefab, transform.position, Quaternion.identity), 5.0f);
            gameObject.SetActive(false);
            Destroy(gameObject, 20.0f);
        }
    }
}
