using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Enemy {

	[Header("Gun Stats")]
	public float fireDelay;
	public float range;
	public float velocity;
	public float gunDamage;
	public float mass;
	public Vector2 bulletSize;
	public GameObject Projectile;

	[Space(20)]
	public bool explosive;
	public float radius;
	public float explosionDamage;

	private float coolDown;

	private void FixedUpdate() {
		if (!Spawner.self.Target)
			return;

		coolDown -= Time.fixedDeltaTime;

		Vector2 direction = Spawner.self.Target.transform.position - transform.position;
		Attack(direction);
		if (direction.magnitude < range && coolDown <= 0)
			Shoot(direction);

		int facing = 0;
		if (direction.x >= 0)
			facing = 1;
		else
			facing = -1;

		rb.velocity = new Vector2(speed * facing, rb.velocity.y);

		RaycastHit2D Hit = Physics2D.Raycast((Vector2)transform.position + Vector2.right * facing, Vector2.right * facing, 2);
		if (Hit.collider && Hit.collider.gameObject.GetComponent<Block>())
			Jump();
	}

	private void Shoot(Vector2 direction) {
		GameObject B = Instantiate(Projectile, (Vector2)transform.position, Quaternion.Euler(direction.normalized));
		B.transform.localScale *= bulletSize;
		Rigidbody2D rb = B.GetComponent<Rigidbody2D>();
		rb.velocity = direction.normalized * velocity;
		rb.mass = mass;

		coolDown = fireDelay;

		EnemyBullet EB = B.GetComponent<EnemyBullet>();
		if (EB) {
			EB.damage = gunDamage;
			EB.explosive = explosive;
			EB.radius = radius;
			EB.explosionDamage = explosionDamage;
		}
	}
}