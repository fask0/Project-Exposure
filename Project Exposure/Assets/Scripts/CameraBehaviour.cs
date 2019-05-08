﻿using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private float _followSpeed = 10.0f;
    [SerializeField] private float _clampAngle = 80.0f;
    [SerializeField] private float _inputSensitivity = 10.0f;

    private float _rotX = 0.0f;
    private float _rotY = 0.0f;

    private GameObject _target;
    private Transform _playerTransform;

    private JoystickBehaviour _joystickBehaviour;

    void Start()
    {
        _target = GameObject.Find("CameraFollowPoint");
        _playerTransform = _target.transform.parent.transform;
        _joystickBehaviour = GameObject.Find("Joystick").GetComponent<JoystickBehaviour>();
    }

    void Update()
    {
        if (_joystickBehaviour.Vertical() != 0 || _joystickBehaviour.Horizontal() != 0)
        {
            _rotX += -_joystickBehaviour.Vertical() * _inputSensitivity * Time.deltaTime;
            _rotY += _joystickBehaviour.Horizontal() * _inputSensitivity * Time.deltaTime;

            _rotX = Mathf.Clamp(_rotX, -_clampAngle, _clampAngle);

            Quaternion localRotation = Quaternion.Euler(_rotX, _rotY, 0);
            transform.rotation = localRotation;
        }
        else if (_joystickBehaviour.GetTimeIdle() > 2)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _playerTransform.rotation, 2 * Time.deltaTime);
            _rotX = transform.rotation.eulerAngles.x;
            _rotY = transform.rotation.eulerAngles.y;
        }
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Slerp(transform.position, _target.transform.position, _followSpeed * Time.deltaTime);
    }
}
