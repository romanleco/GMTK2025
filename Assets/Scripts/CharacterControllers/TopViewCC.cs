using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopViewCC : MonoBehaviour
{
     //*Calculate Movement Variables
    [Header("VARIABLES")]
    [SerializeField] private float _speed;
    private float _horizontal;
    private float _vertical;
    private Vector2 _movementVector;
    private Rigidbody2D _rb;
    //*Face Mouse Direction Variables
    private Vector3 _mouseWorldPos;
    private Vector3 _mousePos;
    private Vector3 _faceDir;
    private float _angle;
    [Header("OPTIONS")]
    [SerializeField] private bool _sideFlip;
    [SerializeField] private bool _faceMouseDirection;
    [Header("ASSIGN")]
    [SerializeField] private Camera _pointerCamera;
    //*Animation Variables
    // [SerializeField] private Animator _animator;
    private SpriteRenderer _sprRenderer;

    void Start()
    {
        _rb = this.transform.GetComponent<Rigidbody2D>();
        if(_rb == null)
        {
            Debug.LogError("Rigidbody2D is NULL");
        }

        // if(_animator == null)
        // {
        //     Debug.LogError("Animator is NULL");
        // }

        _sprRenderer = this.transform.GetComponent<SpriteRenderer>();
        if(_sprRenderer == null)
        {
            Debug.Log("SpriteRenderer is NULL");
        }

        if(_pointerCamera == null && _faceMouseDirection)
            Debug.LogError("A Pointer Camera must be assigned for the face mouse direction to work");
    }

    private void FixedUpdate()
    {
        CalculateMovement();
        // AnimationHandling();

        if(_sideFlip)
            SideFlip();
            
        if(_faceMouseDirection)    
            FaceMouseDirection();
            
        // GameManager.Singleton.SetPlayerPosition(transform.position);
    }

    private void CalculateMovement()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        _movementVector.x = _horizontal;
        _movementVector.y = _vertical;

        _rb.linearVelocity = _movementVector * _speed;
    }

    // private void AnimationHandling()
    // {
    //     if(Mathf.Abs(_horizontal) > 0 || Mathf.Abs(_vertical) > 0)
    //     {
    //         _animator.SetBool("IsMoving", true);
    //     }
    //     else
    //     {
    //         _animator.SetBool("IsMoving", false);
    //     }
    // }

    private void SideFlip()
    {
        if(_horizontal > 0)
        {
            _sprRenderer.flipX = true;
        }

        if(_horizontal < 0)
        {
            _sprRenderer.flipX = false;
        }
    }

    public void UpdatePlayerSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    private void FaceMouseDirection()
    {
        _mousePos = Input.mousePosition;
        _mousePos.z = (_pointerCamera.transform.position.z * -1);

        _mouseWorldPos = _pointerCamera.ScreenToWorldPoint(_mousePos);
        _faceDir = (_mouseWorldPos - transform.position);
        _angle = Mathf.Atan2(_faceDir.y, _faceDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, _angle);

        _pointerCamera.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
