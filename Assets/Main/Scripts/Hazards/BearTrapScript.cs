using System.Collections;
using UnityEngine;

public class BearTrapScript : MonoBehaviour
{
	public float deathAnimationTime = 0.5f;
	private bool closed = false;

	public GameObject particleSpark;
	public GameObject leftSideTrap;
	public GameObject rightSideTrap;

	// Use this for initialization
	private void Start()
	{
		leftSideTrap.GetComponent<Animator>();
		rightSideTrap.GetComponent<Animator>();
	}

	private void OnTriggerStay(Collider p_other)
	{
		if (p_other.tag == "Player" && closed == false)
		{
			print(p_other.GetComponent<NewPlayerController>().canMove);
			p_other.GetComponent<NewPlayerController>().canMove = false;
			print(p_other.GetComponent<NewPlayerController>().canMove);

			closed = true;
			leftSideTrap.GetComponent<Animator>().SetTrigger("Close");
			rightSideTrap.GetComponent<Animator>().SetTrigger("Close");
			GameObject newEffect = Instantiate(particleSpark, p_other.transform.position, this.transform.rotation);
			Destroy(newEffect, 3f);

			StartCoroutine(KillOnTime(p_other));

			leftSideTrap.GetComponent<Animator>().SetTrigger("Open");
			rightSideTrap.GetComponent<Animator>().SetTrigger("Open");
			Invoke("WaitforClosed", 2.6f);
		}

		if ((p_other.tag == "Bullet" || p_other.tag == "Icicle") && closed == false)
		{
			closed = true;
			leftSideTrap.GetComponent<Animator>().SetTrigger("Close");
			rightSideTrap.GetComponent<Animator>().SetTrigger("Close");

			leftSideTrap.GetComponent<Animator>().SetTrigger("Open");
			rightSideTrap.GetComponent<Animator>().SetTrigger("Open");
			Invoke("WaitforClosed", 2.6f);
		}
	}

	private void WaitforClosed()
	{
		closed = false;
	}

	private IEnumerator KillOnTime(Collider p_player)
	{
		yield return new WaitForSeconds(deathAnimationTime);

		p_player.gameObject.GetComponent<NewPlayerController>().playerHandler.KillPlayer();
	}
}