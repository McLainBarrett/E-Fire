using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : Block {

	protected new void Update() {
		if (player.Menu && sr.isVisible && Fixed)
			FixInd.SetActive(true);
		else
			FixInd.SetActive(false);

		if (health <= 0 || transform.position.y < -10) {
			Destroy(gameObject);
		}
		float HP = health / totalHealth;
		sr.color = new Color(HP, 1, HP, sr.color.a);

		if (Use) {
			Color color = sr.color;
			color.a = 0.75f;
			sr.color = color;
		} else {
			Color color = sr.color;
			color.a = 1;
			sr.color = color;
		}
	}

	protected void FixedUpdate() {
		if (Use) {
			if (rb.bodyType == RigidbodyType2D.Static) {
				transform.position += new Vector3(0, Time.deltaTime, 0);
			} else {
				rb.velocity = Vector2.up * 2;
			}
		}
	}
}