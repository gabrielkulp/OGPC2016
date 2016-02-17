using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CrossSceneController : MonoBehaviour {
	public GameObject player;

	void Start () {
		DontDestroyOnLoad(this.gameObject);
	}
	
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)) {

			Save(player.transform.position);
			Destroy(this.gameObject);
			Cursor.visible = true;
			SceneManager.LoadScene(0);
		}
	}

	void OnLevelWasLoaded (int level) {
		player = GameObject.Find("Player");
        if (level == 1)
			Load();
	}

	public void LoadSideLevel (int targetSceneIndex, Vector3 respawnPos) {
		Save(respawnPos);
		DontDestroyOnLoad(player);
		SceneManager.LoadScene(targetSceneIndex);
	}

	public void ReturnFromSideLevel () {
		DontDestroyOnLoad(this.gameObject);
		Destroy(player);
		SceneManager.LoadScene(1);
	}

	public void Save (Vector3 playerPos) {
		Transform ship = GameObject.Find("AirShip").transform;
		Vector3 shipPos;
		Vector3 shipRot;
		shipPos = ship.position;
		shipRot = ship.eulerAngles;
		PlayerPrefs.SetFloat("shipPosX", shipPos.x);
		PlayerPrefs.SetFloat("shipPosY", shipPos.y);
		PlayerPrefs.SetFloat("shipPosZ", shipPos.z);
		PlayerPrefs.SetFloat("shipRotY", shipRot.y);

		AirplaneController plane = GameObject.Find("Airplane").GetComponent<AirplaneController>();
		Vector3 planePos;
		Vector3 planeRot;
		planePos = plane.transform.position;
		planeRot = plane.transform.eulerAngles;
		PlayerPrefs.SetFloat("planePosX", planePos.x);
		PlayerPrefs.SetFloat("planePosY", planePos.y);
		PlayerPrefs.SetFloat("planePosZ", planePos.z);
		PlayerPrefs.SetFloat("planeRotY", planeRot.y);

		//player = GameObject.Find("Player");
		PlayerPrefs.SetFloat("playerPosX", playerPos.x);
		PlayerPrefs.SetFloat("playerPosY", playerPos.y);
		PlayerPrefs.SetFloat("playerPosZ", playerPos.z);
		PlayerPrefs.SetFloat("playerRotY", player.transform.eulerAngles.y);
		PlayerPrefs.SetString("playerOnShip", player.GetComponent<Player>().onShip.ToString());
	}

	public void Load () {
		Transform ship = GameObject.Find("AirShip").transform;
		Vector3 shipPos = ship.position;
		Vector3 shipRot = ship.eulerAngles;
		shipPos.x = PlayerPrefs.GetFloat("shipPosX", shipPos.x);
		shipPos.y = PlayerPrefs.GetFloat("shipPosY", shipPos.y);
		shipPos.z = PlayerPrefs.GetFloat("shipPosZ", shipPos.z);
		shipRot.y = PlayerPrefs.GetFloat("shipRotY", shipRot.y);
		ship.position = shipPos;
		ship.eulerAngles = shipRot;

		AirplaneController plane = GameObject.Find("Airplane").GetComponent<AirplaneController>();
		Vector3 planePos = plane.dockPos;
		Vector3 planeRot = Vector3.zero;
		planePos.x = PlayerPrefs.GetFloat("planePosX", planePos.x);
		planePos.y = PlayerPrefs.GetFloat("planePosY", planePos.y);
		planePos.z = PlayerPrefs.GetFloat("planePosZ", planePos.z);
		planeRot.y = PlayerPrefs.GetFloat("planeRotY", shipRot.y);
		plane.transform.position = planePos;
		plane.transform.eulerAngles = planeRot;

		//player = GameObject.Find("Player");
		Vector3 playerPos = player.transform.position;
		Vector3 playerRot = player.transform.eulerAngles;
		playerPos.x = PlayerPrefs.GetFloat("playerPosX", playerPos.x);
		playerPos.y = PlayerPrefs.GetFloat("playerPosY", playerPos.y);
		playerPos.z = PlayerPrefs.GetFloat("playerPosZ", playerPos.z);
		playerRot.y = PlayerPrefs.GetFloat("playerRotY", playerRot.y);
		player.GetComponent<Player>().onShip = bool.Parse(PlayerPrefs.GetString("playerOnShip", bool.TrueString));
		player.transform.position = playerPos;
		player.transform.eulerAngles = playerRot;
    }
}
