using UnityEngine;

public class SensitivityManager : MonoBehaviour
{
    public static SensitivityManager Instance { get; private set; }

    [Header("Global Mouse Sensitivity")]
    [Tooltip("This value is used by all PlayerController/MouseLook scripts.")]
    public float currentSensitivity = 100f;

    [Header("Bounds")]
    [SerializeField] private float minSensitivity = 50f;
    [SerializeField] private float maxSensitivity = 300f;

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

        // Load saved sens (falls back to inspector default if none saved).
        currentSensitivity = PlayerPrefs.GetFloat(PrefKey, currentSensitivity);
        currentSensitivity = Mathf.Clamp(currentSensitivity, minSensitivity, maxSensitivity);
    }

    void Start()
    {
        // Optional: fire initial event in Start so most listeners have subscribed.
        OnSensitivityChanged?.Invoke(currentSensitivity);

        // Optional: write an initial snapshot so the graph has a starting dot.
        WriteGlobalHistoryPoint(currentSensitivity);
    }

    public void SetSensitivity(float newSens)
    {
        // Clamp + ignore no-op updates
        newSens = Mathf.Clamp(newSens, minSensitivity, maxSensitivity);
        if (Mathf.Approximately(newSens, currentSensitivity))
            return;

        currentSensitivity = newSens;
        PlayerPrefs.SetFloat(PrefKey, newSens);
        PlayerPrefs.Save();

        OnSensitivityChanged?.Invoke(newSens);

        // Write to global history so your GlobalSensitivityOverlay can plot it
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

            // 1) Load existing history (or an empty one)
            TaskHistory hist = HistoryIO.Load(key);

            // 2) Make sure it has a task name (optional but nice for overlays/tools)
            if (string.IsNullOrEmpty(hist.taskName))
                hist.taskName = "SENS_GLOBAL";

            // 3) Append the point (order: sens, utility) and keep last 200 by default
            HistoryIO.Add(hist, sens, 0f, keepLast: 200);

            // 4) Save back to PlayerPrefs
            HistoryIO.Save(key, hist);
        }
        catch
        {
            // History system not present in the current scene — safe to ignore
        }
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
    catch {}
}
}
