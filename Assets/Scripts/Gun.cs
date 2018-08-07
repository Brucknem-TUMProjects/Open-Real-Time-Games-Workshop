using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public GameObject projectilePrefab;
    public float timeBetweenFire;
    public bool canFire = true;

    private float timeUntilNextFire = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (timeUntilNextFire > 0)
            timeUntilNextFire -= Time.deltaTime;
	}

    public void Fire()
    {
        if (canFire && timeUntilNextFire <= 0)
        {
            muzzleFlash.Play();
            GameObject g = Instantiate(projectilePrefab, transform.position, transform.rotation);
            g.GetComponent<Projectile>().direction = transform.up;
            g.GetComponent<Projectile>().addedVelocity = transform.parent.GetComponent<Rigidbody>().velocity;
            timeUntilNextFire = timeBetweenFire;
        }
    }
}
