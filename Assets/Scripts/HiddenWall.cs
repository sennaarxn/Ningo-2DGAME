using UnityEngine;

public class HiddenWall : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D wallCollider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Make invisible & passable
            spriteRenderer.enabled = false;
            wallCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Make visible & solid again
            spriteRenderer.enabled = true;
            wallCollider.isTrigger = false;
        }
    }
}
