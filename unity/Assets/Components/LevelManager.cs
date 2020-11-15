using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Racket;
    public GameObject Ball;

    private bool _loaded = false;
    private GameObject _root = null;
    private GameObject _walls = null;
    private GameObject _balls = null;
    private GameObject _racket = null;
    private GameObject _ball = null;
    
    private static LevelManager _Instance = null;

    public static LevelManager Get()
	{
        return _Instance;
	}

	public bool IsLoaded() { return _loaded; }
	public GameObject GetBall() { return _ball; }
	public GameObject GetRacket() { return _racket; }

	public void LoadLevel()
	{
		EnvironmentManager.Get().LoadEnvironment(0);

		_root = new GameObject("root");
		_root.transform.position = Vector3.zero;
		_root.transform.rotation = Quaternion.identity;

		// Create blocks.

		for (int y = 0; y < Settings.Height; ++y)
		{
			for (int x = 0; x < Settings.Width; ++x)
			{
				BlockManager.Get().CreateBlock((int)(Random.value * BlockManager.Get().Library.Length), x, y);
			}
		}

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

		_loaded = true;
	}

    public void UnloadLevel()
	{
		EnvironmentManager.Get().UnloadEnvironment(0);

		while (_walls.transform.childCount > 0)
			DestroyImmediate(_walls.transform.GetChild(0).gameObject);
		DestroyImmediate(_walls);

		while (_balls.transform.childCount > 0)
			DestroyImmediate(_balls.transform.GetChild(0).gameObject);
		DestroyImmediate(_balls);

		DestroyImmediate(_racket);
		_racket = null;

		_loaded = false;
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
        // TODO: out of screen ball maybe
        if (_ball != null && _ball.transform.position.y < _racket.transform.position.y - 4.0f)
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
}
