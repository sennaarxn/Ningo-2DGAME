[System.Serializable]
public class Character
{
    public string name;
    public int price;
    public bool isUnlocked;

    // Optional constructor
    public Character(string characterName, int characterPrice, bool unlocked = false)
    {
        name = characterName;
        price = characterPrice;
        isUnlocked = unlocked;
    }
}