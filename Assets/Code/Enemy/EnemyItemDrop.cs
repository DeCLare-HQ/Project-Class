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

    private float dropChance; // 아이템 드롭 확률

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
        }
    }

    // 적이 죽을 때 호출
    public void HandleEnemyDeath()
    {
        // 일반 아이템 드롭 확률에 따라 랜덤 드롭
        if (Random.value <= dropChance)
        {
            DropRandomItem();
        }

        // 돈 아이템은 무조건 드롭
        DropMoneyItem();
    }

    // 랜덤한 아이템을 드롭하는 메서드
    private void DropRandomItem()
    {
        // Money 타입을 제외한 아이템 선택
        Item[] items = { itemManager.testMelee, itemManager.testMagic, itemManager.testHealth, itemManager.testDefense };
        Item randomItem = items[Random.Range(0, items.Length)];

        Vector3 dropPosition = GetDropPosition();
        ItemUtility.SpawnItem(randomItem, dropPosition, itemLayer, itemPhysicsMaterial);
    }

    // 돈 아이템 드롭 (확률에 따라 1원, 5원, 10원)
    private void DropMoneyItem()
    {
        float rand = Random.value;
        Item moneyItem;

        if (rand < 0.5f)
        {
            moneyItem = itemManager.money1; // 1원 (50% 확률)
        }
        else if (rand < 0.85f)
        {
            moneyItem = itemManager.money5; // 5원 (35% 확률)
        }
        else
        {
            moneyItem = itemManager.money10; // 10원 (15% 확률)
        }

        Vector3 dropPosition = GetDropPosition();
        ItemUtility.SpawnItem(moneyItem, dropPosition, itemLayer, itemPhysicsMaterial);
    }

    // 드롭 위치 설정 메서드
    private Vector3 GetDropPosition()
    {
        Vector3 dropPosition = transform.position + new Vector3(0, 2.0f, 0);
        if (playerTransform != null && Vector3.Distance(playerTransform.position, dropPosition) > pickupRange)
        {
            dropPosition = playerTransform.position + new Vector3(1.5f, 0, 0);
        }
        return dropPosition;
    }
}