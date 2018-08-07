using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Altimeter : MonoBehaviour {

    [Header("Settings")]
    public float minValue;
    public float maxValue;
    public float currValue;
    public float maxAngle;
    public bool useScala;

    [Header("Used Visualizations")]
    public Transform pointer;
    public Text title;
    public Text[] scala;

    private float scalaWidth;
    private float textCount;
    private float angle = 0;

	// Use this for initialization
	void Start () {
        SetScala();
        float maxValue = maxAngle / 180 * 4 * scalaWidth + minValue;
    }
	
	// Update is called once per frame
	void Update () {
        angle = (Mathf.Clamp(currValue, minValue, maxValue) - minValue) / scalaWidth * 45;
        Quaternion rotation = pointer.localRotation;
        rotation.eulerAngles = new Vector3(90f + angle, -90f, -90f);
        pointer.localRotation = rotation;
    }

    public void SetScala()
    {
        scalaWidth = (maxValue - minValue) / 8 * (360 / maxAngle);
        if (useScala)
        {
            textCount = (maxAngle + 0.001f) / 360 * 8;
            float offset = Mathf.Clamp(Mathf.Floor((currValue - minValue) / (8 * scalaWidth)), 0, Mathf.Clamp((maxAngle / 360) - 1, 0, 1000000));

            for (int i = 0; i < scala.Length; i++)
            {
                if (i < textCount)
                {
                    scala[i].text = (minValue + i * scalaWidth + offset * (8 * scalaWidth)).ToString();
                }
                else
                    scala[i].text = "";
            }
        }
        else
        {
            foreach (var s in scala)
            {
                s.text = "";
            }
        }
    }

    private void OnValidate()
    {
        Start();
        Update();
        title.text = gameObject.name;
    }
}