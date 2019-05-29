using System;
using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 5.0f;
    [SerializeField] private float _acceleration = 1.0f;

    private Animator _animator;

    private JoystickBehaviour _joystickBehaviour;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private Vector3 _direction = Vector3.zero;

    private float _velocity;
    private float _waterResistance;

    //Stunns
    private bool _isStunned;
    private DateTime _stopStunTime;

    //Following
    private bool _isFollowing;
    private GameObject _followTarget;
    private Transform _followPoint;

    void Start()
    {
        _waterResistance = _acceleration * 0.5f;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();
        _joystickBehaviour = Camera.main.transform.GetChild(0).GetChild(1).GetComponent<JoystickBehaviour>();

        SingleTons.GameController.Player = this.gameObject;
    }

    void Update()
    {
        if (_isStunned)
        {
            if (DateTime.Now >= _stopStunTime)
            {
                _isStunned = false;
            }
        }
        else
        {
            if (_isFollowing)
            {
                transform.position = Vector3.Slerp(transform.position, _followPoint.position, Time.deltaTime * 3);
                transform.rotation = Quaternion.Slerp(transform.rotation, _followPoint.rotation, Time.deltaTime * 3);
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsSwimming", true);

                if (_joystickBehaviour.IsPressed())
                {
                    _followTarget = null;
                    _isFollowing = false;
                }
            }
            else
            {
                //Rotation and movement
                if (_joystickBehaviour.IsPressed())
                {
                    if (_joystickBehaviour.GetTimeAtZero() >= 0.5f || _joystickBehaviour.Vertical() != 0)
                    {
                        _velocity += _acceleration * Time.deltaTime;
                        _animator.SetBool("IsIdle", false);
                        _animator.SetBool("IsSwimming", true);
                    }
                    else
                    {
                        _velocity -= _waterResistance * Time.deltaTime;
                        _rigidbody.velocity = Vector3.zero;

                        if (_velocity < 2)
                        {
                            _animator.SetBool("IsIdle", true);
                            _animator.SetBool("IsSwimming", false);
                        }
                    }
                }
                else
                {
                    _velocity -= _waterResistance * Time.deltaTime;
                    _rigidbody.velocity = Vector3.zero;
                    _animator.SetBool("IsIdle", true);
                    _animator.SetBool("IsSwimming", false);
                }

                _direction = Camera.main.transform.forward;
                _velocity = Mathf.Clamp(_velocity, 0, _maxSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(90 * ((_velocity / _maxSpeed) - _joystickBehaviour.Vertical()),
                                                                                           Camera.main.transform.parent.transform.rotation.eulerAngles.y + 90 * _joystickBehaviour.Horizontal(),
                                                                                           0), 2 * Time.deltaTime);

                transform.Translate(_direction * _velocity * Time.deltaTime, Space.World);
            }
        }
    }

    public void StartFollowingGameObject(GameObject pGameObject)
    {
        if (_followTarget == pGameObject) return;

        _isFollowing = true;
        _followTarget = pGameObject;
        _followPoint = pGameObject.GetComponent<SetFollowPoints>().GetClosestPoint(transform);
    }

    public float GetVelocity()
    {
        return _velocity;
    }

    public void StunPlayer(int stunTimeInMs)
    {
        _isStunned = true;
        _stopStunTime = DateTime.Now.AddMilliseconds(stunTimeInMs);
    }

    public bool CheckIfFollowingGameObject(GameObject pGameObject)
    {
        if (pGameObject == _followTarget)
            return true;

        return false;
    }

    public GameObject GetFollowTarget()
    {
        return _followTarget;
    }

    public bool GetIsFollowing()
    {
        return _isFollowing;
    }
}
