using System.Collections;
using UnityEngine;

public class FlameON : MonoBehaviour
{
	public ParticleSystem lavaEmitter;
	private Collider flameCollider;

	private float timeLeftToFlaming;
	private bool activateFlamesOnce = false;

	private FMODUnity.StudioEventEmitter a_fireLoop;

	// Use this for initialization
	private void Start()
	{
		flameCollider = GetComponent<Collider>();
		flameCollider.enabled = false;

		InitializeAudio();

		timeLeftToFlaming = GetTimeToPlayFlames();
	}

	private void InitializeAudio()
	{
		a_fireLoop = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_fireLoop.Event = "event:/hazard_flameOn";
		a_fireLoop.Preload = true;
	}

	private float GetTimeToPlayFlames()
	{
		float timeToPlayFlames = Random.Range(4f, 8f);
		return timeToPlayFlames;
	}

	// Update is called once per frame
	private void Update()
	{
		timeLeftToFlaming -= Time.deltaTime;
		if (timeLeftToFlaming <= 0f)
		{
			//play the particle effect.
			//Has to be in Update() for it to look nice, since the particles aren't spawned correctly in the prefab.
			lavaEmitter.Play();

			//Do once
			if (!activateFlamesOnce)
			{
				float randomInterval = Random.Range(3f, 5f);
				StartCoroutine(PlayFlamesAndStopAfterTime(randomInterval));
			}
		}
	}

	private IEnumerator PlayFlamesAndStopAfterTime(float timeToWait)
	{
		StartPlayingFlames();
		yield return new WaitForSeconds(timeToWait);
		StopPlayingFlames();
	}

	private void StartPlayingFlames()
	{
		activateFlamesOnce = true;

		//enable the box collider of the specific lava emitter.
		flameCollider.enabled = true;

		if (!a_fireLoop.IsPlaying())
			a_fireLoop.Play();
	}

	private void StopPlayingFlames()
	{
		timeLeftToFlaming = GetTimeToPlayFlames();
		activateFlamesOnce = false;

		//when the interval has passed, disable the box collider so players aren't killed when the particles are inactive.
		flameCollider.enabled = false;

		//stop the particle effect.
		lavaEmitter.Stop();

		a_fireLoop.Stop();
	}

	public void OnTriggerEnter(Collider p_other)
	{
		if (p_other.tag == _Tags.player)
		{
			//To the PlayerHandler: Send which gameObject collided with the Hazard.
			p_other.gameObject.GetComponent<NewPlayerController>().playerHandler.KillPlayer();
		}
	}

	private void OnDestroy()
	{
		a_fireLoop.Stop();
	}
}