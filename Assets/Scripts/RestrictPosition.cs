using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictPosition : MonoBehaviour
{
    public Vector3 origin;
    public float maxDistance;
    public float maxHeight;
    public GameObject restriction;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float y = Mathf.Min(transform.position.y, maxHeight);
        Vector3 pos = Vector3.ProjectOnPlane(transform.position, Vector3.up) - origin;
        restriction.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white * (Mathf.Clamp01(Mathf.Pow((pos.magnitude / maxDistance), 2)) + Mathf.Clamp01(Mathf.Pow(y/maxHeight, 2))));
        Vector3 clamped = transform.position;
        if (pos.magnitude > maxDistance)
        {
            clamped = origin + (pos / (pos.magnitude / maxDistance));
        }
        transform.position = new Vector3(clamped.x, y, clamped.z);
    }

    public void OnDestroy ()
    {
        Destroy(restriction);
    }
}
