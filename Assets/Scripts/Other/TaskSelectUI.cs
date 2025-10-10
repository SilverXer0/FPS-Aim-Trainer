using UnityEngine;
using UnityEngine.UI;

public class TaskSelectUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button playAllButton;
    public Button headshotButton;
    public Button gridshotButton;
    public Button sixshotButton;
    public Button spidershotButton;
    public Button kingggButton;  // or whatever your 5th is

    void Start()
    {
        // Play all in sequence
        playAllButton.onClick.AddListener(() =>
        {
            Debug.Log("PlayAll Clicked");
            MenuNavigator.Instance.LoadSequence();
        });

        // Load individual scenes
        headshotButton.onClick.AddListener(() =>
        {
            MenuNavigator.Instance.LoadSingle("Headshot");
        });
        gridshotButton.onClick.AddListener(() =>
        {
            MenuNavigator.Instance.LoadSingle("Gridshot");
        });
        sixshotButton.onClick.AddListener(() =>
        {
            MenuNavigator.Instance.LoadSingle("Sixshot");
        });
        spidershotButton.onClick.AddListener(() =>
        {
            MenuNavigator.Instance.LoadSingle("Spidershot");
        });
        kingggButton.onClick.AddListener(() =>
        {
            MenuNavigator.Instance.LoadSingle("Kinggg");
        });
    }
}
