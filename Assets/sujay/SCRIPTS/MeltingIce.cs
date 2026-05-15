using UnityEngine;

public class MeltingIce : MonoBehaviour
{
    [Header("Melting Settings")]
    public float meltDuration = 5f; // Seconds to fully melt
    public float meltDamage = 20f; // Damage per second from heat
    public bool isMelting = false;
    public GameObject waterEffectPrefab; // Optional water particles

    [Header("Visual")]
    public Material meltMaterial; // Material that changes as it melts
    public Color meltColor = new Color(0.5f, 0.5f, 1f, 0.3f); // More transparent when melted

    private float currentMeltTime = 0f;
    private Vector3 originalScale;
    private Renderer objRenderer;

    void Start()
    {
        originalScale = transform.localScale;
        objRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (!isMelting) return;

        currentMeltTime += Time.deltaTime;
        float meltPercent = currentMeltTime / meltDuration;

        // Shrink object
        transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, meltPercent);

        // Change material transparency
        if (objRenderer != null && meltMaterial != null)
        {
            meltMaterial.color = Color.Lerp(Color.white, meltColor, meltPercent);
        }

        // Fully melted
        if (currentMeltTime >= meltDuration)
        {
            MeltCompletely();
        }
    }

    public void StartMelting()
    {
        if (isMelting) return;
        
        isMelting = true;
        currentMeltTime = 0f;
    }

    public void TakeHeatDamage(float damage)
    {
        // Called when hit by fire/heat source
        currentMeltTime += damage / meltDamage * Time.deltaTime;
        
        if (!isMelting)
        {
            StartMelting();
        }
    }

    void MeltCompletely()
    {
        // Spawn water effect
        if (waterEffectPrefab != null)
        {
            Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if hit by fire bullet
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet != null && bullet.damage > 0)
        {
            TakeHeatDamage(bullet.damage);
        }
    }
}
