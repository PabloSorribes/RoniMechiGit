using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TotallyNotAnEasterEgg : MonoBehaviour
{
    public MenuManager menuManager;
    // Use this for initialization
    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {

            SceneManager.LoadScene("Scene_Elton");
        }
    }
}