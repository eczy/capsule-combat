using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	public Text[] texts;
	public string[] scene_names;
	public float input_delay = 1f;

	bool get_input = true;
	int index_active = 0;

	Coroutine co;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		texts [index_active].color = Color.red;

		for (int i = 0; i < texts.Length; i++) {
			if (i == index_active)
				continue;

			texts [i].color = Color.white;
		}

		var inputDevice = InputManager.ActiveDevice;
		if (inputDevice == null)
		{
			Debug.LogWarning ("WARNING: MainMenuController could not find controller!");
		}

		if (inputDevice.Direction.Y < 0 && get_input) {
			index_active = (index_active + 1) % texts.Length;
			co = StartCoroutine (DelayInput ());
		} else if (inputDevice.Direction.Y > 0 && get_input) {
			index_active -= 1;
			if (index_active < 0)
				index_active = texts.Length - 1;
			co = StartCoroutine (DelayInput ());
		}

		if (inputDevice.Action1.WasPressed) {
			StopCoroutine (co);
			get_input = false;

			if (index_active == texts.Length - 1)
				Application.Quit();
			StartCoroutine (LoadAsyncScene (scene_names [index_active]));
		}
	}

	IEnumerator DelayInput(){
		get_input = false;
		yield return new WaitForSeconds (input_delay);
		get_input = true;
	}

	IEnumerator LoadAsyncScene(string scene_name){
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (scene_name);

		while (!asyncLoad.isDone)
			yield return null;
	}
}
