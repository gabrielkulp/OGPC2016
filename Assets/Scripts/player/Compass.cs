using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public Transform[] follow;
	public int index = 0;
	Transform needle;
	bool showing = false;
	Vector3 showpos;

	void Start () {
		needle = transform.GetChild(0);
		showpos = transform.localPosition;
		index = PlayerPrefs.GetInt("CompassIndex", 0);
	}
	
	void Update () {

		showing = Input.GetButton("Compass");

		if (showing) {
			if (follow != null) {
				Vector3 needleDir = Vector3.ProjectOnPlane(transform.position - follow[index].position, transform.up);
				needle.LookAt(transform.position + needleDir, transform.up);
			} else {
				needle.localRotation *= Quaternion.Euler(Vector3.forward);
			}
			transform.localPosition = showpos;
		} else
			transform.localPosition = showpos + Vector3.back;
	}
}
