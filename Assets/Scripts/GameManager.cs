using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public GameObject mainMenu;

	public GameObject player1;
	public GameObject player2;

	public Text restartButtonText;
	public GameObject resumeGameButton;
	public Text winText;

	public bool isGameBegan = false;
	public bool isPaused = true;

	private BoardManager boardScript;

	void Awake ()
	{
		boardScript = GetComponent<BoardManager> ();
		winText.enabled = false;
	}

	void Update () {
		bool escape = Input.GetButtonDown ("Cancel");
		if (escape) {
			if (isGameBegan) {
				isPaused = !isPaused;
				if (isPaused) {
					PauseGame ();
				} else {
					ResumeGame ();
				}
			} else {
				PauseGame ();
			}
		}
	}

	void ResetPlayers () {
		PlayerController player1Controller = player1.GetComponentInChildren <PlayerController> ();
		player1Controller.ResetPosition ();
		PlayerHealth player1Health = player1.GetComponentInChildren<PlayerHealth> ();
		player1Health.ResetHealth ();

		PlayerController player2Controller = player2.GetComponentInChildren <PlayerController> ();
		player2Controller.ResetPosition ();
		PlayerHealth player2Health = player2.GetComponentInChildren<PlayerHealth> ();
		player2Health.ResetHealth ();
	}

	public static bool isGamePaused() {
		GameObject gManager = GameObject.Find ("GameManager");
		return gManager.GetComponent<GameManager> ().isPaused;
	}

	public void StartGame ()
	{
		boardScript.SetupScene ();
		ResetPlayers ();
		isGameBegan = true;
		ResumeGame ();
	}

	public void PauseGame ()
	{
		winText.text = "";
		winText.enabled = false;

		if (isGameBegan) {
			restartButtonText.text = "RESTART";
			resumeGameButton.SetActive (true);
		} else {
			restartButtonText.text = "START GAME";
			resumeGameButton.SetActive (false);
		}
		mainMenu.SetActive (true);
		isPaused = true;
	}

	public void ResumeGame () {
		mainMenu.SetActive (false);
		isPaused = false;
	}

	public void FinishGame (bool firstPlayerWin) {
		isGameBegan = false;
		isPaused = true;

		string playerNum = "1";
		if (!firstPlayerWin)
			playerNum = "2";

		winText.text = "Player " + playerNum + " Win!\n\nPress esc to\ncontinue";
		winText.enabled = true;
	}

	public void QuitGame ()
	{
	#if UNITY_EDITOR 
		UnityEditor.EditorApplication.isPlaying = false;
	#else
		Application.Quit();
	#endif
	}

}
