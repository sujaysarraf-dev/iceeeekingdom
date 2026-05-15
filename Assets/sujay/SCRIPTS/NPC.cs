using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName = "Villager";
    [TextArea(3, 5)]
    public string message = "Hello there!";
    public float interactionRange = 3f;

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[" + npcName + "]: " + message);
        }
    }
}
