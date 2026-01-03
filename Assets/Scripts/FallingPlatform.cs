using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float destroyDelay = 2f;

    private bool falling = false;

    [SerializeField] private Rigidbody2D rb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (falling)
            return;

        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(StartFall(collision.collider));
        }
    }

    private IEnumerator StartFall(Collider2D playerCollider)
    {
        falling = true;

        yield return new WaitForSeconds(fallDelay);

        rb.bodyType = RigidbodyType2D.Dynamic;

        // Ignore collision between player and platform before destroying
        Collider2D platformCollider = GetComponent<Collider2D>();
        if (platformCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, platformCollider);
        }

        Destroy(gameObject, destroyDelay);
    }
}
