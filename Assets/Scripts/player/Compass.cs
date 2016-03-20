using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public Transform follow;
	Transform needle;
	bool showing = false;
	Vector3 showpos;

	void Start () {
		needle = transform.GetChild(0);
		showpos = transform.localPosition;
	}
	
	void Update () {
		if (follow != null) {
			//Vector3 needleDir = Vector3.ProjectOnPlane(transform.position - follow.position, transform.up);
			//needle.LookAt(needleDir, transform.up);
			needle.LookAt(follow, transform.up);
			needle.rotation *= Quaternion.FromToRotation(needle.up, transform.up);
		} else {
			needle.localRotation *= Quaternion.Euler(Vector3.forward);
		}

		showing = Input.GetButton("Compass");

		//if (showing)
		//	transform.localPosition = showpos;
		//else
		//	transform.localPosition = showpos + Vector3.back;
	}
}
