using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager_Net : NetworkBehaviour
{

	public GameObject mainMenu;

	//public GameObject player1;
	//public GameObject player2;

	//public Text restartButtonText;
	//public GameObject resumeGameButton;
	public Text winText;

	public bool isGameBegan = true;
	public bool isPaused = false;
	public bool isMenuShown = false;

	public float HeartAppearRate = 30.0f;
	public float NuclearAppearRate = 7.0f;

	private int Player1Score = 0;
	private int Player2Score = 0;

	private BoardManager_Net boardScript;
	private float heartTimer = 0.0f;
	private float nuclearTimer = 0.0f;

	void Awake ()
	{
		boardScript = GetComponent<BoardManager_Net> ();
		winText.enabled = false;
		//StartGame ();`
	}

	public override void OnStartServer () {
		StartGame ();
	}

	public override void OnStartLocalPlayer () {
		StartLocalGame ();
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

		bool enter = Input.GetButtonDown ("Submit");
		if (enter) {
			if (!isGameBegan && !isMenuShown) {
				StartGame ();
			}
		}
	}

	void FixedUpdate () {
		if (isGameBegan && !isPaused) {
			heartTimer += Time.deltaTime;
			nuclearTimer += Time.deltaTime;

			if (heartTimer >= HeartAppearRate) {
			//	boardScript.AddRandomHeart ();
				heartTimer = 0.0f;
			}

			if (nuclearTimer >= NuclearAppearRate) {
			//	boardScript.AddRandomNuclearPickup ();
				nuclearTimer = 0.0f;
			}
		}
	}

	void ResetPlayers () {
	/*	PlayerController player1Controller = player1.GetComponentInChildren <PlayerController> ();
		player1Controller.ResetPosition ();
		PlayerHealth player1Health = player1.GetComponentInChildren<PlayerHealth> ();
		player1Health.ResetHealth ();

		PlayerController player2Controller = player2.GetComponentInChildren <PlayerController> ();
		player2Controller.ResetPosition ();
		PlayerHealth player2Health = player2.GetComponentInChildren<PlayerHealth> ();
		player2Health.ResetHealth ();
		*/
	}

	[Server]
	public void StartGame ()
	{
		//winText.text = "";
	//	winText.enabled = false;
		boardScript.CmdSetupScene ();
	//	boardScript.RpcBoardSetup ();
		//ResetPlayers ();
		isGameBegan = true;
	//	ResumeGame ();
	}

	[Client]
	public void StartLocalGame () {
		winText.text = "";
		winText.enabled = false;
	//	boardScript.CmdSetupScene ();
		boardScript.BoardSetup ();
		//ResetPlayers ();
		isGameBegan = true;
		ResumeGame ();
	}

	public void PauseGame ()
	{
		mainMenu.SetActive (true);
		isPaused = true;
		isMenuShown = true;
	}

	public void ResumeGame () {
		mainMenu.SetActive (false);
		isPaused = false;
		isMenuShown = false;
	}

	public void FinishGame (bool firstPlayerWin) {
		isGameBegan = false;
		isPaused = true;

		string playerNum;
		if (!firstPlayerWin) {
			playerNum = "2";
			Player2Score++;
		} else {
			playerNum = "1";
			Player1Score++;
		}

		winText.text = "Player " + playerNum + " Win!\nScore " + Player1Score + " : " + Player2Score + "\nPress enter to\ncontinue";
		winText.enabled = true;
	}

	public int GetScoreForPlayer(int playerNum) {
		if (playerNum == 1)
			return Player1Score;
		if (playerNum == 2)
			return Player2Score;

		return 0;
	}

}
