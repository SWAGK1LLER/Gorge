using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EcholocationManager : MonoBehaviour
{
    private float timer;
    private List<Vector4> echoList;
    private List<float> echoRange;
    private List<float> echoTravelSpeed;
    private List<int> echoPriority;
    [SerializeField] private ChangeShaderProperty controller;
    [SerializeField] private float echoWidth;

    public List<Vector4> EchoList { get => echoList; set => echoList = value; }
    public List<float> EchoRange { get => echoRange; set => echoRange = value; }
    public List<float> EchoTravelSpeed { get => echoTravelSpeed; set => echoTravelSpeed = value; }
    public List<int> EchoPriority { get => echoPriority; set => echoPriority = value; }
    public ChangeShaderProperty Controller { get => controller; set => controller = value; }

    // Start is called before the first frame update
    void Awake()
    {
        EchoList = new List<Vector4>();
        EchoRange = new List<float>();
        EchoTravelSpeed = new List<float>();
        EchoPriority = new List<int>();
        Controller.Material.SetFloat("_EchoWidth", echoWidth);
        Controller.shootEcho += spawnEcho;
        EchoList.Add(new Vector4());
        EchoRange.Add(0);
        EchoTravelSpeed.Add(0);
        EchoPriority.Add(int.MaxValue);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = EchoList.Count - 1; i > 0; --i)
        {
            EchoList[i] = new Vector4(EchoList[i].x, EchoList[i].y, EchoList[i].z, EchoList[i].w + ((EchoTravelSpeed[i] * Time.deltaTime) / EchoRange[i]));

            if (EchoList[i].w >= 1)
            {
                try
                {
                    EchoList.RemoveAt(i);
                    EchoRange.RemoveAt(i);
                    EchoTravelSpeed.RemoveAt(i);
                    echoPriority.RemoveAt(i);
                }
                catch(ArgumentOutOfRangeException)
                {
                    Debug.Log(i);
                }
                
            }
        }

        for(int i = 0; i < echoList.Count; ++i)
        {
            Controller.Points[i] = echoList[i];
            Controller.EchoRange[i] = EchoRange[i];
        }
        Controller.NbIndex = echoList.Count;
    }

    private void spawnEcho(Vector4 position, float echoRange, float echoTravelSpeed, int priority)
    {
        EchoList.Add(position);
        this.EchoRange.Add(echoRange);
        this.EchoTravelSpeed.Add(echoTravelSpeed);
        this.EchoPriority.Add(priority);
    }
}
