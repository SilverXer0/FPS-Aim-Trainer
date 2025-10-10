using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class EightDirRandomMover : MonoBehaviour
{
    [Header("Movement Bounds (in world units)")]
    [Tooltip("How far left/right from the start X the sphere can travel.")]
    [SerializeField] private float halfWidth = 3f;

    [Tooltip("How far up/down from the start Y the sphere can travel.")]
    [SerializeField] private float halfHeight = 2f;

    [Header("Speed & Timing")]
    [Tooltip("Movement speed in units/sec.")]
    [SerializeField] private float speed = 2f;

    [Tooltip("Minimum seconds to hold one direction before picking a new one.")]
    [SerializeField] private float minDirectionInterval = 1f;

    [Tooltip("Maximum seconds to hold one direction before picking a new one.")]
    [SerializeField] private float maxDirectionInterval = 3f;

    private float _fixedZ;

    private Vector2 _centerXY;

    private Vector2 _dir;

    private Transform _tf;

    void Start()
    {
        _tf = transform;

        Vector3 start = _tf.position;
        _centerXY = new Vector2(start.x, start.y);
        _fixedZ = start.z;

        PickNewDirection();
        StartCoroutine(DirectionPickerCoroutine());
    }

    void Update()
    {
        Vector3 pos = _tf.position;

        pos.x += _dir.x * speed * Time.deltaTime;
        pos.y += _dir.y * speed * Time.deltaTime;
        pos.z = _fixedZ; 

        float dx = pos.x - _centerXY.x;
        float dy = pos.y - _centerXY.y;

        bool hitBound = false;

        if (dx > halfWidth) { pos.x = _centerXY.x + halfWidth; hitBound = true; }
        else if (dx < -halfWidth) { pos.x = _centerXY.x - halfWidth; hitBound = true; }

        if (dy > halfHeight) { pos.y = _centerXY.y + halfHeight; hitBound = true; }
        else if (dy < -halfHeight) { pos.y = _centerXY.y - halfHeight; hitBound = true; }

        _tf.position = pos;

        if (hitBound)
        {
            PickNewDirection();
        }
    }

    private IEnumerator DirectionPickerCoroutine()
    {
        while (true)
        {
            float wait = Random.Range(minDirectionInterval, maxDirectionInterval);
            yield return new WaitForSeconds(wait);
            PickNewDirection();
        }
    }

    private void PickNewDirection()
    {
        Vector2[] eightDirs = new Vector2[]
        {
            new Vector2( 1,  0),
            new Vector2(-1,  0),
            new Vector2( 0,  1),
            new Vector2( 0, -1),
            new Vector2( 1,  1).normalized,
            new Vector2(-1,  1).normalized,
            new Vector2( 1, -1).normalized,
            new Vector2(-1, -1).normalized
        };
        int idx = Random.Range(0, eightDirs.Length);
        _dir = eightDirs[idx];
    }

    void OnDrawGizmosSelected()
    {

        Vector3 center = transform.position;
        if (Application.isPlaying)
            center = new Vector3(_centerXY.x, _centerXY.y, _fixedZ);

        Gizmos.color = Color.yellow;
        Vector3 size = new Vector3(halfWidth * 2f, halfHeight * 2f, 0.01f);
        Gizmos.DrawWireCube(center, size);
    }
}
