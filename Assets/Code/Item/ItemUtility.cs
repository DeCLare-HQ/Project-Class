using UnityEngine;

public static class ItemUtility
{
    public static void SpawnItem(Item item, Vector3 spawnPosition, string itemLayer, PhysicsMaterial2D physicsMaterial)
    {
        GameObject newItemObject = new GameObject(item.itemName);
        newItemObject.transform.position = spawnPosition;


        // 스프라이트 렌더러
        SpriteRenderer spriteRenderer = newItemObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemSprite;

        //레이어 우선 순위
        spriteRenderer.sortingOrder = 5;

        // Rigidbody2D
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