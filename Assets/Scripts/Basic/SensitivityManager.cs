using UnityEngine;

public class SensitivityManager : MonoBehaviour
{
    public static SensitivityManager Instance { get; private set; }

    [Header("Global Mouse Sensitivity")]
    [Tooltip("This value is used by all PlayerController/MouseLook scripts.")]
    public float currentSensitivity = 100f;

    private const string PrefKey = "GlobalSensitivity";

    public static event System.Action<float> OnSensitivityChanged;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        currentSensitivity = PlayerPrefs.GetFloat(PrefKey, currentSensitivity);
    }

    public void SetSensitivity(float newSens)
    {
        currentSensitivity = newSens;
        PlayerPrefs.SetFloat(PrefKey, newSens);
        PlayerPrefs.Save();
        OnSensitivityChanged?.Invoke(newSens);
    }
}
