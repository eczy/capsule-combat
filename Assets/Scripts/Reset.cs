using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour {

	Vector3 original_pos;
	Quaternion original_rot;
	float original_health;

	Health health;
	// Use this for initialization
	void Start () {
		health = GetComponent<Health> ();
		if (health != null)
			original_health = health.health;
		original_pos = transform.position;
		original_rot = transform.rotation;
	}

	public void ResetObject(){
		if (health != null)
			health.health = original_health;
		transform.position = original_pos;
		transform.rotation = original_rot;
	}
}
