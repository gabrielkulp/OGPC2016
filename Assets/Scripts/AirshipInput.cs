using UnityEngine;
using System.Collections;

public class AirshipInput : MonoBehaviour {
	public AirshipController controller;

	public Player Player;

	public Interactable wheel;
	public Interactable throttle;
	public Interactable climb;
	Vector3 localPos;
	Quaternion localRot;
//	Rigidbody RB;

	void Start () {
		localPos = transform.localPosition;
        localRot = transform.localRotation;
//		RB = GetComponent<Rigidbody>();
	}
	
	void Update () {
		controller.yaw = wheel.output;
		controller.throttle = throttle.output;
		controller.climb = climb.output;
	}

	void FixedUpdate () {
		transform.localPosition = localPos;
		transform.localRotation = localRot;
	}
}
