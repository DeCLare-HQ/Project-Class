using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;  // 아이템 정보 저장
    private Transform playerTransform;  // 플레이어의 Transform 참조
    public float pickupRange = 1.0f;  // 아이템을 습득할 수 있는 거리 범위

    private void Start()
    {
        // 플레이어의 Transform을 찾습니다. 태그로 플레이어를 검색합니다.
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // 플레이어와 아이템 사이의 거리를 계산합니다.
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
