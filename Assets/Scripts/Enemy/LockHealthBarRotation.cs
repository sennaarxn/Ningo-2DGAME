using UnityEngine;

public class LockHealthBarRotation : MonoBehaviour
{
    private Quaternion fixedRotation;

    void Start()
    {
        // Lock it to its current rotation in world space
        fixedRotation = Quaternion.Euler(0, 0, 0); // Or Camera.main.transform.rotation for facing camera
    }

    void LateUpdate()
    {
        transform.rotation = fixedRotation;
    }
}
