// EntryScoreCounter.cs
using UnityEngine;
using TMPro;

public class EntryScoreCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public static int Score { get; private set; }

    void OnEnable()
    {
        Score = 0;
        text.text = "Score: 0";
        EntryTarget.OnTargetHit += HandleHit;
    }

    void OnDisable()
    {
        EntryTarget.OnTargetHit -= HandleHit;
    }

    private void HandleHit()
    {
        Score++;
        text.text = $"Score: {Score}";
    }
}
