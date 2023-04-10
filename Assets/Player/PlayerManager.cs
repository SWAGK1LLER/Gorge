using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    private enum State { Ground, Air };
    private enum Environment { Cave, Forest };
    private GameMaster gameMaster;
    private LevelScript levelMaster;

    [System.Serializable]
    public class AudioSettings
    {
        public AudioMixer mixer;
        public AudioSource sound;
        public AudioClip wingFlap;
        public AudioClip waterDeathSound;
        public List<AudioClip> screams;
        public LayerMask ceilling;
    }

    [System.Serializable]
    public class UISettings
    {
        public TMP_Text collectTxt;
        public Animator stopwatch;
        public Animator stopwatchButton;
        public Animator stopwwatchIdle;
        public AudioClip stopwatchAudio;
        public float stopwatchFirstTimer;
        public float stopwatchSecondTimer;
        public float collectTxtFadeSpeed;
    }

    [System.Serializable]
    public class MouvSettings
    {
        public LayerMask wallToCollide;  
        public float movementSpeed;
        public float dashSpeed;
        public float jumpHeight;
        public float originalGravity;
        public float gravityScale;
        public float jumpCooldown;
        public float dashCooldown;
        public float dashTimer;
        public float eatingTime;
    }

    [System.Serializable]
    public class EchoSettings
    {
        public float eatEchoRadius;
        public float eatEchoTravelSpeed;
        public float echoCooldown;
        public float playerEchoDist;
        public float playerEchoTravelSpeed;
        public int nbEchoShot;
        public int nbEchoFromEating;
        public int nbEchoFromWings;
        public int nbEchoFromDash;
        public float wingEchoTravelSpeed;
        public float wingEchoDist;
        public float dashEchoTravelSpeed;
        public float dashEchoDist; 
    }

    [System.Serializable] 
    public class AnimationSettings
    {
        public Animator animator;
        public Animation anime;
    }

    [System.Serializable]
    public class ObjectSettings
    {
        public ChangeShaderProperty controller;
        public PlayerInput input;
        public GameObject playerCam;
        public Transform originRayCast;
        public List<GameObject> distToCollide;
        public Rigidbody body;
        public GameObject pauseMenu;
    }

    public AudioSettings sound = new AudioSettings();
    public MouvSettings mouv = new MouvSettings();
    public EchoSettings echo = new EchoSettings();
    public AnimationSettings animate = new AnimationSettings();
    public ObjectSettings objects = new ObjectSettings();
    public UISettings UI = new UISettings();

    //UI Collected Txt
    private Mesh collectTxtMesh;
    private LevelChanger lvlChanger;
    private Vector3[] collectTxtVertices;
    private Color[] collectTxtColors;
    private float collectTxtAlpha = 0;
    private bool collectTxtActive = false;
    private bool collectTxtfadeIncreaseDone = false;

    private State gameState;
    private Environment environment;
    private float wingTimer;
    private float dashInCooldown;
    private float jumpForce;
    private float currentHeight;
    private float turnSmoothVelocity;
    private float wingEchoTimer = 0;
    private float timer = 0;
    private float eatTimer = 0;
    private float prevTimeScale;
    private bool wingReady;
    private bool jumping;
    private bool activated = false;
    private bool inCollectibleRange = false;
    private bool inFrontOfWall = false;
    private bool waterDeath = false;

    Vector2 movementVector;
    private List<Collider> collectableInRange;

    private AnimationClip clip;
    private AnimationEvent evt;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("GM") != null)
        {
            gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }

        if (GameObject.FindGameObjectWithTag("LM") != null)
        {
            levelMaster = GameObject.FindGameObjectWithTag("LM").GetComponent<LevelScript>();
        }

        if(GameObject.FindGameObjectWithTag("LV") != null)
            lvlChanger = GameObject.FindGameObjectWithTag("LV").GetComponent<LevelChanger>();

        if (gameMaster != null && SceneManager.GetActiveScene().buildIndex != 0 && gameMaster.CurrentCheckpoint != Vector3.zero)
        {
            objects.body.transform.position = gameMaster.CurrentCheckpoint;
            objects.body.transform.eulerAngles = gameMaster.CurrentRotation;
        }

        wingTimer = 0;
        wingReady = true;
        objects.body.drag = mouv.originalGravity;
        jumpForce = Mathf.Sqrt(mouv.jumpHeight * -2 * (Physics2D.gravity.y * objects.body.drag));
        collectableInRange = new List<Collider>();

        gameState = State.Air;
        environment = Environment.Cave;

        evt = new AnimationEvent();
        evt.functionName = "wingEchoShot";
        evt.time = 1;

        clip = animate.animator.runtimeAnimatorController.animationClips[0];
        clip.AddEvent(evt);

        dashInCooldown = mouv.dashCooldown;

        if(!UI.stopwatch || !UI.stopwatchButton || !UI.stopwwatchIdle)
        {
            UI = null;
        }

        prevTimeScale = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchGravity();

        CheckIsJumping();

        EchoScream();

        CollectTxtUpdate();

        CheckEnvironment();

        if(prevTimeScale != Time.timeScale && Time.timeScale == 1)
        {
            StartCoroutine(EnableInputs());
        }

        prevTimeScale = Time.timeScale;
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        Vector2 inputDir = movementVector.normalized;
        if (inputDir != Vector2.zero)
        {
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + objects.playerCam.transform.eulerAngles.y;
            objects.body.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(objects.body.transform.eulerAngles.y, rotation, ref turnSmoothVelocity, 0.2f);
        }
        
        for (int i = 0; i < objects.distToCollide.Count; ++i)
        {
            if (objects.distToCollide[i] != null)
            {
                Debug.DrawLine(objects.originRayCast.position, objects.distToCollide[i].transform.position);
                if (!Physics.Linecast(objects.originRayCast.position, objects.distToCollide[i].transform.position, mouv.wallToCollide))
                {
                    inFrontOfWall = false;
                    continue;
                }
                inFrontOfWall = true;
                break;
            }
        }

        if (!inFrontOfWall)
        {
            objects.body.transform.Translate(objects.body.transform.forward * (mouv.movementSpeed * inputDir.magnitude) * Time.deltaTime, Space.World);
        }
    }

    private void SwitchGravity()
    {
        if (this.gameState == State.Air)
            objects.body.drag = mouv.gravityScale;

        else
            objects.body.drag = mouv.originalGravity;
    }

    private void CheckIsJumping()
    {
        if (jumping)
        {
            if (objects.body.transform.position.y < currentHeight + mouv.jumpHeight)
                objects.body.velocity = new Vector3(objects.body.velocity.x, jumpForce, objects.body.velocity.z);
            else
                jumping = false;
        }

        if (!wingReady)
            wingTimer += Time.deltaTime;

        dashInCooldown += Time.deltaTime;

        if (wingTimer > mouv.jumpCooldown)
        {
            wingTimer = 0;
            wingReady = true;
        }
        else
        {
            wingEchoTimer += Time.deltaTime;
            if (wingEchoTimer > animate.anime.clip.length)
            {
                wingEchoTimer = 0;
            }
        }
    }

    private void EchoScream()
    {
        if (activated)
        {
            timer += Time.deltaTime;
            if (timer > echo.echoCooldown)
            {
                timer = 0;
                activated = false;
            }
        }
    }

    private void CollectTxtUpdate()
    {
        if (collectTxtActive)
        {
            UI.collectTxt.SetText((gameMaster.LevelCollectibleGot + levelMaster.LevelCollectibleGot) + " / " + gameMaster.LevelCollectibleNb + " meat collected");
            UI.collectTxt.ForceMeshUpdate();
            collectTxtMesh = UI.collectTxt.mesh;
            collectTxtVertices = collectTxtMesh.vertices;
            collectTxtColors = collectTxtMesh.colors;

            CollectTxtHandler();
        }
    }

    private void CollectTxtHandler()
    {
        if (collectTxtActive)
        {
            if (collectTxtAlpha > 1)
                collectTxtfadeIncreaseDone = true;
            if (collectTxtfadeIncreaseDone)
                collectTxtAlpha -= Time.deltaTime * 0.1f * UI.collectTxtFadeSpeed;
            else
                collectTxtAlpha += Time.deltaTime * 0.1f * UI.collectTxtFadeSpeed;
        }

        for (int i = 0; i < collectTxtVertices.Length; ++i)
        {
            Vector3 offset = Wobble(Time.time + i);

            collectTxtVertices[i] += offset;
            collectTxtColors[i] = new Color(collectTxtColors[i].r, collectTxtColors[i].g, collectTxtColors[i].b, collectTxtAlpha);
        }

        collectTxtMesh.vertices = collectTxtVertices;
        collectTxtMesh.colors = collectTxtColors;
        UI.collectTxt.canvasRenderer.SetMesh(collectTxtMesh);

        if (collectTxtAlpha < 0)
        {
            collectTxtActive = false;
        }
    }

    private Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.8f));
    }

    private void CheckEnvironment()
    {
        if(Physics.Raycast(this.transform.position, this.transform.up, Mathf.Infinity, sound.ceilling) && environment != Environment.Cave)
        {
            StartCoroutine(FadeMixerGroup.StartFade(sound.mixer, "CaveSoundtrack", 10, 1));
            StartCoroutine(FadeMixerGroup.StartFade(sound.mixer, "ForestSoundtrack", 10, 0));
            environment = Environment.Cave;
        }

        else if(!Physics.Raycast(this.transform.position, this.transform.up, Mathf.Infinity, sound.ceilling) && environment == Environment.Cave)
        {
            StartCoroutine(FadeMixerGroup.StartFade(sound.mixer, "CaveSoundtrack", 10, 0));
            StartCoroutine(FadeMixerGroup.StartFade(sound.mixer, "ForestSoundtrack", 10, 0.5f));
            environment = Environment.Forest;
        }
    }

    private IEnumerator SwitchState()
    {
        yield return new WaitUntil(() => this.GetComponent<Rigidbody>().velocity.y < 0);
        this.gameState = State.Air;
    }

    private IEnumerator EnableInputs()
    {
        yield return new WaitForSeconds(0.1f);
        objects.input.enabled = true;
    }

    private IEnumerator EatCooldown()
    {
        float startTime = Time.time;
        

        while (Time.time < startTime + mouv.eatingTime)
        {            
            yield return null;
        }

        objects.input.currentActionMap.FindAction("Move").Enable();
        objects.input.currentActionMap.FindAction("Fire").Enable();
        objects.input.currentActionMap.FindAction("Jump").Enable();
        objects.input.currentActionMap.FindAction("Collect").Enable();
        objects.input.currentActionMap.FindAction("Dash").Enable();
    }

    private IEnumerator DashActive()
    {
        if (dashInCooldown > mouv.dashCooldown)
        {
            float startTime = Time.time;
            dashInCooldown = 0;
            objects.playerCam.GetComponent<CameraControl>().InDash = true;

            objects.controller.Fire(objects.body.transform.position, echo.dashEchoDist, echo.dashEchoTravelSpeed, 0, echo.nbEchoFromDash);
            
            while (Time.time < startTime + mouv.dashTimer)
            {
                for (int i = 0; i < objects.distToCollide.Count; ++i)
                {
                    Debug.DrawLine(objects.originRayCast.position, objects.distToCollide[i].transform.position);
                    if (!Physics.Linecast(objects.originRayCast.position, objects.distToCollide[i].transform.position, mouv.wallToCollide))
                    {
                        inFrontOfWall = false;
                        continue;
                    }
                    inFrontOfWall = true;
                    break;
                }

                if (!inFrontOfWall)
                {
                    Vector2 inputDir = movementVector.normalized;
                    objects.body.transform.Translate(objects.body.transform.forward * mouv.dashSpeed * Time.deltaTime, Space.World);
                }
                /*for (int i = 0; i < objects.distToCollide.Count; ++i)
                {
                    if (!Physics.Linecast(objects.originRayCast.position, objects.distToCollide[i].transform.position))
                    {
                        Vector2 inputDir = movementVector.normalized;
                        objects.body.transform.Translate(objects.body.transform.forward * mouv.dashSpeed * Time.deltaTime, Space.World);
                    }
                }*/
                yield return null;
            }
            objects.playerCam.GetComponent<CameraControl>().InDash = false;
        }
    }

    private IEnumerator StopWatchTimer()
    {
        yield return new WaitForSeconds(UI.stopwatchFirstTimer);
        UI.stopwatchButton.SetTrigger("Press");
        yield return new WaitForSeconds(UI.stopwatchSecondTimer);
        UI.stopwatch.SetBool("Popped", false);
    }

    public void Move(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (wingReady && Time.timeScale == 1)
        {
            if (context.performed)
            {
                objects.body.velocity = new Vector3(objects.body.velocity.x, jumpForce, objects.body.velocity.z);
                objects.body.AddRelativeForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                wingReady = false;
            }
        }
    }

    public void Collect(InputAction.CallbackContext context)
    {
        this.GetComponent<Animator>().Play("Collect");
        if (collectableInRange.Count > 0)
        {
            objects.input.currentActionMap.FindAction("Move").Disable();
            objects.input.currentActionMap.FindAction("Fire").Disable();
            objects.input.currentActionMap.FindAction("Jump").Disable();
            objects.input.currentActionMap.FindAction("Dash").Disable();

            LevelScript checkpointSystem = GameObject.FindGameObjectWithTag("LM").GetComponent<LevelScript>();
            for (int i = 0; i < collectableInRange.Count; ++i)
            {
                objects.controller.Fire(collectableInRange[i].gameObject.transform.position, echo.eatEchoRadius, echo.eatEchoTravelSpeed, 0, echo.nbEchoFromEating);
                checkpointSystem.Collectibles.Add(collectableInRange[i].gameObject.name);
                checkpointSystem.LevelCollectibleGot++;
                Destroy(collectableInRange[i].gameObject);
            }
            StartCoroutine(EatCooldown());
            collectTxtActive = true;
            collectTxtAlpha = 0;
            collectTxtfadeIncreaseDone = false;
        }

        collectableInRange.Clear();
        inCollectibleRange = false;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        StartCoroutine(DashActive());
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!activated)
            {
                this.GetComponent<Animator>().Play("Echo");
                int rand = UnityEngine.Random.Range(0, sound.screams.Count);
                sound.sound.PlayOneShot(sound.screams[rand]);
                Vector4 position = new Vector4(objects.body.transform.position.x, objects.body.transform.position.y, objects.body.transform.position.z, -0.25f);
                objects.controller.Fire(position, echo.playerEchoDist, echo.playerEchoTravelSpeed, 0, echo.nbEchoShot);
                activated = true;
                if (UI != null)
                {
                    if (UI.stopwatch != null)
                    {
                        UI.stopwatch.SetBool("Popped", true);
                        UI.stopwwatchIdle.SetTrigger("Echo");
                        sound.sound.PlayOneShot(UI.stopwatchAudio);
                        StartCoroutine(StopWatchTimer());
                    }
                   
                }
            }
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        objects.input.enabled = false;
        Time.timeScale = 0;        
        objects.pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioListener.pause = true;
    }

    public void FinishLevel()
    {
        gameMaster.Collectibles.Clear();
        gameMaster.CheckpointPassed.Clear();
        gameMaster.LevelCollectibleGot = 0;
        gameMaster.LevelCollectibleNb = 0;
        if (lvlChanger != null)
            lvlChanger.FadeInToNextLevel(gameMaster.CurrentScene + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable") && !inCollectibleRange)
        {
            inCollectibleRange = true;
            collectableInRange.Add(other);
            other.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        if (other.gameObject.CompareTag("Finish") && gameMaster != null)
        {
            FinishLevel();
        }

        if(other.gameObject.CompareTag("Water") && !waterDeath)
        {
            waterDeath = true;
            if (sound.waterDeathSound != null)
                sound.sound.PlayOneShot(sound.waterDeathSound);
            objects.input.enabled = false;
            lvlChanger.FadeInToNextLevel(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            inCollectibleRange = false;
            collectableInRange.Remove(other);
            other.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && this.gameState == State.Air)
        {
            this.gameState = State.Ground;
            StopCoroutine(SwitchState());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            StartCoroutine(SwitchState());
    }

    public void wingEchoShot()
    {
        sound.sound.PlayOneShot(sound.wingFlap);
        objects.controller.Fire(objects.body.transform.GetChild(2).position, echo.wingEchoDist, echo.wingEchoTravelSpeed, 0, echo.nbEchoFromWings);
        objects.controller.Fire(objects.body.transform.GetChild(3).position, echo.wingEchoDist, echo.wingEchoTravelSpeed, 0, echo.nbEchoFromWings);
    }
}
