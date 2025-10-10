using UnityEngine;
using TMPro;

public class ZapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ZapTarget zapTarget;     
    [SerializeField] private TargetBounds bounds;     
    [SerializeField] private Camera cam;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestReactText;
    [SerializeField] private TMP_Text avgReactText;

    private int _score;
    private float _bestReaction = float.PositiveInfinity;
    private float _totalReaction;
    private int _hitCount;

    void OnEnable()
    {
        ZapTarget.OnZapVisible += OnVisible;
        ZapShooter.OnZapHit   += OnHit;
        ZapShooter.OnZapMisclick += OnMisclick;
    }

    void OnDisable()
    {
        ZapTarget.OnZapVisible -= OnVisible;
        ZapShooter.OnZapHit   -= OnHit;
        ZapShooter.OnZapMisclick -= OnMisclick;
    }

    void Start()
    {
        _score = 0;
        scoreText.text     = $"Score: 0";
        bestReactText.text = $"Best RT: –";
        avgReactText.text  = $"Avg RT: –";

        zapTarget.transform.position = bounds.GetRandomPosition();
    }

    private void OnVisible(float timestamp)
    {

        zapTarget.transform.position = bounds.GetRandomPosition();
    }

    private void OnHit(float reactionTime)
    {
        _score++;
        _hitCount++;
        _totalReaction += reactionTime;
        if (reactionTime < _bestReaction) _bestReaction = reactionTime;

        scoreText.text     = $"Score: {_score}";
        bestReactText.text = $"Best RT: {_bestReaction:0.000}s";
        avgReactText.text  = $"Avg RT: {_totalReaction/_hitCount:0.000}s";
    }

    private void OnMisclick()
    {
      
    }
}
