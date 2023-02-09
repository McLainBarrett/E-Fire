using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Block {

	protected Collider2D Col;

	protected void Start() {
		Col = GetComponent<BoxCollider2D>();
	}

	protected void FixedUpdate() {
		if (Use) {
			gameObject.layer = 10;
			Color color = sr.color;
			color.a = 0.25f;
			sr.color = color;
		} else {
			gameObject.layer = 0;
			Color color = sr.color;
			color.a = 1f;
			sr.color = color;
		}
	}
}