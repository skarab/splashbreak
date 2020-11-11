using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Racket : MonoBehaviour
{
	public float Force = 40.0f;
	public float Feedback = 1.0f;
	public float PositionSpeedMultiplier = 0.3f;

	private float _position = 0.0f;
	private Rigidbody _rigidBody = null;

	void Start()
	{
		_rigidBody = GetComponent<Rigidbody>();
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

	private void OnCollisionEnter(Collision collision)
	{
		Ball ball = collision.gameObject.GetComponent<Ball>();
		if (ball != null && collision.contactCount > 0)
		{
			float x = collision.contacts[0].point.x;
			float distance = Mathf.Abs(x - transform.position.x);
			float length = transform.localScale.x;
			distance = Mathf.Min(distance, length);
			float speed = Mathf.Pow(distance / length, 4.0f) * PositionSpeedMultiplier;
			collision.rigidbody.velocity *= 1.0f + speed;
		}
	}
}
