using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using FMODUnity;

public class NewPlayerController : MonoBehaviour {

	[HideInInspector] public PlayerHandler playerHandler;
	
	[HideInInspector] public Shooting myShooter;
	[HideInInspector] public Rigidbody playerRigidbody;
	private Animator playerAnimator;

	[HideInInspector] public string currentPlayer;

	public enum PlayerState { idle, running, jumping, inAir }
	[Header("--- MAIN STATS ---")]
	public PlayerState currentStatePlayer;

	public bool canMove = true;
	[HideInInspector] public bool isGrounded;
	public Renderer theBodyColor;

	[Header("--- MOVEMENT ---")]
	public float moveSpeed;
	public float moveSpeedAirborne;
	public float jumpVelocity;
	private int jumpsRemaining = 2;

	[HideInInspector] public float horizontalAxis;
	[HideInInspector] public float verticalAxis;
	private Vector3 movementDirection;
	private GameObject movingPlatform;

	[Header("--- GRAVITY ---")]
	//Used in the BetterFall-function
	[SerializeField] private float fallMultiplier = 3f;
	[SerializeField] private float lowJumpMultiplier = 2f;


	[Header("---PARTICLES---")]
	public ParticleSystem ps_spawnExplosion;
	public ParticleSystem ps_deathExplosion;
	public ParticleSystem ps_deathPlasma;


	/// <summary>
	/// AUDIO
	/// </summary>
	private StudioEventEmitter a_spawn;
	private StudioEventEmitter a_death;
	private StudioEventEmitter a_jump;
	private StudioEventEmitter a_runningLoop;

	void Start() {
		myShooter = GetComponentInChildren<Shooting>();
		playerAnimator = GetComponent<Animator>();
		playerRigidbody = GetComponent<Rigidbody>();
		playerAnimator.applyRootMotion = false;

		currentStatePlayer = PlayerState.idle;

		//Rename the player object to which controller it is assigned to.
		this.name = "Player_RoniMechi" + currentPlayer;

		ChangePlayerColor();

		InitializeSounds();
		PlayStopSound(a_spawn, true);

		SpawnParticles();
	}

	private void ChangePlayerColor() {

		Color _color = new Color();

		if (currentPlayer == "_P1") {
			_color = Color.blue;
		}

		if (currentPlayer == "_P2") {
			_color = Color.red;
		}

		if (currentPlayer == "_P3") {
			_color = Color.yellow;
		}

		if (currentPlayer == "_P4") {
			_color = Color.black;
		}

		theBodyColor.materials[1].color = _color;
		theBodyColor.materials[4].color = _color;
	}

	private void InitializeSounds() {

		a_spawn = gameObject.AddComponent<StudioEventEmitter>();
		a_spawn.Event = "event:/Player/Controller/player_spawn";

		a_death = gameObject.AddComponent<StudioEventEmitter>();
		a_death.Event = "event:/Player/Controller/player_death";

		a_jump = gameObject.AddComponent<StudioEventEmitter>();
		a_jump.Event = "event:/Player/Controller/player_jump";

		a_runningLoop = gameObject.AddComponent<StudioEventEmitter>();
		a_runningLoop.Event = "event:/Player/Controller/player_runningLoop";
	}

	/// <summary>
	/// Either play or stop the inserted StudioEventEmitter-component.
	/// <para></para>
	/// TRUE = Play sound.
	/// <para></para> FALSE = Stop the sound.
	/// </summary>
	/// <param name="p_fmodComponent"></param>
	private void PlayStopSound(StudioEventEmitter p_fmodComponent, bool p_playStop) {

		if (p_playStop) {
			p_fmodComponent.Play();
		}
		else {
			p_fmodComponent.Stop();
		}
	}

