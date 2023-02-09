using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour {

	public static Spawner self;
	public GameObject Target;
	private List<GameObject> Enemies;
	private List<GameObject> validTypes;
	private List<float> chanceChart;
	//
	public float waveWeight = 0;
	public static int waveCount = 0;
	public float time = 5;
	public float IEtime = 0;

	private void Start() {
		self = this;
		StartCoroutine(TargLoop());
	}

	private void FixedUpdate() {
		time -= Time.fixedDeltaTime;
		IEtime += Time.fixedDeltaTime;

		if (waveWeight <= 0 && time <= 0) {
			waveCount += 1;
			SetUpWave();
		}
		if (waveWeight > 0) {
			float waveValue = 0;
			if (waveWeight >= 25)
				waveValue = 25;
			else
				waveValue = waveWeight % 25;
			waveWeight -= waveValue;
			SpawnWave(waveValue);
		}
	}

	private void SetUpWave() {
		Enemies = new List<GameObject>();

		Enemies.AddRange(Resources.LoadAll<GameObject>("Enemies/I"));
		Enemies.AddRange(Resources.LoadAll<GameObject>("Enemies/II"));
		Enemies.AddRange(Resources.LoadAll<GameObject>("Enemies/III"));
		Enemies.AddRange(Resources.LoadAll<GameObject>("Enemies/IV"));

		validTypes = new List<GameObject>();
		chanceChart = new List<float>();

		for (int i = 0; i < Enemies.Count; i++) {
			Enemy e = Enemies[i].GetComponent<Enemy>();
			if (e.minLevel <= waveCount) {
				validTypes.Add(Enemies[i]);
				#region ChanceCalculation
				float totalChance = 0;
				if (i > 0)
					totalChance = chanceChart[i - 1];

				chanceChart.Add(e.spawnChance + totalChance);
				#endregion
			}
		}

		waveWeight = waveCount;
		time = waveCount * 0.05f + 3;
	}
	private void SpawnWave(float weight) {
		while (weight > 0 && validTypes.Count > 0) {
			float eType = Random.Range(0, chanceChart.Max());

			for (int i = 0; i < validTypes.Count; i++) {
				if (eType < chanceChart[i] ) {
					weight -= validTypes[i].GetComponent<Enemy>().spawnCost;
					Instantiate(validTypes[i], transform.position, Quaternion.identity);
					break;
				}
			}
		}
	}

	IEnumerator TargLoop() {
		List<GameObject> TList = GameObject.FindGameObjectsWithTag("Target").ToList();
		if (TList.Count < 1) {
			Application.Quit();
			yield return null;
		}
		TList.AddRange(GameObject.FindGameObjectsWithTag("MinorTarget"));
		TList = TList.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();
		if (TList.Count >= 1) {
			Target = TList[0];
		}
		yield return new WaitUntil(() => !Target || IEtime > 1);
		IEtime = 0;
		StartCoroutine(TargLoop());
	}
}