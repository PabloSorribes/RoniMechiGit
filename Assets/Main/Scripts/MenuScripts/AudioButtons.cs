using UnityEngine;

public class AudioButtons : MonoBehaviour
{
	private AudioManager audioManager;

	public enum ButtonAction { mute, unMute, up, down };
	public ButtonAction audioAction;

	private FMODUnity.StudioEventEmitter a_volumeUpDownSound;

	// Use this for initialization
	private void Start()
	{
		audioManager = AudioManager.GetInstance();
		a_volumeUpDownSound = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
	}

	private void OnTriggerEnter(Collider p_other)
	{
		if (p_other.tag == _Tags.bullet || p_other.tag == _Tags.player)
		{
			AudioMuteAndVolumeChange();
		}
	}

	private void AudioMuteAndVolumeChange()
	{
		float volumeChange = 0.15f;

		//Decide audio change
		switch (audioAction)
		{
			case ButtonAction.mute:
				audioManager.MuteAudio(true);
				break;

			case ButtonAction.unMute:
				audioManager.MuteAudio(false);
				break;

			case ButtonAction.up:
				audioManager.SetAudioVolume(volumeChange);
				a_volumeUpDownSound.Event = "event:/uiButton_volumeUp";
				a_volumeUpDownSound.Play();
				break;

			case ButtonAction.down:
				a_volumeUpDownSound.Event = "event:/uiButton_volumeDown";
				a_volumeUpDownSound.Play();
				audioManager.SetAudioVolume(-volumeChange);
				break;
		}
	}
}