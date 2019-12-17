using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	public int num_enemies = 1;
	public float min_distance = 0;
	public float max_failures_before_break = 3;
	public GameObject enemy;

	Collider collider;
	Vector3[] spawn_points;
	GameObject[] instantiated_enemies;
	int num_active_enemies = 0;
	int num_alive_enemies = 0;

	public GameObject[] GetEnemyArray(){
		return instantiated_enemies;
	}

	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider> ();
		Spawn ();
	}

	void Update(){
		num_alive_enemies = 0;
		foreach (GameObject e in instantiated_enemies) {
			if (e != null && e.activeInHierarchy)
				num_alive_enemies++;
		}
	}

	public int AliveEnemies(){
		return num_alive_enemies;
	}

	public void Spawn(){
		if (num_active_enemies != 0) {
			foreach (GameObject e in instantiated_enemies)
				Destroy (e);
		}

		Vector3 region_center = collider.bounds.center;
		Vector3 region_size = collider.bounds.size;

		spawn_points = new Vector3[num_enemies];
		instantiated_enemies = new GameObject[num_enemies];

		int i = 0;
		int num_failures = 0;
		while (i < num_enemies){
			float x = Random.Range (region_center.x - region_size.x/2, region_center.x + region_size.x/2);
			float z = Random.Range (region_center.z - region_size.z/2, region_center.z + region_size.z/2);

			Vector3 spawn_point = new Vector3 (x, transform.position.y, z);
			Debug.Log ("Potential spawn_point: " + spawn_point.ToString ());

			int num_collided = 0;
			foreach (Vector3 point in spawn_points) {
				if (Vector3.Distance (point, spawn_point) < min_distance) {
					num_collided++;
					continue;
				}
			}

			if (num_failures == max_failures_before_break) {
				Debug.LogWarning ("WARNING: giving up on spawn placement.");
				break;
			}

			if (num_collided > 0) {
				num_failures++;
				continue;
			} else {
				num_failures = 0;
				spawn_points [i] = spawn_point;
				++i;
			}
		}
		num_active_enemies = i;
		num_alive_enemies = i;

		for (int j = 0; j < i; j++) {
			instantiated_enemies[j] = Instantiate (enemy, spawn_points [j], Quaternion.LookRotation (transform.forward));
		}
	}

	public void ResetEnemies(){
		for (int i = 0; i < num_active_enemies; i++) {
			instantiated_enemies [i].SetActive (true);
			instantiated_enemies [i].transform.position = spawn_points [i];
			instantiated_enemies [i].transform.rotation = Quaternion.LookRotation (transform.forward);
			instantiated_enemies [i].GetComponent<EnemyController> ().Reset ();
		}
	}
}
