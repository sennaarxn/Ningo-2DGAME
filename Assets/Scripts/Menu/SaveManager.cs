using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public int TotalCoins { get; private set; }
    public int TotalDiamonds { get; private set; }
    public HashSet<string> collectedItems = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void EnsureExists()
    {
        if (Instance == null)
        {
            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();
        }
    }

    private void LoadData()
    {
        TotalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        TotalDiamonds = PlayerPrefs.GetInt("TotalDiamonds", 0);

        if (PlayerPrefs.HasKey("CollectedItems"))
        {
            string itemsJson = PlayerPrefs.GetString("CollectedItems");
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(itemsJson);
            if (wrapper != null && wrapper.items != null)
            {
                collectedItems = new HashSet<string>(wrapper.items);
            }
        }
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        if (TotalCoins < 0) TotalCoins = 0;
        PlayerPrefs.SetInt("TotalCoins", TotalCoins);
    }

    public void AddDiamonds(int amount)
    {
        TotalDiamonds += amount;
        if (TotalDiamonds < 0) TotalDiamonds = 0;
        PlayerPrefs.SetInt("TotalDiamonds", TotalDiamonds);
    }

    public void SaveAllData()
    {
        PlayerPrefs.SetInt("TotalCoins", TotalCoins);
        PlayerPrefs.SetInt("TotalDiamonds", TotalDiamonds);

        List<string> itemsList = new List<string>(collectedItems);
        string itemsJson = JsonUtility.ToJson(new StringListWrapper(itemsList));
        PlayerPrefs.SetString("CollectedItems", itemsJson);

        PlayerPrefs.Save();
    }

    public void ResetAllData()
    {
        TotalCoins = 0;
        TotalDiamonds = 0;
        collectedItems.Clear();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class StringListWrapper
{
    public List<string> items;
    public StringListWrapper(List<string> items) { this.items = items; }
}