using UnityEngine;

public static class SensitivityOptimizer
{
    public static float MinSens = 40f;
    public static float MaxSens = 220f;

    public static float K_signed_base   = 0.15f;
    public static float K_accuracy_base = 20f;   
    public static float TargetAcc       = 0.90f;  

    public static float VarDampen = 0.4f;       

    public static int   warmupRuns     = 8;      
    public static float warmupMaxBoost = 3.0f;   
    public static float warmupMinBoost = 1.0f;    

    public static float Update(float currentSens, float meanSignedDeg, float meanAbsDeg, float accuracy, int runsSoFar)
    {
        float t = Mathf.Clamp01(runsSoFar / Mathf.Max(1f, (float)warmupRuns));
        float boost = Mathf.Lerp(warmupMaxBoost, warmupMinBoost, t);

        float K_signed   = K_signed_base   * boost;
        float K_accuracy = K_accuracy_base * boost;

        float signedTerm = -K_signed * meanSignedDeg;

        float accTerm = K_accuracy * (accuracy - TargetAcc);

        float damp = Mathf.Clamp01(1f - VarDampen * Mathf.InverseLerp(0f, 10f, meanAbsDeg));

        float delta = damp * (signedTerm + accTerm);

        return Mathf.Clamp(currentSens + delta, MinSens, MaxSens);
    }
}
