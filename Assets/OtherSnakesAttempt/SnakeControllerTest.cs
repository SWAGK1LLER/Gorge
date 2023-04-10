using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeControllerTest : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private List<Transform> _bodyParts;
    [SerializeField] private float _distanceBetwenParts = 1f;
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float _turningSpeed = 90f;
    [SerializeField] private float _sMouvSpeed;
    [SerializeField] private float sTime;

    private List<Vector3> _positions = new List<Vector3>();

    private void Start()
    {
        for (int i = 0; i < _bodyParts.Count; i++)
        {
            _positions.Add(_bodyParts[i].position);
        }
    }

    private void Update()
    {
        HandleInput();
        transform.Rotate(Vector3.up, (curve.Evaluate(Time.time * _sMouvSpeed) * sTime) * _turningSpeed * Time.deltaTime);
        HandleSnakeMovement();
    }

    private void HandleInput()
    {
        //transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * _turningSpeed * Time.deltaTime);
    }

    private void HandleSnakeMovement()
    {
        _bodyParts[0].Translate(_bodyParts[0].forward * _movementSpeed * Time.deltaTime, Space.World);
        float distance = Vector3.Distance(_bodyParts[0].position, _positions[0]);

        if (distance > _distanceBetwenParts)
        {
            Vector3 direction = (_bodyParts[0].position - _positions[0]).normalized;
            Vector3 newPos = _positions[0] + direction * _distanceBetwenParts;
            _positions.Insert(0, newPos);
            _positions.RemoveAt(_positions.Count - 1);
            distance = Vector3.Distance(_bodyParts[0].position, _positions[0]);
        }

        float lerpProgress = distance / _distanceBetwenParts;

        for (int i = 1; i < _bodyParts.Count; i++)
        {
            _bodyParts[i].position = Vector3.Lerp(_positions[i], _positions[i - 1], lerpProgress);
            var dir = (_bodyParts[i - 1].position - _bodyParts[i].position).normalized;
            _bodyParts[i].rotation = Quaternion.LookRotation(dir);
        }
    }
}