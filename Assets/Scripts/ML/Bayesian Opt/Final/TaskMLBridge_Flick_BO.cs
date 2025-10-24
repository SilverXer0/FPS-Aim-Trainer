using UnityEngine;

public class TaskMLBridge_Flick_BO : MonoBehaviour
{
    [Header("Task Key")]
    public string taskKey = "Gridshot";   // set per scene in Inspector

    [Header("Utility Weights")]
    public float wAcc = 1.0f;
    public float wAbs = 0.5f;
    public float wBias = 0.3f;
    public float degCap = 15f;

    void OnEnable()  { Timer.OnGameEnded += OnEnd; }
    void OnDisable() { Timer.OnGameEnded -= OnEnd; }

    void Start()
    {
        // At task start, optionally pre-set sensitivity from policy
        var hist = HistoryIO.Load($"HIST_{taskKey}");
        var ctx  = BuildContext(lastAcc:0f, lastAbs:0f, lastSigned:0f);
        float cur = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityPolicy.Instance.ProposeNext(taskKey, cur, hist, ctx);
        SensitivityManager.Instance.SetSensitivity(next);
    }

    void OnEnd()
    {
        // Collect metrics -> utility
        int hits=0, miss=0;
        try { hits = ScoreCounter.Score; miss = MissCounter.Misses; } catch {}
        float acc = (hits+miss)>0 ? (float)hits/(hits+miss) : 0f;

        float meanSigned = 0f, meanAbs = 0f;
        if (FlickErrorTracker.Instance != null)
        {
            meanSigned = FlickErrorTracker.Instance.MeanSignedDeg;
            meanAbs    = FlickErrorTracker.Instance.MeanAbsDeg;
            FlickErrorTracker.Instance.ResetRun();
        }

        float utility = Utility.FlickUtility(acc, meanSigned, meanAbs, wAcc, wAbs, wBias, degCap);

        // Observe
        float sens = SensitivityManager.Instance.currentSensitivity;
        var ctx = BuildContext(acc, meanAbs, meanSigned);
        SensitivityPolicy.Instance.Observe(taskKey, sens, utility, ctx);

        // Immediately pick next for future run
        var hist = HistoryIO.Load($"HIST_{taskKey}");
        float next = SensitivityPolicy.Instance.ProposeNext(taskKey, sens, hist, ctx);
        SensitivityManager.Instance.SetSensitivity(next);

        Debug.Log($"[ML-BO Flick] util={utility:0.000} sens {sens:0.0}→{next:0.0} pts={hist.data.Count}");
    }

    private TaskContext BuildContext(float lastAcc, float lastAbs, float lastSigned)
    {
        return new TaskContext {
            taskKey = taskKey,
            targetSizeNorm = 0.5f,     // fill if you have it
            avgZNorm = 0.5f,           // fill if you have it
            trackingSpeedNorm = 0f,
            dpiNorm = 0f, hzNorm = 0f,
            lastAcc = lastAcc,
            lastAbsErr = lastAbs,
            lastSignedErr = lastSigned
        };
    }
}
