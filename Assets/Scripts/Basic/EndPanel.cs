using UnityEngine;
using TMPro;

public class EndPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private TMP_Text sensitivityChangeText;

    void OnEnable()
    {
        Timer.OnGameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= OnGameEnded;
    }

    void OnGameEnded()
    {

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (SensitivityManager.Instance != null && sensitivityChangeText != null)
        {
            float oldSens = SensitivityManager.Instance.lastSensitivityBeforeChange;
            float newSens = SensitivityManager.Instance.lastSensitivityAfterChange;
            sensitivityChangeText.text = $"Sensitivity: {oldSens:0.0} → {newSens:0.0}";
        }
    }
}