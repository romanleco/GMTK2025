using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SideViewCC : MonoBehaviour
{
    private bool _dashing;
    [Header("SETTINGS")]
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canDash = true;
    private Vector2 _movementVector;
    private bool _facingRight;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    [SerializeField] private float _speed, _jumpforce, _gravity, _maxFallingSpeed, _dashingSpeed;
    private float _horizontal, _vertical;
    private WaitForSeconds _resetJumpTimer = new WaitForSeconds(0.1f);
    private WaitForSeconds _resetDashTimer = new WaitForSeconds(1.5f);
    private WaitForSeconds _dashTimer = new WaitForSeconds(0.15f);
    private RaycastHit2D[] _raycastHits = new RaycastHit2D[3];
    private bool _isGroundedFull;
    private Vector2 _rayOrigin;
    [Header("GROUND CHECK")]
    [SerializeField] private float _rayDistance;
    [SerializeField] private float _rayOffsetY;
    [SerializeField] private float _rayOffsetX;
    [SerializeField] private LayerMask _rayLayerMask;
    // private Animator _animator;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        if(_rb == null)
        {
            Debug.LogError("Player::Start() _rb Rigidbody is null");
        }

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if(_spriteRenderer == null)
        {
            Debug.LogError("Player::Start() _spriteRenderer SpriteRenderer is null");
        }

        // _animator = gameObject.GetComponent<Animator>();
        // if(_animator == null)
        // {
        //     Debug.LogError("Player::Start() _animator Animator is null");
        // }
    }

    void Update()
    {
        DebugGroundCheck();
        CalculateMovement();
        FacingDirection();
    }

    private void CalculateMovement()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        // if(Mathf.Abs(_horizontal) > 0.1f)
        // {
        //     _animator.SetBool("Walking", true);
        // }
        // else
        // {
        //     _animator.SetBool("Walking", false);
        // }

        if(Input.GetKeyDown(KeyCode.LeftShift) && !_dashing && _canDash)
        {
            StartCoroutine(DashRoutine());
        }

        if(!_dashing)
        {
            _movementVector.x = _horizontal * _speed;

            if(_isGroundedFull)
            {
                if(Input.GetKeyDown(KeyCode.Space) && _canJump)
                {
                    _movementVector.y = _jumpforce;
                    StartCoroutine(JumpReset());
                }
            }
            else
            {
                if(_movementVector.y > _maxFallingSpeed)
                {
                    _movementVector.y -= _gravity * Time.deltaTime;
                }
                else if(_movementVector.y != _maxFallingSpeed)
                {
                    _movementVector.y = _maxFallingSpeed;
                }
            }
        }

        _rb.linearVelocity = _movementVector;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.CompareTag("Ground"))
        {
            // Debug.Log("Ground Collision: " + Time.time);
            if(GroundCheck())
            {
                _movementVector.y = 0;
                _isGroundedFull = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if(other.transform.CompareTag("Ground"))
        {
            // Debug.Log("Collision Exit: " + Time.time);
            _isGroundedFull = false;
        }
    }

    private bool GroundCheck()
    {
        _rayOrigin = transform.position;
        _rayOrigin.y += _rayOffsetY;

        _rayOrigin.x -= _rayOffsetX;
        _raycastHits[0] = Physics2D.Raycast(_rayOrigin, Vector2.down, _rayDistance, _rayLayerMask);

        _rayOrigin.x += _rayOffsetX;
        _raycastHits[1] = Physics2D.Raycast(_rayOrigin, Vector2.down, _rayDistance, _rayLayerMask);

        _rayOrigin.x += _rayOffsetX;
        _raycastHits[2] = Physics2D.Raycast(_rayOrigin, Vector2.down, _rayDistance, _rayLayerMask);

        foreach(RaycastHit2D ray in _raycastHits)
        {
            if(ray.collider != null)
            {
                if(ray.collider.CompareTag("Ground"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DebugGroundCheck()
    {
        Vector2 tempOrigin = transform.position;
        tempOrigin.y += _rayOffsetY;

        tempOrigin.x -= _rayOffsetX;
        Debug.DrawRay(tempOrigin, Vector2.down * _rayDistance, Color.magenta);

        tempOrigin.x += _rayOffsetX;
        Debug.DrawRay(tempOrigin, Vector2.down * _rayDistance, Color.magenta);

        tempOrigin.x += _rayOffsetX;
        Debug.DrawRay(tempOrigin, Vector2.down * _rayDistance, Color.magenta);
    }

    private IEnumerator JumpReset()
    {
        _canJump = false;
        yield return _resetJumpTimer;
        _canJump = true;
    }

    private IEnumerator DashRoutine()
    {
        _movementVector.y = 0;
        _dashing = true;

        if(_facingRight)
        {
            _movementVector.x = _dashingSpeed;
        }
        else
        {
            _movementVector.x = -_dashingSpeed;
        }

        yield return _dashTimer;
        _dashing = false;

        _canDash = false;
        yield return _resetDashTimer;
        _canDash = true;
    }

    private void FacingDirection()
    {
        if(_horizontal > 0)
        {
            _facingRight = true;
            _spriteRenderer.flipX = false;
        }
        else if(_horizontal < 0)
        {
            _facingRight = false;
            _spriteRenderer.flipX = true;
        }
    }
}
