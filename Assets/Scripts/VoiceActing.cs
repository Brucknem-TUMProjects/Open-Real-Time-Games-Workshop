using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceActing : MonoBehaviour
{
    [System.Serializable]
    public struct Line
    {
        [HideInInspector]
        public string name;
        public AudioClip clip;
        [Tooltip("Negative times are ignored!")]
        public Vector3 playTimes;
    }

    public bool showTime = false;
    public float volumeScale = 1;
    [Header("Each line can be played max. 3 times!")]
    public Line[] lines;

    private AudioSource source;
    private float time = 0;

    SortedList plays = new SortedList();

	// Use this for initialization
	void Start ()
    {
        source = GetComponent<AudioSource>();

		for(int i=0; i<lines.Length; i++)
        {
            if(lines[i].clip != null)
                lines[i].name = lines[i].clip.name;
            //lines[i].playTimes = -Vector3.one;    //Reset all times

            //Sort all lines into a list
            for (int j=0; j<3; j++)
            {
                if (lines[i].playTimes[j] >= 0)
                    plays.Add(lines[i].playTimes[j], i);
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (plays.Count > 0)
        {
            float nextPlayTime = (float)plays.GetKey(0);
            int nextLineIndex = (int)plays.GetByIndex(0);

            if (nextPlayTime <= time)
            {
                Debug.Log("Playing " + lines[nextLineIndex].name + " at " + time + "s");
                source.PlayOneShot(lines[nextLineIndex].clip, volumeScale);
                plays.RemoveAt(0);
            }
        }

        if (showTime)
            Debug.Log(time);

        time += Time.deltaTime;
	}

    public VoiceActing getDuplicate ()
    {
        VoiceActing tmp = new VoiceActing();
        tmp.showTime = showTime;
        tmp.volumeScale = volumeScale;
        tmp.lines = new Line[lines.Length];
        lines.CopyTo(tmp.lines, 0);
        tmp.time = time;
        tmp.plays = (SortedList) plays.Clone();
        return tmp;
    }

    public void set (VoiceActing v)
    {
        showTime = v.showTime;
        volumeScale = v.volumeScale;
        lines = new Line[v.lines.Length];
        v.lines.CopyTo(lines, 0);
        time = v.time;
        plays = (SortedList)v.plays.Clone();
    }
}
