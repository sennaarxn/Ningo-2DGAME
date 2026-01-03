using UnityEngine;

public class SawMovementDistance : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float move = Mathf.PingPong(Time.time * speed, moveDistance);
        transform.position = startPos + Vector3.right * move;
    }
}
