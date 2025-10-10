using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class EntryTarget : MonoBehaviour
{
    public static event Action OnTargetHit;

    [Header("Movement")]
    [Tooltip("Units per second horizontally")]
    [SerializeField] private float speed = 5f;

    [Header("Spawn X Range")]
    [Tooltip("Minimum X coordinate when spawning")]
    [SerializeField] private float spawnXMin = -3f;
    [Tooltip("Maximum X coordinate when spawning")]
    [SerializeField] private float spawnXMax =  3f;

    private float _baseY;
    private float _baseZ;

    private int _direction = 1; 
    private int _hitCount  = 0;

    private Vector3 _boundsCenter;
    private Vector3 _boundsHalfSize;

    void Start()
    {
        _baseY = transform.position.y;
        _baseZ = transform.position.z;

        var tb  = TargetBounds.Instance;
        var col = tb.GetComponent<BoxCollider>();
        _boundsCenter   = col.center + tb.transform.position;
        _boundsHalfSize = col.size * 0.5f;

        RespawnAtRandomX();
    }

    void Update()
    {
        transform.position += Vector3.right * _direction * speed * Time.deltaTime;


        float x = transform.position.x;
        if (x > _boundsCenter.x + _boundsHalfSize.x ||
            x < _boundsCenter.x - _boundsHalfSize.x)
        {
            RespawnAtRandomX();
            _hitCount = 0;
        }
    }

    public void Hit()
    {
        OnTargetHit?.Invoke();

        _hitCount++;
        if (_hitCount >= 3)
        {
            RespawnAtRandomX();
            _hitCount = 0;
        }
        else
        {

            _direction *= -1;
        }
    }

    private void RespawnAtRandomX()
    {
        float x = UnityEngine.Random.Range(spawnXMin, spawnXMax);
        transform.position = new Vector3(x, _baseY, _baseZ);
        _direction = 1;
    }
}
