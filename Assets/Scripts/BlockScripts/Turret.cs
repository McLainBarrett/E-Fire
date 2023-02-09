using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Block {

	public GameObject bullet;

	public int SubType = 0;
	[Header("Turret Stats")]
	#region Stats
	public float fireDelay = 1;
	public float range = 10;
	public float fireVel = 10;
	public float bulletSize = 0.25f;
	public float bulletMass = 1;
	#endregion
	protected GameObject Target;
	protected float coolDown = 0;

	protected new void Awake() {
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		FixInd = Instantiate(GameMaster.Self.FixIndicator, transform);
		FixInd.transform.localPosition = Vector3.zero;
		gameObject.layer = 8;
		if (!bullet) {
			bullet = GameObject.Find("Player").GetComponent<PlayerController>().Bullet;
		}
	}

	private void FixedUpdate() {
		if (!Target) {
			Target = GetTarget();
			return;
		}

		coolDown -= Time.fixedDeltaTime;
		if (coolDown <= 0) {
			Fire(Target.transform.position - transform.position);
		}
		if (Vector2.Distance(Target.transform.position, transform.position) > range) {
			Target = null;
		}
	}

	protected void Fire(Vector2 direction) {
		direction.Normalize();
		GameObject B = Instantiate(bullet, (Vector2)transform.position, Quaternion.identity);
		B.GetComponent<Rigidbody2D>().velocity = direction * fireVel;
		B.GetComponent<Rigidbody2D>().mass = bulletMass;
		B.transform.localScale = new Vector2(bulletSize, bulletSize);
		coolDown = fireDelay;
	}
	protected GameObject GetTarget() {
		List<Collider2D> cols = new List<Collider2D>();
		List<GameObject> targs = new List<GameObject>();
		cols.AddRange(Physics2D.OverlapCircleAll(transform.position, range));
		foreach (Collider2D col in cols) {
			if (col.GetComponent<Enemy>()) {
				return col.gameObject; //targs.Add(col.gameObject);
			}
		}
		return null;


		//targs = targs.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();
		//foreach (GameObject targ in targs) {
		//	Vector2 dir = targ.transform.position - transform.position;
		//	if (Physics2D.Raycast((Vector2)transform.position + dir.normalized, dir.normalized, dir.magnitude).collider.gameObject == targ) {
		//		return targ;
		//	}
		//}
		//return null;
	}
}