using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController_Net: NetworkBehaviour {

	public float BulletSpeed = 50.0f;
	public float AccelerationTime = 1.0f;
	public int Damage = 10;
	public int WallDamage = 1;
	public ParticleSystem blastParticles;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;
	private AudioSource audioPlayer;

	private bool blasting = false;
	private float timer = 0.0f;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		audioPlayer = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	[ServerCallback]
	void FixedUpdate () {
		if (timer <= 1.0f && !blasting) {
			rb.AddForce (transform.up * BulletSpeed * Time.deltaTime);
		} 

		timer += Time.deltaTime;

		if (blasting) {
			rb.velocity = Vector2.zero;
			rb.angularVelocity = 0.0f;
		}
	}

	[ServerCallback]
	void Update () {
		if (blasting) {
			if (blastParticles.isStopped) {
				NetworkServer.Destroy (gameObject);
			}
		}
	}

	[ClientRpc]
	void RpcExplode () {
		audioPlayer.Play ();

		if (spriteRenderer) {
			spriteRenderer.sprite = null;
			blasting = true;
			blastParticles.Play ();
		}
	}
		
	void OnCollisionEnter2D(Collision2D collision) {

		if (!isServer)
			return;

		RpcExplode ();

		if (boxCollider) {
			boxCollider.enabled = false;
		}
			

		if (collision.gameObject.CompareTag ("Destructible")) {
			collision.gameObject.SendMessage ("TakeDamage", WallDamage);
		}
		if (collision.gameObject.CompareTag ("Player")) {
			collision.gameObject.SendMessage ("TakeDamage", Damage);
		}

	}
}
