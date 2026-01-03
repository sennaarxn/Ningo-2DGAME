using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    void Awake()
    {
        // Singleton pattern: only one AudioManager allowed
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup all sounds
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixer;
        }
    }

    private void Start()
    {
        Play("MainTheme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        if (!s.source.isPlaying)
            s.source.Play();
    }

    // NEW METHOD: For UI button clicks (plays every time)
    public void PlayUI(string name)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            Debug.LogWarning("UI Sound not found: " + name);
            return;
        }

        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        if (s.source.isPlaying)
            s.source.Stop();
    }

    public void RestartMusic(string name, float fadeDuration = 1f)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        // Stop immediately if playing
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }

        // Reset volume and play
        s.source.volume = s.volume; // Reset to original volume
        s.source.Play();

        // Optional: Fade in if desired
        StartCoroutine(FadeIn(name, s.volume, fadeDuration));
    }

    public void ResumeMusicFromSilent(string name, float targetVolume = 0.8f, float duration = 1f)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("ResumeMusicFromSilent: Sound not found: " + name);
            return;
        }

        // Do NOT restart the track — just fade in volume
        StartCoroutine(FadeIn(name, targetVolume, duration));
    }


    // ✅ Fades in the "MainTheme" background music smoothly
    public System.Collections.IEnumerator FadeInBackground(float targetVolume = 0.8f, float duration = 1f)
    {
        Sound bgSound = Array.Find(sounds, s => s.name == "MainTheme");
        if (bgSound == null || bgSound.source == null)
        {
            Debug.LogWarning("MainTheme not found for fading.");
            yield break;
        }

        bgSound.source.volume = 0;
        if (!bgSound.source.isPlaying)
            bgSound.source.Play();

        float startVolume = 0f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            bgSound.source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        bgSound.source.volume = targetVolume;
    }

    public System.Collections.IEnumerator FadeOut(string name, float duration = 1f)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("FadeOut: Sound not found: " + name);
            yield break;
        }

        float startVolume = s.source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            s.source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        s.source.volume = 0f;

    }

    public System.Collections.IEnumerator FadeIn(string name, float targetVolume = 0.8f, float duration = 1f)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("FadeIn: Sound not found: " + name);
            yield break;
        }

        s.source.volume = 0f;
        if (!s.source.isPlaying)
            s.source.Play();

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            s.source.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }

        s.source.volume = targetVolume;
    }
}