using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SinusoidalStrafeMover : MonoBehaviour
{
    [Header("Axis Settings")]
    [Tooltip("Check X if you want horizontal sine movement. Check Y for vertical sine movement.")]
    [SerializeField] private bool moveAlongX = true;
    [SerializeField] private bool moveAlongY = false;

    [Header("Wave Parameters")]
    [Tooltip("How far (in world units) the sphere travels from its center (peak of sine).")]
    [SerializeField] private float amplitude = 3f;

    [Tooltip("How many full sine cycles per second (Hz).")]
    [SerializeField] private float frequency = 0.5f;

    private Vector3 _startPos; 
    private float _omega;     
    private float _fixedZ;      
    void Start()
    {
        _startPos = transform.position;
        _fixedZ = _startPos.z;

        _omega = 2f * Mathf.PI * frequency;
    }

    void Update()
    {
        float t = Time.time;

        Vector3 pos = _startPos;

        float offset = amplitude * Mathf.Sin(_omega * t);

        if (moveAlongX)
            pos.x += offset;
        if (moveAlongY)
            pos.y += offset;

        pos.z = _fixedZ;

        transform.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.cyan;
        if (moveAlongX)
        {
            Vector3 left = new Vector3(_startPos.x - amplitude, _startPos.y, _fixedZ);
            Vector3 right = new Vector3(_startPos.x + amplitude, _startPos.y, _fixedZ);
            Gizmos.DrawLine(left, right);
        }
        if (moveAlongY)
        {
            Vector3 up = new Vector3(_startPos.x, _startPos.y + amplitude, _fixedZ);
            Vector3 down = new Vector3(_startPos.x, _startPos.y - amplitude, _fixedZ);
            Gizmos.DrawLine(down, up);
        }
    }
}
