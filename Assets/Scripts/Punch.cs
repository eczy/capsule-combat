using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour {

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

	float trigger_input;
	float bumper_input;
	bool trigger_released = true;
	bool bumper_released = true;
	bool can_punch = true;

	// Use this for initialization
	void Start () {
		if (left_fist == null || right_fist == null)
			Debug.LogWarning ("WARNING: punching object has no fists!");
		if (target == null)
			Debug.LogWarning ("WARNING: punch object has no target!");
	}
	
	// Update is called once per frame
	void Update () {
		trigger_input = Input.GetAxis ("Triggers");
		bumper_input = Input.GetAxis ("Bumpers");

		if (trigger_input > 0 && can_punch && trigger_released) {
			trigger_released = false;
			StartCoroutine (left_fist.Jab (target, jab_speed, jab_recovery_speed, jab_damage));
			StartCoroutine (AttackDelay (jab_reload_time));
		} else if (trigger_input < 0 && can_punch && trigger_released) {
			trigger_released = false;
			StartCoroutine (right_fist.Jab (target, jab_speed, jab_recovery_speed, jab_damage));
			StartCoroutine (AttackDelay (jab_reload_time));
		} else if (trigger_input == 0)
			trigger_released = true;

		if (bumper_input < 0 && can_punch && bumper_released) {
			bumper_released = false;
			StartCoroutine (left_fist.Hook (target, hook_speed, hook_recovery_speed, hook_damage));
			StartCoroutine (AttackDelay (hook_reload_time));
		} else if (bumper_input > 0 && can_punch && bumper_released) {
			bumper_released = false;
			StartCoroutine (right_fist.Hook (target, hook_speed, hook_recovery_speed, hook_damage));
			StartCoroutine (AttackDelay (hook_reload_time));
		} else if (bumper_input == 0)
			bumper_released = true;
	}

	IEnumerator AttackDelay(float time){
		can_punch = false;
		yield return new WaitForSeconds (time);
		can_punch = true;
	}
}
