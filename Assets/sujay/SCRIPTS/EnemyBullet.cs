using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 3f;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void SetTarget(Transform playerTarget)
    {
        target = playerTarget;
    }

    void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage((int)damage);
        }

        Destroy(gameObject);
    }
}
