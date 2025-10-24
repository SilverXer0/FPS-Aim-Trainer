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

    [Header("Audio (continuous loop while tracking)")]
    [Tooltip("Loopable clip that plays continuously while on target.")]
    [SerializeField] private AudioClip trackingLoopClip;
    [Range(0f, 1f)] [SerializeField] private float trackingLoopVolume = 0.6f;

    private AudioSource _loopSrc;      // dedicated looping source
    private bool _onTarget;            // state this frame

    // Scorekeeping
    private float _goodTime = 0f;
    private float _totalTime = 0f;
    private float _score = 0f;
    private bool _alreadyCalculated = false;

    void Awake()
    {
        // Build a private AudioSource for the continuous loop
        _loopSrc = gameObject.AddComponent<AudioSource>();
        _loopSrc.clip = trackingLoopClip;
        _loopSrc.loop = true;
        _loopSrc.playOnAwake = false;
        _loopSrc.spatialBlend = 0f;           // 2D
        _loopSrc.volume = trackingLoopVolume;
        _loopSrc.ignoreListenerPause = false; // so it pauses with the game if you pause AudioListener
    }

    void Start()
    {
        if (finalAccuracyText) finalAccuracyText.gameObject.SetActive(false);
        if (scoreText) scoreText.text = "Score: 0";
        if (liveAccuracyText) liveAccuracyText.text = "Accuracy: 0.0%";
    }

    void OnEnable()
    {
        Timer.OnGameEnded += OnTaskEnded;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= OnTaskEnded;
        StopLoop();
    }

    void Update()
    {
        if (Timer.GameEnded)
        {
            StopLoop();
            return;
        }

        _totalTime += Time.deltaTime;

        // Determine if we are "tracking" this frame: LMB held and ray hits the target sphere
        bool onTargetThisFrame = false;
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider && hit.collider.gameObject == strafeSphere)
                {
                    onTargetThisFrame = true;
                    _goodTime += Time.deltaTime;
                }
            }
        }

        // Start/stop the continuous loop exactly on state transitions
        if (onTargetThisFrame && !_onTarget)
            StartLoop();
        else if (!onTargetThisFrame && _onTarget)
            StopLoop();

        _onTarget = onTargetThisFrame;

        // Scoring
        if (onTargetThisFrame)
        {
            _score += pointsPerSecond * Time.deltaTime;
            if (scoreText) scoreText.text = $"Score: {Mathf.FloorToInt(_score)}";
        }

        // Live accuracy
        float liveAccPercent = (_totalTime > 0f) ? (_goodTime / _totalTime) * 100f : 0f;
        if (liveAccuracyText) liveAccuracyText.text = $"Accuracy: {liveAccPercent:0.0}%";
    }

    private void OnTaskEnded()
{
    if (_alreadyCalculated) return;
    _alreadyCalculated = true;

    float finalAcc = 0f;
    if (_totalTime > 0f)
        finalAcc = (_goodTime / _totalTime);

    if (StrafeTrackStats.Instance != null)
        StrafeTrackStats.Instance.OnTargetRatio01 = finalAcc;

    finalAccuracyText.text = $"Final Accuracy: {finalAcc * 100f:0.0}%";
    finalAccuracyText.gameObject.SetActive(true);
    liveAccuracyText.gameObject.SetActive(false);
}


    private void StartLoop()
    {
        if (_loopSrc && _loopSrc.clip && !_loopSrc.isPlaying)
            _loopSrc.Play();
    }

    private void StopLoop()
    {
        if (_loopSrc && _loopSrc.isPlaying)
            _loopSrc.Stop();
    }
}
