using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlobalButtonClickSound : MonoBehaviour
{
    void OnEnable()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (var b in allButtons)
            b.onClick.AddListener(PlayClick);
    }

    void OnDisable()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (var b in allButtons)
            b.onClick.RemoveListener(PlayClick);
    }

    void PlayClick()
    {
        AudioHub.Instance?.PlayUIClick();
    }
}
