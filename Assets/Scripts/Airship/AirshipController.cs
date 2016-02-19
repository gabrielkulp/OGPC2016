using UnityEngine;
using System.Collections;

public class AirshipController : MonoBehaviour {
	//	public Transform[] engines;
	public Vector3 input;					//Yaw,Climb,Throttle % -1 to 1
	public float maxThrottleForce = 20f;	//Newtons
	public float maxClimbForce = 5f;		//Newtons
	public float maxYawTorque = 10f;        //Newton*Meters
	public float maxForceScale = 100000f;
	public GameObject propellor;
	float propSpeed;                        //percentage, -1 to 1
	public GameObject engines;
	float maxEngineVel = 90f;				//deg/s
	Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody>();
		propSpeed = 0f;
	}
	
	void FixedUpdate () {
		//Graphics and effects
		propSpeed = input.z;
		propSpeed = Mathf.Clamp(propSpeed, -1f, 1f);
		propellor.transform.Rotate(0f, 0f, propSpeed * -200f * Time.deltaTime);
		engines.transform.localRotation =
			Quaternion.RotateTowards(
				engines.transform.localRotation,
				Quaternion.Euler(input.y * -90f, 0f, 0f),
				maxEngineVel * Time.fixedDeltaTime
			);


		//Move the ship
		Vector3 force = Vector3.zero;
		force += transform.forward * maxThrottleForce * propSpeed;
		force += transform.up * input.y * maxClimbForce;
		rb.AddForce(force * maxForceScale);
		rb.AddTorque(transform.up * input.x * maxYawTorque * maxForceScale);
	}
}
