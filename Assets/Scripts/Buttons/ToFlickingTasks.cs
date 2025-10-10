using UnityEngine;
using UnityEngine.SceneManagement;

public class ToFlickingTasks : MonoBehaviour
{
    public void LoadFlickingTasks()
    {
        SceneManager.LoadScene("FlickingTasks");
    }
}
