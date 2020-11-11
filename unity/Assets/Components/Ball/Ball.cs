using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public float MinimumBallSpeed = 60.0f;
	public float MaximumBallSpeed = 200.0f;
	public float MinimumBallAngle = 30.0f;

	private Rigidbody _rigidBody = null;
	private Transform _attachment = null;
	private float _attachmentOffset = 0.0f;

	public void Attach(Transform attachment, float offset)
	{
		_attachment = attachment;
		_attachmentOffset = offset;
		transform.position = _attachment.position + Vector3.up * _attachmentOffset;
	}

	private void Start()
	{
		_rigidBody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (_attachment != null)
		{
			transform.position = _attachment.position + Vector3.up * _attachmentOffset;

			if (Input.GetKey(KeyCode.Space))
			{
				_rigidBody.AddForce(new Vector3(0.0f, 2.0f, 0.0f), ForceMode.Force);
				_attachment = null;
			}
		}
		else
		{
			float velocityMagnitude = _rigidBody.velocity.magnitude;
			if (velocityMagnitude >= 0.00001f)
			{
				// Handle speed.
				if (velocityMagnitude < MinimumBallSpeed)
				{
					_rigidBody.velocity = _rigidBody.velocity * MinimumBallSpeed / velocityMagnitude;
					velocityMagnitude = MinimumBallSpeed;
				}
				else if (velocityMagnitude > MaximumBallSpeed)
				{
					_rigidBody.velocity = _rigidBody.velocity * MaximumBallSpeed / velocityMagnitude;
					velocityMagnitude = MaximumBallSpeed;
				}

				// Handle angle.
				Vector3 normalizedVelocity = _rigidBody.velocity / velocityMagnitude;
				Vector3 axis = normalizedVelocity.x < 0.0f ? Vector3.left : Vector3.right;
				float angle = Vector3.SignedAngle(normalizedVelocity, axis, Vector3.forward);
				if (Mathf.Abs(angle) < MinimumBallAngle)
				{
					normalizedVelocity = Quaternion.Euler(new Vector3(0.0f, 0.0f, -MinimumBallAngle * Mathf.Sign(angle))) * axis;
					_rigidBody.velocity = normalizedVelocity * velocityMagnitude;
				}
			}
		}
	}

}
