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

	private const float _Width = 320.0f;
	private const float _Height = 150.0f;
	private const float _Border = 1.0f;
	private const int _GridWidth = 32;
	private const int _GridHeight = 20;
	private const float _BarOffset = -40.0f;
	private const float _WallSpaces = 10.0f;

	void Start()
	{
		float blockWidth = (_Width - _Border * (_GridWidth - 1.0f)) / _GridWidth;
		float blockHeight = (_Height - _Border * (_GridHeight - 1.0f)) / _GridHeight;

		for (int j = 0; j < _GridHeight; ++j)
		{
			for (int i = 0; i < _GridWidth; ++i)
			{
				GameObject blockNode = Object.Instantiate<GameObject>(block, root);
				blockNode.transform.position = new Vector2(i * (blockWidth + _Border), j * (blockHeight + _Border));
				blockNode.transform.localScale = new Vector3(blockWidth, blockHeight, blockHeight);
			}
		}

		GameObject barNode = Object.Instantiate<GameObject>(bar, transform);
		barNode.transform.position = new Vector2(_Width / 2.0f, _BarOffset);
		barNode.transform.localScale = new Vector3(blockHeight, blockWidth * 2.0f, blockHeight);

		GameObject wallTop = Object.Instantiate<GameObject>(wall, walls);
		wallTop.transform.position = new Vector2(_Width / 2.0f, _Height + _WallSpaces);
		wallTop.transform.localScale = new Vector3(_Width + _WallSpaces * 2.0f, 1.0f);

		GameObject wallLeft = Object.Instantiate<GameObject>(wall, walls);
		wallLeft.transform.position = new Vector2(-_WallSpaces, _Height / 2.0f + _BarOffset / 2.0f);
		wallLeft.transform.localScale = new Vector3(1.0f, _Height + _WallSpaces * 2.0f - _BarOffset);

		GameObject wallRight = Object.Instantiate<GameObject>(wall, walls);
		wallRight.transform.position = new Vector2(_Width + _WallSpaces, _Height / 2.0f + _BarOffset / 2.0f);
		wallRight.transform.localScale = new Vector3(1.0f, _Height + _WallSpaces * 2.0f - _BarOffset);
	}

	void Update()
	{

	}
}
