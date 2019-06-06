using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private float _followSpeed = 10.0f;
    [SerializeField] private float _clampAngle = 80.0f;
    [SerializeField] private float _inputSensitivity = 10.0f;
    [SerializeField] private float _menuFollowSpeed = 6.0f;

    private float _rotX = 0.0f;
    private float _rotY = 0.0f;

    private GameObject _originalTarget;
    private GameObject _target;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    private JoystickBehaviour _joystickBehaviour;
    private Vector3 _initialCamPointPos;
    private GameObject _dummyGO;
    private GameObject _artifact;
    private Quaternion _dummyRotation;
    private Vector2 _clamp = Vector2.zero;

    private float _currentFollowSpeed;
    private bool _isScanningArtifact;

    void Start()
    {
        _originalTarget = SingleTons.GameController.Player.transform.parent.GetChild(1).gameObject;
        _target = _originalTarget;
        _playerMovementBehaviour = SingleTons.GameController.Player.GetComponent<PlayerMovementBehaviour>();
        transform.position = _target.transform.position;
        _joystickBehaviour = Camera.main.transform.GetChild(0).GetChild(1).GetComponent<JoystickBehaviour>();
        _currentFollowSpeed = _followSpeed;
        _dummyGO = new GameObject();
    }

    void Update()
    {
        if (_target != _originalTarget)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _target.transform.rotation, Time.deltaTime * 4);
        }
        else
        {
            if (_playerMovementBehaviour.GetIsFollowing())
            {
                transform.LookAt(_target.transform.position + _playerMovementBehaviour.gameObject.transform.up + _playerMovementBehaviour.gameObject.transform.forward * 0.35f);
            }
            else
            {
                if (_isScanningArtifact)
                {
                    _dummyGO.transform.position = transform.position;
                    _dummyGO.transform.LookAt(_artifact.transform);
                    transform.rotation = Quaternion.Slerp(transform.rotation, _dummyGO.transform.rotation, Time.deltaTime);
                }
                else if (_joystickBehaviour.IsPressed())
                {
                    if (_joystickBehaviour.Vertical() != 0)
                        _rotX += -_joystickBehaviour.Vertical() * _inputSensitivity * Time.deltaTime;
                    if (_joystickBehaviour.Horizontal() != 0)
                        _rotY += _joystickBehaviour.Horizontal() * _inputSensitivity * Time.deltaTime * 1.5f;

                    _clamp = Vector2.Lerp(_clamp, new Vector2(_clampAngle * Mathf.Abs(_joystickBehaviour.Vertical()), 0), Time.deltaTime * 3);
                    _rotX = Mathf.Clamp(_rotX, -_clamp.x, _clamp.x);

                    Quaternion localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_rotX, _rotY, 0), Time.deltaTime * 2);
                    transform.rotation = localRotation;

                    _dummyRotation = transform.rotation;
                }

                if (_joystickBehaviour.Vertical() == 0)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(((_dummyRotation.eulerAngles.x > 180) ? _dummyRotation.eulerAngles.x - 360 : _dummyRotation.eulerAngles.x) * 0.66f, transform.eulerAngles.y, 0), Time.deltaTime);
                    _rotX = transform.rotation.eulerAngles.x;
                    _rotX = (_rotX > 180) ? _rotX - 360 : _rotX;
                }

                if (_joystickBehaviour.Horizontal() == 0)
                {
                    _rotY = transform.localRotation.eulerAngles.y;
                }
            }
        }
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Slerp(transform.position, _target.transform.position, _currentFollowSpeed * Time.deltaTime);
    }

    public void SetTemporaryTarget(GameObject gameObject)
    {
        _target = gameObject;
        _currentFollowSpeed = _menuFollowSpeed;
    }

    public void SetToOriginalTarget()
    {
        _target = _originalTarget;
        _currentFollowSpeed = _followSpeed;
    }

    public void StartScanningArtifact(GameObject pGameObject)
    {
        _isScanningArtifact = true;
        _artifact = pGameObject;
    }

    public void StopScanningArtifact()
    {
        _isScanningArtifact = false;
    }
}
