using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadTask()
    {
        SceneManager.LoadScene("TaskSelect");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
