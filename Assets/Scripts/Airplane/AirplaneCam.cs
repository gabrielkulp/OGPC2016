using UnityEngine;
using System.Collections;

public class AirplaneCam : MonoBehaviour {
	public float sensitivity = 1f;
	public float zoomSensitivity = 1f;
	public Transform center;
	public Vector2 zoomLimit = new Vector2(1f, 30f);
	public Vector2 pitchLimit = new Vector2(10f, 85f);
	Vector3 eulers = Vector3.zero;
	Vector3 smoothEulers = Vector3.zero;
	public float lerpConst = 0.1f;
	float zoomMag = 3f;
	float smoothZoom = 3f;

	void Start () {
		Cursor.visible = false;
		Quaternion startRot;
		startRot = Quaternion.FromToRotation(Vector3.back, transform.position - center.position);
		eulers = startRot.eulerAngles;
		zoomMag = Vector3.Distance(transform.position, center.position);
		transform.LookAt(center, Vector3.up);
	}

	void Update () {
		//Camera rig movement
		eulers.x -= Input.GetAxis("Mouse Y") * sensitivity;
		eulers.y += Input.GetAxis("Mouse X") * sensitivity;
		eulers.x = Mathf.Clamp(eulers.x, pitchLimit.x, pitchLimit.y);

		smoothEulers = Vector3.Lerp(smoothEulers, eulers, lerpConst);

		zoomMag *= 1f + Input.GetAxis("Mouse ScrollWheel") * -zoomSensitivity;
		zoomMag = Mathf.Clamp(zoomMag, zoomLimit.x, zoomLimit.y);
		smoothZoom = Mathf.Lerp(smoothZoom, zoomMag, lerpConst);

		transform.position = center.position + (Quaternion.Euler(smoothEulers) * Vector3.back * smoothZoom);
		transform.LookAt(center, Vector3.up);
	}

	public void ResetPosition () {
		transform.position = center.position - (center.forward * zoomMag);
		transform.LookAt(center.position, Vector3.up);
	}
}

