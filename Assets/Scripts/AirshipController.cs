using UnityEngine;
using System.Collections;

public class AirshipController : MonoBehaviour {
	public Transform[] engines;
	public float yaw = 0f;			//percentage, -1 to 1
	public float throttle = 0.3f;   //percentage, -1 to 1
	public float climb = 0f;		//percentage, -1 to 1
	public float maxSpeed = 20f;	//meters/second
	public float maxClimb = 5f;		//meters/second
	public float maxYaw = 10f;      //degrees/second
	public Rigidbody RB;
	
	[HideInInspector]
	public Vector3 velocity = Vector3.zero;
	[HideInInspector]
	public Vector3 angularVelocity = Vector3.zero;
	

	void Start () {
		
	}
	
	void FixedUpdate () {
		velocity = transform.forward * throttle * maxSpeed;
		velocity += transform.up * climb * maxClimb;
		angularVelocity = transform.up * yaw * maxYaw;
		transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
		transform.Rotate(angularVelocity * Time.fixedDeltaTime);
		//RB.velocity = velocity;
		//RB.angularVelocity = angularVelocity;

	}
}
