﻿using UnityEngine;

public class Icicle : MonoBehaviour
{
	public GameObject shockwave;

	private FMODUnity.StudioEventEmitter a_icicleIsHit;
	private FMODUnity.StudioEventEmitter a_icicleHitGround;

	// Use this for initialization
	private void Start()
	{
		InitializeAudio();
	}

	private void InitializeAudio()
	{
		a_icicleIsHit = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_icicleIsHit.Event = "event:/hazard_icicle_isHit";

		a_icicleHitGround = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_icicleHitGround.Event = "event:/hazard_icicle_hitGround";
	}

	private void OnTriggerStay(Collider p_collide)
	{
		if (p_collide.tag == "Player")
		{
			a_icicleIsHit.Play();

			GameObject newEffect = Instantiate(shockwave, this.transform.position, this.transform.rotation);
			Destroy(newEffect, 3f);
			p_collide.gameObject.GetComponent<NewPlayerController>().playerHandler.KillPlayer();

			GetComponent<Rigidbody>().useGravity = true;
		}
		if (p_collide.tag == "Bullet")
		{
			a_icicleIsHit.Play();
			GetComponent<Rigidbody>().useGravity = true;
		}
		if (p_collide.tag == "Ground")
		{
			a_icicleHitGround.Play();

			GetComponent<Rigidbody>().isKinematic = true;
			transform.SetParent(p_collide.transform);
			GetComponent<CapsuleCollider>().enabled = false;
			Invoke("DestroyIcicleAfterTime", 2.0f);
		}
	}

	private void DestroyIcicleAfterTime()
	{
		Destroy(this.gameObject);
	}
}