using UnityEngine;

public class SwingingAxe : MonoBehaviour
{
    public enum SwingAxis { X, Y, Z }
    public SwingAxis swingAxis = SwingAxis.Z;
    public float swingAngle = 60f;
    public float swingSpeed = 2f;
    public int damage = 10;

    private float startAngle;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        Vector3 euler = transform.localEulerAngles;
        if (swingAxis == SwingAxis.X) startAngle = euler.x;
        else if (swingAxis == SwingAxis.Y) startAngle = euler.y;
        else startAngle = euler.z;
    }

    void Update()
    {
        float angle = startAngle + Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        Vector3 rot = transform.localEulerAngles;
        if (swingAxis == SwingAxis.X) rot.x = angle;
        else if (swingAxis == SwingAxis.Y) rot.y = angle;
        else rot.z = angle;
        transform.localEulerAngles = rot;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(damage);
        }
    }
}
