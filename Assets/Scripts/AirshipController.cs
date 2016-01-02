using UnityEngine;
using System.Collections;

public class AirshipController : MonoBehaviour {
	public Transform[] engines;
	public float throttle = 0.3f;			//percentage, 0 to 1
	public Vector2 steer = Vector3.zero;	//percentages, -1 to 1
	public float maxSpeed = 20f;			//meters/second
	public float maxSteer = 15f;            //degrees/second
	public float maxPitchSpeed = 10f;		//degrees/second

	public float pitch = 0f;
	
	[HideInInspector]
	public Vector3 velocity = Vector3.zero;
	[HideInInspector]
	public Vector3 angularVelocity = Vector3.zero;
	

	void Start () {
		
	}
	
	void FixedUpdate () {
		pitch += steer.x * maxPitchSpeed * Time.fixedDeltaTime;
		pitch = Mathf.Clamp(pitch, -90f, 90f);


		velocity = Quaternion.Euler(-pitch, 0f, 0f) * transform.forward * throttle * maxSpeed;
		angularVelocity = transform.up * steer.y * maxSteer;
		transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
		transform.Rotate(angularVelocity * Time.fixedDeltaTime);
	}
}
