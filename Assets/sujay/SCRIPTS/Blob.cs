using UnityEngine;

public class Blob : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public int scoreValue = 100;

    [Header("Death Effect")]
    public GameObject deathEffect;
    public float deathDamage = 50f;
    public float deathRadius = 5f;

    private bool isDead = false;

    void Start()
    {
        if (health <= 0) health = 50f;
        
        // Ensure we have a collider that is NOT a trigger (so bullet trigger can hit it)
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
            Debug.Log("[Blob] Added SphereCollider");
        }
        col.isTrigger = false; // Important: must NOT be trigger for bullet's trigger to hit it
        
        // Ensure we have a rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("[Blob] Added Rigidbody");
        }
        rb.useGravity = false;
        rb.isKinematic = false; // Can be affected by physics, but no movement code = stationary
        
        Debug.Log("[Blob] Started on " + gameObject.name + " - Health: " + health);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log("[Blob] Took damage: " + damage + ", Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("[Blob] Died! +" + scoreValue + " score");

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        // Play death effect (once, not looped)
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Destroy effect after 2 seconds
        }

        // Deal area damage once
        Collider[] colliders = Physics.OverlapSphere(transform.position, deathRadius);
        foreach (Collider col in colliders)
        {
            // Skip self
            if (col.gameObject == gameObject) continue;

            // Try to damage Blob
            Blob blob = col.GetComponent<Blob>();
            if (blob != null && blob != this)
            {
                blob.TakeDamage(deathDamage);
                Debug.Log("[Blob] Area damage to Blob: " + deathDamage);
                continue;
            }

            // Try to damage Dragon
            Dragon dragon = col.GetComponent<Dragon>();
            if (dragon != null)
            {
                dragon.TakeDamage(deathDamage);
                Debug.Log("[Blob] Area damage to Dragon: " + deathDamage);
                continue;
            }

            // Try to damage FlyingEnemy
            FlyingEnemy flyingEnemy = col.GetComponent<FlyingEnemy>();
            if (flyingEnemy != null)
            {
                flyingEnemy.TakeDamage(deathDamage);
                Debug.Log("[Blob] Area damage to FlyingEnemy: " + deathDamage);
                continue;
            }

            // Try to damage SplittingEnemy
            SplittingEnemy splittingEnemy = col.GetComponent<SplittingEnemy>();
            if (splittingEnemy != null)
            {
                splittingEnemy.TakeDamage(deathDamage);
                Debug.Log("[Blob] Area damage to SplittingEnemy: " + deathDamage);
                continue;
            }

            // Try to damage Player
            Player player = col.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage((int)deathDamage);
                Debug.Log("[Blob] Area damage to Player: " + deathDamage);
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Blob] OnTriggerEnter with: " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ")");
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("[Blob] Hit by bullet (trigger)!");
            TakeDamage(25f);
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[Blob] OnCollisionEnter with: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("[Blob] Hit by bullet (collision)!");
            TakeDamage(25f);
            Destroy(collision.gameObject);
        }
    }
}
