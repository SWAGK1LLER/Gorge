using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyChasingAI))]
public class ChasingFOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyChasingAI fov = (EnemyChasingAI)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(fov.Spotter.position, Vector3.up, Vector3.forward, 360, fov.SoundDetectionRange);

        Vector3 rightAngle = DirFromAngle(fov.Spotter.eulerAngles.y, -fov.DetectionFOV / 2);
        Vector3 leftAngle = DirFromAngle(fov.Spotter.eulerAngles.y, fov.DetectionFOV / 2);

        Handles.color = Color.yellow;
        Handles.DrawWireArc(fov.Spotter.position, Vector3.up, Vector3.forward, 360, fov.ChaseRange);
        Handles.DrawLine(fov.Spotter.position, fov.Spotter.position + rightAngle * fov.ChaseRange);
        Handles.DrawLine(fov.Spotter.position, fov.Spotter.position + leftAngle * fov.ChaseRange);

        if(fov.CanSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.Spotter.position, fov.Player.position);
        }

        Handles.color = Color.red;
        Handles.DrawWireArc(fov.Spotter.position, Vector3.up, Vector3.forward, 360, fov.AttackRange);

        Handles.color = Color.white;
        if(fov.FreeRoamingEnemy)
            Handles.DrawWireCube(fov.Spotter.position, new Vector3(fov.WalkPointRange * 2, 0, fov.WalkPointRange * 2));
        else
            Handles.DrawWireCube(fov.PatrolPointOrigin, new Vector3(fov.WalkPointRange * 2, 0, fov.WalkPointRange * 2));

        Handles.DrawLine(fov.WalkPoint + new Vector3(0, 10, 0), fov.WalkPoint - new Vector3(0, 10, 0));
    }

    private Vector3 DirFromAngle(float eulerY, float angle)
    {
        angle += eulerY;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
