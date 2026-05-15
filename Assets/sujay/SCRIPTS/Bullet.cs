using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 3f;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.linearDamping = 0; // No drag
        Debug.Log("[Bullet] Spawned at " + transform.position + " with velocity " + rb.linearVelocity);
        Destroy(gameObject, lifetime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Bullet] Hit: " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ")");
        
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
        {
            return; // Don't hit player or other bullets
        }

        // Try to damage Target
        Target target = other.GetComponentInParent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Debug.Log("[Bullet] Damaged Target for " + damage);
            Destroy(gameObject);
            return;
        }

        // Try to damage Blob
        Blob blob = other.GetComponentInParent<Blob>();
        if (blob != null)
        {
            blob.TakeDamage(damage);
            Debug.Log("[Bullet] Damaged Blob for " + damage);
            Destroy(gameObject);
            return;
        }

        // Try to damage Dragon
        Dragon dragon = other.GetComponentInParent<Dragon>();
        if (dragon != null)
        {
            dragon.TakeDamage(damage);
            Debug.Log("[Bullet] Damaged Dragon for " + damage);
            Destroy(gameObject);
            return;
        }

        // Try to damage FlyingEnemy
        FlyingEnemy flyingEnemy = other.GetComponentInParent<FlyingEnemy>();
        if (flyingEnemy != null)
        {
            flyingEnemy.TakeDamage(damage);
            Debug.Log("[Bullet] Damaged FlyingEnemy for " + damage);
            Destroy(gameObject);
            return;
        }

        // Try to damage SplittingEnemy
        SplittingEnemy splittingEnemy = other.GetComponentInParent<SplittingEnemy>();
        if (splittingEnemy != null)
        {
            splittingEnemy.TakeDamage(damage);
            Debug.Log("[Bullet] Damaged SplittingEnemy for " + damage);
            Destroy(gameObject);
            return;
        }

        // Try to damage Arachnid
        Arachnid arachnid = other.GetComponentInParent<Arachnid>();
        if (arachnid != null)
        {
            arachnid.TakeDamage(damage);
            Debug.Log("[Bullet] Damaged Arachnid for " + damage);
            Destroy(gameObject);
            return;
        }
        
        // Hit something else - destroy bullet
        Debug.Log("[Bullet] Hit unrecognized object, destroying");
        Destroy(gameObject);
    }
}
