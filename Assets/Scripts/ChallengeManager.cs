using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChallengeManager : MonoBehaviour {

	public PlayerController player;
	public EnemySpawner enemy_spawn;
	public GameObject gameover_ui;
	public string main_menu_scene_name;
	public Text paused_text;
	public Text round_text;

	bool paused = false;
	Vector3[] pause_velocities;
	int round_number = 0;
	string original_round_text;
	// Use this for initialization
	void Start () {
		if (player == null)
			Debug.LogWarning ("WARNING: no player!!!");
		if (enemy_spawn == null)
			Debug.LogWarning ("WARNING: no enemy spawn!!!");
		if (gameover_ui != null)
			gameover_ui.SetActive (false);
		enemy_spawn.num_enemies = 0;
		paused_text.gameObject.SetActive (false);
		original_round_text = round_text.text;
		StartRound ();
	}
	
	// Update is called once per frame
	void Update () {
		var inputDevice = InputManager.ActiveDevice;
		if (inputDevice == null)
		{
			Debug.LogWarning ("WARNING: ChallengeManager could not find controller!");
		}

		if (player.isDead () && !player.isActiveAndEnabled) {
			if (gameover_ui != null)
				gameover_ui.SetActive (true);
			if (inputDevice.Action1.WasPressed) {
				StartRound (reset:true);
			}
			if (inputDevice.Action2.WasPressed) {
				StartCoroutine(LoadAsyncScene (main_menu_scene_name));
			}
		}
			

		if (inputDevice.MenuWasPressed) {
			if (paused)
				Unpause ();
			else
				Pause ();
			paused = !paused;
		}

		if (enemy_spawn.AliveEnemies () == 0) {
			StartRound ();
		}

		if (inputDevice.DPadDown.WasPressed)
			StartCoroutine(LoadAsyncScene (main_menu_scene_name));
	}

	void StartRound(bool reset=false){
		gameover_ui.SetActive (false);
		player.Reset ();
		if (reset)
			enemy_spawn.ResetEnemies ();
		else {
			enemy_spawn.num_enemies++;
			enemy_spawn.Spawn ();
			++round_number;
		}
		if (round_text)
			round_text.text = original_round_text + round_number.ToString ();
		player.gameObject.SetActive (true);
	}

	IEnumerator LoadAsyncScene(string scene_name){
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (scene_name);

		while (!asyncLoad.isDone)
			yield return null;
	}

	void Pause(){
		Time.timeScale = 0;
		pause_velocities = new Vector3[enemy_spawn.GetEnemyArray ().Length + 1];
		pause_velocities [0] = player.GetComponent<Rigidbody> ().velocity;
		int i = 1;
		foreach (GameObject enemy in enemy_spawn.GetEnemyArray()) {
			pause_velocities [i++] = enemy.GetComponent<Rigidbody> ().velocity;
			enemy.GetComponent<Rigidbody> ().Sleep ();
			enemy.GetComponent<EnemyController> ().enabled = false;
		}
		player.GetComponent<Rigidbody> ().Sleep ();
		player.GetComponent<PlayerController> ().enabled = false;
		paused_text.gameObject.SetActive (true);
	}

	void Unpause(){
		Time.timeScale = 1;
		player.GetComponent<Rigidbody> ().WakeUp();
		player.GetComponent<Rigidbody> ().velocity = pause_velocities [0];
		int i = 1;
		foreach (GameObject enemy in enemy_spawn.GetEnemyArray()) {
			enemy.GetComponent<Rigidbody> ().WakeUp();
			enemy.GetComponent<Rigidbody> ().velocity = pause_velocities [i++];
			enemy.GetComponent<EnemyController> ().enabled = true;
		}
		player.GetComponent<PlayerController> ().enabled = true;
		paused_text.gameObject.SetActive (false);
	}
}
