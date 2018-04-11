using FMODUnity;
using UnityEngine;

public class MapSelection : MonoBehaviour
{
	public enum ArrowDirection { left, right };
	public ArrowDirection myArrowDirection;

	public GameObject mapSelector;
	private LoadLevel levelSelect;

	private StudioEventEmitter a_buttonSound;

	private void Start()
	{
		levelSelect = mapSelector.GetComponent<LoadLevel>();

		a_buttonSound = gameObject.AddComponent<StudioEventEmitter>();
		a_buttonSound.Event = "event:/uiButton_levelSelect";
	}

	//When the player jumps/shoots into the Left-/Right-buttons in the Level Selection Menu.
	private void OnTriggerEnter(Collider p_other)
	{
		if (myArrowDirection == ArrowDirection.left && p_other.tag == "Player")
		{
			levelSelect.mapCounter--;
			ShowLevel();

			a_buttonSound.Play();
		}
		else if (myArrowDirection == ArrowDirection.right && p_other.tag == "Player")
		{
			levelSelect.mapCounter++;
			ShowLevel();

			a_buttonSound.Play();
		}
	}

	private void ShowLevel()
	{
		if (levelSelect.mapCounter == 0 || levelSelect.mapCounter == 3)
			levelSelect.playSpikeyCavern = true;

		if (levelSelect.mapCounter == 1)
			levelSelect.playMayanTempel = true;

		if (levelSelect.mapCounter == 2 || levelSelect.mapCounter == -1)
			levelSelect.playIcicleStuffs = true;
	}
}