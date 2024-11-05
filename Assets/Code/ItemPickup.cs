using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;  // 아이템 정보 저장
    private Transform playerTransform;  // 플레이어의 Transform 참조
    public float pickupRange = 2.0f;  // 아이템을 습득할 수 있는 거리 범위

    private void Start()
    {
        // "Player" 태그로 플레이어를 자동으로 찾아 Transform 할당
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. ItemPickup will wait for player spawn.");
        }
    }

    private void Update()
    {
        // 플레이어 Transform을 찾지 못한 경우 더 이상 처리하지 않음
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                Debug.Log("Player Transform found and assigned in Update.");
            }
            else
            {
                // 플레이어가 아직 스폰되지 않았을 경우 처리하지 않음
                return;
            }
        }

        // 플레이어와 아이템 사이의 거리를 계산
        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        // 플레이어가 일정 범위 내에 있고, 상단 방향키가 눌렸을 때 아이템을 인벤토리에 추가
        if (distanceToPlayer <= pickupRange && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (PlayerState.instance.AddItemToInventory(item))
            {
                Debug.Log($"{item.itemName} 아이템 인벤토리에 추가");
                Destroy(gameObject);  // 아이템 오브젝트 제거
            }
        }
    }
}
