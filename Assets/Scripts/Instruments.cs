using UnityEngine;
using System.Collections;

public class Instruments : MonoBehaviour {

	public Rigidbody follow;
	public Transform compassDial;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		compassDial.rotation = Quaternion.FromToRotation(follow.transform.forward, Vector3.up);

	}
}
