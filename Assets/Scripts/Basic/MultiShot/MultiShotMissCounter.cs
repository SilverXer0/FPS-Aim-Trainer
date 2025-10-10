using UnityEngine;
using TMPro;

public class MultiShotMissCounter : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public static int Misses { get; private set; }

    void OnEnable()
    {
        Misses = 0;
        MultiShotShooter.OnMiss += HandleMiss;
    }

    void OnDisable()
    {
        MultiShotShooter.OnMiss -= HandleMiss;
    }

    void HandleMiss()
    {
        Misses++;
        if (text) text.text = $"Misses: {Misses}";
    }
}
