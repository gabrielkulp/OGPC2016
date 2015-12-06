using UnityEngine;
using System.Collections;

public class LandingCamFollow : MonoBehaviour {
	public Camera cam;
	public Rigidbody follow;
	public Vector3 offset;
	public float smoothCam;
	public AnimationCurve fovKick;
	
	void Start () {
		//Setting these at the beginning helps make the lerping look better in the first few frames.
		transform.position = follow.position + (follow.rotation * offset);
		transform.LookAt(follow.position);
	}

	void Update () {
		//Since FOV is graphical, it is manipulated every rendered frame.
		cam.fieldOfView = fovKick.Evaluate(follow.velocity.magnitude);


		if (Input.GetKeyUp(KeyCode.R))
			Application.LoadLevel(Application.loadedLevel);



	}

	void FixedUpdate () {
		//Since pos and rot are physical, they are updated every physics frame.
		//Update position
		Vector3 oldPos = transform.position;
		transform.position = follow.position + (follow.velocity.normalized * offset.z) + (Vector3.up * offset.y);
		transform.position = Vector3.Lerp(oldPos, transform.position, smoothCam);

		//Update rotation
		Quaternion oldRot = transform.rotation;
		transform.LookAt(follow.position, Vector3.up);
		transform.rotation = Quaternion.Lerp(oldRot, transform.rotation, smoothCam);
	}
}
