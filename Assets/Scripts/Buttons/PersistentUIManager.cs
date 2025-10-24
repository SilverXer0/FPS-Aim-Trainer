using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PersistentUIManager : MonoBehaviour
{
    public static PersistentUIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Attach in the first scene too
        StartCoroutine(AttachNextFrame());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wait a frame so lazy-loaded UI is present, then attach
        StartCoroutine(AttachNextFrame());
    }

    private IEnumerator AttachNextFrame()
    {
        yield return null; // end of frame
        AttachToAllButtons();
    }

    private void AttachToAllButtons()
    {
        var buttons = FindObjectsOfType<Button>(true); // include inactive
        foreach (var b in buttons)
        {
            // Prevent duplicate subscriptions across rescans
            b.onClick.RemoveListener(PlayClick);
            b.onClick.AddListener(PlayClick);
        }
#if UNITY_EDITOR
        Debug.Log($"[UI] Wired click sound to {buttons.Length} buttons in scene '{SceneManager.GetActiveScene().name}'.");
#endif
    }

    private void PlayClick()
    {
        AudioHub.Instance?.PlayUIClick();
    }
}
