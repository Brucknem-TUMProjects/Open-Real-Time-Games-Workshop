using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {

    private static GameData instance;

    private GameData()
    {
        if (instance != null)
            return;
        instance = this;
    }

    public static GameData Instance
    {
        get
        {
            if (instance == null)
                instance = new GameData();
            return instance;
        }
    }

    public float score = 0;
    public int killedEnemies = 0;
    public float velocity = 0;
    public float timePassed = 0;
    public string objective = "";

    void Reset()
    {
        score = 0;
        killedEnemies = 0;
        velocity = 0;
        timePassed = 0;
        objective = "";
    }
}
