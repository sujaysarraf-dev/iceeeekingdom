using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode activateKey = KeyCode.Q;
    public float cooldown = 5f;

    private bool isActive = false;
    private float cooldownTimer = 0f;
    private Collider shieldCollider;
    private Renderer shieldRenderer;

    void Start()
    {
        shieldCollider = GetComponent<Collider>();
        shieldRenderer = GetComponent<Renderer>();

        if (shieldCollider == null)
            Debug.LogWarning("[Shield] No Collider found! Add a Sphere Collider for blocking.");
        if (shieldRenderer == null)
            Debug.LogWarning("[Shield] No Renderer found! Shield won't be visible.");

        // Start inactive
        SetShieldActive(false);
        Debug.Log("[Shield] Initialized. Press " + activateKey + " to toggle. Cooldown: " + cooldown + "s");
    }

    void Update()
    {
        // Cooldown timer
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        // Toggle shield
        if (Input.GetKeyDown(activateKey) && cooldownTimer <= 0)
        {
            ToggleShield();
        }
    }

    void ToggleShield()
    {
        isActive = !isActive;
        SetShieldActive(isActive);

        if (isActive)
            Debug.Log("[Shield] Activated!");
        else
        {
            cooldownTimer = cooldown;
            Debug.Log("[Shield] Deactivated! Cooldown: " + cooldown + "s");
        }
    }

    void SetShieldActive(bool active)
    {
        if (shieldCollider != null)
            shieldCollider.enabled = active;
        if (shieldRenderer != null)
            shieldRenderer.enabled = active;

        isActive = active;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        // Block enemy projectiles
        if (other.CompareTag("EnemyProjectile") || other.CompareTag("Bullet"))
        {
            Debug.Log("[Shield] Blocked: " + other.gameObject.name);
            Destroy(other.gameObject);
        }

        // Block enemy touch
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("[Shield] Blocked enemy touch: " + other.gameObject.name);
        }
    }

    void OnDestroy()
    {
        SetShieldActive(false);
    }
}
