using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeOnDamage : MonoBehaviour {
	public CameraManager manager;

	void OnCollisionEnter(Collision coll){
		if (coll.collider.GetComponent<Hurtbox> () != null) {
			foreach (GameObject g in manager.cameras) {
				CameraShake shake = g.GetComponent<CameraShake> ();
				if (shake != null)
					StartCoroutine (shake.Shake ());
			}
		}
	}
}
