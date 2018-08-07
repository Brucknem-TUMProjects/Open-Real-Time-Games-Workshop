using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitController : MonoBehaviour {

    [Header("Altimeters")]
    public Altimeter velocity;
    public Altimeter pitch;
    public Altimeter roll;
    public Altimeter yaw;

    [Header("Reference Object")]
    public GameObject mustang;

    // Use this for initialization
    void Start () {
        EdgarMustang em = mustang.GetComponent<EdgarMustang>();
        velocity.maxValue = em.maxSpeed * 4;
        velocity.SetScala();
    }
	
	// Update is called once per frame
	void Update () {
        velocity.currValue = GameData.Instance.velocity * 3;
        pitch.currValue = mustang.transform.rotation.eulerAngles.x;
        roll.currValue =  (mustang.transform.rotation.eulerAngles.z + 90) % 360;
        //Debug.Log("Roll Value: " + roll.currValue);
        yaw.currValue =   mustang.transform.rotation.eulerAngles.y;
	}
}
