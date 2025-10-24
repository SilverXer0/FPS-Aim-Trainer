using UnityEngine;

// Drop this in a bootstrap scene (or auto-spawn). Survives loads.
public class SensitivityPolicy : MonoBehaviour, ISensitivityPolicy
{
    public static SensitivityPolicy Instance { get; private set; }

    [Header("Global bounds & trust region")]
    public float minSens = 10f;
    public float maxSens = 200f;
    public float maxStep = 12f;     // max change per run

    [Header("BLR features (toggle what you actually fill)")]
    public bool useContextualPrior = true;
    public int  featureDim = 8;     // adjust to match BuildFeatures()

    private ContextualBLR blr;
    private System.Collections.Generic.Dictionary<string, TaskHistory> cache =
        new System.Collections.Generic.Dictionary<string, TaskHistory>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        blr = new ContextualBLR(featureDim, priorVar:400f, noiseVar:25f);
    }



    public void Observe(string taskKey, float sens, float utility, TaskContext ctx)
    {
        var hist = LoadHist(taskKey);
        HistoryIO.Add(hist, sens, utility, keepLast:30);
        SaveHist(taskKey, hist);

        if (useContextualPrior)
        {
            var phi = BuildFeatures(ctx);
            blr.Observe(phi, sens);
            // TODO: persist BLR params occasionally if desired
        }
    }

    public float ProposeNext(string taskKey, float currentSens, TaskHistory hist, TaskContext ctx)
    {
        // 1) Prior center via BLR (or keep current)
        float priorCenter = currentSens;
        if (useContextualPrior)
        {
            var phi = BuildFeatures(ctx);
            // Thompson sample gives nicer exploration; MAP is fine too.
            priorCenter = Mathf.Clamp(blr.SamplePredict(phi), minSens, maxSens);
        }

        // 2) Fit 1D BO on this task's history
        var bo = new OneDBayesOpt { minSens = Mathf.Max(minSens, priorCenter - 25f),
                                    maxSens = Mathf.Min(maxSens, priorCenter + 25f) };
        bo.FitFromHistory(hist);

        // Wider exploration for first few points:
        double next = bo.SuggestNext(currentSens, maxStep: (hist.data.Count<4 ? 20f : maxStep), grid:61);

        // 3) Trust region clamp around current
        float clamped = Mathf.Clamp((float)next, currentSens - maxStep, currentSens + maxStep);
        clamped = Mathf.Clamp(clamped, minSens, maxSens);

        // Optional deadband: avoid jitter < 1
        if (Mathf.Abs(clamped - currentSens) < 1f) clamped = currentSens;

        return clamped;
    }

    // --- helpers ---

    private TaskHistory LoadHist(string key)
    {
        if (cache.TryGetValue(key, out var h)) return h;
        var hist = HistoryIO.Load($"HIST_{key}");
        cache[key] = hist;
        return hist;
    }

    private void SaveHist(string key, TaskHistory hist)
    {
        HistoryIO.Save($"HIST_{key}", hist);
        cache[key] = hist;
    }

    // Build your φ vector. Keep it stable and small.
    private float[] BuildFeatures(TaskContext ctx)
    {
        // Example 8-D feature vector. Replace with what you actually have:
        return new float[]
        {
            1f,                         // bias
            ctx.targetSizeNorm,         // 0..1
            ctx.avgZNorm,               // 0..1
            ctx.trackingSpeedNorm,      // 0..1
            ctx.dpiNorm,                // 0..1
            ctx.hzNorm,                 // 0..1
            ctx.lastAcc,                // 0..1
            Mathf.Clamp01(Mathf.Abs(ctx.lastSignedErr)/15f) // normalized error
        };
    }
}
