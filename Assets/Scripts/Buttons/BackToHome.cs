using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToHome : MonoBehaviour
{
    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }
}
