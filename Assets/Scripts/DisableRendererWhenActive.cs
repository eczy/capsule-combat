using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRendererWhenActive : MonoBehaviour {

	public Renderer rend;

	void OnDisable(){
		rend.enabled = true;
	}

	void OnEnable(){
		rend.enabled = false;
	}
}
