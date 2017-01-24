﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public bool SecondPlayer = false;

	public GameObject playerHealthObject;
	public GameObject playerReloadObject;

	public float Velocity = 7.0f;
	public float MaximalSpeed = 40.0f;
	public float RotationSpeed = 2.0f;
	public float FrictionScale = 10.0f;

	public float ReloadSpeed = 1.0f;
	public float ShootingKick = 70.0f;

	public GameObject Bullet;
	public ParticleSystem trackParticles;
	public ParticleSystem burnParticles;
	public ParticleSystem explodeParticles;

	public Sprite normalTankSprite;
	public Sprite killedTankSprite;

	public GameManager gameManager;

	private Rigidbody2D rb;
	private float timer = 0;
	private PlayerHealth playerHealth;
	private PlayerReload playerReload;
	private SpriteRenderer spriteRender;

	private string vAxisName = "Vertical";
	private string hAxisName = "Horizontal";
	private string attackAxisName = "Attack";

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		playerHealth = playerHealthObject.GetComponent<PlayerHealth> ();
		playerReload = playerReloadObject.GetComponent<PlayerReload> ();
		playerReload.reloadSpeed = ReloadSpeed;
		spriteRender = GetComponent<SpriteRenderer> ();

		if (SecondPlayer) {
			vAxisName += "Second";
			hAxisName += "Second";
			attackAxisName += "Second";
		}
	}

	void FixedUpdate() {

		if (GameManager.isGamePaused())
			return;

		float vertical = Input.GetAxisRaw (vAxisName);
		float horizontal = Input.GetAxisRaw (hAxisName);
		float shoot = Input.GetAxisRaw (attackAxisName);

		Vector2 localUp = new Vector2 (transform.up.x, transform.up.y);

		if (vertical != 0) {
			Vector2 engineForce = Vector2.Lerp (Vector2.zero, localUp * vertical * MaximalSpeed, Velocity * Time.deltaTime);
			rb.AddForce (engineForce);
		}

		SetupParticles (vertical);

		Vector2 velocityVector = rb.velocity.normalized;
		float dot = Vector2.Dot (velocityVector, localUp);

		if (horizontal != 0) {
			if (dot < 0 && rb.velocity.magnitude > 0 && vertical < 0) {
				rb.MoveRotation (rb.rotation + RotationSpeed * horizontal);
			} else {
				rb.MoveRotation (rb.rotation - RotationSpeed * horizontal);
			}
		}



		if ((dot != 1) && (dot != -1) && (velocityVector.magnitude != 0)) {
			rb.velocity = Vector2.Lerp (rb.velocity, localUp * dot * rb.velocity.magnitude, FrictionScale * Time.deltaTime);
		}

		if (shoot != 0 && timer >= ReloadSpeed) {
			Instantiate (Bullet, transform.position + new Vector3(localUp.x, localUp.y, 0.0f) * 0.5f, transform.rotation);
			timer = 0.0f;
			rb.AddForce (-localUp * ShootingKick);
			playerReload.Reload ();
		}

		timer += Time.deltaTime;

	}

	void SetupParticles (float vertical) {
		if (vertical == 1) {
			if (!trackParticles.isPlaying) {
				trackParticles.Play ();
			}
		} else {
			trackParticles.Stop ();
		}
	}

	public void TakeDamage (int damage) {
		playerHealth.AdjustCurrentHealth (-damage);
		if (playerHealth.curHealth <= 20 && !burnParticles.isPlaying) {
			burnParticles.Play ();
		}

		if (playerHealth.curHealth == 0 && !GameManager.isGamePaused()) {
			//GameOver
			spriteRender.sprite = killedTankSprite;
			explodeParticles.Play ();
			gameManager.FinishGame (SecondPlayer);
		}
	}

	public void ResetPosition () {
		transform.localPosition = Vector3.zero;
		transform.localRotation = new Quaternion (0.0f, 0.0f, (SecondPlayer) ? 180.0f : 0.0f, 0.0f);
		spriteRender.sprite = normalTankSprite;
		if (burnParticles.isPlaying)
			burnParticles.Stop ();
	}
		
}
