using UnityEngine;
using TMPro;

public class FlickShotManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject target;        
    [SerializeField] private Camera cam;
    [SerializeField] private TargetBounds bounds;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestFlickText;
    [SerializeField] private TMP_Text avgFlickText;

    private int    _score = 0;
    private float  _spawnTime;
    private float  _bestFlick = float.PositiveInfinity;
    private float  _totalFlickTime = 0f;
    private int    _flickCount = 0;

    public float LastSpawnTime => _spawnTime;
    public float AverageFlickTime => _flickCount > 0 ? _totalFlickTime / _flickCount : 0f;
    public int   Score => _score;

    void OnEnable()
    {
        UnfixedZTarget.OnTargetHit += OnTargetHit;
    }

    void OnDisable()
    {
        UnfixedZTarget.OnTargetHit -= OnTargetHit;
    }

    void Start()
    {
        scoreText.text     = $"Score: 0";
        bestFlickText.text = $"Best Flick: –";
        avgFlickText.text  = $"Avg Flick: –";

        SpawnNewTarget();
    }

    private void SpawnNewTarget()
    {
        Vector3 randomPos = bounds.GetRandomPosition();
        target.transform.position = randomPos;
        _spawnTime = Time.time;                
    }

    private void OnTargetHit()
    {
        float flickTime = Time.time - _spawnTime;

        Vector3 toTarget = (target.transform.position - cam.transform.position).normalized;
        float angleDeg = Vector3.Angle(cam.transform.forward, toTarget);

        _score++;
        _flickCount++;
        _totalFlickTime += flickTime;
        if (flickTime < _bestFlick) _bestFlick = flickTime;

        scoreText.text     = $"Score: {_score}";
        bestFlickText.text = $"Best Flick: {_bestFlick:0.00}s";
        float avg = _totalFlickTime / _flickCount;
        avgFlickText.text  = $"Avg Flick: {avg:0.00}s";

        SpawnNewTarget();
    }
}
