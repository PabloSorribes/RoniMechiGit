using UnityEngine;

public class IcicleSpawner : MonoBehaviour
{
	public GameObject icicle;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == _Tags.bullet || other.tag == _Tags.player)
		{
			Invoke("DoThatThingYaWantToDo", 5.0f);
		}
	}

	private void DoThatThingYaWantToDo()
	{
		Instantiate(icicle, transform);
	}
}