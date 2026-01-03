using UnityEngine;

public class DiamondPickup : MonoBehaviour
{
    [SerializeField] private int diamondValue = 1;
    private string id;

    private void Start()
    {
        // Get or create a unique ID for this diamond pickup
        id = GetComponent<CollectibleID>()?.collectibleID;

        if (string.IsNullOrEmpty(id))
        {
            // Create a unique ID
            id = "DiamondPickup_" + gameObject.name + "_" + transform.position.ToString();

            // Add CollectibleID component if missing
            if (GetComponent<CollectibleID>() == null)
            {
                CollectibleID collectibleID = gameObject.AddComponent<CollectibleID>();
                collectibleID.collectibleID = id;
            }
        }

        // Destroy if already collected (using ShouldShowItem which is the new method)
        if (CollectedTracker.ShouldShowItem(id) == false)  // CHANGED THIS LINE
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectDiamond();
        }
    }

    // For 2D games (if you need it)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectDiamond();
        }
    }

    private void CollectDiamond()
    {
        // Check if already collected (using ShouldShowItem)
        if (CollectedTracker.ShouldShowItem(id) == false)  // CHANGED THIS LINE
        {
            return;
        }

        // Mark as collected
        CollectedTracker.AddItem(id);

        // Add diamond to player manager
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.AddDiamond();
        }

        // Play sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("Diamond");
        }

        Destroy(gameObject);
    }

    // Public getter if other scripts need to know the value
    public int GetDiamondValue()
    {
        return diamondValue;
    }
}