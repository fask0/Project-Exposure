﻿using UnityEngine;

public class SetFollowPoints : MonoBehaviour
{
    private Transform[] _followPointTransforms;
    private FollowPointIdentifier[] _followPoints;
    private Vector3 _colliderBounds;
    private Vector3 _playerColliderBounds;

    void Start()
    {
        _followPoints = GetComponentsInChildren<FollowPointIdentifier>();
        _followPointTransforms = new Transform[_followPoints.Length];
        _colliderBounds = GetComponent<MeshCollider>().bounds.extents;
        _playerColliderBounds = SingleTons.GameController.Player.GetComponent<CapsuleCollider>().bounds.extents;

        float radius = Mathf.Max(_colliderBounds.x, _colliderBounds.y) + _playerColliderBounds.x * 2;
        for (int i = 0; i < _followPointTransforms.Length; i++)
        {
            _followPointTransforms[i] = _followPoints[i].transform;
            float angle = (i * Mathf.PI * 2.0f) / _followPointTransforms.Length;
            _followPointTransforms[i].localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            _followPointTransforms[i].LookAt(transform, transform.forward);
        }
    }

    public Transform GetClosestPoint(Transform pTransform)
    {
        int closestElement = 0;
        float closest = float.MaxValue;
        for (int i = 0; i < _followPointTransforms.Length; i++)
        {
            float mag = 0.0f;
            mag = (_followPointTransforms[i].position - pTransform.position).magnitude;

            closest = Mathf.Min(closest, mag);

            if (closest == mag)
                closestElement = i;
        }

        return _followPointTransforms[closestElement];
    }
}
