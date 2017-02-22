using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerReload_Net : NetworkBehaviour {

	public GameObject playerReloadPrefab;

	public float ReloadSpeed = 1.0f;

	public bool isReadyToShoot = false;

	private PlayerBarRenderer_Net reloadRenderer;

	private float timer = 0.0f;

	[SyncVar(hook = "OnReloadPercentChanged")]
	private float currentReload = 0.0f;

	// Use this for initialization
	void Start () {
		GameObject reload = Instantiate (playerReloadPrefab, 
			new Vector3 (transform.position.x, transform.position.y + 0.92f, 0.0f), 
			Quaternion.identity);

		reloadRenderer = reload.GetComponent<PlayerBarRenderer_Net> ();
		reloadRenderer.target = transform;
		//reloadRenderer.transform.SetParent (transform);
	}
	
	[ServerCallback]
	void FixedUpdate () {
		if (timer <= ReloadSpeed) {
			timer += Time.deltaTime;
			currentReload = timer / ReloadSpeed;
		}
	}

	void OnReloadPercentChanged (float percent) {
		currentReload = percent;
		isReadyToShoot = (currentReload >= 1);
		if (reloadRenderer)
			reloadRenderer.SetPercent (currentReload);
	}

	[Command]
	public void CmdReload () {
		timer = 0.0f;
		currentReload = 0.0f;
	}

	public void Hide () {
		reloadRenderer.Hide ();
	}

	public void Show () {
		reloadRenderer.Show ();
	}
}
