using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReady : MonoBehaviour {
	MenuManager menuManager;

	//Is set in the inspector to move PLAYER ONE to the next place in the menu.
	public Transform playerTargetPosition;
	GameObject player;
	public string animationToRun;

	private FMODUnity.StudioEventEmitter FmodComponent_forwardBackward;

	// Use this for initialization
	void Start() {
		menuManager = FindObjectOfType<MenuManager>();
		//player = GameObject.FindGameObjectWithTag("Player");

		FmodComponent_forwardBackward = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		FmodComponent_forwardBackward.Event = "event:/uiButton_forward";

		Invoke("GetPlayerOne", .5f);

	}

	void GetPlayerOne() {

		player = GameManager.GetInstance().playerHandlers[0].playerController.gameObject;
	}

	// Update is called once per frame
	public void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			//Is the button is "unready", set it to "ready"
			if (GetComponent<Animator>().GetBool("ReadyUnready") == false) {

				GetComponent<Animator>().SetBool("ReadyUnready", true);
				menuManager.anim.SetTrigger(animationToRun);

				FmodComponent_forwardBackward.Play();

				Invoke("TeleportPlayer", 1);
				Invoke("BecomeUnreadyAfterTime", 4);
			}

			else {

				GetComponent<Animator>().SetBool("ReadyUnready", false);
			}
		}
	}

	//This should just be done for Player One
	private void TeleportPlayer() {
		GameStateManager.GetInstance().currentMainMenuState = GameStateManager.MenuState.levelSelection;

		//Avoid unnecessary NullReferences showing up by checking if they are valid.
		if (player && playerTargetPosition) {

			player.transform.position = playerTargetPosition.position;
		}
	}

	//Called
	private void BecomeUnreadyAfterTime() {
		GetComponent<Animator>().SetBool("ReadyUnready", false);
	}
}