using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskMLBridge_Tracking : MonoBehaviour
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
    public float maxStep = 10f;

    void OnEnable(){ Timer.OnGameEnded += OnEnd; }
    void OnDisable(){ Timer.OnGameEnded -= OnEnd; }

    void OnEnd()
    {
        float ratio = 0f;
        try {
            ratio = StrafeTrackStats.Instance != null ? StrafeTrackStats.Instance.OnTargetRatio01 : 0f;
        } catch {}

        float utility = Utility.TrackingUtility(ratio, 1.0f);

        var histKey = $"HIST_{taskKey}";
        var hist = HistoryIO.Load(histKey);
        var cur  = SensitivityManager.Instance.currentSensitivity;

        HistoryIO.Add(hist, cur, utility);
        var bo = new OneDBayesOpt { minSens = minSens, maxSens = maxSens, length = 20f };
        bo.FitFromHistory(hist);

        double next = bo.SuggestNext(cur, maxStep, 61);
        SensitivityManager.Instance.SetSensitivity((float)next);
        HistoryIO.Save(histKey, hist);

        Debug.Log($"[ML-BO Track] util={utility:0.000} sens {cur:0.0}→{next:0.0} pts={hist.data.Count}");
    }
}
