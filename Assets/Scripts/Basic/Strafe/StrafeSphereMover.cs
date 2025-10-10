using UnityEngine;

public class StrafeSphereMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed in units/sec")]
    public float speed = 2f;

    [Tooltip("Maximum distance left/right from center on X axis")]
    public float halfWidth = 3f;

    private float _direction = 1f;  
    private Vector3 _startPos;      

    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        float dx = _direction * speed * Time.deltaTime;
        transform.position += new Vector3(dx, 0, 0);

        float offsetX = transform.position.x - _startPos.x;
        if (offsetX >= halfWidth)
        {
            _direction = -1f;
        }
        else if (offsetX <= -halfWidth)
        {
            _direction = +1f;
        }
    }
}
