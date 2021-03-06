﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuContoller : MonoBehaviour {

	public AudioSource backgroundMusic;
	public float quitingTime = 2.0f;
	public Image darkeningImage;

	private float timer = 0.0f;
	private bool needsQuit = false;

	public delegate void QuitAction();
	QuitAction onQuitAction;

	// Public methods


	public void StartSingleGame () {
		BeginQuitingWithAction (LoadSingGameScene);
	}

	public void StartMultiplayer () {
		BeginQuitingWithAction (LoadMultiplayerLobby);
	}

	public void StopGameToMainMenu () {
		BeginQuitingWithAction (LoadMainMenuScene);
	}

	public void QuitGame ()
	{
		#if UNITY_EDITOR 
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	//Private methods

	void FixedUpdate () {
		if (needsQuit) {
			timer += Time.deltaTime;
			if (backgroundMusic)
				backgroundMusic.volume = Mathf.Lerp (backgroundMusic.volume, 0.0f, quitingTime * Time.deltaTime);

			if (darkeningImage)
				darkeningImage.color = Color.Lerp (darkeningImage.color, Color.black, quitingTime * Time.deltaTime);
			
			if (timer >= quitingTime) {
				onQuitAction ();
			}
		}
	}

	void BeginQuitingWithAction (QuitAction action) {
		onQuitAction = action;
		if (darkeningImage)
			darkeningImage.gameObject.SetActive (true);
		needsQuit = true;
	}

	void LoadSingGameScene() {
		SceneManager.LoadScene ("SingleGame");
	}

	void LoadMainMenuScene() {
		SceneManager.LoadScene ("MainMenu");
	}

	void LoadMultiplayerLobby () {
		SceneManager.LoadScene ("Lobby");
	}

}
