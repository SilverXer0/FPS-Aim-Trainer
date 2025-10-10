using UnityEngine;

public class TaskMLBridge_Flickshot : MonoBehaviour
{
    public string taskName = "Flickshot";

    [Header("Speed blend (optional)")]
    [Tooltip("Weight for reaction-time factor in 'effective accuracy' (0=ignore RT, 1=only RT)")]
    [Range(0f, 1f)] public float rtWeight = 0.25f;
    [Tooltip("RT time constant for exp decay; lower = rewards faster flicks more")]
    public float rtTau = 0.40f;

    [Header("Target accuracy for this task")]
    [Range(0.5f, 0.99f)] public float targetAcc = 0.90f;

    void OnEnable()
    {
        SensitivityOptimizer.TargetAcc = targetAcc;
        Timer.OnGameEnded += OnTaskEnd;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= OnTaskEnd;
    }

    void OnTaskEnd()
    {
        int hits   = Safe(() => FlickshotStats.Instance.Hits,   0);
        int misses = Safe(() => FlickshotStats.Instance.Misses, 0);
        float acc  = (hits + misses) > 0 ? (float)hits / (hits + misses) : 0f;

        float avgRT    = Safe(() => FlickshotStats.Instance.AvgRT, 0.5f);
        float rtFactor = Mathf.Exp(-avgRT / Mathf.Max(0.05f, rtTau));

        float effectiveAcc = Mathf.Clamp01((1f - rtWeight) * acc + rtWeight * rtFactor);

        float meanSigned = Safe(() => FlickErrorTracker.Instance.MeanSignedDeg, 0f);
        float meanAbs    = Safe(() => FlickErrorTracker.Instance.MeanAbsDeg,    0f);

        string runsKey = $"ML_Runs_{taskName}";
        int runsSoFar = PlayerPrefs.GetInt(runsKey, 0);

        float cur  = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityOptimizer.Update(cur, meanSigned, meanAbs, effectiveAcc, runsSoFar);
        SensitivityManager.Instance.SetSensitivity(next);

        PlayerPrefs.SetInt(runsKey, runsSoFar + 1);
        PlayerPrefs.Save();

        FlickshotStats.Instance?.ResetRun();
        FlickErrorTracker.Instance?.ResetRun();

        Debug.Log($"[FlickShot ML] run#{runsSoFar+1} acc={acc:0.00} rt={avgRT:0.00}s eff={effectiveAcc:0.00} signed={meanSigned:0.0} abs={meanAbs:0.0}  {cur:0.0}→{next:0.0}");
    }

    private T Safe<T>(System.Func<T> f, T d){ try { return f(); } catch { return d; } }
}
