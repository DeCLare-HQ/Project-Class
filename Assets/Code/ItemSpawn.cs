using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public Transform playerTransform;
    public ItemManager itemManager;
    public string itemLayer = "Item";
    public PhysicsMaterial2D itemPhysicsMaterial;

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
        }

        if (playerTransform != null)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1)) SpawnItem(itemManager.testMelee);
            if (Input.GetKeyDown(KeyCode.Keypad2)) SpawnItem(itemManager.testMagic);
            if (Input.GetKeyDown(KeyCode.Keypad3)) SpawnItem(itemManager.testHealth);
            if (Input.GetKeyDown(KeyCode.Keypad4)) SpawnItem(itemManager.testDefense);
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void SpawnItem(Item item)
    {
        Vector3 spawnPosition = playerTransform.position;
        ItemUtility.SpawnItem(item, spawnPosition, itemLayer, itemPhysicsMaterial);
    }
}