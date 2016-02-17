using UnityEngine;
using System.Collections;

public class Instruments : MonoBehaviour {

	public Rigidbody follow;
	public Transform compassDial;
	public Vector2 needleEulerRange = new Vector2(-120f, 120f);
	public Transform altimeter;
	public Vector2 altimeterRange = new Vector2(-100f, 300f);
	public Transform speedometer;
	public Vector2 speedometerRange = Vector2.right * 200f;

	void FixedUpdate () {
		compassDial.rotation = Quaternion.FromToRotation(follow.transform.forward, Vector3.up);
		Vector3 altRot = altimeter.localEulerAngles;
		altRot.z = findNeedleAngle(altimeterRange, follow.position.y);
		altimeter.localEulerAngles = altRot;

		Vector3 spdRot = speedometer.localEulerAngles;
		spdRot.z = findNeedleAngle(speedometerRange, follow.velocity.magnitude);
		speedometer.localEulerAngles = spdRot;
	}

	float findNeedleAngle (Vector2 range, float value) {
		float output = 0f;
		output =
			Mathf.Lerp(
				needleEulerRange.x,
				needleEulerRange.y,
				Mathf.InverseLerp(range.x, range.y, value)
			);

		return output;
	}
}
