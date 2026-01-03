using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static int health = 3;

    public Image[] hearts;
    public Sprite fullheart;
    public Sprite emptyheart;

    private int previousHealth; // 🔹 track old health

    private void Awake()
    {
        health = 3;
        previousHealth = health; // set initial
    }

    void Update()
    {
        // 🔹 Detect health decrease
        if (health < previousHealth)
        {
            // 🔊 Play sound when HP is lost
            AudioManager.instance.Play("Hit");
        }

        // 🔹 Update hearts UI
        foreach (Image img in hearts)
        {
            img.sprite = emptyheart;
        }

        for (int i = 0; i < health; i++)
        {
            hearts[i].sprite = fullheart;
        }

        previousHealth = health; // update tracker
    }
}
