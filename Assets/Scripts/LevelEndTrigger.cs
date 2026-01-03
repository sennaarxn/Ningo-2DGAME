using UnityEngine;
using System.Collections;

public class LevelEndTrigger : MonoBehaviour
{
    private bool levelCompleted = false;

    [Header("UI")]
    public GameObject levelCompleteUI;

    [Header("Level Info")]
    public int currentLevel; // Set this in the inspector for each level

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!levelCompleted && collision.CompareTag("Player"))
        {
            levelCompleted = true;
            StartCoroutine(LevelCompleteSequence());
        }
    }

    private IEnumerator LevelCompleteSequence()
    {
        CompleteLevel();

        // Unlock the next level
        UnlockNextLevel(currentLevel);

        // Play audio
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("LevelFinish");
            StartCoroutine(AudioManager.instance.FadeOut("MainTheme", 1.5f));
        }

        yield break;
    }

    private void CompleteLevel()
    {
        // Save collected items
        if (CollectedTracker.instance != null)
        {
            CollectedTracker.SaveAllData();
        }

        // Save player currency
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.OnLevelEnd();
        }

        // Freeze the game
        Time.timeScale = 0;

        // Show level complete UI
        ShowLevelCompleteUI();
    }

    private void ShowLevelCompleteUI()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
        }
        else
        {
            //Debug.LogWarning("Level Complete UI is not assigned in the inspector.");
        }
    }

    private void UnlockNextLevel(int levelNumberJustFinished)
    {
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Only update if player progressed further
        if (levelNumberJustFinished >= currentUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelNumberJustFinished + 1);
            PlayerPrefs.Save();
           // Debug.Log("Next level unlocked: " + (levelNumberJustFinished + 1));
        }
    }
}
