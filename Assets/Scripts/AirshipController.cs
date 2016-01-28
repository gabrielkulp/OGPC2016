using UnityEngine;
using System.Collections;

public class AirshipController : MonoBehaviour {
	public Transform[] engines;
	public float yaw = 0f;					//percentage, -1 to 1
	public float throttle = 0.3f;			//percentage, -1 to 1
	public float climb = 0f;				//percentage, -1 to 1
	public float maxThrottleForce = 20f;	//Newtons
	public float maxClimbForce = 5f;		//Newtons
	public float maxYawTorque = 10f;        //Newton*Meters
	public float maxForceScale = 100000f;
	public Rigidbody RB;

	void Start () {
		
	}
	
	void FixedUpdate () {
		Vector3 force = Vector3.zero;
		force += transform.forward * throttle * maxThrottleForce;
		force += transform.up * climb * maxClimbForce;
		RB.AddForce(force * maxForceScale);
		RB.AddTorque(transform.up * yaw * maxYawTorque * maxForceScale);
	}
}
