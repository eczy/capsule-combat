using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fist : MonoBehaviour {	
	public bool isRight = true;
	public AudioClip jab_sound;
	public AudioClip hook_sound;
	public AudioClip miss_sound;
	public float jab_volume = 1f;
	public float hook_volume = 1f;
	public float miss_volume = 1f;

	Collider coll;
	Rigidbody rb;
	Animator anim;
	Hurtbox hurt;

	bool hit = false;
	bool played_hit_sound = false;
	void Start () {
		coll = GetComponent<Collider> ();
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		hurt = GetComponent<Hurtbox> ();
		coll.enabled = false;
	}

	void OnCollisionEnter(Collision c){
		if (c.collider.GetComponent<Hitbox> () != null)
			coll.enabled = false;
			hit = true;
	}

	public IEnumerator Jab(Transform target, float speed, float recovery, float damage=1f){
		hurt.damage = damage;
		anim.enabled = false;
		coll.enabled = true;
		float t = 0;
		Vector3 start_pos = transform.localPosition;
		Vector3 target_pos = target.transform.localPosition;
		while (t < speed) {
			if (hit && jab_sound && !played_hit_sound) {
				AudioSource.PlayClipAtPoint (jab_sound, transform.position, jab_volume);
				played_hit_sound = true;
			}
			t += Time.deltaTime;
			float progress = t / speed;
			transform.localPosition = Vector3.Lerp (start_pos, target_pos, progress);
			yield return null;
		}
		if (hit && jab_sound && !played_hit_sound) {
			AudioSource.PlayClipAtPoint (jab_sound, transform.position, jab_volume);
			played_hit_sound = true;
		}
		else if (!hit && miss_sound)
			AudioSource.PlayClipAtPoint (miss_sound, transform.position, miss_volume);
		transform.localPosition = target_pos;
		coll.enabled = false;
		t = 0;
		while (t < recovery) {
			t += Time.deltaTime;
			float progress = t / recovery;
			transform.localPosition = Vector3.Lerp (target_pos, start_pos, progress);
			yield return null;
		}
		transform.localPosition = start_pos;
		hit = false;
		anim.enabled = true;
		played_hit_sound = false;
		yield return null;
	}

	public IEnumerator Hook(Transform target, float speed, float recovery, float damage=1f){
		hurt.damage = damage;
		anim.enabled = false;
		coll.enabled = true;
		float t = 0;
		Vector3 start_pos = transform.localPosition;
		Vector3 target_pos = target.transform.localPosition;

		// Used to determine direction of hook
		Vector3 adjustment = new Vector3 (0.01f, 0, 0);
		if (isRight)
			adjustment *= -1;

		Vector3 midpoint = start_pos + (target_pos - start_pos) * 0.5f + adjustment;
		Vector3 target_vec = target_pos - midpoint;
		Vector3 start_vec = start_pos - midpoint;
		while (t < speed) {
			if (hit && hook_sound && !played_hit_sound) {
				AudioSource.PlayClipAtPoint (hook_sound, transform.position, hook_volume);
				played_hit_sound = true;
			}
			t += Time.deltaTime;
			float progress = t / speed;
			transform.localPosition = midpoint + Vector3.Slerp (start_vec, target_vec, progress);
			yield return null;
		}
		if (hit && hook_sound && !played_hit_sound) {
			AudioSource.PlayClipAtPoint (hook_sound, transform.position, hook_volume);
			played_hit_sound = true;
		}
		else if (!hit && miss_sound)
			AudioSource.PlayClipAtPoint (miss_sound, transform.position, miss_volume);
		transform.localPosition = target_pos;
		coll.enabled = false;
		while (t < recovery) {
			t += Time.deltaTime;
			float progress = t / recovery;
			transform.localPosition = Vector3.Lerp (target_pos, start_pos, progress);
			yield return null;
		}
		transform.localPosition = start_pos;
		hit = false;
		anim.enabled = true;
		played_hit_sound = false;
		yield return null;
	}
}
