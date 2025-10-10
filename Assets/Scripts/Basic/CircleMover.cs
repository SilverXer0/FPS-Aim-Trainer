using UnityEngine;

public class CircleMover : MonoBehaviour
{
    [Header("Circle Settings")]
    [Tooltip("Center of the circle in world space")]
    public Vector3 center = new Vector3(0f, 1.5f, 10f);

    [Tooltip("Radius of the circle")]
    public float radius = 3f;

    [Tooltip("Angular speed in radians per second (2π = one lap per second)")]
    public float angularSpeed = 2 * Mathf.PI;

    private float _angle = 0f;

    void Start()
    {

    }

    void Update()
    {
        _angle += angularSpeed * Time.deltaTime;

        if (_angle > Mathf.PI * 2f) _angle -= Mathf.PI * 2f;

        float x = center.x + radius * Mathf.Cos(_angle);
        float y = center.y + radius * Mathf.Sin(_angle);
        float z = center.z;

        transform.position = new Vector3(x, y, z);
    }

    public float CurrentAngle => _angle;
}
