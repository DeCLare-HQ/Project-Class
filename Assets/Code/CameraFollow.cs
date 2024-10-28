using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform
    public float smoothSpeed = 0.125f;  // 카메라 부드러운 이동 속도
    public Vector3 offset;  // 카메라와 플레이어 간의 오프셋
    public float fixedZPosition = -10f;  // 고정할 Z축 값

    // 카메라 이동 범위
    public Vector2 minBounds;  // 최소 X, Y
    public Vector2 maxBounds;  // 최대 X, Y

    // 플레이어 찾기 (씬 전환 후에도 적용)
    void Start()
    {
        if (player == null)
        {
            FindPlayer();
        }
    }

    void OnEnable()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // 카메라 이동 로직
    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;

        // Z축 값을 고정
        desiredPosition.z = fixedZPosition;

        // 카메라 위치를 최소/최대 값으로 제한
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    // 카메라 이동 영역을 Gizmos로 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0f);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0f);

        Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, 0), new Vector3(maxBounds.x, minBounds.y, 0));
        Gizmos.DrawLine(new Vector3(maxBounds.x, minBounds.y, 0), new Vector3(maxBounds.x, maxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, 0), new Vector3(minBounds.x, maxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, 0), new Vector3(minBounds.x, minBounds.y, 0));
    }
}
