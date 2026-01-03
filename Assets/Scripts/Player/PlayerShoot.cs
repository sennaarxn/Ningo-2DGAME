using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    PlayerControls controls;
    public Animator animator;

    [Header("Kunai Settings")]
    public GameObject Kunai;
    public Transform KunaiHole;
    public float speed = 10f;
    public float cooldownTime = 0.5f; // Customizable delay between shots

    private PlayerMovement playerMovement;
    private bool isActive = true;
    private bool canShoot = true; // Controls if player can shoot

    private void Awake()
    {
        controls = new PlayerControls();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Land.Shoot.performed += OnShootPerformed;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Land.Shoot.performed -= OnShootPerformed;
    }

    private void OnDestroy()
    {
        isActive = false;
        controls.Dispose();
    }

    private void OnShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (isActive && gameObject.activeInHierarchy && canShoot)
        {
            Fire();
            StartCooldown();
        }
    }

    void Fire()
    {
        // Check if animator is still valid
        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.SetTrigger("shoot");
        }

        bool isFacingRight = playerMovement != null ? playerMovement.isFacingRight : true;

        // Instantiate Kunai
        if (Kunai != null && KunaiHole != null)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, isFacingRight ? -90 : 90);
            GameObject go = Instantiate(Kunai, KunaiHole.position, rotation);

            if (go.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = new Vector2(isFacingRight ? speed : -speed, 0f);
                Destroy(go, 1.5f);
            }

            AudioManager.instance.Play("Kunai");
        }
    }

    private void StartCooldown()
    {
        canShoot = false;
        Invoke(nameof(ResetCooldown), cooldownTime); // Reset after cooldown
    }

    private void ResetCooldown()
    {
        canShoot = true; // Allow shooting again
    }
}