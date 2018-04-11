using System.Collections;
using UnityEngine;

public class FlameON : MonoBehaviour
{
	public ParticleSystem lavaEmitter;
	private Collider flameCollider;

	private float timeLeftToFlaming;
	private bool playFlames = false;

	private float time;
	public float emitterNumber;

	private FMODUnity.StudioEventEmitter a_fireLoop;

	// Use this for initialization
	private void Start()
	{
		time = 0;
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
		//print("Time to start flames: " + timeLeftToFlaming);
		print("Play flames: " + playFlames);
		if (timeLeftToFlaming <= 0f)
		{
			//play the particle effect. 
			//Has to be in Update for it to look nice, since the particles aren't spawned correctly in the prefab. 
			lavaEmitter.Play();

			//Do once
			if (!playFlames)
			{
				float randomInterval = Random.Range(3f, 5f);
				StartCoroutine(PlayFlamesAndStopAfterTime(randomInterval));
			}
		}

		//OldFunction();
	}

	private IEnumerator PlayFlamesAndStopAfterTime(float timeToWait)
	{
		StartPlayingFlames();
		yield return new WaitForSeconds(timeToWait);
		StopPlayingFlames();
	}

	private void StartPlayingFlames()
	{
		print("Flame on!");

		playFlames = true;

		//enable the box collider of the specific lava emitter.
		flameCollider.enabled = true;

		//play the particle effect.
		lavaEmitter.Play();

		if (!a_fireLoop.IsPlaying())
			a_fireLoop.Play();
	}

	private void StopPlayingFlames()
	{
		print("Stop flaming.");

		timeLeftToFlaming = GetTimeToPlayFlames();
		playFlames = false;

		//when the interval has passed, disable the box collider so players aren't killed when the particles are inactive.
		flameCollider.enabled = false;
		//stop the particle effect.
		lavaEmitter.Stop();

		a_fireLoop.Stop();
	}

	private void OldFunction()
	{
		//The int time is counted by each lava emitter and depending on what number the emitter has, it will activate its collider and spawn fire at
		//different intervals.
		if (time > (500 * emitterNumber) && time < (800 * emitterNumber))
		{
			//enable the box collider of the specific lava emitter.
			flameCollider.enabled = true;
			//play the particle effect.
			lavaEmitter.Play();
			//print("Burn!");

			if (!a_fireLoop.IsPlaying())
			{
				a_fireLoop.Play();
			}
		}
		else if (time > (800 * emitterNumber))
		{
			//when the interval has passed, disable the box collider so players aren't killed when the particles are inactive.
			flameCollider.enabled = false;
			//stop the particle effect.
			lavaEmitter.Stop();

			a_fireLoop.Stop();
		}

		//100 frames after the last particle effect has stopped, reset the timing to 0 and start over.
		if (time >= 2500)
		{
			time = 0;
		}

		time++;
	}

	public void OnTriggerEnter(Collider p_collide)
	{
		if (p_collide.gameObject.tag == "Player")
		{
			//To the PlayerHandler: Send which gameObject collided with the Hazard.
			p_collide.gameObject.GetComponent<NewPlayerController>().playerHandler.KillPlayer();
		}
	}

	private void OnDestroy()
	{
		a_fireLoop.Stop();
	}
}