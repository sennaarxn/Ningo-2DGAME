using UnityEngine;

public class Coin : MonoBehaviour
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
            if (PlayerManager.instance != null) PlayerManager.instance.AddCoin();
            if (AudioManager.instance != null) AudioManager.instance.Play("Coins");
            Destroy(gameObject);
        }
    }
}