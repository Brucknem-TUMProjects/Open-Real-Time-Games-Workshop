using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RecordMustangToFile : MonoBehaviour
{
    public AudioSource propellerSound;
    public float recordsPerSecond;
    public string fileName;

    private float timeUntilNextRecord = 0;
    private string filePath;
    private float time;

    public class Record
    {
        public float time;
        public Vector3 position;
        public Vector3 rotationEuler;
        public float propellerSpeed;
        public float propellerVolume;
        public float propellerPitch;
    }

    private LinkedList<Record> records = new LinkedList<Record>();

    // Use this for initialization
    void Start ()
    {
        filePath = Application.dataPath + "/Animations/" + fileName;
        Debug.Log("RecordToFile filePath: " + filePath);
        time = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(timeUntilNextRecord <= 0)
        {
            Record r = new Record();
            r.time = time;
            r.position = transform.position;
            r.rotationEuler = transform.rotation.eulerAngles;
            r.propellerSpeed = GetComponentInChildren<RotatePropeller>().speed;
            r.propellerVolume = propellerSound.volume;
            r.propellerPitch = propellerSound.pitch;
            records.AddLast(r);
            timeUntilNextRecord = 1/recordsPerSecond;
        }
        else
        {
            timeUntilNextRecord -= Time.deltaTime;
        }

        time += Time.deltaTime;
	}

    void OnDestroy ()
    {
        if (GetComponent<RecordMustangToFile>().enabled)
        {
            // Save data to file in time posX posY posZ rotX rotY rotZ propellerSpeed propellerVolume
            if (!File.Exists(filePath))
                File.Create(filePath);

            string[] data = new string[records.Count];
            int index = 0;
            foreach (Record r in records)
            {
                data[index] = "" + r.time + " " + r.position.x + " " + r.position.y + " " + r.position.z + " " + r.rotationEuler.x + " " + r.rotationEuler.y + " " + r.rotationEuler.z + " " + r.propellerSpeed + " " + r.propellerVolume + " " + r.propellerPitch;
                index++;
            }

            File.WriteAllLines(filePath, data);
            Debug.Log("Saved " + data.Length + " states to " + filePath);
        }
    }
}
