﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappearingPlatform : MonoBehaviour {

    public int time;
    BoxCollider platformCollider;
    BoxCollider childCollider;
    MeshRenderer platform;
    private Transform childTransform;



    // Use this for initialization
    void Start ()
    {
        platformCollider = gameObject.AddComponent<BoxCollider>();
        platform = GetComponentInChildren<MeshRenderer>();
        childCollider = GetComponentInChildren<BoxCollider>();

        platformCollider.enabled = false;
        platform.enabled = false;
        childCollider.enabled = false;

        childTransform = transform.GetChild(0);
        platformCollider.size = new Vector3(childTransform.transform.localScale.x, childTransform.transform.localScale.y, childTransform.transform.localScale.z);

    }
	
	// Update is called once per frame
	void Update ()
    {

		if (time > 800 && time < 1500)
        {
            platformCollider.enabled = true;
            platform.enabled = true;
           
        }
        
        if (time > 2000)
        {
            platformCollider.enabled = false;
            platform.enabled = false;
            time = 0;
        }

        time++;
	}
}
