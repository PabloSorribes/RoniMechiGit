using System.Collections;
using UnityEngine;

public class LavaButton : MonoBehaviour
{
	public GameObject groundToMakeDisappear;

	[SerializeField]
	private FMODUnity.StudioEventEmitter FmodComponent_forwardBackward;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Bullet" || other.tag == "Player")
		{
			groundToMakeDisappear.SetActive(false);
			StartCoroutine("Timelavaplat");

			FmodComponent_forwardBackward.Play();
		}
	}

	private IEnumerator Timelavaplat()
	{
		yield return new WaitForSeconds(1f);
		groundToMakeDisappear.SetActive(true);
	}
}