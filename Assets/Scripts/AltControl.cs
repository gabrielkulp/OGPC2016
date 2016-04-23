using UnityEngine;
using System.Collections;

public class AltControl : MonoBehaviour {

	public float distance = 300f;
	public Transform follow;
	public Color startColor;
	public Color endColor;
	TextMesh text;

	void Start () {
		text = GetComponent<TextMesh>();
	}
	
	void Update () {
		if (follow.position.magnitude > distance)
			text.color = Color.Lerp(text.color, endColor, 0.01f);
	}
}
