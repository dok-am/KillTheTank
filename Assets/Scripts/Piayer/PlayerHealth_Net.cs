using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth_Net : NetworkBehaviour {

	public int maxHealth = 100;

	[SyncVar(hook = "OnHealthChanged")]
	public int curHealth = 100;

	public GameObject PlayerHealthPrefab;

	public float healthAnimationSpeed = 10.0f;

	private PlayerBarRenderer_Net healthRenderer;

	private float currentPercent = 1.0f;

	void Start ()
	{
		GameObject health = Instantiate (PlayerHealthPrefab, 
			new Vector3 (transform.position.x, transform.position.y + 0.8f, 0.0f), 
			Quaternion.identity);

		healthRenderer = health.GetComponent<PlayerBarRenderer_Net> ();
		healthRenderer.yOffset = 0.8f;
		healthRenderer.target = transform;
	}

	[ClientCallback]
	void Update ()
	{  
		float newPercent = ((float)curHealth / (float)maxHealth);
		float lerpHealth = Mathf.Lerp (currentPercent, newPercent, Time.deltaTime * healthAnimationSpeed);
		healthRenderer.SetPercent (lerpHealth);
		currentPercent = lerpHealth;
	}

	[ClientRpc]
	public void RpcResetHealth () {
		curHealth = maxHealth;
		currentPercent = 1.0f;
		AdjustCurrentHealth (0);
	}

	public void AdjustCurrentHealth (int adj)
	{
		curHealth += adj;

		if (curHealth < 0)
			curHealth = 0;

		if (curHealth > maxHealth)
			curHealth = maxHealth;

		if (maxHealth < 1)
			maxHealth = 1;

	}

	void OnHealthChanged(int NewHealth) {
		curHealth = NewHealth;
		Debug.Log ("Health updated on client: " + curHealth);
	}

	public void Hide () {
		healthRenderer.Hide ();
	}

	public void Show () {
		healthRenderer.Show ();
	}
}
