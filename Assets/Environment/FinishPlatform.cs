using UnityEngine;

public class FinishPlatform : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject platform;
    [SerializeField] private AnimationCurve curve;
    private LevelScript levelHandler;
    private GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        levelHandler = GameObject.FindGameObjectWithTag("LM").GetComponent<LevelScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        arrow.GetComponent<SpriteRenderer>().enabled = false;
        platform.GetComponent<MeshRenderer>().enabled = false;
        platform.GetComponent<CapsuleCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (levelHandler.FinishAvailable)
        {
            arrow.GetComponent<SpriteRenderer>().enabled = true;
            platform.GetComponent<MeshRenderer>().enabled = true;
            platform.GetComponent<CapsuleCollider>().enabled = true;

            arrow.transform.LookAt(cam.transform.position);

            arrow.transform.position = new Vector3(transform.position.x, curve.Evaluate(Time.time), transform.position.z);
        }
    }
}
