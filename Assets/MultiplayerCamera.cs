using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MultiplayerCamera : MonoBehaviour {
	public float camera_rotate_speed = 1f;
	public float camera_follow_speed = 1f;
	public float camera_distance_factor = 1f;
	public float camera_distance_offset = 1f;
	public float camera_height_factor = 1f;
	public float camera_height_offset = 1f;
	public Transform P1;
	public Transform P2;
	public Transform midpoint;

	// Use this for initialization
	void Start () {
		midpoint.position = P1.position + (P2.transform.position - P1.transform.position) / 2;
		midpoint.LookAt (P2);
		transform.position = midpoint.transform.position + midpoint.transform.right * camera_distance_offset + Vector3.up * camera_height_offset;
		transform.LookAt (midpoint);
	}

	// Update is called once per frame
	void Update () {
		var inputDevice = InputManager.ActiveDevice;
		if (inputDevice == null)
		{
			Debug.LogWarning ("WARNING: MainMenuController could not find controller!");
		}

		midpoint.position = P1.position + (P2.transform.position - P1.transform.position) / 2;
		midpoint.LookAt (P2);

		float player_distance = Vector3.Distance (P1.position, P2.position);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(midpoint.position - transform.position), Time.deltaTime * camera_rotate_speed);
		transform.position = Vector3.Lerp(transform.position, midpoint.transform.position + midpoint.transform.right * camera_distance_offset * (1 + player_distance * camera_distance_factor) + Vector3.up * camera_height_offset * (1 + player_distance * camera_height_factor), Time.deltaTime * camera_follow_speed);
	}
}
