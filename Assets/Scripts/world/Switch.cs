using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	public Transform lever;
	public Vector2 xRotLimit; 
	public Transform target;
	public Vector3 targetPos;
	public Vector3 targetRot;
	bool activated = false;

	void Start () {
		lever.localEulerAngles = Vector3.forward * xRotLimit.x;
	}
	
	void Update () {
		if (activated) {
			lever.localRotation = Quaternion.Slerp(lever.localRotation, Quaternion.Euler(Vector3.forward * xRotLimit.y), 0.01f);
			target.localPosition = Vector3.Lerp(target.localPosition, targetPos, 0.01f);
			target.localRotation = Quaternion.Slerp(target.localRotation, Quaternion.Euler(targetRot), 0.01f);
		}
	}

	void OnTriggerStay (Collider other) {
		if (other.tag == "Player" && Input.GetButtonUp("Use")) {
			activated = true;
		}
	}
}
