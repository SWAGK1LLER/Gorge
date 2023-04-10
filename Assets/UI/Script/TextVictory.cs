using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextVictory : MonoBehaviour
{
    [SerializeField] private TMP_Text victoryText;

    private Mesh victoryMesh;
    private Vector3[] victoryVertices;

    void Update()
    {
        victoryText.ForceMeshUpdate();
        victoryMesh = victoryText.mesh;
        victoryVertices = victoryMesh.vertices;

        VictoryTextHandler();
    }

    void VictoryTextHandler()
    {
        for (int i = 0; i < victoryVertices.Length; i++)
        {
            Vector3 offset = Wobble(Time.time + i);

            victoryVertices[i] += offset;
        }

        victoryMesh.vertices = victoryVertices;
        victoryText.canvasRenderer.SetMesh(victoryMesh);
    }

    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 6.6f), Mathf.Cos(time * 5.6f));
    }
}
