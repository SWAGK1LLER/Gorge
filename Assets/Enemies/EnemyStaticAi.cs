using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStaticAi : MonoBehaviour
{
    private GameMaster gameMaster;

    [SerializeField] private Transform player;
    [SerializeField] private EcholocationManager echoManager;
    [SerializeField] private ChangeShaderProperty shaderControls;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] private float detectionFOV;

    //Patroling
    [SerializeField] private Vector3 patrolPointOrigin;
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private float DistractionThreshold;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float rotateHeadSpeed;
    private bool walkPointSet;

    //Attacking
    [SerializeField] private float increaseOfRangeToKill;
    private float originalAttackRange;

    //States
    [SerializeField] private float soundDetectionRange;
    [SerializeField] private float chaseRange;
    [SerializeField] private float attackRange;
    private bool playerInChaseRange;
    private bool playerInAttackRange;

    //Sounds
    [SerializeField] private AudioSource soundSourceAmbient;
    [SerializeField] private AudioSource soundSourceAttack;
    [SerializeField] private List<AudioClip> ambiantSoundList;
    [SerializeField] private List<AudioClip> attackSoundList;
    [SerializeField] private List<float> ambiantSound1Delay;
    [SerializeField] private List<float> ambiantSound2Delay;
    [SerializeField] private float minSoundTime;
    [SerializeField] private float maxSoundTime;
    private float timeToPlaySound;
    private float soundTimer;
    private List<List<float>> ambiantSoundDelay;
    private List<List<float>> attackSoundDelay;

    //Echo    
    [SerializeField] private float soundRange;
    [SerializeField] private float soundTravelSpeed;
    [SerializeField] private int soundRepetition;

    private bool canSeePlayer = false;
    private bool isAware = false;
    private float lookingTimer = 0;
    private float lostSightTimer = 0;
    private int owlIsDetectingPlayer = 0;
    private int owlJumpscareIndexScene = 5;
    private bool attackSoundPlayed = false;
    private bool ambiantSoundPlayed = false;

    //private float offsetmodif;
    public bool WalkPointSet { get => walkPointSet; set => walkPointSet = value; }
    public float DetectionFOV { get => detectionFOV; set => detectionFOV = value; }
    public float SoundDetectionRange { get => soundDetectionRange; set => soundDetectionRange = value; }
    public bool CanSeePlayer { get => canSeePlayer; set => canSeePlayer = value; }
    public Transform Player { get => player; set => player = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public float WalkPointRange { get => walkPointRange; set => walkPointRange = value; }
    public Vector3 PatrolPointOrigin { get => patrolPointOrigin; set => patrolPointOrigin = value; }
    public float ChaseRange { get => chaseRange; set => chaseRange = value; }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FOVRoutine());
        gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        originalAttackRange = attackRange;
        echoManager.Controller.Material.SetInt("_OwlActive", 0);
        echoManager.Controller.Material.SetFloat("_OwlRange", chaseRange + 7);

        timeToPlaySound = Random.Range(minSoundTime, maxSoundTime);

        ambiantSoundDelay = new List<List<float>>();
        attackSoundDelay = new List<List<float>>();

        ambiantSoundDelay.Add(ambiantSound1Delay);
        ambiantSoundDelay.Add(ambiantSound2Delay);
    }
    private void Update()
    {
        Vector3 currentPos = this.transform.position;
        lookingTimer += Time.deltaTime;
        Vector3 targetDir = walkPoint - currentPos;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDir, rotateHeadSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);

        if (owlIsDetectingPlayer == 1)
        {
            echoManager.Controller.Material.SetInt("_OwlActive", owlIsDetectingPlayer);
            Vector4 alertedOwlShaderVector = new Vector4(currentPos.x, currentPos.y, currentPos.z, (attackRange / (chaseRange + 1)));
            echoManager.Controller.Material.SetVector("_OwlPos", alertedOwlShaderVector);
        }

        lostSightTimer += Time.deltaTime;
        soundTimer += Time.deltaTime;

        if (lostSightTimer > 2)
        {            
            isAware = false;
        }

        if (timeToPlaySound < soundTimer && !ambiantSoundPlayed)
        {
            ambiantSoundPlayed = true;
            int rand = Random.Range(0, ambiantSoundList.Count);
            soundSourceAmbient.PlayOneShot(ambiantSoundList[rand]);
            StartCoroutine(FireAmbientSound(rand));
            timeToPlaySound = Random.Range(ambiantSoundList[rand].length + minSoundTime, ambiantSoundList[rand].length + maxSoundTime);
            soundTimer = 0;
        }
        else
            ambiantSoundPlayed = false;

        if (!attackSoundPlayed && owlIsDetectingPlayer > 0 && isAware)
        {
            int rand = Random.Range(0, attackSoundList.Count);
            soundSourceAttack.PlayOneShot(attackSoundList[rand]);
            attackSoundPlayed = true;
            soundTimer = 0;
        }
        else if(attackSoundPlayed && owlIsDetectingPlayer > 0 && !isAware)
            attackSoundPlayed = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerInChaseRange = Physics.CheckSphere(transform.position, ChaseRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, whatIsPlayer);

        if (playerInChaseRange && isAware && canSeePlayer)           
            ChasePlayer();
        
        else
            Patroling();

        if (playerInAttackRange)
            AttackPlayer();
    }

    private void Patroling()
    {
        animator.SetBool("TargetedPlayer", false);

        if (attackRange > originalAttackRange && !isAware)
        {
            owlIsDetectingPlayer = 0;
            echoManager.Controller.Material.SetInt("_OwlActive", owlIsDetectingPlayer);
            echoManager.Controller.Material.SetVector("_OwlPos", Vector4.zero);
        }

        if (!WalkPointSet)
            SearchNewWalkPoint();

        if (WalkPointSet && isAware)
            return;

        for (int i = 0; i < echoManager.EchoList.Count; ++i)
        {
            float dist = Vector3.Distance(this.transform.position, echoManager.EchoList[i]);
            if (dist < soundDetectionRange && echoManager.EchoPriority[i] == 0)
            {
                isAware = true;
                walkPoint = new Vector3(echoManager.EchoList[i].x, echoManager.EchoList[i].y, echoManager.EchoList[i].z);
                lookingTimer = 0;
                break;
            }
        }

        if (lookingTimer > DistractionThreshold)
        {
            WalkPointSet = false;
            lookingTimer = 0;
        }
    }

    private void SearchNewWalkPoint()
    {
        float randomX = Random.Range(-WalkPointRange, WalkPointRange);
        float randomZ = Random.Range(-WalkPointRange, WalkPointRange);

        walkPoint = new Vector3(this.transform.position.x + randomX, this.transform.position.y, this.transform.position.z + randomZ);

        WalkPointSet = true;
    }

    private void ChasePlayer()
    {
        animator.SetBool("TargetedPlayer", true);
        walkPoint = Player.position;
        lookingTimer = 0;
        WalkPointSet = true;
        attackRange += increaseOfRangeToKill * Time.deltaTime;
        owlIsDetectingPlayer = 1;
    }

    private void AttackPlayer()
    {
        gameMaster.PlayerIsDead(owlJumpscareIndexScene);
    }

    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            CheckConeRange();
        }
    }

    private void CheckConeRange()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, ChaseRange, whatIsPlayer);

        if (rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector3 dir = (target.position - transform.position).normalized;

            float angle = Vector3.Angle(transform.forward, dir);
            if (angle < DetectionFOV / 2)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                if (Physics.Raycast(transform.position, dir, dist, whatIsPlayer) && !Physics.Raycast(transform.position, dir, dist, whatIsGround))
                {
                    CanSeePlayer = true;
                    lostSightTimer = 0;
                }
                else
                    canSeePlayer = false;
            }
        }
    }

    private IEnumerator FireAmbientSound(int clipIndex)
    {
        float timePassed = 0;
        for (int i = 0; i < ambiantSoundDelay[clipIndex].Count; ++i)
        {
            yield return new WaitForSeconds(ambiantSoundDelay[clipIndex][i] - timePassed);
            shaderControls.Fire(this.transform.position, soundRange, soundTravelSpeed, int.MaxValue, soundRepetition);
            timePassed = ambiantSoundDelay[clipIndex][i];
        }
    }
}