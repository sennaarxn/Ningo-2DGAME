using UnityEngine;

public class Diamond : MonoBehaviour
{
    private string id;

    private void Start()
    {
        id = GetComponent<CollectibleID>().collectibleID;
        if (CollectedTracker.IsCollected(id)) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CollectedTracker.ShouldShowItem(id))
        {
            CollectedTracker.AddItem(id);
            if (PlayerManager.instance != null) PlayerManager.instance.AddDiamond();
            if (AudioManager.instance != null) AudioManager.instance.Play("Diamond");
            Destroy(gameObject);
        }
    }
}