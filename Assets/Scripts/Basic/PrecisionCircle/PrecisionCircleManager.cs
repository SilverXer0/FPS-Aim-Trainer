using UnityEngine;
using TMPro;
using System;

public class PrecisionCircleManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The moving sphere that runs CircleMover")]
    public CircleMover mover;

    [Tooltip("Camera for raycasting")]
    public Camera cam;

    [Header("Zone Settings")]
    [Tooltip("Target angle in degrees where click is 'on time' (0° = +X axis, increasing CCW)")]
    public float targetAngleDeg = 90f;

    [Tooltip("Angular tolerance in degrees (+/–) around targetAngle")]
    public float toleranceDeg = 10f;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text missText;
    public TMP_Text timerText;       
    public TMP_Text finalTimingText;  

    [Header("Timing")]
    public float taskDuration = 60f;

    [Header("Difficulty Ramp")]
    [Tooltip("How many radians/sec to add to angularSpeed on each hit.")]
    public float speedIncrement = 0.2f;

    [Tooltip("Maximum angular speed (radians/sec) the circle can reach.")]
    public float maxAngularSpeed = 20f;


    private float _hits = 0f;
    private float _misses = 0f;

    void Start()
    {
        scoreText.text = "Hits: 0";
        missText.text  = "Misses: 0";
        finalTimingText.gameObject.SetActive(false);


    }

   void Update()
{
    if (Timer.GameEnded) return;
    if (!Input.GetMouseButtonDown(0)) return;

    float currentDeg = mover.CurrentAngle * Mathf.Rad2Deg;
    currentDeg = (currentDeg + 360f) % 360f;
    float delta    = Mathf.DeltaAngle(currentDeg, targetAngleDeg);
    float absError = Mathf.Abs(delta);

        if (absError <= toleranceDeg)
        {
            _hits++;
            scoreText.text = $"Hits: {_hits}";
            mover.angularSpeed += speedIncrement;

   
            mover.angularSpeed = Mathf.Min(mover.angularSpeed, maxAngularSpeed);
    }
        else
        {

            _misses++;
            missText.text = $"Misses: {_misses}";
        }
}


    void OnEnable()
    {
        Timer.OnGameEnded += OnTaskEnd;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= OnTaskEnd;
    }

    private void OnTaskEnd()
    {
        if (_hits > 0)
        {
            float accuracy = (_hits / (_hits + _misses)) * 100;
            finalTimingText.text = $"Accuracy: {accuracy}%";
        }
        else
        {
            finalTimingText.text = "No Hits!";
        }
        finalTimingText.gameObject.SetActive(true);
    }
}
