using UnityEngine;
using TMPro;

public class EntryAccuracyCalculator : MonoBehaviour
{
    [SerializeField] private TMP_Text accuracyText;

    void OnEnable()
    {
        Timer.OnGameEnded += ShowAccuracy;
    }

    void OnDisable()
    {
        Timer.OnGameEnded -= ShowAccuracy;
    }

    private void ShowAccuracy()
    {
        int hits   = EntryScoreCounter.Score;
        int misses = EntryMissCounter.Misses;
        float acc  = (hits + misses) > 0
                   ? 100f * hits / (hits + misses)
                   : 0f;
        accuracyText.text = $"Accuracy: {acc:0}%";
    }
}
