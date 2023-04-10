using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyStaticAi))]
public class StaticFOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyStaticAi fov = (EnemyStaticAi)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.SoundDetectionRange);

        Vector3 rightAngle = DirFromAngle(fov.transform.eulerAngles.y, -fov.DetectionFOV / 2);
        Vector3 leftAngle = DirFromAngle(fov.transform.eulerAngles.y, fov.DetectionFOV / 2);

        Handles.color = Color.yellow;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ChaseRange);
        Handles.DrawWireArc(fov.transform.position, Vector3.right, Vector3.forward, 360, fov.ChaseRange);
        Handles.DrawLine(fov.transform.position, fov.transform.position + rightAngle * fov.ChaseRange);
        Handles.DrawLine(fov.transform.position, fov.transform.position + leftAngle * fov.ChaseRange);

        if(fov.CanSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.Player.position);
        }

        Handles.color = Color.red;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.AttackRange);

        Handles.color = Color.white;
        Handles.DrawWireCube(fov.transform.position, new Vector3(fov.WalkPointRange * 2, 0, fov.WalkPointRange * 2));
    }

    private Vector3 DirFromAngle(float eulerY, float angle)
    {
        angle += eulerY;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
