﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSlime : Block
{
	public int ParticlesCount = 120;
	
	private const int StrengthDecrease = 1;

	public static void OnGrab(int count)
	{
		Racket.Get().Strength -= StrengthDecrease;
	}

	protected override void OnTouch()
	{
		for (int i = 0; i < ParticlesCount; ++i)
		{
			ParticleSystem.EmitParams particle = new ParticleSystem.EmitParams();
			particle.position = transform.position + new Vector3((Random.value - 0.5f) * transform.parent.localScale.x * 2.0f, (Random.value - 0.5f) * transform.parent.localScale.y * 2.0f, -Settings.RacketHeight);
			particle.startSize = 0.5f;
			
			_type.Particles.Emit(particle, 10);
		}
	}
}