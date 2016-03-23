using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	public float walkSpeed;
	public float sprintSpeed;
	public float swimSpeedMult;		//Multiply walk speed by this to get swim speed
	public float airSpeedMult;		//Fractional multiplier of moveSpeed for when you're in the air.
	public float jumpSpeed;

	public AudioClip walkSound;
	public AudioClip sprintSound;

	public AnimationCurve viewBobX;
	public AnimationCurve viewBobY;
	public Vector2 viewBobMagnitude = Vector2.one * 0.1f;
	public Vector2 sprintViewBobMagnitude = Vector2.one * 0.1f;
    public float viewBobLoopLength = 0.5f;
	public float sprintViewBobLoopLength = 0.2f;
	public float viewBobTime = 0f;
	public float viewBobResetLerpCoeff = 0.2f;
	Vector3 headOffset = Vector3.up * 1.45f;
	Vector3 viewDelta = Vector3.zero;
	
	[HideInInspector]
	public Vector3 lastShipPos;

	public float gravMultiplier = 2f;
	CharacterController cc;
	Vector3 ccMotion = Vector3.zero;
	public Vector3 respawnPos = Vector3.zero;

	public float camSpeed = 8f;
	public Transform floatCam;	//The moveable head that goes on the ship sometimes
	Transform fixedHead;		//The dummy head on the player
	Camera fixedCam;			//The dummy camera on the player used for raycasts
	float camRot = 0f;

	[HideInInspector]
	public bool onShip = true;
	[HideInInspector]
	public bool flying = false;
	//[HideInInspector]
	public bool swimming = false;

	//Fix things when going to a side level
	void OnLevelWasLoaded (int level) {
		if (level > 1) {
			swimming = false;
			respawnPos = Vector3.zero;
			transform.position = respawnPos;
		}
	}

	//Initialization
	void Start () {
		cc = GetComponent<CharacterController>();
		ccMotion = Vector3.zero;
		camRot = 0f;
		viewBobTime = 0f;
		//headOffset = camRig.transform.localPosition;
		viewDelta = Vector3.zero;
		fixedHead = transform.GetChild(0);
		headOffset = fixedHead.localPosition;
		fixedCam = fixedHead.GetComponentInChildren<Camera>();
		Cursor.visible = false;
	}

	//Enable/disable head, check for respawn
	void Update () {
		fixedHead.gameObject.SetActive(!flying);

		//shipCam.fieldOfView = cam.fieldOfView;
		if (Input.GetKeyUp(KeyCode.R))
			Respawn();
	}

	//Fly, interact, move, view bob, gravity
	void FixedUpdate () {
		if (flying)
			return;

		//Move the character controller
		float moveSpeed = (Input.GetButton("Sprint") ? sprintSpeed : walkSpeed);
        ccMotion = new Vector3(0, ccMotion.y, 0);
		ccMotion += Vector3.ClampMagnitude(
			new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1f) * moveSpeed;
		ccMotion = transform.rotation * ccMotion;

		//Audio
		Vector3 flatMotion = Vector3.Scale(ccMotion, new Vector3(1f, 0f, 1f));
        if (Input.GetButton("Sprint"))
			GetComponent<AudioSource>().clip = sprintSound;
		else
			GetComponent<AudioSource>().clip = walkSound;

		if (flatMotion.magnitude > 0.1f && cc.isGrounded) {
			if (!GetComponent<AudioSource>().isPlaying)
				GetComponent<AudioSource>().Play();
		} else {
			GetComponent<AudioSource>().Stop();
        }

		//View bob
		float bobLength = (Input.GetButton("Sprint") ? sprintViewBobLoopLength : viewBobLoopLength);
		Vector2 bobMag = (Input.GetButton("Sprint") ? sprintViewBobMagnitude : viewBobMagnitude);

		if (cc.isGrounded) {
			if (flatMotion.magnitude > 0f) {
				viewBobTime += (Time.fixedDeltaTime * ccMotion.magnitude) / (bobLength * moveSpeed);
				viewDelta.x = viewBobX.Evaluate(viewBobTime);
				viewDelta.y = viewBobY.Evaluate(viewBobTime);
				viewDelta = Vector3.Scale(viewDelta, bobMag);
				viewDelta = Vector3.Lerp(Vector3.zero, viewDelta, viewBobResetLerpCoeff);
			} else {
				viewDelta = Vector3.Lerp(viewDelta, Vector3.zero, viewBobResetLerpCoeff);
				viewBobTime = 0f;
			}
		}
		fixedHead.localPosition = headOffset + viewDelta;

		//Deal with jumping and gravity
		if (cc.isGrounded) {
			ccMotion.y = Physics.gravity.y * gravMultiplier * Time.fixedDeltaTime;
		} else {
			ccMotion += Physics.gravity * gravMultiplier * Time.fixedDeltaTime;
		}
		if (Input.GetButton("Jump") && (cc.isGrounded || swimming))
			ccMotion.y = jumpSpeed;

		cc.Move(ccMotion * Time.fixedDeltaTime * (swimming ? swimSpeedMult / 2 : 1f));
		if (onShip && !flying)
			lastShipPos = transform.position;

		//Ensures it stays upright
		transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up);
	}

	//Camera movement
	void LateUpdate () {
		//if (floatCam != null)
			floatCam.gameObject.SetActive(!flying);
		//if (fixedCam != null)
			fixedCam.gameObject.SetActive(!flying);

		if (flying)
			return;

		
		//Rotate the controller and/or camera
		float mouseX = Input.GetAxisRaw("Mouse X");
		float mouseY = Input.GetAxisRaw("Mouse Y");
		camRot += -mouseY * camSpeed;
		camRot = Mathf.Clamp(camRot, -90, 90);

		transform.Rotate(transform.up, mouseX * camSpeed);
		fixedHead.localEulerAngles = Vector3.right * camRot;
		

		if (onShip) {
			floatCam.localRotation = fixedHead.rotation;
			floatCam.localPosition = fixedHead.position;
		} else {
			floatCam.rotation = fixedHead.rotation;
			floatCam.position = fixedHead.position;
		}
	}

	void Respawn() {
		GetComponent<AudioSource>().Stop();
		transform.position = respawnPos;
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.layer == 4)	//Layer 4 is water
			swimming = true;
		if (other.tag == "Respawn")
			Respawn();
	}

	void OnTriggerExit (Collider other) {
		if (other.gameObject.layer == 4)    //Layer 4 is water
			swimming = false;
	}
}
