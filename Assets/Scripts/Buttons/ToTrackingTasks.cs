using UnityEngine;
using UnityEngine.SceneManagement;

public class ToTrackingTasks : MonoBehaviour
{
    public void LoadTrackingTasks()
    {
        SceneManager.LoadScene("TrackingTasks");
    }
}
