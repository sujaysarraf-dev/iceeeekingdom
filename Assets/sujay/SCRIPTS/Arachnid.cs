using UnityEngine;

public class Arachnid : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public int scoreValue = 100;

    [Header("Movement")]
    public float detectionRange = 15f;
    public float chaseSpeed = 4f;
    public float turnSpeed = 5f;
    public float stopDistance = 2f;

    [Header("Damage")]
    public int touchDamage = 15;
    public float damageInterval = 1f;

    [Header("Death")]
    public GameObject deathEffect;

    private Transform player;
    private float damageTimer = 0f;
    private bool isDead = false;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.freezeRotation = true;
        }
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > stopDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

            rb.linearVelocity = new Vector3(transform.forward.x * chaseSpeed, rb.linearVelocity.y, transform.forward.z * chaseSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        if (distance <= stopDistance + 1f)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                Player p = player.GetComponent<Player>();
                if (p != null)
                {
                    p.TakeDamage(touchDamage);
                    Debug.Log("[Arachnid] Damaged player: " + touchDamage);
                }
                damageTimer = damageInterval;
            }
        }
        else
        {
            damageTimer = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        health -= damage;
        Debug.Log("[Arachnid] Took damage: " + damage + ", HP: " + health);
        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }
}
