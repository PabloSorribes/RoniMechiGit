using UnityEngine;

public class ExitButton : MonoBehaviour
{
	private FMODUnity.StudioEventEmitter a_quitSound;
	private FMODUnity.StudioEventEmitter a_quitSnap;

	public void Start()
	{
		a_quitSound = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_quitSound.Event = "event:/uiButton_Quit";

		a_quitSnap = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
		a_quitSnap.Event = "snapshot:/snapshot_quitGame";
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == _Tags.bullet)
		{
			a_quitSound.Play();
			a_quitSnap.Play();

			Invoke("Quit", .5f);
		}
	}

	private void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		 Application.OpenURL(webplayerQuitURL);
#else
		 Application.Quit();
#endif
	}
}