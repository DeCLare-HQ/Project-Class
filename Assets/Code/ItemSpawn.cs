using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public Transform playerTransform;
    public ItemManager itemManager;
    public string itemLayer = "Item";
    public PhysicsMaterial2D itemPhysicsMaterial;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1)) SpawnItem(itemManager.testMelee);
        if (Input.GetKeyDown(KeyCode.Keypad2)) SpawnItem(itemManager.testMagic);
        if (Input.GetKeyDown(KeyCode.Keypad3)) SpawnItem(itemManager.testHealth);
        if (Input.GetKeyDown(KeyCode.Keypad4)) SpawnItem(itemManager.testDefense);
    }

    private void SpawnItem(Item item)
    {
        Vector3 spawnPosition = playerTransform.position;
        GameObject newItemObject = new GameObject(item.itemName);

        // 스프라이트 렌더러 추가
        SpriteRenderer spriteRenderer = newItemObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemSprite;

        // Rigidbody2D 추가
        Rigidbody2D rb = newItemObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // 콜라이더
        BoxCollider2D collisionCollider = newItemObject.AddComponent<BoxCollider2D>();
        collisionCollider.isTrigger = false;

        // 아이템 레이어 및 위치 설정
        newItemObject.transform.position = spawnPosition;
        newItemObject.layer = LayerMask.NameToLayer(itemLayer);

        // ItemPickup 컴포넌트 추가 및 아이템 정보 설정
        ItemPickup itemPickup = newItemObject.AddComponent<ItemPickup>();
        itemPickup.item = item;

        Debug.Log($"Spawned Item: {item.itemName} at {spawnPosition}");
    }
}