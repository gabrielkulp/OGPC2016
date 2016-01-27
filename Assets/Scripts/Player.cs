using UnityEngine;
//using UnityEditor;
using System.Collections;

public class Player : MonoBehaviour {
	public float walkSpeed;
	public float sprintSpeed;
	public float airSpeedMult;		//Fractional multiplier of moveSpeed for when you're in the air.
	public float jumpSpeed;
	bool onShip = false;
	AirshipController airship;

	public AnimationCurve viewBobX;
	public AnimationCurve viewBobY;
	public Vector2 viewBobMagnitude = Vector2.one * 0.1f;
	public Vector2 sprintViewBobMagnitude = Vector2.one * 0.1f;
    public float viewBobLoopLength = 0.5f;
	public float sprintViewBobLoopLength = 0.2f;
	public float viewBobTime = 0f;
	public float viewBobResetLerpCoeff = 0.2f;
	Vector3 viewBobOrigin;
	Vector3 viewDelta = Vector3.zero;

	public float gravMultiplier = 2f;
	public float inheritVelocityHeight = 10f;	//How high you have to be to not inherit velocity
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
	public GameObject camRig;
	Camera cam;
	float camRot = 0f;


	void Start () {
		cc = GetComponent<CharacterController>();
		ccMotion = Vector3.zero;
		camRot = 0f;
		viewBobTime = 0f;
		viewBobOrigin = camRig.transform.localPosition;
		viewDelta = Vector3.zero;
		cam = camRig.GetComponentInChildren<Camera>();
		Cursor.visible = false;
	}

	void FixedUpdate () {
		//Click and drag objects on screen
		interactionMode = Input.GetKey(KeyCode.LeftAlt);

		//Start of interaction cycle
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);	//Don't move this.  Magic at play.

		if (interactionMode && Input.GetMouseButton(0) && !lastMouseState) {
			if (Input.GetKey(KeyCode.LeftControl))
//				EditorApplication.isPaused = true;

			interacting = Physics.Raycast(ray, out interactionHit, reach);
			
			if (interacting) {
				Debug.Log(interactionHit.transform.name);
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
				mousePos = touchPosWorld + (cam.transform.rotation * mouseDelta);

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
				camRig.transform.position) >= reach)) {

			interacting = false;
		}
		if (Input.GetKeyUp(KeyCode.LeftAlt)) {
			interacting = false;
			interactionMode = false;
		}

		//Cursor.visible = interactionMode && !interacting;


		//Move the character controller
		float moveSpeed = (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed);
        ccMotion = new Vector3(0, ccMotion.y, 0);
		ccMotion += Vector3.ClampMagnitude(
			new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1f) * moveSpeed;
		ccMotion = transform.rotation * ccMotion;

		//View bob
		float bobLength = (Input.GetKey(KeyCode.LeftShift) ? sprintViewBobLoopLength : viewBobLoopLength);
		Vector2 bobMag = (Input.GetKey(KeyCode.LeftShift) ? sprintViewBobMagnitude : viewBobMagnitude);

		if (cc.isGrounded) {
			Vector3 flatMotion = Vector3.Scale(ccMotion, new Vector3(1f, 0f, 1f) / moveSpeed);
			if (flatMotion.magnitude > 0f) {
				viewBobTime += (Time.fixedDeltaTime * flatMotion.magnitude) / bobLength;
				viewDelta.x = viewBobX.Evaluate(viewBobTime);
				viewDelta.y = viewBobY.Evaluate(viewBobTime);
				viewDelta = Vector3.Scale(viewDelta, bobMag);
				viewDelta = Vector3.Lerp(camRig.transform.localPosition - viewBobOrigin, viewDelta, viewBobResetLerpCoeff);
			} else {
				viewDelta = Vector3.Lerp(viewDelta, Vector3.zero, viewBobResetLerpCoeff);
				viewBobTime = 0f;
			}

			camRig.transform.localPosition = viewBobOrigin + viewDelta;
		}

		//Check for things under you to inherit velocity from
		onShip = false;
		RaycastHit groundTestHit;
		if (Physics.Raycast(transform.position, Vector3.down, out groundTestHit, inheritVelocityHeight)) {
			if (groundTestHit.rigidbody) {
				ccMotion += groundTestHit.rigidbody.velocity;
				transform.RotateAround(
					groundTestHit.rigidbody.worldCenterOfMass,
					groundTestHit.transform.up,
					groundTestHit.rigidbody.angularVelocity.y * 57f * Time.fixedDeltaTime);
				//TODO: Fix magic number
				Debug.Log("Rigidbody " + Time.time);
			} else {

				//Get top level gameObject
				GameObject otherGO = groundTestHit.collider.gameObject;
				while (otherGO.transform.parent != null) {
					otherGO = otherGO.transform.parent.gameObject;
				}
				
				if (otherGO.GetComponent<AirshipController>() != null) {
					onShip = true;
					airship = otherGO.GetComponent<AirshipController>();
					ccMotion += Vector3.Scale(airship.velocity, new Vector3(1, 0, 1));
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
			if (!onShip && Input.GetButton("Jump"))
				ccMotion.y = jumpSpeed;
			
			if (onShip && Input.GetButton("Jump"))
				ccMotion.y = jumpSpeed + airship.velocity.y * Time.fixedDeltaTime;
		} else {
			ccMotion += Physics.gravity * gravMultiplier * Time.fixedDeltaTime;
		}

		cc.Move(ccMotion * Time.fixedDeltaTime);

		//Ensures it stays upright
		transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up);

		lastMouseState = Input.GetMouseButton(0);

	}

	void LateUpdate () {
		if (!interactionMode) {
			//Rotate the controller and/or camera
			float mouseX = Input.GetAxisRaw("Mouse X");
			float mouseY = Input.GetAxisRaw("Mouse Y");
			/*
			dmouseX *= Mathf.Abs(mouseX);	//Squares while keeping sign
			mouseY *= Mathf.Abs(mouseY);    //in order to help accuracy

			mouseX = Mathf.Clamp(mouseX, -1f, 1f);
			mouseY = Mathf.Clamp(mouseY, -1f, 1f);
			*/
			camRot += -mouseY * camSpeed;
			camRot = Mathf.Clamp(camRot, -90, 90);

			camRig.transform.localRotation = Quaternion.Euler(camRot, 0f, 0f);
			transform.Rotate(transform.up, mouseX * camSpeed);
		}

	}

	void OnGUI () {
		if (interactionMode && !interacting) {
			GUI.Box(new Rect(Input.mousePosition.x - 5f, (-(Input.mousePosition.y + 5f) + Screen.height), 10f, 10f), "");
		}

		if (interacting && interactionHit.point != null) {
			//Draw contact point
			Vector3 tPos = cam.WorldToScreenPoint(interactionHit.transform.TransformPoint(touchPos));
			GUI.Box(new Rect(tPos.x - 5f, (-(tPos.y + 5f) + Screen.height), 10f, 10f), "");
			
			//Draw mouse point
			Vector3 mPos = cam.WorldToScreenPoint(mousePos);
			GUI.Box(new Rect(mPos.x - 5f, (-(mPos.y + 5f) + Screen.height), 	10f, 10f), "");
		}
	}
}
