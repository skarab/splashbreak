using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
	private const int _ballGold = 500;

	public GameObject Wall;
	public GameObject Racket;
	public GameObject Ball;
	public GameObject UI;
	public TextMeshProUGUI ScoreUI;
	public TextMeshProUGUI InfosUI;

	private bool _loaded = false;
	private Level _level = null;
	private GameObject _root = null;
	private GameObject _walls = null;
	private GameObject _balls = null;
	private GameObject _racket = null;
	private GameObject _ball = null;
	private int _score = 0;
	private int _gold = 0;
	private float _time = 0.0f;
	private int _blockCount = 0;

	private static LevelManager _Instance = null;

	public static LevelManager Get()
	{
		return _Instance;
	}

	public bool IsLoaded() { return _loaded; }
	public GameObject GetBall() { return _ball; }
	public GameObject GetRacket() { return _racket; }

	public void AddScore(int score)
	{
		_score += score;
	}

	public void AddGold(int gold)
	{
		_gold += gold;
	}

	public void LoadLevel(Level level)
	{
		LoadEnvironmentAndBlocks(level, false);

		// Create walls.

		_walls = new GameObject("walls");
		_walls.transform.parent = _root.transform;
		_walls.transform.position = Vector3.zero;
		_walls.transform.rotation = Quaternion.identity;

		GameObject wallTop = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallTop.transform.position = new Vector2(Settings.WorldWidth / 2.0f, Settings.WorldHeight + Settings.Space);
		wallTop.transform.localScale = new Vector3(Settings.WorldWidth + Settings.Space * 2.0f, Settings.Space, Settings.Depth);

		GameObject wallLeft = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallLeft.transform.position = new Vector2(-Settings.Space, Settings.WorldHeight / 2.0f + Settings.RacketOffset / 2.0f - Settings.WallHeight / 2.0f);
		wallLeft.transform.localScale = new Vector3(Settings.Space, Settings.WorldHeight + Settings.Space * 2.0f - Settings.RacketOffset + Settings.WallHeight, Settings.Depth);

		GameObject wallRight = Object.Instantiate<GameObject>(Wall, _walls.transform);
		wallRight.transform.position = new Vector2(Settings.WorldWidth + Settings.Space, Settings.WorldHeight / 2.0f + Settings.RacketOffset / 2.0f - Settings.WallHeight / 2.0f);
		wallRight.transform.localScale = new Vector3(Settings.Space, Settings.WorldHeight + Settings.Space * 2.0f - Settings.RacketOffset + Settings.WallHeight, Settings.Depth);

		// Create balls.

		_balls = new GameObject("balls");
		_balls.transform.parent = _root.transform;
		_balls.transform.position = Vector3.zero;
		_balls.transform.rotation = Quaternion.identity;

		// Create racket.

		_racket = Object.Instantiate<GameObject>(Racket, _root.transform);

		// Parameters

		UI.SetActive(true);
		_score = 0;
		_gold = 0;
		_time = 0.0f;
		_loaded = true;
	}

	public void LoadEnvironmentAndBlocks(Level level, bool bypassEffects)
	{
		if (_level != null)
		{
			UnloadLevel();
		}

		_level = new Level(level);
		EnvironmentManager.Get().LoadEnvironment(_level.EnvironmentID, bypassEffects);

		_root = new GameObject("root");
		_root.transform.position = Vector3.zero;
		_root.transform.rotation = Quaternion.identity;

		// Create blocks.

		_blockCount = 0;
		for (int y = 0; y < Settings.Height; ++y)
		{
			for (int x = 0; x < Settings.Width; ++x)
			{
				if (_level.Grid[x, y] != 0 && BlockManager.Get().CreateBlock(_level.Grid[x, y] - 1, x, y))
				{
					++_blockCount;
				}
			}
		}
	}

	public void UnloadLevel()
	{
		EnvironmentManager.Get().UnloadEnvironment(_level.EnvironmentID);
		BlockManager.Get().Clear();
		UI.SetActive(false);

		if (_walls != null)
		{
			while (_walls.transform.childCount > 0)
				DestroyImmediate(_walls.transform.GetChild(0).gameObject);
			DestroyImmediate(_walls);
			_walls = null;
		}

		if (_balls != null)
		{
			while (_balls.transform.childCount > 0)
				DestroyImmediate(_balls.transform.GetChild(0).gameObject);
			DestroyImmediate(_balls);
			_balls = null;
		}

		if (_racket != null)
		{
			DestroyImmediate(_racket);
			_racket = null;
		}

		DestroyImmediate(_root);
		_root = null;
		_level = null;
		_loaded = false;
	}

	public void OnHitBlock()
	{
		--_blockCount;
		if (_blockCount == 0)
		{
			Editor.Get().NotifyEndLevel(true);
		}
	}

	void Awake()
	{
		_Instance = this;
	}

	// Update is called once per frame
	void Update()
	{
		if (_loaded)
		{
			UpdateLevel();
		}
	}

	private void UpdateLevel()
	{
		_time += Time.deltaTime;

		// TODO: out of screen ball maybe
		if (_ball != null && _ball.transform.position.y < _racket.transform.position.y - 4.0f)
		{
			Destroy(_ball);
			_ball = null;

			if (_gold / _ballGold == 0)
			{
				Editor.Get().NotifyEndLevel(false);
				return;
			}

			_gold -= _ballGold;
		}

		if (_ball == null)
		{
			_ball = Object.Instantiate<GameObject>(Ball, _balls.transform);
			_ball.GetComponent<Ball>().Attach(_racket.transform, (Settings.RacketHeight + _ball.GetComponent<Renderer>().bounds.size.y) / 2.0f);
		}

		ScoreUI.text = _score.ToString();
		InfosUI.text = System.TimeSpan.FromSeconds(_time).ToString("mm':'ss") + "\n+" + _gold.ToString() + "\n" + (_gold / _ballGold).ToString() + " BALLS";
	}
}
