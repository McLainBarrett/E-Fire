using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : Block {

	private void FixedUpdate() {
		foreach(Collider2D Col in Physics2D.OverlapBoxAll(transform.position, new Vector2(0.5f, 0.5f), 90)) {
			PlayerController PC = Col.GetComponent<PlayerController>();
			float cost = Time.fixedDeltaTime * 5;
			if (PC && GameMaster.resource >= cost && PC.Health + cost < 100 && PC.Health > 0) {
				PC.Health += cost;
				GameMaster.resource -= cost;
			}
		}
	}
}