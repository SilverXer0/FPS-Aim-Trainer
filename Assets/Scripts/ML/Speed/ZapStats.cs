using UnityEngine;

public class ZapStats : MonoBehaviour
{
    public static ZapStats Instance { get; private set; }
    public int Hits { get; private set; }
    public int Misclicks { get; private set; }
    public float AvgRT { get; private set; }

    int _n; float _sum;

    void Awake(){ Instance = this; ResetRun(); }

    void OnEnable()
    {
        ZapShooter.OnZapHit     += OnHit;
        ZapShooter.OnZapMisclick += OnMiss;
    }
    void OnDisable()
    {
        ZapShooter.OnZapHit     -= OnHit;
        ZapShooter.OnZapMisclick -= OnMiss;
    }

    public void ResetRun(){ Hits=0; Misclicks=0; _n=0; _sum=0f; AvgRT=0f; }

    void OnHit(float rt)
    {
        Hits++;
        _sum += Mathf.Max(0f, rt);
        _n++;
        AvgRT = _n>0 ? _sum/_n : 0f;
    }
    void OnMiss(){ Misclicks++; }
}
