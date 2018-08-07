using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("Game will be over/reset when the player health reaches 0")]
    public float health;
    public float healthRegenerationSpeed;
    public bool isInLight = false;
    public GameObject youAreDeadCanvas, explosionPrefab;

    private float maxHealth;
    private bool dead = false;

	// Use this for initialization
	void Start ()
    {
        maxHealth = health;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(!isInLight)
        {
            health = Mathf.Min(health + healthRegenerationSpeed * Time.deltaTime, maxHealth);
        }

        if(!dead && health <= 0)
        {
            Die();
        }
	}

    public virtual float getDamagePercentage ()
    {
        return 1 - (health / maxHealth);
    }

    public virtual void Die ()
    {
        // TODO: Reset scene
        Debug.Log("Game over");
        dead = true;
        youAreDeadCanvas.SetActive(true);
        StartCoroutine(loadScene());
    }

    public virtual void OnCollisionEnter (Collision c)
    {
        health -= c.impulse.magnitude / 4000;
        Debug.Log("Impuls: " + c.impulse.magnitude / 4000);
        if (health <= 0)
            Destroy(Instantiate(explosionPrefab, transform, false), 10.0f);
    }

    IEnumerator loadScene ()
    {
        yield return new WaitForSeconds(5.0f);
        if (GameObject.FindObjectOfType<UfoSpawner>())
            UnityEngine.SceneManagement.SceneManager.LoadScene("Action");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
