using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBarRenderer_Net : HideableSprite {

	public Transform target;

	public Sprite[] reloadSprites;
	public float yOffset = 0.92f;
	public SpriteDuplicator duplicator;

	// Use this for initialization
	protected override void Awake ()
	{
		base.Awake ();
		spriteRender = GetComponent<SpriteRenderer> ();
	}

	void FixedUpdate() 
	{
		if (target) {
			transform.position = new Vector3 (target.position.x, target.position.y + yOffset, 0.0f);
		} else {
			Destroy (gameObject);
		}
	}
		
	protected override void Update ()
	{
		base.Update ();
	}

	public void SetPercent(float percent) {
		float curSprite = reloadSprites.Length * percent - 1;
		if (curSprite < 0) {
			curSprite = 0;
		} else if (curSprite >= reloadSprites.Length) {
			curSprite = reloadSprites.Length - 1;
		}

		spriteRender.sprite = reloadSprites [(int)curSprite];

		if (duplicator)
			duplicator.SetSprite (reloadSprites [(int)curSprite]);
	}
		
		
}
