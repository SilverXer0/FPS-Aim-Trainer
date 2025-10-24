using UnityEngine;

public static class Utility
{
    public static float FlickUtility(float accuracy01, float meanSignedDeg, float meanAbsDeg,
                                     float wAcc=1.0f, float wAbs=0.5f, float wBias=0.3f, float degCap=15f)
    {
        float absNorm  = Mathf.Clamp01(meanAbsDeg / degCap);
        float biasNorm = Mathf.Clamp01(Mathf.Abs(meanSignedDeg) / degCap);
        float u = wAcc * accuracy01
                  - wAbs  * absNorm
                  - wBias * biasNorm;
        return Mathf.Clamp(u, -1f, 1f);
    }

    public static float TrackingUtility(float onTargetRatio01, float w=1.0f)
    {
        return Mathf.Clamp(w * onTargetRatio01, -1f, 1f);
    }

    public static float ReactionUtility(float avgRT, float target=0.25f, float max=0.75f, float w=0.6f)
    {
        if (avgRT <= target) return Mathf.Clamp(w * 1f, -1f, 1f);
        float norm = Mathf.Clamp01((avgRT - target) / Mathf.Max(0.001f, max-target));
        return Mathf.Clamp(w * (1f - norm), -1f, 1f);
    }
}
