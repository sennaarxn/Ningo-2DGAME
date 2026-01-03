using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    void Start()
    {
        // Setup exit button
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ShowExitPanel);
        }

        // Setup yes button
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(ConfirmExit);
        }

        // Setup no button
        {
            noButton.onClick.AddListener(HideExitPanel);
        }

        // Hide panel at start
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Optional: Escape key support
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitConfirmationPanel != null && exitConfirmationPanel.activeSelf)
            {
                HideExitPanel();
            }
            else
            {
                ShowExitPanel();
            }
        }
    }

    void ShowExitPanel()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }

    void HideExitPanel()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game
        }
    }

    void ConfirmExit()
    {
        Debug.Log("Game exiting...");

        // Resume time before exiting
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}