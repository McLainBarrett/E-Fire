using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public float radius;
	private SpriteRenderer Backing;
	ParticleSystem PS;
	private float time;
	private Color color;

	void Start() {
		gameObject.transform.localScale *= radius / 2;
		Backing = GetComponentInChildren<SpriteRenderer>();
		color = Backing.color;

		PS = GetComponent<ParticleSystem>();
		ParticleSystem.MainModule PSM = PS.main;
		PSM.startSpeedMultiplier = radius * 2;
		PSM.startSizeMultiplier = radius;
		ParticleSystem.ShapeModule PSS = PS.shape;
		PSS.radius = radius;

		PS.Play();
		time = PS.main.duration;
		Destroy(gameObject, PS.main.duration);
	}
	private void Update() {
		time -= Time.deltaTime;
		if (Backing) {
			color.a = time / PS.main.duration;
			Backing.color = color;
		}
	}
}