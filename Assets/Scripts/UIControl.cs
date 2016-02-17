using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour {

	public void ButtonResume () {
		SceneManager.LoadScene(1);
	}

	public void ButtonRestart () {
		/*
		PlayerPrefs.DeleteKey("shipPosX");
		PlayerPrefs.DeleteKey("shipPosY");
		PlayerPrefs.DeleteKey("shipPosZ");

		PlayerPrefs.DeleteKey("shipRotY");


		PlayerPrefs.DeleteKey("planePosX");
		PlayerPrefs.DeleteKey("planePosY");
		PlayerPrefs.DeleteKey("planePosZ");

		PlayerPrefs.DeleteKey("planeRotY");
		*/
		PlayerPrefs.DeleteAll();

		SceneManager.LoadScene(1);
	}

	public void ButtonQuit () {
		Application.Quit();
	}
}
