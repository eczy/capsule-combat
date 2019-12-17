using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public Fist left_fist, right_fist;
	public float movement_speed = 1f;
	public float attack_distance = 1f;
	public float reload_time = 1f;
	public float death_delay = 1f;
	public float jab_speed = 1f;
	public float hook_speed = 1f;
	public float jab_recovery_speed = 1f;
	public float hook_recovery_speed = 1f;
	public float jab_reload_time = 1f;
	public float hook_reload_time = 1f;
	public float jab_damage = 1f;
	public float hook_damage = 1f;
	public float min_attack_delay = 1f;
	public float max_attack_delay = 1f;
	public float fallspeed = 1f;
	public Transform attack_target;
	public ParticleSystem death_explosion;
	public AudioClip death_sound;
	public float death_sound_volume = 1f;

	Transform target = null;
	Rigidbody rb;
	Health health;
	bool inAttackRange = false;
	bool canAttack = true;
	bool dead = false;
	Coroutine attack_co = null;
	Coroutine fist_co = null;
	Vector3 original_position;
	Quaternion original_rotation;
	RigidbodyConstraints rb_original_constraints;

	public bool isDead(){
		return dead;
	}
	// Use this for initialization
	void Start () {
		left_fist.GetComponent<Animator> ().speed = 0;
		right_fist.GetComponent<Animator> ().speed = 0;
		left_fist.GetComponent<Collider> ().enabled = false;
		right_fist.GetComponent<Collider> ().enabled = false;
		rb = GetComponent<Rigidbody> ();
		health = GetComponent<Health> ();
		original_position = transform.position;
		original_rotation = transform.rotation;
		rb_original_constraints = rb.constraints;
	}
	
	// Update is called once per frame
	void Update () {
		if (dead)
			return;
		if (target != null && target.GetComponent<Health> () != null && target.GetComponent<Health> ().health == 0)
			PlayerLost ();
		
		if (health.health == 0 && !dead) {
			StartCoroutine (Die ());
		}
		if (inAttackRange && canAttack && target != null) {
			attack_co = StartCoroutine (Attack ());
		}
		if (target != null) {
			if (Vector3.Magnitude (target.position - transform.position) < attack_distance) {
				left_fist.GetComponent<Animator> ().speed = 0;
				right_fist.GetComponent<Animator> ().speed = 0;
				inAttackRange = true;
				transform.position = Vector3.Lerp (transform.position, target.position + (transform.position - target.position).normalized * attack_distance, Time.deltaTime);
			} else {
				left_fist.GetComponent<Animator> ().speed = 1;
				right_fist.GetComponent<Animator> ().speed = 1;
				inAttackRange = false;
				rb.velocity += Vector3.Scale(transform.forward * movement_speed, new Vector3(1, 0, 1));
			}
			transform.LookAt (target.position);
		}
		if (rb.velocity.y < -0.1)
			rb.velocity += Vector3.down * fallspeed;
	}

	public void PlayerSpotted(Transform player){
		target = player;
		left_fist.GetComponent<Animator> ().speed = 1;
		right_fist.GetComponent<Animator> ().speed = 1;
	}

	public void PlayerLost(){
		left_fist.GetComponent<Animator> ().speed = 0;
		right_fist.GetComponent<Animator> ().speed = 0;
		target = null;
	}

	public void Reset(){
		rb.constraints = rb_original_constraints;
		transform.position = original_position;
		transform.rotation = original_rotation;
		health.health = health.max_health;
		health.invincible = false;
		dead = false;
	}

	IEnumerator Attack(){
		canAttack = false;
		yield return new WaitForSeconds (Random.Range (min_attack_delay, max_attack_delay));
		int r = Random.Range (0, 4);
		if (r == 0) {
			fist_co = StartCoroutine (left_fist.Jab (attack_target, jab_speed, jab_recovery_speed, jab_damage));
		} else if (r == 1) {
			fist_co = StartCoroutine (right_fist.Jab (attack_target, jab_speed, jab_recovery_speed, jab_damage));
		} else if (r == 2) {
			fist_co = StartCoroutine (left_fist.Hook (attack_target, hook_speed, hook_recovery_speed, hook_damage));
		} else if (r == 3) {
			fist_co = StartCoroutine (right_fist.Hook (attack_target, hook_speed, hook_recovery_speed, hook_damage));
		}
		yield return new WaitForSeconds (reload_time);
		canAttack = true;
		yield return null;
	}
		
	IEnumerator Die(){
		dead = true;
		if (attack_co != null)
			StopCoroutine (attack_co);
		PlayerLost ();
		left_fist.GetComponent<Collider> ().enabled = false;
		right_fist.GetComponent<Collider> ().enabled = false;
		rb.constraints = RigidbodyConstraints.None;

		yield return new WaitForSeconds (death_delay);

		if (death_explosion != null)
			Instantiate (death_explosion, transform.position, Quaternion.LookRotation (Vector3.up));

		if (death_sound)
			AudioSource.PlayClipAtPoint (death_sound, transform.position, death_sound_volume);

		gameObject.SetActive (false);
		yield return null;
	}
}
