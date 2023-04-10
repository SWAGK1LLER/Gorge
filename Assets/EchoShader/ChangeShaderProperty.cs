using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeShaderProperty : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Material water;
    [SerializeField] private GameObject player;
    private Vector4[] points;
    private float[] echoRange;
    private int nbIndex;
    public delegate void EventHandler(Vector4 position, float echoRange, float echoTravelSpeed, int priority);
    public event EventHandler shootEcho = delegate { };

    public Material Material { get => material; set => material = value; }
    public GameObject Player { get => player; set => player = value; }
    public Vector4[] Points { get => points; set => points = value; }
    public float[] EchoRange { get => echoRange; set => echoRange = value; }
    public int NbIndex { get => nbIndex; set => nbIndex = value; }


    // Start is called before the first frame update
    void Start()
    {        
        Points = new Vector4[750];
        EchoRange = new float[750];
        nbIndex = Points.Length;
        Material.SetInt("_PointsSize", nbIndex);
        Material.SetVectorArray("_Points", Points);
        Material.SetFloatArray("_EchoRange", EchoRange);
        if (water != null)
        {
            water.SetInt("_PointsSize", nbIndex);
            water.SetVectorArray("_Points", Points);
            water.SetFloatArray("_EchoRange", EchoRange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Material.SetInt("_PointsSize", nbIndex);
        Material.SetVectorArray("_Points", Points);
        Material.SetFloatArray("_EchoRange", EchoRange);
        if (water != null)
        {
            water.SetInt("_PointsSize", nbIndex);
            water.SetVectorArray("_Points", Points);
            water.SetFloatArray("_EchoRange", EchoRange);
        }
    }

    public void Fire(Vector4 position, float echoRange, float echoTravelSpeed, int priority, int nbEcho)
    {
        StartCoroutine(additionEffect(position, echoRange, echoTravelSpeed, priority, nbEcho));
    }

    private IEnumerator additionEffect(Vector4 position,float echoRange, float echoTravelSpeed, int priority, int nbEcho)
    {
        for (int i = 0; i < nbEcho; ++i)
        {
            shootEcho.Invoke(position, echoRange, echoTravelSpeed, priority);

            yield return new WaitForSeconds(0.2f * (echoRange / echoTravelSpeed));
        }
    }
}