	void SpawnParticles() {

		Destroy(Instantiate(ps_spawnExplosion.gameObject, this.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up)) as GameObject, 1.5f);
	}



	// Update is called once per frame
	void Update() {

		NoSleepingRigidbody();

		GetAxisValues();
		CharacterMovement();

		DebugKeyboardInput();

		//DEBUG:
		//print("Current State of Player: " + currentStatePlayer);
		//print("Current jumps left: " + jumpsRemaining);
	}

	/// <summary>
	/// Avoids the rigidbody from falling asleep and fucking with collision n shiz. Should run in Update().
	/// </summary>
	private void NoSleepingRigidbody() {

		if (playerRigidbody.IsSleeping()) {
			playerRigidbody.WakeUp();
		}
	}

	/// <summary>
	/// Save the current values of the X-/Y-Axis on the Left Thumbstick of the selected controller.
	/// </summary>
	private void GetAxisValues() {

		horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal" + currentPlayer);
		verticalAxis = CrossPlatformInputManager.GetAxis("Vertical" + currentPlayer);
	}

	private void FixedUpdate() {
		BetterFall();
	}

	public void CharacterMovement() {
		
		//If the player is on the ground and is not moving, it is Idle
		if (isGrounded && horizontalAxis == 0) {

			PlayStopSound(a_runningLoop, false);
			//print("Stopped run audio");
			CharacterWalk(horizontalAxis);
			currentStatePlayer = PlayerState.idle;
		}

		//The player can only move if it is on the ground, no walking in the air!
		if (isGrounded && horizontalAxis != 0 && canMove) {

			if (!a_runningLoop.IsPlaying()) {
				PlayStopSound(a_runningLoop, true);
				//print("Playing run audio");
			}
			
			CharacterWalk(horizontalAxis);
			currentStatePlayer = PlayerState.running;
		}

		//If the player presses the A button, make the character jump!
		if (CrossPlatformInputManager.GetButtonDown("Jump" + currentPlayer) && canMove) {
			CharacterJump();
			//JumpSound();
			currentStatePlayer = PlayerState.jumping;
		}

		//TODO: Add statement to see if you've been hit by something/are falling, or if you are jumping by yourself.
		if (!isGrounded) {

			PlayStopSound(a_runningLoop, false);
			currentStatePlayer = PlayerState.inAir;
		}

		if (!isGrounded && myShooter.shootingState == Shooting.ShootingState.aiming) {
			MovementInAir();
		}
		else if (!isGrounded && canMove) {
			AirControl(horizontalAxis);
		}
	}

	public void CharacterWalk(float p_inputDirection) {
		///** MOVEMENT: LEFT **///
		//if the player is pressing left on the left stick, move left
		if (p_inputDirection < 0) {

			//Vector for flying and aiming at the same time.
			movementDirection = Vector3.left;

			//move left at the movementspeed
			transform.position += Vector3.left * moveSpeed * Time.deltaTime;
			
			//Rotate the character to face in the direction it is moving
			transform.rotation = Quaternion.LookRotation(Vector3.left);
		}

		///** MOVEMENT: RIGHT **///
		if (p_inputDirection > 0) {
			movementDirection = Vector3.right;
			transform.position += Vector3.right * moveSpeed * Time.deltaTime;
			transform.rotation = Quaternion.LookRotation(Vector3.right);
		}

		WalkAnimationsHandler();
	}
	
	//TODO: A separate script or a different way to add all the possible Animation Transitions into the same function.
	// These should then be called through: AnimationsHandler().AnimationTransitionName()
	public void WalkAnimationsHandler() {

		//if (myShooter.shootingState == Shooting.ShootingState.aiming) {
		//    playerAnimator.SetTrigger("ToShooting");
		//}

		//if the character is standing still, start the idle animation
		if (!canMove)
		{
			playerAnimator.SetTrigger("ToIdle");
		}
		else if (AxisInputForAnimations() < 0.1) {
			playerAnimator.SetTrigger("ToIdle");
		}

		//Start the animation for walking, based on X-AxisValues
		playerAnimator.SetFloat("Speed", AxisInputForAnimations());
	}

	public float AxisInputForAnimations() {

		//By using the absolute value, the trigger condition for running (in the animation controller) 
		//is NOT stopped by LeftHorizontal being less than zero (ie. -0.1 to -1)
		float movementInput = Mathf.Abs(Input.GetAxisRaw("Horizontal" + currentPlayer));

		return movementInput; 
	}

	public void CharacterJump() {
		//if the character is on the ground and has more than 0 jumps remaining
		if (isGrounded && jumpsRemaining > 0) {

			JumpAllDirections(horizontalAxis);
		}
	   
		///** DOUBLE JUMP **///
		//The code for double jumping will execute only if the player is in the air, and has exactly one jump left
		else if (!isGrounded && jumpsRemaining == 1) {

			JumpAllDirections(horizontalAxis);
		}

	}

	private void JumpAllDirections(float p_inputDirection) {

		//Start the animation for jumping
		playerAnimator.SetTrigger("ToJump");

		//Jump Up
		playerRigidbody.velocity = Vector3.up * jumpVelocity;

		//The player is no longer on the ground
		isGrounded = false;

		//Remove one jump from the jumps remaining
		jumpsRemaining--;

		PlayStopSound(a_jump, true);
	}

	private void AirControl(float p_inputDirection) {

		///** MOVEMENT: LEFT **///
		//if the player is pressing left on the left stick, move left
		if (p_inputDirection < 0) {

			//move left at the movementspeed
			transform.position += Vector3.left * moveSpeedAirborne * Time.deltaTime;

			//Rotate the character to face in the direction it is moving
			transform.rotation = Quaternion.LookRotation(Vector3.left);
		}

		///** MOVEMENT: RIGHT **///
		if (p_inputDirection > 0) {
			transform.position += Vector3.right * moveSpeedAirborne * Time.deltaTime;
			transform.rotation = Quaternion.LookRotation(Vector3.right);
		}
	}

	private void MovementInAir() {

		if (movementDirection == Vector3.left) {
			transform.position += Vector3.left * moveSpeedAirborne * Time.deltaTime;
		}
		else if (movementDirection == Vector3.right) {
			transform.position += Vector3.right * moveSpeedAirborne * Time.deltaTime;
		}
	}

	//Sets the isGrounded-variable if the player is on the Ground
	public void OnCollisionEnter(Collision p_collision) {

		if (p_collision.transform.tag == "Ground") {
			
			//Ref platform
			movingPlatform = p_collision.gameObject;

			//Stick the player to platform by parenting to it.
			transform.parent = movingPlatform.transform;

			isGrounded = true;
		} 

		else if (p_collision.transform.tag == "MenuPlatforms") {
			isGrounded = true;
		}

		if (isGrounded) {

			jumpsRemaining = 2;
		}
	}

	public void OnCollisionExit(Collision p_collision) {

		//Re-parent the player to its PlayerHandler.
		//transform.SetParent(playerHandler.transform, true);

		//Un-parent the player from any platform
		transform.SetParent(null);

		//Reset the player scale to its original value (for when accidentally landing on a non-prepared Default Platform)
		transform.localScale = new Vector3(1, 1, 1);
	}

	/// <summary>
	/// For giving the fall more juicyness.
	/// Should be applied in Update() or FixedUpdate().
	/// <para></para> Source: https://youtu.be/7KiK0Aqtmzc
	/// </summary>
	public void BetterFall() {
		if (playerRigidbody.velocity.y < 0)
		{
			playerRigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
		}
		//Reason of Removal: It added gravity to player 3 only.
		//UPDATE: See if this works now.
		//else if (playerRigidbody.velocity.y > 0 && CrossPlatformInputManager.GetButton("Jump" + currentPlayer)) {
		//	playerRigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
		//}
	}

	public void DebugKeyboardInput() {

		///** LEFT **///
		if (Input.GetKey(KeyCode.A)) {
			CharacterWalk(-1f);
			playerAnimator.SetFloat("Speed", 0.5f);
		}

		//if the character is standing still, start the idle animation
		if (Input.GetKeyUp(KeyCode.A)) {
			playerAnimator.SetFloat("Speed", 0.0f);
		}

		///** RIGHT **///
		if (Input.GetKey(KeyCode.D)) {
			CharacterWalk(1f);
			playerAnimator.SetFloat("Speed", 0.5f);
		}

		//if the character is standing still, start the idle animation
		if (Input.GetKeyUp(KeyCode.D)) {
			playerAnimator.SetFloat("Speed", 0.0f);
		}

		///** JUMP **///
		if (Input.GetKeyDown(KeyCode.W) && jumpsRemaining >= 1) {
			JumpAllDirections(0f);
		}

		//If the character has landed, start the idle animation
		if (Input.GetKeyUp(KeyCode.W) && isGrounded) {
			playerAnimator.SetTrigger("ToIdle");
		}
	}

	private void OnDestroy() {

		DeathParticles();

		PlayStopSound(a_runningLoop, true);
		PlayStopSound(a_death, true);
	}

	void DeathParticles() {

		Destroy(Instantiate(ps_deathExplosion.gameObject, this.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up)) as GameObject, 2f);
		Destroy(Instantiate(ps_deathPlasma.gameObject, this.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up)) as GameObject, 2f);
	}
}
