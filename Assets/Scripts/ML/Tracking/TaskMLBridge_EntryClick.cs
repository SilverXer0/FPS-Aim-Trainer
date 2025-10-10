// TaskMLBridge_EntryClick.cs
using UnityEngine;

public class TaskMLBridge_EntryClick : MonoBehaviour
{
    [Tooltip("Unique key for run counting & tuning")]
    public string taskName = "EntryClick";

    [Header("Per-task tuning")]
    [Range(0.5f, 0.99f)] public float targetAcc = 0.80f; 

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
        int hits   = Safe(() => EntryScoreCounter.Score, 0);
        int misses = Safe(() => EntryMissCounter.Misses, 0);
        float acc  = (hits + misses) > 0 ? (float)hits / (hits + misses) : 0f;

        float meanSigned = Safe(() => FlickErrorTracker.Instance.MeanSignedDeg, 0f);
        float meanAbs    = Safe(() => FlickErrorTracker.Instance.MeanAbsDeg,    0f);

        string runsKey = $"ML_Runs_{taskName}";
        int runsSoFar  = PlayerPrefs.GetInt(runsKey, 0);

        float cur  = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityOptimizer.Update(cur, meanSigned, meanAbs, acc, runsSoFar);
        SensitivityManager.Instance.SetSensitivity(next);

        PlayerPrefs.SetInt(runsKey, runsSoFar + 1);
        PlayerPrefs.Save();

        FlickErrorTracker.Instance?.ResetRun();

        Debug.Log($"[Entry ML] run#{runsSoFar+1} acc={acc:0.00} signed={meanSigned:0.0} abs={meanAbs:0.0} {cur:0.0}→{next:0.0}");
    }

    private T Safe<T>(System.Func<T> f, T d){ try { return f(); } catch { return d; } }
}
