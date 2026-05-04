using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new(0f, 7f, -10f);
    [SerializeField] Vector3 lookOffset = new(0f, 0f, 8f);

    [Header("Follow Speed")]
    [SerializeField] float zFollowSpeed = 6f;   // 전후 추적 (약간 느리게 → 캐릭터가 앞으로 달리는 느낌)
    [SerializeField] float xFollowSpeed = 4f;   // 좌우 추적 (더 느리게 → 레인 전환이 눈에 보임)
    [SerializeField] float yFollowSpeed = 8f;   // 상하 추적 (점프가 보이게)

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 currentPos = transform.position;

        // 축별로 다른 추적 속도 → 캐릭터의 움직임이 눈에 보임
        float newX = Mathf.Lerp(currentPos.x, desiredPosition.x, xFollowSpeed * Time.deltaTime);
        float newY = Mathf.Lerp(currentPos.y, desiredPosition.y, yFollowSpeed * Time.deltaTime);
        float newZ = Mathf.Lerp(currentPos.z, desiredPosition.z, zFollowSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, newY, newZ);
        transform.LookAt(target.position + lookOffset);
    }
}
