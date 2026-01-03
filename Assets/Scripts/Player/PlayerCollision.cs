using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    public bool isInvincible = false;

    // Add reference to SpriteRenderer for red blink effect
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        // Get SpriteRenderer component for blink effect
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnEnable()
    {
        // Re-enable collisions between player and enemies on respawn
        Physics2D.IgnoreLayerCollision(6, 8, false);

        // Reset color when player is enabled
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle instant death
        if (collision.transform.CompareTag("Death"))
        {
            HealthManager.health = 0; // 🟥 Set health to 0 to empty all hearts
            HandlePlayerDeath();

            // Disable movement
            var movementScript = GetComponent<PlayerMovement>();
            if (movementScript != null)
                movementScript.enabled = false;

            // Fix: delay destruction to avoid transform parenting error
            StartCoroutine(DestroyPlayerNextFrame());
            return;
        }

        // Handle normal enemy damage
        if (collision.transform.tag == "Enemy")
        {
            TakeDamage();
        }
    }

    IEnumerator GetHurt()
    {
        Physics2D.IgnoreLayerCollision(6, 8);
        Animator animator = GetComponent<Animator>();
        animator.SetLayerWeight(1, 1);
        isInvincible = true;

        // Add red blink effect
        StartCoroutine(BlinkRed());

        yield return new WaitForSeconds(0.7f);
        isInvincible = false;
        animator.SetLayerWeight(1, 0);
        Physics2D.IgnoreLayerCollision(6, 8, false);

        // Ensure color is reset after blinking
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    // New coroutine for red blinking effect
    IEnumerator BlinkRed()
    {
        if (spriteRenderer == null) yield break;

        float blinkDuration = 0.7f;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;
        bool isRed = false;

        while (elapsedTime < blinkDuration)
        {
            // Toggle between red and original color
            spriteRenderer.color = isRed ? originalColor : Color.red;
            isRed = !isRed;

            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }
    }

    public void TakeDamage()
    {
        HealthManager.health--;

        if (HealthManager.health <= 0)
        {
            HandlePlayerDeath();

            // Fix: delay deactivation to prevent parenting conflict
            StartCoroutine(DisablePlayerNextFrame());
        }
        else
        {
            StartCoroutine(GetHurt());
        }
    }

    // ✅ NEW METHOD: Handle player death
    private void HandlePlayerDeath()
    {
        PlayerManager.isGameOver = true;

        // ✅ Notify PlayerManager about death
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.OnPlayerDeath();
        }

        // Audio
        if (AudioManager.instance != null)
        {
            AudioManager.instance.FadeOut("MainTheme", 1.5f);
            AudioManager.instance.Play("GameOver");
        }
    }

    IEnumerator DisablePlayerNextFrame()
    {
        yield return null; // wait one frame
        gameObject.SetActive(false);
    }

    IEnumerator DestroyPlayerNextFrame()
    {
        yield return null; // wait one frame
        Destroy(gameObject);
    }
}