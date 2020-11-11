using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockType
{
    public GameObject Prefab;
    public ParticleSystem Particles;
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

    public void CreateBlock(int id, int x, int y)
	{
        float blockWidth = (Settings.WorldWidth - Settings.Space * (Settings.Width - 1.0f)) / Settings.Width;
        float blockHeight = (Settings.WorldHeight - Settings.Space * (Settings.Height - 1.0f)) / Settings.Height;

        GameObject block = Object.Instantiate<GameObject>(Library[id].Prefab, Blocks);
        block.transform.position = new Vector2(x * (blockWidth + Settings.Space), y * (blockHeight + Settings.Space));
        block.transform.localScale = new Vector3(blockWidth, blockHeight, Settings.Depth);
    }

    public void Clear()
	{
        while (Blocks.transform.childCount > 0)
            DestroyImmediate(Blocks.transform.GetChild(0).gameObject);
    }

    void Awake()
    {
        _Instance = this;
    }
        
    void Update()
    {
        /*
     
		List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();
		Particles.GetCollisionEvents(_racket, collisions);
		if (collisions.Count>0)
		{
			int i=0;
			++i;
		}*/
    }
}
