using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public GameObject Block;
	public GameObject Wall;
	public GameObject Racket;
	public GameObject Ball;
	public Camera Cam;
	
	private const float _width = 320.0f;
	private const float _height = 80.0f;
	private const float _border = 1.0f;
	private const int _gridWidth = 16;
	private const int _gridHeight = 8;
	private const float _barOffset = -40.0f;

	private GameObject _root = null;
	private GameObject _blocks = null;
	private GameObject _walls = null;
	private GameObject _balls = null;
	private GameObject _racket = null;
	private GameObject _ball = null;
	private float _blockWidth = 0.0f;
	private float _blockHeight = 0.0f;

	void Start()
	{
		LoadLevel();
	}

	void Update()
	{		
		if (_ball != null && _ball.transform.position.y < _racket.transform.position.y - _blockHeight * 5.0f)
		{
			Destroy(_ball);
			_ball = null;
		}

		if (_ball == null)
		{
			_ball = Object.Instantiate<GameObject>(Ball, _balls.transform);
			_ball.GetComponent<Ball>().Attach(_racket.transform, (_blockHeight + _ball.GetComponent<Renderer>().bounds.size.y) / 2.0f);
		}
	}

	private void LoadLevel()
	{
		_root = new GameObject("root");
		_root.transform.position = Vector3.zero;
		_root.transform.rotation = Quaternion.identity;

		// Create blocks.

		_blocks = new GameObject("blocks");
		_blocks.transform.parent = _root.transform;
		_blocks.transform.position = Vector3.zero;
		_blocks.transform.rotation = Quaternion.identity;
		
		_blockWidth = (_width - _border * (_gridWidth - 1.0f)) / _gridWidth;
		_blockHeight = (_height - _border * (_gridHeight - 1.0f)) / _gridHeight;

		for (int j = 0; j < _gridHeight; ++j)
		{
			for (int i = 0; i < _gridWidth; ++i)
			{
				GameObject block = Object.Instantiate<GameObject>(Block, _blocks.transform);
				block.transform.position = new Vector2(i * (_blockWidth + _border), j * (_blockHeight + _border));
				block.transform.localScale = new Vector3(_blockWidth, _blockHeight, _blockHeight);
			}
		}

		// Create walls.

		_walls = new GameObject("walls");
		_walls.transform.parent = _root.transform;
		_walls.transform.position = Vector3.zero;
		_walls.transform.rotation = Quaternion.identity;

		GameObject wallTop = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallTop.transform.position = new Vector2(_width / 2.0f, _height + _border);
		wallTop.transform.localScale = new Vector3(_width + _border * 2.0f, 1.0f);

		GameObject wallLeft = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallLeft.transform.position = new Vector2(-_border, _height / 2.0f + _barOffset / 2.0f);
		wallLeft.transform.localScale = new Vector3(1.0f, _height + _border * 2.0f - _barOffset);

		GameObject wallRight = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallRight.transform.position = new Vector2(_width + _border, _height / 2.0f + _barOffset / 2.0f);
		wallRight.transform.localScale = new Vector3(1.0f, _height + _border * 2.0f - _barOffset);

		// Create balls.

		_balls = new GameObject("balls");
		_balls.transform.parent = _root.transform;
		_balls.transform.position = Vector3.zero;
		_balls.transform.rotation = Quaternion.identity;

		// Create racket.

		_racket = Object.Instantiate<GameObject>(Racket, _root.transform);
		_racket.transform.position = new Vector2(_width / 2.0f, _barOffset);
		_racket.transform.localScale = new Vector3(_blockHeight, _blockWidth, _blockHeight);
		
		// Update camera.
		
		float hfov = Camera.VerticalToHorizontalFieldOfView(Cam.fieldOfView, Screen.width / (float)Screen.height);
		float hdistance = (_width / 2.0f + _border * 6.0f) / Mathf.Tan(Mathf.Deg2Rad * hfov / 2.0f);
		float vdistance = ((_height - _barOffset) / 2.0f + _border * 4.0f) / Mathf.Tan(Mathf.Deg2Rad * Cam.fieldOfView / 2.0f);
		Cam.transform.position = new Vector3(_width / 2.0f, _height / 2.0f + _barOffset / 2.0f, -Mathf.Max(hdistance, vdistance));
		Cam.transform.rotation = Quaternion.identity;
	}

	private void DestroyLevel()
	{
		while (_blocks.transform.childCount > 0)
			DestroyImmediate(_blocks.transform.GetChild(0).gameObject);
		DestroyImmediate(_blocks);
		
		while (_walls.transform.childCount > 0)
			DestroyImmediate(_walls.transform.GetChild(0).gameObject);
		DestroyImmediate(_walls);

		while (_balls.transform.childCount>0)
			DestroyImmediate(_balls.transform.GetChild(0).gameObject);
		DestroyImmediate(_balls);

		DestroyImmediate(_racket);
		_racket = null;
	}
}
