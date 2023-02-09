using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineViewer : MonoBehaviour {

	public Material Mat;
	public bool Draw;

	private Camera cam;

	private void Start() {
		cam = GetComponent<Camera>();
	}

	private void OnPostRender() {
		if (!Draw)
			return;

		float camSize = cam.orthographicSize * 2;
		Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, new Vector2(camSize * cam.aspect, camSize), 0);
		List<GameObject> Objs = new List<GameObject>();
		foreach (Collider2D col in cols) {
			if (col.GetComponent<FixedJoint2D>()) {
				Objs.Add(col.gameObject);
			}
		}
		foreach (GameObject Obj in Objs) {
			for (int i = 0; i < Obj.GetComponents<FixedJoint2D>().Length; i++) {
				try {
					DrawLine(Obj.transform.position, Obj.GetComponents<FixedJoint2D>()[i].connectedBody.transform.position);
				} catch (System.NullReferenceException e) {
					print(e);
					Destroy(Obj.GetComponents<FixedJoint2D>()[i]);
				}
			}
		}
	}

	public void DrawLine(Vector3 P1, Vector3 P2) {
		GL.Begin(GL.LINES);
		Mat.SetPass(0);
		GL.Color(Mat.color);
		GL.Vertex(P1);
		GL.Vertex(P2);
		GL.End();
	}
}