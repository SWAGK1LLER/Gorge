using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character {
  public class SnakeTest : MonoBehaviour {

    //lets store some info on every segment, so we could access it later
    public class SnakeSeg {
      public GameObject ObjRef;
      public GameObject ParentToWatch;
      public int Id= 0;
      public float CurveOffset = 0.0f;
    }

    public List<GameObject> JointsList = new List<GameObject>();
    private readonly List<SnakeSeg> _segmentsList = new List<SnakeSeg>();
    
    //you can move it with your input, or script some automovement if you want
    public GameObject HeadTarget;

    //how often new node should be spawned
    public float SpawnNodeDistance = 0.01f;

    //offset, if your model is a bit undergound you can tweak it this way
    public float MeshOffset = 0.06f;

    //we will store our node position here, and then evaluate with _curveGlobalOffset var
    public AnimationCurve XAxisCurve = new AnimationCurve();
    public AnimationCurve YAxisCurve = new AnimationCurve();
    public AnimationCurve ZAxisCurve = new AnimationCurve();
    
    //those are for debug purposes
    public GameObject DebugNode;
    public bool DebugMode = false;
    private List<GameObject> _debugNodes = new List<GameObject>();
    private Camera _raycastCamera; 

    //temp vars to calcule offset
    private float _prevAngle = 0.0f;
    private float _curveGlobalOffset = 0.0f;
    private Vector3 _prevNodePosition;

    private float _startDist;
    private float _lastCreateTime;

    void Start() {
      int cnt = 1;
      foreach (var joint in JointsList) {
        SnakeSeg seg = new SnakeSeg();
        seg.ObjRef = joint;
        seg.Id = cnt;
        _segmentsList.Add(seg);
        cnt++;
      }

      cnt = 0;
      foreach (var segment in _segmentsList) {
        if (cnt < _segmentsList.Count-1) {
          segment.ParentToWatch = _segmentsList[cnt + 1].ObjRef;
        }
        segment.CurveOffset = (_segmentsList[0].ObjRef.transform.position - segment.ObjRef.transform.position).magnitude;       
        CreateNode(segment.ObjRef.transform.position);
        cnt++;
      }
      _startDist = (HeadTarget.transform.position - _segmentsList[0].ObjRef.transform.position).magnitude;

      _raycastCamera = Camera.main;;
    }

    void UpdateCurve(AnimationCurve curve, float time, float val) {
      //consider it as a pool, to optimize things we freeing and adding a new key
      if (curve.length < _segmentsList.Count *2)
        curve.AddKey(time, val);
      else{
        curve.RemoveKey(0);
        curve.AddKey(time, val);
      }
    }

    void CreateNode(Vector3 position) {
      if (XAxisCurve.length > 0) {
        float lastKeyTime = 0.0f;
        if (XAxisCurve.keys.Length > 0)
          lastKeyTime = XAxisCurve.keys[XAxisCurve.keys.Length - 1].time;

        float time = lastKeyTime + (position - _prevNodePosition).magnitude;
        _lastCreateTime = time;

        UpdateCurve(XAxisCurve, time, position.x);
        UpdateCurve(YAxisCurve, time, position.y);
        UpdateCurve(ZAxisCurve, time, position.z);

        _prevAngle = CalculateAngle(HeadTarget.transform.position, _prevNodePosition);
      }
      else {
        XAxisCurve.AddKey(0, position.x);
        YAxisCurve.AddKey(0, position.y);
        ZAxisCurve.AddKey(0, position.z);
      }

      _prevNodePosition = position;

      if (DebugMode) {
        if (_debugNodes.Count > _segmentsList.Count * 2) {
          GameObject.Destroy(_debugNodes[0]);
          _debugNodes.RemoveAt(0);
        }

        GameObject debugNode = GameObject.Instantiate(DebugNode);
        debugNode.SetActive(true);
        _debugNodes.Add(debugNode);
        debugNode.transform.position = position;
        debugNode.transform.parent = DebugNode.transform.parent;
      }
    }

    void Update() {
      UpdateInput();

      bool createNode = false;
      float dist = (HeadTarget.transform.position - _prevNodePosition).magnitude;

      if (dist > SpawnNodeDistance)
        createNode = true;
	
	 //you can use this line to smooth positions on a hard edges
     /* else if (dist > 0.01f && Mathf.Abs(_prevAngle - CalculateAngle(HeadTarget.transform.position, _prevNodePosition)) > 55.0f) {
        createNode = true;
      }*/

      if (createNode)
        CreateNode(HeadTarget.transform.position);

      if (_lastCreateTime > _curveGlobalOffset + _startDist) {
        _curveGlobalOffset = Mathf.Lerp(_curveGlobalOffset, _lastCreateTime, (_lastCreateTime - (_curveGlobalOffset + _startDist))*Time.deltaTime );

        foreach (var segment in _segmentsList) {
          float evalTime = _curveGlobalOffset + segment.CurveOffset;
          Vector3 finalVec = new Vector3(XAxisCurve.Evaluate(evalTime), YAxisCurve.Evaluate(evalTime),ZAxisCurve.Evaluate(evalTime));

          if (segment.Id < _segmentsList.Count)
            FaceObject(segment.ObjRef.transform, segment.ParentToWatch.transform.position);
          segment.ObjRef.transform.position = finalVec;
        }
      }

      FaceObject(_segmentsList[_segmentsList.Count - 1].ObjRef.transform, HeadTarget.transform.position);
    }

    public void UpdateInput() {
      if (Input.GetMouseButton(0)){
        RaycastHit hit;
        Ray ray = _raycastCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
          HeadTarget.transform.position = hit.point + hit.normal* MeshOffset;
        }
      }
    }

    private float CalculateAngle(Vector3 from, Vector3 to){
      return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.x;
    }

    private static void FaceObject(Transform srcObject, Vector3 targetPos) {
      srcObject.rotation = Quaternion.LookRotation(srcObject.position - targetPos);
            //srcObject.eulerAngles += new Vector3(-90, 0, 0);
    }

  }
}

