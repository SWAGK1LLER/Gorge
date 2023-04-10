using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectibleBehaviour : MonoBehaviour
{
    [SerializeField] private float pickupRadius;
    [SerializeField] private float pickupHeight;
    [SerializeField] private GameObject player;
    private CapsuleCollider collider;

    public float PickupRadius { get => pickupRadius; set => pickupRadius = value; }
    public float PickupHeight { get => pickupHeight; set => pickupHeight = value; }

    // Start is called before the first frame update
    void Start()
    {
        collider = this.GetComponent<CapsuleCollider>();
        collider.radius = PickupRadius;
        collider.height = PickupHeight;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        Vector3 objPos = new Vector3(transform.position.x, 0, transform.position.z);
        Quaternion lookRotation = Quaternion.LookRotation((playerPos - objPos).normalized);
        this.transform.rotation = lookRotation;
    }
}
