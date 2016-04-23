using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour {

	public Transform cam;
	float length;

	void Start () {
		
		if (PlayerPrefs.GetInt("PlayedLetter", 0) == 1)
			Destroy(gameObject);
		else
			PlayerPrefs.SetInt("PlayedLetter", 1);
		
		length = GetComponent<AudioSource>().clip.length;
        Destroy(gameObject, length + 1f);
	}
	void FixedUpdate () {
		//transform.position = cam.position;
		transform.eulerAngles = Vector3.Scale(cam.eulerAngles, Vector3.up);
		if (Time.timeSinceLevelLoad > length) {
			transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, Vector3.back, 0.05f);
		}
	}
}
