using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	public Transform lever;
	public Vector2 xRotLimit; 
	public Transform target;
	public Vector3 targetPos;
	public Vector3 targetRot;
	public bool activated = false;
    public float lerpCoeff = 0.1f;
	public GameObject text;
	public AudioClip grindSound;
	TextMesh textMesh;

	void Start () {
		lever.localEulerAngles = Vector3.forward * xRotLimit.x;
		textMesh = text.GetComponent<TextMesh>();
	}

	void Update () {
		if (activated) {
			lever.localRotation = Quaternion.Slerp(lever.localRotation, Quaternion.Euler(Vector3.forward * xRotLimit.y), lerpCoeff * 2f);
			target.localPosition = Vector3.Lerp(target.localPosition, targetPos, lerpCoeff);
			target.localRotation = Quaternion.Slerp(target.localRotation, Quaternion.Euler(targetRot), lerpCoeff);
			if (textMesh.color.a <= 0.3f)
				text.SetActive(false);
			else
				textMesh.color = Color.Lerp(textMesh.color, Color.clear, 0.01f);
		}
	}

	void OnTriggerStay (Collider other) {
		if (other.tag == "Player" && Input.GetButton("Use")) {
			target.gameObject.SetActive(true);
			activated = true;
			GetComponent<AudioSource>().Play();
			GetComponent<AudioSource>().PlayOneShot(grindSound);
			if (text != null) {
				text.SetActive(true);
			}
		}
	}
}
