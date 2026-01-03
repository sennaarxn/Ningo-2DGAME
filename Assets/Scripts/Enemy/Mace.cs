using UnityEngine;

public class Mace : MonoBehaviour
{
    public enum MovementDirection { Vertical, Horizontal }
    public MovementDirection moveDirection = MovementDirection.Vertical;

    public float speed = 0.8f;
    public float range = 3f;

    private Vector3 startPos;
    private int dir = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        if (moveDirection == MovementDirection.Vertical)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime * dir);
            if (transform.position.y < startPos.y || transform.position.y > startPos.y + range)
                dir *= -1;
        }
        else if (moveDirection == MovementDirection.Horizontal)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime * dir);
            if (transform.position.x < startPos.x || transform.position.x > startPos.x + range)
                dir *= -1;
        }
    }
}
