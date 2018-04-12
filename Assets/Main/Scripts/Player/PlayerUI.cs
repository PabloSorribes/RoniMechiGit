using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	//To get currect amount of health
	private NewPlayerController myPlayer;
	private Canvas gameCanvas;

	[Header("--- Ammo Slider Properties ---")]
	public GameObject healthBarSliderPrefab;
	public Vector3 offset;
	public Vector3 offsetHealthBar;

	private GameObject healthBarSliderHolder;
	private Slider healthBarSliderRef;

	private void Start()
	{
		myPlayer = GetComponent<NewPlayerController>();
		gameCanvas = FindObjectOfType<Canvas>();

		ActivateUI();
	}

	private void ActivateUI()
	{
		//- Instantiate the Slider-prefabs and sets it at the position of the canvas.
		healthBarSliderHolder = Instantiate(healthBarSliderPrefab, FindObjectOfType<Canvas>().transform);
		healthBarSliderHolder.GetComponentInChildren<Text>().text = "unknown";

		//- Set the ammoSliderRef to be able to change its values in Update.
		healthBarSliderRef = healthBarSliderHolder.GetComponent<Slider>();

		//- Text above player's head.
		if (myPlayer.currentPlayer == "_P1")
			healthBarSliderHolder.GetComponentInChildren<Text>().text = "P1";

		if (myPlayer.currentPlayer == "_P2")
			healthBarSliderHolder.GetComponentInChildren<Text>().text = "P2";

		if (myPlayer.currentPlayer == "_P3")
			healthBarSliderHolder.GetComponentInChildren<Text>().text = "P3";

		if (myPlayer.currentPlayer == "_P4")
			healthBarSliderHolder.GetComponentInChildren<Text>().text = "P4";
	}

	private void Update()
	{
		ChooseMyUIPosition();

		//TODO: GUI and alignElement.cs make no sense.

		//- Move the sliders with the player.
		healthBarSliderHolder.transform.position = Camera.main.WorldToScreenPoint(transform.position + offsetHealthBar);

		// Update the value of the sliders
		healthBarSliderRef.value = myPlayer.playerHandler.lifeLeft;
	}

	private void ChooseMyUIPosition()
	{
		int _myIndex = myPlayer.playerHandler.playerIndexRobert;
		int pixelOffset = 50;

		Vector3 _uiPosition;

		if (_myIndex == 0)
		{
			//Up-Left
			//gameCanvas.pixelRect.xMin + pixelOffset

			//healthBarSliderHolder.GetComponent<eageramoeba.DetectResize.alignElement>().gui

			_uiPosition = new Vector3(gameCanvas.pixelRect.xMin + pixelOffset, gameCanvas.pixelRect.yMin + pixelOffset);

			healthBarSliderHolder.transform.position = Camera.main.WorldToScreenPoint(_uiPosition);
		}
		if (_myIndex == 1)
		{
			//Up-Right
		}
		if (_myIndex == 2)
		{
			//Down-Left
		}
		if (_myIndex == 3)
		{
			//Down-Right
		}
	}

	private void OnDestroy()
	{
		DestroyUI();
	}

	public void DestroyUI()
	{
		Destroy(healthBarSliderHolder);
	}
}