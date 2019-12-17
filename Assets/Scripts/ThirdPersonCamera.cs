using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

	public Transform player = null;
	public float rotate_speed = 1f;
	public float follow_speed = 1f;

	Vector3 original_pos;
	Vector3 player_rot;

	// Use this for initialization
	void Start () {
		if (player == null)
			Debug.LogWarning ("WARNING: No player attached to this camera!");
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Lerp (transform.rotation, player.rotation, Time.deltaTime * rotate_speed);
		transform.position = Vector3.Lerp (transform.position, player.position, Time.deltaTime * follow_speed);
	}
}
