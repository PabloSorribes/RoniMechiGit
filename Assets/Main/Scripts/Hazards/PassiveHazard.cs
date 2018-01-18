using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveHazard : MonoBehaviour {
    private GameManager gameManager;
    public float deathAnimationTime;
    public GameObject splashParticleOne;
    public GameObject splashParticleTwo;

    public enum HazardObject { spikes, lava};
    public enum SpikesTopDown { Down, Air};
    public HazardObject thisHazardObject;
    public SpikesTopDown thisSpikesTopDown;

	void Start () {
        gameManager = FindObjectOfType<GameManager>();
    }
	
	void Update () {
		//TODO: Add animations & trigger behaviours here?
	}

    public void OnTriggerEnter(Collider p_collide)
    {
        if (p_collide.tag == "Player") {

            if (thisHazardObject == HazardObject.lava)
            {
                //print("Entered lava pool");
                p_collide.gameObject.GetComponent<NewPlayerController>().playerHandler.KillPlayer();
                GameObject splash1 = Instantiate(splashParticleOne, this.transform.position, this.transform.rotation);
                GameObject splash2 = Instantiate(splashParticleTwo, this.transform.position, this.transform.rotation);
                Destroy(splash1, 5f);
                Destroy(splash2, 5f);
            }

            if (thisHazardObject == HazardObject.spikes)
            {
				//Make the player slide downwards awkardly
				p_collide.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                GameObject splash1 = Instantiate(splashParticleOne, transform);
                splash1.transform.position = p_collide.transform.position;
                splash1.transform.LookAt(Vector3.up*1000);

                StartCoroutine(KillOnTime(p_collide));

				//Make the player follow the falling spikes
                p_collide.transform.SetParent(this.transform);

				//Disable player movement
                p_collide.GetComponent<NewPlayerController>().canMove = false;
                p_collide.GetComponent<Animator>().enabled = false;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (thisHazardObject == HazardObject.spikes && thisSpikesTopDown == SpikesTopDown.Down)
            {
                other.transform.position += Vector3.down / 100;
            }
        }
    }

    IEnumerator KillOnTime(Collider p_player) {

		bool instantKill = false;

		if (!instantKill) {

			yield return new WaitForSeconds(1f);
			if(p_player != null)
			p_player.gameObject.GetComponent<NewPlayerController>().playerHandler.KillPlayer();
		}
		else {
			p_player.gameObject.GetComponent<NewPlayerController>().playerHandler.KillPlayer();
		}
    }
}

