using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool alreadyActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!alreadyActivated && collision.transform.tag == "Player")
        {
            alreadyActivated = true;
            PlayerManager.lastCheckPointPos = transform.position;
            GetComponent<SpriteRenderer>().color = Color.white;

            if (AudioManager.instance != null) AudioManager.instance.Play("CheckPoints");
            if (PlayerManager.instance != null) PlayerManager.instance.SaveCheckpointState();
            if (CollectedTracker.instance != null) CollectedTracker.SaveCheckpointState();
        }
    }
}