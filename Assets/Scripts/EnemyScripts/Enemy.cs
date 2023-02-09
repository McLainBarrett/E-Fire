using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	#region Tier Specification
	/*
		I (0)

		II (20)

		III (40)

		IV (60)


	 */
	#endregion

	protected Rigidbody2D rb;
	protected SpriteRenderer sr;
	protected bool noDamage = true;
	[Header("Enemy Stats")]
	public float health = 20;
	public float damage = 5;
	public float speed = 5;
	public float value = 10;
	[Header("Spawn Stats")]
	public float spawnChance = 0.5f;
	public float spawnCost = 0.5f;
	public int minLevel = 0;
	private float totalHealth;

	protected void Start() {
		StatBuff();
		totalHealth = health;
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		StartCoroutine(SpawnTimer(1));
	}
	protected void Update() {
		sr.color = Color.Lerp(Color.red, Color.yellow, health / totalHealth);
		if (health <= 0) {
			GameMaster.resource += value;
			Destroy(gameObject);
		}
	}

	private void FixedUpdate() {
		if (Spawner.self.Target)
			InfantryAI();
	}
	protected void StatBuff() {
		int wC = Spawner.waveCount;
		float buff = 1;
		if (wC > 20) {
			wC -= 20;
			buff = 1 + wC / 40;
		}
		buff *= Random.Range(0.5f, 2);
		health *= buff;
		damage *= buff;
		speed *= buff;
	}

	protected void InfantryAI() {
		Vector2 direction = Spawner.self.Target.transform.position - transform.position;
		Attack(direction);

		int facing = 0;
		if (direction.x >= 0)
			facing = 1;
		else
			facing = -1;

		rb.velocity = new Vector2(speed * facing, rb.velocity.y);

		RaycastHit2D Hit = Physics2D.Raycast((Vector2)transform.position + Vector2.right * facing, Vector2.right * facing, 2);
		if ((Hit.collider && Hit.collider.gameObject.GetComponent<Block>()) || (Mathf.Abs(direction.x) < 3 && direction.y > 3))
			Jump();
	}

	protected void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.GetComponent<Bullet>() || col.gameObject.GetComponent<Block>()) {
			health -= col.relativeVelocity.magnitude;
		}
	}

	protected void Attack(Vector2 direction) {
		direction.Normalize();
		Collider2D col = Physics2D.Raycast((Vector2)transform.position + direction, direction, 1).collider;
		if (!col)
			return;
		GameObject target = col.gameObject;
		if (target.GetComponent<Block>()) {
			target.GetComponent<Block>().health -= Time.fixedDeltaTime * damage;
		} else if (target.GetComponent<PlayerController>()) {
			target.GetComponent<PlayerController>().Health -= Time.fixedDeltaTime * damage;
		}
	}
	protected void Jump() {
		if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), Vector2.down, 0.1f).collider) {
			rb.velocity = new Vector2(rb.velocity.x, 10);
		}
	}

	protected IEnumerator SpawnTimer(float time) {
		noDamage = true;
		yield return new WaitForSeconds(time);
		noDamage = false;
	}
}