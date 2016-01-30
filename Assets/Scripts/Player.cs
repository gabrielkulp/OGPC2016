using UnityEngine;
//using UnityEditor;
using System.Collections;

public class Player : MonoBehaviour {
	public float walkSpeed;
	public float sprintSpeed;
	public float airSpeedMult;		//Fractional multiplier of moveSpeed for when you're in the air.
	public float jumpSpeed;

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

	public float reach = 2f;
	public float springConstant = 5f;
	public float maxPull = 15f;
	bool interactionMode = false;				//Are you pressing alt to interact?
	bool interacting = false;					//Are you currently dragging something?
	RaycastHit interactionHit = new RaycastHit();
	Vector3 touchPos = Vector3.zero;			//Point of contact in local coordinates
	Vector3 mousePos = Vector3.zero;			//Mouse position relative to the world
	Vector3 mouseDelta = Vector3.zero;			//The mouse's position relative to the point of contact
	bool lastMouseState = false;

	public float camSpeed = 8f;
	Transform floatHead;
	public Camera floatCam;
	Camera fixedCam;
	float camRot = 0f;

	[HideInInspector]
	public bool onShip = true;
	[HideInInspector]
	public bool flying = false;

	void Start () {
		cc = GetComponent<CharacterController>();
		ccMotion = Vector3.zero;
		camRot = 0f;
		viewBobTime = 0f;
		//headOffset = camRig.transform.localPosition;
		viewDelta = Vector3.zero;
		floatHead = transform.GetChild(0);
		headOffset = floatHead.localPosition;
		fixedCam = floatHead.GetComponentInChildren<Camera>();
		Cursor.visible = false;
	}

	void FixedUpdate () {
		if (flying)
			return;

		//Click and drag objects on screen
		interactionMode = Input.GetButton("Interact");

		//Start of interaction cycle
		Ray ray = fixedCam.ScreenPointToRay(Input.mousePosition);    //Don't move this.  Magic at play.

		if (interactionMode && Input.GetMouseButton(0) && !lastMouseState) {
			//if (Input.GetKey(KeyCode.LeftControl))
				//EditorApplication.isPaused = true;

			interacting = Physics.Raycast(ray, out interactionHit, reach);
			
			if (interacting) {
				touchPos = interactionHit.transform.InverseTransformPoint(interactionHit.point);
				mousePos = touchPos;
				mouseDelta = Vector3.zero;
			}
		}
		//Update of interaction cycle
		if (interacting) {
			if (interactionHit.rigidbody != null) {
				mouseDelta += new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0f) * 0.1f;
				mouseDelta = Vector3.ClampMagnitude(mouseDelta, maxPull / springConstant);
				Vector3 touchPosWorld = interactionHit.transform.TransformPoint(touchPos);
				mousePos = touchPosWorld + (fixedCam.transform.rotation * mouseDelta);

				Vector3 interactionForce = (mousePos - touchPosWorld) * springConstant;

				interactionHit.rigidbody.AddForceAtPosition(
					interactionForce,
					interactionHit.transform.transform.TransformPoint(touchPos),
					ForceMode.Force);
			}
		}
		//End interaction cycle
		if ((interacting && !Input.GetMouseButton(0)) || 
			(interactionHit.transform != null && Vector3.Distance(
				interactionHit.transform.transform.TransformPoint(touchPos),
				floatHead.position) >= reach)) {

			interacting = false;
		}
		if (!Input.GetButton("Interact")) {
			interacting = false;
			interactionMode = false;
		}

		//Move the character controller
		float moveSpeed = (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed);
        ccMotion = new Vector3(0, ccMotion.y, 0);
		ccMotion += Vector3.ClampMagnitude(
			new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1f) * moveSpeed;
		ccMotion = transform.rotation * ccMotion;

		//View bob
		float bobLength = (Input.GetButton("Sprint") ? sprintViewBobLoopLength : viewBobLoopLength);
		Vector2 bobMag = (Input.GetButton("Sprint") ? sprintViewBobMagnitude : viewBobMagnitude);

		if (cc.isGrounded) {
			if (Vector3.Scale(ccMotion, new Vector3(1f, 0f, 1f)).magnitude > 0f) {
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
		floatHead.localPosition = headOffset + viewDelta;

		//Deal with jumping and gravity
		if (cc.isGrounded) {
			ccMotion.y = 0f;
			if (Input.GetButton("Jump"))
				ccMotion.y = jumpSpeed;
		} else {
			ccMotion += Physics.gravity * gravMultiplier * Time.fixedDeltaTime;
		}

		cc.Move(ccMotion * Time.fixedDeltaTime);
		if (onShip && !flying)
			lastShipPos = transform.position;

		//Ensures it stays upright
		transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up);

		lastMouseState = Input.GetMouseButton(0);
	}

	void LateUpdate () {
		floatCam.gameObject.SetActive(!flying);
		fixedCam.gameObject.SetActive(!flying);
		if (flying)
			return;

		if (!interactionMode) {
			//Rotate the controller and/or camera
			float mouseX = Input.GetAxisRaw("Mouse X");
			float mouseY = Input.GetAxisRaw("Mouse Y");
			camRot += -mouseY * camSpeed;
			camRot = Mathf.Clamp(camRot, -90, 90);

			transform.Rotate(transform.up, mouseX * camSpeed);
			floatHead.localEulerAngles = Vector3.right * camRot;
		}

		if (onShip) {
			floatCam.transform.localRotation = floatHead.rotation;
			floatCam.transform.localPosition = floatHead.position;
		} else {
			floatCam.transform.rotation = floatHead.rotation;
			floatCam.transform.position = floatHead.position;
		}
	}

	void Update () {
		floatHead.gameObject.SetActive(!flying);

		//shipCam.fieldOfView = cam.fieldOfView;
		if (Input.GetKeyUp(KeyCode.R))
			Application.LoadLevel(Application.loadedLevel);
	}

	void OnGUI () {
		if (flying)
			return;

		if (interactionMode && !interacting) {
			GUI.Box(new Rect(Input.mousePosition.x - 5f, (-(Input.mousePosition.y + 5f) + Screen.height), 10f, 10f), "");
		}

		if (interacting && interactionHit.point != null) {
			//Draw contact point
			Vector3 tPos = fixedCam.WorldToScreenPoint(interactionHit.transform.TransformPoint(touchPos));
			GUI.Box(new Rect(tPos.x - 5f, (-(tPos.y + 5f) + Screen.height), 10f, 10f), "");
			
			//Draw mouse point
			Vector3 mPos = fixedCam.WorldToScreenPoint(mousePos);
			GUI.Box(new Rect(mPos.x - 5f, (-(mPos.y + 5f) + Screen.height), 	10f, 10f), "");
		}
	}
}
