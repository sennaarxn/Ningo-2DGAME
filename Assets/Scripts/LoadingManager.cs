using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;

public class LoadingManager : MonoBehaviour
{
    public string sceneToLoad = "Menu";
    public Slider progressBar;
    public TMP_Text percentText;
    public TMP_Text loadingDotsText;

    public float loadingDuration = 6f;

    public AudioSource loadingMusic; // 🎵 add this

    void Start()
    {
        // FIXED: Only reset if version changed
        CheckAndResetGameData();

        // play music only in loading scene
        if (loadingMusic != null)
            loadingMusic.Play();

        StartCoroutine(LoadAsync());
    }

    // FIXED: Only resets when version changes
    void CheckAndResetGameData()
    {
        string currentVersion = Application.version;
        string savedVersion = PlayerPrefs.GetString("GameVersion", "0.0");

        Debug.Log($"Current Version: {currentVersion}, Saved Version: {savedVersion}");

        // ONLY RESET IF VERSIONS ARE DIFFERENT
        if (savedVersion != currentVersion)
        {
            Debug.Log("=== VERSION CHANGED - RESETTING GAME DATA ===");
            ResetAllGameData();
        }
        else
        {
            Debug.Log("=== SAME VERSION - KEEPING PROGRESS ===");
            Debug.Log("Coins: " + PlayerPrefs.GetInt("Coins", 0));
            Debug.Log("Level: " + PlayerPrefs.GetInt("CurrentLevel", 1));
        }
    }

    void ResetAllGameData()
    {
        // 1. RESET ALL PlayerPrefs (Main method)
        PlayerPrefs.DeleteAll();

        // 2. SET DEFAULTS FOR COMMON SAVE KEYS
        // Coins & Currency
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("TotalCoins", 0);
        PlayerPrefs.SetInt("Money", 0);
        PlayerPrefs.SetInt("Gold", 0);
        PlayerPrefs.SetInt("Gems", 0);
        PlayerPrefs.SetInt("Score", 0);

        // Levels & Progress
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("LevelsCompleted", 0);
        PlayerPrefs.SetInt("LevelsUnlocked", 1);
        PlayerPrefs.SetInt("TotalLevelsCompleted", 0);

        // Shop items (assuming 0 = not purchased, 1 = purchased)
        PlayerPrefs.SetInt("ShopItem_1", 0);
        PlayerPrefs.SetInt("ShopItem_2", 0);
        PlayerPrefs.SetInt("ShopItem_3", 0);
        PlayerPrefs.SetInt("ShopItem_4", 0);
        PlayerPrefs.SetInt("ItemPurchased", 0);
        PlayerPrefs.SetInt("SkinUnlocked_1", 1); // Usually first skin is free
        for (int i = 2; i <= 10; i++)
        {
            PlayerPrefs.SetInt("SkinUnlocked_" + i, 0);
        }

        // Settings (common defaults)
        PlayerPrefs.SetInt("MusicOn", 1);
        PlayerPrefs.SetInt("SoundOn", 1);
        PlayerPrefs.SetInt("VibrationOn", 1);

        // Mark as initialized
        PlayerPrefs.SetInt("GameInitialized", 1);

        // 3. DELETE ANY CUSTOM SAVE FILES
        string savePath = Application.persistentDataPath;

        if (Directory.Exists(savePath))
        {
            // Look for common save file names
            string[] possibleFiles = {
                "save.dat", "save.json", "save.txt", "gameSave.dat",
                "playerData.dat", "progress.dat", "userData.json"
            };

            foreach (string fileName in possibleFiles)
            {
                string fullPath = Path.Combine(savePath, fileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Debug.Log("Deleted save file: " + fullPath);
                }
            }

            // Also delete any .dat files (common binary saves)
            string[] allFiles = Directory.GetFiles(savePath, "*.dat");
            foreach (string file in allFiles)
            {
                File.Delete(file);
                Debug.Log("Deleted: " + file);
            }

            // Delete .json files
            string[] jsonFiles = Directory.GetFiles(savePath, "*.json");
            foreach (string file in jsonFiles)
            {
                File.Delete(file);
                Debug.Log("Deleted: " + file);
            }
        }

        // 4. SAVE THE NEW VERSION
        PlayerPrefs.SetString("GameVersion", Application.version);

        // 5. FORCE SAVE
        PlayerPrefs.Save();

        Debug.Log("=== RESET COMPLETE - READY FOR VERSION " + Application.version + " ===");
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!operation.isDone)
        {
            elapsedTime += Time.deltaTime;
            float smoothProgress = Mathf.Clamp01(elapsedTime / loadingDuration);

            if (progressBar != null)
                progressBar.value = smoothProgress;

            if (percentText != null)
                percentText.text = Mathf.RoundToInt(smoothProgress * 100f) + "%";

            if (loadingDotsText != null)
            {
                int dotCount = Mathf.FloorToInt(Time.time % 4);
                loadingDotsText.text = "Loading" + new string('.', dotCount);
            }

            // when finished -> stop music automatically on scene change
            if (smoothProgress >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}