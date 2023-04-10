using UnityEngine;

public class CrowControl : MonoBehaviour
{
    [System.Serializable]
    public class AudioSettings
    {
        public AudioSource sound;
        public AudioClip scream1;
        public AudioClip scream2;
    }

    [System.Serializable]
    public class ObjectSettings
    {
        public Animator animator;
        public Rigidbody body;
        public ChangeShaderProperty controller;
        public BirdLandingZoneHandler branches;
        public FlyingController flyController;
    }
    
    [System.Serializable]
    public class EchoSettings
    {
        public float soundRange;
        public float soundTravelSpeed;
        public int soundRepetition;
        public int soundPriority;
        public float minTimeBeforeSound;
        public float maxTimeBeforeSound;
        [Range(0, 10)] public float wingsSoundRange;
        [Range(10, 20)] public float wingsSoundTravelSpeed;
        [Range(0, 1)] public int wingsSoundRepetition;
        public int wingsSoundPriority;
    }

    [System.Serializable]
    public class MouvSettings
    {
        public Vector2 standStillTimerMinMax, flyingTimeMinMax;
    }

    public AudioSettings sound = new AudioSettings();
    public ObjectSettings objects = new ObjectSettings();
    public EchoSettings echo = new EchoSettings();
    public MouvSettings mouv = new MouvSettings();

    private Transform homeTarget;
    private Vector3 direction;
    private float distanceFromBase;
    private bool standingStill = false;
    private float standingStillTimer = 0;
    private float flyingTimer = 0;
    private float soundTimer;
    private float currentSoundTime;
    private bool doSound = false;
    public bool returnToBase = false;

    // Start is called before the first frame update
    void Start()
    {
        direction = Quaternion.Euler(transform.eulerAngles) * Vector3.forward;

        homeTarget = objects.branches.getRandomBranchToLand();
        objects.flyController.MoveToPoint = homeTarget;
    }

    void Update()
    {
        if (distanceFromBase <= 1f)
        {
            standingStill = true;
            flyingTimer = 0;
        }
        else
        {
            standingStill = false;
            standingStillTimer = 0;
            if (objects.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.31f * (3.0f / 4.0f) && Time.timeScale != 0)
            {
                objects.controller.Fire(this.transform.position, echo.wingsSoundRange, echo.wingsSoundTravelSpeed, echo.wingsSoundPriority, echo.wingsSoundRepetition);
            }
        }

        if (standingStill)
            standingStillTimer += Time.deltaTime;
        else
            flyingTimer += Time.deltaTime;

        if (standingStillTimer > Random.Range(mouv.standStillTimerMinMax.x, mouv.standStillTimerMinMax.y))
        {
            returnToBase = false;
            objects.flyController.GoToPoint = false;
            homeTarget = objects.branches.getRandomBranchToLand();
            objects.flyController.MoveToPoint = homeTarget;
            StartCoroutine(objects.flyController.Coroutine_MoveRandom());
            objects.flyController.Agent.Status = AStarAgent.AStarAgentStatus.InProgress;
        }

        if (flyingTimer > Random.Range(mouv.flyingTimeMinMax.x, mouv.flyingTimeMinMax.y))
        {
            returnToBase = true;
            objects.flyController.GoToPoint = true;
        }

        soundTimer += Time.deltaTime;
        if (soundTimer > currentSoundTime)
        {
            int rand = Random.Range(0, 2);
            if (rand == 0)
                sound.sound.clip = sound.scream1;
            else if (rand == 1)
                sound.sound.clip = sound.scream2;
            sound.sound.Play();
            currentSoundTime = Random.Range(echo.minTimeBeforeSound, echo.maxTimeBeforeSound);
            objects.controller.Fire(this.transform.position, echo.soundRange, echo.soundTravelSpeed, echo.soundPriority, echo.soundRepetition);
            soundTimer = 0;
            doSound = false;
        }
        if(sound.sound.clip.ToString() == sound.scream1.ToString())
        {
            if (sound.sound.time > 0.6f && sound.sound.time < 0.7f && !doSound)
            {
                objects.controller.Fire(this.transform.position, echo.soundRange, echo.soundTravelSpeed, echo.soundPriority, echo.soundRepetition);
                doSound = true;
            }
        }
        else
        {
            if(sound.sound.time > 0.7f && sound.sound.time < 0.8f && !doSound)
            {
                objects.controller.Fire(this.transform.position, echo.soundRange, echo.soundTravelSpeed, echo.soundPriority, echo.soundRepetition);
                doSound = true;
            }
            if(sound.sound.time > 1.3f && sound.sound.time < 1.4f && doSound)
            {
                objects.controller.Fire(this.transform.position, echo.soundRange, echo.soundTravelSpeed, echo.soundPriority, echo.soundRepetition);
                doSound = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        objects.animator.SetBool("flying", true);

        distanceFromBase = Vector3.Magnitude(homeTarget.position - objects.body.position);

        if(returnToBase && distanceFromBase < 10)
        {
            if(distanceFromBase <= 2)
            {
                objects.animator.SetBool("flying", false);
                objects.body.velocity = Vector3.zero;
                objects.body.transform.position = Vector3.MoveTowards(objects.body.transform.position, homeTarget.position, 0.2f);
                objects.body.transform.eulerAngles = Vector3.zero;
                objects.body.transform.LookAt(homeTarget.position);
                return;
            }
            objects.body.transform.position = Vector3.MoveTowards(objects.body.transform.position, homeTarget.position, 0.1f);
            objects.body.transform.LookAt(homeTarget.position);
            return;
        }
    }   
}
