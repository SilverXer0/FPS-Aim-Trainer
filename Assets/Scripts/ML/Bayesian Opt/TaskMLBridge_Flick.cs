using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskMLBridge_Flick : MonoBehaviour
{
    [Header("Task Key")]
    public string taskKey = "";

    void Awake()
    {
        if (string.IsNullOrEmpty(taskKey))
            taskKey = SceneManager.GetActiveScene().name;
    }
    
    [Header("BO Bounds")]
    public float minSens = 10f, maxSens = 200f;
    public float maxStep = 12f;

    [Header("Utility Weights")]
    public float wAcc = 1.0f;
    public float wAbs = 0.5f;
    public float wBias = 0.3f;
    public float degCap = 15f;

    void OnEnable() { Timer.OnGameEnded += OnEnd; }
    void OnDisable(){ Timer.OnGameEnded -= OnEnd; }

    void OnEnd()
    {
        // Metrics you already compute:
        float acc = 0f;
        try {
            int hits = ScoreCounter.Score;
            int miss = MissCounter.Misses;
            acc = (hits + miss) > 0 ? (float)hits / (hits + miss) : 0f;
        } catch {}

        float meanSigned = 0f, meanAbs = 0f;
        if (FlickErrorTracker.Instance != null)
        {
            meanSigned = FlickErrorTracker.Instance.MeanSignedDeg;
            meanAbs    = FlickErrorTracker.Instance.MeanAbsDeg;
            FlickErrorTracker.Instance.ResetRun();
        }

        float utility = Utility.FlickUtility(acc, meanSigned, meanAbs, wAcc, wAbs, wBias, degCap);

        var histKey = $"HIST_{taskKey}";
        var hist = HistoryIO.Load(histKey);
        var cur  = SensitivityManager.Instance.currentSensitivity;

        HistoryIO.Add(hist, cur, utility);
        var bo = new OneDBayesOpt { minSens = minSens, maxSens = maxSens };
        bo.FitFromHistory(hist);

        double next = bo.SuggestNext(cur, maxStep, 61);
        SensitivityManager.Instance.SetSensitivity((float)next);
        HistoryIO.Save(histKey, hist);

        Debug.Log($"[ML-BO Flick] util={utility:0.000} sens {cur:0.0}→{next:0.0} pts={hist.data.Count}");
    }
}
