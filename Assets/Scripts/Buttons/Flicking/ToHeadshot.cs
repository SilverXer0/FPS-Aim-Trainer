using UnityEngine;
using UnityEngine.SceneManagement;

public class ToHeadshot : MonoBehaviour
{
    public void LoadHeadshot()
    {
        SceneManager.LoadScene("Headshot");
    }
}
