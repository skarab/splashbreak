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

	private bool _running = false;

	private static BlockManager _Instance = null;

	public static BlockManager Get()
	{
		return _Instance;
	}

	public static bool IsInstantiated()
	{
		return _Instance != null;
	}

	public bool CreateBlock(int id, int x, int y)
	{
		float blockWidth = (Settings.WorldWidth - Settings.Space * (Settings.Width - 1.0f)) / Settings.Width;
		float blockHeight = (Settings.WorldHeight - Settings.Space * (Settings.Height - 1.0f)) / Settings.Height;

		GameObject block = Object.Instantiate<GameObject>(Library[id].Prefab, Blocks);
		block.transform.position = new Vector3(x * (blockWidth + Settings.Space) + blockWidth / 2.0f, y * (blockHeight + Settings.Space) + blockHeight / 2.0f, Settings.Depth / 2.0f);
		block.transform.localScale = new Vector3(blockWidth, blockHeight, Settings.Depth);

		block.GetComponent<Block>().Create(Library[id]);
		_running = true;

		return block.GetComponent<Block>().IsDestroyable();
	}

	public void Clear()
	{
		while (Blocks.transform.childCount > 0)
			DestroyImmediate(Blocks.transform.GetChild(0).gameObject);

		for (int i = 0; i < Library.Length; ++i)
		{
			Library[i].Particles.Clear();
		}	

		_running = false;
	}

	void Awake()
	{
		_Instance = this;

		// TODO find a way to make this out.
		Library[0].OnGrab = new BlockTypeOnGrab(BlockDum.OnGrab);
		Library[1].OnGrab = new BlockTypeOnGrab(BlockWater.OnGrab);
		Library[2].OnGrab = new BlockTypeOnGrab(BlockBlood.OnGrab);
		Library[3].OnGrab = new BlockTypeOnGrab(BlockGold.OnGrab);
		Library[4].OnGrab = new BlockTypeOnGrab(BlockGlass.OnGrab);
	}

	void Update()
	{
		if (_running)
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
}
