using UnityEngine;

public class TaskMLBridge_Tracking_BO : MonoBehaviour
{
    [Header("Task Key")]
    public string taskKey = "StrafeTrackBasic";

    void OnEnable()  { Timer.OnGameEnded += OnEnd; }
    void OnDisable() { Timer.OnGameEnded -= OnEnd; }

    void Start()
    {
        var hist = HistoryIO.Load($"HIST_{taskKey}");
        var ctx  = new TaskContext { taskKey = taskKey, trackingSpeedNorm = 0.5f };
        float cur = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityPolicy.Instance.ProposeNext(taskKey, cur, hist, ctx);
        SensitivityManager.Instance.SetSensitivity(next);
    }

    void OnEnd()
    {
        float ratio01 = 0f;
        try { ratio01 = StrafeTrackStats.Instance != null ? StrafeTrackStats.Instance.OnTargetRatio01 : 0f; } catch {}

        float utility = Utility.TrackingUtility(ratio01, 1.0f);

        float sens = SensitivityManager.Instance.currentSensitivity;
        var ctx = new TaskContext { taskKey = taskKey, lastAcc = ratio01, trackingSpeedNorm = 0.5f };
        SensitivityPolicy.Instance.Observe(taskKey, sens, utility, ctx);

        var hist = HistoryIO.Load($"HIST_{taskKey}");
        float next = SensitivityPolicy.Instance.ProposeNext(taskKey, sens, hist, ctx);
        SensitivityManager.Instance.SetSensitivity(next);

        Debug.Log($"[ML-BO Track] util={utility:0.000} sens {sens:0.0}→{next:0.0} pts={hist.data.Count}");
    }
}
