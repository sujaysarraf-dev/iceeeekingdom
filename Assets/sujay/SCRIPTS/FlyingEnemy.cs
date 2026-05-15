using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float health = 80f;
    public float moveSpeed = 3f;
    public float detectionRange = 20f;
    public int scoreValue = 150;

    [Header("Flying")]
    public float hoverAmplitude = 0.5f;
    public float hoverSpeed = 2f;

    [Header("Combat")]
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int touchDamage = 15;
    public GameObject attackSplashEffect;

    private Transform player;
    private float originalY;
    private float timer = 0f;
    private float attackTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        originalY = transform.position.y;
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Hover motion
        timer += Time.deltaTime * hoverSpeed;
        float newY = originalY + Mathf.Sin(timer) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Chase until attack range, then stop
            if (distanceToPlayer > attackRange)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            }

            // Attack while in range
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    void Attack()
    {
        if (projectilePrefab == null || player == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            Vector3 direction = (player.position - spawnPos).normalized;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * 12f, ForceMode.Impulse);
        }

    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(25f);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(touchDamage);
            }
        }
    }
}
