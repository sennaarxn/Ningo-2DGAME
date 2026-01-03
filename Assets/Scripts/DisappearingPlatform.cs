using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Timing Settings")]
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float totalDuration = 3f;

    [Header("Blink & Fall Settings")]
    [SerializeField] private float blinkSpeed = 0.15f;
    [SerializeField] private float fallSpeed = 0.5f;

    private SpriteRenderer[] childSprites;
    private Color[] originalColors;
    private Rigidbody2D[] childRigidbodies;
    private bool isActive = true;
    private bool playerTouched = false;
    private float platformTimer = 0f;

    void Start()
    {
        childSprites = GetComponentsInChildren<SpriteRenderer>();
        childRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        originalColors = new Color[childSprites.Length];

        for (int i = 0; i < childSprites.Length; i++)
        {
            originalColors[i] = childSprites[i].color;
        }

        foreach (Rigidbody2D rb in childRigidbodies)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActive && !playerTouched)
        {
            playerTouched = true;
            StartCoroutine(PlatformSequence());
        }
    }

    IEnumerator PlatformSequence()
    {
        isActive = false;
        platformTimer = 0f;

        yield return new WaitForSeconds(startDelay);
        StartCoroutine(BlinkAndFallSequence());
    }

    IEnumerator BlinkAndFallSequence()
    {
        float blinkStartTime = platformTimer;
        float fallStartTime = platformTimer;
        bool isFalling = false;
        float fallStartDelay = 1f;

        while (platformTimer < totalDuration)
        {
            platformTimer += Time.deltaTime;

            bool isRed = (Mathf.FloorToInt((platformTimer - blinkStartTime) / blinkSpeed) % 2) == 0;

            if (isRed)
            {
                foreach (SpriteRenderer sprite in childSprites)
                {
                    sprite.color = Color.red;
                }
            }
            else
            {
                for (int i = 0; i < childSprites.Length; i++)
                {
                    childSprites[i].color = originalColors[i];
                }
            }

            if (!isFalling && (platformTimer - fallStartTime) >= fallStartDelay)
            {
                StartFalling();
                isFalling = true;
            }

            if (isFalling)
            {
                float fallProgress = (platformTimer - (fallStartTime + fallStartDelay)) /
                                    (totalDuration - startDelay - fallStartDelay);
                float alpha = Mathf.Lerp(1f, 0f, fallProgress);

                foreach (SpriteRenderer sprite in childSprites)
                {
                    Color color = sprite.color;
                    color.a = alpha;
                    sprite.color = color;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    void StartFalling()
    {
        foreach (Rigidbody2D rb in childRigidbodies)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = fallSpeed;
            rb.freezeRotation = true;
        }

        // Ignore collisions so player can fall through
        // But DON'T disable colliders - death zone needs them!
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                foreach (Collider2D platformCollider in GetComponentsInChildren<Collider2D>())
                {
                    Physics2D.IgnoreCollision(playerCollider, platformCollider);
                }
            }
        }
    }
}