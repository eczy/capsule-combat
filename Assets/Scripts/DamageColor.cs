using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColor : MonoBehaviour {

	Renderer rend;
	Health health;
	Color original;

	// Use this for initialization
	void Start () {
		health = GetComponent<Health> ();
		rend = GetComponent<Renderer> ();
		original = rend.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		rend.material.color = Color.Lerp (original, Color.red, 1 - (health.health / health.max_health));
	}
}
