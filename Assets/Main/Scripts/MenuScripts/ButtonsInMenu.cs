using UnityEngine;

public class ButtonsInMenu : MonoBehaviour
{
	private GameStateManager gsManager;
	private MenuManager menuManager;
	private GameObject player;

	public enum TypeOfButton { staticTeleporter, lavaTeleport, goForwardInMenu, goBackInMenu };
	public TypeOfButton buttonType;

	public Transform playerTargetPosition;
	public string animationToRun;

	private FMODUnity.StudioEventEmitter a_buttonSound;

	public void Start()
	{
		menuManager = FindObjectOfType<MenuManager>();

		gsManager = GameStateManager.GetInstance();

		Invoke("GetPlayerOne", .2f);

		InitializeAudio();
	}

	private void InitializeAudio()
	{
		string eventstring = "event:/uiButton_teleport";

		switch (buttonType)
		{
			case TypeOfButton.staticTeleporter:
				eventstring = "event:/uiButton_teleport";
				break;

			case TypeOfButton.lavaTeleport:
				eventstring = "event:/uiButton_lavaTeleport";
				break;

			case TypeOfButton.goForwardInMenu:
				eventstring = "event:/uiButton_forward";
				break;

			case TypeOfButton.goBackInMenu:
				eventstring = "event:/uiButton_backward";
				break;

			default:
				break;
		}

		a_buttonSound = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_buttonSound.Event = eventstring;
	}

	//TODO: Improve the getting of the player
	private void GetPlayerOne()
	{
		player = GameManager.GetInstance().playerHandlers[0].playerController.gameObject;
	}

	private void OnTriggerEnter(Collider other)
	{
		//Animate camera, teleport player and update the Menu Status
		if (other.tag == _Tags.bullet || other.tag == _Tags.player)
		{
			//Avoid sending an unnecessary warning when not activating an animation.
			if (animationToRun != "")
			{
				menuManager.anim.SetTrigger(animationToRun);
				UpdateMenuStatus(animationToRun);
			}
			player.transform.position = playerTargetPosition.position;
			a_buttonSound.Play();
		}
	}

	//TODO: Add statements for each animation that moves the camera to a different state.
	private void UpdateMenuStatus(string p_animationToRun)
	{
		if (p_animationToRun == "BackPSelect")
		{
			gsManager.currentMainMenuState = GameStateManager.MenuState.start;
		}

		//Players can join the game
		else if (p_animationToRun == "toPlayerSelection" || p_animationToRun == "BackLSelect")
		{
			gsManager.currentMainMenuState = GameStateManager.MenuState.playerSelection;
		}
	}
}