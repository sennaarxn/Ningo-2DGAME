using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuEvents : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer mixer;
    public AudioMixer backgroundmixer;
    public Slider volumeSlider;
    public Slider volumeBackground;

    private void Start()
    {
        // Initialize volume sliders
        if (mixer != null && volumeSlider != null)
        {
            if (mixer.GetFloat("volume", out float volValue))
                volumeSlider.value = Mathf.Pow(10, volValue / 20f);
            else
                volumeSlider.value = 0.8f;
        }

        if (backgroundmixer != null && volumeBackground != null)
        {
            if (backgroundmixer.GetFloat("background", out float bgValue))
                volumeBackground.value = Mathf.Pow(10, bgValue / 20f);
            else
                volumeBackground.value = 0.8f;
        }
    }

    public void SetVolume()
    {
        if (mixer == null || volumeSlider == null) return;
        float dBValue = volumeSlider.value <= 0.0001f ? -80f : 20f * Mathf.Log10(volumeSlider.value * 1.5f);
        mixer.SetFloat("volume", dBValue);
    }

    public void SetBackground()
    {
        if (backgroundmixer == null || volumeBackground == null) return;
        float dBValue = volumeBackground.value <= 0.0001f ? -80f : 20f * Mathf.Log10(volumeBackground.value * 1.5f);
        backgroundmixer.SetFloat("background", dBValue);
    }

    public void LoadLevel(int index)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.FadeIn("MainTheme", 1.5f, 1f);
        SceneManager.LoadScene(index);
    }

    public void LoadLevel01()
    {
        Time.timeScale = 1;
        PlayerManager.ResetCheckpointData();
        if (AudioManager.instance != null)
            AudioManager.instance.FadeIn("MainTheme", 1.5f, 1f);
        SceneManager.LoadScene("Level01");
    }

    public void LoadLevel02()
    {
        Time.timeScale = 1;
        PlayerManager.ResetCheckpointData();
        if (AudioManager.instance != null)
            AudioManager.instance.FadeIn("MainTheme", 1.5f, 1f);
        SceneManager.LoadScene("Level02");
    }

    public void LoadLevel03()
    {
        Time.timeScale = 1;
        PlayerManager.ResetCheckpointData();
        if (AudioManager.instance != null)
            AudioManager.instance.FadeIn("MainTheme", 1.5f, 1f);
        SceneManager.LoadScene("Level03");
    }
}