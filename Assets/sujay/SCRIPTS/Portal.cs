using UnityEngine;
namespace Sujay
{
    public class Portal : MonoBehaviour
    {
        [Header("Settings")]
        public Transform teleportTarget;
        public float requiredTime = 3f;
        public KeyCode manualActivateKey = KeyCode.E;

        [Header("Optional")]
        public bool useManualActivation = false;
        public GameObject portalEffect;

        private bool playerInPortal = false;
        private float timer = 0f;
        private GameObject player;
        private CharacterController playerController;

        void Start()
        {
            if (teleportTarget == null)
                Debug.LogWarning("[Portal] No teleport target set! Assign a target Transform in Inspector.");

            Collider col = GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                col.isTrigger = true;
                Debug.Log("[Portal] Auto-set Collider to Is Trigger = true");
            }

            Debug.Log("[Portal] Initialized. Required time: " + requiredTime + "s. Manual activation: " + useManualActivation);
        }

        void Update()
        {
            if (useManualActivation && playerInPortal && Input.GetKeyDown(manualActivateKey))
            {
                TeleportPlayer();
            }

            if (playerInPortal && !useManualActivation)
            {
                timer += Time.deltaTime;

                if (timer >= requiredTime)
                {
                    TeleportPlayer();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.gameObject;
                playerController = player.GetComponent<CharacterController>();
                playerInPortal = true;
                timer = 0f;

                Debug.Log("[Portal] Player entered portal. Stand for " + requiredTime + "s to teleport.");
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInPortal = false;
                timer = 0f;
                player = null;
                playerController = null;

                Debug.Log("[Portal] Player left portal. Timer reset.");
            }
        }

        void TeleportPlayer()
        {
            if (player == null || teleportTarget == null)
            {
                Debug.LogWarning("[Portal] Cannot teleport: Player or Target is null!");
                return;
            }

            if (playerController != null)
                playerController.enabled = false;

            player.transform.position = teleportTarget.position;
            player.transform.rotation = teleportTarget.rotation;

            if (playerController != null)
                playerController.enabled = true;

            if (portalEffect != null)
                Instantiate(portalEffect, teleportTarget.position, Quaternion.identity);

            Debug.Log("[Portal] Player teleported to " + teleportTarget.name + "!");

            playerInPortal = false;
            timer = 0f;
            player = null;
            playerController = null;
        }

        void OnDestroy()
        {
            playerInPortal = false;
            timer = 0f;
        }
    }
}

