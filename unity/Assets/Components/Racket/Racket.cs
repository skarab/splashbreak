using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Racket : MonoBehaviour
{
	public float Force = 40.0f;
	public float Feedback = 1.0f;
	public float PositionSpeedMultiplier = 0.3f;
	[Range(0, 100)] public int Strength = 50;
	public Transform Cylinder;
	public Transform CapsuleLeft;
	public Transform CapsuleRight;

	private float _position = 0.0f;
	private float _width = 0.0f;
	private Rigidbody _rigidBody = null;

	void Start()
	{
		_rigidBody = GetComponent<Rigidbody>();

		transform.position = new Vector2(Settings.WorldWidth / 2.0f, Settings.RacketOffset);
		UpdateSize();
	}

	void Update()
	{
		float movement = (transform.position.x - _position) / Time.deltaTime;

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			float force = -Force;
			if (movement > 0.0f)
				force -= movement * Feedback;
			_rigidBody.AddForce(new Vector3(force, 0.0f, 0.0f), ForceMode.Force);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			float force = Force;
			if (movement < 0.0f)
				force -= movement * Feedback;
			_rigidBody.AddForce(new Vector3(force, 0.0f, 0.0f), ForceMode.Force);
		}

		_position = transform.position.x;
	}

	public void OnCollisionEnter(Collision collision)
	{
		Ball ball = collision.gameObject.GetComponent<Ball>();
		if (ball != null && collision.contactCount > 0)
		{
			float x = collision.contacts[0].point.x;
			float distance = Mathf.Abs(x - transform.position.x);
			distance = Mathf.Min(distance, _width);
			float speed = Mathf.Pow(distance / _width, 4.0f) * PositionSpeedMultiplier;
			collision.rigidbody.velocity *= 1.0f + speed;
		}
	}

	private void UpdateSize()
	{
		_width = Mathf.Lerp(Settings.RacketWidthMinimum, Settings.RacketWidthMaximum, Strength / 100.0f) / 2.0f;
		Cylinder.localScale = new Vector3(Settings.RacketHeight, _width, Settings.RacketHeight);
		CapsuleLeft.localScale = new Vector3(Settings.RacketHeight, Settings.RacketHeight * 2.0f, Settings.RacketHeight);
		CapsuleLeft.localPosition = new Vector3(-Cylinder.localScale.y, 0.0f, 0.0f);
		CapsuleRight.localScale = new Vector3(Settings.RacketHeight, Settings.RacketHeight * 2.0f, Settings.RacketHeight);
		CapsuleRight.localPosition = new Vector3(Cylinder.localScale.y, 0.0f, 0.0f);
	}
}
