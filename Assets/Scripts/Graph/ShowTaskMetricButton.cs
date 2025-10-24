using UnityEngine;
public class ShowTaskMetricButton : MonoBehaviour
{
    [SerializeField] private TaskMetricOverlay overlay;
    [SerializeField] private string taskKey;
    public void Show() => overlay.Show(taskKey);
}
