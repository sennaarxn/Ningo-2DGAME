using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Portal : MonoBehaviour
{
    public Transform destination;
    public GameObject promptUI;

    [Header("Fade Durations")]
    [Tooltip("How long it takes to fade to black (seconds)")]
    [Range(0.1f, 3f)]
    [SerializeField] private float fadeOutTime = 0.3f;

    [Tooltip("How long it takes to fade back in (seconds)")]
    [Range(0.1f, 3f)]
    [SerializeField] private float fadeInTime = 0.3f;

    [Tooltip("How long the screen stays black during teleport (seconds)")]
    [Range(0f, 2f)]
    [SerializeField] private float blackScreenTime = 0.2f;

    [Space(10)]
    [Header("Cooldown")]
    [Tooltip("Time before player can use portal again (seconds)")]
    [SerializeField] private float teleportCooldown = 2f;

    private bool playerIsInside = false;
    private bool canTeleport = true;
    private Image fadePanel;

    // Cached player references
    private GameObject player;
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;

    void Start()
    {
        CreateFadePanel();
        CachePlayerReferences();
    }

    void Update()
    {
        if (playerIsInside && Input.GetKeyDown(KeyCode.E) && canTeleport)
        {
            StartCoroutine(TeleportWithCooldown());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInside = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInside = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    IEnumerator TeleportWithCooldown()
    {
        if (!canTeleport) yield break;

        canTeleport = false;

        // Refresh player references
        if (player == null || playerMovement == null || playerRb == null)
        {
            CachePlayerReferences();
        }

        if (player == null || destination == null)
        {
            Debug.LogError("Portal: Player or destination is null!");
            canTeleport = true;
            yield break;
        }

        // ⭐ PLAY SOUND IMMEDIATELY WHEN E IS PRESSED ⭐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("Teleport");
        }

        // Temporarily disable this portal's collider
        Collider2D portalCollider = GetComponent<Collider2D>();
        if (portalCollider != null)
        {
            portalCollider.enabled = false;
        }

        // Disable prompt and trigger state
        if (promptUI != null) promptUI.SetActive(false);
        playerIsInside = false;

        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.DisableMovement();
        }

        // Store original velocity
        Vector2 originalVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;

        // Freeze Rigidbody
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        // Fade to black - using customizable fadeOutTime
        yield return StartCoroutine(Fade(0f, 1f, fadeOutTime));

        // Teleport player instantly (no delay here)
        player.transform.position = destination.position;

        // Brief pause at black screen - using customizable blackScreenTime
        yield return new WaitForSeconds(blackScreenTime);

        // Fade from black - using customizable fadeInTime
        yield return StartCoroutine(Fade(1f, 0f, fadeInTime));

        // Restore velocity
        if (playerRb != null)
        {
            playerRb.linearVelocity = originalVelocity;
        }

        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.EnableMovement();
        }

        // Re-enable portal collider after delay
        if (portalCollider != null)
        {
            StartCoroutine(EnableColliderAfterDelay(portalCollider, 1f));
        }

        // Wait before allowing next teleport - using customizable teleportCooldown
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }

    IEnumerator EnableColliderAfterDelay(Collider2D collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadePanel == null)
        {
            Debug.LogError("Portal: Fade panel is null!");
            yield break;
        }

        fadePanel.gameObject.SetActive(true);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);

            Color color = fadePanel.color;
            color.a = alpha;
            fadePanel.color = color;

            yield return null;
        }

        // Set final alpha
        Color finalColor = fadePanel.color;
        finalColor.a = endAlpha;
        fadePanel.color = finalColor;

        fadePanel.gameObject.SetActive(endAlpha > 0.01f);
    }

    void CreateFadePanel()
    {
        GameObject fadeObject = GameObject.Find("PortalFadePanel");

        if (fadeObject == null)
        {
            // Create new Canvas for fade effect
            GameObject canvasObject = new GameObject("PortalFadeCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // Create fade panel
            fadeObject = new GameObject("PortalFadePanel");
            fadeObject.transform.SetParent(canvasObject.transform);

            fadePanel = fadeObject.AddComponent<Image>();
            RectTransform rect = fadeObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Set to black
            fadePanel.color = new Color(0f, 0f, 0f, 0f);

            fadeObject.SetActive(false);
        }
        else
        {
            fadePanel = fadeObject.GetComponent<Image>();
            if (fadePanel == null)
            {
                fadePanel = fadeObject.AddComponent<Image>();
            }
            fadePanel.color = new Color(0f, 0f, 0f, 0f);
            fadeObject.SetActive(false);
        }
    }

    void CachePlayerReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }
}