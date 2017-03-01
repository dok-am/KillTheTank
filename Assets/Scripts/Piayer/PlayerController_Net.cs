using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController_Net : NetworkBehaviour {

	public bool SecondPlayer = false;
	public bool isDead = false;

	public float Velocity = 7.0f;
	public float MaximalSpeed = 40.0f;
	public float RotationSpeed = 2.0f;
	public float FrictionScale = 10.0f;


	public float ShootingKick = 70.0f;

	public GameObject Bullet;
	public ParticleSystem trackParticles;
	public ParticleSystem burnParticles;
	public ParticleSystem explodeParticles;

	public RuntimeAnimatorController SecondPlayerAnimation;

	private Rigidbody2D rb;


	private PlayerHealth_Net playerHealth;
	private PlayerReload_Net playerReload;

	private PlayerSoundManager soundManager;
	private Animator animator;

	private GameObject nextBullet;

	private string vAxisName = "Vertical";
	private string hAxisName = "Horizontal";
	private string attackAxisName = "Attack";

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		soundManager = GetComponentInChildren<PlayerSoundManager> ();
		nextBullet = Bullet;

		playerHealth = GetComponent<PlayerHealth_Net> ();
		playerReload = GetComponent<PlayerReload_Net> ();

		if (FindObjectsOfType<PlayerController_Net> ().Length > 1) {
			SecondPlayer = true;
			animator.runtimeAnimatorController = SecondPlayerAnimation;
		} 
	}
		
	void Start () {
		if (SecondPlayer) {
			playerHealth.SetDuplicator (GameObject.Find("Health2Dupl").GetComponent<SpriteDuplicator> ());
			playerReload.SetDuplicator (GameObject.Find("Reload2Dupl").GetComponent<SpriteDuplicator> ());
		} else {
			playerHealth.SetDuplicator (GameObject.Find("Health1Dupl").GetComponent<SpriteDuplicator> ());
			playerReload.SetDuplicator (GameObject.Find("Reload1Dupl").GetComponent<SpriteDuplicator> ());
		}

		if (isLocalPlayer) {
			SelectUIForPlayer (SecondPlayer);
		}
	}

	void SelectUIForPlayer(bool secondPlayer) {
		LocalPlayerSelector[] selectors = FindObjectsOfType <LocalPlayerSelector> ();
		foreach (LocalPlayerSelector selector in selectors) {
			if (selector.isSecondPlayer == secondPlayer) {
				selector.SelectRed ();
			}
		}
	}

	void FixedUpdate() {

		CheckAlive ();

		GameManager_Net manager = FindObjectOfType<GameManager_Net> ();

		if (manager)
			if (manager.isPaused)
				return;

		if (!isLocalPlayer)
			return;

		float vertical = Input.GetAxisRaw (vAxisName);
		float horizontal = Input.GetAxisRaw (hAxisName);
		float shoot = Input.GetAxisRaw (attackAxisName);

		Vector2 localUp = new Vector2 (rb.transform.up.x, rb.transform.up.y);

		if (vertical != 0) {
			Vector2 engineForce = Vector2.Lerp (Vector2.zero, localUp * vertical * MaximalSpeed, Velocity * Time.deltaTime);
			rb.AddForce (engineForce);
			soundManager.PlayRide ();
			animator.SetBool ("isMoving", true);
		} else {
			animator.SetBool ("isMoving", false);
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

		if (shoot != 0 && playerReload.isReadyToShoot) {
			Shoot (localUp);
		}

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

	[Command]
	void CmdSpawnBullet (Vector2 localUp) {
		GameObject bullet = Instantiate (nextBullet, transform.position + new Vector3(localUp.x, localUp.y, 0.0f) * 0.5f, transform.rotation);
		NetworkServer.Spawn (bullet);
		nextBullet = Bullet;
	}

	void Shoot (Vector2 localUp) {
		CmdSpawnBullet (localUp);
		rb.AddForce (-localUp * ShootingKick);
		playerReload.isReadyToShoot = false;
		playerReload.CmdReload ();
		soundManager.PlayShoot ();
	}

	/*
	 * 
	 * Public functions
	 * 
	 */

	[Server]
	public void TakeDamage (int damage) {
		playerHealth.AdjustCurrentHealth (-damage);
		soundManager.PlayHurt ();
		if (playerHealth.curHealth <= 0 && !FindObjectOfType<GameManager_Net> ().isPaused) {
			FindObjectOfType<GameManager_Net>().FinishGame (SecondPlayer);
		}
	}

	[ClientRpc]
	void RpcCheckPlayerAlive () {
		CheckAlive ();
	}

	[Client]
	void CheckAlive () {
		if (playerHealth.curHealth <= 25 && !burnParticles.isPlaying) {
			burnParticles.Play ();
		}

		if (playerHealth.curHealth <= 0 && !isDead) {
			//GameOver
			animator.SetBool("isMoving", false);
			animator.SetBool("isKilled", true);
			explodeParticles.Play ();
			soundManager.PlayDeath ();
			isDead = true;
		} 
	}

	[Server]
	public void Heal (int health) {
		playerHealth.AdjustCurrentHealth (health);
	}

	[ClientRpc]
	public void RpcResetPosition () {
		transform.localPosition = (SecondPlayer) ?  new Vector3(7.0f, 14.0f) : new Vector3(7.0f, 0.0f);
		transform.localRotation = new Quaternion (0.0f, 0.0f, (SecondPlayer) ? 180.0f : 0.0f, 0.0f);
		animator.SetBool ("isKilled", false);
		playerHealth.Show ();
		playerReload.Show ();
		if (burnParticles.isPlaying)
			burnParticles.Stop ();
		isDead = false;
	}

	[Server]
	public void SetNextBullet (GameObject bullet) {
		nextBullet = bullet;
	}

	/*
	 * 
	 * Collider delegates
	 *
	 */
	[ClientCallback]
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Cover") && !isLocalPlayer) {
			playerHealth.Hide ();
			playerReload.Hide ();
		}
	}

	[ClientCallback]
	void OnTriggerStay2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Cover") && !isLocalPlayer) {
			playerHealth.Hide ();
			playerReload.Hide ();
		}
	}

	[ClientCallback]
	void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Cover") && !isLocalPlayer) {
			playerHealth.Show ();
			playerReload.Show ();
		}
	}
}
