using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public Slider healthBar;
    public Image healthFillImage;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float detectionRadius = 8f;
    public float loseRadius = 12f;

    [Header("Enemy Shooting")]
    public float shootRange = 6f;
    public float shootRate = 1.5f;
    public GameObject enemyBulletPrefab;
    public Transform enemyFirePoint;

    private Transform player;
    private Vector3 patrolPoint;
    private float patrolTimer;
    private bool isChasing;
    private float nextShootTime = 0f;

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNewPatrolPoint();
        UpdateHealthBar();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > loseRadius)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0f || Vector3.Distance(transform.position, patrolPoint) < 1f)
        {
            SetNewPatrolPoint();
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolPoint, moveSpeed * Time.deltaTime);
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }

        // Shooting disabled for now
        // float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // if (distanceToPlayer <= shootRange && Time.time >= nextShootTime && enemyBulletPrefab != null && enemyFirePoint != null)
        // {
        //     ShootAtPlayer();
        //     nextShootTime = Time.time + shootRate;
        // }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection.y = 0;
        patrolPoint = transform.position + randomDirection;
        patrolTimer = Random.Range(3f, 6f);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        UpdateHealthBar();

        if (health <= 0f)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
        }
        Destroy(gameObject);
    }
}
