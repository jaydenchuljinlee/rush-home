using UnityEngine;

/// <summary>
/// 러너 카메라의 X/Z 프레이밍은 유지하고, 플레이어의 세로 이동만 부드럽게 추적한다.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float verticalSmoothTime = 0.2f;
    [SerializeField] private bool followVerticalOnly = true;
    [SerializeField] private bool clampToInitialY = true;

    private Vector3 _initialPosition;
    private Vector3 _offset;
    private float _verticalVelocity;

    public Transform Target => target;

    private void Awake()
    {
        _initialPosition = transform.position;

        if (target != null)
        {
            _offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + _offset;
        Vector3 nextPosition = transform.position;

        if (!followVerticalOnly)
        {
            nextPosition.x = desiredPosition.x;
        }

        float targetY = clampToInitialY
            ? Mathf.Max(_initialPosition.y, desiredPosition.y)
            : desiredPosition.y;

        nextPosition.y = Mathf.SmoothDamp(
            transform.position.y,
            targetY,
            ref _verticalVelocity,
            verticalSmoothTime
        );
        nextPosition.z = _initialPosition.z;

        transform.position = nextPosition;
    }

    public void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + _offset;
        transform.position = new Vector3(
            followVerticalOnly ? _initialPosition.x : desiredPosition.x,
            clampToInitialY ? Mathf.Max(_initialPosition.y, desiredPosition.y) : desiredPosition.y,
            _initialPosition.z
        );
    }
}
