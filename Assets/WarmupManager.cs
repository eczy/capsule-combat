using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WarmupManager : MonoBehaviour {

	public string main_menu_scene_name;
	public float tutorial_message_delay = 1f;
	public Text tutorial_text;
	public GameObject tutorial_panel;
	public PlayerController player;
	public Text paused_text;
	public bool handle_pause = true;

	bool paused = false;
	Vector3 pause_velocity;

	// Use this for initialization
	IEnumerator Start () {
		paused_text.gameObject.SetActive (false);
		if (tutorial_text == null)
			yield break;
		
		var inputDevice = InputManager.ActiveDevice;
		if (inputDevice == null) {
			Debug.LogWarning ("WARNING: MainMenuController could not find controller!");
		}

		// Run tutorial
		float original_jumpforce = player.jumpforce;
		player.jumpforce = 0f;
		tutorial_text.text = "Welcome to the tutorial! (Press A to continue.)";

		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.Action1.WasPressed)
			yield return null;

		player.jumpforce = original_jumpforce;
		tutorial_text.text = "Press A to jump.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.Action1.WasPressed)
			yield return null;
		player.jumpforce = 0f;
		
		tutorial_text.text = "Press B to switch targets.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.Action2.WasPressed)
			yield return null;
	
		tutorial_text.text = "Press the left bumper to throw a left jab.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.LeftBumper.WasPressed)
			yield return null;

		tutorial_text.text = "Press right bumper to throw a right jab.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.RightBumper.WasPressed)
			yield return null;

		tutorial_text.text = "Press left trigger to throw a left hook.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.LeftTrigger.WasPressed)
			yield return null;

		tutorial_text.text = "Press right trigger to throw a right hook.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.RightTrigger.WasPressed)
			yield return null;

		tutorial_text.text = "Note that hooks do more damage but take much longer to recover,";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.Action1.WasPressed)
			yield return null;

		tutorial_text.text = "and if you are too close to your target, your hook will miss!";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.Action1.WasPressed)
			yield return null;

		tutorial_text.text = "Press Start to pause and unpause.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.MenuWasPressed || paused)
			yield return null;

		player.GetComponent<Health> ().Damage (3);
		tutorial_text.text = "You have been hurt! Press Left Stick to regenerate health.";
		while (!inputDevice.LeftStickButton.WasPressed)
		yield return null;

		tutorial_text.text = "Restore yourself to full health.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (player.GetComponent<Health>().health != player.GetComponent<Health>().max_health)
			yield return null;

		tutorial_text.text = "Note that regeneration does not work when you are being attacked!";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.Action1.WasPressed)
			yield return null;

		tutorial_text.text = "You have completed the tutorial! Press DPad Down at any time to return to the main menu.";
		yield return new WaitForSeconds (tutorial_message_delay);
		while (!inputDevice.Action1.WasPressed)
			yield return null;
		player.jumpforce = original_jumpforce;

		tutorial_text.gameObject.SetActive (false);
		tutorial_panel.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		var inputDevice = InputManager.ActiveDevice;
		if (inputDevice == null)
		{
			Debug.LogWarning ("WARNING: MainMenuController could not find controller!");
		}
		if (inputDevice.DPadDown.WasPressed) {
			StartCoroutine (LoadAsyncScene (main_menu_scene_name));
		}

		if (inputDevice.MenuWasPressed && handle_pause) {
			if (paused)
				Unpause ();
			else
				Pause ();
			paused = !paused;
		}
	}

	IEnumerator LoadAsyncScene(string scene_name){
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (scene_name);

		while (!asyncLoad.isDone)
			yield return null;
	}

	void Pause(){
		Time.timeScale = 0;
		pause_velocity = player.GetComponent<Rigidbody> ().velocity;
		player.GetComponent<Rigidbody> ().Sleep ();
		player.GetComponent<PlayerController> ().enabled = false;
		paused_text.gameObject.SetActive (true);
	}

	void Unpause(){
		Time.timeScale = 1;
		player.GetComponent<Rigidbody> ().WakeUp();
		player.GetComponent<Rigidbody> ().velocity = pause_velocity;
		player.GetComponent<PlayerController> ().enabled = true;
		paused_text.gameObject.SetActive (false);
	}
}
