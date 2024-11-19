using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Unity 에디터에서 아이템 스프라이트 할당
    public Sprite Money1Sprite;
    public Sprite Money5Sprite;

    public Sprite Money10Sprite;

    public Sprite testMeleeSprite;
    public Sprite testMagicSprite;
    public Sprite testHealthSprite;
    public Sprite testDefenseSprite;

    // 생성된 아이템을 외부에서 참조할 수 있도록 변수로 저장
    public Item money1;
    public Item money5;
    public Item money10;
    public Item testMelee;
    public Item testMagic;
    public Item testHealth;
    public Item testDefense;

    private void Start()
    {
        // Money-1
        money1 = new Item
        (
            "Money-1", 
            ItemType.Money, 
            1f, 
            Money1Sprite, 
            1, 
            "작은 단위의 돈"
        );

        // Money-5
        money5 = new Item
        (
            "Money-5", 
            ItemType.Money, 
            5f, 
            Money5Sprite, 
            5, 
            "중간 단위의 돈"
        );

        // Money-10
        money10 = new Item
        (
            "Money-10", 
            ItemType.Money, 
            10f, 
            Money10Sprite, 
            10, 
            "큰 단위의 돈"
        );

        // TEST1-Melee
        testMelee = new Item
        (
            "TEST1-Melee", 
            ItemType.MeleeAttackBoost, 
            5f, 
            testMeleeSprite, 
            5, 
            "테스트-근접 5 증가"
        );

        // TEST2-Magic
        testMagic = new Item
        (
            "TEST2-Magic", 
            ItemType.MagicAttackBoost, 
            5f, 
            testMagicSprite, 
            5, 
            "테스트-원거리 5 증가"
        );

        // TEST3-Health
        testHealth = new Item
        (
            "TEST3-Health", 
            ItemType.HealthMax, 
            20f, 
            testHealthSprite, 
            3, 
            "테스트-체력 20 증가"
        );

        // TEST4-Defense
        testDefense = new Item
        (
            "TEST4-Defense", 
            ItemType.DefenseBoost, 
            2f, 
            testDefenseSprite, 
            8, 
            "테스트-방어 2 증가"
        );
    }
}
