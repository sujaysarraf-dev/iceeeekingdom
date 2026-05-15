using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 10;
    public float damageInterval = 2f;
    public bool instantDamageOnTouch = true;

    private float timer = 0f;
    private bool playerOnSpike = false;
    private Player player;

    void Update()
    {
        if (!playerOnSpike || player == null) return;

        timer += Time.deltaTime;

        if (timer >= damageInterval)
        {
            timer = 0f;

            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("[Spike] Interval damage: " + damage);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnSpike = true;
            timer = 0f;

            player = other.GetComponent<Player>();

            if (instantDamageOnTouch && player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("[Spike] Instant damage: " + damage);
            }

            Debug.Log("[Spike] Player stepped on spike");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnSpike = false;
            timer = 0f;
            player = null;

            Debug.Log("[Spike] Player left spike");
        }
    }
}