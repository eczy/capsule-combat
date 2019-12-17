using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEnemy : MonoBehaviour {

	public Fist left_fist, right_fist;
	public float reload_time = 1f;
	public float jab_speed = 1f;
	public float hook_speed = 1f;
	public float jab_recovery_speed = 1f;
	public float hook_recovery_speed = 1f;
	public float jab_reload_time = 1f;
	public float hook_reload_time = 1f;
	public float min_attack_delay = 1f;
	public float max_attack_delay = 1f;
	public Transform target;

	bool canAttack = true;

	// Use this for initialization
	void Start () {
		left_fist.GetComponent<Animator> ().speed = 0;
		right_fist.GetComponent<Animator> ().speed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (canAttack)
			StartCoroutine (Attack ());
	}

	IEnumerator Attack(){
		canAttack = false;
		yield return new WaitForSeconds (Random.Range (min_attack_delay, max_attack_delay));
		int r = Random.Range (0, 4);
		if (r == 0) {
			StartCoroutine (left_fist.Jab (target, jab_speed, jab_recovery_speed, 0));
		} else if (r == 1) {
			StartCoroutine (right_fist.Jab (target, jab_speed, jab_recovery_speed, 0));
		} else if (r == 2) {
			StartCoroutine (left_fist.Hook (target, hook_speed, hook_recovery_speed, 0));
		} else if (r == 3) {
			StartCoroutine (right_fist.Hook (target, hook_speed, hook_recovery_speed, 0));
		}
		yield return new WaitForSeconds (reload_time);
		canAttack = true;
		yield return null;
	}
}
