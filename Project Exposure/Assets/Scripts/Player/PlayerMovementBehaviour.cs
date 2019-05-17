using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 5.0f;
    [SerializeField] private float _acceleration = 1.0f;

    private JoystickBehaviour _joystickBehaviour;
    private Rigidbody _rigidbody;

    private Vector3 _direction = Vector3.zero;

    private float _velocity;
    private float _waterResistance;

    void Start()
    {
        _waterResistance = _acceleration * 0.5f;
        _rigidbody = GetComponent<Rigidbody>();
        _joystickBehaviour = GameObject.Find("Joystick").GetComponent<JoystickBehaviour>();

        SingleTons.GameController.Player = this.gameObject;
    }

    void Update()
    {
        if (_joystickBehaviour.IsPressed())
        {
            if (_joystickBehaviour.GetTimeAtZero() >= 0.5f || _joystickBehaviour.Vertical() != 0)
            {
                _velocity += _acceleration * Time.deltaTime;
            }
            else
            {
                _velocity -= _waterResistance * Time.deltaTime;
                _rigidbody.velocity = Vector3.zero;
            }
        }
        else
        {
            _velocity -= _waterResistance * Time.deltaTime;
            _rigidbody.velocity = Vector3.zero;
        }

        _velocity = Mathf.Clamp(_velocity, 0, _maxSpeed);
        _direction = Camera.main.transform.forward;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(90 * ((_velocity / _maxSpeed) - _joystickBehaviour.Vertical()),
                                                                                   Camera.main.transform.parent.transform.rotation.eulerAngles.y + 90 * _joystickBehaviour.Horizontal(),
                                                                                   0), 2 * Time.deltaTime);
        transform.Translate(_direction * _velocity * Time.deltaTime, Space.World);
    }

    public float GetVelocity()
    {
        return _velocity;
    }
}
