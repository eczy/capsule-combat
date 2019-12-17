using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

	public float knockback_force = 1f;
	public float invincibility_time = 1f;

	Rigidbody rb;
	Health health;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		health = GetComponent<Health> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision coll){
		if (coll.collider.GetComponent<Hurtbox> () != null) {
			Vector3 collision_direction = coll.contacts [0].normal;
			rb.AddForce (collision_direction.normalized * knockback_force, ForceMode.Impulse);

			if (health != null) {
				float freq = (coll.collider.GetComponent<Hurtbox> ().damage <= 1) ? health.flash_frequency : health.flash_frequency * 2;
				health.Damage (coll.collider.GetComponent<Hurtbox> ().damage, freq);
				StartCoroutine (health.Invincibility (invincibility_time));
			}
		}
	}
}