using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float TranslationSpeed = 1.0f;
	public float RotationSpeed = 1.0f;
	public float RotationMax = 20.0f;
	public float EditModeOffset = -1.0f;

	private Camera _camera;

	void Awake()
	{
		_camera = GetComponent<Camera>();

		transform.position = ComputeAAPosition();
		transform.rotation = Quaternion.identity;
	}

	void Update()
	{
		Vector3 position = ComputeAAPosition();
		Quaternion rotation = Quaternion.identity;

		if (LevelManager.Get().IsLoaded())
		{
			Vector3 target = (LevelManager.Get().GetRacket().transform.position + LevelManager.Get().GetBall().transform.position) / 2.0f;
			float x = (target.x / Settings.WorldWidth - 0.5f) * 2.0f;
			rotation = Quaternion.Euler(0.0f, -x * RotationMax, 0.0f);
			position = target + rotation * (position-target) + new Vector3(x * Settings.WorldWidth * RotationMax / 300.0f, 0.0f, 0.0f);
		}

		if (Editor.Get().IsEditing())
		{
			position += new Vector3(0.0f, 0.0f, EditModeOffset);
		}

		transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * TranslationSpeed);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);
	}

	private Vector3 ComputeAAPosition()
	{
		float hfov = Camera.VerticalToHorizontalFieldOfView(_camera.fieldOfView, Screen.width / (float)Screen.height);
		float hdistance = (Settings.WorldWidth / 2.0f + Settings.Space * 6.0f) / Mathf.Tan(Mathf.Deg2Rad * hfov / 2.0f);
		float vdistance = ((Settings.WorldHeight - Settings.RacketOffset) / 2.0f + Settings.Space * 4.0f) / Mathf.Tan(Mathf.Deg2Rad * _camera.fieldOfView / 2.0f);
		return new Vector3(Settings.WorldWidth / 2.0f, Settings.WorldHeight / 2.0f + Settings.RacketOffset / 2.0f, -Mathf.Max(hdistance, vdistance));
	}
}
