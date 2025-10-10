using UnityEngine;

[RequireComponent(typeof(Target))]
public class HeadshotTarget : MonoBehaviour
{
    [Header("Horizontal Spawn Settings")]
    [Tooltip("Start on the left half if true, otherwise start on the right")]
    public bool startOnLeft = true;

   

    private bool _spawnLeft;
    private BoxCollider _col;
    private Vector3 _center;
    private float _halfSizeX;

    private float _fixedY;  
    private float _fixedZ;   
    void OnEnable()
    {
        Target.OnTargetHit += OnTargetHit;
    }

    void OnDisable()
    {
        Target.OnTargetHit -= OnTargetHit;
    }

    void Start()
    {

        _fixedY = transform.position.y;
        _fixedZ = transform.position.z;

        _spawnLeft = startOnLeft;


        var tb = TargetBounds.Instance;
        _col = tb.GetComponent<BoxCollider>();
        _center = _col.center + tb.transform.position;
        _halfSizeX = _col.size.x * 0.5f;


        Teleport();
    }

    private void OnTargetHit()
    {
        Teleport();
    }

    private void Teleport()
    {

        float x = _spawnLeft
            ? Random.Range(_center.x - _halfSizeX, _center.x)
            : Random.Range(_center.x,          _center.x + _halfSizeX);
        transform.position = new Vector3(x, _fixedY, _fixedZ);

        _spawnLeft = !_spawnLeft;
    }
}
