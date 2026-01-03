using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Settings")] public KeyCode interactKey = KeyCode.E;
    public bool isOpened = false;
    [Header("Diamond Settings")] public GameObject diamondPrefab;
    public Vector3 diamondOffset = new Vector3(0, 1f, 0);
    public Vector2 diamondLaunchForce = new Vector2(0, 5f);
    public float diamondDamping = 1.8f;
    [Header("UI Settings")] public GameObject interactUI;

    private Animator animator;
    private string id;
    private bool playerInRange = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        id = GetComponent<CollectibleID>().collectibleID;

        if (CollectedTracker.IsCollected(id))
        {
            isOpened = true;
            animator.Play("Chest", 0, 0.99f);
            animator.Update(0);
            this.enabled = false;
            if (interactUI != null) interactUI.SetActive(false);
            return;
        }

        if (interactUI != null) interactUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !isOpened && Input.GetKeyDown(interactKey)) OpenChest();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI != null && !isOpened) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    void OpenChest()
    {
        isOpened = true;
        animator.SetTrigger("Open");
        if (AudioManager.instance != null) AudioManager.instance.Play("Chest");
        if (interactUI != null) interactUI.SetActive(false);
        CollectedTracker.AddItem(id);

        if (diamondPrefab != null)
        {
            GameObject diamondEffect = Instantiate(diamondPrefab, transform.position + diamondOffset, Quaternion.identity);
            Rigidbody2D rb = diamondEffect.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0.8f;
                rb.linearVelocity = new Vector2(0, 2f);
                rb.linearDamping = diamondDamping;
                Destroy(diamondEffect, 2f);
            }
        }

        this.enabled = false;
    }
}