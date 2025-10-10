using UnityEngine;
using TMPro;

public class SensitivityHUDLabel : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] string prefix = "Sens: ";

    void OnEnable()
    {
        if (label && SensitivityManager.Instance != null)
            label.text = prefix + SensitivityManager.Instance.currentSensitivity.ToString("0.##");
        SensitivityManager.OnSensitivityChanged += HandleChanged;
    }
    void OnDisable()
    {
        SensitivityManager.OnSensitivityChanged -= HandleChanged;
    }
    void HandleChanged(float val)
    {
        if (label) label.text = prefix + val.ToString("0.##");
    }
}
