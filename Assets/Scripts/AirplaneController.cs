using UnityEngine;
using System.Collections;

public class AirplaneController : MonoBehaviour {
	public float throttle = 1f;
	public float fuel = 1f;
	public float fuelPerN = 0.001f;	//Fuel lost per N/s of thrust
	public Vector3 stabilizingDrag = new Vector3(0.5f, 1f, 0f);
	public Vector3 drag = new Vector3(2f, 8f, 0.05f);
	public float trim = 0.1f;
	public float pitchTorque = 1f;
	public float rollTorque = 2f;
	public float yawTorque = 3f;
	public float boostTorqueMult = 0.5f;
	public float maxThrust = 500f;
	public float boostThrustMult = 2f;
	public float yawPortion = 0.3f;
	public float maxSqrVel = 800f;
	public float minSqrVel = 200f;
	Rigidbody rb;

	public Transform rightElevon;
	public Transform leftElevon;
	public float maxAileronDeflection;
	public float aileronDeflectionOffset;
	Vector2 currentAileronDeflection;   //x is right aileron
	public GameObject trails;
	public ParticleSystem boostTrail;
	public float boostTrailEmitMult;
	float boostTrailBaseRate;

	public Rigidbody airship;
	public float maxLaunchSpeed = 5f;

	void Start () {
		rb = GetComponent<Rigidbody>();
		trails.SetActive(false);
		if (rightElevon == null || leftElevon == null)
			Debug.LogWarning("Elevons not set up on " + gameObject.name);
		boostTrailBaseRate = boostTrail.startLifetime;
		//boostTrail.startLifetime = 0f;
		
    }

	void FixedUpdate () {
		//Old method:
		//rb.velocity = transform.forward * speed;
		trails.SetActive(true);

		//engine
		float thrust = maxThrust * throttle * (Input.GetButton("Sprint") ? boostThrustMult : 1f);
        fuel -= fuelPerN * thrust * Time.deltaTime;
		fuel = Mathf.Clamp01(fuel);
		boostTrail.startLifetime = boostTrailBaseRate * throttle * (Input.GetButton("Sprint") ? boostThrustMult : 1f);
		if (fuel > 0f)
			rb.AddRelativeForce(0f, 0f, thrust);
		else
			boostTrail.emissionRate = 0f;


		//Wings and drag
		Vector3 dragDir = transform.InverseTransformDirection(rb.velocity);
		Vector3 dragForce = -Vector3.Scale(dragDir, drag) * rb.velocity.magnitude;
		rb.AddRelativeForce(dragForce);

		//Stabilization
		float sqrVel = Mathf.Pow(Vector3.Dot(rb.velocity, transform.forward), 2);
		Vector3 stabilizationForces = -Vector3.Scale(dragDir, stabilizingDrag) * rb.velocity.magnitude;
		rb.AddRelativeTorque(stabilizationForces.y, stabilizationForces.z, -stabilizationForces.x);

		//Control
		Vector3 torque = new Vector3(
			(Input.GetAxis("Vertical") - trim) * pitchTorque,
			Input.GetAxis("Horizontal") * yawPortion * yawTorque,
			Input.GetAxis("Horizontal") * rollTorque * -1);
		if (Input.GetButton("Sprint"))
			torque *= boostTorqueMult;

		rb.AddRelativeTorque(Mathf.Clamp(sqrVel, minSqrVel, maxSqrVel) * torque);
		currentAileronDeflection.x = Mathf.Clamp(Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"), -1f, 1f);
		currentAileronDeflection.y = Mathf.Clamp(-Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"), -1f, 1f);

		//Move elevons
		if (rightElevon != null && leftElevon != null) {
			rightElevon.localRotation = Quaternion.Slerp(rightElevon.localRotation, Quaternion.Euler(
				Vector3.right * ((currentAileronDeflection.x * maxAileronDeflection) + aileronDeflectionOffset)), .1f);
			leftElevon.localRotation = Quaternion.Slerp(leftElevon.localRotation, Quaternion.Euler(
				Vector3.right * ((currentAileronDeflection.y * maxAileronDeflection) + aileronDeflectionOffset)), .1f);
		}
	}

	public void ShutDown () {
		if (rightElevon != null && leftElevon != null) {
			rightElevon.localEulerAngles = Vector3.right * aileronDeflectionOffset;
			leftElevon.localEulerAngles = Vector3.right * aileronDeflectionOffset;
		}
		trails.SetActive(false);
		this.enabled = false;
		//boostTrail.emissionRate = 0f;

	}

	void OnTriggerStay (Collider other) {
		if (other.tag != "Player")
			return;

		if (airship.velocity.magnitude >= maxLaunchSpeed)
			return;

		if (Input.GetButtonUp("Use")) {
			GameObject.Find("Player Controller").GetComponent<PlayerState>().SetFlyingState(true);
		}
	}

	void OnGUI () {
		GUI.Box(new Rect(Screen.width - 200, Screen.height - 30, 200, 30), "Fuel");
		GUI.Box(new Rect(Screen.width - (200 * fuel), Screen.height - 30, 200 * fuel, 30), "");
	}
}
