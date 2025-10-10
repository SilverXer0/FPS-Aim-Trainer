using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AccuracyCalculator : MonoBehaviour
{
	[SerializeField] TMP_Text accuracyText;

	void OnEnable()
	{
		Timer.OnGameEnded += CalculateAccuracy;
	}

	void OnDisable()
	{
		Timer.OnGameEnded -= CalculateAccuracy;
	}

	void CalculateAccuracy()
	{
		float accuracy = (float)ScoreCounter.Score / (float)(ScoreCounter.Score + MissCounter.Misses);
		accuracy *= 100f;
		accuracyText.text = $"Accuracy: {accuracy.ToString("0")}%";
	}
}
