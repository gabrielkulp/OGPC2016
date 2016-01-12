using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float moveSpeed;
	public float airSpeedMult;	//Fractional multiplier of moveSpeed for when you're in the air.
	public float jumpSpeed;
	public AnimationCurve viewBobX;
	public AnimationCurve viewBobY;
	public Vector2 viewBobMagnitude = Vector2.one * 0.1f;
	public float viewBobLoopLength = 0.5f;
	public float viewBobTime = 0f;
	public float viewBobResetLerpCoeff = 0.2f;
	Vector3 viewBobOrigin;
	Vector3 viewDelta = Vector3.zero;

	public float gravMultiplier = 2f;
	public float inheritVelocityHeight = 10f;	//How high you have to be to not inherit velocity
	CharacterController cc;
	Vector3 ccMotion = Vector3.zero;

	public bool canPush = true;
	public float pushPower = 2f;

	public float camSpeed = 8f;
	public GameObject cam;
	float camRot = 0f;

	void Start () {
		cc = GetComponent<CharacterController>();
		ccMotion = Vector3.zero;
		camRot = 0f;
		viewBobTime = 0f;
		viewBobOrigin = cam.transform.localPosition;
		viewDelta = Vector3.zero;
	}

	void FixedUpdate () {
		//Move the character controller
		ccMotion = new Vector3(0, ccMotion.y,0);
		ccMotion += Vector3.ClampMagnitude(
			new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1f) * moveSpeed;
        ccMotion = transform.rotation * ccMotion;

		if (cc.isGrounded) {
			Vector3 flatMotion = Vector3.Scale(ccMotion, new Vector3(1f, 0f, 1f) / moveSpeed);
			Debug.Log(flatMotion.magnitude);
            if (flatMotion.magnitude > 0f) {
				viewBobTime += (Time.fixedDeltaTime * flatMotion.magnitude) / viewBobLoopLength;
				viewDelta.x = viewBobX.Evaluate(viewBobTime);
				viewDelta.y = viewBobY.Evaluate(viewBobTime);
				viewDelta = Vector3.Scale(viewDelta, viewBobMagnitude);
			} else {
				viewDelta = Vector3.Lerp(viewDelta, Vector3.zero, viewBobResetLerpCoeff);
				viewBobTime = 0f;
			}
			
			cam.transform.localPosition = viewBobOrigin + viewDelta;
		}

		//Check for things under you to inherit velocity from
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, inheritVelocityHeight)) {
			if (hit.rigidbody) {
				ccMotion += hit.rigidbody.velocity;
				transform.RotateAround(
					hit.rigidbody.worldCenterOfMass,
					hit.transform.up,
					hit.rigidbody.angularVelocity.y * 57f * Time.fixedDeltaTime);
				//TODO: Fix magic number
				Debug.Log("Rigidbody " + Time.time);
			} else {

				//Get top level gameObject
				GameObject otherGO = hit.collider.gameObject;
				while (otherGO.transform.parent != null) {
					otherGO = otherGO.transform.parent.gameObject;
				}
				if (otherGO.GetComponent<AirshipController>() != null) {
					AirshipController airship = otherGO.GetComponent<AirshipController>();
					if (cc.isGrounded)
						ccMotion.y = airship.velocity.y * Time.fixedDeltaTime;
                    ccMotion += Vector3.Scale(airship.velocity, new Vector3(1,0,1));
					transform.RotateAround(otherGO.transform.position, otherGO.transform.up,
						airship.angularVelocity.y * Time.fixedDeltaTime);
					//Debug.Log("Airship " + Time.time);

				} else {
					//Debug.Log("No RB or Airship " + Time.time);
				}
            }
		} else {
			//Debug.Log("No hit " + Time.time);
		}

		//Deal with jumping and gravity
		if (cc.isGrounded) {
			ccMotion.y = 0f;

			if (Input.GetButton("Jump"))
				ccMotion.y = jumpSpeed;
		} else {
			ccMotion += Physics.gravity * gravMultiplier * Time.fixedDeltaTime;
		}

		cc.Move(ccMotion * Time.fixedDeltaTime);

		//Ensures it stays upright
		transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up);

		//Rotate the controller and/or camera
		camRot += -Input.GetAxis("Mouse Y") * camSpeed;
		camRot = Mathf.Clamp(camRot, -90, 90);

		cam.transform.localRotation = Quaternion.Euler(camRot, 0f, 0f);
		transform.Rotate(transform.up, Input.GetAxis("Mouse X") * camSpeed);
	}

	
	//This is so you can push rigidbodies.
	void OnControllerColliderHit (ControllerColliderHit hit) {
		if (!canPush)
			return;

		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic)
			return;

		if (hit.moveDirection.y < -0.3F)
			return;

		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.AddForce(pushDir * pushPower);
	}
}
