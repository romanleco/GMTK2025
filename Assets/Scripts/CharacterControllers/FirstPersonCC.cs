using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonCC : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _movementVector;
    private bool _isGrounded;
    [Header("PHYSICS")]
    [SerializeField] private float _speed;
    [SerializeField] private float _gravity;
    [SerializeField] private float _maxFallingSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _canRun;
    [SerializeField] private float _runningSpeedMultiplier;
    private bool _running;
    [Header("CAMERA")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _horizontalSensitivity;
    [SerializeField] private float _verticalSensitivity;
    [SerializeField] private float _minVerticalRot;
    [SerializeField] private float _maxVerticalRot;
    private float _horizontalRot;
    private float _verticalRot;
    private Vector2 _cameraRotation;
    [Header("GROUND DETECTION")]
    [SerializeField] private float _rayOffsetCenter;
    [SerializeField] private float _rayOffsetY;
    [SerializeField] private float _rayDistance;
    private Vector3[] _rayOrigins = new Vector3[5];
    private RaycastHit[] _raycastHits = new RaycastHit[5];
    private Ray[] _rays = new Ray[5];
    private List<int> _currentCollisionInstanceIds = new List<int>();
    [SerializeField] private bool _isGroundedRay;
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        if(_rb == null)
        {
            Debug.LogError("Rigidbody is NULL");
        }
    }
    void Update()
    {
        _isGroundedRay = GroundDetection();
        DebugRaycasts();
        CalculateMovement();
        LookRotation();
        // Debug.Log("Movement Vector: " + _movementVector + " | Grounded: " + _isGrounded);
    }

    private void CalculateMovement()
    {
        _movementVector.x = Input.GetAxis("Horizontal") * _speed * Time.deltaTime * 100;
        _movementVector.z = Input.GetAxis("Vertical") * _speed * Time.deltaTime * 100;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _running = true;
        }
        
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _running = false;
        }

        if(_isGrounded && _isGroundedRay)
        {
            if(Input.GetKeyDown(KeyCode.Space) && _isGrounded)
            {
                _movementVector.y = _jumpForce;
            }

            if(_running && _canRun)
            {
                _movementVector.x *= _runningSpeedMultiplier;
                _movementVector.z *= _runningSpeedMultiplier;
            }
        }
        else
        {
            if(_movementVector.y <= _maxFallingSpeed)
            {
                _movementVector.y = _maxFallingSpeed;
            }
            else
            {
                _movementVector.y -= _gravity * Time.deltaTime;
            }
        }

        _rb.linearVelocity = transform.TransformDirection(_movementVector);
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.transform.CompareTag("Ground"))
        {
            _currentCollisionInstanceIds.Add(other.transform.GetInstanceID());
        }

        if(_currentCollisionInstanceIds.Count > 0)
        {
            _movementVector.y = 0;
            _isGrounded = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if(other.transform.CompareTag("Ground"))
        {
            int collisionInstanceId = other.transform.GetInstanceID();

            if(_currentCollisionInstanceIds.Count > 0)
            {
                if(_currentCollisionInstanceIds.Contains(collisionInstanceId))
                {
                    _currentCollisionInstanceIds.Remove(collisionInstanceId);
                }
            }
        }

        if(_currentCollisionInstanceIds.Count <= 0)
        {
            _isGrounded = false;
        }
    }

    private void LookRotation()
    {
        _horizontalRot = Input.GetAxis("Mouse X") * _horizontalSensitivity * Time.deltaTime * 100;
        _verticalRot -= Input.GetAxis("Mouse Y") * _verticalSensitivity * Time.deltaTime * 100;

        transform.Rotate(0, _horizontalRot, 0);

        _verticalRot = Mathf.Clamp(_verticalRot, _minVerticalRot, _maxVerticalRot);

        _cameraRotation.x = _verticalRot;
        _cameraRotation.y += _horizontalRot;

        _playerCamera.transform.eulerAngles = _cameraRotation;
    }

    private bool GroundDetection()
    {
        for(int i = 0; i < _rayOrigins.Length; i++)
        {
            _rayOrigins[i] = transform.position;
            _rayOrigins[i].y += _rayOffsetY;
        }

        _rayOrigins[1].x += _rayOffsetCenter;
        _rayOrigins[2].x -= _rayOffsetCenter;
        _rayOrigins[3].z += _rayOffsetCenter;
        _rayOrigins[4].z -= _rayOffsetCenter;

        for(int i = 0; i < _rays.Length; i++)
        {
            _rays[i].origin = _rayOrigins[i];
            _rays[i].direction = Vector3.down;

            if(Physics.Raycast(_rays[i], out _raycastHits[i]))
            {
                if(_raycastHits[i].transform.CompareTag("Ground") && _raycastHits[i].distance < _rayDistance)
                {
                    Debug.Log("Distance: " + ((_raycastHits[i].collider.transform.position.y - _rayOrigins[i].y) * -1).ToString());
                    return true;
                }
            }
        }

        Debug.Log("Ground Detection: false | Time: " + Time.time);
        return false;
    }

    private void DebugRaycasts()
    {
        Vector3[] tempRayOrigins = new Vector3[5];

        for(int i = 0; i < tempRayOrigins.Length; i++)
        {
            tempRayOrigins[i] = transform.position;
            tempRayOrigins[i].y += _rayOffsetY;
        }

        tempRayOrigins[1].x += _rayOffsetCenter;
        tempRayOrigins[2].x -= _rayOffsetCenter;
        tempRayOrigins[3].z += _rayOffsetCenter;
        tempRayOrigins[4].z -= _rayOffsetCenter;

        for(int i = 0; i < _rayOrigins.Length; i++)
        {
            Debug.DrawRay(tempRayOrigins[i], Vector2.down * _rayDistance, Color.magenta);
        }
    }
}
