using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Updates the GameStatus
/// </summary>
public class GameManager : MonoBehaviour
{
	private GameStateManager gsManager;
	private NewGameUI gameUI;

	//Variables
	public PlayerHandler playerHandlerPrefab;

	[HideInInspector] public List<PlayerHandler> playerHandlers;
	public Transform[] spawnPoints;
	public int alivePlayers;

	public float respawnTime = 1f;
	public bool useRandomSpawnLocations;

	private bool gameStarted;

	//Singleton
	private static GameManager instance;

	public static GameManager GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		gsManager = GameStateManager.GetInstance();
		gameUI = NewGameUI.GetInstance();

		//TODO: If we are inGame, play the start animation thingy whatevs
		if (gsManager.currentGameState == GameStateManager.GameState.inGame)
		{
			//Invoke("StartGame", 1);
			//Invoke("StartGame", 2);
			//Invoke("StartGame", 3);
			//Invoke("StartGame", 4);
			useRandomSpawnLocations = true;
			StartGame();
		}

		if (gsManager.currentGameState == GameStateManager.GameState.inMenu)
		{
			useRandomSpawnLocations = false;
		}

		//print("Amount of playerHandlers: " + playerHandlers.Count);
		//alivePlayers = playerHandlers.Count;

		//Know status for each player
		foreach (var handler in playerHandlers)
		{
			if (handler.playerIndexRobert == 0)
			{
				//but do this in the PlayerHandler / send it's index and player's position to the CameraController
			}
		}
	}

	private void StartGame()
	{
		//Add players depending on how many have joined
		//foreach (var p_index in gsManager.joinedPlayersInt) {
		//	AddPlayerHandler(p_index);
		//}

		foreach (var p_joinedPlayer in gsManager.joinedPlayersList)
		{
			AddPlayerHandler(p_joinedPlayer.myIndex);
		}

		UpdateAlivePlayers(true, true);

		//gameUI.StartCounterStuff();
		AudioOnStart();
	}

	private void AudioOnStart()
	{
		FMODUnity.RuntimeManager.PlayOneShot("event:/openLevelGong");
	}

	public void AddPlayerHandler(int p_playerIndex)
	{
		var _newPlayerHandler = Instantiate(playerHandlerPrefab, transform);
		_newPlayerHandler.playerIndexRobert = p_playerIndex;
		playerHandlers.Add(_newPlayerHandler);
	}

	//TODO: Fix double spawning when Adding all players → Removing Player1 → Adding Player2 → duplicate Player 2 is added
	public void AddPlayerHandlerInMenu(int p_playerIndex)
	{
		var _newPlayerHandler = Instantiate(playerHandlerPrefab, transform);
		_newPlayerHandler.playerIndexRobert = p_playerIndex;

		//playerHandlers.RemoveAt(p_playerIndex);
		playerHandlers.Insert(p_playerIndex, _newPlayerHandler);

		//TODO: Getting nullrefs here when spawning in an unordered fashion.
		if (playerHandlers[p_playerIndex].handlerHasSpawned == true)
		{
			playerHandlers[p_playerIndex].Start();
		}

		if (playerHandlers[p_playerIndex].handlerHasSpawned == false)
		{
			playerHandlers[p_playerIndex].handlerHasSpawned = true;
		}
	}

	//EXPERIMENTAL CODE
	public void RemovePlayerHandler(int p_playerIndex)
	{
		//var _newPlayerHandler = playerHandlers[p_playerIndex];
		//_newPlayerHandler.playerIndexRobert = p_playerIndex;

		playerHandlers[p_playerIndex].DestroyPlayerInMenu();

		//playerHandlers.Remove(playerHandlers[p_playerIndex]);
	}

	private void Update()
	{
		//print("Amount of playerHandlers: " + playerHandlers.Count);
		//print("Amount of alive players: " + alivePlayers);
	}

	/// <summary>
	/// <paramref name="p_setAmountOnStart"/>: Set to TRUE when using once, eg. in Start().
	/// Set to FALSE when calling more than once, eg. in UpdateGameStatus().
	///
	/// <para> <paramref name="p_isPlayerAlive"/>: Takes the "isAlive"-bool from a PlayerHandler.
	/// </para>
	/// </summary>
	/// <param name="p_setAmountOnStart"></param>
	/// <param name="p_isPlayerAlive"></param>
	public void UpdateAlivePlayers(bool p_setAmountOnStart, bool p_isPlayerAlive)
	{
		if (p_setAmountOnStart)
		{
			//alivePlayers = gsManager.joinedPlayersInt.Count;

			int _myAmountOfJoinedPlayers = 0;

			foreach (var joinedPlayer in gsManager.joinedPlayersList)
			{
				if (joinedPlayer.hasBeenSpawned)
				{
					_myAmountOfJoinedPlayers++;

					//alivePlayers++;
				}
			}

			alivePlayers = _myAmountOfJoinedPlayers;

			//alivePlayers = gsManager.joinedPlayersList.Count;
		}

		if (!p_isPlayerAlive)
		{
			alivePlayers--;
		}

		//print("Alive Players: " + alivePlayers);
	}

	/// <summary>
	/// Check if we only have one player left, etc.
	/// </summary>
	/// <param name="p_playerHandler"></param>
	public void UpdateGameStatus(PlayerHandler p_playerHandler)
	{
		//TODO: Check if we are inGame or not. If yes, add players and shit. Needs refactoring.
		if (gsManager.currentGameState == GameStateManager.GameState.inGame)
		{
			//Update the players in the CameraController's List of players.
			Camera.main.GetComponent<CameraController>().UpdatePlayersInList(p_playerHandler);

			UpdateAlivePlayers(false, p_playerHandler.isAlive);

			//Allow the game to be stopped (by eg. postGameUI) only if we FIRST have 2 or more players in the scene, and THEN one of them dies.
			if (alivePlayers > 1)
			{
				gameStarted = true;
			}

			//When there's only one player left and the game has already started →
			// → show the PostGameUI and send which player is left.
			if (alivePlayers == 1 && gameStarted)
			{
				gsManager.currentGameState = GameStateManager.GameState.gameOver;
				//print("Sending call to PostGameInfo");
				gameUI.PostGameInformation(p_playerHandler);
			}
			else if (alivePlayers < 1 && gameStarted)
			{
				gsManager.currentGameState = GameStateManager.GameState.gameOver;
				print("Everybody dieded! D: ");
			}
		}
	}
}