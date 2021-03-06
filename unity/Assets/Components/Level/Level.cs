﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
	public string Name;
	public int EnvironmentID = 0;
	public int[,] Grid = new int[Settings.Width, Settings.Height];

	public Level()
	{
		for (int y = 0; y < Settings.Height; ++y)
		{
			for (int x = 0; x < Settings.Width; ++x)
			{
				Grid[x, y] = 0;
			}
		}
	}

	public Level(Level other)
	{
		Name = other.Name;
		EnvironmentID = other.EnvironmentID;
		
		for (int y=0 ; y< Settings.Height ; ++y)
		{
			for (int x = 0; x < Settings.Width; ++x)
			{
				Grid[x, y] = other.Grid[x, y];
			}
		}
	}

	public void Clear()
	{
		for (int y = 0; y < Settings.Height; ++y)
		{
			for (int x = 0; x < Settings.Width; ++x)
			{
				Grid[x, y] = 0;
			}
		}
	}

	public void Randomize()
	{
		for (int y = 0; y < Settings.Height; ++y)
		{
			for (int x = 0; x < Settings.Width; ++x)
			{
				Grid[x, y] = Mathf.RoundToInt(Random.value * (BlockManager.Get().Library.Length-1)) + 1;
			}
		}
	}

}
