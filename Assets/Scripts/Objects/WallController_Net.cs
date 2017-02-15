using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WallController_Net : NetworkBehaviour {

	public Sprite[] WallStates;

	[SyncVar]
	private int currentWallState = 0;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	public void TakeDamage (int damage) {
		currentWallState += damage;
		if (currentWallState >= WallStates.Length) {
			BlowTheWall ();
		}
	}

	void Update ()
	{
		spriteRenderer.sprite = WallStates [currentWallState];
	}

	void BlowTheWall () {
		//Destroy (gameObject);
		NetworkServer.Destroy (gameObject);
	}
}
