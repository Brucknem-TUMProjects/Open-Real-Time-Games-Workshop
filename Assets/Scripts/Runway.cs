using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runway : MonoBehaviour
{
    public enum Type { Start, Land };
    public Type type = Type.Start;
    public Vector3 startRotation;
    public float fastRotationX = 0;
    public float speedToUnlockYPosition, heightIncrease;
    public float landingSpeed;
    public bool debugSpeed = false;
    public AudioClip wellDone, bestFlightStudent;
    public GameObject marker;

    private Rigidbody edgarMustang = null;
    private Vector3 landingVelocity, landingRotation, startPosition;
    private Vector3 landingDirection;
    private InputDevice input;
    private AudioSource source;
    private bool landed = false;

	// Use this for initialization
	void Start ()
    {
        input = GameObject.FindObjectOfType<InputDevice>();
        if (!GetComponent<AudioSource>())
            gameObject.AddComponent<AudioSource>().playOnAwake = false;
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (type == Type.Start)
        {
            if (edgarMustang != null)
            {
                float speed = edgarMustang.velocity.magnitude;
                if (debugSpeed)
                    Debug.Log(speed);
                if (speed > speedToUnlockYPosition)
                {
                    edgarMustang.constraints &= ~RigidbodyConstraints.FreezePositionY;
                }
                else
                {
                    float factor = Mathf.Clamp01(speed / speedToUnlockYPosition);
                    float x = (1 - factor) * startRotation.x + factor * fastRotationX;
                    edgarMustang.transform.localRotation = Quaternion.Euler(x, startRotation.y, startRotation.z);
                    edgarMustang.transform.position = new Vector3(edgarMustang.transform.position.x, startPosition.y + factor * heightIncrease, edgarMustang.transform.position.z);
                }

                // Pervent the mustang from sliding backwards when throttle is zero
                if (Vector3.Angle(edgarMustang.velocity, transform.forward) < 90)
                    edgarMustang.velocity = Vector3.zero;
            }
        }
        else if (type == Type.Land)
        {
            if(edgarMustang != null)
            {
                input.throttle = Mathf.Max(0, input.throttle - Time.deltaTime);
                if (!landed)
                {
                    Vector3 projected = Vector3.ProjectOnPlane(edgarMustang.transform.position, transform.up);
                    Vector3 downToRollway = projected - edgarMustang.transform.position;
                    Vector3 distanceToCenter = -Vector3.ProjectOnPlane(projected - transform.position, transform.forward);
                    Debug.DrawLine(edgarMustang.transform.position, edgarMustang.transform.position + downToRollway, Color.black, Time.deltaTime);
                    Debug.DrawLine(edgarMustang.transform.position, edgarMustang.transform.position + distanceToCenter, Color.gray, Time.deltaTime);
                    Debug.DrawLine(edgarMustang.transform.position, edgarMustang.transform.position + landingVelocity, Color.cyan, Time.deltaTime);

                    float crashAvoider = 0.5f / downToRollway.magnitude;
                    landingVelocity = Vector3.RotateTowards(landingVelocity, landingDirection, (landingSpeed + crashAvoider) * Time.deltaTime, 0);
                    Vector3 end = transform.position + 4 * transform.localScale.z * landingDirection;
                    float blend = Mathf.Clamp01((end - edgarMustang.transform.position).magnitude / (2 * (end - transform.position).magnitude));
                    Vector3 velocity = landingVelocity * blend;

                    if (velocity.magnitude < 1)
                    {
                        landingVelocity = Vector3.zero;
                        landed = true;
                        if (GameData.Instance.score > PlayerPrefs.GetInt("Highscore", 0))
                        {
                            source.clip = bestFlightStudent;
                            PlayerPrefs.SetInt("Highscore", (int)GameData.Instance.score);
                            GameData.Instance.objective = "Wohoo!\nNew highscore!";
                        }
                        else
                        {
                            source.clip = wellDone;
                            GameData.Instance.objective = "Beat the highscore\nnext time!";
                        }
                        StartCoroutine(loadScene());
                        source.Play();
                    }

                    edgarMustang.transform.position += velocity * Time.deltaTime;
                    edgarMustang.transform.position += 10 * landingSpeed * Time.deltaTime * downToRollway + 10 * landingSpeed * Time.deltaTime * distanceToCenter;

                    Quaternion toRotation = Quaternion.LookRotation(end - transform.position, Vector3.up);
                    toRotation = Quaternion.Euler(startRotation.x, toRotation.eulerAngles.y, toRotation.eulerAngles.z);
                    edgarMustang.transform.rotation = Quaternion.Lerp(Quaternion.Euler(landingRotation), toRotation, 1.0f - blend);
                }
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EdgarMustang"))
        {
            if (type == Type.Start)
            {
                edgarMustang = other.GetComponent<Rigidbody>();
                edgarMustang.constraints = ~(RigidbodyConstraints.FreezePositionZ);
                edgarMustang.useGravity = false;
                startPosition = edgarMustang.transform.position;
            }
            else if (type == Type.Land)
            {
                float rot = other.GetComponent<Rigidbody>().transform.localRotation.eulerAngles.x;
                if (rot > -20 && rot < 20)
                {
                    marker.SetActive(false);
                    edgarMustang = other.GetComponent<Rigidbody>();
                    landingVelocity = edgarMustang.velocity;
                    landingRotation = edgarMustang.transform.localRotation.eulerAngles;
                    landingDirection = Vector3.ProjectOnPlane(landingVelocity, transform.up);
                    landingDirection = Vector3.ProjectOnPlane(landingDirection, transform.right).normalized;
                    edgarMustang.velocity = Vector3.zero;
                    edgarMustang.isKinematic = true;
                    edgarMustang.detectCollisions = false;
                }
            }
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.CompareTag("EdgarMustang"))
        {
            if (type == Type.Start)
            {
                if (edgarMustang != null)
                {
                    edgarMustang.constraints = RigidbodyConstraints.None;
                    edgarMustang.useGravity = true;
                    // Avoid some bug with an enabled collider while on the runway
                    Collider[] colliders = edgarMustang.GetComponents<Collider>();
                    foreach (Collider c in colliders)
                        c.enabled = true;
                    edgarMustang = null;
                }
                foreach (Runway r in transform.parent.GetComponentsInChildren<Runway>())
                {
                    r.type = Runway.Type.Land;
                }
            }
        }
    }

    IEnumerator loadScene ()
    {
        yield return new WaitForSeconds(5.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
