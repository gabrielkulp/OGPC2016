using UnityEngine;
using System.Collections;

public class LandingShip : MonoBehaviour {
	public Vector3 stabilizingDrag = new Vector3(0.5f, 1f, 0f);
	public Vector3 drag = new Vector3(2f, 8f, 0.05f);
	public float trim = 0.1f;
	public float pitchTorque = 1f;
	public float rollTorque = 2f;
	public float yawTorque = 3f;
	public float thrust = 500f;
	public float yawPortion = 0.3f;
	public float maxSqrVel = 800f;
	public float minSqrVel = 200f;
	Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		//Old method
		//rb.velocity = transform.forward * speed;

		//engine
		rb.AddRelativeForce(0f, 0f, thrust);

		//Wings and drag
		Vector3 dragDir = transform.InverseTransformDirection(rb.velocity);
		Vector3 dragForce = -Vector3.Scale(dragDir, drag) * rb.velocity.magnitude;
		rb.AddRelativeForce(dragForce);

		//Stabilization
		float sqrVel = Mathf.Pow(Vector3.Dot(rb.velocity, transform.forward), 2);
		Vector3 stabilizationForces = -Vector3.Scale(dragDir, stabilizingDrag) * rb.velocity.magnitude;
		rb.AddRelativeTorque(stabilizationForces.y, stabilizationForces.z, -stabilizationForces.x);

		//Control
		rb.AddRelativeTorque(Mathf.Clamp(sqrVel, minSqrVel, maxSqrVel) * new Vector3(
			(Input.GetAxis("Vertical") - trim) * pitchTorque,
			Input.GetAxis("Horizontal") * yawPortion * yawTorque,
			Input.GetAxis("Horizontal") * rollTorque * -1));
	}
}
