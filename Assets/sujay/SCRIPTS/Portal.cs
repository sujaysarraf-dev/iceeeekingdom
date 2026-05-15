using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sujay
{
    public class Portal : MonoBehaviour
    {
        [Header("Target Scene")]
        public string sceneName = "boss";

        [Header("Settings")]
        public bool useManualActivation = false;
        public KeyCode manualActivateKey = KeyCode.E;
        public float requiredTime = 0f;

        [Header("Optional")]
        public GameObject portalEffect;

        private bool playerInPortal = false;
        private float timer = 0f;
        private GameObject player;

        void Start()
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogError("[Portal] No Collider found on " + gameObject.name + "! Add a Collider and set it to Trigger.");
                return;
            }

            if (!col.isTrigger)
            {
                col.isTrigger = true;
                Debug.Log("[Portal] Auto-set Collider to Is Trigger = true on " + gameObject.name);
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[Portal] sceneName is empty on " + gameObject.name + "! Fill it in the Inspector.");
            }
            else
            {
                Debug.Log("[Portal] Ready on " + gameObject.name + " — will load scene: \"" + sceneName + "\"");
            }
        }

        void Update()
        {
            if (!playerInPortal) return;

            if (useManualActivation)
            {
                if (Input.GetKeyDown(manualActivateKey))
                {
                    Debug.Log("[Portal] Manual activation key pressed");
                    TeleportToScene();
                }
                return;
            }

            timer += Time.deltaTime;
            if (timer >= requiredTime)
            {
                TeleportToScene();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("[Portal] Trigger entered by: " + other.name + " (tag: " + other.tag + ")");
            if (other.CompareTag("Player"))
            {
                player = other.gameObject;
                playerInPortal = true;
                timer = 0f;
                Debug.Log("[Portal] Player entered portal — will teleport to \"" + sceneName + "\"");
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInPortal = false;
                timer = 0f;
                player = null;
                Debug.Log("[Portal] Player left portal");
            }
        }

        void TeleportToScene()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[Portal] Cannot teleport — sceneName is empty!");
                return;
            }

            if (portalEffect != null)
                Instantiate(portalEffect, transform.position, Quaternion.identity);

            Debug.Log("[Portal] Loading scene: \"" + sceneName + "\"");
            SceneManager.LoadScene(sceneName);
        }

        void OnDestroy()
        {
            playerInPortal = false;
            timer = 0f;
        }
    }
}
