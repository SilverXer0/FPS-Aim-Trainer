using UnityEngine;

public class TaskMLBridge : MonoBehaviour
{
    public string taskName = "Gridshot"; 
    void OnEnable()
    {
        Timer.OnGameEnded += OnTaskEnd;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= OnTaskEnd;
    }

    void OnTaskEnd()
    {
        int hits   = Safe(() => ScoreCounter.Score, 0);
        int misses = Safe(() => MissCounter.Misses, 0);
        float accuracy = (hits + misses) > 0 ? (float)hits / (hits + misses) : 0f;

        float meanSigned = Safe(() => FlickErrorTracker.Instance.MeanSignedDeg, 0f);
        float meanAbs    = Safe(() => FlickErrorTracker.Instance.MeanAbsDeg,    0f);

        string runsKey = $"ML_Runs_{taskName}";
        int runsSoFar = PlayerPrefs.GetInt(runsKey, 0);

        float cur  = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityOptimizer.Update(cur, meanSigned, meanAbs, accuracy, runsSoFar);

        SensitivityManager.Instance.SetSensitivity(next);

        PlayerPrefs.SetInt(runsKey, runsSoFar + 1);
        PlayerPrefs.Save();

        if (FlickErrorTracker.Instance) FlickErrorTracker.Instance.ResetRun();

        Debug.Log($"[Gridshot ML] run#{runsSoFar+1} acc={accuracy:0.00} signed={meanSigned:0.0} abs={meanAbs:0.0}  {cur:0.0}→{next:0.0}");
    }

    private T Safe<T>(System.Func<T> f, T d){ try { return f(); } catch { return d; } }
}
