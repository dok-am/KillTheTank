using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerSelector : MonoBehaviour {

	public bool isSecondPlayer;
	
	public void SelectRed () {
		Text text = GetComponent <Text> ();
		text.color = new Color (1.0f, 0.5f, 0.5f);
	}

	public void DeselectWhite () {
		Text text = GetComponent <Text> ();
		text.color = new Color (1.0f, 1.0f, 1.0f);
	}
}
