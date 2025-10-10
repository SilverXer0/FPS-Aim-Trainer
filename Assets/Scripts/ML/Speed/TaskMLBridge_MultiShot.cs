using UnityEngine;

public class TaskMLBridge_MultiShot : MonoBehaviour
{
    public string taskName = "MultiShot";
    [Range(0.5f,0.99f)] public float targetAcc = 0.90f;

    void OnEnable()
    {
        SensitivityOptimizer.TargetAcc = targetAcc;
        Timer.OnGameEnded += OnTaskEnd;
    }
    void OnDisable(){ Timer.OnGameEnded -= OnTaskEnd; }

    void OnTaskEnd()
    {
        int hits   = Safe(()=> ScoreCounter.Score, 0);
        int misses = Safe(()=> MultiShotMissCounter.Misses, 0);   // ← local misses
        float acc  = (hits + misses) > 0 ? (float)hits / (hits + misses) : 0f;

        float meanSigned = Safe(()=> FlickErrorTracker.Instance.MeanSignedDeg, 0f);
        float meanAbs    = Safe(()=> FlickErrorTracker.Instance.MeanAbsDeg,    0f);

        string runsKey = $"ML_Runs_{taskName}";
        int runs = PlayerPrefs.GetInt(runsKey, 0);

        float cur  = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityOptimizer.Update(cur, meanSigned, meanAbs, acc, runs);
        SensitivityManager.Instance.SetSensitivity(next);

        PlayerPrefs.SetInt(runsKey, runs + 1);
        PlayerPrefs.Save();

        FlickErrorTracker.Instance?.ResetRun();

        Debug.Log($"[MultiShot ML] run#{runs+1} acc={acc:0.00} signed={meanSigned:0.0} abs={meanAbs:0.0}  {cur:0.0}→{next:0.0}");
    }

    private T Safe<T>(System.Func<T> f, T d){ try { return f(); } catch { return d; } }
}
