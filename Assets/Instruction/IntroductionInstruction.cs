using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroductionInstruction : MonoBehaviour
{
    [SerializeField] private LevelScript checkpointSystem;
    [SerializeField] private TMP_Text mouv;
    [SerializeField] private TMP_Text echo;
    [SerializeField] private float mouvTextFadeSpeed;
    [SerializeField] private float echoTextFadeSpeed;

    private Mesh mouvMesh;
    private Mesh echoMesh;
    private Vector3[] mouvVertices;
    private Vector3[] echoVertices;
    private Color[] mouvColors;
    private Color[] echoColors;
    private float mouvAlpha = 0;
    private float echoAlpha = 0;
    private bool mouvTextActive = true;
    private bool echoTextActive = false;
    private bool mouvfadeIncreaseDone = false;
    private bool echofadeIncreaseDone = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mouv.ForceMeshUpdate();
        echo.ForceMeshUpdate();
        mouvMesh = mouv.mesh;
        echoMesh = echo.mesh;
        mouvVertices = mouvMesh.vertices;
        echoVertices = echoMesh.vertices;
        mouvColors = mouvMesh.colors;
        echoColors = echoMesh.colors;

        MouvTextHandler();

        EchoTextHandler();

        if (!mouvTextActive && !echoTextActive)
        {            
            checkpointSystem.CheckpointPassed.Add(this.gameObject.name);
            Destroy(this.gameObject);
        }
    }

    private Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.8f));
    }

    private void MouvTextHandler()
    {
        if (mouvTextActive)
        {
            if (mouvAlpha > 1)
                mouvfadeIncreaseDone = true;
            if (mouvfadeIncreaseDone)
                mouvAlpha -= Time.deltaTime * 0.1f * mouvTextFadeSpeed;
            else
                mouvAlpha += Time.deltaTime * 0.1f * mouvTextFadeSpeed;
        }
        for (int i = 0; i < mouvVertices.Length; ++i)
        {
            Vector3 offset = Wobble(Time.time + i);

            mouvVertices[i] += offset;
            mouvColors[i] = new Color(mouvColors[i].r, mouvColors[i].g, mouvColors[i].b, mouvAlpha);
        }

        mouvMesh.vertices = mouvVertices;
        mouvMesh.colors = mouvColors;
        mouv.canvasRenderer.SetMesh(mouvMesh);

        if (mouvAlpha < 0)
        {
            mouvTextActive = false;
            echoTextActive = true;
        }
    }

    private void EchoTextHandler()
    {
        if (echoTextActive)
        {
            if (echoAlpha > 1)
                echofadeIncreaseDone = true;
            if (echofadeIncreaseDone)
                echoAlpha -= Time.deltaTime * 0.1f * echoTextFadeSpeed;
            else
                echoAlpha += Time.deltaTime * 0.1f * echoTextFadeSpeed;
        }
        for (int i = 0; i < echoVertices.Length; ++i)
        {
            Vector3 offset = Wobble(Time.time + i);

            echoVertices[i] += offset;
            echoColors[i] = new Color(echoColors[i].r, echoColors[i].g, echoColors[i].b, echoAlpha);
        }

        echoMesh.vertices = echoVertices;
        echoMesh.colors = echoColors;
        echo.canvasRenderer.SetMesh(echoMesh);

        if (echoAlpha < 0)
        {
            echoTextActive = false;
        }
    }
}
