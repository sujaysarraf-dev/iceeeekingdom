using UnityEngine;

// This script forwards collision events to the parent Dragon script
public class DragonCollisionForwarder : MonoBehaviour
{
    private Dragon dragonScript;

    void Start()
    {
        dragonScript = GetComponentInParent<Dragon>();
        if (dragonScript == null)
        {
            Debug.LogError("[DragonCollisionForwarder] Dragon script not found in parent!");
        }
        else
        {
            Debug.Log("[DragonCollisionForwarder] Found Dragon script: " + dragonScript.name);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[DragonCollisionForwarder] Collision on child: " + collision.gameObject.name + ", forwarding to Dragon...");
        if (dragonScript != null)
        {
            // Call Dragon's collision handler via reflection or make method public
            // For now, let's just call TakeDamage if it's a bullet
            if (collision.gameObject.CompareTag("Bullet"))
            {
                dragonScript.TakeDamage(25f);
                Destroy(collision.gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("[DragonCollisionForwarder] Trigger on child: " + other.name + " - collider isTrigger might be true");
    }
}
