using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [Header("References")]
    public GameObject pauseMenuPanel;          // Pause menu panel
    public GameObject settingsPanel;           // NEW settings panel
    public GameObject gameOverPanel;           // Optional
    public GameObject levelCompletedPanel;     // Optional

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            bool shouldHide = false;

            if (pauseMenuPanel != null)
                shouldHide = shouldHide || pauseMenuPanel.activeInHierarchy;

            if (settingsPanel != null)
                shouldHide = shouldHide || settingsPanel.activeInHierarchy;

            if (gameOverPanel != null)
                shouldHide = shouldHide || gameOverPanel.activeInHierarchy;

            if (levelCompletedPanel != null)
                shouldHide = shouldHide || levelCompletedPanel.activeInHierarchy;

            gameObject.SetActive(!shouldHide);
        }
    }

    // Pause button clicked
    public void OnPauseButtonClicked()
    {
        pauseMenuPanel.SetActive(true);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    // Resume button clicked
    public void OnResumeClicked()
    {
        pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        gameObject.SetActive(true);
        Time.timeScale = 1;
    }

    // ---------- NEW SETTINGS FUNCTIONS ----------

    // Open settings from pause menu
    public void OnSettingsClicked()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    // Back button inside settings
    public void OnBackFromSettingsClicked()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }
}
