using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	public Transform lever;
	public Vector2 xRotLimit; 
	public Transform target;
	public Vector3 targetPos;
	public Vector3 targetRot;
	bool activated = false;
    public float lerpCoeff = 0.1f;

	void Start () {
		lever.localEulerAngles = Vector3.forward * xRotLimit.x;
	}
	
	void Update () {
		if (activated) {
			lever.localRotation = Quaternion.Slerp(lever.localRotation, Quaternion.Euler(Vector3.forward * xRotLimit.y), lerpCoeff);
			target.localPosition = Vector3.Lerp(target.localPosition, targetPos, lerpCoeff);
			target.localRotation = Quaternion.Slerp(target.localRotation, Quaternion.Euler(targetRot), lerpCoeff);
		}
	}

	void OnTriggerStay (Collider other) {
		if (other.tag == "Player" && Input.GetButton("Use")) {
            target.gameObject.SetActive(true);
            activated = true;
		}
	}
}
