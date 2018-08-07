using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UfoAI : MonoBehaviour
{
    public FlightAI flightAI;
    public Player player;
    public Transform ufoLight, target, ufoKuppel;
    public float health = 1000, lightDamage, lightAimSpeed, kuppelRotationSpeed;
    public float escapeProbability, maxEscapeTime;
    public float stabilizationTime = 2, tumbleTime = 2;
    public AudioSource lightSound;
    public AudioClip deathSound;
    public GameObject explosion, smoke;
    
    protected float tumbleStartTime, lastHealth, escapeTime;
    protected bool isTumbling = false, dead = false;
    protected Vector2 rotationAngles;

    protected Rigidbody rb;
    protected Vector3 lastTarget;
    protected Vector3 currentLook;

    // Use this for initialization
    void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        currentLook = -transform.up;
        lastHealth = health;
        initializeVoiceActing();
	}

	// Update is called once per frame
	void Update ()
    {
        if (!dead)
        {
            // Switch flightAI states
            if (flightAI.state == FlightAI.AIState.Attack)
            {
                // If the ufo is close to its target
                if (!isTumbling && (transform.position - flightAI.follow.position).magnitude <= (1.25 * flightAI.attackDistance.magnitude + flightAI.attackRadius))
                {
                    // Higher escape probability if the ufo health is low
                    float probability = Mathf.Clamp01(Random.Range(0.0f, 1.0f) - (health / lastHealth)) * Time.deltaTime * escapeProbability;
                    float random = Random.Range(0.0f, 1.0f);
                    if (random < probability)
                    {
                        //Escape
                        Debug.Log("Escape " + Time.time);
                        flightAI.state = FlightAI.AIState.Escape;
                        escapeTime = 0;
                    }
                }
            }
            else if (flightAI.state == FlightAI.AIState.Escape)
            {
                if (flightAI.isEscapePointReached())
                {
                    escapeTime += Time.deltaTime;
                    if (escapeTime >= maxEscapeTime)
                    {
                        Debug.Log("Attack " + Time.time);
                        flightAI.state = FlightAI.AIState.Attack;
                        lastHealth = health;
                    }
                }
            }

            if (health <= 0)
            {
                dead = true;
                GetComponentInChildren<AudioSource>().PlayOneShot(deathSound);
                StartCoroutine(Die());
            }

            // Set shader values
            ufoLight.GetComponent<MeshRenderer>().material.SetVector("_LightWorldPos", ufoLight.position);
            float d = player.getDamagePercentage();
            ufoLight.GetComponent<MeshRenderer>().material.SetColor("_Color", (1 - d) * new Color(1, 1, 1, 0.35f) + d * Color.red);

            //Rotate kuppel
            ufoKuppel.Rotate(0, 0, kuppelRotationSpeed * Time.deltaTime, Space.Self);

            loopVoiceActing();
        }
    }

    void LateUpdate()
    {
        if (!dead)
        {
            if (!isTumbling)
            {
                StabilizeRotation();
                ufoLight.gameObject.SetActive(true);
            }
            else
            {
                Tumble();
                ufoLight.gameObject.SetActive(false);
            }

            if (ufoLight.gameObject.activeSelf)
            {
                // Rotate the light
                float distance = (target.position - ufoLight.position).magnitude;
                Vector3 toTarget = target.position - ufoLight.position;
                if (!isTumbling && distance < (gameObject.GetComponentInChildren<Cone>().height * transform.localScale.x) && Vector3.Angle(target.position - ufoLight.position, -transform.up) < 85)
                {
                    currentLook = Vector3.RotateTowards(currentLook, toTarget, lightAimSpeed * Time.deltaTime, 0);
                }
                else
                {
                    currentLook = Vector3.RotateTowards(currentLook, -transform.up, lightAimSpeed * Time.deltaTime, 0);
                }

                if (Vector3.Angle(currentLook, target.position - ufoLight.position) < 0.5f)
                {
                    ufoLight.LookAt(target);
                    //Damage player
                    player.health -= lightDamage * Time.deltaTime;
                    player.isInLight = true;
                    //ToDo: Adjust sound and volume
                    lightSound.volume = 1;
                }
                else
                {
                    player.isInLight = false;
                    lightSound.volume = 0;
                    ufoLight.LookAt(ufoLight.position + currentLook);
                }
            }
            else
            {
                player.isInLight = false;
                lightSound.volume = 0;
            }
        }
    }

    void StabilizeRotation()
    {
        rb.angularVelocity = Vector3.zero;// Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * stabilizationTime);
        //print((transform.up - Vector3.up).magnitude);
        if ((transform.up - Vector3.up).magnitude < 0.01)
            transform.up = Vector3.up;
    }

    void Tumble()
    {
        if (Time.time > tumbleStartTime + tumbleTime)
        {
            print("End tumbling");
            isTumbling = false;
            return;
        }
    }

    public virtual void initializeVoiceActing ()
    {

    }

    public virtual void loopVoiceActing ()
    {

    }

    public virtual IEnumerator Die ()
    {
        float timeUntilDestroy = 10.0f;
        GameData.Instance.killedEnemies++;
        GameData.Instance.score += 1000;

        GetComponent<FlightAI>().enabled = false;
        ufoLight.gameObject.SetActive(false);
        transform.SetParent(null);
        rb.useGravity = true;
        GameObject s = Instantiate(smoke, transform.position, Quaternion.identity);
        GameObject e = Instantiate(explosion, transform.position, Quaternion.identity);
        while(timeUntilDestroy > 0)
        {
            rb.AddForce(Vector3.down * 20 * Time.deltaTime, ForceMode.Acceleration);
            s.transform.position = transform.position;
            e.transform.position = transform.position;
            timeUntilDestroy -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log(gameObject.name + " died.");
        Destroy(s);
        Destroy(e);
        Destroy(gameObject);
    }

    void OnCollisionEnter (Collision c)
    {
        if(c.gameObject.tag.Equals("Projectile"))
        {
            Debug.Log(transform.name + " hit by " + c.gameObject.name + " and taking damage");
            Projectile p = c.gameObject.GetComponent<Projectile>();
            GameData.Instance.score += 10;
            health -= p.damage;
            Destroy(Instantiate(p.hitParticleEffect, p.transform.position, Quaternion.identity, null), 1);
            Destroy(p.gameObject);
            tumbleStartTime = Time.time;
            isTumbling = true;
        }
    }
}
