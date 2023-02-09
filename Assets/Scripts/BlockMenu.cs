using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMenu : MonoBehaviour {

	private bool active;
	public bool weldingMode;

	#region References
		private PlayerController player;
		private GameObject Menu;
		private GameObject Backing;
		public GameObject button;
		public Text BlockInfo;
		private LineViewer LV;
	#endregion

	private void Start() {
		player = transform.GetComponentInParent<PlayerController>();
		Menu = transform.Find("BlockMenu").gameObject;
		Backing = transform.Find("Backing").gameObject;
		LV = transform.parent.GetComponentInChildren<LineViewer>();
		for (int i = 0; i < player.BlockList.Count; i++) {
			GameObject b = Instantiate(button, Menu.transform);
			var e = i; b.GetComponent<Button>().onClick.AddListener(() => SelectBlock(e));
			b.GetComponent<Image>().sprite = player.BlockList[i].GetComponent<SpriteRenderer>().sprite;
			//	
			int height = Mathf.FloorToInt(450/50);
			Vector2 pos = new Vector2((i + height) / height * -50, ((i + height) % height) * -50);
			b.transform.localPosition = pos;
		}
	}
	private void Update() {
		if (active != player.Menu) {
			ToggleMenu(!active);
		}
		if (!active)
			weldingMode = false;
		if (weldingMode) {
			LV.Draw = true;
		} else {
			LV.Draw = false;
		}
	}

	private void ToggleMenu(bool State) {
		Menu.SetActive(State);
		Backing.SetActive(State);
		active = State;
		if (player.playerNum != "1_" && State) {
			ToggleViewWeld();
		} else if (player.playerNum != "1_") {
			weldingMode = false;
		}
	}
	public void SelectBlock(int ID) {
		player.blockType = ID;
		GameObject block = player.BlockList[ID];
		BlockInfo.text = block.name + "\n" + block.GetComponent<Block>().cost;
	}

	public void ToggleViewWeld() {
		weldingMode = !weldingMode;
		Menu.SetActive(!weldingMode);
		player.Welding = weldingMode;
	}
}