using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    [Header("Characters & Skins")]
    public GameObject[] skins;
    public Character[] characters;
    private int selectedCharacter; // Currently viewing
    private int currentlySelectedCharacter; // Actually selected

    [Header("Main Menu Connection")]
    public MainMenuCharacter mainMenuCharacter;

    [Header("UI Elements")]
    public Button unlockButton;
    public TextMeshProUGUI coinsText;
    public Button selectButton;
    public TextMeshProUGUI selectButtonText;

    [Header("Reset Button")]
    public Button resetButton;

    // This runs when shop opens
    void OnEnable()
    {
        // Reset to show selected character when shop opens
        currentlySelectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        selectedCharacter = currentlySelectedCharacter;

        // Update display
        UpdateCharacterDisplay();
        UpdateUI();
    }

    private void Start()
    {
        InitializeCharacterSelect();
        if (resetButton != null) resetButton.onClick.AddListener(ResetForTesting);

        if (mainMenuCharacter == null)
        {
            mainMenuCharacter = FindAnyObjectByType<MainMenuCharacter>();
        }
    }

    private void InitializeCharacterSelect()
    {
        currentlySelectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        selectedCharacter = currentlySelectedCharacter;

        UpdateCharacterDisplay();

        // Load unlock status
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].price == 0)
                characters[i].isUnlocked = true; // Free characters
            else
                characters[i].isUnlocked = PlayerPrefs.GetInt("Unlocked_" + characters[i].name, 0) == 1;
        }

        UpdateUI();
    }

    private void UpdateCharacterDisplay()
    {
        // Hide all skins
        foreach (GameObject skin in skins)
            if (skin != null) skin.SetActive(false);

        // Show current character
        if (selectedCharacter < skins.Length && skins[selectedCharacter] != null)
            skins[selectedCharacter].SetActive(true);
    }

    public void ChangeNext()
    {
        if (skins.Length == 0) return;

        // Move to next
        selectedCharacter = (selectedCharacter + 1) % skins.Length;
        UpdateCharacterDisplay();
        UpdateUI();
    }

    public void ChangePrevious()
    {
        if (skins.Length == 0) return;

        // Move to previous
        selectedCharacter--;
        if (selectedCharacter < 0) selectedCharacter = skins.Length - 1;
        UpdateCharacterDisplay();
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (SaveManager.Instance == null || coinsText == null) return;

        // Update coins
        coinsText.text = SaveManager.Instance.TotalCoins.ToString();

        if (selectedCharacter >= characters.Length) return;

        Character currentChar = characters[selectedCharacter];

        if (currentChar.isUnlocked)
        {
            // Character is unlocked
            unlockButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);

            // Check if this is the selected character
            if (selectedCharacter == currentlySelectedCharacter)
            {
                selectButton.interactable = false;
                if (selectButtonText != null) selectButtonText.text = "SELECTED";
            }
            else
            {
                selectButton.interactable = true;
                if (selectButtonText != null) selectButtonText.text = "SELECT";
            }
        }
        else
        {
            // Character is locked
            unlockButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);

            // Set price
            TextMeshProUGUI unlockText = unlockButton.GetComponentInChildren<TextMeshProUGUI>();
            if (unlockText != null) unlockText.text = currentChar.price.ToString();

            // Check if affordable
            unlockButton.interactable = SaveManager.Instance.TotalCoins >= currentChar.price;
        }
    }

    public void Unlock()
    {
        if (SaveManager.Instance == null || selectedCharacter >= characters.Length) return;

        Character currentChar = characters[selectedCharacter];
        if (currentChar.isUnlocked || SaveManager.Instance.TotalCoins < currentChar.price) return;

        // Purchase
        SaveManager.Instance.AddCoins(-currentChar.price);
        currentChar.isUnlocked = true;
        PlayerPrefs.SetInt("Unlocked_" + currentChar.name, 1);
        PlayerPrefs.Save();

        UpdateUI();
    }

    public void SelectCharacter()
    {
        if (selectedCharacter >= characters.Length) return;

        Character currentChar = characters[selectedCharacter];
        if (!currentChar.isUnlocked) return;

        // Save selection
        currentlySelectedCharacter = selectedCharacter;
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
        PlayerPrefs.Save();

        // Update main menu
        if (mainMenuCharacter != null)
        {
            mainMenuCharacter.DisplaySelectedCharacter();
        }

        UpdateUI();
    }

    public void OnSelectButtonClicked()
    {
        SelectCharacter();
    }

    public void ResetForTesting()
    {
        PlayerPrefs.DeleteAll();

        // REMOVED: Give coins line - No more test coins
        // if (SaveManager.Instance != null)
        //     SaveManager.Instance.AddCoins(9999);

        // Reset unlocks (first character free)
        for (int i = 0; i < characters.Length; i++)
        {
            if (i == 0 || characters[i].price == 0)
            {
                characters[i].isUnlocked = true;
                PlayerPrefs.SetInt("Unlocked_" + characters[i].name, 1);
            }
            else
            {
                characters[i].isUnlocked = false;
                PlayerPrefs.SetInt("Unlocked_" + characters[i].name, 0);
            }
        }

        // Reset to first character
        currentlySelectedCharacter = 0;
        selectedCharacter = 0;
        PlayerPrefs.SetInt("SelectedCharacter", 0);
        PlayerPrefs.Save();

        // Update display
        UpdateCharacterDisplay();

        // Update main menu
        if (mainMenuCharacter != null)
        {
            mainMenuCharacter.DisplaySelectedCharacter();
        }

        UpdateUI();
    }
}