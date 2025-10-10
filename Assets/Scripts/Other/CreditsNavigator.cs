using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsNavigator : MonoBehaviour
{
    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }
}
