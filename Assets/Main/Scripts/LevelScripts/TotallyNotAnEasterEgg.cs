using UnityEngine;
using UnityEngine.SceneManagement;

public class TotallyNotAnEasterEgg : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == _Tags.bullet)
		{
			SceneManager.LoadScene(_Levels.elton);
		}
	}
}