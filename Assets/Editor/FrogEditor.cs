using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FrogController))]
public class FrogEditor : Editor
{    
    private void OnSceneGUI()
    {
        FrogController fov = (FrogController)target;
        
        Handles.color = Color.white;
        Handles.DrawWireCube(fov.PatrolPointOrigin, new Vector3(fov.WalkPointRange * 2, 0, fov.WalkPointRange * 2));

        Handles.color = Color.green;
        Handles.DrawLine(fov.transform.position, fov.WalkPoint);
    }
}
