using UnityEngine;

public class CollectibleID : MonoBehaviour
{
    public string collectibleID = "";

    private void Awake()
    {
        // Generate a unique ID if none is set
        if (string.IsNullOrEmpty(collectibleID))
        {
            collectibleID = gameObject.name + "_" + transform.position.ToString();
        }
    }
}