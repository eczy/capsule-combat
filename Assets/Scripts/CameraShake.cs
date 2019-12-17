using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	public float shake_duration = 1f;
	public float shake_strength = 1f;

	bool can_shake = true;

	public IEnumerator Shake(){
		if (can_shake == false)
			yield break;
		can_shake = false;
		Vector3 original_pos = transform.localPosition;
		float t = 0;
		while (t < shake_duration) {
			t += Time.deltaTime;
			transform.localPosition = original_pos + Random.onUnitSphere * shake_strength;
			yield return null;
		}
		transform.localPosition = original_pos;
		can_shake = true;
	}
}
