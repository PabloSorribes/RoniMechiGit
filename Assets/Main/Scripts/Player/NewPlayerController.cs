﻿using FMODUnity;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class NewPlayerController : MonoBehaviour
{
	internal PlayerHandler playerHandler;
	internal Shooting myShooter;
	internal Rigidbody playerRigidbody;
	private Animator playerAnimator;

	internal string currentPlayer;

	public enum PlayerState { idle, running, jumping, inAir }
	[Header("--- MAIN STATS ---")]
	public PlayerState currentStatePlayer;

	public bool canMove = true;
	internal bool isGrounded;
	public Renderer theBodyColor;
	public Renderer deflectionSphereColor;

	[Header("--- MOVEMENT ---")]
	public float moveSpeed;

	public float moveSpeedAirborne;
	public float jumpVelocity;
	private int jumpsRemaining = 2;

	internal float horizontalAxis;
	internal float verticalAxis;
	private Vector3 lastGroundedMovementDirection;
	private GameObject movingPlatform;

	[Header("--- GRAVITY ---")]
	//Used in the BetterFall-function
	[SerializeField]
	private float fallMultiplier = 3f;
	[SerializeField] private float lowJumpMultiplier = 2f;

	[Header("---PARTICLES---")]
	public ParticleSystem ps_spawnExplosion;
	public ParticleSystem ps_deathExplosion;
	public ParticleSystem ps_deathPlasma;

	#region Audio
	private StudioEventEmitter a_spawn;
	private StudioEventEmitter a_death;
	private StudioEventEmitter a_jump;
	private StudioEventEmitter a_runningLoop;
	#endregion

	private void Start()
	{
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

		//This is a hack. Has to be activated exactly 0.2 or more seconds after spawning for it to work. Don't know why.
		Invoke("MakePlayerVisibleInMenu", .2f);
	}

	/// <summary>
	/// For some reason the player is invisible until it makes a jump when in the start menu. This is a hack.
	/// </summary>
	void MakePlayerVisibleInMenu()
	{
		if (GameStateManager.GetInstance().currentGameState == GameStateManager.GameState.inMenu)
			ChooseJump();
	}

	private void ChangePlayerColor()
	{
		Color _color = new Color();

		if (currentPlayer == "_P1")
			_color = Color.blue;

		if (currentPlayer == "_P2")
			_color = Color.red;

		if (currentPlayer == "_P3")
			_color = Color.yellow;

		if (currentPlayer == "_P4")
			_color = Color.black;

		theBodyColor.materials[1].color = _color;
		theBodyColor.materials[4].color = _color;
		deflectionSphereColor.materials[0].color = _color;
	}

	private void InitializeSounds()
	{
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
	private void PlayStopSound(StudioEventEmitter p_fmodComponent, bool p_playStop)
	{
		if (p_playStop)
			p_fmodComponent.Play();
		else
			p_fmodComponent.Stop();
	}

	private void SpawnParticles()
	{
		Destroy(Instantiate(ps_spawnExplosion.gameObject, this.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up)) as GameObject, 1.5f);
	}

	// Update is called once per frame
	private void Update()
	{
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
	private void NoSleepingRigidbody()
	{
		if (playerRigidbody.IsSleeping())
			playerRigidbody.WakeUp();
	}

	/// <summary>
	/// Save the current values of the X-/Y-Axis on the Left Thumbstick of the selected controller.
	/// </summary>
	private void GetAxisValues()
	{
		horizontalAxis = CrossPlatformInputManager.GetAxis(_Inputs.horizontalAxis + currentPlayer);
		verticalAxis = CrossPlatformInputManager.GetAxis(_Inputs.verticalAxis + currentPlayer);
	}

	private void FixedUpdate()
	{
		BetterFall();
	}

	/// <summary>
	/// State-machine for choosing what the character should do, based on inputs.
	/// </summary>
	private void CharacterMovement()
	{
		//If the player is on the ground and is not moving, it is Idle
		if (isGrounded && horizontalAxis == 0)
		{
			PlayStopSound(a_runningLoop, false);
			Movement(horizontalAxis);
			currentStatePlayer = PlayerState.idle;
		}

		//The player can only move if it is on the ground, no walking in the air!
		if (isGrounded && horizontalAxis != 0 && canMove)
		{
			if (!a_runningLoop.IsPlaying())
				PlayStopSound(a_runningLoop, true);

			Movement(horizontalAxis);
			currentStatePlayer = PlayerState.running;
		}

		//If the player presses the A button, make the character jump!
		if (CrossPlatformInputManager.GetButtonDown(_Inputs.jumpButton + currentPlayer) && canMove)
		{
			ChooseJump();
			currentStatePlayer = PlayerState.jumping;
		}

		//TODO: Add statement to see if you've been hit by something/are falling, or if you are jumping by yourself.
		if (!isGrounded)
		{
			PlayStopSound(a_runningLoop, false);
			currentStatePlayer = PlayerState.inAir;
		}

		if (!isGrounded && myShooter.shootingState == Shooting.ShootingState.aiming)
		{
			AirMovementWhileAiming();
		}
		else if (!isGrounded && canMove)
		{
			Movement(horizontalAxis, isGrounded);
		}
	}

	/// <summary>
	/// Left/right movement based on "<paramref name="p_inputDirection"/>". Set "<paramref name="isGrounded"/>" to FALSE for AirControl.
	/// </summary>
	/// <param name="p_inputDirection"></param>
	private void Movement(float p_inputDirection, bool isGrounded = true)
	{
		///** MOVEMENT: LEFT **///
		//if the player is pressing left on the left stick, move left
		if (p_inputDirection < 0)
		{
			//move left at the movementspeed
			transform.position += Vector3.left * moveSpeed * Time.deltaTime;

			//Rotate the character to face in the direction it is moving
			transform.rotation = Quaternion.LookRotation(Vector3.left);

			if (isGrounded)
			{
				//Vector for flying and aiming at the same time.
				lastGroundedMovementDirection = Vector3.left;
			}
		}

		///** MOVEMENT: RIGHT **///
		if (p_inputDirection > 0)
		{
			transform.position += Vector3.right * moveSpeed * Time.deltaTime;
			transform.rotation = Quaternion.LookRotation(Vector3.right);

			if (isGrounded)
			{
				lastGroundedMovementDirection = Vector3.right;
			}
		}

		WalkAnimationsHandler();
	}

	/// <summary>
	/// Chooses what animations to play depending on if the player can move and the current Axis-values.
	/// </summary>
	//TODO: A separate script or a different way to add all the possible Animation Transitions into the same function.
	// These should then be called through: AnimationsHandler().AnimationTransitionName()
	private void WalkAnimationsHandler()
	{
		//if (myShooter.shootingState == Shooting.ShootingState.aiming) {
		//    playerAnimator.SetTrigger("ToShooting");
		//}

		//if the character is standing still, start the idle animation
		if (!canMove)
		{
			playerAnimator.SetTrigger("ToIdle");
		}
		else if (AxisInputForAnimations() < 0.1)
		{
			playerAnimator.SetTrigger("ToIdle");
		}

		//Start the animation for walking, based on X-AxisValues
		playerAnimator.SetFloat("Speed", AxisInputForAnimations());
	}

	/// <summary>
	/// Returns a float with an absolute value, ie. not below zero (0.1 to 1)
	/// </summary>
	/// <returns></returns>
	private float AxisInputForAnimations()
	{
		//By using the absolute value, the trigger condition for running (in the animation controller)
		//is NOT stopped by LeftHorizontal being less than zero (ie. -0.1 to -1)
		float movementInput = Mathf.Abs(Input.GetAxisRaw(_Inputs.horizontalAxis + currentPlayer));

		return movementInput;
	}

	/// <summary>
	/// Chooses if the player should do a "Normal Jump" or a "Double Jump".
	/// </summary>
	private void ChooseJump()
	{
		//if the character is on the ground and has more than 0 jumps remaining
		if (isGrounded && jumpsRemaining > 0)
			JumpUp();

		///** DOUBLE JUMP **///
		//The code for double jumping will execute only if the player is in the air, and has exactly one jump left
		else if (!isGrounded && jumpsRemaining == 1)
			JumpUp();
	}

	/// <summary>
	/// Jump up based on the "jumpVelocity"-variable and remove a jump. Sets "isGrounded" to FALSE and plays a sound.
	/// </summary>
	private void JumpUp()
	{
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

	/// <summary>
	/// Moves in the last inputed direction to allow aiming without activating movement.
	/// </summary>
	private void AirMovementWhileAiming()
	{
		if (lastGroundedMovementDirection == Vector3.left)
		{
			transform.position += Vector3.left * moveSpeedAirborne * Time.deltaTime;
		}
		else if (lastGroundedMovementDirection == Vector3.right)
		{
			transform.position += Vector3.right * moveSpeedAirborne * Time.deltaTime;
		}
	}

	/// <summary>
	/// Sets the isGrounded-variable if the player is on the Ground.
	/// </summary>
	/// <param name="p_other"></param>
	private void OnCollisionEnter(Collision p_other)
	{
		if (p_other.transform.tag == _Tags.ground)
		{
			//Ref platform
			movingPlatform = p_other.gameObject;

			//Stick the player to platform by parenting to it.
			transform.parent = movingPlatform.transform;

			isGrounded = true;
		}
		else if (p_other.transform.tag == _Tags.menuPlatforms || p_other.transform.tag == _Tags.player)
		{
			isGrounded = true;
		}

		if (isGrounded)
		{
			jumpsRemaining = 2;
		}
	}

	/// <summary>
	/// Un-parent the player from any platform and Reset the player scale to its original value
	/// </summary>
	/// <param name="p_collision"></param>
	private void OnCollisionExit(Collision p_collision)
	{
		//Un-parent the player from any platform
		transform.SetParent(null);

		//Reset the player scale to its original value (for when accidentally landing on a non-prepared Default Platform)
		transform.localScale = new Vector3(1, 1, 1);
	}

	/// <summary>
	/// For giving the fall more juicyness.
	/// Should be applied in FixedUpdate().
	/// <para></para> Source: https://youtu.be/7KiK0Aqtmzc
	/// </summary>
	public void BetterFall()
	{
		if (playerRigidbody.velocity.y < 0)
		{
			playerRigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
		}

		//REASON OF REMOVAL: Made the fall waaaay too fast.
		//Add gravity to player.
		//else if (playerRigidbody.velocity.y > 0 && CrossPlatformInputManager.GetButton(_Inputs.jumpButton + currentPlayer))
		//{
		//	playerRigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
		//}
	}

	public void DebugKeyboardInput()
	{
		///** LEFT **///
		if (Input.GetKey(KeyCode.A))
		{
			Movement(-1f);
			playerAnimator.SetFloat("Speed", 0.5f);
		}

		//if the character is standing still, start the idle animation
		if (Input.GetKeyUp(KeyCode.A))
		{
			playerAnimator.SetFloat("Speed", 0.0f);
		}

		///** RIGHT **///
		if (Input.GetKey(KeyCode.D))
		{
			Movement(1f);
			playerAnimator.SetFloat("Speed", 0.5f);
		}

		//if the character is standing still, start the idle animation
		if (Input.GetKeyUp(KeyCode.D))
		{
			playerAnimator.SetFloat("Speed", 0.0f);
		}

		///** JUMP **///
		if (Input.GetKeyDown(KeyCode.W) && jumpsRemaining >= 1)
		{
			JumpUp();
		}

		//If the character has landed, start the idle animation
		if (Input.GetKeyUp(KeyCode.W) && isGrounded)
		{
			playerAnimator.SetTrigger("ToIdle");
		}
	}

	/// <summary>
	/// Called by the playerHandler. Plays deathparticles and a death-sound.
	/// </summary>
	// Called like this instead of through OnDestroy(), to avoid spawning objects OnDestroy (and thus in-between scenes).
	internal void OnKillPlayer()
	{
		DeathParticles();
		PlayStopSound(a_death, true);
	}

	private void DeathParticles()
	{
		Destroy(Instantiate(ps_deathExplosion.gameObject, this.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up)) as GameObject, 2f);
		Destroy(Instantiate(ps_deathPlasma.gameObject, this.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up)) as GameObject, 2f);
	}

	/// <summary>
	/// Since the running sound is a loop which is independent of the object, it has to be turned off in OnDestroy.
	/// </summary>
	private void OnDestroy()
	{
		PlayStopSound(a_runningLoop, false);
	}
}