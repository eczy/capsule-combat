using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float max_health = 1f;
	public float health = 1f;
	public bool invincible = false;
	public bool flash_on_damage = false;
	public Color flash_color = Color.white;
	public float flash_frequency = 1f;
	public float flash_duration = 1f;
	public bool regen = false;
	public float regen_rate = 1f;
	public float regen_amount = 1f;
	public float regen_cooldown = 1f;
	public bool flash_outline_when_regenerating = false;
	public float outline_flash_frequency = 1f;
	public float outline_flash_size_change = 1f;

	Renderer rend;
	DamageColor dc;
	bool flashing = false;
	bool regenerating = false;
	float time_to_regen = 0f;
	Coroutine cooldown_co;

	void Start(){
		rend = GetComponent<Renderer> ();
		dc = GetComponent<DamageColor> ();
		if (regen)
			StartCoroutine (CooldownRegenerate ());
	}

	public void StartRegenerate(){
		if (regen && !regenerating && health < max_health) {
			time_to_regen = 0f;
			regenerating = true;
			StartCoroutine (Regenerate ());
			if (flash_outline_when_regenerating)
				StartCoroutine (FlashOutline ());
		}
	}

	public void StopRegenerate(){
		regenerating = false;
		time_to_regen = 0f;
	}

	IEnumerator CooldownRegenerate(){
		while (true) {
			if (time_to_regen > regen_cooldown)
				regen = true;
			else
				regen = false;
			time_to_regen += Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator Regenerate(){
		regen = false;
		while (regenerating) {
			yield return new WaitForSeconds (1/regen_rate);
			if (health >= max_health || !regenerating) {
				StopRegenerate ();
				yield break;
			} else {
				Heal (regen_amount);
			}
		}
	}

	IEnumerator FlashOutline(){
		Material m = GetComponent<Renderer> ().material;
		if (m == null)
			yield break;

		float original_outline_size = m.GetFloat ("_Outline");
		float t = 0;
		while (regenerating) {
			m.SetFloat("_Outline", original_outline_size + (Mathf.Sin(t * outline_flash_frequency)+1) * outline_flash_size_change);
			t += Time.deltaTime;
			yield return null;
		}
		m.SetFloat ("_Outline", original_outline_size);
	}

	public void Damage(float amount, float freq=-1f){
		StopRegenerate ();
		if (invincible)
			return;
		else if (flash_on_damage && !flashing) {
			if (freq == -1f)
				freq = flash_frequency;
			StartCoroutine (DamageAndFlash (amount, freq));
			return;
		} else {
			health -= amount;
			if (health < 0)
				health = 0;
		}
	}

	public void Heal(float amount){
		health += amount;
		if (health > max_health)
			health = max_health;
	}

	public IEnumerator Invincibility(float time){
		if (invincible)
			yield break;
		invincible = true;
		yield return new WaitForSeconds (time);
		invincible = false;
	}

	public IEnumerator DamageAndFlash(float amount, float freq){
		if (invincible)
			yield break;
		health -= amount;
		if (health < 0)
			health = 0;
		flashing = true;
		dc.enabled = false;
		Color original_color = rend.material.color;
		float t = 0;
		float s = 0;
		while (t < flash_duration) {
			while (s < 1 / freq) {
				s += Time.deltaTime;
				t += Time.deltaTime;
				yield return null;
			}
			s = 0;
			if (rend.material.color == original_color)
				rend.material.color = flash_color;
			else
				rend.material.color = original_color;
			yield return null;
		}
		rend.material.color = original_color;
		dc.enabled = true;
		flashing = false;
	}
}
