using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeflectionScript : MonoBehaviour {

	private FMODUnity.StudioEventEmitter a_deflectShootBack;

	private void Start() {
		a_deflectShootBack = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_deflectShootBack.Event = "event:/Player/Deflect/player_deflect_shootBack";
	}

	//If the bullet is inside the trigger, reverse its movement.
	private void OnTriggerEnter(Collider p_other) {

		if (p_other.tag == _Tags.bullet) {

			a_deflectShootBack.Play();

			//Instantiate a new bullet.
			Transform instantiatedProjectile = Instantiate(p_other.gameObject, p_other.transform.position, p_other.transform.rotation).transform;

			//- Change the bullet's name to the name of the player that deflected it.
			instantiatedProjectile.transform.name = transform.parent.name;

			//- Don't collide with self.
			Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), transform.GetComponent<Collider>(), true);

			//TODO: add random angle (rotation?)
			//- Flip the direction of the bullet
			instantiatedProjectile.forward = -instantiatedProjectile.forward;

			//Destroy the original (incoming) bullet to avoid stacking of bullets and general fuckups.
			Destroy(p_other.gameObject);
		}
	}
}
