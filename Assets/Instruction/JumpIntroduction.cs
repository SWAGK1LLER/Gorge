using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JumpIntroduction : MonoBehaviour
{
    [SerializeField] private LevelScript checkpointSystem;
    [SerializeField] private TMP_Text jump;
    [SerializeField] private float jumpTextFadeSpeed;

    private Mesh jumpMesh;
    private Vector3[] jumpVertices;
    private Color[] jumpColors;
    private float jumpAlpha = 0;
    private bool jumpTextActive = true;
    private bool jumpfadeIncreaseDone = false;
    private bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        jump.ForceMeshUpdate();
        jumpMesh = jump.mesh;
        jumpVertices = jumpMesh.vertices;
        jumpColors = jumpMesh.colors;

        for (int i = 0; i < jumpVertices.Length; ++i)
        {
            jumpColors[i] = new Color(jumpColors[i].r, jumpColors[i].g, jumpColors[i].b, jumpAlpha);
        }

        jumpMesh.vertices = jumpVertices;
        jumpMesh.colors = jumpColors;
        jump.canvasRenderer.SetMesh(jumpMesh);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            jump.ForceMeshUpdate();
            jumpMesh = jump.mesh;
            jumpVertices = jumpMesh.vertices;
            jumpColors = jumpMesh.colors;

            JumpTextHandler();

            if (!jumpTextActive)
            {                
                checkpointSystem.CheckpointPassed.Add(this.gameObject.name);
                Destroy(this.gameObject);
            }
        }
    }

    private Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.8f));
    }

    private void JumpTextHandler()
    {
        if (jumpTextActive)
        {
            if (jumpAlpha > 1)
                jumpfadeIncreaseDone = true;
            if (jumpfadeIncreaseDone)
                jumpAlpha -= Time.deltaTime * 0.1f * jumpTextFadeSpeed;
            else
                jumpAlpha += Time.deltaTime * 0.1f * jumpTextFadeSpeed;
        }

        for (int i = 0; i < jumpVertices.Length; ++i)
        {
            Vector3 offset = Wobble(Time.time + i);

            jumpVertices[i] += offset;
            jumpColors[i] = new Color(jumpColors[i].r, jumpColors[i].g, jumpColors[i].b, jumpAlpha);
        }

        jumpMesh.vertices = jumpVertices;
        jumpMesh.colors = jumpColors;
        jump.canvasRenderer.SetMesh(jumpMesh);

        if (jumpAlpha < 0)
        {
            jumpTextActive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered)
            triggered = true;
    }
}
