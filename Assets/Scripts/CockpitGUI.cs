using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CockpitGUI : MonoBehaviour {

    public Text objective, health, score;
    public Player player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        objective.text = GameData.Instance.objective;
        health.text = player.health.ToString("0");
        score.text = GameData.Instance.score.ToString("0");
    }
}
