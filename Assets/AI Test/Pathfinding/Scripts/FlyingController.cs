using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingController : MonoBehaviour
{
    private AStarAgent agent;
    private Transform moveToPoint;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] float speed;

    private bool goToPoint = false;

    public bool GoToPoint { get => goToPoint; set => goToPoint = value; }
    public Transform MoveToPoint { get => moveToPoint; set => moveToPoint = value; }
    public AStarAgent Agent { get => agent; set => agent = value; }

    private void Start()
    {
        Agent = GetComponent<AStarAgent>();
        StartCoroutine(Coroutine_MoveRandom());
        //StartCoroutine(Coroutineanimation());
    }

    public IEnumerator Coroutine_MoveRandom()
    {
        List<Point> freePoints = WorldManager.Instance.GetFreePoints();
        /*Point start = freePoints[Random.Range(0, freePoints.Count)];
        transform.position = start.WorldPosition;*/
        while (true)
        {
            Point p = freePoints[Random.Range(0, freePoints.Count)];

            if (GoToPoint)
                Agent.Pathfinding(MoveToPoint.position);
            else
                Agent.Pathfinding(p.WorldPosition);

            yield return new WaitUntil(() => Agent.Status == AStarAgent.AStarAgentStatus.Finished);
        }
    }
}
