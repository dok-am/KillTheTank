using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AtomicPickup_Net : NetworkBehaviour {

	public float removingTime = 0.45f;
	public GameObject atomicBullet;

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
				Destroy (gameObject);
			}
		}
	}

	[ServerCallback]
	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.CompareTag ("Player")) {
			PlayerController_Net player = collider.gameObject.GetComponent<PlayerController_Net> ();
			player.SetNextBullet (atomicBullet);

			RpcPlayClientThings ();

			toRemove = true;
		}
	}

	[ClientRpc]
	void RpcPlayClientThings () {
		audioPlayer.Play (); 
		particles.Play ();
	}
}
