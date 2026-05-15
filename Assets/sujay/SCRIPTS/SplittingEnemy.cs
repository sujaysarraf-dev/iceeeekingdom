using UnityEngine;

public class SplittingEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float moveSpeed = 3f;
    public float detectionRange = 20f;
    public int scoreValue = 200;
    public int currentSize = 1; // 1 = large, 2 = medium, 4 = small

    [Header("Combat")]
    public int touchDamage = 15;

    [Header("Splitting")]
    public int splitCount = 2;

    [Header("Wandering")]
    public float wanderRadius = 3f;
    public float wanderTimer = 3f;

    private Transform player;
    private Rigidbody rb;
    private bool isDead = false;
    private Vector3 wanderTarget;
    private float wanderCooldown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("[SplittingEnemy] Player not found! Tag Player as 'Player'.");

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = 1000f;
            rb.freezeRotation = true;
            rb.useGravity = true;
        }

        wanderTarget = transform.position;
        wanderCooldown = wanderTimer;

        Debug.Log("[SplittingEnemy] Size " + currentSize + " initialized. Health: " + health);
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        Vector3 moveDirection = Vector3.zero;

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > 2f)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;
                moveDirection = direction * moveSpeed;
            }
        }
        else
        {
            moveDirection = GetWanderDirection() * moveSpeed * 0.5f;
        }

        // Move using Rigidbody to respect physics
        if (rb != null && moveDirection != Vector3.zero)
        {
            Vector3 newPos = transform.position + moveDirection * Time.deltaTime;
            rb.MovePosition(newPos);
        }
    }

    Vector3 GetWanderDirection()
    {
        wanderCooldown -= Time.deltaTime;

        if (wanderCooldown <= 0f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0;
            wanderTarget = transform.position + randomDirection;
            wanderCooldown = wanderTimer;
        }

        Vector3 direction = (wanderTarget - transform.position).normalized;
        direction.y = 0;
        return direction;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log("[SplittingEnemy] Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("[SplittingEnemy] Size " + currentSize + " destroyed! +" + scoreValue + " score");

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        if (currentSize < 4)
        {
            SpawnSmallerEnemies();
        }

        Destroy(gameObject);
    }

    void SpawnSmallerEnemies()
    {
        for (int i = 0; i < splitCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            GameObject smallerEnemy = Instantiate(gameObject, spawnPos, Quaternion.identity);

            SplittingEnemy enemyScript = smallerEnemy.GetComponent<SplittingEnemy>();
            if (enemyScript != null)
            {
                enemyScript.currentSize = currentSize * 2;
                enemyScript.health = GetHealthForSize(enemyScript.currentSize);
                enemyScript.moveSpeed = moveSpeed * 1.2f;
                enemyScript.scoreValue = scoreValue / 2;
                enemyScript.transform.localScale = transform.localScale * 0.5f;
                enemyScript.isDead = false;
            }

            Debug.Log("[SplittingEnemy] Spawned smaller enemy (size " + (currentSize * 2) + ")");
        }
    }

    float GetHealthForSize(int size)
    {
        if (size == 2) return 50f;
        if (size == 4) return 25f;
        return health;
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
                Debug.Log("[SplittingEnemy] Touched player for " + touchDamage + " damage");
            }
        }
    }
}
