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

	public virtual bool IsDestroyable()
	{
		return true;
	}

	protected virtual void OnTouch()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Ball>()!=null)
		{
			OnTouch();
		
			if (IsDestroyable())
			{
				Destroy(transform.gameObject);
			}
		}
	}
}
