using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Block : MonoBehaviour {

	#region Stats
	public int Type = 0;
	public float cost = 20;
	public float totalHealth = 20;
	public float health = 20;
	#endregion
	#region References
	public static GameObject blockPrefab;
	public Rigidbody2D rb;
	public SpriteRenderer sr;
	public PlayerController player;
	protected GameObject FixInd;
	#endregion
	public bool Fixed = true;
	public bool Use;

	protected void Awake() {
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		//player = PlayerController.player.GetComponent<PlayerController>();
		FixInd = Instantiate(GameMaster.Self.FixIndicator, transform);
		FixInd.transform.localPosition = Vector3.zero;
	}
	protected void Update() {
		if (GameMaster.isPaused > 0 && sr.isVisible && Fixed)
			FixInd.SetActive(true);
		else
			FixInd.SetActive(false);

		if (health <= 0 || transform.position.y < -10) {
			Destroy(gameObject);
		}
		float HP = health / totalHealth;
		sr.color = new Color(HP, 1, HP, sr.color.a);
	}

	public void Fix(bool b) {
		Fixed = b;
		if (!rb) {
			rb = GetComponent<Rigidbody2D>();
		}
		if (Fixed) {
			rb.bodyType = RigidbodyType2D.Static;
		} else {
			rb.bodyType = RigidbodyType2D.Dynamic;
		}
	}
	public static void PlaceBlock(Vector2 position, GameObject block, ref float resource) {
		Collider2D col = Physics2D.OverlapBox(position, new Vector2(0.5f, 0.5f), 90);
		Block B = null; if (col) B = col.gameObject.GetComponent<Block>();
		float cost = block.GetComponent<Block>().cost;

		if (col && B) {
			//Destroy block and place another
			float val = Mathf.Clamp((B.totalHealth - B.health)/B.totalHealth * B.cost, 0, resource);
			resource -= val;
			B.health += val;
		} else if ((!col || (!col.GetComponent<PlayerController>() && !col.GetComponent<Enemy>())) && resource >= cost) {
			GameObject Bl = Instantiate(block, position, Quaternion.identity);
			resource -= cost;
		}
	}
}