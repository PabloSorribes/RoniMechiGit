using System;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private GameStateManager gsManager;

	//Creates field in inspector. Drag FmodStudioEventEmitter-component on it. Set event in the component.
	private FMODUnity.StudioEventEmitter musicMenu;
	private FMODUnity.StudioEventEmitter musicGame;
	private FMODUnity.StudioEventEmitter ambience;
	private FMODUnity.StudioEventEmitter snap_inPauseMenu;

	private string masterBusString = "Bus:/MAIN";
	private FMOD.Studio.Bus masterBus;
	private float mainBusVolumeToSet = 1;

	//Singleton
	private static AudioManager instance;
	public static AudioManager GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	private void Start()
	{
		//Makes Audio run in background (when Alt-Tabbing for live-mixing and so on).
		Application.runInBackground = true;

		gsManager = GameStateManager.GetInstance();
		GameStateManager.OnGameStateChanged += HandleGameStateChanged;

		GameManager.OnGameStarted += HandleGameStarted;

		//Initialize the bus.
		masterBus = FMODUnity.RuntimeManager.GetBus(masterBusString);

		InitializeAudio();

		ChooseLevelMusic();
	}

	private void HandleGameStateChanged(GameStateManager.GameState obj)
	{
		switch (obj)
		{
			case GameStateManager.GameState.inMenu:
				break;
			case GameStateManager.GameState.inGame:
				break;
			case GameStateManager.GameState.inPauseMenu:
				break;
			case GameStateManager.GameState.gameOver:
				break;
			default:
				break;
		}

	}

	private void InitializeAudio()
	{
		//Add a StudioEventEmitter-component to the AudioManager-gameObject. This also automatically gets that as a referece.
		musicMenu = gameObject.AddComponent<StudioEventEmitter>();
		musicMenu.Event = "event:/musicMenu";

		musicGame = gameObject.AddComponent<StudioEventEmitter>();
		musicGame.Event = "event:/musicGame";

		ambience = gameObject.AddComponent<StudioEventEmitter>();
		ambience.Event = "event:/ambience";

		snap_inPauseMenu = gameObject.AddComponent<StudioEventEmitter>();
		snap_inPauseMenu.Event = "snapshot:/inPauseMenu";
	}

	/// <summary>
	/// Choose music depending on which GameState we are in.
	/// </summary>
	private void ChooseLevelMusic()
	{
		if (gsManager.currentGameState == GameStateManager.GameState.inMenu)
		{
			ambience.Stop();
			musicGame.Stop();
			musicMenu.Play();
		}
		else if (gsManager.currentGameState == GameStateManager.GameState.inGame)
		{
			ambience.Play();
			musicMenu.Stop();
		}
	}

	/// <summary>
	/// Called when Game Manager has spawned all the players.
	/// </summary>
	private void HandleGameStarted()
	{
		musicGame.Play();
	}

	/// <summary>
	/// TRUE = starts snapshot. FALSE = stops snapshot.
	/// </summary>
	/// <param name="p_active"></param>
	public void LowerVolumeInPauseMenu(bool p_active)
	{
		if (p_active)
		{
			musicGame.SetParameter("vaporwave", 1f);
			snap_inPauseMenu.Play();
		}
		else
		{
			musicGame.SetParameter("vaporwave", 0f);
			snap_inPauseMenu.Stop();
		}
	}

	/// <summary>
	/// <paramref name="p_turnOffAudio"/> → TRUE for turning off audio. FALSE for turning it back on.
	/// </summary>
	/// <param name="p_turnOffAudio"></param>
	public void MuteAudio(bool p_turnOffAudio)
	{
		masterBus.setMute(p_turnOffAudio);
	}

	/// <summary>
	/// <paramref name="p_volumeToChangeWith"/> should preferably be something small, such as 0.15f (volume increase) or -0.15f (volume decrease)
	/// <para></para> 0 == No volume, -80 dB.
	/// <para></para> 1 == Max volume, 0 dB (or the level set in the Fmod-project?)
	/// </summary>
	/// <param name="p_volumeToChangeWith"></param>
	public void SetAudioVolume(float p_volumeToChangeWith)
	{
		mainBusVolumeToSet += p_volumeToChangeWith;

		if (mainBusVolumeToSet > 1)
		{
			mainBusVolumeToSet = 1;
		}

		if (mainBusVolumeToSet < 0)
		{
			mainBusVolumeToSet = 0;
		}

		masterBus.setVolume(mainBusVolumeToSet);

		print(mainBusVolumeToSet);
	}

	private void OnDestroy()
	{
		//Reset Pause-/GameOver-menu snapshot for audio when loading a new/the same Scene.
		LowerVolumeInPauseMenu(false);
		musicGame.Stop();
	}
}