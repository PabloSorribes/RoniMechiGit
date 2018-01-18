using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using FMODUnity;

public class Shooting : MonoBehaviour {
	[Header("--- PUBLIC OBJECTS ---")]
	public NewPlayerController playerController;
	MuzzleFlash muzzleflash;
	DeflectionHandler deflectionHandler;

	//For bulletspawn position
	public Transform playerTransform;
	public Rigidbody playerRigidbody;
	public GameObject projectile;
	public GameObject crosshair;

	private bool useEightDirectionAiming;
	private float leftStickX;
	private float leftStickY;
	private Vector3 offsetFromPlayerRoot;

	public enum ShootingState { idle, aiming }
	[Header("--- Current Shooting State ---")]
	public ShootingState shootingState;

	[Header("--- FIRE RATE & AMMO ---")]
	private float fireCooldown = .5f;
	private float fireCooldownTimer = 0;

	[Header("--- RECOIL/PUSHBACK ---")]
	public bool useRecoil = true;
	public float pushbackMultiplierGrounded = 2500;
	public float pushbackMultiplierAirborne = 500;
	public float extraPushbackWhenFalling = 500;

	private StudioEventEmitter a_aim;
	private StudioEventEmitter a_shoot;

	private void Start() {

		muzzleflash = GetComponent<MuzzleFlash>();

		crosshair.SetActive(false);

		offsetFromPlayerRoot = new Vector3(0f, 0.5f, 0f);

		useEightDirectionAiming = false;

		deflectionHandler = playerController.GetComponent<DeflectionHandler>();

		//Subscibe to when the deflection shield is activated, use in hiding the aiming-object.
		deflectionHandler.OnActivateShield += HandleOnShieldActivation;

		InitializeAudio();
	}

	/// <summary>
	/// Called by event in DeflectionHandler, when the deflection shield is activated.
	/// </summary>
	private void HandleOnShieldActivation() {

		StopShootingAndAiming();
	}

	public void StopShootingAndAiming() {

		shootingState = ShootingState.idle;
		crosshair.SetActive(false);
		//print("heyoo");
	}

	private void InitializeAudio() {

		a_aim = gameObject.AddComponent<StudioEventEmitter>();
		a_aim.Event = "event:/Player/Shooting/player_aim";

		a_shoot = gameObject.AddComponent<StudioEventEmitter>();
		a_shoot.Event = "event:/Player/Shooting/player_shoot";
	}

	private void Update() {

		GetShootInput();
		AxisInputDirection();
		UpdateShootPosition();
	}


	private Vector3 AxisInputDirection() {

		//Get references to the thumbstick's values
		leftStickX = CrossPlatformInputManager.GetAxis("Horizontal" + playerController.currentPlayer);
		leftStickY = CrossPlatformInputManager.GetAxis("Vertical" + playerController.currentPlayer);

		//Create the vector when getting input.
		Vector3 axisInput = new Vector3(leftStickX, leftStickY, 0);

		//If there is no input (on the thumbsticks), get the player's lookRotation and set the axisInput to the same direction (left/right).
		if (axisInput == Vector3.zero) {
			axisInput = playerTransform.forward;
		}

		//Clamps aiming to 8 directions
		if (useEightDirectionAiming) {

			float deadZone = 0.1f;
			bool isXInDeadzone;
			bool isYInDeadzone;

			//Get the deadzone-status of the different axises.
			if (Mathf.Abs(axisInput.x) < deadZone) {
				isXInDeadzone = true;
			} else {
				isXInDeadzone = false;
			}

			if (Mathf.Abs(axisInput.y) < deadZone) {
				isYInDeadzone = true;
			} else {
				isYInDeadzone = false;
			}


			//Left
			if (axisInput.x < -deadZone && isYInDeadzone) {
				axisInput = Vector3.left;
			}
			//Left-Up
			if (axisInput.x < -deadZone && axisInput.y > deadZone) {
				axisInput = Vector3.left + Vector3.up;
			}
			//Left-Down
			if (axisInput.x < -deadZone && axisInput.y < -deadZone) {
				axisInput = Vector3.left + Vector3.down;
			}


			//Up
			if (isXInDeadzone && axisInput.y > deadZone) {
				axisInput = Vector3.up;
			}
			//Down
			if (isXInDeadzone && axisInput.y < -deadZone) {
				axisInput = Vector3.down;
			}


			//Right
			if (axisInput.x > deadZone && isYInDeadzone) {
				axisInput = Vector3.right;
			}
			//Right-Up
			if (axisInput.x > deadZone && axisInput.y > deadZone) {
				axisInput = Vector3.right + Vector3.up;
			}
			//Right-Down
			if (axisInput.x > deadZone && axisInput.y < -deadZone) {
				axisInput = Vector3.right + Vector3.down;
			}
		}

		return axisInput.normalized;
	}

