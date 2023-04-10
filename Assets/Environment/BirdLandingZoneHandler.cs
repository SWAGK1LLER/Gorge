using System.Collections.Generic;
using UnityEngine;

public class BirdLandingZoneHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> branches;

    public List<Transform> Branches { get => branches; set => branches = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform getRandomBranchToLand()
    {
        return branches[Random.Range(0, branches.Count)];
    }
}
