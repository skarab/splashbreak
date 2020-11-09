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
	public Rigidbody ball;

	private const float _width = 320.0f;
	private const float _height = 150.0f;
	private const float _border = 1.0f;
	private const int _gridWidth = 32;
	private const int _gridHeight = 20;
	private const float _barOffset = -40.0f;

	private Rigidbody _bar;

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
		float vdistance = ((_height- _barOffset) / 2.0f + _border * 4.0f) / Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2.0f);
		camera.transform.position = new Vector3(_width / 2.0f, _height / 2.0f + _barOffset / 2.0f, -Mathf.Max(hdistance, vdistance));
		camera.transform.rotation = Quaternion.identity;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			_bar.AddForce(new Vector3(-50.0f, 0.0f, 0.0f), ForceMode.Force);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			_bar.AddForce(new Vector3(50.0f, 0.0f, 0.0f), ForceMode.Force);
		}

		if (Input.GetKey(KeyCode.Space))
		{
			ball.AddForce(new Vector3(0.0f, 1.0f, 0.0f), ForceMode.Force);
		}
	}
}
