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
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                PlayerState.instance = playerObject.GetComponent<PlayerState>(); // PlayerState 참조 보장
                Debug.Log("Player Transform과 PlayerState가 할당되었습니다.");
            }
            else
            {
                return;
            }
        }

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        if (distanceToPlayer <= pickupRange && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (PlayerState.instance.AddItemToInventory(item))
            {
                Debug.Log($"{item.itemName} 아이템 인벤토리에 추가");
                Destroy(gameObject);
            }
        }
    }
}