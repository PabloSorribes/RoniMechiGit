using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IMPORTANT: This script can ONLY be in the Menu-scene, not in-game.
/// </summary>
public class GameManagerMenu : MonoBehaviour {

	GameManager gameManager;
	GameStateManager gsManager;

	int counter = 0;
	bool stopDoubleSpawning = true;

	public enum AmountOfPlayersToSpawn { zero, one, two, three, four };
	public AmountOfPlayersToSpawn amountOfPlayersToSpawn;

	//Singleton
	private static GameManagerMenu instance;
	public static GameManagerMenu GetInstance() {

		return instance;
	}

	// Use this for initialization
	void Start() {
		gsManager = GameStateManager.GetInstance();
		gameManager = GameManager.GetInstance();


		//Just automatically add x amount of players on start.
		SpawnPlayersOnStart();
	}

	//Intended for adding ONE player on Start in the Menu.
	private void SpawnPlayersOnStart() {

		for (int index = 0; index < (int)amountOfPlayersToSpawn; index++) {

			AddPlayer(index);
		}

		if (amountOfPlayersToSpawn == AmountOfPlayersToSpawn.zero) {
			//print("No players are spawned on Start");
		}

		//print("GM Menu - Amount of PlayerHandlers: " + gameManager.playerHandlers.Count);
	}

	// Update is called once per frame
	void Update() {

		//print("Stop Double-Spawning: " + stopDoubleSpawning);
		//print("Counter: " + counter);

		//If we are in the player selection menu, allow spawning of players
		if (gsManager.currentMainMenuState == GameStateManager.MenuState.playerSelection) {
			GetPlayerInputAssignment();
		}
	}

	private void GetPlayerInputAssignment() {

		//Spawn new player in menu
		if (Input.GetButtonDown("Jump_P1") || Input.GetKeyDown(KeyCode.Keypad1))
			AddPlayer(0);
		if (Input.GetButtonDown("Jump_P2") || Input.GetKeyDown(KeyCode.Keypad2))
			AddPlayer(1);
		if (Input.GetButtonDown("Jump_P3") || Input.GetKeyDown(KeyCode.Keypad3))
			AddPlayer(2);
		if (Input.GetButtonDown("Jump_P4") || Input.GetKeyDown(KeyCode.Keypad4))
			AddPlayer(3);

		//Despawn a player in menu
		if (Input.GetButtonDown("Deflect_P1") || Input.GetKeyDown(KeyCode.Alpha1))
			RemovePlayer(0);
		if (Input.GetButtonDown("Deflect_P2") || Input.GetKeyDown(KeyCode.Alpha2))
			RemovePlayer(1);
		if (Input.GetButtonDown("Deflect_P3") || Input.GetKeyDown(KeyCode.Alpha3))
			RemovePlayer(2);
		if (Input.GetButtonDown("Deflect_P4") || Input.GetKeyDown(KeyCode.Alpha4))
			RemovePlayer(3);
	}

	private void AddPlayer(int p_index) {

		//if () {

		//}

		//TODO: Make this operation unique for each player. 
		if (counter == 0) {
			stopDoubleSpawning = false;
			counter++;
		}
		else if (counter > 0) {
			stopDoubleSpawning = true;
		}

		if (stopDoubleSpawning) {

			//Om index redan är tilldelat, hoppa ur funktionen.
			foreach (var handler in gameManager.playerHandlers) {
				if (handler.playerIndexRobert == p_index) {
					return;
				}
			}
		}

		//Else, add a PlayerHandler with its correct index, and add the selected player in the joinedPlayers-list in GameStateManager.
		gameManager.AddPlayerHandlerInMenu(p_index);
		gsManager.RegisterPlayer(p_index);
	}

	private void RemovePlayer(int p_index) {

		counter = 0;	

		////Check if the index is valid.
		//if (gsManager.joinedPlayersInt.Contains(gsManager.joinedPlayersInt[p_index])) {

		gameManager.RemovePlayerHandler(p_index);
		gsManager.UnRegisterPlayer(p_index);
		//}
	}
}
