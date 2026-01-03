using System.Collections.Generic;
using UnityEngine;

public class CollectedTracker : MonoBehaviour
{
    public static CollectedTracker instance { get; private set; }
    public static HashSet<string> currentAttemptItems = new HashSet<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void AddItem(string itemId)
    {
        currentAttemptItems.Add(itemId);
    }

    public static bool ShouldShowItem(string itemId)
    {
        return !currentAttemptItems.Contains(itemId);
    }

    public static bool IsCollected(string itemId)
    {
        return !ShouldShowItem(itemId);
    }

    public static void SaveCheckpointState() { }

    public static void SaveToPermanent()
    {
        if (SaveManager.Instance != null)
        {
            foreach (var item in currentAttemptItems)
            {
                if (!SaveManager.Instance.collectedItems.Contains(item))
                {
                    SaveManager.Instance.collectedItems.Add(item);
                }
            }
            SaveManager.Instance.SaveAllData();
        }
        currentAttemptItems.Clear();
    }

    public static void RestartLevel()
    {
        currentAttemptItems.Clear();
    }

    public static void ClearSession()
    {
        currentAttemptItems.Clear();
    }

    public static void ClearSessionItems() => ClearSession();
    public static void SaveAllData() => SaveToPermanent();
    public static void SaveCurrentRunToPermanent() => SaveToPermanent();
    public static void SaveAllToPermanent() => SaveToPermanent();
    public static void LoadSavedToSession() { }
    public static void ClearAll() => ClearSession();
}