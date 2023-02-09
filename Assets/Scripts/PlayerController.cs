using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	#region TODO
	/*



		New damage system
		Bullet has set damage, if hit target, damage target. If target has more health than bullets damage, just subtract health and destroy bullet.
		If bullet has more damage than enemies health, destroy enemy and subtract difference from bullet's damage.

		Increase enemy health, reduce enemy speed

		---

		Bubble shield
		Infantry Launcher

		---

		Engineer (Circular)- Build structures and tanks

		Team system, (Red, Greeen Blue, White)
		Teams are hostile to each other
		Dealing damage gives resource to that team

		---
		Radar/Firing Computer for turrets, they need detection and computing power

		Artillery
		Rocket Artillery(Burst fire)

		Unwelding

		Shield, Repair station

		Wiring, sensors
	*/
	#endregion

	#region References
	public GameObject Bullet;
		public GameObject block;
		public GameObject GhostBlock;
		public GameObject Cursor;
		public BlockMenu blockMenu;
		public RectTransform canvas;
		private Rigidbody2D RB;
		private Image healthBar;
		private Text ResourceI;
		private Image TypeI;
		private Camera Cam;
	#endregion
	#region Control
	public string playerNum;
	public bool dead = false;

	#endregion
	#region Stats
		public float Health = 100;
		private readonly float moveSpeed = 25;
		private readonly float moveForce = 50;
		private readonly float jumpSpeed = 10;
	#endregion
	#region MiscVars
		public bool Menu;
		private bool MenuToggle;
		public bool Welding;
		private int FixState = -1;
		private int UseState = -1;
		private float coolDown;
		private bool grounded;
		private List<Vector2> BList = new List<Vector2>();
		private List<GameObject> GList = new List<GameObject>();
		private bool weldingMode;
		private GameObject targetBlockToWeld;
		private bool PrevFire2;
	#endregion
	#region Blocks
		public int blockType = 0;
		public List<GameObject> BlockList;
	#endregion

	private void Start() {
		Block.blockPrefab = block;
		RB = GetComponent<Rigidbody2D>();
		Cam = GetComponentInChildren<Camera>();
		canvas = GetComponentInChildren<RectTransform>();
		blockMenu = GetComponentInChildren<BlockMenu>();
		#region UI
			healthBar = transform.Find("Canvas").Find("HealthBar").GetComponent<Image>();
			ResourceI = transform.Find("Canvas").Find("ResourceI").GetComponent<Text>();
			TypeI = transform.Find("Canvas").Find("TypeI").GetComponent<Image>();
		#endregion
		#region BlockList
		BlockList = new List<GameObject>();
		BlockList.AddRange(Resources.LoadAll<GameObject>("Blocks"));
		#endregion
	}
	private void Update() {
		#region UI
		if (healthBar.fillAmount > Health / 100) {
			GetComponent<SpriteRenderer>().color = Color.red;
		} else {
			GetComponent<SpriteRenderer>().color = Color.white;
		}

		healthBar.fillAmount = Health / 100;
		ResourceI.text = System.Math.Round(GameMaster.resource, 1).ToString();
		TypeI.sprite = BlockList[blockType].GetComponent<SpriteRenderer>().sprite;
		if (BlockList[blockType].GetComponent<Block>().cost > GameMaster.resource)
			TypeI.color = Color.red;
		else
			TypeI.color = Color.white;
		#endregion
		#region Movement
			if (Input.GetAxis(playerNum + "Vertical") > 0 && Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), Vector2.down, 0.1f).collider && !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.6f), Vector2.up, 0.1f).collider) {
				RB.velocity = new Vector2(RB.velocity.x, jumpSpeed);
			}
			if (Input.GetAxis(playerNum + "Horizontal") < 0 && RB.velocity.x > -moveSpeed) {
				//RB.velocity = new Vector2(-moveSpeed, RB.velocity.y);
				RB.AddForce(new Vector2(-moveForce, 0) * Time.deltaTime);
			} else if (Input.GetAxis(playerNum + "Horizontal") > 0 && RB.velocity.x < moveSpeed) {
				//RB.velocity = new Vector2(moveSpeed, RB.velocity.y);
				RB.AddForce(new Vector2(moveForce, 0) * Time.deltaTime);
			} else if (Input.GetAxis(playerNum + "Horizontal") == 0) {
				RB.velocity = new Vector2(0, RB.velocity.y);
			}

			if (Input.GetButton(playerNum + "Zoom++") || Input.GetAxis(playerNum + "Zoom++") > 0)
				Cam.orthographicSize -= Input.GetAxis(playerNum + "Zoom") * 10;
			else
				Cam.orthographicSize -= Input.GetAxis(playerNum + "Zoom");
			if (Cam.orthographicSize < 1)
				Cam.orthographicSize = 1;

		#endregion
		if (dead)
			return;
		#region Controls
			coolDown -= Time.deltaTime;
			bool Fire1 = (Input.GetButton(playerNum + "Fire1") || Input.GetAxis(playerNum + "Fire1") > 0);
			bool Fire2 = (Input.GetButton(playerNum + "Fire2") || Input.GetAxis(playerNum + "Fire2") > 0);
			if (Fire1 && coolDown <= 0 && !Menu) {
				Fire();
			}
			if (Fire1 && Welding) {
				Vector2 targPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (Cursor)
					targPos = Cursor.transform.position;

				Collider2D col = Physics2D.Raycast(targPos, Vector3.forward, 1f).collider;
				if (col)
					WeldBlock(col.gameObject);
			}
			if (Fire2) {
				FixBlock();
			}
			if (Input.GetButtonUp(playerNum + "Fire2") || (!Fire2 && PrevFire2)) {
				FixState = -1;
			}
			if (Input.GetButton(playerNum + "Menu")) {
				MenuToggle = false;
				Menu = true;
			} else if (!MenuToggle) {
				Welding = false;
				Menu = false;
			}
			if (Input.GetButton(playerNum + "Use")) {
				UseBlock();
			}
			if (Input.GetButtonUp(playerNum + "Use")) {
				UseState = -1;
			}
			if (Input.GetButton(playerNum + "Remove")) {
				DestroyBlock();
			}
			if (Input.GetButton(playerNum + "Build")) {
				Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (Cursor)
					pos = Cursor.transform.position;
				
				pos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
				if (!BList.Contains(pos)) {
					BList.Add(pos);
					GList.Add(Instantiate(GhostBlock, pos, Quaternion.identity));
				}
			}
			if (Input.GetButtonUp(playerNum + "Build")) {
				SetBList();
			}
			if (Input.GetButtonDown(playerNum + "Pause")) {
				MenuToggle = !MenuToggle;
				Menu = MenuToggle;
			}
			PrevFire2 = Fire2;
		#endregion
		#region Controller Control
		if (Cursor) {
			Cursor.transform.localPosition += new Vector3(Input.GetAxis(playerNum + "LookX"), Input.GetAxis(playerNum + "LookY")) * 6;
			if (Cursor.transform.localPosition.x > canvas.rect.width / 2)
				Cursor.transform.localPosition = new Vector2(canvas.rect.width / 2, Cursor.transform.localPosition.y);
			if (Cursor.transform.localPosition.x < -canvas.rect.width / 2)
				Cursor.transform.localPosition = new Vector2(-canvas.rect.width / 2, Cursor.transform.localPosition.y);
			if (Cursor.transform.localPosition.y > canvas.rect.height / 2)
				Cursor.transform.localPosition = new Vector2(Cursor.transform.localPosition.x, canvas.rect.height / 2);
			if (Cursor.transform.localPosition.y < -canvas.rect.height / 2)
				Cursor.transform.localPosition = new Vector2(Cursor.transform.localPosition.x, -canvas.rect.height / 2);
			//Add limits
		}
		if (playerNum != "1_") {
			if (Input.GetButtonDown(playerNum + "SBL")) {
				int blockID = blockType - 1;
				if (blockID < 0)
					blockID = BlockList.Count - 1;
				blockMenu.SelectBlock(blockID);
			}
			if (Input.GetButtonDown(playerNum + "SBR")) {
				int blockID = blockType + 1;
				if (blockID >= BlockList.Count)
					blockID = 0;
				blockMenu.SelectBlock(blockID);
			}
		}
		#endregion
		#region Death
			if (RB.position.y < -10) {
				RB.position = new Vector2(0,2);
			}

			if (Health <= 0) {
				Die();
			}
			if (Health < 100) {
				Health += Time.deltaTime * 0.1f;
			}
		#endregion

	}

	private void Die() {
		dead = true;
		gameObject.layer = 10;
		gameObject.tag = "Untagged";
		StartCoroutine(Live());
	}
	private IEnumerator Live() {
		yield return new WaitForSeconds(5);

		if (GameMaster.resource >= 2000 && dead) {
			Respawn RP = FindObjectOfType<Respawn>();
			if (RP) {
				GameMaster.resource -= 2000;
				gameObject.layer = 8;
				gameObject.tag = "Target";
				Health = 100;
				dead = false;

				transform.position = RP.transform.position;
				gameObject.SetActive(true);
			}
		}
		
		if (dead)
			StartCoroutine(Live());
	}
	public void ForceLive() {
		gameObject.layer = 8;
		gameObject.tag = "Target";
		Health = 100;
		dead = false;

		Respawn RP = FindObjectOfType<Respawn>();
		if (RP)
			transform.position = RP.transform.position;

		gameObject.SetActive(true);
	}
	private void Fire() {
		Vector2 targPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Cursor)
			targPos = Cursor.transform.position;

		Vector2 Direction = targPos - (Vector2)transform.position;
		GameObject B = Instantiate(Bullet, (Vector2)transform.position, Quaternion.identity);
		B.GetComponent<Rigidbody2D>().velocity = Direction.normalized * 20;
		coolDown = 0.5f;
	}
	private void FixBlock() {
		Vector2 targPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Cursor)
			targPos = Cursor.transform.position;

		Collider2D col = Physics2D.Raycast(targPos, Vector3.forward, 1f).collider;
		if (!col)
			return;
		Block B = col.gameObject.GetComponent<Block>();
		if (!B)
			return;
		if (FixState == -1) {
			B.Fix(!B.Fixed);
			if (B.Fixed) FixState = 1;
			else FixState = 0;

		} else if (FixState == 0) {
			B.Fix(false);
		} else {
			B.Fix(true);
		}
	}
	private void UseBlock() {
		Vector2 targPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Cursor)
			targPos = Cursor.transform.position;

		Collider2D col = Physics2D.Raycast(targPos, Vector3.forward, 1f).collider;
		if (col && col.gameObject.GetComponent<Block>()) {
			Block B = col.gameObject.GetComponent<Block>();

			if (UseState == -1) {
				B.Use = !B.Use;
				if (B.Use) UseState = 1;
				else UseState = 0;

			} else if (UseState == 0) {
				B.Use = false;
			} else {
				B.Use = true;
			}
		}
	}
	private void WeldBlock(GameObject blockToWeld) {
		if (!blockToWeld.GetComponent<Block>() || blockToWeld == targetBlockToWeld)
			return;
		if (!targetBlockToWeld) {
			targetBlockToWeld = blockToWeld;
		} else {
			FixedJoint2D joint = targetBlockToWeld.AddComponent<FixedJoint2D>();
			joint.connectedBody = blockToWeld.GetComponent<Rigidbody2D>();
			joint.breakForce = 2000;
			targetBlockToWeld = null;
		}
	}
	private void DestroyBlock() {
		Vector2 targPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Cursor)
			targPos = Cursor.transform.position;

		Collider2D col = Physics2D.Raycast(targPos, Vector3.forward, 1f).collider;
		if (!col)
			return;
		Block B = col.gameObject.GetComponent<Block>();
		if (B) {
			GameMaster.resource += B.health/B.totalHealth * B.cost;
			Destroy(B.gameObject);
		}
	}
	private void SetBList() {
		foreach (Vector2 blockpos in BList) {
			Block.PlaceBlock(blockpos, BlockList[blockType], ref GameMaster.resource);
		}
		foreach (var item in GList) {
			Destroy(item);
		}
		BList.RemoveRange(0, BList.Count);
		GList.RemoveRange(0,GList.Count);
	}
}