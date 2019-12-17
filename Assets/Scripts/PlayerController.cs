using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerController : MonoBehaviour {

	public int playerNum = 1;
	public float movement_speed = 1f;
	public float rotation_speed = 1f;
	public float jumpforce = 1f;
	public float jump_movement_damper = 1f;
	public float air_drag = 1f;
	public float fallspeed = 1f;
	public Animator right_fist_anim, left_fist_anim;
	public Fist left_fist;
	public Fist right_fist;
	public float jab_speed = 1f;
	public float hook_speed = 1f;
	public float jab_recovery_speed = 1f;
	public float hook_recovery_speed = 1f;
	public float jab_reload_time = 1f;
	public float hook_reload_time = 1f;
	public float jab_damage = 1f;
	public float hook_damage = 1f;
	public Transform target = null;
	public float death_delay = 1f;
	public DeathCamera death_cam;
	public ParticleSystem death_explosion;
	public float sideways_move_raycast_dist = 0.6f;
	public float grounded_raycast_dist = 1f;
	public bool multiplayer_input = false;
	public bool is_MP_right = false;
	public Transform enemy_target;
	public AudioClip death_sound;
	public float death_sound_volume = 1f;

	Rigidbody rb;
	RigidbodyConstraints rb_original_constraints;
	float rb_original_adrag;
	Vector2 left_stick_input;
	Vector2 right_stick_input;
	Health health;
	bool canjump = true;
	bool grounded = false;
	bool can_punch = true;
	bool dead = false;

	Vector3 original_position;
	Quaternion original_rotation;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		if (left_fist == null || right_fist == null)
			Debug.LogWarning ("WARNING: punching object has no fists!");
		if (target == null)
			Debug.LogWarning ("WARNING: punch object has no target!");
		health = GetComponent<Health> ();
		if (death_cam != null)
			death_cam.enabled = false;

		original_position = transform.position;
		original_rotation = transform.rotation;
		rb_original_constraints = rb.constraints;
		rb_original_adrag = rb.angularDrag;
	}
	
	// Update is called once per frame
	void Update () {
		if (health.health == 0 && !dead)
			StartCoroutine (Die ());

		if (dead)
			return;

		var inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		if (inputDevice == null)
		{
			Debug.LogWarning ("WARNING: Player exists with no controller!");
			return;
		}

		if (Physics.Raycast (transform.position, Vector3.down, grounded_raycast_dist, 1 << 8)) {
			grounded = true;
			if (!canjump)
				canjump = true;
		} else {
			grounded = false;
		}

		if (enemy_target != null && enemy_target.gameObject.activeInHierarchy) {
			transform.LookAt (enemy_target);
		}

		if (inputDevice.LeftStickButton.IsPressed && health.health < health.max_health) {
			health.StartRegenerate ();
			left_stick_input = Vector2.zero;
			left_fist_anim.speed = 0;
			right_fist_anim.speed = 0;
			return;
		} else if (inputDevice.LeftStickButton.WasReleased) {
			health.StopRegenerate ();
		}

		left_stick_input = inputDevice.LeftStick;
		right_stick_input = inputDevice.RightStick;
			
		if (inputDevice.Action1.WasPressed && canjump && grounded) {
			StartCoroutine (Jump ());
		}

		if (grounded) {
			left_fist_anim.speed = Vector3.Magnitude (left_stick_input);
			right_fist_anim.speed = Vector3.Magnitude (left_stick_input);
		} else {
			left_fist_anim.speed = 0;
			right_fist_anim.speed = 0;
		}

		if (inputDevice.LeftBumper.WasPressed && can_punch) {
			StartCoroutine (left_fist.Jab (target, jab_speed, jab_recovery_speed, jab_damage));
			StartCoroutine (AttackDelay (jab_reload_time));
		} else if (inputDevice.RightBumper.WasPressed && can_punch) {
			StartCoroutine (right_fist.Jab (target, jab_speed, jab_recovery_speed, jab_damage));
			StartCoroutine (AttackDelay (jab_reload_time));
		}
			
		if (inputDevice.LeftTrigger.WasPressed && can_punch) {
			StartCoroutine (left_fist.Hook (target, hook_speed, hook_recovery_speed, hook_damage));
			StartCoroutine (AttackDelay (hook_reload_time));
		} else if (inputDevice.RightTrigger.WasPressed && can_punch) {
			StartCoroutine (right_fist.Hook (target, hook_speed, hook_recovery_speed, hook_damage));
			StartCoroutine (AttackDelay (hook_reload_time));
		}

		if (rb.velocity.y < 0.1 && !grounded)
			rb.velocity += Vector3.down * fallspeed;

	}

	void FixedUpdate() {
		if (dead)
			return;

		if (grounded == false) {
			left_stick_input *= jump_movement_damper;
			rb.AddForce (new Vector3(rb.velocity.x, 0, rb.velocity.z) * -air_drag);
		}
		if (!Physics.Raycast (transform.position + Vector3.up * 0.5f, (transform.forward * left_stick_input.y + transform.right * left_stick_input.x).normalized, sideways_move_raycast_dist) &&
			!Physics.Raycast (transform.position - Vector3.up * 0.5f, (transform.forward * left_stick_input.y + transform.right * left_stick_input.x).normalized, sideways_move_raycast_dist)) {
			if (multiplayer_input) {
				if (!is_MP_right) {
					rb.AddForce (Vector3.Scale((transform.forward * left_stick_input.x + transform.right * -left_stick_input.y), new Vector3(1, 0, 1)).normalized * movement_speed, ForceMode.Impulse);
				} else {
					rb.AddForce (Vector3.Scale((transform.forward * -left_stick_input.x + transform.right * left_stick_input.y), new Vector3(1, 0, 1)).normalized * movement_speed, ForceMode.Impulse);
				}
			} else {
				rb.AddForce ((transform.forward * left_stick_input.y + transform.right * left_stick_input.x).normalized * movement_speed, ForceMode.Impulse);
				rb.AddTorque ((transform.up * right_stick_input.x) * rotation_speed);
			}
		}
	}

	IEnumerator Jump(){
		Debug.Log ("Jumping");
		canjump = false;
		rb.AddForce (Vector3.up * jumpforce, ForceMode.Impulse);
		yield return null;
	}

	public bool isDead(){
		return dead;
	}

	public void Reset(){
		rb.constraints = rb_original_constraints;
		rb.angularDrag = rb_original_adrag;
		transform.position = original_position;
		transform.rotation = original_rotation;
		health.health = health.max_health;
		dead = false;
		can_punch = true;
		canjump = true;

		if(death_cam != null)
			death_cam.enabled = false;
	}

	IEnumerator AttackDelay(float time){
		can_punch = false;
		yield return new WaitForSeconds (time);
		can_punch = true;
	}

	IEnumerator Die(){
		dead = true;
		if (death_cam != null) {
			death_cam.enabled = true;
			death_cam.dead_player = this.gameObject;
		}
		
		left_fist.GetComponent<Collider> ().enabled = false;
		right_fist.GetComponent<Collider> ().enabled = false;
		left_fist_anim.speed = 0;
		right_fist_anim.speed = 0;
		rb.constraints = RigidbodyConstraints.None;
		rb.angularDrag = 0;

		yield return new WaitForSeconds (death_delay);

		if (death_explosion != null)
			Instantiate (death_explosion, transform.position, Quaternion.LookRotation (Vector3.up));

		if (death_sound)
			AudioSource.PlayClipAtPoint (death_sound, transform.position, death_sound_volume);

		gameObject.SetActive (false);
		yield return null;
	}
}
