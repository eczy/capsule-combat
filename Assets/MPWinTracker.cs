using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPWinTracker : MonoBehaviour {
	int wins = 0;
	string original_text;
	Text text;

	void Start(){
		text = GetComponent<Text> ();
		if (text == null){
			Destroy (this);
		}
		original_text = text.text;
	}

	public void Increase(){
		++wins;
	}

	void Update(){
		text.text = original_text + wins.ToString ();
	}
}
