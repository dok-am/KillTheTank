using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager_Net : NetworkBehaviour
{

	public GameObject mainMenu;

	public Text winText;
	public Text scoreText;

	public bool isPaused = false;
	public bool isMenuShown = false;

	public float HeartAppearRate = 30.0f;
	public float NuclearAppearRate = 7.0f;

	[SyncVar(hook="Player1ScoreChanged")]
	private int Player1Score = 0;

	[SyncVar(hook="Player2ScoreChanged")]
	private int Player2Score = 0;

	private BoardManager_Net boardScript;
	private float heartTimer = 0.0f;
	private float nuclearTimer = 0.0f;

	private float restartTimer = 0.0f;

	void Awake ()
	{
		boardScript = GetComponent<BoardManager_Net> ();
		winText.enabled = false;
		scoreText.enabled = false;
	}

	public override void OnStartServer ()
	{
		StartGame ();
	}

	public override void OnStartClient ()
	{
		StartLocalGame ();
	}

	[ClientCallback]
	void Update ()
	{
		bool escape = Input.GetButtonDown ("Cancel");
		if (escape) {
			isMenuShown = !isMenuShown;
			if (isMenuShown) {
				ShowMenu ();
			} else {
				HideMenu ();
			}
		} 

		bool enter = Input.GetButtonDown ("Submit");
		if (enter && isPaused) {
			StartGame ();
		}
	}

	[ServerCallback]
	void FixedUpdate ()
	{
		heartTimer += Time.deltaTime;
		nuclearTimer += Time.deltaTime;

		if (heartTimer >= HeartAppearRate) {
			boardScript.AddRandomHeart ();
			heartTimer = 0.0f;
		}

		if (nuclearTimer >= NuclearAppearRate) {
			boardScript.AddRandomNuclearPickup ();
			nuclearTimer = 0.0f;
		}

		if (isPaused) {
			restartTimer += Time.deltaTime;
			if (restartTimer >= 10.0f) {
				RestartGame ();
				isPaused = false;
			}
		}
	}

	void ResetPlayers ()
	{
		PlayerController_Net[] players =  FindObjectsOfType<PlayerController_Net> ();

		foreach (PlayerController_Net player in players) {
			player.RpcResetPosition ();
			PlayerHealth_Net health = player.GetComponentInChildren<PlayerHealth_Net> ();
			health.RpcResetHealth ();
		}
	}

	[Server]
	void RestartGame ()
	{
		ResetPlayers ();
		boardScript.ClearBoard ();
		boardScript.ClearObjects ();
		RpcResetUIState ();
		StartGame ();
	}

	[Server]
	public void StartGame ()
	{
		boardScript.CmdSetupScene ();
		isPaused = false;
	}
		
	public void StartLocalGame ()
	{
		boardScript.BoardSetup ();
		isPaused = false;
	}

	[ClientRpc]
	void RpcResetUIState () {
		winText.text = "";
		winText.enabled = false;
		scoreText.text = "";
		scoreText.enabled = false;
	}

	[Client]
	public void ShowMenu ()
	{
		mainMenu.SetActive (true);
		isMenuShown = true;
	}

	[Client]
	public void HideMenu ()
	{
		mainMenu.SetActive (false);
		isMenuShown = false;
	}

	[Server]
	public void FinishGame (bool firstPlayerWin)
	{
		isPaused = true;
		restartTimer = 0.0f;

		string playerNum;
		if (!firstPlayerWin) {
			playerNum = "2";
			Player2Score += 1;
		} else {
			playerNum = "1";
			Player1Score += 1;
		}

		RpcShowWinTextWith (playerNum);
	}

	[ClientRpc]
	void RpcShowWinTextWith(string playerNum) {
		winText.text = "Player " + playerNum + " Win!";
		winText.enabled = true;
		UpdateScoreText ();
	}

	[Client]
	void UpdateScoreText () {
		scoreText.text = "Score " + Player1Score + " : " + Player2Score + "\nNext round\nin 10 seconds...";
		scoreText.enabled = true;
	}

	void Player1ScoreChanged(int score) {
		Player1Score = score;
		UpdateScoreText ();
	}

	void Player2ScoreChanged(int score) {
		Player2Score = score;
		UpdateScoreText ();
	}

	public int GetScoreForPlayer (int playerNum)
	{
		if (playerNum == 1)
			return Player1Score;
		if (playerNum == 2)
			return Player2Score;

		return 0;
	}

}
