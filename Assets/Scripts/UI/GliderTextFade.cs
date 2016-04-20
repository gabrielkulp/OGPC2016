using UnityEngine;
using System.Collections;

public class GliderTextFade : MonoBehaviour {
	TextMesh text;
	public string messageNormal = "Press E to fly";
	public string messageTooFast = "Too fast to launch!";
	public float dispDist = 5f;
	public Color nearColor = Color.white;
	public Color farColor = Color.clear;
	public float colorLerp = 0.1f;
	public bool near = false;
	public Transform cam;
	public Rigidbody airship;
	public AirplaneController airplane;
	public Player player;

	void Start () {
		text = GetComponent<TextMesh>();
		text.color = farColor;
		
	}

	//Changes color according to where the player is
	void Update () {
		near = (Vector3.Distance(transform.position, cam.position) <= dispDist);

		//Changes message based on launch availability
		if ((airship.velocity.magnitude <= airplane.maxLaunchSpeed) || !player.onShip)
			text.text = messageNormal;
		else
			text.text = messageTooFast;

		//Makes color transition pretty
		text.color = Color.Lerp(text.color, near ? nearColor : farColor, colorLerp);

		//If you can see the back, flip
		if (Vector3.Dot(transform.forward,
		  (transform.position - cam.position).normalized) < 0f) {
			transform.Rotate(Vector3.up, 180f, Space.Self);
		}

		//Hides text when you're flying
		if (player.flying) {
			text.color = farColor;
			near = false;
		}

		if (near && Input.GetButtonUp("Use")) {
			if (airship.velocity.magnitude <= airplane.maxLaunchSpeed) {
				airplane.enabled = true;
				airplane.StartUp(); //Deals with player, too
			}
		}
	}
}
