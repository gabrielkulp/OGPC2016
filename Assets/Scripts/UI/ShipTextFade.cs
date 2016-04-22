using UnityEngine;
using System.Collections;

public class ShipTextFade : MonoBehaviour {
	public TextMesh text;
	public Color nearColor = Color.white;
	public Color farColor = Color.clear;
	public float colorLerp = 0.1f;
	bool near = false;
	string content;

	void Start () {
		text.color = farColor;
		content = text.text;
		text.text = "";
	}

	void Update () {
		text.color = Color.Lerp(text.color, near ? nearColor : farColor, colorLerp);
		if (text.color.a - .5f <= 0)
			text.text = "";
		else
			text.text = content;
    }

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player")
			near = true;
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "Player")
			near = false;
	}
}
