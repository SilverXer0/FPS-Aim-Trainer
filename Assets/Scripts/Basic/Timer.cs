using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public static Action OnGameEnded;
	public static bool GameEnded { get; private set; }

	[SerializeField] TMP_Text timerText;

	float endTime;

	const float gameTime = 60f;

	void Start()
	{
		GameEnded = false;
		endTime = Time.time + gameTime;
	}

	void Update()
	{
		if(GameEnded)
			return;

		float timeLeft = endTime - Time.time;

		if(timeLeft <= 0)
		{
			GameEnded = true;
			OnGameEnded?.Invoke();

			timeLeft = 0;
		}

		timerText.text = $"Time Left: {timeLeft.ToString("0.0")}";
	}
}
