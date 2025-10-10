using UnityEngine;

[DisallowMultipleComponent]
public class TrackableTarget : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    public Vector3 Position => transform.position;

    [SerializeField] private float velocitySmoothing = 0.2f;
    private Vector3 _lastPos;
    private bool _first = true;

    void LateUpdate()
    {
        if (_first)
        {
            _lastPos = transform.position;
            Velocity = Vector3.zero;
            _first = false;
            return;
        }

        Vector3 rawVel = (transform.position - _lastPos) / Mathf.Max(Time.deltaTime, 1e-6f);
        Velocity = Vector3.Lerp(Velocity, rawVel, velocitySmoothing);
        _lastPos = transform.position;
    }
}
