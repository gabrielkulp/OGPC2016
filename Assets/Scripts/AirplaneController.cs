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

	public Vector3 launchForce = Vector3.zero;
	public float launchTime = 3f;   //Seconds to get away before checking landing collisions again
	float lastLaunchTime;

	Rigidbody rb;
	Vector3 dockPos = Vector3.zero;


	public GameObject trails;
	public ParticleSystem boostTrail;
	public float boostTrailEmitMult;
	float boostTrailBaseRate;

	public Rigidbody airship;
	public float maxLaunchSpeed = 5f;
	public GameObject trailCam;
	public Player player;

	void Start () {
		rb = GetComponent<Rigidbody>();
		boostTrailBaseRate = boostTrail.startLifetime;
		dockPos = airship.transform.InverseTransformPoint(transform.position);
		ShutDown();
    }

	void FixedUpdate () {
		//Old method:
		//rb.velocity = transform.forward * speed;

		//engine
		float thrust = maxThrust * throttle * (Input.GetButton("Sprint") ? boostThrustMult : 1f);
        fuel -= fuelPerN * thrust * Time.deltaTime;
		fuel = Mathf.Clamp01(fuel);
		boostTrail.startLifetime = boostTrailBaseRate * throttle * (Input.GetButton("Sprint") ? boostTrailEmitMult : 1f);

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
	}

	public void ShutDown () {
		player.flying = false;
		trailCam.SetActive(false);
		trails.SetActive(false);
		boostTrail.enableEmission = false;
		if (player.onShip) {
			transform.position = airship.transform.TransformPoint(dockPos);
			transform.rotation = airship.transform.rotation;
			FixedJoint joint = gameObject.AddComponent<FixedJoint>();
			joint.connectedBody = airship;
			joint.enableCollision = false;
		} else {
			transform.rotation = Quaternion.Euler(0f, transform.localEulerAngles.y, 0f);
			rb.isKinematic = true;

			//Places the player and glider on the ground
			RaycastHit groundTest;
			if (Physics.Raycast(transform.position, Vector3.down, out groundTest, 100f)) {
				player.transform.position = groundTest.point + (Vector3.up * 0.1f);
				transform.position = groundTest.point + (Vector3.up * 0.5f);
			} else {
				//Landing failed.  Placeholder code
				player.transform.position = transform.position;
			}
		}
		this.enabled = false;
	}

	public void StartUp () {
		lastLaunchTime = Time.time;
		rb.isKinematic = false;	//Only matters when leaving the ground
		player.flying = true;
		trails.SetActive(true);
		boostTrail.enableEmission = true;
		trailCam.SetActive(true);
		FixedJoint joint = gameObject.GetComponent<FixedJoint>();
		if (joint != null)
			Destroy(joint);
		else
			Debug.Log("FixedJoint doesn't exist!?");
		rb.AddRelativeForce(launchForce * 10000f);
	}

	void OnTriggerStay (Collider other) {
		if (Time.time < lastLaunchTime + launchTime)
			return;

		if (player.flying) {
			if (other.tag == "Airship") {
				player.onShip = true;
				player.transform.position = player.lastShipPos;
				ShutDown();
			} else {
				player.flying = false;
				player.onShip = false;
				ShutDown();
			}
		}
	}

	void OnGUI () {
		GUI.Box(new Rect(Screen.width - 200, Screen.height - 30, 200, 30), "Fuel");
		GUI.Box(new Rect(Screen.width - (200 * fuel), Screen.height - 30, 200 * fuel, 30), "");
	}
}
