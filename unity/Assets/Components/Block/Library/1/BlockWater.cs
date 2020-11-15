﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWater : Block
{
	private const int _ParticlesCount = 60;	
	private const int _Strength = 1;
	private const int _Score = 1;

	public static void OnGrab(int count)
	{
		Racket.Get().Strength += _Strength;
		LevelManager.Get().AddScore(_Score * count);
	}
	
	protected override void OnTouch()
	{
		for (int i = 0; i < _ParticlesCount; ++i)
		{
			ParticleSystem.EmitParams particle = new ParticleSystem.EmitParams();
			particle.position = transform.position + new Vector3((Random.value - 0.5f) * transform.parent.localScale.x * 2.0f, (Random.value - 0.5f) * transform.parent.localScale.y * 2.0f, -Settings.RacketHeight);
			particle.startSize = 0.5f;
			
			_type.Particles.Emit(particle, 10);
		}
	}
}
