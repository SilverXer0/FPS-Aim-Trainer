using UnityEngine;

public class SensitivityManager : MonoBehaviour
{
    public static SensitivityManager Instance { get; private set; }

    [Header("Global Mouse Sensitivity")]
    public float currentSensitivity = 100f;

    // NEW: what it was right before the *last* SetSensitivity() succeeded
    public float lastSensitivityBeforeChange { get; private set; }

    // NEW: what it was changed *to* last time (usually same as current)
    public float lastSensitivityAfterChange  { get; private set; }

    [Header("Bounds")]
    [SerializeField] private float minSensitivity = 50f;
    [SerializeField] private float maxSensitivity = 300f;

    private const string PrefKey = "GlobalSensitivity";

    // you already had this
    public static event System.Action<float> OnSensitivityChanged;
    // NEW: a richer event if you want both
    public static event System.Action<float, float> OnSensitivityChangedDetailed;

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
        currentSensitivity = Mathf.Clamp(currentSensitivity, minSensitivity, maxSensitivity);

        // On startup, there hasn't been a change yet, so "old == new"
        lastSensitivityBeforeChange = currentSensitivity;
        lastSensitivityAfterChange  = currentSensitivity;
    }

    void Start()
    {
        OnSensitivityChanged?.Invoke(currentSensitivity);
        WriteGlobalHistoryPoint(currentSensitivity);
    }

    public void SetSensitivity(float newSens)
    {
        newSens = Mathf.Clamp(newSens, minSensitivity, maxSensitivity);

        // if no actual change, do nothing
        if (Mathf.Approximately(newSens, currentSensitivity))
            return;

        // remember old → new
        float oldSens = currentSensitivity;
        lastSensitivityBeforeChange = oldSens;
        lastSensitivityAfterChange  = newSens;

        currentSensitivity = newSens;
        PlayerPrefs.SetFloat(PrefKey, newSens);
        PlayerPrefs.Save();

        OnSensitivityChanged?.Invoke(newSens);
        OnSensitivityChangedDetailed?.Invoke(oldSens, newSens);

        WriteGlobalHistoryPoint(newSens);
    }

    public void BumpSensitivity(float delta)
    {
        SetSensitivity(currentSensitivity + delta);
    }

    private void WriteGlobalHistoryPoint(float sens)
    {
        try
        {
            const string key = "HIST_SENS_GLOBAL";
            TaskHistory hist = HistoryIO.Load(key);
            if (string.IsNullOrEmpty(hist.taskName))
                hist.taskName = "SENS_GLOBAL";

            HistoryIO.Add(hist, sens, 0f, keepLast: 200);
            HistoryIO.Save(key, hist);
        }
        catch { }
    }

    private void WritePerTaskPoint(string taskKey, float sens, float utility = 0f)
    {
        try
        {
            string key = $"HIST_{taskKey}";
            TaskHistory hist = HistoryIO.Load(key);
            if (string.IsNullOrEmpty(hist.taskName))
                hist.taskName = taskKey;

            HistoryIO.Add(hist, sens, utility, keepLast: 200);
            HistoryIO.Save(key, hist);
        }
        catch { }
    }
}