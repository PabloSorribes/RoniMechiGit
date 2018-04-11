using UnityEngine;

public class SpinHand : MonoBehaviour
{
	public float speed = 10f;

	private void Update()
	{
		transform.Rotate(Vector3.up, speed * Time.deltaTime);
	}
}