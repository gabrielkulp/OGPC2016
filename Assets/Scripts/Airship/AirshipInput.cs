using UnityEngine;
using System.Collections;

public class AirshipInput : MonoBehaviour {
	public AirshipController controller;
	public Transform yaw, climb, throttle;

	public AnimationCurve outputScale;
	Vector3 input;
	Vector3 output;
	
	void Start () {
		yaw.localRotation = Quaternion.identity;
		climb.localRotation = Quaternion.identity;
		throttle.localRotation = Quaternion.identity;
	}
	
	void Update () {
		if (Mathf.Abs(input.z) < 0.2f && !Input.GetButton("Ship Throttle"))
			input.z *= 1f - (Time.deltaTime * 10f);

		output = AnimScale(input);

		yaw.localRotation = Quaternion.Lerp(
			yaw.localRotation,
			Quaternion.Euler(0, 0, output.x * -360f),
			0.1f);
		climb.localEulerAngles = Vector3.forward * output.y * -75f;
		throttle.localEulerAngles = Vector3.forward * output.z * 75f;

		controller.input = output;
		
	}

	void OnTriggerStay (Collider other) {
		if (other.tag == "Player") {
			input.x = Input.GetAxis("Ship Steer");
			input.y = Input.GetAxis("Ship Climb");
			input.z += Input.GetAxis("Ship Throttle") * Time.deltaTime;
			input.z = Mathf.Clamp(input.z, -1f, 1f);
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "Player") {
			input.x = 0f;
			input.y = 0f;
		}
	}

	Vector3 AnimScale (Vector3 vector) {
		vector.x = outputScale.Evaluate(vector.x);
		vector.y = outputScale.Evaluate(vector.y);
		//No curve on throttle

		return vector;
	}
}
