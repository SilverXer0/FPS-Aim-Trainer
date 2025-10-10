using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivityUIController : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;                     
    public TMP_InputField inputField;        
    [Header("Value Range")]
    public float minSens = 1f;
    public float maxSens = 1000f;

    void Start()
    {
        slider.minValue = minSens;
        slider.maxValue = maxSens;

        float current = SensitivityManager.Instance.currentSensitivity;
        current = Mathf.Clamp(current, minSens, maxSens);

        slider.value = current;
        inputField.text = current.ToString("0");  


        slider.onValueChanged.AddListener(OnSliderChanged);
        inputField.onEndEdit.AddListener(OnInputFieldChanged);
    }

    void OnSliderChanged(float val)
    {
 
        SensitivityManager.Instance.SetSensitivity(val);
        inputField.text = val.ToString("0");
    }

    void OnInputFieldChanged(string text)
    {

        if (float.TryParse(text, out float val))
        {

            val = Mathf.Clamp(val, minSens, maxSens);

            slider.value = val;
            SensitivityManager.Instance.SetSensitivity(val);

            inputField.text = val.ToString("0");
        }
        else
        {

            inputField.text = slider.value.ToString("0");
        }
    }
}
