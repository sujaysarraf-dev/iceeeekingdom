using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public int arrowsPerVolley = 5;
    public float spreadAngle = 30f;
    public float arrowSpeed = 20f;
    public float fireRate = 0.1f;
    public float cooldown = 2f;
    public int arrowDamage = 10;
    public Vector3 arrowRotation = Vector3.zero;

    private float fireTimer = 0f;
    private float cooldownTimer = 0f;
    private int arrowsFired = 0;
    private bool isFiring = false;

    void Start()
    {
        if (arrowPrefab == null)
            Debug.LogError("arrowPrefab not assigned!");
        if (firePoint == null)
            firePoint = transform;
    }

    void Update()
    {
        if (arrowPrefab == null) return;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isFiring)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                FireArrow();
                arrowsFired++;
                fireTimer = fireRate;
                if (arrowsFired >= arrowsPerVolley)
                {
                    isFiring = false;
                    cooldownTimer = cooldown;
                }
            }
        }
        else if (cooldownTimer <= 0f)
        {
            arrowsFired = 0;
            isFiring = true;
            fireTimer = 0f;
        }
    }

    void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        if (arrow.GetComponent<Collider>() == null) arrow.AddComponent<BoxCollider>();
        arrow.GetComponent<Collider>().isTrigger = true;

        float angle = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
        Vector3 dir = Quaternion.Euler(0, angle, 0) * firePoint.forward;
        arrow.transform.rotation = Quaternion.Euler(arrowRotation);

        ArrowMover mover = arrow.AddComponent<ArrowMover>();
        mover.direction = dir;
        mover.speed = arrowSpeed;
        mover.damage = arrowDamage;

        Destroy(arrow, 5f);
    }

    void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, firePoint.forward * 5f);
            Gizmos.color = Color.yellow;
            float a = spreadAngle / 2f;
            Gizmos.DrawRay(firePoint.position, Quaternion.Euler(0, -a, 0) * firePoint.forward * 5f);
            Gizmos.DrawRay(firePoint.position, Quaternion.Euler(0, a, 0) * firePoint.forward * 5f);
        }
    }
}

public class ArrowMover : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 20f;
    public int damage = 10;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
