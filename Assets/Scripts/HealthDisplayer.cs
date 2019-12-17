using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayer : MonoBehaviour {

	public Health health;

	Text text;
	string original_text;

	void Start(){
		text = GetComponent<Text> ();
		original_text = text.text;
		text.text = original_text + health.health + "/" + health.max_health;
	}
	
	// Update is called once per frame
	void Update () {
		text.text = original_text + health.health + "/" + health.max_health;
	}
}
