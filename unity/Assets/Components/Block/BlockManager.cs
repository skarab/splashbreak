using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void BlockTypeOnGrab(int count);

[System.Serializable]
public struct BlockType
{
	public GameObject Prefab;
	public ParticleSystem Particles;
	public BlockTypeOnGrab OnGrab;
}

public class BlockManager : MonoBehaviour
{
	public BlockType[] Library;
	public Transform Blocks;

	private static BlockManager _Instance = null;

	public static BlockManager Get()
	{
		return _Instance;
	}

	public static bool IsInstantiated()
	{
		return _Instance != null;
	}

	public void CreateBlock(int id, int x, int y)
	{
		float blockWidth = (Settings.WorldWidth - Settings.Space * (Settings.Width - 1.0f)) / Settings.Width;
		float blockHeight = (Settings.WorldHeight - Settings.Space * (Settings.Height - 1.0f)) / Settings.Height;

		GameObject block = Object.Instantiate<GameObject>(Library[id].Prefab, Blocks);
		block.transform.position = new Vector3(x * (blockWidth + Settings.Space) + blockWidth / 2.0f, y * (blockHeight + Settings.Space) + blockHeight / 2.0f, Settings.Depth / 2.0f);
		block.transform.localScale = new Vector3(blockWidth, blockHeight, Settings.Depth);

		block.GetComponent<Block>().Create(Library[id]);
	}

	public void Clear()
	{
		while (Blocks.transform.childCount > 0)
			DestroyImmediate(Blocks.transform.GetChild(0).gameObject);
	}

	void Awake()
	{
		_Instance = this;

		// TODO find a way to make this out.
		Library[0].OnGrab = new BlockTypeOnGrab(BlockDum.OnGrab);
		Library[1].OnGrab = new BlockTypeOnGrab(BlockGood.OnGrab);
		Library[2].OnGrab = new BlockTypeOnGrab(BlockBad.OnGrab);
	}

	void Update()
	{
		for (int i=0 ; i<Library.Length ; ++i)
		{ 
			List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();
			Library[i].Particles.GetCollisionEvents(Racket.Get().Capsule.gameObject, collisions);
			if (collisions.Count>0)
			{
				Library[i].OnGrab(collisions.Count);
			}
		}
	}
}
