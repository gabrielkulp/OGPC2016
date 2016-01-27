using UnityEngine;
using System.Collections;

public class AirshipTextFade : MonoBehaviour {
	public TextMesh text;
	GameObject textGO;
	public Color nearColor = Color.white;
	public Color farColor = Color.clear;
	public float colorLerp = 0.1f;
	public bool near = false;
	PlayerState playerState;
	public Camera cam;
	public AirshipController airship;
	public AirplaneController airplane;

	void Start () {
		textGO = text.gameObject;
		text.color = farColor;
		playerState = GameObject.Find("Player Controller").GetComponent<PlayerState>();
	}

	//Changes color according to where the player is
	void Update () {

		text.color = Color.Lerp(text.color, near ? nearColor : farColor, colorLerp);

		//If you can see the back, flip
		if (Vector3.Dot(textGO.transform.forward,
		  (textGO.transform.position - cam.transform.position).normalized) < 0f) {
			textGO.transform.Rotate(Vector3.up, 180f, Space.Self);
		}

		//Hides text when you're flying
		if (playerState.flying) {
			text.color = farColor;
			near = false;
		}
	}

	//Updates value of near depending on if the player enters or leaves the collider
	void OnTriggerEnter (Collider other) {
		if (other.tag != "Player") return;

		near = true;
	}

	void OnTriggerExit (Collider other) {
		if (other.tag != "Player") return;

		near = false;
	}
}
