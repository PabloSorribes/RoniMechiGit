using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public float speedMultiplier;
	public float destroyBulletTime;
	public float pushbackMultiplier = 1000;

	private FMODUnity.StudioEventEmitter a_bulletHitSound;

	public void Start()
	{
		a_bulletHitSound = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();

		Destroy(gameObject, destroyBulletTime);
	}

	private void Update()
	{
		//Move forward.
		transform.position += transform.forward * Time.deltaTime * speedMultiplier;
	}

	private void OnTriggerEnter(Collider p_other)
	{
		if (p_other.tag == _Tags.player)
		{
			//print(p_otherObject.gameObject.name + " was hit.");

			//If the player is not deflecting → apply pushback & play sound.
			if (p_other.GetComponent<DeflectionHandler>().deflectionState != DeflectionHandler.DeflectionState.deflecting)
			{
				p_other.GetComponent<Rigidbody>().AddForce(this.transform.forward * pushbackMultiplier);
				a_bulletHitSound.Event = "event:/Player/Controller/player_hit";
				a_bulletHitSound.Play();

				//Debug.Log(p_otherObject.gameObject.ToString() + " Bullet has hit a player");
				Destroy(this.gameObject);
			}
		}

		if (p_other.tag == _Tags.wall)
		{
			a_bulletHitSound.Event = "event:/Player/Shooting/shotHitWall";
			a_bulletHitSound.Play();
			Destroy(this.gameObject);
		}
	}
}