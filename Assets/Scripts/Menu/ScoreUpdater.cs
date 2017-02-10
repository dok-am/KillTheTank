﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdater : MonoBehaviour {

	public GameManager gameManager;
	public int playerNumber = 1;

	private Text text;

	void Awake () {
		text = GetComponent<Text> ();
	}

	void FixedUpdate () {
		text.text = "Score: " + gameManager.GetScoreForPlayer (playerNumber);
	}
}
