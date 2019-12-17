using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public GameObject[] cameras;
	public float switch_delay = 0.5f;
	public bool can_switch = true;
	public int active_index = 0;

	// Use this for initialization
	void Start () {
		foreach (GameObject g in cameras) {
			if (g == null)
				Debug.LogWarning ("WARNING: null camera assigned to player");
			g.SetActive (false);
		}
		cameras [active_index].SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("DPadV") < 0 && can_switch) {
			StartCoroutine (Switch ());
		}
	}

	IEnumerator Switch(){
		Debug.Log ("Switching cameras.");
		can_switch = false;
		cameras [active_index].SetActive (false);
		active_index = (active_index + 1) % cameras.Length;
		cameras [active_index].SetActive (true);
		yield return new WaitForSeconds(switch_delay);
		can_switch = true;
		yield return null;
	}
}
