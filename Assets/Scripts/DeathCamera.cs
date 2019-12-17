using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamera : MonoBehaviour {

	ThirdPersonCamera tpc;
	public GameObject dead_player;

	// Use this for initialization
	void Start () {
		tpc = GetComponent<ThirdPersonCamera> ();
	}

	void Update(){
		tpc.enabled = false;
		if (dead_player != null)
			transform.LookAt (dead_player.transform);
	}

	void OnDisable(){
		tpc.enabled = true;
	}
}