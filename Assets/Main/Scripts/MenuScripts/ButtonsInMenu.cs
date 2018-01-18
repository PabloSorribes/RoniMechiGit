using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsInMenu: MonoBehaviour {
    public Transform playerTargetPosition;
    public string animationToRun;
    public string secondAnimationToRun;

	//Creates field in inspector. Drag FmodStudioEventEmitter-component on it. Set event in the component.
	[SerializeField]
	private FMODUnity.StudioEventEmitter FmodComponent_forwardBackward;

	GameStateManager gsManager;

    MenuManager menuManager;
    GameObject player;

    public void Start() {
        menuManager = FindObjectOfType<MenuManager>();

        gsManager = GameStateManager.GetInstance();

        Invoke("GetPlayerOne", 1f);

		FmodComponent_forwardBackward = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    //TODO: Improve the getting of the player
    void GetPlayerOne() {
        player = GameManager.GetInstance().playerHandlers[0].playerController.gameObject;
    }

    private void OnTriggerEnter(Collider other) {

        //Animate camera, teleport player and update the Menu Status
        if (other.tag == "Bullet" || other.tag == "Player") {

            menuManager.anim.SetTrigger(animationToRun);
            player.transform.position = playerTargetPosition.position;

            UpdateMenuStatus(animationToRun);

			FmodComponent_forwardBackward.Play();
		}

    }

    //TODO: Add statements for each animation that moves the camera to a different state.
    void UpdateMenuStatus(string p_animationToRun) {
        if (p_animationToRun == "BackPSelect") {
            gsManager.currentMainMenuState = GameStateManager.MenuState.start;
        }

        //Players can join the game
        else if (p_animationToRun == "toPlayerSelection" || p_animationToRun == "BackLSelect") {
            gsManager.currentMainMenuState = GameStateManager.MenuState.playerSelection;
        }
    }
}