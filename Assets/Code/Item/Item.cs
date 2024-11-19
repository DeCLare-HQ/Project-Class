using UnityEngine;

public enum ItemType
{
    Money,               // 돈 증가
    HealthMax,          // 최대 체력 증가
    MeleeAttackBoost,   // 물리 공격력 증가
    MagicAttackBoost,   // 마법 공격력 증가
    DefenseBoost        // 방어력 증가
}

[System.Serializable]
public class Item
{
    public string itemName;      // 아이템 이름
    public ItemType itemType;    // 아이템 종류
    public float effectAmount;   // 아이템 효과 수치
    public Sprite itemSprite;    // 아이템 이미지

    public Item(string name, ItemType type, float effect, Sprite sprite)
    {
        itemName = name;
        itemType = type;
        effectAmount = effect;
        itemSprite = sprite;
    }
}