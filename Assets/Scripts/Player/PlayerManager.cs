using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public static bool isGameOver;
    public static Vector2 lastCheckPointPos = new Vector2(-4f, 0.5f);

    public int numberOfCoins;
    public int numberOfDiamonds;

    private static int deathPersistentCoins = 0;
    private static int deathPersistentDiamonds = 0;

    [Header("Cinemachine")] public CinemachineCamera VCam;
    [Header("Player Settings")] public GameObject[] playerPrefabs;
    [Header("UI References")]
    [SerializeField] public TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI diamondsText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseMenuScreen;

    private bool hasFadedOutMusicOnGameOver = false;
    private GameObject currentPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        isGameOver = false;
        hasFadedOutMusicOnGameOver = false;
        numberOfCoins = deathPersistentCoins;
        numberOfDiamonds = deathPersistentDiamonds;

        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        currentPlayer = Instantiate(playerPrefabs[characterIndex], lastCheckPointPos, Quaternion.identity);

        if (VCam != null) VCam.Follow = currentPlayer.transform;

        if (AudioManager.instance != null)
        {
            Sound mainTheme = Array.Find(AudioManager.instance.sounds, s => s.name == "MainTheme");
            if (mainTheme != null && !mainTheme.source.isPlaying)
            {
                AudioManager.instance.StartCoroutine(AudioManager.instance.FadeIn("MainTheme", 0.8f, 1f));
            }
        }
    }

    private void Update()
    {
        if (coinsText != null) coinsText.text = numberOfCoins.ToString();
        if (diamondsText != null) diamondsText.text = numberOfDiamonds.ToString();

        if (isGameOver)
        {
            if (!hasFadedOutMusicOnGameOver && AudioManager.instance != null)
            {
                StartCoroutine(AudioManager.instance.FadeOut("MainTheme", 1.5f));
                hasFadedOutMusicOnGameOver = true;
            }
            if (gameOverScreen != null) gameOverScreen.SetActive(true);
        }
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1;
        numberOfCoins = 0;
        numberOfDiamonds = 0;
        deathPersistentCoins = 0;
        deathPersistentDiamonds = 0;

        if (CollectedTracker.instance != null) CollectedTracker.RestartLevel();
        ResetCheckpointData();
        StartCoroutine(ReloadSceneWithMusic());
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        if (pauseMenuScreen != null) pauseMenuScreen.SetActive(true);
        if (AudioManager.instance != null) AudioManager.instance.StartCoroutine(AudioManager.instance.FadeOut("MainTheme", 0.5f));
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false);
        if (AudioManager.instance != null) AudioManager.instance.StartCoroutine(AudioManager.instance.FadeIn("MainTheme", 0.8f, 0.5f));
    }

    public void ReplayFromCheckpoint()
    {
        Time.timeScale = 1;
        deathPersistentCoins = numberOfCoins;
        deathPersistentDiamonds = numberOfDiamonds;
        StartCoroutine(ReloadSceneWithMusic());
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        deathPersistentCoins = 0;
        deathPersistentDiamonds = 0;
        numberOfCoins = 0;
        numberOfDiamonds = 0;

        if (CollectedTracker.instance != null) CollectedTracker.ClearSession();
        ResetCheckpointData();
        SceneManager.LoadScene("Menu");

        if (AudioManager.instance != null)
        {
            AudioManager.instance.ResumeMusicFromSilent("MainTheme", 0.8f, 1f);
        }
    }

    public void GoToMenuAfterLevelComplete()
    {
        Time.timeScale = 1;
        StartCoroutine(LoadMenuWithMusic());
    }

    public void GoToLevel02()
    {
        Time.timeScale = 1;
        OnLevelEnd();
        SceneManager.LoadScene("Level02");
        if (AudioManager.instance != null) AudioManager.instance.StartCoroutine(AudioManager.instance.FadeIn("MainTheme", 0.8f, 0.5f));
    }

    public void GoToLevel03()
    {
        Time.timeScale = 1;
        OnLevelEnd();
        SceneManager.LoadScene("Level03");
        if (AudioManager.instance != null) AudioManager.instance.StartCoroutine(AudioManager.instance.FadeIn("MainTheme", 0.8f, 0.5f));
    }

    public void OnPlayerDeath()
    {
        deathPersistentCoins = numberOfCoins;
        deathPersistentDiamonds = numberOfDiamonds;
    }

    public void OnLevelEnd()
    {
        if (CollectedTracker.instance != null) CollectedTracker.SaveToPermanent();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.AddCoins(numberOfCoins);
            SaveManager.Instance.AddDiamonds(numberOfDiamonds);
            SaveManager.Instance.SaveAllData();
        }

        deathPersistentCoins = 0;
        deathPersistentDiamonds = 0;
        ResetCheckpointData();
        numberOfCoins = 0;
        numberOfDiamonds = 0;
    }

    public void AddCoin()
    {
        numberOfCoins++;
    }

    public void AddDiamond()
    {
        numberOfDiamonds++;
    }

    public void SaveCheckpointState() { }

    public static void ResetCheckpointData()
    {
        lastCheckPointPos = new Vector2(-4f, 0.5f);
    }

    private IEnumerator ReloadSceneWithMusic()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
        yield return new WaitForEndOfFrame();

        if (AudioManager.instance != null)
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.FadeIn("MainTheme", 0.8f, 1f));
        }
    }

    private IEnumerator LoadMenuWithMusic()
    {
        SceneManager.LoadScene("Menu");
        yield return new WaitForEndOfFrame();

        if (AudioManager.instance != null)
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.FadeIn("MainTheme", 0.8f, 1f));
        }
    }
}