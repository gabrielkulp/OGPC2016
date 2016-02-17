using UnityEngine;
using System.Collections;

public class AirshipController : MonoBehaviour {
//	public Transform[] engines;
	public float yaw = 0f;					//percentage, -1 to 1
	public float throttle = 0.3f;			//percentage, -1 to 1
	public float climb = 0f;				//percentage, -1 to 1
	public float maxThrottleForce = 20f;	//Newtons
	public float maxClimbForce = 5f;		//Newtons
	public float maxYawTorque = 10f;        //Newton*Meters
	public float maxForceScale = 100000f;
	public GameObject propellor;
	float propSpeed;                        //percentage, -1 to 1
	float propDecay = 0.99f;				//percentage, 0 to 1
	public GameObject engines;
	float maxEngineVel = 90f;				//deg/s
	Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody>();
		propSpeed = 0f;
	}
	
	void FixedUpdate () {
		//Graphics and effects
		propSpeed += throttle * (1f/propDecay);
		propSpeed *= propDecay;
		propSpeed = Mathf.Clamp(propSpeed, -1f, 1f);
		propellor.transform.Rotate(0f, 0f, propSpeed * -100f * Time.deltaTime);
		engines.transform.localRotation =
			Quaternion.RotateTowards(
				engines.transform.localRotation,
				Quaternion.Euler(climb * -90f, 0f, 0f),
				maxEngineVel * Time.fixedDeltaTime
			);


		//Move the ship
		Vector3 force = Vector3.zero;
		force += transform.forward * maxThrottleForce * propSpeed;
		force += transform.up * climb * maxClimbForce;
		rb.AddForce(force * maxForceScale);
		rb.AddTorque(transform.up * yaw * maxYawTorque * maxForceScale);
	}
}
