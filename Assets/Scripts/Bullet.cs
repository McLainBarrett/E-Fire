using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private void Start() {
		StartCoroutine(Death());
	}

	IEnumerator Death() {
		yield return new WaitForSeconds(3);
		Destroy(gameObject);
	}
}