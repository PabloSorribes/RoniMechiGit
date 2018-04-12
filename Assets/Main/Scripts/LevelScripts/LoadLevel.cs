using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used when choosing map in menu
/// </summary>
public class LoadLevel : MonoBehaviour
{
	public MenuManager menuManager;
	[HideInInspector] public int mapCounter = 0;
	private int beforeCounter = 0;
	public bool playMayanTempel;
	public bool playSpikeyCavern;
	public bool playIcicleStuffs;

	private FMODUnity.StudioEventEmitter a_enterLevelSound;

	private void Start()
	{
		a_enterLevelSound = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_enterLevelSound.Event = "event:/uiButton_forward";
	}

	private void Update()
	{
		//reset counter.
		CounterChecker();

		//Check which map is choosen and have the right animation for it.
		if (mapCounter == 0 && beforeCounter == 2)
		{
			GetComponent<Animator>().SetTrigger("playIStoSC");
			beforeCounter = 0;
		}
		if (mapCounter == 1 && beforeCounter == 0)
		{
			GetComponent<Animator>().SetTrigger("playSCtoMT");
			beforeCounter = 1;
		}
		if (mapCounter == 2 && beforeCounter == 1)
		{
			GetComponent<Animator>().SetTrigger("playMTtoIS");
			beforeCounter = 2;
		}
		if (mapCounter == 0 && beforeCounter == 1)
		{
			GetComponent<Animator>().SetTrigger("playMTtoSC");
			beforeCounter = 0;
		}
		if (mapCounter == 1 && beforeCounter == 2)
		{
			GetComponent<Animator>().SetTrigger("playIStoMT");
			beforeCounter = 1;
		}
		if (mapCounter == 2 && beforeCounter == 0)
		{
			GetComponent<Animator>().SetTrigger("playSCtoIS");
			beforeCounter = 2;
		}
	}

	/// <summary>
	/// When the player jumps into the image of the level.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		a_enterLevelSound.Play();

		if (other.tag == _Tags.player)
		{
			if (mapCounter == 0)
				SceneManager.LoadScene(_Levels.spikeyCavern);
			if (mapCounter == 1)
				SceneManager.LoadScene(_Levels.mayanTemple);
			if (mapCounter == 2)
				SceneManager.LoadScene(_Levels.icicleStuff);
		}
	}

	private void CounterChecker()
	{
		if (mapCounter == 3)
		{
			mapCounter = 0;
		}
		if (mapCounter == -1)
		{
			mapCounter = 2;
		}
	}
}