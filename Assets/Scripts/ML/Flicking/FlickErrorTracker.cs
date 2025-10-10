using UnityEngine;

public class FlickErrorTracker : MonoBehaviour
{
    public static FlickErrorTracker Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private Camera cam;

    public int Shots { get; private set; }
    public int Hits  { get; private set; }

    public float MeanSignedDeg { get; private set; }
    public float MeanAbsDeg { get; private set; }

    [Range(0f,1f)] public float emaAlpha = 0.15f;

    void Awake()
    {
        Instance = this;
        if (!cam) cam = Camera.main;
        ResetRun();
    }

    public void ResetRun()
    {
        Shots = 0; Hits = 0;
        MeanSignedDeg = 0f;
        MeanAbsDeg    = 0f;
    }

    public void RegisterShot(RaycastHit? hitInfo, Vector3 targetWorldPos)
    {
        Shots++;

        Ray aimRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 toTarget = (targetWorldPos - cam.transform.position).normalized;

        float angleDeg = Vector3.Angle(aimRay.direction, toTarget);

        float sign = Mathf.Sign(Vector3.Dot(Vector3.Cross(aimRay.direction, toTarget), cam.transform.right));
        float signedDeg = angleDeg * sign;

        MeanSignedDeg = Mathf.Lerp(MeanSignedDeg, signedDeg, emaAlpha);
        MeanAbsDeg    = Mathf.Lerp(MeanAbsDeg, Mathf.Abs(signedDeg), emaAlpha);

        if (hitInfo.HasValue)
        {
            var h = hitInfo.Value;
            if (h.collider && h.collider.GetComponent<Target>() != null) Hits++;
        }
    }
}
