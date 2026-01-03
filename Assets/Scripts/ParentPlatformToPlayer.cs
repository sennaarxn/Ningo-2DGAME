using UnityEngine;

public class ParentPlatformToPlayer : MonoBehaviour
{
    private int platformCount = 0;
    private Transform playerTransform;
    void Start()
    {
        playerTransform = transform;
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            platformCount++;
            if(gameObject.activeInHierarchy)
            playerTransform.parent = collision.transform;
        }
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platformCount--;
            
            if(platformCount <= 0)
            {
                
                platformCount = 0;

                if (gameObject.activeInHierarchy)
                    playerTransform.parent = null;
            }
        }
    }

}
