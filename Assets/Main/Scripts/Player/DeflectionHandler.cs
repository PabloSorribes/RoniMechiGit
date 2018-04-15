using FMODUnity;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class DeflectionHandler : MonoBehaviour
{
	public enum DeflectionState { idle, deflecting }
	[Header("--- Current Shooting State ---")]
	public DeflectionState deflectionState;

	private NewPlayerController playerController;
	public GameObject deflectionSphere;
	public GameObject spotlights;
	public GameObject star;
	public ParticleSystem sphereSpawnEffect;

	private float deflectionCounter;
	public float deflectionCooldown = 1f;
	public float timeToTurnOffShield = 0.5f;

	private StudioEventEmitter a_deflectSpawn;
	private StudioEventEmitter a_deflectReloaded;
	private bool stopReloadedSound;

	public System.Action OnActivateShield;

	private bool CanDeflect
	{
		get
		{
			bool canDeflect;
			if (deflectionCounter >= deflectionCooldown)
				canDeflect = true;
			else
				canDeflect = false;

			return canDeflect;
		}
	}

	private string DeflectionButton
	{
		get
		{
			string deflectionButton = _Inputs.deflectionButton + playerController.currentPlayer;
			return deflectionButton;
		}
	}

	// Use this for initialization
	private void Start()
	{
		playerController = GetComponent<NewPlayerController>();
		deflectionSphere.SetActive(false);

		//To be able to use Deflect on start.
		deflectionCounter = deflectionCooldown;

		InitializeAudio();
	}

	private void InitializeAudio()
	{
		a_deflectSpawn = gameObject.AddComponent<StudioEventEmitter>();
		a_deflectSpawn.Event = "event:/Player/Deflect/player_deflect_spawn";

		a_deflectReloaded = gameObject.AddComponent<StudioEventEmitter>();
		a_deflectReloaded.Event = "event:/Player/Deflect/player_deflect_reloaded";
	}

	// Update is called once per frame
	private void Update()
	{
		GetDeflectionInput();
		LoadUpGraphics();
	}

	/// <summary>
	/// Checks for Deflection-button input. 
	/// If the player is allowed to deflect, call functions to spawn a Deflection Sphere and so on.
	/// </summary>
	private void GetDeflectionInput()
	{
		//Count continously upwards. Is reset when activating deflection shield.
		deflectionCounter += Time.deltaTime;

		//Activate deflection shield.
		if (CrossPlatformInputManager.GetButtonDown(DeflectionButton) || Input.GetKeyDown(KeyCode.F))
		{
			//Check if you can activate the shield or not.
			if (CanDeflect)
			{
				deflectionState = DeflectionState.deflecting;

				deflectionCounter = 0;

				ShowDeflectionSphere();
			}
		}
	}

	private void ShowDeflectionSphere()
	{
		//Function call for Shooting.cs to hook up into.
		OnActivateShield();

		//Audio
		a_deflectSpawn.Play();

		//UX-Graphics
		star.SetActive(false);
		spotlights.SetActive(false);

		//Activate shield
		deflectionSphere.SetActive(true);

		//Particles
		Destroy(Instantiate(sphereSpawnEffect.gameObject, deflectionSphere.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up)) as GameObject, 2f);

		//Turn off shield in x seconds.
		Invoke("HideDeflectionSphere", timeToTurnOffShield);
	}

	private void HideDeflectionSphere()
	{
		a_deflectSpawn.Stop();

		deflectionSphere.SetActive(false);

		deflectionState = DeflectionState.idle;
	}

	/// <summary>
	/// UI for when the player can deflect again.
	/// </summary>
	private void LoadUpGraphics()
	{
		if (CanDeflect)
		{
			if (!star.activeSelf)
			{
				star.SetActive(true);
				spotlights.SetActive(true);
			}

			//"Do once"-bool condition, based on ability to activate the shield or not.
			if (stopReloadedSound == false)
			{
				a_deflectReloaded.Play();
				stopReloadedSound = true;
			}
		}
		else
		{
			stopReloadedSound = false;
		}
	}

	private void OnDestroy()
	{
		a_deflectSpawn.Stop();
	}
}