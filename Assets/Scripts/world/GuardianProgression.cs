﻿using UnityEngine;
using System.Collections;

public class GuardianProgression : MonoBehaviour {

	public int id;              //The way to differentiate islands
	public int progress = 0;    //0 is untouched, 1 is started, 2 is done

	public string introText;
	public string questText;
	public string completeText;
	public float lerpCoeff;
	public TextMesh text;
	public Transform portal;
	public Transform gateObject;
	public Compass compass;
	public int nextCompassPos;
	public Vector3 gateObjectGoalPos;
	public Vector3 portalGoalPos;
	string key;
	bool playedMusic = false;
	bool playedGrind = false;

	void Start () {
		introText = introText.Replace("\\n", "\n");
		questText = questText.Replace("\\n", "\n");
		completeText = completeText.Replace("\\n", "\n");
		text.color = Color.black;
		text.text = "";
		key = "islandProgress" + id;
		progress = PlayerPrefs.GetInt(key, 0);
		if (progress != 0) {
			if (gateObject != null)
				gateObject.localPosition = gateObjectGoalPos;
			portal.localPosition = portalGoalPos;
			playedGrind = true;
		}
	}
	
	void Update () {
		if (progress != 0) {
			if (gateObject != null) {
				gateObject.localPosition = Vector3.Lerp(gateObject.localPosition, gateObjectGoalPos, lerpCoeff);
				if (!playedGrind)
					gateObject.GetComponent<AudioSource>().Play();
				playedGrind = true;
            }
			if (progress == 1)
				portal.localPosition = Vector3.Lerp(portal.localPosition, portalGoalPos, lerpCoeff);
		}
	}

	void OnTriggerStay (Collider other) {
		progress = PlayerPrefs.GetInt(key, 0);

		if (other.tag == "Player") {
			if (progress == 0 && Input.GetButtonUp("Use")) {
				progress = 1;
            }
			
			PlayerPrefs.SetInt(key, progress);

			switch (progress) {
				default:
				case 0:
					text.text = introText;
					break;
				case 1:
					text.text = questText;
					break;
				case 2:
					FinishQuest();
					text.text = completeText;
					break;
			}
			
			text.color = Color.black;
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "Player") {
			text.color = Color.clear;
		}
	}

	void FinishQuest () {
		if (compass.index != nextCompassPos) {
			if (!playedMusic)
				GetComponent<AudioSource>().Play();
			playedMusic = true;
			PlayerPrefs.SetInt("CompassIndex", nextCompassPos);
			compass.index = nextCompassPos;
		}
	}
}
