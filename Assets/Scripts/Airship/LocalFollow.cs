using UnityEngine;
using System.Collections;

public class LocalFollow : MonoBehaviour {

	public Transform follow;

	void FixedUpdate () {
		transform.localRotation = follow.rotation;
		transform.localPosition = follow.localPosition;
	}
}
