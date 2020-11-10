using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public GameObject block;
	public GameObject bar;
	public GameObject wall;
	public Transform root;
	public Transform walls;
	public Camera camera;
	public GameObject ball;

	private const float _width = 320.0f;
	private const float _height = 150.0f;
	private const float _border = 1.0f;
	private const int _gridWidth = 32;
	private const int _gridHeight = 20;
	private const float _barOffset = -40.0f;

	private const float _minimumBallSpeed = 40.0f;
	private const float _maximumBallSpeed = 150.0f;
	private const float _minimumBallAngle = 30.0f;

	private Rigidbody _bar = null;
	private float _barPosition = 0.0f;
	private Rigidbody _ball = null;
	private bool _starting = false;

	void Start()
	{
		float blockWidth = (_width - _border * (_gridWidth - 1.0f)) / _gridWidth;
		float blockHeight = (_height - _border * (_gridHeight - 1.0f)) / _gridHeight;

		for (int j = 0; j < _gridHeight; ++j)
		{
			for (int i = 0; i < _gridWidth; ++i)
			{
				GameObject blockNode = Object.Instantiate<GameObject>(block, root);
				blockNode.transform.position = new Vector2(i * (blockWidth + _border), j * (blockHeight + _border));
				blockNode.transform.localScale = new Vector3(blockWidth, blockHeight, blockHeight);
			}
		}

		GameObject barNode = Object.Instantiate<GameObject>(bar, transform);
		barNode.transform.position = new Vector2(_width / 2.0f, _barOffset);
		barNode.transform.localScale = new Vector3(blockHeight, blockWidth * 2.0f, blockHeight);
		_bar = barNode.GetComponent<Rigidbody>();

		GameObject wallTop = Object.Instantiate<GameObject>(wall, walls);
		wallTop.transform.position = new Vector2(_width / 2.0f, _height + _border);
		wallTop.transform.localScale = new Vector3(_width + _border * 2.0f, 1.0f);

		GameObject wallLeft = Object.Instantiate<GameObject>(wall, walls);
		wallLeft.transform.position = new Vector2(-_border, _height / 2.0f + _barOffset / 2.0f);
		wallLeft.transform.localScale = new Vector3(1.0f, _height + _border * 2.0f - _barOffset);

		GameObject wallRight = Object.Instantiate<GameObject>(wall, walls);
		wallRight.transform.position = new Vector2(_width + _border, _height / 2.0f + _barOffset / 2.0f);
		wallRight.transform.localScale = new Vector3(1.0f, _height + _border * 2.0f - _barOffset);

		float hfov = Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, Screen.width / (float)Screen.height);
		float hdistance = (_width / 2.0f + _border * 6.0f) / Mathf.Tan(Mathf.Deg2Rad * hfov / 2.0f);
		float vdistance = ((_height - _barOffset) / 2.0f + _border * 4.0f) / Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2.0f);
		camera.transform.position = new Vector3(_width / 2.0f, _height / 2.0f + _barOffset / 2.0f, -Mathf.Max(hdistance, vdistance));
		camera.transform.rotation = Quaternion.identity;
	}

	void Update()
	{
		float movement = (_bar.transform.position.x - _barPosition) / Time.deltaTime;

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			float force = -40.0f;
			if (movement > 0.0f)
				force -= movement * 1.0f;
			_bar.AddForce(new Vector3(force, 0.0f, 0.0f), ForceMode.Force);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			float force = 40.0f;
			if (movement < 0.0f)
				force -= movement * 1.0f;
			_bar.AddForce(new Vector3(force, 0.0f, 0.0f), ForceMode.Force);
		}

		_barPosition = _bar.transform.position.x;

		if (_ball != null)
		{
			if (_starting)
			{
				_ball.transform.position = _bar.transform.position + Vector3.up * 4.0f;

				if (Input.GetKey(KeyCode.Space))
				{
					_ball.AddForce(new Vector3(0.0f, 2.0f, 0.0f), ForceMode.Force);
					_starting = false;
				}
			}

			float velocityMagnitude = _ball.velocity.magnitude;
			if (velocityMagnitude >= 0.00001f)
			{
				if (velocityMagnitude < _minimumBallSpeed)
				{
					_ball.velocity = _ball.velocity * _minimumBallSpeed / velocityMagnitude;
					velocityMagnitude = _minimumBallSpeed;
				}
				else if (velocityMagnitude > _maximumBallSpeed)
				{
					_ball.velocity = _ball.velocity * _maximumBallSpeed / velocityMagnitude;
					velocityMagnitude = _maximumBallSpeed;
				}

				// I dont want the ball bouncing to much horizontally.
				Vector3 normalizedVelocity = _ball.velocity / velocityMagnitude;
				Vector3 axis = normalizedVelocity.x < 0.0f ? Vector3.left : Vector3.right;
				float angle = Vector3.SignedAngle(normalizedVelocity, axis, Vector3.forward);
				if (Mathf.Abs(angle) < _minimumBallAngle)
				{
					normalizedVelocity = Quaternion.Euler(new Vector3(0.0f, 0.0f, _minimumBallAngle * (-Mathf.Sign(angle)))) * axis;
					_ball.velocity = normalizedVelocity * velocityMagnitude;
				}
			}

			if (_ball.transform.position.y < _bar.transform.position.y - 4.0f)
			{
				Destroy(_ball.gameObject);
				_ball = null;
			}
		}

		if (_ball == null)
		{
			GameObject node = Object.Instantiate<GameObject>(ball);
			_ball = node.GetComponent<Rigidbody>();
			node.transform.position = _bar.transform.position + Vector3.up * 4.0f;
			_starting = true;
		}
	}
}
