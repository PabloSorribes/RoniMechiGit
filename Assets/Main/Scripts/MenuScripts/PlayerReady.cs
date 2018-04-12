using UnityEngine;

public class PlayerReady : MonoBehaviour
{
	private MenuManager menuManager;

	//Is set in the inspector to move PLAYER ONE to the next place in the menu.
	public Transform playerTargetPosition;
	private GameObject player;
	public string animationToRun;

	private FMODUnity.StudioEventEmitter a_forwardButtonSound;

	// Use this for initialization
	private void Start()
	{
		menuManager = FindObjectOfType<MenuManager>();

		a_forwardButtonSound = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_forwardButtonSound.Event = "event:/uiButton_forward";

		Invoke("GetPlayerOne", .5f);
	}

	private void GetPlayerOne()
	{
		player = GameManager.GetInstance().playerHandlers[0].playerController.gameObject;
	}

	// Update is called once per frame
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == _Tags.player)
		{
			//Is the button is "unready", set it to "ready"
			if (GetComponent<Animator>().GetBool("ReadyUnready") == false)
			{
				GetComponent<Animator>().SetBool("ReadyUnready", true);
				menuManager.anim.SetTrigger(animationToRun);

				a_forwardButtonSound.Play();

				Invoke("TeleportPlayer", 1);
				Invoke("BecomeUnreadyAfterTime", 4);
			}
			else
			{
				GetComponent<Animator>().SetBool("ReadyUnready", false);
			}
		}
	}

	//This should just be done for Player One
	private void TeleportPlayer()
	{
		GameStateManager.GetInstance().currentMainMenuState = GameStateManager.MenuState.levelSelection;

		//Avoid unnecessary NullReferences showing up by checking if they are valid.
		if (player && playerTargetPosition)
		{
			player.transform.position = playerTargetPosition.position;
		}
	}

	//Called
	private void BecomeUnreadyAfterTime()
	{
		GetComponent<Animator>().SetBool("ReadyUnready", false);
	}
}