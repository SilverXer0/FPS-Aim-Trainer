using UnityEngine;

public class SensitivityOptimizerConfig : MonoBehaviour
{
    public float minSens=40, maxSens=220;
    public float kSigned=0.20f, kAcc=25f, varDampen=0.4f;
    public float targetAcc=0.90f;
    public int warmupRuns=8;
    public float warmupMaxBoost=3f, warmupMinBoost=1f;

    void Awake(){
        SensitivityOptimizer.MinSens = minSens;
        SensitivityOptimizer.MaxSens = maxSens;
        SensitivityOptimizer.K_signed_base = kSigned;
        SensitivityOptimizer.K_accuracy_base = kAcc;
        SensitivityOptimizer.VarDampen = varDampen;
        SensitivityOptimizer.TargetAcc = targetAcc;
        SensitivityOptimizer.warmupRuns = warmupRuns;
        SensitivityOptimizer.warmupMaxBoost = warmupMaxBoost;
        SensitivityOptimizer.warmupMinBoost = warmupMinBoost;
    }
}
