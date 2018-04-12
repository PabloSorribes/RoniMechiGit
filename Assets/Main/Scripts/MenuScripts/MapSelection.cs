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

	/// <summary>
	/// When the player jumps/shoots into the Left-/Right-buttons in the Level Selection Menu.
	/// </summary>
	/// <param name="p_other"></param>
	private void OnTriggerEnter(Collider p_other)
	{
		if (p_other.tag == _Tags.player || p_other.tag == _Tags.bullet)
		{
			switch (myArrowDirection)
			{
				case ArrowDirection.left:
					levelSelect.mapCounter--;
					break;

				case ArrowDirection.right:
					levelSelect.mapCounter++;
					break;

				default:
					break;
			}

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