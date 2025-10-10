// TaskMLBridge_Zap.cs
using UnityEngine;

public class TaskMLBridge_Zap : MonoBehaviour
{
    public string taskName = "ZapShot";

    [Header("RT → effective accuracy")]
    [Tooltip("How much RT contributes vs. click accuracy. Keep small.")]
    [Range(0f,1f)] public float rtWeight = 0.15f;   
    [Tooltip("Time constant for exp decay: faster RT → closer to 1.")]
    public float rtTau = 0.35f;

    [Header("Base target accuracy expectation")]
    [Range(0.5f,0.99f)] public float targetAcc = 0.90f; 

    void OnEnable()
    {
        SensitivityOptimizer.TargetAcc = targetAcc;
        Timer.OnGameEnded += OnTaskEnd;
    }
    void OnDisable(){ Timer.OnGameEnded -= OnTaskEnd; }

    void OnTaskEnd()
    {
        int hits   = Safe(()=> ZapStats.Instance.Hits, 0);
        int misses = Safe(()=> ZapStats.Instance.Misclicks, 0);
        float clickAcc = (hits+misses)>0 ? (float)hits/(hits+misses) : 0f;

        float avgRT    = Safe(()=> ZapStats.Instance.AvgRT, 0.5f);
        float rtFactor = Mathf.Exp(-avgRT / Mathf.Max(0.05f, rtTau));

        float effectiveAcc = Mathf.Clamp01((1f-rtWeight)*clickAcc + rtWeight*rtFactor);

        float meanSigned = 0f, meanAbs = 0f;

        string runsKey = $"ML_Runs_{taskName}";
        int runs = PlayerPrefs.GetInt(runsKey, 0);

        float cur  = SensitivityManager.Instance.currentSensitivity;
        float next = SensitivityOptimizer.Update(cur, meanSigned, meanAbs, effectiveAcc, runs);
        SensitivityManager.Instance.SetSensitivity(next);

        PlayerPrefs.SetInt(runsKey, runs+1);
        PlayerPrefs.Save();

        ZapStats.Instance?.ResetRun();

        Debug.Log($"[Zap ML] run#{runs+1} clickAcc={clickAcc:0.00} avgRT={avgRT:0.000}s eff={effectiveAcc:0.00} {cur:0.0}→{next:0.0}");
    }

    private T Safe<T>(System.Func<T> f, T d){ try{ return f(); } catch{ return d; } }
}
