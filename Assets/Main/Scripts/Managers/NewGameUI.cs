﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewGameUI : MonoBehaviour {

	GameManager gameManager;
	GameStateManager gsManager;
	AudioManager audioMan;

	private EventSystem eventSystem;
	public GameObject postGameScreen;
	public GameObject pauseMenu;

	public Text startCounterTextObject;
	private int counter = 4;

	//Singleton
	private static NewGameUI instance;
	public static NewGameUI GetInstance() {
		return instance;
	}

	private void Awake() {
		instance = this;
	}

	private void Start() {
		gameManager = GameManager.GetInstance();
		gsManager = GameStateManager.GetInstance();
		audioMan = AudioManager.GetInstance();

		eventSystem = GetComponentInChildren<EventSystem>();
	}

	public void Update() {

		//If not in the MainMenu, allow pausing.
		if (gsManager.currentGameState != GameStateManager.GameState.inMenu || gsManager.currentGameState != GameStateManager.GameState.gameOver) {
			//print("allow pausing");
			PauseUI();
		}
	}

	public void PauseUI() {

		//When in playmode and game isn't over.
		if ((Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape)) 
			&& gsManager.currentGameState != GameStateManager.GameState.gameOver) {

			// First keypress → Pause the game and highlight the ContinueButton.
			if (gsManager.currentGameState == GameStateManager.GameState.inGame) {

				PauseGame();
				eventSystem.SetSelectedGameObject(GameObject.Find("ContinueButton"));
			}
			// Second keypress → Unpause the game.
			else if (gsManager.currentGameState == GameStateManager.GameState.inPauseMenu) {

				ResumeGame();
			}
		}
	}

	public void PauseGame() {

		audioMan.LowerVolumeInPauseMenu(true);
		Time.timeScale = 0;
		pauseMenu.SetActive(true);
		gsManager.currentGameState = GameStateManager.GameState.inPauseMenu;
	}

	public void ResumeGame() {

		audioMan.LowerVolumeInPauseMenu(false);
		Time.timeScale = 1;
		pauseMenu.SetActive(false);
		gsManager.currentGameState = GameStateManager.GameState.inGame;
	}

	//Overload Method where we can get sent the player which died or so.
	public void PostGameInformation(PlayerHandler p_playerHandler) {

		ShowPostMenu();
		print(p_playerHandler.playerController.currentPlayer + " has won!");
	}

	private void ShowPostMenu() {
		//print("Showing Post Menu");

		Time.timeScale = 0;
		postGameScreen.SetActive(true);

		eventSystem.SetSelectedGameObject(GameObject.Find("RestartPostButton"));

		audioMan.LowerVolumeInPauseMenu(true);
	}

	/// <summary>
	/// Called by GameManager at StartGame()
	/// </summary>
	public void StartCounterStuff() {

		counter--;
		startCounterTextObject.text = "MATCH STARTS IN " + counter;
		startCounterTextObject.text = "";

		if (counter == 0) {
		}
	}

	/*	
		------------------------------------------------
		PUBLIC FUNCTIONS ACCESSED BY THE MENU UI BUTTONS
		------------------------------------------------
	*/
	public void LoadLevel(string p_levelName) {
		SceneManager.LoadScene(p_levelName);
	}

	public void LoadLevel_ReloadThisLevel() {
		//audioMan.LowerVolumeInPauseMenu(false);
		string thisSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(thisSceneName);
	}

	public void LoadLevel_SpikeyCavern() {
		SceneManager.LoadScene("Spikey Cavern_TestVersion2");
	}

	public void LoadLevel_MayanTempel() {
		SceneManager.LoadScene("Mayan Tempel");
	}

	public void LoadLevel_MainMenu() {
		SceneManager.LoadScene("Scene_Menu");
	}

	//Quit the game regardless if playing in Editor, WebPlayer or in .exe-file.
	public void QuitGame() {

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
	}

}
