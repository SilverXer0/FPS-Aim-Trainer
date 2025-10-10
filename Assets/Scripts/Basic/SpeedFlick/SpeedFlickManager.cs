using UnityEngine;
using TMPro;

public class SpeedFlickManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The single target with UnfixedZTarget on it")]
    public GameObject target;

    [Tooltip("Your camera for raycasting")]
    public Camera cam;

    [Tooltip("Bounds helper to pick random positions")]
    public TargetBounds bounds;

    [Header("Spawn Settings")]
    [Tooltip("Minimum distance from last spawn to force a genuine flick")]
    public float minSpawnDistance = 3f;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text bestTimeText;
    public TMP_Text avgTimeText;

    private int   _score;
    private float _lastSpawnTime;
    private Vector3 _lastPosition;
    private float _bestTime = float.PositiveInfinity;
    private float _totalTime;
    private int   _count;

    void OnEnable()
    {
        UnfixedZTarget.OnTargetHit += OnHit;
    }

    void OnDisable()
    {
        UnfixedZTarget.OnTargetHit -= OnHit;
    }

    void Start()
    {
        _score = 0;
        _lastPosition = Vector3.zero;
        scoreText.text    = "Score: 0";
        bestTimeText.text = "Best: –";
        avgTimeText.text  = "Avg: –";

        SpawnNext(); 
    }

    private void SpawnNext()
    {
        Vector3 pos;
        do
        {
            pos = bounds.GetRandomPosition();
        }
        while (_count > 0 && Vector3.Distance(pos, _lastPosition) < minSpawnDistance);

        target.transform.position = pos;
        _lastPosition = pos;
        _lastSpawnTime = Time.time;
    }

    private void OnHit()
    {
        float flickTime = Time.time - _lastSpawnTime;

        _score++;
        _count++;
        _totalTime += flickTime;
        if (flickTime < _bestTime) _bestTime = flickTime;

        scoreText.text = $"Score: {_score}";
        bestTimeText.text = $"Best: {_bestTime:0.00}s";
        avgTimeText.text  = $"Avg: {(_totalTime/_count):0.00}s";

        SpawnNext();
    }
}
