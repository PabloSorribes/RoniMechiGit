using UnityEngine;

public class IcicleSpawner : MonoBehaviour
{
	public GameObject icicle;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Bullet" || other.tag == "Player")
		{
			Invoke("DoThatThingYaWantToDo", 5.0f);
		}
	}

	private void DoThatThingYaWantToDo()
	{
		Instantiate(icicle, transform);
	}
}