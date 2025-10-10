using UnityEngine;

public class FlickshotStats : MonoBehaviour
{
    public static FlickshotStats Instance { get; private set; }

    [SerializeField] private FlickShotManager manager;

    public int   Hits   { get; private set; }
    public int   Misses { get; private set; }
    public float AvgRT  { get; private set; }

    private int   _rtCount = 0;
    private float _rtSum   = 0f;

    void Awake()
    {
        Instance = this;
        if (!manager) manager = FindObjectOfType<FlickShotManager>();
        ResetRun();
    }

    void OnEnable()
    {
        UnfixedZTarget.OnTargetHit += OnHit;
        TargetShooter.OnTargetMissed += OnMiss;
    }

    void OnDisable()
    {
        UnfixedZTarget.OnTargetHit -= OnHit;
        TargetShooter.OnTargetMissed -= OnMiss;
    }

    public void ResetRun()
    {
        Hits = 0; Misses = 0;
        _rtCount = 0; _rtSum = 0f;
        AvgRT = 0f;
    }

    private void OnHit()
    {
        Hits++;
        if (manager != null)
        {
            float rt = Mathf.Max(0f, Time.time - manager.LastSpawnTime);
            _rtSum += rt; _rtCount++;
            AvgRT = _rtCount > 0 ? _rtSum / _rtCount : 0f;
        }
    }

    private void OnMiss()
    {
        Misses++;
    }
}
