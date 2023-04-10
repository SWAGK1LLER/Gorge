using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollectibleBehaviour))]
public class CollectibleRadius : Editor
{
    private void OnSceneGUI()
    {
        CollectibleBehaviour collectible = (CollectibleBehaviour)target;

        Handles.DrawWireDisc(collectible.transform.position, Vector3.up, collectible.PickupRadius);
        Handles.DrawWireCube(collectible.transform.position, new Vector3(collectible.PickupRadius * 2, collectible.PickupHeight, 0));
    }
}
