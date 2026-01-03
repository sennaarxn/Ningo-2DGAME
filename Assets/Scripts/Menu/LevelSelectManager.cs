using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    [Header("Optional Reset Button")]
    public Button resetButton; // assign your Reset Progress button here

    void Start()
    {
        UpdateLevelButtons();

        // Optional: setup Reset Button
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetProgress);
        }
    }

    // Update buttons interactable based on unlocked levels
    public void UpdateLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        level1Button.interactable = unlockedLevel >= 1;
        level2Button.interactable = unlockedLevel >= 2;
        level3Button.interactable = unlockedLevel >= 3;
    }

    public void OpenLevel(string levelName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(levelName);
    }

    // Reset all progress
    private void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        //Debug.Log("Progress reset. Only Level 1 is unlocked.");

        // Update buttons immediately
        UpdateLevelButtons();
    }
}
