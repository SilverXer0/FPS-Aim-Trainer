using UnityEngine;
using TMPro;

public class StrafeTrackScorer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your moving sphere (StrafeSphere) here.")]
    public GameObject strafeSphere;

    [Tooltip("The camera for raycasting (usually the Main Camera).")]
    public Camera cam;

    [Tooltip("UI Text to show running score (e.g. 'Score: 123').")]
    public TMP_Text scoreText;

    [Tooltip("UI Text to show running accuracy (e.g. 'Accuracy: 85.3%').")]
    public TMP_Text liveAccuracyText;

    [Tooltip("UI Text to show final accuracy once task ends.")]
    public TMP_Text finalAccuracyText;

    [Header("Scoring Settings")]
    [Tooltip("Points per second of holding on target.")]
    public float pointsPerSecond = 10f;

    [Header("Audio")]
    [Tooltip("Play repeated ticks while holding on target (instead of just once).")]
    [SerializeField] private bool playTickWhileOnTarget = false;
    [Tooltip("Seconds between repeated tick sounds while holding on target.")]
    [SerializeField] private float tickInterval = 0.15f;

    private bool _wasOnTarget = false;
    private float _tickTimer = 0f;

    private float _goodTime = 0f;
    private float _totalTime = 0f;
    private float _score = 0f;

    private bool _alreadyCalculated = false;

    void Start()
    {
        finalAccuracyText.gameObject.SetActive(false);

        scoreText.text = $"Score: 0";
        liveAccuracyText.text = $"Accuracy: 0.0%";
    }

    void OnEnable()
    {
        Timer.OnGameEnded += OnTaskEnded;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= OnTaskEnded;
    }

    void Update()
    {
        if (Timer.GameEnded)
            return;

        _totalTime += Time.deltaTime;

        bool onTargetThisFrame = false;

        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == strafeSphere)
                {
                    _goodTime += Time.deltaTime;
                    onTargetThisFrame = true;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (onTargetThisFrame && !_wasOnTarget)
            {
                AudioHub.Instance?.PlayHit();
                _tickTimer = 0f;
            }

            if (onTargetThisFrame && playTickWhileOnTarget)
            {
                _tickTimer += Time.deltaTime;
                if (_tickTimer >= tickInterval)
                {
                    AudioHub.Instance?.PlayHit(); 
                    _tickTimer = 0f;
                }
            }
        }

        _wasOnTarget = onTargetThisFrame;

        if (onTargetThisFrame)
        {
            _score += pointsPerSecond * Time.deltaTime;
            scoreText.text = $"Score: {Mathf.FloorToInt(_score)}";
        }

        float liveAccPercent = (_totalTime > 0f)
            ? (_goodTime / _totalTime) * 100f
            : 0f;

        liveAccuracyText.text = $"Accuracy: {liveAccPercent:0.0}%";
    }

    private void OnTaskEnded()
    {
        if (_alreadyCalculated) return;
        _alreadyCalculated = true;

        float finalAcc = (_totalTime > 0f) ? (_goodTime / _totalTime) * 100f : 0f;

        finalAccuracyText.text = $"Final Accuracy: {finalAcc:0.0}%";
        finalAccuracyText.gameObject.SetActive(true);
        liveAccuracyText.gameObject.SetActive(false);
    }
}
