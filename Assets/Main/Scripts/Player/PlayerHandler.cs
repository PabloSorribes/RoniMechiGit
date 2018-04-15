using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
	private GameManager gameManager;

	//Persistance player data
	public bool active;
	public bool isAlive;
	internal bool handlerHasSpawned;
	public bool spawnLookingLeft;

	public int maxLife = 3;
	internal int lifeLeft;

	public GameObject playerCharacterPrefab;
	internal int playerIndexRobert;

	//Save a reference of the Player prefab, which in turn means that you can destroy the reference without affecting the actual prefab.
	private GameObject playerObject;
	public NewPlayerController playerController;

	public void Start()
	{
		gameManager = GameManager.GetInstance();

		lifeLeft = maxLife;

		if (lifeLeft > 0)
			isAlive = true;

		SpawnPlayer();
	}

	private void Update()
	{
		Debug_ToggleUnlimitedLife();

		Debug_Respawn();
		//DebugThumbstickValues();
		//DebugPlayersLifeStatus();
	}

	internal void SpawnPlayer()
	{
		active = true;
		playerObject = Instantiate(playerCharacterPrefab,
								SpawnLocation,
								SpawnLookRotation,
								transform);

		playerController = playerObject.GetComponent<NewPlayerController>();

		//Chooses the player-string by adding the playerindex plus an offset to compensate for our naming convention.
		playerController.currentPlayer = "_P" + (playerIndexRobert + 1);

		//Set reference to this player's PlayerHandler
		playerController.playerHandler = this;

		//Rename the PlayerHandler after the player's name, for easier recognition in the Hierarchy.
		this.gameObject.name = "PlayerHandler" + playerController.currentPlayer;

		gameManager.UpdateGameStatus(this);
	}

	//TODO: Turn into a function which takes a bool instead?
	//PROS: Less public variables, can be set directly through code here.
	//CONS: Can't be set from different script or in the inspector.
	private Vector3 SpawnLocation
	{
		get
		{
			try
			{
				//Random spawn from the array of spawnlocations.
				if (gameManager.useRandomSpawnLocations)
				{
					return gameManager.spawnPoints[UnityEngine.Random.Range(0, gameManager.spawnPoints.Length)].position;
				}

				//Use the player's index to use fixed spawn positions.
				else
				{
					return gameManager.spawnPoints[playerIndexRobert].position;
				}
			}
			catch (System.IndexOutOfRangeException)
			{
				Debug.LogError("There are no spawnpoints assigned to the GameManager. Drag and Drop a GameObject in the spawnPoints-array on the GameManager-object.");
				return Vector3.zero;
			}
		}
	}

	private Quaternion SpawnLookRotation
	{
		get
		{
			Quaternion spawnQuaternion = new Quaternion();

			if (spawnLookingLeft)
			{
				spawnQuaternion = Quaternion.LookRotation(Vector3.left);
				return spawnQuaternion;
			}
			else
			{
				spawnQuaternion = Quaternion.LookRotation(Vector3.right);
				return spawnQuaternion;
			}
		}
	}

	//Is called by the Hazards
	public void KillPlayer()
	{
		active = false;

		RemoveLife();
		playerController.OnKillPlayer();
		Destroy(playerObject);
		gameManager.UpdateGameStatus(this);

		// If still alive, Respawn in x seconds
		if (isAlive)
			Invoke("SpawnPlayer", gameManager.respawnTime);
	}

	public void RemoveLife()
	{
		//Un-parent the player from any platform it would be attached to.
		playerController.transform.SetParent(null, true);

		if (lifeLeft > 0)
			lifeLeft--;

		if (lifeLeft <= 0)
			isAlive = false;
	}

	public void DestroyPlayerInMenu()
	{
		active = false;
		isAlive = false;

		Destroy(playerObject);
		gameManager.UpdateGameStatus(this);
	}

	/// <summary>
	/// DEBUG: Unlimited Life:
	/// </summary>
	private void Debug_ToggleUnlimitedLife()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			lifeLeft = 100000;
			print("You now have Unlimited Life.");
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			lifeLeft = maxLife;
			print("You now have the default Life left again.");
		}
	}

	/// <summary>
	/// Respawn players through keycommands.
	/// </summary>
	public void Debug_Respawn()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			SpawnPlayer();
	}

	/// <summary>
	/// What values does the Left Thumbstick give right now?
	/// </summary>
	public void Debug_ThumbstickValues()
	{
		if (playerController != null)
		{
			print(playerController.currentPlayer + " LeftStickX: " + playerController.horizontalAxis);
			print(playerController.currentPlayer + " LeftStickY: " + playerController.verticalAxis);
		}
	}

	/// <summary>
	/// How many lives does each player have right now?
	/// </summary>
	public void Debug_PlayersLifeStatus()
	{
		if (playerController != null)
			print(playerController.currentPlayer + " Life left: " + lifeLeft);
	}
}