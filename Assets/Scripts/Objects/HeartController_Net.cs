using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeartController_Net : NetworkBehaviour {

	public int lifeGiving = 20;
	public float removingTime = 0.45f;

	private ParticleSystem particles;
	private AudioSource audioPlayer;

	private bool toRemove = false;
	private float removeTimer = 0.0f;

	// Use this for initialization
	void Awake () {
		particles = GetComponentInChildren<ParticleSystem> ();
		audioPlayer = GetComponent<AudioSource> ();
	}

	[ServerCallback]
	void FixedUpdate () {
		if (toRemove) {
			removeTimer += Time.deltaTime;

			if (removeTimer >= removingTime) {
				NetworkServer.Destroy (gameObject);
			}
		}
	}

	[ServerCallback]
	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.CompareTag ("Player")) {
			PlayerController_Net player = collider.gameObject.GetComponent<PlayerController_Net> ();
			player.Heal (lifeGiving);

			RpcBlowHeart ();

			toRemove = true;
		}
	}

	[ClientRpc]
	void RpcBlowHeart () {
		audioPlayer.Play (); 
		particles.Play ();
	}
}
