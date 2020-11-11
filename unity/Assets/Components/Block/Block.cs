using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	protected BlockType _type;

	public void Create(BlockType type)
	{
		_type = type;
	}

	protected virtual void OnTouch()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		OnTouch();
		Destroy(transform.gameObject);
	}
}
