using UnityEngine;

public class TrackingErrorTracker : MonoBehaviour
{
    public static TrackingErrorTracker Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private Camera cam;
    [SerializeField] private TrackableTarget target;

    [Header("Sampling")]
    [Tooltip("If true, only sample while LMB is held.")]
    public bool requireMouseHeld = true;

    [Header("Time-on-target")]
    [Tooltip("Viewport radius considered 'on target' (0..0.5). ~0.01 ≈ 1% of screen width.")]
    public float onTargetRadiusViewport = 0.012f;

    [Header("Smoothing")]
    [Range(0f,1f)] public float emaAlpha = 0.15f;
    
    public float TrackingAccuracy01 { get; private set; }      
    public float MeanSignedAlongMotion { get; private set; }   
    public float MeanAbsAlongMotion    { get; private set; }

    float _trackedTime = 0f, _onTargetTime = 0f;

    void Awake()
    {
        Instance = this;
        if (!cam) cam = Camera.main;
        ResetRun();
    }

    public void SetTarget(TrackableTarget t) => target = t; 

    public void ResetRun()
    {
        TrackingAccuracy01 = 0f;
        MeanSignedAlongMotion = 0f;
        MeanAbsAlongMotion = 0f;
        _trackedTime = 0f;
        _onTargetTime = 0f;
    }

    void Update()
    {
        if (Timer.GameEnded || !target || !cam) return;
        if (requireMouseHeld && !Input.GetMouseButton(0)) return;

        Vector3 vp = cam.WorldToViewportPoint(target.Position);
        if (vp.z <= 0f) return;

        Vector2 center = new Vector2(0.5f, 0.5f);
        Vector2 pos2   = new Vector2(vp.x, vp.y);
        Vector2 err2   = pos2 - center;

        Vector3 worldAhead = target.Position + target.Velocity * Time.deltaTime;
        Vector3 vpAhead    = cam.WorldToViewportPoint(worldAhead);
        Vector2 v2         = (new Vector2(vpAhead.x, vpAhead.y) - pos2) / Mathf.Max(Time.deltaTime, 1e-6f);
        float speed        = v2.magnitude;

        float dt = Time.deltaTime;
        _trackedTime += dt;
        if (err2.magnitude <= onTargetRadiusViewport) _onTargetTime += dt;
        TrackingAccuracy01 = _trackedTime > 0f ? _onTargetTime / _trackedTime : 0f;

        if (speed < 1e-4f) return; 
        Vector2 vdir = v2.normalized;
        float signed = Vector2.Dot(err2, vdir); 
        float absVal = Mathf.Abs(signed);

        MeanSignedAlongMotion = Mathf.Lerp(MeanSignedAlongMotion, signed, emaAlpha);
        MeanAbsAlongMotion    = Mathf.Lerp(MeanAbsAlongMotion,    absVal,  emaAlpha);
    }
}
