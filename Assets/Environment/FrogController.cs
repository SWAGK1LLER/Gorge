using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FrogController : MonoBehaviour
{

    [SerializeField] private Animator anim;
    [SerializeField] private Animation animations;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ChangeShaderProperty controller;
    [SerializeField] private AudioSource croakSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Vector3 patrolPointOrigin;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float minTimeBeforeSound;
    [SerializeField] private float maxTimeBeforeSound;
    [SerializeField] private float soundRange;
    [SerializeField] private float soundTravelSpeed;
    [SerializeField] private int soundRepetition;
    [SerializeField] private int soundPriority;
    [SerializeField] private float jumpSoundRange;
    [SerializeField] private float jumpSoundTravelSpeed;
    [SerializeField] private int jumpSoundRepetition;
    [SerializeField] private int jumpSoundPriority;
    private Vector3 walkPoint;
    private float currentSoundTime;
    private float soundTimer;
    private bool walkPointSet;
    private bool standingStill;
    private bool readyToSoundJump;
    private float idleAnimationTime;
    private float jumpAnimationTime;
    private float normalSpeed;
    private bool doSound = false;

    public Vector3 PatrolPointOrigin { get => patrolPointOrigin; set => patrolPointOrigin = value; }
    public float WalkPointRange { get => walkPointRange; set => walkPointRange = value; }
    public Vector3 WalkPoint { get => walkPoint; set => walkPoint = value; }
    public NavMeshAgent Agent { get => agent; set => agent = value; }

    private void Awake()
    {
        idleAnimationTime = animations.clip.length;
        jumpAnimationTime = anim.GetCurrentAnimatorClipInfo(0).Length;
        normalSpeed = Agent.speed;
    }

    private void Update()
    {
        Patroling();
        soundTimer += Time.deltaTime;
        if (soundTimer > currentSoundTime)
        {
            croakSound.Play();
            currentSoundTime = Random.Range(minTimeBeforeSound, maxTimeBeforeSound);
            controller.Fire(this.transform.position, soundRange, soundTravelSpeed, soundPriority, soundRepetition);
            soundTimer = 0;
            doSound = false;
        }
        
        if (croakSound.time > 1 && croakSound.time < 1.1f && !doSound)
        {
            controller.Fire(this.transform.position, soundRange, soundTravelSpeed, soundPriority, soundRepetition);
            doSound = true;
        }
        if(croakSound.time > 1.9f && croakSound.time < 2 && doSound)
        {
            controller.Fire(this.transform.position, soundRange, soundTravelSpeed, soundPriority, soundRepetition);
            doSound = false;
        }
        if (croakSound.time > 2.7f && croakSound.time < 2.8f && !doSound)
        {
            controller.Fire(this.transform.position, soundRange, soundTravelSpeed, soundPriority, soundRepetition);
            doSound = true;
        }
        if (croakSound.time > 3.3f && croakSound.time < 3.4f && doSound)
        {
            controller.Fire(this.transform.position, soundRange, soundTravelSpeed, soundPriority, soundRepetition);
            doSound = false;
        }
        if (croakSound.time > 3.9f && croakSound.time < 4 && !doSound)
        {
            controller.Fire(this.transform.position, soundRange, soundTravelSpeed, soundPriority, soundRepetition);
            doSound = true;
        }
    }

    public void Idle()
    {
        RootMotion();
        anim.SetTrigger("Idle");
    }

    public void Jump()
    {
        RootMotion();
        anim.SetTrigger("Jump");
    }

    public void Swim()
    {
        RootMotion();
        anim.SetTrigger("Swim");
    }

    public void TurnLeft()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("TurnLeft");
    }

    public void TurnRight()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("TurnRight");
    }

    void RootMotion()
    {
        if (anim.applyRootMotion)
        {
            anim.applyRootMotion = false;
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchNewWalkPoint();

        Agent.SetDestination(WalkPoint);

        Vector3 distanceToWalkPoint = transform.position - WalkPoint;

        if (distanceToWalkPoint.magnitude < 0.5f && !standingStill)
            StartCoroutine(InIdle());

        if (!standingStill)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < jumpAnimationTime * (3.0f / 4.0f))
            {
                Agent.speed = normalSpeed;
                readyToSoundJump = true;
            }
            else
            {
                if(readyToSoundJump)
                {
                    readyToSoundJump = false;
                    jumpSound.Play();
                    controller.Fire(this.transform.position, jumpSoundRange, jumpSoundTravelSpeed, jumpSoundPriority, jumpSoundRepetition);
                }
                Agent.speed = 0;
            }

            Jump();
        }
        else
            Idle();
    }

    private void SearchNewWalkPoint()
    {
        RaycastHit hit;
        float randomX = Random.Range(-WalkPointRange, WalkPointRange);
        float randomZ = Random.Range(-WalkPointRange, WalkPointRange);

        WalkPoint = new Vector3(PatrolPointOrigin.x + randomX, PatrolPointOrigin.y, PatrolPointOrigin.z + randomZ);

        if (Physics.Raycast(WalkPoint + new Vector3(0,1,0), -transform.up, out hit, 2, whatIsGround))
        {
            WalkPoint += new Vector3(0, hit.point.y, 0);
            Vector3 newPoint;
            if (SetDestination(PatrolPointOrigin, walkPointRange, out newPoint))
            {
                WalkPoint = newPoint;
                walkPointSet = true;
                return;
            }
        }
        
        SearchNewWalkPoint();
    }

    private IEnumerator InIdle()
    {
        standingStill = true;
        yield return new WaitForSeconds(idleAnimationTime);
        walkPointSet = false;
        standingStill = false;
    }

    private bool SetDestination(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}