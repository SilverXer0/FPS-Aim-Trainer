using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private GameObject pauseMenuRoot;   // panel + buttons root
    [SerializeField] private CanvasGroup canvasGroup;    // optional, on same root

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    [Header("Scenes")]
    [SerializeField] private string taskSelectSceneName = "TaskSelect";

    private bool isPaused;

    void Awake()
    {
        // keep root active; we hide via CanvasGroup so children render correctly
        if (pauseMenuRoot && !pauseMenuRoot.activeSelf) pauseMenuRoot.SetActive(true);
        SetMenuVisible(false);
    }

    void Start()
    {
        if (resumeButton)  resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton) restartButton.onClick.AddListener(RestartTask);
        if (homeButton)    homeButton.onClick.AddListener(GoHome);
    }

    void Update()
    {
        // don't open pause after the task ends
        if (Timer.GameEnded) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else          PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuRoot) pauseMenuRoot.transform.SetAsLastSibling(); // draw on top
        SetMenuVisible(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ResumeGame()
    {
        isPaused = false;

        SetMenuVisible(false);
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void RestartTask()
    {
        // unpause first, then reload current scene (gets you back to your countdown flow)
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    private void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(taskSelectSceneName);
    }

    private void SetMenuVisible(bool visible)
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        else if (pauseMenuRoot)
        {
            pauseMenuRoot.SetActive(visible);
        }
    }
}
