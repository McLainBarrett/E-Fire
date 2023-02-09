using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : Enemy {

	private void FixedUpdate() {
		if (!Spawner.self.Target)
			return;

		Vector2 direction = Spawner.self.Target.transform.position - transform.position;
		Attack(direction);

		rb.velocity = direction.normalized * speed;
		if (direction.magnitude > 10 && direction.y < 5) {
			rb.velocity += new Vector2(0, speed / 4);
		}
	}
}