	private void UpdateShootPosition() {

		float spawnOffsetDivider = 10f;

		//- Move the BulletSpawn position to the player, with an offset calculated by the Thumbstick's direction.
		// The greater the spawnOffsetDivider → SpawnPosition is closer to the Player
		transform.position = playerTransform.position + (AxisInputDirection() / spawnOffsetDivider) + offsetFromPlayerRoot;

		//- Rotate the object based on the thumbstick direction 
		transform.rotation = Quaternion.LookRotation(AxisInputDirection());
	}


	/// <summary>
	/// Aiming and shooting.
	/// </summary>
	private void GetShootInput() {

		HandleShootCooldownTimer();

		//If the player is not deflecting and can shoot, allow aiming and shooting.
		if (fireCooldownTimer == 0 && deflectionHandler.deflectionState != DeflectionHandler.DeflectionState.deflecting) {

			//When pressing the shoot button.
			if (CrossPlatformInputManager.GetButtonDown("Fire1" + playerController.currentPlayer) || Input.GetKeyDown(KeyCode.Space)) {

				a_aim.Play();
			}

			//While holding the shoot button, show the Crosshair and stop grounded movement.
			if ((CrossPlatformInputManager.GetButton("Fire1" + playerController.currentPlayer) || Input.GetKey(KeyCode.Space)) ) {

				//If it isn't already active, show the Crosshair.
				if (!crosshair.activeSelf) {
					crosshair.SetActive(true);
				}

				shootingState = ShootingState.aiming;

				//If not inAir, you cannot move while holding the shoot button.
				if (playerController.isGrounded) {
					playerController.canMove = false;
				}
			}

			// When releasing the shoot button, hide the crosshair, allow grounded movement, Shoot and Reset the fireCooldownTimer.
			if ((CrossPlatformInputManager.GetButtonUp("Fire1" + playerController.currentPlayer) || Input.GetKeyUp(KeyCode.Space)) ) {

				//Reset CooldownTimer (start counting toward zero so that the player is able to shoot again).
				fireCooldownTimer = fireCooldown;

				shootingState = ShootingState.idle;

				Shoot();
				crosshair.SetActive(false);
				playerController.canMove = true;
			}
		}
	}

	private void HandleShootCooldownTimer() {

		//Count down toward zero.
		if (fireCooldownTimer > 0) {
			fireCooldownTimer -= Time.deltaTime;
		}

		//Sets the timer to zero, ie. it's possible to shoot.
		if (fireCooldownTimer < 0) {
			fireCooldownTimer = 0;
		}
	}

	private void Shoot() {

		a_shoot.Play();

		//- Create a reference for the instantiated projectile.
		Transform instantiatedProjectile = Instantiate(projectile, transform.position, transform.rotation).transform;

		//- Ignore collision with the player that shot the projectile.
		Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), playerController.gameObject.GetComponent<Collider>(), true);

		//AUDIO: Play shoot-sound. Either here OR directly in Start() of the BulletScript?

		//- Begin timer to reset physics collision with the player that shot the projectile.
		//StartCoroutine(ResetBulletPhysics(instantiatedProjectile));

		//- Apply recoil and activate MuzzleFlash.
		Recoil();
		ShootEffect();
	}

	private void ShootEffect() {
		muzzleflash.Activate();
	}

	/// <summary>
	/// DEPRECATED! Use for eventual bouncing from walls. 
	/// <para></para> Resets the collision with the player after a set time.
	/// </summary>
	/// <param name="p_instantiatedProjectile"></param>
	/// <returns></returns>
	IEnumerator ResetBulletPhysics(Transform p_instantiatedProjectile) {

		float timeToResetPhysics = .5f;

		yield return new WaitForSeconds(timeToResetPhysics);

		//Would get a NullRef if the object gets destroyed (by eg. travel distance, collision with objects that destroy the projectile, etc).
		if (p_instantiatedProjectile != null) {
			Physics.IgnoreCollision(p_instantiatedProjectile.GetComponent<Collider>(), playerController.gameObject.GetComponent<Collider>(), false);
		}
	}

	/// <summary>
	/// Recoil from shot.
	/// </summary>
	public void Recoil() {
		if (useRecoil) {

			if (playerController.isGrounded) {
				playerRigidbody.AddForce(-transform.forward * pushbackMultiplierGrounded);
			}
			else {
				//If already falling. 
				//To stop the sick gravity we have.
				if (playerRigidbody.velocity.y < 0f) {
					playerRigidbody.AddForce(-transform.forward * (pushbackMultiplierAirborne + extraPushbackWhenFalling));
				}
				else {
					playerRigidbody.AddForce(-transform.forward * pushbackMultiplierAirborne);
				}
			}
		}
	}
}