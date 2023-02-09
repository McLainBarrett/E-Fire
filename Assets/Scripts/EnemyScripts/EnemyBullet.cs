using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
	
	public float damage;

	public bool explosive;
	public float radius;
	public float explosionDamage;

	private void Start() {
		StartCoroutine(Death());
	}
	IEnumerator Death() {
		yield return new WaitForSeconds(3);
		Explode();
		Destroy(gameObject);
	}

	private void Update() {
		if (damage <= 0) {
			if (explosive)
				Explode();
			Destroy(gameObject);
		}
	}
	private void Explode() {
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);

		GameObject Explosion = Instantiate(GameMaster.Self.Explosion, transform.position, Quaternion.identity);
		Explosion.GetComponent<Explosion>().radius = radius;

		foreach (Collider2D col in cols) {
			Block b = col.gameObject.GetComponent<Block>();
			if (b)
				b.health -= explosionDamage;

			PlayerController pc = col.gameObject.GetComponent<PlayerController>();
			if (pc)
				pc.Health -= explosionDamage;

			if (b || pc) {
				Vector2 direction = col.transform.position - transform.position;
				col.GetComponent<Rigidbody2D>().AddForce(direction.normalized * 1 / direction.magnitude);
			}
		}
	}
	private void OnCollisionEnter2D(Collision2D col) {
		Block b = col.gameObject.GetComponent<Block>();
		if (b) {
			b.health -= damage;
			damage = 0;
		}


		PlayerController pc = col.gameObject.GetComponent<PlayerController>();
		if (pc) {
			pc.Health -= damage;
			damage = 0;
		}
	}
}