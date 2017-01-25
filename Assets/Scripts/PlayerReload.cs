﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReload : MonoBehaviour {

	public Transform target;

	public float reloadSpeed = 1.0f;

	public Sprite[] reloadSprites;

	private SpriteRenderer spriteRender;
	private float timer = 0.0f;

	// Use this for initialization
	void Awake ()
	{
		spriteRender = GetComponent<SpriteRenderer> ();
	}

	void FixedUpdate() 
	{
		transform.position = new Vector3 (target.position.x, target.position.y + 0.92f, 0.0f);
	}

	// Update is called once per frame
	void Update ()
	{
		if (timer <= reloadSpeed) {
			float currentSpriteNumber = reloadSprites.Length * (timer / reloadSpeed) - 1;
			if (currentSpriteNumber < 0) {
				currentSpriteNumber = 0;
			}
			spriteRender.sprite = reloadSprites [(int)currentSpriteNumber];
		}

		timer += Time.deltaTime;
	}

	public void Reload () {
		timer = 0.0f;
	}

}