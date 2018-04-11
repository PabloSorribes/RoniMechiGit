using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Courtesy of: https://youtu.be/_EROILoOnT4
//And: https://github.com/FirstGearGames/SuperCam
public class CameraController : MonoBehaviour {

	private GameManager gameMan;

	//Reference to all the players in the level
	[HideInInspector] public List<NewPlayerController> playerControllers;
	private FocusLevel focusLevel;

	public float DepthUpdateSpeed = 5f;
	public float AngleUpdateSpeed = 7f;
	public float PositionUpdateSpeed = 5f;

	public float DepthMax = -25f;
	public float DepthMin = -25f;

	public float AngleMax = 0f;
	public float AngleMin = 0f;

	private float CameraEulerX;
	private Vector3 CameraPosition;

	public float offsetY = 30f;

	// Use this for initialization
	void Start() {

		focusLevel = FindObjectOfType<FocusLevel>();

		gameMan = GameManager.GetInstance();

		//TODO: See if Actions can send objects (ie. p_PlayerHandler) to their delegates.
		//GameManager.OnUpdateGameStatus(GameManager) += GetPlayers;
	}


	// Update is called once per frame
	private void LateUpdate() {

		//print("Amount of players in Camera List: " + playerControllers.Count);

		//If we have elements in the List, do stuff.
		if (playerControllers.Count > 0) {

			CalculateCameraLocations();
			MoveCamera();
		}
		else {
			float depth = Mathf.Lerp(DepthMax, DepthMin, 0.5f);

			CameraPosition = new Vector3(focusLevel.transform.position.x, focusLevel.transform.position.y, depth);
		}
	}

	//Is called from UpdateGameStatus() in GameManager
	public void UpdatePlayersInList(PlayerHandler p_playerHandler) {

		int p_index = p_playerHandler.playerIndexRobert;
		bool isThisHandlerActive = p_playerHandler.active;

		//Om index redan är tilldelat och spelaren är aktiv, lägg till en spelare.
		foreach (var handler in gameMan.playerHandlers) {

			if (handler.playerIndexRobert == p_index && isThisHandlerActive) {

				//print("Adding player to Camera List");
				playerControllers.Add(p_playerHandler.playerController);
			}
			//Else, remove the Handler's player from the List.
			else if (handler.playerIndexRobert == p_index && !isThisHandlerActive) {

				//print("Removing player from Camera List");
				playerControllers.Remove(p_playerHandler.playerController);
			}
		}
	}

	private void CalculateCameraLocations() {

		//Initialize the Vectors
		Vector3 averageCenter = Vector3.zero;       //Average center of all the players
		Vector3 totalPositions = Vector3.zero;      //The sum of all the positions of our players
		Bounds playerBounds = new Bounds();

		for (int _index = 0; _index < playerControllers.Count; _index++) {

			Vector3 playerPosition = playerControllers[_index].transform.position;

			//Check if the player is within the focus bounds.
			if (!focusLevel.focusBounds.Contains(playerPosition)) {

				float playerX = Mathf.Clamp(playerPosition.x, focusLevel.focusBounds.min.x, focusLevel.focusBounds.max.x);
				float playerY = Mathf.Clamp(playerPosition.y, focusLevel.focusBounds.min.y, focusLevel.focusBounds.max.y);
				float playerZ = Mathf.Clamp(playerPosition.z, focusLevel.focusBounds.min.z, focusLevel.focusBounds.max.z);
				playerPosition = new Vector3(playerX, playerY, playerZ);
			}
			totalPositions += playerPosition;
			playerBounds.Encapsulate(playerPosition);
		}
		
		averageCenter = (totalPositions / playerControllers.Count);

		float extents = (playerBounds.extents.x + playerBounds.extents.y + playerBounds.extents.z);
		float lerpPercent = Mathf.InverseLerp(0, (focusLevel.halfXBounds + focusLevel.halfYBounds) / 2, extents);

		float depth = Mathf.Lerp(DepthMax, DepthMin, lerpPercent);
		float angle = Mathf.Lerp(AngleMax, AngleMin, lerpPercent);

		CameraEulerX = angle;
		CameraPosition = new Vector3(averageCenter.x, averageCenter.y, depth);
		//Debug.Log(CameraPosition);
	}

	private void MoveCamera() {

		Vector3 position = gameObject.transform.position;

		if (position != CameraPosition) {

			Vector3 targetPosition = Vector3.zero;
			targetPosition.x = Mathf.MoveTowards(position.x, CameraPosition.x, PositionUpdateSpeed * Time.deltaTime);
			targetPosition.y = Mathf.MoveTowards(position.y, CameraPosition.y + offsetY, PositionUpdateSpeed * Time.deltaTime);
			targetPosition.z = Mathf.MoveTowards(position.z, CameraPosition.z, DepthUpdateSpeed * Time.deltaTime);
			gameObject.transform.position = targetPosition;
		}

		Vector3 localEulerAngles = gameObject.transform.localEulerAngles;

		if (localEulerAngles.x != CameraEulerX) {

			Vector3 targetEulerAngles = new Vector3(CameraEulerX, localEulerAngles.y, localEulerAngles.z);
			gameObject.transform.localEulerAngles = Vector3.MoveTowards(localEulerAngles, targetEulerAngles, AngleUpdateSpeed * Time.deltaTime);
		}
	}

}
