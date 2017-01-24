using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

	public Transform target;

	public int maxHealth = 100;
	public int curHealth = 100;

	public float healthAnimationSpeed = 10.0f;

	public Sprite[] healthSprites;

	private SpriteRenderer spriteRender;
	private float visibleHealthSprite;

	// Use this for initialization
	void Awake ()
	{
		spriteRender = GetComponent<SpriteRenderer> ();
		visibleHealthSprite = healthSprites.Length-1;
	}

	void FixedUpdate() 
	{
		transform.position = new Vector3 (target.position.x, target.position.y + 0.8f, 0.0f);
	}

	// Update is called once per frame
	void Update ()
	{
		AdjustCurrentHealth (0);   
	}

	public void ResetHealth () {
		curHealth = maxHealth;
		Awake ();
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

		float currentSpriteNumber = healthSprites.Length * ((float)curHealth / (float)maxHealth) - 1;
		float lerpHealth = Mathf.Lerp (visibleHealthSprite, currentSpriteNumber, Time.deltaTime * healthAnimationSpeed);
		spriteRender.sprite = healthSprites [(int)lerpHealth];
		visibleHealthSprite = lerpHealth;
	//	healthBarLength = (Screen.width / 6) * (curHealth / (float)maxHealth);
	}
}
