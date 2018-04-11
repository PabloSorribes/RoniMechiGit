using UnityEngine;

//Used by all scripts in the Menu to get references for stuff.
public class MenuManager : MonoBehaviour
{
	[HideInInspector] public Animator anim;

	// Use this for initialization
	private void Start()
	{
		anim = Camera.main.GetComponent<Animator>();
	}
}