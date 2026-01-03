using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CascadingPlatform : MonoBehaviour
{
    [Header("Cascade Settings")]
    [SerializeField] private float delayBetweenFalls = 0.3f;
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float startDelay = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] private float blinkSpeed = 0.1f;
    [SerializeField] private int blinkCount = 3;

    private List<Transform> childWalls = new List<Transform>();
    private bool isActive = true;
    private bool cascadeStarted = false;
    private GameObject playerOnPlatform;
    private bool playerIsOnThisPlatform = false;

    void Start()
    {
        // Get all child walls
        foreach (Transform child in transform)
        {
            childWalls.Add(child);

            // Ensure each has Rigidbody2D
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            if (rb == null) rb = child.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActive && !cascadeStarted)
        {
            playerOnPlatform = collision.gameObject;
            playerIsOnThisPlatform = true;
            isActive = false;
            cascadeStarted = true;
            StartCoroutine(StartCascadeSequence());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsOnThisPlatform = false;
        }
    }

    IEnumerator StartCascadeSequence()
    {
        // Initial delay before cascade starts
        yield return new WaitForSeconds(startDelay);

        // Start cascading fall for each wall in order
        for (int i = 0; i < childWalls.Count; i++)
        {
            if (childWalls[i] != null)
            {
                StartCoroutine(FallWallSequence(childWalls[i], i));
                yield return new WaitForSeconds(delayBetweenFalls);
            }
        }
    }

    IEnumerator FallWallSequence(Transform wall, int wallIndex)
    {
        if (wall == null) yield break;

        SpriteRenderer spriteRenderer = wall.GetComponent<SpriteRenderer>();
        Rigidbody2D rb = wall.GetComponent<Rigidbody2D>();
        Collider2D wallCollider = wall.GetComponent<Collider2D>();

        if (rb == null) yield break;

        Color originalColor = Color.white;
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        // Step 1: Blink red before falling
        for (int blink = 0; blink < blinkCount; blink++)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(blinkSpeed);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(blinkSpeed);
            }
            else
            {
                yield return new WaitForSeconds(blinkSpeed * 2);
            }
        }

        // Step 2: If player is on THIS wall, make player fall too
        if (playerIsOnThisPlatform && playerOnPlatform != null)
        {
            Rigidbody2D playerRb = playerOnPlatform.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Make player fall a bit faster so they don't get stuck
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, -fallSpeed * 0.5f);
            }
        }

        // Step 3: Start falling
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallSpeed;
        rb.freezeRotation = true;

        // Step 4: Don't disable collider immediately - let player stay on it while falling
        // We'll disable it after a short delay
        StartCoroutine(DisableColliderAfterDelay(wallCollider, 0.2f));

        // Step 5: Fade out while falling
        float fallTimer = 0f;
        float fadeTime = 2f;

        while (fallTimer < fadeTime && wall != null)
        {
            fallTimer += Time.deltaTime;

            if (spriteRenderer != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, fallTimer / fadeTime);
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            // Keep applying downward force
            if (rb != null)
            {
                rb.AddForce(new Vector2(0f, -fallSpeed * 10f * Time.deltaTime), ForceMode2D.Force);

                // If player is still on platform, push them down with it
                if (playerIsOnThisPlatform && playerOnPlatform != null && fallTimer < 0.5f)
                {
                    Rigidbody2D playerRb = playerOnPlatform.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, rb.linearVelocity.y * 0.8f);
                    }
                }
            }

            yield return null;
        }

        // Destroy this wall after falling
        if (wall != null)
        {
            Destroy(wall.gameObject);
        }
    }

    IEnumerator DisableColliderAfterDelay(Collider2D collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}