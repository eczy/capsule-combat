using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour {

	public EnemyController controller = null;

	void Start(){
		if (controller == null)
			Debug.LogWarning ("WARNING: playertrigger does not belong to an enemy!");
	}

	void OnTriggerEnter(Collider coll){
		if (coll.GetComponent<PlayerController> () != null) {
			Debug.Log ("Enemy has spotted player");
			controller.PlayerSpotted (coll.transform);
		}
	}

	void OnTriggerExit(Collider coll){
		if (coll.GetComponent<PlayerController> () != null) {
			Debug.Log ("Enemy has lost player");
			controller.PlayerLost ();
		}
	}
}
