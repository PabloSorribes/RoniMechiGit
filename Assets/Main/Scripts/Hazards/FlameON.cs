using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameON : MonoBehaviour
{
    private GameManager gameManager;
    public ParticleSystem lavaEmitter;
    public int time;
    public int emitterNumber;
    public bool isFlame;
    Collider flameCollider;

	private FMODUnity.StudioEventEmitter a_fireLoop;
	bool dontPlayAudio;

    // Use this for initialization
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        time = 0;
        flameCollider = GetComponent<Collider>();
        flameCollider.enabled = false;

		a_fireLoop = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_fireLoop.Event = "event:/hazard_flameOn";

	}

    // Update is called once per frame
    void Update()
    {
        //The int time is counted by each lava emitter and depending on what number the emitter has, it will activate its collider and spawn fire at 
        //different intervals. 
        if (time > (500*emitterNumber) && time < (800*emitterNumber))
        {
            //enable the box collider of the specific lava emitter.
            flameCollider.enabled = true;
            //play the particle effect.
            lavaEmitter.Play();
			//print("Burn!");

			if (!a_fireLoop.IsPlaying()) {
				a_fireLoop.Play();
			}
        }
      
        else if (time > (800*emitterNumber))
        {
            //when the interval has passed, disable the box collider so players aren't killed when the particles are inactive.
            flameCollider.enabled = false;
            //stop the particle effect.
            lavaEmitter.Stop();

			a_fireLoop.Stop();
        }

        //100 frames after the last particle effect has stopped, reset the timing to 0 and start over.
        if(time >= 2500)
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

	private void OnDestroy() {
		a_fireLoop.Stop();
	}
}