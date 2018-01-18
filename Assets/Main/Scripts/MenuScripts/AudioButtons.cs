using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioButtons : MonoBehaviour {

	AudioManager audioManager;

	public enum ButtonAction { mute, unMute, up, down };
	public ButtonAction audioAction;

	private FMODUnity.StudioEventEmitter a_volumeUpDownSound;

	// Use this for initialization
	void Start () {

		audioManager = AudioManager.GetInstance();

		a_volumeUpDownSound = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
	}

	private void OnTriggerEnter(Collider p_other) {

		if (p_other.tag == "Bullet" || p_other.tag == "Player") {

			AudioMuteAndVolumeChange();
		}
	}

	private void AudioMuteAndVolumeChange() {

		float volumeChange = 0.15f;

		//Decide direction
		switch (audioAction) {
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
