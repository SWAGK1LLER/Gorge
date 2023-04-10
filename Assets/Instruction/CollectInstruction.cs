using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectInstruction : MonoBehaviour
{
    [SerializeField] private LevelScript checkpointSystem;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform collectibleToLookAt;
    [SerializeField] private TMP_Text collect;
    [SerializeField] private AnimationCurve myCurve;
    [SerializeField] private float collectTextFadeSpeed;
    [SerializeField] private float camMouvSpeed;
    [SerializeField] private float distCamFromCollectible;

    private PlayerManager playerController;
    private PlayerInput playerInput;
    private Mesh collectMesh;
    private Vector3[] collectVertices;
    private Color[] collectColors;
    private float collectAlpha = 0;
    private bool collectTextActive = true;
    private bool collectfadeIncreaseDone = false;
    private bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        arrow.GetComponent<SpriteRenderer>().enabled = false;

        collect.ForceMeshUpdate();
        collectMesh = collect.mesh;
        collectVertices = collectMesh.vertices;
        collectColors = collectMesh.colors;

        for (int i = 0; i < collectVertices.Length; ++i)
        {
            collectColors[i] = new Color(collectColors[i].r, collectColors[i].g, collectColors[i].b, collectAlpha);
        }

        collectMesh.vertices = collectVertices;
        collectMesh.colors = collectColors;
        collect.canvasRenderer.SetMesh(collectMesh);

        playerController = player.GetComponent<PlayerManager>();
        playerInput = player.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            showCollectible();

            collect.ForceMeshUpdate();
            collectMesh = collect.mesh;
            collectVertices = collectMesh.vertices;
            collectColors = collectMesh.colors;

            JumpTextHandler();


            if (!collectTextActive)
            {                
                checkpointSystem.CheckpointPassed.Add(this.gameObject.name);
                cam.GetComponent<CameraControl>().enabled = true;
                arrow.GetComponent<SpriteRenderer>().enabled = false;
                playerController.enabled = true;
                playerInput.enabled = true;
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
        if (collectTextActive)
        {
            if (collectAlpha > 1)
                collectfadeIncreaseDone = true;
            if (collectfadeIncreaseDone)
                collectAlpha -= Time.deltaTime * 0.1f * collectTextFadeSpeed;
            else
                collectAlpha += Time.deltaTime * 0.1f * collectTextFadeSpeed;
        }

        for (int i = 0; i < collectVertices.Length; ++i)
        {
            Vector3 offset = Wobble(Time.time + i);

            collectVertices[i] += offset;
            collectColors[i] = new Color(collectColors[i].r, collectColors[i].g, collectColors[i].b, collectAlpha);
        }

        collectMesh.vertices = collectVertices;
        collectMesh.colors = collectColors;
        collect.canvasRenderer.SetMesh(collectMesh);

        if (collectAlpha < 0)
        {
            collectTextActive = false;
        }
    }

    private void showCollectible()
    {
        arrow.transform.position = new Vector3(arrow.transform.position.x, myCurve.Evaluate((Time.time % myCurve.length)), arrow.transform.position.z);

        if (Vector3.Distance(cam.transform.position, collectibleToLookAt.position) > distCamFromCollectible)
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, collectibleToLookAt.position, Time.deltaTime * camMouvSpeed);

        cam.transform.LookAt(collectibleToLookAt.position);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.5f);
        if (!triggered)
        {
            PlayerManager.EchoSettings echo = playerController.echo;
            triggered = true;
            cam.GetComponent<CameraControl>().enabled = false;
            arrow.GetComponent<SpriteRenderer>().enabled = true;
            playerController.enabled = false;
            playerInput.enabled = false;
            playerController.objects.controller.Fire(collectibleToLookAt.position, echo.eatEchoRadius, echo.eatEchoTravelSpeed, 0, echo.nbEchoFromEating);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Timer());
    }
}
