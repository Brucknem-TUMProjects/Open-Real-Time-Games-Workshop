using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rollway))]
public class RollwayEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Rollway rollway = (Rollway)target;

        if (GUILayout.Button("Generate")){
            rollway.Start();
        }
    }
}
