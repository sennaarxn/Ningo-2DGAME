using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovePlatformUpDown : MonoBehaviour
{
    public float moveDistance = 10f;
    public float moveSpeed = 2f;
    public bool moveUp = true; // Set this in Inspector to control initial direction

    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingToEnd = true;

    void Start()
    {
        startPos = transform.position;

        // Decide direction based on moveUp setting
        if (moveUp)
            endPos = startPos + Vector3.up * moveDistance;
        else
            endPos = startPos - Vector3.up * moveDistance;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = movingToEnd ? endPos : startPos;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (transform.position == targetPos)
        {
            movingToEnd = !movingToEnd;
        }
    }
}
