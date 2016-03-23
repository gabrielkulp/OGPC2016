using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIControl : MonoBehaviour {
	public Transform camRig;
	public Transform defaultCamPos;
	public float camRotConst = 0.00001f;
	public float camWobbleConst = 5f;
	public float camWobbleTimeConst = 0.25f;
	public float lerpConst = 0.1f;
	public AudioMixer mixer;
	public Toggle invertToggle;
	public Slider masterSlider;
	public Slider musicSlider;
	public Slider sfxSlider;
	Transform cam;
	Transform targetTransform;
	Transform lastTransform;
	Vector3 wobbleTime;

	void Start () {
		cam = camRig.GetChild(0);
		targetTransform = defaultCamPos;
		defaultCamPos.parent.GetComponent<UIMaskControl>().Open();
		wobbleTime.x = Random.Range(0, 2 * Mathf.PI);
		wobbleTime.y = Random.Range(0, 2 * Mathf.PI);
		wobbleTime.z = Random.Range(0, 2 * Mathf.PI);

		if (PlayerPrefs.GetInt("InvertFlight", 0) == 1)
			invertToggle.isOn = true;
		else
			invertToggle.isOn = false;
		//invertToggle.isOn = (PlayerPrefs.GetInt("InvertFlight", 0) == 1) ? true : false;

		float masVol;
		float musVol;
		float sfxVol;
		if (mixer.GetFloat("MasterVol", out masVol)) {
			masterSlider.value = masVol;
		}
		if (mixer.GetFloat("MusicVol", out musVol)) {
			musicSlider.value = musVol;
		}
		if (mixer.GetFloat("SFXVol", out sfxVol)) {
			sfxSlider.value = sfxVol;
		}
	}

	void Update () {
		//Cam motion
		camRig.position = Vector3.Lerp(camRig.position, targetTransform.position, lerpConst);
		camRig.rotation = Quaternion.Slerp(camRig.rotation, targetTransform.rotation, lerpConst / 2);

		//Cam wobble
		cam.localRotation = Quaternion.FromToRotation(Vector3.forward,
			(Input.mousePosition * camRotConst) + Vector3.forward) *
			Quaternion.Euler(
				Mathf.Sin(wobbleTime.x) * camWobbleConst,
				Mathf.Sin(wobbleTime.y) * camWobbleConst,
				Mathf.Sin(wobbleTime.z) * camWobbleConst);
		wobbleTime.x += Random.Range(0.75f, 1.25f) * camWobbleTimeConst * Time.deltaTime;
		wobbleTime.y += Random.Range(0.75f, 1.25f) * camWobbleTimeConst * Time.deltaTime;
		wobbleTime.z += Random.Range(0.75f, 1.25f) * camWobbleTimeConst * Time.deltaTime;
	}

	public void ButtonGotoPos (Transform camPos) {
		targetTransform.parent.GetComponent<UIMaskControl>().Close();
		targetTransform = camPos;
		camPos.parent.GetComponent<UIMaskControl>().Open();
	}

	public void ButtonResume () {
		SceneManager.LoadScene(1);
	}

	public void ButtonRestart () {
		ButtonDeleteSave(targetTransform);
		SceneManager.LoadScene(1);
	}

	public void ButtonDeleteSave (Transform nextCamPos) {
		PlayerPrefs.DeleteAll();
		mixer.SetFloat("MasterVol", 0f);
		mixer.SetFloat("MusicVol", 0f);
		mixer.SetFloat("SFXVol", 0f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ButtonQuit () {
		Application.Quit();
	}

	public void ToggleInvert (Toggle source) {
		if (source.isOn)
			PlayerPrefs.SetInt("InvertFlight", 1);
		else
			PlayerPrefs.SetInt("InvertFlight", 0);
	}

	public void SliderMasterVol (Slider source) {
		mixer.SetFloat("MasterVol", source.value);
	}

	public void SliderMusicVol (Slider source) {
		mixer.SetFloat("MusicVol", source.value);
	}

	public void SliderSFXVol (Slider source) {
		mixer.SetFloat("SFXVol", source.value);
	}
}
