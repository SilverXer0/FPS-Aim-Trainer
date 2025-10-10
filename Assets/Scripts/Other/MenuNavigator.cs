using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    // ← Add this
    public static MenuNavigator Instance { get; private set; }

    [Header("Scenes in sequence")]
    public List<string> taskScenes = new List<string> {
        "Gridshot",
        "StrafeTrack",
        "Headshot",
        "Spidershot",
        "KingggTask"
    };

    private int _currentIndex;
    private bool _playSequence;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        Timer.OnGameEnded += OnTaskFinished;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= OnTaskFinished;
    }

    public void LoadTaskSelect()
    {
        SceneManager.LoadScene("TaskSelect");
    }

    public void LoadSingle(string sceneName)
    {
        _playSequence = false;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSequence()
    {
        _playSequence = true;
        _currentIndex = 0;
        SceneManager.LoadScene(taskScenes[_currentIndex]);
    }

    private void OnTaskFinished()
    {
        if (!_playSequence) return;
        _currentIndex++;
        if (_currentIndex < taskScenes.Count)
            SceneManager.LoadScene(taskScenes[_currentIndex]);
        else
        {
            _playSequence = false;
            SceneManager.LoadScene("Home");
        }
    }

    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }
}
