using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovePlatformLeftRight : MonoBehaviour
{
    public float moveDistance = 10f;
    public float moveSpeed = 2f;
    public bool startOnTouch = false;  // Toggle this in Inspector

    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingToEnd = true;
    private bool isActivated = false;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + Vector3.right * moveDistance;

        // Activate immediately if not waiting for touch
        if (!startOnTouch)
        {
            isActivated = true;
        }
    }

    void FixedUpdate()
    {
        if (!isActivated)
            return;

        Vector3 targetPos = movingToEnd ? endPos : startPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (transform.position == targetPos)
        {
            movingToEnd = !movingToEnd;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActivated && startOnTouch && collision.collider.CompareTag("Player"))
        {
            isActivated = true;
        }
    }
}
