using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Unity 에디터에서 아이템 스프라이트 할당
    public Sprite testMeleeSprite;
    public Sprite testMagicSprite;
    public Sprite testHealthSprite;
    public Sprite testDefenseSprite;

    // 생성된 아이템을 외부에서 참조할 수 있도록 변수로 저장
    public Item testMelee;
    public Item testMagic;
    public Item testHealth;
    public Item testDefense;

    private void Start()
    {
        // TEST1-Melee
        testMelee = new Item("TEST1-Melee", ItemType.MeleeAttackBoost, 5f, testMeleeSprite);

        // TEST2-Magic
        testMagic = new Item("TEST2-Magic", ItemType.MagicAttackBoost, 5f, testMagicSprite);

        // TEST3-Health
        testHealth = new Item("TEST3-Health", ItemType.HealthMax, 20f, testHealthSprite);

        // TEST4-Defense
        testDefense = new Item("TEST4-Defense", ItemType.DefenseBoost, 5f, testDefenseSprite);
    }
}
