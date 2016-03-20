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
		lever.localEulerAngles = Vector3.right * xRotLimit.x;
	}
	
	void Update () {
		if (activated) {
			lever.localRotation = Quaternion.Slerp(lever.localRotation, Quaternion.Euler(Vector3.right * xRotLimit.y), 0.01f);
			target.position = Vector3.Lerp(target.position, targetPos, 0.01f);
			target.rotation = Quaternion.Slerp(target.rotation, Quaternion.Euler(targetRot), 0.01f);
		}
	}

	void OnTriggerStay (Collider other) {
		if (other.tag == "Player" && Input.GetButtonUp("Use")) {
			activated = true;
		}
	}
}
