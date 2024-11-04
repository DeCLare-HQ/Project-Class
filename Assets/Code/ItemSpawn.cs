using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public Transform playerTransform; // 아이템을 소환할 플레이어의 위치

    // 아이템 매니저 참조 (아이템 정보를 가져오기 위함)
    public ItemManager itemManager;

    // 아이템이 속할 레이어 (플레이어와 충돌하지 않도록)
    public string itemLayer = "Item";  // 아이템이 속할 레이어 이름

    // 아이템에 적용할 Physics Material 2D (마찰력 조절용)
    public PhysicsMaterial2D itemPhysicsMaterial;

    private void Update()
    {
        // 1번 키를 눌렀을 때 TEST1-Melee 아이템 생성
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SpawnItem(itemManager.testMelee);
        }

        // 2번 키를 눌렀을 때 TEST2-Magic 아이템 생성
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SpawnItem(itemManager.testMagic);
        }

        // 3번 키를 눌렀을 때 TEST3-Health 아이템 생성
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SpawnItem(itemManager.testHealth);
        }

        // 4번 키를 눌렀을 때 TEST4-Defense 아이템 생성
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SpawnItem(itemManager.testDefense);
        }
    }

    // 아이템을 소환하는 메서드
    private void SpawnItem(Item item)
    {
        // 플레이어 위치에서 아이템을 생성할 위치 설정
        Vector3 spawnPosition = playerTransform.position;

        // 빈 GameObject 생성
        GameObject newItemObject = new GameObject(item.itemName);

        // 스프라이트 설정을 위한 SpriteRenderer 추가
        SpriteRenderer spriteRenderer = newItemObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemSprite;

        // Rigidbody2D 추가해서 중력 적용
        Rigidbody2D rb = newItemObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1;

        // 충돌 처리를 위한 Collider2D 추가 (BoxCollider2D 또는 CircleCollider2D 선택)
        BoxCollider2D collider = newItemObject.AddComponent<BoxCollider2D>();

        // Physics Material 2D 적용 (아이템이 미끄러지지 않게)
        collider.sharedMaterial = itemPhysicsMaterial;

        // 생성된 아이템 오브젝트의 위치 설정
        newItemObject.transform.position = spawnPosition;

        // 아이템 레이어 설정 (플레이어와 충돌하지 않도록)
        newItemObject.layer = LayerMask.NameToLayer(itemLayer);

        Debug.Log($"Spawned Item: {item.itemName} at {spawnPosition}");
    }
}
