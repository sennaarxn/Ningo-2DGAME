using UnityEngine;

public class MainMenuCharacter : MonoBehaviour
{
    public GameObject[] characterModels;

    void Start()
    {
        DisplaySelectedCharacter();
    }

    public void DisplaySelectedCharacter()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Hide all models
        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i] != null)
                characterModels[i].SetActive(false);
        }

        // Show selected model
        if (selectedIndex < characterModels.Length && characterModels[selectedIndex] != null)
        {
            characterModels[selectedIndex].SetActive(true);
        }
    }
}