using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleSpawner : MonoBehaviour {
    public GameObject icicle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter (Collider other)
    {
        if(other.tag == "Bullet" || other.tag == "Player")
        {
        Invoke("DoThatThingYaWantToDo",5.0f);
        }
    }

    private void DoThatThingYaWantToDo()
    {
        Instantiate(icicle, transform);
    }
}
