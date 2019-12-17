using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class EnemyTargetCycling : MonoBehaviour {

	public MultiplayerCamera camera;
	public PlayerController player;
	public EnemySpawner spawner;
	public float outline_flash_duration = 1f;
	public float outline_flash_max_size = 1f;
	public float outline_min_size = 1f;
	public bool manual_enemy_list;
	public GameObject[] enemies;

	GameObject[] active_enemies;
	Transform targeted_enemy;
	int target_index = 0;
	int flashing = -1;

	// Use this for initialization
	void Start () {
		TargetNextEnemy ();
	}
	
	// Update is called once per frame
	void Update () {
		var inputDevice = InputManager.ActiveDevice;
		if (inputDevice == null)
		{
			Debug.LogWarning ("WARNING: MainMenuController could not find controller!");
		}

		if (targeted_enemy == null || !targeted_enemy.gameObject.activeInHierarchy) {
			TargetNextEnemy ();
		} else if (targeted_enemy != null && targeted_enemy.GetComponent<EnemyController> () != null && targeted_enemy.GetComponent<EnemyController> ().isDead ()) {
			TargetNextEnemy ();
		} else if (targeted_enemy != null && targeted_enemy.gameObject.activeInHierarchy) {
			camera.P2 = targeted_enemy;
			player.enemy_target = targeted_enemy;
		} else {
			camera.P2 = player.transform;
			player.enemy_target = player.target;
		}

		if (inputDevice.Action2.WasPressed)
			TargetNextEnemy ();
	}

	void TargetNextEnemy(){
		if (manual_enemy_list) {
			active_enemies = enemies;
		} else {
			active_enemies = spawner.GetEnemyArray ();
		}
		int i = target_index + 1;
		while (true) {
			if (i >= active_enemies.Length)
				i = 0;

			if (active_enemies [i] != null && active_enemies[i].activeInHierarchy) {
				targeted_enemy = active_enemies [i].transform;
				target_index = i;
				break;
			}
			if (i == target_index) {
				targeted_enemy = null;
				break;
			}
			++i;
		}
		if (flashing != target_index)
			StartCoroutine (FlashOutline (targeted_enemy.gameObject));
	}

	IEnumerator FlashOutline(GameObject target){
		Material m = target.GetComponent<Renderer> ().material;
		if (m == null)
			yield break;

		flashing = target_index;

		float t = 0;
		while (t < outline_flash_duration / 2) {
			t += Time.deltaTime;
			float lerped = Mathf.Lerp (outline_min_size, outline_flash_max_size, t / (outline_flash_duration / 2));
			m.SetFloat ("_Outline", lerped);
			yield return null;
		}
		m.SetFloat ("_Outline", outline_flash_max_size);
		t = 0;
		while (t < outline_flash_duration / 2) {
			t += Time.deltaTime;
			float lerped = Mathf.Lerp (outline_flash_max_size, outline_min_size, t / (outline_flash_duration / 2));
			m.SetFloat ("_Outline", lerped);
			yield return null;
		}
		m.SetFloat ("_Outline", outline_min_size);
	}
}
