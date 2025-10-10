
using UnityEngine;
using TMPro;

public class EntryMissCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public static int Misses { get; private set; }

    void OnEnable()
    {
        Misses = 0;
        text.text = "Misses: 0";
        EntryShooter.OnEntryMiss += HandleMiss;
    }

    void OnDisable()
    {
        EntryShooter.OnEntryMiss -= HandleMiss;
    }

    private void HandleMiss()
    {
        Misses++;
        text.text = $"Misses: {Misses}";
    }
}
