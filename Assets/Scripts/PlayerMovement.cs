using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _direction;
    float _rotation;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _verticalSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 2.5f;
    private float _shiftingMultiplier = 2;
    private bool _isShifting;
    private bool _isRotatingLeft;
    private bool _isRotatingRight;

    [SerializeField] private float _maxHeight = 12;
    [SerializeField] private float _minHeight = 2;
    [SerializeField] private float _adjustedMapSize = 10.5f;
    void Update()
    {
        _direction.x = Input.GetAxis("Horizontal") * _speed;
        _direction.z = Input.GetAxis("Vertical") * _speed;
        _direction.y = Input.mouseScrollDelta.y * _verticalSpeed;

        _rotation = 0;

        if(Input.GetKeyDown(KeyCode.Q)) _isRotatingLeft = true;
        if(Input.GetKeyUp(KeyCode.Q)) _isRotatingLeft = false;
        if(_isRotatingLeft) _rotation -= 1;

        if(Input.GetKeyDown(KeyCode.E)) _isRotatingRight = true;
        if(Input.GetKeyUp(KeyCode.E)) _isRotatingRight = false;
        if(_isRotatingRight) _rotation += 1;

        if(Input.GetKeyDown(KeyCode.LeftShift)) _isShifting = true;
        if(Input.GetKeyUp(KeyCode.LeftShift)) _isShifting = false;
    }

    void FixedUpdate()
    {
        float shift = _isShifting ? _shiftingMultiplier : 1;
        transform.Translate(_direction * Time.fixedDeltaTime * shift, Space.Self);

        _rotation *= _speed;
        transform.Rotate(Vector3.up * _rotation * Time.fixedDeltaTime * _rotationSpeed, Space.World);
        
        ConfineMovement();
    }

    private void ConfineMovement()
    {
        float newX = Mathf.Clamp(transform.position.x, -_adjustedMapSize, _adjustedMapSize);
        float newY = Mathf.Clamp(transform.position.y, _minHeight, _maxHeight);
        float newZ = Mathf.Clamp(transform.position.z, -_adjustedMapSize, _adjustedMapSize);

        if (newX != transform.position.x || newY != transform.position.y || newZ != transform.position.z)
        {
            Vector3 assignmentVector = transform.position;
            assignmentVector.x = newX;
            assignmentVector.y = newY;
            assignmentVector.z = newZ;
            transform.position = assignmentVector;
        }
    }
}