using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public Transform[] follow;
	public int index = 0;
	Transform needle;

	void Start () {
		needle = transform.GetChild(0);
		index = PlayerPrefs.GetInt("CompassIndex", 0);
	}
	
	void Update () {
		if (follow != null) {
			Vector3 needleDir = Vector3.ProjectOnPlane(transform.position - follow[index].position, transform.up);
			needle.LookAt(transform.position + needleDir, transform.up);
		} else {
			needle.localRotation *= Quaternion.Euler(Vector3.forward);
		}

	}
}
