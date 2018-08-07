using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public Transform center;
    public float maxDistanceFromCenter;

    private Vector3 targetScale;

	// Use this for initialization
	void Start ()
    {
        targetScale = transform.localScale;
        setLocalScales(Vector3.zero);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if ((center.position - transform.position).magnitude > maxDistanceFromCenter)
            Destroy(gameObject);

        if (transform.localScale.magnitude < targetScale.magnitude)
        {
            setLocalScales(Vector3.Lerp(transform.localScale, targetScale, 0.5f * Time.deltaTime));
        }
	}

    public void setLocalScales(Vector3 s)
    {
        transform.localScale = s;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).localScale = s;
    }
}
