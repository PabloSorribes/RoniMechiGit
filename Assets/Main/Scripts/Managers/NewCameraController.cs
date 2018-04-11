﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class NewCameraController : MonoBehaviour
{
	public List<Transform> targets;

	public Vector3 offset;
	public float smoothTime = .5f;
	public float minZoom = 100f;
	public float maxZoom = 60f;
	public float zoomLimiter = 50f;
	private Vector3 velocity;
	private Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		if (targets.Count == 0)
			return;

		Move();
		Zoom();
	}

	private void Move()
	{
		Vector3 centerPoint = GetCenterPoint();

		Vector3 newPosition = centerPoint + offset;

		transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
	}

	private Vector3 GetCenterPoint()
	{
		if (targets.Count == 1)
			return targets[0].position;

		var bounds = new Bounds(targets[0].position, Vector3.zero);
		for (int i = 0; i < targets.Count; i++)
		{
			bounds.Encapsulate(targets[i].position);
		}

		return bounds.center;
	}

	private void Zoom()
	{
		float newZoom = Mathf.Lerp(minZoom, minZoom, GetGreatestDistance() / zoomLimiter);
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
	}

	private float GetGreatestDistance()
	{
		var bounds = new Bounds(targets[0].position, Vector3.zero);

		for (int i = 0; i < targets.Count; i++)
		{
			bounds.Encapsulate(targets[i].position);
		}

		return bounds.size.x;
	}
}