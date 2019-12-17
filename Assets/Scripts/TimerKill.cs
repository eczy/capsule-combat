using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerKill : MonoBehaviour {

	public float time = 1f;
	IEnumerator Start () {
		yield return new WaitForSeconds (time);
		Destroy (gameObject);
	}
}
