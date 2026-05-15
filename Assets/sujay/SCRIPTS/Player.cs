using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float mouseSensitivity = 2f;
    public float pickUpRange = 3f;
    public Transform gunHoldPoint;

    [Header("Audio - Drag & Drop")]
    public AudioClip walkSound;
    public AudioClip sprintSound;
    public AudioClip damageSound;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;
    public Image healthFillImage;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public RawImage damageOverlay;
    public float damageOverlayDuration = 0.2f;

    [Header("Dash")]
    public float dashDistance = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public KeyCode dashKey = KeyCode.K;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private Gun currentGun;
    private CharacterController controller;
    private float verticalRotation = 0f;
    private AudioSource audioSource;
    private bool isSprinting = false;
    private Vector3 lastPosition;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    private AudioSource sfxSource;
    private float overlayTimer = 0f;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentHealth = maxHealth;
        UpdateHealthBar();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        lastPosition = transform.position;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        if (damageOverlay != null)
        {
            Color c = damageOverlay.color;
            c.a = 0f;
            damageOverlay.color = c;
        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthBar();

        if (damageSound != null)
            sfxSource.PlayOneShot(damageSound);

        if (damageOverlay != null)
        {
            Color c = damageOverlay.color;
            c.a = 1f;
            damageOverlay.color = c;
            overlayTimer = damageOverlayDuration;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }

        if (healthFillImage != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            healthFillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }

        Debug.Log("[Player] Health: " + currentHealth + "/" + maxHealth);
    }

    public void TestDamage()
    {
        TakeDamage(10);
    }

    public void TestHeal()
    {
        currentHealth += 10;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Die()
    {
        Debug.Log("[Player] Died! Loading death scene...");
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("death");
    }

    void Update()
    {
        HandleDamageOverlay();
        HandleSprint();
        if (HandleDash()) return;
        MovePlayer();
        LookAround();
        HandlePickup();
        HandleDrop();
        HandleTestKeys();
        HandleMovementSound();
    }

    void HandleDamageOverlay()
    {
        if (damageOverlay != null && overlayTimer > 0f)
        {
            overlayTimer -= Time.deltaTime;
            Color c = damageOverlay.color;
            c.a = overlayTimer / damageOverlayDuration;
            damageOverlay.color = c;
        }
    }

    void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
    }

    bool HandleDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            controller.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);
            if (dashTimer <= 0f)
                isDashing = false;
            return true;
        }

        if (Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0f)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            dashDirection = (transform.right * h + transform.forward * v).normalized;
            if (dashDirection.magnitude < 0.1f)
                dashDirection = transform.forward;

            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            Debug.Log("[Player] Dash!");
        }

        return false;
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickUpRange);
            foreach (Collider col in colliders)
            {
                Gun gun = col.GetComponent<Gun>();
                if (gun != null && currentGun == null)
                {
                    PickUpGun(gun);
                    break;
                }

                // No inventory items - removed inventory system
            }
        }
    }

    void HandleDrop()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentGun != null)
        {
            DropGun();
        }
    }

    void HandleTestKeys()
    {
        if (Input.GetKeyDown(KeyCode.T)) TestDamage();
        if (Input.GetKeyDown(KeyCode.H)) TestHeal();
    }

    void HandleMovementSound()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        if (!isMoving)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            return;
        }

        AudioClip targetClip = isSprinting ? sprintSound : walkSound;

        if (targetClip == null)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            return;
        }

        if (!audioSource.isPlaying || audioSource.clip != targetClip)
        {
            audioSource.clip = targetClip;
            audioSource.Play();
        }
    }

    void PickUpGun(Gun gun)
    {
        currentGun = gun;
        gun.PickUp(transform, gunHoldPoint);
    }

    void DropGun()
    {
        if (currentGun != null)
        {
            currentGun.Drop();
            currentGun = null;
        }
    }
}
