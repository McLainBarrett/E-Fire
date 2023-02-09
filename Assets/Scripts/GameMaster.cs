using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

	public static int isPaused;
	public static int extraPlayers = 0;
	public static float resource = 100;
	public static GameMaster Self;
	public List<PlayerController> Players;
	[Header("References")]
	public GameObject FixIndicator;
	public GameObject Explosion;

	private void Start() {
		Self = this;
	}

	private void Update() {
		#region Debug
		if (Input.GetKeyDown(KeyCode.Keypad5)) {
			resource += 1000;
		}
		if (Input.GetKeyDown(KeyCode.Asterisk)) {
			GameObject.Find("Spawner").SetActive(false);
		}
		if (Input.GetKeyDown(KeyCode.Slash)) {
			SceneManager.LoadScene(0);
		}
		if (Input.GetKeyDown(KeyCode.Keypad1)) {
			GameObject.Find("Player").GetComponent<PlayerController>().ForceLive();
		}
		if (Input.GetKeyDown(KeyCode.Keypad2)) {
			GameObject.Find("Player2").GetComponent<PlayerController>().ForceLive();
		}
		if (Input.GetKeyDown(KeyCode.Keypad3)) {
			GameObject.Find("Player3").GetComponent<PlayerController>().ForceLive();
		}
		#endregion

		if (Input.GetKeyDown(KeyCode.KeypadPlus) && Display.displays.Length > extraPlayers + 1) {
			extraPlayers++;
			Players[extraPlayers].gameObject.SetActive(true);
			Display.displays[extraPlayers].Activate();
		}

		int pNum = 0;
		foreach (PlayerController item in Players) {
			if (item.Menu)
				pNum++;
		}
		isPaused = pNum;

		if (isPaused > extraPlayers) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}
}