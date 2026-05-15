using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Settings")]
    public float openAngle = 90f;
    public float smooth = 2f;
    public KeyCode interactionKey = KeyCode.E;
    public float interactionDistance = 3f;

    private bool isOpen = false;
    private bool playerInRange = false;
    private Quaternion defaultRotation;
    private Quaternion openRotation;

    void Start()
    {
        defaultRotation = transform.localRotation;
        openRotation = defaultRotation * Quaternion.Euler(0, openAngle, 0);

        // Ensure there's a trigger collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning("DoorController needs a Collider! Adding a BoxCollider as trigger.");
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(interactionDistance, 2, interactionDistance);
        }
        else if (!GetComponent<Collider>().isTrigger)
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            isOpen = !isOpen;
        }

        // Smoothly rotate the door
        Quaternion targetRotation = isOpen ? openRotation : defaultRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player near door. Press " + interactionKey + " to open/close.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
