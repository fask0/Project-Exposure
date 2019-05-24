using UnityEngine;

public class SetFollowPoints : MonoBehaviour
{
    private Transform[] _followPointTransforms;
    private Vector3 _colliderBounds;
    private Vector3 _playerColliderBounds;

    void Start()
    {
        _followPointTransforms = GetComponentsInChildren<Transform>();
        _colliderBounds = GetComponent<MeshCollider>().bounds.extents;
        _playerColliderBounds = SingleTons.GameController.Player.GetComponent<CapsuleCollider>().bounds.extents;
        _followPointTransforms[1].localPosition = new Vector3(-_colliderBounds.x * 3 - _playerColliderBounds.x * 2, 0, 0);
        _followPointTransforms[1].localRotation = Quaternion.Euler(0, 90, 90);
        _followPointTransforms[2].localPosition = new Vector3(_colliderBounds.x * 3 + _playerColliderBounds.x * 2, 0, 0);
        _followPointTransforms[2].localRotation = Quaternion.Euler(0, 270, 270);
        _followPointTransforms[3].localPosition = new Vector3(0, _colliderBounds.y * 3 + _playerColliderBounds.x * 2, 0);
        _followPointTransforms[3].localRotation = Quaternion.Euler(90, 0, 0);
        _followPointTransforms[4].localPosition = new Vector3(0, -_colliderBounds.y * 3 - _playerColliderBounds.x * 2, 0);
        _followPointTransforms[4].localRotation = Quaternion.Euler(270, 180, 0);
    }

    public Transform GetClosestPoint(Transform pTransform)
    {
        int closestElement = 0;
        float closest = 9999.0f;
        for (int i = 1; i < _followPointTransforms.Length; i++)
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
