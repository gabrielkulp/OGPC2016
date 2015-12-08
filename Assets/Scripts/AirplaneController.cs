using UnityEngine;
using System.Collections;

public class AirplaneController : MonoBehaviour {
	public float fuel = 1f;
	public float fuelTime = 5f;	//Minutes of fuel
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

	public Transform rightAileron;
	public Transform leftAileron;
	public float maxAileronDeflection;
	public float aileronDeflectionOffset;
	Vector2 currentAileronDeflection;	//x is right aileron

	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		//Old method:
		//rb.velocity = transform.forward * speed;

		//engine
		fuel -= Time.fixedDeltaTime/(fuelTime * 60);
		fuel = Mathf.Clamp01(fuel);
		if (fuel > 0f)
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
		currentAileronDeflection.x = Mathf.Clamp(Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"), -1f, 1f);
		currentAileronDeflection.y = Mathf.Clamp(-Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"), -1f, 1f);
		rightAileron.localRotation = Quaternion.Slerp(rightAileron.localRotation, Quaternion.Euler (
			Vector3.right * ((currentAileronDeflection.x * maxAileronDeflection) + aileronDeflectionOffset)), .1f);
		leftAileron.localRotation = Quaternion.Slerp(leftAileron.localRotation, Quaternion.Euler(
			Vector3.right * ((currentAileronDeflection.y * maxAileronDeflection) + aileronDeflectionOffset)), .1f);
	}

	public void ShutDown () {
		rightAileron.localEulerAngles = Vector3.right * aileronDeflectionOffset;
		leftAileron.localEulerAngles = Vector3.right * aileronDeflectionOffset;
		this.enabled = false;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.name != "Player")
			return;

		GameObject.Find("Player Controller").GetComponent<PlayerState>().state = playerMode.gliding;
	}

	void OnGUI () {
		GUI.Box(new Rect(Screen.width - 200, Screen.height - 30, 200, 30), "Fuel");
		GUI.Box(new Rect(Screen.width - (200 * fuel), Screen.height - 30, 200 * fuel, 30), "");
	}
}
