using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    [Tooltip("Name of the scene to load when Home is pressed")]
    public string taskSelectSceneName = "TaskSelect";

    public void GoHome()
    {
        SceneManager.LoadScene(taskSelectSceneName);
    }
}
