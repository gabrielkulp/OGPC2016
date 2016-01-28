using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {
	public enum InteractionMode { Linear, Rotational };
	public enum OutputMode { ZeroToOne, NegativeOneToOne };
	public enum RotationAxis { x, y, z };

	public InteractionMode motion = InteractionMode.Linear;
	public RotationAxis rotationAxis = RotationAxis.z;
	public Vector2 limits = Vector2.up;
	Vector3 startPos = Vector3.zero;
	Quaternion startRot = Quaternion.identity;
//	Quaternion lastRot = Quaternion.identity;
	public float position = 0f;
	public bool snap = false;
	public float snapIncrement = 35f;
	HingeJoint joint;
	JointSpring spring;
	
	public OutputMode outputRange = OutputMode.ZeroToOne;
	public float output = 0f;

	void Start () {
		startPos = transform.localPosition;
		startRot = transform.localRotation;
		//		lastRot = startRot;
		if (motion == InteractionMode.Rotational) {
			joint = GetComponent<HingeJoint>();
			spring = joint.spring;
		}
	}

	void Update () {

		if (motion == InteractionMode.Linear) {
			position = Vector3.Distance(startPos, transform.position);
		} else if (motion == InteractionMode.Rotational) {
		/*			float rotDelta = Quaternion.Angle(lastRot, transform.localRotation);
		//			Debug.Log(rotDelta);
		//			position += rotDelta;
					position = Quaternion.Angle(startRot, transform.localRotation);
					lastRot = transform.rotation; */

			Vector3 vecA = startRot * Vector3.up;
			Vector3 vecB = transform.localRotation * Vector3.up;
			//Debug.Log(vecA + "  " + vecB);
			float angleA;
			float angleB;
		
			switch (rotationAxis) {
				case RotationAxis.x:
					angleA = Mathf.Atan2(vecA.z, vecA.y) * Mathf.Rad2Deg;
					angleB = Mathf.Atan2(vecB.z, vecB.y) * Mathf.Rad2Deg;
					break;
				case RotationAxis.y:
					angleA = Mathf.Atan2(vecA.x, vecA.z) * Mathf.Rad2Deg;
					angleB = Mathf.Atan2(vecB.x, vecB.z) * Mathf.Rad2Deg;
					break;
				default:
				case RotationAxis.z:
					angleA = Mathf.Atan2(vecA.x, vecA.y) * Mathf.Rad2Deg;
					angleB = Mathf.Atan2(vecB.x, vecB.y) * Mathf.Rad2Deg;
					break;
			}

			position = Mathf.DeltaAngle(angleA, angleB);
			if (snap) {
				if (position - joint.spring.targetPosition > snapIncrement / 2f) {
					spring.targetPosition += snapIncrement;
					joint.spring = spring;
				} else if (position - joint.spring.targetPosition < -snapIncrement / 2f) {
					spring.targetPosition -= snapIncrement;
					joint.spring = spring;
				}
			}
		}
		
		if (outputRange == OutputMode.ZeroToOne)
			output = Mathf.InverseLerp(limits.x, limits.y, position);
		else if (outputRange == OutputMode.NegativeOneToOne)
			output = (Mathf.InverseLerp(limits.x, limits.y, position) * 2f) - 1f;
	}
}
