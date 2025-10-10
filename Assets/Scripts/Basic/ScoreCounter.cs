using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ScoreCounter : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public static int Score { get; private set; }

    void OnEnable()
    {
        Score = 0;                               
        if (text) text.text = "Score: 0";
        Target.OnTargetHit += OnTargetHit;        
    }

    void OnDisable()
    {
        Target.OnTargetHit -= OnTargetHit;        
    }

    void OnTargetHit()
    {
        Score++;
        if (text) text.text = $"Score: {Score}";
    }
}

