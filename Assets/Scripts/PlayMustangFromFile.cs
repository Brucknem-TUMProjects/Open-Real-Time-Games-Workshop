using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayMustangFromFile : MonoBehaviour
{
    public Transform spinner, edgarMustang;
    public AudioSource propellerSound, voiceActing;
    public AudioClip followMe;
    public float volumeScale = 1;
    public string fileName;
    public bool isPlaying = true;

    private string filePath;
    private float time;
    private string[] data;
    private int index;
    private RecordMustangToFile.Record currentRecord, nextRecord;
    private Vector3 endDirection;
    private Rigidbody rb;
    private float outOfRangeTime, timeUntilNextFollowMe = 0;
    private int outOfRangeIndex;
    private Vector3 edgarOutOfRangePos;

    // Use this for initialization
    void Start()
    {
        filePath = Application.dataPath + "/Animations/" + fileName;
        data = File.ReadAllLines(filePath);
        Debug.Log("Loaded " + data.Length + " states from " + filePath);
        time = 0;
        index = 2;
        currentRecord = parseLine(0);
        nextRecord = parseLine(1);
        GameData.Instance.objective = "Follow Ray Rice";
    }

    // Update is called once per frame
    void Update ()
    {
		if(nextRecord != null)
        {
            if(time >= nextRecord.time)
            {
                RecordMustangToFile.Record r = parseLine(index++);
                if(r == null)
                {
                    isPlaying = false;
                    // Save the last velocity and keep flying in this direction
                    endDirection = (nextRecord.position - currentRecord.position).normalized;
                    Debug.Log("End of Mustang record reached. Continue to fly in the same direction...");
                    Debug.Log("Enabling AttackRayRice script...");
                    rb = gameObject.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    GetComponent<AttackRayRice>().enabled = true;
                }
                currentRecord = nextRecord;
                nextRecord = r;
            }

            if (nextRecord != null)
            {
                float blend = (time - currentRecord.time) / (nextRecord.time - currentRecord.time);
                transform.position = Vector3.Lerp(currentRecord.position, nextRecord.position, blend);
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(currentRecord.rotationEuler), Quaternion.Euler(nextRecord.rotationEuler), blend);
                spinner.Rotate(Vector3.forward, (1-blend) * currentRecord.propellerSpeed + blend * nextRecord.propellerSpeed);
                propellerSound.volume = ((1 - blend) * currentRecord.propellerVolume + blend * nextRecord.propellerVolume) * volumeScale;
                propellerSound.pitch = (1 - blend) * currentRecord.propellerPitch + blend * nextRecord.propellerPitch;
            }
        }
        else
        {
            //transform.position += endVelocity * Time.deltaTime;
            spinner.Rotate(Vector3.forward, currentRecord.propellerSpeed);
            propellerSound.volume = currentRecord.propellerVolume * volumeScale;
            propellerSound.pitch = currentRecord.propellerPitch;
        }
        time += Time.deltaTime;

        if (timeUntilNextFollowMe > 0)
            timeUntilNextFollowMe -= Time.deltaTime;

        if(!isPlaying && rb.useGravity == false)
        {
            rb.velocity = endDirection * edgarMustang.GetComponent<Rigidbody>().velocity.magnitude;
        }

        float distance = (transform.position - edgarMustang.position).magnitude;
        GameData.Instance.score += (1 - Mathf.Clamp01(distance/375.0f)) * 10 * Time.deltaTime;
	}

    private RecordMustangToFile.Record parseLine (int index)
    {
        if (index < data.Length)
        {
            // Parse line
            RecordMustangToFile.Record r = new RecordMustangToFile.Record();
            string[] record = data[index].Split(' ');
            r.time = float.Parse(record[0]);
            r.position.x = float.Parse(record[1]);
            r.position.y = float.Parse(record[2]);
            r.position.z = float.Parse(record[3]);
            r.rotationEuler.x = float.Parse(record[4]);
            r.rotationEuler.y = float.Parse(record[5]);
            r.rotationEuler.z = float.Parse(record[6]);
            r.propellerSpeed = float.Parse(record[7]);
            r.propellerVolume = float.Parse(record[8]);
            r.propellerPitch = float.Parse(record[9]);
            return r;
        }
        return null;
    }

    public void OnTriggerExit (Collider c)
    {
        if(c.CompareTag("EdgarMustang") && isPlaying && timeUntilNextFollowMe <= 0)
        {
            timeUntilNextFollowMe = 5.0f;
            GetComponent<AudioSource>().enabled = true;
            GameData.Instance.objective = "WARNING!\nFly back to Ray Rice";
            Debug.Log("OUT OF RANGE");
            voiceActing.PlayOneShot(followMe, 1);
            outOfRangeTime = time;
            outOfRangeIndex = index;
            edgarOutOfRangePos = c.transform.position;
            StartCoroutine(resetPosition());
        }
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("EdgarMustang"))
        {
            GetComponent<AudioSource>().enabled = false;
            GameData.Instance.objective = "Follow Ray Rice";
        }
    }

    IEnumerator resetPosition ()
    {
        // TODO: Add image effect or warning on screen
        VoiceActing tmp = voiceActing.GetComponent<VoiceActing>().getDuplicate();
        yield return new WaitForSeconds(10.0f);
        if(edgarMustang != null && !GetComponent<Collider>().bounds.Contains(edgarMustang.position))
        {
            Debug.Log("RESETTING BECAUSE STILL OUT OF RANGE");
            voiceActing.GetComponent<VoiceActing>().set(tmp);
            time = outOfRangeTime;
            index = outOfRangeIndex;
            currentRecord = parseLine(Mathf.Max(0, index - 2));
            nextRecord = parseLine(Mathf.Max(1, index - 1));
            edgarMustang.position = edgarOutOfRangePos + 0.5f * GetComponent<SphereCollider>().radius * (currentRecord.position - edgarOutOfRangePos).normalized;
            edgarMustang.rotation = Quaternion.Euler(currentRecord.rotationEuler);
            edgarMustang.GetComponent<Rigidbody>().velocity = (nextRecord.position - currentRecord.position) / (nextRecord.time - currentRecord.time);
        }
    }
}
