using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles GameStates and Number of Joined Players.
/// <para>
/// IMPORTANT: There can only be ONE object with this script in the entire game, since it isn't destroyed when loading scenes.</para>
/// </summary>
public class GameStateManager : MonoBehaviour
{
	//public List<int> joinedPlayersInt;
	public List<JoinedPlayer> joinedPlayersList;

	public enum GameState { inMenu, inGame, inPauseMenu, gameOver };
	public GameState currentGameState;

	public enum MenuState { nothing, start, playerSelection, levelSelection, settings, credits };
	public MenuState currentMainMenuState;

	private string testMenuName = _Levels.testMenu;
	private string realMenuName = _Levels.mainMenu;

	private FMODUnity.StudioEventEmitter presentationSnapshot;

	//Singleton
	private static GameStateManager instance;
	public static GameStateManager GetInstance()
	{
		return instance;
	}

	/// <summary>
	/// This makes this instance
	/// </summary>
	public void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// The class used to populate the JoinedPlayers list.
	/// <para> "myIndex" can be used to know which player is allocated in which spot of the List. </para>
	/// </summary>
	public class JoinedPlayer
	{
		public int myIndex;
		public bool hasBeenSpawned;

		/// <summary>
		/// The class used to populate the JoinedPlayers list.
		/// <para> "myIndex" can be used to know which player is allocated in which spot of the List. </para>
		/// </summary>
		public JoinedPlayer(int p_myIndex, bool p_hasBeenSpawned)
		{
			myIndex = p_myIndex;
			hasBeenSpawned = p_hasBeenSpawned;
		}

		//This method is required by the IComparable interface, ie. SORTING A LIST
		public int CompareTo(JoinedPlayer other)
		{
			if (other == null)
			{
				return 1;
			}

			//Return your index compared to the other one's.
			//Used for SORTING.
			return myIndex - other.myIndex;
		}
	}

	private void Start()
	{
		//Subscribe to the event of scenes being loaded.
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;

		//Make the List a fixed length.
		//joinedPlayersInt.Capacity = 4;

		//Initialize items for the List
		joinedPlayersList = new List<JoinedPlayer>();
		joinedPlayersList.Capacity = 4;

		//Debug for simulating spawning players in the player selection menu, when in the test_menu
		if (SceneManager.GetActiveScene().name == testMenuName && currentGameState == GameState.inMenu)
		{
			currentMainMenuState = MenuState.playerSelection;
		}

		// WARNING: The GameStateManager-prefab cannot be underneath another object, else it WILL GET DESTROYED when loading a new scene.
		transform.parent = null;
		DontDestroyOnLoad(transform.gameObject);

		//- Hide the mouse-cursor
		Cursor.visible = false;

		//- Only keep one instance of this object in the game.
		if (instance == null)
		{
			instance = this;
		}
		else if (FindObjectOfType<GameStateManager>().gameObject != this.gameObject)
		{
			Destroy(FindObjectOfType<GameStateManager>().gameObject);
		}

		InitializeAudio();
	}

	private void InitializeAudio()
	{
		presentationSnapshot = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		presentationSnapshot.Event = "snapshot:/presentationSnapshot";
	}

	private void Update()
	{
		//print("GameState: " + currentGameState);
		//print("MenuState: " + currentMenuState);
		//print("Joined players Int: " + joinedPlayersInt.Count);
		//print("Joined players list Length: " + joinedPlayersList.Count);

		foreach (var _joinedPlayer in joinedPlayersList)
		{
			//print("JoinedPlayer: " + _joinedPlayer.myIndex + ". → Has been spawned: " + _joinedPlayer.hasBeenSpawned);
		}

		//Status of each player
		//for (int _index = 0; _index < joinedPlayersList.Count; _index++) {
		//	print("Index " + _index + ": " + joinedPlayersList[_index]);
		//}

		Debug_LoadPlaySceneOrMenuScene();

		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			RestartGame();
		}

