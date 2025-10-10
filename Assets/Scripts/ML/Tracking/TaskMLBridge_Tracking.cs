using UnityEngine;

public class TaskMLBridge_Tracking : MonoBehaviour
{
    [Tooltip("Unique task name (e.g., StrafeTrack, StrafeRandom, SineTrack, EightDir, EntryTrack)")]
    public string taskName = "StrafeTrack";

    [Header("Per-task tuning")]
    [Range(0.5f, 0.99f)] public float targetAccuracy = 0.92f; 
    [Tooltip("Scale viewport error into 'deg-equivalent' for optimizer.")]
    public float viewportToDegScale = 1200f; 

    void OnEnable()
    {
        SensitivityOptimizer.TargetAcc = targetAccuracy;
        Timer.OnGameEnded += OnTaskEnd;
    }
    void OnDisable()
    {
        Timer.OnGameEnded -= OnTaskEnd;
    }

    void OnTaskEnd()
    {
        float acc = Safe(()=> TrackingErrorTracker.Instance.TrackingAccuracy01, 0f);
        float meanSignedViewport = Safe(()=> TrackingErrorTracker.Instance.MeanSignedAlongMotion, 0f);
        float meanAbsViewport    = Safe(()=> TrackingErrorTracker.Instance.MeanAbsAlongMotion,    0f);

        float meanSignedDegEq = meanSignedViewport * viewportToDegScale;
        float meanAbsDegEq    = Mathf.Abs(meanAbsViewport) * viewportToDegScale;

        string runsKey = $"ML_Runs_{taskName}";
        int runsSoFar = PlayerPrefs.GetInt(runsKey, 0);

        float cur  = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityOptimizer.Update(cur, meanSignedDegEq, meanAbsDegEq, acc, runsSoFar);
        SensitivityManager.Instance.SetSensitivity(next);

        PlayerPrefs.SetInt(runsKey, runsSoFar + 1);
        PlayerPrefs.Save();

        TrackingErrorTracker.Instance?.ResetRun();

        Debug.Log($"[Tracking ML:{taskName}] run#{runsSoFar+1} acc={acc:0.00} signedV={meanSignedViewport:0.000} absV={meanAbsViewport:0.000}  {cur:0.0}→{next:0.0}");
    }

    private T Safe<T>(System.Func<T> f, T d) { try { return f(); } catch { return d; } }
}
