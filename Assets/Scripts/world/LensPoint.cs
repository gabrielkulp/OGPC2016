using UnityEngine;
using System.Collections;

public class LensPoint : MonoBehaviour {
	public Transform follow;
	public LineRenderer line;
	public float length = 10f;

	void Update () {
		line.SetPosition(1, Quaternion.Inverse(transform.rotation) * 
			Vector3.Scale(new Vector3(1f,0f,1f), (follow.position - transform.position).normalized) * 
			length);
	}
}