		GetPresentationAudioInput();
	}

	private void GetPresentationAudioInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha5))
			ActivatePresentationAudio();

		if (Input.GetKeyDown(KeyCode.Alpha6))
			DeActivatePresentationAudio();
	}

	private void ActivatePresentationAudio()
	{
		if (!presentationSnapshot.IsPlaying())
			presentationSnapshot.Play();
	}

	private void DeActivatePresentationAudio()
	{
		if (presentationSnapshot.IsPlaying())
			presentationSnapshot.Stop();
	}

	/// <summary>
	/// Should be called by the GameManagerMenu when adding a player.
	/// <para>Registers the player with the index being inputted in the function.</para>
	/// </summary>
	/// <param name="p_index"></param>
	public void RegisterPlayer(int p_index)
	{
		if (p_index == 0 || p_index == 1 || p_index == 2 || p_index == 3)
		{
			JoinedPlayer _newJoinedPlayer = new JoinedPlayer(0, false)
			{
				myIndex = p_index,
				hasBeenSpawned = true
			};

			//If the player doesn't exist in the List, add it.
			joinedPlayersList.Add(_newJoinedPlayer);
			//print("Added a player to JoinedPlayers List.");

			int amountOfThisPlayerIndexThatHaveJoined = 0;

			//Scan the list for players that have the same "myIndex", ie. duplicates. Add them to the int.
			for (int i = 0; i < joinedPlayersList.Count; i++)
			{
				if (joinedPlayersList[i].myIndex == p_index)
				{
					amountOfThisPlayerIndexThatHaveJoined++;
				}
			}

			//Remove the extra player that was added the second time this function was called.
			if (amountOfThisPlayerIndexThatHaveJoined > 1)
			{
				joinedPlayersList.Remove(_newJoinedPlayer);
				//print("Removed excess player from Players Joined-list");
			}

			//Set the first player that was added into the "spawned"-state again (because of how UnRegisterPlayer() works)
			if (joinedPlayersList[p_index] != null)
			{
				//print("List contains Player: " + _newJoinedPlayer.myIndex);
				joinedPlayersList[p_index].hasBeenSpawned = true;
			}
		}
	}

	/// <summary>
	/// Sets the selected player from the joinedPlayers-list to "hasBeenSpawned" = false.  Used to despawn players in the Menu.
	/// </summary>
	/// <param name="p_index"></param>
	public void UnRegisterPlayer(int p_index)
	{
		if (p_index == 0 || p_index == 1 || p_index == 2 || p_index == 3)
		{
			joinedPlayersList[p_index].hasBeenSpawned = false;
			//joinedPlayersList.RemoveAt(p_index);

			//TODO:	Using an int is bad.
			//		When removing players in a non-sorted manner the player will keep their index,
			//		but the length of the joinedPlayersInt-list will be shorter than the p_index being sent to it.
			//		This will create a NullRef, and the player won't be able to despawn.
			//joinedPlayersInt.Remove(p_index);
		}
	}

	public void Debug_LoadPlaySceneOrMenuScene()
	{
		if (Input.GetKeyDown(KeyCode.N))
			SceneManager.LoadScene(realMenuName);

		if (Input.GetKeyDown(KeyCode.B))
			SceneManager.LoadScene(testMenuName);
	}

	/// <summary>
	/// Debug for resetting the game during a presentation.
	/// </summary>
	/// <returns></returns>
	public void RestartGame()
	{
		SceneManager.LoadScene(realMenuName);
	}

	//When loading a scene, checks which scene was loaded, and does actions through that.
	private void SceneManager_sceneLoaded(Scene p_scene, LoadSceneMode p_loadingMode)
	{
		//Reset the flow of Time when going to any new Scene.
		Time.timeScale = 1;

		////Reset Pause-/GameOver-menu snapshot for audio when loading a new Scene.
		//AudioManager.GetInstance().LowerVolumeInPauseMenu(false);

		//- When menu is loaded, clear the currently joined players list and change the Game State.
		if (p_scene.name == realMenuName || p_scene.name == testMenuName)
		{
			currentGameState = GameState.inMenu;
			//print("Clearing List of Joined Players in GameStateManager");
			//joinedPlayersInt.Clear();
			joinedPlayersList.Clear();
		}

		//DEBUG: For being able to spawn players in the test-scene
		if (p_scene.name == testMenuName)
		{
			currentMainMenuState = MenuState.playerSelection;
		}

		//If not in a Menu Scene, we are inGame. Also, menu state is nothing.
		if (p_scene.name != realMenuName || p_scene.name != testMenuName)
		{
			currentGameState = GameState.inGame;
			currentMainMenuState = MenuState.nothing;
		}

		//This is an enum, so you'll have to use this syntax when loading it from some other function:
		//p_loadingMode(LoadSceneMode.Single)
	}
}