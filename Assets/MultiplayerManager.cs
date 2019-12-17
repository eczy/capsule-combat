using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour {

	public string main_menu_scene_name;
	public Camera camera;
	public float camera_rotate_speed = 1f;
	public float camera_follow_speed = 1f;
	public float camera_distance_factor = 1f;
	public float camera_distance_offset = 1f;
	public float camera_height_factor = 1f;
	public float camera_height_offset = 1f;
	public Transform P1;
	public Transform P2;
	public Transform midpoint;
	public GameObject game_over_stuff;
	public MPWinnerText winner_text;
	public Text ready_text;
	public Text fight_text;
	public float ready_duration = 1f;
	public float fight_duration = 1f;
	public Text paused_text;
	public MPWinTracker P1_win_tracker;
	public MPWinTracker P2_win_tracker;

	bool paused = false;
	bool counted_score = false;
	Vector3 P1_paused_velocity;
	Vector3 P2_paused_velocity;

	// Use this for initialization
	void Start () {
		paused_text.gameObject.SetActive (false);
		ready_text.gameObject.SetActive (false);
		fight_text.gameObject.SetActive (false);
		game_over_stuff.SetActive (false);
		midpoint.position = P1.position + (P2.transform.position - P1.transform.position) / 2;
		midpoint.LookAt (P2);
		camera.transform.position = midpoint.transform.position + midpoint.transform.right * camera_distance_offset + Vector3.up * camera_height_offset;
		camera.transform.LookAt (midpoint);
		StartCoroutine (StartRound ());
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

		if (inputDevice.MenuWasPressed) {
			if (paused)
				Unpause ();
			else
				Pause ();
			paused = !paused;
		}

		midpoint.position = P1.position + (P2.transform.position - P1.transform.position) / 2;
		midpoint.LookAt (P2);

		if (!P1.GetComponent<PlayerController>().isDead() && !P2.GetComponent<PlayerController>().isDead()) {
			P1.LookAt (P2);
			P2.LookAt (P1);
		} else if (!P1.gameObject.activeInHierarchy || !P2.gameObject.activeInHierarchy) {
			P1.GetComponent<PlayerController> ().enabled = false;
			P2.GetComponent<PlayerController> ().enabled = false;
			string winner = (P1.GetComponent<PlayerController> ().isDead ()) ? "P2" : "P1";
			game_over_stuff.SetActive (true);
			winner_text.ShowWinner (winner);

			if (P1_win_tracker && winner == "P1" && !counted_score)
				P1_win_tracker.Increase ();
			else if (P2_win_tracker && winner == "P2" && !counted_score)
				P2_win_tracker.Increase ();
			counted_score = true;

			if (inputDevice.Action1.WasPressed) {
				Restart ();
			}

			if (inputDevice.Action2.WasPressed) {
				StartCoroutine(LoadAsyncScene (main_menu_scene_name));
			}
		}
		float player_distance = Vector3.Distance (P1.position, P2.position);
		camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, Quaternion.LookRotation(midpoint.position - camera.transform.position), Time.deltaTime * camera_rotate_speed);
		camera.transform.position = Vector3.Lerp(camera.transform.position, midpoint.transform.position + midpoint.transform.right * camera_distance_offset * (1 + player_distance * camera_distance_factor) + Vector3.up * camera_height_offset * (1 + player_distance * camera_height_factor), Time.deltaTime * camera_follow_speed);
	}

	IEnumerator LoadAsyncScene(string scene_name){
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (scene_name);

		while (!asyncLoad.isDone)
			yield return null;
	}

	void Restart(){
		counted_score = false;
		game_over_stuff.SetActive (false);
		P1.GetComponent<PlayerController> ().Reset ();
		P2.GetComponent<PlayerController> ().Reset ();
		P1.gameObject.SetActive (true);
		P2.gameObject.SetActive (true);
		StartCoroutine (StartRound ());
	}

	public IEnumerator StartRound(){
		ready_text.gameObject.SetActive (true);
		P1.GetComponent<PlayerController> ().enabled = false;
		P2.GetComponent<PlayerController> ().enabled = false;
		yield return new WaitForSeconds (ready_duration);
		ready_text.gameObject.SetActive (false);
		fight_text.gameObject.SetActive (true);
		P1.GetComponent<PlayerController> ().enabled = true;
		P2.GetComponent<PlayerController> ().enabled = true;
		yield return new WaitForSeconds (fight_duration);
		fight_text.gameObject.SetActive (false);
	}

	void Pause(){
		Time.timeScale = 0;
		P1_paused_velocity = P1.GetComponent<Rigidbody> ().velocity;
		P2_paused_velocity = P2.GetComponent<Rigidbody> ().velocity;
		P1.GetComponent<PlayerController> ().enabled = false;
		P2.GetComponent<PlayerController> ().enabled = false;
		P1.GetComponent<Rigidbody> ().Sleep ();
		P2.GetComponent<Rigidbody> ().Sleep ();
		paused_text.gameObject.SetActive (true);
	}

	void Unpause(){
		Time.timeScale = 1;
		P1.GetComponent<Rigidbody> ().WakeUp ();
		P2.GetComponent<Rigidbody> ().WakeUp ();
		P1.GetComponent<Rigidbody> ().velocity = P1_paused_velocity;
		P2.GetComponent<Rigidbody> ().velocity = P2_paused_velocity;
		P1.GetComponent<PlayerController> ().enabled = true;
		P2.GetComponent<PlayerController> ().enabled = true;
		paused_text.gameObject.SetActive (false);
	}
}
