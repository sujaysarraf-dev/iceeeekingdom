using UnityEngine;
using UnityEngine.SceneManagement;

public class Dragon : MonoBehaviour
{
    [Header("Stats")]
    public float health = 200f;
    public float detectionRange = 30f;
    public int scoreValue = 500;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float turnSpeed = 15f;
    public float wanderStrength = 2f;
    public float changeDirectionTime = 3f;

    [Header("Combat")]
    public float attackRange = 25f;
    public float attackCooldown = 3f;
    public float attackDamageDuration = 1f;
    public int touchDamage = 30;
    public float touchDamageRange = 3f;
    public float touchDamageInterval = 1f;
    private float touchDamageTimer = 0f;

    [Header("Audio")]
    public AudioClip breathSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
    public float breathVolume = 0.5f;
    public float attackVolume = 1f;
    public float deathVolume = 1f;
    private AudioSource audioSource;

    [Header("Animation States")]
    public Animator animator;
    public string idleState = "Idle";
    public string walkState = "Walk";
    public string runState = "Run";
    public string attackState1 = "Basic Attack";
    public string attackState2 = "Horn Attack";
    public string attackState3 = "Claw Attack";
    public string hitState = "Get Hit";
    public string dieState = "Die";

    private Transform player;
    private float timer = 0f;
    private float attackTimer = 0f;
    private float directionTimer = 0f;
    private bool isDead = false;
    private Vector3 wanderOffset = Vector3.zero;
    private bool isAttacking = false;
    private float hitTimer = 0f;
    private string currentState = "";
    private float stateChangeTimer = 0f;
    private bool isChasing = false;
    private bool canDamagePlayer = false;
    private float damageTimer = 0f;
    private Rigidbody rb;
    private Collider dragonCollider;

    void Start()
    {
        Debug.Log("[Dragon] ===== START =====");

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("[Dragon] Player NOT FOUND!");

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Use non-kinematic rigidbody for proper collision
        rb.mass = 100f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.freezeRotation = true;

        dragonCollider = GetComponent<Collider>();
        if (dragonCollider == null)
            dragonCollider = GetComponentInChildren<Collider>();

        if (dragonCollider != null)
        {
            dragonCollider.isTrigger = false;
            dragonCollider.enabled = true;
            Debug.Log("[Dragon] Collider: " + dragonCollider.GetType().Name + ", isTrigger: " + dragonCollider.isTrigger);
        }
        else
        {
            Debug.LogError("[Dragon] NO COLLIDER! Adding BoxCollider...");
            dragonCollider = gameObject.AddComponent<BoxCollider>();
            dragonCollider.isTrigger = false;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 50f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        if (breathSound != null)
        {
            audioSource.clip = breathSound;
            audioSource.volume = breathVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        directionTimer = changeDirectionTime;
        wanderOffset = Vector3.zero;
        currentState = idleState;
        ChangeState(idleState);

        Debug.Log("[Dragon] Initialized. Health: " + health);
    }

    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[Dragon] MANUAL DAMAGE TEST (T key)");
            TakeDamage(25f);
        }

        timer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        directionTimer += Time.deltaTime;
        stateChangeTimer += Time.deltaTime;

        if (canDamagePlayer)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f) canDamagePlayer = false;
        }

        if (hitTimer > 0f)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0f && !isDead)
                ChangeState(isChasing ? runState : walkState);
            return;
        }

        if (isAttacking) return;

        if (directionTimer >= changeDirectionTime)
        {
            directionTimer = 0f;
            wanderOffset = new Vector3(
                Random.Range(-wanderStrength, wanderStrength),
                0f,
                Random.Range(-wanderStrength, wanderStrength)
            );
        }

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Proximity damage - damage player when very close
            if (distanceToPlayer <= touchDamageRange)
            {
                touchDamageTimer -= Time.deltaTime;
                if (touchDamageTimer <= 0f)
                {
                    Player playerScript = player.GetComponent<Player>();
                    if (playerScript != null)
                    {
                        playerScript.TakeDamage(touchDamage);
                        Debug.Log("[Dragon] Proximity damage to player: " + touchDamage);
                    }
                    touchDamageTimer = touchDamageInterval;
                }
            }
            else
            {
                touchDamageTimer = 0f;
            }

            if (distanceToPlayer <= detectionRange)
            {
                isChasing = true;
                ChangeState(runState);

                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
                }

                if (distanceToPlayer <= attackRange && attackTimer >= attackCooldown)
                    StartAttack();
            }
            else if (distanceToPlayer <= detectionRange * 1.5f)
            {
                isChasing = false;
                ChangeState(walkState);
            }
            else
            {
                isChasing = false;
                ChangeState(walkState);
            }
        }
        else
        {
            ChangeState(idleState);
        }
    }

    void FixedUpdate()
    {
        if (isDead || hitTimer > 0f || isAttacking || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 moveDir = transform.forward * runSpeed;
            moveDir.y = 0f;
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }
        else if (distanceToPlayer <= detectionRange * 1.5f)
        {
            Vector3 moveDir = transform.forward * walkSpeed;
            moveDir += wanderOffset * 0.5f;
            moveDir.y = 0f;
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }
        else
        {
            Vector3 moveDir = transform.forward * walkSpeed;
            moveDir += wanderOffset;
            moveDir.y = 0f;
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }
    }

    void ChangeState(string newState)
    {
        if (currentState != newState && stateChangeTimer > 0.2f)
        {
            currentState = newState;
            stateChangeTimer = 0f;
            if (animator != null && animator.enabled)
                animator.Play(newState, 0, 0f);
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;
        canDamagePlayer = true;
        damageTimer = attackDamageDuration;

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound, attackVolume);

        int attackIndex = Random.Range(0, 3);
        string attackAnim = attackState1;
        if (attackIndex == 1) attackAnim = attackState2;
        else if (attackIndex == 2) attackAnim = attackState3;

        currentState = attackAnim;
        if (animator != null)
            animator.Play(attackAnim, 0, 0f);

        Invoke("ResetAttack", 1.5f);
    }

    void ResetAttack()
    {
        isAttacking = false;
        canDamagePlayer = false;
        ChangeState(isChasing ? runState : walkState);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log("[Dragon] TOOK DAMAGE: " + damage + ", Health: " + health + "/" + 200f);

        if (!isAttacking && hitTimer <= 0f)
        {
            currentState = hitState;
            if (animator != null)
                animator.Play(hitState, 0, 0f);
            hitTimer = 0.5f;
        }

        if (health <= 0)
        {
            Debug.Log("[Dragon] Health <= 0, calling Die()");
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("[Dragon] DIED! +" + scoreValue + " score");

        if (audioSource != null)
        {
            audioSource.Stop();
            if (deathSound != null)
                audioSource.PlayOneShot(deathSound, deathVolume);
        }

        currentState = dieState;
        if (animator != null)
            animator.Play(dieState, 0, 0f);

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        Invoke("LoadWonScene", 2f);
        Destroy(gameObject, 3f);
    }

    void LoadWonScene()
    {
        SceneManager.LoadScene("won");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[Dragon] OnCollisionEnter with: " + collision.gameObject.name + ", Tag: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("[Dragon] HIT BY BULLET!");
            TakeDamage(25f);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[Dragon] Collision with PLAYER! canDamagePlayer: " + canDamagePlayer);
            if (canDamagePlayer)
            {
                Player playerScript = collision.gameObject.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(touchDamage);
                    Debug.Log("[Dragon] DAMAGED PLAYER for " + touchDamage);
                }
            }
        }
    }
}
