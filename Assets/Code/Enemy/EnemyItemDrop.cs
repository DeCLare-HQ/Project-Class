using UnityEngine;

public enum EnemyType
{
    Melee,   // 근접 공격 적
    Range    // 원거리 공격 적
}

public class EnemyItemDrop : MonoBehaviour
{
    public ItemManager itemManager;  // 아이템 매니저 참조
    public string itemLayer = "Item"; // 아이템 레이어
    public PhysicsMaterial2D itemPhysicsMaterial; // 물리 재질 참조
    public float pickupRange = 5.0f; // 아이템 드롭 시 플레이어와의 최대 거리

    private Transform playerTransform;

    [Header("적의 종류 설정")]
    public EnemyType enemyType; // 적의 종류

    private float dropChance; // 드롭 확률

    private void Start()
    {
        // 적의 종류에 따른 드롭 확률 설정
        SetDropChanceByEnemyType();

        // 플레이어 위치 확인
        FindPlayer();
    }

    // 적의 타입에 따른 드롭 확률 설정
    private void SetDropChanceByEnemyType()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                dropChance = 0.7f; // Melee 확률
                break;
            case EnemyType.Range:
                dropChance = 0.5f; // Range 확률
                break;
            default:
                dropChance = 1f; // 기본값
                break;
        }

        Debug.Log($"Enemy Type: {enemyType}, Drop Chance: {dropChance * 100}%");
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            Debug.Log("Player Transform found and assigned.");
        }
        else
        {
            Debug.LogWarning("Player object not found for EnemyItemDrop.");
        }
    }

    // 아이템 드롭 처리 메서드 (적이 죽을 때 호출)
    public void HandleEnemyDeath()
    {
        // 드롭 확률에 따라 아이템 드롭 결정
        if (Random.value <= dropChance)
        {
            DropRandomItem();
        }
    }

    // 랜덤한 아이템을 드롭하는 메서드
    private void DropRandomItem()
    {
        // ItemManager에서 랜덤 아이템 선택
        Item[] items = { itemManager.testMelee, itemManager.testMagic, itemManager.testHealth, itemManager.testDefense };
        Item randomItem = items[Random.Range(0, items.Length)];

        // 기본 드롭 위치는 적의 위치에서 약간 위
        Vector3 dropPosition = transform.position + new Vector3(0, 2.0f, 0);

        // 플레이어와의 거리 확인
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(playerTransform.position, dropPosition);

            // 플레이어와의 거리가 pickupRange 이상이면 플레이어 근처에 드롭
            if (distanceToPlayer > pickupRange)
            {
                dropPosition = playerTransform.position + new Vector3(1.5f, 0, 0); // 플레이어 근처에 드롭
                Debug.Log("Item drop position adjusted closer to the player.");
            }
        }

        // 아이템 유틸리티를 사용하여 아이템 소환
        ItemUtility.SpawnItem(randomItem, dropPosition, itemLayer, itemPhysicsMaterial);
    }
}