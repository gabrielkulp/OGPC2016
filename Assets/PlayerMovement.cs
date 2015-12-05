using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float moveSpeed;
	public float airSpeedMult;	//Fractional multiplier of moveSpeed for when you're in the air.
	public float jumpSpeed;
	CharacterController cc;
	Vector3 ccMotion = Vector3.zero;

	public float pushPower = 2f;

	public float camSpeed = 8f;
	public GameObject cam;
	float camRot = 0f;

	void Start () {
		cc = GetComponent<CharacterController>();
		ccMotion = Vector3.zero;
		camRot = 0f;
	}

	void Update () {
		if (cc.isGrounded) {
			ccMotion = new Vector3(Input.GetAxis("Horizontal"), ccMotion.y / moveSpeed, Input.GetAxis("Vertical")) * moveSpeed;
			ccMotion = transform.rotation * ccMotion;
			if (Input.GetButton("Jump") && cc.isGrounded) {
				ccMotion.y = jumpSpeed;
				Debug.Log("Jump at t=" + Time.time);
			}
		} else {
			float airSpeed = moveSpeed * airSpeedMult;
			ccMotion += transform.rotation *
				new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * airSpeed;
		}
		ccMotion += Physics.gravity * Time.deltaTime;
		cc.Move(ccMotion * Time.deltaTime);

		camRot += -Input.GetAxis("Mouse Y") * camSpeed;
		camRot = Mathf.Clamp(camRot, -90, 90);
		cam.transform.localRotation = Quaternion.Euler(camRot, 0f, 0f);
		transform.Rotate(transform.up, Input.GetAxis("Mouse X") * camSpeed);
	}

	
	void OnControllerColliderHit (ControllerColliderHit hit) {
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic)
			return;

		if (hit.moveDirection.y < -0.3F)
			return;

		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir * pushPower;
	}
}
