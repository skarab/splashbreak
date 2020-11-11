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

	private GameObject _root = null;
	private GameObject _walls = null;
	private GameObject _balls = null;
	private GameObject _racket = null;
	private GameObject _ball = null;

	void Start()
	{
		LoadLevel();
	}

	void Update()
	{
		// TODO: out of screen ball maybe
		if (_ball != null && _ball.transform.position.y < _racket.transform.position.y - 40.0f)
		{
			Destroy(_ball);
			_ball = null;
		}

		if (_ball == null)
		{
			_ball = Object.Instantiate<GameObject>(Ball, _balls.transform);
			_ball.GetComponent<Ball>().Attach(_racket.transform, (Settings.RacketHeight + _ball.GetComponent<Renderer>().bounds.size.y) / 2.0f);
		}
	}

	private void LoadLevel()
	{
		_root = new GameObject("root");
		_root.transform.position = Vector3.zero;
		_root.transform.rotation = Quaternion.identity;

		// Create blocks.

		for (int y = 0; y < Settings.Height; ++y)
		{
			for (int x = 0; x < Settings.Width; ++x)
			{
				BlockManager.Get().CreateBlock(0, x, y);
			}
		}

		// Create walls.

		_walls = new GameObject("walls");
		_walls.transform.parent = _root.transform;
		_walls.transform.position = Vector3.zero;
		_walls.transform.rotation = Quaternion.identity;

		GameObject wallTop = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallTop.transform.position = new Vector2(Settings.WorldWidth / 2.0f, Settings.WorldHeight + Settings.Space);
		wallTop.transform.localScale = new Vector3(Settings.WorldWidth + Settings.Space * 2.0f, 1.0f, Settings.Depth);

		GameObject wallLeft = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallLeft.transform.position = new Vector2(-Settings.Space, Settings.WorldHeight / 2.0f + Settings.RacketOffset / 2.0f);
		wallLeft.transform.localScale = new Vector3(1.0f, Settings.WorldHeight + Settings.Space * 2.0f - Settings.RacketOffset, Settings.Depth);

		GameObject wallRight = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallRight.transform.position = new Vector2(Settings.WorldWidth + Settings.Space, Settings.WorldHeight / 2.0f + Settings.RacketOffset / 2.0f);
		wallRight.transform.localScale = new Vector3(1.0f, Settings.WorldHeight + Settings.Space * 2.0f - Settings.RacketOffset, Settings.Depth);

		// Create balls.

		_balls = new GameObject("balls");
		_balls.transform.parent = _root.transform;
		_balls.transform.position = Vector3.zero;
		_balls.transform.rotation = Quaternion.identity;

		// Create racket.

		_racket = Object.Instantiate<GameObject>(Racket, _root.transform);

		// Update camera.

		float hfov = Camera.VerticalToHorizontalFieldOfView(Cam.fieldOfView, Screen.width / (float)Screen.height);
		float hdistance = (Settings.WorldWidth / 2.0f + Settings.Space * 6.0f) / Mathf.Tan(Mathf.Deg2Rad * hfov / 2.0f);
		float vdistance = ((Settings.WorldHeight - Settings.RacketOffset) / 2.0f + Settings.Space * 4.0f) / Mathf.Tan(Mathf.Deg2Rad * Cam.fieldOfView / 2.0f);
		Cam.transform.position = new Vector3(Settings.WorldWidth / 2.0f, Settings.WorldHeight / 2.0f + Settings.RacketOffset / 2.0f, -Mathf.Max(hdistance, vdistance));
		Cam.transform.rotation = Quaternion.identity;
	}

	private void DestroyLevel()
	{
		while (_walls.transform.childCount > 0)
			DestroyImmediate(_walls.transform.GetChild(0).gameObject);
		DestroyImmediate(_walls);

		while (_balls.transform.childCount > 0)
			DestroyImmediate(_balls.transform.GetChild(0).gameObject);
		DestroyImmediate(_balls);

		DestroyImmediate(_racket);
		_racket = null;
	}
}
