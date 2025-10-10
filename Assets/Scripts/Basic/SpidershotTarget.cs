using System;
using UnityEngine;

[RequireComponent(typeof(Target))]
public class SpidershotTarget : MonoBehaviour
{
    private enum Phase { Center, Left, Center2, Right }
    private Phase _phase = Phase.Center;

    private BoxCollider _bounds;
    private Vector3 _center;
    private float _halfX;
    private float _minY, _maxY, _minZ, _maxZ;

    void OnEnable()
    {
        Target.OnTargetHit += OnHit;
    }

    void OnDisable()
    {
        Target.OnTargetHit -= OnHit;
    }

    void Start()
    {

        var tb = TargetBounds.Instance;
        _bounds = tb.GetComponent<BoxCollider>();
        Vector3 bCenter = _bounds.center + tb.transform.position;
        _center = bCenter;
        _halfX  = _bounds.size.x * 0.5f;
        _minY   = bCenter.y - _bounds.size.y * 0.5f;
        _maxY   = bCenter.y + _bounds.size.y * 0.5f;
        _minZ   = bCenter.z - _bounds.size.z * 0.5f;
        _maxZ   = bCenter.z + _bounds.size.z * 0.5f;

        Teleport(); 
    }

    private void OnHit()
    {
        Teleport();
    }

    private void Teleport()
    {
        Vector3 pos = Vector3.zero;
        switch (_phase)
        {
            case Phase.Center:
            case Phase.Center2:
                pos.x = _center.x;
                pos.y = UnityEngine.Random.Range(_minY, _maxY);
                pos.z = UnityEngine.Random.Range(_minZ, _maxZ);
                break;

            case Phase.Left:
                pos.x = UnityEngine.Random.Range(_center.x - _halfX, _center.x);
                pos.y = UnityEngine.Random.Range(_minY, _maxY);
                pos.z = UnityEngine.Random.Range(_minZ, _maxZ);
                break;

            case Phase.Right:
                pos.x = UnityEngine.Random.Range(_center.x, _center.x + _halfX);
                pos.y = UnityEngine.Random.Range(_minY, _maxY);
                pos.z = UnityEngine.Random.Range(_minZ, _maxZ);
                break;
        }

        transform.position = pos;

        _phase = (Phase)(((int)_phase + 1) % 4);
    }
}
