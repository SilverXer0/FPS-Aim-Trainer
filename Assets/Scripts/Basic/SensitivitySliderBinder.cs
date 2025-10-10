// SensitivitySliderBinder.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySliderBinder : MonoBehaviour
{
    [SerializeField] Slider slider;             
    [SerializeField] TMP_InputField inputField;
    bool _updatingUI; 

    void OnEnable()
    {
        slider.onValueChanged.AddListener(OnUserSlider);
        if (inputField) inputField.onEndEdit.AddListener(OnUserText);

        if (SensitivityManager.Instance != null)
            SetUI(SensitivityManager.Instance.currentSensitivity);

        SensitivityManager.OnSensitivityChanged += HandleChanged;
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnUserSlider);
        if (inputField) inputField.onEndEdit.RemoveListener(OnUserText);
        SensitivityManager.OnSensitivityChanged -= HandleChanged;
    }

    void HandleChanged(float val) => SetUI(val);

    void SetUI(float val)
    {
        _updatingUI = true;
        if (slider) slider.value = Mathf.Clamp(val, slider.minValue, slider.maxValue);
        if (inputField) inputField.text = val.ToString("0.##");
        _updatingUI = false;
    }

    void OnUserSlider(float val)
    {
        if (_updatingUI) return; 
        SensitivityManager.Instance?.SetSensitivity(val);
        if (inputField) inputField.text = val.ToString("0.##");
    }

    void OnUserText(string s)
    {
        if (_updatingUI) return;
        if (float.TryParse(s, out var v))
        {
            v = Mathf.Clamp(v, slider.minValue, slider.maxValue);
            SetUI(v); 
            SensitivityManager.Instance?.SetSensitivity(v);
        }
        else
        {
            if (SensitivityManager.Instance != null) SetUI(SensitivityManager.Instance.currentSensitivity);
        }
    }
}
