using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private float _followSpeed = 10.0f;
    [SerializeField] private float _clampAngle = 80.0f;
    [SerializeField] private float _inputSensitivity = 10.0f;

    private float _rotX = 0.0f;
    private float _rotY = 0.0f;

    private GameObject _target;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    private JoystickBehaviour _joystickBehaviour;
    private Vector3 _initialCamPointPos;

    void Start()
    {
        _target = SingleTons.GameController.Player.transform.parent.GetChild(1).gameObject;
        _playerMovementBehaviour = SingleTons.GameController.Player.GetComponent<PlayerMovementBehaviour>();
        transform.position = _target.transform.position;
        _joystickBehaviour = Camera.main.transform.GetChild(0).GetChild(1).GetComponent<JoystickBehaviour>();
    }

    void Update()
    {
        if (_playerMovementBehaviour.GetIsFollowing())
        {
            transform.LookAt(_target.transform.position + _playerMovementBehaviour.gameObject.transform.up + _playerMovementBehaviour.gameObject.transform.forward * 0.35f);
        }
        else
        {
            if (_joystickBehaviour.Vertical() != 0 || _joystickBehaviour.Horizontal() != 0)
            {
                _rotX += -_joystickBehaviour.Vertical() * _inputSensitivity * Time.deltaTime;
                _rotY += _joystickBehaviour.Horizontal() * _inputSensitivity * Time.deltaTime;

                float clamp = _clampAngle * Mathf.Abs(_joystickBehaviour.Vertical());
                _rotX = Mathf.Clamp(_rotX, -clamp, clamp);

                Quaternion localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_rotX, _rotY, 0), Time.deltaTime * 2);
                transform.rotation = localRotation;
            }

            if (_joystickBehaviour.Vertical() == 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime);
                _rotX = transform.rotation.eulerAngles.x;
                _rotX = (_rotX > 180) ? _rotX - 360 : _rotX;
            }

            if (_joystickBehaviour.Horizontal() == 0)
            {
                _rotY = transform.localRotation.eulerAngles.y;
            }
        }
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Slerp(transform.position, _target.transform.position, _followSpeed * Time.deltaTime);
    }
}
