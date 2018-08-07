using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldGUI : MonoBehaviour {

    public Text killedEnemies, timeLeft;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        killedEnemies.text = "Killed Enemies:\n" + GameData.Instance.killedEnemies;
        timeLeft.text = "Time left:\n" + GameData.Instance.timePassed.ToString("00.00");
    }
}
