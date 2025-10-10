using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToTaskSelect : MonoBehaviour
{
    public void LoadTaskSelect()
    {
        SceneManager.LoadScene("TaskSelect");
    }
}
