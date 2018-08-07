using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed, damage;
    public Vector3 direction;
    public float timeUntilDestroy;
    public Vector3 addedVelocity = Vector3.zero;
    public AudioClip sound;
    public GameObject hitParticleEffect;


    private Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = addedVelocity + direction * speed;
        AudioSource.PlayClipAtPoint(sound, transform.position, 2);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(timeUntilDestroy <= 0)
        {
            GameData.Instance.score = Mathf.Max(0, GameData.Instance.score - 10);
            Destroy(gameObject);
        }
        else
        {
            timeUntilDestroy -= Time.deltaTime;
        }
	}
}
