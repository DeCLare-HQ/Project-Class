using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Sprite testSprite; // Unity 에디터에서 아이템 스프라이트 할당

    private void Start()
    {
        // TEST1 아이템 타입-MeleeAttackBoost, 효과 +5
        Item testItem = new Item("TEST1", ItemType.MeleeAttackBoost, 5f, testSprite);

        // 확인을 위해 로그 출력
        Debug.Log($"Item Created: {testItem.itemName}, Type: {testItem.itemType}, Effect: +{testItem.effectAmount} Physical Attack, Sprite: {testItem.itemSprite}");
    }
}
