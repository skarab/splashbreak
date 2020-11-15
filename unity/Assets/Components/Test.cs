using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	
	void Start()
	{
		Level level = new Level();
		level.Randomize();

		LevelManager.Get().LoadLevel(level);
	}

	void Update()
	{		
	}

}
