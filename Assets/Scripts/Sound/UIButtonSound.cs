using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private string soundName = "ButtonClick";

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("UIButtonSound attached to non-button GameObject: " + gameObject.name);
        }
    }

    void OnButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayUI(soundName);
        }
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}