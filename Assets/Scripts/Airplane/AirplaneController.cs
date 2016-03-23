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
	public float maxThrust = 500f;
	public float yawPortion = 0.3f;
	public float maxSqrVel = 800f;
	public float minSqrVel = 200f;

	public Vector3 launchForce = Vector3.zero;
	public float VTOLHeight = 10f;
	public AnimationCurve VTOLSpeed;
	public float launchTime = 3f;   //Seconds to get away before checking landing collisions again
	float lastLaunchTime;

	Rigidbody rb;
	
	public Vector3 dockPos = Vector3.zero;

	public GameObject trails;
	public ParticleSystem boostTrail;

	public Rigidbody airship;
	public float maxLaunchSpeed = 5f;
	public GameObject trailCam;
	public Player player;

	bool VTOL = false;
	float VTOLCompletion = 0f;

	bool invertPitch = false;
	ParticleSystem.EmissionModule trailEmission;

	void Start () {
		invertPitch = PlayerPrefs.GetInt("InvertFlight", 0) == 1 ? true : false;
		rb = GetComponent<Rigidbody>();
		trailEmission = boostTrail.emission;
        ShutDown();
    }

	void FixedUpdate () {
		if (VTOL) {
			float moveDelta = VTOLSpeed.Evaluate(VTOLCompletion) * Time.fixedDeltaTime;
            VTOLCompletion += Time.fixedDeltaTime;

			if (VTOLCompletion < 1f)
				transform.localPosition += Vector3.up * moveDelta;
			else {
				VTOL = false;
				VTOLCompletion = 0f;
				rb.isKinematic = false;
				rb.AddRelativeForce(launchForce * 10000f);
				//rb.MoveRotation(Quaternion.FromToRotation(Vector3.forward, launchForce));
				trails.SetActive(true);
				trailEmission.enabled = true;
			}
			return;
		}

		//Old method:
		//rb.velocity = transform.forward * speed;

		//engine
		float thrust = maxThrust * throttle * (Input.GetButton("Jump") ? 0f : 1f);
        fuel -= fuelPerN * thrust * Time.deltaTime;
		fuel = Mathf.Clamp01(fuel);

		if (fuel > 0f && !Input.GetButton("Jump")) {
			rb.AddRelativeForce(0f, 0f, thrust);
			trailEmission.enabled = true;
		} else
			trailEmission.enabled = false;

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
			(Input.GetAxis("Vertical") - trim) * pitchTorque * (invertPitch ? -1 : 1),
			Input.GetAxis("Horizontal") * yawPortion * yawTorque,
			Input.GetAxis("Horizontal") * rollTorque * -1);

		rb.AddRelativeTorque(Mathf.Clamp(sqrVel, minSqrVel, maxSqrVel) * torque);
	}

	public void StartUp () {
		player.GetComponent<AudioSource>().Stop();
		lastLaunchTime = Time.time;
		player.flying = true;
		trailCam.SetActive(true);
		FixedJoint joint = gameObject.GetComponent<FixedJoint>();
		if (player.onShip) {
			trails.SetActive(true);
			trailEmission.enabled = true;
			Destroy(joint);
			rb.AddRelativeForce(launchForce * 10000f);
			//rb.MoveRotation(Quaternion.FromToRotation(Vector3.forward, launchForce));
		} else {
			rb.isKinematic = true;
			VTOL = true;

		}
	}

	public void ShutDown () {
		player.flying = false;
		trailCam.SetActive(false);
		trails.SetActive(false);
		trailEmission.enabled = false;
		if (player.onShip) {
			transform.position = airship.transform.TransformPoint(dockPos);
			transform.rotation = airship.transform.rotation;
			FixedJoint joint = gameObject.AddComponent<FixedJoint>();
			joint.connectedBody = airship;
			joint.enableCollision = false;
			fuel = 1f;
		} else {
			transform.rotation = Quaternion.Euler(0f, transform.localEulerAngles.y, 0f);
			player.transform.rotation = transform.rotation;
			rb.isKinematic = true;
		}
		this.enabled = false;
	}

	void OnTriggerStay (Collider other) {
		if (Time.time < lastLaunchTime + launchTime)
			return;

		if (player.flying) {
			if (other.tag == "Airship") {
				player.onShip = true;
				player.transform.position = player.lastShipPos;
			} else {
				//Places the player and glider on the ground
				RaycastHit planeGroundTest;
				if (Physics.Raycast(transform.position, Vector3.down, out planeGroundTest, 100f)) {
					//if (Vector3.Angle(planeGroundTest.normal, Vector3.zero) > 20f)
					//	return;
					Vector3 testPos = transform.position;
					testPos += Quaternion.Euler(0f, transform.localEulerAngles.y, 0f) * Vector3.back * 3f;
					RaycastHit playerGroundTest;
					if (Physics.Raycast(testPos, Vector3.down, out playerGroundTest, 3f)) {
						player.transform.position = playerGroundTest.point + (Vector3.up * 0.1f);
						transform.position = planeGroundTest.point + (Vector3.up * 0.5f);
					} else
						return;
				} else
					return;
				player.flying = false;
				player.onShip = false;
			}
			ShutDown();
			player.respawnPos = player.transform.position;
		}
	}

	void OnGUI () {
		GUI.Box(new Rect(Screen.width - 200, Screen.height - 30, 200, 30), "Fuel");
		GUI.Box(new Rect(Screen.width - (200 * fuel), Screen.height - 30, 200 * fuel, 30), "");
	}
}
