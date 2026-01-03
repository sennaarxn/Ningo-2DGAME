using UnityEngine;

public class Kunai : MonoBehaviour
{
    [Header("Range Settings")]
    public float maxDistance = 10f; // Adjustable in Inspector
    private Vector2 spawnPosition;

    private void Start()
    {
        spawnPosition = transform.position; // Record where the kunai was spawned
    }

    private void Update()
    {
        // Calculate distance traveled
        float distanceTraveled = Vector2.Distance(spawnPosition, transform.position);

        // Destroy kunai if it exceeds max distance
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // if (collision.CompareTag("Enemy"))
       // {
            //Destroy(collision.gameObject); // Destroy enemy
            //Destroy(gameObject); // Destroy kunai
       // }
        if (collision.CompareTag("Minotaur"))
        {
            Minotaur minotaur = collision.GetComponent<Minotaur>();
            if (minotaur != null)
            {
                minotaur.TakeDamage(25); // Deal damage
            }
            Destroy(gameObject); // Destroy kunai
        }
        else if (!collision.CompareTag("Player") && !collision.CompareTag("Kunai"))
        {
            Destroy(gameObject); // Destroy kunai on collision with walls/objects
        }
    }
}